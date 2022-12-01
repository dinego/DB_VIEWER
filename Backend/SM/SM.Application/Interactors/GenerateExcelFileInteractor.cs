using CMC.Common.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class GenerateExcelFileRequest
    {
        public List<ExcelHeader> Header { get; set; }
        public List<List<ExcelBody>> Body { get; set; }
        public string FileName { get; set; } = "Nome do Arquivo";
        public string SheetName { get; set; } = "Nome da Planilha";
        public string TitleSheet { get; set; } = "Titulo na Planilha";
        public string SubTitleSheet { get; set; } = string.Empty;
    }

   
    public class ExcelBody
    {
        public string Value { get; set; }
    }

    public class ExcelHeader
    {
        public ExcelFieldType @Type { get; set; } = ExcelFieldType.Default;
        public string Value { get; set; }
        public string GroupName { get; set; }
        public int? Width { get; set; } = null;
        public bool IsCenter { get; set; } = false;
        public bool IsBold { get; set; } = false;
        public bool IsConditionalFormatting { get; set; } = false;
    }

    public class GroupRange
    {
        public string GroupName { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public bool IsCenter { get; set; }
    }

    public class HeaderDataTable
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public int Position { get; set; }

    }

    public class ExcelHeaderIndexDTO
    {
        public ExcelHeader Value { get; set; }
        public int Index { get; set; }
    }

    public class GenerateExcelFileInteractor : IGenerateExcelFileInteractor
    {
        private readonly ColorScheme _colorScheme;
        private List<HeaderDataTable> _headerDataTableList;

        public GenerateExcelFileInteractor(ColorScheme colorScheme)
        {
            _colorScheme = colorScheme;
            _headerDataTableList = new List<HeaderDataTable>();
        }
        public async Task<byte[]> Handler(GenerateExcelFileRequest request)
        {
            return await Task.Run(() =>
            {
                Color striepColor = ColorTranslator.FromHtml("#f2f2f2");
                var rowStart = 4;
                var rowLoadTable = rowStart + 1;
                var rowLoadTableOutHeader = rowLoadTable + 1;
                var rowEnd = rowLoadTable + request.Body.Count();
                var headerIndex = request.Header
                                    .Select((value, index) =>
                                    new ExcelHeaderIndexDTO
                                    {
                                        Value = value,
                                        Index = index
                                    });
                int headerCount = request.Header.Count();


                var dt = GenerateDataTable(request, headerIndex);

                using var package = new ExcelPackage();

                var worksheet = package.Workbook.Worksheets.Add(request.SheetName);
                worksheet.Cells[rowLoadTable, 1].LoadFromDataTable(dt, true);

                ReplaceZero(rowLoadTableOutHeader, rowEnd, headerIndex, worksheet);

                ReplaceHeaderDataTable(rowLoadTable, worksheet);

                FormatHeader(rowLoadTable, headerIndex, worksheet);

                //AddImage(worksheet, 0, 0);

                FormatStyleGeneral(worksheet, defaultHeight: 15);

                FormatTitle(request, worksheet);

                FormatSubTitle(request, worksheet);

                FormatValuesByColumns(rowLoadTableOutHeader, rowEnd, headerIndex, worksheet);

                ConditionalFormatting(rowLoadTableOutHeader, rowEnd, headerIndex, worksheet, striepColor, headerCount);

                FormatStyleGroupHeader(rowStart, headerIndex, worksheet, rowLoadTable);
                worksheet.View.FreezePanes(6, 1);
                package.Workbook.Properties.Title = request.FileName;
                worksheet.DefaultRowHeight = 24;
                return package.GetAsByteArray();
            });
        }
        public byte[] GenerateCustomHandler(GenerateExcelFileRequest request, bool isAddTotal = true)
        {
            Color striepColor = ColorTranslator.FromHtml("#f2f2f2");
            var rowStart = 4;
            var rowLoadTable = rowStart + 1;
            var rowLoadTableOutHeader = rowLoadTable + 1;
            var rowEnd = rowLoadTable + request.Body.Count();
            var columnEnd = request.Header.Count();
            var headerIndex = request.Header
                                .Select((value, index) =>
                                new ExcelHeaderIndexDTO
                                {
                                    Value = value,
                                    Index = index
                                });
            int headerCount = request.Header.Count();
            var dt = GenerateDataTable(request, headerIndex);

            using var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add(request.SheetName);

            worksheet.Cells[rowLoadTable, 1].LoadFromDataTable(dt, true);

            ReplaceZero(rowLoadTableOutHeader, rowEnd, headerIndex, worksheet);
            
            ReplaceHeaderDataTable(rowLoadTable, worksheet);

            FormatHeader(rowLoadTable, headerIndex, worksheet);

            AddImage(worksheet, 0, 0);

            FormatStyleGeneral(worksheet, defaultHeight: 24);

            FormatTitle(request, worksheet);

            if (isAddTotal)
                AddTotal(request.Body, rowEnd, headerIndex, worksheet);

            CustomConditionalFormatting(rowLoadTableOutHeader,
                rowEnd,
                columnEnd,
                worksheet,
                headerCount,
                striepColor);
            
            FormatValuesByColumns(rowLoadTableOutHeader, rowEnd, headerIndex, worksheet);
            //merge
            FormatStyleGroupHeader(rowLoadTable - 1, headerIndex, worksheet, rowLoadTable);

            return package.GetAsByteArray();
        }
        private static void FormatHeader(int rowLoadTable, IEnumerable<ExcelHeaderIndexDTO> headerIndex, ExcelWorksheet worksheet)
        {
            var rangeHeader = worksheet.Cells[rowLoadTable, 1, rowLoadTable, headerIndex.Count()];

            rangeHeader.Style.Font.Color.SetColor(Color.White);
            rangeHeader.Style.Font.Size = 10;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
            rangeHeader.Style.Fill.BackgroundColor.SetColor(Color.DimGray);
            rangeHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            rangeHeader.Style.Border.Right.Style = ExcelBorderStyle.Medium;
            rangeHeader.Style.Border.Right.Color.SetColor(Color.White);
            rangeHeader.AutoFilter = true;
        }
        private void ReplaceHeaderDataTable(int rowLoadTable, ExcelWorksheet worksheet)
        {
            foreach (var headerReplace in _headerDataTableList)
            {
                worksheet.Cells[rowLoadTable, headerReplace.Position].Value = headerReplace.NewValue;
            }
        }
        private void ReplaceZero(int rowLoadTableOutHeader, int rowEnd, IEnumerable<ExcelHeaderIndexDTO> headerIndex, ExcelWorksheet worksheet)
        {
            for (int i = rowLoadTableOutHeader; i <= rowEnd; i++)
            {

                foreach (var header in headerIndex)
                {
                    var colHeader = header.Index + 1;

                    if (worksheet.Cells[i, colHeader].Value != null &&
                        worksheet.Cells[i, colHeader].Value.ToString().Equals("0"))
                    {
                        worksheet.Cells[i, colHeader].Value = "-";
                    }

                }
            }
        }
        private void AddImage(ExcelWorksheet oSheet, int rowIndex, int colIndex)
        {
            var imagePath = $"{Directory.GetCurrentDirectory()}\\images\\logo-salary-mark.png";
            Bitmap image = new Bitmap(imagePath);
            ExcelPicture excelImage = null;
            if (image != null)
            {
                excelImage = oSheet.Drawings.AddPicture("Logo", image);
                excelImage.From.Column = colIndex;
                excelImage.From.Row = rowIndex;
                excelImage.SetSize(200, 40);

                // 2x2 px space for better alignment
                excelImage.From.ColumnOff = Pixel2MTU(10);
                excelImage.From.RowOff = Pixel2MTU(10);
            }
        }
        private int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }
        private void FormatTitle(GenerateExcelFileRequest request, ExcelWorksheet worksheet)
        {
            //var rangeMerge = worksheet.Cells[3, 1, 3, 2];
            var rangeMerge = worksheet.Cells[3, 1, 3, 8];
            rangeMerge.Merge = true;
            rangeMerge.Value = request.TitleSheet;
            rangeMerge.Style.Indent = 1;
            rangeMerge.Style.Font.Size = 12;
            rangeMerge.Style.Font.Bold = true;

            rangeMerge.Style.Font.Color.SetColor(Color.Black);
            rangeMerge.Style.Fill.PatternType = ExcelFillStyle.Solid;
            rangeMerge.Style.Fill.BackgroundColor.SetColor(Color.White);

            rangeMerge.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
        private void FormatSubTitle(GenerateExcelFileRequest request, ExcelWorksheet worksheet)
        {
            var rangeMerge = worksheet.Cells[4, 1, 4, 2];
            rangeMerge.Merge = true;
            rangeMerge.Value = request.SubTitleSheet;
            rangeMerge.Style.Indent = 1;
            rangeMerge.Style.Font.Size = 12;
            rangeMerge.Style.Font.Bold = true;

            //rangeMerge.Style.Font.Color.SetColor(Color.Black);
            //rangeMerge.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //rangeMerge.Style.Fill.BackgroundColor.SetColor(Color.Purple);

            rangeMerge.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
        private void FormatStyleGeneral(ExcelWorksheet worksheet, int defaultHeight)
        {
            worksheet.DefaultRowHeight = defaultHeight;

            worksheet.View.ShowGridLines = false;

            var allCells = worksheet.Cells[worksheet.Dimension.Address];
            allCells.Style.Indent = 1;
            allCells.Style.Font.Size = 11;
            allCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            allCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        }
        private void FormatValuesByColumns(int rowLoadTableOutHeader, int rowEnd, IEnumerable<ExcelHeaderIndexDTO> headerIndex, ExcelWorksheet worksheet)
        {
            //format columns
            foreach (var header in headerIndex)
            {
                var colHeader = header.Index + 1;

                using (var range = worksheet.Cells[rowLoadTableOutHeader, colHeader, rowEnd, colHeader])  
                {
                    range.Style.Numberformat.Format = header.Value.Type.GetDescription();

                    if (header.Value.Width.HasValue)
                        worksheet.Column(colHeader).Width = header.Value.Width.Value;
                    else
                    {
                        worksheet.Column(colHeader).AutoFit();
                        worksheet.Column(colHeader).Width = worksheet.Column(colHeader).Width * 1.1;
                    }

                    if (header.Value.IsBold)
                        range.Style.Font.Bold = true;

                    if (header.Value.IsCenter)
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Column(colHeader).Style.WrapText = true;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.White);
                }
            }
        }
        private void ConditionalFormatting(int rowLoadTableOutHeader, int rowEnd, IEnumerable<ExcelHeaderIndexDTO> headerIndex, ExcelWorksheet worksheet, Color striepColor, int headerCount)
        {
            bool setColor = true;
            int endColumn = headerCount < worksheet.Dimension.End.Column ? headerCount : worksheet.Dimension.End.Column;
            for (int i = 6; i <= worksheet.Dimension.End.Row; i++)
            {
                worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Border.Bottom.Color.SetColor(striepColor);
                if (setColor)
                {
                    worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Fill.BackgroundColor.SetColor(striepColor);
                    setColor = false;
                    continue;
                }
                setColor = true;
            }

            var headerResult = 
                headerIndex.Where(h => h.Value.IsConditionalFormatting);
            //format columns
            foreach (var header in headerResult)
            {
                var colHeader = header.Index + 1;

                using (var range = worksheet.Cells[rowLoadTableOutHeader, colHeader, rowEnd, colHeader])
                {
                    
                    foreach (var item in _colorScheme.Colors)
                    {
                        var f = worksheet.ConditionalFormatting.AddBetween(range);

                        f.Formula = (item.Min / 100).ToString(CultureInfo.InvariantCulture);
                        f.Formula2 = (item.Max / 100).ToString(CultureInfo.InvariantCulture);
                        Color color = ColorTranslator.FromHtml(item.Color);
                        Color fontColor = ColorTranslator.FromHtml(item.FontColor);
                        f.Style.Fill.BackgroundColor.Color = color;
                        f.Style.Font.Color.Color = fontColor;
                    }

                }
            }
        }
        private void FormatStyleGroupHeader(int rowStart, IEnumerable<ExcelHeaderIndexDTO> headerIndex, ExcelWorksheet worksheet, int rowLoadTable)
        {
            var groupsHeader = headerIndex
                       .Where(h => !string.IsNullOrWhiteSpace(h.Value.GroupName))?
                       .GroupBy(g => g.Value.GroupName, (key, value) => new GroupRange
                       {
                           GroupName = key,
                           Start = value.Min(m => m.Index) + 1,
                           End = value.Max(m => m.Index) + 1,
                           IsCenter = value.FirstOrDefault().Value.IsCenter
                       });

            foreach (var item in groupsHeader)
            {
                var rangeMerge = worksheet.Cells[rowStart, item.Start, rowStart, item.End];

                rangeMerge.Merge = true;
                rangeMerge.Value = item.GroupName;
                rangeMerge.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rangeMerge.Style.Font.Color.SetColor(Color.White);
                rangeMerge.Style.Font.Size = 10;
                rangeMerge.Style.Font.Bold = true;
                rangeMerge.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeMerge.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                rangeMerge.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                rangeMerge.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                rangeMerge.Style.Border.Right.Color.SetColor(Color.White);
                rangeMerge.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                rangeMerge.Style.Border.Left.Color.SetColor(Color.White);
                rangeMerge.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                rangeMerge.Style.Border.Bottom.Color.SetColor(Color.White);

                var rangeAll = worksheet.Cells[5, item.Start, worksheet.Dimension.End.Row, item.End];
                rangeAll.Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.White);

                if (item.IsCenter)
                {
                    var rangeHeader = worksheet.Cells[rowLoadTable, item.Start, rowLoadTable, item.End];
                    rangeAll.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
        }
        private DataTable GenerateDataTable(GenerateExcelFileRequest request, IEnumerable<ExcelHeaderIndexDTO> headerIndex)
        {
            DataTable dt = new DataTable();

            foreach (ExcelHeader header in request.Header)
            {
                var headerValue = FixColumnsDataTable(request, dt, header);
                switch (header.Type)
                {
                    case ExcelFieldType.NumberSimples:
                        dt.Columns.Add(headerValue, typeof(double));
                        break;
                    case ExcelFieldType.Money:
                        dt.Columns.Add(headerValue, typeof(double));
                        break;
                    case ExcelFieldType.Percentagem:
                        dt.Columns.Add(headerValue, typeof(double));
                        break;
                    case ExcelFieldType.Date:
                        dt.Columns.Add(headerValue, typeof(DateTime));
                        break;
                    case ExcelFieldType.Default:
                        dt.Columns.Add(headerValue, typeof(string));
                        break;
                    default:
                        dt.Columns.Add(headerValue, typeof(string));
                        break;
                }
            }

            foreach (List<ExcelBody> row in request.Body)
            {
                
                var dataRow = dt.NewRow();

                var rowIndex = row.Select((value, index) => new { value, index });

                foreach (var col in rowIndex)
                {

                    if (string.IsNullOrWhiteSpace(col.value.Value))
                    {
                        dataRow[col.index] = col.value.Value == null && dt.Columns[col.index].DataType.Name.Equals(typeof(double).Name) ? "0" : col.value.Value;
                        continue;
                    }

                    var typeCell = headerIndex
                    .FirstOrDefault(f => f.Index == col.index)
                    .Value
                    .Type;
      

                    switch (typeCell)
                    {
                        case ExcelFieldType.NumberSimples:
                            dataRow[col.index] = Math.Round(double.Parse(col.value.Value, CultureInfo.InvariantCulture),0);
                            break;
                        case ExcelFieldType.Money:
                            dataRow[col.index] = Math.Round(double.Parse(col.value.Value, CultureInfo.InvariantCulture),0);
                            break;
                        case ExcelFieldType.Percentagem:
                            dataRow[col.index] = double.Parse(col.value.Value, CultureInfo.InvariantCulture);
                            break;
                        case ExcelFieldType.Date:
                            dataRow[col.index] = DateTime.Parse(col.value.Value, CultureInfo.InvariantCulture);
                            break;
                        case ExcelFieldType.Default:
                            dataRow[col.index] = col.value.Value;
                            break;
                        default:
                            dataRow[col.index] = col.value.Value;
                            break;
                    }
                }

                dt.Rows.Add(dataRow);

            }

            return dt;
        }
        private string FixColumnsDataTable(GenerateExcelFileRequest request, DataTable dt, ExcelHeader header)
        {
            if (request.Header.Count(c => c.Value == header.Value) > 1)
            {

                _headerDataTableList.Add(new HeaderDataTable
                {
                    NewValue = header.Value,
                    OldValue = $"{header.Value}{dt.Columns.Count + 1}",
                    Position = dt.Columns.Count + 1
                });

                return $"{header.Value}{dt.Columns.Count + 1}";
            }

            return header.Value;
        }
        private static void AddTotal(List<List<ExcelBody>> bodyExcel, int rowEnd, IEnumerable<ExcelHeaderIndexDTO> headerIndex, ExcelWorksheet worksheet)
        {
            var lstGroupHeader = new List<string> {ComparativeAnalyseTableEnum.People.GetDescription(), ComparativeAnalyseTableEnum.Positions.GetDescription()};
            var columnsMerged = headerIndex
                       .Where(h => !string.IsNullOrWhiteSpace(h.Value.GroupName)
                       && lstGroupHeader.Contains(h.Value.GroupName))?
                       .GroupBy(g => g.Value.GroupName, (key, value) => new GroupRange
                       {
                           GroupName = key,
                           Start = value.Min(m => m.Index) + 1,
                           End = value.Max(m => m.Index) + 1,
                           IsCenter = value.FirstOrDefault().Value.IsCenter
                       });

            foreach(var colMerge in columnsMerged){

                var rangeMerge = worksheet.Cells[rowEnd, colMerge.Start, rowEnd, colMerge.End];

                rangeMerge.Merge = true;
                rangeMerge.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rangeMerge.Style.Font.Color.SetColor(Color.White);
                rangeMerge.Style.Font.Size = 11;
                rangeMerge.Style.Font.Bold = true;
                rangeMerge.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeMerge.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                rangeMerge.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                rangeMerge.Style.Border.Right.Style = ExcelBorderStyle.Thick;
                rangeMerge.Style.Border.Right.Color.SetColor(Color.White);
                rangeMerge.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                rangeMerge.Style.Border.Left.Color.SetColor(Color.White);
                rangeMerge.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                rangeMerge.Style.Border.Bottom.Color.SetColor(Color.White);

                rangeMerge.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
        }

        private void CustomConditionalFormatting(int rowLoadTableOutHeader, int rowEnd, int columnEnd, ExcelWorksheet worksheet, int headerCount, Color striepColor)
        {
            bool setColor = true;
            int endColumn = headerCount < worksheet.Dimension.End.Column ? headerCount : worksheet.Dimension.End.Column;
            for (int i = 6; i <= worksheet.Dimension.End.Row; i++)
            {
                worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Border.Bottom.Color.SetColor(striepColor);
                if (setColor)
                {
                    worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[i, worksheet.Dimension.Start.Column, i, endColumn].Style.Fill.BackgroundColor.SetColor(striepColor);
                    setColor = false;
                    continue;
                }
                setColor = true;
            }

            //conditional formatting
            var rowEndFix = rowEnd;

            var rangeConditional = worksheet.Cells[rowLoadTableOutHeader, 6, rowEndFix, columnEnd];


            foreach (var item in _colorScheme.Colors)
            {
                var f = worksheet.ConditionalFormatting.AddBetween(rangeConditional);

                f.Formula = (item.Min / 100).ToString(CultureInfo.InvariantCulture);
                f.Formula2 = (item.Max / 100).ToString(CultureInfo.InvariantCulture);
                Color color = ColorTranslator.FromHtml(item.Color);
                Color fontColor = ColorTranslator.FromHtml(item.FontColor);
                f.Style.Fill.BackgroundColor.Color = color;
                f.Style.Font.Color.Color = fontColor;
            }
        }
    }
}

