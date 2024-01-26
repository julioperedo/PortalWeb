//#region Variables Globales
var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 15, localUser = $("#LocalUser").val() === "Y";
//#endregion

//#region Eventos

$(() => {
    setupControls();
    setTimeout(function () { setGridHeight("Listado", _gridMargin) }, 800);
});

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

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

$(".chk-state").click(function () {
    if (this.checked === false) {
        $(this.id === "ord-open" ? "#ord-close" : "#ord-open").prop("checked", true);
    }
});

$("#action-clean").click(cleanFilters);

$("#action-filter").click(filterData);

$("#action-excel").click(exportExcel);

$("#Listado").on("click", ".order", function (e) {
    var wnd = $("#Report").data("kendoWindow"), grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), billNumber, item;
    grd.select(row);
    billNumber = $(this).text();
    item = grd.dataItem(row);
    wnd.title(`Orden ${billNumber} de ${item.providerName}`);
    loadReport(billNumber, item.subsidiary, "Order");
    wnd.open().center();
});

$("#Listado").on("click", ".bill", function (e) {
    var wnd = $("#Report").data("kendoWindow");
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var billNumber = $(this).text();
    var item = grd.dataItem(row);
    wnd.title(`Factura ${billNumber} de ${item.providerName}`);
    loadReport(billNumber, item.subsidiary, "Bill");
    wnd.open().center();
});

$("#Listado").on("click", ".item-code", function (e) {
    var itemCode = $(this).text();
    var grd = $(this).closest(".k-grid").parent().closest(".k-grid").data("kendoGrid");
    var row = $(this).closest(".k-grid").parent().closest("tr").prev();
    grd.select(row);
    var item = grd.dataItem(row);

    var lstItemCodes = ["FLETES", "ENVIO", "DMCSERVICIOS"];
    if (lstItemCodes.indexOf(itemCode) == -1) {
        var wnd = $("<div>").kendoWindow({ width: 750, title: "Detalle Stock" }).data("kendoWindow");
        wnd.refresh({ url: urlGetStock, data: { Subsidiary: item.subsidiary, Warehouse: item.warehouse, ItemCode: itemCode } });
        wnd.open().center();
    }
});

