//#region Global Variables
const _gridMargin = 20, _alignCenter = { "class": "k-text-center !k-justify-content-center" }, _alignRight = { style: "text-align: right;" }, _alignRight2 = { "class": "table-header-cell !k-text-right" },
    _userName = $("#username").val(), _userCode = $("#usercode").val(), _cardName = $("#cardname").val(), _cardCode = $("#cardcode").val(), _permission = $("#permission").val() == "Y",
    _states = [
        { id: "R", name: "Solicitado" }, { id: "A", name: "Aprobado" }, { id: "D", name: "Entregado" },
        { id: "F", name: "Terminado y devuelto" }, { id: "C", name: "Cancelado" }
    ];
var _gridItems, _wnd, _clients = [], _filClients, _filSince, _filUntil, _filProduct = $("#fil-product"), _filState = $("#fil-state"),
    _product, _quantity = $("#quantity"), _since, _until, _comments = $("#comments");
//#endregion

//#region Events

$(function () {
    setupControls();
    filterData();
});

$("#filter").click(onFilter);

$("#clean-filters").click(onCleanFilters);

$("#list-items").on("click", ".action-new, .action-edit, .action-delete", onEdit);

$("#cancel-detail").click(onCancelEdit);

$("#save-detail").click(onSave);

$(".change-state").click(onChangeState);

//#endregion

//#region Private Methods

function setupControls() {
    _filClients = $("#fil-client").kendoDropDownList({
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", label: "Clientes", virtual: { itemHeight: 26, valueMapper: clientMapper },
        dataSource: { data: _clients }
    }).data("kendoDropDownList");
    _filState.append(_states.map((x) => new Option(x.name, x.id)));
    _filState.multipleSelect();
    _filSince = $("#fil-since").kendoDatePicker({ label: "Desde" }).data("kendoDatePicker");
    _filUntil = $("#fil-until").kendoDatePicker({ label: "Hasta" }).data("kendoDatePicker");
    _filProduct = $("#fil-product");

    _since = $("#since").kendoDatePicker().data("kendoDatePicker");
    _until = $("#until").kendoDatePicker().data("kendoDatePicker");
    _product = $("#product").kendoDropDownList({
        dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", dataSource: { data: [] }, height: 350,
        template: (e) => `<span>${e.name}</span><br /><span>Marca: </span><span>${e.brand}</span><br /><span>Categor&iacute;a: </span><span>${e.category}</span>`
    }).data("kendoDropDownList");

    $.get(urlClients, {}, d => {
        _clients = d;
        _filClients.setDataSource(d);
    });

    _gridItems = $("#list-items").kendoGrid({
        noRecords: { "template": '<div class="w-100 text-center p-2">No existen resultados para el criterio de búsqueda.</div>' },
        scrollable: true, sortable: true, selectable: true,
        columns: [
            { field: "cardName", title: "Cliente", width: 120 },
            { field: "itemCode", title: "Cod.Producto", width: 100 },
            { field: "productName", title: "Descripción", width: 180, attributes: { "class": "no-wrap-column" } },
            { field: "initialDate", title: "Desde", width: 90, attributes: _alignCenter, headerAttributes: _alignCenter, format: "{0:dd-MM-yyyy}" },
            { field: "finalDate", title: "Hasta", width: 90, attributes: _alignCenter, headerAttributes: _alignCenter, format: "{0:dd-MM-yyyy}" },
            { field: "quantity", title: "Cantidad", width: 80, attributes: _alignRight2, headerAttributes: _alignRight },
            { field: "stateName", title: "Estado", width: 100, attributes: _alignCenter, headerAttributes: _alignCenter },
            { field: "comments", title: " ", attributes: _alignCenter, width: 35, template: (e) => e.comments != null && e.comments.trim() != "" ? `<a class="comments action-link" title="${e.comments}"><i class="fas fa-comment-alt"></i></a>` : "" },
            {
                field: "id", title: " ", attributes: _alignCenter, width: 70, sortable: false, headerAttributes: _alignCenter,
                headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nueva Solicitud Préstamo"></i>',
                template: function (e) {
                    var attrDisabled = e.state != "R" ? 'disabled="disabled"' : "", css = e.state != "R" ? "action disabled" : "action action-link action-delete",
                        result = `<a class="action action-link action-edit" title="Editar Solicitud de Préstamo"><i class="fas fa-pen"></i></a><a class="${css}" ${attrDisabled} title="Eliminar Solicitud de Préstamo"><i class="fas fa-trash-alt"></i></a>`;
                    return result;
                }
            }
        ],
        dataSource: getDS([])
    }).data("kendoGrid");

    $.get(urlProducts, {}, function (d) { _product.setDataSource(d.items); });

    _wnd = $("#item-detail").kendoWindow({ title: "Préstamo de Equipos", visible: false, scrollable: true, modal: true, width: 950, iframe: false, activate: onRefreshWindow }).data("kendoWindow");
}

