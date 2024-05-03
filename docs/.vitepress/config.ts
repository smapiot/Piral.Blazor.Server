import { defineConfig } from "vitepress";
import type { DefaultTheme } from "vitepress";

const sidebars = (): DefaultTheme.SidebarItem[] => [
  {
    text: "Getting Started",
    collapsed: true,
    items: [
      { text: "Basics", link: "/getting-started/" },
      { text: "Concepts", link: "/getting-started/concepts" },
    ],
  },
  {
    text: "Server",
    collapsed: true,
    items: [
      { text: "Setup", link: "/getting-started/server/setup" },
      { text: "Tooling", link: "/getting-started/server/tooling" },
      { text: "Emulator", link: "/getting-started/server/emulator" },
      { text: "Micro Frontend", link: "/getting-started/server/microfrontend" },
      { text: "Publishing", link: "/getting-started/server/publishing" },
      { text: "Runtime Configuration", link: "/getting-started/server/environment" },
      { text: "MF Service", link: "/getting-started/server/mf-service" },
    ],
  },
  {
    text: "SPA",
    collapsed: true,
    items: [
      { text: "Setup", link: "/getting-started/spa/setup" },
      { text: "Micro Frontend", link: "/getting-started/spa/microfrontend" },
      { text: "Debugging", link: "/getting-started/spa/debugging" },
      { text: "Publishing", link: "/getting-started/spa/publishing" },
      { text: "Build Configuration", link: "/getting-started/spa/configuration" },
      { text: "Dependency Injection", link: "/getting-started/spa/dependency-injection" },
      { text: "Page Components", link: "/getting-started/spa/pages" },
      { text: "Extension Components", link: "/getting-started/spa/extensions" },
      { text: "Other Components", link: "/getting-started/spa/other-components" },
      { text: "Provider Components", link: "/getting-started/spa/providers" },
      { text: "Root Component", link: "/getting-started/spa/root" },
      { text: "Parameters", link: "/getting-started/spa/parameters" },
      { text: "Runtime Configuration", link: "/getting-started/server/environment" },
      { text: "Pilet Service", link: "/getting-started/spa/pilet-service" },
      { text: "Localization", link: "/getting-started/spa/localization" },
      { text: "HTTP Injector", link: "/getting-started/spa/http" },
    ],
  },
  {
    text: "Guides",
    collapsed: true,
    items: [
      { text: "Overview", link: "/guides/" },
      { text: "Helpers", link: "/guides/helpers" },
      { text: "FAQs", link: "/guides/faq" },
      { text: "Roadmap", link: "/guides/roadmap" },
    ],
  },
  {
    text: "Examples",
    collapsed: true,
    items: [
      { text: "Introduction", link: "/examples/" },
    ],
  },
];

export default defineConfig({
  lang: "en-US",
  title: "Piral.Blazor",
  description:
    "Piral.Blazor allows you to write distributed web applications using dotnet and Blazor.",
  lastUpdated: true,
  ignoreDeadLinks: true,
  cleanUrls: true,

  markdown: {
    theme: {
      light: "github-light",
      dark: "github-dark",
    },
  },
  themeConfig: {
    logo: "/images/logo-small.png",
    siteTitle: "Piral.Blazor",
    socialLinks: [
      {
        icon: "github",
        link: "https://github.com/smapiot/Piral.Blazor.Server",
      },
      { icon: "discord", link: "https://discord.gg/kKJ2FZmK8t" },
      { icon: "x", link: "https://twitter.com/cloudpiral" },
    ],
    editLink: {
      pattern: "https://github.com/smapiot/Piral.Blazor.Server/edit/main/docs/:path",
      text: "Edit this page on GitHub",
    },
    footer: {
      message: "Released under the MIT License.",
      copyright: "Copyright © 2020 - 2024 smapiot & contributors",
    },
    nav: [
      { text: "Docs", link: "/getting-started/" },
      { text: "Examples", link: "/examples/" },
      { text: "Imprint", link: "/imprint" },
    ],
    sidebar: {
      "/": sidebars(),
    },
  },
  head: [
    [
      "meta",
      {
        property: "og:image",
        content: "https://blazor.piral.io/images/og-title.png",
      },
    ],
    ["meta", { property: "og:type", content: "website" }],
    ["meta", { property: "twitter:domain", content: "blazor.piral.io" }],
    [
      "meta",
      {
        property: "twitter:image",
        content: "https://blazor.piral.io/images/og-title.png",
      },
    ],
    ["meta", { property: "twitter:card", content: "summary_large_image" }],
    ["link", { rel: "shortcut icon", href: "/favicon.ico" }],
  ],
  titleTemplate: ":title - Piral.Blazor",
});
