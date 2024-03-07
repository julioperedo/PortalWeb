//#region Variables Globales
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", _gridMargin = 15, localUser = $("#LocalUser").val() === "Y";
var _minDate, _maxDate, receiptTemplate = `<div class="row">
        <div class="col-5">Recibo : <strong>#= data.docNumber #</strong></div>
        <div class="col-5">Fecha : #= kendo.toString(data.docDate, "dd/MM/yyyy") #</div>
    </div>
    <div class="row">
        <div class="col-5">Estado Recibo : #= data.state #</div>
    </div>
    <div class="row">
        <div class="col-5">Total Recibo : <strong>#= kendo.toString(data.totalReceipt, "N2") #</strong></div>
    </div>
    <div class="row">
        <div class="col-5">Pago a Cuenta : #= kendo.toString(data.onAccount, "N2") #</div>
        <div class="col-5">Total Sin Aplicar : #= kendo.toString(data.notAppliedTotal, "N2") #</div>
    </div>
    <br />
    <table class="table table-hover">
        <thead>
            <tr><td><strong>Nota de Venta</strong></td><td class="text-center"><strong>Fecha</strong></td><td class="text-center"><strong>T&eacute;rmino</strong></td><td class="text-right"><strong>Monto Pagado</strong></td><td class="text-right"><strong>Total Nota</strong></td><td class="text-right"><strong>D&iacute;as Mora</strong></td></tr>
        </thead>
        # var decTotal = 0, decPayed = 0; #
        # for (var i = 0; i < data.notes.length; i++) { #
        # decTotal += data.notes[i].total; #
        # decPayed += data.notes[i].amountPaid; #
        <tr><td># if (data.notes[i].noteNumber > 0) { # #= data.notes[i].noteNumber # # } #</td><td class="text-center"># if (data.notes[i].noteNumber > 0) { # #= kendo.toString(data.notes[i].docDate, "dd/MM/yyyy") # # } #</td><td class="text-center"># if (data.notes[i].noteNumber > 0) { # #= data.notes[i].terms # # } #</td><td class="text-right">#= kendo.toString(data.notes[i].amountPaid, "N2") #</td><td class="text-right">#= kendo.toString(data.notes[i].total, "N2") #</td><td class="text-right">#= kendo.toString(data.notes[i].days, "N0") #</td></tr>
        # } #
        <tfoot>
            <tr><td><strong>TOTAL</strong></td><td></td><td></td><td class="text-right"><strong>#= kendo.toString(decPayed, "N2") #</strong></td><td class="text-right"><strong>#= kendo.toString(decTotal, "N2") #</strong></td><td></td></tr>
        </tfoot>
    </table>`, adjustTemplate = `<div class="row">
        <div class="col-5">No. Nota de Cr&eacute;dito : <strong>#= data.docNumber #</strong></div>
        <div class="col-5">Fecha : #= kendo.toString(data.docDate, "dd/MM/yyyy") #</div>
    </div>	
    <div class="row">
        <div class="col-5">Referencia : #= data.comments #</div>
    </div>
    <div class="row">
        <div class="col-5">Total : <strong>#= kendo.toString(data.totalReceipt, "N2") #</strong></div>
    </div>
    <br />
    <table class="table table-hover">
        <thead>
            <tr><td><strong>Descripci&oacute;n</strong></td><td><strong>C&oacute;d. Cuenta</strong></td><td><strong>Cuenta de Mayor</strong></td><td class="text-right"><strong>Total</strong></td></tr>
        </thead>
        # var decTotal = 0; #
        # for (var i = 0; i < data.items.length; i++) { #
        # decTotal += data.items[i].total; #
        <tr><td>#= data.items[i].description #</td><td>#= data.items[i].accountCode #</td><td>#= data.items[i].accountName #</td><td class="text-right">#= kendo.toString(data.items[i].total, "N2") #</td></tr>
        # } #
        <tfoot>
            <tr><td><strong>TOTAL</strong></td><td></td><td></td><td class="text-right"><strong>#= kendo.toString(decTotal, "N2") #</strong></td></tr>
        </tfoot>
    </table>`;
