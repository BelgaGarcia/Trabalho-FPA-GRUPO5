from pathlib import Path

from flask import Flask, jsonify, request, send_from_directory

from src.csharp_adapter import CSharpAdapterErro, calcular_lcs
from src.validacao import ValidacaoErro, validar_caso

BASE_DIR = Path(__file__).resolve().parent
FRONTEND_DIR = BASE_DIR / "frontend"
METODOS_ACEITOS = {"dp", "backtracking"}

app = Flask(__name__, static_folder=str(FRONTEND_DIR), static_url_path="")


@app.route("/")
def index():
    return send_from_directory(FRONTEND_DIR, "index.html")


@app.route("/api/health")
def health():
    return jsonify({"status": "ok", "app": "Sincronizador de Padroes"})


@app.route("/api/sincronizar", methods=["POST"])
def sincronizar():
    dados = request.get_json(silent=True) or {}
    metodo = str(dados.get("metodo", "dp")).strip().lower()

    try:
        helena, marcus = validar_caso(
            str(dados.get("helena", "")),
            str(dados.get("marcus", "")),
        )

        if metodo not in METODOS_ACEITOS:
            raise ValidacaoErro("Metodo deve ser 'dp' ou 'backtracking'.")

        resultado = calcular_lcs(helena, marcus, metodo)
        return jsonify(resultado)
    except ValidacaoErro as erro:
        return jsonify({"erro": str(erro)}), 400
    except CSharpAdapterErro as erro:
        return jsonify({"erro": str(erro)}), 503


if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5000, debug=True)
