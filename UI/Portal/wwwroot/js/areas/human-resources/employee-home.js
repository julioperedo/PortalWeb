//#region Variables Globales
var _newId = 0, _minDate, _maxDate, today, tommorow, startDate, endDate, startDateReplacement, endDateReplacement, startPeriod = "AM", endPeriod = "PM",
    _dpDate, _tpInitial, _tpFinal, notificator, _wndDetail, _ddlType, _ddlLicenseReason, _dpSince, _dpUntil, _ddlSincePeriod, _ddlUntilPeriod,
    _grdYears, _grdDaysTaken, _lvRequests, _grdReplacements, _ddlReplace, _dpFromDateReplace, _dpToDateReplace, _ddlFromDatePeriodReplace,
    _ddlToDatePeriodReplace;
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { "class": "text-right", style: "justify-content: right" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}",
    _gridMargin = 30, perActivities = $("#permission").val(),
    requestTemplate = `<div class="request #if(idState == 225){#open#}##if(idState == 221){#sent#}##if(idState == 222){#cancelled#}##if(idState == 223){#rejected#}##if(idState === 224){#approved#}#">
    <div class="row">
        <div class="col">            
            <label>Per&iacute;odo:</label> <span class="from-date">#=kendo.toString(fromDate, "dd-MM-yyyy")# #=fromDatePeriod#</span> - <span class="to-date">#=kendo.toString(toDate, "dd-MM-yyyy")# #=toDatePeriod#</span>
            &nbsp;&nbsp;&nbsp;( #=days# d&iacute;a#if(days>1) {#s#} # )
        </div>
        <div class="col text-right">
            <span class="state">#=stateName#</span>#if($.trim(rejectComments) != "" && idState === 223) {# <br /><span>#=rejectComments#</span> #}#
        </div>
    </div>
    <div class="row">
        <div class="col">
            <label>Remplazo:</label> <span class="replacement">#=replacementName#</span>
        </div>
    </div>
    <div class="row">
        <div class="col">
            <label>Jefe:</label> <span class="manager">#=managerName#</span>
        </div>
    </div>
    <div class="row">
        <div class="col text-right">
            <button type="button" class="btn btn-outline-primary action-edit" title="Editar Solicitud">Editar</button> 
            <button type="button" class="btn btn-outline-warning action-open" title="Abrir Solicitud para editar">Abrir</button> 
            <button type="button" class="btn btn-outline-danger action-cancel" title="Anular Solicitud">Anular</button>
            <button type="button" class="btn btn-outline-success action-send" title="Enviar Solicitud para aprobación">Enviar</button>
        </div>
    </div>
    <div class="row">
        <div class="col text-right">
            creado: <span class="request-date">#=kendo.toString(requestDate, "dd-MM-yyyy HH:mm")#</span>
            &uacute;ltima actualizaci&oacute;n: #=kendo.toString(logDate, "dd-MM-yyyy HH:mm")#
        </div>
    </div>    
</div>`,
    replaceForm = `<form id="form-replacement" class="mt-2">
                       <input type="hidden" id="idDetailReplacement" />
                       <input id="idReplacement" name="idReplacement" class="w-100" required />
                       <div class="form-group mt-2">
                           <label for="fromDate-replacement" class="mr-1">Desde</label>
                           <input id="fromDate-replacement" name="fromDate-replacement" type="date" style="width: 130px" required />
                           <select id="fromDatePeriod-replacement" name="fromDatePeriod-replacement" style="width: 70px">
                               <option>AM</option>
                               <option>PM</option>
                           </select>
                       </div>
                       <div class="form-group">
                           <label for="toDate-replacement" class="mr-1">Hasta</label>
                           <input id="toDate-replacement" name="toDate-replacement" type="date" style="width: 130px" required />
                           <select id="toDatePeriod-replacement" name="toDatePeriod-replacement" style="width: 70px">
                               <option>AM</option>
                               <option selected="selected">PM</option>
                           </select>
                       </div>
                       <div class="text-right">
                           <button type="button" class="btn btn-outline-secondary cancel-replacement">Cancelar</button>
                           <button type="button" class="btn btn-outline-primary save-replacement">Aceptar</button>
                       </div>
                   </form>`,
    _states = [
        { id: 225, name: 'open', desc: 'Abierta' },
        { id: 221, name: 'sent', desc: 'Enviada' },
        { id: 222, name: 'cancelled', desc: 'Cancelada' },
        { id: 223, name: 'rejected', desc: 'Rechazada' },
        { id: 224, name: 'approved', desc: 'Aprobada' }
    ];
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(setCustomHeight);

$(".total-days").click(showTotalDays);

$(".days-taken, .rejected-days").click(showDaysTaken);

$(".see-requests").click(showRequests);

$("body").on("click", ".add-request, .detail-request", openRequestForm);

$("#cancel-request").click(cancelRequest);

$("#save-request").click(saveRequest);

