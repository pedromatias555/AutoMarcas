

let veiculos = JSON.parse(localStorage.getItem('veiculos')) || [];


veiculos = veiculos.map(v => ({
    ...v,
    ultimaInspecao: new Date(v.ultimaInspecao)
}));


if (veiculos.length === 0) {
    veiculos = [...db];
    localStorage.setItem('veiculos', JSON.stringify(veiculos));
}

const form = document.getElementById('formVeiculo');
const tabela = document.getElementById('tabela');
const fMarca = document.getElementById('fMarca');
const fAno = document.getElementById('fAno');
const fVendido = document.getElementById('fVendido');

function setDate(y, m, d) {
    let tmp = new Date(y, m, d);
    tmp.setMonth(tmp.getMonth() - 1);
    return tmp;
}

function toInputDateLocal(d) {
    const pad = n => String(n).padStart(2, "0");
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

function inspecaoEstado(data) {
    const agora = new Date();
    const diffMeses = (agora - data) / (1000 * 60 * 60 * 24 * 30.44); // Aproximação de meses
    if (diffMeses > 12) return '<span class="vendido">Expirada</span>';
    if (diffMeses > 10) return '<span class="aviso">A expirar</span>';
    return '<span class="ok">Válida</span>';
}


function guardar() {
    localStorage.setItem('veiculos', JSON.stringify(veiculos));
    preencherFiltros(); 
    render();
}

function eliminar(index) {
    if (confirm("Tem a certeza que deseja remover este veículo?")) {
        veiculos.splice(index, 1);
        guardar();
    }
}

function editar(i) {
    const v = veiculos[i];
    document.getElementById('editIndex').value = i;
    document.getElementById('marca').value = v.marca;
    document.getElementById('modelo').value = v.modelo;
    document.getElementById('ano').value = v.ano;
    document.getElementById('inspecao').value = toInputDateLocal(v.ultimaInspecao);
    document.getElementById('vendido').checked = v.vendido;
    window.scrollTo(0, 0); 
}

function preencherFiltros() {
    const marcaAtual = fMarca.value;
    const anoAtual = fAno.value;

    const marcas = [...new Set(veiculos.map(v => v.marca))].sort();
    const anos = [...new Set(veiculos.map(v => v.ano))].sort((a, b) => b - a);

    fMarca.innerHTML = '<option value="">Todas as marcas</option>' + 
        marcas.map(m => `<option value="${m}">${m}</option>`).join('');
    
    fAno.innerHTML = '<option value="">Todos os anos</option>' + 
        anos.map(a => `<option value="${a}">${a}</option>`).join('');

    fMarca.value = marcaAtual;
    fAno.value = anoAtual;
}

function render() {
    tabela.innerHTML = '';

    const filtrados = veiculos.filter(v => {
        const mMarca = !fMarca.value || v.marca === fMarca.value;
        const mAno = !fAno.value || v.ano.toString() === fAno.value;
        const mVendido = !fVendido.value || v.vendido.toString() === fVendido.value;
        return mMarca && mAno && mVendido;
    });

    filtrados.forEach((v, i) => {
      
        const originalIndex = veiculos.indexOf(v);
        
        const row = tabela.insertRow();
        row.innerHTML = `
            <td>${v.marca}</td>
            <td>${v.modelo}</td>
            <td>${v.ano}</td>
            <td>${toInputDateLocal(v.ultimaInspecao)} ${inspecaoEstado(v.ultimaInspecao)}</td>
            <td class="${v.vendido ? 'vendido' : ''}">${v.vendido ? 'Vendido' : 'Disponível'}</td>
            <td>
                <button onclick="editar(${originalIndex})">Editar</button>
                <button onclick="eliminar(${originalIndex})">Remover</button>
            </td>
        `;
    });
}


form.addEventListener('submit', (e) => {
    e.preventDefault();
    const index = document.getElementById('editIndex').value;
    
    const novoVeiculo = {
        marca: document.getElementById('marca').value,
        modelo: document.getElementById('modelo').value,
        ano: parseInt(document.getElementById('ano').value),
        ultimaInspecao: new Date(document.getElementById('inspecao').value),
        vendido: document.getElementById('vendido').checked
    };

    if (index === "") {
        veiculos.push(novoVeiculo);
    } else {
        veiculos[index] = novoVeiculo;
        document.getElementById('editIndex').value = "";
    }

    guardar();
    form.reset();
});


document.getElementById('carregarLS').onclick = () => {
  
    veiculos = [...db]; 
    guardar();
};

document.getElementById('limparLS').onclick = () => {
    if(confirm("Limpar todos os dados?")) {
        veiculos = [];
        guardar();
    }
};

[fMarca, fAno, fVendido].forEach(f => f.addEventListener('change', render));


preencherFiltros();
render();