//#region GLOBAL DECLARATIONS
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, _gridMargin = -20;
//#endregion

//#region EVENTS

$(function () {
    setupControls();
});

$(window).resize(function () {
    setGridHeightLocal();
});

$("#filter").click(() => {
    var isClient = $("#isClient").val(), clientCode = isClient === "N" ? $("#clients").data("kendoDropDownList").value() : $("#clients").val(), initialDate = $("#initialDate").data("kendoDatePicker").value(),
        finalDate = $("#finalDate").data("kendoDatePicker").value(), message = "";
    if (clientCode === "") {
        message += "- Cliente <br />";
    }
    if (!initialDate) {
        message += "- Fecha Inicial <br />";
    }
    if (!finalDate) {
        message += "- Fecha Final <br />";
    }
    if (initialDate && finalDate) {
        if (initialDate > finalDate) {
            message += "- La Fecha Inicial debe ser menor a la Fecha Final <br />";
        } else {
            initialDate = initialDate.toISOString();
            finalDate = finalDate.toISOString();
        }
    }

    if (message === "") {
        loadData(clientCode, initialDate, finalDate);
    } else {
        showInfo(`Se requieren los siguientes campos: <br /> ${message}`);
    }
});

$("#action-excel").click(() => {
    var isClient = $("#isClient").val(), clientCode = isClient === "N" ? $("#clients").data("kendoDropDownList").value() : $("#clients").val(), initialDate = $("#initialDate").data("kendoDatePicker").value(),
        finalDate = $("#finalDate").data("kendoDatePicker").value(), message = "";
    if (clientCode === "") {
        message += "- Cliente <br />";
    }
    if (!initialDate) {
        message += "- Fecha Inicial <br />";
    }
    if (!finalDate) {
        message += "- Fecha Final <br />";
    }
    if (initialDate && finalDate) {
        if (initialDate > finalDate) {
            message += "- La Fecha Inicial debe ser menor a la Fecha Final <br />";
        } else {
            initialDate = initialDate.toISOString();
            finalDate = finalDate.toISOString();
        }
    }

    if (message === "") {
        window.location.href = urlExport + "?" + $.param({ ClientCode: clientCode, InitialDate: initialDate, FinalDate: finalDate });
    } else {
        showInfo(`Se requieren los siguientes campos: <br /> ${message}`);
    }
});

$("#Listado").on("click", ".sale-note", function () {
    var wnd = $("#report").data("kendoWindow");
    var grid = $("#Listado").data("kendoGrid"), row = $(this).closest("tr"), item = grid.dataItem(row);
    grid.select(row);

    var user = $.trim($(".user-info > .user-name").first().text());
    var report = { report: "SaleNote.trdp", parameters: { Subsidiary: item.subsidiary, DocNumber: item.docNum, User: user } };

    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource(report);
            viewer.refreshReport();
        } catch (e) {
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        $("#reportViewer1").telerik_ReportViewer({
            serviceUrl: urlService,
            reportSource: report,
            viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
            scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
        });
    }

    wnd.open().center();
});

$("#Listado").on("click", ".payment", function () {
    var wnd = $("#detail").data("kendoWindow");
    var grid = $("#Listado").data("kendoGrid"), row = $(this).closest("tr"), item = grid.dataItem(row);
    grid.select(row);

    $.get(urlPayment, { Subsidiary: item.subsidiary, DocNum: item.docNum }, function (data) {
        if (data.message === "") {
            var detailsTemplate = kendo.template($("#template").html());
            data.item.docDate = JSON.toDate(data.item.docDate);
            data.item.notes.forEach((x) => { x.fecha = JSON.toDate(x.fecha); });
            wnd.content(detailsTemplate(data.item));
            wnd.center().open();
        } else {
            showError(`Se ha producido el siguiente error: <br />${data.message}.`);
        }
    });
});

$("#Listado").on("click", ".credit-note", function () {
    var wnd = $("#report").data("kendoWindow");
    var grid = $("#Listado").data("kendoGrid"), row = $(this).closest("tr"), item = grid.dataItem(row);
    grid.select(row);

    var user = $.trim($(".user-info > .user-name").first().text());
    var report = { report: item.typeId === "AC" ? "CreditNote.trdp" : "CreditNoteItem.trdp", parameters: { Subsidiary: item.subsidiary, DocNumber: item.docNum, User: user } };

    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource(report);
            viewer.refreshReport();
        } catch (e) {
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        $("#reportViewer1").telerik_ReportViewer({
            serviceUrl: urlService,
            reportSource: report,
            viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
            scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
        });
    }

    wnd.open().center();
});

//#endregion

//#region METHODS