function onFilter(e) {
    e.preventDefault();
    filterData();
}

function filterData() {
    let cardCode = _filClients.value(), since = _filSince.value(), until = _filUntil.value(),
        states = _filState.multipleSelect('getSelects').map(x => `'${x}'`).join(), product = _filProduct.val();
    if (since) since = kendo.toString(since, "yyyy-MM-dd");
    if (until) until = kendo.toString(until, "yyyy-MM-dd");
    $.get(urlFilter, { CardCode: cardCode, StartDate: since, EndDate: until, StateCodes: states, Product: product }, function (d) {
        if (d.message == "") {
            loadGrid(d.items);
        } else {
            console.error(d.message);
            showError('Se ha producido un error al traer los datos del servidor.');
        }
    });
}

function onCleanFilters(e) {
    e.preventDefault();
    cleanFilters();
}
function cleanFilters() {
    _filClients.value("");
    _filSince.value("");
    _filUntil.value("");
    _filState.multipleSelect('setSelects', []);
    _filProduct.val("");
}

function getDS(items) {
    return new kendo.data.DataSource({ data: items, schema: { model: { id: "id" } } });
}

function loadGrid(items) {
    if (items) {
        items.forEach(function (x) {
            x.requestDate = JSON.toDate(x.requestDate);
            x.initialDate = JSON.toDate(x.initialDate);
            x.finalDate = JSON.toDate(x.finalDate);
        });
        var ds = getDS(items);
        _gridItems.setDataSource(ds);
        if (items && items.length > 0) {
            $('#filter-box').collapse("hide");
        }
        setTimeout(() => setGridHeight("list-items", _gridMargin), 200);
    }
}

function clientMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.code === options.value));
    options.success(index);
}

