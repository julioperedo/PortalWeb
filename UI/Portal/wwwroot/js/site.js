//#region Variables Globales

$.fn.multipleSelect.defaults.formatSelectAll = () => "Seleccionar Todos";
$.fn.multipleSelect.defaults.formatAllSelected = () => "Todos seleccionados";
$.fn.multipleSelect.defaults.formatCountSelected = (x, y) => `${x} de ${y} seleccionados`;
$.fn.multipleSelect.defaults.formatNoMatchesFound = () => "No se encontraron coincidencias";
$.fn.multipleSelect.defaults.width = "100%";

if (kendo.ui.ImageBrowser) {
    kendo.ui.ImageBrowser.prototype.options.messages =
        $.extend(true, kendo.ui.FileBrowser.prototype.options.messages, {
            "uploadFile": "Subir fichero",
            "orderBy": "Ordenar por",
            "orderByName": "Nombre",
            "orderBySize": "Tamaño",
            "directoryNotFound": "Un directorio con este nombre no fue encontrado.",
            "emptyFolder": "Carpeta vacía",
            "deleteFile": '¿Está seguro que quiere eliminar "{0}"?',
            "invalidFileType": "El fichero seleccionado \"{0}\" no es válido. Los tipos de ficheros soportados son {1}.",
            "overwriteFile": "Un fichero con el nombre \"{0}\" ya existe en el directorio actual. ¿Desea reemplazarlo?",
            "dropFilesHere": "arrastre un fichero aquí para subir",
            "search": "Buscar"
        });
}
if (kendo.ui.DateTimePicker) {
    kendo.ui.DateTimePicker.prototype.options.messages = $.extend(true, kendo.ui.DateTimePicker.prototype.options.messages, {
        "set": "Colocar",
        "cancel": "Cancelar",
        "hour": "hora",
        "minute": "minuto",
        "second": "segundo",
        "millisecond": "milisegundos",
        "now": "Ahora",
        "date": "Fecha",
        "time": "Tiempo",
        "today": "Hoy"
    });
}
if (kendo.ui.Calendar) {
    kendo.ui.Calendar.prototype.options.messages = $.extend(true, kendo.ui.Calendar.prototype.options.messages, { "today": "Hoy" });
}

const hanaError = "HanaException: Connection failed", serverHana = "srvsaph";

//#endregion

//#region Eventos

//$.ajaxSetup({ timeout: 9001 });
$(document).ajaxStart(() => { showLoading(); }).ajaxStop(() => { hideLoading(); });

$(function () {
    kendo.culture("es-BO");

    //Barra de búqueda en el menu
    $("#search-menu").keypress((e) => {
        if (e.which === 13) {
            alert("Buscar");
        }
    });
    //Botones del pop up de las notificaciones
    $("#notifications").on("click", ".menu-item", function () {
        if ($(this).hasClass("selected") === false) {
            $(".menu-item.selected, .content-item.selected").removeClass("selected");
            $(this).addClass("selected");
            var strId = this.id.replace("menu", "content");
            $("#" + strId).addClass("selected");
            $(".notifications .tool .title").text(this.innerHTML);
            var notification = $("#notifications").getKendoWindow();
            if (notification) {
                notification.center();
            }
        }
    });
    $("#notifications").on("click", ".tool-prior", function () {
        var sel = $(".notifications .menu-item.selected");
        sel.removeClass("selected");
        var strId = sel.attr("id").replace("menu", "content");
        $(".notifications #" + strId).removeClass("selected");
        if (sel.attr("id") === $(".notifications .menu-item").first().attr("id")) {
            $(".notifications .menu-item").last().addClass("selected");
            $(".notifications .content-item").last().addClass("selected");
            $(".notifications .tool .title").text($(".notifications .content-item").last().attr("title"));
        } else {
            sel.prev().addClass("selected");
            $(".notifications #" + strId).prev().addClass("selected");
            $(".notifications .tool .title").text($(".notifications #" + strId).prev().attr("title"));
        }
        $("#notifications").getKendoWindow().center();
    });
    $("#notifications").on("click", ".tool-next", function () {
        var sel = $(".notifications .menu-item.selected");
        sel.removeClass("selected");
        var strId = sel.attr("id").replace("menu", "content");
        $(".notifications #" + strId).removeClass("selected");
        if (sel.attr("id") === $(".notifications .menu-item").last().attr("id")) {
            $(".notifications .menu-item").first().addClass("selected");
            $(".notifications .content-item").first().addClass("selected");
            $(".notifications .tool .title").text($(".notifications .content-item").first().attr("title"));
        } else {
            sel.next().addClass("selected");
            $(".notifications #" + strId).next().addClass("selected");
            $(".notifications .tool .title").text($(".notifications #" + strId).next().attr("title"));
        }
        $("#notifications").getKendoWindow().center();
    });
    document.addEventListener('scroll', function (event) {
        var scrollDistance = $(event.target).scrollTop();
        if (scrollDistance > 100) {
            $('.scroll-to-top').fadeIn();
        } else {
            $('.scroll-to-top').fadeOut();
        }
    }, true);
    $("#notification-message").kendoNotification({
        button: true,
        autoHideAfter: 15000,
        position: { bottom: 20, right: 20 },
        show: function (e) {
            var element = e.element.parent(), pos = element.position();
            element.css({ top: pos.top - 5 });
        },
        templates: [
            { type: "error", template: '<div class="new-noti"><span class="new-noti-icon"><i class="fas fa-times"></i></span><span class="new-noti-content"><span class="new-noti-close"><i class="fas fa-times"></i></span><span class="new-noti-title">#=title#</span><span class="new-noti-subtitle">#=message#</span></span></div>' },
            { type: "success", template: '<div class="new-noti"><span class="new-noti-icon"><i class="fas fa-check"></i></span><span class="new-noti-content"><span class="new-noti-close"><i class="fas fa-times"></i></span><span class="new-noti-title">#=title#</span><span class="new-noti-subtitle">#=message#</span></span></div>' }
        ]
    });

    //$(".page-wrapper").mCustomScrollbar({ scrollInertia: 100 });
});

