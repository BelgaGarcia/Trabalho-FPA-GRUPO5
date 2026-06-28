# Sincronizador de Padroes

Projeto da disciplina de Fundamentos de Projeto e Analise de Algoritmos.

## Objetivo

Comparar as sequencias de eventos de Helena e Marcus e encontrar todas as
maiores subsequencias comuns (LCS), respeitando a ordem dos eventos.

## Estado atual

Esta main contem:

- validacao Python das entradas (OK)
- tabela de programacao dinamica em C# (OK)
- enumeracao DP iterativa em C# (OK)
- backtracking recursivo em C# (OK)
- API Flask com `/api/health` e `/api/sincronizar` (OK)
- frontend integrado com a API, resultados e tabela DP (OK)
- testes C# da DP base, enumeracao DP e backtracking

## Como executar

Requisitos:

- Python 3
- .NET 8 SDK

```powershell
dotnet build csharp/FpaaLcs.Core/FpaaLcs.Core.csproj
pip install -r requirements.txt
python server.py
```

Abra `http://127.0.0.1:5000`.

## Como testar

```powershell
dotnet test csharp/FpaaLcs.Core.Tests/FpaaLcs.Core.Tests.csproj
python -m py_compile server.py src/validacao.py src/csharp_adapter.py
dotnet build csharp/FpaaLcs.Core/FpaaLcs.Core.csproj
```

Teste manual da API:

```powershell
Invoke-RestMethod http://127.0.0.1:5000/api/health

Invoke-RestMethod `
  -Method Post `
  -ContentType "application/json" `
  -Body '{"helena":"ijkijkii","marcus":"ikjikji","metodo":"dp"}' `
  http://127.0.0.1:5000/api/sincronizar
```

O caso do roteiro deve retornar comprimento maximo `5`, quantidade `7`, tabela
DP e as subsequencias:

```text
ijiji
ijiki
ijkji
ikiji
ikiki
ikjii
ikjki
```

## API

### `GET /api/health`

Retorna o status basico do servidor Flask.

### `POST /api/sincronizar`

Entrada:

```json
{
  "helena": "ijkijkii",
  "marcus": "ikjikji",
  "metodo": "dp"
}
```

`metodo` aceita `dp` ou `backtracking`.

Resposta de sucesso:

```json
{
  "helena": "ijkijkii",
  "marcus": "ikjikji",
  "comprimentoMaximo": 5,
  "quantidade": 7,
  "padroes": ["ijiji", "ijiki"],
  "algoritmo": "Enumeracao DP",
  "tabelaDp": [[0]]
}
```

Erros de validacao retornam HTTP 400 com:

```json
{
  "erro": "mensagem"
}
```

## Organizacao

- `server.py`: servidor Flask e rotas da API
- `frontend/`: interface web
- `src/validacao.py`: validacao das entradas
- `src/csharp_adapter.py`: chamada do Python para o executavel C#
- `csharp/FpaaLcs.Core/`: nucleo em C# com DP, enumeracao e backtracking
- `csharp/FpaaLcs.Core.Tests/`: testes do nucleo C#
