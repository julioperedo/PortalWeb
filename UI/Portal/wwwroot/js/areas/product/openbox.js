//#region Global Variables

const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy HH:mm:ss}", _gridMargin = 30;

//#endregion

//#region Events

$(function () {
	setupControls();
	filterData();
});

$(window).resize(function () { setGridHeight("Listado", _gridMargin); });

$("#action-search").click(function (e) {
	filterData();
});

$("#action-clean").click(() => {
	$("#FilSubsidiary").data("kendoDropDownList").value("");
	$("#FilProduct").val("");
	$("#FilEnabled").prop("checked", true);
	$("#FilWithStock").prop("checked", true);
	loadGrid([]);
});

$("#Listado").on("click", ".new-product", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: 0 } });
	wnd.open();
});

$("#Listado").on("click", ".action-edit", function (e) {
	var grd = $("#Listado").data("kendoGrid");
	var item = grd.dataItem($(this).closest("tr"));

	var wnd = $("#Detail").data("kendoWindow");
	wnd.setOptions({ title: "Detalle de Producto", width: 900 });
	wnd.refresh({ url: urlEdit, data: { Id: item.id } });
	wnd.open();
});

$("#Listado").on("click", ".action-delete", function (e) {
	var grd = $("#Listado").data("kendoGrid");
	var item = grd.dataItem($(this).closest("tr"));
	var filters = getFilters();
	showConfirm(`¿Está seguro que desea eliminar el producto <b>${item.itemCode}</b>?`, () => {
		$.post(urlDelete, { Id: item.id, Filters: filters }, function (data) {
			if (data.message === "") {
				loadGrid(data.items);
				showMessage(`Se ha eliminado el producto <b>${item.itemCode}</b> correctamente.`);
			} else {
				showError(data.message);
			}
		});
	});
});

$("#Detail").on("click", "#action-cancel", function () {
	$("#Detail").data("kendoWindow").close();
});

$("#Detail").on("click", "#action-save", function () {
	var objConfig = {
		rules: { amounts: (input) => input.is("#Quantity") || input.is("#Price") ? input.val() > 0 : true },
		messages: { required: "*", amounts: "Debe ser mayor a 0." }
	};
	var form = $(this).closest("form");
	var validator = form.kendoValidator(objConfig).data("kendoValidator");
	var wnd = $("#Detail").data("kendoWindow");
	var filters = getFilters();
	if (validator.validate()) {
		var openBox = form.serializeObject();
		$.post(urlEdit, { Item: openBox, Filters: filters }, function (data) {
			if (data.message === "") {
				showMessage("Sus datos se actualizaron correctamente.");
				wnd.close();
				loadGrid(data.items);
			} else {
				showError(data.message);
			}
		});
	} else {
		wnd.center();
	}
});

//#endregion

//#region Methods

function setupControls() {
	$("#FilSubsidiary").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Sucursal...", dataSource: { transport: { read: { url: urlSubsidiaries } } } });

	$("#Listado").kendoGrid({
		columns: [
			{ title: "Sucursal", width: "150px", field: "subsidiary" },
			{ title: "Almacén", width: "150px", field: "warehouse" },
			{ title: "Cod.", width: "200px", field: "itemCode" },
			{ title: "Producto", field: "itemName" },
			{ title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: "100px", field: "quantity" },
			{ title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: "100px", field: "price", format: "{0:N2}", groupable: false },
			{ title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: "100px", template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled" },
			{ title: "Con Stock", attributes: alignCenter, headerAttributes: alignCenter, width: "100px", template: '# if(withStock) {# <i class="fas fa-check"></i> #} #', field: "withStock" },
			{
				field: "id", title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action new-product" title="Nuevo Producto"></i>',
				template: '<i class="fas fa-pen action action-edit" title="Editar Producto"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-delete" title="Eliminar Producto"></i>'
			}
		],
		sortable: true, selectable: "Single, Row", noRecords: { template: "No existen resultados para el criterio de búsqueda." },
		dataSource: []
	});

	$("#Detail").kendoWindow({
		visible: false, modal: true, iframe: false, scrollable: true, title: "Producto Abierto", resizable: false, width: 800, actions: ["Close"], refresh: function (e) {
			$("#IdProduct").kendoDropDownList({
				dataTextField: "name", dataValueField: "id", ignoreCase: true, optionLabel: "Seleccione un Producto...", filter: "contains",
				virtual: {
					itemHeight: 26, valueMapper: function (options) {
						var items = $("#IdProduct").data("kendoDropDownList").dataSource.data();
						var item = Enumerable.From(items).Where(x => x.id === options.value).FirstOrDefault();
						var index = Enumerable.From(items).IndexOf(item);
						options.success(index);
					}
				},
				dataSource: { transport: { read: { url: urlProducts } } }
			});
			$("#IdSubsidiary").kendoDropDownList({ dataTextField: "name", dataValueField: "id", ignoreCase: true, optionLabel: "Seleccione una Sucursal...", dataSource: { transport: { read: { url: urlSubsidiaries } } } });
			$("#Warehouse").kendoDropDownList({
				autoBind: false, cascadeFrom: "IdSubsidiary", dataTextField: "name", dataValueField: "name", enable: false, ignoreCase: true, optionLabel: "Seleccione un Almacén...",
				dataSource: {
					transport: {
						read: {
							url: urlWarehouses, data: (e) => {
								return { Subsidiary: $("#IdSubsidiary").data("kendoDropDownList").text() };
							}
						}
					}, serverFiltering: true
				}
			});

			onRefreshWindow(e);
		}
	});

}

function getFilters() {
	var subsidiary = $("#FilSubsidiary").data("kendoDropDownList").value(), product = $("#FilProduct").val(), enabled = $("#FilEnabled").prop("checked"), withStock = $("#FilWithStock").prop("checked");
	return { IdSubsidiary: subsidiary, Product: product, Enabled: enabled, WithStock: withStock };
}

function filterData() {
	var filters = getFilters();
	$.get(urlFilter, filters, function (data) {
		if (data.message !== "") {
			showError(data.message);
			loadGrid([]);
		} else {
			loadGrid(data.items);
		}
	});
}

function loadGrid(items) {
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

function getWarehousesFilter(e) {
	return { Subsidiary: $("#IdSubsidiary").data("kendoDropDownList").text() };
}

//#endregion