//#endregion

//#region Eventos

$(function () {
    _maxDate = $("#final-date").data("kendoDatePicker").max();
    _minDate = $("#initial-date").data("kendoDatePicker").min();
    var startDate = $("#initial-date").data("kendoDatePicker").value(), endDate = $("#final-date").data("kendoDatePicker").value();
    $("input.final-date").data("kendoDatePicker").min(startDate);
    $("input.initial-date").data("kendoDatePicker").max(endDate);

    setupControls();
    if (!localUser) { filterData("O", true); } else { setGridHeight("Listado", _gridMargin); }
});

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(function () { cleanFilters(); });

$("#action-filter").click(function () { filterData(false); });

$("#Listado").on("click", ".receipt", openReceipt);

$("#Listado").on("click", ".note", openSaleNote);

$("#Listado").on("click", ".adjust", openAdjustDetail);

$("#action-excel").on("click", ExportExcel);

//#endregion

//#region Metodos Privados

function setupControls() {
    if (localUser) {
        $("#client").kendoDropDownList({
            dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", virtual: { itemHeight: 26, valueMapper: clientMapper },
            dataSource: { transport: { read: { url: urlClients } } }
        });
    }
    $("#Listado").kendoGrid({
        columns: [
            { title: "Cliente", hidden: true, field: "clientName", groupHeaderTemplate: "Cliente: #=value#    ( Total: #= count#,  Total Recibos: #= kendo.toString(aggregates.totalReceipt.sum, 'N2') #, Días Mora ( Promedio: #= kendo.toString(aggregates.totalDueDays.sum / ( aggregates.totalBilled.sum > 0 ? aggregates.totalBilled.sum : 1 ), 'N0') # , M&aacute;ximo: #= aggregates.maxDueDays.max # )" },
            { title: "Sucursal", hidden: true, field: "subsidiary", groupHeaderTemplate: "Sucursal: #= value #    ( Total: #= count#,  Total Recibos: #= kendo.toString(aggregates.totalReceipt.sum, 'N2') #, Días Mora ( Promedio: #= kendo.toString(aggregates.totalDueDays.sum / ( aggregates.totalBilled.sum > 0 ? aggregates.totalBilled.sum : 1 ), 'N0') # , M&aacute;ximo: #= aggregates.maxDueDays.max # )" },
            { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 120, field: "docDate", format: "{0:dd/MM/yyyy}" },
            {
                title: "Recibo", attributes: alignCenter, headerAttributes: alignCenter, width: 120, field: "docNumber",
                template: e => !e.adjust ? `<a class='receipt action action-link'>${e.docNumber}</a>` : (e.state === "Ajustes" ? `<a class='adjust action action-link'>${e.docNumber}</a>` : e.docNumber)
            },
            { title: "Estado Recibo", attributes: alignCenter, headerAttributes: alignCenter, width: 120, field: "state" },
            { title: "# Notas de Venta (días mora)", attributes: alignCenter, headerAttributes: alignCenter, width: 300, field: "noteNumbers", template: x => x.notes.map(i => `<a class="note action action-link">${i.noteNumber}</a> (${i.days})`).join(", ") },
            { title: "Pago a Cuenta", attributes: alignRight, headerAttributes: alignRight, width: 130, field: "onAccount", format: "{0:N2}" },
            { title: "Total Recibo", attributes: alignRight, headerAttributes: alignRight, width: 120, field: "totalReceipt", format: "{0:N2}", aggregates: ["sum"] },
            { title: "Total Sin Aplicar", attributes: alignRight, headerAttributes: alignRight, width: 140, field: "notAppliedTotal", format: "{0:N2}" },
            { title: "Referencia", width: 200, field: "comments" },
            { title: "Ajuste", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(adjust) {# <i class="fas fa-check"></i> #} #', field: "adjust" }
        ],
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="w-100 p-2 text-center">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        dataSource: getDataSource([])
    });
}

function clientMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.Code === options.value));
    options.success(index);
}

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

