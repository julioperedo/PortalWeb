//#region Variables Globales

//#endregion

//#region Eventos

//Evento al hacer click en los switch de selección
$(".selected").click(function (e) {
	if (this.checked) {
		$(this).closest("tr").find(".client").removeAttr("disabled");
	} else {
		$(this).closest("tr").find(".client").attr("disabled", "");
	}
});

$("#save-warehouses").click(function (e) {
	e.preventDefault();
	var lstSelected = [];
	$(".selected:checked").each(function () {
		var subsidiary = $(this).closest("table").data("name");
		var row = $(this).closest("tr");
		var code = row.data("code");
		var name = row.data("name");
		var id = row.data("id");
		var client = row.find(".client").prop("checked");
		lstSelected.push({ Id: id, Code: code, Name: name, Subsidiary: subsidiary, ClientVisible: client });
	});

	$.post(urlEdit, { items: lstSelected }, function (data) {
		if (data.message === "") {
			showMessage("Se realizaron los cambios correctamente.");
		} else {
			showError(data.message);
		}
	});
});

//#endregion

//#region Métodos Locales

//#endregion