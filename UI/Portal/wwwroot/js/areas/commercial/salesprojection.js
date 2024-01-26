//#region Variables Globales

//#endregion

//#region Eventos

$(() => {
	$("#month").kendoDropDownList({ value: currentMonth });
	loadData();
});

$("#action-filter").click(() => loadData());

$("#action-save").click(() => saveQuote());

//#endregion

//#region Metodos Privados

function loadData() {
	$.get(urlFilter, { Year: $("#year").val(), Month: $("#month").val() }, function (data) {
		if (data.message === "") {
			renderGrid(data.items);
		} else {
			showError(data.message);
		}
	});
}

function renderGrid(Items) {
	var content = $("#QuoteContent");
	var $div = $("<div>");
	content.html($div);

	$div.handsontable({
		data: Items,
		columns: [
			{ data: "id", readOnly: true },
			{ data: "subsidiary", readOnly: true, width: 150 },
			{ data: "division", readOnly: true, width: 200 },
			{ data: "amount", type: "numeric", numericFormat: { pattern: "#,##0.00" }, width: 150 }
		],
		rowHeaders: false,
		colHeaders: ["Id", "Sucursal", "División", "Monto"],
		columnSorting: true,
		height: 700,
		className: "htMiddle",
		hiddenColumns: {
			columns: [0],
			indicators: false
		},
		mergeCells: [
			{ row: 0, col: 1, rowspan: 3, colspan: 1 },
			{ row: 3, col: 1, rowspan: 3, colspan: 1 },
			{ row: 6, col: 1, rowspan: 3, colspan: 1 }
		],
		licenseKey: "non-commercial-and-evaluation"
	});
}

function saveQuote() {
	var table = $(".handsontable").handsontable('getInstance');
	var lstData = table.getData();
	var lstItems = [];
	lstData.forEach(x => { lstItems.push({ Id: x[0], Subsidiary: x[1], Division: x[2], Amount: x[3], Year: $("#year").val(), Month: $("#month").val() }) });
	$.post(urlSave, { Items: lstItems, Year: $("#year").val(), Month: $("#month").val() }, function (data) {
		if (data.message == "") {
			renderGrid(data.items);
			showMessage("Se han guardado los presupuestos correctamente");
		} else {
			showError(data.message);
		}
	});
	
}

//#endregion