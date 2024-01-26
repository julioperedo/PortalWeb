//#region Variables Globales
var newId = 0, clients = [];
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30,
    perInsert = [1, 3, 5, 7], perUpdate = [2, 3, 6, 7], perDelete = [4, 5, 6, 7];
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(function () { cleanFilters(); });

$("#action-filter").click(function () { filterData(false); });

$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-assignments", onOpenAssigments);

$("#Listado").on("click", ".action-delete", onDelete);

$("#Listado").on("click", ".action-send-email", onSendMail);

$("#Listado").on("click", ".permissions", onEditPermissions);

$("#action-cancel-assigment").click(e => $("#assignments").data("kendoWindow").close());

$("#action-save-assigment").click(onSaveAssigment);

$("#Detail").on("click", "#action-cancel", e => $("#Detail").data("kendoWindow").close());

$("#Detail").on("click", "#action-save", onSave);

$("body").on("click", "#see-password", onClickSeePassword);

$("#action-cancel-permission").click(e => $("#extra-permissions").data("kendoWindow").close());

$("#action-save-permission").click(onSavingPermissions);

//#endregion

//#region Metodos Privados

function setupControls() {
    var ddlClients = $("#filClients").kendoDropDownList({
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", virtual: { itemHeight: 26, valueMapper: clientMapper },
        dataSource: { data: clients }
    }).data("kendoDropDownList");

    $("#filProfiles").kendoDropDownList({
        cascadeFrom: "filClients", cascadeFromField: "cardCode", dataTextField: "name", dataValueField: "id", enable: false, optionLabel: "Seleccione un Perfil ...",
        dataSource: { transport: { read: { url: urlProfiles, data: function (e) { return { CardCode: $("#filClients").val() }; } } }, serverFiltering: true }
    });

    $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de búsqueda.</div>' },
        columns: [
            { title: "Client Name", hidden: true, field: "clientName", groupHeaderTemplate: "Cliente: #= value #" },
            { title: "Profile Name", hidden: true, field: "profileName", groupHeaderTemplate: "Perfil: #= value #" },
            { title: "Nombre", field: "name", media: "lg" },
            { title: "E-Mail", field: "eMail", media: "lg" },
            { title: " ", attributes: alignCenter, width: 35, template: '# if($.trim(commentaries) !== "") {# <a class="comments action action-link" title="#=commentaries#"><i class="fas fa-comment-alt"></i></a> #} #', field: "commentaries", media: "lg" },
            { title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled", media: "lg" },
            { title: "Datos", field: "id", template: '<div class="row"><div class="col-12"><b>Nombre:</b> #=name#</div><div class="col-12"><b>E-Mail:</b> #=eMail#</div></div>', media: "(max-width: 991px)" },
            { title: " ", width: 40, template: '# if(enabled && eMail && eMail.length > 0) {# <a title="Enviar Datos de Acceso por Correo" class="action-send-email action action-link"><i class="fas fa-envelope"></i></a> #} #', field: "eMail", sortable: false },
            { title: " ", width: 40, template: '# if(enabled) {# <a title="Asignaciones de Empresa y Perfil extra" class="action-assignments action action-link"><i class="fas fa-id-card"></i></a> #} #', field: "cardCode", sortable: false },
            { title: " ", width: 40, template: e => e.enabled ? '<a class="action action-link permissions" title="Permisos Personalizados"><i class="fas fa-user-lock"></i></a>' : '', sortable: false },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo Usuario"></i>',
                template: function (e) {
                    var attrDisabled = "", css = "action action-link action-delete", title = "Eliminar Usuario", result;
                    if (e.sessionCount > 0) {
                        css = "disabled";
                        attrDisabled = 'disabled="disabled"';
                        title = "No se puede eliminar el usuario porque ya ha sido usado";
                    }
                    result = `<a class="action action-link action-edit" title="Editar Usuario"><i class="fas fa-pen"></i></a>&nbsp;&nbsp;<a class="${css}" ${attrDisabled} title="${title}"><i class="fas fa-trash-alt"></i></a>`;
                    return result;
                }
            }
        ],
        sortable: true, selectable: "Single, Row", dataSource: []
    });

    $("#Listado").kendoTooltip({ filter: "a.comments", offset: 10, content: kendo.template('<div class="template-wrapper"><p>#=target.data("title")#</p></div>') });
    $("#assignments").kendoWindow({ width: 900, visible: false, modal: true });
    $("#assigned-clients").kendoMultiSelect({ filter: "contains", dataTextField: "name", dataValueField: "code", placeholder: "Seleccione los Clientes" });
    $("#assigned-profiles").kendoMultiSelect({ filter: "contains", dataTextField: "name", dataValueField: "id", placeholder: "Seleccione los Perfiles" });

    $("#extra-permissions").kendoWindow({ width: 900, visible: false, modal: true, close: onPermissionsClose, activate: onRefreshWindow });

    $.get(urlClients, {}, d => {
        clients = d;
        ddlClients.setDataSource(d);
    });

    filterData(true);
}

