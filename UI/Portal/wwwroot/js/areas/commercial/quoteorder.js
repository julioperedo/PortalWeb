//#region GLOBAL VARIABLES

var objConfig = {
	rules: {
		detail: function (input) {
			if (input.is("#detail-count")) {
				return +$.trim(input.val()) > 0;
			}
			return true;
		}
	}, messages: { required: "*", detail: "Debe ingresar al menos un producto.", email: "Debe ser un correo válido." }
};
var _intMaxItems = 50;

//#endregion

//#region EVENTS

$(document).ready(function () {
	setupControls();
	setHeight();
});

$(window).resize(() => { setHeight(); });

$("#search-products").click((e) => {
	var strLine = $("#Line").val(), strCategory = $("#Category").multipleSelect("getSelects").join(), strName = $("#ProductName").val(), strAvailable = $("#Subsidiary").multipleSelect("getSelects").join();
	$.get(urlFilterProducts, { IdLine: strLine, Category: strCategory, Name: strName, Available: strAvailable }, (data) => {
		if (data.message === "") {
			var intQuantity = data.items.length;
			if (data.items.length > 0) {
				data.items.splice(0, 0, { id: -1, name: "Agregar Todos", itemCode: "TODOS", prices: [], stock: [] });
			}
			$("#products-resume").text(`Se han encontrado ${intQuantity} productos.`);

			var lv = $("#Products").getKendoListView();
			var ds = new kendo.data.DataSource({ data: data.items });
			lv.setDataSource(ds);

			setHeight();
		} else {
			showError(`Se ha producido el siguiente error al intentar traer los datos: <br /> ${data.message}`);
		}
	});
});

$("#search-quotes").click((e) => { filterQuotes(); });

$('a[data-toggle="tab"]').on('shown.bs.tab', (e) => { setHeight(); })

$("#Products").on("click", ".product a", function () {
	var strMesasge = "";
	var strCode = $(this).parent().find(".code").text();
	if (strCode == "TODOS") {
		var Codes = [];
		var lstProducts = $("#Products .product a");
		var intTotal = +$("#detail-count").val();
		intTotal = (lstProducts.length - intTotal) <= _intMaxItems ? (lstProducts.length - intTotal) : (_intMaxItems - intTotal);
		for (var i = 0; i < intTotal; i++) {
			strCode = $(lstProducts[i]).parent().find(".code").text();
			if (strCode != "TODOS") {
				if (!addProduct(strCode)) {
					Codes.push(strCode);
				}
			} else {
				intTotal++;
			}
		}
		if (Codes.length > 0) {
			strMesasge += `<br />Los productos ${Codes.join(", ")} ya fueron agregados a la Cotización.`;
		}
		if (strMesasge != "") {
			showInfo(strMesasge);
		}
	} else {
		if (+$("#detail-count").val() < _intMaxItems) {
			if (!addProduct(strCode)) {
				showInfo(`El producto ${strCode} ya fue agregado a la Cotización.`);
			}
		} else {
			showInfo(`Sólo se pueden agregar ${_intMaxItems} productos a una cotización.`);
		}
	}
});

$("#action-new").on("click", function () { cleanForm(); });

$("#action-save").on("click", function () {
	var validator = $("#form-quote").kendoValidator(objConfig).data("kendoValidator");
	if (validator.validate()) {
		saveQuote(false);
	}
});

$("#action-sendmail").on("click", function () {
	var validator = $("#form-quote").kendoValidator(objConfig).data("kendoValidator");
	if (validator.validate()) {
		saveQuote(true);
	}
});

$("#action-delete").on("click", function () {
	if (isEmpty($(".quote-sent"))) {
		var intId = +$("#IdQuote").val();
		if (intId === 0) {
			cleanForm();
		} else {
			$.post(urlDelete, { Id: intId }, function (data) {
				if (data.message === "") {
					showMessage("Se eliminó la cotización correctamente.");
					$("#Quotes :input[value='" + intId + "']").parent().parent().remove();
					cleanForm();
				} else {
					showError(data.message);
				}
			});
		}
	} else {
		showInfo("No se puede eliminar esta cotización porque ya fue enviada al Cliente.");
	}
});

$(".quote .content").on("click", ".remove-product", function () {
	var intCont = +$("#detail-count").val();
	intCont -= 1;
	$("#detail-count").val(intCont);
	$("#detail-count-label").text(intCont);
	$(this).parent().remove();
});

