//#region Global Variables

var _loaded = false, _reconfigured = false;
var _options = {
	pivot: {
		rows: ["GP", "Linea", "Marca", "Descripcion", "Item"],
		columns: ["Sucursal"],
		values: [
			{ field: "Stock" },
			{ field: "Costo" }
		],
		xfields: [
			{ field: "Sucursal", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Almacen", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "GP", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Linea", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Categoria", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Subcategoria", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Item", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Descripcion", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Marca", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
			{ field: "Stock", op: "sum", formatter: function (value) { return kendo.toString(+value, "#,##0"); } },
			{ field: "Costo", op: "sum", formatter: function (value) { return kendo.toString(+value, "#,##0.00"); } }
		]
	},
	forzenColumnTitle: "<span style=\"font-weight:bold\">Detalle</span>",
	valueFormatter: function (value) { return kendo.toString(+value, "#,##0.00"); },
	valuePrecision: 2,
	valueFieldWidth: 100,
	showFooter: true,
	FooterTitle: "TOTAL",
	showColumnsTotals: true,
	i18n: { fields: "Campos", filters: "Filtros", rows: "Filas", columns: "Columnas", values: "Valores", ok: "Aceptar", cancel: "Cancelar" },
	onDblClickRow: function (row) {
		$(this).pivotgrid("unselectAll");
	}
};

//#endregion

//#region Events

$(function () {
	setupControls();
});

$(window).resize(function () {
	var intHeight = $(window).height() - $(".navbar").height() - $(".toolbar").height() - 50;
	$("#pivot").pivotgrid("resize", { height: intHeight });
});

$("#action-config").click(function (e) {
	e.preventDefault();
	$('#pivot').pivotgrid('layout');
});

$("#action-clean").click(function (e) {
	$("#Subsidiary").multipleSelect("uncheckAll");
	$("#Category").multipleSelect("uncheckAll");
	$("#Line").multipleSelect("uncheckAll");
	$("#GP").data("kendoDropDownList").value("");

	loadPivot([]);
});

$("#action-filter").click(function (e) {
	var filters = getFilters();
	$("#pivot-container").empty();

	$.ajaxSetup({ cache: false });
	$.get(urlFilter, filters, function (data) {
		$('#filter-box').collapse("hide");
		if (data.message === "") {
			loadPivot(data.items);
		} else {
			showError(`Se ha producido el siguiente error al traer los datos: <br />${data.message}`);
		}
	});
});

$("#action-excel").click(function (e) {
	var filters = getFilters();
	window.location.href = urlExport + "?" + $.param(filters);
});

//#endregion

//#region Methods

function setupControls() {
	$("#Subsidiary").multipleSelect({
		placeholder: "Seleccione una Sucursal",
		onUncheckAll: () => { $("#Storehouse").empty().multipleSelect("refresh").multipleSelect("disable"); },
		onCheckAll: () => { loadStoreHouses(); },
		onClick: (view) => { loadStoreHouses(); }
	});
	$("#Storehouse").multipleSelect({ placeholder: "Seleccione algún Almacén" }).multipleSelect("disable");
	$("#Category").multipleSelect({ placeholder: "Seleccione alguna Categoría", onClick: function (view) { loadSubcategories(); }, onCheckAll: function () { loadSubcategories(); }, onUncheckAll: function () { loadSubcategories(); } });
	$("#SubCategory").multipleSelect({ placeholder: "Seleccione alguna Subcategoría" });
	$("#Line").multipleSelect({ placeholder: "Seleccione alguna Línea" });
	$("#GP").kendoDropDownList({ dataTextField: "name", dataValueField: "name", optionLabel: "Seleccione un Valor ...", dataSource: { transport: { read: { url: urlPManagers } } } });

	$.get(urlGetSubsidiaries, {}, (data) => {
		if (data.message === "") {
			var ddl = $("#Subsidiary");
			data.items.forEach((x) => {
				ddl.append(new Option(x.name, x.id, false, false));
			});
			ddl.multipleSelect("refresh");
		} else {
			showError(`Ha ocurrido el siguiente error al traer las Sucursales: <br />${data.message}.`);
		}
	});
	$.get(urlLines, {}, (data) => {
		if (data) {
			var ddl = $("#Line");
			data.forEach((x) => {
				ddl.append(new Option(x.name, x.id, false, false));
			});
			ddl.multipleSelect("refresh");
		}
	});
	$.get(urlCategories, {}, (data) => {
		if (data) {
			var ddl = $("#Category");
			data.forEach((x) => {
				ddl.append(new Option(x.name, x.id, false, false));
			});
			ddl.multipleSelect("refresh");
		}
	});

}

function loadStoreHouses() {
	$("#Storehouse").empty();
	var objSelected = $("#Subsidiary").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
		$.get(urlWarehouses, { Subsidiary: strData }, (data) => {
			if (data.message !== "") {
				showError(data.message);
				$("#Storehouse").multipleSelect("disable");
			} else {
				if (data.items.length > 0) {
					$.each(data.items, function (i, obj) {
						$("#Storehouse").append(new Option(obj.name, obj.name, true, true));
					});
					$("#Storehouse").multipleSelect("enable");
				} else {
					$("#Storehouse").multipleSelect("disable");
				}
			}
			$("#Storehouse").multipleSelect("refresh");
		});
	} else {
		$("#Storehouse").multipleSelect("refresh").multipleSelect("disable");
	}
}

function loadSubcategories() {
	$("#SubCategory").empty();
	var objSelected = $("#Category").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
		$.get(urlSubcategories, { Categories: strData }, (data) => {
			if (data.message !== "") {
				showError(data.message);
				$("#SubCategory").multipleSelect("disable");
			} else {
				if (data.items.length > 0) {
					$.each(data.items, function (i, obj) {
						$("#SubCategory").append(new Option(obj.name, obj.name, false, false));
					});
					$("#SubCategory").multipleSelect("enable");
				} else {
					$("#SubCategory").multipleSelect("disable");
				}
			}
			$("#SubCategory").multipleSelect("refresh");
		});
	} else {
		$("#SubCategory").multipleSelect("refresh").multipleSelect("disable");
	}
}

function getFilters() {
	var strRegionals = "", strWarehouses = "", strCategories = "", strSubcategories = "", strLines = "", strProductManager = "";
	var objSelected = $("#Subsidiary").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		strRegionals = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
	}
	objSelected = $("#Storehouse").multipleSelect("getSelects");
	if (objSelected && objSelected.length > 0) {
		strWarehouses = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
	}
	objSelected = $("#Category").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		strCategories = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
	}
	objSelected = $("#Subcategory").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		strSubcategories = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
	}
	objSelected = $("#Line").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		strLines = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
	}
	strProductManager = $("#GP").getKendoDropDownList().value();

	return { Regionals: strRegionals, Warehouses: strWarehouses, Categories: strCategories, SubCategories: strSubcategories, Lines: strLines, ProductManager: strProductManager };
}

function loadPivot(items) {
	var intHeight = $(window).height() - $(".navbar").height() - $(".toolbar").height() - 50;

	var collapsed = $("#ColapsedRows").prop("checked");
	$.map(_options.pivot.xfields, function (obj) { obj.state = collapsed ? "closed" : "opened"; });
	_options.height = intHeight;
	_options.data = items;
	$(".datagrid-row-selected, .datagrid-row-checked").removeClass("datagrid-row-checked").removeClass("datagrid-row-selected");

	$("#action-excel").toggleClass("d-none", items.length <= 0);

	$("#pivot").pivotgrid(_options);
	_loaded = true;
}

//#endregion