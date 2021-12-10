module.exports = {
  publicPath: process.env.NODE_ENV === 'production' ? '/e-table-order/' : '/',
  outputDir: 'dist\\e-table-order',
  devServer: {
    https: true,
    port: 8080,
    disableHostCheck: true
  },
  configureWebpack: {
    devtool: 'source-map'
  }
}
