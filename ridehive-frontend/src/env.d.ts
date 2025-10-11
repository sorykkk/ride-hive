/// <reference types="vite/client" />
// TypeScript does not natively know what a .vue file is. When you import a .vue file, TypeScript would normally throw an error because it doesn't know the type of the imported module.
// The declare module '*.vue' statement tells TypeScript: "Whenever you see an import for a file ending in .vue, treat it as a Vue component."
// It imports DefineComponent from Vue and uses it to type the default export, so TypeScript knows what kind of object is being imported.
declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}