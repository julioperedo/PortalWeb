//#region Global Variables

const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy HH:mm:ss}", _gridMargin = 30;

//#endregion

//#region Events

$(function () {
	setupControls();
	setGridHeight("Listado", _gridMargin);
});

$(window).resize(function () { setGridHeight("Listado", _gridMargin); });

$("#action-clean").click(function (e) {
	$("#Subsidiary").multipleSelect("uncheckAll");
	$("#ItemCode, #Description").val("");
	$("#Category").data("kendoDropDownList").value("");
	$("#Line").multipleSelect("uncheckAll");
	$("#Blocked").data("kendoDropDownList").value("");
	loadGrid([]);
});

$("#action-filter").click(function (e) {
	filterData();
});

$("#action-excel").on("click", function (e) {
	e.preventDefault();
	var filtersData = getFilters();
	if (filtersData.message === "") {
		window.location.href = urlExport + "?" + $.param(filtersData.data);
	} else {
		showInfo(`Debe seleccionar los siguientes campos: <br />${filtersData.message}`);
	}
});

$("#Listado").on("click", ".item-code", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var dataItem = grd.dataItem(row);
	$.get(urlDetail, { ItemCode: dataItem.itemCode, Subsidiary: dataItem.subsidiary }, (data) => {
		$("#item-code").text(data.code);
		$("#item-name").text(data.name);
		$("#category").text(data.category);
		$("#subcategory").text(data.subcategory);
		$("#line").text(data.line);
		$("#detail").text(data.detail);
		$("#comments").text(data.commentaries);
		wnd.center().open();
	});
});

$("#Listado").on("click", ".see-reserved", function (e) {
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var item = grd.dataItem(row);
	if (item.reserved > 0) {
		$.get(urlGetReservedItems, { Subsidiary: item.subsidiary, Warehouse: item.warehouse, ItemCode: item.itemCode }, function (data) {
			if (data.message === "") {
				var extraTitle = item.subsidiary.toLowerCase() === "iquique" ? "<th>Autorizado</th><th>Correlativo</th>" : "";
				var content = `<table class="table"><thead class="thead-light"><tr><th scope="col">Ejecutivo</th><th scope="col">Cliente</th><th scope="col">Orden</th><th scope="col">Fecha</th><th scope="col" class="text-right">Cantidad</th><th scope="col" class="text-right">Precio</th>${extraTitle}<th>&nbsp;</th></thead><tbody>`;
				$.each(data.items, function (i, obj) {
					var extraData = item.subsidiary.toLowerCase() === "iquique" ? `<td class="text-center">${obj.authorized === "Y" ? '<i class="fas fa-check"></i>' : ''}</td><td>${obj.correlative}</td>` : "";
					content += `<tr><td>${obj.sellerName}</td><td><strong>${obj.clientCode}</strong> - ${obj.clientName}</td><td>${obj.docNum}</td><td>${kendo.toString(JSON.toDate(obj.docDate), "dd-MM-yyyy")}</td><td class="text-right">${obj.quantity}</td><td class="text-right">${obj.price !== null ? kendo.toString(obj.price, "N2") : ""}</td>${extraData}<td>`;
					if (obj.hasFiles) {
						$.each(obj.files, function (j, oFile) {
							content += `<a href="#" title="${oFile}" class="open-file" data-subsidiary="${obj.subsidiary}" data-code="${obj.docEntry}" data-file="${oFile}"><span class="glyphicon glyphicon-paperclip"></span></a>&nbsp;&nbsp;&nbsp;`;
						});
					}
					content += `</td></tr>`;
				});
				content += '</tbody></table>';

				var wnd = $("#Detail2").data("kendoWindow");
				wnd.setOptions({ title: "Detalle Reservas" });
				wnd.content(content);
				wnd.open().center();
			} else {
				showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
			}
		});
	}
});

//#endregion

//#region Methods

