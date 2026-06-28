/*
 * ============================================================================
 * Sincronizador de Padroes (LCS) - Logica da interface
 * Trabalho pratico - Fundamentos de Projeto e Analise de Algoritmos (FPAA)
 * PUC Minas - Contagem
 *
 * Modulo : Frontend / js/app.js
 * Autores: Grupo 5 - Lucas Fernandes (Frontend e Documentacao)
 * Versao : 1.0
 * Data   : 27/06/2026
 *
 * Responsabilidades (Lucas Fernandes):
 *   A. Coletar a entrada (D conjuntos + sequencias) e enviar para a API.
 *   B. Renderizar os resultados (subsequencias comuns) de forma organizada.
 *   C. Construir dinamicamente a tabela de programacao dinamica (DP).
 *   D. Capturar e exibir os erros retornados pelo backend.
 *
 * IMPORTANTE: a validacao das entradas e feita exclusivamente no backend.
 * Este script apenas coleta os dados, consome /api/sincronizar e renderiza
 * a resposta (sucesso ou erro), sem aplicar regras de negocio.
 * ============================================================================
 */

(function () {
  "use strict";

  // Limites apenas de interface (a validacao real ocorre no backend).
  var D_MIN = 1;
  var D_MAX = 10;
  var MAX_CARACTERES = 80;
  var METODO_PADRAO = "backtracking"; // metodo padrao caso nenhum esteja marcado
  var TEMA_CHAVE = "fpaa-tema";

  // Referencias do DOM.
  var inputD = document.querySelector("#input-d");
  var conjuntosEl = document.querySelector("#conjuntos");
  var resultadosEl = document.querySelector("#resultados");
  var emptyStateEl = document.querySelector("#empty-state");
  var alertEl = document.querySelector("#alert-erro");
  var alertMsgEl = document.querySelector("#alert-erro-msg");
  var btnSync = document.querySelector("#btn-sync");
  var btnExample = document.querySelector("#btn-example");
  var btnTheme = document.querySelector("#btn-theme");
  var themeLabel = document.querySelector("[data-theme-label]");
  var statusEl = document.querySelector("#server-status");
  var statusTextEl = statusEl.querySelector(".status__text");

  /* ------------------------------------------------------------------------
   * Utilitarios
   * ---------------------------------------------------------------------- */

  function limitarD(valor) {
    var n = parseInt(valor, 10);
    if (isNaN(n)) {
      n = D_MIN;
    }
    return Math.min(D_MAX, Math.max(D_MIN, n));
  }

  // Sanitizacao puramente visual: confina a digitacao a letras minusculas.
  // Nao substitui a validacao do backend, apenas guia o usuario.
  function ligarSanitizacao(input) {
    input.addEventListener("input", function () {
      var limpo = input.value.toLowerCase().replace(/[^a-z]/g, "");
      if (limpo !== input.value) {
        input.value = limpo;
      }
      atualizarContador(input);
    });
  }

  function atualizarContador(input) {
    var contador = input.parentElement.querySelector("[data-counter]");
    if (contador) {
      contador.textContent = input.value.length + " / " + MAX_CARACTERES;
    }
  }

  /* ------------------------------------------------------------------------
   * Requisito A: construcao dinamica dos blocos de conjuntos
   * ---------------------------------------------------------------------- */

  function criarCampoSequencia(rotulo, indice, lado) {
    var field = document.createElement("label");
    field.className = "field";

    var head = document.createElement("div");
    head.className = "field__head";

    var span = document.createElement("span");
    span.className = "field__label";
    span.textContent = "Sequência de " + rotulo;

    var contador = document.createElement("span");
    contador.className = "field__hint";
    contador.setAttribute("data-counter", "");
    contador.textContent = "0 / " + MAX_CARACTERES;

    head.appendChild(span);
    head.appendChild(contador);

    var input = document.createElement("input");
    input.type = "text";
    input.className = "input input--mono";
    input.maxLength = MAX_CARACTERES;
    input.autocomplete = "off";
    input.spellcheck = false;
    input.placeholder = lado === "helena" ? "ex.: ijkijkii" : "ex.: ikjikji";
    input.setAttribute("data-seq", lado);
    input.setAttribute("data-conjunto", String(indice));

    field.appendChild(head);
    field.appendChild(input);
    ligarSanitizacao(input);

    return field;
  }

  // Recria os D blocos preservando os valores ja digitados quando possivel.
  function renderConjuntos(d) {
    var anteriores = coletarPares();
    conjuntosEl.textContent = "";

    for (var i = 0; i < d; i++) {
      var bloco = document.createElement("div");
      bloco.className = "conjunto";

      var titulo = document.createElement("p");
      titulo.className = "conjunto__title";
      titulo.textContent = "Conjunto " + (i + 1);
      bloco.appendChild(titulo);

      var campoHelena = criarCampoSequencia("Helena", i, "helena");
      var campoMarcus = criarCampoSequencia("Marcus", i, "marcus");
      bloco.appendChild(campoHelena);
      bloco.appendChild(campoMarcus);
      conjuntosEl.appendChild(bloco);

      if (anteriores[i]) {
        var inHelena = campoHelena.querySelector("input");
        var inMarcus = campoMarcus.querySelector("input");
        inHelena.value = anteriores[i].helena;
        inMarcus.value = anteriores[i].marcus;
        atualizarContador(inHelena);
        atualizarContador(inMarcus);
      }
    }
  }

  // Le os pares Helena/Marcus diretamente do DOM, em ordem.
  function coletarPares() {
    var pares = [];
    var blocos = conjuntosEl.querySelectorAll(".conjunto");
    blocos.forEach(function (bloco) {
      var helena = bloco.querySelector('[data-seq="helena"]');
      var marcus = bloco.querySelector('[data-seq="marcus"]');
      pares.push({
        helena: helena ? helena.value.trim() : "",
        marcus: marcus ? marcus.value.trim() : "",
      });
    });
    return pares;
  }

  /* ------------------------------------------------------------------------
   * Requisito D: exibicao de erros do backend
   * ---------------------------------------------------------------------- */

  function limparSaida() {
    resultadosEl.textContent = "";
    ocultarErro();
  }

  function ocultarErro() {
    alertEl.classList.add("is-hidden");
    alertEl.hidden = true;
    alertMsgEl.textContent = "";
  }

  function mostrarErro(mensagem) {
    // Limpa qualquer resultado anterior antes de exibir a falha.
    resultadosEl.textContent = "";
    emptyStateEl.classList.add("is-hidden");
    alertMsgEl.textContent = mensagem;
    alertEl.classList.remove("is-hidden");
    alertEl.hidden = false;
    alertEl.focus();
  }

  // Extrai a mensagem do JSON de erro do backend.
  // 400 -> {erro}; 500/503 -> {status, motivo}.
  function extrairMensagemErro(dados) {
    if (dados && typeof dados === "object") {
      if (dados.erro) return dados.erro;
      if (dados.motivo) return dados.motivo;
    }
    return "Ocorreu um erro inesperado ao processar a requisição.";
  }

  /* ------------------------------------------------------------------------
   * Requisitos B e C: renderizacao de resultados e tabela DP
   * ---------------------------------------------------------------------- */

  function criarStat(rotulo, valor) {
    var stat = document.createElement("div");
    stat.className = "stat";

    var label = document.createElement("span");
    label.className = "stat__label";
    label.textContent = rotulo;

    var value = document.createElement("span");
    value.className = "stat__value";
    value.textContent = valor;

    stat.appendChild(label);
    stat.appendChild(value);
    return stat;
  }

  // Requisito B: lista as subsequencias comuns retornadas pela API.
  function criarSubsequencias(subsequencias) {
    var wrap = document.createElement("div");

    var label = document.createElement("p");
    label.className = "subseq__label";
    label.textContent = "Maiores subsequências comuns";
    wrap.appendChild(label);

    var lista = document.createElement("ul");
    lista.className = "subseq__list";

    if (!Array.isArray(subsequencias) || subsequencias.length === 0) {
      var vazio = document.createElement("li");
      vazio.className = "chip chip--empty";
      vazio.textContent = "nenhuma subsequência comum";
      lista.appendChild(vazio);
    } else {
      subsequencias.forEach(function (item) {
        var chip = document.createElement("li");
        chip.className = "chip";
        chip.textContent = item && item.length ? item : "(vazia)";
        if (!item || !item.length) {
          chip.classList.add("chip--empty");
        }
        lista.appendChild(chip);
      });
    }

    wrap.appendChild(lista);
    return wrap;
  }

  // Requisito C: constroi a <table> da matriz DP.
  // A matriz tem dimensao (len(helena)+1) x (len(marcus)+1):
  //   - linhas  -> caracteres de Helena (eixo vertical)
  //   - colunas -> caracteres de Marcus (eixo horizontal)
  function renderTabelaDP(tabela, helena, marcus) {
    var wrap = document.createElement("div");
    wrap.className = "dp";

    var label = document.createElement("p");
    label.className = "dp__label";
    label.textContent = "Tabela de programação dinâmica (L[i][j])";
    wrap.appendChild(label);

    if (!Array.isArray(tabela) || tabela.length === 0) {
      var aviso = document.createElement("p");
      aviso.className = "card__desc";
      aviso.textContent = "Tabela DP indisponível para este conjunto.";
      wrap.appendChild(aviso);
      return wrap;
    }

    var scroll = document.createElement("div");
    scroll.className = "dp__scroll";

    var table = document.createElement("table");
    table.className = "dp-table";

    var helenaChars = String(helena || "").split("");
    var marcusChars = String(marcus || "").split("");
    var ultimaLinha = tabela.length - 1;

    // Cabecalho: canto + epsilon + caracteres de Marcus.
    var thead = document.createElement("thead");
    var headRow = document.createElement("tr");
    headRow.appendChild(criarCelulaCabecalho("", "dp-corner"));
    headRow.appendChild(criarCelulaCabecalho("ε")); // epsilon
    marcusChars.forEach(function (ch) {
      headRow.appendChild(criarCelulaCabecalho(ch, "dp-axis"));
    });
    thead.appendChild(headRow);
    table.appendChild(thead);

    // Corpo: cada linha inicia com o caractere de Helena (ou epsilon).
    var tbody = document.createElement("tbody");
    tabela.forEach(function (linha, i) {
      var tr = document.createElement("tr");
      var rotuloLinha = i === 0 ? "ε" : helenaChars[i - 1] || "";
      tr.appendChild(
        criarCelulaCabecalho(rotuloLinha, i === 0 ? "" : "dp-axis")
      );

      var ultimaColuna = linha.length - 1;
      linha.forEach(function (valor, j) {
        var td = document.createElement("td");
        td.textContent = valor;
        // Destaca a celula final L[m][n] = comprimento da LCS.
        if (i === ultimaLinha && j === ultimaColuna) {
          td.classList.add("dp-final");
        }
        tr.appendChild(td);
      });
      tbody.appendChild(tr);
    });
    table.appendChild(tbody);

    scroll.appendChild(table);
    wrap.appendChild(scroll);
    return wrap;
  }

  function criarCelulaCabecalho(texto, classe) {
    var th = document.createElement("th");
    th.scope = "col";
    th.textContent = texto;
    if (classe) {
      th.className = classe;
    }
    return th;
  }

  // Normaliza a resposta para um formato canonico, aceitando os dois contratos
  // de API que existem no projeto:
  //   - API do Daniel / base : comprimentoMaximo, padroes, tabelaDp  (camelCase)
  //   - integrante3-dp        : comprimento_maximo, subsequencias, tabela
  // Assim o frontend funciona independentemente de qual contrato subir na main.
  function normalizarResultado(dados) {
    var d = dados || {};
    var subs = d.subsequencias || d.padroes || [];
    if (!Array.isArray(subs)) {
      subs = [];
    }

    var comprimento = d.comprimento_maximo;
    if (comprimento === undefined || comprimento === null) {
      comprimento = d.comprimentoMaximo;
    }

    var quantidade = d.quantidade;
    if (quantidade === undefined || quantidade === null) {
      quantidade = subs.length;
    }

    return {
      helena: d.helena || "",
      marcus: d.marcus || "",
      comprimento: comprimento !== undefined ? comprimento : "—",
      quantidade: quantidade,
      subsequencias: subs,
      tabela: d.tabela || d.tabelaDp || [],
      algoritmo: d.algoritmo || "",
    };
  }

  // Monta o cartao completo de um conjunto (Requisitos B + C).
  function renderResultado(dados, numero) {
    var r = normalizarResultado(dados);

    var card = document.createElement("article");
    card.className = "result";

    // Cabecalho com identificacao do conjunto e par de sequencias.
    var head = document.createElement("div");
    head.className = "result__head";

    var titulo = document.createElement("h3");
    titulo.className = "result__title";
    titulo.textContent = "Conjunto " + numero;
    head.appendChild(titulo);

    var par = document.createElement("div");
    par.className = "result__pair";
    par.appendChild(criarParInfo("Helena", r.helena));
    par.appendChild(criarParInfo("Marcus", r.marcus));
    head.appendChild(par);

    // Algoritmo usado (quando o backend informa, ex.: API do Daniel).
    if (r.algoritmo) {
      var algo = document.createElement("p");
      algo.className = "result__algo";
      algo.textContent = r.algoritmo;
      head.appendChild(algo);
    }

    card.appendChild(head);

    // Estatisticas.
    var stats = document.createElement("div");
    stats.className = "stats";
    stats.appendChild(
      criarStat("Comprimento máximo", String(r.comprimento))
    );
    stats.appendChild(criarStat("Subsequências", String(r.quantidade)));
    card.appendChild(stats);

    // Requisito B: subsequencias.
    card.appendChild(criarSubsequencias(r.subsequencias));

    // Requisito C: tabela DP.
    card.appendChild(renderTabelaDP(r.tabela, r.helena, r.marcus));

    resultadosEl.appendChild(card);
  }

  function criarParInfo(rotulo, valor) {
    var frag = document.createElement("span");
    var b = document.createElement("b");
    b.textContent = rotulo + ": ";
    var val = document.createElement("span");
    val.textContent = valor && valor.length ? valor : "(vazia)";
    frag.appendChild(b);
    frag.appendChild(val);
    return frag;
  }

  /* ------------------------------------------------------------------------
   * Fluxo principal de envio (Requisito A)
   * ---------------------------------------------------------------------- */

  function setOcupado(ocupado) {
    btnSync.disabled = ocupado;
    btnExample.disabled = ocupado;
    inputD.disabled = ocupado;
    btnSync.setAttribute("aria-busy", ocupado ? "true" : "false");
    btnSync.textContent = ocupado ? "Processando…" : "Sincronizar";
  }

  // Le o metodo escolhido no controle segmentado. A main suporta "dp"
  // (LcsDpEnumerator) e "backtracking" (LcsBacktracker); enviamos esse campo
  // no payload para o nucleo C# decidir a estrategia de enumeracao.
  function obterMetodo() {
    var escolhido = document.querySelector('input[name="metodo"]:checked');
    return escolhido ? escolhido.value : METODO_PADRAO;
  }

  // Faz UMA chamada a /api/sincronizar para um par.
  //
  // Nota: o backend atual (base da main) ainda nao expoe a rota de
  // processamento /api/sincronizar -- ela entra na etapa de integracao da API.
  // Por isso tratamos 404 e respostas nao-JSON de forma elegante, mantendo o
  // frontend pronto para quando a rota existir.
  function sincronizarPar(par, metodo) {
    return fetch("/api/sincronizar", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        helena: par.helena,
        marcus: par.marcus,
        metodo: metodo,
      }),
    }).then(function (resposta) {
      var tipo = resposta.headers.get("content-type") || "";

      // Quando a rota de processamento ainda nao existe, o Flask responde com
      // uma pagina de erro HTML (404/405), nao JSON. Detectamos pelo
      // content-type e exibimos uma mensagem clara, em vez de um erro generico.
      if (tipo.indexOf("application/json") === -1) {
        return {
          ok: false,
          dados: {
            status: "indisponivel",
            motivo:
              "A rota de processamento (/api/sincronizar) ainda não está " +
              "disponível neste backend. A integração da API será adicionada " +
              "na etapa do Integrante 6.",
          },
        };
      }

      return resposta
        .json()
        .then(function (dados) {
          return { ok: resposta.ok, dados: dados };
        })
        .catch(function () {
          return {
            ok: false,
            dados: {
              status: "erro",
              motivo: "Resposta inválida do servidor (era esperado JSON).",
            },
          };
        });
    });
  }

  // Percorre os D conjuntos chamando a API para cada par.
  function sincronizar() {
    limparSaida();
    var pares = coletarPares();

    if (pares.length === 0) {
      mostrarErro("Adicione ao menos um conjunto de dados.");
      return;
    }

    emptyStateEl.classList.add("is-hidden");
    setOcupado(true);

    var metodo = obterMetodo();
    var indice = 0;

    function processarProximo() {
      if (indice >= pares.length) {
        setOcupado(false);
        return;
      }

      var numero = indice + 1;
      sincronizarPar(pares[indice], metodo)
        .then(function (resultado) {
          // Falha: limpa resultados e interrompe o lote (Requisito D).
          if (!resultado.ok || (resultado.dados && resultado.dados.status === "erro") ||
              (resultado.dados && resultado.dados.status === "indisponivel")) {
            mostrarErro(
              "Conjunto " + numero + ": " + extrairMensagemErro(resultado.dados)
            );
            setOcupado(false);
            return;
          }

          renderResultado(resultado.dados, numero);
          indice++;
          processarProximo();
        })
        .catch(function () {
          mostrarErro("Não foi possível conectar ao servidor.");
          setOcupado(false);
        });
    }

    processarProximo();
  }

  /* ------------------------------------------------------------------------
   * Exemplo do roteiro (D = 1)
   * ---------------------------------------------------------------------- */

  function carregarExemplo() {
    inputD.value = "1";
    renderConjuntos(1);
    var helena = conjuntosEl.querySelector('[data-seq="helena"]');
    var marcus = conjuntosEl.querySelector('[data-seq="marcus"]');
    if (helena && marcus) {
      helena.value = "ijkijkii";
      marcus.value = "ikjikji";
      atualizarContador(helena);
      atualizarContador(marcus);
    }
  }

  /* ------------------------------------------------------------------------
   * Tema claro / escuro
   * ---------------------------------------------------------------------- */

  function aplicarTema(tema) {
    document.documentElement.setAttribute("data-theme", tema);
    if (themeLabel) {
      themeLabel.textContent = tema === "dark" ? "Claro" : "Escuro";
    }
    btnTheme.setAttribute(
      "aria-label",
      tema === "dark" ? "Ativar tema claro" : "Ativar tema escuro"
    );
  }

  function iniciarTema() {
    var salvo = null;
    try {
      salvo = localStorage.getItem(TEMA_CHAVE);
    } catch (e) {
      salvo = null;
    }
    // Identidade visual e dark-first: so usa claro se houver escolha salva
    // ou preferencia explicita do sistema por tema claro.
    var prefereLight =
      window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: light)").matches;
    aplicarTema(salvo || (prefereLight ? "light" : "dark"));
  }

  function alternarTema() {
    var atual = document.documentElement.getAttribute("data-theme");
    var proximo = atual === "dark" ? "light" : "dark";
    aplicarTema(proximo);
    try {
      localStorage.setItem(TEMA_CHAVE, proximo);
    } catch (e) {
      /* ignora indisponibilidade do localStorage */
    }
  }

  /* ------------------------------------------------------------------------
   * Status do servidor (/api/health)
   * ---------------------------------------------------------------------- */

  // Atualiza apenas o texto e o estado, preservando a bolinha pulsante.
  function definirStatus(estado, texto) {
    statusEl.setAttribute("data-state", estado);
    if (statusTextEl) {
      statusTextEl.textContent = texto;
    }
  }

  function verificarServidor() {
    fetch("/api/health")
      .then(function (resposta) {
        if (!resposta.ok) throw new Error("offline");
        return resposta.json();
      })
      .then(function () {
        definirStatus("ok", "Servidor ativo");
      })
      .catch(function () {
        definirStatus("off", "Servidor offline");
      });
  }

  /* ------------------------------------------------------------------------
   * Inicializacao
   * ---------------------------------------------------------------------- */

  function init() {
    iniciarTema();
    renderConjuntos(limitarD(inputD.value));

    inputD.addEventListener("change", function () {
      var d = limitarD(inputD.value);
      inputD.value = String(d);
      renderConjuntos(d);
    });

    btnSync.addEventListener("click", sincronizar);
    btnExample.addEventListener("click", carregarExemplo);
    btnTheme.addEventListener("click", alternarTema);

    verificarServidor();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
