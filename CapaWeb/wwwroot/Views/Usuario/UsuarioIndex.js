
let tablaData;
let idEditar = 0;
const controlador = "Usuario";
const modal = "mdData";
const preguntaEliminar = "Desea inactivar al usuario";
const confirmaEliminar = "El usuario fue modificado.";
const confirmaRegistro = "Usuario registrado!";

document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        "ajax": {
            "url": `/${controlador}/ListaUsuarios`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "Nro CI", "data": "nroCi", width: "100px" },
            { title: "Nombres", "data": "nombre" },
            { title: "Apellidos", "data": "apellido" },
            { title: "Correo", "data": "correo" },
            {
                title: "Rol", "data": "rolUsuario", render: function (data) {
                    return data.nombre
                }
            },
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
                title: "", "data": "idUsuario", width: "100px", render: function () {
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

    fetch(`/${controlador}/ListaRoles`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data.length > 0) {
            $("#cboRolUser").append($("<option>").val("").text(""));
            responseJson.data.forEach((item) => {
                $("#cboRolUser").append($("<option>").val(item.idRolUsuario).text(item.nombre));
            });
            $('#cboRolUser').select2({
                theme: 'bootstrap-5',
                dropdownParent: $('#mdData'),
                placeholder: "Seleccionar"
            });
        }
    }).catch((error) => {
        Swal.fire({
            title: "Error!",
            text: "No se encontraron coincidencias.",
            icon: "warning"
        });
    })

});

$("#tbData tbody").on("click", ".btn-editar", function () {
    const filaSeleccionada = $(this).closest('tr');
    const data = tablaData.row(filaSeleccionada).data();

    idEditar = data.idUsuario;
    $("#txtNroDocumento").val(data.nroCi);
    $("#txtNombres").val(data.nombre);
    $("#txtApellidos").val(data.apellido);
    $("#txtCorreo").val(data.correo);
    $("#txtClave").val(data.clave);
    $("#cboRolUser").val(data.rolUsuario.idRolUsuario.toString()).trigger("change");
    $(`#${modal}`).modal('show');
})


$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtNroDocumento").val("");
    $("#txtNombres").val("");
    $("#txtApellidos").val("");
    $("#txtCorreo").val("");
    $("#txtClave").val("");
    $("#cboRolUser").val("").trigger('change');
    $(`#${modal}`).modal('show');
})

$("#btnGuardar").on("click", function () {
    if ($("#txtNroDocumento").val().trim() == "" ||
        $("#txtNombres").val().trim() == "" ||
        $("#txtApellidos").val().trim() == "" ||
        $("#txtCorreo").val() == "" ||
        $("#txtClave").val() == "" || $("#cboRolUser").val() == ""
    ) {
        Swal.fire({
            title: "Error!",
            text: "Falta completar datos.",
            icon: "warning"
        });
        return
    }

    const objeto = {
        IdUsuario: idEditar,
        NroCi: $("#txtNroDocumento").val().trim(),
        Nombre: $("#txtNombres").val().trim(),
        Apellido: $("#txtApellidos").val().trim(),
        Correo: $("#txtCorreo").val(),
        Clave: $("#txtClave").val(),
        RolUsuario: {
            IdRolUsuario: $("#cboRolUser").val()
        }
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
                if (responseJson.estado) {
                    Swal.fire({ text: responseJson.mensaje, icon: "success" });
                    $(`#${modal}`).modal('hide');
                    idEditar = 0;
                    tablaData.ajax.reload();
                } else {
                    Swal.fire({ title: "Error!", text: responseJson.mensaje, icon: "warning" });
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
                if (responseJson.estado) {
                    Swal.fire({ text: responseJson.mensaje, icon: "success" });
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