function cleanFilters() {
    var objClients = $("#filClients").getKendoDropDownList();
    objClients.text("");
    objClients.value("");
    $("#filName").val("");
}

function getFilters() {
    var message = "", cardCode = $("#filClients").getKendoDropDownList().value(), name = $("#filName").val(), profile = $("#filProfiles").getKendoDropDownList().value();
    if (cardCode === "" && name === "" && profile === "") {
        message = "- Por lo menos un criterio de búsqueda."
    }
    return { message: message, data: { CardCode: cardCode, ProfileCode: profile, Name: name } };
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
        setGridHeight("Listado", _gridMargin);
        if (!pageLoad) {
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
}

function loadGrid(items) {
    if (items) {
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({
            data: items, group: [{ field: "clientName", dir: "asc" }, { field: "profileName", dir: "asc" }],
            sort: [{ field: "name", dir: "asc" }],
            schema: { model: { id: "id" } }
        });
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

function clientMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.code === options.value));
    options.success(index);
}

function clientMapperMulti(options) {
    //deberia traer los objetos del multiselect pero todavía no ha traido los datos
    var items = $("#filClients").getKendoDropDownList().dataSource.data();
    var indexs = [];
    options.value.forEach((x) => {
        indexs.push(items.indexOf(items.find(i => i.code === x)));
    });
    //var index = items.indexOf(items.find(i => i.Code === options.value));
    options.success(indexs);
}

function saveUser() {
    var user = $("form").serializeObject();
    if (user.Id <= 0) {
        var ddlClient = $("#CardCode").getKendoDropDownList();
        if (ddlClient) {
            user.ClientName = " ( " + ddlClient.text().replace(" - ", " ) ");
            user.ProfileName = $("#IdProfile").getKendoDropDownList().text();
        }
    }
    user.Enabled = $("#Enabled").prop("checked");
    user.AllowLinesBlocked = $("#AllowLinesBlocked").prop("checked");
    var filters = getFilters();
    $.post(urlEdit, { User: user, Filters: filters.data }, function (data) {
        if (data.message === "") {
            loadGrid(data.items);

            $("#Detail").data("kendoWindow").close();
            showMessage("Se realizaron los cambios correctamente.");
        } else {
            showError(data.message);
        }
    });
}

function loadAssigments(id, name, clients, profiles) {
    $("#assigned-userid").val(id);
    $.get(urlAssigments, { Id: id }, function (data) {
        if (data.message === "") {
            clients.value(data.clients);
            profiles.value(data.profiles);

            var dialog = $("#assignments").data("kendoWindow");
            dialog.title(`Asignaciones de ${name}`);
            dialog.open().center();
        } else {
            showError(`Se ha producido el siguiente error al traer los datos: <br />${data.message}`);
        }
    });
}

function onClickSeePassword(e) {
    var txtPassword = $("#Password");
    if (txtPassword.attr("type") === "password") {
        txtPassword.attr("type", "text");
    } else {
        txtPassword.attr("type", "password");
    }
    $(this).find("i").toggleClass("fa-eye").toggleClass("fa-eye-slash");
}

