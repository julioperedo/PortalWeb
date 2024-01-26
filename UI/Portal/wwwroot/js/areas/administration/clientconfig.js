//#region Variables Globales
const alignCenter = { style: "text-align: center;" }, _gridMargin = 30;
//#endregion

//#region Eventos

$(() => {
    setupControls();
    setTimeout(() => setGridHeight("Listado", _gridMargin), 800);
});

$(window).resize(() => {
    setGridHeight("Listado", _gridMargin);
});

$("#Listado").on("click", ".action-new", function (e) {
    var wnd = $("#Detail").data("kendoWindow");
    var item = { Id: 0, Name: "", EMail: "", Type: 1 };
    loadDetail(item);
    wnd.center().open();
});

$("#Listado").on("click", ".action-edit", function (e) {
    var wnd = $("#Detail").data("kendoWindow");
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    loadDetail(item);
    wnd.center().open();
});

$("#Listado").on("click", ".action-delete", function (e) {
    var dataItem = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el Contacto <b>${dataItem.name}</b> para <b>${dataItem.typeDesc}</b>?`, function () {
        $.post(urlDelete, { Id: dataItem.id }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage(`Se ha eliminado el Usuario <b>${dataItem.name}</b> correctamente.`);
            } else {
                showError(data.message);
            }
        });
    });
});

$("#Detail").on("click", "#action-cancel", function (e) {
    $("#Detail").data("kendoWindow").close();
});

$("#Detail").on("click", "#action-save", function (e) {
    var form = $(this).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        $.post(urlEdit, { Item: item }, data => {
            if (data.message === "") {
                loadGrid(data.items);
                $("#Detail").data("kendoWindow").close();
            } else {
                showError("Se ha producido un error al intentar guardar los datos en el servidor.");
            }
        });
    }
});

$("#ListadoUsuarios").on("click", ".action-new", e => {
    var wnd = $("#DetailUser").data("kendoWindow");
    var item = { id: 0, login: "", password: "", cardCode: cardCode, lastGeneratedTokenDate: null, enabled: true };
    loadDetailUser(item);
    wnd.center().open();
});

$("#ListadoUsuarios").on("click", ".action-edit", e => {
    var wnd = $("#DetailUser").data("kendoWindow");
    var grd = $("#ListadoUsuarios").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    loadDetailUser(item);
    wnd.center().open();
});

$("#ListadoUsuarios").on("click", ".action-delete", e => {
    var dataItem = $("#ListadoUsuarios").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el usuario <b>${dataItem.login}</b>?`, function () {
        $.post(urlDeleteUser, { Id: dataItem.id }, function (data) {
            if (data.message === "") {
                loadGridUsers(data.items);
                showMessage(`Se ha eliminado el Usuario <b>${dataItem.login}</b> correctamente.`);
            } else {
                showError(data.message);
            }
        });
    });
});

$("#see-password").click(e => {
    var txtPassword = $("#password");
    txtPassword.attr("type", txtPassword.attr("type") === "password" ? "text" : "password");
    $(e.currentTarget).find("i").toggleClass("fa-eye").toggleClass("fa-eye-slash");
});

$("#DetailUser").on("click", "#action-cancel-user", e => $("#DetailUser").data("kendoWindow").close());

$("#DetailUser").on("click", "#action-save-user", e => {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "campo requerido" } }).data("kendoValidator");
    if (validator.validate()) {
        var item = JSON.parse($("#user-data").val());
        item.login = $("#user-name").val();
        item.password = $("#password").val();
        item.enabled = $("#enabled").prop("checked");
        $.post(urlEditUser, { Item: item }, data => {
            if (data.message === "" & data.errors === "") {
                loadGridUsers(data.items);
                $("#DetailUser").data("kendoWindow").close();
                showMessage("Se han guardado los datos exitosamente.");
            } else {
                if (data.message === "") {
                    console.error(data.errors);
                    showError("Se ha producido un error al intentar guardar los datos en el servidor.");
                } else {
                    showError(data.message);
                }
            }
        });
    }
});

//#endregion

//#region Metodos Locales

function setupControls() {
    $("#Listado").kendoGrid({
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        dataSource: [],
        columns: [
            { field: "name", title: "Nombre" },
            { field: "eMail", title: "Correo Electrónico" },
            { field: "typeDesc", title: "Tipo" },
            { field: "blackList", width: 120, title: "En Lista Negra", attributes: alignCenter, headerAttributes: alignCenter, template: '# if(blackList) {# <i class="fas fa-check"></i> #} #' },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-new" title="Nuevo Contacto"></i>',
                template: '<i class="fas fa-pen action action-edit" title="Editar Contacto"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-delete" title="Eliminar Contacto"></i>'
            }
        ],
        mobile: true, sortable: true, selectable: true
    });

    $("#ListadoUsuarios").kendoGrid({
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
        dataSource: [],
        width: 700,
        columns: [
            { field: "login", title: "Login", width: 300 },
            { field: "lastGeneratedTokenDate", title: "Última generación de Token", format: "{0:dd-MM-yyyy}", width: 180 },
            { field: "enabled", width: 120, title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #' },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-new" title="Nuevo Usuario"></i>',
                template: '<i class="fas fa-pen action action-edit" title="Editar Usuario"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-delete" title="Eliminar Usuario"></i>'
            }
        ],
        mobile: true, sortable: true, selectable: true
    });

    $("#Detail").kendoWindow({ visible: false, width: 950, title: "Contacto", modal: true });
    $("#DetailUser").kendoWindow({ visible: false, width: 950, title: "Usuario del WebSevice", modal: true });

    $("#Type").kendoDropDownList({ dataTextField: "name", dataValueField: "id", dataSource: { transport: { read: { url: urlGetTypes } } } });

    $.get(urlGetItems, {}, data => {
        if (data.message === "") {
            loadGrid(data.items);
            setGridHeight("Listado", _gridMargin);
        } else {
            showError("Se ha producido un error al traer los datos del servidor, por favor intente nuevamente.");
        }
    });

    $.get(urlGetUsers, {}, data => {
        if (data.message === "") {
            if (data.allowed) {
                $("#ListadoUsuarios").removeClass("d-none");
                $("#not-allowed").addClass("d-none");
                loadGridUsers(data.users);
            }
        } else {
            showError("Se ha producido un error al traer los datos del servidor, por favor intente nuevamente.");
        }
    });
}

function loadGrid(items) {
    var grid = $("#Listado").data("kendoGrid");
    grid.setDataSource(new kendo.data.DataSource({ data: items }));
}

function loadDetail(item) {
    $("#Id").val(item.id);
    $("#Name").val(item.name);
    $("#EMail").val(item.eMail);
    $("#Type").data("kendoDropDownList").value(item.type);
}

function loadGridUsers(items) {
    items.forEach(x => {
        x.lastGeneratedTokenDate = JSON.toDate(x.lastGeneratedTokenDate);
        x.logDate = JSON.toDate(x.logDate);
    });
    var grid = $("#ListadoUsuarios").data("kendoGrid");
    grid.setDataSource(new kendo.data.DataSource({ data: items }));
}

function loadDetailUser(item) {
    $("#user-data").val(JSON.stringify(item));
    $("#user-name").val(item.login);
    $("#password").val(item.password);
    $("#enabled").prop("checked", item.enabled);
}

//#endregion