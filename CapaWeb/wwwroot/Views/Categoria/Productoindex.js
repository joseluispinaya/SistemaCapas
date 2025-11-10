
let tablaData;
let idEditar = 0;
const controlador = "Producto";
const modal = "mdData";
const confirmaRegistro = "Producto registrado!";

document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        "ajax": {
            "url": `/${controlador}/Lista`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                title: "Imagen",
                data: "imagenPro",
                width: "80px",
                render: function (data) {
                    // si no tiene imagen, muestra una imagen por defecto
                    if (!data)
                        data = "https://joseluis1989-007-site1.ltempurl.com/Imagenes/sinimagen.png";

                    return `<img src="${data}" class="img-fluid" style="max-width: 40px; max-height: 40px; border-radius: 8px" />`;
                }
            },
            { title: "Nombre", "data": "nombre" },
            { title: "Pre Compra", "data": "precioCompra" },
            { title: "Pre Venta", "data": "precioVenta" },
            {
                title: "Categoria", "data": "categoria", render: function (data) {
                    return data.nombre
                }
            },
            { title: "Cantidad", "data": "cantidad" },
            {
                title: "", "data": "idProducto", width: "100px", render: function () {
                    return `<div class="btn-group dropstart">
                    <button type="button" class="btn btn-secondary dropdown-toggle" data-bs-toggle="dropdown">
                        Acción
                    </button>
                    <ul class="dropdown-menu">
                        <li><button class="dropdown-item btn-editar">Editar</button></li>
                        <li><button class="dropdown-item btn-eliminar">Eliminar</button></li>
                    </ul>
                `;
                }
            }
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    fetch(`/Categoria/Lista`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data.length > 0) {
            $("#cboCategoria").append($("<option>").val("").text(""));
            responseJson.data.forEach((item) => {
                $("#cboCategoria").append($("<option>").val(item.idCategoria).text(item.nombre));
            });
            $('#cboCategoria').select2({
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
    var filaSeleccionada = $(this).closest('tr');
    var data = tablaData.row(filaSeleccionada).data();

    idEditar = data.idProducto;
    $("#txtNombre").val(data.nombre);
    $("#cboCategoria").val(data.categoria.idCategoria.toString()).trigger("change");
    $("#txtDescripcion").val(data.descripcion);
    $("#txtCantidad").val(data.cantidad);
    $("#txtCompra").val(data.precioCompra);
    $("#txtVenta").val(data.precioVenta);
    $("#imgProducto").attr("src", data.imagenPro == "" ? "images/bot1.png" : data.imagenPro);
    $("#txtImagen").val("");
    //$("#imgProducto").attr("src", data.photoFull);
    $(`#${modal}`).modal('show');
})

$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtNombre").val("");
    $("#cboCategoria").val("").trigger('change');
    $("#txtDescripcion").val("");
    $("#txtCantidad").val("");
    $("#txtCompra").val("");
    $("#txtVenta").val("");
    $("#imgProducto").attr("src", "https://joseluis1989-007-site1.ltempurl.com/Imagenes/sinimagen.png");
    $("#txtImagen").val("");
    $(`#${modal}`).modal('show');
})

$("#txtImagen").on("change", function () {
    let file = this.files[0];

    if (file) {
        let reader = new FileReader();

        reader.onload = function (e) {
            $("#imgProducto").attr("src", e.target.result);
        }

        reader.readAsDataURL(file);
    }
});


$("#btnGuardar").on("click", function () {
    if ($("#txtNombre").val().trim() == "" || $("#cboCategoria").val() == "") {
        Swal.fire({
            title: "Error!",
            text: "Falta completar datos.",
            icon: "warning"
        });
        return
    }

    const fileInput = document.getElementById('txtImagen');

    if (fileInput.files.length === 0) {
        Swal.fire({ text: "Debe seleccionar una imagen.", icon: "warning" });
        return;
    }

    let precioCompraStr = $("#txtCompra").val().trim();
    let precioVentaStr = $("#txtVenta").val().trim();
    const precioCompra = Number(parseFloat(precioCompraStr).toFixed(2));
    const precioVenta = Number(parseFloat(precioVentaStr).toFixed(2));

    let modelo = {
        IdProducto: idEditar,
        Nombre: $("#txtNombre").val().trim(),
        Descripcion: $("#txtDescripcion").val().trim(),
        PrecioCompra: precioCompra,
        PrecioVenta: precioVenta,
        Cantidad: parseInt($("#txtCantidad").val()),
        Categoria: {
            IdCategoria: $("#cboCategoria").val()
        },
        ImagenPro: $("#imgProducto").attr("src")
    }

    const formData = new FormData();
    formData.append("foto", fileInput.files[0]);
    formData.append("modelo", JSON.stringify(modelo));

    $(`#${modal}`).find("div.modal-content").LoadingOverlay("show");

    if (idEditar != 0) {

        fetch(`/${controlador}/Editar`, {
            method: "PUT",
            body: formData
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
            body: formData
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

$("#btnGuardarImg").on("click", function () {

    const fileInput = document.getElementById('inputFoto');

    if (fileInput.files.length === 0) {
        Swal.fire({ text: "Debe seleccionar una imagen.", icon: "warning" });
        return;
    }

    const formData = new FormData();
    formData.append("foto", fileInput.files[0]);

    $("#mdDataPrueba").find("div.modal-content").LoadingOverlay("show");

    fetch("/Producto/GuardarFoto", {
        method: "POST",
        body: formData
    })
        .then(response => {
            if (!response.ok) throw response;
            return response.json();
        })
        .then(responseJson => {
            console.log(responseJson);

            if (responseJson.estado) {  // 👈 Asegúrate del case (Estado o estado)
                Swal.fire({ text: responseJson.mensaje, icon: "success" });
                console.log("Ruta de la imagen:", responseJson.objeto);

                $("#mdDataPrueba").modal('hide');
                $("#inputFoto").val(""); // Limpia input
            } else {
                Swal.fire({ title: "Error!", text: responseJson.mensaje, icon: "warning" });
            }
        })
        .catch(() => {
            Swal.fire({ title: "Error!", text: "No se pudo ejecutar.", icon: "error" });
        })
        .finally(() => {
            $("#mdDataPrueba").find("div.modal-content").LoadingOverlay("hide");
        });
});

//fin