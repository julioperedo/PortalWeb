//#region GLOBAL VARIABLES
var _minDate, _maxDate, filSince, filUntil, filClients, filEmail, filReported;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy HH:mm}";
//#endregion

//#region EVENTS

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", gridMargin));

$("#action-filter").click(e => filterData());

$("#action-clean").click(e => cleanFilters());

$("#action-email").click(onSendingEmail);

$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".check-one", onCheckOne);

$("#Listado").on("click", ".check-all", onCheckAll);

$("#search-mail").click(searchEmail);

$("#Email").on("keypress", searchEmail);

$("#save").click(onSave);

//#endregion

//#region METHODS

function setupControls() {
    filSince = $("#FilSince").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    filUntil = $("#FilUntil").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");
    $("#RejectionDate").kendoDateTimePicker({ format: "dd-MM-yyyy HH:mm", componentType: "modern", culture: "es-BO" });

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    filClients = $("#FilClients").kendoDropDownList({ dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", dataSource: { transport: { read: { url: urlClients } } }, virtual: { itemHeight: 26, valueMapper: clientMapper } }).data("kendoDropDownList");
    filReported = $("#FilReported").kendoDropDownList({ dataTextField: "Text", dataValueField: "Value", optionLabel: "Seleccione un Estado ...", value: "N", dataSource: [{ Text: "Ambos", Value: "B" }, { Text: "Enviado", Value: "Y" }, { Text: "No Enviado", Value: "N", "Selected": true }] }).data("kendoDropDownList");
    $("#Reason").kendoDropDownList({ dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un motivo...", filter: "contains", dataSource: { transport: { read: { url: urlReasons } } } });
    filEmail = $("#FilEMail");

    $("#detail").kendoWindow({
        visible: false, title: "Registrar insidencia", scrollable: true, modal: true, width: 600, iframe: false,
        close: function (e) {
            $("#Email").val(""), $("#ClientCode").text(""), $("#ClientName").text(""), $("#Seller").text(""), $("#Reason").data("kendoDropDownList").value(""), $("#RejectionDate").val(""), $("#Comments").val("");
        },
        activate: function (e) {
            var wnd = this;
            setTimeout(function () {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });

    $("#Listado").kendoGrid({
        columns: [
            { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 80, field: "rejectionDate", format: dateFormat },
            { title: "Correo", field: "email", width: 200 },
            { title: "Cliente", width: 230, field: "clientName" },
            { title: "Vendedor", width: 150, field: "seller" },
            { title: "Motivo", width: 120, field: "reasonDesc" },
            { title: "Reportado", width: 70, field: "reported", attributes: alignCenter, headerAttributes: alignCenter, template: e => e.reported ? '<i class="fas fa-check"></i>' : '' },
            {
                title: " ", width: 30, attributes: alignCenter, headerAttributes: alignCenter,
                template: checkTemplate, field: "id", sortable: false,
                headerTemplate: '<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-all" id="chk-all" name="check-all"><label class="custom-control-label" for="chk-all"></label></div>'
            },
            {
                field: "id", title: " ", attributes: alignCenter, width: 40, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                template: '<i class="fas fa-trash-alt action action-link action-delete" title="Eliminar Usuario"></i>'
            }
        ],
        groupable: { enabled: false },
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="text-center w-100 p-2">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([]),
        excel: { fileName: "RMA.xlsx" },
        dataBound: function (e) {
            var grid = e.sender;
            grid.element.find("table").attr("style", "");
        },
    });
    filterData();
}

function clientMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.code === options.value));
    options.success(index);
}

function cleanFilters() {
    filSince.value("");
    filUntil.value("");
    filClients.value("");
    filEmail.val("");
    filReported.value("N");
}

function getDataSource(items) {
    $.each(items, (i, obj) => obj.rejectionDate = JSON.toDate(obj.rejectionDate));
    var ds = new kendo.data.DataSource({
        data: items,
        sort: [{ field: "id", dir: "asc" }],
        schema: { model: { id: "id" } }
    });
    return ds;
}

function getFilters() {
    var message = "", email = filEmail.val(), since = filSince.value(), until = filUntil.value(), client = filClients.value(), reported = filReported.value();

    if ((reported === "B" || reported === "Y") && (since === null && until === null)) {
        message = "- Debe seleccionar al menos una Fecha cuando escoja registros reportados.<br />";
    }
    if (since) since = since.toISOString();
    if (until) until = until.toISOString();

    return {
        message: message,
        data: { Email: email, ClientCode: client, InitialDate: since, FinalDate: until, Reported: reported }
    };
}

function filterData() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        $.get(urlFilter, filtersData.data, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
            } else {
                console.log(data.message);
                showNotification("", "Se produjo un error al traer los datos.", "error");
            }
        });
    } else {
        showInfo(`Los siguientes campos son necesarios: <br /> ${filtersData.message}`);
    }
}

