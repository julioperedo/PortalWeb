//#region Variables Globales
var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Eventos

$(function () {
	setupControls();
});

$("#filter-mails").keypress(function (e) {
	if (e.which == 13) {
		filterMails();
	}
});

$("#action-filter-mails").click(function (e) {
	filterMails();
});

$("#new-mail").keypress(function (e) {
	if (e.which == 13) {
		addMail();
	}
});

$("#add-mail").click(function (e) {
	addMail();
});

$("#action-save-clients").click(function (e) {
	var lstIds = $("#clients").getKendoMultiSelect().value();
	if (lstIds.length > 0) {
		$.post(urlSaveClients, { Clients: lstIds }, function (data) {
			if (data.message === "") {
				showMessage("Se realizaron los cambios correctamente.");
			} else {
				showError(`Se ha producido el siguiente error al intentar guardar los datos: <br />${data.message}`);
			}
		});
	} else {
		showMessage("Debe seleccionar algún Cliente para Guardar.")
	}
});

//#endregion

//#region Metodos Privados

function setupControls() {
	var clients = $("#clients").kendoMultiSelect({
		filter: "contains", dataTextField: "name", dataValueField: "code", noDataTemplate: "No hay resultados para el criterio de búsqueda.",
		virtual: {
			itemHeight: 26, valueMapper: function (options) {
				var items = this.dataSource.data();
				var indexs = [];
				options.value.forEach((x) => {
					indexs.push(items.indexOf(items.find(i => i.code === x)));
				});
				options.success(indexs);
			}
		},
		dataSource: { transport: { read: urlClients } }
	}).data("kendoMultiSelect");

	$("#emails").kendoGrid({
		noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }, height: 300,
		columns: [
			{ title: "Correo Electrónico", field: "eMail" },
			{ title: "Agregado por", field: "userName" },
			{ title: "Agregado el", field: "date", format: "{0:dd-MM-yyyy HH:mm}", width: 120 }
		],
		sortable: true, selectable: "Single, Row", dataSource: []
	});

	$.get(urlClientsListed, {}, function (data) {
		if (data.message === "") {
			if (data.items) {
				clients.value(data.items);
			}
		} else {
			showError("Se ha producido un error al traer los datos del servidor.");
		}
	});

}

function filterMails() {
	var filter = $.trim($("#filter-mails").val());
	if (filter === "") {
		showInfo("Debe ingresar algún criterio de búsqueda.");
	} else {
		$.get(urlFilterMails, { Filter: filter }, function (data) {
			if (data.message === "") {
				loadGrid(data.items);
			} else {
				loadGrid([]);
				showError(`Se ha producido el siguiente error al traer los datos: <br />${data.message}`);
			}
		});
	}
}

function loadGrid(items) {
	if (items) {
		items.forEach(x => x.date = JSON.toDate(x.date));
		var grd = $("#emails").data("kendoGrid");
		var ds = new kendo.data.DataSource({ data: items });
		grd.setDataSource(ds);
	}
}

function addMail() {
	var form = $("#form-add-mail");
	var validator = form.kendoValidator({ messages: { required: "Es requerido", email: "No es un correo electrónico válido" } }).data("kendoValidator");
	if (validator.validate()) {
		var email = $("#new-mail").val();
		var grid = $("#emails").data("kendoGrid");
		var newDate = new Date();
		$.post(urlAddMail, { EMail: email }, function (data) {
			if (data.message === "") {
				showMessage(`Se ha agregado el correo <b>${email}</b> a la lista negra.`);
				grid.dataSource.add({ EMail: email, UserName: $.trim($(".user-info > .user-name").first().text()), Date: newDate });
			} else {
				if (data.error === "Y") {
					showError(`Se ha producido el siguiente error al tratar de guardar el correo electrónico: <br />${data.message}`);
				} else {
					showInfo(data.message);
				}
			}
		});
	}
}

//#endregion