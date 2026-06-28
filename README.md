# Sincronizador de Padroes

Projeto inicial da disciplina de Fundamentos de Projeto e Analise de Algoritmos.

## Perguntas avaliativas (LCS)

> Nas respostas abaixo, **n = |Helena|**, **m = |Marcus|**, **L** = comprimento da
> maior subsequência comum e **k** = número de LCS distintas encontradas.

### 1. Como a programação dinâmica foi aplicada na solução?

O problema é encontrar a(s) **maior(es) subsequência(s) comum(ns)** (LCS) entre as
sequências de Helena e Marcus, preservando a ordem dos eventos. A programação
dinâmica é aplicada construindo uma tabela `L` de tamanho `(n+1) x (m+1)`, onde
`L[i][j]` é o comprimento da LCS entre os prefixos `Helena[0..i-1]` e
`Marcus[0..j-1]` (implementação em `LcsDpTable.Montar`).

A recorrência é:

- **Base:** `L[i][0] = L[0][j] = 0` — um prefixo vazio gera LCS vazia.
- **Caracteres iguais** (`Helena[i-1] == Marcus[j-1]`):
  `L[i][j] = L[i-1][j-1] + 1` — o caractere coincidente estende a LCS dos prefixos
  menores.
- **Caracteres diferentes:** `L[i][j] = max(L[i-1][j], L[i][j-1])` — descarta-se um
  caractere de uma das sequências, mantendo o melhor resultado.

O preenchimento é **bottom-up** (`i` de 1 a n, `j` de 1 a m), o que garante que cada
subproblema seja resolvido uma única vez e reaproveitado — explorando as duas
propriedades que justificam DP: **subestrutura ótima** e **subproblemas
sobrepostos**. O valor `L[n][m]` é o comprimento da LCS, e a tabela completa é
exatamente o que a interface renderiza.

### 2. Por que o uso de backtracking é necessário neste problema?

A tabela DP fornece o **comprimento** da LCS, mas não as subsequências em si — e
pode haver **várias LCS distintas** de mesmo comprimento máximo, sempre que ocorrem
empates (`L[i-1][j] == L[i][j-1]`). Para **recuperar as strings** — e, em especial,
para listar **todas** sem omissões — é preciso percorrer a tabela de volta, de
`(n, m)` até `(0, 0)`:

- Em uma **coincidência** (diagonal), o caractere pertence à LCS; vai-se para
  `(i-1, j-1)`.
- Em uma **divergência**, segue-se a(s) direção(ões) que preservam o valor ótimo;
  quando "cima" e "esquerda" empatam, é necessário **ramificar para ambos** — e é
  justamente nessa ramificação que surgem as múltiplas LCS.

A DP, sozinha (apenas para frente), não enumera essas ramificações. O backtracking
— recursivo em `LcsBacktracker` ou iterativo com pilha explícita em
`LcsDpEnumerator` — é o que explora cada caminho ótimo. Um `HashSet<string>`
remove duplicatas (a mesma string pode ser alcançada por caminhos diferentes) e a
ordenação `Ordinal` garante saída determinística em ordem alfabética. Portanto, o
backtracking é necessário para sair do "qual o tamanho" e chegar em "quais são,
todas elas".

### 3. Houve desafios na implementação? Quais? Como foram superados?

- **Múltiplas LCS e duplicatas:** a mesma subsequência pode ser alcançada por
  caminhos diferentes na tabela. Resolvido com `HashSet<string>` para deduplicar e
  ordenação `Ordinal` para uma saída determinística e em ordem alfabética.
- **Explosão combinatória:** enumerar todos os caminhos ótimos é exponencial no
  pior caso. Mitigado no backtracking com **memoização**
  (`Dictionary<(i,j), HashSet<string>>`), que reaproveita os subconjuntos já
  calculados, e oferecendo a opção de obter **apenas o comprimento** em `O(n·m)`
  quando não se precisa listar tudo.
- **Recursão vs. iteração:** o backtracking recursivo pode estourar a pilha em
  sequências longas; por isso há também uma versão **iterativa** com pilha explícita
  (`LcsDpEnumerator`).
- **Integração frontend/backend:** branches diferentes adotaram contratos de JSON
  divergentes (`camelCase`: `tabelaDp`, `padroes`, `comprimentoMaximo` vs.
  `snake_case`: `tabela`, `subsequencias`, `comprimento_maximo`). Superado tornando o
  frontend **tolerante a ambos** (normalização da resposta) e mantendo a validação
  exclusivamente no backend.
- **Execução multiplataforma:** diferenças no nome do executável C# (com/sem `.exe`)
  e alinhamento de portas entre os servidores, tratados na camada de
  adaptação/execução.

### 4. Qual é a complexidade da solução proposta?

**Construção da tabela DP (comum às duas versões):**

1. Alocar e zerar a tabela `(n+1) x (m+1)` → `O(n·m)` de espaço.
2. Dois laços aninhados, `i = 1..n` e `j = 1..m` → `n·m` iterações.
3. Cada célula faz uma comparação e uma atribuição em `O(1)`.

