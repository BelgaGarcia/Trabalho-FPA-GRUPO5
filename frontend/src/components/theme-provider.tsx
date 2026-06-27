"use client";

import { ThemeProvider as NextThemesProvider } from "next-themes";

/** Envolve a aplicação para habilitar tema claro/escuro com persistência. */
export function ThemeProvider({
  children,
  ...props
}: React.ComponentProps<typeof NextThemesProvider>) {
  return <NextThemesProvider {...props}>{children}</NextThemesProvider>;
}
