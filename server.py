#!/usr/bin/env python3
"""
Servidor da interface grÃ¡fica â€” Sincronizador de Padrões (Helena & Marcus).

Autores: Grupo FPAA â€” PUC Minas Contagem (preencher nomes dos 7â€“8 integrantes)
Versão: 1.0.0
Data: 2026-05-25

Uso:
    pip install -r requirements.txt
    python3 server.py
    Acesse http://127.0.0.1:5000
"""

from pathlib import Path

from flask import Flask, jsonify, request, send_from_directory
from flask_cors import CORS

from src.csharp_dp_adapter import calcular_dp_via_csharp, validar_d_via_csharp
from src.validacao import ValidacaoErro

BASE_DIR = Path(__file__).resolve().parent
FRONTEND_DIR = BASE_DIR / "frontend"

app = Flask(__name__, static_folder=str(FRONTEND_DIR), static_url_path="")
CORS(app)


@app.route("/")
def index():
    return send_from_directory(FRONTEND_DIR, "index.html")


@app.route("/api/health")
def health():
    return jsonify({"status": "ok", "app": "Sincronizador de Padrões FPAA"})


@app.route("/api/sincronizar", methods=["POST"])
def sincronizar():
    dados = request.get_json(silent=True) or {}
    helena = dados.get("helena", "")
    marcus = dados.get("marcus", "")
    metodo = dados.get("metodo", "backtracking")

    try:
        if metodo == "dp":
            dp_resultado = calcular_dp_via_csharp(helena, marcus, metodo="dp")
            tabela = dp_resultado["tabelaDp"]
            comprimento = dp_resultado["comprimentoMaximo"]
            padroes = dp_resultado["padroes"]
            algoritmo = "Programação dinâmica (enumeração iterativa)"
        else:
            dp_resultado = calcular_dp_via_csharp(helena, marcus, metodo="backtracking")
            tabela = dp_resultado["tabelaDp"]
            comprimento = dp_resultado["comprimentoMaximo"]
            padroes = dp_resultado["padroes"]
            algoritmo = "Programação dinâmica + backtracking"

        return jsonify(
            {
                "helena": helena,
                "marcus": marcus,
                "comprimentoMaximo": comprimento,
                "quantidade": len(padroes),
                "padroes": padroes,
                "algoritmo": algoritmo,
                "tabelaDp": tabela,
            }
        )
    except ValidacaoErro as erro:
        return jsonify({"erro": str(erro)}), 400


@app.route("/api/lote", methods=["POST"])
def lote():
    dados = request.get_json(silent=True) or {}
    casos = dados.get("casos", [])
    metodo = dados.get("metodo", "backtracking")

    try:
        d = validar_d_via_csharp(str(len(casos)))
        if len(casos) != d:
            raise ValidacaoErro(f"Informe exatamente {d} pares de sequências.")

        resultados = []
        for par in casos:
            helena = par.get("helena", "")
            marcus = par.get("marcus", "")
            if metodo == "dp":
                dp_resultado = calcular_dp_via_csharp(helena, marcus, metodo="dp")
                comprimento = dp_resultado["comprimentoMaximo"]
                padroes = dp_resultado["padroes"]
            else:
                dp_resultado = calcular_dp_via_csharp(helena, marcus, metodo="backtracking")
                comprimento = dp_resultado["comprimentoMaximo"]
                padroes = dp_resultado["padroes"]
            resultados.append(
                {
                    "helena": helena,
                    "marcus": marcus,
                    "comprimentoMaximo": comprimento,
                    "padroes": padroes,
                }
            )

        return jsonify({"casos": resultados, "total": d})
    except ValidacaoErro as erro:
        return jsonify({"erro": str(erro)}), 400


if __name__ == "__main__":
    app.run(host="127.0.0.1", port=6000, debug=True)
