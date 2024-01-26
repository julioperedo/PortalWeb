//#region Variables Globales
var newId = 0;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30,
    states = { Created: 21, Accepted: 22, Rejected: 23, Deleted: 24 };
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("processed-requests", _gridMargin));

$("#action-clean").click(function (e) { cleanFilters(); });

$("#action-filter").click(function (e) { filterData(); });

$("#pendings").on("click", ".btn-aprove", aproveRequest);

$("#pendings").on("click", ".btn-reject", rejectRequest);

$("#pendings").on("click", ".btn-delete", deleteRequest);

$("#processed-requests").on("click", ".undo-request", undoRequest);

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#pendings").kendoListView({
        dataSource: {
            transport: { read: { url: urlFilter, data: { State: states.Created, Filter: "" } } },
            schema: { model: { fields: { "requestDate": { "type": "date" }, "createDate": { "type": "date" } } } }
        },
        template: kendo.template(jQuery("#tPending").html())
    });

    $("#request-type").kendoDropDownList();

    $("#processed-requests").kendoGrid({
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        columns: [
            { title: "Cod.", width: "80px", field: "cardCode", width: 90 },
            { title: "Cliente", field: "clientName" },
            { title: "Nombre Completo", field: "fullName" },
            { title: "Correo", field: "eMail" },
            { title: "Teléfono", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "phone" },
            { title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "requestDate", format: dateFormat },
            { title: " ", attributes: alignCenter, width: 30, template: '# if($.trim(comments) !== "") {# <a class="comments action" title="#=Comments#"><i class="fas fa-comment-alt"></i></a> #} #', field: "comments" },
            { title: "Compras", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: '# if(hasOrders) {# <i class="fas fa-check"></i> #} #', field: "hasOrders" },
            { title: "Correo Val.", attributes: alignCenter, headerAttributes: alignCenter, width: 110, template: '# if(validEMail) {# <i class="fas fa-check"></i> #} #', field: "validEMail" },
            { title: "Lista Negra", attributes: alignCenter, headerAttributes: alignCenter, width: 110, template: '# if(inBlackList) {# <i class="fas fa-check"></i> #} #', field: "inBlackList" },
            { title: "Revertir", field: "stateIdc", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(validEMail && hasOrders && !inBlackList) {# <a title="Revertir la solicitud" class="action undo-request"><i class="fas fa-undo-alt"></i></a> #} #' }
        ],
        sortable: true, selectable: "Single, Row",
        dataSource: [],
        detailInit: function (e) {
            var url = `${urlDetail}?IdRequest=${e.data.id}`;
            $("<div/>").appendTo(e.detailCell).kendoGrid({
                dataSource: {
                    transport: { read: url }, sort: [{ field: "logDate", dir: "desc" }],
                    schema: { model: { fields: { logDate: { type: "date" } } } }
                },
                sortable: true, pageable: false,
                columns: [
                    { field: "stateName", title: "Estado" },
                    { field: "userName", title: "Usuario" },
                    { field: "logDate", title: "Fecha", format: "{0:dd-MM-yyyy HH:mm}", attributes: alignCenter, headerAttributes: alignCenter }
                ]
            });
        },
        dataBound: function (e) {
            var grid = e.sender;
            var type = +$("#request-type").data("kendoDropDownList").value();
            if (type === states.Accepted) {
                grid.hideColumn("stateIdc");
            } else {
                grid.showColumn("stateIdc");
            }
            if (type === states.Deleted) {
                grid.showColumn("comments");
            } else {
                grid.hideColumn("comments");
            }
        }
    });

    $("#processed-requests").kendoTooltip({
        filter: "a.comments", offset: 150, content: kendo.template($("#tooltip-template").html())
    });
}

function cleanFilters() {
    $("#request-filter").val("");
    loadGrid([]);
}

function filterData() {
    var filter = $("#request-filter").val(), type = $("#request-type").data("kendoDropDownList").value();
    $.get(urlFilter, { State: type, Filter: filter }, function (data) {
        loadGrid(data);
    });
}

function loadGrid(items) {
    if (items) {
        items.forEach(x => x.requestDate = JSON.toDate(x.requestDate));
        var grd = $("#processed-requests").data("kendoGrid");
        var ds = new kendo.data.DataSource({ data: items, schema: { model: { id: "id" } } });
        grd.setDataSource(ds);
        setGridHeight("processed-requests", _gridMargin);
    }
}

function approveUser(idRequest, idUser, row) {
    $.post(urlApproveUser, { IdRequest: idRequest, IdUser: idUser }, function (data) {
        if (data.result) {
            showMessage(data.message);
            row.remove();
        } else {
            showError(data.message);
        }
    });
}

function undoRequest(e) {
    var grid = $(e.currentTarget).closest(".k-grid").getKendoGrid();
    var row = $(e.currentTarget).closest("tr");
    var item = grid.dataItem(row);

    grid.select(row);
    var type = +$("#request-type").data("kendoDropDownList").value() === states.Rejected ? "el rechazo" : "la eliminación";
    showConfirm(`¿Está seguro que desea revertir ${type} de la solicitud de <b>( ${item.cardCode} ) ${item.clientName}</b>?`, () => {
        $.post(urlUndo, { IdRequest: item.cd }, (data) => {
            if (data.result) {
                row.remove();
                $("#pendings").data("kendoListView").dataSource.add(item);
                showMessage(data.message);
            } else {
                showError(data.message);
            }
        });
    });
}

function deleteRequest(e) {
    var row = $(e.currentTarget).closest(".request-item");
    var item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row);
    var text = `¿Est&aacute; seguro que desea eliminar la solicitud de: <b>${item.clientName}</b>?<br /><label for="delete-reason">Motivo:</label><input id="delete-reason" name="delete-reason" class="form-control" style="width: 400px;">`;
    showConfirm(text, () => {
        var reason = $("#delete-reason").val();
        $.post(urlDelete, { IdRequest: item.id, ReasonReject: reason }, (data) => {
            if (data.message === "") {
                row.remove();
                showMessage("Solictud eliminada correctamente");
            } else {
                showError(`Se ha producido el siguiente error al querer eliminar la solicitud: <br />${data.message}`);
            }
        });
    });
}

