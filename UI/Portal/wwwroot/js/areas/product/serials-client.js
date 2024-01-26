//#region Variables Globales
var pageLoad = true;
//#endregion

//#region Eventos

$(function () { setupControls(); });

$("#filter").click(onFiltering);

$("#serial").on("keydown", onFiltering2);

$("#Listado").on("click", ".sale-note, .delivery-note", openNote);

//#endregion

//#region Métodos Locales

function setupControls() {
    $("#Listado").kendoListView({ template: kendo.template($("#temp-item").html()), dataBound: function (e) { $(".no-items").toggleClass("d-none", this.dataSource.data().length > 0 || pageLoad); } });
    pageLoad = false;

    $("#Report").kendoWindow({ visible: false, width: 1100, modal: true });
}

function onFiltering(e) {
    var serial = $.trim($("#serial").val()), cardCode = $("#card-code").val();
    if (serial !== "") {
        $.get(urlFilter, { CardCode: cardCode, Serial: serial }, function (d) {
            if ($.trim(d.message) !== "") {
                console.error(d.message);
                showError("Se ha producido un error al traer los datos del servidor");
            } else {
                d.items.forEach(v => v.date = JSON.toDate(v.date));
                var lv = $("#Listado").getKendoListView(), ds = new kendo.data.DataSource({ data: d.items });
                lv.setDataSource(ds);


            }
        });
    }
}

function onFiltering2(e) {
    if (e.keyCode === 13) onFiltering(e);
}

function openNote(e) {
    e.preventDefault();
    var row = $(e.currentTarget).closest(".doc-detail"), item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row), wnd = $("#Report").data("kendoWindow");

    loadReport(item.number, item.subsidiary, item.docType === 13 ? "Note" : "Delivery");
    wnd.title(`${item.docType === 13 ? "Nota de Venta" : "Nota de Entrega"} ${item.number}`);
    wnd.open().center();
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