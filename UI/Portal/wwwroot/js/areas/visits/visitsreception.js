//#region Global Variables

var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };

//#endregion

//#region Events

$(function () {
	setupControls();
	filterData(true);
});

$(window).resize(function () { setGridHeight("Listado", _gridMargin); });

$("#action-clean").click((e) => { cleanFilters(); });

$("#action-filter").click((e) => { filterData(false); });

$("#Listado").on("click", ".action-new", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: 0 } });
	wnd.open();
});

$("#Listado").on("click", ".action-edit", function (e) {
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var item = grd.dataItem(row);
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: item.id } });
	wnd.open();
});

$("#Listado").on("click", ".finish-visit", function () {
	var grid = $("#Listado").getKendoGrid();
	var row = $(this).closest("tr");
	grid.select(row);
	var item = grid.dataItem(row);
	$.post(urlFinish, { Id: item.id }, function (data) {
		if (data.message === "") {
			cleanFilters();
		} else {
			showError("Se ha producido un error al terminar la visita, por favor intente nuevamente.");
		}
	});
});

$("#Detail").on("click", ".save-visit", function (e) {
	e.preventDefault();
	var objConfig = {
		rules: {
			client: function (input) {
				if (input.is("[name=ClientDescription]")) {
					return !($("#CardCode").getKendoDropDownList().value() == "CVAR-001" & input.val() == "");
				}
				return true;
			},
			securityCard: function (input) {
				if (input.is("[name=SecurityCardId]")) {
					return input.val() > 0;
				}
				return true;
			},
			reason: function (input) {
				if (input.is("[name=ReasonVisit]")) {
					return !($("#IdReason").getKendoDropDownList().value() == "55" & input.val() == "");
				}
				return true;
			}
		},
		messages: { required: "*", client: "Se requiere aclaración de nombre.", securityCard: "Debe ser un número válido de tarjeta", reason: "Debe aclarar el motivo." }
	};
	var form = $(this).closest("form");
	var validator = form.kendoValidator(objConfig).data("kendoValidator");
	if (validator.validate()) {
		var visit = form.serializeObject();
		$.post(urlEdit, { Item: visit }, function (data) {
			if (data.message === "") {
				cleanFilters();

				$("#Detail").getKendoWindow().close();
			} else {
				showError("Se ha producido un error al guardar la visita, por favor intente nuevamente.");
			}
		});
	}
});

$("#newVisitor").on("click", ".save-visitor", function (e) {
	e.preventDefault();
	var form = $(this).closest("form");
	var validator = form.kendoValidator().data("kendoValidator");
	if (validator.validate()) {
		var docId, firstName, lastName, phone;
		docId = $("#newVisitor").find("#DocumentId").val();
		firstName = $("#newVisitor").find("#FirstName").val();
		lastName = $("#newVisitor").find("#LastName").val();
		phone = $("#newVisitor").find("#Phone").val();

		$.post(urlAddVisitor, { DocumentId: docId, FirstName: firstName, LastName: lastName, Phone: phone }, function (data) {
			if (data.message === "") {
				var widget = $("#VisitorId").getKendoDropDownList();
				var dataSource = widget.dataSource;
				dataSource.add({ id: data.id, name: docId + " - " + firstName + " " + lastName });
				dataSource.one("sync", function () { widget.select(dataSource.view().length - 1); });
				dataSource.sync();

				widget.close();
				widget.value(data.id);

				$("#newVisitor").getKendoWindow().close();
			} else {
				showError("Se ha producido un error al guardar el visitante, por favor intente nuevamente.");
			}
		}, "json");
	}

});

//#endregion

//#region Methods

function setupControls() {
	$("#FilClient").kendoDropDownList({
		dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente", filter: "contains",
		dataSource: { transport: { read: { url: urlClients } } },
		virtual: {
			itemHeight: 26, valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.code === options.value));
				options.success(index);
			}
		}
	});
	$("#FilStaff").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una persona", filter: "contains",
		dataSource: { transport: { read: { url: urlStaff } } }
	});
	var today = new Date();
	var filSince = $("#FilSince").kendoDatePicker({
		value: today, change: function (e) {
			var startDate = this.value();
			if (startDate === null) this.value("");
			filUntil.min(startDate ? startDate : _minDate);
		}
	}).data("kendoDatePicker");
	var filUntil = $("#FilUntil").kendoDatePicker({
		value: today, change: function (e) {
			var endDate = this.value();
			if (endDate === null) this.value("");
			filSince.max(endDate ? endDate : _maxDate);
		}
	}).data("kendoDatePicker");

	_maxDate = filUntil.max();
	_minDate = filSince.min();

	$("#Listado").kendoGrid({
		sortable: true, noRecords: { template: "No hay resultados para el criterio de búsqueda." },
		columns: [
			{ title: "Tarjeta", attributes: alignCenter, headerAttributes: alignCenter, width: 80, field: "securityCardId" },
			{ title: "Cliente", width: 180, field: "clientName" },
			{ title: "Nombre", width: 180, field: "visitor.fullName" },
			{ title: "Doc. Identidad", width: 120, field: "visitor.documentId" },
			{ title: "Personal Visitado", width: 220, field: "staff.fullName" },
			{ title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "initialDate", format: dateFormat },
			{ title: "Ingreso", attributes: alignCenter, headerAttributes: alignCenter, width: 80, field: "initialDate", format: "{0:HH:mm:ss}" },
			{ title: "Salida", attributes: alignCenter, headerAttributes: alignCenter, width: 130, template: '# if(finalDate) {# #=kendo.toString(finalDate, "HH:mm:ss")# #} else {# #if(editable) {# <a class="finish-visit k-button">Marcar Salida</a> #}# #} #', field: "finalDate" },
			{
				field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-new" title="Nueva Visita"></i>',
				template: '<i class="fas fa-pen action action-edit" title="Editar Visita"></i>'
			}
		],
		selectable: "Single, Row",
		"dataSource": []
	});

	$("#Detail").kendoWindow({
		visible: false, width: 1050, title: "Visita", modal: true, close: onCloseWindow,
		refresh: function (e) {
			setupControlsDetail();
			onRefreshWindow(e);
		}
	});

}

