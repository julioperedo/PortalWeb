
$(() => {
	$(".day-list").kendoSortable({
		connectWith: ".day-list",
		hint: function (element) {
			return element.clone().addClass("hint");
		},
		placeholder: function (element) {
			return $("<li class='list-item' id='placeholder' data-idline='0'>¡Soltar ac&aacute;!</li>");
		}
	});
});

$("#Save").click(function () {
	var items = [];
	$(".week-days .day-list-wrapper").each(function (i, obj) {
		$(obj).find(".list-item").each(function (j, item) {
			if ($(item).text() != "") {
				items.push({ Id: $(item).attr("data-id"), IdLine: $(item).attr("data-idline"), WeekDay: $(item).parent().attr("id").split("-")[1] });
			}
		});
	});
	$(".availables .day-list-wrapper").each(function (i, obj) {
		$(obj).find(".list-item").each(function (j, item) {
			if ($(item).text() != "") {
				if ($(item).attr("data-id") != "0") {
					items.push({ Id: $(item).attr("data-id"), IdLine: $(item).attr("data-idline"), WeekDay: 0 });
				}
			}
		});
	});
	$.post(urlSave, { Items: items }, function (data) {
		if (data.message == "") {
			$.each(data.items, function (i, obj) {
				if (obj.weekDay != 0) {
					$("[data-idline='" + obj.idLine + "']").attr("data-id", obj.id);
				} else {
					$("[data-idline='" + obj.idLine + "']").attr("data-id", obj.weekDay);
				}
			});
			showInfo("Se ha guardado la configuraci&oacute;n de los correos de oferta exitosamente.")
		} else {
			showError(`Se ha producido el siguiente error al guardar la configuraci&oacute;n de los correos de ofertas:<br />${data.message}`);
		}
	});
});