$(".quote .content").on("click", ".refresh-product", function () {
	var parent = $(this).parent();
	var strCode = parent.find(".code").text();
	$.get(urlProduct, { ItemCode: strCode }, function (data) {
		parent.after(data);
		parent.remove();
	});
});

$("#Quotes").on("click", ".quote-result a", function () {
	var strCode = $(this).parent().find(".IdQuote").val();
	$.get(urlEdit, { Id: strCode }, function (data) {
		if (data.message === "") {
			var x = data.item;
			$("#IdQuote").val(x.id);
			$(".code").text(x.quoteCode);
			var quoteSent = $(".quote-sent");
			quoteSent.empty();
			$(".quote-header").removeClass("p-4");
			if (x.sentMails && x.sentMails.length > 0) {
				$(".quote-header").addClass("p-4");
				quoteSent.append($("<span>").addClass("label").text("Enviado a:"));
				quoteSent.append($("<br>"));
				x.sentMails.forEach((i) => {
					quoteSent.append($("<span>").addClass("date").text(`${i.date}`));
					quoteSent.append($("<span>").addClass("card-detail").text(`${i.cardDeatil}`));
					quoteSent.append($("<span>").addClass("client").text(`${i.clientDetail}`));
					quoteSent.append($("<br>"));
				});
			}
			$("#Client").getKendoDropDownList().value(x.cardCode);
			$("#ClientName").val(x.clientName);
			$("#ClientMail").val(x.clientMail);
			$("#Header").data("kendoEditor").value(x.header);
			$("#detail-count-label").text(x.details.length);
			$("#detail-count").val(x.details.length);
			var details = $("#quote-detail");
			details.empty();
			var template = kendo.template($("#addedProductTemplate").html());
			if (x.details && x.details.length > 0) {
				x.details.forEach((i) => {
					i.imageUrl = $.trim(i.productImageURL) === "" ? urlNoImage : (urlFolder + i.productImageURL);
					i.name = `Detail-${i.id}-${(new Date()).getTime()}`;
					details.append(template(i));
				});
				$(".description-editor").kendoEditor({ tools: [{ name: "foreColor" }, { name: "backColor" }, { name: "fontSize" }, { name: "bold" }, { name: "italic" }, { name: "underline" }, { name: "justifyLeft" }, { name: "justifyCenter" }, { name: "justifyRight" }, { name: "justifyFull" }, { name: "indent" }, { name: "insertOrderedList" }, { name: "insertUnorderedList" }] });
			}
			$("#Footer").data("kendoEditor").value(x.footer);
		} else {
			showError(`Se produjo un error al traer los datos del servidor: <br /> ${data.message}`);
		}
	});
});

$("#Products").on("click", ".selected-product", function (e) {
	if (this.dataset.value === "TODOS") {
		$("#Products .product .selected-product").prop("checked", this.checked);
	}
});

$("#Products").on("click", ".add-selected", function (e) {
	var lstProducts = $("#Products .product .selected-product:checked");
	if (lstProducts.length > 0) {
		var Codes = [];
		lstProducts.each(function (i, obj) {
			if (Codes.length < _intMaxItems) {
				var value = obj.dataset.value;
				if (value !== "TODOS") {
					if (!addProduct(value)) {
						Codes.push(value);
					}
				}
			}
		});
		if (Codes.length > 0) {
			showInfo("<br />Los productos " + Codes.join(", ") + " ya fueron agreagados a la Cotización.");
		}
	}
});

$("#ProductName").bind("paste", function (e) {
	// access the clipboard using the api
	var pastedData = e.originalEvent.clipboardData.getData('text');
	var codes = [];
	if (pastedData.indexOf("\n") >= 0 || pastedData.indexOf("\t") >= 0) {
		pastedData.split("\t").forEach((v, i) => {
			if ($.trim(v) !== "") {
				if (v.indexOf("\n") >= 0) {
					v.split("\n").forEach((v1, i1) => {
						if ($.trim(v1) !== "") {
							codes.push(`'${$.trim(v1)}'`);
						}
					});
				} else {
					codes.push(`'${$.trim(v)}'`);
				}
			}
		});
	} else {
		codes.push(`'${$.trim(pastedData)}'`);
	}
	if (codes.length > 0) {

	}
	//alert(codes.join());
	$.get(urlFilterProducts2, { ItemCodes: codes.join() }, function (data) {
		if (data.message === "") {
			var items = data.items;
			var intQuantity = items.length;
			if (items.length > 0) {
				items.splice(0, 0, { id: -1, name: "Agregar Todos", itemCode: "TODOS", prices: [], stock: [] });
			}
			$("#products-resume").text(`Se han encontrado ${intQuantity} productos.`);

			var lv = $("#Products").getKendoListView();
			var ds = new kendo.data.DataSource({ data: items });
			lv.setDataSource(ds);

			setHeight();
		} else {
			showError(`Se ha producido el siguiente error al traer los datos del servidor: <br />${data.message}`);
		}
	});
});