function onRefresh(e) {
    $("#CardCode").kendoDropDownList({
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", virtual: { itemHeight: 26, valueMapper: clientMapper },
        dataSource: { data: clients },
        change: function (e) {
            var value = this.value();
            $.get(urlBlackList, { CardCode: value }, function (data) {
                $("#BlackList").val(data.isBlackListed);
                $("#ValidForEnabling").val(data.validForEnabling);
            });
        }
    });
    var idProfile = +$("#IdProfile").val();
    var ddlProfile = $("#IdProfile").kendoDropDownList({
        cascadeFrom: "CardCode", cascadeFromField: "cardCode", dataTextField: "name", dataValueField: "id", enable: false, optionLabel: "Seleccione un Perfil ...",
        dataSource: {
            transport: {
                read: {
                    url: urlProfiles,
                    data: function (e) {
                        return { CardCode: $("#CardCode").val(), CardName: e.filter && e.filter.filters.length > 1 ? e.filter.filters[1].value : "" };
                    }
                }
            }, serverFiltering: true
        }
    }).data("kendoDropDownList");
    ddlProfile.value(idProfile);

    onRefreshWindow(e);
}

function onSave(e) {
    var form = $(this).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var validForEnabling = $("#ValidForEnabling").val() === "Y", isBlacklisted = $("#BlackList").val() === "Y";
        if (isBlacklisted || !validForEnabling) {
            if (isBlacklisted) {
                showConfirm("Ese Cliente está marcado como Lista Negra, ¿Desea guardarlo de todas maneras?", () => saveUser());
            } else {
                showConfirm("Ese Cliente no tiene compras, ¿Desea guardarlo de todas maneras?", () => saveUser());
            }
        } else {
            saveUser();
        }
    }
}

function onSaveAssigment(e) {
    var lstIds = $("#assigned-clients").getKendoMultiSelect().value(), lstIdProfiles = $("#assigned-profiles").getKendoMultiSelect().value();
    var idUser = $("#assigned-userid").val();
    $.post(urlAssigments, { IdUser: idUser, ClientIds: lstIds, ProfileIds: lstIdProfiles }, function (data) {
        if (data.message === "") {
            showMessage("Se realizaron los cambios correctamente.");
            $("#assignments").data("kendoWindow").close();
        } else {
            showError(data.message);
        }
    });
}

function onSendMail(e) {
    var item = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    $.post(urlSendEmail, { Id: item.id, WithCopy: true }, function (data) {
        if (data.message !== "") {
            showError(data.message);
        } else {
            showMessage("Se ha enviado el correo exitosamente.");
        }
    });
}

