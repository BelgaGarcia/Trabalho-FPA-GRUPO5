# Arquitetura Inicial

## Visao geral

O projeto comeca com duas camadas:

- Flask para servir a interface e expor o endpoint de calculo
- C# para concentrar o nucleo de processamento da LCS

## Estado desta etapa

- validacao das entradas
- construcao da tabela de programacao dinamica
- calculo do comprimento da maior subsequencia comum
- backtracking sobre a tabela DP
- remocao de subsequencias repetidas
- ordenacao alfabetica das subsequencias

## Observacao sobre a saida

No formato do roteiro, o programa imprime apenas as subsequencias comuns mais longas, uma por linha.
Entre conjuntos diferentes, e impressa uma linha em branco.
