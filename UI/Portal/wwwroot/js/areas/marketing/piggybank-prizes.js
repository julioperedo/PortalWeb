//#region Variables Globales
var newId = 0;
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Eventos

$(() => {
    setupControls();
    setTimeout(function () { setGridHeight("Listado", _gridMargin) }, 800);
    filterData(true);
});

$('#Listado').on("click", ".action", onItemClick);

$("#delete-image").click(onDeleteImage);

$("#cancel-update").click(e => $("#Detail").data("kendoWindow").close());

$("#save-prize").click(onSavingPrize);

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#Detail").kendoWindow({ visible: false, title: "Detalle Premio", modal: true, width: 900 });

    $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-enter w-100">No se encontraron registros para el criterio de búsqueda.</div>' },
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Puntos", field: "points", attributes: alignRight, headerAttributes: alignRight, width: 100, format: '{0:N0}' },
            { title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled" },
            { field: "imageUrl", title: " ", attributes: alignCenter, width: 30, template: '# if($.trim(imageUrl) != "") {# <i class="fas fa-image" title="Tiene Imagen"></i> #} #' },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a class="action action-link action-new" title="Nuevo Clasificador"><i class="fas fa-plus"></i></a>',
                template: '<a class="action action-link action-edit" title="Editar Clasificador"><i class="fas fa-pen"></i></a><a class="action action-link action-delete" title="Eliminar Clasificador"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", dataSource: []
    });

    $("#points").kendoNumericTextBox({ decimals: 0, format: "N0", label: "Puntos" });
    $("#enabled").kendoSwitch({ checked: true, size: "small" });
    $("#photos").kendoUpload({
        async: { saveUrl: urlSaveImage, removeUrl: urlDeleteImage },
        validation: { maxFileSize: 41943040 },
        success: function (e) {
            var imageName = e.files[0].name;
            $("#new-image").val(imageName);
            $("#imageUrl").attr("src", `${urlImages}${imageName}`);
        }
    });
}

function getFilters() {
    var message = "", filter = $("#filter").val();
    return { message: message, data: { FilterData: filter } };
}

function filterData(pageLoad) {
    var filtersData = getFilters();
    $.get(urlFilter, filtersData.data, function (data) {
        loadGrid(data.items);
        if (data.message !== "") {
            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
        }
    });
}

function loadGrid(items) {
    if (items) {
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({ data: items, schema: { model: { id: "id" } } });
        grd.setDataSource(ds);
        if (items && items.length > 0) {
            $('#filter-box').collapse("hide");
            $("#action-excel").removeClass("d-none");
        } else {
            $("#action-excel").addClass("d-none");
        }
        setGridHeight("Listado", _gridMargin);
    }
}

function onItemClick(e) {
    var wnd = $("#Detail").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    if (e.currentTarget.classList.contains('action-new')) {
        item = { id: 0, name: '', description: '', points: "", enabled: true, imageUrl: '' };
        loadDetail(item);
        wnd.open().center();
    }
    if (e.currentTarget.classList.contains('action-edit')) {
        loadDetail(item);
        grd.select(row);
        wnd.open().center();
    }
    if (e.currentTarget.classList.contains('action-delete')) {
        showConfirm(`¿Está seguro que desea eliminar el premio <b>${item.name}</b>?`, function () {
            $.post(urlDelete, { Id: item.id }, d => {
                if (d.message === "") {
                    showMessage("Se ha eliminado el premio.");
                    item = grd.dataSource.get(item.id);
                    grd.dataSource.remove(item);
                    grd.refresh();
                } else {
                    console.error(data.message);
                    showError("Se ha producido un error al eliminar el premio.");
                }
            });
        });
    }
}

function loadDetail(item) {
    $("#id").val(item.id);
    $("#name").val(item.name);
    $("#description").val(item.description);
    $("#points").data("kendoNumericTextBox").value(item.points);
    $("#enabled").data("kendoSwitch").check(item.enabled);
    $("#new-image").val(item.imageUrl);
    $("#imageUrl").attr("src", $.trim(item.imageUrl) !== '' ? `${urlImages}${item.imageUrl}` : urlNoImage);
}

function onDeleteImage(e) {
    $("#new-image").val("");
    $("#imageUrl").attr("src", urlNoImage);
    $("#photos").data("kendoUpload").clearAllFiles();
}

function onSavingPrize(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "", email: "no es un correo electrónico válido" } }).data("kendoValidator");
    if (validator.validate()) {
        var t = form.serializeObject();
        t.enabled = $("#enabled").data("kendoSwitch").check();
        t.imageUrl = t["new-image"];
        console.log(t);

        $.post(urlEdit, { Item: t }, function (d) {
            if (d.message === '') {
                var grd = $("#Listado").data("kendoGrid");
                t.id = d.id;
                grd.dataSource.pushUpdate(t);

                $("#Detail").data("kendoWindow").close();
            } else {
                console.error(d.message);
                showError('Se ha producido un error al guardar el premio');
            }
        });        
    }    
}

//#endregion