function rejectRequest(e) {
    var row = $(e.currentTarget).closest(".request-item");
    var item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row);
    var text = `<span>¿Está seguro que desea rechazar la Solicitud de: <b>${item.clientName}</b> ?</span><br /><label for="valid-names" class="action">¿El nombre de la solicitud es v&aacute;lido?&nbsp;&nbsp;&nbsp;</label><label class="switch"><input id="valid-names" name="valid-names" type="checkbox"><span class="slider round"></span></label>`;
    showConfirm(text, () => {
        var validNames = $("#valid-names").prop("checked");
        $.post(urlReject, { IdRequest: item.id, Code: item.cardCode, Name: item.fullName, EMail: item.eMail, NameMismatch: !validNames, WithoutOrders: !item.hasOrders, InvalidEMail: !item.validEMail, IsNew: item.isNew }, function (data) {
            if (data.result) {
                showMessage(data.message);
                row.remove();
            } else {
                showError(data.message);
            }
        });
    });
}

function aproveRequest(e) {
    var row = $(e.currentTarget).closest(".request-item");
    var item = $(e.currentTarget).closest(".k-listview").data("kendoListView").dataItem(row);
    if ((item.idUser > 0 && item.sameClient) || item.idUser === 0) {
        approveUser(item.id, item.idUser, row);
    } else {
        showConfirm(`¿Ese correo está asignado a otro cliente, desea continuar y reasignar a el cliente (<b>${item.cardCode}</b>) ${item.clientName}?`, () => approveUser(item.id, item.idUser, row));
    }
}

//#endregion