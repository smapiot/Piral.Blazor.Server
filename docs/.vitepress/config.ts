import { defineConfig } from "vitepress";
import type { DefaultTheme } from "vitepress";

const sidebars = (): DefaultTheme.SidebarItem[] => [
  {
    text: "Getting Started",
    collapsed: true,
    items: [
      { text: "Basics", link: "/getting-started/" },
      { text: "Setup", link: "/getting-started/setup" },
    ],
  },
  {
    text: "Guides",
    collapsed: true,
    items: [
      { text: "Overview", link: "/guides/" },
      { text: "Helpers", link: "/guides/helpers" },
      { text: "FAQs", link: "/guides/faq" },
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
      pattern: "https://github.com/smapiot/Piral.Blazor.Server/edit/main/:path",
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
