//#region Global Variables

var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };

//#endregion

//#region Events

$(function () {
	setupControls();
	filterData(true);
});

$(window).resize(function () { setGridHeight("Listado", _gridMargin); });

$("#action-clean").click((e) => { cleanFilters(); });

$("#action-filter").click((e) => { filterData(false); });

$("#Listado").on("click", ".action-new", function (e) {
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: 0 } });
	wnd.open();
});

$("#Listado").on("click", ".action-edit", function (e) {
	var grd = $("#Listado").data("kendoGrid");
	var row = $(e.currentTarget).closest("tr");
	grd.select(row);
	var item = grd.dataItem(row);
	var wnd = $("#Detail").data("kendoWindow");
	wnd.refresh({ url: urlEdit, data: { Id: item.id } });
	wnd.open();
});

$("#Detail").on("click", ".take-picture", function (e) {
	var wnd = $("#camera").kendoWindow({ title: "Foto", visible: false, modal: true, close: function (e) { Webcam.reset(); } }).data("kendoWindow");
	Webcam.set({ width: 600, height: 450, dest_width: 960, dest_height: 720, crop_width: 960, crop_height: 720, image_format: 'jpeg', jpeg_quality: 90 });
	Webcam.attach('#my_camera');
	$("#picture-type").val("E");
	wnd.open().center();
});

$("#Detail").on("click", ".save-visit", function (e) {
	var objConfig = {
		rules: {
			client: function (input) {
				if (input.is("[name=ClientDescription]")) {
					return !(($("#CardCode").getKendoDropDownList().value() == "CVAR-001" || $("#CardCode").getKendoDropDownList().value() == "AVAR-001") & input.val() == "");
				}
				return true;
			}
		},
		messages: { required: "*", client: "Se requiere aclaración de nombre." }
	};
	var form = $(this).closest("form");
	var validator = form.kendoValidator(objConfig).data("kendoValidator");
	if (validator.validate()) {
		var visit = form.serializeObject();
		$.post(urlEdit, { Item: visit }, function (data) {
			if (data.message === "") {
				$("#Detail").getKendoWindow().close();
				cleanFilters();
				showMessage("Se ha guardado exitosamente.")
			} else {
				showError("Se ha producido un error al guardar la visita, por favor intente nuevamente.");
			}
		});
	}
});

$("#newVisitor").on("click", "#photo-camera", function (e) {
	var wnd = $("#camera").kendoWindow({ title: "Foto", visible: false, modal: true, close: function (e) { Webcam.reset(); } }).data("kendoWindow");
	Webcam.set({
		// live preview size
		width: 600,
		height: 450,
		// device capture size
		dest_width: 960,
		dest_height: 720,
		// final cropped size
		crop_width: 960,
		crop_height: 720,
		// format and quality
		image_format: 'jpeg',
		jpeg_quality: 90
	});

	Webcam.attach('#my_camera');
	$("#picture-type").val("N");
	wnd.open().center();
});

$("#newVisitor").on("click", "#photo-gallery, #docId-gallery, #docIdR-gallery", function (e) {
	$("#gallery-data").val(this.id.split("-")[0]);
	$("#gallery").click();
});

$("#newVisitor").on("click", "#photo-remove, #docId-remove, #docIdR-remove", function (e) {
	var name = this.id.split("-")[0];
	var fileEmpty = name == "photo" ? urlNoPhoto : (name == "docId" ? urlNoCI : urlNoCIR);
	$("#new-" + name).attr("src", fileEmpty);
	$("#" + name + "-value").val("");
});

$("#Listado").on("click", ".finish-visit", function () {
	var grid = $("#Listado").getKendoGrid();
	var row = $(this).closest("tr");
	grid.select(row);
	var item = grid.dataItem(row);
	$.post(urlFinish, { Id: item.id }, function (data) {
		if (data.message === "") {
			cleanFilters();
		} else {
			showError("Se ha producido un error al terminar la visita, por favor intente nuevamente.");
		}
	});
});

