const colorsRangeBasedOnValues = [
  {
    min: 130,
    max: 999,
    color: "#537158",
  },
  {
    min: 121,
    max: 130,
    color: "#668C6C",
  },
  {
    min: 110,
    max: 120.999,
    color: "#81A487",
  },
  {
    min: 103,
    max: 109.999,
    color: "#9FBDA1",
  },
  {
    min: 98,
    max: 102.999,
    color: "#B5CBB7",
  },
  {
    min: 93,
    max: 97.999,
    color: "#D4E1D6",
  },
  {
    min: 89,
    max: 92.999,
    color: "#EBF1EC",
  },
  {
    min: 86,
    max: 88.999,
    color: "#FAEEEC",
  },
  {
    min: 83,
    max: 85.999,
    color: "#F3D9D5",
  },
  {
    min: 78,
    max: 82.999,
    color: "#E2AAA2",
  },
  {
    min: 70,
    max: 77.999,
    color: "#CE7F74",
  },
  {
    min: 0,
    max: 69.999,
    color: "#A4483E",
  },
];

export function styleBasedOnValue(data: number) {
  if (!data) {
    return "";
  }
  const colorRange = colorsRangeBasedOnValues.find(
    (value) => data >= value.min && data <= value.max
  );
  const text = data <= 102.999 && data >= 83 ? "#595959" : "white";

  return `
  position: relative;
  height: 30px;
  bottom: 8px;
  color:${!colorRange ? "#595959" : text};
  background-color:${colorRange ? colorRange.color : "#ddd"}`;
}

export function fillBasedOnValue(data: number): any {
  if (!data) {
    return "";
  }
  const colorRange = colorsRangeBasedOnValues.find(
    (value) => data >= value.min && data <= value.max
  );
  const text = data <= 102.999 && data >= 83 ? "595959" : "ffffff";

  const bg = colorRange ? colorRange.color : "#ddd";
  const color = !colorRange ? "595959" : text;
  const returnColored = {
    backgroundColor: bg.replace("#", ""),
    color: color,
  };

  return returnColored;
}

export function fillFinancialImpactBasedOnValue(data: number): any {
  if (!data) {
    return "";
  }

  const dataFloated = parseFloat((data / 1500).toString());

  // green by Carreira
  const valueToGreen = RGBAtoRGB(
    46,
    128,
    46,
    dataFloated > 1 ? 1 : dataFloated
  );

  const hexed = rgbToHex(valueToGreen.r, valueToGreen.g, valueToGreen.b);
  return hexed;
}

function RGBAtoRGB(r, g, b, a, r2 = 255, g2 = 255, b2 = 255) {
  var r3 = Math.round((1 - a) * r2 + a * r);
  var g3 = Math.round((1 - a) * g2 + a * g);
  var b3 = Math.round((1 - a) * b2 + a * b);
  return { r: r3, g: g3, b: b3 };
}

function componentToHex(c) {
  var hex = c.toString(16);
  return hex.length == 1 ? "0" + hex : hex;
}

function rgbToHex(r, g, b) {
  return componentToHex(r) + componentToHex(g) + componentToHex(b);
}
