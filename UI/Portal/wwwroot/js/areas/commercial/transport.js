//#region Global Variables

var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;

//#endregion

//#region Events

$(function () {
    setupControls();
    filterData();
});

$(window).resize(() => { setGridHeight("Listado", _gridMargin); });

$("#action-filter").click((e) => {
    filterData();
});

$("#action-clean").click((e) => {
    cleanFilters();
    loadGrid([]);
});

$("#action-email").click((e) => {
    var values = Enumerable.From($(".check-one:checked")).Select("+$.value").ToArray().join();

    var wnd = $("#Detail").data("kendoWindow");
    wnd.title("Enviar despacho por Correo");
    wnd.setOptions({ width: 1300 });
    wnd.refresh({ url: urlOpenPreview, data: { Ids: values } });
    wnd.open();
});

$("#Listado").on("click", ".new-item", (e) => {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: 0 } });
    wnd.open();
});

$("#Listado").on("click", ".action-edit", (e) => {
    var row = $(e.target).closest("tr");
    var grid = $("#Listado").data("kendoGrid");
    var dataItem = grid.dataItem(row);
    grid.select(row);
    var wnd = $("#Detail").data("kendoWindow");
    wnd.title("Detalle Despacho");
    wnd.setOptions({ width: 800 });
    wnd.refresh({ url: urlEdit, data: { Id: dataItem.id } });
    wnd.open();
});

$("#Listado").on("click", ".action-delete", (e) => {
    e.preventDefault();
    var grid = $("#Listado").data("kendoGrid");
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);
    grid.select(row);
    showConfirm(`¿Está seguro que desea eliminar el Despacho con guía <b>${dataItem.docNumber}</b>?`, () => {
        $.post(urlDelete, { Id: dataItem.id }, (data) => {
            if (data.message === "") {
                grid.removeRow(row);
                showMessage(`La Guía <b>${dataItem.docNumber}</b> fue eliminada correctamente`);
            } else {
                showError(`Se ha producido el siguiente error al intentar eliminar la Guía <b>${dataItem.docNumber}</b>.<br /> ${data.message}`);
            }
        });
    });
});

$("#Listado").on("click", ".check-one", (e) => {
    $("#action-email").toggleClass("d-none", $(".check-one:checked").length === 0);
    $("#check-all").prop("checked", $(".check-one:checked").length === $(".check-one").length);
});

$("#Listado").on("click", ".check-all", (e) => {
    if ($(".check-one:not(:disabled)").length > 0) {
        $(".check-one:not(:disabled)").prop("checked", e.target.checked);
        $("#action-email").toggleClass("d-none", !e.target.checked);
    } else {
        e.target.checked = false;
    }
});

$("#Detail").on("click", "[name='transport-type']", function (e) {
    var typeId = +this.value;
    setTransportType(typeId, "");
});

$("#Detail").on("click", ".action-cancel", (e) => {
    e.preventDefault();
    $("#Detail").data("kendoWindow").close();
});

$("#Detail").on("click", ".action-save", (e) => {
    e.preventDefault();
    var ms = $("#DetailValue").magicSuggest();
    var typeId = +$("[name='transport-type']:checked").val();

    var lstItems;
    if (typeId !== 149) {
        //var table = $(".handsontable").handsontable('getInstance');
        //var lstData = table.getData();
        //lstItems = Enumerable.From(lstData).Where("$[1] !== null").Select("{ Name: $[0], EMail: $[1] }").Distinct().ToArray();
        lstItems = [];
        for (var i = 1; i <= 5; i++) {
            var name = $.trim($("#name" + i).val()), email = $.trim($("#email" + i).val());
            if (name !== "" && email !== "") {
                lstItems.push({ Name: $("#name" + i).val(), EMail: $("#email" + i).val() });
            }
        }
    } else {
        lstItems = [];
    }
    var objConfig = {
        rules: {
            orders: (i) => (i.is("#DetailValue") && typeId === 149) ? ms.isValid() : true,
            names: (i) => (i.is("#StringValues") && typeId !== 149) ? lstItems.length > 0 & $(".handsontable").find(".htInvalid").length === 0 : true
        },
        messages: { required: "*", names: "*", email: "No es un E-Mail válido", orders: "*" }
    };

    var form = $(e.target).closest("form");
    var validator = form.kendoValidator(objConfig).data("kendoValidator");
    if (validator.validate()) {
        var transport = form.serializeObject();
        if (typeId === 149) {
            transport.Values = ms.getValue();
        } else {
            transport.Values = Enumerable.From(lstItems).Select("JSON.stringify($)").ToArray();
        }
        transport.TypeId = typeId;
        transport.Transporter = { Name: $("#TransporterId").data("kendoDropDownList").text() };
        transport.Source = { Name: $("#SourceId").data("kendoDropDownList").text() };
        transport.Destination = { Name: $("#DestinationId").data("kendoDropDownList").text() };
        var filters = getFilters();
        $.post(urlEdit, { Item: transport, Filters: filters }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                $("#Detail").data("kendoWindow").close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(`Se ha producido el siguiente error al intentar guardar el despacho: <br /> ${data.message}`);
            }
        });
    }
});

