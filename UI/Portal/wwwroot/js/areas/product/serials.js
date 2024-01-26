//#region Variables Globales

var _minDate, _maxDate;

//#endregion

//#region Eventos

$(function () {
    _maxDate = $("#FilUntil").data("kendoDatePicker").max();
    _minDate = $("#FilSince").data("kendoDatePicker").min();
    setupControls();
    setGridHeight("Listado", 30);
});

$(window).resize(function () {
    setGridHeight("Listado", 30);
});

$('#filter-box').on('hidden.bs.collapse', function () {
    setGridHeight("Listado", 30);
});

$('#filter-box').on('shown.bs.collapse', function () {
    setGridHeight("Listado", 30);
});

//Evento que se dispara cuando se modifica le fecha inicial
function onSinceChange(e) {
    var startDate = this.value();
    $("#FilUntil").data("kendoDatePicker").min(startDate ? startDate : _minDate);
}

//Evento que se dispara cuando se modifica le fecha final
function onUntilChange(e) {
    var endDate = this.value();
    $("#FilSince").data("kendoDatePicker").max(endDate ? endDate : _maxDate);
}

$("#action-clean").click(function () { cleanFilters(); });

$("#action-filter").click(function () { filterData(); });

$("#action-excel").on("click", function (e) {
    ExportExcel();
});

$("#Listado").on("click", ".sale-note", function (e) {
    var wnd = $("#Report").data("kendoWindow");
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    loadReport(item.number, item.subsidiary, "Note");
    wnd.title(`Nota de Venta ${item.number}`);
    wnd.open().center();
});

$("#Listado").on("click", ".delivery-note", function (e) {
    var wnd = $("#Report").data("kendoWindow");
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    loadReport(item.number, item.subsidiary, "Delivery");
    wnd.title(`Nota de Entrega ${item.number}`);
    wnd.open().center();
});

//#endregion

//#region Métodos Locales

function setupControls() {
    var alignCenter = { style: "text-align: center" };
    $("#Listado").kendoGrid({
        groupable: { enabled: false },
        pageable: { enabled: true },
        noRecords: { template: "No existen resultados para el criterio de búsqueda." },
        scrollable: true, sortable: true, selectable: true,
        detailTemplate: kendo.template($("#detailOrder").html()),
        detailInit: onDetailInit,
        columns: [
            { field: "subsidiary", title: "Sucursal" },
            { field: "clientCode", title: "Cod.Cliente", width: 120 },
            { field: "clientName", title: "Cliente" },
            { field: "date", title: "F. Nota", format: "{0: dd/MM/yyyy}", attributes: alignCenter, headerAttributes: alignCenter, width: 130 },
            {
                field: "number", title: "# Nota", width: 120, template: function (x) {
                    var result = x.number;
                    if (x.isOwn) {
                        if (x.docType === 13) {
                            result = `<a class="sale-note action action-link">${x.number}</a>`;
                        }
                        if (x.docType === 15) {
                            result = `<a class="delivery-note action action-link">${x.number}</a>`;
                        }
                    }                    
                    return result;
                }
            },
            { field: "docTypeDesc", title: "Tipo", width: 120 }
        ],
        dataSource: getDataSource([])
    });
}

function getDataSource(items) {
    return new kendo.data.DataSource({ data: items, pageSize: 500 });
}

function onDetailInit(e) {
    var detailRow = e.detailRow;
    var data = e.data;
    detailRow.find(".detailNote").kendoGrid({
        dataSource: { data: data.items },
        scrollable: false,
        sortable: true,
        pageable: false,
        selectable: true,
        columns: [
            { field: "itemCode", title: "Cod.Producto", width: 200 },
            { field: "name", title: "Producto", width: 300 },
            { field: "count", title: "Total", width: 90 },
            { field: "serialsResume", title: "Seriales" }
        ]
    });
}

function clientMapper(options) {
    var items = $("#FilClient").data("kendoDropDownList").dataSource.data();
    var item = Enumerable.From(items).Where(x => x.Code === options.value).FirstOrDefault();
    var index = Enumerable.From(items).IndexOf(item);
    options.success(index);
}

function cleanFilters() {
    $("#FilClient").data("kendoDropDownList").value("");
    $("#FilProduct").val("");
    $("#FilSerial").val("");
    $("#FilSince").data("kendoDatePicker").value("");
    $("#FilUntil").data("kendoDatePicker").value("");
}

function loadGrid(items) {
    $.each(items, function (i, obj) {
        obj.date = JSON.toDate(obj.date);
    });
    var grd = $("#Listado").data("kendoGrid");
    var ds = getDataSource(items);
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
    } else {
        $("#action-excel").addClass("d-none");
    }
    setGridHeight("Listado", 30);
}

function filterData() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        $.get(urlFilter, filtersData.data, function (data) {
            if (data.message !== "") {
                showError(data.message);
                loadGrid([]);
            } else {
                loadGrid(data.items);
            }
        });
    } else {
        showInfo(`Debe seleccionar los siguientes campos: <br />${filtersData.message}`);
    }
}

function getFilters() {
    var clientCode = $("#FilClient").data("kendoDropDownList").value(), product = $("#FilProduct").val(), serial = $("#FilSerial").val(), since = $("#FilSince").data("kendoDatePicker").value(),
        until = $("#FilUntil").data("kendoDatePicker").value(), number = $("#FilDocNumber").val(), message = "";
    if (since) {
        since = kendo.toString(since, "yyyy-MM-dd");
    } else {
        message += "- Fecha Inicial<br />";
    }
    if (until) {
        until = kendo.toString(until, "yyyy-MM-dd");
    } else {
        message += "- Fecha Final<br />";
    }
    if (clientCode === "" && product === "" && serial === "" && number === "") {
        message += "- Al menos un criterio aparte del rango de fechas";
    }
    return { message: message, data: { CardCode: clientCode, ItemCode: product, InitDate: since, FinalDate: until, Serial: serial, DocNumber: number } };
}

function ExportExcel() {
    var filterData = getFilters();
    if (filterData.message === "") {
        window.location.href = urlExcel + "?" + $.param(filterData.data);
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filterData.message}`);
    }
}

function loadReport(Id, Subsidiary, Type) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = Type === "Note" ? "SaleNote.trdp" : "DeliveryNote.trdp";
    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (Type === "Delivery") {
        objParams.SearchType = 2;
    }
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

//#endregion