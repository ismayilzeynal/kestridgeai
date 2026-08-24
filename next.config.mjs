/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  // The About page was folded into the single-page site. Keep old links alive.
  async redirects() {
    return [{ source: "/about", destination: "/#company", permanent: true }];
  },
};

export default nextConfig;
