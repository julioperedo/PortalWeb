//#region Gobal variables
var _id = -1000, _wndDetail, _grdItems, _swEnabled, _grdDepartments, _ddDepartment, _ddManager;
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Events

$(() => setupPage());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData(false));

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-sync", onSync);

$("#Detail").on("click", ".action-cancel", onCancel);

$("#Detail").on("click", ".action-save", onSave);

$("#departments").on("click", ".action-new, .action-edit, .action-delete", onEditDepartment);

$("#department-detail").on("click", ".action-cancel-department", onCancelDepartment);

$("#department-detail").on("click", ".action-save-department", onSaveDepartment);

//#endregion

//#region Private Methods

function setupPage() {
    _grdItems = $("#Listado").kendoGrid({
        noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Gerencia", field: "isManager", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: (e) => e.isManager ? '<i class="fas fa-check"></i>' : '' },
            { title: "Habilitado", field: "enabled", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: (e) => e.enabled ? '<i class="fas fa-check"></i>' : '' },
            { field: "photo", title: " ", attributes: alignCenter, width: 30, template: (e) => $.trim(e.photo) != "" ? '<i class="fas fa-image" title="Tiene Imagen"></i>' : '' },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a title="Sincronizar empleados" class="action action-link action-sync"><i class="fas fa-sync"></i></a>',
                template: '<a title="Editar Area de Trabajo" class="action action-link action-edit"><i class="fas fa-pen"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row",
        dataSource: getDataSource([])
    }).data("kendoGrid");
    filterData(true);

    _grdDepartments = $("#departments").kendoGrid({
        noRecords: { template: '<div class="text-center w-100 p-2">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { title: "Area de Trabajo", field: "department" },
            { title: "Inmediato Superior", field: "manager" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a title="Sincronizar empleados" class="action action-link action-new"><i class="fas fa-plus"></i></a>',
                template: '<a title="Editar Area de Trabajo" class="action action-link action-edit"><i class="fas fa-pen"></i></a><a title="Eliminar Area de Trabajo" class="action action-link action-delete"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row",
        dataSource: getDSDepartment([])
    }).data("kendoGrid");

    _wndDetail = $("#Detail").kendoWindow({ width: 750, title: "Detalle de Empleado", visible: false, modal: true, activate: onRefreshWindow }).data("kendoWindow");
    _swEnabled = $("#enabled").kendoSwitch({ messages: { checked: "", unchecked: "" }, size: "small" }).data("kendoSwitch");
    _ddDepartment = $("#idDepartment").kendoDropDownList({
        dataSource: { transport: { read: { url: urlDepartments } } },
        optionLabel: "Seleccione un Area de Trabajo", filter: "contains", dataTextField: "name", dataValueField: "id"
    }).data("kendoDropDownList");
    _ddManager = $("#idManager").kendoDropDownList({
        optionLabel: "Seleccione el Inmediato Superior", filter: "contains", dataTextField: "name", dataValueField: "id",
        select: function (e) {
            if (!e.dataItem.enabled) {
                e.preventDefault();
            }
        },
        template: (e) => `<span class="${(e.enabled ? '' : 'k-disabled')}">${e.name}</span>`
    }).data("kendoDropDownList");
}

function getDataSource(items) {
    return new kendo.data.DataSource({ data: items, schema: { model: { id: "id" } } });
}

function getDSDepartment(items) {
    return new kendo.data.DataSource({ data: items, schema: { model: { id: "id" } }, filter: [{ field: "statusType", operator: "neq", value: "3" }] });
}

function cleanFilters() {
    $("#filter").val("");
}

function getFilters() {
    var message = "", filter = $("#filter").val();
    return { message: message, data: { FilterData: filter } };
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
        setGridHeight("Listado", _gridMargin);
        if (!pageLoad) {
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
}

function loadGrid(items) {
    if (items) {
        var ds = getDataSource(items);
        _grdItems.setDataSource(ds);
        if (items && items.length > 0) {
            $('#filter-box').collapse("hide");
            $("#action-excel").removeClass("d-none");
        } else {
            $("#action-excel").addClass("d-none");
        }
        setTimeout(() => setGridHeight("Listado", _gridMargin), 100);
    }
}

function onEdit(e) {
    e.preventDefault();
    var item;
    if (this.classList.contains("action-new")) {
        _grdItems.clearSelection();
        item = { id: 0, name: "", enabled: true };
    } else {
        var row = $(e.currentTarget).closest("tr");
        _grdItems.select(row);
        item = _grdItems.dataItem(row);
    }

    $.get(urlEdit, { Id: item.id }, function (d) {
        _ddManager.setDataSource(d.managers);
        _grdDepartments.setDataSource(getDSDepartment(d.items));
    });

    $("#id").val(item.id);
    $("#photo").val(item.photo);
    $("#name").val(item.name);
    $("#shortName").val(item.shortName);
    $("#position").val(item.position);
    $("#hierarchyLevel").val(item.hierarchyLevel);
    $("#mail").val(item.mail);
    $("#phone").val(item.phone);
    _swEnabled.check(item.enabled);
    $("#picture").attr("src", $.trim(item.photo) == "" ? urlNoPhoto : urlPath + item.photo);
    $("#department-detail").addClass("d-none");

    _wndDetail.center().open();
}

function onCancel(e) {
    e.preventDefault();
    _wndDetail.close();
}

function onSave(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "Campo requerido" } }).data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        var departments = _grdDepartments.dataSource.data();
        var items = departments.map((x) => Object.assign({}, { id: x.id, idEmployee: +item.id, idDepartment: x.idDepartment, idManager: x.idManager, statusType: x.statusType }));

        $.post(urlEdit, { Item: item, Departments: items }, function (d) {
            if (d.message == "") {
                _wndDetail.close();
                showMessage("Se realizaron los cambios correctamente.");
                _grdItems.dataSource.pushUpdate(item);
            } else {
                showError(d.message);
            }
        });
    }
}

