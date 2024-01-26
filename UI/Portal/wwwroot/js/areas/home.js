//#region Global Variables
var chartList, _minDate, _maxDate, regionalData = {}, productData = {}, teamData = {}, gridGrouped, kbytesLoaded = false, productsLoaded = false, teamLoaded = false, eventsLoaded = false, _canSee = false, resumeItem;
const alignRight = { style: "text-align: right;" }, alignCenter = { style: "text-align: center;" }, seeSalesDetail = $("#SeeAllSalesDetail").val() === "Y";
//#endregion

//#region Events

$(document).ajaxError((event, jqxhr, settings, thrownError) => catchError(event, jqxhr, settings, thrownError));

$(() => setupControls());

$("#accordion").on("hide.bs.collapse show.bs.collapse", showHideAccordion);

$("#accordion").on("shown.bs.collapse", e => $(".page-content").animate({ scrollTop: $(e.target).parent().offset().top }, 400));

$("#switch-warehouses").click(switchWarehouses);

$("#divisions .btn").click(changeDivision);

$("#subsidiaries .btn").click(changeSubsidiary);

$("#tableResume").on("click", ".billed-today", e => openBilledPeriod(e, "T"));

$("#tableResume").on("click", ".billed-period", e => openBilledPeriod(e, "P"));

$("#tableResume").on("click", ".delivered-period", openDelivered);

$("#tableResume").on("click", ".aov", e => showOpenOrders(e, true));

$("#tableResume").on("click", ".openov", e => showOpenOrders(e, false));

$("#tableResume").on("click", ".stockvalue", openStockDetail);

$("body").on("click", ".sale-order", e => openReport(e, "SO"));

$("#Detail").on("click", ".sale-note", e => openReport(e, "SN"));

$("#Detail").on("click", ".item-code", openItemStock);

$("#Detail").on("click", ".comment", e => openDetails(e, "comment"));

$("#Detail").on("click", ".attach", e => openDetails(e, "attach"));

$("#Detail").on("click", ".delivery-note", openDeliveryNote);

$("body").on("click", ".reserved", showReservedDetail);

$("body").on("click", ".open-file", openFile);

$("#events-container").on("click", ".event", openEvent);

$("#resumes").on("click", ".see-details", seeDetails);

//#endregion

//#region Methods