function setupControls() {
    var isClient = $("#isClient").val();
    if (isClient === "N") {
        $("#clients").kendoDropDownList({
            dataSource: { transport: { read: { url: urlClients } } }, dataTextField: "name", dataValueField: "code",
            optionLabel: "Seleccione un cliente", filter: "contains",
            virtual: {
                itemHeight: 26,
                valueMapper: function (options) {
                    var items = this.dataSource.data();
                    var index = items.indexOf(items.find(i => i.code === options.value));
                    options.success(index);
                }
            }
        });
    }

    var today = new Date(), since = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate());
    $("#initialDate").kendoDatePicker({ value: since });

    $("#finalDate").kendoDatePicker({ value: today });

    $("#Listado").kendoGrid({
        dataSource: { data: [] },
        sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
        columns: [
            { field: "subsidiary", title: "Sucursal", width: 120 },
            { field: "type", title: "Tipo", width: 150 },
            { field: "docNum", title: "No. Doc.", template: templateDocNum, width: 90 },
            { field: "docDate", title: "Fecha", format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter, width: 90 },
            { field: "docTotal", title: "Monto", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, width: 100 },
            { field: "terms", title: "Referencia" },
            { field: "sellerName", title: "Ejecutivo Ventas" }
        ]
    });

    setGridHeightLocal();

    $("#report").removeClass("hidden");
    $("#report").kendoWindow({ visible: false, width: 1100, title: "Nota de Venta", modal: true });
    $("#detail").kendoWindow({ visible: false, width: 650, title: "Recibo de Pago", modal: true });
}

function loadData(clientCode, initialDate, finalDate) {
    var dateFormat = "dd-MM-yyyy";
    $.get(urlFilter, { ClientCode: clientCode, InitialDate: initialDate, FinalDate: finalDate }, (data) => {
        if (data.message === "") {
            loadGrid(data.items);
            var periodBefore = $("#periodBefore"), periodSelected = $("#periodSelected"), periodNow = $("#periodNow");
            periodBefore.empty();
            var total = 0;
            if (data.before) {
                var row = $("<tr>").addClass("level-1");
                //row.append($("<td>").text("Período"));
                row.append($("<td>").attr("colspan", 2).addClass("text-right").addClass("text-nowrap").text(`antes de ${kendo.toString(JSON.toDate(initialDate), dateFormat)}`));
                periodBefore.append(row);

                data.before.forEach((x) => {
                    row = $("<tr>");
                    row.append($("<td>").text(x.type));
                    row.append($("<td>").addClass("text-right").text(`${kendo.toString(x.total, "N2")}`));
                    periodBefore.append(row);
                    total += x.total;
                });
                //row = $("<tr>").addClass("level-2");
                //row.append($("<td>").text("TOTAL PERIODO"));
                //row.append($("<td>").addClass("text-right").text(`${kendo.toString(total, "N2")}`));
                //periodBefore.append(row);
            }

            periodSelected.empty();
            if (data.selected) {
                var row = $("<tr>").addClass("level-1");
                //row.append($("<td>").text("Período"));
                row.append($("<td>").attr("colspan", 2).addClass("text-right").addClass("text-nowrap").text(`${kendo.toString(JSON.toDate(initialDate), dateFormat)} al ${kendo.toString(JSON.toDate(finalDate), dateFormat)}`));
                periodSelected.append(row);
                var totalPeriod = 0;
                data.selected.forEach((x) => {
                    row = $("<tr>");
                    row.append($("<td>").text(x.type).addClass("text-nowrap"));
                    row.append($("<td>").addClass("text-right").text(`${kendo.toString(x.total, "N2")}`));
                    periodSelected.append(row);
                    totalPeriod += x.total;
                });
                total += totalPeriod;
                row = $("<tr>").addClass("level-2");
                row.append($("<td>").text("TOTAL PERIODO"));
                row.append($("<td>").addClass("text-right").text(`${kendo.toString(totalPeriod, "N2")}`));
                periodSelected.append(row);

                //row = $("<tr>").addClass("level-3");
                //row.append($("<td>").text("BALANCE"));
                //row.append($("<td>").addClass("text-right").text(`${kendo.toString(total, "N2")}`));
                //periodSelected.append(row);
            }

            periodNow.empty();
            var row = $("<tr>").addClass("level-4");
            row.append($("<td>").text("BALANCE"));
            row.append($("<td>").addClass("text-right").text(`${kendo.toString(data.balance, "N2")}`));
            periodNow.append(row);
            showExports(true);
        } else {
            showExports(false);
            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
        }
    });
}

function cleanDetail() {
    loadGrid([]);
    showExports(false);
}

function loadGrid(items) {
    items.forEach((x) => { x.docDate = JSON.toDate(x.docDate); });
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

function setGridHeightLocal() {
    var navHeight = $(".navbar").height(), toolHeight = $(".toolbar").height(), windowHeight = $(window).height(), footerHeight = $("footer").height(), gridHeight = $("#Listado").height(),
        gridContentHeight = $("#Listado").find(".k-grid-content").height(), margins = 26;
    var newHeight = windowHeight - navHeight - footerHeight - toolHeight - margins - gridHeight + gridContentHeight;
    $("#Listado").find(".k-grid-content").height(newHeight);
}

function showExports(visible) {
    $("#label-exports, #action-excel").toggleClass("hidden", !visible);
}

function templateDocNum(e) {
    var localUser = $("#isClient").val() === "N";
    var template;
    if (e.typeId === "NV") {
        template = `<a class="sale-note action action-link">${e.docNum}</a>`;
    } else if (e.typeId === "PE") {
        template = `<a class="payment action action-link">${e.docNum}</a>`;
    } else if ((e.typeId === "AC" || e.typeId === "D") & localUser) {
        template = `<a class="credit-note action action-link">${e.docNum}</a>`;
    } else {
        template = e.docNum;
    }
    return template;
}

//#endregion