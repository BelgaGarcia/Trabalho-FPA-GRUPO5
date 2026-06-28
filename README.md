# Sincronizador de Padroes

Projeto inicial da disciplina de Fundamentos de Projeto e Analise de Algoritmos.

## Estado atual da main

Esta main foi estabilizada para voltar a ser uma base minima de colaboracao.
Ela contem:

- estrutura inicial do projeto
- servidor Flask basico
- frontend estatico inicial
- validacao Python isolada (OK)
- DP base em C# com tabela e comprimento maximo (OK)
- adapter Python inicial para integracao futura com C#
- nucleo C# com backtracking isolado (OK)
- testes C# da DP base e do backtracking
- documentacao curta da arquitetura

A rota `/api/sincronizar` ainda nao faz parte da main. Ela deve entrar na
branch do Integrante 6 depois que DP, enumeracao e backtracking estiverem
consolidados.

## Como executar

```powershell
pip install -r requirements.txt
python server.py
```

Abra `http://127.0.0.1:5000`.

## Como testar o nucleo C#

Requisitos:

- .NET 8 SDK

```powershell
dotnet test csharp/FpaaLcs.Core.Tests/FpaaLcs.Core.Tests.csproj
```

## Organizacao

- `server.py`: servidor Flask inicial
- `frontend/`: interface estatica inicial
- `src/validacao.py`: validacao de D e dos pares de sequencias
- `src/csharp_adapter.py`: adapter inicial para integracao futura
- `csharp/FpaaLcs.Core/`: nucleo C# inicial
- `csharp/FpaaLcs.Core.Tests/`: testes do backtracking
- `docs/`: notas tecnicas do grupo

## Ordem sugerida de merge

1. Enumeracao DP
2. Ajustes finais do backtracking, se forem necessarios apos a enumeracao
3. API
4. Frontend e documentacao

## Avisos para as branches

- DP base ja foi absorvida de forma seletiva a partir de `feat/integrante3-dp`.
- O Integrante 4 deve usar `LcsDpTable` como base para a enumeracao DP.
- Lucas deve rebasear `feat/lucas-frontend-docs` antes de finalizar frontend e README.
- Felipe deve recriar ou atualizar sua branch a partir desta main estabilizada.
- Novas rotas de API devem ser feitas apenas na branch do Integrante 6.