function setupControls() {
    chartList = JSON.parse($("#Charts").val());
    var isMobile = Boolean(kendo.support.mobileOS);
    var today = new Date(), initDate = new Date(today.getFullYear(), today.getMonth(), 1);
    var filSince = $("#regional-since").kendoDatePicker({
        format: "d/M/yyyy", value: initDate, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
            getRegionalData();
        }
    }).data("kendoDatePicker");
    var filUntil = $("#regional-until").kendoDatePicker({
        format: "d/M/yyyy", value: today, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
            getRegionalData();
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    if (Enumerable.From(chartList).Where("$.IdChart === 1 || $.IdChart === 7").Count() > 0) {
        _canSee = Enumerable.From(chartList).Where("$.IdChart === 7").Count() > 0;
        $("#tableResume").kendoGrid({
            dataSource: getDatasourceResume([]),
            scrollable: false,
            sortable: true,
            pageable: false,
            selectable: "Single, Row",
            columns: [
                { field: "subsidiary", title: "Sucursal", footerTemplate: "TOTAL", media: "(min-width:1175px)" },
                { field: "warehouse", title: "Almacén", media: "(min-width:1175px)" },
                {
                    field: "today", title: "Facturado Hoy", format: "{0:N2}", footerTemplate: "#:kendo.toString(sum, 'N2')#", attributes: alignRight, headerAttributes: alignRight,
                    footerAttributes: alignRight, width: 100, template: e => seeSalesDetail ? `<a class='billed-today action action-link'>${kendo.toString(e.today, "N2")}</a>` : kendo.toString(e.today, "N2"),
                    media: "(min-width:1175px)"
                },
                {
                    field: "period", title: "Facturado Período", format: "{0:N2}", footerTemplate: "#:kendo.toString(sum, 'N2')#", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 100,
                    template: e => seeSalesDetail ? `<a class='billed-period action action-link'>${kendo.toString(e.period, 'N2')}</a>` : kendo.toString(e.period, 'N2'), media: "(min-width:1175px)"
                },
                {
                    field: "notBilled", title: "Sólo Entregado", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 100,
                    footerTemplate: "#:kendo.toString(sum, 'N2')#", template: e => seeSalesDetail ? `<a class='delivered-period action action-link'>${kendo.toString(e.notBilled, 'N2')}</a>` : kendo.toString(e.notBilled, 'N2'),
                    media: "(min-width:1175px)"
                },
                { field: "percentage", title: "Porcentaje", format: "{0:N2} %", attributes: alignRight, headerAttributes: alignRight, width: 100, media: "(min-width:1175px)" },
                { field: "expected", title: "Proyectado", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 100, footerTemplate: "#:kendo.toString(sum, 'N2')#", footerAttributes: alignRight, media: "(min-width:1175px)" },
                { field: "expectedDifference", title: "Dif. Proyectado", attributes: alignRight, headerAttributes: alignRight, width: 100, template: templateExpectedDiff, footerTemplate: footerTemplateExpectedDiff, footerAttributes: alignRight, media: "(min-width:1175px)" },
                { field: "expectedDifferencePercentage", title: "Dif. Proyectado %", format: "{0:N2} %", attributes: alignRight, headerAttributes: alignRight, width: 100, template: templateExpectedDiffPercentage, footerTemplate: footerTemplateExpectedDiffPercentage, footerAttributes: alignRight, media: "(min-width:1175px)" },
                { field: "margin", title: "Margen", format: "{0:N2} %", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 70, footerTemplate: marginTotal, media: "(min-width:1175px)" },
                {
                    field: "authorized", title: "OV Autorizadas", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 100, footerAttributes: alignRight,
                    footerTemplate: "#:kendo.toString(sum, 'N2')#", template: e => seeSalesDetail ? `<a class='aov action action-link'>${kendo.toString(e.authorized, 'N2')}</a>` : kendo.toString(e.authorized, 'N2'),
                    media: "(min-width:1175px)"
                },
                {
                    field: "open", title: "OV Abiertas", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 100, footerAttributes: alignRight,
                    footerTemplate: "#:kendo.toString(sum, 'N2')#", template: e => seeSalesDetail ? `<a class='openov action action-link'>${kendo.toString(e.open, 'N2')}</a>` : kendo.toString(e.open, 'N2'),
                    media: "(min-width:1175px)"
                },
                {
                    field: "stock", title: "Valor Stock", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 100, footerAttributes: alignRight, media: "(min-width:1175px)",
                    footerTemplate: "#:kendo.toString(sum, 'N2')#", template: dataItem => (gridGrouped ? `<a class="stockvalue action action-link">${kendo.toString(dataItem.stock, "N2")}</a>` : kendo.toString(dataItem.stock, "N2"))
                },
                { field: "subsidiary", title: "Resumen", template: templateColumnMobile, media: "(max-width:1174px)", footerTemplate: footerTemplateColumnMobile }
            ],
            dataBound: function (e) {
                var grid = e.sender, width = $(window).width();
                if (width >= 1175) {
                    if (gridGrouped) {
                        grid.hideColumn("warehouse");
                        if (Enumerable.From(regionalData.projection).Select("$.amount").Sum() > 0) {
                            grid.showColumn("expected");
                            grid.showColumn("expectedDifference");
                            grid.showColumn("expectedDifferencePercentage");
                        }
                    } else {
                        grid.showColumn("warehouse");
                        grid.hideColumn("expected");
                        grid.hideColumn("expectedDifference");
                        grid.hideColumn("expectedDifferencePercentage");
                    }
                    if (Enumerable.From(chartList).Where("$.IdChart === 7").Count() > 0) {
                        grid.showColumn("margin");
                        grid.showColumn("authorized");
                        grid.showColumn("stock");
                    } else {
                        grid.hideColumn("margin");
                        grid.hideColumn("authorized");
                        grid.hideColumn("stock");
                    }
                    if (regionalData.notBilled && regionalData.notBilled.length > 0) {
                        grid.showColumn("notBilled");
                    } else {
                        grid.hideColumn("notBilled");
                    }
                }
            }
        });

        $("#openOVs").kendoGrid({
            columns: [
                { title: "Almacen", width: 90, field: "warehouse" },
                { title: "# Orden", width: 100, field: "docNumber", template: e => e.docNumber ? `<a class="sale-order action action-link" data-number="${e.docNumber}">${e.docNumber}</a>` : '' },
                { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "docDate", format: "{0:dd/MM/yyyy}" },
                { title: "Order Cliente", width: 100, field: "clientOrder" },
                { title: "Cliente", width: "180px", field: "clientName" },
                { title: "Vendedor", width: "120px", field: "sellerName" },
                { title: "Total", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "total", format: "{0:#,##0.00}" },
                { title: "Abierto", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "openAmount", format: "{0:#,##0.00}" },
                { title: "Completo", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "complete", template: e => e.complete ? '<i class="fas fa-check"></i>' : '' },
                { title: " ", width: 30, field: "header", template: e => e.header || e.footer ? '<a class="comment action action-link"><i class="fas fa-list-alt"></i></a>' : '' },
                { title: " ", width: 30, field: "hasFiles", template: e => e.hasFiles ? '<a class="attach action action-link"><i class="fas fa-paperclip"></i></a>' : '' }
            ],
            sortable: true, selectable: "Single, Row",
            dataSource: [],
            detailInit: function (e) {
                var lstItemCodes = ["FLETES", "ENVIO", "DMCSERVICIOS", "DMCCARGOS"];
                var item = e.data;
                $.get(urlOrderDetail, { Subsidiary: item.subsidiary, DocEntry: item.id, State: item.state }, data => {
                    if (data.message === "") {
                        $("<div>").appendTo(e.detailCell).kendoGrid({
                            scrollable: false,
                            sortable: true,
                            pageable: false,
                            selectable: "Single, Row",
                            columns: [
                                { title: "Cod. Item DMC", width: 150, field: "itemCode", template: (e) => lstItemCodes.indexOf(e.itemCode) >= 0 ? e.itemCode : `<span class='item-code action action-link'>${e.itemCode}</span>` },
                                { title: "Descripción", field: "itemName" },
                                { title: "Línea", width: 200, field: "line" },
                                { title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "openQuantity" },
                                { title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "price", format: "{0:N2}" },
                                { title: "Subtotal", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "itemTotal", format: "{0:N2}" },
                                { title: "Completo", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "complete", template: (e) => e.complete ? '<i class="fas fa-check"></i>' : '' }
                            ],
                            dataSource: data.items,
                            noRecords: { template: "No hay items con acelerador" }
                        });
                    } else {
                        showError(`Se ha producido el siguiente error al traer los datos: ${data.message}.`);
                    }
                });
            }
        });
        $("#notesBilled").kendoGrid({
            columns: [
                { title: "Almacen", field: "warehouse" },
                { title: "# Nota", width: 100, field: "noteNumber", template: e => e.noteNumber ? `<a class="sale-note action action-link">${e.noteNumber}</a>` : '' },
                {
                    title: "# Orden", width: 80, field: "orderNumber", template: function (e) {
                        var result = "";
                        if (e.orderNumber) {
                            if (e.orderNumber.indexOf(",")) {
                                var temp = [];
                                e.orderNumber.split(", ").forEach(x => {
                                    temp.push(`<a class="sale-order action action-link" data-number="${x}">${x}</a>`);
                                });
                                result = temp.join("<br />");
                            } else {
                                result = `<a class="sale-order action action-link" data-number="${e.orderNumber}">${e.orderNumber}</a>`;
                            }
                        }
                        return result;
                    }
                },
                { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "docDate", format: "{0:dd-MM-yyyy}" },
                { title: "Nombre Cliente", field: "clientName" },
                { title: "Vendedor", field: "sellerName" },
                { title: "Total", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "total", format: "{0:#,##0.00}" },
                { title: "Margen", attributes: alignRight, headerAttributes: alignRight, width: 80, field: "margin", format: "{0:#,##0.00} %" }
            ],
            sortable: true, selectable: "Single, Row",
            dataSource: [],
            detailInit: function (e) {
                var item = e.data;
                $.get(urlNoteDetail, { Subsidiary: item.subsidiary, DocNumber: item.noteNumber }, data => {
                    if (data.message === "") {
                        $("<div>").appendTo(e.detailCell).kendoGrid({
                            "columns": [
                                { title: "Cod. Item DMC", width: 150, template: "<span class='item-code action action-link'>#=itemCode#</span>", field: "itemCode" },
                                { title: "Descripción", field: "itemName" },
                                { title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "quantity" },
                                { title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "price", format: "{0:N2}" },
                                { title: "Subtotal", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "itemTotal", format: "{0:N2}" },
                                { title: "Margen", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "margin", format: "{0:N2} %" }
                            ],
                            "sortable": true, "selectable": "Single, Row", "scrollable": false,
                            "dataSource": data.items
                        });
                    } else {
                        showError(`Se ha producido el siguiente error al traer los datos: ${data.message}.`);
                    }
                });
            }
        });
        $("#justDelivered").kendoGrid({
            columns: [
                { title: "Almacen", field: "warehouse", width: 150 },
                { title: "# Nota E.", width: 100, field: "docNumber", template: e => `<a class="delivery-note action action-link">${e.docNumber}</a>`, width: 90 },
                { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "docDate", format: "{0:dd-MM-yyyy}" },
                { title: "Nombre Cliente", field: "clientName", width: 300 },
                { title: "Vendedor", field: "sellerName", width: 200 },
                { title: "Total", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "total", format: "{0:#,##0.00}" }//,
                //{ title: "Margen", attributes: alignRight, headerAttributes: alignRight, width: 80, field: "margin", format: "{0:#,##0.00} %" }
            ],
            sortable: true, selectable: "Single, Row",
            dataSource: [],
            detailInit: function (e) {
                var item = e.data;
                $.get(urlDeliveryNoteDetail, { Subsidiary: item.subsidiary, DocNumber: item.docNumber }, data => {
                    if (data.message === "") {
                        if (data.items && data.items.length > 0) data.items.forEach(x => {
                            x.itemTotal = x.quantity * x.price;
                            x.warehouse = item.warehouse;
                        });
                        $("<div>").appendTo(e.detailCell).kendoGrid({
                            "columns": [
                                { title: "Cod. Item DMC", width: 150, template: "<span class='item-code action action-link'>#=itemCode#</span>", field: "itemCode" },
                                { title: "Descripción", field: "itemName" },
                                { title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "quantity" },
                                { title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "price", format: "{0:N2}" },
                                { title: "Subtotal", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "itemTotal", format: "{0:N2}" }//,
                                //{ title: "Margen", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "margin", format: "{0:N2} %" }
                            ],
                            "sortable": true, "selectable": "Single, Row", "scrollable": false,
                            "dataSource": data.items
                        });
                    } else {
                        showError(`Se ha producido el siguiente error al traer los datos: ${data.message}.`);
                    }
                });
            }
        });
        $("#stockDetail").kendoGrid({
            columns: [
                { title: "Almacén", field: "warehouse" },
                { title: "Stock", attributes: alignRight, headerAttributes: alignRight, width: 120, field: "total", format: "{0:#,##0.00}" }
            ],
            sortable: true, selectable: "Single, Row",
            dataSource: [],
            detailInit: function (e) {
                var item = e.data, division = $("#divisions .active input").val() || "General";
                $.get(urlStockWarehouse, { Subsidiary: resumeItem.subsidiary, Warehouse: item.warehouse, Division: division }, function (d) {
                    $("<div>").appendTo(e.detailCell).kendoGrid({
                        scrollable: false, sortable: true, pageable: false, selectable: true,
                        columns: [
                            { field: "line", title: "Línea", width: 200 },
                            { field: "total", title: "Stock", width: 120, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                        ],
                        dataSource: d.items,
                        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }
                    });
                    e.masterRow.closest(".k-window-content").css("max-height", "");
                    var wnd = $("#Detail").getKendoWindow();
                    wnd.setOptions({ height: 800 });
                    wnd.center();
                });
            }
        });
        if (Enumerable.From(chartList).Where("$.IdChart === 7").Count() === 0) {
            $("#switch-warehouses").addClass("d-none");
        }
    } else {
        $("#resumeSales").addClass("d-none");
    }

    if (Enumerable.From(chartList).Where("$.IdChart === 2 || $.IdChart === 8").Count() > 0) {
        $("#tableTopClients").kendoGrid({
            dataSource: getDatasourceTopClients([]),
            scrollable: false,
            sortable: true,
            pageable: false,
            selectable: true,
            columns: [
                { field: "name", title: "Cliente", footerTemplate: "TOTAL" },
                { field: "total", title: "Total", format: "{0:N2}", footerTemplate: "#:kendo.toString(sum, 'N2')#", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 90 },
                { field: "percentage", title: "Porcentaje", format: "{0:N2} %", footerTemplate: "#:kendo.toString(sum, 'N2')# %", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 80, minScreenWidth: 1120 },
                { field: "margin", title: "Margen", format: "{0:N2} %", width: 80, attributes: alignRight, headerAttributes: alignRight }
            ],
            dataBound: function (e) {
                var grid = e.sender;
                if (Enumerable.From(chartList).Where("$.IdChart === 8").Count() > 0) {
                    grid.showColumn("margin");
                } else {
                    grid.hideColumn("margin");
                }
            }
        });
    } else {
        $("#topClients").addClass("d-none");
    }

    $("#Detail").kendoWindow({ visible: false, open: onRefreshWindow, modal: true, iframe: false, scrollable: true, title: "OV Autorizadas", resizable: false, width: 1200, actions: ["Close"] });
    $("#Report").kendoWindow({ visible: false, width: 1100, title: "Producto", modal: true });
    $("#Detail2").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "Detalle Stock", resizable: false, width: 1100, actions: ["Close"], activate: function (e) {
            wnd = this;
            setTimeout(() => wnd.center(), 250);
        }
    });

    if (Enumerable.From(chartList).Where("$.IdChart === 11").Count() > 0) {
        $("#tableKbytes").kendoGrid({
            dataSource: { data: [], aggregate: [{ field: "amount", aggregate: "sum" }, { field: "points", aggregate: "sum" }] },
            scrollable: false,
            sortable: true,
            pageable: false,
            selectable: true,
            excel: { fileName: "Kbytes.xlsx" },
            columns: [
                { field: "code", title: "Cod.", width: 100, footerTemplate: "TOTAL" },
                { field: "name", title: "Nombre", width: 250 },
                { field: "status", title: "Status", width: 130 },
                { field: "amount", title: "Monto", format: "{0:N2}", footerTemplate: "#:kendo.toString(sum, 'N2')#", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 90 },
                { field: "points", title: "Puntos", format: "{0:N0}", footerTemplate: "#:kendo.toString(sum, 'N0')#", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, width: 90 }
            ]
        });
    }

    if (Enumerable.From(chartList).Where("$.IdChart === 5 || $.IdChart === 6").Count() === 0) {
        $("#sellersResume").addClass("d-none");
    }

    $("#kbytes-year").kendoNumericTextBox({ format: "####", change: () => loadKbytesData() });

    var productSince = $("#products-since").kendoDatePicker({
        format: "d/M/yyyy", value: initDate, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            productUntil.min(startDate ? startDate : _minDate);
            getProductsData();
        }
    }).data("kendoDatePicker");
    var productUntil = $("#products-until").kendoDatePicker({
        format: "d/M/yyyy", value: today, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            productSince.max(endDate ? endDate : _maxDate);
            getProductsData();
        }
    }).data("kendoDatePicker");

    var sellerResumeSince = $("#seller-resume-since").kendoDatePicker({
        format: "d/M/yyyy", value: initDate, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            sellerResumeUntil.min(startDate ? startDate : _minDate);
            getTeamData();
        }
    }).data("kendoDatePicker");
    var sellerResumeUntil = $("#seller-resume-until").kendoDatePicker({
        format: "d/M/yyyy", value: today, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            sellerResumeSince.max(endDate ? endDate : _maxDate);
            getTeamData();
        }
    }).data("kendoDatePicker");

    if (Enumerable.From(chartList).Where("$.ChartGroup === 'Regional'").Count() > 0) {
        getRegionalData();
    } else {
        $(".card-regional").addClass("d-none");
    }
    if (Enumerable.From(chartList).Where("$.ChartGroup === 'Kbytes'").Count() === 0) {
        $(".card-kbytes").addClass("d-none");
    }
    if (Enumerable.From(chartList).Where("$.ChartGroup === 'Productos'").Count() === 0) {
        $(".card-products").addClass("d-none");
    }
    if (Enumerable.From(chartList).Where("$.ChartGroup === 'Rendimiento de Equipo'").Count() === 0) {
        $(".card-team").addClass("d-none");
    }
}