function onEdit(e) {
    var item = _gridItems.dataItem($(this).closest("tr"));
    if ($(e.currentTarget).hasClass("action-delete")) {
        showConfirm(`¿Está seguro que desea eliminar la Solicitud del <b>${item.itemCode}</b>?`, function () {
            if (item) {
                $.post(urlDelete, { Id: item.id }, function (d) {
                    if (d.message == "") {
                        showMessage(`La Solicitud del <b>${item.itemCode}</b> ha sido eliminada con &eacute;xito.`);
                        _gridItems.dataSource.pushDestroy(item);
                    } else {
                        showError(`Se ha producido un error al intentar eliminar la solicitud: <br/>${d.message}`);
                    }
                });
            }
        });
    }
    if ($(e.currentTarget).hasClass("action-new") || $(e.currentTarget).hasClass("action-edit")) {
        if ($(e.currentTarget).hasClass("action-new")) {
            item = {
                id: 0, idProduct: 0, requestDate: new Date(), quantity: 1, initialDate: new Date(), finalDate: new Date(), state: "R", stateName: "En Creación", comments: "",
                idUser: _userCode, userName: _userName
            };
        }
        $("#Id").val(item.id);
        $("#IdUser").val(item.idUser);
        $("#RequestDate").val(kendo.toString(item.requestDate, "yyyy-MM-dd HH:mm:ss"));
        $("#State").val(item.state);
        _product.value(item.idProduct);
        $("#quantity").val(item.quantity);
        _since.value(item.initialDate);
        _until.value(item.finalDate);
        $("#comments").val(item.comments);
        $("#creator").text(`${item.userName} ( ${item.cardName} )`);
        $("#create-date").text(kendo.toString(item.requestDate, "dd-MM-yyyy HH:mm:ss"));
        $("#state-name").text(item.stateName);

        $("#save-detail").toggleClass("d-none", item.id != 0);
        $("#cancel-request").toggleClass("d-none", item.id == 0 || ["D", "F", "C"].includes(item.state) || !_permission);
        $("#aprove-request").toggleClass("d-none", item.id == 0 || item.state != "R" || !_permission);
        $("#delivered-request").toggleClass("d-none", item.id == 0 || item.state != "A" || !_permission);
        $("#finish-request").toggleClass("d-none", item.id == 0 || item.state != "D" || !_permission);

        _wnd.center().open();
    }
}

function onCancelEdit(e) {
    e.preventDefault();
    _wnd.close();
}

function onSave(e) {
    e.preventDefault();
    var form = $(this).closest("form"), validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var item = {
            id: $("#Id").val(), idUser: $("#IdUser").val(), requestDate: $("#RequestDate").val(), state: $("#State").val(), idProduct: _product.value(), productName: _product.text(),
            quantity: $("#quantity").val(), initialDate: _since.value(), finalDate: _until.value(), comments: $("#comments").val(), userName: $("#creator").text(),
            stateName: $("#state-name").text(), cardCode: _cardCode, cardName: _cardName, itemCode: _product.dataItem().itemCode
        };
        if (item.initialDate) item.initialDate = kendo.toString(item.initialDate, "yyyy-MM-dd");
        if (item.finalDate) item.finalDate = kendo.toString(item.finalDate, "yyyy-MM-dd");
        $.post(urlEdit, { Item: item }, function (d) {
            if (d.message == "") {
                item.id = d.id;
                _gridItems.dataSource.pushUpdate(item);
                _wnd.close();
            } else {
                console.error(d.message);
                showError(`Se ha producido un error al guardar los datos de la solicitud en el servidor.`);
            }
        });
    }
}

function onChangeState(e) {
    e.preventDefault();
    var buttonId = e.currentTarget.id, state = "R";
    if (buttonId == "cancel-request") state = "C";
    if (buttonId == "aprove-request") state = "A";
    if (buttonId == "delivered-request") state = "D";
    if (buttonId == "finish-request") state = "F";
    changeState(state);
}

function changeState(state) {
    var id = $("#Id").val();
    $.post(urlChangeState, { Id: id, State: state }, function (d) {
        if (d.message == "") {
            var name = 'La solicitud ha sido aprobada.', stateName = "Aprobada";
            if (state == "D") {
                name = "El producto solicitado ha sido entregado.";
                stateName = "Entregado";
            }
            if (state == "F") {
                name = "El producto solicitado ha sido devuelto y la solicitud terminada.";
                stateName = "Terminado y devuelto";
            }
            if (state == "C") {
                name = "La solicitud ha sido cancelada.";
                stateName = "Cancelada";
            }
            var item = _gridItems.dataSource.get(id);
            item.state = state;
            item.stateName = stateName;
            _gridItems.dataSource.pushUpdate(item);

            showMessage(name);
            _wnd.close();
        } else {
            console.error(d.message);
            showError(`Se ha producido un error al intentar cambiar el estado de la solicitud.<br />${d.message}`);
        }
    });
}

//#endregion