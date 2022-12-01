export function emptyRowBySize(size: number, init: number = 0) {
  const data = [];

  for (var i = init; i < size; i++) {
    data.push("");
  }

  return Object.assign(data);
}

export function convertUrlToBase64(url: string) {
  return fetch(url)
    .then((response) => response.blob())
    .then(
      (blob) =>
        new Promise((resolve, reject) => {
          const reader = new FileReader();
          reader.onloadend = () => resolve(reader.result);
          reader.onerror = reject;
          reader.readAsDataURL(blob);
        })
    );
}

export function getNumberByPercent(f: string): string {
  switch (f) {
    case "160%":
      return "+6";
    case "150%":
      return "+5";
    case "140%":
      return "+4";
    case "130%":
      return "-3";
    case "120%":
      return "+2";
    case "110%":
      return "+1";
    case "100%":
      return "Mid";
    case "90%":
      return "-1";
    case "80%":
      return "-2";
    case "70%":
      return "-3";
    case "60%":
      return "-4";
    case "50%":
      return "-5";
    case "40%":
      return "-6";
    default:
      return "";
  }
}

export function generateHeader(
  dinamicTitle: string,
  line4: any,
  line5: any,
  nameTableRequiredFields: string
): any[] {
  const headers = [
    {
      1: `${dinamicTitle}`,
      2: null,
      3: "",
      4: "",
      5: "",
      6: "",
      7: "",
      8: "",
      9: "",
      10: "",
      11: "",
      12: "",
      13: "",
      14: "",
      15: "",
      16: "",
      17: "",
      18: "",
      19: "",
      20: "",
    },
    {
      1: nameTableRequiredFields,
      2: "",
      3: "",
      4: "",
      5: "",
      6: "",
      7: "",
      8: "",
      9: "",
      10: "",
      11: "",
      12: "",
      13: "",
      14: "",
      15: "",
      16: "",
      17: "",
      18: "",
      19: "",
      20: "",
    },
    Object.assign(line4),
    Object.assign(line5),
  ];

  return headers;
}

export function generateHeaderImpact(
  dinamicTitle: string,
  line4: any,
  line5: any,
  nameTableRequiredFields: string
): any[] {
  const headers = [
    {
      1: `${dinamicTitle}`,
      2: null,
      3: "",
      4: "",
      5: "",
      6: "",
      7: "",
      8: "",
      9: "",
      10: "",
      11: "",
      12: "",
      13: "",
      14: "",
      15: "",
      16: "",
      17: "",
      18: "",
      19: "",
      20: "",
    },
    {
      1: nameTableRequiredFields,
      2: "",
      3: "",
      4: "",
      5: "",
      6: "",
      7: "",
      8: "",
      9: "",
      10: "",
      11: "",
      12: "",
      13: "",
      14: "",
      15: "",
      16: "",
      17: "",
      18: "",
      19: "",
      20: "",
    },
    Object.assign(line4),
    Object.assign(line5),
    {
      1: "",
      2: "",
      3: "Func.",
      4: "R$",
      5: "%",
      6: "",
      7: "Func.",
      8: "R$",
      9: "%",
      10: "",
      11: "Func.",
      12: "R$",
      13: "%",
      14: "",
      15: "Func.",
      16: "R$",
      17: "%",
      18: "",
      19: "",
      20: "",
    },
  ];

  return headers;
}
