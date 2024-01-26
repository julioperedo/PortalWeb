//#region Variables Globales
var _minDate, _maxDate, notification;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };
//#endregion

//#region Eventos

$(() => {
    setupControls();
    filterData(true);
});

$(window).resize(function () {
    setGridHeight("Listado", _gridMargin);
});

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

$("#Listado").on("click", ".action-delete", function (e) {
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    var item = grd.dataItem(row);
    var filters = getFilters();
    showConfirm("¿Est&aacute; seguro que desea eliminar esta Notificaci&oacute;n?", function () {
        $.post(urlDelete, { Id: item.id, Filters: filters.data }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                showMessage("Se ha eliminado la Notificaci&oacute;n correctamente.");
            } else {
                showError(data.message);
            }
        });
    });
});

$("#Detail").on("click", "#Popup", () => $(".frequency, label[for='Frequency']").toggleClass("d-none", !$(this).prop("checked")));

$("#Detail").on("click", ".save-notification", function (e) {
    var form = $(this).closest("form");
    var validator = form.kendoValidator({ messages: { required: "*" } }).data("kendoValidator");
    if (validator.validate()) {
        var notification = form.serializeObject();
        var filters = getFilters();

        var lstIds = $("#Lines").getKendoMultiSelect().value();
        var lstItems = Enumerable.From(lstIds).Select("{ IdLine: $ }").ToArray();
        var lstClientIds = $("#Clients").getKendoMultiSelect().value();
        var lstClientItems = Enumerable.From(lstClientIds).Select("{ CardCode: $ }").ToArray();
        notification.ListNotificationDetails = lstItems;
        notification.ListNotificationClients = lstClientItems;

        var newPhoto = $("#new-photo").val();
        if (newPhoto !== "") {
            notification.MobileValue = newPhoto;
        }

        $.post(urlEdit, { Item: notification, Filters: filters.data }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                var wnd = $("#Detail").data("kendoWindow");
                wnd.close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });
    }
});

$("#Detail").on("click", "#photo-gallery", function (e) {
    $("#gallery").val(null);
    $("#gallery").click();
});

$("#Detail").on("click", "#photo-remove", function (e) {
    var lastPhoto = $("#MobileValue").val(), urlLastPhoto = urlFolderPhotos + lastPhoto, urlPhoto = lastPhoto === "" ? urlNoPhoto : urlLastPhoto;
    $("#photo").attr("src", urlPhoto);
    $("#new-photo").val("");
});

$("#Detail").on("change", "#gallery", function (e) {
    if (this.files && this.files[0]) {
        var FR = new FileReader();
        FR.addEventListener("load", function (e) {
            if (e.target.result !== "") {
                $.post(urlSavePhoto, { ImageBase64: e.target.result }, function (data) {
                    if (data.message === "") {
                        $("#photo").attr("src", e.target.result);
                        $("#new-photo").val(data.fileName);
                        notification.show({ title: "Imagen Subida Correctamente", message: `Imagen: ${data.fileName}` }, "success");
                    } else {
                        console.error(data.message);
                        notification.show({ title: "Error", message: "Error al subir la imagen" }, "error");
                    }
                });
            }
        });
        FR.readAsDataURL(this.files[0]);
    }
});

//arregla la dirección de una carpeta interna que sino aparece codificada
$(document).on("click", ".k-imagebrowser>.k-filemanager-listview>.k-listview-content>.k-listview-item", function (e) {
    var input = $("#k-editor-image-url")[0];
    if (input && /%2F/.test(input.value)) {
        input.value = input.value.replace(/%2F/g, "/").replace("notification/", "");
    }
});

//#endregion

//#region Metodos Privados

