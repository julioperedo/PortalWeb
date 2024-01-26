//#region Variables Globales
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
//#endregion

//#region Eventos

$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    if (jqxhr.responseText) {
        if (jqxhr.responseText.includes("Document/refresh with ID")) {
            var viewer = $("#reportViewer1").data("telerik_ReportViewer");
            if (viewer) viewer.refreshReport();
        }
    }
});

$(document).ready(() => setupControls());

$(".box-container").on("click", "a", openEvent);

$("#openOrders").on("click", ".sale-order, .sale-note, .sale-bill", openDocument);

//#endregion

//#region Metodos

function setupControls() {
    $("#Detail").kendoWindow({ refresh: onRefreshWindow, scrollable: true, visible: false, width: 1000, actions: ["Close"], resizable: false, title: "Detalle de Evento", modal: true, iframe: false });
    loadData();
}

function loadData() {
    $.get(ulrLastEvents, {}, function (data) {
        if (data.length > 0) {
            $(".box-container").append('<div class="col-12 pt-3"><h4>Eventos</h4></div>');
            $.each(data, function (i, obj) {
                var div, divBox, divInner, h4, p, hidden, spDate;
                p = $("<span>").text(obj.description);
                obj.date = JSON.toDate(obj.date);
                spDate = $("<span>").addClass("date");
                var dateFormat = (obj.date.getHours() === 0 && obj.date.getMinutes() === 0) ? "dd-MM-yyyy" : "dd-MM-yyyy HH:mm";
                spDate.text(kendo.toString(obj.date, dateFormat));
                if (obj.detail) {
                    var url = `${urlEvent}/${obj.id}`;
                    var link = $("<a>").attr("href", url).text(obj.name);
                    h4 = $("<h4>").html(link);
                } else {
                    h4 = $("<h4>").text(obj.name);
                }
                hidden = $("<input>").attr("type", "hidden").val(obj.id);
                divBox = $("<div>").addClass("box").addClass("color-" + ((i + 1) % 5));
                divInner = $("<div>").addClass("inner-box");
                divInner.append(hidden);
                divInner.append(h4);
                divInner.append(spDate);
                divInner.append("<br />");
                divInner.append(p);
                div = $("<div>").addClass("col-12").addClass("col-sm-6").addClass("col-md-4").addClass("col-lg-4");
                divBox.append(divInner);
                div.append(divBox);
                $(".box-container").append(div);
            });
            $(".box-container").after(`<a href="${urlAllEvents}">Ver todos los eventos</a>`);
        }
    });

    $.get(urlClientResume, {}, (data) => {
        if (data.message === "") {
            var i = data.item;
            $("#name").text(i.name);
            $("#legalName").text(i.legalName);
            $("#nit").text(i.nit);
            $("#currentPoints").text(kendo.toString(i.points, "N0"));
            $("#currentAmount").text(kendo.toString(i.amount, "N2"));

            if (i.points >= 300000) {
                $(".voucher").removeClass("d-none");
            }
        } else {
            showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
        }
    });

    $.get(urlOpenOrders, {}, data => {
        if (data.message === "") {
            if (data.items && data.items.length > 0) {
                $("#openOrdersTitle").text("Ordenes Abiertas");
                $("#openOrders").kendoGrid({
                    columns: [
                        { title: "Sucursal", width: 120, field: "subsidiary", aggregates: ["count"], groupHeaderTemplate: 'Sucursal: #= value #    ( Total: #= count#,  Monto Total: #= kendo.toString(aggregates.total.sum, "N2") # )' },
                        { title: "Almacen", width: 120, field: "warehouse", aggregates: ["count"], groupHeaderTemplate: 'Almacén: #= value #    ( Total: #= count#,  Monto Total: #= kendo.toString(aggregates.total.sum, "N2") # )' },
                        { title: "Vendedor", width: 200, field: "sellerName", aggregates: ["count"], groupHeaderTemplate: 'Vendedor: # if(value) {# #= value # #} #    ( Total: #= count#,  Monto Total: #= kendo.toString(aggregates.total.sum, "N2") # )' },
                        { title: "Orden Compra Cliente", width: 180, field: "clientOrder" },
                        {
                            title: "Orden", headerAttributes: alignCenter,
                            columns: [
                                { title: "Número", width: 90, template: '# if(docNumber) {# <a class="sale-order action action-link">#:docNumber#</a> #} #', field: "docNumber" },
                                { title: "Total($us)", attributes: alignRight, footerAttributes: alignRight, headerAttributes: alignRight, width: 130, footerTemplate: '#=kendo.toString(sum, "N2")#', field: "total", format: "{0:N2}", aggregates: ["sum"] },
                                { title: "Total Abierto ($us)", attributes: alignRight, footerAttributes: alignRight, headerAttributes: alignRight, width: 150, footerTemplate: '#=kendo.toString(sum, "N2")#', field: "openAmount", format: "{0:N2}", aggregates: ["sum"] },
                                { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "docDate", format: dateFormat },
                            ]
                        },
                        {
                            title: "Notas", headerAttributes: alignCenter,
                            columns: [
                                { title: "Números", width: 160, template: "#=getFactNumLinks(noteNumbers, subsidiary)#", field: "noteNumbers" },
                                { title: "Fechas", width: 85, field: "billDates", encoded: false, attributes: alignCenter, headerAttributes: alignCenter },
                                { title: "Facturado($us)", attributes: alignRight, footerAttributes: alignRight, headerAttributes: alignRight, width: 140, footerTemplate: '#=kendo.toString(sum, "N2")#', field: "totalBilled", format: "{0:N2}", aggregates: ["sum"] }
                            ]
                        },
                    ],
                    groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" }, enabled: true },
                    pageable: { buttonCount: 10 }, sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
                    dataSource: getDataSource(data.items),
                    detailTemplate: kendo.template($('#detailOrder').html()),
                    dataBound: function (e) {
                        var grid = e.sender;
                        for (var i = 0; i < grid.columns.length; i++) {
                            grid.showColumn(i);
                        }
                        $("div.k-group-indicator").each(function (i, v) {
                            grid.hideColumn($(v).data("field"));
                        });
                        grid.element.find("table").attr("style", "");
                    },
                    detailInit: function (e) {
                        $.get(urlDetail, { Subsidiary: e.data.subsidiary, Id: e.data.id, Type: "Orden de Venta", State: e.data.state }, function (data) {
                            $("<div>").appendTo(e.detailCell).kendoGrid({
                                scrollable: false,
                                sortable: true,
                                pageable: false,
                                selectable: true,
                                columns: [
                                    { field: "itemCode", title: "Cod. Item DMC", width: 150 },
                                    { field: "itemName", title: "Descripción" },
                                    { field: "warehouse", title: "Almacén", width: 120 },
                                    { field: "line", title: "Línea", width: 200 },
                                    { field: "quantity", title: "Cantidad", width: 90, attributes: alignRight, headerAttributes: alignRight },
                                    { field: "openQuantity", title: "Abierta", width: 90, attributes: alignRight, headerAttributes: alignRight },
                                    { field: "price", title: "Precio", width: 90, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                                    { field: "itemTotal", title: "Subtotal", width: 90, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                                ],
                                dataSource: data.items
                            });
                        });
                    }
                })
            }
        } else {
            showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
        }
    });
}

function getDataSource(items) {
    $.each(items, function (i, obj) { obj.docDate = JSON.toDate(obj.docDate); });
    var ds = new kendo.data.DataSource({
        data: items,
        aggregate: [
            { aggregate: "sum", field: "total" },
            { aggregate: "sum", field: "openAmount" },
            { aggregate: "sum", field: "totalBilled" },
            { aggregate: "count", field: "subsidiary" },
            { aggregate: "count", field: "warehouse" },
            { aggregate: "count", field: "sellerName" }
        ],
        group: [
            { field: "subsidiary", dir: "asc", aggregates: [{ field: "subsidiary", aggregate: "count" }, { field: "total", aggregate: "sum" }] },
            { field: "warehouse", dir: "asc", aggregates: [{ field: "warehouse", aggregate: "count" }, { field: "total", aggregate: "sum" }] }
        ],
        sort: [{ field: "docDate", dir: "asc" }, { field: "docNumber", dir: "asc" }],
        schema: { model: { id: "id" } }
    });
    return ds;
}

function openDocument(e) {
    e.preventDefault();
    var wnd = $("#Report").data("kendoWindow"), grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row) ?? { docNumber: "" },
        docNumber = item.docNumber, type = "Order", title = `Orden de Venta ${item.docNumber}`, subsidiary = item.subsidiary;
    grd.select(row);
    if (e.currentTarget.classList.contains("sale-note")) {
        var isDelivery = e.currentTarget.getAttribute("delivery") === "Y" && item.subsidiary.toLowerCase() === "santa cruz";
        type = isDelivery ? "Delivery" : "Note";
        title = isDelivery ? "Nota de Entrega" : `Nota de Venta ${noteNumber}`;
    }
    if (e.currentTarget.classList.contains("sale-bill")) {
        docNumber = e.currentTarget.getAttribute("number");
        type = "Bill";
        title = `Factura de la Nota de Venta ${noteNumber}`;
        subsidiary = "";
    }
    loadReport(docNumber, subsidiary, type);
    wnd.title(title);
    wnd.open().center();
}

function getFactNumLinks(nums, subsidiary) {
    var strReturns = "";
    if (nums && nums.length > 0) {
        if (nums.length > 0) {
            strReturns = Enumerable.From(nums).Select(function (x, i) {
                if (subsidiary.toLowerCase() === "santa cruz") {
                    return `${x.number} (<a class="sale-note action action-link" number="${x.number}" delivery="${x.delivery}">Nota</a>,&nbsp;<a class="sale-bill action action-link" number="${x.number}">Factura</a>)&nbsp;`;
                } else {
                    return `<a class="sale-note action action-link" number="${x.number}" delivery="${x.delivery}">${x.number}</a>`;
                }
            }).ToArray().join("<br  />");
        }
    }
    return strReturns;
}

function loadReport(Id, Subsidiary, Report) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    if (Report === "Note") {
        strReport = "SaleNote.trdp";
    }
    if (Report === "Delivery") {
        strReport = "DeliveryNote.trdp";
        objParams.SearchType = 1;
    }
    if (Report === "Bill") {
        strReport = "Bill.trdp";
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
            scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
        });
    }
}

function openEvent(e) {
    e.preventDefault();
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: this.href });
    wnd.open();
}

//#endregion