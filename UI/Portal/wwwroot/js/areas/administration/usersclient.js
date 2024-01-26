//#region Variables Globales
var newId = 0;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Eventos

$(() => {
	setupControls();
	setTimeout(function () { setGridHeight("Listado", _gridMargin) }, 800);
	filterData(true);
});

$(window).resize(function () {
	setGridHeight("Listado", _gridMargin);
});

$('#filter-box').on('hidden.bs.collapse', function () {
	setGridHeight("Listado", _gridMargin);
});

$('#filter-box').on('shown.bs.collapse', function () {
	setGridHeight("Listado", _gridMargin);
});

$("#action-clean").click(function () { cleanFilters(); });

$("#action-filter").click(function () { filterData(false); });

$("#Listado").on("click", ".action-new", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: 0 } });
	wnd.center().open();
});

$("#Listado").on("click", ".action-edit", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var item = grd.dataItem(row);

	wnd.refresh({ url: urlEdit, data: { Id: item.id } });
	wnd.center().open();
});

$("#Listado").on("click", ".action-delete", function (e) {
	var dataItem = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
	showConfirm(`¿Está seguro que desea eliminar el Usuario <b>${dataItem.name}</b>?`, function () {
		var filtersData = getFilters();
		$.post(urlDelete, { Id: dataItem.id, Filters: filtersData.data }, function (data) {
			if (data.message === "") {
				loadGrid(data.items);
				showMessage(`Se ha eliminado el Usuario <b>${dataItem.name}</b> correctamente.`);
			} else {
				showError(data.message);
			}
		});
	});
});

$("#Listado").on("click", ".action-send-email", function (e) {
	var item = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
	$.post(urlSendEmail, { Id: item.id, WithCopy: false }, function (data) {
		if (data.message !== "") {
			showError(data.message);
		} else {
			showMessage("Se ha enviado el correo exitosamente.");
		}
	});
});

function onRefresh(e) {
	$("#IdProfile").kendoDropDownList({
		cascadeFrom: "CardCode", cascadeFromField: "cardCode", dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Perfil ...",
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
	});
	onRefreshWindow(e);
}

$("#Detail").on("click", "#action-cancel", function (e) {
	$("#Detail").data("kendoWindow").close();
});

$("#Detail").on("click", "#action-save", function (e) {
	var form = $(this).closest("form");
	var validator = form.kendoValidator().data("kendoValidator");
	if (validator.validate()) {
		saveUser();
	}
});

//#endregion

//#region Metodos Privados

function setupControls() {

	$("#filProfiles").kendoDropDownList({
		cascadeFrom: "filClients", cascadeFromField: "cardCode", dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Perfil ...",
		dataSource: { transport: { read: { url: urlProfiles, data: function (e) { return { CardCode: $("#CardCode").val() }; } } }, serverFiltering: true }
	});

	$("#Listado").kendoGrid({
		noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
		columns: [
			{ title: "Client Name", hidden: true, field: "clientName", groupHeaderTemplate: "Cliente: #= value #" },
			{ title: "Profile Name", hidden: true, field: "profileName", groupHeaderTemplate: "Perfil: #= value #" },
			{ title: "Nombre", field: "name" },
			{ title: "E-Mail", field: "eMail" },
			{ title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled" },
			{ title: " ", width: 35, template: '# if(enabled && eMail && eMail.length > 0) {# <a title="Enviar Datos de Acceso por Correo" class="action-send-email action action-link"><i class="fas fa-envelope"></i></a> #} #', field: "eMail", sortable: false },
			{
				field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-new" title="Nuevo Usuario"></i>',
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
}

function cleanFilters() {
	$("#filProfiles").getKendoDropDownList().value("");
	$("#filProfiles").getKendoDropDownList().text("");
	$("#filName").val("");
}

function getFilters() {
	var message = "", cardCode = $("#CardCode").val(), name = $("#filName").val(), profile = $("#filProfiles").getKendoDropDownList().value();
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
		if (!pageLoad) {
			setGridHeight("Listado", _gridMargin);
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

//#endregion