function setupControls() {
    var today = new Date(), lastMonth = new Date();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    var filSince = $("#filter-initial-date").kendoDatePicker({
        value: lastMonth, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#filter-final-date").kendoDatePicker({
        value: today, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    $("#filter-state").kendoDropDownList({
        dataSource: [{ Text: "Cualquiera", Value: "A" }, { Text: "Habilitados", Value: "E" }, { Text: "Deshabilitados", Value: "D" }], dataTextField: "Text", dataValueField: "Value", value: "E"
    });

    $("#Detail").kendoWindow({
        visible: false, width: 1500, title: "Notificación", modal: true, close: onCloseWindow,
        refresh: function (e) {
            $("#InitialDate").kendoDatePicker();
            $("#FinalDate").kendoDatePicker();
            var popUp = $("#Popup").prop("checked");
            $(".frequency, label[for='Frequency']").toggleClass("d-none", !popUp);
            $("#Value").kendoEditor({
                resizable: { content: true },
                tools: [
                    "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent",
                    "createLink", "unlink", "insertImage", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
                ],
                imageBrowser: {
                    fileTypes: "*.png,*.gif,*.jpg,*.jpeg", path: "notification",
                    transport: {
                        read: urlImageRead,
                        destroy: { url: urlImageDestroy, type: "POST" },
                        create: { url: urlImageCreate, type: "POST" },
                        uploadUrl: urlImageUpload,
                        imageUrl: urlImage,
                        type: "filebrowser-aspnetmvc"
                    }
                }
            });
            var data = JSON.parse($("#item-data").val());
            $("#Lines").kendoMultiSelect({
                dataTextField: "name",
                dataValueField: "id",
                value: data.Lines,
                placeholder: "Seleccione las Líneas", filter: "contains",
                dataSource: { serverFiltering: false, transport: { read: { url: urlLines } } },
            });

            $("#Clients").kendoMultiSelect({
                dataTextField: "name",
                dataValueField: "code",
                value: data.Clients,
                placeholder: "Seleccione los Clientes", filter: "contains",
                dataSource: { serverFiltering: false, transport: { read: { url: urlClients } } },
            });

            onRefreshWindow(e);
        }
    });

    $("#Listado").kendoGrid({
        columns: [
            { title: "Nombre", field: "name" },
            { title: "Desde", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "initialDate", format: dateFormat },
            { title: "Hasta", attributes: alignCenter, headerAttributes: alignCenter, width: 90, field: "finalDate", format: dateFormat },
            { title: "V. Emergente", attributes: alignCenter, headerAttributes: alignCenter, width: 110, template: '# if(popup) {# <i class="fas fa-check"></i> #} #', field: "popup" },
            { title: "Frecuencia (Hrs)", attributes: alignRight, width: 120, field: "frequency" },
            { title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 90, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled" },
            {
                field: "id", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nueva Notificación"></i>',
                template: '<i class="fas fa-pen action action-link action-edit" title="Editar Notificación"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action action-link action-delete" title="Eliminar Notificación"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row", noRecords: { template: "No hay resultados para el criterio de búsqueda." },
        dataSource: []
    });

    notification = $("#notification").kendoNotification({
        position: { pinned: true, top: 60, right: 30 },
        autoHideAfter: 10000,
        stacking: "down",
        templates: [{ type: "error", template: $("#errorTemplate").html() }, { type: "success", template: $("#successTemplate").html() }]
    }).data("kendoNotification");

}

function cleanFilters() {
    var today = new Date();
    $("#filter-initial-date").data("kendoDatePicker").value(today.addMonths(-1)), $("#filter-final-date").data("kendoDatePicker").value(today), $("#filter-name").val(""), $("#filter-state").data("kendoDropDownList").value("E");
}

function getFilters() {
    var message = "", name = $("#filter-name").val(), state = $("#filter-state").val(), initialDate = $("#filter-initial-date").data("kendoDatePicker").value(), finalDate = $("#filter-final-date").data("kendoDatePicker").value();
    if (initialDate) initialDate = initialDate.toISOString();
    if (finalDate) finalDate = finalDate.toISOString();
    if (initialDate === "" & finalDate === "" & name === "") {
        message = "Debe escoger algún criterio de búsqueda.";
    }
    return { message: message, data: { Name: name, State: state, InitialDate: initialDate, FinalDate: finalDate } };
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
    if (items) {
        var margin = _gridMargin;
        items.forEach(x => {
            x.initialDate = JSON.toDate(x.initialDate);
            x.finalDate = JSON.toDate(x.finalDate);
        });
        var grd = $("#Listado").data("kendoGrid");
        var ds = new kendo.data.DataSource({ data: items });
        grd.setDataSource(ds);
        if (items && items.length > 0) {
            $('#filter-box').collapse("hide");
            margin -= 20;
        }
        setTimeout(() => { setGridHeight("Listado", margin) }, 200);
    }
}

function clientMapper(options) {
    var items = this.dataSource.data();
    var indexs = [];
    options.value.forEach((x) => {
        indexs.push(items.indexOf(items.find(i => i.code === x)));
    });
    options.success(indexs);
}

//#endregion