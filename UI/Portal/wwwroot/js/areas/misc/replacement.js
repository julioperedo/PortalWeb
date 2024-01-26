//#region Global Variables

var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy HH:mm:ss}", _gridMargin = 30;

//#endregion

//#region Events

$(function () {
	setupControls();
	setGridHeight("Listado", _gridMargin);
});

$(window).resize(() => { setGridHeight("Listado", _gridMargin); });

$("#action-filter").click(() => filterData());

$("#action-clean").click(() => {
	$("#FilInitialDate").data("kendoDatePicker").value(new Date());
	$("#FilFinalDate").data("kendoDatePicker").value(new Date());
	if ($("#form-type").val() === "C") { //Formulario general, con permisos
		$("#FilSeller").data("kendoDropDownList").value("");
		$("#FilReplacement").data("kendoDropDownList").value("");
	} else { //formulario para cada vendedor
		$("#FilType").data("kendoDropDownList").value("X");
	}
	loadGrid([]);
});

$("#Listado").on("click", ".new-item", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: 0 } });
	wnd.open();
});

$("#Listado").on("click", ".action-edit", function (e) {
	e.preventDefault();
	var wnd = $("#Detail").data("kendoWindow");
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");

	grd.select(row);
	var item = grd.dataItem(row);
	wnd.refresh({ url: urlEdit, data: { Id: item.id } });
	wnd.open();
});

$("#Listado").on("click", ".action-delete", function (e) {
	var grd = $("#Listado").data("kendoGrid");
	var item = grd.dataItem($(this).closest("tr"));
	var filters = getFilters();
	showConfirm(`¿Está seguro que desea eliminar el remplazo de <b>${item.sellerName}</b> a <b>${item.replacementName}</b>?`, () => {
		$.post(urlDelete, { Id: item.id, Filters: filters }, function (data) {
			if (data.message === "") {
				loadGrid(data.items);
				showMessage(`Se ha eliminado el remplazo de <b>${item.sellerName}</b> a <b>${item.replacementName}</b> correctamente.`);
			} else {
				showError(data.message);
			}
		});
	});
});

$("#Detail").on("click", "#save-item", function (e) {
	e.preventDefault();
	var form = $(this).closest("form");
	var validator = form.kendoValidator().data("kendoValidator");
	if (validator.validate()) {
		var filters = getFilters();
		var item = form.serializeObject();
		var valid = true, sellerCode = $("#CurrentSeller").val();
		if (sellerCode) {
			if (item.SellerCode !== sellerCode && item.ReplaceCode !== sellerCode) {
				valid = false;
			}
		}
		if (valid) {
			$.post(urlEdit, { Item: item, Filters: filters }, function (data) {
				if (data.message === "") {
					loadGrid(data.items);
					showMessage("Datos guardados correctamente.");
					$("#Detail").data("kendoWindow").close();
				} else {
					showError(data.message);
				}
			});
		} else {
			showInfo("Ud. debe ser unos de los dos (remplazante o remplazado).");
		}
	}
});

//#endregion

//#region Methods

