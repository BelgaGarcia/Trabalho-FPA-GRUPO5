"use client";

import { useTheme } from "next-themes";
import { Button } from "@/components/ui/button";
import { MoonIcon } from "@/components/ui/moon";
import { SunIcon } from "@/components/ui/sun";

/**
 * Botão que alterna entre tema claro e escuro.
 * Os dois ícones são renderizados e a visibilidade é decidida via CSS
 * (classe `.dark` no <html>), evitando divergência de hidratação.
 */
export function ThemeToggle() {
  const { resolvedTheme, setTheme } = useTheme();

  return (
    <Button
      variant="outline"
      size="icon"
      aria-label="Alternar tema claro e escuro"
      onClick={() => setTheme(resolvedTheme === "dark" ? "light" : "dark")}
    >
      <SunIcon size={16} className="hidden dark:block" />
      <MoonIcon size={16} className="block dark:hidden" />
    </Button>
  );
}
