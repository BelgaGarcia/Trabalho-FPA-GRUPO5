/**
 * api.ts — Cliente da API do "Sincronizador de Padrões" (LCS)
 * ---------------------------------------------------------------------------
 * Trabalho de Fundamentos de Projeto e Análise de Algoritmos (FPAA) — Grupo 5.
 *
 * Responsabilidade de Lucas (Frontend): apenas CONSUMIR a API e
 * RENDERIZAR os resultados. O cálculo (validação, tabela de PD, enumeração e
 * backtracking) é feito no backend Flask + C# pelos demais integrantes.
 *
 * Contrato definido aqui e documentado em `frontend/README.md` para alinhamento
 * com o Integrante 6 (API).
 *
 * Endpoint:  POST /api/sincronizar
 * Health:    GET  /api/health
 *
 * As chamadas usam caminhos relativos (/api/...). Em desenvolvimento, o
 * `next.config.ts` faz proxy de /api/* para o servidor Flask, evitando CORS.
 * ---------------------------------------------------------------------------
 */

export type Metodo = "dp" | "backtracking";

/** Corpo enviado para POST /api/sincronizar. */
export interface SincronizarRequest {
  helena: string;
  marcus: string;
  metodo: Metodo;
}

/**
 * Contrato LIMPO consumido pelos componentes do front.
 *
 * - `comprimento`: comprimento da maior subsequência comum.
 * - `tabela`: matriz de PD (|helena|+1) x (|marcus|+1) preenchida pelo backend.
 * - `subsequencias`: todas as LCS distintas, em ordem alfabética. Pode vir
 *   vazia quando `metodo === "dp"` (apenas comprimento).
 *
 * Este tipo é o resultado JÁ normalizado por `sincronizar()`. A forma crua que o
 * núcleo C# devolve (ver `RawSincronizarResponse`) usa nomes diferentes
 * (`comprimento_maximo`, `motivo`); a normalização acontece em um único lugar.
 */
export interface SincronizarResponse {
  status: "ok";
  helena: string;
  marcus: string;
  metodo: Metodo;
  comprimento: number;
  tabela: number[][];
  subsequencias: string[];
}

/**
 * Forma CRUA da resposta de POST /api/sincronizar, exatamente como o núcleo C#
 * (via Flask) a serializa. O front aceita ambos os nomes de campo para o
 * comprimento e para a mensagem de erro, mantendo a interface tolerante a
 * pequenas variações do backend.
 */
interface RawSincronizarResponse {
  status: string;
  helena?: string;
  marcus?: string;
  metodo?: string;
  comprimento?: number;
  comprimento_maximo?: number;
  tabela?: number[][];
  subsequencias?: string[];
  erro?: string;
  motivo?: string;
}

export interface ValidationResult {
  valid: boolean;
  message?: string;
}

/** Erro lançado quando a comunicação com a API falha. */
export class ApiError extends Error {}

/** Normaliza a entrada do usuário para o alfabeto válido (a–z, minúsculas). */
export function sanitize(value: string): string {
  return value.toLowerCase().replace(/[^a-z]/g, "");
}

/**
 * Validação LEVE, apenas para feedback imediato na interface.
 * A validação oficial (incluindo D e mensagens detalhadas) é do Integrante 2,
 * no backend.
 */
export function validateSequence(value: string, label: string): ValidationResult {
  if (value.length === 0) {
    return { valid: false, message: `A sequência de ${label} está vazia.` };
  }
  if (value.length > 80) {
    return { valid: false, message: `A sequência de ${label} excede 80 letras.` };
  }
  if (!/^[a-z]+$/.test(value)) {
    return {
      valid: false,
      message: `A sequência de ${label} deve conter apenas letras de 'a' a 'z'.`,
    };
  }
  return { valid: true };
}

/** Verifica se o servidor Flask está respondendo. */
export async function checkHealth(signal?: AbortSignal): Promise<boolean> {
  try {
    const res = await fetch("/api/health", { signal, cache: "no-store" });
    return res.ok;
  } catch {
    return false;
  }
}

/** Envia as sequências para a API e devolve o resultado já tipado. */
export async function sincronizar(
  payload: SincronizarRequest,
): Promise<SincronizarResponse> {
  let res: Response;
  try {
    res = await fetch("/api/sincronizar", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
      cache: "no-store",
    });
  } catch {
    throw new ApiError(
      "Não foi possível conectar ao servidor. Verifique se o Flask está em execução.",
    );
  }

  let raw: RawSincronizarResponse;
  try {
    raw = (await res.json()) as RawSincronizarResponse;
  } catch {
    throw new ApiError("A resposta do servidor não é um JSON válido.");
  }

  // O backend sinaliza falha com status "erro" (mensagem em "erro" ou "motivo")
  // ou "indisponivel" (núcleo C# ainda não compilado).
  if (!res.ok || raw.status === "erro" || raw.status === "indisponivel") {
    throw new ApiError(
      raw.erro ?? raw.motivo ?? `Erro do servidor (HTTP ${res.status}).`,
    );
  }

  // Normaliza a forma crua do núcleo C# (comprimento_maximo) para o contrato
  // limpo usado pelos componentes (comprimento). Defaults seguros evitam que a
  // interface quebre caso algum campo não venha (ex.: modo "dp" parcial).
  return {
    status: "ok",
    helena: raw.helena ?? payload.helena,
    marcus: raw.marcus ?? payload.marcus,
    metodo: payload.metodo,
    comprimento: raw.comprimento ?? raw.comprimento_maximo ?? 0,
    tabela: raw.tabela ?? [],
    subsequencias: raw.subsequencias ?? [],
  };
}