$("#newVisitor").on("click", "#save-visitor", function (e) {
	e.preventDefault();
	var form = $(this).closest("form");
	var objConfig = { rules: {}, messages: { required: "*" } };
	var validator = form.kendoValidator(objConfig).data("kendoValidator");
	if (validator.validate()) {
		var docId, firstName, lastName, phone, photo, docIdImg, docIdRImg;
		docId = $("#newVisitor").find("#DocumentId").val();
		firstName = $("#newVisitor").find("#FirstName").val();
		lastName = $("#newVisitor").find("#LastName").val();
		phone = $("#newVisitor").find("#Phone").val();
		photo = $("#newVisitor").find("#photo-value").val();
		docIdImg = $("#newVisitor").find("#docId-value").val();
		docIdRImg = $("#newVisitor").find("#docIdR-value").val();

		$.post(urlAddVisitor, { DocumentId: docId, FirstName: firstName, LastName: lastName, Phone: phone, PhotoImage: photo, DocumentIdImage: docIdImg, DocumentIdReverseImage: docIdRImg }, function (data) {
			if (data.message === "") {
				var widget = $("#VisitorId").getKendoDropDownList();
				var dataSource = widget.dataSource;
				dataSource.add({ id: data.id, name: `${docId} - ${firstName} ${lastName}` });
				dataSource.one("sync", function () { widget.select(dataSource.view().length - 1); });
				dataSource.sync();

				widget.close();
				widget.value(data.id);

				if (photo != "") {
					$("#Detail").find("#photo").attr("src", urImages + photo);
				}
				if (docIdImg != "") {
					$("#Detail").find("#clientDocId").attr("src", urImages + docIdImg);
				}
				if (docIdRImg != "") {
					$("#Detail").find("#clientDocIdRev").attr("src", urImages + docIdRImg);
				}

				$("#newVisitor").getKendoWindow().close();
			} else {
				showError("Se ha producido un error al guardar el visitante, por favor intente nuevamente.");
			}
		});
	}
});

$("#gallery").on("change", function () {
	var name = $("#gallery-data").val();
	if (this.files && this.files[0]) {
		if (this.files[0].size <= 2900000) {
			var FR = new FileReader();
			FR.addEventListener("load", function (e) {
				$("#new-" + name).attr("src", e.target.result);
				$.post(urlSaveImage, { ImageBase64: e.target.result }, function (data) {
					if (data.message === "") {
						$("#" + name + "-value").val(data.fileName);
					} else {
						showError(data.message);
					}
				});
			});
			FR.readAsDataURL(this.files[0]);
		} else {
			showInfo("El archivo no debe pesar más de 2.2MB.");
		}
		$(this).val("");
	}
});

//#endregion

//#region Methods

function setupControls() {
	$("#FilClient").kendoDropDownList({
		dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente", filter: "contains",
		dataSource: { transport: { read: { url: urlClients } } },
		virtual: {
			itemHeight: 26, valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.code === options.value));
				options.success(index);
			}
		}
	});
	$("#FilStaff").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una persona", filter: "contains",
		dataSource: { transport: { read: { url: urlStaff } } }
	});
	var today = new Date();
	var filSince = $("#FilSince").kendoDatePicker({
		value: today, change: function (e) {
			var startDate = this.value();
			if (startDate === null) this.value("");
			filUntil.min(startDate ? startDate : _minDate);
		}
	}).data("kendoDatePicker");
	var filUntil = $("#FilUntil").kendoDatePicker({
		value: today, change: function (e) {
			var endDate = this.value();
			if (endDate === null) this.value("");
			filSince.max(endDate ? endDate : _maxDate);
		}
	}).data("kendoDatePicker");

	_maxDate = filUntil.max();
	_minDate = filSince.min();

	$("#Listado").kendoGrid({
		sortable: true, noRecords: { template: '<div class="w-100 text-center p-3">No hay resultados para el criterio de b&uacute;squeda.</div>' },
		columns: [
			{ title: "Cliente", width: 180, field: "clientName" },
			{ title: "Nombre", width: 180, field: "visitor.fullName" },
			{ title: "Doc. Identidad", width: 120, field: "visitor.documentId" },
			{ title: "Personal Visitado", width: 220, field: "staff.fullName" },
			{ title: "Fecha", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "initialDate", format: dateFormat },
			{ title: "Ingreso", attributes: alignCenter, headerAttributes: alignCenter, width: 80, field: "initialDate", format: "{0:HH:mm:ss}" },
			{ title: "Salida", attributes: alignCenter, headerAttributes: alignCenter, width: 130, template: '# if(finalDate) {# #=kendo.toString(finalDate, "HH:mm:ss")# #} else {# #if(editable) {# <a class="finish-visit k-button">Marcar Salida</a> #}# #} #', field: "finalDate" },
			{
				field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-new" title="Nueva Visita"></i>',
				template: '<i class="fas fa-pen action action-edit" title="Editar Visita"></i>'
			}
		],
		selectable: "Single, Row",
		"dataSource": []
	});

	$("#Detail").kendoWindow({
		visible: false, width: 1050, title: "Visita", modal: true, close: onCloseWindow,
		refresh: function (e) {
			setupControlsDetail();
			onRefreshWindow(e);
		}
	});

}

