import type { NextConfig } from "next";

// Alvo do servidor Flask. Em desenvolvimento, todas as chamadas a /api/* são
// encaminhadas para o Flask, evitando problemas de CORS entre :3000 e :5001.
// A porta deve casar com a usada em `server.py` (app.run(..., port=5001)).
// Pode ser sobrescrita via variável de ambiente FLASK_URL.
const FLASK_TARGET = process.env.FLASK_URL ?? "http://127.0.0.1:5001";

const nextConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: `${FLASK_TARGET}/api/:path*`,
      },
    ];
  },
};

export default nextConfig;