function cleanFilters() {
    if (localUser) $("#client").data("kendoDropDownList").value("");
    $(`#initial-date`).data("kendoDatePicker").value(initialDate), $(`#final-date`).data("kendoDatePicker").value(finalDate), $("#receipt").val(""), $("#note").val("");
}

function getFilters() {
    var message = "", clientCode = $(`#client`).val(), initialDate = $(`#initial-date`).data("kendoDatePicker").value(), finalDate = $(`#final-date`).data("kendoDatePicker").value(),
        receipt = $(`#receipt`).val(), note = $(`#note`).val();

    if (initialDate) {
        initialDate = initialDate.toISOString();
    }
    if (finalDate) {
        finalDate = finalDate.toISOString();
    }
    if (!initialDate && !finalDate && clientCode === "" && (receipt === "" || receipt === 0) && (note === "" || note === 0)) {
        message = "Debe ingresar al menos un criterio de búsqueda";
    }

    return { message: message, data: { ClientCode: clientCode, InitialDate: initialDate, FinalDate: finalDate, ReceiptCode: receipt, NoteCode: note } };
}

function filterData(isPageLoad) {
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
        if (!isPageLoad) {
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
}

function loadGrid(items) {
    if (items) {
        var grd = $("#Listado").data("kendoGrid");
        var ds = getDataSource(items);
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

function getDataSource(items) {
    items.forEach(x => {
        x.docDate = JSON.toDate(x.docDate);
        x.notes.forEach(y => y.docDate = JSON.toDate(y.docDate));
        //x.maxDueDays = x.notes.length > 0 ? Enumerable.From(x.notes).Max("$.days") : 0;
    });
    console.log('items', items.filter((x) => x.clientCode == 'CALP-004'));
    var ds = new kendo.data.DataSource({
        data: items,
        aggregate: [
            { aggregate: "sum", field: "totalReceipt" },
            { aggregate: "count", field: "totalDueDays" }
        ],
        group: [
            { field: "clientName", dir: "asc", aggregates: [{ field: "clientName", aggregate: "count" }, { field: "totalReceipt", aggregate: "sum" }, { field: "totalDueDays", aggregate: "sum" }, { field: "maxDueDays", aggregate: "max" }, { field: "totalBilled", aggregate: "sum" }] },
            { field: "subsidiary", dir: "asc", aggregates: [{ field: "subsidiary", aggregate: "count" }, { field: "totalReceipt", aggregate: "sum" }, { field: "totalDueDays", aggregate: "sum" }, { field: "maxDueDays", aggregate: "max" }, { field: "totalBilled", aggregate: "sum" }] }
        ],
        sort: [{ field: "docDate", dir: "desc" }, { field: "docNumber", dir: "desc" }]
    });
    return ds;
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

function ExportExcel(e) {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        window.location.href = urlExcel + "?" + $.param(filtersData.data);
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filtersData.message}`);
    }
}

function openSaleNote(e) {
    var wnd = $("#Report").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr"),
        item = grd.dataItem(row), noteNumber = $(this).text(), report;
    grd.select(row);
    if (item.isDelivery === "Y" && item.subsidiary.toLowerCase() === "santa cruz") {
        report = "Delivery";
        wnd.title("Nota de Entrega");
    } else {
        report = "Note";
        wnd.title(`Nota de Venta ${noteNumber}`);
    }
    loadReport(noteNumber, item.subsidiary, report);
    wnd.center().open();
}

function openReceipt(e) {
    var wnd = $("#Detail").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr"),
        dataItem = grd.dataItem(row), detailsTemplate = kendo.template(receiptTemplate);
    grd.select(row);
    wnd.title("Detalle del Recibo");
    wnd.content(detailsTemplate(dataItem));
    wnd.center().open();
}

function openAdjustDetail(e) {
    var wnd = $("#Detail").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr"),
        item = grd.dataItem(row), detailsTemplate = kendo.template(adjustTemplate);
    grd.select(row);
    $.get(urlGetAdjustmentItems, { Subsidiary: item.subsidiary, DocNum: item.docNumber }, function (d) {
        item.items = d.items;
        wnd.title("Detalle del Ajuste");
        wnd.content(detailsTemplate(item));
        wnd.center().open();
    });
}

//#endregion