function getRegionalData() {
    var objSince = $("#regional-since").data("kendoDatePicker").value();
    if (objSince) {
        objSince = objSince.toISOString();
    }
    var objUntil = $("#regional-until").data("kendoDatePicker").value();
    if (objUntil) {
        objUntil = objUntil.toISOString();
    }
    $.get(ulrSalesResume, { InitialDate: objSince, FinalDate: objUntil }, data => {
        if (data.message === "") {
            regionalData.authorized = data.authorized;
            regionalData.opens = data.opens;
            regionalData.sales = data.sales;
            regionalData.salesToday = data.salesToday;
            regionalData.stock = data.stock;
            regionalData.warehouses = data.warehouses;
            regionalData.projection = data.projection;
            regionalData.projectionChart = data.projectionChart;
            regionalData.notBilled = data.notBilled;

            loadRegionalResume("G", true);
            $("#divisions > div:first").addClass("active");
        } else {
            showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
        }
    });

    $.get(ulrTopClients, { InitialDate: objSince, FinalDate: objUntil }, data => {
        if (data.message === "") {
            regionalData.topClients = data.clients;
            loadTopClients("G", "General");
            $("#subsidiaries > div:first").addClass("active");
        } else {
            showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
        }
    });
}

function loadRegionalResume(division, grouped) {
    var items = [];
    gridGrouped = grouped;
    var filterDivision = division === "G" ? "" : (division === "C" ? "$.division === ''" : `$.division === '${division}'`);
    var total = Enumerable.From(regionalData.sales).Where(filterDivision).Select("$.total").Sum();
    regionalData.warehouses.forEach(x => {
        var item = { subsidiary: x.subsidiary, warehouse: x.warehouse };
        var filter = `$.subsidiary === '${x.subsidiary}' && $.warehouse === '${x.warehouse}'`;

        var filterDivisionStock = division === "M" ? "$.division === 'M'" : "";
        var filterStock = (filterDivisionStock !== "" && filter !== "" ? `${filterDivisionStock} &&` : filterDivisionStock) + filter;

        filter = (filterDivision !== "" && filter !== "" ? `${filterDivision} &&` : filterDivision) + filter;

        item.period = Enumerable.From(regionalData.sales).Where(filter).Select("$.total").Sum();
        item.marginTotal = Enumerable.From(regionalData.sales).Where(filter).Select("$.margin").Sum();
        item.taxlessTotal = Enumerable.From(regionalData.sales).Where(filter).Select("$.taxlessTotal").Sum();
        item.today = Enumerable.From(regionalData.salesToday).Where(filter).Select("$.total").Sum();
        item.margin = item.period > 0 ? (item.marginTotal / item.period) * 100 : 0;
        item.authorized = Enumerable.From(regionalData.authorized).Where(filter).Select("$.total").Sum();
        item.open = Enumerable.From(regionalData.opens).Where(filter).Select("$.total").Sum();
        item.stock = Enumerable.From(regionalData.stock).Where(filterStock).Select("$.total").Sum();
        item.percentage = total > 0 ? (item.period / total) * 100 : 0;
        item.notBilled = Enumerable.From(regionalData.notBilled).Where(filter).Select("$.total").Sum();

        items.push(item);
    });

    var grid = $("#tableResume").data("kendoGrid");
    var ds;
    if (grouped) {
        var lstGrouped = Enumerable.From(items).GroupBy("{ subsidiary: $.subsidiary }", null,
            function (key, g) {
                var today = 0, period = 0, percetage = 0, margin = 0, marginTotal = 0, authorized = 0, open = 0, stock = 0, taxless = 0, expected = 0, expectedDifference = 0, expectedDifferencePercentage = 0, notBilled = 0;
                g.ForEach(function (item) {
                    today += item.today;
                    period += item.period;
                    authorized += item.authorized;
                    open += item.open;
                    stock += item.stock;
                    marginTotal += item.marginTotal;
                    taxless += item.taxlessTotal;
                    notBilled += item.notBilled;
                });
                if (total > 0) {
                    percetage = period * 100 / total;
                }
                margin = (taxless > 0 ? (marginTotal / taxless) * 100 : 0);

                var filter = division === "G" ? `$.subsidiary === '${key.subsidiary}'` : `$.subsidiary === '${key.subsidiary}' && $.division === '${division}'`;
                expected = Enumerable.From(regionalData.projection).Where(filter).Select("$.amount").Sum();
                expectedDifference = period - expected;
                expectedDifferencePercentage = expected > 0 ? (expectedDifference / expected) * 100 : 0;

                var result = { subsidiary: key.subsidiary, today: today, period: period, margin: margin, notBilled: notBilled, percentage: percetage, authorized: authorized, open: open, stock: stock, taxlessTotal: taxless, marginTotal: marginTotal, extended: Enumerable.From(chartList).Where("$.IdChart === 7").Count() > 0, expected: expected, expectedDifference: expectedDifference, expectedDifferencePercentage: expectedDifferencePercentage };
                return result;
            },
            "$.subsidiary" // compare selector needed
        ).ToArray();
        ds = getDatasourceResume(lstGrouped);
    } else {
        items.forEach(x => x.extended = Enumerable.From(chartList).Where("$.IdChart === 7").Count() > 0);
        ds = getDatasourceResume(items);
    }
    grid.setDataSource(ds);

    var colors = Highcharts.getOptions().colors;
    var objChart = $("#chartResume");
    var objOptions;
    if (grouped) {
        var lstData = Enumerable.From(lstGrouped).Select("{ name: $.subsidiary, y: $.period }").ToArray();
        objOptions = {
            credits: false,
            chart: { plotBackgroundColor: null, plotBorderWidth: null, plotShadow: false, type: 'pie', backgroundColor: "transparent", width: 300 },
            title: { text: "" },
            tooltip: { pointFormat: 'Total: <b>{point.y:,.2f} ( {point.percentage:.2f} % )</b>' },
            plotOptions: { pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: false }, showInLegend: true } },
            series: [{ name: "Clientes", colorByPoint: true, data: lstData }]
        };
    } else {
        var lstSeries = [], lstLabels = [], lstSubsidiaries = [], lstWarehouses = [];
        var i = 0;
        Enumerable.From(items).OrderBy("$.subsidiary, $.warehouse").GroupBy("{ subsidiary: $.subsidiary }", null,
            function (key, g) {
                var total = 0;
                var totalCount = Enumerable.From(g.source).Where("$.period > 0").Count();
                var step = totalCount > 1 ? ((5 / (totalCount - 1)) / 10) : 0;
                i++;
                var x = 0;
                g.ForEach(function (item) {
                    total += item.period;
                    if (item.period > 0) {
                        var brightness = 0.2 - x * step;
                        lstWarehouses.push({ name: item.warehouse, y: item.period, color: new Highcharts.Color(colors[i]).brighten(brightness).get() });
                        x++;
                    }
                });
                lstSubsidiaries.push({ name: key.subsidiary, y: total === 0 ? null : total, color: new Highcharts.Color(colors[i]).brighten(-0.6).get() });
                return "";
            },
            "$.subsidiary" // compare selector needed
        ).ToArray();

        lstSeries = [
            { type: "pie", name: "Sucursales", data: lstSubsidiaries, size: "60%", dataLabels: { formatter: function () { return this.y > 5 ? this.point.name : null; }, color: "white", distance: -30 } },
            { type: "pie", name: "Almacenes", data: lstWarehouses, size: "80%", innerSize: "60%" }
        ];
        objOptions = {
            credits: false,
            chart: { type: "pie", backgroundColor: "transparent", width: 300 },
            title: { text: "Ventas por Sucursal / Almacen" },
            labels: { items: lstLabels },
            series: lstSeries,
            tooltip: { pointFormat: "{series.name}: <b>{point.y:,.2f}</b>", valueDecimals: 2 },
            exporting: { sourceWidth: 1400 },
            plotOptions: { pie: { dataLabels: { enabled: true, format: "<b>{point.name}</b><br /> {point.y:,.2f}" } } }
        };
    }
    objChart.highcharts(objOptions);

    var scp = Enumerable.From(regionalData.projectionChart.series).Where("$.name == 'Santa Cruz Proyectado' && $.division == '" + division + "'").Select("$.values").First(),
        scf = Enumerable.From(regionalData.projectionChart.series).Where("$.name == 'Santa Cruz Facturado' && $.division == '" + division + "'").Select("$.values").First(),
        iqp = Enumerable.From(regionalData.projectionChart.series).Where("$.name == 'Iquique Proyectado' && $.division == '" + division + "'").Select("$.values").First(),
        iqf = Enumerable.From(regionalData.projectionChart.series).Where("$.name == 'Iquique Facturado' && $.division == '" + division + "'").Select("$.values").First(),
        mip = Enumerable.From(regionalData.projectionChart.series).Where("$.name == 'Miami Proyectado' && $.division == '" + division + "'").Select("$.values").First(),
        mif = Enumerable.From(regionalData.projectionChart.series).Where("$.name == 'Miami Facturado' && $.division == '" + division + "'").Select("$.values").First(),
        labels = [];
    regionalData.projectionChart.labels.forEach((x) => labels.push(x));
    Highcharts.chart('chartProjection', {
        chart: { type: 'column' },
        title: { text: 'Proyección de Ventas' },
        xAxis: { categories: regionalData.projectionChart.labels },
        yAxis: [{ min: 0, title: { text: 'Ventas' } }],
        legend: { shadow: false },
        tooltip: { shared: true },
        plotOptions: { column: { grouping: false, shadow: false, borderWidth: 0 } },
        series: [{
            name: 'Santa Cruz Proyectado',
            color: 'rgba(21,87,36,.9)',
            data: scp,
            pointPadding: 0.3,
            pointPlacement: -0.3
        }, {
            name: 'Santa Cruz Facturado',
            color: 'rgba(195,230,134,1)',
            data: scf,
            pointPadding: 0.4,
            pointPlacement: -0.3
        }, {
            name: 'Iquique Proyectado',
            color: 'rgba(0,64,133,.9)',
            data: iqp,
            pointPadding: 0.3,
            pointPlacement: 0
        }, {
            name: 'Iquique Facturado',
            color: 'rgba(184,218,255,1)',
            data: iqf,
            pointPadding: 0.4,
            pointPlacement: 0
        }, {
            name: 'Miami Proyectado',
            color: 'rgba(248,161,203,.9)',
            data: mip,
            pointPadding: 0.3,
            pointPlacement: 0.3
        }, {
            name: 'Miami Facturado',
            color: 'rgba(186,60,61,1)',
            data: mif,
            pointPadding: 0.4,
            pointPlacement: 0.3
        }],
        credits: false
    });

}

