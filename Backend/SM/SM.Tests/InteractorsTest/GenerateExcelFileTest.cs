using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Domain.Enum;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Tests.InteractorsTest
{
    [TestClass]
    public class GenerateExcelFileTest
    {

        [TestMethod]
        public async Task SaveFile()
        {

            var generateExcelFileInteractor = new GenerateExcelFileInteractor(new ColorScheme
            {
                Colors = new List<ColorData>
                           {
                               new ColorData
                               {
                                   Color = "#fafafa",
                                   Min = 50,
                                   Max = 90
                               }
                           }
            });

            var header = new List<ExcelHeader>
            {
                new ExcelHeader
                {
                    Value = "Cargo"
                },
                new ExcelHeader
                {
                    Value = "Empresa"
                },
                new ExcelHeader
                {
                    GroupName = "Grupo01",
                    Value = "Cidade",
                    IsBold = true,
                    Width = 150
                },
                new ExcelHeader
                {
                    GroupName = "Grupo01",
                    Value = "Salário",
                    Type = ExcelFieldType.Money
                }
            };

            var body = new List<List<ExcelBody>>
            {
                new List<ExcelBody>
                {

                    new ExcelBody
                    {
                        Value = "Administrativo",
                    },
                    new ExcelBody
                    {
                        Value = "Empresa01",
                    },
                    new ExcelBody
                    {
                        Value = "Campinas",
                    },
                    new ExcelBody
                    {
                        Value = "10000",
                    }

                },
                new List<ExcelBody>
                {

                    new ExcelBody
                    {
                        Value = "Engenheiro",
                    },
                    new ExcelBody
                    {
                        Value = "Empresa02",
                    },
                    new ExcelBody
                    {
                        Value = "São José do Rio Preto",
                    },
                    new ExcelBody
                    {
                        Value = "2",
                    }

                },
                new List<ExcelBody>
                {

                    new ExcelBody
                    {
                        Value = "Analista de Sistemas",
                    },
                    new ExcelBody
                    {
                        Value = "Empresa03",
                    },
                    new ExcelBody
                    {
                        Value = "Indaituba",
                    },
                    new ExcelBody
                    {
                        Value = "1000.50",
                    }

                }
            };

            var result = await generateExcelFileInteractor.Handler(new GenerateExcelFileRequest
            {
                Body = body,
                Header = header
            });


           // Assert.AreEqual(result, 1);
        }


    }
}
