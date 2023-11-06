(function () {
  const loaded = [];

  class BlazorScript extends HTMLElement {
    constructor() {
      super();
    }

    connectedCallback() {
      const src = this.getAttribute("src");
      const type = this.getAttribute("type");

      if (src && !loaded.includes(src)) {
        const script = document.createElement("script");
        script.src = src;

        if (type) {
          script.type = type;
        }

        loaded.push(src);
        document.body.appendChild(script);
      }
    }
  }

  customElements.define("blazor-script", BlazorScript);
})();
