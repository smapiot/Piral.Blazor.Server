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

  class PiralComponent extends HTMLElement {
    get name() {
      return this.getAttribute("name");
    }

    get origin() {
      return this.getAttribute("origin");
    }

    connectedCallback() {
      window.dispatchEvent(
        new CustomEvent("add-component", {
          detail: { name: this.name, origin: this.origin },
        })
      );
    }

    disconnectedCallback() {
      window.dispatchEvent(
        new CustomEvent("remove-component", {
          detail: { name: this.name, origin: this.origin },
        })
      );
    }
  }

  customElements.define("blazor-script", BlazorScript);
  customElements.define("piral-component", PiralComponent);
})();
