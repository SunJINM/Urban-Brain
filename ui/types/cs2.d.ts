// 游戏提供的模块的最小类型声明。
//
// 官方 toolchain 能生成完整的 .d.ts（在 ui/ 目录跑 `npm run update`，
// 也就是 npx create-csii-ui-mod update）。那套类型更准确，拿到之后
// 可以删掉这个文件。这里只是让工程在没有官方类型时也能编译。

declare module "cs2/modding" {
  export interface ModuleRegistry {
    append(anchor: string, render: () => JSX.Element): void;
    extend(path: string, render: unknown): void;
  }
  export type ModRegistrar = (moduleRegistry: ModuleRegistry) => void;
}

declare module "cs2/api" {
  export interface ValueBinding<T> {
    readonly value: T;
    subscribe(callback: (value: T) => void): { dispose(): void };
  }
  export function bindValue<T>(group: string, name: string, fallback?: T): ValueBinding<T>;
  export function useValue<T>(binding: ValueBinding<T>): T;
  export function trigger(group: string, name: string, ...args: unknown[]): void;
}

declare module "cohtml/cohtml" {
  const engine: {
    call(name: string, ...args: unknown[]): Promise<unknown>;
    on(event: string, callback: (...args: unknown[]) => void): void;
    off(event: string, callback: (...args: unknown[]) => void): void;
  };
  export default engine;
}
