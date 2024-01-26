//#region Variables Globales
var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Eventos

$(function () {
	setupControls();
});

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(function (e) {
	$("#filClients").multipleSelect("uncheckAll");
		$("#filSince").data("kendoDatePicker").value(""), $("#filUntil").data("kendoDatePicker").value("");
});

$("#action-filter").click(function (e) {
	var filters = getFilters();
	if (filters.message === "") {
		$.get(urlFilter, filters.data, function (data) {
			loadGrid(data.items);
			if (data.message !== "") {
				showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
			}
		});
	} else {
		showInfo(`Los siguientes campos son requeridos: <br />${filters.message}`);
	}
});

$("#action-excel").click(function (e) {
	var filters = getFilters();
	if (filters.message === "") {
		window.location.href = urlExcel + "?" + $.param(filters.data);
	} else {
		showInfo(`Los siguientes campos son requeridos: <br />${filters.message}`);
	}
});

//#endregion

//#region Metodos Privados

function setupControls() {
	var filClients = $("#filClients"), filUsers = $("#filUsers");
	if ($("#local-user").val() === "Y") {
		filClients.multipleSelect({
			filter: true, minimumCountSelected: 1, selectAll: false,
			onUncheckAll: () => filUsers.empty().multipleSelect("refresh").multipleSelect("disable"),
			onClick: (view) => loadUsers(filClients, filUsers)
		});
	} else {
		loadUsers(filClients, filUsers);
	}
	filUsers.multipleSelect();

	var filSince = $("#filSince").kendoDatePicker({
		change: function () {
			var startDate = this.value();
			if (startDate === null) this.value("");
			filUntil.min(startDate ? startDate : _minDate);
		}
	}).data("kendoDatePicker");

	var filUntil = $("#filUntil").kendoDatePicker({
		change: function () {
			var endDate = this.value();
			if (endDate === null) this.value("");
			filSince.max(endDate ? endDate : _maxDate);
		}
	}).data("kendoDatePicker");

	_maxDate = filUntil.max();
	_minDate = filSince.min();

	$("#Listado").kendoGrid({
		noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
		columns: [
			{ title: "Cliente", hidden: true, field: "clientCode", aggregates: ["count"], groupHeaderTemplate: 'Cliente: #= value #    ( Total: #=count# )' },
			{ title: "Nombre", width: 250, field: "userName", aggregates: ["count"], groupHeaderTemplate: 'Usuario: #= value #    ( Total: #=count# )' },
			{ title: "Descripción", width: 200, field: "description" },
			{ title: "Fecha", width: 120, field: "logDate", format: "{0:dd/MM/yyyy HH:mm}" }
		],
		sortable: true, selectable: "Single, Row", dataSource: []
	});

	if ($("#local-user").val() === "Y") {
		$.get(urlClients, {}, function (data) {
			data.forEach(x => {
				filClients.append(new Option(x.name, x.code, false));
			});
			filClients.multipleSelect("refresh").multipleSelect("uncheckAll");
		});
	}
	setGridHeight("Listado", _gridMargin);
}

function loadUsers(clients, users) {
	var objSelected = $("#local-user").val() === "Y" ? clients.multipleSelect("getSelects") : [clients.val()];
	if (objSelected && objSelected.length > 0) {
		var codes = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
		$.get(urlUsers, { CardCode: codes }, function (data) {
			if (data.message !== "") {
				showError(data.message);
				users.multipleSelect("disable");
			} else {
				users.empty();
				if (data.items.length > 0) {
					data.items.forEach(x => {
						var opt = $("<optgroup>").attr("label", x.client);
						x.users.forEach(i => opt.append(new Option(i.name, i.id)));
						users.append(opt);
					});
					users.multipleSelect("enable");
				} else {
					users.multipleSelect("disable");
				}
				users.multipleSelect("refresh");
			}
			users.multipleSelect("uncheckAll");
		});
	} else {
		users.multipleSelect("disable");
	}
}

function getFilters() {
	var message = "", users = Enumerable.From($("#filUsers").multipleSelect('getSelects')).Select("$").ToArray().join(), since = $("#filSince").data("kendoDatePicker").value(), until = $("#filUntil").data("kendoDatePicker").value();
	if (users === "") {
		message = "- Debe seleccionar al menos un usuario.<br />";
	}
	if (since === null && until === null) {
		message += "- Debe seleccionar al menos una fecha.";
	} else {
		if (since) {
			since = since.toISOString();
		}
		if (until) {
			until = until.toISOString();
		}
	}
	return { message: message, data: { Users: users, Since: since, Until: until } };
}

function loadGrid(items) {
	if (items) {
		items.forEach(x => { x.logDate = JSON.toDate(x.logDate) });
		var grd = $("#Listado").data("kendoGrid");
		var ds = new kendo.data.DataSource({
			data: items,
			group: [
				{ field: "clientCode", dir: "asc", aggregates: [{ field: "clientCode", aggregate: "count" }] },
				{ field: "userName", dir: "asc", aggregates: [{ field: "userName", aggregate: "count" }] }
			], pageSize: 500,
			sort: [{ field: "logDate", dir: "desc" }],
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

//#endregion