function loadTopClients(division, region) {
    var filterDivision = division === "G" ? "" : (division === "C" ? "$.division === ''" : `$.division === '${division}'`);
    var filterRegion = region === "General" ? "" : `$.subsidiary === '${region}'`;
    var filter = (filterDivision !== "" && filterRegion !== "" ? `${filterDivision} && ` : filterDivision) + filterRegion;
    var total = Enumerable.From(regionalData.topClients).Where(filter).Select("$.total").Sum();
    var items = Enumerable.From(regionalData.topClients).Where(filter).GroupBy("{ code: $.code, name: $.name }", null,
        function (key, g) {
            var totalClient = 0, marginTotal = 0, margin = 0, taxlessTotal = 0, percentage = 0;
            g.ForEach(x => {
                totalClient += x.total;
                marginTotal += x.marginTotal;
                taxlessTotal += x.taxlessTotal;
            });
            if (total > 0) percentage = totalClient * 100 / total;
            margin = (taxlessTotal > 0 ? (marginTotal / taxlessTotal) * 100 : 0);
            var result = { code: key.code, name: key.name, total: totalClient, marginTotal: marginTotal, margin: margin, taxlessTotal: taxlessTotal, percentage: percentage };
            return result;
        }, "$.code, $.name").ToArray();

    var grid = $("#tableTopClients").data("kendoGrid");
    var itemsFiltered = Enumerable.From(items).OrderByDescending("$.total").Take(10).ToArray();
    var ds = getDatasourceTopClients(itemsFiltered);
    grid.setDataSource(ds);

    var colors = Highcharts.getOptions().colors;
    var itemsChart = Enumerable.From(itemsFiltered).Select("{ name: $.name, y: $.total }").ToArray();
    var totalGE = Enumerable.From(itemsFiltered).Select("$.total").Sum();
    if (items.length > 10) {
        itemsChart.push({ name: "Varios", y: total - totalGE, sliced: true, selected: true, color: new Highcharts.Color(colors[5]).brighten(-0.6).get() });
    }
    $("#chartTopClients").highcharts({
        credits: false,
        chart: { plotBackgroundColor: null, plotBorderWidth: null, plotShadow: false, type: 'pie', backgroundColor: "transparent" },
        title: { text: "" },
        tooltip: { pointFormat: 'Total: <b>{point.y:,.2f} ( {point.percentage:.2f} % )</b>' },
        plotOptions: { pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: false }, showInLegend: true } },
        series: [{ name: "Clientes", colorByPoint: true, data: itemsChart }]
    });
}

function openBilledPeriod(e, type) {
    var url, params;

    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var divisionSelected = $("#divisions .active input").val();
    var wnd = $("#Detail").getKendoWindow();

    if (type === "P") {
        var objSince = $("#regional-since").data("kendoDatePicker").value();
        if (objSince) objSince = objSince.toISOString();

        var objUntil = $("#regional-until").data("kendoDatePicker").value();
        if (objUntil) objUntil = objUntil.toISOString();
        url = urlPeriodBills;
        params = { Subsidiary: item.subsidiary, Warehouse: item.warehouse, Division: divisionSelected, InitialDate: objSince, FinalDate: objUntil };
        wnd.title("Facturadas Período");
    } else {
        url = urlTodayBills;
        params = { Subsidiary: item.subsidiary, Warehouse: item.warehouse, Division: divisionSelected };
        wnd.title("Facturadas Hoy");
    }

    $("#notesBilled").removeClass("d-none");
    $("#openOVs").addClass("d-none");
    $("#stockDetail").addClass("d-none");
    $("#justDelivered").addClass("d-none");

    //var divisionSelected = $("#divisions .active input").val();
    var grid = $("#notesBilled").data("kendoGrid");
    $.get(url, params, d => {
        d.items.forEach(x => { x.docDate = JSON.toDate(x.docDate); });
        var ds = new kendo.data.DataSource({ data: d.items });
        grid.setDataSource(ds);
        wnd.open();
    });
}

function openDelivered(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item, divisionSelected, wnd, grid, ds;
    grd.select(row);
    item = grd.dataItem(row);
    divisionSelected = $("#divisions .active input").val();
    wnd = $("#Detail").getKendoWindow();

    $("#notesBilled").addClass("d-none");
    $("#openOVs").addClass("d-none");
    $("#stockDetail").addClass("d-none");
    $("#justDelivered").removeClass("d-none");

    wnd.title("Notas de Entrega sin Facturar");
    $.get(urlOpenDeliveryNotes, { Subsidiary: item.subsidiary, Warehouse: item.warehouse, Division: divisionSelected }, d => {
        grid = $("#justDelivered").data("kendoGrid");
        d.items.forEach(x => x.docDate = JSON.toDate(x.docDate));
        ds = new kendo.data.DataSource({ data: d.items });
        grid.setDataSource(ds);
        wnd.open();
    });
}

function marginTotal(e) { return kendo.toString(e.taxlessTotal.sum > 0 ? (e.marginTotal.sum / e.taxlessTotal.sum) * 100 : 0, "N2") + " %"; }

function getDatasourceResume(items) {
    var ds = new kendo.data.DataSource({
        data: items,
        aggregate: [{ field: "period", aggregate: "sum" }, { field: "today", aggregate: "sum" }, { field: "marginTotal", aggregate: "sum" }, { field: "taxlessTotal", aggregate: "sum" }, { field: "authorized", aggregate: "sum" }, { field: "open", aggregate: "sum" }, { field: "stock", aggregate: "sum" }, { field: "expected", aggregate: "sum" }, { field: "expectedDifference", aggregate: "sum" }, { field: "notBilled", aggregate: "sum" }]
    });
    return ds;
}

function getDatasourceTopClients(items) {
    var ds = new kendo.data.DataSource({
        data: items,
        aggregate: [{ field: "total", aggregate: "sum" }, { field: "percentage", aggregate: "sum" }]
    });
    return ds;
}

function isEmpty(el) {
    return !$.trim(el.html())
}

function loadReport(Id, Subsidiary, Report, SearchBy) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    if (Report === "Note") {
        strReport = "SaleNote.trdp";
    }
    if (Report === "Delivery") {
        strReport = "DeliveryNote.trdp";
        objParams.SearchType = SearchBy;
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
            console.error(`Error al recargar el reporte: ${e}`);
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        try {
            $("#reportViewer1").telerik_ReportViewer({
                serviceUrl: urlService,
                reportSource: { report: strReport, parameters: objParams },
                scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
            });
        } catch (e) {
            console.error(`Error al cargar el reporte: ${e}`);
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    }
}

