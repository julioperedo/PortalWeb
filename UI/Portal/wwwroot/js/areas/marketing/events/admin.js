//#region Variables Globales
var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };
//#endregion

//#region Eventos

$(() => initForm());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData(false));

$("#Listado").on("click", ".new-item", (e) => openDetail());

$("#Listado").on("click", ".action-edit", (e) => openDetail(e));

$("#Listado").on("click", ".action-delete", (e) => onDelete(e));

$("#DetailEvent").on("click", "#cancel-event", (e) => { e.preventDefault(), $("#DetailEvent").data("kendoWindow").close(); });

$("#DetailEvent").on("click", "#save-event", (e) => onSaveEvent(e));

//arregla la dirección de una carpeta interna que sino aparece codificada
$(document).on("click", ".k-imagebrowser>.k-filemanager-listview>.k-listview-content>.k-listview-item", (e) => fixFolderUrl(e));

//#endregion

//#region Metodos Privados

function initForm() {
    setupControls();
    setTimeout(function () { setGridHeight("Listado", _gridMargin) }, 800);
    filterData(true);
}

function setupControls() {
    var finalDatePicker = $("#final-date").data("kendoDatePicker"), initialDatePicker = $("#initial-date").data("kendoDatePicker");
    _maxDate = finalDatePicker.max();
    _minDate = initialDatePicker.min();
    finalDatePicker.min(initialDatePicker.value());
    initialDatePicker.max(finalDatePicker.value());

    $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Nombre", width: 350, field: "name" },
            { title: "Descripción", field: "description" },
            { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 120, field: "date", format: dateFormat },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action new-item" title="Nuevo Evento"></i>',
                template: '<i class="fas fa-pen action action-edit" title="Editar Evento"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-delete" title="Eliminar Evento"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row",
        dataSource: {
            schema: {
                model: { id: "id", fields: { "id": { type: "number" }, "name": { type: "string" }, "description": { type: "string" }, "date": { type: "date" } } }
            }, data: []
        }
    });
}

function cleanFilters() {
    var today = new Date();
    $("#initial-date").data("kendoDatePicker").value(today.addMonths(-1)), $("#final-date").data("kendoDatePicker").value(today);
}

function getFilters() {
    var message = "", initialDate = $("#initial-date").data("kendoDatePicker").value(), finalDate = $("#final-date").data("kendoDatePicker").value();
    if (initialDate) initialDate = initialDate.toISOString();
    if (finalDate) finalDate = finalDate.toISOString();
    if (initialDate === "" & finalDate === "") {
        message = "Debe escoger algún criterio de búsqueda.";
    }
    return { message: message, data: { InitialDate: initialDate, FinalDate: finalDate } };
}

function filterData(pageLoad) {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        $.get(urlFilter, filtersData.data, function (data) {
            loadGrid(data.items);
            if (data.message !== "") {
                showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
            }
        });
    } else {
        if (!pageLoad) {
            setGridHeight("Listado", _gridMargin);
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
}

function loadGrid(items) {
    if (items) {
        items.forEach(x => { x.date = JSON.toDate(x.date); });
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({ data: items });
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

//Evento que se dispara cuando se modifica le fecha inicial
function onSinceChange(e) {
    var startDate = this.value();
    if (startDate === null) this.value("");
    $(e.sender.element).closest(".row").find("input.final-date").data("kendoDatePicker").min(startDate ? startDate : _minDate);
}

//Evento que se dispara cuando se modifica le fecha final
function onUntilChange(e) {
    var endDate = this.value();
    if (endDate === null) this.value("");
    $(e.sender.element).closest(".row").find("input.initial-date").data("kendoDatePicker").max(endDate ? endDate : _maxDate);
}

function openDetail(e) {
    var wnd = $("#DetailEvent").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), id = 0;
    if (e) {//Edit
        var row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), id = item.id;
        grd.select(row);
    } else {//New
        grd.clearSelection();
    }
    wnd.refresh({ url: urlEdit, data: { Id: id } });
    wnd.center().open();
}

function onRefresh(e) {
    $("#Detail").kendoEditor({
        resizable: { content: true }, encoded: false,
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "insertImage", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        imageBrowser: {
            fileTypes: "*.png,*.gif,*.jpg,*.jpeg", path: "events",
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

function onDelete(e) {
    var dataItem = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    showConfirm("¿Está seguro que desea aliminar el evento?", function () {
        var strInitial = $("#initial-date").getKendoDatePicker().value(), strFinal = $("#final-date").getKendoDatePicker().value();
        if (strInitial) {
            strInitial = strInitial.toISOString();
        }
        if (strFinal) {
            strFinal = strFinal.toISOString();
        }
        $.post(urlDelete, { Id: dataItem.id, InitialDate: strInitial, FinalDate: strFinal }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage("Se ha eliminado el Evento correctamente.");
            } else {
                showError(data.message);
            }
        });
    });
}

function onSaveEvent(e) {
    e.preventDefault();
    var form = $(e.target).closest("form");
    var validator = form.kendoValidator(objConfig).data("kendoValidator");
    if (validator.validate()) {
        var event = form.serializeObject();
        var strInitial = $("#initial-date").getKendoDatePicker().value(), strFinal = $("#final-date").getKendoDatePicker().value();
        if (strInitial) strInitial = strInitial.toISOString();
        if (strFinal) strFinal = strFinal.toISOString();
        $.post(urlEdit, { Item: event, InitialDate: strInitial, FinalDate: strFinal }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                $("#DetailEvent").data("kendoWindow").close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });
    }
}

function fixFolderUrl(e) {
    var input = $("#k-editor-image-url")[0];
    if (input && /%2F/.test(input.value)) {
        input.value = input.value.replace(/%2F/g, "/").replace("events/", "");
    }
}

//#endregion