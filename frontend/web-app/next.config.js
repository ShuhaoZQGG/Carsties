/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    domains: [
      "cdn.pixabay.com"
    ]
  },
  typescript: {
    ignoreBuildErrors: true,
 },
  output: "standalone"
}

module.exports = nextConfig