function loadKbytesData() {
    cleanDetail();
    var resumeDiv = $("#resumes");
    $.get(urlKbytes, {}, data => {
        if (data.message === "") {
            kbytesLoaded = true;
            var tempYear = `<div class="panel slide">
                                <div class="">
                                    <div class="text-center details-title">
                                        <div>
                                            <span class="small-label">año</span><br />
                                            <span class="year-title">#=year#</span><br />
                                        </div>                    
                                        <a class="see-details" data-type="N" data-year="#=year#"><span class="small-label action">ver detalle</span></a>
                                    </div>
                                    <div class="text-right details">
                                        <div>
                                            <span class="year-values">#=kendo.toString(amount, "N2")#</span><br />
                                            <span class="small-label">monto acumulado</span>
                                        </div>
                                        <div>
                                            <span class="year-values">#=kendo.toString(points, "N0")#</span><br />
                                            <span class="small-label">puntos acumulados</span>
                                        </div>                 
                                    </div>
                                </div>
                            </div>`;
            var template = kendo.template(tempYear);

            var titlesDiv = $("<div>").addClass("width-panel");
            var divName = `<div class="panel available width-panel">
                                <div class="row">
                                    <div class="col text-center">
                                        <div>                                                
                                            <span class="points-title">${kendo.toString(data.availablePoints, "N0")}</span><br />                                       
                                            <span class="small-label">puntos disponibles</span>&nbsp;&nbsp;&nbsp;<a class="see-details" data-type="AP" data-year="0"><span class="small-label action">ver detalle</span></a>
                                        </div>
                                    </div>                                       
                                </div>
                            </div>`;
            titlesDiv.append(divName);

            if (data.claimedPoints > 0) {
                var divUsed = `<div class="panel  width-panel">
                                   <div class="row">
                                       <div class="col text-center">
                                            <div>
                                                <span class="small-label">puntos usados</span>
                                                <span class="points-title">${kendo.toString(data.claimedPoints, "N0")}</span>                                                
                                            </div>
                                            <a class="see-details" data-type="UP" data-year="0"><span class="small-label action">ver detalle</span></a>
                                       </div>                                       
                                   </div>
                               </div>`;
                titlesDiv.append(divUsed);
            }
            resumeDiv.append(titlesDiv);

            var slider = $("<div>").attr("id", "years-slider").addClass("slider-2");
            data.years.forEach((x) => {
                slider.append(template(x));
            });
            slider.append(`<div onclick="prev()" class="control-btn prev"><i class="fas fa-arrow-left"></i></div>
                           <div onclick="next()" class="control-btn next"><i class="fas fa-arrow-right"></i></div>`);
            resumeDiv.append(slider);
        } else {
            showError(`Se ha producido el siguiente error al traer los datos del servidor:<br /> ${data.message}`);
        }
    });
}

function cleanDetail() {
    $("#resumes").empty();
    loadGridNotes([], 0);
    $("#notesContainer").addClass("d-none");
    loadGridAwards([]);
    $("#awardsContainer").addClass("d-none");
    //loadGridPoints([]);
    $("#pointsContainer").addClass("d-none");

    //$("#action-excel").addClass("d-none");
    //$("#action-add").addClass("d-none");
}

function loadGridAwards(items) {
    items.forEach((x) => { x.claimDate = JSON.toDate(x.claimDate); });

    $("#notesContainer").addClass("d-none");
    $("#pointsContainer").addClass("d-none");
    $("#awardsContainer").removeClass("d-none");

    var grd = $("#listAwards").data("kendoGrid");
    if (grd) {
        var ds = new kendo.data.DataSource({ data: items });
        grd.setDataSource(ds);
    } else {
        $("#listAwards").kendoGrid({
            toolbar: ["excel"],
            dataSource: { data: items },
            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
            columns: [
                { field: "cardName", title: "Cliente", width: 300 },
                { field: "claimDate", title: "Fecha", format: "{0:dd-MM-yyyy}", width: 100, attributes: alignCenter, headerAttributes: alignCenter },
                { field: "quantity", title: "Cantidad", width: 80, attributes: alignRight, headerAttributes: alignRight },
                { field: "award", title: "Premio", width: 450 },
                { field: "points", title: "Puntos Usados", width: 120, format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight }
            ]
        });
    }
}

function loadGridNotes(items, year) {
    $("#notesContainer").removeClass("d-none");
    $("#awardsContainer").addClass("d-none");
    $("#pointsContainer").addClass("d-none");

    var grd = $("#listNotes").data("kendoGrid"), getDS, columns, getDatabound;
    if (year >= 2023) {
        getDS = (e) => new kendo.data.DataSource({ data: e, sort: { field: "amount", dir: "desc" }, aggregate: [{ aggregate: "sum", field: "amount" }, { aggregate: "sum", field: "points" }] });
        columns = [
            { field: "cardCode", title: "Cod. Cliente", width: 90 },
            { field: "cardName", title: "Cliente", width: 250 },
            { field: "points", title: "Puntos", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.points.sum !== null ? kendo.toString(e.points.sum, "N0") : "", width: 100 },
            { field: "amount", format: "{0:N2}", title: "Monto", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.amount.sum !== null ? kendo.toString(e.amount.sum, "N2") : "", width: 100 }
        ];
        getDatabound = (e) => {
            var grid = e.sender;
            grid.element.find("table").attr("style", "");
        }
    } else {
        items.forEach(x => {
            x.year = year;
            if (x.amountQ1 === 0 & x.pointsQ1 === 0) {
                x.amountQ1 = null;
                x.pointsQ1 = null;
                x.statusQ1 = null;
            }
            if (x.amountQ2 === 0 & x.pointsQ2 === 0) {
                x.amountQ2 = null;
                x.pointsQ2 = null;
                x.statusQ2 = null;
            }
            if (x.amountQ3 === 0 & x.pointsQ3 === 0) {
                x.amountQ3 = null;
                x.pointsQ3 = null;
                x.statusQ3 = null;
            }
            if (x.amountQ4 === 0 & x.pointsQ4 === 0) {
                x.amountQ4 = null;
                x.pointsQ4 = null;
                x.statusQ4 = null;
            }
        });

        getDS = (e) => new kendo.data.DataSource({
            data: e, sort: { field: "totalAmount", dir: "desc" }, aggregate: [
                { aggregate: "sum", field: "amountQ1" }, { aggregate: "sum", field: "amountQ2" }, { aggregate: "sum", field: "amountQ3" }, { aggregate: "sum", field: "amountQ4" },
                { aggregate: "sum", field: "pointsQ1" }, { aggregate: "sum", field: "pointsQ2" }, { aggregate: "sum", field: "pointsQ3" }, { aggregate: "sum", field: "pointsQ4" },
                { aggregate: "sum", field: "totalPoints" }, { aggregate: "sum", field: "totalAmount" }
            ]
        });
        columns = [
            { field: "cardCode", title: "Cod. Cliente", width: 90 },
            { field: "cardName", title: "Cliente", width: 250 },
            {
                title: "Q1", headerAttributes: alignCenter, columns: [
                    { field: "pointsQ1", title: "Puntos", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.pointsQ1.sum !== null ? kendo.toString(e.pointsQ1.sum, "N0") : "", width: 100 },
                    { field: "amountQ1", format: "{0:N2}", title: "Monto", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.amountQ1.sum !== null ? kendo.toString(e.amountQ1.sum, "N2") : "", width: 100 },
                    { field: "statusQ1", title: "Estatus", width: 100 }
                ]
            },
            {
                title: "Q2", headerAttributes: alignCenter, columns: [
                    { field: "pointsQ2", title: "Puntos", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.pointsQ2.sum !== null ? kendo.toString(e.pointsQ2.sum, "N0") : "", width: 100 },
                    { field: "amountQ2", format: "{0:N2}", title: "Monto", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.amountQ2.sum !== null ? kendo.toString(e.amountQ2.sum, "N2") : "", width: 100 },
                    { field: "statusQ2", title: "Estatus", width: 100 }
                ]
            },
            {
                title: "Q3", headerAttributes: alignCenter, columns: [
                    { field: "pointsQ3", title: "Puntos", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.pointsQ3.sum !== null ? kendo.toString(e.pointsQ3.sum, "N0") : "", width: 100 },
                    { field: "amountQ3", format: "{0:N2}", title: "Monto", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.amountQ3.sum !== null ? kendo.toString(e.amountQ3.sum, "N2") : "", width: 100 },
                    { field: "statusQ3", title: "Estatus", width: 100 }
                ]
            },
            {
                title: "Q4", headerAttributes: alignCenter, columns: [
                    { field: "pointsQ4", title: "Puntos", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.pointsQ4.sum !== null ? kendo.toString(e.pointsQ4.sum, "N0") : "", width: 100 },
                    { field: "amountQ4", format: "{0:N2}", title: "Monto", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.amountQ4.sum !== null ? kendo.toString(e.amountQ4.sum, "N2") : "", width: 100 },
                    { field: "statusQ4", title: "Estatus", width: 100 }
                ]
            },
            {
                title: "TOTAL", headerAttributes: alignCenter, columns: [
                    { field: "totalPoints", title: "Puntos", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.totalPoints.sum !== null ? kendo.toString(e.totalPoints.sum, "N0") : "", width: 100 },
                    { field: "totalAmount", format: "{0:N2}", title: "Monto", headerAttributes: alignRight, attributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e.totalAmount.sum !== null ? kendo.toString(e.totalAmount.sum, "N2") : "", width: 100 }
                ]
            }
        ];
        getDatabound = (e) => {
            var grid = e.sender;
            var currentYear = Enumerable.From(grid.dataSource.data()).Select("$.year").FirstOrDefault();
            if (currentYear > "2021") {
                grid.showColumn("statusQ1");
                grid.showColumn("statusQ2");
                grid.showColumn("statusQ3");
                grid.showColumn("statusQ4");
            } else {
                grid.hideColumn("statusQ1");
                grid.hideColumn("statusQ2");
                if (currentYear === "2021") {
                    grid.showColumn("statusQ3");
                    grid.showColumn("statusQ4");
                } else {
                    grid.hideColumn("statusQ3");
                    grid.hideColumn("statusQ4");
                }
            }
            grid.element.find("table").attr("style", "");
        }
    }
    if (grd) {
        var ds = getDS(items);
        grd.setOptions({ dataSource: getDS(items), columns: columns });
    } else {
        $("#listNotes").kendoGrid({
            toolbar: ["excel"],
            dataSource: getDS(items),
            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
            columns: columns,
            dataBound: getDatabound
        });
    }
}

