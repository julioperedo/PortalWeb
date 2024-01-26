//#region Variables Globales
const gridMargin = 30;
//#endregion

//#region Eventos

$(function () {
	$.get(urlSubsidiaries, {}, function (data) {
		var select = $("#FilSubsidiaries");
		$.each(data.items, function (i, obj) {
			select.append(new Option(obj.name, obj.name));
		});
		select.multipleSelect({
			onUncheckAll: () => $("#FilWarehouses").empty().multipleSelect("refresh").multipleSelect("disable"),
			onCheckAll: () => loadWarehouses(),
			onClick: (view) => loadWarehouses()

		}).multipleSelect("uncheckAll");
	});
	$("#FilWarehouses").multipleSelect().multipleSelect("disable");
	$.get(urlLines, {}, function (data) {
		var select = $("#FilLines");
		$.each(data, function (i, obj) {
			select.append(new Option(obj.name, obj.name, false, false));
		});
		select.multipleSelect().multipleSelect("uncheckAll");
	});
	setupGrid($("#IsLocal").val() === "Y", $("#IsClientAllowed").val() === "Y");
	setGridHeight("Listado", gridMargin);
});

$(window).resize(function () {
	setGridHeight("Listado", gridMargin);
});

$('#filter-box').on('hidden.bs.collapse', function () {
	setGridHeight("Listado", gridMargin);
});

$('#filter-box').on('shown.bs.collapse', function () {
	setGridHeight("Listado", gridMargin);
});

$("#action-clean").click(function () { cleanFilters(); });

$("#action-filter").click(function () { filterData(); });

$("#Listado").on("click", ".item-code", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var dataItem = grd.dataItem(row);
	wnd.refresh({ url: urlDetail, data: { ItemCode: dataItem.itemCode, Subsidiary: dataItem.subsidiary } });
	wnd.open();
});

$("#action-excel").on("click", function (e) {
	ExportExcel();
});

//#endregion

//#region Métodos

function loadWarehouses() {
	$("#FilWarehouses").empty();
	var objSelected = $("#FilSubsidiaries").multipleSelect("getSelects");
	if (objSelected && objSelected.length > 0) { //"'" + x + "'"
		var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
		$.get(urlWarehouses, { Subsidiary: strData }, function (data) {
			if (data.message !== "") {
				showError(data.message);
				$("#FilWarehouses").multipleSelect("disable");
			} else {
				if (data.items.length > 0) {
					$.each(data.items, function (i, obj) {
						$("#FilWarehouses").append(new Option(obj.name, obj.name));
					});
					$("#FilWarehouses").multipleSelect("enable");
				} else {
					$("#FilWarehouses").multipleSelect("disable");
				}
			}
			$("#FilWarehouses").multipleSelect("refresh").multipleSelect("uncheckAll");
		});
	} else {
		$("#FilWarehouses").multipleSelect("refresh").multipleSelect("disable");
	}
}

function filterSubcategories(e) {
	return { CategoryId: $("#FilCategory").val() };
}

function cleanFilters() {
	$("#FilSubsidiaries").multipleSelect("uncheckAll");
	$("#FilItemCode").val("");
	$("#FilDescription").val("");
	$("#FilCategory").data("kendoDropDownList").value("");
	$("#FilLines").multipleSelect("uncheckAll");
	$("#FilAvailable").prop("checked", true);
	$("#FilBlocked").prop("checked", false);
}

function loadGrid(items) {
	var grd = $("#Listado").data("kendoGrid");
	var ds = getDataSource(items);
	grd.setDataSource(ds);
	if (items && items.length > 0) {
		$('#filter-box').collapse("hide");
		$("#action-excel").removeClass("d-none");
	} else {
		$("#action-excel").addClass("d-none");
	}
	setGridHeight("Listado", gridMargin);
}

function filterData() {
	var filtersData = getFilters();
	if (filtersData.message === "") {
		$.get(urlFilter, filtersData.data, function (data) {
			if (data.message !== "") {
				showError(data.message);
				loadGrid([]);
			} else {
				loadGrid(data.items);
			}
		});
	} else {
		showInfo(`Los siguientes campos son necesarios: <br />${filtersData.message}`);
	}
}

