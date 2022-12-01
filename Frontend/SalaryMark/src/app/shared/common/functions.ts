export const copyObject = (array: any) => {
  return JSON.parse(JSON.stringify(array));
};

export const isUndefined = (input: any) => {
  return input === undefined;
};

export const delay = (callback, ms) => {
  var timer = null;
  return function () {
    var context = this,
      args = arguments;
    clearTimeout(timer);
    timer = setTimeout(function () {
      callback.apply(context, args);
    }, ms || 0);
  };
};

export const applyLink = (content: string) => {
  const replaceMail: RegExp =
    /(([a-zA-Z0-9\-\_\.])+@[a-zA-Z\_]+?(\.[a-zA-Z]{2,6})+)/gim;
  const replaceWww: RegExp = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
  const replaceHttpContent: RegExp =
    /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;

  let replacedText: string = "";

  replacedText = content.replace(
    replaceHttpContent,
    '<a href="$1" target="_blank">$1</a>'
  );

  replacedText = replacedText.replace(
    replaceWww,
    '$1<a href="http://$2" target="_blank">$2</a>'
  );

  replacedText = replacedText.replace(
    replaceMail,
    '<a href="mailto:$1">$1</a>'
  );

  replacedText = replacedText.replace(/\n/g, "<br />");

  return replacedText;
};

export const changeChartSvgFont = (
  elementRef: {
    nativeElement: { getElementsByTagName: (arg0: string) => any };
  },
  tagName: string,
  fontFamily: string
) => {
  var element = elementRef.nativeElement.getElementsByTagName(tagName);
  for (const node of element) {
    node.setAttribute(
      "style",
      `color:#656565;cursor:default;font-size:0.875rem;font-family:${fontFamily};font-weight:600;text-align:right;fill:#656565;`
    );
    node.replaceWith(node);
  }
};