function loadGridPoints(items) {
    $("#notesContainer").addClass("d-none");
    $("#awardsContainer").addClass("d-none");
    $("#pointsContainer").removeClass("d-none");

    var resultItems = [];
    var years = [...new Set(items.map(x => x.year).sort())]; //Enumerable.From(items).Select("$.year").Distinct().ToArray();
    var clientCodes = [...new Map(items.map(x => [x.cardCode, { cardCode: x.cardCode, cardName: x.cardName }])).values()]; //Enumerable.From(items).Distinct("$.cardCode").Select("{ cardCode: $.cardCode, cardName: $.cardName }").ToArray();
    clientCodes.forEach((c) => {
        var total = 0, item = { cardCode: c.cardCode, cardName: c.cardName };
        years.forEach((y) => {
            var x = items.find(i => i.cardCode === c.cardCode && i.year === y); //Enumerable.From(items).Where("$.cardCode === '" + c.cardCode + "' && $.year === " + y).Select("$").FirstOrDefault();
            if (x) {
                item["points" + y] = x.points;
                item["claimed" + y] = x.claimed;
                total = total + x.points + x.claimed;
            } else {
                item["points" + y] = 0;
                item["claimed" + y] = 0;
            }
        });
        item.total = total;
        resultItems.push(item);
    });

    var grd = $("#listPoints").data("kendoGrid");
    if (grd) {
        var ds = new kendo.data.DataSource({ data: resultItems, sort: { field: "cardName", dir: "asc" }, aggregate: [{ aggregate: "sum", field: "total" }] });
        grd.setDataSource(ds);
    } else {
        var columns = [
            { field: "cardCode", title: "Cod. Cliente", width: 90 },
            { field: "cardName", title: "Cliente", width: 300 }
        ];
        years.forEach((x) => {
            columns.push({ title: `Puntos ${x}`, headerAttributes: alignCenter, columns: [{ field: "points" + x, title: "Acumulado", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, width: 100 }, { field: "claimed" + x, format: "{0:N0}", title: "Usados", headerAttributes: alignRight, attributes: alignRight, width: 100 }] });
        });
        columns.push({ title: "TOTAL", field: "total", format: "{0:N0}", headerAttributes: alignRight, attributes: alignRight, width: 100 });
        $("#listPoints").kendoGrid({
            toolbar: ["excel"],
            dataSource: { data: resultItems, sort: { field: "amount", dir: "desc" } },
            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
            columns: columns
        });
    }
}

function getProductsData() {
    var objSince = $("#products-since").data("kendoDatePicker").value();
    if (objSince) {
        objSince = kendo.toString(objSince, "yyyy-MM-dd")
    }
    var objUntil = $("#products-until").data("kendoDatePicker").value();
    if (objUntil) {
        objUntil = kendo.toString(objUntil, "yyyy-MM-dd");
    }
    if (Enumerable.From(chartList).Where("$.IdChart === 3").Count() > 0) {
        $.get(urlResumeLines, { InitialDate: objSince, FinalDate: objUntil }, data => {
            if (data.message === "") {
                productsLoaded = true;
                productData.lines = data.chart;
                loadLines();
            } else {
                showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
            }
        });
    }

    if (Enumerable.From(chartList).Where("$.IdChart === 4").Count() > 0) {
        $.get(urlResumeCategories, { InitialDate: objSince, FinalDate: objUntil }, data => {
            if (data.message === "") {
                productsLoaded = true;
                productData.categories = data.chart;
                loadCategories();
            } else {
                showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
            }
        });
    }
}

function loadLines() {
    var colors = Highcharts.getOptions().colors;
    Highcharts.setOptions({ lang: { decimalPoint: ".", thousandsSep: "," } });

    var objOptions;
    var lstSeries = [], lstLabels = [], objChart = productData.lines;
    $.each(objChart.series, function (j, objSerie) {
        var lstData = [];
        $.each(objSerie.data, function (k, objData) {
            lstData.push({ name: objData.label, y: objData.value, color: new Highcharts.Color(colors[k % 10]).brighten(0.3 - (k / 10) / 5).get() });
        });
        lstSeries.push({ type: objSerie.type, name: objSerie.name, data: lstData });
    });
    objOptions = {
        credits: false,
        title: { text: objChart.title },
        xAxis: { categories: objChart.labels },
        yAxis: { min: 0, title: { text: "Monto en $us" }, stackLabels: { enabled: true, format: "{total:,.2f}", rotation: -45, y: -30 } },
        series: lstSeries,
        legend: { enabled: false },
        tooltip: { pointFormat: "{series.name}: <b>{point.y:,.2f}</b>" },
        exporting: { sourceWidth: 1400, sourceHeight: 900 },
        plotOptions: { column: { stacking: "normal" } },
        chart: { height: 500, backgroundColor: "transparent" }
    };

    $("#chartLines").highcharts(objOptions);
}

function loadCategories() {
    var colors = Highcharts.getOptions().colors;
    Highcharts.setOptions({ lang: { decimalPoint: ".", thousandsSep: "," } });

    var categorySeries = [], subcategorySeries = [], objChart = productData.categories;
    $.each(objChart.series, function (i, obj) {
        var lstData = [];
        $.each(obj.data, function (j, obj2) {
            //var css = this.chart.options.drilldown.activeDataLabelStyle;
            //css.textDecoration = 'none';
            lstData.push({ name: obj2.label, y: obj2.value, drilldown: obj2.label });
        });
        categorySeries.push({ name: obj.name, colorByPoint: true, data: lstData });
    });
    $.each(objChart.drillDown, function (i, obj) {
        var lstData = [];
        $.each(obj.data, function (j, obj2) {
            lstData.push({ name: obj2.label, y: obj2.value });
        });
        subcategorySeries.push({ name: obj.name, id: obj.name, data: lstData });
    });

    $("#chartCategories").highcharts({
        credits: false,
        chart: { type: "column", backgroundColor: "transparent" },
        title: { text: objChart.title },
        xAxis: { type: 'category' },
        yAxis: { title: { text: "Monto en $us" } },
        legend: { enabled: false },
        plotOptions: { series: { borderWidth: 0, dataLabels: { enabled: true, format: '{point.y:,.2f}' } } },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:,.2f}</b><br/>'
        },
        series: categorySeries,
        labels: { style: { "text-decoration": "none", color: "blue" } },
        exporting: { sourceWidth: 1400, sourceHeight: 900 },
        drilldown: { series: subcategorySeries }
    });
}