function setupGrid(isLocal, isClientAllowed) {
	var alignRight = { style: "text-align: right" };
	var lstColumns = [
		{ field: "category", title: "Categoría", hidden: true, width: 120, aggregates: ["count"], groupHeaderTemplate: "Categoría: #=value#    (Total: #=count#)" },
		{ field: "subcategory", title: "Subcategoría", width: 120, aggregates: ["count"], groupHeaderTemplate: "Subcategoría: #= value #    (Total: #= count#)" },
		{ field: "subsidiary", title: "Sucursal", width: 120, aggregates: ["count"], groupHeaderTemplate: "Sucursal: #= value #    (Total: #= count#)" },
		{ field: "warehouse", title: "Almacén", hidden: true, width: 250, aggregates: ["count"], groupHeaderTemplate: "Almacén: #= value #    (Total: #= count#)" },
		{ field: "itemCode", title: "Item", width: 160, groupable: false, template: "<a class='item-code action action-link'>#:itemCode#</a>" },
		{ field: "itemName", title: "Descripción", width: 300, groupable: false }
	];
	if (isLocal) {
		lstColumns.push(
			{ field: "stock", title: "Stock", width: 90, groupable: false, attributes: alignRight, headerAttributes: alignRight },
			{ field: "reserved", title: "Reserva", width: 90, groupable: false, attributes: alignRight, headerAttributes: alignRight },
			{ field: "requested", title: "Pedido", width: 90, groupable: false, attributes: alignRight, headerAttributes: alignRight },
			{ field: "available", title: "Disponible", width: 110, groupable: false, attributes: alignRight, headerAttributes: alignRight }
		);
	} else {
		if (isClientAllowed) {
			lstColumns.push({ field: "available2", title: "Disponible", width: 110, groupable: false, attributes: alignRight, headerAttributes: alignRight });
		} else {
			lstColumns.push({ field: "percentage", title: "Disponible", width: 110, groupable: false, attributes: alignRight, headerAttributes: alignRight });
		}
	}

	$("#Listado").kendoGrid({
		dataBound: e => {
			var grid = e.sender;
			for (var i = 0; i < grid.columns.length; i++) {
				grid.showColumn(i);
			}
			$("div.k-group-indicator").each(function (i, v) {
				grid.hideColumn($(v).data("field"));
			});
			grid.element.find("table").attr("style", "");
		},
		groupable: { enabled: true, messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" } },
		pageable: { enabled: true },
		noRecords: { "template": "No existen resultados para el criterio de búsqueda." },
		scrollable: true, sortable: true, selectable: true,
		columns: lstColumns,
		dataSource: getDataSource([])
	});
}

function getDataSource(items) {
	var ds = new kendo.data.DataSource({
		data: items,
		pageSize: 500,
		aggregate: [
			{ aggregate: "count", field: "subsidiary" },
			{ aggregate: "count", field: "warehouse" },
			{ aggregate: "count", field: "category" },
			{ aggregate: "count", field: "subcategory" }
		],
		group: [
			{ field: "category", dir: "asc", aggregates: [{ field: "category", aggregate: "count" }] },
			{ field: "subcategory", dir: "asc", aggregates: [{ field: "subcategory", aggregate: "count" }] }
		]
	});
	return ds;
}

function ExportExcel() {
	var filterData = getFilters();
	if (filterData.message === "") {
		window.location.href = urlExcel + "?" + $.param(filterData.data);
	} else {
		showInfo(`Los siguientes campos son necesarios <br />${filterData.message}`);
	}
}

function getFilters() {
	var strSubsidiaries, strWarehouses, strItemCode, strDescription, strCategory, strSubcategory, strLine, boAvailable, boStock, boBlocked, strMessage = "";
	strSubsidiaries = Enumerable.From($("#FilSubsidiaries").multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join();
	strWarehouses = Enumerable.From($("#FilWarehouses").multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join();
	if ($.trim(strSubsidiaries) === "") {
		strMessage += " - Al menos una Sucursal <br />";
	}
	strItemCode = $.trim($("#FilItemCode").val());
	strDescription = $.trim($("#FilDescription").val());
	if ($("#FilCategory").data("kendoDropDownList").value() !== "") {
		strCategory = $("#FilCategory").data("kendoDropDownList").text();
	}
	if ($("#FilSubcategory").data("kendoDropDownList").value() !== "") {
		strSubcategory = $("#FilSubcategory").data("kendoDropDownList").text();
	}
	strLine = Enumerable.From($("#FilLines").multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join();
	boAvailable = $("#FilAvailable").prop("checked");
	boStock = $("#FilStock").prop("checked");
	boBlocked = $("#FilBlocked").prop("checked");
	if (strItemCode === "" && strDescription === "" && strCategory === "" && strSubcategory === "" && strLine === "") {
		strMessage += " - Al menos un criterio de búsqueda que no sea de localización";
	}
	return {
		message: strMessage,
		data: {
			Subsidiaries: strSubsidiaries, WareHouses: strWarehouses, ItemCode: strItemCode, Description: strDescription, Category: strCategory, Subcategory: strSubcategory, Line: strLine, Available: boAvailable, Stock: boStock, Blocked: boBlocked
		}
	};
}

//#endregion