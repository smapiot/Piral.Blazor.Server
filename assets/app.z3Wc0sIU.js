import{t as i}from"./chunks/theme.DgMz8hrq.js";import{j as o,a1 as u,a2 as l,a3 as c,a4 as f,a5 as d,a6 as m,a7 as h,a8 as g,a9 as A,aa as P,d as v,u as y,l as C,x as b,ab as w,ac as E,ad as R,ae as S}from"./chunks/framework.B-P3EkDQ.js";function p(e){if(e.extends){const a=p(e.extends);return{...a,...e,async enhanceApp(t){a.enhanceApp&&await a.enhanceApp(t),e.enhanceApp&&await e.enhanceApp(t)}}}return e}const s=p(i),T=v({name:"VitePressApp",setup(){const{site:e,lang:a,dir:t}=y();return C(()=>{b(()=>{document.documentElement.lang=a.value,document.documentElement.dir=t.value})}),e.value.router.prefetchLinks&&w(),E(),R(),s.setup&&s.setup(),()=>S(s.Layout)}});async function j(){globalThis.__VITEPRESS__=!0;const e=D(),a=x();a.provide(l,e);const t=c(e.route);return a.provide(f,t),a.component("Content",d),a.component("ClientOnly",m),Object.defineProperties(a.config.globalProperties,{$frontmatter:{get(){return t.frontmatter.value}},$params:{get(){return t.page.value.params}}}),s.enhanceApp&&await s.enhanceApp({app:a,router:e,siteData:h}),{app:a,router:e,data:t}}function x(){return g(T)}function D(){let e=o,a;return A(t=>{let n=P(t),r=null;return n&&(e&&(a=n),(e||a===n)&&(n=n.replace(/\.js$/,".lean.js")),r=import(n)),o&&(e=!1),r},s.NotFound)}o&&j().then(({app:e,router:a,data:t})=>{a.go().then(()=>{u(a.route,t.site),e.mount("#app")})});export{j as createApp};