function getEventsData() {
    $("#events-container").empty();
    $.get(ulrLastEvents, {}, function (data) {
        if (data.length > 0) {
            $.each(data, function (i, obj) {
                var div, divBox, divInner, h4, p, hidden, spDate;
                p = $("<span>").text(obj.description);
                obj.date = JSON.toDate(obj.date);
                spDate = $("<span>").addClass("date");
                if (obj.date.getHours() == 0 && obj.date.getMinutes() == 0) {
                    spDate.text(kendo.toString(obj.date, "dd/MM/yyyy"));
                } else {
                    spDate.text(kendo.toString(obj.date, "dd/MM/yyyy HH:mm"));
                }
                if (obj.detail) {
                    var url = urlEvent + "/" + obj.id;
                    var link = $("<a class='event'>").attr("href", url).text(obj.name);
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
                $("#events-container").append(div);
            });
            //$(".box-container").after("<a href='" + _urlAllEvents + "'>Ver todos los eventos</a>");
        }
    });
}

function getTeamData() {
    var objSince = $("#seller-resume-since").data("kendoDatePicker").value();
    if (objSince) {
        objSince = kendo.toString(objSince, "yyyy-MM-dd")
    }
    var objUntil = $("#seller-resume-until").data("kendoDatePicker").value();
    if (objUntil) {
        objUntil = kendo.toString(objUntil, "yyyy-MM-dd");
    }
    if (Enumerable.From(chartList).Where("$.IdChart === 10").Count() > 0) {
        if (!teamLoaded) {
            $.get(urlTeamQuota, {}, data => {
                if (data.message === "") {
                    teamLoaded = true;
                    var lstSeries = [], objChartData = data.chart;
                    $.each(objChartData.series, function (j, objSerie) {
                        var lstData = [];
                        $.each(objSerie.data, function (k, objData) {
                            lstData.push({ name: objData.label, y: objData.percentage, percentage: objData.value, total: objData.total });
                        });
                        lstSeries.push({ type: objSerie.type, name: objSerie.name, data: lstData });
                    });
                    var objOptions = {
                        credits: false,
                        chart: { type: "bar", backgroundColor: "transparent", height: 600 },
                        title: { text: objChartData.title },
                        xAxis: { categories: objChartData.labels },
                        yAxis: { min: 0, title: { text: "Cumplimiento" } },
                        legend: { enabled: false },
                        series: lstSeries,
                        tooltip: {
                            formatter: function () {
                                return "<b>" + this.point.category + "</b><br/>Cumplimiento: " + Highcharts.numberFormat(Math.abs(this.point.percentage), 2) + " $US"; // + " de " + Highcharts.numberFormat(Math.abs(this.point.total), 2);
                            }
                        },
                        plotOptions: { bar: { dataLabels: { enabled: true, format: "{point.y:,.2f} %" } } }
                    };
                    $("#chartTeamQuota").highcharts(objOptions);
                } else {
                    showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
                }
            });
        }
    }
    if (Enumerable.From(chartList).Where("$.IdChart === 5").Count() > 0) {
        if (objSince && objUntil) {
            $.get(urlTeamResume, { InitialDate: objSince, FinalDate: objUntil }, data => {
                if (data.message === "") {
                    teamLoaded = true;

                    var colors = Highcharts.getOptions().colors;
                    var objOptions;
                    var lstSeries = [], lstLabels = [], objChartData = data.chart;
                    $.each(objChartData.series, function (j, objSerie) {
                        var lstData = [];
                        $.each(objSerie.data, function (k, objData) {
                            lstData.push({ name: objData.label, y: objData.value, color: new Highcharts.Color(colors[k % 10]).brighten(0.3 - (k / 10) / 5).get() });
                        });
                        lstSeries.push({ type: objSerie.type, name: objSerie.name, data: lstData });
                    });
                    objOptions = {
                        credits: false,
                        title: { text: objChartData.title },
                        xAxis: { categories: objChartData.labels },
                        yAxis: { min: 0, title: { text: "Monto en $us" }, stackLabels: { enabled: true, format: "{total:,.2f}", rotation: -45, y: -30 } },
                        series: lstSeries,
                        legend: { enabled: false },
                        tooltip: { pointFormat: "{series.name}: <b>{point.y:,.2f}</b>" },
                        exporting: { sourceWidth: 1400, sourceHeight: 900 },
                        plotOptions: { column: { stacking: "normal" } },
                        chart: { height: 500, backgroundColor: "transparent" }
                    };
                    $("#chartSellersResume").highcharts(objOptions);

                } else {
                    showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
                }
            });
        }
    }
    if (Enumerable.From(chartList).Where("$.IdChart === 6").Count() > 0) {
        if (objSince && objUntil) {
            $.get(urlTeamResumeInTime, { InitialDate: objSince, FinalDate: objUntil }, data => {
                if (data.message === "") {
                    teamLoaded = true;

                    objChartData = data.chart;
                    lstSeries = [];
                    $.each(objChartData.series, function (j, objSerie) {
                        var lstData = [];
                        $.each(objSerie.data, function (k, objData) {
                            if (objData.value > 0) {
                                lstData.push({ name: objData.label, y: objData.value });
                            } else {
                                lstData.push({ name: objData.label, y: null });
                            }
                        });
                        lstSeries.push({ type: objSerie.type, name: objSerie.name, data: lstData });
                    });
                    var objOptions2 = {
                        credits: false,
                        chart: { backgroundColor: "transparent", height: 500 },
                        title: { text: objChartData.title, x: -20 },
                        xAxis: { categories: objChartData.labels },
                        yAxis: { title: { text: "Monto en Us$" }, plotLines: [{ value: 0, width: 1, color: '#808080' }] },
                        tooltip: { valueSuffix: " Us$" },
                        legend: { layout: 'vertical', align: 'right', verticalAlign: 'middle', borderWidth: 0 },
                        exporting: { sourceWidth: 1400, sourceHeight: 900 },
                        series: lstSeries
                    };
                    $("#chartSellersOnTime").highcharts(objOptions2);
                } else {
                    showError(`Se ha producido un error al traer los datos del servidor: <br />${data.message}`);
                }
            });
        }
    }
}

function templateExpectedDiff(e) {
    var cls = e.expectedDifference >= 0 ? "expected-positive" : "expected-negative";
    return `<span class="${cls}">${kendo.toString(e.expectedDifference, "N2")}</span>`;
}

function footerTemplateExpectedDiff(e) {
    var cls = e.expectedDifference.sum >= 0 ? "expected-positive" : "expected-negative";
    return `<span class="${cls}">${kendo.toString(e.expectedDifference.sum, 'N2')}</span>`;
}

function templateExpectedDiffPercentage(e) {
    var cls = e.expectedDifference >= 0 ? "expected-positive" : "expected-negative";
    return `<span class="${cls}">${kendo.toString(e.expectedDifferencePercentage, 'N2')} %</span>`;
}

function footerTemplateExpectedDiffPercentage(e) {
    var cls = e.expectedDifference.sum >= 0 ? "expected-positive" : "expected-negative", expected = e.expected.sum > 0 ? e.expected.sum : 0;
    return `<span class="${cls}">${kendo.toString(expected > 0 ? ((e.expectedDifference.sum / expected) * 100) : 0, 'N2')} %</span>`;
}

function templateColumnMobile(e) {
    //regionalData.projection
    var ext1 = e.extended ? `<div class="col-6"><span class="title">Margen: </span> ${kendo.toString(e.margin, "N2")} %</div>` : ``,
        ext2 = e.extended ? `<div class="col-6"><span class="title">Autorizadas: </span> ${(seeSalesDetail ? `<a class="aov action action-link">` : ``)}${kendo.toString(e.authorized, "N2")}${(seeSalesDetail ? `</a>` : ``)}</div>` : ``,
        ext3 = e.extended ? `<div class="col-6"><span class="title">Stock: </span> <a class="stockvalue action action-link">${kendo.toString(e.stock, "N2")}</a></div>` : ``,
        cls = e.expectedDifference >= 0 ? "expected-positive" : "expected-negative",
        notBilled = regionalData.notBilled && regionalData.notBilled.length > 0 ? `<div class="col-12"><span class="title">S&oacute;lo entregado: </span> ${(seeSalesDetail ? `<a class='delivered-period action action-link'>` : ``)}${kendo.toString(e.notBilled, "N2")}</a></div>` : ''
    template = `<div class="row" data-uid="${e.uid}">
                    <div class="col-12"><span class="title subsidiary">${e.subsidiary}</span></div>
                    <div class="col-6"><span class="title">Fact. Hoy: </span> <a class="billed-today action action-link">${kendo.toString(e.today, "N2")}</a></div>
                    <div class="col-6"><span class="title">Fact. Periodo: </span> <a class="billed-period action action-link">${kendo.toString(e.period, "N2")}</a></div>
                    ${notBilled}
                    ${ext1}
                    <div class="col-6"><span class="title">Porcentage: </span> ${kendo.toString(e.percentage, "N2")} %</div>
                    ${(e.expected > 0 ? `<div class="col-6"><span class="title">Proyectado: </span> ${kendo.toString(e.expected, "N2")}</div>` : '')}
                    ${(e.expected > 0 ? `<div class="col-6"><span class="title">Dif. Proyectado: </span> <span class="${cls}">${kendo.toString(e.expectedDifference, "N2")}</span></div>` : '')}
                    ${(e.expected > 0 ? `<div class="col-12"><span class="title">Dif. Proy. Porc.: </span> <span class="${cls}">${kendo.toString(e.expectedDifferencePercentage, "N2")} %</span></div>` : '')}
                    ${ext2}
                    <div class="col-6"><span class="title">Abiertas: </span> <a class="openov action action-link">${kendo.toString(e.open, "N2")}</a></div>
                    ${ext3}
                </div>`;
    return template;
}

function footerTemplateColumnMobile(e) {
    var ext1 = _canSee ? `<div class="col-6"><span class="title">Margen: </span> ${kendo.toString(e.period.sum > 0 ? (e.marginTotal.sum / e.taxlessTotal.sum) * 100 : 0, "N2")} %</div>` : ``,
        ext2 = _canSee ? `<div class="col-6"><span class="title">Autorizadas: </span> <a class="aov action action-link">${kendo.toString(e.authorized.sum, "N2")}</a></div>` : ``,
        ext3 = _canSee ? `<div class="col-6"><span class="title">Stock: </span> <a class="stockvalue action action-link">${kendo.toString(e.stock.sum, "N2")}</a></div>` : ``,
        cls = e.expectedDifference.sum >= 0 ? "expected-positive" : "expected-negative", expected = e.expected.sum > 0 ? e.expected.sum : 0,
        notBilled = regionalData.notBilled && regionalData.notBilled.length > 0 ? `<div class="col-12"><span class="title">S&oacute;lo entregado: </span> <span>${kendo.toString(e.notBilled.sum, "N2")}</span></div>` : ''
    template = `<div class="row">
                        <div class="col-12"><span class="title subsidiary">Total</span></div>
                        <div class="col-6"><span class="title">Fact. Hoy: </span> <a class="billed-today action action-link">${kendo.toString(e.today.sum, "N2")}</a></div>
                        <div class="col-6"><span class="title">Fact. Periodo: </span> <a class="billed-period action action-link">${kendo.toString(e.period.sum, "N2")}</a></div>
                        ${notBilled}
                        ${ext1}
                        ${(e.expected.sum > 0 ? `<div class="col-6"><span class="title">Proyectado: </span> ${kendo.toString(e.expected.sum, "N2")}</div>` : '')}
                        ${(e.expected.sum > 0 ? `<div class="col-6"><span class="title">Dif. Proyectado: </span> <span class="${cls}">${kendo.toString(e.expectedDifference.sum, "N2")}</span></div>` : '')}
                        ${(e.expected.sum > 0 ? `<div class="col-6"><span class="title">Dif. Proy. Porc.: </span> <span class="${cls}">${kendo.toString(expected > 0 ? ((e.expectedDifference.sum / expected) * 100) : 0, "N2")} %</span></div>` : '')}
                        ${ext2}
                        <div class="col-6"><span class="title">Abiertas: </span> <a class="openov action action-link">${kendo.toString(e.open.sum, "N2")}</a></div>
                        ${ext3}
                    </div>`;
    return template;
}

function showHideAccordion(e) {
    $(e.target).prev().find("i:last-child").toggleClass("fa-angle-up fa-angle-down");
    if (e.type === "show") {
        if (e.target.id === "contentKbytes" && !kbytesLoaded) {
            loadKbytesData();
        }
        if (e.target.id === "contentProducts" && !productsLoaded) {
            getProductsData();
        }
        if (e.target.id === "contentTeam" && !teamLoaded) {
            getTeamData();
        }
        if (e.target.id === "contentEvents" && !eventsLoaded) {
            getEventsData();
        }
    }
}

function switchWarehouses(e) {
    $(e.currentTarget).text(e.target.innerText === "Ver Almacenes" ? "Ocultar Almacenes" : "Ver Almacenes");
    var division = $("#divisions .active input").val().charAt(0);
    loadRegionalResume(division, !gridGrouped);
}

function changeDivision(e) {
    var division = $(e.target).find("input").val().charAt(0), regional = $("#subsidiaries .active input").val();
    loadRegionalResume(division, gridGrouped);
    loadTopClients(division, regional);
}

function changeSubsidiary(e) {
    var division = $("#divisions .active input").val().charAt(0), regional = $(e.target).find("input").val();
    loadTopClients(division, regional);
}

function showOpenOrders(e, authorized) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), wnd = $("#Detail").getKendoWindow();
    grd.select(row);

    $("#notesBilled").addClass("d-none");
    $("#openOVs").removeClass("d-none");
    $("#stockDetail").addClass("d-none");
    $("#justDelivered").addClass("d-none");

    wnd.title(`OV ${authorized ? 'Autorizadas' : 'Abiertas'}`);
    var divisionSelected = $("#divisions .active input").val();
    var grid = $("#openOVs").data("kendoGrid");
    $.get(urlOpenOrders, { Subsidiary: item.subsidiary, Warehouse: item.warehouse, Division: divisionSelected, Authorized: authorized }, data => {
        data.items.forEach(x => { x.docDate = JSON.toDate(x.docDate); });
        var ds = new kendo.data.DataSource({ data: data.items });
        grid.setDataSource(ds);
        wnd.open();
    });
}