function loadGrid(items) {
    var grd = $("#Listado").data("kendoGrid");
    var ds = getDataSource(items);
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        //$("#action-excel").removeClass("d-none");
    } else {
        //$("#action-excel").addClass("d-none");
    }
    setGridHeight("Listado", gridMargin);
}

function searchEmail(e) {
    e.preventDefault();
    var valid = e.currentTarget.id === "search-mail" || e.keyCode === 13;
    if (valid) {
        var email = $("#Email").val();
        $.get(urlVerifyEmail, { Email: email }, function (d) {
            if (d.message === "") {
                $("#ClientCode").text(d.clientCode);
                $("#ClientName").text(d.clientName);
                $("#Seller").text(d.seller);
            } else {
                console.log(d.message);
                showNotification("", "Se produjo un error al traer los datos.", "error");
            }
        });
    }
}

function checkTemplate(e) {
    var template = "";
    if (e.reported === false) {
        template = `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input check-one" id="chk-one-${e.id}" value="${e.id}"><label class="custom-control-label" for="chk-one-${e.id}"></label></div>`;
    }
    return template;
}

function onSendingEmail(e) {
    var ids = Enumerable.From($(".check-one:checked")).Select("+$.value").ToArray();
    var values = ids.join();

    var message = `<div class="row"><div class="col"><p class="font-weight-bold">¿Está seguro que desea enviar el correo y agregar a la lista negra a los correos selccionados?</p>
                   <label for="extra-comnnets">Comentarios Extra</label><textarea id="extra-comnnets" class="form-control"></textarea></div></div>`;
    showConfirm(message, function (e) {
        var grd = $("#Listado").data("kendoGrid");
        var extraComments = $("#extra-comnnets").val();
        $.post(urlReportEmails, { Ids: values, Comments: extraComments }, function (d) {
            if (d.message === "") {
                ids.forEach((v, i) => grd.dataSource.remove(grd.dataSource.get(v)));
                showNotification("", "Correos Marcados exitosamente", "success");
            } else {
                console.error(d.message);
                showNotification("", "Se produjo un error al Reportar los Correos.", "error");
            }
        });
    });
}

function onNew(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    grd.clearSelection();
    $("#detail").data("kendoWindow").open();
}

function onCheckOne(e) {
    $("#action-email").toggleClass("d-none", $(".check-one:checked").length === 0);
    $("#check-all").prop("checked", $(".check-one:checked").length === $(".check-one").length);
}

function onCheckAll(e) {
    if ($(".check-one:not(:disabled)").length > 0) {
        $(".check-one:not(:disabled)").prop("checked", e.target.checked);
        $("#action-email").toggleClass("d-none", !e.target.checked);
    } else {
        e.target.checked = false;
    }
}

function onSave(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var email = $("#Email").val(), clientCode = $("#ClientCode").text(), clientName = $("#ClientName").text(), date = $("#RejectionDate").data("kendoDateTimePicker").value(), ddlReason = $("#Reason").data("kendoDropDownList"),
            reason = ddlReason.value(), reasonDesc = ddlReason.text(), seller = $("#Seller").text(), comments = $("#Comments").val(),
            item = { email: email, clientCode: clientCode, clientName: clientName, rejectionDate: date.toISOString(), reason: reason, seller: seller, comments: comments, reported: false };
        $.post(urlEdit, { Item: item }, function (d) {
            if (d.message === "") {
                var grd = $("#Listado").data("kendoGrid");
                grd.dataSource.add({ id: d.id, email: email, clientCode: clientCode, clientName: clientName, rejectionDate: date, reason: reason, seller: seller, comments: comments, reported: false, reasonDesc: reasonDesc });
                $("#detail").data("kendoWindow").close();
                showNotification("", "Registro guardado exitosamente.", "success");
            } else {
                console.log(d.message);
                showNotification("", "Hubo un error al guardar el registros.", "error");
            }
        });
    }
}

//#endregion