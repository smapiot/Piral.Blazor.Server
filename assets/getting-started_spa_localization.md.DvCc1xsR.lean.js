import{_ as s,c as a,a0 as t,o as n}from"./chunks/framework.B-P3EkDQ.js";const g=JSON.parse('{"title":"Localization","description":"","frontmatter":{},"headers":[],"relativePath":"getting-started/spa/localization.md","filePath":"getting-started/spa/localization.md","lastUpdated":1733327392000}'),e={name:"getting-started/spa/localization.md"};function l(h,i,p,k,r,o){return n(),a("div",null,i[0]||(i[0]=[t(`<h1 id="localization" tabindex="-1">Localization <a class="header-anchor" href="#localization" aria-label="Permalink to &quot;Localization&quot;">​</a></h1><p>Localization works almost exactly as with standard Blazor, except that the language can be changed at runtime directly rather then requiring a full reload of the page.</p><p>The other difference is that the initial language is no longer decided by the server&#39;s response headers, but rather by the app shell. The initial configuration options of the <code>piral-blazor</code> plugin allow setting the <code>initialLanguage</code>. These options also allow setting up a callback to decide when to change the language (and to what language). If not explicitly stated Blazor will just listen to the <code>select-language</code> event of Piral, providing a key <code>currentLanguage</code> in the event arguments.</p><p>To dynamically change / refresh your components when the language change you&#39;ll need to listen to the <code>LanguageChanged</code> event emitted by the injected <code>IPiletService</code> instance:</p><div class="language-razor vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">razor</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">@inject</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IStringLocalizer</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">MyComponent</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">loc</span></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">@inject</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IPiletService</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> pilet</span></span>
<span class="line"></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#22863A;--shiki-dark:#85E89D;">h2</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">@</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">loc[</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">&quot;greeting&quot;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">]&lt;/</span><span style="--shiki-light:#22863A;--shiki-dark:#85E89D;">h2</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;</span></span>
<span class="line"></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">@code</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> {</span></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">    protected</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> OnInitialized</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    {</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">        pilet.LanguageChanged </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">+=</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> (</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">s</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">e</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">) </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">=&gt;</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> this</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">.</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">StateHasChanged</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">();</span></span>
<span class="line"><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">        base</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">.</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">OnInitialized</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">();</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    }</span></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">}</span></span></code></pre></div><p>This way, your components will always remain up-to-date and render the right translations.</p>`,6)]))}const c=s(e,[["render",l]]);export{g as __pageData,c as default};