//#endregion

//#region Métodos y Eventos para las grillas

//Para ajustar el tamaño de las ventanas
function onRefreshWindow(e) {
    var win = $(window);
    var margins = 80; //Asumimos 80 la suma de los márgenes por eje
    var height = e.sender.wrapper.height(), width = e.sender.wrapper.width();
    if ((win.height() <= (height + margins)) || (win.width() <= (width + margins))) {
        if (win.width() >= 992) { //Pantallas de computadoras
            if (win.height() <= (height + margins)) {
                height = win.height() - margins;
            }
            if (win.width() <= (width + margins)) {
                width = win.width() - margins;
            }
            e.sender.element.css({ maxHeight: height, maxWidth: width });
            setTimeout(() => { e.sender.center(); }, 100);
        } else {
            e.sender.maximize();
        }
    } else {
        e.sender.center();
    }
}

function onCloseWindow(e) { e.sender.element.empty(); }

function onCloseDestroy(e) { this.destroy(); }

function setGridHeight(gridName, margins) {
    if (!gridName) gridName = "Listado";
    if (!margins) margins = 15;
    var content = $(`#${gridName} .k-grid-content`);
    var isFirefox = typeof InstallTrigger !== 'undefined';
    if (isFirefox) margins += 15;
    content.height(content.height() + $(window).height() - $(".container-fluid .card").height() - margins);
}

function setListViewHeight(listviewName, margins) {
    if (!listviewName) listviewName = "Listado";
    if (!margins) margins = 15;
    var content = $(`#${listviewName} .k-listview-content`);
    var isFirefox = typeof InstallTrigger !== 'undefined';
    if (isFirefox) margins += 15;
    content.height(content.height() + $(window).height() - $(".container-fluid .card").height() - margins);
}

//#endregion

//#region Loading

function showLoading() {
    //$.blockUI({ message: '<div class="loading"><div class="spinner-border text-primary" style="width: 4rem; height: 4rem; margin-right: 3rem;" role="status"></div><span>Por favor espere...</span></div>', css: { backgroundColor: "#FFF", opacity: .9 }, baseZ: 10000 });
    $.blockUI({ message: '<div class="loading"><div class="spinner-border text-primary" style="width: 3rem; height: 3rem; margin-right: 3rem;" role="status"></div><span>Por favor espere...</span></div>', css: { right: "20px", left: "", top: "", bottom: "20px", width: "" }, showOverlay: false, baseZ: 10000 });
}

function hideLoading() { $.unblockUI(); }

//#endregion

//#region Mensajes