$("#Detail").on("click", ".action-send", (e) => {
    e.preventDefault();
    var ids = Enumerable.From($(".transport-id")).Select("+$.value").ToArray().join();
    var filters = getFilters();
    showConfirm(`¿Está seguro que desea enviar por correo las Guías seleccionadas?`, () => {
        $.post(urlSendMail, { Ids: ids, Filters: filters }, (data) => {
            if (data.message === "") {
                $("#Detail").data("kendoWindow").close();
                loadGrid(data.items);
                $("#check-all").prop("checked", false);
                showMessage("Se enviaron los correos correctamente.");
            } else {
                showError(`Se ha producido el siguiente error al intentar enviar los despachos por correo: <br />${data.message}`);
            }
        });
    });
});

//#endregion

//#region Methods

function setupControls() {
    var filSince = $("#FilSince").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#FilUntil").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    $("#FilTransporter").kendoDropDownList({ dataSource: { transport: { read: { url: urlTransporters } } }, dataTextField: "name", filter: "contains", dataValueField: "id", optionLabel: "Seleccione un Transportista ..." });
    $("#FilSent").kendoDropDownList({ dataTextField: "Text", dataValueField: "Value", optionLabel: "Seleccione un Estado ...", value: "N", dataSource: [{ Text: "Ambos", Value: "B" }, { Text: "Enviado", Value: "Y" }, { Text: "No Enviado", Value: "N", "Selected": true }] });
    $("#FilSource").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Origen ...", filter: "contains", dataSource: { transport: { read: { url: urlSources } } } });
    $("#FilDestiny").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Destino ...", filter: "contains", dataSource: { transport: { read: { url: urlDestines } } } });

    $("#Listado").kendoGrid({
        columns: [
            { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "date", format: dateFormat },
            { title: "Tipo", width: 100, field: "type.name" },
            { title: "Transporte", attributes: { style: "min-width: 120px;" }, field: "transporter.name" },
            { title: "No. de Guía", width: 110, field: "docNumber" },
            { title: "Origen", width: 120, field: "source.name" },
            { title: "Destino", width: 120, field: "destination.name" },
            { title: "Destinatario", field: "deliveryTo" },
            { title: "Pago Entrega", attributes: alignRight, headerAttributes: alignRight, width: 120, field: "remainingAmount", format: "{0:N2}" },
            {
                title: " ", width: 60,
                template: checkTemplate, field: "id", sortable: false,
                headerTemplate: '<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-all" id="chk-all" name="check-all"><label class="custom-control-label" for="chk-all"></label></div>'
            },
            {
                field: "id", title: " ", attributes: alignCenter, width: 60, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action new-item" title="Nuevo Despacho"></i>',
                template: '<i class="fas fa-pen action action-edit" title="Editar Despacho"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-delete" title="Eliminar Despacho"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de búsqueda.</div>' },
        toolbar: ["excel"],
        excel: { fileName: "Despachos.xlsx" },
        dataSource: []
    });

    $("#Detail").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "Detalle despacho", resizable: false, width: 1000, actions: ["Close"], close: onCloseWindow, refresh: (e) => {
            $("#Date").kendoDatePicker({ format: "d/M/yyyy" });
            $("#SourceId").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Origen ...", filter: "contains", dataSource: { transport: { read: { url: urlSources } } } });
            $("#DestinationId").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Destino ...", filter: "contains", dataSource: { transport: { read: { url: urlDestines } } } });
            $("#TransporterId").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Transportista ...", filter: "contains", dataSource: { transport: { read: { url: urlTransporters } } } });

            var typeId = +$("#TypeId").val();
            var value = $("#StringValues").val();
            setTransportType(typeId, value);

            onRefreshWindow(e);
        }
    });
}

function cleanFilters() {
    var today = new Date();
    $("#FilSince").data("kendoDatePicker").value(today);
    $("#FilUntil").data("kendoDatePicker").value(today);
    $("#FilTransporter").data("kendoDropDownList").value("");
    $("#FilSource").data("kendoDropDownList").value("");
    $("#FilDestiny").data("kendoDropDownList").value("");
    $("#FilName").val("");
}

