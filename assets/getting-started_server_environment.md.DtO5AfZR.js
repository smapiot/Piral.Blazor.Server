import{_ as s,c as i,o as a,a2 as n}from"./chunks/framework.a4z-mAWu.js";const E=JSON.parse('{"title":"Environment Configuration","description":"","frontmatter":{},"headers":[],"relativePath":"getting-started/server/environment.md","filePath":"getting-started/server/environment.md","lastUpdated":1715296913000}'),e={name:"getting-started/server/environment.md"},t=n(`<h1 id="environment-configuration" tabindex="-1">Environment Configuration <a class="header-anchor" href="#environment-configuration" aria-label="Permalink to &quot;Environment Configuration&quot;">​</a></h1><p>Runtime configuration works different for micro frontends. This is mostly true due to the fact that micro frontends are distributed, whereas the classic way is a configuration found at a central location such as a file or a some environment variables at a single server.</p><p>Nevertheless, at the end all configuration (like the rest of the code) has to end up in the application. Therefore, the configuration is assumed to be coming <em>with</em> the micro frontend. For this we rely on the response from the micro frontend discovery service (e.g., the Piral Cloud Feed Service) to also bring in configuration.</p><p>The Piral framework takes care of understanding the discovery service response and dispatching the delivered configuration to the micro frontend. In the micro frontend you can obtain the configuration via the <code>IMfService</code> or <code>IMfAppService</code> instances, e.g., passed into the setup lifecycle:</p><div class="language-cs vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">cs</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Module</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IMfModule</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">{</span></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">    public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Setup</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IMfAppService</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> app</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    {</span></span>
<span class="line"><span style="--shiki-light:#6A737D;--shiki-dark:#6A737D;">        // use: app.Meta.Config</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    }</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    </span></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">    public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Teardown</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IMfAppService</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> app</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    {</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    }</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    </span></span>
<span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">    public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Configure</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IServiceCollection</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> services</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    {</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">    }</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">}</span></span></code></pre></div><p>As the discovery service might set the configuration for published micro frontends, the debug issue for local development still exists. How do you get in configuration in here?</p><p>To make local development work you can add a <code>config.json</code> file to your micro frontend, which has to be copied to the output directory. This file can now contain a valid configuration as JSON:</p><div class="language-json vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">json</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">{</span></span>
<span class="line"><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">    &quot;foo&quot;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">: </span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">&quot;bar&quot;</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">}</span></span></code></pre></div><p>This file is automatically picked up by the emulator when you start debugging your micro frontend.</p>`,9),l=[t];function h(p,o,r,k,d,c){return a(),i("div",null,l)}const u=s(e,[["render",h]]);export{E as __pageData,u as default};