function showError(Message) {
    var message = Message;
    if ($.trim(message) != "") {
        if (message.includes(hanaError) || message.includes(serverHana)) message = "Estamos teniendo problemas con nuestro servidor de Datos y estamos intentando solucionarlo lo más rápido posible, por favor tenga paciencia e intente más tarde.";
        var objMessage = `<div class="message-error">${message}</div>`;
        showDialog(objMessage);
    }
}

function showMessage(Message) {
    var objMessage = `<div class="message-success">${Message}</div>`;
    showDialog(objMessage);
}

function showInfo(Message) {
    var objMessage = `<div class="message-info">${Message}</div>`;
    showDialog(objMessage);
}

function showDialog(Message) {
    $("<div>").kendoDialog({ title: false, content: Message, visible: true, minWidth: 450, maxWidth: 700, buttonLayout: "normal", actions: [{ text: "Aceptar", primary: true }], close: function (e) { this.destroy(); } });
}

function showConfirm(message, actionAccept, actionCancel, titleAccept, titleCancel) {
    var textAccept = "Aceptar", textCancel = "Cancelar";
    if (titleAccept) textAccept = titleAccept;
    if (titleCancel) textCancel = titleCancel;
    $("<div>").kendoDialog({
        title: false, content: message, visible: true, buttonLayout: "normal",
        actions: [{ text: textCancel, action: actionCancel }, { text: textAccept, primary: true, action: actionAccept }],
        close: function (e) { this.destroy(); }
    });
}

function showNotification(title, message, type) {
    var popupNotification = $("#notification-message").data("kendoNotification");
    popupNotification.show({ title: title, message: message }, type);
    //popupNotification.show(message, type);
}

function hideNotification() {
    $("#notification-message").data("kendoNotification").hide();
}

//#endregion

//#region JQuery aggregate methods 

$.fn.extend({
    serializeObject: function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (this.name.startsWith("_") === false) {
                if (o[this.name] !== undefined) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    o[this.name].push(this.value || '');
                } else {
                    o[this.name] = this.value || '';
                }
            }
        });
        $(this).find('input[type="checkbox"]').each(function (i, obj) {
            o[obj.id] = obj.checked;
        });
        return o;
    },
    serializeToObject: function () {
        var o = {};
        $(this).find('input[type="hidden"], input[type="text"], input[type="number"], input[type="password"], input[type="checkbox"], input[type="radio"], select').each(function () {
            if ($(this).attr('type') == 'hidden') { //if checkbox is checked do not take the hidden field
                var $parent = $(this).parent();
                var $chb = $parent.find('input[type="checkbox"][name="' + this.name.replace(/\[/g, '\[').replace(/\]/g, '\]') + '"]');
                if ($chb != null) {
                    if ($chb.prop('checked')) return;
                }
            }
            if (this.name === null || this.name === undefined || this.name === '')
                return;
            var elemValue = null;
            if ($(this).is('select'))
                elemValue = $(this).find('option:selected').val();
            else {
                if ($(this).attr('type') === 'checkbox')
                    elemValue = $(this).prop("checked");
                else
                    elemValue = this.value;
            }
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(elemValue || '');
            } else {
                o[this.name] = $(this).attr('type') === 'checkbox' ? elemValue : elemValue || '';
            }
        });
        return o;
    },
    toggleHtml: function (a, b) {
        return this.html(this.html() === b ? a : b);
    }
});

$.fn.size = function () {
    return this.length;
}

//#endregion

//#region DATE METHODS

if (typeof JSON !== 'object') {
    JSON = {};
}

var reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/,
    reISO2 = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)$/,
    reISO3 = /^(\d{4})-(\d{2})-(\d{2})$/,
    reISO3_1 = /^(\d{4})\/(\d{2})\/(\d{2})$/,
    reISO4 = /^(\d{2})-(\d{2})-(\d{4})$/,
    reISO4_1 = /^(\d{2})\/(\d{2})\/(\d{4})$/,
    reMsAjax = /^\/Date\((d|-|.*)\)\/$/;

JSON.fromDate = function (p_objDate) {
    return "/Date(" + Date.parse(p_objDate) + ")/";
};

