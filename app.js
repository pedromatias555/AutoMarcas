const api = "https://localhost:44345";

const selectMarca = document.getElementById("fMarca");
const selectAno = document.getElementById("fAno");
const selectVendido = document.getElementById("fVendido");
const tabela = document.getElementById("tabela");
const formVeiculo = document.getElementById("formVeiculo");
const editMarca = document.getElementById("editMarca");
const editModelo = document.getElementById("editModelo");
const editInspecao = document.getElementById("inspecao");
const editAno = document.getElementById("ano");
const editVendido = document.getElementById("vendido");
let carIndex = document.getElementById("editIndex");

let listaMarcasGlobal = [];
let listaModelosGlobal = [];

async function Init() {
    listaMarcasGlobal = await obterMarcas();
    listaModelosGlobal = await obterModelos();

    PreencherCombos(listaMarcasGlobal, "fMarca");
    PreencherCombos(listaModelosGlobal, "fModelo");
    PreencherDatalist(listaMarcasGlobal, "listaMarcas", "marcaDetails");
    PreencherDatalist(listaModelosGlobal, "listaModelos", "modelDetails");

    await PreencherLista();

    formVeiculo.addEventListener("submit", async function (e) {
        e.preventDefault();
        await Guardar();
        await PreencherLista();
    });
}

async function PreencherLista() {
    const veiculos = await obterVeiculos();
    MostrarTabela(veiculos);
    PreencherComboAno(veiculos);
}

function MostrarTabela(list) {
    tabela.innerHTML = "";

    list.forEach((item) => {
        let newRow = document.createElement("tr");
        newRow.innerHTML = `
            <td>${item.marcaDetails}</td>
            <td>${item.modelDetails}</td>
            <td>${item.ano}</td>
            <td>${ShowDate(item.ultimaInspecao)} (${inspecaoEstado(new Date(item.ultimaInspecao))})</td>
            <td class="${item.vendido ? 'vendido' : ''}">${item.vendido ? 'Vendido' : 'Disponível'}</td>
        `;

        let actionsCell = document.createElement("td");

        let btnEditar = document.createElement("button");
        btnEditar.textContent = "Editar";
        btnEditar.onclick = () => PreencherTxtEditar(item.veiculoId);
        actionsCell.appendChild(btnEditar);

        let btnDeletar = document.createElement("button");
        btnDeletar.textContent = "Remover";
        btnDeletar.onclick = () => DeletarItem(item.veiculoId);
        actionsCell.appendChild(btnDeletar);

        newRow.appendChild(actionsCell);
        tabela.appendChild(newRow);
    });
}

async function PreencherTxtEditar(id) {
    const veiculo = await obterVeiculo(id);

    const marca = listaMarcasGlobal.find(m => m.marcaId === veiculo.marcaId);
    const modelo = listaModelosGlobal.find(m => m.modeloId === veiculo.modeloId);

    editMarca.value = marca ? marca.marcaDetails : "";
    editModelo.value = modelo ? modelo.modelDetails : "";
    editAno.value = veiculo.ano;
    editVendido.checked = veiculo.vendido;
    editInspecao.value = toInputDateEdit(veiculo.ultimaInspecao);
    carIndex.value = id;
    window.scrollTo(0, 0);
}

function PreencherDatalist(lista, elementId, campoNome) {
    const dl = document.getElementById(elementId);
    dl.innerHTML = "";
    lista.forEach(item => {
        const option = document.createElement("option");
        option.value = item[campoNome];
        dl.appendChild(option);
    });
}

function PreencherCombos(lista, elementId) {
    const p = document.getElementById(elementId);

    if (elementId === "fMarca") {
        p.innerHTML = '<option value="">Todas as marcas</option>';
    } else if (elementId === "fModelo") {
        p.innerHTML = '<option value="">Todos os modelos</option>';
    } else {
        p.innerHTML = '<option value="">-- Selecionar --</option>';
    }

    lista.forEach(item => {
        const option = document.createElement("option");
        option.value = item.marcaId || item.modeloId;
        option.innerText = item.marcaDetails || item.modelDetails;
        p.appendChild(option);
    });
}

function PreencherComboAno(veiculos) {
    const anos = [...new Set(veiculos.map(v => v.ano))].sort((a, b) => b - a);
    const p = document.getElementById("fAno");
    p.innerHTML = '<option value="">Todos os anos</option>';
    anos.forEach(a => {
        const option = document.createElement("option");
        option.value = a;
        option.innerText = a;
        p.appendChild(option);
    });
}