function getFilters() {
	var message = "", cardCode = $("#FilClient").data("kendoDropDownList").value(), visitor = $("#FilVisitor").val(), staff = $("#FilStaff").data("kendoDropDownList").value(),
		initialDate = $("#FilSince").data("kendoDatePicker").value(), finalDate = $("#FilUntil").data("kendoDatePicker").value(), notFinished = $("#NotFinished").prop("checked");
	if (initialDate) initialDate = initialDate.toISOString();
	if (finalDate) finalDate = finalDate.toISOString();
	if (initialDate === "" & finalDate === "" & cardCode === "" & visitor === "" & staff === "") {
		message = "Debe escoger algún criterio de búsqueda.";
	}
	return { message: message, data: { CardCode: cardCode, Visitor: visitor, Staff: staff, Since: initialDate, Until: finalDate, NotFinished: notFinished, _: new Date() } };
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
	items.forEach(x => {
		x.initialDate = JSON.toDate(x.initialDate);
		x.finalDate = JSON.toDate(x.finalDate);
	});
	var grd = $("#Listado").getKendoGrid();
	var ds = new kendo.data.DataSource({ data: items });
	grd.setDataSource(ds);

	var margin = _gridMargin;
	if (items && items.length > 0) {
		$('#filter-box').collapse("hide");
		margin -= 20;
	}
	setTimeout(() => { setGridHeight("Listado", margin) }, 200);
}

function cleanFilters() {
	var today = new Date();
	$("#FilClient").data("kendoDropDownList").value("");
	$("#FilVisitor").val("");
	$("#FilStaff").data("kendoDropDownList").value("")
	var ddlSince = $("#FilSince").data("kendoDatePicker"), ddlUntil = $("#FilUntil").data("kendoDatePicker");
	ddlUntil.min(today);
	ddlSince.max(today);
	ddlSince.value(today);
	ddlUntil.value(today);
	$("#NotFinished").prop("checked", true);
	filterData(false);
}

function setupControlsDetail() {
	$("#CardCode").kendoDropDownList({
		dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente", filter: "contains",
		dataSource: { transport: { read: { url: urlClients } } },
		virtual: {
			itemHeight: 26, valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.code === options.value));
				options.success(index);
			}
		},
		change: function () {
			var label = $("label[for='ClientDescription']");
			var div = $("#ClientDescription").parent();

			//Si es "Varios" muestra el campo de aclaración de nombre, sino lo oculta
			if (this.value() == "CVAR-001") {
				label.removeClass("d-none");
				div.removeClass("d-none");
			} else {
				label.addClass("d-none");
				div.addClass("d-none");
			}
		}
	});
	$("#VisitorId").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", noDataTemplate: $("#noDataTemplate").html(), optionLabel: "Seleccione una persona", filter: "contains",
		dataSource: { transport: { read: { url: urlVisitors } } },
		virtual: {
			itemHeight: 26,
			valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.id === +options.value));
				options.success(index);
			}
		}
	});
	$("#StaffId").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una persona", filter: "contains",
		dataSource: { transport: { read: { url: urlStaff } } }
	});
	$("#IdReason").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un motivo", filter: "contains",
		dataSource: { transport: { read: { url: urlReasons } } },
		change: function (e) {
			if (this.value() === "55") {
				$("#ReasonVisit").removeClass("d-none");
			} else {
				$("#ReasonVisit").addClass("d-none");
			}
		}
	});
}

function addNewVisitor() {
	$("#VisitorId").getKendoDropDownList().close();
	var wnd = $("#newVisitor").kendoWindow({ width: 900, title: "Nuevo Visitante", visible: false, modal: true }).data("kendoWindow");
	var item = {};
	var detailsTemplate = kendo.template($("#newVisitorTemplate").html());
	wnd.content(detailsTemplate(item));
	wnd.open().center();
}

//#endregion