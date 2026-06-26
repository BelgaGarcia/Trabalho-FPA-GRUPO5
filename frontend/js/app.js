const helenaInput = document.querySelector("#input-helena");
const marcusInput = document.querySelector("#input-marcus");
const previewOutput = document.querySelector("#preview-output");
const statusEl = document.querySelector("#server-status");

function sanitizeInput(input) {
  input.addEventListener("input", () => {
    input.value = input.value.toLowerCase().replace(/[^a-z]/g, "");
  });
}

function formatarTabela(tabela) {
  if (!Array.isArray(tabela)) {
    return "(tabela indisponivel)";
  }

  return tabela.map((linha) => linha.join("\t")).join("\n");
}

function formatarResultado(dados) {
  const linhas = [
    `Status: ${dados.status}`,
    `Metodo: ${dados.metodo}`,
    `Helena: ${dados.helena}`,
    `Marcus: ${dados.marcus}`,
  ];

  if (dados.comprimento_maximo !== undefined) {
    linhas.push(`Comprimento maximo da LCS: ${dados.comprimento_maximo}`);
  }

  if (dados.quantidade !== undefined) {
    linhas.push(`Quantidade de subsequencias: ${dados.quantidade}`);
  }

  if (Array.isArray(dados.subsequencias)) {
    linhas.push("", "Subsequencias encontradas:");
    dados.subsequencias.forEach((subsequencia, indice) => {
      linhas.push(`${indice + 1}. ${subsequencia || "(vazia)"}`);
    });
  }

  if (Array.isArray(dados.tabela)) {
    linhas.push("", "Tabela DP:");
    linhas.push(formatarTabela(dados.tabela));
  }

  return linhas.join("\n");
}

sanitizeInput(helenaInput);
sanitizeInput(marcusInput);

document.querySelector("#btn-sync").addEventListener("click", async () => {
  const helena = helenaInput.value.trim();
  const marcus = marcusInput.value.trim();
  const metodo = document.querySelector('input[name="metodo"]:checked')?.value || "dp";

  if (!helena || !marcus) {
    previewOutput.textContent = "Preencha as sequencias de Helena e Marcus antes de sincronizar.";
    return;
  }

  previewOutput.textContent = "Processando...";

  try {
    const resposta = await fetch("/api/sincronizar", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ helena, marcus, metodo }),
    });

    const dados = await resposta.json();

    if (!resposta.ok) {
      previewOutput.textContent = `Erro: ${dados.erro || dados.motivo || JSON.stringify(dados, null, 2)}`;
      return;
    }

    previewOutput.textContent = formatarResultado(dados);
  } catch {
    previewOutput.textContent = "Nao foi possivel conectar ao servidor.";
  }
});

fetch("/api/health")
  .then((response) => response.json())
  .then(() => {
    statusEl.textContent = "Servidor Flask ativo";
  })
  .catch(() => {
    statusEl.textContent = "Servidor offline";
  });
