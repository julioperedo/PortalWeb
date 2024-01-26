//#region Variables Globales
const marginGrid = 30, alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy}", numberFormat = "{0:N2}";
var _intId = 0;
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", marginGrid));

$("#prices").on("click", ".action-edit", onEditPrice);

$("#cancel-price-esd").click(onCancelPriceESD);

$("#save-price-esd").click(onSavePriceESD);

$("#Listado").on("click", ".action-edit", onEdit);

$("#save-product").click(onSave);

//#endregion

//#region Métodos Locales

function setupControls() {
    $("#Listado").kendoGrid({
        columns: [
            { field: "sku", title: "Sku", width: 140 },
            { field: "itemCode", title: "Cod. DMC", width: 160 },
            { field: "name", title: "Nombre", width: 270 },
            //{ field: "returnType", title: "Devolución", width: 130 },
            { field: "fulfillmentType", title: "Tipo", width: 180 },
            { field: "cost", title: "Costo", format: "{0:N2}", width: 90, attributes: alignRight, headerAttributes: alignRight },
            { field: "price", title: "Precio", format: "{0:N2}", width: 90, attributes: alignRight, headerAttributes: alignRight },
            { field: "enabled", title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 90, template: e => e.enabled ? '<i class="fas fa-check"></i>' : '' },
            {
                field: "itemCode", title: " ", attributes: alignCenter, width: 20, sortable: false,
                template: e => $.trim(e.itemCode) !== "" ? '<i class="fas fa-pen action action-link action-edit" title="Editar Producto"></i>' : ''
            }
        ],
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 },
        messages: { noRecords: "No hay registros para el criterio de búsqueda." },
        dataSource: { schema: { /*data: "Data", total: "Total", errors: "Errors",*/ model: { id: "id" } }, data: [] }
    });

    $("#detail").kendoWindow({
        title: "Detalle Producto", visible: false, scrollable: true, modal: true, width: 850, iframe: false,
        activate: function (e) {
            var wnd = this;
            setTimeout(() => {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });

    $("#prices").kendoGrid({
        columns: [
            { field: "amount", title: "Monto", format: numberFormat, attributes: alignRight, headerAttributes: alignRight },
            { field: "currency", title: "Moneda" },
            { field: "priceType", title: "Tipo" },
            { field: "validFrom", title: "Desde", attributes: alignCenter, headerAttributes: alignCenter, format: dateFormat },
            { field: "validTo", title: "Hasta", attributes: alignCenter, headerAttributes: alignCenter, format: dateFormat },
            { field: "id", title: " ", width: 35, sortable: false, attributes: alignCenter, template: '<i class="fas fa-pen action action-link action-edit" title="Editar Producto"></i>' }
        ],
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 },
        messages: { noRecords: "No hay registros para el criterio de búsqueda." },
        dataSource: { schema: { /*data: "Data", total: "Total", errors: "Errors",*/ model: { id: "id" } }, data: [] }
    });

    filterData();
}

function filterData() {
    $.get(urlFilter, {}, function (data) {
        if (data.message !== "") {
            showError(data.message);
            loadGrid([]);
        } else {
            loadGrid(data.items);
        }
    });
}

function loadGrid(items) {
    var grd = $("#Listado").data("kendoGrid");
    var ds = new kendo.data.DataSource({ data: items });
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
    }
    setGridHeight("Listado", marginGrid);
}

function onEditPrice(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);

    $("#price-esd").val(item.amount);
    $("#section-edit-price-esd").removeClass("d-none");
    $("#save-product").attr("disabled", "disabled");
    $("#save-price-esd").data("id", item.id);
}

function onCancelPriceESD(e) {
    $("#section-edit-price-esd").addClass("d-none");
    $("#save-product").removeAttr("disabled");
}

function onSavePriceESD(e) {
    e.preventDefault();
    var d = $(e.currentTarget).data(), id = d.id, grid = $("#prices").data("kendoGrid"), ds = grid.dataSource, item = ds.get(id);
    console.log(ds.data());
    item.amount = +$("#price-esd").val();
    item.statusType = 2;
    ds.pushUpdate(item);
    $("#section-edit-price-esd").addClass("d-none");
    $("#save-product").removeAttr("disabled");
}

function onEdit(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);

    $("#sku").text(item.sku);
    $("#item-code").text(item.itemCode);
    $("#name").text(item.name);
    $("#description").text(item.description);
    $("#fulfillment-type").text(item.fulfillmentType);
    $("#return-type").text(item.returnType);
    $("#section-edit-price-esd").addClass("d-none");

    var grd = $("#prices").getKendoGrid();
    $.get(urlPrices, { Id: item.id, ItemCode: item.itemCode }, d => {
        if (d.message === "") {
            d.items.forEach(x => {
                if (x.validFrom) x.validFrom = JSON.toDate(x.validFrom);
                if (x.validTo) x.validTo = JSON.toDate(x.validTo);
            });
            var ds = new kendo.data.DataSource({ data: d.items, schema: { model: { id: "id" } } });
            grd.setDataSource(ds);

            $("#regular").val(d.price.regular);
            $("#offer").val(d.price.offer);
            $("#offerDescription").val(d.price.offerDescription);
            $("#observations").val(d.price.observations);
            $("#commentaries").val(d.price.commentaries);
            $("#id").val(d.price.id);
            $("#idProduct").val(d.price.idProduct);
            $("#idSudsidiary").val(d.price.idSudsidiary);
            $("#clientSuggested").val(d.price.clientSuggested);

            $("#detail").getKendoWindow().open();
        } else {
            console.error(d.message);
            showError("Se producido un error al traer los datos.");
        }
    });
}

function onSave(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject(), grid = $("#prices").data("kendoGrid"), ds = grid.dataSource, tempItems = ds.data();
        items = Enumerable.From(tempItems).Where("$.statusType === 2").Select("{ id: $.id, idProduct: $.idProduct, amount: $.amount, currency: $.currency, priceType: $.priceType, validFrom: kendo.toString($.validFrom, 'yyyy-MM-dd'), validTo: kendo.toString($.validTo, 'yyyy-MM-dd'), statusType: 2 }").ToArray(); //tempItems.find(x => x.statusType === 2);
        $.post(urlEdit, { Item: item, Prices: items }, d => {
            if (d.message === "") {
                $("#detail").getKendoWindow().close();
                showMessage("Datos guardados exitisamente.");
            } else {
                console.error(d.message);
                showError("Se producido un error al guardar los datos.");
            }
        });
    }
}

//#endregion