JSON.toDate = function (p_strDate) {
    var a = reISO.exec(p_strDate);
    if (a)
        return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]));
    a = reISO2.exec(p_strDate);
    if (a)
        return new Date(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]);
    a = reISO3.exec(p_strDate);
    if (a)
        return new Date(+a[1], +a[2] - 1, +a[3]);
    a = reISO3_1.exec(p_strDate);
    if (a)
        return new Date(+a[1], +a[2] - 1, +a[3]);
    a = reISO4.exec(p_strDate);
    if (a)
        return new Date(+a[3], +a[2] - 1, +a[1]);
    a = reISO4_1.exec(p_strDate);
    if (a)
        return new Date(+a[3], +a[2] - 1, +a[1]);
    a = reMsAjax.exec(p_strDate);
    if (a) {
        var b = a[1].split(/[-,.]/);
        return new Date(+b[0]);
    }
    return null;
};

JSON.toTime = function (timeString) {
    var reISOTime = /^(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)$/;
    var a = reISOTime.exec(timeString);
    if (a) {
        return new Date(0, 0, 0, +a[1], +a[2], +a[3]);
    }
    return null;
}

Date.prototype.addDays = function (value) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + value);
    return date;
}

Date.prototype.addMonths = function (value) {
    var d = new Date(this);
    var years = Math.floor(value / 12);
    var months = value - (years * 12);
    if (years) d.setFullYear(d.getFullYear() + years);
    if (months) d.setMonth(d.getMonth() + months);
    return d;
}

Date.prototype.addHours = function (h) {
    this.setTime(this.getTime() + (h * 60 * 60 * 1000));
    return this;
}

Date.prototype.toFormattedString = function () {
    return kendo.toString(this, "yyyy-MM-dd HH:mm:ss");
}

Date.prototype.toTimeFormattedString = function () {
    return kendo.toString(this, "hh:mm:ss");
}

//#endregion

//#region String Methods

String.prototype.toTitleCase = function () {
    return this.replace(/(^|\s)\S/g, function (t) { return t.toUpperCase() });
}

//#endregion

//#region Errors

function onReportError(event, jqxhr, settings, thrownError) {
    if (jqxhr.responseText) {
        if (jqxhr.responseText.includes("Document/refresh with ID") || jqxhr.responseText.includes("not found. Expired")) {
            var viewer = $("#reportViewer1").data("telerik_ReportViewer");
            if (viewer) viewer.refreshReport();
        }
    }
}

function onAjaxError(event, jqxhr, settings, thrownError) {
    if (jqxhr.responseText) {
        if (jqxhr.responseText.includes("HanaException: Connection failed")) {
            showError("Se ha producido un error al intentar acceder al servidor el SAP.");
        } else if (jqxhr.responseText.includes("Document/refresh with ID")) {
            showError("El Cliente de reporte requiere ser refrescado.");
        } else if (jqxhr.responseText === "" && jqxhr.status === 401) {
            showError("Al parecer se ha perdido su sesión, su página se recargará en unos segundos.");
            setTimeout(() => { window.location.reload(); }, 4000);
        } else {
            showError(`Error no determinado. Por favor recargue la página.<br />${thrownError}`);
        }
    } else {
        showError(`Error no determinado. Por favor recargue la página.<br />`);
    }
}

//#endregion

//#region Miscelanous

function getColors() {
    var lstColors = [];
    lstColors.push("#006d2c"); // 0
    lstColors.push("#045a8d"); // 1
    lstColors.push("#b30000"); // 2
    lstColors.push("#810f7c"); // 3
    lstColors.push("#2ca25f"); // 4
    lstColors.push("#2b8cbe"); // 5
    lstColors.push("#e34a33"); // 6
    lstColors.push("#8856a7"); // 7
    lstColors.push("#66c2a4"); // 8
    lstColors.push("#74a9cf"); // 9
    lstColors.push("#fc8d59"); //10
    lstColors.push("#8c96c6"); //11
    lstColors.push("#b2e2e2"); //12
    lstColors.push("#bdc9e1"); //13
    lstColors.push("#fdcc8a"); //14
    lstColors.push("#b3cde3"); //15
    lstColors.push("#edf8fb"); //16
    lstColors.push("#f1eef6"); //17
    lstColors.push("#fef0d9"); //18
    lstColors.push("#edf8fb"); //19
    lstColors.push("#b8bbc2"); //20
    return lstColors;
}

