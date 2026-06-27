"use client";

import { useState } from "react";
import { Loader2, TriangleAlert } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { DeleteIcon } from "@/components/ui/delete";
import { FlaskIcon } from "@/components/ui/flask";
import { GitCompareArrowsIcon } from "@/components/ui/git-compare-arrows";
import { RefreshCWIcon } from "@/components/ui/refresh-cw";
import { WorkflowIcon } from "@/components/ui/workflow";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Separator } from "@/components/ui/separator";
import {
  ApiError,
  sanitize,
  sincronizar,
  validateSequence,
  type Metodo,
  type SincronizarResponse,
} from "@/lib/api";
import { DpTable } from "./dp-table";
import { ResultsPanel } from "./results-panel";

const EXAMPLE = { helena: "ijkijkii", marcus: "ikjikji" } as const;
const MAX_LEN = 80;

/**
 * Componente principal: coleta as sequências, envia para a API (/api/sincronizar)
 * e renderiza a resposta (comprimento, tabela de PD e subsequências).
 * O cálculo é responsabilidade do backend; aqui só há entrada e visualização.
 */
export function Synchronizer() {
  const [helena, setHelena] = useState("");
  const [marcus, setMarcus] = useState("");
  const [method, setMethod] = useState<Metodo>("backtracking");
  const [result, setResult] = useState<SincronizarResponse | null>(null);
  const [errors, setErrors] = useState<string[]>([]);
  const [apiError, setApiError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const run = async (metodo: Metodo, hRaw = helena, mRaw = marcus) => {
    const h = sanitize(hRaw);
    const m = sanitize(mRaw);
    const messages = [
      validateSequence(h, "Helena"),
      validateSequence(m, "Marcus"),
    ]
      .filter((v) => !v.valid)
      .map((v) => v.message as string);

    setErrors(messages);
    if (messages.length > 0) {
      setResult(null);
      setApiError(null);
      return;
    }

    setLoading(true);
    setApiError(null);
    try {
      const data = await sincronizar({ helena: h, marcus: m, metodo });
      setResult(data);
    } catch (e) {
      const message =
        e instanceof ApiError ? e.message : "Falha inesperada na sincronização.";
      setResult(null);
      setApiError(message);
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  const handleSync = () => run(method);

  const handleMethodChange = (value: string) => {
    const next = value as Metodo;
    setMethod(next);
    // Re-consulta a API: cada método devolve um conjunto diferente de dados.
    if (result || apiError) run(next);
  };

  const loadExample = () => {
    setHelena(EXAMPLE.helena);
    setMarcus(EXAMPLE.marcus);
    run(method, EXAMPLE.helena, EXAMPLE.marcus);
  };

  const swap = () => {
    const nextHelena = marcus;
    const nextMarcus = helena;
    setHelena(nextHelena);
    setMarcus(nextMarcus);
    if (result) run(method, nextHelena, nextMarcus);
  };

  const clear = () => {
    setHelena("");
    setMarcus("");
    setErrors([]);
    setApiError(null);
    setResult(null);
  };

  return (
    <div className="grid gap-6 lg:grid-cols-12">
      {/* Painel de entrada */}
      <Card className="lg:col-span-4 lg:sticky lg:top-24 lg:self-start h-full">
        <CardHeader>
          <CardTitle className="flex items-center gap-2 font-heading text-base">
            <WorkflowIcon size={16} className="text-muted-foreground" />
            Sequências de eventos
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-5">
          <SequenceField
            id="helena"
            label="Helena"
            tone="helena"
            value={helena}
            onChange={setHelena}
            onSubmit={handleSync}
          />
          <SequenceField
            id="marcus"
            label="Marcus"
            tone="marcus"
            value={marcus}
            onChange={setMarcus}
            onSubmit={handleSync}
          />

          {errors.length > 0 && (
            <Alert variant="destructive">
              <TriangleAlert />
              <AlertTitle>Verifique as entradas</AlertTitle>
              <AlertDescription>
                <ul className="list-disc pl-4">
                  {errors.map((e) => (
                    <li key={e}>{e}</li>
                  ))}
                </ul>
              </AlertDescription>
            </Alert>
          )}

          <Button onClick={handleSync} className="w-full" disabled={loading}>
            {loading ? (
              <Loader2 className="size-4 animate-spin" />
            ) : (
              <GitCompareArrowsIcon size={16} />
            )}
            {loading ? "Sincronizando…" : "Sincronizar"}
          </Button>

          <div className="flex flex-wrap gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={loadExample}
              disabled={loading}
            >
              <FlaskIcon size={16} />
              Exemplo
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={swap}
              disabled={loading}
            >
              <RefreshCWIcon size={16} />
              Trocar
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={clear}
              disabled={loading}
            >
              <DeleteIcon size={16} />
              Limpar
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Painel de resultados */}
      <div className="space-y-6 lg:col-span-8">
        <Tabs value={method} onValueChange={handleMethodChange}>
          <TabsList className="w-full">
            <TabsTrigger value="dp">Apenas DP</TabsTrigger>
            <TabsTrigger value="backtracking">DP + Backtracking</TabsTrigger>
          </TabsList>
        </Tabs>

        {loading ? (
          <LoadingState />
        ) : apiError ? (
          <ErrorState message={apiError} onRetry={handleSync} />
        ) : result ? (
          <div className="space-y-6">
            <ResultsPanel
              result={result}
              showSubsequences={method === "backtracking"}
            />
            <Separator />
            <section className="space-y-3">
              <h2 className="text-sm font-medium">
                Tabela de programação dinâmica
              </h2>
              <DpTable
                helena={result.helena}
                marcus={result.marcus}
                tabela={result.tabela}
              />
            </section>
          </div>
        ) : (
          <EmptyState />
        )}
      </div>
    </div>
  );
}

interface SequenceFieldProps {
  id: string;
  label: string;
  tone: "helena" | "marcus";
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
}

function SequenceField({
  id,
  label,
  tone,
  value,
  onChange,
  onSubmit,
}: SequenceFieldProps) {
  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        <Label htmlFor={id} className="flex items-center gap-2">
          <span
            className={`size-2.5 rounded-full ${
              tone === "helena" ? "bg-helena" : "bg-marcus"
            }`}
            aria-hidden
          />
          {label}
        </Label>
        <span className="font-mono text-xs text-muted-foreground tabular-nums">
          {value.length}/{MAX_LEN}
        </span>
      </div>
      <Input
        id={id}
        value={value}
        maxLength={MAX_LEN}
        autoComplete="off"
        spellCheck={false}
        placeholder="ex.: ijkijkii"
        className="font-mono tracking-wider"
        onChange={(e) => onChange(sanitize(e.target.value))}
        onKeyDown={(e) => {
          if (e.key === "Enter") onSubmit();
        }}
      />
    </div>
  );
}

function LoadingState() {
  return (
    <div className="flex min-h-72 flex-col items-center justify-center gap-3 rounded-lg border border-dashed bg-card/50 p-10 text-center">
      <Loader2 className="size-6 animate-spin text-muted-foreground" />
      <p className="text-sm text-muted-foreground">
        Consultando o servidor…
      </p>
    </div>
  );
}

function ErrorState({
  message,
  onRetry,
}: {
  message: string;
  onRetry: () => void;
}) {
  return (
    <Alert variant="destructive">
      <TriangleAlert />
      <AlertTitle>Não foi possível sincronizar</AlertTitle>
      <AlertDescription className="space-y-3">
        <p>{message}</p>
        <Button variant="outline" size="sm" onClick={onRetry}>
          Tentar novamente
        </Button>
      </AlertDescription>
    </Alert>
  );
}

function EmptyState() {
  return (
    <div className="flex min-h-72 flex-col items-center justify-center gap-3 rounded-lg border border-dashed bg-card/50 p-10 text-center">
      <div className="rounded-full border bg-muted p-3 text-muted-foreground">
        <GitCompareArrowsIcon size={24} />
      </div>
      <p className="text-sm font-medium">Nenhuma sincronização ainda</p>
      <p className="max-w-sm text-sm text-muted-foreground">
        Informe as sequências de Helena e Marcus e clique em{" "}
        <span className="font-medium text-foreground">Sincronizar</span> para ver
        a tabela de PD e as maiores subsequências comuns.
      </p>
    </div>
  );
}
