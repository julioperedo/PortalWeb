//#region Variables Globales
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}";
var localUser = false, permission = { seeMargin: false, seeAllClients: false, sellerCode: "" }, showMargin = false;
//#endregion

//#region Eventos

$(() => initForm());

$(window).resize(() => setHeights());

$(document).ajaxError((event, jqxhr, settings, thrownError) => catchError(event, jqxhr, settings, thrownError));

$('#filter-box').on('hidden.bs.collapse', () => setHeights());

$('#filter-box').on('shown.bs.collapse', () => setHeights());

$('a[data-toggle="tab"]').on('shown.bs.tab', (e) => setHeights(e));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData());

$("#action-show-all").click(() => toggleShowAll());

$("#action-excel").on("click", (e) => ExportExcel());

$("#Listado").on("click", ".sale-note", (e) => showReport(e, "SaleNote"));

$("#Listado").on("click", ".sale-order", (e) => showReport(e, "SaleOrder"));

$("#Listado").on("click", ".sale-bill", (e) => showReport(e, "SaleBill"));

$("#Listado").on("click", ".comment", (e) => showComments(e));

$("#Resume").on("click", ".open-amount", (e) => openAmount(e));

$("#Resume").on("click", ".due-amount", (e) => openDueAmounts(e));

$("#openOrdersList").on("click", ".sale-order", (e) => showReport(e, "SaleOrder2"));

//#endregion

//#region Métodos Locales

function initForm() {
    setupControls();
    if ($("#Client").val() !== "") {
        setTimeout(function () { filterData(); }, 200);
    } else {
        setGridHeight("Listado", gridMargin);
        setGridHeight("Resume", gridMargin);
    }
}

