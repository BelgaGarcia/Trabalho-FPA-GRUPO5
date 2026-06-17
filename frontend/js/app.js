const helenaInput = document.querySelector("#input-helena");
const marcusInput = document.querySelector("#input-marcus");
const previewOutput = document.querySelector("#preview-output");
const statusEl = document.querySelector("#server-status");

function sanitizeInput(input) {
  input.addEventListener("input", () => {
    input.value = input.value.toLowerCase().replace(/[^a-z]/g, "");
  });
}

sanitizeInput(helenaInput);
sanitizeInput(marcusInput);

document.querySelector("#btn-sync").addEventListener("click", () => {
  const metodo = document.querySelector('input[name="metodo"]:checked')?.value || "dp";
  previewOutput.textContent =
    "Entrada preparada:\n" +
    `Helena: ${helenaInput.value || "(vazio)"}\n` +
    `Marcus: ${marcusInput.value || "(vazio)"}\n` +
    `Metodo: ${metodo}\n\n` +
    "A integracao completa sera adicionada nas proximas etapas.";
});

fetch("/api/health")
  .then((response) => response.json())
  .then(() => {
    statusEl.textContent = "Servidor Flask ativo";
  })
  .catch(() => {
    statusEl.textContent = "Servidor offline";
  });