//#endregion

//#region METHODS

function setupControls() {
	var categories = $("#Category"), subsidiaries = $("#Subsidiary");
	subsidiaries.multipleSelect({ selectAll: false });
	categories.multipleSelect({ selectAll: false });
	$("#Line").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Línea ...", dataSource: { transport: { read: { url: urlGetLines } } } });
	$("#Products").kendoListView({ template: kendo.template($("#productTemplate").html()), dataSource: [] });

	var today = new Date();
	$("#SinceDate").kendoDatePicker({ format: "d/M/yyyy", value: today });
	$("#UntilDate").kendoDatePicker({ format: "d/M/yyyy", value: today });

	$("#Quotes").kendoListView({ template: kendo.template($("#quoteTemplate").html()), dataSource: [] });

	$("#Client").kendoDropDownList({
		dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", dataSource: { transport: { read: { url: urlClients } } },
		virtual: {
			itemHeight: 26, valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.code === options.value));
				options.success(index);
			}
		},
		change: function (e) {
			var value = this.value();
			$.get(urlClient, { CardCode: value }, function (data) {
				$("#ClientName").val(data.name);
				$("#ClientMail").val(data.eMail);
			});
		}
	});

	var editTools = [{ name: "fontName" }, { name: "fontSize" }, { name: "foreColor" }, { name: "bold" }, { name: "italic" }, { name: "formatting" }, { name: "indent" }, { name: "insertOrderedList" }, { name: "insertUnorderedList" }, { name: "justifyCenter" }, { name: "justifyFull" }, { name: "justifyLeft" }, { name: "justifyRight" }, { name: "outdent" }];
	$("#Header").kendoEditor({ tools: editTools });
	$("#Footer").kendoEditor({ tools: editTools });

	$.get(urlGetCategories, {}, (data) => {
		data.forEach((x) => {
			categories.append(new Option(x.name, x.name));
		});
		categories.multipleSelect("refresh");
	});
	$.get(urlGetSubsidiaries, {}, (data) => {
		data.forEach((x) => {
			subsidiaries.append(new Option(x.name, x.name));
		});
		subsidiaries.multipleSelect("refresh");
	});
	$.get(urlGetSignature, {}, (data) => {
		$("#signature").append(data);
	});
}

function setHeight() {
	var intHeight = $(window).height() - $(".title").height();
	$(".quote .content").height(intHeight - 60);

	if ($("#Products").is(":visible")) {
		var heightFilters = $("#product-filters").height()
		$("#Products").height(intHeight - heightFilters - 130);
		intHeight = intHeight - $("#tabstrip .k-content .quote-filter").height() - 90;
		$(".product .data").width($("#Products").width() - 40);
	}

	if ($("#Quotes").is(":visible")) {
		var heightFilters = $("#quote-filters").height();
		$(".quote-result .data").width($("#Quotes").width() - 40);
		$("#Quotes").height(intHeight - heightFilters - 130);
	}
}

function filterQuotes() {
	var dtSince = $("#SinceDate").data("kendoDatePicker").value(), dtUntil = $("#UntilDate").data("kendoDatePicker").value(), strName = $("#Name").val();
	if (dtSince) {
		dtSince = dtSince.toISOString();
	}
	if (dtUntil) {
		dtUntil = dtUntil.toISOString();
	}
	$.get(urlFilter, { Since: dtSince, Until: dtUntil, Client: strName }, function (data) {
		var lv = $("#Quotes").getKendoListView(), ds;
		if (data.message === "") {
			$("#quotes-resume").text("Se han encontrado " + data.items.length + " cotizaciones.");
			ds = new kendo.data.DataSource({ data: data.items });
		} else {
			$("#quotes-resume").text("Se ha producido un error.");
			ds = new kendo.data.DataSource({ data: [] });
			showError(data.message);
		}
		lv.setDataSource(ds);

		setHeight();
	});
}

