//#region Variables Globales
const marginGrid = 30, alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy}";
var _minDate, _maxDate, subsidiaries, products = [], clients = [];
//#endregion

//#region Eventos

$(() => setupControls());

$("#action-filter").click(filterData);

$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-delete", onDelete);

$("#Listado").on("click", ".check-one", onCheckOne);

$("#Listado").on("click", ".check-all", onCheckAll);

$("#save-request").click(onSave);

$("#action-email").click(onSendingEmail);

//#endregion

//#region Metodos

function setupControls() {
    $("#detail").kendoWindow({
        title: "Registrar Solicitud de Productos", visible: false, scrollable: true, modal: true, width: 750, iframe: false,
        activate: function (e) {
            var wnd = this;
            setTimeout(() => {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });

    var filSince = $("#fil-since").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#fil-until").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    var optionsProduct = {
        dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains",
        dataSource: new kendo.data.DataSource({ data: products, group: { field: "line" } }), virtual: { itemHeight: 45, valueMapper: productMapper },
        template: '<span class="k-state-default">#: data.name.replace(data.itemCode + " - ", "") #<p>#: data.itemCode #</p></span>'
    };
    var optionsClient = {
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains",
        dataSource: { data: clients }, virtual: { itemHeight: 45, valueMapper: clientMapper },
        template: '<span class="k-state-default">#: data.name.replace(data.code + " - ", "") #<p>#: data.code #</p></span>'
    };
    var optionSubsidiary = { dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Sucursal...", filter: "contains" };
    var ddlFilterProduct = $("#fil-product").kendoDropDownList(optionsProduct).data("kendoDropDownList"), ddlProduct = $("#IdProduct").kendoDropDownList(optionsProduct).data("kendoDropDownList"),
        ddlFilterClient = $("#fil-client").kendoDropDownList(optionsClient).data("kendoDropDownList"), ddlClient = $("#CardCode").kendoDropDownList(optionsClient).data("kendoDropDownList"),
        ddlFilterSubsidiary = $("#fil-subsidiary").kendoDropDownList(optionSubsidiary).data("kendoDropDownList"), ddlSubsidiary = $("#IdSubsidiary").kendoDropDownList(optionSubsidiary).data("kendoDropDownList");
    $("#fil-reported").kendoDropDownList();

    var columns = [
        { title: "Producto", width: 230, field: "productName", aggregates: ["count"], groupHeaderTemplate: 'Producto: #= value #    ( Total: #= count# )' },
        { title: "Cliente", field: "cardName", width: 220, attributes: { class: "text-nowrap" }, aggregates: ["count"], groupHeaderTemplate: 'Cliente: #= value #    ( Total: #= count# )' },
        { title: "Sucursal", width: 140, field: "subsidiaryName", aggregates: ["count"], groupHeaderTemplate: 'Solicitante: #= value #    ( Total: #= count# )' },
        { title: "Cantidad", field: "quantity", width: 80, attributes: alignRight, headerAttributes: alignRight },
        { title: "Fecha", field: "requestDate", attributes: alignCenter, headerAttributes: alignCenter, width: 90, format: dateFormat },
        { title: "Solicitante", width: 160, field: "userName", aggregates: ["count"], groupHeaderTemplate: 'Solicitante: #= value #    ( Total: #= count# )' },
        { title: "Reportado", field: "reported", attributes: alignCenter, headerAttributes: alignCenter, width: 60, template: e => e.reported ? '<i class="fas fa-check"></i>' : '', sortable: false }
    ];
    if (permission > 0) {
        columns.push({
            title: " ", width: 40, attributes: alignCenter, headerAttributes: alignCenter,
            template: checkTemplate, field: "id", sortable: false,
            headerTemplate: '<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-all" id="chk-all" name="check-all"><label class="custom-control-label" for="chk-all"></label></div>'
        });
    }
    columns.push({
        title: " ", width: 50, sortable: false, attributes: alignCenter, headerAttributes: alignCenter, sticky: true,
        headerTemplate: '<a class="action action-link action-new" title="Nuevo"><i class="fas fa-plus"></i></a>',
        template: '<a class="action action-link action-edit" title="Editar"><i class="fas fa-pen"></i></a><a class="action action-link action-delete" title="Eliminar"><i class="fas fa-trash"></i></a>'
    });
    $("#Listado").kendoGrid({
        columns: columns,
        groupable: { messages: { empty: "Arrastre un encabezado de columna y suelte acá para agrupar por esa columna" }, enabled: true },
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([]),
        dataBound: function (e) {
            var grid = e.sender;
            grid.columns.forEach((v, i) => grid.showColumn(i));
            $("div.k-group-indicator").each((i, v) => grid.hideColumn($(v).data("field")));
            grid.element.find("table").attr("style", "");
        }
    });

    $.get(urlProducts, {}, d => {
        products = Enumerable.From(d).OrderBy("$.line, $.name").ToArray();
        ddlFilterProduct.setDataSource(new kendo.data.DataSource({ data: products/*, group: { field: "line" }*/ }));
        ddlProduct.setDataSource(new kendo.data.DataSource({ data: products/*, group: { field: "line" }*/ }));
    });
    $.get(urlClients, {}, d => {
        clients = d;
        ddlFilterClient.setDataSource(d);
        ddlClient.setDataSource(d);
    });
    $.get(urlSubsidiaries, {}, d => {
        if (d.message === "") {
            ddlFilterSubsidiary.setDataSource(d.items);
            ddlSubsidiary.setDataSource(d.items);
        }
    });
    filterData();
}

function getDataSource(items) {
    items.forEach(x => x.requestDate = JSON.toDate(x.requestDate));
    var ds = new kendo.data.DataSource({
        data: items,
        sort: [{ field: "id", dir: "asc" }],
        schema: { model: { id: "id" } }
    });
    return ds;
}

function clientMapper(options) {
    var items = clients || []; //this.dataSource.data();
    var index = items.findIndex(x => x.code === options.value);
    options.success(index);
}

function productMapper(options) {
    var items = products || []; //this.dataSource.data();
    var index = items.findIndex(i => i.id === options.value);
    options.success(index);
}

function checkTemplate(e) {
    var template = "";
    if (e.reported === false) {
        template = `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-one" id="chk-one-${e.id}" value="${e.id}"><label class="custom-control-label" for="chk-one-${e.id}"></label></div>`;
    }
    return template;
}

function onCheckOne(e) {
    $("#action-email").toggleClass("d-none", $(".check-one:checked").length === 0);
    $("#check-all").prop("checked", $(".check-one:checked").length === $(".check-one").length);
}

function onCheckAll(e) {
    if ($(".check-one:not(:disabled)").length > 0) {
        $(".check-one:not(:disabled)").prop("checked", e.target.checked);
        $("#action-email").toggleClass("d-none", !e.target.checked);
    } else {
        e.target.checked = false;
    }
}

function filterData(e) {
    var filters = getFilters();
    if (filters.message === "") {
        $.get(urlFilter, filters.data, function (d) {
            if (d.message === "") {
                loadGrid(d.items);
            } else {
                console.error(d.message);
                showError(`Ha ocurrido un error al traer los datos del servidor.`);
            }
        });
    } else {
        showInfo(filters.message);
    }
}

function getFilters() {
    var productId = $("#fil-product").data("kendoDropDownList").value(), subsidiaryId = $("#fil-subsidiary").data("kendoDropDownList").value(), cardCode = $("#fil-client").data("kendoDropDownList").value(),
        since = $("#fil-since").data("kendoDatePicker").value(), until = $("#fil-until").data("kendoDatePicker").value(), reported = $("#fil-reported").data("kendoDropDownList").value(), message = "";
    if (reported !== "N" & (since == null || until == null)) {
        message += "Debe seleccionar ambas fechas para poder realizar la búsqueda";
    } else {
        if (since) since = kendo.toString(since, "yyyy-MM-dd");
        if (until) until = kendo.toString(until, "yyyy-MM-dd");
    }
    return { message, data: { ProductId: productId, SubsidiaryId: subsidiaryId, CardCode: cardCode, Since: since, Until: until, Reported: reported } };
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
    setTimeout(() => setGridHeight("Listado", marginGrid), 200);
}

function loadDetail(item) {
    $("#Id").val(item.id);
    $("#IdUser").val(item.idUser);
    $("#RequestDate").val(kendo.toString(item.requestDate, "yyyy-MM-dd"));
    $("#Reported").val(item.reported);
    $("#IdSubsidiary").data("kendoDropDownList").value(item.idSubsidiary);
    $("#IdProduct").data("kendoDropDownList").value(item.idProduct);
    $("#CardCode").data("kendoDropDownList").value(item.cardCode);
    $("#Quantity").val(item.quantity);
    $("#Description").val(item.description);

    $("#detail").data("kendoWindow").open();
}

function onNew(e) {
    var item = { id: 0, idUser: 0, requestDate: new Date(), reported: false, idProduct: 0, idSubsidiary: 0, cardCode: "", quantity: 1, description: "" };
    loadDetail(item);
}

function onEdit(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);
    loadDetail(item);
}

function onSave(e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var validator = form.kendoValidator({ messages: { required: "" } }).data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        $.post(urlEdit, { Item: item }, function (d) {
            if (d.message === "") {
                item = d.item;
                item.productName = $("#IdProduct").data("kendoDropDownList").dataItem().name;
                var clientDataItem = $("#CardCode").data("kendoDropDownList").dataItem();
                item.cardName = clientDataItem.name.replace(clientDataItem.code + " - ", "");
                item.subsidiaryName = $("#IdSubsidiary").data("kendoDropDownList").text();
                item.requestDate = new Date(item.requestDate);
                $("#Listado").data("kendoGrid").dataSource.pushUpdate(item);
                $("#detail").data("kendoWindow").close();
            } else {
                console.error(d.message);
                showError("Se ha producido un error al guardar los datos en el servidor.");
            }
        });
    }
}

function onDelete(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    showConfirm(`¿Está seguro que desea eliminar la solicitud?`, function () {
        $.post(urlDelete, { Id: item.id }, function (d) {
            if (d.message === "") {
                var ds = grd.dataSource;
                ds.remove(item);
            } else {
                console.error(d.message);
                showError(`Se ha producido un error al eliminar la solicitud.`);
            }
        });
    });
}

function onSendingEmail(e) {
    var ids = Enumerable.From($(".check-one:checked")).Select("+$.value").ToArray();
    var values = ids.join();

    showConfirm(`¿Está seguro que desea enviar las solicitudes seleccinadas a sus respectivos Gerentes de Producto?`, function (e) {
        var grd = $("#Listado").data("kendoGrid");
        $.post(urlSendEmail, { Ids: values }, function (d) {
            if (d.message === "") {
                ids.forEach((v, i) => grd.dataSource.remove(grd.dataSource.get(v)));
                showMessage(`Se han enviado los correos exitosamente.`);
            } else {
                console.error(d.message);
                showError(`Se ha producido un error al intentar enviar los correos.`)
            }
        });
    });
}

//#endregion