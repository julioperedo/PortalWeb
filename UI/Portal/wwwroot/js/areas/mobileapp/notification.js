//#region Variables Globales
var _minDate, _maxDate, wndDetail, grdItems, kufiles, ddlType, ddlUsers;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };
//#endregion

//#region Eventos

$(() => {
    setupControls();
    filterData(true);
});

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$("#action-clean").click((e) => cleanFilters());

$("#action-filter").click((e) => filterData(false));

//$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".action-new, .action-edit", onEdit);

$("#Detail").on("click", "#addFile", onAddingFile);

$("#Detail").on("click", ".remove-file", onRemovingFile);

$("#send-msg").click(onSendingMessage);

//#endregion

//#region Metodos Privados

function setupControls() {
    var today = new Date(), since = today; //.addDays(-7);
    var filSince = $("#filter-initial-date").kendoDatePicker({
        value: since, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#filter-final-date").kendoDatePicker({
        value: today, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
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

    kufiles = $("#files").kendoUpload({
        async: { saveUrl: urlSaveFile, removeUrl: urlRemoveFile },
        multiple: false,
        validation: { maxFileSize: 41943040 }
    }).data("kendoUpload");

    ddlType = $("#RecipientsType").kendoDropDownList({
        change: function (e) {
            var value = $(e.sender.element).val();
            $("#divTopics").toggleClass("d-none", value != "T");
            $("#divUsers").toggleClass("d-none", value != "U");
        },
        dataTextField: "Text", dataValueField: "Value", value: "U",
        dataSource: [{ Text: "Todos", Value: "A", Selected: true }, /*{ Text: "Por Tema", Value: "T" },*/ { Text: "Por Usuario", Value: "U" }]
    }).data("kendoDropDownList");

    ddlUsers = $("#messageUsers").kendoMultiSelect({
        dataTextField: "name", dataValueField: "id", placeholder: "Seleccione destinatarios...", filter: "contains",
        dataSource: { transport: { read: { url: urlUsers } } }
    }).data("kendoMultiSelect");

    grdItems = $("#Listado").kendoGrid({
        noRecords: { template: '<div class="w-100 text-center p-3">No hay resultados para el criterio de búsqueda.</div>' },
        columns: [
            { title: "Título", minScreenWidth: 150, field: "title" },
            { title: "Enviado", attributes: alignCenter, headerAttributes: alignCenter, width: 140, field: "date", format: "{0:dd-MM-yyyy HH:mm}" },
            { title: "Destinatarios", width: 250, template: "#=getType(recipientsType)#", field: "recipientsType" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-new" title="Nuevo Mensaje"></i>',
                template: '<i class="fas fa-pen action action-edit" title="Editar Mensaje"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row",
        "dataSource": []
    }).data("kendoGrid");

}

function cleanFilters() {
    var today = new Date();
    $("#filter-initial-date").data("kendoDatePicker").value(today/*.addDays(-7)*/), $("#filter-final-date").data("kendoDatePicker").value(today), $("#filter-name").val("");
}

function getFilters() {
    var message = "", name = $("#filter-name").val(), initialDate = $("#filter-initial-date").data("kendoDatePicker").value(), finalDate = $("#filter-final-date").data("kendoDatePicker").value();
    if (initialDate) initialDate = initialDate.toISOString();
    if (finalDate) finalDate = finalDate.toISOString();
    if (initialDate === "" & finalDate === "" & name === "") {
        message = "Debe escoger algún criterio de búsqueda.";
    }
    return { message: message, data: { Name: name, InitialDate: initialDate, FinalDate: finalDate } };
}

function filterData(pageLoad) {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        $.get(urlFilter, filtersData.data, function (data) {
            loadGrid(data.items);
            if (data.message !== "") {
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
        items.forEach(x => {
            x.date = JSON.toDate(x.date);
        });
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

function getType(type) {
    return type === "U" ? "A usuarios seleccionados" : (type === "T" ? "Por Temas" : "Todos");
}

function onEdit(e) {
    var row, item = { id: 0, title: "", body: "", recipientsType: "U" };
    if (e.currentTarget.classList.contains("action-new")) {
        $("#divUsers").removeClass("d-none");
        $("#divTopics").addClass("d-none");
    } else {
        row = $(e.currentTarget).closest("tr");
        item = grdItems.dataItem(row)
        grdItems.select(row);
    }
    loadDetail(item);
    wndDetail.open();
}

function loadDetail(item) {
    $.get(urlGetRecipients, { MessageId: item.id }, function (d) {
        if (d.message == "") {
            item.recipients = d.items;
            //console.log(item);
            $("#msg-title").val(item.title);
            $("#msg-body").val(item.body);
            ddlType.value(item.recipientsType);
            ddlUsers.value(d.items.map((x) => x.recipient));
            $("#divTopics").toggleClass("d-none", item.recipientsType != "T");
            $("#divUsers").toggleClass("d-none", item.recipientsType != "U");
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer los datos del servidor");
        }
    });
}

function onAddingFile(e) {
    var txt = $("<input>").addClass("k-textbox").addClass("message-file").css("width", "95%").attr("type", "text");
    var div = $("<div>").append(txt);
    div.append(' <button type="button" class="btn btn-danger remove-file" title="Eliminar imagen"><i class="fas fa-trash-alt"></i></button>');
    div.insertBefore("#addFile");
    if ($(".message-file").length == 5) {
        $("#addFile").addClass("d-none");
    }
}

function onRemovingFile(e) {
    $(this).parent().remove();
    if ($("#addFile").hasClass("d-none")) {
        $("#addFile").removeClass("d-none");
    }
}

function onSendingMessage(e) {
    e.preventDefault();
    var title = $("#msg-title").val(), message = $("#msg-body").val(), image = kufiles.getFiles().length > 0 ? kufiles.getFiles()[0] : "", type = ddlType.value(), tos = ddlUsers.value();
    //console.log(image);
    $.post(urlSend, { Title: title, Message: message, ToType: type, Tos: tos, ImageUrl: image.name }, function (data) {
        if (data.sent) {
            grdItems.dataSource.add({ id: data.id, title: title, body: message, date: new Date(), imageUrl: image, recipientsType: type });
            showMessage("Mensaje enviado");
            wndDetail.close();
        } else {
            if (data.message != "") {
                showError("Se ha producido el siguiente error al enviar el mensaje: " + data.message);
            } else {
                showError("No se ha podido enviar el mensaje");
            }
        }
        kufiles.clearAllFiles();
    });
}

//#endregion