//#region GLOBAL VARIABLES

//#endregion

//#region EVENTS

$(function () {
	$(".card-body").on("click", ".linecheck", function (e) {
		if (e.currentTarget.value == "0") {
			$(".linecheck").prop("checked", e.currentTarget.checked);
		}
	});
});

$(window).resize(function () { });

$("#AllClients").on("change", function () {
	loadClientsCombo(this.checked);
});

$("#action-filter").click(function (e) {
	e.preventDefault();
	var strCardCode = $("#Clients").val();
	if (strCardCode != "") {
		//$("#lines-data").load(urlDetail, { CardCode: strCardCode }, function () { $("#action-filter").removeClass("btn-primary").addClass("btn-outline-secondary"); });
		$.get(urlDetail, { CardCode: strCardCode }, function (data) {
			if (data.message === "") {
				$("#lines-data").empty();
				var template = kendo.template($("#lineTemplate").html());
				$.each(data.items, function (i, x) {
					$("#lines-data").append(template(x));
				});
				$("#action-save").removeAttr("disabled").removeClass("d-none");
			} else {
				showError(data.message);
				$("#action-save").attr("disabled");
			}
		});
	} else {
		Clean();
		showInfo("Debe seleccionar un cliente.");
	}
});

$("#action-clean").click(function (e) {
	e.preventDefault();
	Clean();
});

$("#action-save").click(function (e) {
	e.preventDefault();
	var lstSelected = [], strCardCode = $("#Clients").val();
	$(".line :checked").each(function (i, obj) {
		if ($(obj).val() !== "0") {
			lstSelected.push($(obj).val());
		}
	});
	$.post(urlEdit, { CardCode: strCardCode, Codes: lstSelected }, function (data) {
		if (data.message === "") {
			showMessage("Se guardaron las Líneas correctamente.")
		} else {
			showError("Se ha producido un error al querer guardar las Líneas: <br />" + data.message);
		}
	});
});

function clientMapper(options) {
	var items = $("#Clients").data("kendoDropDownList").dataSource.data();
	var item = Enumerable.From(items).Where(x => x.cardCode === options.value).FirstOrDefault();
	var index = Enumerable.From(items).IndexOf(item);
	options.success(index);
}

//#endregion

//#region METHODS

function Clean() {
	$("#Clients").getKendoDropDownList().value("");
	$("#lines-data").empty();
	$("#action-filter").removeClass("btn-outline-secondary").addClass("btn-primary");
	$("#AllClients").prop("checked", true);
	$("#action-save").addClass("d-none");
	loadClientsCombo(true);
}

function loadClientsCombo(all) {
	$.get(urlClients, { All: all }, function (data) {
		var ds = new kendo.data.DataSource({ data: data });
		var dropdownlist = $("#Clients").getKendoDropDownList();
		dropdownlist.setDataSource(ds);
	});
}

//#endregion