→ **Tempo `O(n·m)`, Espaço `O(n·m)`.**

#### (a) Versão utilizando apenas programação dinâmica

1. Construir a tabela: `O(n·m)`.
2. Comprimento da LCS = `L[n][m]`: leitura em `O(1)`.

→ Para obter **apenas o comprimento**: **`O(n·m)`**.

3. Para **listar todas** as LCS (enumeração iterativa por pilha em
   `LcsDpEnumerator`): percorre-se todos os caminhos ótimos; cada caminho tem no
   máximo `n + m` passos e copia o caminho parcial em `O(L)`. O número de caminhos
   (e de LCS distintas `k`) é, no pior caso, **exponencial** (até `~2^min(n,m)`
   quando há muitos empates). Deduplicação `O(k·L)` + ordenação `O(k·L·log k)`.

→ **`O(n·m)` para o comprimento; `O(n·m + k·L·log k)` para listar todas — exponencial
no pior caso, limitado inferiormente pelo próprio tamanho da saída.**

#### (b) Versão que combina programação dinâmica com backtracking

1. Construir a tabela: `O(n·m)`.
2. Backtracking a partir de `(n, m)` (`LcsBacktracker`): em coincidência, segue a
   diagonal; em empate, ramifica para cima e/ou esquerda. Com **memoização**, cada
   célula `(i, j)` calcula seu conjunto de subsequências uma única vez (há `O(n·m)`
   células).
3. Materializar as strings: concatenar o caractere a cada prefixo custa
   `O(|conjunto|·L)` por célula; agregado `O(k·L)` no pior caso.
4. Deduplicação (`HashSet`) + ordenação `Ordinal`: `O(k·L·log k)`.

→ **`O(n·m + k·L·log k)`; no melhor/típico (LCS única) `~ O(n·m + (n+m))`; pior caso
exponencial em `k`.**

**Observação:** a parte de **programação dinâmica** (a tabela e o comprimento) é
sempre **polinomial `O(n·m)`**. A explosão só aparece quando se exige **enumerar
todas** as subsequências, porque a quantidade de respostas distintas pode crescer
exponencialmente — isso é um **limite inferior pelo tamanho da saída**, não uma
ineficiência do algoritmo. Para `D` conjuntos (`D ≤ 10`, constante), multiplica-se
por `D`: `O(D·n·m) = O(n·m)`.

### 5. O que o grupo aprendeu ao resolver esse problema?

- Modelar um problema com **subestrutura ótima** e **subproblemas sobrepostos**, e
  por que isso justifica DP (preencher uma tabela bottom-up em vez de uma recursão
  ingênua exponencial).
- A diferença entre **calcular** um valor ótimo (o comprimento) e
  **reconstruir/enumerar** as soluções (as subsequências), e como o backtracking
  complementa a DP nessa segunda etapa.
- **Separar responsabilidades em camadas** (núcleo de algoritmo em C#, validação no
  backend, frontend apenas de apresentação), o que facilitou o trabalho em branches
  paralelas e a integração de contratos diferentes.
- A importância de **saída determinística** (deduplicação + ordenação) e de
  **testes** cobrindo a DP base, a enumeração iterativa e o backtracking.
- Fazer uma **análise de complexidade realista**: reconhecer quando o custo é
  polinomial (a tabela) e quando é inerentemente exponencial (a enumeração), e
  oferecer a versão de só-comprimento quando a listagem completa não é necessária.

## Estado atual da main

Esta main foi estabilizada para voltar a ser uma base minima de colaboracao.
Ela contem:

- estrutura inicial do projeto
- servidor Flask basico
- frontend estatico inicial
- validacao Python isolada (OK)
- DP base em C# com tabela e comprimento maximo (OK)
- enumeracao DP iterativa em C# (OK)
- adapter Python inicial para integracao futura com C#
- nucleo C# com backtracking isolado (OK)
- testes C# da DP base, da enumeracao DP e do backtracking
- documentacao curta da arquitetura

A rota `/api/sincronizar` ainda nao faz parte da main. Ela deve entrar na
branch do Integrante 6 depois que DP, enumeracao e backtracking estiverem
consolidados.

## Como executar

```powershell
# 1. Compilar o nucleo C# (gera o executavel usado pelo backend)
dotnet build csharp/FpaaLcs.Core

# 2. Instalar dependencias do servidor e inicia-lo
pip install -r requirements.txt
python server.py
```

Abra `http://127.0.0.1:5001`.

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

1. API
2. Frontend e documentacao

## Avisos para as branches

- DP base ja foi absorvida de forma seletiva a partir de `feat/integrante3-dp`.
- Enumeracao DP ja foi absorvida de forma seletiva a partir de `feat/felipe-enumeracao-dp`.
- O Integrante 6 deve integrar a API usando `LcsDpTable`, `LcsDpEnumerator` e `LcsBacktracker`.
- Novas rotas de API devem ser feitas apenas na branch do Integrante 6.