function loadGrid(items) {
    $.each(items, function (i, obj) { obj.date = JSON.toDate(obj.date); });
    var grd = $("#Listado").data("kendoGrid");
    var ds = new kendo.data.DataSource({ data: items });
    grd.setDataSource(ds);

    var margin = _gridMargin;
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        margin -= 110;
    }

    setGridHeight("Listado", margin);
}

function getFilters() {
    var since = $("#FilSince").data("kendoDatePicker").value(), until = $("#FilUntil").data("kendoDatePicker").value(), transporteId = $("#FilTransporter").data("kendoDropDownList").value(),
        sourceId = $("#FilSource").data("kendoDropDownList").value(), destinationId = $("#FilDestiny").data("kendoDropDownList").value(), sent = $("#FilSent").data("kendoDropDownList").value(), filter = $("#FilName").val();
    if (since) {
        since = since.toISOString();
    }
    if (until) {
        until = until.toISOString();
    }
    return { InitialDate: since, FinalDate: until, TransporterId: transporteId, SourceId: sourceId, DestinationId: destinationId, Filter: filter, Sent: sent };
}

function filterData() {
    var filters = getFilters();
    $.get(urlFilter, { Filters: JSON.stringify(filters) }, function (data) {
        if (data.message === "") {
            loadGrid(data.items);
            $(".toolbar .filters").toggleClass("hidden");
            $("#check-all").prop("checked", false);
        } else {
            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
        }
    });
}

function setTransportType(typeId, value) {
    $(".data-container").addClass("d-none");
    var values;
    if (typeId === 149) {
        $("#transport-type-1").prop("checked", true);
        $("#DetailValue").closest(".data-container").removeClass("d-none");
        $("label[for='DetailValue']").text("Ordenes:");
        var ms = $("#DetailValue").magicSuggest({ hideTrigger: true, placeholder: "Ingrese las órdenes", maxDropHeight: 0, maxSelection: null, minChars: 100, minCharsRenderer: () => "", vregex: /^\d*$/ });
        if (value && value !== "") {
            values = value.split(",");
            ms.setValue(values);
        }
        $("#WithCopies, label[for='WithCopies']").removeClass("d-none");
    } else {
        if (typeId === 151) {
            $("#transport-type-3").prop("checked", true);
        } else {
            $("#transport-type-2").prop("checked", true);
        }
        $("label[for='DetailValue']").text("Destinatarios:");
        $("#contacts-content").closest(".data-container").removeClass("d-none");
        var items = [];
        if (value && value !== "") {
            items = JSON.parse(value);
        }
        var quantity = 5 - items.length;
        for (var i = 0; i < quantity; i++) {
            items.push({ Name: "", EMail: "" });
        }
        renderGrid(items);
        $("#WithCopies, label[for='WithCopies']").addClass("d-none");
        $("#WithCopies").prop("checked", false);
    }
    $("#Detail").data("kendoWindow").center();
}

function renderGrid(Items) {
    var content = $("#contacts-content");
    //var $div = $("<div>");
    //content.html($div);

    //var emailValidator = (value, callback) => $.trim(value) == "" ? callback(true) : setTimeout(() => callback(/^\w+([\.-]?\w+)+@\w+([\.:]?\w+)+(\.[a-zA-Z0-9]{2,})+$/.test(value)), 100);
    //setTimeout(() => {
    //    $div.handsontable({
    //        data: Items,
    //        columns: [
    //            { data: "Name", width: 200 },
    //            { data: "EMail", validator: emailValidator, width: 300 }
    //        ],
    //        rowHeaders: false,
    //        colHeaders: ["Nombre", "E-Mail"],
    //        columnSorting: true,
    //        height: 150,
    //        licenseKey: "non-commercial-and-evaluation"
    //    });
    //}, 200);

    for (var i = 1; i <= Items.length; i++) {
        var item = Items[i - 1];
        $("#name" + i).val(item.Name);
        $("#email" + i).val(item.EMail);
    }
}

function checkTemplate(e) {
    var template = "";
    if (e.listTransportDetails && e.listTransportDetails.length > 0) {
        //template = `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-one" id="chk-one-${e.id}" value="${e.id}" ${e.validEmail ? `` : `disabled="disabled"`}><label class="custom-control-label" for="chk-one-${e.id}"></label></div>`;
        template = `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-one" id="chk-one-${e.id}" value="${e.id}"><label class="custom-control-label" for="chk-one-${e.id}"></label></div>`;
    }
    return template;
}

//#endregion