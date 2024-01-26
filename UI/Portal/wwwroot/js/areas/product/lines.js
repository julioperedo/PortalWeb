//#region Variables Globales
const marginGrid = 30;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" };
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", marginGrid));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", marginGrid));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", marginGrid));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData());

$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-delete", onDelete);

$("#Detail").on("click", ".action-save", onSave);

//#endregion

//#region Métodos Locales

function setupControls() {
    $("#Listado").kendoGrid({
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Descripci&oacute;n", field: "description" },
            { title: "Detalle", field: "hasDetail", attributes: alignCenter, headerAttributes: alignCenter, width: 70, template: '# if(hasDetail) {# <i class="fas fa-check"></i> #} #' },
            { title: "Tipo Filtro", field: "filterType", attributes: alignCenter, headerAttributes: alignCenter, width: 200, template: e => e.filterType == "AllBut" ? 'Todos menos selección' : 'Ninguno menos selección' },
            { title: "Mostrar data en filtrado", field: "whenFilteredShowInfo", attributes: alignCenter, headerAttributes: alignCenter, width: 200, template: '# if(whenFilteredShowInfo) {# <i class="fas fa-check"></i> #} #' },
            { title: " ", field: "imageURL", attributes: { "style": "text-align: center;" }, width: 50, template: '# if(imageURL != null) {# <i class="fas fa-image" title="Tiene Imagen"></i> #} #' },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<a class="action action-link action-new" title="Nueva línea"><i class="fas fa-plus"></i></a>',
                template: '<a class="action action-link action-edit" title="Editar línea"><i class="fas fa-pen"></i></a><a class="action action-link action-delete" title="Eliminar línea"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", scrollable: { height: "200px" },
        messages: { noRecords: "No hay registros para el criterio de búsqueda." },
        dataSource: {
            schema: { data: "Data", total: "Total", errors: "Errors", model: { id: "id" } },
            data: []
        }
    });
    filterData();
    setGridHeight("Listado", marginGrid);
}

function cleanFilters() {
    $("#FilName").val("");
}

function loadGrid(items) {
    var grd = $("#Listado").data("kendoGrid");
    var ds = getDataSource(items);
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
    } else {
        $("#action-excel").addClass("d-none");
    }
    setGridHeight("Listado", marginGrid);
}

function filterData() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        $.get(urlFilter, filtersData.data, function (data) {
            if (data.message !== "") {
                showError(data.message);
                loadGrid([]);
            } else {
                loadGrid(data.items);
            }
        });
    } else {
        showInfo(`Debe seleccionar los siguientes campos: <br />${filtersData.message}`);
    }
}

function getDataSource(items) {
    return new kendo.data.DataSource({ data: items, pageSize: 500 });
}

function getFilters() {
    var name = $("#FilName").val(), message = "";
    return { message: message, data: { filterData: name } };
}

function onSelect(e) {
    var strExt = e.files[0].extension.toLowerCase();
    var ValidExtensions = [".jpg", ".gif", ".png", ".bmp"];

    if (Enumerable.From(ValidExtensions).Where(`$ === '${strExt}'`).Count() === 0) {
        e.preventDefault();
        showInfo("Debe ser una imagen.");
    }
}

function onUploadSucceded(e) {
    var files = e.files;
    if (e.operation === "upload") {
        $.each(files, function (i, file) {
            $("#Photo").attr("src", urlImages + file.name);
            $("#DeletePhoto").hide();
            $("#NewImageURL").val(file.name);
        });
    } else {
        var ImageURL = $("#ImageURL").val();
        $("#NewImageURL").val("");
        if (ImageURL === "") {
            $("#Photo").attr("src", urlNoImage);
            $("#DeletePhoto").hide();
        } else {
            $("#Photo").attr("src", urlImages + ImageURL);
            $("#DeletePhoto").show();
        }
    }
}

function onRefresh(e) {
    $("#Footer").kendoEditor({
        resizable: { content: true },
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        messages: { dialogCancel: "Cancelar", dialogInsert: "Agregar", dialogUpdate: "Aceptar", viewHtml: "Ver HTML" }
    });
    e.sender.center();

    var maxheight = 0;
    tabs = $('.tab-content.main-tab > .tab-pane');
    $.each(tabs, function () {
        this.classList.add("active"); /* make all visible */
        maxheight = (this.clientHeight > maxheight ? this.clientHeight : maxheight);
        if (!$(this).hasClass("show")) {
            this.classList.remove("active"); /* hide again */
        }
    });

    var data = JSON.parse($("#item-data").val());
    $("#SAPLines").kendoMultiSelect({
        dataTextField: "name",
        dataValueField: "id",
        value: data.SAPLines,
        placeholder: "Seleccione las Líneas", filter: "contains",
        dataSource: { serverFiltering: false, transport: { read: { url: urlSAPLines, data: function () { return { LineId: $("#Id").val() } } } } },
    });
    $.each(tabs, function () {
        $(this).height(maxheight);
    });
}

function onNew(e) {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: 0 } });
    wnd.open();
}

function onEdit(e) {
    var grd = $("#Listado").data("kendoGrid");
    var item = grd.dataItem($(this).closest("tr"));

    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: item.id } });
    wnd.open();
}

function onDelete(e) {
    var grd = $("#Listado").data("kendoGrid");
    var item = grd.dataItem($(this).closest("tr"));
    var filters = getFilters();
    showConfirm(`¿Está seguro que desea eliminar la línea <b>${item.name}</b>?`, () => {
        $.post(urlDelete, { Id: item.id, Filters: filters.data }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage(`Se ha eliminado la línea <b>${item.name}</b> correctamente.`);
            } else {
                showError(data.message);
            }
        });
    });
}

function onSave(e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var line = form.serializeObject();
        line.Footer = $("#Footer").getKendoEditor().value();
        line.SAPLines = $("#SAPLines").data("kendoMultiSelect").value();

        $.post(urlEdit, { Line: line }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                $("#FilName").val("");
                $("#NewImageURL").val("");

                $("#Detail").data("kendoWindow").close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });
    }
}

//#endregion