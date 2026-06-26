from pathlib import Path

from flask import Flask, jsonify, request, send_from_directory

from src.csharp_adapter import executar_nucleo

BASE_DIR = Path(__file__).resolve().parent
FRONTEND_DIR = BASE_DIR / "frontend"

app = Flask(__name__, static_folder=str(FRONTEND_DIR), static_url_path="")


@app.route("/")
def index():
    return send_from_directory(FRONTEND_DIR, "index.html")


@app.route("/api/health")
def health():
    return jsonify({"status": "ok", "app": "Sincronizador de Padroes"})


@app.route("/api/sincronizar", methods=["POST"])
def sincronizar():
    dados = request.get_json(silent=True)

    if not dados:
        return jsonify({"erro": "O corpo da requisicao precisa ser JSON."}), 400

    helena = (dados.get("helena") or "").strip()
    marcus = (dados.get("marcus") or "").strip()
    metodo = (dados.get("metodo") or "dp").strip()

    if not helena:
        return jsonify({"erro": "A sequencia de Helena nao pode estar vazia."}), 400

    if not marcus:
        return jsonify({"erro": "A sequencia de Marcus nao pode estar vazia."}), 400

    resultado = executar_nucleo(
        {
            "helena": helena,
            "marcus": marcus,
            "metodo": metodo,
        }
    )

    if resultado.get("status") == "indisponivel":
        return jsonify(resultado), 503

    if resultado.get("status") == "erro":
        return jsonify(resultado), 500

    return jsonify(resultado)


if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5001, debug=True)