function getFilters() {
	var message = "", cardCode = $("#FilClient").data("kendoDropDownList").value(), visitor = $("#FilVisitor").val(), staff = $("#FilStaff").data("kendoDropDownList").value(),
		initialDate = $("#FilSince").data("kendoDatePicker").value(), finalDate = $("#FilUntil").data("kendoDatePicker").value(), notFinished = $("#NotFinished").prop("checked");
	if (initialDate) initialDate = initialDate.toISOString();
	if (finalDate) finalDate = finalDate.toISOString();
	if (initialDate === "" & finalDate === "" & cardCode === "" & visitor === "" & staff === "") {
		message = "Debe escoger algún criterio de búsqueda.";
	}
	return { message: message, data: { CardCode: cardCode, Visitor: visitor, Staff: staff, Since: initialDate, Until: finalDate, NotFinished: notFinished, _: new Date() } };
}

function filterData(pageLoad) {
	var filtersData = getFilters();
	if (filtersData.message === "") {
		$.get(urlFilter, filtersData.data, function (data) {
			loadGrid(data.items);
			if (data.message !== "") {
				showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
			}
		});
	} else {
		if (!pageLoad) {
			setGridHeight("Listado", _gridMargin);
			showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
		}
	}
}

function loadGrid(items) {
	items.forEach(x => {
		x.initialDate = JSON.toDate(x.initialDate);
		x.finalDate = JSON.toDate(x.finalDate);
	});
	var grd = $("#Listado").getKendoGrid();
	var ds = new kendo.data.DataSource({ data: items });
	grd.setDataSource(ds);

	var margin = _gridMargin;
	if (items && items.length > 0) {
		$('#filter-box').collapse("hide");
		margin -= 20;
	}
	setTimeout(() => { setGridHeight("Listado", margin) }, 200);
}

function cleanFilters() {
	var today = new Date();
	$("#FilClient").data("kendoDropDownList").value("");
	$("#FilVisitor").val("");
	$("#FilStaff").data("kendoDropDownList").value("")
	var ddlSince = $("#FilSince").data("kendoDatePicker"), ddlUntil = $("#FilUntil").data("kendoDatePicker");
	ddlUntil.min(today);
	ddlSince.max(today);
	ddlSince.value(today);
	ddlUntil.value(today);
	$("#NotFinished").prop("checked", true);
	filterData(false);
}

