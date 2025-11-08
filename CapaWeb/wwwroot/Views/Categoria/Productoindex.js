
let tablaData;
let idEditar = 0;
const controlador = "Categoria";
const modal = "mdData";
const confirmaRegistro = "Producto registrada!";

$("#btnNuevo").on("click", function () {
    $(`#${modal}`).modal('show');
})

//fin