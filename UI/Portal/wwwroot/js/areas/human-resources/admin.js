//#region Variables Globales
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { "class": "text-right", style: "justify-content: right" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
var _minDate, _maxDate, _grdEmp, _lvRequests;
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("list-employees", gridMargin));

$("#list-requests").on("click", ".aprove-request", onAprove);

$("#list-requests").on("click", ".deny-request", onDeny);

$('a[data-toggle="tab"]').on('shown.bs.tab', onTabActive);

//#endregion

//#region Metodos Locales

function setupControls() {
    _grdEmp = $("#list-employees").kendoGrid({
        columns: [
            { title: "Apellidos", field: "lastName", width: 140 },
            { title: "Nombres", field: "firstName", width: 140 },
            { title: "F. Ingreso", field: "startDate", width: 70, format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter },
            { title: "A&ntilde;os", field: "years", width: 70, attributes: alignRight, headerAttributes: alignRight },
            { title: "D&iacute;as", field: "days", format: "{0: 0.0}", width: 90, aggregates: ["sum"], attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e?.days?.sum ?? 0 },
            { title: "D&iacute;as (Duodécima)", field: "extraDays", format: "{0: 0.0}", width: 90, aggregates: ["sum"], attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e?.extraDays?.sum ?? 0 },
            { title: "Tomados", field: "taken", format: "{0: 0.0}", width: 90, aggregates: ["sum"], attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e?.taken?.sum ?? 0 },
            { title: "Restantes", field: "remaining", format: "{0: 0.0}", width: 90, aggregates: ["sum"], attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => e?.remaining?.sum ?? 0 },
            { title: " ", field: "id", attributes: alignCenter, width: 50, template: '<a class="action action-link detail-employee" title="Ver detalle"><i class="fas fa-pen"></i></a>' }
        ],
        noRecords: { template: '<div class="w-100 p-4">No hay items para el criterio de b&uacute;squeda.</div>' },
        sortable: true, selectable: "Single, Row", dataSource: []
    }).data("kendoGrid");

    _lvRequests = $("#list-requests").kendoListView({
        dataSource: {
            transport: { read: { url: urlRequests } },
            schema: { model: { id: "id", fields: { "requestDate": { "type": "date" }, "fromDate": { "type": "date" }, "toDate": { "type": "date" } } } },
            sort: { field: "id", dir: "asc" }
        },
        template: function (e) {
            var period = '', type = e.idType == "L" ? "Licencia" : e.idType == "V" ? "Vacaci&oacute;n" : e.idType == "H" ? "Home Office" : "Viaje de Trabajo",
                management = '', reason = '', replacement = '', comments = '', availableDays = '', isEnabled = true;
            if ($.trim(e.comments) != "") {
                comments += `<div class="row"><div class="col"><label>Comentarios:</label> <span class="comments">${e.comments}</span></div></div>`;
            }
            if (e.managers.length > 0) {
                management += '<div class="row">';
                e.managers.forEach((x) => management += `<div class="col"><label>Jefe:</label> <span class="label-data">${x.name}</span></div>`);
                management += '</div>';
            }
            if (e.idType == "V") {
                period = `<div class="row">
                <div class="col">
                    <label>Desde:</label> <span class="from-date">${kendo.toString(JSON.toDate(e.vacation.fromDate), "dd-MM-yyyy")}  ${e.vacation.fromDatePeriod}</span>
                </div>
                <div class="col">
                    <label>Hasta:</label> <span class="to-date">${kendo.toString(JSON.toDate(e.vacation.toDate), "dd-MM-yyyy")}  ${e.vacation.toDatePeriod}</span>
                </div>
                <div class="col">
                    <span class="days">${e.vacation.days}</span><label>d&iacute;as</label>
                </div>
            </div>`;
                if (e.vacation.replacements.length > 0) {
                    replacement += '<div class="row">';
                    e.vacation.replacements.forEach(function (r) {
                        replacement += `<div class="col"><label>Remplazo:</label> <span class="replacemnet">${r.replacementName}</span></div>`;
                        if (!r.available) isEnabled = false;
                    });
                    replacement += '</div>';
                }
                availableDays = `<span class="available-days">(${e.availableDays} d&iacute;as disponibles)</span>`;
            }
            if (e.idType == "L") {
                e.license.date = JSON.toDate(e.license.date);
                e.license.initialTime = JSON.toTime(e.license.initialTime);
                e.license.finalTime = JSON.toTime(e.license.finalTime);
                period = `<div class="row">
                <div class="col">
                    <label>Fecha:</label> <span class="from-date">${kendo.toString(e.license.date, "dd-MM-yyyy")} ( ${kendo.toString(e.license.initialTime, "HH:mm")} al ${kendo.toString(e.license.finalTime, "HH:mm")} )</span>
                </div>
            </div>`;
                var reasonDesc = $.trim(e.license.idReason == -1 ? e.license.reasonDescription : e.license.reasonName);
                reason += reasonDesc != "" ? `<div class="row"><div class="col"><label>Motivo:</label><span class="reason">${reasonDesc}</span></div></div>` : "";
            }
            if (e.idType == "H") {
                period = `<div class="row">
                <div class="col">
                    <label>Desde:</label> <span class="from-date">${kendo.toString(JSON.toDate(e.homeOffice.fromDate), "dd-MM-yyyy")}  ${e.homeOffice.fromDatePeriod}</span>
                </div>
                <div class="col">
                    <label>Hasta:</label> <span class="to-date">${kendo.toString(JSON.toDate(e.homeOffice.toDate), "dd-MM-yyyy")}  ${e.homeOffice.toDatePeriod}</span>
                </div>
            </div>`;
            }
            if (e.idType == "T") {
                period = `<div class="row">
                <div class="col">
                    <label>Desde:</label> <span class="from-date">${kendo.toString(JSON.toDate(e.travel.fromDate), "dd-MM-yyyy")}  ${e.travel.fromDatePeriod}</span>
                </div>
                <div class="col">
                    <label>Hasta:</label> <span class="to-date">${kendo.toString(JSON.toDate(e.travel.toDate), "dd-MM-yyyy")}  ${e.travel.toDatePeriod}</span>
                </div>
            </div>`;
                if (e.travel.replacements.length > 0) {
                    replacement += '<div class="row">';
                    e.travel.replacements.forEach(function (r) {
                        replacement += `<div class="col"><label>Remplazo:</label> <span class="replacemnet">${r.replacementName}</span></div>`;
                        if (!r.available) isEnabled = false;
                    });
                    replacement += '</div>';
                }
            }
            var content = `<div class="request">
    <div class="row">
        <div class="col-9">
            <div class="row">
                <div class="col"> <span class="request-type">Solicitud de ${type}</span></div>
            </div>
            <div class="row">
                <div class="col">
                    <span class="employee-name">${e.employeeName}</span> ${availableDays}                    
                    <span class="request-date">${kendo.toString(e.requestDate, "dd-MM-yyyy HH:mm")}</span>
                </div>
            </div>            
            ${period}
            ${reason}
            ${replacement}
            ${management}
            ${comments}
        </div>
        <div class="col-3 text-center">
            <button class="btn aprove-request align-middle" ${(!isEnabled ? "disabled" : "")}>Aprobar</button><button class="btn btn-danger deny-request align-middle">Rechazar</button>
        </div>
    </div>
</div>`;
            return content;
        }
    }).data("kendoListView");

    $.get(urlEmployeesResume, {}, function (d) {
        if (d.message == "") {
            d.items.forEach((x) => x.startDate = JSON.toDate(x.startDate));
            var ds = new kendo.data.DataSource({
                data: d.items.filter(x => x.isAvailable),
                aggregate: [
                    { aggregate: "sum", field: "days" },
                    { aggregate: "sum", field: "extraDays" },
                    { aggregate: "sum", field: "taken" },
                    { aggregate: "sum", field: "remaining" }
                ],
                //sort: [{ field: "id", dir: "asc" }],
                schema: { model: { id: "id" } }
            });
            _grdEmp.setDataSource(ds);
        }
    });
}

