(async function () {
  const target = "/_debug";
  const colors = [
    "#001F3F",
    "#0074D9",
    "#7FDBFF",
    "#39CCCC",
    "#3D9970",
    "#2ECC40",
    "#01FF70",
    "#FFDC00",
    "#FF851B",
    "#FF4136",
    "#85144B",
    "#F012BE",
    "#B10DC9",
  ];

  function installPiralDebug(options) {
    const {
      getGlobalState,
      getExtensions,
      getDependencies,
      getRoutes,
      getPilets,
      fireEvent,
      removePilet,
      updatePilet,
      addPilet,
    } = options;
    const events = [];
    const settings = {
      extensionCatalogue: true,
      viewOrigins: true,
    };
    const selfSource = "piral-debug-api";
    const debugApiVersion = "v1";

    const retrievePilets = () =>
      getPilets().map((pilet) => ({
        name: pilet.name,
        version: pilet.version,
        disabled: pilet.disabled,
      }));

    const inspectorSettings = {
      viewOrigins: {
        value: settings.viewOrigins,
        type: "boolean",
        label: "Visualize component origins",
        onChange(value) {
          settings.viewOrigins = value;
        },
      },
      extensionCatalogue: {
        value: settings.extensionCatalogue,
        type: "boolean",
        label: "Enable extension catalogue",
        onChange(value) {
          settings.extensionCatalogue = value;
        },
      },
    };

    const sendMessage = (content) => {
      window.postMessage(
        {
          content,
          source: selfSource,
          version: debugApiVersion,
        },
        "*"
      );
    };

    const getSettings = () => {
      return Object.keys(inspectorSettings).reduce((obj, key) => {
        const setting = inspectorSettings[key];

        if (
          setting &&
          typeof setting === "object" &&
          typeof setting.label === "string" &&
          typeof setting.type === "string" &&
          ["boolean", "string", "number"].includes(typeof setting.value)
        ) {
          obj[key] = {
            label: setting.label,
            value: setting.value,
            type: setting.type,
          };
        }

        return obj;
      }, {});
    };

    const updateSettings = (values) => {
      Object.keys(values).forEach((name) => {
        const setting = inspectorSettings[name];

        switch (setting.type) {
          case "boolean": {
            const prev = setting.value;
            const value = values[name];
            setting.value = value;
            setting.onChange(value, prev);
            break;
          }
          case "number": {
            const prev = setting.value;
            const value = values[name];
            setting.value = value;
            setting.onChange(value, prev);
            break;
          }
          case "string": {
            const prev = setting.value;
            const value = values[name];
            setting.value = value;
            setting.onChange(value, prev);
            break;
          }
        }
      });

      sendMessage({
        settings: getSettings(),
        type: "settings",
      });
    };

    const togglePilet = (name) => {
      const pilet = getPilets().find((m) => m.name === name);

      if (!pilet) {
        // nothing to do, obviously invalid call
      } else if (pilet.disabled) {
        pilet.disabled = false;
        updatePilet(name, false);
      } else {
        pilet.disabled = true;
        updatePilet(name, true);
      }

      sendMessage({
        type: "pilets",
        pilets: retrievePilets(),
      });
    };

    const toggleVisualize = () => {
      let visualizer = document.querySelector("#piral-inspector-visualizer");

      if (visualizer) {
        visualizer.remove();
      } else {
        const moduleColor = {};
        visualizer = document.body.appendChild(document.createElement("div"));
        visualizer.id = "piral-inspector-visualizer";
        visualizer.style.position = "absolute";
        visualizer.style.top = "0";
        visualizer.style.left = "0";
        visualizer.style.width = "0";
        visualizer.style.height = "0";

        document.querySelectorAll("piral-component").forEach((element) => {
          const targetRect = element.firstElementChild.getBoundingClientRect();
          const pilet = element.getAttribute("origin");
          const vis = visualizer.appendChild(document.createElement("div"));
          const info = vis.appendChild(document.createElement("div"));
          vis.style.position = "absolute";
          vis.style.left = targetRect.left + "px";
          vis.style.top = targetRect.top + "px";
          vis.style.width = targetRect.width + "px";
          vis.style.height = targetRect.height + "px";
          vis.style.pointerEvents = "none";
          vis.style.zIndex = "99999999999";
          vis.style.border = "1px solid #ccc";
          info.style.color = "white";
          info.textContent = pilet;
          info.style.position = "absolute";
          info.style.right = "0";
          info.style.top = "0";
          info.style.fontSize = "8px";
          info.style.background =
            moduleColor[pilet] ||
            (moduleColor[pilet] =
              colors[Object.keys(moduleColor).length % colors.length]);
        });
      }
    };

    const depMap = {};

    const debugApi = {
      debug: debugApiVersion,
      instance: {
        name: "piral-server-demo",
        version: "v1",
        dependencies: getDependencies(),
      },
      build: {
        date: "",
        cli: "",
        compat: "next",
      },
    };

    const details = {
      name: debugApi.instance.name,
      version: debugApi.instance.version,
      kind: debugApiVersion,
      mode: "development",
      capabilities: [
        "events",
        "container",
        "routes",
        "pilets",
        "settings",
        "extensions",
        "dependencies",
        "dependency-map",
      ],
    };

    const start = () => {
      const container = getGlobalState();
      const routes = getRoutes();
      const extensions = getExtensions();
      const settings = getSettings();
      const dependencies = getDependencies();
      const pilets = retrievePilets();

      sendMessage({
        type: "available",
        ...details,
        state: {
          routes,
          pilets,
          container,
          settings,
          events,
          extensions,
          dependencies,
        },
      });
    };

    const check = () => {
      sendMessage({
        type: "info",
        ...details,
      });
    };

    const getDependencyMap = () => {
      const dependencyMap = {};
      const addDeps = (pilet, dependencies) => {
        const deps = dependencyMap[pilet] || [];

        for (const depName of Object.keys(dependencies)) {
          if (!deps.some((m) => m.demanded === depName)) {
            deps.push({
              demanded: depName,
              resolved: dependencies[depName],
            });
          }
        }

        dependencyMap[pilet] = deps;
      };
      const pilets = getPilets()
        .map((pilet) => ({
          name: pilet.name,
          link: pilet.link,
          basePath: pilet.basePath,
        }))
        .filter((m) => m.link);

      Object.keys(depMap).forEach((url) => {
        const dependencies = depMap[url];
        const pilet = pilets.find((p) => p.link === url);

        if (pilet) {
          addDeps(pilet.name, dependencies);
        } else if (!pilet) {
          const parent = pilets.find((p) => url.startsWith(p.basePath));

          if (parent) {
            addDeps(parent.name, dependencies);
          }
        }
      });

      sendMessage({
        type: "dependency-map",
        dependencyMap,
      });
    };

    window.addEventListener("message", (event) => {
      const { source, version, content } = event.data;

      if (source !== selfSource && version === debugApiVersion) {
        switch (content.type) {
          case "init":
            return start();
          case "check-piral":
            return check();
          case "get-dependency-map":
            return getDependencyMap();
          case "update-settings":
            return updateSettings(content.settings);
          case "append-pilet":
            return addPilet(content.meta);
          case "remove-pilet":
            return removePilet(content.name);
          case "toggle-pilet":
            return togglePilet(content.name);
          case "emit-event":
            return fireEvent(content.name, content.args);
          case "goto-route":
            return history.pushState(content.state, undefined, content.route);
          case "visualize-all":
            return toggleVisualize();
        }
      }
    });

    window["dbg:piral"] = debugApi;
    start();
  }

  function writeState(type, data) {
    return fetch(`${target}/${type}`, {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "content-type": "application-json",
      },
    })
      .then((res) => res.json())
      .then((newState) => {
        state = newState;
      });
  }

  function readState() {
    return fetch(target, {
      method: "GET",
      headers: {
        "content-type": "application-json",
      },
    }).then((res) => res.json());
  }

  let state = await readState();

  installPiralDebug({
    fireEvent(name, args) {
      writeState("event", {
        name,
        args,
      });
    },
    getDependencies() {
      return Object.keys(state.dependencies);
    },
    getExtensions() {
      return Object.keys(state.extensions);
    },
    getGlobalState() {
      return state;
    },
    getPilets() {
      return state.pilets;
    },
    getRoutes() {
      return state.routes;
    },
    addPilet(pilet) {
      writeState("pilet", {
        mode: "add",
        name: pilet.name,
        version: pilet.version,
        disabled: false,
      });
    },
    removePilet(name) {
      writeState("pilet", {
        mode: "remove",
        name,
      });
    },
    updatePilet(name, disabled) {
      writeState("pilet", {
        mode: "update",
        name,
        disabled,
      });
    },
  });
})();