function setupControls() {
	var today = new Date();
	var filSince = $("#FilInitialDate").kendoDatePicker({
		format: "d/M/yyyy", value: today, change: function (e) {
			var startDate = this.value();
			if (startDate === null) this.value("");
			filUntil.min(startDate ? startDate : _minDate);
		}
	}).data("kendoDatePicker");
	var filUntil = $("#FilFinalDate").kendoDatePicker({
		format: "d/M/yyyy", value: today, change: function (e) {
			var endDate = this.value();
			if (endDate === null) this.value("");
			filSince.max(endDate ? endDate : _maxDate);
		}
	}).data("kendoDatePicker");

	_maxDate = filUntil.max();
	_minDate = filSince.min();

	if ($("#form-type").val() === "C") {
		$("#FilSeller").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", optionLabel: "Seleccione un Ejecutivo...", dataSource: { transport: { read: { url: urlSellers } } } });
		$("#FilReplacement").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", optionLabel: "Seleccione un Remplazo...", dataSource: { transport: { read: { url: urlSellers } } } });
	} else {
		$("#FilType").kendoDropDownList({ dataTextField: "Text", dataValueField: "Value", value: "X", dataSource: [{ Text: "Como Reemplazado", Value: "X", Selected: true }, { Text: "Como Reemplazante", Value: "Y" }] });
	}
	
	$("#Listado").kendoGrid({
		sortable: true, noRecords: { template: "No hay resultados para el criterio de búsqueda." }, selectable: "Single, Row",
		columns: [
			{ title: "Nombre", field: "sellerName" },
			{ title: "Reemplazo", field: "replacementName" },
			{ title: "Desde", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "initialDate", format: "{0:dd/MM/yyyy}" },
			{ title: "Hasta", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "finalDate", format: "{0:dd/MM/yyyy}" },
			{
				field: "id", title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action new-item" title="Nuevo Rempazo"></i>',
				template: '<i class="fas fa-pen action action-edit" title="Editar Remplazo"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-delete" title="Eliminar Remplazo"></i>'
			}
		],
		dataSource: []
	});

	$("#Detail").kendoWindow({
		scrollable: true, visible: false, width: 900, actions: ["Close"], resizable: false, title: "Reemplazo", modal: true, refresh: (e) => {
			var since = $("#InitialDate").kendoDatePicker({
				format: "d/M/yyyy", change: function (e) {
					var startDate = this.value();
					if (startDate === null) this.value("");
					until.min(startDate ? startDate : _minDate);
				}
			}).data("kendoDatePicker");
			var until = $("#FinalDate").kendoDatePicker({
				format: "d/M/yyyy", change: function (e) {
					var endDate = this.value();
					if (endDate === null) this.value("");
					since.max(endDate ? endDate : _maxDate);
				}
			}).data("kendoDatePicker");
			$("#SellerCode").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", optionLabel: "Seleccione un Ejecutivo...", dataSource: { transport: { read: { url: urlSellers } } } });
			$("#ReplaceCode").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", optionLabel: "Seleccione un Remplazo...", dataSource: { transport: { read: { url: urlSellers } } } });

			onRefreshWindow(e);
		}
	});
}

function getFilters() {
	var initialDate = $("#FilInitialDate").data("kendoDatePicker").value(), finalDate = $("#FilFinalDate").data("kendoDatePicker").value(), seller = "", replacement = "";
	if (initialDate) { initialDate = initialDate.toISOString(); }
	if (finalDate) { finalDate = finalDate.toISOString(); }
	if ($("#form-type").val() === "C") { //Formulario general, con permisos
		seller = $("#FilSeller").data("kendoDropDownList").value(), replacement = $("#FilReplacement").data("kendoDropDownList").value();
	} else { //formulario para cada vendedor
		if ($("#FilType").data("kendoDropDownList").value() === "X") {
			seller = $("#CurrentSeller").val();
		} else {
			replacement = $("#CurrentSeller").val();
		}
	}
	return { InitialDate: initialDate, FinalDate: finalDate, SellerCode: seller, ReplacementCode: replacement };
}

function filterData() {
	var filters = getFilters();
	$.get(urlFilter, filters, function (data) {
		if (data.message === "") {
			loadGrid(data.items);
		} else {
			showError(data.message);
		}
	});
}

function loadGrid(items) {
	$.each(items, function (i, x) {
		x.initialDate = JSON.toDate(x.initialDate);
		x.finalDate = JSON.toDate(x.finalDate);
	});
	var grd = $("#Listado").data("kendoGrid");
	var ds = new kendo.data.DataSource({ data: items });
	grd.setDataSource(ds);

	var margin = _gridMargin;
	if (items && items.length > 0) {
		$('#filter-box').collapse("hide");
		margin -= 20;
	}
	setTimeout(() => { setGridHeight("Listado", margin) }, 200);
}

//#endregion