function onSync(e) {
    e.preventDefault();
    $.post(urlSync, {}, function (d) {
        if (d.message == "") {
            showMessage("Se han empleado los empleados exitosamente.");
            _grdItems.setDataSource(d.items);
        } else {
            console.error(d.message);
            showError("Se ha producido un error al sincronizar los empleados.");
        }
    });
}

function onEditDepartment(e) {
    e.preventDefault();
    var item;
    if (this.classList.contains("action-new") || this.classList.contains("action-edit")) {
        if (this.classList.contains("action-new")) {
            _grdDepartments.clearSelection();
            item = { id: _id++, idDepartment: 0, idManager: 0 };
        }
        if (this.classList.contains("action-edit")) {
            var row = $(e.currentTarget).closest("tr");
            _grdDepartments.select(row);
            item = _grdDepartments.dataItem(row);
        }
        $("#id-detail").val(item.id);
        _ddDepartment.value(item.idDepartment);
        _ddManager.value(item.idManager);

        $("#department-detail").removeClass("d-none");
        setRequiredToControls(true);
        _wndDetail.center();
    }
    if (this.classList.contains("action-delete")) {
        var row = $(e.currentTarget).closest("tr");
        item = _grdDepartments.dataItem(row);
        showConfirm(`¿Está seguro que desea eliminar el Area de Trabajo <b>${item.department}</b>?`, function () {
            if (item.id > 0) {
                item.statusType = 3;
                _grdDepartments.dataSource.pushUpdate(item);
            } else {
                _grdDepartments.dataSource.remove(item);
            }
        });
    }
}

function setRequiredToControls(required) {
    if (required) {
        $('#idDepartment, #idManager').attr('required', 'required');
    } else {
        $('#idDepartment, #idManager').removeAttr('required');
    }
}

function onCancelDepartment(e) {
    e.preventDefault();
    $("#department-detail").addClass("d-none");
    setRequiredToControls(false);
    _wndDetail.center();
}

function onSaveDepartment(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "Campo requerido" } }).data("kendoValidator");
    if (validator.validate()) {
        var item = { id: +$("#id-detail").val(), idDepartment: +_ddDepartment.value(), department: _ddDepartment.text(), idManager: +_ddManager.value(), manager: _ddManager.text(), statusType: +$("#id-detail").val() > 0 ? 2 : 1 };
        _grdDepartments.dataSource.pushUpdate(item);
        $("#department-detail").addClass("d-none");
        _wndDetail.center();
    }
}

//#endregion