function openStockDetail(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);
    var divisionSelected = $("#divisions .active input").val();
    resumeItem = item;

    $("#notesBilled").addClass("d-none");
    $("#openOVs").addClass("d-none");
    $("#stockDetail").removeClass("d-none");
    $("#justDelivered").addClass("d-none");

    $.get(urlStock, { Subsidiary: item.subsidiary, Division: divisionSelected }, d => {
        var wnd = $("#Detail").getKendoWindow(), grid = $("#stockDetail").data("kendoGrid"), ds = new kendo.data.DataSource({ data: d.items });
        wnd.title("Detalle Stock");
        grid.setDataSource(ds);
        wnd.open();
    });
}

function openReport(e, type) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), wnd = $("#Report").getKendoWindow();
    grd.select(row);

    wnd.title(type === "SO" ? "Orden de Venta" : (item.isDeliveryNote === "Y" ? "Nota de Entrega" : "Nota de Venta"));
    loadReport(type === "SO" ? e.currentTarget.dataset.number : item.noteNumber, item.subsidiary, type === "SO" ? "Order" : (item.isDeliveryNote === "Y" ? "Delivery" : "Note"), 1);
    wnd.open().center();
}

function openItemStock(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);

    var grdMaster = $(e.currentTarget).closest(".k-grid").closest("td").closest(".k-grid").data("kendoGrid"), rowMaster = $(e.currentTarget).closest(".k-grid").closest("tr").prev(".k-master-row"),
        itemMaster = grdMaster.dataItem(rowMaster);

    var lstItemCodes = ["FLETES", "ENVIO", "DMCSERVICIOS"];
    if (lstItemCodes.indexOf(item.itemCode) === -1) {
        $.get(urlGetStock, { Subsidiary: itemMaster.subsidiary, Warehouse: item.warehouse, ItemCode: item.itemCode }, data => {
            if (data.message === "") {
                var grid = $("<div>");
                grid.kendoGrid({
                    columns: [
                        { title: "Sucursal", field: "subsidiary" },
                        { title: "Almacen", field: "warehouse" },
                        { title: "Cod. Item", width: 160, field: "itemCode" },
                        { title: "Stock", attributes: alignRight, headerAttributes: alignRight, width: 95, field: "stock" },
                        { title: "Reservado", attributes: alignRight, headerAttributes: alignRight, width: 95, field: "reserved", template: e => e.reserved > 0 ? `<a class="reserved action action-link">${e.reserved}</a>` : e.reserved },
                        { title: "Disponible", attributes: alignRight, headerAttributes: alignRight, width: 95, field: "available2" },
                        { title: "Pedido", attributes: alignRight, headerAttributes: alignRight, width: 95, field: "requested" }
                    ],
                    sortable: true, selectable: "Single, Row",
                    dataSource: data.items
                });
                var content = $("<div>");
                content.append(grid);
                content.append($("<div>").addClass("items-reserved mt-3"));
                var wnd = $("#Detail2").data("kendoWindow");
                wnd.setOptions({ width: 1050, title: "Detalle Stock" });
                wnd.content(content);
                wnd.open();
            } else {
                showError(`Se ha producido el siguiente error al traer los datos: ${data.message}.`);
            }
        });
    }
}

function openDetails(e, type) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), template = type === "comment" ? "#templateComments" : "#templateAttach",
        title = type === "comment" ? "Comentarios" : "Archivos Adjuntos";
    grd.select(row);

    var detailsTemplate = kendo.template($(template).html());
    var wnd = $("<div>").kendoWindow({ width: 650, title: title }).data("kendoWindow");
    wnd.content(detailsTemplate(item));
    wnd.center().open();
}

function showReservedDetail(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var wnd = $("#Detail2").data("kendoWindow");

    var div = $(e.currentTarget).closest(".k-grid").parent().find(".items-reserved");
    var columns = [
        { title: "Ejecutivo", field: "sellerName" },
        { title: "Cliente", field: "clientCode", template: e => `<strong>${e.clientCode}</strong> - ${e.clientName}` },
        { title: "Orden", field: "docNum", width: 90, template: e => `<a class="sale-order action action-link" data-number="${e.docNum}">${e.docNum}</a>` },
        { title: "Fecha", field: "docDate", format: "{0: dd-MM-yyyy}", width: 95, attributes: alignCenter, headerAttributes: alignCenter },
        { title: "Cantidad", field: "quantity", width: 100, attributes: alignRight, headerAttributes: alignRight },
        { title: "Precio", field: "price", format: "{0: #,##0.00}", width: 90, attributes: alignRight, headerAttributes: alignRight }
    ];
    if (item.subsidiary.toLowerCase() === "iquique") {
        columns.push({ title: "Autorizado", field: "authorized", template: e => e.authorized === "Y" ? '<i class="fas fa-check"></i>' : '', width: 90, attributes: alignCenter, headerAttributes: alignCenter });
        columns.push({ title: "Correlativo", field: "correlative", width: 130 });
    }

    $.get(urlReservedItems, { Subsidiary: item.subsidiary, Warehouse: item.warehouse, ItemCode: item.itemCode }, function (data) {
        if (data.message === "") {
            data.items.forEach(x => x.docDate = JSON.toDate(x.docDate));
            var grid = div.data("kendoGrid");
            if (grid) {
                grid.setDataSource(new kendo.data.DataSource({ data: data.items }));
            } else {
                div.kendoGrid({ columns: columns, sortable: true, selectable: "Single, Row", dataSource: data.items });
            }
            setTimeout(() => wnd.center(), 100);
        } else {
            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
        }
    });
}

function openFile(e) {
    var docEntry = $(e.currentTarget).attr("data-file");
    var subsidiary = $(e.currentTarget).attr("data-subsidiary");
    var fileName = $(e.currentTarget).text();
    window.location.href = urlDownloadFile + "?" + $.param({ Subsidiary: subsidiary, DocEntry: docEntry, FileName: fileName });
}

function openEvent(e) {
    e.preventDefault();
    var wnd = $("<div>").kendoWindow({ activate: onRefreshWindow, title: "Detalle del Evento", width: 1100, modal: true, iframe: false, scrollable: true, refresh: onRefreshWindow }).data("kendoWindow");
    wnd.refresh({ url: e.target.href });
    setTimeout(() => { wnd.open().center(); }, 100);
}

function seeDetails(e) {
    $("#resumes .selected").removeClass("selected");
    $(e.currentTarget).closest(".panel").addClass("selected");
    var dataset = e.currentTarget.dataset;
    if (dataset.type === "N") {
        $.get(urlNotes, { Year: dataset.year }, function (data) {
            if (data.message === "") {
                var items = dataset.year >= 2023 ? data.items.filter(x => x.amount > 0 || x.points > 0) : data.items;
                loadGridNotes(items, dataset.year);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor");
            }
        });
    } else if ((dataset.type === "UP")) {
        $.get(urlAwards, {}, function (data) {
            if (data.message === "") {
                loadGridAwards(data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor");
            }
        });
    } else {
        $.get(urlAvailablePoints, {}, function (data) {
            if (data.message === "") {
                loadGridPoints(data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor");
            }
        });
    }
}

function openDeliveryNote(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), wnd = $("#Report").getKendoWindow();
    grd.select(row);

    wnd.title("Nota de Entrega");
    loadReport(item.docNumber, item.subsidiary, "Delivery", 2);
    wnd.open().center();
}

function prev() {
    document.getElementById('years-slider').scrollLeft -= 270;
}

function next() {
    document.getElementById('years-slider').scrollLeft += 270;
}

//#endregion