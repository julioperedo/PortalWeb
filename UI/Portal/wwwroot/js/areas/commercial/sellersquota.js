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
			{ data: "code", readOnly: true },
			{ data: "name", readOnly: true, width: 200 },
			{ data: "amount", type: "numeric", numericFormat: { pattern: "#,##0.00" }, width: 150 },
			{ data: "commentaries", width: 300 }
		],
		rowHeaders: false,
		colHeaders: ["Nom. Corto", "Nombre", "Monto", "Comentarios"],
		columnSorting: true,
		height: 700,
		licenseKey: "non-commercial-and-evaluation"
	});
}

function saveQuote() {
	var table = $(".handsontable").handsontable('getInstance');
	var lstData = table.getData();
	var lstItems = [];
	lstData.forEach(x => { if (x[2] > 0) lstItems.push({ SalesmanCode: x[0], Amount: x[2], Commentaries: x[3] }) });
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