"use client";

import { useState } from "react";
import { ListTree, Ruler } from "lucide-react";
import { toast } from "sonner";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { CheckIcon } from "@/components/ui/check";
import { CopyIcon } from "@/components/ui/copy";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import type { SincronizarResponse } from "@/lib/api";

interface ResultsPanelProps {
  result: SincronizarResponse;
  /** Quando falso, apenas o comprimento é mostrado (modo "apenas DP"). */
  showSubsequences: boolean;
}

/** Painel com o comprimento da LCS e a lista de subsequências recebidas da API. */
export function ResultsPanel({ result, showSubsequences }: ResultsPanelProps) {
  const [copied, setCopied] = useState(false);

  const copyAll = async () => {
    await navigator.clipboard.writeText(result.subsequencias.join("\n"));
    setCopied(true);
    toast.success("Subsequências copiadas para a área de transferência.");
    setTimeout(() => setCopied(false), 1500);
  };

  return (
    <div className="space-y-5">
      <div className="grid grid-cols-2 gap-3">
        <Stat
          icon={<Ruler className="size-4" />}
          label="Comprimento da LCS"
          value={result.comprimento}
        />
        <Stat
          icon={<ListTree className="size-4" />}
          label="Subsequências distintas"
          value={showSubsequences ? result.subsequencias.length : "—"}
        />
      </div>

      {showSubsequences ? (
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <h3 className="text-sm font-medium">
              Maiores subsequências comuns
              <span className="ml-1.5 text-muted-foreground">
                (ordem alfabética)
              </span>
            </h3>
            <Button
              variant="ghost"
              size="sm"
              onClick={copyAll}
              disabled={result.subsequencias.length === 0}
            >
              {copied ? <CheckIcon size={16} /> : <CopyIcon size={16} />}
              Copiar
            </Button>
          </div>
          <div className="flex flex-wrap gap-2">
            {result.subsequencias.map((s) => (
              <Badge
                key={s}
                variant="secondary"
                className="font-mono text-sm tracking-wider"
              >
                {s}
              </Badge>
            ))}
          </div>
        </div>
      ) : (
        <Alert>
          <ListTree />
          <AlertTitle>Modo apenas programação dinâmica</AlertTitle>
          <AlertDescription>
            Esta versão calcula somente o comprimento da LCS. Para listar todas
            as subsequências, selecione &ldquo;DP + Backtracking&rdquo;.
          </AlertDescription>
        </Alert>
      )}
    </div>
  );
}

function Stat({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode;
  label: string;
  value: React.ReactNode;
}) {
  return (
    <div className="rounded-lg border bg-card p-4">
      <div className="flex items-center gap-2 text-muted-foreground">
        {icon}
        <span className="text-xs font-medium">{label}</span>
      </div>
      <p className="mt-2 font-mono text-3xl font-semibold tabular-nums">
        {value}
      </p>
    </div>
  );
}
