// 简化自 TLEFrontend 的官方模板配置。
// 刻意去掉了 sass / css-loader / MiniCssExtractPlugin / CSSPresencePlugin：
// 面板全部使用内联样式，少一层构建依赖就少一处出错的地方。
const path = require("path");
const MOD = require("./mod.json");
const TerserPlugin = require("terser-webpack-plugin");

const OUTPUT_DIR = "./dist/";

module.exports = {
  mode: "production",
  stats: "minimal",
  entry: {
    [MOD.id]: "./src/index.tsx",
  },
  // 这些模块由游戏在运行时提供，不能打进产物里
  externalsType: "window",
  externals: {
    react: "React",
    "react-dom": "ReactDOM",
    "cs2/modding": "cs2/modding",
    "cs2/api": "cs2/api",
    "cs2/bindings": "cs2/bindings",
    "cs2/l10n": "cs2/l10n",
    "cs2/ui": "cs2/ui",
    "cs2/input": "cs2/input",
    "cs2/utils": "cs2/utils",
    "cohtml/cohtml": "cohtml/cohtml",
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
    modules: ["node_modules", path.join(__dirname, "src")],
    alias: {
      "@": path.resolve(__dirname, "src"),
      "mod.json": path.resolve(__dirname, "mod.json"),
    },
  },
  output: {
    path: path.resolve(__dirname, OUTPUT_DIR),
    library: { type: "module" },
    publicPath: "coui://ui-mods/",
  },
  optimization: {
    minimize: true,
    minimizer: [new TerserPlugin()],
  },
  experiments: {
    outputModule: true,
  },
};
