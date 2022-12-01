export function validatorCelIsNumeric(cell: any): boolean {
  let colContainsValueNumber = false;

  try {
    const validator = parseInt(cell.value.toString());
    colContainsValueNumber = !isNaN(validator);
  } catch (error) {
    colContainsValueNumber = false;
  }

  return colContainsValueNumber;
}

export function getBgColorByCol(colNumber: number): string {
  switch (colNumber) {
    case 1:
      return "7f7f7f";
    case 3:
      return "924A73";
    case 5:
      return "73AA8B";
    case 7:
      return "6C82AB";
    default:
      return "FFFFFF";
  }
}

export function getBgColorByColFinancialImpact(colNumber: number): string {
  switch (colNumber) {
    case 1:
      return "FFFFFF";
    case 3:
      return "73AA8B";
    case 7:
      return "EBCB64";
    case 11:
      return "6C82AB";
    case 15:
      return "B95455";
    default:
      return "FFFFFF";
  }
}

export function getBgColorBySubColFinancialImpact(colNumber: number): string {
  if (colNumber >= 3 && colNumber <= 5) return "609075";
  if (colNumber >= 7 && colNumber <= 9) return "CCB86B";
  if (colNumber >= 11 && colNumber <= 13) return "607396";
  if (colNumber >= 15 && colNumber <= 17) return "964747";

  return "FFFFFF";
}

export function constainsColumnForBold(colNumber: number): boolean {
  return (
    colNumber === 3 ||
    colNumber === 5 ||
    colNumber === 7 ||
    colNumber === 9 ||
    colNumber === 11 ||
    colNumber === 13 ||
    colNumber === 15 ||
    colNumber === 17
  );
}

export function getBgColorByColProposedMovements(colNumber: number): string {
  switch (colNumber) {
    case 1:
      return "FFFFFF";
    case 3:
      return "5F7CB1";
    case 5:
      return "924A73";
    case 7:
      return "A6A6A6";
    default:
      return "FFFFFF";
  }
}
