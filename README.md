# Sincronizador de Padroes

Projeto inicial da disciplina de Fundamentos de Projeto e Analise de Algoritmos.

## Objetivo

Comparar as sequencias de Helena e Marcus e imprimir todas as maiores subsequencias comuns.

## Estado atual

Esta versao resolve o problema completo:

- validacao das entradas;
- construcao da tabela de programacao dinamica;
- calculo do comprimento da LCS;
- backtracking sobre a tabela DP;
- remocao de repeticoes;
- ordenacao alfabetica das subsequencias;
- linha em branco entre blocos de saida.

## Como executar

Compile o nucleo C#:

```powershell
dotnet build .\csharp\FpaaLcs.Core\FpaaLcs.Core.csproj
```

Execute com uma entrada no formato do roteiro:

```powershell
dotnet run --project .\csharp\FpaaLcs.Core\FpaaLcs.Core.csproj
```

## Formato da entrada

```text
2
abcbdab
bdcaba
abc
def
```

## Organizacao

- `server.py`: servidor Flask usado na demonstracao
- `frontend/`: interface estatica inicial
- `src/`: suporte Python e integracao com o nucleo C#
- `src/LcsEtapa1/`: versao simples em C# para apresentar o algoritmo
- `csharp/FpaaLcs.Core/`: nucleo C# usado pelo servidor
- `docs/`: notas tecnicas do grupo
