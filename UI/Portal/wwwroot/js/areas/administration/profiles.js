//#region Variables Globales
var newId = 0;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData(false));

$("#Listado").on("click", ".action-new", onEdit);

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-delete", onDelete);

$("#Detail").on("click", "#action-cancel", (e) => $("#Detail").data("kendoWindow").close());

$("#Detail").on("click", "#action-save", onSave);

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Descripción", field: "description" },
            { title: "Tipo", field: "type", hidden: true },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<a title="Nuevo Perfil" class="action action-link action-new"><i class="fas fa-plus"></i></a>',
                template: '<a title="Editar Perfil" class="action action-link action-edit"><i class="fas fa-pen"></i></a><a title="Eliminar Perfil" class="action action-link action-delete"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", dataSource: []
    });
    filterData(true);
}

function cleanFilters() {
    $("#filter").val("");
}

function getFilters() {
    var message = "", filter = $("#filter").val();
    return { message: message, data: { FilterData: filter } };
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
        setGridHeight("Listado", _gridMargin);
        if (!pageLoad) {
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
}

function loadGrid(items) {
    if (items) {
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({
            data: items, group: [{ field: "type", dir: "asc" }],
            sort: [{ field: "type", dir: "asc" }],
            schema: { model: { id: "id" } }
        });
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

function toBool(value) {
    return value === "true" || value === "True";
}

function onEdit(e) {
    e.preventDefault();
    var wnd = $("#Detail").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), item;
    if (this.classList.contains("action-new")) {
        grd.clearSelection();
        item = { id: 0 };
    } else {
        var row = $(e.currentTarget).closest("tr");
        grd.select(row);
        item = grd.dataItem(row);
    }
    wnd.refresh({ url: urlEdit, data: { Id: item.id } });
    wnd.center().open();
}

function onDelete(e) {
    e.preventDefault();
    var dataItem = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el Perfil <b>${dataItem.name}</b>?`, function () {
        var filtersData = getFilters();
        $.post(urlDelete, { Id: dataItem.id, FilterData: filtersData.FilterData }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage(`Se ha eliminado el Perfil <b>${dataItem.name}</b> correctamente.`);
            } else {
                showError(data.message);
            }
        });
    });
}

function onSave(e) {
    e.preventDefault();
    var form = $("#form-profile");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        item.isExternalCapable = $("#isExternalCapable").prop("checked");
        item.ListProfileActivitys = [];
        $(".activity-row").each(function (i, row) {
            var insertChk = $(row).find(".p-insert"), updateChk = $(row).find(".p-update"), deleteChk = $(row).find(".p-delete");
            if (toBool(insertChk.data("value")) !== insertChk.prop("checked") || toBool(updateChk.data("value")) !== updateChk.prop("checked") || toBool(deleteChk.data("value")) !== deleteChk.prop("checked")) {
                var status = insertChk.data("id") === 0 ? 1 : 2;
                item.ListProfileActivitys.push({ Id: insertChk.data("id"), IdActivity: insertChk.data("idactivity"), Insert: insertChk.prop("checked"), Update: updateChk.prop("checked"), Delete: deleteChk.prop("checked"), StatusType: status });
            }
        });
        item.ListProfilePages = [];
        $(".page-row").each(function (i, row) {
            var check = $(row).find("[type=checkbox]");
            if (toBool(check.data("value")) !== check.prop("checked")) {
                var status = check.prop("checked") ? 1 : 3;
                item.ListProfilePages.push({ Id: check.data("id"), IdPage: check.data("idpage"), StatusType: status });
            }
        });
        item.ListProfileCharts = [];
        $(".chart-row").each(function (i, row) {
            var check = $(row).find("[type=checkbox]");
            if (toBool(check.data("value")) !== check.prop("checked")) {
                var status = check.prop("checked") ? 1 : 3;
                item.ListProfileCharts.push({ Id: check.data("id"), IdChart: check.data("idchart"), StatusType: status });
            }
        });

        var filtersData = getFilters();
        $.post(urlEdit, { Item: item, FilterData: filtersData.FilterData }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                $("#Detail").data("kendoWindow").close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });
    }
}

//#endregion