$("#Listado").on("click", ".open-file", openFile);

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#category").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Categoría...", filter: "contains", dataSource: { transport: { read: { url: urlCategories } } } });
    $("#subcategory").kendoDropDownList({
        dataSource: { serverFiltering: true, transport: { read: { url: urlSubcategories, data: getSubcategoriesFilter } } },
        cascadeFrom: "category", enable: false, optionLabel: "Seleccione una Subcategoría...", filter: "contains", dataTextField: "name", dataValueField: "id"
    });
    $("#line").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Línea...", filter: "contains", dataSource: { transport: { read: { url: urlLines } } } });
    $.get(urlSubsidiaries, {}, function (data) {
        var subsidiaries = $("#subsidiary"), warehouses = $("#warehouse");
        warehouses.multipleSelect();
        $.each(data.items, function (i, obj) {
            subsidiaries.append(new Option(obj.name, obj.name));
        });
        subsidiaries.multipleSelect({
            onUncheckAll: () => warehouses.empty().multipleSelect("refresh").multipleSelect("disable"),
            onCheckAll: () => loadWarehouses(subsidiaries, warehouses),
            onClick: (view) => loadWarehouses(subsidiaries, warehouses)
        }).multipleSelect("checkAll");
    });
    $.get(urlProviders, {}, function (data) {
        var provider = $("#provider");
        data.forEach(x => { provider.append(new Option(x.name, x.code)) });
        provider.multipleSelect({ filter: true });
    });

    $("#Listado").kendoGrid({
        sortable: true,
        pageable: true,
        selectable: true,
        groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" } },
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        detailInit: function (e) {
            $.get(urlDetail, { Subsidiary: e.data.subsidiary, OrderNumber: e.data.docNumber }, function (data) {
                $("<div>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: data.items },
                    scrollable: false,
                    sortable: true,
                    pageable: false,
                    selectable: true,
                    columns: [
                        { field: "itemCode", title: "Cod.Producto", width: 110, template: '<a href="\\#" class="item-code action action-link">#: itemCode #</a>' },
                        { field: "itemName", title: "Producto", width: 250 },
                        { field: "quantity", title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: 80 },
                        { field: "openQuantity", title: "Cant. Abierta", attributes: alignRight, headerAttributes: alignRight, width: 90 },
                        { field: "price", title: "P. Unirario", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 110 },
                        { field: "subtotal", title: "Subtotal", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 110 }
                    ]
                });
                if (data.message !== "") showError(data.message);
            });
        },
        columns: [
            { field: "subsidiary", aggregates: ["count"], title: "Sucursal", groupHeaderTemplate: "Sucursal: #= value #    ( Total: #= count#,  Monto Total: #= kendo.toString(aggregates.total.sum, 'N2') # )", width: 150 },
            { field: "warehouse", aggregates: ["count"], title: "Almacén", groupHeaderTemplate: "Almacén: #= value #    ( Total: #= count#,  Monto Total: #= kendo.toString(aggregates.total.sum, 'N2') # )", width: 150 },
            { field: "providerName", aggregates: ["count"], title: "Proveedor", groupHeaderTemplate: "Proveedor: #= value #    ( Total: #= count#,  Monto Total: #= kendo.toString(aggregates.total.sum, 'N2') # )", width: 250 },
            { field: "docNumber", title: "# Orden", width: 80, template: '<a class="order action action-link">#: docNumber #</a>' },
            { field: "docDate", title: "F. Orden Compra", format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter, width: 130 },
            { field: "estimatedDate", title: "F. Estimada", format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter, width: 100 },
            { field: "terms", title: "Términos Pago", width: 120 },
            { field: "otherCosts", title: "Gastos Adicionales", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 140 },
            { field: "quantity", title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: 100 },
            { field: "openQuantity", title: "Cant. Abierta", attributes: alignRight, headerAttributes: alignRight, width: 100 },
            { field: "total", aggregates: ["sum"], title: "Total", format: "{0:N2}", footerTemplate: "#=kendo.toString(sum, 'N2')#", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 110 },
            { field: "items", title: "Facturas", width: 145, template: (x) => x.items.map((i) => `<a class="bill action action-link">${i.billNumber}</a>${(i.filePath && i.fileName ? `<a class="open-file action action-link" data-path="${i.filePath}" data-name="${i.fileName}" title="Factura f&iacute;sica de ${i.billNumber}"><i class="fas fa-file-invoice"></i></a>` : '')}`).join(", ") },
            { field: "state", title: "Estado", width: 80 }
        ],
        dataSource: { data: [], aggregate: [{ field: "total", aggregate: "sum" }] },
        dataBound: function (e) {
            var grid = e.sender;
            grid.columns.forEach((v, i) => grid.showColumn(i));
            $("div.k-group-indicator").each((i, v) => grid.hideColumn($(v).data("field")));
            grid.element.find("table").attr("style", "");
        }
    });
}

function getSubcategoriesFilter(e) {
    return { CategoryId: e.filter.filters[0].value };
}

function loadWarehouses(subsidiaries, warehouses) {
    warehouses.empty();
    var objSelected = subsidiaries.multipleSelect("getSelects");
    if (objSelected && objSelected.length > 0) {
        var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
        $.get(urlWarehouses, { Subsidiary: strData }, function (data) {
            if (data.message !== "") {
                showError(data.message);
                warehouses.multipleSelect("disable");
            } else {
                if (data.items.length > 0) {
                    $.each(data.items, function (i, obj) {
                        warehouses.append(new Option(obj.name, obj.name));
                    });
                    setTimeout(() => warehouses.multipleSelect("enable"), 200);
                } else {
                    warehouses.multipleSelect("disable");
                }
            }
            warehouses.multipleSelect("refresh").multipleSelect("uncheckAll");
        });
    } else {
        warehouses.multipleSelect("refresh").multipleSelect("disable");
    }
}

