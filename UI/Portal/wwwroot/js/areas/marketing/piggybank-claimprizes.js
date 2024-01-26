//#region Gobal variables
var _wndDetail, _grdItems, _ddlUsers, _ddlPrizes, _txtQuantity, _dpDate;
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

$("#Listado").on("click", ".action-delete", onDelete);

$("#Detail").on("click", ".action-cancel", onCancel);

$("#Detail").on("click", ".action-save", onSave);

//#endregion

//#region Private Methods

function setupPage() {
    _grdItems = $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Usuario", field: "user" },
            { title: "Premio", field: "prize" },
            { title: "Fecha", field: "claimDate", width: 100, attributes: alignCenter, headerAttributes: alignCenter, format: dateFormat },
            { title: "Cantidad", field: "quantity", width: 85, attributes: alignRight, headerAttributes: alignRight, format: "{0:N0}" },
            { title: "Monedas", field: "points", width: 85, attributes: alignRight, headerAttributes: alignRight, format: "{0:N0}" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a title="Nuevo Canje" class="action action-link action-new"><i class="fas fa-plus"></i></a>',
                template: '<a title="Editar Canje" class="action action-link action-edit"><i class="fas fa-pen"></i></a><a title="Eliminar Canje" class="action action-link action-delete"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", dataSource: { schema: { model: { id: "id" } }, data: [] }
    }).data("kendoGrid");
    filterData(true);

    var calculatePoints = function (quantity, points) {
        $("#points-label").text(`${quantity * points} monedas`);
        $("#points").val(quantity * points);
    };

    _ddlUsers = $("#idUser").kendoDropDownList({
        dataSource: { transport: { read: { url: urlUsers } } },
        optionLabel: "Seleccione un Usuario", filter: "contains", dataTextField: "name", dataValueField: "id"
    }).data("kendoDropDownList");
    _ddlPrizes = $("#idPrize").kendoDropDownList({
        dataSource: { transport: { read: { url: urlPrizes } } },
        optionLabel: "Seleccione un Premio", filter: "contains", dataTextField: "name", dataValueField: "id",
        height: 500,
        template: (i) => `<div class="prize-item"><div class="picture"><img src="${urlPath}${i.imageUrl}" /></div><div><span class="name">${i.name}</span><br /><span class="coins">${i.points} monedas</span></div></div>`,
        change: function (e) {
            calculatePoints(_txtQuantity.value(), e.sender.dataItem().points);
        }
    }).data("kendoDropDownList");
    _txtQuantity = $("#quantity").kendoNumericTextBox({
        decimals: 0, format: "N0", change: function () {
            calculatePoints(this.value(), _ddlPrizes.dataItem().points);
        }
    }).data("kendoNumericTextBox");
    _dpDate = $("#claimDate").kendoDatePicker().data("kendoDatePicker");

    _wndDetail = $("#Detail").kendoWindow({ width: 550, title: "Detalle de Canje", visible: false, modal: true, activate: onRefreshWindow }).data("kendoWindow");
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
        items.forEach((x) => x.claimDate = JSON.toDate(x.claimDate));
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

function onDelete(e) {
    e.preventDefault();
    var item = _grdItems.dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el Canje de <b>${item.user}</b>?`, function () {
        $.post(urlDelete, { Id: item.id }, function (d) {
            if (d.message == "") {
                showMessage(`Se ha eliminado el Canje de <b>${item.user}</b> correctamente.`);
                _grdItems.dataSource.remove(item);
            } else {
                console.error(d.message);
                showError('Se ha producido un error al intentar eliminar el Canje');
            }
        });
    });
}

function onEdit(e) {
    e.preventDefault();
    var item;
    if (this.classList.contains("action-new")) {
        _grdItems.clearSelection();
        item = { id: 0, idPrize: 0, idUser: 0, claimDate: new Date(), quantity: 1, points: 0 };
    } else {
        var row = $(e.currentTarget).closest("tr");
        _grdItems.select(row);
        item = _grdItems.dataItem(row);
    }
    
    $("#id").val(item.id);
    _ddlPrizes.value(item.idPrize);
    _ddlUsers.value(item.idUser);
    _txtQuantity.value(item.quantity);
    _dpDate.value(item.claimDate);
    $("#points").val(item.points);
    $("#points-label").text(`${item.points} monedas`);

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
        var item = form.serializeObject(), user = _ddlUsers.dataItem();
        item.claimDate = kendo.toString(_dpDate.value(), "yyyy-MM-dd");
        item.user = user.name;
        item.prize = _ddlPrizes.text();

        if (user.points >= item.points) {
            saveClaim(item);
        } else {
            showConfirm(`El usuario <b>${user.name}</b> no tiene las monedas necesarios para realizar el canje, el usuario necesita <b>${item.points}</b> monedas y solamente tiene <b>${user.points}</b> acumuladas.<br /> ¿Desea continuar con el canje?`, function () {
                saveClaim(item);
            });
        }
    }
}

function saveClaim(item) {
    $.post(urlEdit, { Item: item }, function (d) {
        if (d.message == "") {
            _wndDetail.close();
            showMessage("Se realizaron los cambios correctamente.");
            item.id = d.id;
            item.claimDate = _dpDate.value();
            _grdItems.dataSource.pushUpdate(item);
        } else {
            showError(d.message);
        }
    });
}

//#endregion