async function FiltrarLista() {
    const veiculos = await obterVeiculos();
    const fM = document.getElementById("fMarca").value;
    const fA = document.getElementById("fAno").value;
    const fV = document.getElementById("fVendido").value;

    const filtrados = veiculos.filter(v => {
        const mMarca = !fM || v.marcaId.toString() === fM;
        const mAno = !fA || v.ano.toString() === fA;
        const mVendido = fV === "" || v.vendido.toString() === fV;
        return mMarca && mAno && mVendido;
    });

    MostrarTabela(filtrados);
}

async function resolverMarcaId(texto) {
    const nome = texto.trim();
    const existente = listaMarcasGlobal.find(
        m => m.marcaDetails.toLowerCase() === nome.toLowerCase()
    );
    if (existente) return existente.marcaId;

    const res = await fetch(`${api}/marcas?marcaDetails=${encodeURIComponent(nome)}`, {
        method: 'POST'
    });
    if (!res.ok) throw new Error("Erro ao criar marca.");
    const nova = await res.json();
    listaMarcasGlobal.push(nova);
    PreencherDatalist(listaMarcasGlobal, "listaMarcas", "marcaDetails");
    return nova.marcaId;
}

async function resolverModeloId(texto) {
    const nome = texto.trim();
    const existente = listaModelosGlobal.find(
        m => m.modelDetails.toLowerCase() === nome.toLowerCase()
    );
    if (existente) return existente.modeloId;

    const res = await fetch(`${api}/modelos?modelDetails=${encodeURIComponent(nome)}`, {
        method: 'POST'
    });
    if (!res.ok) throw new Error("Erro ao criar modelo.");
    const novo = await res.json();
    listaModelosGlobal.push(novo);
    PreencherDatalist(listaModelosGlobal, "listaModelos", "modelDetails");
    return novo.modeloId;
}

async function Guardar() {
    const idExistente = carIndex.value;

    let marcaId, modeloId;
    try {
        marcaId = await resolverMarcaId(editMarca.value);
        modeloId = await resolverModeloId(editModelo.value);
    } catch (err) {
        alert(err.message);
        return;
    }

    const dto = {
        marcaId: marcaId,
        modeloId: modeloId,
        ano: parseInt(editAno.value),
        vendido: editVendido.checked,
        ultimaInspecao: editInspecao.value || null,
        tipoId: 1
    };

    if (idExistente === "") {
        await CreateObject(dto);
    } else {
        await EditObject(dto, idExistente);
    }

    LimparEdits();
}

function LimparEdits() {
    formVeiculo.reset();
    document.getElementById("editIndex").value = "";
}

function inspecaoEstado(data) {
    const agora = new Date();
    const diffMeses = (agora - data) / (1000 * 60 * 60 * 24 * 30);
    if (diffMeses > 12) return '<span class="vendido">Expirada</span>';
    if (diffMeses > 10) return '<span class="aviso">A expirar</span>';
    return '<span class="ok">Válida</span>';
}

function toInputDateEdit(d) {
    const dataObj = new Date(d);
    return `${dataObj.getFullYear()}-${String(dataObj.getMonth() + 1).padStart(2, '0')}-${String(dataObj.getDate()).padStart(2, '0')}`;
}

function ShowDate(d) {
    const dataObj = new Date(d);
    const pad = n => String(n).padStart(2, "0");
    return `${pad(dataObj.getDate())}/${pad(dataObj.getMonth() + 1)}/${dataObj.getFullYear()}`;
}

async function obterVeiculos() {
    const result = await fetch(api + "/veiculos");
    return await result.json();
}

async function obterVeiculo(id) {
    const result = await fetch(api + "/veiculos/" + id);
    return await result.json();
}

async function obterMarcas() {
    const result = await fetch(api + "/marcas");
    return await result.json();
}

async function obterModelos() {
    const result = await fetch(api + "/modelos");
    return await result.json();
}

async function CreateObject(dto) {
    const response = await fetch(api + "/veiculos", {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dto)
    });
    if (!response.ok) throw new Error("Erro ao criar veículo.");
    alert("Veículo adicionado com sucesso!");
}

async function EditObject(dto, id) {
    const response = await fetch(`${api}/veiculos/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dto)
    });
    if (!response.ok) throw new Error("Erro ao editar veículo.");
    alert("Veículo editado com sucesso!");
}

async function DeletarItem(id) {
    if (!confirm("Tem a certeza que deseja remover este veículo?")) return;
    const response = await fetch(`${api}/veiculos/${id}`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' }
    });
    if (!response.ok) throw new Error("Erro ao remover veículo.");
    await PreencherLista();
    alert("Veículo removido com sucesso!");
}