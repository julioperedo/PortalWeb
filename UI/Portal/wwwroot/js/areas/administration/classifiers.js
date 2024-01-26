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

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse, shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData(false));

$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-delete", onDelete);

$("#Detail").on("click", ".action-new", onNewDetail);

$("#Detail").on("click", ".action-edit", onEditDetail);

$("#Detail").on("click", ".action-delete", onDeleteDetail);

$("#Detail").on("click", "#action-cancel-item", hideDetail);

$("#Detail").on("click", "#action-save-item", saveDetailItem);

$("#Detail").on("click", "#action-cancel", (e) => $("#Detail").data("kendoWindow").close());

$("#Detail").on("click", "#action-save", onSave);

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#Listado").kendoGrid({
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        columns: [
            { title: "Tipo", field: "name" },
            { title: "Descripción", field: "description" },
            { title: "Base", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(isBase) {# <i class="fas fa-check"></i> #} #', field: "isBase" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a class="action action-link action-new" title="Nuevo Clasificador"><i class="fas fa-plus"></i></a>',
                template: '<a class="action action-link action-edit" title="Editar Clasificador"><i class="fas fa-pen"></i></a><a class="action action-link action-delete" title="Eliminar Clasificador"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", dataSource: []
    });

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

function loadDetailItem(item) {
    $("#detail-id").val(item.id);
    $("#detail-name").val(item.name);
    $("#detail-description").val(item.description);
    $("#detail-value").val(item.value);
    $("#detail-isbase").prop("checked", item.isBase);

    $("#action-save").attr("disabled", true);
    $("#detail-classifier").removeClass("d-none");
    $("#Detail").data("kendoWindow").center();
}

function hideDetail(e) {
    $("#action-save").removeAttr("disabled");
    $("#detail-classifier").addClass("d-none");
    $("#Detail").data("kendoWindow").center();
}

function saveDetailItem(e) {
    var validator = $("#detail-form").kendoValidator({ messages: { required: "Campo requerido" } }).data("kendoValidator");
    if (validator.validate()) {
        var id = $("#detail-id").val(), name = $.trim($("#detail-name").val()), description = $.trim($("#detail-description").val()), value = $.trim($("#detail-value").val()), isbase = $("#detail-isbase").prop("checked");
        var items = JSON.parse($("#items").val());
        var item = Enumerable.From(items).Where("$.id === " + id).FirstOrDefault();
        if (item) {
            if ($.trim(item.name) !== name || $.trim(item.description) !== description || $.trim(item.value) !== value || item.isBase !== isbase) {
                item.name = name;
                item.description = description;
                item.value = value;
                item.isBase = isbase;
                if (item.statusType !== 1) item.statusType = 2;
            }
        } else {
            item = { id: getId(), name: name, description: description, isBase: isbase, value: value, statusType: 1 };
            items.push(item);
        }

        $("#ListItems").data("kendoGrid").setDataSource(new kendo.data.DataSource({ data: items.filter((x) => x.statusType !== 3) }));

        $("#items").val(JSON.stringify(items));
        hideDetail();
    }
}

function getId() {
    newId--;
    return newId;
}

function onNew(e) {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: 0 } });
    wnd.center().open();
}

function onEdit(e) {
    var wnd = $("#Detail").data("kendoWindow");
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    wnd.refresh({ url: urlEdit, data: { Id: item.id } });
    wnd.center().open();
}

function onDelete(e) {
    var dataItem = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    showConfirm("¿Está seguro que desea eliminar el Clasificador?", function () {
        var filtersData = getFilters();
        $.post(urlDelete, { Id: dataItem.id, FilterData: filtersData.FilterData }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage("Se ha eliminado el Clasificador correctamente.");
            } else {
                showError(data.message);
            }
        });
    });
}

function onRefresh(e) {
    var items = JSON.parse($("#items").val());
    $("#ListItems").kendoGrid({
        sortable: true, selectable: "Single, Row", height: 400, noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Base", attributes: alignCenter, headerAttributes: alignCenter, width: 70, template: '# if(isBase) {# <i class="fas fa-check"></i> #} #', field: "isBase" },
            { title: "Valor", field: "value", width: 80 },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<a class="action action-link action-new" title="Nuevo Item"><i class="fas fa-plus"></i></a>',
                template: '<a class="action action-link action-edit"><i class="fas fa-pen" title="Editar Item"></i></a><a class="action action-link action-delete" title="Eliminar Item"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        dataSource: items
    });
    onRefreshWindow(e);
}

function onNewDetail(e) {
    var item = { id: 0, name: "", description: "", isBase: false, value: "", statusType: 0 };
    loadDetailItem(item);
}

function onEditDetail(e) {
    var grid = $("#ListItems").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grid.select(row);
    var item = grid.dataItem(row);
    loadDetailItem(item);
}

function onDeleteDetail(e) {
    hideDetail();
    var grid = $("#ListItems").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    var item = grid.dataItem(row);
    var items = JSON.parse($("#items").val());
    showConfirm(`¿Está seguro que desea eliminar el item: <b>${item.name}</b>?`, function () {
        var x = Enumerable.From(items).Where("$.id === " + item.id).FirstOrDefault();
        if (x.statusType !== 1) {
            x.statusType = 3;
        } else {
            items = items.filter((i) => i.id !== x.id);
        }
        $("#ListItems").data("kendoGrid").setDataSource(new kendo.data.DataSource({ data: items.filter((x) => x.statusType !== 3) }));
        $("#items").val(JSON.stringify(items));
    });
}

function onSave(e) {
    var form = $("#form-classifier");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        var items = JSON.parse($("#items").val());
        item.listClassifiers = items.filter(x => x.statusType !== 0);
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