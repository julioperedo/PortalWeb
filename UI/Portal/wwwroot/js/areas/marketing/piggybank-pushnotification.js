//#region Variables Globales
var _minDate, _maxDate, wndDetail, grdItems, kufiles, ddlType, ddlUsers;
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };
//#endregion

//#region Eventos

$(() => {
    setupControls();
    filterData(true);
});

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$("#action-clean").click((e) => { cleanFilters(); });

$("#action-filter").click((e) => { filterData(false); });

$("#Listado").on("click", ".action-new, .action-edit", onEdit);

$("#send-msg").click(onSendingMessage);

//#endregion

//#region Metodos Privados

function setupControls() {
    var today = new Date(), since = today; //.addDays(-7);
    var filSince = $("#filter-initial-date").kendoDatePicker({
        value: since, change: function (e) {
            var startDate = this.value();
            if (startDate == null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#filter-final-date").kendoDatePicker({
        value: today, change: function (e) {
            var endDate = this.value();
            if (endDate == null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    wndDetail = $("#Detail").kendoWindow({
        visible: false, width: 750, title: "Mensaje", modal: true, close: function (e) {
            $("#msg-title").val("");
            $("#msg-body").val("");
        }
    }).data("kendoWindow");

    grdItems = $("#Listado").kendoGrid({
        noRecords: { template: '<div class="w-100 text-center p-3">No hay resultados para el criterio de búsqueda.</div>' },
        columns: [
            { title: "Título", minScreenWidth: 150, field: "title" },
            { title: "Enviado", attributes: alignCenter, headerAttributes: alignCenter, width: 140, field: "date", format: "{0:dd-MM-yyyy HH:mm}" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo Mensaje"></i>',
                template: '<i class="fas fa-pen action action-link action-edit" title="Editar Mensaje"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row",
        "dataSource": []
    }).data("kendoGrid");

    kufiles = $("#files").kendoUpload({
        async: { saveUrl: urlSaveFile, removeUrl: urlRemoveFile },
        multiple: false,
        validation: { maxFileSize: 41943040 },
        success: function (e) {
            //var filesData = $("#filesData"), data = JSON.parse(filesData.val());
            //if (e.operation === "upload") {
            //    e.files.forEach(x => data.push({ id: _docId--, fileURL: x.name, statusType: 1 }));
            //} else {
            //    e.files.forEach(x => data = data.filter(i => i.fileURL !== x.name || i.statusType !== 1));
            //}
            //filesData.val(JSON.stringify(data));
        }
    }).data("kendoUpload");

    ddlType = $("#msg-type").kendoDropDownList({
        change: function (e) {
            var value = $(e.sender.element).val();
            $("#divTopics").toggleClass("d-none", value != "T");
            $("#divUsers").toggleClass("d-none", value != "U");
        },
        dataTextField: "Text", dataValueField: "Value", value: "A",
        dataSource: [{ Text: "Todos", Value: "A", Selected: true }, /*{ Text: "Por Tema", Value: "T" },*/ { Text: "Por Usuario", Value: "U" }]
    }).data("kendoDropDownList");

    ddlUsers = $("#msg-users").kendoMultiSelect({
        dataTextField: "fullName", dataValueField: "id", placeholder: "Seleccione destinatarios...", filter: "contains",
        dataSource: { transport: { read: { url: urlUsers } } }
    }).data("kendoMultiSelect");
}

function cleanFilters() {
    var today = new Date();
    $("#filter-initial-date").data("kendoDatePicker").value(today), $("#filter-final-date").data("kendoDatePicker").value(today), $("#filter-name").val("");
}

function getFilters() {
    var message = "", name = $("#filter-name").val(), initialDate = $("#filter-initial-date").data("kendoDatePicker").value(), finalDate = $("#filter-final-date").data("kendoDatePicker").value();
    if (initialDate) initialDate = initialDate.toISOString();
    if (finalDate) finalDate = finalDate.toISOString();
    if (initialDate == "" & finalDate == "" & name == "") {
        message = "Debe escoger algún criterio de búsqueda.";
    }
    return { message: message, data: { Name: name, InitialDate: initialDate, FinalDate: finalDate } };
}

function filterData(pageLoad) {
    var filtersData = getFilters();
    if (filtersData.message == "") {
        $.get(urlFilter, filtersData.data, function (data) {
            loadGrid(data.items);
            if (data.message != "") {
                showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
            }
        });
    } else {
        if (!pageLoad) {
            setGridHeight("Listado", _gridMargin);
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
}

function loadGrid(items) {
    if (items) {
        var margin = _gridMargin;
        items.forEach(x => x.date = JSON.toDate(x.date));
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({ data: items });
        grd.setDataSource(ds);
        if (items && items.length > 0) {
            $('#filter-box').collapse("hide");
            margin -= 20;
        }
        setTimeout(() => { setGridHeight("Listado", margin) }, 200);
    }
}

function onSendingMessage(e) {
    var title = $("#msg-title").val(), message = $("#msg-body").val(), image = kufiles.getFiles().length > 0 ? kufiles.getFiles()[0] : "", type = ddlType.value(), tos = ddlUsers.value();
    $.post(urlSendMessage, { Title: title, Message: message, ImageUrl: image.name, ToType: type, Tos: tos }, function (data) {
        if (data.sent) {
            cleanFilters();
            grdItems.dataSource.add({ id: data.id, title: title, body: message, date: new Date() });

            showMessage("Mensaje enviado");
            wndDetail.close();
        } else {
            if (data.message != "") {

            } else {
                showError("Se ha producido el siguiente error al enviar el mensaje: " + data.message);
            }
        }
        kufiles.clearAllFiles();
    });
}

function onEdit(e) {
    var row, item;
    if (e.currentTarget.classList.contains("action-edit")) {
        row = $(e.currentTarget).closest("tr");
        grdItems.select(row);
        item = grdItems.dataItem(row);
    } else {
        item = { title: "", body: "" };
    }
    $("#msg-title").val(item.title);
    $("#msg-body").val(item.body);
    wndDetail.open();
}

//#endregion