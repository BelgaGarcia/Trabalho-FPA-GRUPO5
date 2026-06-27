import { WaypointsIcon } from "@/components/ui/waypoints";
import { Badge } from "@/components/ui/badge";
import { ThemeToggle } from "@/components/theme-toggle";
import { ServerStatus } from "@/components/server-status";
import { Synchronizer } from "@/components/sync/synchronizer";

export default function Home() {
  return (
    <div className="flex min-h-dvh flex-col">
      <header className="sticky top-0 z-20 border-b bg-background/80 backdrop-blur">
        <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4 sm:px-6">
          <div className="flex items-center gap-3">
            <div className="flex size-9 items-center justify-center rounded-lg bg-primary text-primary-foreground">
              <WaypointsIcon size={20} />
            </div>
            <div className="leading-tight">
              <p className="font-heading text-sm font-semibold">
                Sincronizador de Padrões
              </p>
              <p className="text-xs text-muted-foreground">
                Maior Subsequência Comum (LCS)
              </p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <ServerStatus />
            <Badge variant="outline" className="hidden sm:inline-flex">
              FPAA · Grupo 5
            </Badge>
            <ThemeToggle />
          </div>
        </div>
      </header>

      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-8 sm:px-6">
        <section className="mb-8 max-w-2xl">
          <h1 className="text-2xl tracking-tight sm:text-3xl">
            Descobrindo padrões em dados complexos
          </h1>
          <p className="mt-2 text-pretty text-muted-foreground">
            Compare as sequências cronológicas de Helena e Marcus para encontrar
            todas as maiores subsequências comuns, preservando a ordem dos
            eventos. A solução usa programação dinâmica e backtracking.
          </p>
        </section>

        <Synchronizer />
      </main>

      <footer className="border-t">
        <div className="mx-auto max-w-6xl px-4 py-6 text-xs text-muted-foreground sm:px-6">
          Fundamentos de Projeto e Análise de Algoritmos — PUC Minas · Visualizador
          da solução LCS (programação dinâmica + backtracking).
        </div>
      </footer>
    </div>
  );
}