function setupControls() {

	$("#Subsidiary").multipleSelect({
		onUncheckAll: () => { $("#Storehouse").empty().multipleSelect("refresh").multipleSelect("disable"); },
		onCheckAll: () => { loadStoreHouses(); },
		onClick: (view) => { loadStoreHouses(); }
	});
	$("#Storehouse").multipleSelect().multipleSelect("disable");
	$("#Category").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Categoría", dataSource: { transport: { read: { url: urlCategories } } } });
	$("#SubCategory").kendoDropDownList({
		cascadeFrom: "Category", dataTextField: "name", dataValueField: "id", enable: false, optionLabel: "Seleccione un Subcategoría...",
		dataSource: { transport: { read: { url: urlSubcategories, data: () => { return { CategoryId: $("#Category").val() }; } } }, serverFiltering: true }
	});
	$("#Line").multipleSelect();
	$("#Blocked").kendoDropDownList({ dataTextField: "Text", dataValueField: "Value", optionLabel: "Cualquiera", dataSource: [{ Text: "Si", Value: "S" }, { Text: "No", Value: "N" }] });

	$("#Listado").kendoGrid({
		dataBound: (e) => {
			var grid = e.sender;
			for (var i = 0; i < grid.columns.length; i++) {
				grid.showColumn(i);
			}
			$("div.k-group-indicator").each(function (i, v) {
				grid.hideColumn($(v).data("field"));
			});
		},
		columns: [
			{ title: "Categoria", width: 150, field: "category", aggregates: ["count"], groupHeaderTemplate: "Categoría: #=value#    (Total: #=count#)" },
			{ title: "Subcategoria", width: 200, field: "subcategory", aggregates: ["count"], groupHeaderTemplate: "Subcategoría: #= value #    (Total: #= count#)" },
			{ title: "Sucursal", width: 100, field: "subsidiary", aggregates: ["count"], groupHeaderTemplate: "Sucursal: #= value #    (Total: #= count#)" },
			{ title: "Almacén", width: 220, field: "warehouse", aggregates: ["count"], groupHeaderTemplate: "Almacén: #= value #    (Total: #= count#)" },
			{ title: "Item", width: 160, template: '<a class="item-code action action-link">#:itemCode#</a>', field: "itemCode", groupable: false },
			{ title: "Descripción", width: 300, field: "itemName", groupable: false },
			{ title: "Línea", width: 220, field: "line", aggregates: ["count"], groupHeaderTemplate: "Línea: #= value #    (Total: #= count#)" },
			{ title: "Stock", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "stock", groupable: false },
			{ title: "Reserva", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "reserved", groupable: false, template: "# if(reserved > 0) {# <a class='action action-link see-reserved'>#=reserved#</a> #} else {# #=reserved# #} #" },
			{ title: "Pedido", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "requested", groupable: false },
			{ title: "Disponibilidad", attributes: alignRight, headerAttributes: alignRight, width: 110, field: "available", groupable: false },
			{ title: "Aviso", width: 70, field: "warning", groupable: false },
			{ title: "Unitario", attributes: alignRight, headerAttributes: alignRight, width: 110, field: "priceReal", format: "{0:N2}", groupable: false },
			{ title: "Total", attributes: alignRight, headerAttributes: alignRight, width: 110, field: "totalReal", format: "{0:N2}", groupable: false },
			{ title: "Unitario Modificado", attributes: alignRight, headerAttributes: alignRight, width: 160, field: "priceModified", format: "{0:N2}", groupable: false },
			{ title: "Total Modificado", attributes: alignRight, headerAttributes: alignRight, width: 140, field: "totalModified", format: "{0:N2}", groupable: false }
		],
		groupable: { enabled: true }, sortable: true, selectable: "Single, Row",
		noRecords: { template: "No hay resultados para el criterio de búsqueda." },
		dataSource: getDatasource([])
	});
	$("#Detail").kendoWindow({ visible: false, width: 800, title: "Producto", modal: true });
	$("#Detail2").kendoWindow({ visible: false, width: 800, title: "Detalle Reservas", modal: true });

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

}

function loadStoreHouses() {
	$("#Storehouse").empty();
	var objSelected = $("#Subsidiary").multipleSelect("getSelects", "text");
	if (objSelected && objSelected.length > 0) {
		var strData = Enumerable.From(objSelected).Select(function (x) { return "'" + x + "'" }).ToArray().join();
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

function getDatasource(items) {
	var ds = new kendo.data.DataSource({
		data: items,
		group: [
			{ field: "category", dir: "asc", aggregates: [{ field: "category", aggregate: "count" }, { field: "subcategory", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "subsidiary", aggregate: "count" }, { field: "warehouse", aggregate: "count" }] },
			{ field: "subcategory", dir: "asc", aggregates: [{ field: "category", aggregate: "count" }, { field: "subcategory", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "subsidiary", aggregate: "count" }, { field: "warehouse", aggregate: "count" }] }
		],
		aggregate: [{ field: "category", aggregate: "count" }, { field: "subcategory", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "subsidiary", aggregate: "count" }, { field: "warehouse", aggregate: "count" }]
	});
	return ds;
}

function getFilters() {
	var message = "", subsidiaries, warehouses, itemCode = $.trim($("#ItemCode").val()), description = $.trim($("#Description").val()), category, subcategory, line, blocked = $("#Blocked").getKendoDropDownList().value();
	subsidiaries = Enumerable.From($("#Subsidiary").multipleSelect("getSelects", "text")).Select((x) => { return `'${x}'` }).ToArray().join();
	warehouses = Enumerable.From($("#Storehouse").val()).Select((x) => { return `'${x}'` }).ToArray().join()
	var ddlCategory = $("#Category").data("kendoDropDownList"), ddlSubcategory = $("#SubCategory").data("kendoDropDownList");
	if (ddlCategory.value() !== "") {
		category = ddlCategory.text();
	}
	if (ddlSubcategory.value() !== "") {
		subcategory = ddlSubcategory.text();
	}
	line = Enumerable.From($("#Line").multipleSelect("getSelects", "text")).Select((x) => { return `'${x}'` }).ToArray().join();

	if ($.trim(subsidiaries) === "") {
		message += " - Al menos una Sucursal <br />";
	}
	if (itemCode === "" && description === "" && category === "" && subcategory === "" && line === "") {
		message += " - Al menos un criterio de búsqueda que no sea de localización <br />";
	}
	return { message: message, data: { Subsidiaries: subsidiaries, StoreHouses: warehouses, ItemCode: itemCode, Description: description, Category: category, Subcategory: subcategory, Line: line, Blocked: blocked, _: new Date() } };
}

function loadGrid(items) {
	var grd = $("#Listado").data("kendoGrid");
	var ds = getDatasource(items);
	grd.setDataSource(ds);

	var margin = _gridMargin;
	if (items && items.length > 0) {
		$('#filter-box').collapse("hide");
		$("#action-excel").removeClass("d-none");
		margin -= 20;
	} else {
		$("#action-excel").addClass("d-none");
	}
	setTimeout(() => { setGridHeight("Listado", margin) }, 200);
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
		showInfo(`Debe seleccionar los siguientes campos: <br />${filtersData.message}`);
	}
}

//#endregion