$("#detail-requests").on("click", ".action-open, .action-cancel, .action-send", onButtonPressed);

$("#detail-requests").on("click", ".action-edit", onEdit);

$('#delete-request').click(deleteRequest);

$('#Send').on('change', onSendChange);

$("#replacements").on("click", ".action-new, .action-edit, .action-delete", onEditReplacement);

$("#replacement-detail").on("click", ".cancel-replacement", (e) => showHidePanelReplacement(false));

$("#replacement-detail").on("click", ".save-replacement", saveReplacement);

//#endregion

//#region Metodos Locales

function setupControls() {
    const types = [{ id: "L", name: "Licencia" }, { id: "V", name: "Vacación" }, { id: "H", name: "Home Office" }, { id: "T", name: "Viaje de Trabajo" }];
    _ddlType = $("#idType").kendoDropDownList({
        dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Tipo...", dataSource: types,
        change: (e) => setRequestForm(e.sender.value())
    }).data("kendoDropDownList");
    _ddlLicenseReason = $("#idReason").kendoDropDownList({
        dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Motivo...", filter: "contains",
        dataSource: { transport: { read: { url: urlLicenseReason } } },
        change: (e) => $("#reasonDescription").toggleClass("d-none", e.sender.value() != -1)
    }).data("kendoDropDownList");
    _wndDetail = $("#form-detail-request").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "Detalle de Solicitud", resizable: false, width: 720, actions: ["Close"],
        activate: (e) => setTimeout(() => onRefreshWindow(e), 50)

    }).data("kendoWindow");

    $.get(urlEmployee, {}, function (d) {
        if (d.message === "") {
            if (d.item) {
                $("title").text(`Datos de RRHH de ${d.item.name} - Portal DMC`);
                $("#employee-data").removeClass("d-none");
                $("#employee-no-data").addClass("d-none");
                $("#name").text(d.item.name);
                $("#start-date").text(kendo.toString(JSON.toDate(d.item.startDate), "dd-MM-yyyy"));
                $("#years").text(d.item.years);
                $("#total-days").text(d.item.vacationDays + (d.item.extraVacationDays > 0 ? ` + ${d.item.extraVacationDays}` : ''));
                $("#days-taken").text(d.item.daysTaken);
                $("#remaining-days").text(`${(d.item.vacationDays - d.item.daysTaken)} + ${d.item.extraVacationDays}`);
                $("#rejected-days").text(d.item.rejectedDays);
            }
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer los datos del servidor.");
        }
    });

    today = new Date();
    today.setHours(0, 0, 0, 0);
    tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    startDate = new Date(today);
    endDate = new Date(tomorrow);

    _dpSince = $("#fromDate").kendoDatePicker({
        format: "dd/MM/yyyy",
        change: function () {
            startDate = this.value();
            if (startDate === null) this.value("");
            calculateDays();
        }
    }).data("kendoDatePicker");
    _dpUntil = $("#toDate").kendoDatePicker({
        format: "dd/MM/yyyy",
        change: function () {
            endDate = this.value();
            if (endDate === null) this.value("");
            calculateDays();
        }
    }).data("kendoDatePicker");

    _ddlSincePeriod = $("#fromDatePeriod").kendoDropDownList({
        change: function (e) {
            startPeriod = this.value();
            calculateDays();
        }
    }).data("kendoDropDownList");
    _ddlUntilPeriod = $("#toDatePeriod").kendoDropDownList({
        change: function (e) {
            endPeriod = this.value();
            calculateDays();
        }
    }).data("kendoDropDownList");

    _dpDate = $("#date").kendoDatePicker().data("kendoDatePicker");
    _tpInitial = $("#initialTime").kendoTimePicker({ componentType: "modern" }).data("kendoTimePicker");
    _tpFinal = $("#finalTime").kendoTimePicker({ componentType: "modern" }).data("kendoTimePicker");

    _grdReplacements = $("#replacements").kendoGrid({
        sortable: true, selectable: "Single, Row", dataSource: [], noRecords: { template: '<div class="w-100 p-2">No hay items para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Remplazo", field: "replacementName" },
            { title: "Desde", field: "fromDate", width: 120, attributes: alignCenter, headerAttributes: alignCenter, template: (e) => `${kendo.toString(e.fromDate, "dd-MM-yyyy")} ${e.fromDatePeriod}` },
            { title: "Hasta", field: "toDate", width: 120, attributes: alignCenter, headerAttributes: alignCenter, template: (e) => `${kendo.toString(e.toDate, "dd-MM-yyyy")} ${e.toDatePeriod}` },
            {
                title: " ", field: "id", width: 70, attributes: alignCenter, headerAttributes: alignCenter, sortable: false,
                headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo Remplazo"></i>',
                template: '<a class="action action-link action-edit" title="Editar Remplazo"><i class="fas fa-pen"></i></a><a title="Eliminar Remplazo" class="action action-link action-delete"><i class="fas fa-trash-alt"></i></a>'
            }
        ]
    }).data("kendoGrid");

    _grdYears = $("#resume-years .grid").kendoGrid({
        columns: [
            { title: "#", field: "number", width: 80 },
            { title: "A&ntilde;o", field: "year", width: 120, attributes: alignCenter, headerAttributes: alignCenter },
            { title: "D&iacute;as", field: "days", width: 120, attributes: alignRight, aggregates: ["sum"], headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e?.days?.sum ?? 0 },
        ],
        sortable: true, selectable: "Single, Row", dataSource: []
    }).data("kendoGrid");
    _grdDaysTaken = $("#resume-days-taken .grid").kendoGrid({
        columns: [
            { title: "Cod.", field: "id", width: 70 },
            { title: "F.Solicitud", field: "docDate", width: 90, format: "{0: dd-MM-yyyy HH:mm}", attributes: alignCenter, headerAttributes: alignCenter },
            { title: "Desde", field: "since", width: 90, format: "{0: dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter },
            { title: "Hasta", field: "until", width: 90, format: "{0: dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter },
            { title: "D&iacute;as", field: "days", format: "{0: 0.0}", width: 90, aggregates: ["sum"], attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e?.days?.sum ?? 0 },
            { title: "# A&ntilde;o", field: "year", width: 70, attributes: alignRight, headerAttributes: alignRight },
        ],
        noRecords: { template: '<div class="w-100 p-4">No hay items para el criterio de b&uacute;squeda.</div>' },
        sortable: true, selectable: "Single, Row", dataSource: []
    }).data("kendoGrid");
    _lvRequests = $("#detail-requests .grid").kendoListView({
        dataSource: {
            transport: { read: { url: urlGetRequests, data: { OnlyPending: true } } },
            schema: {
                model: { id: "id", fields: { "requestDate": { "type": "date" }, "logDate": { "type": "date" }/*, "fromDate": { "type": "date", from: "vacation.fromDate" }*/ } }
            }
        },
        template: function (e) {
            var content, type = "", period = "", replacement = "", management = "", reason = "", comments = "";
            type = e.idType === "L" ? "Licencia" : e.idType === "V" ? "Vacaci&oacute;n" : e.idType === "H" ? "Home Office" : "Viaje de Trabajo";
            if ($.trim(e.comments) !== "") {
                comments += `<div class="row"><div class="col"><label>Comentarios:</label> <span class="label-data">${e.comments}</span></div></div>`;
            }
            if (e.managers.length > 0) {
                management += '<div class="row">';
                e.managers.forEach((x) => management += `<div class="col"><label>Jefe:</label> <span class="label-data">${x.name}</span></div>`);
                management += '</div>';
            }
            if (e.idType === "V") {
                e.vacation.fromDate = JSON.toDate(e.vacation.fromDate);
                e.vacation.toDate = JSON.toDate(e.vacation.toDate);
                e.vacation.replacements.forEach(function (r) {
                    r.fromDate = JSON.toDate(r.fromDate);
                    r.toDate = JSON.toDate(r.toDate);
                });
                period += `<span class="label-data">${kendo.toString(e.vacation.fromDate, "dd-MM-yyyy")} ${e.vacation.fromDatePeriod}</span> al <span class="label-data">${kendo.toString(e.vacation.toDate, "dd-MM-yyyy")} ${e.vacation.toDatePeriod}</span>&nbsp;&nbsp;&nbsp;( <span class="label-data">${e.vacation.days} d&iacute;a${e.vacation.days > 1 ? "s" : ""}</span> )`;
                if (e.vacation.replacements.length > 0) {
                    replacement += '<div class="row">';
                    e.vacation.replacements.forEach((r) => replacement += `<div class="col"><label>Remplazo:</label> <span class="label-data">${r.replacementName}</span></div>`);
                    replacement += '</div>';
                }
            }
            if (e.idType === "H") {
                e.homeOffice.fromDate = JSON.toDate(e.homeOffice.fromDate);
                e.homeOffice.toDate = JSON.toDate(e.homeOffice.toDate);
                period += `<span class="label-data">${kendo.toString(e.homeOffice.fromDate, "dd-MM-yyyy")} ${e.homeOffice.fromDatePeriod}</span> al <span class="label-data">${kendo.toString(e.homeOffice.toDate, "dd-MM-yyyy")} ${e.homeOffice.toDatePeriod}</span>`;
            }
            if (e.idType === "T") {
                e.travel.fromDate = JSON.toDate(e.travel.fromDate);
                e.travel.toDate = JSON.toDate(e.travel.toDate);
                type += `&nbsp;&nbsp;&nbsp;( ${e.travel.destiny} )`;
                period += `<span class="label-data">${kendo.toString(e.travel.fromDate, "dd-MM-yyyy")}</span> al <span class="label-data">${kendo.toString(e.travel.toDate, "dd-MM-yyyy")}</span>`;
            }
            if (e.idType === "L") {
                e.license.date = JSON.toDate(e.license.date);
                e.license.initialTime = JSON.toTime(e.license.initialTime);
                e.license.finalTime = JSON.toTime(e.license.finalTime);
                period += `<span class="label-data">${kendo.toString(e.license.date, "dd-MM-yyyy")} ( ${kendo.toString(e.license.initialTime, "HH:mm")} al ${kendo.toString(e.license.finalTime, "HH:mm")} )</span>`;
                var reasonDesc = $.trim(e.license.idReason == -1 ? e.license.reasonDescription : e.license.reasonName);
                reason += reasonDesc !== "" ? `<div class="row"><div class="col"><label>Motivo:</label><span class="label-data">${reasonDesc}</span></div></div>` : "";
            }
            content = `<div class="request ${(e.idState == 225 ? "open" : e.idState == 221 ? "sent" : e.idState == 222 ? "cancelled" : e.idState == 223 ? "rejected" : "approved")}">
    <div class="row">
        <div class="col"><label>Tipo:</label> <span class="label-data">${type}</span></div>
    </div>
    <div class="row">
        <div class="col"><label>Per&iacute;odo:</label> ${period}</div>
        <div class="col text-right">
            <span class="state">${e.stateName}</span>${($.trim(e.rejectComments) != "" && e.idState == 223 ? `<br /><span>${e.rejectComments}</span>` : "")}
        </div>
    </div>
    ${reason}
    ${replacement}
    ${management}
    ${comments}
    <div class="row">
        <div class="col text-right">
            <button type="button" class="btn btn-outline-primary action-edit" title="Editar Solicitud">Editar</button> 
            <button type="button" class="btn btn-outline-warning action-open" title="Abrir Solicitud para editar">Abrir</button> 
            <button type="button" class="btn btn-outline-danger action-cancel" title="Anular Solicitud">Anular</button>
            <button type="button" class="btn btn-outline-success action-send" title="Enviar Solicitud para aprobación">Enviar</button>
        </div>
    </div>
    <div class="row">
        <div class="col text-right pt-2">
            <span>creado:</span> <span class="request-date">${kendo.toString(e.requestDate, "dd-MM-yyyy HH:mm")}</span>
            <span class="ml-4">&uacute;ltima actualizaci&oacute;n:</span> <span class="request-date">${kendo.toString(e.logDate, "dd-MM-yyyy HH:mm")}</span>
        </div>
    </div>
</div>`;
            return content;
        },
        dataBound: function (e) {
            if (this.dataSource.data().length == 0) {
                this.content.append('<div class="py-3 w-100 text-center">No hay items para el criterio de b&uacute;squeda</div>');
            }
        },
    }).data("kendoListView");

    $.get(urlHolidays, {}, function (d) {
        if (d.message === "") {
            d.items.forEach(function (x) {
                x.since = JSON.toDate(x.since);
                x.until = JSON.toDate(x.until);
            });
            $("#holidays").val(JSON.stringify(d.items));
        }
    });

    setCustomHeight();

    let template1 = '<div><i class="fas fa-check"></i><i class="fas fa-times"></i><h5>#= title #</h5><p>#= message #</p></div>',
        template2 = '<div><i class="fas fa-times-circle"></i><i class="fas fa-times"></i><h5>#= title #</h5><p>#= message #</p></div>';
    notificator = $("#custom-message").kendoNotification({
        button: true,
        autoHideAfter: 0,
        position: { bottom: 20, right: 20 },
        show: function (e) {
            let element = e.element.parent(), pos = element.position();
            element.css({ top: pos.top - 5 });
        },
        templates: [
            { type: 'success', template: template1 },
            { type: 'error', template: template2 },
            { type: 'open', template: template1 },
            { type: 'cancelled', template: template1 },
            { type: 'sent', template: template1 }
        ]
    }).data("kendoNotification");
}

function setRequestForm(type) {
    $("#form-detail-request .row, .selected-days, .k-picker").removeClass("d-none");
    $("#form-detail-request input, #form-detail-request select").prop("required", false);
    $("#idType").prop("required", true);
    if (type === "L") {
        $("#fromDate, #destiny, #replacements").closest(".row").addClass("d-none");
        $("#date, #initialTime, #finalTime, #idReason").prop("required", true);
    }
    if (type === "V") {
        $("#initialTime, #idReason, #destiny").closest(".row").addClass("d-none");
    }
    if (type === "H") {
        $(".selected-days").addClass("d-none");
        $("#initialTime, #idReason, #destiny, #replacements").closest(".row").addClass("d-none");
    }
    if (type === "T") {
        $(".selected-days").addClass("d-none");
        $("#fromDatePeriod, #toDatePeriod").closest(".k-picker").addClass("d-none");
        $("#initialTime, #idReason").closest(".row").addClass("d-none");
    }
    calculateDays();
}

function showTotalDays(e) {
    e.preventDefault();
    hideDetails();
    $.get(urlResumeYears, {}, function (d) {
        $("#resume-years").removeClass("d-none");
        var grd = $("#resume-years .grid").data("kendoGrid"), 
            ds = new kendo.data.DataSource({
                data: d.items,
                aggregate: [{ aggregate: "sum", field: "days" }],
                sort: [{ field: "number", dir: "asc" }],            
                schema: { model: { id: "number" } }
            });
        grd.setDataSource(ds);
    });
}

function showDaysTaken(e) {
    e.preventDefault();
    hideDetails();
    var accepted = e.currentTarget.classList.contains('days-taken');
    $.get(urlResumeDaysTaken, { Accepted: accepted }, function (d) {
        if (d.message === "") {
            $("#resume-days-taken").removeClass("d-none");
            $("#resume-days-taken label").html(`Detalle de d&iacute;as de vacaciones ${(accepted ? "tomadas" : "rechazadas")}`);
            var grd = $("#resume-days-taken .grid").data("kendoGrid");
            d.items.forEach((x) => {
                x.docDate = JSON.toDate(x.docDate);
                x.since = JSON.toDate(x.since);
                x.until = JSON.toDate(x.until);
            });
            var ds = new kendo.data.DataSource({
                data: d.items,
                aggregate: [{ aggregate: "sum", field: "days" }],
                sort: [{ field: "id", dir: "asc" }],
                schema: { model: { id: "id" } }
            });
            grd.setDataSource(ds);
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer los datos del servidor.");
        }
    });
}

function hideDetails() {
    $("#resume-years, #resume-days-taken, #detail-requests").addClass("d-none");
}

function setCustomHeight() {
    var margins = _gridMargin;
    var isFirefox = typeof InstallTrigger !== 'undefined';
    if (isFirefox) margins += 15;
    $(".grid-details .grid").each((i, x) => {
        var content = $(x).find(".k-grid-content");
        content.css("max-height", $(window).height() - 325 - margins);
    });
}

function showRequests(e) {
    e.preventDefault();
    hideDetails();
    $.get(urlGetRequests, {}, (d) => showRequestsGrid(d));
}

function showRequestsGrid(items) {
    items.forEach((x) => {
        x.requestDate = JSON.toDate(x.requestDate);
        x.logDate = JSON.toDate(x.logDate);
    });
    $("#detail-requests").removeClass("d-none");
    _lvRequests.dataSource.data(items);
}

function openRequestForm(e) {
    var isNew = e.currentTarget.classList.contains("add-request"), item;
    if (isNew) {
        var newDate = new Date(), newDate2 = (new Date()).addHours(2);
        item = {
            id: 0, idEmployee: 0, requestDate: newDate, idState: _states.find(x => x.name == 'open').id, stateName: 'Abierta', idType: 'L',
            comments: '', rejectComments: '', externalCode: 'NO sincronizado',
            license: { id: 0, date: newDate, initialTime: newDate, finalTime: newDate2, idReason: 0, reasonDescription: '' }
        };
    } else {
        var grd = $("#detail-requests .grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr");
        grd.select(row);
        item = grd.dataItem(row);
    }
    loadRequestForm(item);
}

function loadRequestForm(item) {
    //console.log('Item:', item);
    setRequestForm(item.idType);

    $("#id").val(item.id);
    $("#idEmployee").val(item.idEmployee);
    $("#externalCode").val(item.externalCode);
    $("#externalCode-label").text($.trim(item.externalCode) != "" ? item.externalCode : "NO sincronizado");
    $("#idState").val(item.idState);
    _ddlType.value(item.idType);
    $("#state-label").text(item.stateName);
    $("#requestDate").val(JSON.fromDate(item.requestDate));
    $("#comments").val(item.comments);
    if (item.idType == "L") {
        $("#idDetail").val(item.license.id);
        _dpDate.value(item.license.date);
        _tpInitial.value(item.license.initialTime);
        _tpFinal.value(item.license.finalTime);
        _ddlLicenseReason.value(item.license.idReason);
        $("#reasonDescription").text(item.license.reasonDescription);
        $("#reasonDescription").toggleClass("d-none", item.license.idReason != -1);
        _grdReplacements.setDataSource(getReplacementDS([]));
    }
    if (item.idType == "V") {
        $("#idDetail").val(item.vacation.id);
        _dpSince.value(item.vacation.fromDate);
        _dpUntil.value(item.vacation.toDate);
        _ddlSincePeriod.value(item.vacation.fromDatePeriod);
        _ddlUntilPeriod.value(item.vacation.toDatePeriod);
        calculateDays();
        //$("#days").val(item.days);
        $(".selected-days span").text(item.days);
        let items = _.cloneDeep(item.vacation.replacements);
        _grdReplacements.setDataSource(getReplacementDS(items));
    }
    if (item.idType == "H") {
        $("#idDetail").val(item.homeOffice.id);
        _dpSince.value(item.homeOffice.fromDate);
        _dpUntil.value(item.homeOffice.toDate);
        _ddlSincePeriod.value(item.homeOffice.fromDatePeriod);
        _ddlUntilPeriod.value(item.homeOffice.toDatePeriod);
        _grdReplacements.setDataSource(getReplacementDS([]));
    }
    if (item.idType == "T") {
        $("#idDetail").val(item.travel.id);
        _dpSince.value(item.travel.fromDate);
        _dpUntil.value(item.travel.toDate);
        $("#destiny").val(item.travel.destiny);
        let items = _.cloneDeep(item.travel.replacements);
        _grdReplacements.setDataSource(getReplacementDS(items));
    }

    $("#replacement-detail").addClass("d-none");

    $("#Send").prop("checked", false);

    _wndDetail.open();
}

function getReplacementDS(items) {
    if (!items) items = [];
    return new kendo.data.DataSource({
        data: items,
        filter: [{ field: "statusType", operator: "neq", value: 3 }],
        schema: { model: { id: "id", fields: { "fromDate": { type: "date" }, "toDate": { type: "date" } } } }
    });
}

function cancelRequest(e) {
    e.preventDefault();
    _wndDetail.close();
}

function saveRequest(e) {
    e.preventDefault();
    notificator.hide();

    var form = $("#form-request"),
        configValidator = {
            messages: { required: "", reason: "", since: "", until: "", destiny: "", period: "Período Inválido", replacements: "Necesita un remplazo por el período solicitado" },
            rules: {
                reason: (i) => i.is("#reasonDescription") && _ddlType.value() == "L" && _ddlLicenseReason.value() == "-1" ? $.trim(i.val()) != "" : true,
                since: (i) => i.is("#fromDate") && _ddlType.value() != "L" ? $.trim(i.val()) != "" : true,
                until: (i) => i.is("#toDate") && _ddlType.value() != "L" ? $.trim(i.val()) != "" : true,
                period: (i) => (i.is("#toDate") || i.is("#fromDate")) && _ddlType.value() != "L" ? JSON.toDate($("#fromDate").val()) <= JSON.toDate($("#toDate").val()) : true,
                destiny: (i) => i.is("#destiny") && _ddlType.value() == "T" ? $.trim(i.val()) != "" : true,
                replacements: function (i) {
                    if (i.is("#id" && _ddlType.value() == "T" && _ddlType.value() == "V")) {
                        var items = _grdReplacements.dataSource.data(), since = JSON.toDate($("#fromDate").val()), until = JSON.toDate($("#toDate").val()), currentDay = since,
                            validPeriod = true;
                        while (currentDay <= until && validPeriod) {
                            validPeriod = items.filter((x) => x.fromDate <= currentDay && x.toDate >= currentDay).length > 0;
                            currentDay = currentDay.addDays(1);
                        }
                        return validPeriod;
                    }
                    return true;
                }
            },
            validationSummary: true
        },
        validator = form.kendoValidator(configValidator).data("kendoValidator");

    if (validator.validate()) {
        var item = {
            id: +$("#id").val(), idEmployee: +$("#idEmployee").val(), requestDate: JSON.toDate($("#requestDate").val()).toFormattedString(),
            idType: _ddlType.value(), idState: $("#Send").prop("checked") ? _states.find((x) => x.name == "sent").id : +$("#idState").val(), comments: $("#comments").val(), rejectComments: $(".reject-comments").text(),
            externalCode: $("#externalCode").val() != "NO sincronizado" ? $("#externalCode").val() : ""
        };
        if (item.idType == "L") {
            item.license = {
                id: +$("#idDetail").val(), date: _dpDate.value().toFormattedString(), initialTime: _tpInitial.value().toTimeFormattedString(),
                finalTime: _tpFinal.value().toTimeFormattedString(), idReason: _ddlLicenseReason.value(), reasonDescription: $("#reasonDescription").val()
            };
        }
        if (item.idType == "V") {
            var replaceItems = _grdReplacements.dataSource.data().map(function (x) {
                return { id: x.id, idReplacement: x.idReplacement, fromDate: x.fromDate.toFormattedString(), fromDatePeriod: x.fromDatePeriod, toDate: x.toDate.toFormattedString(), toDatePeriod: x.toDatePeriod, idState: "O" };
            });
            item.vacation = {
                id: +$("#idDetail").val(), fromDate: _dpSince.value().toFormattedString(), fromDatePeriod: _ddlSincePeriod.value(),
                toDate: _dpUntil.value().toFormattedString(), toDatePeriod: _ddlUntilPeriod.value(),
                days: getDays(_dpSince.value(), _ddlSincePeriod.value(), _dpUntil.value(), _ddlUntilPeriod.value()),
                replacements: replaceItems
            };
        }
        if (item.idType == "H") {
            item.homeOffice = {
                id: +$("#idDetail").val(), fromDate: _dpSince.value().toFormattedString(), fromDatePeriod: _ddlSincePeriod.value(),
                toDate: _dpUntil.value().toFormattedString(), toDatePeriod: _ddlUntilPeriod.value()
            };
        }
        if (item.idType == "T") {
            var replaceItems = _grdReplacements.dataSource.data().map(function (x) {
                return { id: x.id, idReplacement: x.idReplacement, fromDate: x.fromDate.toFormattedString(), fromDatePeriod: x.fromDatePeriod, toDate: x.toDate.toFormattedString(), toDatePeriod: x.toDatePeriod, idState: "O" };
            });
            item.travel = {
                id: +$("#idDetail").val(), fromDate: _dpSince.value().toFormattedString(), fromDatePeriod: _ddlSincePeriod.value(), toDate: _dpUntil.value().toFormattedString(),
                toDatePeriod: _ddlUntilPeriod.value(), destiny: $("#destiny").val(), replacements: replaceItems
            };
        }
        //console.log(item);

        $.post(urlEdit, { item: item }, function (d) {
            if (d.message == "") {
                showMessage("Se ha guardado la solicitud exitosamente");
                hideDetails();
                showRequestsGrid(d.items);
                _wndDetail.close();
            } else {
                //showError('Se ha producido un error al guardar la solicitud.');
                notificator.show({ title: 'Error', message: d.message }, 'error');
            }
        });
    } else {
        var errors = validator.errors();
        console.log("NO validó", errors);
    }
}

function deleteRequest(e) {
    e.preventDefault();
    notificator.hide();

    let id = $("#id").val();
    if (id == 0) {
        _wndDetail.close();
    } else {
        $.post(urlDelete, { Id: id }, function (d) {
            if (d.message == '') {
                notificator.show({ title: '', message: 'Solicitud eliminada exitosamente.' }, 'success');
                hideDetails();
                showRequestsGrid(d.items);
                _wndDetail.close();
            } else {
                notificator.show({ title: 'Error', message: d.message }, 'error');
            }
        });
    }
}

function getDays(startDate, startDatePeriod, endDate, endDatePeriod) {
    var days = 0, currentDate = new Date(startDate);
    if (startDate & endDate) {
        var holidays = JSON.parse($("#holidays").val());
        holidays.forEach(function (x) {
            x.since = JSON.toDate(x.since);
            x.until = JSON.toDate(x.until);
        });
        while (currentDate <= endDate) {
            var temp = holidays.some(x => x.since <= currentDate && x.until >= currentDate);
            if (!temp && currentDate.getDay() != 6 && currentDate.getDay() != 0) {
                days += 1;
            }
            currentDate.setDate(currentDate.getDate() + 1);
        }
        if (startDatePeriod == "PM") days -= 0.5;
        if (endDatePeriod == "AM") days -= 0.5;
    }
    return days;
}

function calculateDays() {
    let startDate = _dpSince.value(), startPeriod = _ddlSincePeriod.value(), endDate = _dpUntil.value(), endPeriod = _ddlUntilPeriod.value();
    let days = getDays(startDate, startPeriod, endDate, endPeriod);
    $(".selected-days span").text(days);
    $("#days").val(days);
}

function onButtonPressed(e) {
    e.preventDefault();
    let row = $(e.currentTarget).closest(".request"), item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row), newStateId = 0, state, rClass, action = $(e.currentTarget).text(), message;
    if (e.currentTarget.classList.contains('action-open')) {
        state = _states.find(x => x.name === 'open');
        message = 'Se ha abierto la solicitud correctamente.';
    }
    if (e.currentTarget.classList.contains('action-cancel')) {
        state = _states.find(x => x.name === 'cancelled');
        message = 'Se ha cancelado la solicitud correctamente.';
    }
    if (e.currentTarget.classList.contains('action-send')) {
        state = _states.find(x => x.name === 'sent');
        message = 'Se ha enviado la solicitud correctamente.';
    }
    newStateId = state.id;
    rClass = state.name;
    var temp = { id: item.id, idEmployee: item.idEmployee, requestDate: item.requestDate, fromDate: item.fromDate, fromDatePeriod: item.fromDatePeriod, toDate: item.toDate, toDatePeriod: item.toDatePeriod, days: item.days, idState: newStateId, idReplacement: item.idReplacement, externalCode: item.externalCode };
    temp.idState = newStateId;

    showConfirm(`¿Est&aacute; seguro que desea <b>${action}</b> la solicitud?`, function () {
        $.post(urlChangeState, { Item: temp, NewStateId: newStateId }, function (d) {
            if (d.message === "") {
                row.closest('.request').attr('class', `request ${rClass}`);
                row.find(".state").text(state.desc);
                //showNotification('', 'Se ha cambiado el estado de la solicitud correctamente.', 'success');
                notificator.show({ title: '', message: message }, rClass);
            }
        });
    });
}

