module.exports = {
  publicPath: './',
  outputDir: 'dist\\SelfOrder',
  devServer: {
    https: true,
    port: 8080,
    disableHostCheck: true,
    proxy: {
      '/api': 
      {
        target: 'http://localhost:61207/option/self_order',
        changeOrigin: true,
        pathRewrite: { "^/api": "" },
      }
    }
  },
  configureWebpack: {
    devtool: 'source-map'
  },
  pages: {
    index: {
      entry: 'src/main.ts',
      title: 'SelfOrder',
    },
  }
}
