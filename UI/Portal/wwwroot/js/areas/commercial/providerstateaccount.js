//#region Variables Globales
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 15;
//#endregion

//#region Eventos

$(() => {
	setupControls();
	setTimeout(function () { setGridHeight("Listado", _gridMargin) }, 800);
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

$("#action-filter").click(function () { filterData(); });

$("#action-excel").click(function () { exportExcel(); });

$("#Listado").on("click", ".order", function (e) {
	var wnd = $("#Report").data("kendoWindow");
	var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var billNumber = $(this).text();
	var item = grd.dataItem(row);
	wnd.title(`Orden ${billNumber} de ${item.providerName}`);
	loadReport(billNumber, item.subsidiary, "Order");
	wnd.open().center();
});

$("#Listado").on("click", ".bill", function (e) {
	var wnd = $("#Report").data("kendoWindow");
	var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var billNumber = $(this).text();
	var item = grd.dataItem(row);
	wnd.title(`Factura ${billNumber} de ${item.providerName}`);
	loadReport(billNumber, item.subsidiary, "Bill");
	wnd.open().center();
});

//#endregion

//#region Metodos Privados

function setupControls() {
	var subsidiary = $("#subsidiary"), provider = $("#provider"), seller = $("#seller");
	subsidiary.multipleSelect(), provider.multipleSelect({ filter: true, selectAll: false }), seller.multipleSelect({ filter: true });

	$.get(urlSubsidiaries, {}, data => {
		data.items.forEach(x => subsidiary.append(new Option(x.name, x.name)));
		subsidiary.multipleSelect("refresh");
	});
	$.get(urlProviders, {}, data => {
		data.forEach(x => provider.append(new Option(x.name, x.code)));
		provider.multipleSelect("refresh");
	});
	$.get(urlPMs, {}, data => {
		data.forEach(x => seller.append(new Option(x.name, x.code)));
		seller.multipleSelect("refresh");
	});

	$("#Listado").kendoGrid({
		dataSource: { data: [], aggregate: [{ field: "balance", aggregate: "sum" }] },
		sortable: true,
		selectable: true,
		groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" } },
		noRecords: { template: "No se encontraron registros para el criterio de búsqueda." },
		pageable: false,
		columns: [
			{ field: "subsidiary", title: "Sucursal", width: 110, aggregates: ["count"], groupHeaderTemplate: "Sucursal: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "providerName", title: "Proveedor", width: 250, aggregates: ["count"], groupHeaderTemplate: "Proveedor: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "productManager", title: "Encargado", width: 180, aggregates: ["count"], groupHeaderTemplate: "Encargado: # if(value) {# #=value# #} else {# - Ninguno - #} #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "docDate", title: "Fecha Doc.", width: 105, format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter, aggregates: ["count"], groupHeaderTemplate: "Fecha Doc.: #= kendo.toString(value, 'dd/MM/yyyy') #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "docDueDate", title: "Fecha Venc.", width: 115, attributes: alignCenter, headerAttributes: alignCenter, format: dateFormat, aggregates: ["count"], groupHeaderTemplate: "Fecha Venc.: #= kendo.toString(value, 'dd/MM/yyyy') #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "type", title: "Tipo Doc.", width: 120, aggregates: ["count"], groupHeaderTemplate: "Tipo Doc.: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "docNum", title: "Nro. Doc.", width: 95, groupable: false, template: (item) => item.billProvider && item.billProvider !== 0 ? `<a class="bill action action-link" id="${kendo.htmlEncode(item.docNum)}">${kendo.htmlEncode(item.docNum)}</a>` : "" },
			{
				field: "docBase", title: "Orden Compra", width: 130, aggregates: ["count"], attributes: alignCenter, headerAttributes: alignCenter,
				groupHeaderTemplate: "Orden Compra: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  ",
				template: (item) => $.trim(item.docBase) !== "" && $.trim(item.docBase) !== "0" ? `<a class="order action action-link" id="${kendo.htmlEncode(item.docBase)}">${kendo.htmlEncode(item.docBase)}</a>` : ""
			},
			{ field: "billProvider", title: "Fact. Proveedor", width: 135, attributes: alignCenter, headerAttributes: alignCenter, groupable: false, template: (item) => item.billProvider && item.billProvider !== 0 ? item.billProvider : "" },
			{ field: "billNumber", width: 135, title: "Fact. Fabricante", groupable: false },
			{ field: "terms", title: "Término", width: 90, aggregates: ["count"], groupHeaderTemplate: "Término: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "docTotal", title: "Total", width: 120, attributes: alignRight, headerAttributes: alignRight, format: "{0:#,##0.00}", groupable: false },
			{ field: "balance", title: "Balance", aggregates: ["sum"], width: 120, attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: "Total : #=kendo.toString(sum, 'N2')#", format: "{0:#,##0.00}", aggregates: ["sum", "count"] },
			{ field: "days", title: "Días", width: 70, attributes: alignRight, headerAttributes: alignRight, aggregates: ["count"], groupHeaderTemplate: "Días: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " },
			{ field: "state", title: "Estado", width: 80, aggregates: ["count"], groupHeaderTemplate: "Estado: #= value #     ( Total: #= count #, Monto Total: #= kendo.toString(aggregates.balance.sum, 'N2') # )  " }
		],
		dataBound: function (e) {
			var grid = e.sender;
			for (var i = 0; i < grid.columns.length; i++) {
				grid.showColumn(i);
			}
			$("div.k-group-indicator").each(function (i, v) {
				grid.hideColumn($(v).data("field"));
			});
		}
	});
}

function cleanFilters() {
	$("#subsidiary").multipleSelect("uncheckAll"), $("#seller").multipleSelect("uncheckAll"), $("#provider").multipleSelect("uncheckAll");
}

function getFilters() {
	var message = "", subsidiary = Enumerable.From($(`#subsidiary`).multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join(),
		seller = Enumerable.From($(`#seller`).multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join(),
		provider = Enumerable.From($(`#provider`).multipleSelect('getSelects')).Select(function (x) { return `'${x}'` }).ToArray().join();
	if (subsidiary === "" & seller === "" & provider === "") {
		message = "Debe escoger algún criterio de búsqueda.";
	}
	return { message: message, data: { Subsidiary: subsidiary, ProductManager: seller, Provider: provider } };
}

function filterData() {
	var filtersData = getFilters();
	if (filtersData.message === "") {
		$.get(urlFilter, filtersData.data, function (data) {
			loadGrid(data.items);
			if (data.message !== "") {
				showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
			}
		});
	} else {
		setGridHeight("Listado", _gridMargin);
		showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
	}
}

function loadGrid(items) {
	if (items) {
		items.forEach(x => {
			x.docDate = JSON.toDate(x.docDate);
			x.docDueDate = JSON.toDate(x.docDueDate);
		});
		var grd = $("#Listado").data("kendoGrid");
		var ds = new kendo.data.DataSource({
			data: items,
			pageSize: 500,
			aggregate: [{ field: "balance", aggregate: "sum" }],
			group: [{ field: "providerName", dir: "asc", aggregates: [{ field: "providerName", aggregate: "count" }, { field: "balance", aggregate: "sum" }] }],
			sort: { field: "docDate", dir: "asc" }
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

function loadReport(Id, Subsidiary, Report) {
	var objParams = { Subsidiary: Subsidiary, Id: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "ProviderOrder.trdp";
	if (Report === "Bill") {
		strReport = "ProviderBill.trdp";
	}
	var viewer = $("#reportViewer1").data("telerik_ReportViewer");
	if (viewer) {
		try {
			viewer.reportSource({ report: strReport, parameters: objParams });
			viewer.refreshReport();
		} catch (e) {
			showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
		}
	} else {
		$("#reportViewer1").telerik_ReportViewer({
			serviceUrl: urlService,
			reportSource: { report: strReport, parameters: objParams },
			viewMode: telerikReportViewer.ViewModes.INTERACTIVE
		});
	}
}

function exportExcel() {
	var filtersData = getFilters();
	if (filtersData.message === "") {
		window.location.href = urlExcel + "?" + $.param(filtersData.data);
	} else {
		showInfo(`Los siguientes campos son necesarios <br />${filtersData.message}`);
	}
}

//#endregion