function onEdit(e) {
    e.preventDefault();
    notificator.hide();
    let row = $(e.currentTarget).closest(".request"), item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row);
    loadRequestForm(item);
}

function onSendChange(e) {
    $('#save-request').removeClass('btn-primary btn-success').addClass(this.checked ? 'btn-success' : 'btn-primary').text(this.checked ? 'Enviar Solicitud' : 'Guardar Solicitud');
}

function onEditReplacement(e) {
    var row, item, loadDetail = function (item) {
        showHidePanelReplacement(true);
        $("#idDetailReplacement").val(item.id);
        _ddlReplace.value(item.idReplacement);
        _dpFromDateReplace.value(item.fromDate);
        _ddlFromDatePeriodReplace.value(item.fromDatePeriod);
        _dpToDateReplace.value(item.toDate);
        _ddlToDatePeriodReplace.value(item.toDatePeriod);
    };
    if (e.currentTarget.classList.contains("action-new")) {
        item = { id: _newId--, idReplacement: -1, fromDate: _dpSince.value(), fromDatePeriod: _ddlSincePeriod.value(), toDate: _dpUntil.value(), toDatePeriod: _ddlUntilPeriod.value(), statusType: 0 };
        loadDetail(item);
    }
    if (e.currentTarget.classList.contains("action-edit")) {
        row = $(e.currentTarget).closest("tr");
        item = _grdReplacements.dataItem(row);
        loadDetail(item);
    }
    if (e.currentTarget.classList.contains("action-delete")) {
        row = $(e.currentTarget).closest("tr");
        item = _grdReplacements.dataItem(row);
        showConfirm(`¿Está seguro que desea eliminar el remplazo <b>${item.replacementName}</b>?`, function () {
            if (item.statusType == 1) {
                _grdReplacements.dataSource.pushDestroy(item);
            } else {
                item.statusType = 3;
                _grdReplacements.dataSource.pushUpdate(item);
            }
        });
    }
}

