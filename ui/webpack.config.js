const path = require("path");
const MOD = require("./mod.json");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const { CSSPresencePlugin } = require("./tools/css-presence");

const OUTPUT_DIR = "./dist/";

const banner = `
 * Cities: Skylines II UI Module
 *
 * Id: ${MOD.id}
 * Author: ${MOD.author}
 * Version: ${MOD.version}
`;

module.exports = {
  mode: "production",
  stats: "none",
  entry: {
    [MOD.id]: "./src/index.tsx",
  },
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
      {
        test: /\.s?css$/,
        include: path.join(__dirname, "src"),
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: "css-loader",
            options: {
              url: true,
              importLoaders: 1,
              modules: {
                auto: true,
                exportLocalsConvention: "camelCase",
                localIdentName: "[local]_[hash:base64:3]",
              },
            },
          },
          "sass-loader",
        ],
      },
      {
        test: /\.(png|jpe?g|gif|svg)$/i,
        type: "asset/resource",
        generator: {
          filename: "traffic-images/[name][ext][query]",
        },
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
    //
    // 注意：这里刻意不把 src 加进 resolve.modules。
    //
    // 两个前端合并到同一工程后，src/tle 与 src/traffic 下都有 components 目录，
    // 若让 src 参与模块解析，"components/x" 会解析到哪一边取决于顺序，很容易出错。
    //
    // 好在 TLE 前端全部使用相对导入，只有 Traffic 前端用了非相对导入
    // （types / modUI / bindings / debugUi / components / helpers / images 七个顶层目录），
    // 所以这里用 alias 把这七个名字精确钉到 traffic 子树，互不干扰。
    // tsconfig.json 的 paths 必须与此保持一致，否则 ts-loader 与 webpack 会解析到不同文件。
    //
    alias: {
      "mod.json": path.resolve(__dirname, "mod.json"),
      // TLE 前端内部用 "@/xxx" 引用自身模块，原工程里 @ 指向 src，
      // 并入后它整体位于 src/tle，因此这里指向 src/tle。
      "@": path.resolve(__dirname, "src/tle"),
      types: path.resolve(__dirname, "src/traffic/types"),
      modUI: path.resolve(__dirname, "src/traffic/modUI"),
      bindings: path.resolve(__dirname, "src/traffic/bindings"),
      debugUi: path.resolve(__dirname, "src/traffic/debugUi"),
      components: path.resolve(__dirname, "src/traffic/components"),
      helpers: path.resolve(__dirname, "src/traffic/helpers"),
      images: path.resolve(__dirname, "src/traffic/images"),
    },
  },
  output: {
    path: path.resolve(__dirname, OUTPUT_DIR),
    filename: "[name].mjs",
    library: {
      type: "module",
    },
    publicPath: `coui://ui-mods/`,
  },
  optimization: {
    minimize: true,
    minimizer: [
      new TerserPlugin({
        terserOptions: {
          format: {
            comments: /^\**!|@preserve|@license|@cc_on/i,
          },
        },
        extractComments: {
          banner: () => banner,
        },
      }),
    ],
  },
  experiments: {
    outputModule: true,
  },
  plugins: [
    new MiniCssExtractPlugin(),
    new CopyPlugin({
      patterns: [
        { from: "src/traffic/images/crowdin-icon-white.svg", to: "traffic-images/" },
        { from: "src/traffic/images/traffic_icon.svg", to: "traffic-images/" },
      ],
    }),
    new CSSPresencePlugin(),
  ],
};
