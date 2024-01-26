//#region Variables Globales
var _minDate, _maxDate, notification;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };
//#endregion

//#region Eventos

$(() => {
    setupControls();
    filterData(true);
});

$(window).resize(() => {
    setGridHeight("Listado", _gridMargin);
});

$("#Listado").on("click", ".action-new", function (e) {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: 0 } });
    wnd.open();
});

$("#Listado").on("click", ".action-edit", function (e) {
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: item.id } });
    wnd.open();
});

$("#Listado").on("click", ".action-delete", function (e) {
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    var item = grd.dataItem(row);
    showConfirm("¿Est&aacute; seguro que desea eliminar este Item?", function () {
        $.post(urlDelete, { Id: item.id }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage("Se ha eliminado el Item correctamente.");
            } else {
                showError(data.message);
            }
        });
    });
});

$("#Detail").on("click", ".save-item", function (e) {
    var form = $(this).closest("form");
    var validator = form.kendoValidator({ messages: { required: "*" } }).data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        $.post(urlEdit, { Item: item }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                var wnd = $("#Detail").data("kendoWindow");
                wnd.close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });
    }
});

//arregla la dirección de una carpeta interna que sino aparece codificada
$(document).on("click", ".k-imagebrowser>.k-filemanager-listview>.k-listview-content>.k-listview-item", function (e) {
    var input = $("#k-editor-image-url")[0];
    if (input && /%2F/.test(input.value)) {
        input.value = input.value.replace(/%2F/g, "/").replace("help/", "");
    }
});

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#Detail").kendoWindow({
        visible: false, width: 1500, title: "Item Manual de Usaurio", modal: true, close: onCloseWindow,
        refresh: function (e) {
            $("#Value").kendoEditor({
                resizable: { content: true },
                tools: [
                    "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent",
                    "createLink", "unlink", "insertImage", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
                ],
                imageBrowser: {
                    fileTypes: "*.png,*.gif,*.jpg,*.jpeg", path: "help",
                    transport: {
                        read: urlImageRead,
                        destroy: { url: urlImageDestroy, type: "POST" },
                        create: { url: urlImageCreate, type: "POST" },
                        uploadUrl: urlImageUpload,
                        imageUrl: urlImage,
                        type: "filebrowser-aspnetmvc"
                    }
                }
            });
            onRefreshWindow(e);
        }
    });

    $("#Listado").kendoGrid({
        columns: [
            { title: "Título", field: "title" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo Item"></i>',
                template: '<i class="fas fa-pen action action-link action-edit" title="Editar Item"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-link action-delete" title="Eliminar Item"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row", noRecords: { template: "No hay resultados para el criterio de búsqueda." },
        dataSource: []
    });
}

function filterData(pageLoad) {
    $.get(urlFilter, {}, function (data) {
        loadGrid(data.items);
        if (data.message !== "") {
            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
        }
    });
}

function loadGrid(items) {
    if (items) {
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({ data: items });
        grd.setDataSource(ds);
        setTimeout(() => { setGridHeight("Listado", _gridMargin) }, 200);
    }
}

//#endregion