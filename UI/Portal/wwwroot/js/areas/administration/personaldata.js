//#region Tab 1 Save general Data

$("#savePersonalData").click(onSavingData);

//#endregion

//#region Tab 2 Change password

$("#changePassword").click(onChangePassword);

//#endregion

//#region Tab 3 Save Other data (signature, etc)

$("#updateOtherData").click(onUpdateOtherData);

//arregla la dirección de una carpeta interna que sino aparece codificada
$(document).on("click", ".k-imagebrowser>.k-filemanager-listview>.k-listview-content>.k-listview-item", function (e) {
	var input = $("#k-editor-image-url")[0];
	if (input && /%2F/.test(input.value)) {
		input.value = input.value.replace(/%2F/g, "/").replace("userdata/", "");
	}
});

//#endregion

//#region Tab 4 Photograph

$("#photo-camera").on("click", function () {
	Webcam.set({ width: 320, height: 240, dest_width: 320, dest_height: 240, crop_width: 240, crop_height: 240 });
	Webcam.attach('#my_camera');
	$("#camera").removeClass("d-none");
});

$("#photo-gallery").click(function () {
	$("#gallery").val(null);
	$("#gallery").click();
});

$("#photo-remove").click(function () {
	$("#photo").attr("src", urlNoPhoto);
	$("#new-photo").val("");
});

$("#gallery").change(function () {
	if (this.files && this.files[0]) {
		var FR = new FileReader();
		FR.addEventListener("load", function (e) {
			$("#photo").attr("src", e.target.result);
			$("#new-photo").val(e.target.result);
		});
		FR.readAsDataURL(this.files[0]);
	}
});

$("#take-photo-ok").click(function () {
	Webcam.snap(function (data_uri) {
		$("#photo").attr("src", data_uri);
		$("#new-photo").val(data_uri);
		Webcam.reset();
		$("#camera").addClass("d-none");
	});
});

$("#take-photo-cancel").click(function () {
	Webcam.reset();
	$("#camera").addClass("d-none");
});

$("#savePicture").click(function () {
	var newPhoto = $("#new-photo").val();
	if (newPhoto === "") {
		showInfo("No hay nada para guardar.");
	} else {
		$.post(urlSavePhoto, { ImageBase64: newPhoto }, function (data) {
			if (data.message === "") {
				$("#new-photo").val("");
				showMessage("Fotografía actualizada correctamente.");
			} else {
				showError(data.message);
			}
		});
	}
});

function onUserChange(e) {
	var item = this.dataItem();
	$("#SAPUser").val(item.code);
}

function onSavingData(e) {
	var form = $(this).closest("form");
	var validator = form.kendoValidator({ messages: { required: "Campo requerido.", email: "No es un E-Mail válido." } }).data("kendoValidator");
	if (validator.validate()) {
		var obj = form.serializeObject();
		console.log(obj);
		$.post(form.attr("action"), { UserData: obj }, function (data) {
			if (data.message !== "") {
				showError("Algo salió mal", `Se ha producido el siguiente error al intentar guardar los datos:<br />${data.message}`);
			} else {
				showMessage("Datos guardados correctamente");
			}
		});
	}
}

function onChangePassword(e) {
	var form = $(this).closest("form");
	var config = {
		rules: {
			oldpassword: i => i.is("#OldPassword") ? $.trim(i.val()) === $.trim($("#CurrentPassword").val()) : true,
			confirmpassword: i => i.is("#ConfirmPassword") ? $.trim(i.val()) === $.trim($("#NewPassword").val()) : true,
			minlen: i => i.is("#NewPassword") || i.is("#ConfirmPassword") ? i.val().length > 5 : true
		},
		messages: {
			required: "Campo requerido", oldpassword: "La contrase&ntilde;a no es correcta", minlen: "La nueva contrase&ntilde;a debe tener al menos 6 caracteres", confirmpassword: "Las contrase&ntilde;as no coinciden"
		}
	};
	var validator = form.kendoValidator(config).data("kendoValidator");
	if (validator.validate()) {
		var obj = form.serializeObject();
		$.post(form.attr("action"), { NewPassword: obj.NewPassword }, function (data) {
			if (data.message !== "") {
				showError("Se ha producido el siguiente error al intentar actualizar la contrase&ntilde;a:<br />" + data.message);
			} else {
				$("#CurrentPassword").val(obj.NewPassword);
				showMessage("Contraseña actualizada correctamente.");
			}
		});
	}
}

function onUpdateOtherData(e) {
	var form = $(this).closest("form");
	var obj = form.serializeObject();
	$.post(form.attr("action"), { UserData: obj }, function (data) {
		if (data.message !== "") {
			showError("Se ha producido el siguiente error al intentar guardar los datos:<br />" + data.message);
		} else {
			showMessage("Datos guardados correctamente");
		}
	});
}

//#endregion