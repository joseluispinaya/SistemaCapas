let tablaData;
let idEditar = 0;
const controlador = "Categoria";
const modal = "mdData";
const confirmaRegistro = "Categoria registrada!";
//"url": '',

document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        scrollX: true,
        "ajax": {
            "url": `/${controlador}/Lista`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "Nombre", "data": "nombre" },
            {
                title: "Estado",
                "data": "activo",
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success">Activo</span>'
                        : '<span class="badge bg-danger">No Activo</span>';
                }
            },
            {
                title: "", "data": "idCategoria", width: "100px", render: function (data, type, row) {
                    return `<div class="btn-group dropstart">
                        <button type="button" class="btn btn-secondary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                            Acción
                        </button>
                        <ul class="dropdown-menu">
                            <li><button class="dropdown-item btn-editar">Editar</button></li>
                            <li><button class="dropdown-item btn-eliminar">Eliminar</button></li>
                        </ul>`
                }
            }
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    $("#cboEstado").append($("<option>").val("").text(""));
    $("#cboEstado").append($("<option>").val("1").text("Activo"));
    $("#cboEstado").append($("<option>").val("0").text("No Activo"));

    $('#cboEstado').select2({
        theme: 'bootstrap-5',
        dropdownParent: $('#mdData'),
        placeholder: "Seleccionar"
    });

});

$("#tbData tbody").on("click", ".btn-editar", function () {
    var filaSeleccionada = $(this).closest('tr');
    var data = tablaData.row(filaSeleccionada).data();

    idEditar = data.idCategoria;
    var esta = data.activo ? 1 : 0;

    $("#txtNombre").val(data.nombre);
    //$("#cboEstado").select2("val", data.activo);
    //$("#cboEstado").select2("val", esta.toString());
    $("#cboEstado").val(esta.toString()).trigger("change");
    $(`#${modal}`).modal('show');
    //Swal.fire({
    //    title: "Mensaje",
    //    text: `Nombre: ${data.nombre}?`,
    //    icon: "success"
    //});
    
})

$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtNombre").val("")
    $("#cboEstado").val("").trigger('change');
    $(`#${modal}`).modal('show');
})

$("#btnGuardar").on("click", function () {
    if ($("#txtNombre").val().trim() == "" || $("#cboEstado").val() == "") {
        Swal.fire({
            title: "Error!",
            text: "Falta completar datos.",
            icon: "warning"
        });
        return
    }

    let objeto = {
        IdCategoria: idEditar,
        Nombre: $("#txtNombre").val().trim(),
        Activo: $("#cboEstado").val() == "1"
    }

    $(`#${modal}`).find("div.modal-content").LoadingOverlay("show");

    if (idEditar != 0) {

        fetch(`/${controlador}/Editar`, {
            method: "PUT",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.data) {
                    Swal.fire({ text: "Se guardaron los cambios!", icon: "success" });
                    $(`#${modal}`).modal('hide');
                    idEditar = 0;
                    tablaData.ajax.reload();
                } else {
                    Swal.fire({ title: "Error!", text: "Ocurrió un error.", icon: "warning" });
                }
            })
            .catch(() => {
                Swal.fire({ title: "Error!", text: "No se pudo editar.", icon: "warning" });
            })
            .finally(() => {
                // finally se ejecuta siempre (success o catch)
                $(`#${modal}`).find("div.modal-content").LoadingOverlay("hide");
            });
    } else {
        fetch(`/${controlador}/Guardar`, {
            method: "POST",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.data) {
                    Swal.fire({ text: confirmaRegistro, icon: "success" });
                    $(`#${modal}`).modal('hide');
                    tablaData.ajax.reload();
                } else {
                    Swal.fire({ title: "Error!", text: "Ocurrió un error.", icon: "warning" });
                }
            })
            .catch(() => {
                Swal.fire({ title: "Error!", text: "No se pudo registrar.", icon: "warning" });
            })
            .finally(() => {
                $(`#${modal}`).find("div.modal-content").LoadingOverlay("hide");
            });
    }
});


//fin