function showHidePanelReplacement(visible) {
    $("#replacement-detail").empty();
    $("#replacement-detail").toggleClass("d-none", !visible);
    if (visible) {
        $("#replacement-detail").append(replaceForm);

        _ddlReplace = $("#idReplacement").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Remplazo...", filter: "contains", dataSource: { transport: { read: { url: urlEmployees } } } }).data("kendoDropDownList");
        _dpFromDateReplace = $("#fromDate-replacement").kendoDatePicker({ format: "dd/MM/yyyy", change: function () { if (this.value() == null) this.value(""); } }).data("kendoDatePicker");
        _dpToDateReplace = $("#toDate-replacement").kendoDatePicker({ format: "dd/MM/yyyy", change: function () { if (this.value() == null) this.value(""); } }).data("kendoDatePicker");
        _ddlFromDatePeriodReplace = $("#fromDatePeriod-replacement").kendoDropDownList().data("kendoDropDownList");
        _ddlToDatePeriodReplace = $("#toDatePeriod-replacement").kendoDropDownList().data("kendoDropDownList");
    }
    _wndDetail.center();
}

function saveReplacement(e) {
    var dSince = _dpSince.value(), dUntil = _dpUntil.value(), id = +$("#idDetailReplacement").val(), idReplace = +$("#idReplacement").val(),
        form = $(this).closest("form"),
        config = {
            rules: {
                since: function (i) {
                    if (i.is("#fromDate-replacement")) {
                        var v = JSON.toDate(i.val());
                        if (v < dSince || v > dUntil) return false;
                        if (_grdReplacements.dataSource.data().filter((x) => x.id != id && x.idReplacement == idReplace && v >= x.fromDate && v <= x.toDate).length > 0 && x.statusType != 3) return false;
                    }
                    return true;
                },
                until: function (i) {
                    if (i.is("#toDate-replacement")) {
                        var v = JSON.toDate(i.val());
                        if (v < dSince || v > dUntil) return false;
                        if (_grdReplacements.dataSource.data().filter((x) => x.id != id && x.idReplacement == idReplace && v >= x.fromDate && v <= x.toDate).length > 0 && x.statusType != 3) return false;
                    }
                    return true;
                }
            },
            messages: { required: "", since: "Fuera del rango a cubrir", until: "Fuera del rango a cubrir" }
        },
        validator = form.kendoValidator(config).data("kendoValidator");
    if (validator.validate()) {
        var item = id > 0 ? _grdReplacements.dataSource.get(id) : { id: id };
        item.statusType = item.id > 0 ? 2 : 1;
        item.idReplacement = +_ddlReplace.value();
        item.replacementName = _ddlReplace.text();
        item.fromDate = _dpFromDateReplace.value();
        item.fromDatePeriod = _ddlFromDatePeriodReplace.value();
        item.toDate = _dpToDateReplace.value();
        item.toDatePeriod = _ddlToDatePeriodReplace.value();

        _grdReplacements.dataSource.pushUpdate(item);

        showHidePanelReplacement(false);
    }
}

//#endregion