"use client";

import { cn } from "@/lib/utils";

interface DpTableProps {
  helena: string;
  marcus: string;
  /** Matriz de PD (|helena|+1) x (|marcus|+1) calculada pelo backend. */
  tabela: number[][];
}

/**
 * Renderiza a matriz de programação dinâmica recebida da API.
 * - Cabeçalho superior: sequência de Marcus (colunas).
 * - Cabeçalho lateral: sequência de Helena (linhas).
 * - Células destacadas: posições em que as letras coincidem (helena[i] === marcus[j]).
 *
 * O destaque é apenas comparação de caracteres (detalhe visual); nenhuma lógica
 * de algoritmo é executada aqui — os valores vêm prontos do backend.
 */
export function DpTable({ helena, marcus, tabela }: DpTableProps) {
  const m = helena.length;
  const n = marcus.length;

  // Ajusta o tamanho das células conforme o comprimento das sequências.
  const compact = Math.max(m, n) > 26;
  const cellSize = compact ? "h-7 w-7 text-[10px]" : "h-9 w-9 text-xs";
  // Anima a varredura diagonal apenas em matrizes pequenas (evita jank).
  const animate = m * n <= 600;

  // Colunas: rótulo da linha + (n + 1) colunas da matriz (j = 0..n).
  const gridTemplateColumns = `auto repeat(${n + 1}, minmax(0, 1fr))`;

  const headerChip = (letter: string, tone: "helena" | "marcus") =>
    cn(
      "flex items-center justify-center rounded-md font-mono font-semibold",
      cellSize,
      tone === "helena"
        ? "bg-helena text-helena-foreground"
        : "bg-marcus text-marcus-foreground",
    );

  const epsilonChip = cn(
    "flex items-center justify-center rounded-md font-mono text-muted-foreground bg-muted",
    cellSize,
  );

  return (
    <div className="space-y-3">
      <div className="overflow-auto rounded-lg border bg-card p-3">
        <div
          className="grid w-max gap-1"
          style={{ gridTemplateColumns }}
          role="img"
          aria-label={`Tabela de programação dinâmica ${m + 1} por ${n + 1}.`}
        >
          {/* Linha de cabeçalho: canto + letras de Marcus */}
          <div className={cn("flex items-center justify-center", cellSize)}>
            <span className="text-[10px] font-medium text-muted-foreground">
              H\M
            </span>
          </div>
          {Array.from({ length: n + 1 }, (_, j) => (
            <div
              key={`mh-${j}`}
              className={j === 0 ? epsilonChip : headerChip(marcus[j - 1], "marcus")}
            >
              {j === 0 ? "ε" : marcus[j - 1]}
            </div>
          ))}

          {/* Linhas da matriz */}
          {Array.from({ length: m + 1 }, (_, i) => (
            <Row
              key={`row-${i}`}
              i={i}
              n={n}
              dpRow={tabela[i]}
              helena={helena}
              marcus={marcus}
              cellSize={cellSize}
              animate={animate}
              headerChip={i === 0 ? epsilonChip : headerChip(helena[i - 1], "helena")}
              helenaLetter={i === 0 ? "ε" : helena[i - 1]}
            />
          ))}
        </div>
      </div>

      <Legend />
    </div>
  );
}

interface RowProps {
  i: number;
  n: number;
  helenaLetter: string;
  dpRow: number[];
  helena: string;
  marcus: string;
  cellSize: string;
  animate: boolean;
  headerChip: string;
}

function Row({
  i,
  n,
  dpRow,
  helena,
  marcus,
  cellSize,
  animate,
  headerChip,
  helenaLetter,
}: RowProps) {
  return (
    <>
      <div className={headerChip}>{helenaLetter}</div>
      {Array.from({ length: n + 1 }, (_, j) => {
        const isMatch = i >= 1 && j >= 1 && helena[i - 1] === marcus[j - 1];
        return (
          <div
            key={`c-${i}-${j}`}
            className={cn(
              "flex items-center justify-center rounded-md border font-mono tabular-nums transition-colors",
              cellSize,
              animate && "cell-in",
              isMatch
                ? "border-path bg-path font-bold text-path-foreground"
                : "border-border/60 text-muted-foreground",
            )}
            style={
              animate
                ? { animationDelay: `${Math.min((i + j) * 18, 700)}ms` }
                : undefined
            }
          >
            {dpRow?.[j] ?? 0}
          </div>
        );
      })}
    </>
  );
}

function Legend() {
  return (
    <div className="flex flex-wrap items-center gap-x-4 gap-y-2 text-xs text-muted-foreground">
      <span className="inline-flex items-center gap-1.5">
        <span className="size-3 rounded bg-helena" /> Helena (linhas)
      </span>
      <span className="inline-flex items-center gap-1.5">
        <span className="size-3 rounded bg-marcus" /> Marcus (colunas)
      </span>
      <span className="inline-flex items-center gap-1.5">
        <span className="size-3 rounded bg-path" /> Letras coincidentes
      </span>
    </div>
  );
}
