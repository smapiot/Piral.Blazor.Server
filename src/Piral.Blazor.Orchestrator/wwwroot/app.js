typeof Blazor === "undefined" &&
  (function () {
    const blazorServer = "_framework/blazor.server.js";
    const loaded = [];
    const loading = [blazorServer];

    function tryInit(src) {
      loading.splice(loading.indexOf(src), 1);

      if (loading.length === 0 && typeof Blazor !== "undefined") {
        Blazor.start();
      }
    }

    function defer(cb) {
      setTimeout(cb, 0);
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
        this.deferEvent("add-component");
      }

      disconnectedCallback() {
        this.deferEvent("remove-component");
      }

      deferEvent(eventName) {
        const ev = new CustomEvent(eventName, {
          detail: { name: this.name, origin: this.origin },
        });
        defer(() => window.dispatchEvent(ev));
      }
    }

    customElements.define("blazor-script", BlazorScript);
    customElements.define("piral-component", PiralComponent);

    defer(() => {
      const s = document.createElement("script");
      s.setAttribute("autostart", "false");
      s.setAttribute("src", blazorServer);
      s.async = true;
      s.onload = () => tryInit(blazorServer);
      document.head.appendChild(s);
    });
  })();
