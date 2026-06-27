# Frontend — Sincronizador de Padrões (LCS)

Interface gráfica do trabalho de FPAA (Integrante 7). É um **cliente** que envia
as sequências de Helena e Marcus para a API e **renderiza os resultados**:
comprimento da LCS, a **tabela de programação dinâmica** e a lista das maiores
subsequências comuns.

> O cálculo (validação, tabela de PD, enumeração e backtracking) é
> responsabilidade do backend (Integrantes 2 a 6). O frontend **não implementa
> algoritmo** — apenas consome a API e exibe a resposta.

## Stack

- **Next.js 16** (App Router) + **React 19** + **TypeScript**
- **Tailwind CSS v4** + **shadcn/ui**
- Ícones animados **lucide-animated** (motion) + **lucide-react**
- Fontes: **Gabarito** (títulos), **Inter** (texto), **Geist Mono** (dados/tabela)
- **next-themes** (tema claro/escuro)

## Como executar

O frontend depende do servidor **Flask** (que expõe a API). Rode os dois:

```bash
# 1) backend (na raiz do projeto)
pip install -r requirements.txt
python server.py            # http://127.0.0.1:5001

# 2) frontend
cd frontend
npm install
npm run dev                 # http://localhost:3000
```

As chamadas do navegador vão para `/api/*` (mesma origem) e o
[`next.config.ts`](./next.config.ts) faz **proxy** para o Flask, evitando CORS.
O alvo pode ser configurado com a variável `FLASK_URL`.

O **indicador de conexão** (bolinha no cabeçalho) pulsa **verde** quando o Flask
responde em `/api/health` e fica **vermelho** quando está offline.

## Contrato da API (alinhar com o Integrante 6)

### `GET /api/health`

```json
{ "status": "ok", "app": "Sincronizador de Padroes" }
```

### `POST /api/sincronizar`

Requisição:

```json
{ "helena": "ijkijkii", "marcus": "ikjikji", "metodo": "backtracking" }
```

- `metodo`: `"dp"` (apenas comprimento) ou `"backtracking"` (todas as LCS).

Resposta (sucesso):

```json
{
  "status": "ok",
  "helena": "ijkijkii",
  "marcus": "ikjikji",
  "metodo": "backtracking",
  "comprimento": 5,
  "tabela": [[0, 0, 0], [0, 0, 1]],
  "subsequencias": ["ijiji", "ijiki", "ijkji", "ikiji", "ikiki", "ikjii", "ikjki"]
}
```

- `comprimento`: comprimento da maior subsequência comum.
- `tabela`: matriz de PD `(|helena|+1) x (|marcus|+1)`.
- `subsequencias`: todas as LCS distintas, em ordem alfabética. Pode vir vazia
  quando `metodo === "dp"`.

Resposta (erro de validação/processamento):

```json
{ "status": "erro", "erro": "Mensagem amigável de erro." }
```

O cliente trata `status: "erro"` (ou HTTP ≠ 2xx) exibindo a mensagem na interface.

## Estrutura

- `src/lib/api.ts` — tipos do contrato, cliente HTTP e sanitização leve de entrada
- `src/components/sync/` — orquestrador, tabela de PD e painel de resultados
- `src/components/server-status.tsx` — indicador de conexão com o Flask
- `src/components/theme-*.tsx` — tema claro/escuro
- `src/app/` — layout e página principal
