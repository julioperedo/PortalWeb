//#region Global Variables

const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy HH:mm:ss}", _gridMargin = 30;

//#endregion

//#region Events

$(function () {
	setupControls();
});

$(window).resize(function () { setGridHeight("Listado", _gridMargin); });

$("#action-search").click(function (e) {
	var strClient = $.trim($("#Client").val()), strSalesMan = $("#Salesman").val();
	$.get(urlFilter, { SellerCode: strSalesMan, ClientName: strClient }, function (data) {
		if (data.message === "") {
			loadGrid(data.items);
		} else {
			showError(data.message);
		}
	});
});

$("#action-excel").click(function (e) {
	var strClient = $.trim($("#Client").val()), strSalesMan = $("#Salesman").val();
	window.location.href = urlExport + "?" + $.param({ SellerCode: strSalesMan, ClientName: strClient });
});

$("#Listado").on("click", ".sessions", function (e) {
	e.preventDefault();
	var wnd = $("#Detail").data("kendoWindow");
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");

	grd.select(row);
	var item = grd.dataItem(row);
	if (item.lastLog) {
		$.get(urlDetail, { CardCode: item.cardCode, LogDate: item.lastLog.toISOString() }, function (data) {
			var items = [];
			if (data.message === "") {
				items = data.items;
				items.forEach(x => {
					x.logDate = JSON.toDate(x.logDate);
				});
				var grd = $("#Sessions").getKendoGrid();
				var ds = new kendo.data.DataSource({ data: items });
				grd.setDataSource(ds);
				wnd.center().open();
			} else {
				showError(`Se ha producido el siguiente error al traer los datos: <br /> ${data.message}`);
			}
		});
	}
});

//#endregion

//#region Methods

function setupControls() {
	$("#Salesman").kendoDropDownList({
		dataSource: { transport: { read: { url: urlSellers } } }, dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Vendedor..."
	});

	$("#Listado").kendoGrid({
		columns: [
			{ title: "Days", hidden: true, field: "days", aggregates: ["count"], groupHeaderTemplate: "#= showPeriod(value, count) #" },
			{ title: "Cliente", template: '# if(lastLog) {# <a class="sessions action action-link">#=clientName#</a> #} else {# #=clientName# #} #', field: "clientName" },
			{ title: "Correo", field: "eMail" },
			{ title: "Vendedor", field: "seller" },
			{ title: "Ultimo Ingreso", width: 150, template: '<a class="sessions action action-link"># if(lastLog) {# #=kendo.toString(lastLog, "dd-MM-yyyy HH:mm:ss")# #} #</a>', field: "LastLog" }
		],
		sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="w-100 text-center p-4">No hay resultados para el criterio de b&uacute;squeda.</div>' },
		dataSource: {
			group: [{ field: "days", dir: "desc", aggregates: [{ field: "days", aggregate: "count" }] }],
			aggregate: [{ field: "days", aggregate: "count" }]
		}
	});

	$("#Sessions").kendoGrid({
		columns: [
			{ title: "Usuario", field: "userName" },
			{ title: "Hora", headerAttributes: alignCenter, attributes: alignCenter, field: "logDate", format: "{0:HH:mm.ss}", width: 150 }
		], sortable: true, selectable: "Single, Row",
		dataSource: []
	});

	$("#Detail").kendoWindow({ visible: false, modal: true, scrollable: true, pinned: false, title: "Accesos", resizable: false, width: 800, actions: ["Close"], open: onRefreshWindow });

}

function loadGrid(items) {
	items.forEach(x => {
		x.lastLog = JSON.toDate(x.lastLog);
	});
	var grd = $("#Listado").getKendoGrid();
	var ds = new kendo.data.DataSource({ data: items, group: [{ field: "days", dir: "desc", aggregates: [{ field: "days", aggregate: "count" }] }] });
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

function showPeriod(days, count) {
	var strDays = "";
	if (days < 1100) {
		if (days < 1000) {
			if (days === 0) {
				strDays = "Ultimo Ingreso: Menor a 15 días    ( Total: " + count + " )";
			} else {
				strDays = "Ultimo Ingreso: Mayor a " + days + " días    ( Total: " + count + " )";
			}
		} else {
			strDays = "Ultimo Ingreso: Nunca    ( Total: " + count + " )";
		}
	} else {
		strDays = "Clientes Sin Usuario      ( Total: " + count + " )";
	}
	return strDays;
}

//#endregion