function onDelete(e) {
    var dataItem = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el Usuario <b>${dataItem.name}</b>?`, function () {
        var filtersData = getFilters();
        $.post(urlDelete, { Id: dataItem.id, Filters: filtersData.data }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage(`Se ha eliminado el Usuario <b>${dataItem.name}</b> correctamente.`);
            } else {
                console.error(data.message);
                showError("No se ha podido eliminar el usuario.");
            }
        });
    });
}

function onOpenAssigments(e) {
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var assignedProfiles = $("#assigned-profiles").data("kendoMultiSelect"), assignedClients = $("#assigned-clients").data("kendoMultiSelect");
    if (assignedClients.dataSource.data().length > 0 && assignedProfiles.dataSource.data().length > 0) {
        loadAssigments(item.id, item.name, assignedClients, assignedProfiles);
    } else {
        $.get(urlProfiles, {}, function (data) {
            data.forEach((x) => { x.name = `${(x.isExternalCapable ? "(Cliente) " : "")}${x.name}` });
            assignedProfiles.setDataSource(new kendo.data.DataSource({ data: data }));
        });
        assignedClients.setDataSource(new kendo.data.DataSource({ data: clients }));
        loadAssigments(item.id, item.name, assignedClients, assignedProfiles);
    }
}

function onEdit(e) {
    var wnd = $("#Detail").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    wnd.refresh({ url: urlEdit, data: { Id: item.id } });
    wnd.center().open();
}

function onNew(e) {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: 0 } });
    wnd.center().open();
}

function onEditPermissions(e) {
    e.preventDefault();

    var wnd = $("#extra-permissions").data("kendoWindow"), grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    $.get(urlPermissions, { Id: item.id }, function (d) {
        if (d.message === "") {
            $("#permissions-userid").val(item.id);
            $("#title-permissions").text(`Permisos especiales de ${item.name} ( ${item.eMail} )`);
            var ulContent = '<ul class="nav nav-tabs" role="tablist">', divContent = '<div class="tab-content">';
            d.items.forEach((mod, i) => {
                var selClass = i === 0 ? "active show" : "", selAttr = i === 0 ? 'aria-selected="true"' : '';
                ulContent += `<li class="nav-item"><a data-toggle="tab" href="#tab${i}" role="tab" class="nav-link ${selClass}" ${selAttr}>${mod.module}</a></li>`;
                divContent += `<div class="tab-pane ${selClass}" id="tab${i}" role="tabpanel"><section><table class="table table-sm"><thead class="thead-light"><tr><th>Nombre</th><th style="text-align: center;">Insertar</th><th style="text-align: center;">Actualizar</th><th style="text-align: center;">Eliminar</th></tr></thead><tbody>`;
                mod.items.forEach((act, j) => {
                    var insertChecked = perInsert.indexOf(act.permission) >= 0 ? 'checked="checked"' : '', updateChecked = perUpdate.indexOf(act.permission) >= 0 ? 'checked="checked"' : ''
                        , deleteChecked = perDelete.indexOf(act.permission) >= 0 ? 'checked="checked"' : '';
                    divContent += `<tr class="activity-row">
    <td>${act.name}</td>
    <td class="text-center"><label class="switch"><input type="checkbox" ${insertChecked} class="p-insert" data-id="${act.id}" data-idactivity="${act.idActivity}" data-updated="F" /><span class="slider round"></span></label></td>
    <td class="text-center"><label class="switch"><input type="checkbox" ${updateChecked} class="p-update" data-updated="F" /><span class="slider round"></span></label></td>
    <td class="text-center"><label class="switch"><input type="checkbox" ${deleteChecked} class="p-delete" data-updated="F" /><span class="slider round"></span></label></td>
</tr>`;
                });
                divContent += '</tbody></table></section></div>';
            });
            ulContent += '</ul>';
            divContent += '</div>';
            $("#tabs").append(ulContent + divContent);
            wnd.center().open();

            $('a[data-toggle="tab"]').on('shown.bs.tab', (e) => wnd.center());

            $(".p-insert, .p-update, .p-delete").on("change", onChange);
        } else {
            showError("Se ha producido un error al intentar traer los permisos del usuario.");
        }
    });
}

function onChange(e) {
    var d = e.currentTarget.dataset;
    d.updated = d.updated === "F" ? 'T' : 'F';
}

function onPermissionsClose(e) {
    $("#permissions-userid").val("0");
    $("#tabs").empty();
}

function onSavingPermissions(e) {
    e.preventDefault();
    var userId = $("#permissions-userid").val(), rows = $(".activity-row"), items = [];

    rows.each((i, r) => {
        var chkI = $(r).find(".p-insert"), chkU = $(r).find(".p-update"), chkD = $(r).find(".p-delete"), dI = chkI.data(), id = dI.id, idActivity = dI.idactivity, insertV = chkI.prop("checked"),
            dU = chkU.data(), dD = chkD.data(), updateV = chkU.prop("checked"), deleteV = chkD.prop("checked"), permission = (insertV ? 1 : 0) + (updateV ? 2 : 0) + (deleteV ? 4 : 0),
            updated = dI.updated === "T" || dU.updated === "T" || dD.updated === "T";
        if (updated) items.push({ id: id, idUser: userId, idActivity: idActivity, permission: permission });
    });

    if (items.length > 0) {
        $.post(urlSavePermissions, { Items: items }, function (d) {
            if (d.message === "") {
                showMessage(`Permisos guardados correctamente.`);
                $("#extra-permissions").data("kendoWindow").close();
            } else {
                showError(`Se ha producido un error al guardar los datos de los permisos.`);
            }
        });
    } else {
        $("#extra-permissions").data("kendoWindow").close();
    }    
}

//#endregion