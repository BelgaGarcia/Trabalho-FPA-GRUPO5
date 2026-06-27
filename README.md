# Sincronizador de Padroes

Projeto inicial da disciplina de Fundamentos de Projeto e Analise de Algoritmos.

## Objetivo

Construir uma solucao para comparar as sequencias de Helena e Marcus e encontrar subsequencias comuns respeitando a ordem dos eventos.

## Estado atual

Esta base contem:

- estrutura inicial do projeto
- servidor Flask para demonstracao
- frontend estatico inicial
- estrutura inicial do nucleo C#
- adaptador Python para integracao futura

## Como executar

```powershell
# 1. Compilar o nucleo C# (gera o executavel usado pelo backend)
dotnet build csharp/FpaaLcs.Core

# 2. Instalar dependencias do servidor e inicia-lo
pip install -r requirements.txt
python server.py
```

Abra `http://127.0.0.1:5001`.

## Organizacao

- `server.py`: servidor Flask inicial
- `frontend/`: interface estatica inicial
- `src/`: suporte Python e integracao futura
- `csharp/FpaaLcs.Core/`: nucleo inicial em C#
- `docs/`: notas tecnicas do grupo