function setupControlsDetail() {
	//$(".zoom").zoom();
	$("#CardCode").kendoDropDownList({
		dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente", filter: "contains",
		dataSource: { transport: { read: { url: urlClients } } },
		virtual: {
			itemHeight: 26, valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.code === options.value));
				options.success(index);
			}
		},
		change: function () {
			var label = $("label[for='ClientDescription']");
			var div = $("#ClientDescription").parent();

			//Si es "Varios" muestra el campo de aclaración de nombre, sino lo oculta
			if (this.value() == "CVAR-001" || this.value() == "AVAR-001") {
				label.removeClass("d-none");
				div.removeClass("d-none");
			} else {
				label.addClass("d-none");
				div.addClass("d-none");
			}
		}
	});
	$("#VisitorId").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", noDataTemplate: $("#noDataTemplate").html(), optionLabel: "Seleccione una persona", filter: "contains",
		dataSource: { transport: { read: { url: urlVisitors } } },
		virtual: {
			itemHeight: 26,
			valueMapper: function (options) {
				var items = this.dataSource.data();
				var index = items.indexOf(items.find(i => i.id === +options.value));
				options.success(index);
			}
		},
		change: function (e) {
			var photo = $("#photo");
			var clientDocId = $("#clientDocId");
			var clientDocIdRev = $("#clientDocIdRev");

			//Coloco las fotos de las persona seleccionada o limpio si no se seleccionó ninguna
			if (this.value() == "") {
				photo.attr("src", urlNoPhoto);
				clientDocId.attr("src", urlNoCI);
				clientDocIdRev.attr("src", urlNoCIR);
				$(".visitor-image").removeClass("selected");
			} else {
				$.get(urlGetImages, { Id: this.value() }, function (data) {
					var urlPhoto = urlNoPhoto, urlCI = urlNoCI, urlCIR = urlNoCIR;
					if (data.message == "") {
						urlPhoto = data.photo != "" ? urImages + data.photo : urlNoPhoto;
						urlCI = data.ci != "" ? urImages + data.ci : urlNoCI;
						urlCIR = data.cir != "" ? urImages + data.cir : urlNoCIR;
					} else {
						showError(data.message);
					}
					photo.attr("src", urlPhoto);
					clientDocId.attr("src", urlCI);
					clientDocIdRev.attr("src", urlCIR);
					$(".visitor-image").addClass("selected");
				});
			}
		}
	});

	var filSince = $("#InitialDate").kendoDateTimePicker({
		change: function (e) {
			var startDate = this.value();
			if (startDate === null) this.value("");
			filUntil.min(startDate ? startDate : _minDate);
		}
	}).data("kendoDatePicker");
	var filUntil = $("#FinalDate").kendoDateTimePicker({
		change: function (e) {
			var endDate = this.value();
			if (endDate === null) this.value("");
			filSince.max(endDate ? endDate : _maxDate);
		}
	}).data("kendoDatePicker");

	$("#StaffId").kendoDropDownList({
		dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una persona", filter: "contains",
		dataSource: { transport: { read: { url: urlStaff } } }
	});
}

function addNewVisitor() {
	$("#VisitorId").getKendoDropDownList().close();
	var wnd = $("#newVisitor").kendoWindow({ width: 900, title: "Nuevo Visitante", visible: false, modal: true }).data("kendoWindow");
	var item = {};
	var detailsTemplate = kendo.template($("#newVisitorTemplate").html());
	wnd.content(detailsTemplate(item));
	wnd.open().center();
}

function takingPhoto() {
	Webcam.snap(function (data_uri) {
		var type = $("#picture-type").val();
		if (type == "N") {
			$("#new-photo").attr("src", data_uri);
			$.post(urlSaveImage, { ImageBase64: data_uri }, function (data) {
				if (data.message == "") {
					$("#photo-value").val(data.fileName);
				} else {
					showError(data.message);
				}
			});
		} else {
			$("#photo").attr("src", data_uri);
			var personId = $("#VisitorId").getKendoDropDownList().value();
			if (personId != "") {
				$.post(urlSaveImagePerson, { PersonId: personId, ImageBase64: data_uri }, function (data) {
					if (data.message == "") {
						$("#photo-value").val(data.fileName);
					} else {
						showError(data.message);
					}
				});
			}
		}
		$("#camera").getKendoWindow().close();
	});
}

//#endregion