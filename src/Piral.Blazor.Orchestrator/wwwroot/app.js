typeof Blazor === 'undefined' && (function () {
  const blazorServer = '_framework/blazor.server.js';
  const loaded = [];
  const loading = [blazorServer];

  function tryInit(src) {
    loading.splice(loading.indexOf(src), 1);

    if (loading.length === 0 && typeof Blazor !== 'undefined') {
      Blazor.start();
    }
  }

  class BlazorScript extends HTMLElement {
    constructor() {
      super();
    }

    connectedCallback() {
      const src = this.getAttribute("src");
      const type = this.getAttribute("type");

      if (src && !loaded.includes(src)) {
        const script = document.createElement("script");
        script.async = true;
        script.src = src;

        if (type) {
          script.type = type;
        }

        loading.push(src);
        loaded.push(src);
        script.onload = () => tryInit(src);
        document.head.appendChild(script);
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

  setTimeout(() => {  
    const s = document.createElement('script');
    s.setAttribute('autostart', 'false');
    s.setAttribute('src', blazorServer)
    s.async = true;
    s.onload = () => tryInit(blazorServer);
    document.head.appendChild(s);
  }, 0);
})();
