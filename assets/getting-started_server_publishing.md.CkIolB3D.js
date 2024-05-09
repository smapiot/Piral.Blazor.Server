import{_ as e,c as i,o as s,a2 as a}from"./chunks/framework.a4z-mAWu.js";const k=JSON.parse('{"title":"Publishing a Micro Frontend","description":"","frontmatter":{},"headers":[],"relativePath":"getting-started/server/publishing.md","filePath":"getting-started/server/publishing.md","lastUpdated":1715296913000}'),t={name:"getting-started/server/publishing.md"},n=a('<h1 id="publishing-a-micro-frontend" tabindex="-1">Publishing a Micro Frontend <a class="header-anchor" href="#publishing-a-micro-frontend" aria-label="Permalink to &quot;Publishing a Micro Frontend&quot;">​</a></h1><p>Micro frontends should be published to a discovery service as well as in form of a NuGet package. If you use the <a href="https://www.piral.cloud/" target="_blank" rel="noreferrer">Piral Cloud Feed Service</a> you can also just publish them as NuGet package; the discovery service will do the rest for you.</p><h2 id="publishing-using-the-piral-server-server-cli" tabindex="-1">Publishing using the <code>piral-server-server</code> CLI <a class="header-anchor" href="#publishing-using-the-piral-server-server-cli" aria-label="Permalink to &quot;Publishing using the `piral-server-server` CLI&quot;">​</a></h2><p>First, make sure you have the CLI installed. If not, do so using this command:</p><div class="language-sh vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">sh</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">dotnet</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> tool</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> install</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> --global</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> Piral.Blazor.Cli</span></span></code></pre></div><p>This will install the piral-blazor-server tool in the standard binary directory. Now you should be able to use it already.</p><p>Now, you can use the <code>publish-microfrontend</code> command:</p><div class="language-sh vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">sh</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">piral-blazor-server</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> publish-microfrontend</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> --source</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> ./SomeMf</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> --url</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> https://feed.piral.cloud/api/v1/nuget/myfeed/index.json</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> --key</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> abcdef1234</span></span></code></pre></div><p>This would publish the micro frontend contained in the <code>./SomeMf</code> directory to the <code>myfeed</code> feed of the publicly available community edition of the Piral Cloud Feed Service.</p><h2 id="publishing-using-the-dotnet-cli" tabindex="-1">Publishing using the <code>dotnet</code> CLI <a class="header-anchor" href="#publishing-using-the-dotnet-cli" aria-label="Permalink to &quot;Publishing using the `dotnet` CLI&quot;">​</a></h2><p>Alternatively, either use the <code>nuget</code> or <code>dotnet</code> tool to publish the NuGet package:</p><div class="language-sh vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">sh</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">dotnet</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> nuget</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> push</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> SomeMf.nupkg</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> --api-key</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> abcdef1234</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> --source</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> https://feed.piral.cloud/api/v1/nuget/myfeed/index.json</span></span></code></pre></div><p>This works almost exactly the same as the <code>piral-blazor-server</code> tool, however, it expects you to have the build and pack command applied separately / beforehand.</p><h2 id="publishing-using-visual-studio" tabindex="-1">Publishing using Visual Studio <a class="header-anchor" href="#publishing-using-visual-studio" aria-label="Permalink to &quot;Publishing using Visual Studio&quot;">​</a></h2><p>Finally, you can also publish a micro frontend using the <a href="https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli" target="_blank" rel="noreferrer">Publish NuGet package</a> feature of Visual Studio.</p><p>For this to work you need to have configured a special NuGet feed in Visual Studio using the URL and credentials that you&#39;ve set up for your micro frontends feed in the Piral Cloud Feed Service.</p>',16),l=[n];function o(h,r,d,p,u,c){return s(),i("div",null,l)}const b=e(t,[["render",o]]);export{k as __pageData,b as default};