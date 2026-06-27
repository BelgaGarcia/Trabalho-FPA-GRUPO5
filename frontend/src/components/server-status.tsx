"use client";

import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";
import { checkHealth } from "@/lib/api";

type Status = "checking" | "online" | "offline";

const LABELS: Record<Status, string> = {
  checking: "Servidor Flask: verificando…",
  online: "Servidor Flask conectado",
  offline: "Servidor Flask offline",
};

/**
 * Indicador discreto da comunicação com o Flask: uma bolinha que pulsa em
 * verde quando o servidor responde e em vermelho quando está offline, com um
 * rótulo "Servidor Flask" sempre visível. Faz polling de /api/health.
 */
export function ServerStatus() {
  const [status, setStatus] = useState<Status>("checking");

  useEffect(() => {
    let active = true;
    const controller = new AbortController();

    const ping = async () => {
      const ok = await checkHealth(controller.signal);
      if (active) setStatus(ok ? "online" : "offline");
    };

    ping();
    const id = setInterval(ping, 8000);

    return () => {
      active = false;
      controller.abort();
      clearInterval(id);
    };
  }, []);

  const color =
    status === "online"
      ? "bg-emerald-500"
      : status === "offline"
        ? "bg-red-500"
        : "bg-muted-foreground";

  return (
    <span
      className="inline-flex items-center gap-2 text-xs font-medium text-muted-foreground"
      role="status"
      aria-label={LABELS[status]}
    >
      <span className="relative flex size-2.5 items-center justify-center">
        <span
          className={cn(
            "absolute inline-flex size-full animate-ping rounded-full opacity-75 motion-reduce:hidden",
            color,
          )}
        />
        <span className={cn("relative inline-flex size-2.5 rounded-full", color)} />
      </span>
      Servidor Flask
    </span>
  );
}
