import{_ as e,c as t,o as a,a2 as o}from"./chunks/framework.Ce_20IA6.js";const g=JSON.parse('{"title":"Getting Started","description":"","frontmatter":{},"headers":[],"relativePath":"getting-started/index.md","filePath":"getting-started/index.md","lastUpdated":1714604282000}'),r={name:"getting-started/index.md"},i=o('<h1 id="getting-started" tabindex="-1">Getting Started <a class="header-anchor" href="#getting-started" aria-label="Permalink to &quot;Getting Started&quot;">​</a></h1><p><code>Piral.Blazor</code> exists in multiple forms. Right now, we have:</p><ul><li><code>Piral.Blazor.Core</code>, which is made to be consumed in a JavaScript / SPA created with the <a href="https://docs.piral.io" target="_blank" rel="noreferrer">Piral</a> framework</li><li><code>Piral.Blazor.Orchestrator</code>, which is made to be consumed in an ASP.NET Core web application to enable server-side micro frontends</li></ul><p>While both are fundamentally different, they share the same goals and have an overlapping development philosophy.</p><div class="info custom-block"><p class="custom-block-title">INFO</p><p><code>Piral.Blazor.Core</code> is never directly referenced, you&#39;d rather install the <a href="https://www.npmjs.com/package/blazor" target="_blank" rel="noreferrer"><code>blazor</code></a> npm package.</p></div><p>In the future there will be an overlap between the two, for now you need to decide in the beginning. Usually, the decision is rather simple - it boils down to:</p><ul><li>If you have an existing Blazor server application that you now want to continue developing using a micro frontend approach then use <code>Piral.Blazor.Orchestrator</code></li><li>If you have an existing SPA that you want to extend with some component written in Blazor then aim for the Piral framework with <code>Piral.Blazor.Core</code></li></ul><p>Generally, <code>Piral.Blazor.Orchestrator</code> will allow you to do everything in Blazor while <code>Piral.Blazor.Core</code> will require you to work with TypeScript and other technologies such as React, too.</p>',8),n=[i];function l(d,c,s,h,p,u){return a(),t("div",null,n)}const _=e(r,[["render",l]]);export{g as __pageData,_ as default};