function onAprove(e) {
    let row = $(e.currentTarget).closest(".request"), item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row);
      var  question = `¿Est&aacute; seguro que desea aprobar la solicitud de <b>${item.employeeName}</b>?`;
    if (item.vacation) question += item.availableDays < item.vacation.days ? `, sólo tiene ${item.availableDays} días disponibles de los ${item.vacation.days} solicitados.` : '';
    question += `<br /><label for="comments" class="pt-2">Comentarios</label><textarea id="comments" type="text" class="form-control" rows="2"></textarea>`;
    //console.log(item);
    showConfirm(question, function (r) {
        let comments = $("#comments").val();
        $.post(urlApprove, { Id: item.id, Comments: comments }, function (d) {
            if (!d.withErrors) {
                if (d.message == "") {
                    showMessage("Se ha aprobado la solicitud correctamente.");
                    row.remove();
                } else {
                    showInfo(d.message);
                }
            } else {
                showError("Se ha producido un error al aprobar la solicitud, vuelva a intentar más tarde.");
            }
        });
    });
}

function onDeny(e) {
    let row = $(e.currentTarget).closest(".request"), item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row),
        question = `¿Est&aacute; seguro que desea rechazar la solicitud de <b>${item.employeeName}</b>?<br />
        <label for="reject-reason" class="pt-2">Motivo</label><textarea id="reject-reason" type="text" class="form-control" rows="2"></textarea>`;
    //console.log(item);
    showConfirm(question, function (r) {
        let reason = $("#reject-reason").val();
        $.post(urlReject, { Id: item.id, RejectReason: reason }, function (d) {
            if (!d.withErrors) {
                if (d.message === "") {
                    showMessage("Se ha rechazado la solicitud correctamente.");
                    row.remove();
                } else {
                    showInfo(d.message);
                }
            } else {
                showError("Se ha producido un error al rechazar la solicitud, vuelva a intentar más tarde.");
            }
        });
    });
}

function onTabActive(e) {
    if ($(e.target).attr("href") === "#tab2") {
        setGridHeight("list-employees", gridMargin);
    }
}

//#endregion