function formatHTMLSafe(data) {
    var result = "";
    if ($.trim(data) !== "") {
        result = data.replaceAll("&amp;lt;", "<").replaceAll("&lt;", "<").replaceAll("&amp;gt;", ">").replaceAll("&gt;", ">").replaceAll("&amp;nbsp;", "&nbsp;")
            .replaceAll("&amp;aacute;", "&aacute;").replaceAll("&amp;eacute;", "&eacute;").replaceAll("&amp;iacute;", "&iacute;").replaceAll("&amp;oacute;", "&oacute;").replaceAll("&amp;uacute;", "&uacute;")
            .replaceAll("&amp;ntilde;", "&ntilde;").replaceAll("&amp;amp;", "&").replaceAll("á", "&aacute;").replaceAll("é", "&eacute;").replaceAll("í", "&iacute;").replaceAll("ó", "&oacute;").replaceAll("ú", "&uacute;").replaceAll("ñ", "&ntilde;")
            .replaceAll("Á", "&Aacute;").replaceAll("É", "&Eacute;").replaceAll("Í", "&Iacute;").replaceAll("Ó", "&Oacute;").replaceAll("Ú", "&Uacute;").replaceAll("Ñ", "&Ntilde;");
    }
    return result;
}

function catchError(event, jqxhr, settings, thrownError) {
    if (jqxhr.responseText) {
        if (jqxhr.responseText.includes("Document/refresh with ID") || jqxhr.responseText.includes("not found") || jqxhr.responseText.includes("Expired")) {
            var viewer = $("#reportViewer1").data("telerik_ReportViewer");
            if (viewer) viewer.refreshReport();
        }
    }
}

//#endregion

//#region Notifications

function Notifications(full) {
    var strURL = urlNotifications;
    if (full) {
        strURL = urlAllNotifications;
    }
    var promo;
    $.post(strURL, {}, function (data) {
        if (data.message === "") {
            promo = $("#notifications");
            promo.empty();

            if (data.items.length > 0) {
                var divContent = $("<div>");
                promo.append(divContent);
                //var template = kendo.template($("#notificationTemplate").html());
                var template = kendo.template(`<div class="notifications">
            # if(data.length > 1) { #
            <div>
                <div class="tool">
                    <i class="fas fa-angle-left action tool-prior" title="Anterior"></i>
                    <i class="fas fa-angle-right action tool-next" title="Pr&oacute;ximo"></i>
                    <span class="title">&nbsp;&nbsp;#= data[0].name #</span>
                    <div class="right d-none d-sm-inline-block"></div>
                </div>
            </div>
            # } #
            <div class="row content-wrapper">
                <div class="content col-sm-8 col-md-8 col-lg-9">
                    # for(var i = 0; i < data.length; i++) { #
                    #   if (i === 0) { #
                    <div id="content-#= data[i].id #" class="content-item selected" title="#= data[i].name #">#= data[i].value #</div>
                    #   } else { #
                    <div id="content-#= data[i].id #" class="content-item" title="#= data[i].name #">#= data[i].value #</div>
                    #   } #
                    # } #
                </div>
                # if (data.length > 1) { #
                <div class="menu d-none d-sm-inline-block col-sm-4 col-md-4 col-lg-3">
                    # for (var i = 0; i < data.length; i++) { #
                    #   if (i === 0) { #
                    <div id="menu-#= data[i].id #" class="menu-item selected">#= data[i].name #</div>
                    #   } else { #
                    <div id="menu-#= data[i].id #" class="menu-item">#= data[i].name #</div>
                    #   } #
                    # } #
                </div>
                # } #
            </div>
        </div>`);
                var result = template(data.items);
                divContent.append(result);

                var intWindowH = $(window).height(), intWindowW = $(window).width();

                var intWidth = 830;
                if (data.items.length > 1) {
                    intWidth = 1090;
                }
                if (intWidth > (intWindowW + 40)) {
                    intWidth = intWindowW - 40;
                }
                if (promo.getKendoWindow()) {
                    promo.getKendoWindow().open();
                } else {
                    promo.kendoWindow({ title: "Comunicados DMC", visible: true, modal: true, iframe: false, width: intWidth, maxHeight: (intWindowH - 80) });
                }

                setTimeout(function () { $("#notifications").getKendoWindow().center(); }, 1050);
            } else {
                if (full) {
                    showMessage("No existen Notificaciones en este momento.");
                }
            }
        } else {
            showError(data.message);
        }
    });
}

//#endregion

//#region Security

function ValidateUser() {
    $.get(urlRequireLogOff, {}, function (d) {
        if (d) {
            window.location.href = urlLogOut;
        }
    });
}

//#endregion