function saveQuote(Send) {
	var Quote = getQuote();
	$.post(urlEdit, { Quote: Quote, SendMail: Send }, (data) => {
		Quote = data.item;
		$("#IdQuote").val(Quote.id);
		$(".quote-header .code").text(Quote.quoteCode);
		if (data.message === "") {
			if (Send) {
				showMessage("Correo enviado exitosamente.");
				var sent = $(".quote-sent");
				if (isEmpty(sent)) {
					sent.append($("<span>").addClass("label").text("Enviado a:"));
					sent.append($("<br>"));
				}
				sent.append("<span class='date'>" + kendo.toString(new Date(), "dd/MM/yyyy HH:mm") + " &nbsp;&nbsp;&nbsp;</span><span class='card-detail'>" + Quote.cardCode + " - " + Quote.cardName + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span class='client'>" + Quote.clientName + " ( " + Quote.clientMail + " ) " + "</span><br />");
			} else {
				showMessage("Cotización Guardada Correctamente.");
			}
		} else {
			showError(data.message);
		}
	});
}

function getQuote() {
	var intId, strQuoteCode, strCardCode, strCardName, strClientName, strClientMail, strHeader, strFooter, lstDetail = $(".quote-product");
	intId = $("#IdQuote").val();
	strQuoteCode = $(".quote-header .code").text();
	strCardCode = $("#Client").getKendoDropDownList().value();
	strCardName = $("#Client").getKendoDropDownList().text().split(" - ")[1];
	strClientName = $("#ClientName").val();
	strClientMail = $("#ClientMail").val();
	strHeader = $("#Header").getKendoEditor().value();
	strFooter = $("#Footer").getKendoEditor().value();

	var Quote = { Id: intId, CardCode: strCardCode, QuoteCode: strQuoteCode, CardName: strCardName, ClientName: strClientName, ClientMail: strClientMail, Header: strHeader, Footer: strFooter, Details: [] };
	if (lstDetail.length > 0) {
		$.each(lstDetail, function (i, obj) {
			var intIdProduct, strProductCode, strProductName, strProductDescription, strProductLink, strProductImageURL, lstPrices = $(obj).find(".price");
			intIdProduct = $(obj).find("#IdProduct").val();
			strProductCode = $(obj).find(".code").text();
			strProductLink = $(obj).find(".code").parent().attr("href");
			strProductName = $.trim($(obj).find(".name").text());
			strProductDescription = $(obj).find(".k-editor").getKendoEditor().value();
			strProductImageURL = $(obj).find("img").attr("src");
			if (strProductImageURL === urlNoImage) {
				strProductImageURL = "";
			}
			var objDetail = { IdProduct: intIdProduct, ProductCode: strProductCode, ProductName: strProductName, ProductLink: strProductLink, ProductDescription: strProductDescription, ProductImageURL: strProductImageURL, Prices: [] };
			if (lstPrices.length > 0) {
				$.each(lstPrices, function (x, div) {
					var strSubsidiary, decPrice, strObservations, boSelected, intQuantity;
					boSelected = $(div).find(".selected-price").prop("checked");
					strSubsidiary = $(div).find(".subsidiary").text();
					intIdSubsidiary = $(div).find(".selected-price").val();
					strObservations = $(div).find(".observations").text();
					decPrice = +$(div).find("[id^='Price-']").val();
					intQuantity = +$(div).find("[id^='Quantity-']").val();
					var objPrice = { IdSubsidiary: intIdSubsidiary, Subsidiary: strSubsidiary, Price: decPrice, Quantity: intQuantity, Observations: strObservations, Selected: boSelected };
					objDetail.Prices.push(objPrice);
				});
			}
			Quote.Details.push(objDetail);
		});
	}
	return Quote;
}

function cleanForm() {
	$(".quote-header .code").text("");
	$(".quote-sent").empty();

	$("#IdQuote").val(0);
	$("#Client").getKendoDropDownList().value("");
	$("#ClientName").val("");
	$("#ClientMail").val("");
	$(".quote-product").remove();
	$("#Header").getKendoEditor().value("");
	$("#Footer").getKendoEditor().value("");
}

function isEmpty(el) {
	return !$.trim(el.html());
}

function addProduct(Code) {
	var boReturn = false;
	var lstDetail = $(".quote-product");
	if (Enumerable.From(lstDetail).Where(function (x) { return $(x).find(".code").text() == Code; }).Count() == 0) {
		$.get(urlProduct, { ItemCode: Code }, function (data) {
			var intCont = +$("#detail-count").val();
			intCont += 1;
			$("#detail-count").val(intCont);
			$("#detail-count-label").text(intCont);
			$("#quote-detail").append(data);
		});
		boReturn = true;
	}
	return boReturn;
}

//#endregion