function setupControls() {
    localUser = $("#LocalUser").val() === "Y";
    if (localUser) {
        permission = JSON.parse($("#permissions").val());
        if (permission.seeMargin) {
            $("#action-show-all").removeClass("d-none");
        }
    }
    $("#Seller").kendoDropDownList({
        dataSource: { transport: { read: { url: urlGetSellers } } }, ignoreCase: true, optionLabel: "Seleccione un Vendedor...", filter: "contains", dataTextField: "name", dataValueField: "shortName",
        value: permission.sellerCode//, enable: permission.seeAllClients | !localUser
    });

    $.get(urlSubsidiaries, {}, function (data) {
        var select = $("#FilSubsidiaries");
        $.each(data.items, function (i, obj) {
            select.append(new Option(obj.name, obj.name));
        });
        select.multipleSelect().multipleSelect("checkAll");
    });
    var lstColumns = [
        { field: "clientName", title: "Cliente", width: 200, aggregates: ["count"], groupHeaderTemplate: "Cliente: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "subsidiary", title: "Sucursal", width: 110, aggregates: ["count"], groupHeaderTemplate: "Sucursal: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "warehouse", title: "Almacen", width: 110, aggregates: ["count"], groupHeaderTemplate: "Almacén: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "type", title: "Tipo Doc.", width: 120, aggregates: ["count"], groupHeaderTemplate: "Tipo Doc.: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "docDate", title: "Fecha Doc.", width: 105, format: "{0:dd/MM/yyyy}", attributes: alignCenter, headerAttributes: alignCenter, aggregates: ["count"], groupHeaderTemplate: "Fecha Doc.: #= kendo.toString(value, 'dd/MM/yyyy') #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "docNum", title: "Nro. Doc.", width: 85, groupable: false, attributes: alignCenter, headerAttributes: alignCenter },
        { field: "sellerName", title: "Vendedor", width: 170, aggregates: ["count"], groupHeaderTemplate: "Vendedor: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "saleNote", title: "Nota Venta", width: 165, attributes: alignCenter, headerAttributes: alignCenter, groupable: false, template: notesTemplate },
        { field: "saleOrder", title: "Orden Venta", width: 100, attributes: alignCenter, headerAttributes: alignCenter, groupable: false, template: ordersTemplate },
        { field: "pickupDate", title: "Fecha Retiro", width: 95, attributes: alignCenter, headerAttributes: alignCenter, format: "{0:dd/MM/yyyy}", aggregates: ["count"], groupHeaderTemplate: "Fecha Retiro: #= kendo.toString(value, 'dd/MM/yyyy') #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "clientOrder", width: 120, title: "Orden Cliente", groupable: false },
        { field: "terms", title: "Término", width: 90, aggregates: ["count"], groupHeaderTemplate: "Término: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "dueDate", title: "Fecha Venc.", width: 95, attributes: alignCenter, headerAttributes: alignCenter, format: "{0:dd/MM/yyyy}", aggregates: ["count"], groupHeaderTemplate: "Fecha Venc.: #= kendo.toString(value, 'dd/MM/yyyy') #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
        { field: "total", title: "Total", width: 100, attributes: alignRight, headerAttributes: alignRight, format: numberFormat, groupable: false },
        { field: "balance", title: "Balance", width: 140, attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: "Total : #=kendo.toString(sum, 'N2')#", format: numberFormat, aggregates: ["sum", "count"] }
    ];
    if (permission.seeMargin) {
        lstColumns.push({ field: "percetageMargin", title: "Margen", width: 70, attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, groupable: false, format: "{0:#,##0.00} %" });
    }
    lstColumns.push({ field: "days", title: "Días", width: 80, attributes: alignRight, headerAttributes: alignRight, aggregates: ["count"], groupHeaderTemplate: "Días: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " });
    lstColumns.push({ field: "state", title: "Estado", width: 80, aggregates: ["count"], groupHeaderTemplate: "Estado: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " });
    if (localUser) {
        lstColumns.push({ field: "header", width: 30, title: " ", template: '# if(header || footer) {# <a class="comment action"><i class="far fa-list-alt"></i></a> #} #', attributes: alignCenter });
    }

    $("#Listado").kendoGrid({
        dataSource: { data: [], aggregate: [{ field: "balance", aggregate: "sum" }] },
        sortable: true,
        selectable: true,
        groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" } },
        noRecords: { template: '<div class="w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
        pageable: false,
        columns: lstColumns,
        dataBound: function (e) {
            var grid = e.sender;
            for (var i = 0; i < grid.columns.length; i++) {
                grid.showColumn(i);
            }
            $("div.k-group-indicator").each(function (i, v) {
                grid.hideColumn($(v).data("field"));
            });
            if (showMargin) {
                grid.showColumn("percetageMargin");
            } else {
                grid.hideColumn("percetageMargin");
            }
            grid.element.find("table").attr("style", "");
        }
    });

    $("#Resume").kendoGrid({
        dataSource: [],
        sortable: true,
        selectable: true,
        pageable: false,
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        columns: [
            { field: "subsidiary", title: "Sucursal", width: 150 },
            { field: "clientName", title: "Cliente" },
            { field: "creditLimit", title: "Límite Crédito", width: 140, attributes: alignRight, headerAttributes: alignRight, format: numberFormat },
            { field: "availableBalance", title: "Saldo Disponible", width: 140, attributes: alignRight, headerAttributes: alignRight, format: numberFormat },
            { field: "balance", title: "Balance", width: 140, attributes: alignRight, headerAttributes: alignRight, format: numberFormat },
            { field: "dueAmount", title: "En Mora", width: 140, attributes: alignRight, headerAttributes: alignRight, template: '# if(dueAmount > 0) {# <a class="action action-link due-amount" title="Monto en Mora">#=kendo.toString(dueAmount, "N2")#</a> #} else {# 0.00 #} #' },
            { field: "ordersBalance", title: "Pedidos", width: 140, attributes: alignRight, headerAttributes: alignRight, template: '# if(ordersBalance > 0) {# <a class="action action-link open-amount" title="Monto Abierto">#=kendo.toString(ordersBalance, "N2")#</a> #} else {# 0.00 #} #' },
            { field: "terms", title: "Términos", width: 100 },
            //{
            //    field: "id", title: "Detalle", media: "xs", template: function (e) {
            //        var content = `<div class="mini-row">
            //                         <div class="row"><div class="col font-weight-bold">Sucursal:</div><div class="col text-right">${e.subsidiary}</div><div class="col font-weight-bold">T&eacute;rminos:</div><div class="col text-right">${e.terms}</div></div>
            //                         <div class="row"><div class="col font-weight-bold">L. Cr&eacute;dito:</div><div class="col text-right">${e.creditLimit}</div><div class="col font-weight-bold">Saldos:</div><div class="col text-right">${e.availableBalance}</div></div>
            //                         <div class="row"><div class="col font-weight-bold">Balance:</div><div class="col text-right">${e.balance}</div><div class="col font-weight-bold">Pedidos:</div><div class="col text-right">${(e.ordersBalance > 0 ? `<a class="action action-link open-amount" title="Monto Abierto">${kendo.toString(e.ordersBalance, "N2")}#</a>` : '0.00')}</div></div>
            //                       </div>`;
            //        return content;
            //    }
            //}
        ],
        dataBound: function (e) {
            var grid = e.sender;
            for (var i = 0; i < grid.columns.length; i++) {
                //console.log(grid.columns[i].field);
                if (grid.columns[i].field !== "id") {
                    grid.showColumn(i);
                }
            }
            $("div.k-group-indicator").each(function (i, v) {
                grid.hideColumn($(v).data("field"));
            });
            grid.element.find("table").attr("style", "");
        }
    });

    $.get(urlGetBankAccounts, {}, data => {
        var div = $("#tab3");
        if (data.message === "") {
            data.items.forEach(group => {
                var card = "", boName = group.items.filter(x => $.trim(x.name) !== "").length > 0, boCountry = group.items.filter(x => $.trim(x.country) !== "").length > 0,
                    boType = group.items.filter(x => $.trim(x.type) !== "").length > 0, boABA = group.items.filter(x => $.trim(x.abaNumber) !== "").length > 0, boSwift = group.items.filter(x => $.trim(x.swift) !== "").length > 0,
                    boComments = group.items.filter(x => $.trim(x.comments) !== "").length > 0;
                card += `<div class="card no-shadow mt-2"><div class="card-header">${group.name}</div><div class="card-body"><div class="table-responsive"><table class="table table-condensed bank-table"><thead><tr><th>BANCO</th>`;
                if (boName) card += `<th>BENEFICIARIO (para transferencias ACH)</th>`;
                card += `<th>MONEDA</th><th>NO. DE CUENTA</th>`;
                if (boCountry) card += `<th>PAIS</th>`;
                if (boType) card += `<th>TIPO DE CUENTA</th>`;
                if (boABA) card += `<th>ABA No.</th>`;
                if (boSwift) card += `<th>SWIFT</th>`;
                if (boComments) card += `<th></th>`;
                card += `<th></th></tr></thead><tbody>`;
                group.items.forEach(account => {
                    card += `<tr><td>${account.bank}</td>`;
                    if (boName) card += `<td>${account.name}</td>`;
                    card += `<td>${account.currency}</td><td>${account.number}</td>`;
                    if (boCountry) card += `<td>${account.country}</td>`;
                    if (boType) card += `<td>${account.type}</td>`;
                    if (boABA) card += `<td>${account.abaNumber}</td>`;
                    if (boSwift) card += `<td>${account.swift}</td>`;
                    if (boComments) card += `<td>${account.comments}</td>`;
                    card += '<td>';
                    if ($.trim(account.qr) !== "") card += `<a class="qr-tooltip" data-image="${account.qr}"><i class="fas fa-qrcode"></i></a>`;
                    card += `</td></tr>`;
                });
                card += `</tbody></table></div></div></div>`;
                div.append(card);
            });
            var template = `<div class="template-wrapper"><img src="../../images/qr/#=target.data('image')#" /><p>#=target.data('title')#</p></div>`;
            $(".bank-table").kendoTooltip({
                autoHide: false,
                filter: "a",
                content: kendo.template(template),
                width: 612,
                height: 812,
                position: "left"
            });
        } else {
            console.error(data.message);
            div.append(`<span>Se ha producido un error al traer los datos del servidor.</span>`);
        }
    });

    $("#OpenOrders").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "Ordenes Abiertas", resizable: false, width: 1100, actions: ["Close"], activate: function (e) {
            var wnd = this;
            setTimeout(() => {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });

    $("#openOrdersList").kendoGrid({
        columns: [
            { title: "Almacen", width: 100, field: "warehouse" },
            { title: "# Orden", width: 90, template: '<a class="sale-order action action-link"># if(docNumber) {# #:docNumber# #} #</a>', field: "docNumber" },
            { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "docDate", format: "{0:dd/MM/yyyy}" },
            { title: "Order Cliente", field: "clientOrder" },
            //{ title: "Cliente", width: "180px", field: "clientName" },
            { title: "Vendedor", width: "120px", field: "sellerName" },
            { title: "Total", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "total", format: "{0:#,##0.00}" },
            { title: "Abierto", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "openAmount", format: "{0:#,##0.00}" },
            { title: "Completo", attributes: alignCenter, headerAttributes: alignCenter, width: 90, template: '# if(nonCompleteItem === 0) {# <i class="fas fa-check"></i> #} #', field: "nonCompleteItem" },
            //{ title: " ", width: 30, template: '# if(header || footer) {# <a class="comment action action-link"><i class="fas fa-list-alt"></i></a> #} #', field: "header" }
            //{ title: " ", width: 30, template: '# if(hasFiles) {# <a class="attach action action-link"><i class="fas fa-paperclip"></i></a> #} #', field: "hasFiles" }
        ],
        sortable: true, selectable: "Single, Row",
        dataSource: []
        //detailTemplate: kendo.template($('#detailOrder').html()),
        //detailInit: function (e) {
        //    var item = e.data;
        //    $.get(urlOrderDetail, { Subsidiary: item.subsidiary, DocEntry: item.id, State: item.state }, data => {
        //        if (data.message === "") {
        //            $("<div>").appendTo(e.detailCell).kendoGrid({
        //                scrollable: false,
        //                sortable: true,
        //                pageable: false,
        //                selectable: "Single, Row",
        //                columns: [
        //                    { title: "Cod. Item DMC", width: 150, template: "<span class='item-code action action-link'>#=itemCode#</span>", field: "itemCode" },
        //                    { title: "Descripción", field: "itemName" },
        //                    { title: "Línea", width: 200, field: "line" },
        //                    { title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "openQuantity" },
        //                    { title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "price", format: "{0:N2}" },
        //                    { title: "Subtotal", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "itemTotal", format: "{0:N2}" },
        //                    { title: "Completo", attributes: alignCenter, headerAttributes: alignCenter, width: 90, template: '# if(complete) {# <i class="fas fa-check"></i> #} #', field: "complete" }
        //                ],
        //                dataSource: data.items,
        //                noRecords: { template: "No hay items con acelerador" }
        //            });
        //        } else {
        //            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}.`);
        //        }
        //    });
        //}
    });
}

function setHeights(e) {
    if (e) {
        if (e.target.text === "Consolidado") {
            setGridHeight("Resume", gridMargin);
        } else if (e.target.text === "Detallado") {
            setGridHeight("Listado", gridMargin);
        }
    } else {
        setGridHeight("Listado", gridMargin);
        setGridHeight("Resume", gridMargin);
    }
}

function toggleShowAll() {
    var grd = $("#Listado").data("kendoGrid");
    var e = $(this).find("i");
    if (e.hasClass("fa-eye-slash")) {
        grd.hideColumn("percetageMargin");
        e.removeClass("fa-eye-slash").addClass("fa-eye");
    } else {
        grd.showColumn("percetageMargin");
        e.removeClass("fa-eye").addClass("fa-eye-slash");
    }
    showMargin = !showMargin;
}

function clientMapper(options) {
    var items = $("#Client").data("kendoDropDownList").dataSource.data();
    var item = Enumerable.From(items).Where(x => x.code === options.value).FirstOrDefault();
    var index = Enumerable.From(items).IndexOf(item);
    options.success(index);
}

function cleanFilters() {
    $("#FilSubsidiaries").multipleSelect("checkAll");
    $("#Seller").data("kendoDropDownList").value("");
    if ($("#LocalUser").val() === "Y") {
        var objClient = $("#Client").data("kendoDropDownList");
        objClient.text("");
        objClient.value("");
    }
}

function getFilters() {
    var strCardCode = $("#Client").val(), strSubsidiaries = Enumerable.From($("#FilSubsidiaries").multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join(), strSalesMan = $.trim($("#Seller").val()),
        message = "";
    if (strSubsidiaries === "") {
        message += "Debe seleccionar al menos una sucursal.<br />";
    }
    if (strCardCode === "" && strSalesMan === "") {
        message += "Debe seleccionar o un cliente o un vendedor.";
    }
    return { message: message, data: { CardCode: strCardCode, Subsidiary: strSubsidiaries, SalesMan: strSalesMan } };
}

function filterData() {
    var filters = getFilters();
    if (filters.message === "") {
        $.get(urlFilter, filters.data, function (data) {
            if (data.message === "") {
                loadResults(data.items, data.resume);
            } else {
                showError(data.message);
            }
        });
    } else {
        showInfo(filters.message);
    }
}

function loadResults(items, resume) {
    var grd, ds;

    grd = $("#Listado").data("kendoGrid");
    $.each(items, function (i, obj) {
        obj.docDate = JSON.toDate(obj.docDate);
        obj.dueDate = JSON.toDate(obj.dueDate);
        obj.pickupDate = JSON.toDate(obj.pickupDate);
    });

    ds = new kendo.data.DataSource({
        data: items,
        aggregate: [{ field: "balance", aggregate: "sum" }],
        group: [{ field: "clientName", dir: "asc", aggregates: [{ field: "clientName", aggregate: "count" }, { field: "balance", aggregate: "sum" }] }],
        sort: { field: "docDate", dir: "asc" }
    });
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
    } else {
        $("#action-excel").addClass("d-none");
    }

    grd = $("#Resume").data("kendoGrid");
    ds = new kendo.data.DataSource({
        data: resume, pageSize: 100,
        aggregate: [{ field: "balance", aggregate: "sum" }],
        group: [{ field: "clientName", dir: "asc", aggregates: [{ field: "clientName", aggregate: "count" }, { field: "balance", aggregate: "sum" }] }]
    });
    grd.setDataSource(ds);

    setGridHeight("Listado", gridMargin);
    setGridHeight("Resume", gridMargin);
}

function ExportExcel() {
    var filterData = getFilters();
    if (filterData.message === "") {
        window.location.href = urlExcel + "?" + $.param(filterData.data);
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filterData.message}`);
    }
}

function showReport(e, type) {
    var wnd = $("#Report").data("kendoWindow");
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    if (type === "SaleOrder") {
        var dataset = e.currentTarget.dataset;
        loadReport(dataset.id, item.subsidiary, dataset.delivery === "Y" ? "Delivery" : "Order");
        wnd.title(`Orden de Venta ${item.saleOrder}`);
    } else if (type === "SaleOrder2") {
        loadReport(item.docNumber, item.subsidiary, "Order");
        wnd.title(`Orden de Venta ${item.docNumber}`);
    } else if (type === "SaleNote") {
        loadReport(item.saleNote, item.subsidiary, "Note");
        wnd.title(`Nota de Venta ${item.saleNote}`);
    } else {
        loadReport(item.saleNote, item.subsidiary, "Bill", item.billSerie);
        wnd.title(`Factura de la Nota de Venta ${item.saleNote}`);
    }

    wnd.open().center();
}

function showComments(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var wnd = $("#Detail").data("kendoWindow");
    var detailsTemplate = kendo.template($("#template").html());
    wnd.content(detailsTemplate(item));
    wnd.center().open();
}

function loadReport(Id, Subsidiary, Report, Series) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    if (Report === "Note") {
        strReport = "SaleNote.trdp";
    }
    if (Report === "Delivery") {
        strReport = "DeliveryNote.trdp";
        objParams.SearchType = 2;
    }
    if (Report === "Bill") {
        switch (Series) {
            case 93:
                strReport = "ElectronicBill.trdp";
                break;
            case 32:
                strReport = "RentReceipt.trdp";
                break;
            default:
                strReport = "Bill.trdp";
        }
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
            viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
            scaleMode: telerikReportViewer.ScaleModes.SPECIFIC,
            scale: 1
        });
    }
}

function openAmount(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    $.get(urlGetOpenOrders, { CardCode: item.clientCode, Subsidiary: item.subsidiary }, function (data) {
        var grid = $("#openOrdersList").data("kendoGrid");
        data.orders.forEach(x => x.docDate = JSON.toDate(x.docDate));
        var ds = new kendo.data.DataSource({ data: data.orders });
        grid.setDataSource(ds);

        $("#OpenOrders").data("kendoWindow").open().center();
    });
}

function ordersTemplate(item) {
    var temp = "";
    //'# if(saleOrder && saleOrder !== 0) {# <a class="sale-order action action-link">#=saleOrder#</a> #} #'
    if (item.saleOrder && (item.saleOrder !== "" && item.saleOrder !== "0")) {
        if (item.saleOrder.includes(",")) {
            temp = Enumerable.From(item.saleOrder.split(", ")).Select((x) => `<a class="sale-order action action-link" data-id="${x}" data-subsidiary="${item.subsidiary}" data-delivery="${item.isDeliveryNote}">${x}</a>`).ToArray().join(", ");
        } else {
            temp = `<a class="sale-order action action-link" data-id="${item.saleOrder}" data-subsidiary="${item.subsidiary}" data-delivery="${item.isDeliveryNote}">${item.saleOrder}</a>`;
        }
    }
    return temp;
}

function notesTemplate(item) {
    var content = "";
    if (item.saleNote && item.saleNote !== 0) {
        content = `<a class="sale-note action action-link">${item.saleNote}</a>`;
        if (item.subsidiary.toLowerCase() === "santa cruz" && item.type.toLowerCase() === "factura") {
            content += ` ( <a class="sale-bill action action-link">Factura</a> )`;
        }
    }
    return content;
}

function openDueAmounts(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var row1, row2, row3, row4;
    row1 = Enumerable.From(item.dueItems).Where("$.days > 0 & $.days < 16").Select("$.balance").Sum();
    row2 = Enumerable.From(item.dueItems).Where("$.days > 15 & $.days < 31").Select("$.balance").Sum();
    row3 = Enumerable.From(item.dueItems).Where("$.days > 30 & $.days < 46").Select("$.balance").Sum();
    row4 = Enumerable.From(item.dueItems).Where("$.days > 45").Select("$.balance").Sum();
    var contentTable = `<table class="table table-striped"><tbody><tr><td>Mora de 1 a 15 d&iacute;as</td><td class="text-right">${kendo.toString(row1, "N2")}</td></tr><tr><td>Mora de 16 a 30 d&iacute;as</td><td class="text-right">${kendo.toString(row2, "N2")}</td></tr><tr><td>Mora de 31 a 45 d&iacute;as</td><td class="text-right">${kendo.toString(row3, "N2")}</td></tr><tr><td>Mora de m&aacute;s de 45 d&iacute;as</td><td class="text-right">${kendo.toString(row4, "N2")}</td></tr></tbody></table>`;

    $("<div>").kendoWindow({
        visible: true, modal: true, iframe: false, title: "Montos en Mora", resizable: false, width: 350, actions: ["Close"], content: { template: contentTable }, activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    });
}

//#endregion