function cleanFilters() {
    $("#subsidiary").multipleSelect("checkAll"), $("#warehouse").multipleSelect("uncheckAll"), $("#order-number, #product").val(""), $("#provider").multipleSelect("uncheckAll"),
        $(`#initial-date`).data("kendoDatePicker").value(""), $(`#final-date`).data("kendoDatePicker").value(""), $("#ord-open").prop("checked", true), $("#ord-close").prop("checked", false),
        $("#line").data("kendoDropDownList").value(""), $("#category").data("kendoDropDownList").value(""), $("#subcategory").data("kendoDropDownList").value("");
}

function getFilters() {
    var message = "", orderNumber = $(`#order-number`).val(), initialDate = $(`#initial-date`).data("kendoDatePicker").value(), finalDate = $(`#final-date`).data("kendoDatePicker").value(),
        product = $(`#product`).val(), ordOpen = $("#ord-open").prop("checked"), ordClose = $("#ord-close").prop("checked"), state = ordOpen & ordClose ? "" : (ordOpen ? "O" : "C"),
        subsidiary = Enumerable.From($(`#subsidiary`).multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join(),
        warehouse = Enumerable.From($(`#warehouse`).multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join(),
        provider = Enumerable.From($(`#provider`).multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join(),
        line = $("#line").data("kendoDropDownList").value(), category = $("#category").data("kendoDropDownList").value(), subcategory = $("#subcategory").data("kendoDropDownList").value();

    if (initialDate) {
        initialDate = initialDate.toISOString();
    }
    if (finalDate) {
        finalDate = finalDate.toISOString();
    }
    if (ordClose & !initialDate & !finalDate) {
        message = "Si desea ver las órdenes cerradas debe escoger un período de tiempo.";
    }

    return { message: message, data: { Subsidiaries: subsidiary, Warehouses: warehouse, OrderNumber: orderNumber, ProviderCode: provider, ProductCode: product, SinceDate: initialDate, UntilDate: finalDate, State: state, Line: line, Category: category, Subcategory: subcategory } };
}

function filterData() {
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
        showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
    }
}

function loadGrid(items) {
    if (items) {
        items.forEach(x => {
            x.docDate = JSON.toDate(x.docDate);
            x.estimatedDate = JSON.toDate(x.estimatedDate);
        });
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({
            data: items,
            pageSize: 500,
            aggregate: [{ field: "total", aggregate: "sum" }],
            group: [
                { field: "subsidiary", dir: "asc", aggregates: [{ field: "subsidiary", aggregate: "count" }, { field: "total", aggregate: "sum" }] },
                { field: "warehouse", dir: "asc", aggregates: [{ field: "warehouse", aggregate: "count" }, { field: "total", aggregate: "sum" }] }
            ]
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

function loadReport(Id, Subsidiary, Report) {
    var objParams = { Subsidiary: Subsidiary, Id: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "ProviderOrder.trdp";
    if (Report === "Bill") {
        strReport = "ProviderBill.trdp";
    }
    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource({ report: strReport, parameters: objParams });
            viewer.refreshReport();
        } catch (e) {
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        $("#reportViewer1").telerik_ReportViewer({
            serviceUrl: urlService,
            reportSource: { report: strReport, parameters: objParams },
            viewMode: telerikReportViewer.ViewModes.INTERACTIVE//,
            //scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
        });
    }
}

function exportExcel() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        window.location.href = urlExcel + "?" + $.param(filtersData.data);
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filtersData.message}`);
    }
}

function openFile(e) {
    var data = e.currentTarget.dataset;
    window.location.href = urlDownloadFile + "?" + $.param({ FilePath: data.path, FileName: data.name });
}

//#endregion