import{_ as i,c as a,a0 as t,o as s}from"./chunks/framework.BuFtVe7P.js";const p=JSON.parse('{"title":"Running and Debugging the Pilet","description":"","frontmatter":{},"headers":[],"relativePath":"getting-started/spa/debugging.md","filePath":"getting-started/spa/debugging.md","lastUpdated":1726577356000}'),n={name:"getting-started/spa/debugging.md"};function o(l,e,r,d,g,u){return s(),a("div",null,e[0]||(e[0]=[t(`<h1 id="running-and-debugging-the-pilet" tabindex="-1">Running and Debugging the Pilet <a class="header-anchor" href="#running-and-debugging-the-pilet" aria-label="Permalink to &quot;Running and Debugging the Pilet&quot;">​</a></h1><p>A micro frontend can be debugged using an IDE or the command line.</p><nav class="table-of-contents"><ul><li><a href="#using-visual-studio">Using Visual Studio</a></li><li><a href="#using-the-command-line">Using the Command Line</a></li></ul></nav><h2 id="using-visual-studio" tabindex="-1">Using Visual Studio <a class="header-anchor" href="#using-visual-studio" aria-label="Permalink to &quot;Using Visual Studio&quot;">​</a></h2><p>The by far easiest way is to install the <code>Piral.Blazor.DevServer</code> package (as a replacement for the <code>Microsoft.AspNet.Components.WebAssembly.DevServer</code> package) and use F5 in Microsoft Visual Studio, JetBrains Rider, ... (essentially your IDE, if capable of running Blazor).</p><h2 id="using-the-command-line" tabindex="-1">Using the Command Line <a class="header-anchor" href="#using-the-command-line" aria-label="Permalink to &quot;Using the Command Line&quot;">​</a></h2><p>Alternatively, from your Blazor project folder, you can run your pilet via the Piral CLI:</p><div class="language-sh vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">sh</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">cd</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> ../piral~/</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&lt;</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">project-nam</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">e</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&gt;</span></span>
<span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">npm</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> start</span></span></code></pre></div><p>In addition to this, if you want to debug your Blazor pilet using for example Visual Studio, these requirements should be considered:</p><ul><li>keep the Piral CLI running</li><li>debug your Blazor pilet using IISExpress</li></ul><blockquote><p>⚠️ if you want to run your pilet and directly visit it in the browser without debugging via IISExpress, you will have to disable a <a href="https://github.com/FlorianRappl/kras" target="_blank" rel="noreferrer">kras</a> script injector <strong>before</strong> visiting the pilet. To do this, go to <code>http://localhost:1234/manage-mock-server/#/injectors</code>, disable the <code>debug.js</code> script, and save your changes. Afterwards, you can visit <code>http://localhost:1234</code>.</p></blockquote>`,11)]))}const c=i(n,[["render",o]]);export{p as __pageData,c as default};
