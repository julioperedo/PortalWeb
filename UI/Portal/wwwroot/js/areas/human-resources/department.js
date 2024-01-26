//#region Gobal variables
var _wndDetail, _grdItems, _swEnabled;
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Events

$(() => setupPage());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData(false));

$("#Listado").on("click", ".action-new, .action-edit", onEdit);

$("#Detail").on("click", ".action-cancel", onCancel);

$("#Detail").on("click", ".action-save", onSave);

//#endregion

//#region Private Methods

function setupPage() {
    _grdItems = $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Habilitado", field: "enabled", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: (e) => e.enabled ? '<i class="fas fa-check"></i>' : '' },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a title="Nueva Area de Trabajo" class="action action-link action-new"><i class="fas fa-plus"></i></a>',
                template: '<a title="Editar Area de Trabajo" class="action action-link action-edit"><i class="fas fa-pen"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", dataSource: { schema: { model: { id: "id" } }, data: [] }
    }).data("kendoGrid");
    filterData(true);


    _wndDetail = $("#Detail").kendoWindow({ width: 500, title: "Detalle de Area de Trabajo", visible: false, modal: true, activate: onRefreshWindow }).data("kendoWindow");
    _swEnabled = $("#enabled").kendoSwitch({ messages: { checked: "", unchecked: "" }, size: "small" }).data("kendoSwitch");
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
    if (filtersData.message == "") {
        $.get(urlFilter, filtersData.data, function (data) {
            loadGrid(data.items);
            if (data.message != "") {
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
            data: items,
            schema: { model: { id: "id" } }
        });
        grd.setDataSource(ds);
        if (items && items.length > 0) {
            $('#filter-box').collapse("hide");
            $("#action-excel").removeClass("d-none");
        } else {
            $("#action-excel").addClass("d-none");
        }
        setTimeout(() => setGridHeight("Listado", _gridMargin), 100);
    }
}

function onEdit(e) {
    e.preventDefault();
    var item;
    if (this.classList.contains("action-new")) {
        _grdItems.clearSelection();
        item = { id: 0, name: "", enabled: true };
    } else {
        var row = $(e.currentTarget).closest("tr");
        _grdItems.select(row);
        item = _grdItems.dataItem(row);
    }

    $("#id").val(item.id);
    $("#name").val(item.name);
    _swEnabled.check(item.enabled);

    _wndDetail.center().open();
}

function onCancel(e) {
    e.preventDefault();
    _wndDetail.close();
}

function onSave(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();

        $.post(urlEdit, { Item: item }, function (d) {
            if (d.message == "") {
                _wndDetail.close();
                showMessage("Se realizaron los cambios correctamente.");
                item.id = d.id;
                _grdItems.dataSource.pushUpdate(item);
            } else {
                showError(d.message);
            }
        });
    }
}

//#endregion