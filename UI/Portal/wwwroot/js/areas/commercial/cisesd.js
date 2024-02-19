//#region Variables Globales
var _minDate, _maxDate, clients, products, exitProductsPermission, _enabledClient;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}",
    _gridMargin = 30, perActivities = $("#permission").val();
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-filter").click(filterData);

$("#action-clean").click(cleanFilters);

$("#Listado").on("click", ".action-new", onNew);

$("#Listado").on("click", ".action-edit", onEdit);

$("#Listado").on("click", ".action-mail", onSendingData);

$("#Listado").on("click", ".action-delete", onDelete);

$("#Listado").on("click", ".doc-number", openReport);

$("#purchase-license").click(onPurchase);

$("#send-data").click(onSendingData);

$("#generate-order").on("change", onMustGenerateChange);

$("#verify-order").click(onVerifyOrder);

$("#action-excel").click(exportExcel);

//#endregion

//#region Métodos Locales

function setupControls() {
    exitProductsPermission = $("#ProductExitPermission").val() === "Y";

    $("#detail").kendoWindow({ title: "Compra de Licencia a Microsoft", visible: false, scrollable: true, modal: true, width: 850, iframe: false, activate: onRefreshWindow });

    var filSince = $("#fil-since").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#fil-until").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    var today = new Date();
    filUntil.value(today);
    filSince.value(today.addDays(-7));

    var ddlFilterClient = $("#fil-client").kendoDropDownList({ dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", dataSource: { data: clients }, virtual: { itemHeight: 26, valueMapper: clientMapper } }).data("kendoDropDownList"),
        ddlClient = $("#client").kendoDropDownList({
            dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", dataSource: { data: clients }, virtual: { itemHeight: 45, valueMapper: clientMapper },
            template: '<span class="k-state-default">#: data.name.replace(data.code + " - ", "") #<p>#: data.code #</p></span>', change: function (e) {
                switch (this.value()) {
                    case "":
                        $(".col-type, .col-gen-order").addClass("d-none");
                        break;
                    case "CDMC-002":
                        $(".col-gen-order").removeClass("d-none");
                        if (exitProductsPermission) {
                            $(".col-type").removeClass("d-none");
                            ddlType.value("Orden de Venta");
                        } else {
                            $(".col-type").addClass("d-none");
                        }
                        break;
                    default:
                        $(".col-gen-order").removeClass("d-none");
                        $(".col-type").addClass("d-none");
                        $.get(urlValidateClient, { CardCode: this.value() }, function (d) {
                            if (d.message === "") {
                                _enabledClient = d.enabled && !d.onHoldDue && !d.onHoldCredit;
                                $("#generate-order").prop("checked", _enabledClient);
                                $("#generate-order").prop("disabled", !_enabledClient);
                                $("#purchase-license").prop("disabled", !_enabledClient);
                                $("#orderNumber").parent().toggleClass("d-none", _enabledClient);
                                $("label[for=orderNumber]").toggleClass("d-none", _enabledClient);
                                if (!_enabledClient) {
                                    showInfo(`Debe crear primero la OV y hacerla aprobar debido a lo siguiente: <br />${!d.enabled ? '- El cliente no está habilitado para compras Microsoft ESD<br />' : ''}${d.onHoldCredit ? '- El cliente está On Hold por crédito<br />' : ''}${d.onHoldDue ? '- El cliente está On Hold por pagos atrasados' : ''}`);
                                }
                            } else {
                                $("#purchase-license").prop("disabled", true);
                            }
                        });
                }
            }
        }).data("kendoDropDownList");
    $("#fil-type").kendoDropDownList();
    var ddlFilterProduct = $("#fil-product").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", dataSource: { data: products } }).data("kendoDropDownList"),
        ddlProduct = $("#product").kendoDropDownList({
            dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", dataSource: { data: products },
            template: '<span class="k-state-default">#: data.name.replace("( " + data.id + " ) ", "") #<p>#: data.id # &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Precio: #:kendo.toString(data.price, "N2")# $US</p></span>'
        }).data("kendoDropDownList");

    $("#Listado").kendoGrid({
        columns: [
            { title: "Número", width: 100, field: "code", attributes: alignCenter, headerAttributes: alignCenter },
            { title: "Cliente", field: "cardName", width: 220, attributes: { class: "text-nowrap" }, aggregates: ["count"], groupHeaderTemplate: 'Cliente: #= value #    ( Total: #= count# )' },
            { title: "Producto", width: 200, field: "productName", aggregates: ["count"], groupHeaderTemplate: 'Producto: #= value #    ( Total: #= count# )' },
            { title: "Fecha", field: "purchaseDate", attributes: alignCenter, headerAttributes: alignCenter, width: 90, format: dateFormat },
            { title: "Precio", field: "price", width: 80, attributes: alignRight, headerAttributes: alignRight },
            { title: "Cantidad", field: "quantity", width: 80, attributes: alignRight, headerAttributes: alignRight },
            //{ title: "O. Cliente", field: "clientOrderId", width: 120 },
            {
                title: "# Doc.", width: 120, field: "docNumber", headerAttributes: alignCenter, attributes: alignCenter,
                template: e => e.docNumber !== null && e.docType === "SO" ? `<a class="doc-number action action-link">${e.docNumber}</a>` : $.trim(e.docNumber)
            },
            {
                title: "Tipo Doc.", width: 120, field: "docType", template: e => e.docType === "SO" ? "Orden de Venta" : "Baja de Producto", aggregates: ["count"],
                groupHeaderTemplate: 'Tipo Doc: #= value === "SO" ? "Orden de Venta" : "Baja de Producto" # ( Total: #= count# )'
            },
            { title: "Enviado", width: 90, field: "sent", headerAttributes: alignCenter, attributes: alignCenter, template: e => e.sent ? '<i class="fas fa-check"></i>' : '' },
            {
                title: "Anulado", width: 90, field: "anulled", headerAttributes: alignCenter, attributes: alignCenter,
                template: e => e.anulled > 0 ? (e.anulled === e.quantity ? '<i title="Anulación Total" class="fas fa-check-double"></i>' : '<i title="Anulación Parcial" class="fas fa-check"></i>') : ''
            },
            {
                title: " ", width: 70, sortable: false, attributes: alignCenter, headerAttributes: alignCenter, sticky: true,
                headerTemplate: '<a class="action action-link action-new" title="Nuevo"><i class="fas fa-plus"></i></a>',
                template: function (e) {
                    var result = '<a class="action action-link action-mail" title="Enviar por Correo"><i class="fas fa-envelope"></i></a><a class="action action-link action-edit" title="Editar"><i class="fas fa-pen"></i></a>';
                    //if (!e.anulled && e.returnable)
                    //    result += '<a class="action action-link action-delete" title="Devolver Licencia"><i class="fas fa-trash"></i></a>';
                    //else
                    //    result += '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
                    return result;
                }
            }
        ],
        groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" }, enabled: true },
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([]),
        dataBound: function (e) {
            var grid = e.sender;
            grid.columns.forEach((v, i) => grid.showColumn(i));
            $("div.k-group-indicator").each((i, v) => grid.hideColumn($(v).data("field")));
            grid.element.find("table").attr("style", "");
        },
        detailInit: function (e) {
            $.get(urlDetail, { Id: e.data.id }, function (d) {
                if (d.message === "") {
                    d.items.forEach(x => x.returnDate = JSON.toDate(x.returnDate));
                    $("<div>").appendTo(e.detailCell).kendoGrid({
                        scrollable: false, sortable: true, pageable: false, selectable: true,
                        columns: [
                            { title: "Número", field: "number", width: 250 },
                            { title: "Secuencia", field: "sequence", width: 90 },
                            { title: "Tipo", field: "type", width: 80 },
                            { title: "Código", field: "code", width: 250 },
                            { title: "Fecha Devolución", field: "returnDate", width: 80, format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter }
                        ],
                        dataSource: d.items,
                        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }
                    });
                } else {
                    showError("Se ha producido un error al traer los datos de la compra.");
                }
            });
        }
    });

    var ddlType = $("#docType").kendoRadioGroup({
        items: ["Orden de Venta", "Baja de Mercadería"], layout: "horizontal", value: "Orden de Venta",
        change: e => $(".col-gen-order").toggleClass("d-none", e.newValue === "Baja de Mercadería")

    }).data("kendoRadioGroup");

    $("#Report").kendoWindow({
        visible: false, width: 1100, title: "", modal: true, activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    });

    $.get(urlProducts, {}, d => {
        products = d;
        ddlFilterProduct.setDataSource(d);
        ddlProduct.setDataSource(d);
    });
    $.get(urlClients, {}, d => {
        clients = d;
        ddlFilterClient.setDataSource(d);
        ddlClient.setDataSource(d);
    });
}

function clientMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.code === options.value));
    options.success(index);
}

function getDataSource(items) {
    items.forEach(x => x.purchaseDate = JSON.toDate(x.purchaseDate));
    var ds = new kendo.data.DataSource({
        data: items,
        sort: [{ field: "id", dir: "asc" }],
        schema: { model: { id: "id" } }
    });
    return ds;
}

function loadGrid(items) {
    var grd = $("#Listado").data("kendoGrid");
    var ds = getDataSource(items);
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
    } else {
        $("#action-excel").addClass("d-none");
    }
    setGridHeight("Listado", _gridMargin);
}

function getFilters() {
    var message = "", code = $("#fil-code").val(), clientCode = $("#fil-client").val(), type = $("#fil-type").val(), product = $("#fil-product").val(),
        initialDate = $(`#fil-since`).data("kendoDatePicker").value(), finalDate = $(`#fil-until`).data("kendoDatePicker").value();
    if (initialDate) {
        initialDate = kendo.toString(initialDate, "yyyy-MM-dd");
    } else {
        message += "- Fecha Inicial<br />";
    }
    if (finalDate) {
        finalDate = kendo.toString(finalDate, "yyyy-MM-dd");
    } else {
        message += "- Fecha Final<br />";
    }
    return {
        message: message,
        data: { Code: code, ClientCode: clientCode, Type: type, ProductId: product, InitialDate: initialDate, FinalDate: finalDate }
    };
}

function filterData() {
    hideNotification();
    var filter = getFilters();
    if (filter.message === "") {
        $.get(urlFilter, filter.data, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor.");
            }
        });
    } else {
        showInfo(`Los siguientes campos son necesarios: <br /> ${filter.message}`);
    }
}

function cleanFilters() {
    $("#fil-code").val("");
    $("#fil-client").data("kendoDropDownList").value("");
    $("#fil-type").data("kendoDropDownList").value("B");
    $("#fil-product").data("kendoDropDownList").value("");
    hideNotification();
    var today = new Date();
    $("#fil-since").data("kendoDatePicker").value(today.addDays(-7));
    $("#fil-until").data("kendoDatePicker").value(today);
}

function getTokenContent(item, withTabs) {
    var content = '';
    if (withTabs) {
        content += `<div class="custom-tab">
    <ul class="nav nav-tabs" role="tablist">
        <li class="nav-item"><a class="nav-link active show" data-toggle="tab" href="#tab-general-${item.id}" role="tab" aria-selected="true">Datos Compra</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#tab-emails-${item.id}" role="tab">Correos enviados</a></li>
    </ul>
    <div class="tab-content">
    	<div id="tab-general-${item.id}" class="tab-pane active show pt-3" role="tabpanel">`;
    }
    content += `<div class="row"><div class="col"><b>C&oacute;digo:</b> ${item.code}</div><div class="col"><b>Fecha:</b> ${kendo.toString(item.purchaseDate, "dd-MM-yyyy")}</div></div>
            <div class="row"><div class="col"><b>Cliente:</b> ( ${item.cardCode} ) ${item.cardName}</div></div>
            <div class="row"><div class="col"><b>Producto:</b> ${item.productName}</div></div>
            <div class="row"><div class="col"><b>OV SAP:</b>${$.trim(item.orderNumber)}</div></div>`;
    if ((item.tokens && item.tokens.length > 0) || (item.links && item.links.length > 0)) {
        content += `<a class="" data-toggle="collapse" href="#licences-detail-${item.id}" role="button" aria-expanded="false" aria-controls="licences-detail-${item.id}">Mostrar/ocultar detalle</a>`;
        var countTokens = item.tokens && item.tokens.length > 0 ? item.tokens.length : 0, countLinks = item.links && item.links.length > 0 ? item.links.length : 0;
        content += `<div id="licences-detail-${item.id}" class="collapse show">`;
        for (var i = 0; i < countTokens; i++) {
            var t = item.tokens[i], l = countLinks > 0 ? item.links[i] : null, bgColorClass = i % 2 === 1 ? 'bg-even' : '';
            content += `<div class="row"><div class="col"><div class="py-2 px-3 ${bgColorClass}"><b>C&oacute;digo:</b> ${t.code}<br /><b>Descripci&oacute;n:</b> ${t.description}<br />`;
            if (l) {
                content += `<a href="${l.uri}" class="action action-link">V&iacute;nculo <i class="fas fa-paperclip"></i></a>&nbsp;&nbsp;&nbsp;${l.description}&nbsp;&nbsp;&nbsp;<b>Expira:</b> ${l.expirationDate ? kendo.toString(l.expirationDate, "dd-MM-yyyy") : ""}<br />`;
            }
            content += `<b>No. Transacci&oacute;n:</b> ${t.transactionNumber}</div></div></div>`;
        }
        content += `</div>`;
    }
    if (withTabs) {
        content += `</div><div id="tab-emails-${item.id}" class="tab-pane pt-3" role="tabpanel">`;
        if (item.sentEmails && item.sentEmails.length > 0) {
            content += `<div class="row"><div class="col-12"><b>Correos Enviados</b></div><div class="col-12"><table class="table"><thead class="thead-light"><tr><th scope="col">Fecha</th><th scope="col">Nombre</th><th scope="col">E-Mail</th><th scope="col">Tipo</th></tr></thead><tbody>`;
            item.sentEmails.forEach(x => {
                content += `<tr><td scope="row">${kendo.toString(JSON.toDate(x.logDate), "dd-MM-yyyy HH:mm")}</td><td>${x.name}</td><td>${x.eMail}</td><td>${x.type === "E" ? "Alta" : "Baja"}</td></tr>`;
            });
            content += ` </tbody></table></div></div>`;
        } else {
            content += `<div class="row"><div class="col text-center pt-4 pb-4">No se han enviado correos todav&iacute;a.</div></div>`;
        }
        content += `</div></div></div>`;
    }
    return content;
}

function onEdit(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), content, div;
    grd.select(row);
    var div = $("<div>");
    $.get(urlGetPurchase, { Id: item.id }, d => {
        if (d.message === "") {
            item.tokens = d.tokens;
            item.links = d.links;
            item.anulled = d.anulled;
            item.sentEmails = d.sentEmails;
            content = getTokenContent(item, true);
        } else {
            console.error(d.message);
            content = `<div class="row"><div class="col">Se ha producido un error al traer los datos de la Orden</div></div>`;
        }
        div.append(content);
        $(div).kendoWindow({ actions: ["Close"], width: 900, title: `Orden Microsoft ${item.code}`, visible: true, activate: onRefreshWindow, close: onCloseDestroy });
        setTimeout(() => $(`#licences-detail-${item.id}`).collapse(), 1);
    });
}

function onNew(e) {
    $("#product").data("kendoDropDownList").value("");
    $("#client").data("kendoDropDownList").value("");
    $("#purchase-license").removeAttr("disabled");
    $("#purchase-license").removeClass("d-none");
    $("#group-send").addClass("d-none");
    $("#quantity").val(1);
    $(".col-type, .col-gen-order").addClass("d-none");
    $("#purchase-content").empty();
    $("#detail").data("kendoWindow").open();
}

function onPurchase(e) {
    e.preventDefault();
    var form = $(this).closest("form"), validator = form.kendoValidator().data("kendoValidator"), items = [], retries = 3, code = "";
    if (validator.validate()) {
        $("#purchase-license").attr("disabled", "disabled");
        var getCode = function (i) {
            $.get(urlGetProductToken, { ItemCode: itemCode, ClientCode: clientCode, PurchaseCode: code, OrderNumber: number, DocType: type }, function (d) {
                if (d.message === "") {
                    d.item.cardName = clientName;
                    d.item.productName = itemName;
                    d.item.purchaseDate = JSON.toDate(d.item.purchaseDate);
                    //console.log(d.item);
                    items.push(d.item);
                    code = d.item.code;
                    if (i > 1) {
                        getCode(i - 1);
                    } else {
                        var tokens = [], links = [];
                        items.forEach(function (x) {
                            x.tokens.forEach(i => tokens.push(i));
                            x.links.forEach(i => links.push(i));
                        });
                        d.item.tokens = tokens;
                        d.item.links = links;
                        $("#Listado").data("kendoGrid").dataSource.add(d.item);
                        $("#purchase-license").addClass("d-none");
                        $("#send-data").attr("data-id", d.item.id);
                        $("#group-send").removeClass("d-none");
                        $("#purchase-content").append(`Se han obtenido <b>${items.length}</b> licencias exitosamente. ¿Desea enviarlas por correo?`);
                        $("#purchase-content").append(getTokenContent(d.item, false));
                        $("#recipient-email").val(d.item.eMail);
                        $("#recipient-name").val(d.item.cardName);
                        adjustWindow($("#detail").data("kendoWindow"));
                        setTimeout(() => $(`#licences-detail-${d.item.id}`).collapse('hide'), 100);
                    }
                } else {
                    console.error(d.message);
                    if (retries > 0) {
                        retries -= 1;
                        getCode(i);
                    } else {
                        var msg = items.length > 0 ? `Sólo se pudo generar ${items.length} licencias.` : `No se pudo generar ninguna licencia`;
                        showError(`Se ha producido un error generar las Licencias, ${msg} `);
                        $("#purchase-license").removeAttr("disabled");
                    }
                }
            });
        };
        var ddlProduct = $("#product").data("kendoDropDownList"), itemCode = ddlProduct.value(), itemName = ddlProduct.text(), number = +$("#orderNumber").val(),
            ddlClient = $("#client").data("kendoDropDownList"), clientCode = ddlClient.value(), clientName = ddlClient.text(), quantity = +$("#quantity").val(),
            type = $("#docType").data("kendoRadioGroup").value() === "Orden de Venta" ? "SO" : "PE";
        showConfirm(`¿Est&aacute; seguro que desea realizar la compra de la(s) licencia(s) de <b>${itemName}</b>?`, () => getCode(quantity));
    }
}

function onSendingData(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row, item, id, email, name;
    if (grd) {
        row = $(e.currentTarget).closest("tr");
        item = grd.dataItem(row);
        id = item.id;
        grd.select(row);

        var form = `<form id="send-mail-${id}">
        <div id="group-send-${id}" class="input-group mb-3">
            <input type="email" id="recipient-email-${id}" class="form-control" placeholder="Correo del Destinatario" aria-label="Correo del Destinatario" aria-describedby="send-data" value="${item.eMail}" required>
            <input type="text" id="recipient-name-${id}" class="form-control" placeholder="Nombre del Destinatario" aria-label="Nombre del Destinatario" aria-describedby="send-data" value="${item.cardName}" required>
            <div class="input-group-append">
                <button class="btn btn-primary" type="button" id="send-data-${id}"><i class="fa fa-envelope"></i>&nbsp;Enviar por correo</button>
            </div>
        </div>
    </form>`;

        var div = $("<div>").attr("id", `send-mail-window-${id}`);
        div.append(form);
        $(div).kendoWindow({ actions: ["Close"], width: 900, title: `Enviar Orden Microsoft ${item.code} por correo`, visible: true, modal: true, activate: function (e) { this.center(); }, close: onCloseDestroy });

        $(`#send-data-${id}`).click(function (e) {
            e.preventDefault();
            var validator = $(`#send-mail-${id}`).kendoValidator({ messages: { required: "" } }).data("kendoValidator");
            if (validator.validate()) {
                email = $(`#recipient-email-${id}`).val();
                name = $(`#recipient-name-${id}`).val();
                sendingData(id, email, name, false);
            }
        });
    } else {
        id = e.currentTarget.dataset.id;
        var validator = $(e.currentTarget).closest("form").kendoValidator({ messages: { required: "" } }).data("kendoValidator");
        if (validator.validate()) {
            email = $("#recipient-email").val();
            name = $("#recipient-name").val();
            sendingData(id, email, name, true);
        }
    }
}

function sendingData(id, email, name, isInPurchase) {
    $.get(urlSendMail, { Id: id, EMail: email, Name: name }, d => {
        if (d.message === "") {
            var wName = isInPurchase ? `#detail` : `#send-mail-window-${id}`;
            $(wName).data("kendoWindow").close();
            showMessage("Se ha enviado el correo exitosamente.");
            var grid = $("#Listado").data("kendoGrid");
            var item = grid.dataSource.get(id);
            item.sent = true;
            grid.refresh();
        } else {
            console.error(d.message);
            showError("Se ha producido un error al enviar el correo.");
        }
    });
}

function onDelete(e) {
    var item = $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));
    var content = `<div class="row"><div class="col">¿Est&aacute; seguro que desea devolver la Licencia de <b>${item.productName}</b> con c&oacute;digo <b>${item.code}</b>?</div></div>
<div class="row"><div class="col pt-2"><label mb-1>Seleccione un c&oacute;digo:</label>`;

    $.get(urlGetTokens, { Id: item.id }, function (d) {
        if (d.message === "") {
            if (d.items && d.items.length > 0) {
                content += d.items.map(x => `<div class="form-check mt-1"><input class="k-radio" id="token-${x.id}" name="token" type="radio" value="${x.code}"><label class="k-radio-label" for="token-${x.id}">${x.code}</label></div>`);
            }
            content += `</div></div>`;

            showConfirm(content, function () {
                var checkeds = $("[name='token']:checked");
                if (checkeds.length > 0) {
                    var code = checkeds.val(), id = item.id;
                    $.get(urlReturnLicence, { LicenceCode: code, PurchaseId: id }, d => {
                        if (d.message === "") {
                            var grid = $("#Listado").data("kendoGrid");
                            var item = grid.dataSource.get(id);
                            item.anulled = true;
                            item.sent = true;
                            grid.refresh();

                            content = `<div class="row">
                                          <div class="col-12">Se ha realizado la devolución de la licencia <b>${code}</b> de <b>${item.productName}</b> correctamente.</div>
                                          <div class="col-12">C&oacute;digo de Transacci&oacute;n: <b>${d.item.serviceTransactionId}</b></div>
                                          <div class="col-12">C&oacute;digo de Transacci&oacute;n Cliente: <b>${d.item.clientTransactionId}</b></div>
                                       </div>`;
                            showMessage(content);
                            return true;
                        } else {
                            console.error(d.message);
                            showError("Se ha producido un error al anular la Orden de Microsoft.");
                            return false;
                        }
                    });
                } else {
                    return false;
                }
            });
        } else {
            showError(`Se ha producido un error al traer los Tokens del producto <b>${item.productName}</b>.`);
        }
    });
}

function adjustWindow(w) {
    var win = $(window);
    var margins = 80; //Asumimos 80 la suma de los márgenes por eje
    var height = w.wrapper.height(), width = w.wrapper.width();
    if ((win.height() <= (height + margins)) || (win.width() <= (width + margins))) {
        if (win.width() >= 992) { //Pantallas de computadoras
            if (win.height() <= (height + margins)) {
                height = win.height() - margins;
            }
            if (win.width() <= (width + margins)) {
                width = win.width() - margins;
            }
            w.element.css({ maxHeight: height, maxWidth: width });
            setTimeout(() => { w.center(); }, 100);
        } else {
            w.maximize();
        }
    } else {
        w.center();
    }
}

function onMustGenerateChange(e) {
    $("#orderNumber").parent().toggleClass("d-none", this.checked);
    $("label[for=orderNumber]").toggleClass("d-none", this.checked);
    $("#purchase-license").prop('disabled', !this.checked);
    if (this.checked) $("#orderNumber").val("");
}

function onVerifyOrder(e) {
    var number = +$("#orderNumber").val(), quantity = +$("#quantity").val(), itemCode = $("#product").data("kendoDropDownList").value(), clientCode = $("#client").data("kendoDropDownList").value(),
        form = $(e.currentTarget).closest("form"), validator = form.kendoValidator({ rules: { req: (i) => $.trim(i.val()) !== "" && $.trim(i.val()) !== "0" }, messages: { req: "" } }).data("kendoValidator");

    if (validator.validate()) {
        $.get(urlValidateOrder, { CardCode: clientCode, ItemCode: itemCode, Quantity: quantity, OrderNumber: number }, function (d) {
            $("#purchase-license").prop('disabled', !d.valid);
            if (d.message !== "") {
                showError(d.message);
            }
        });
    }
}

function exportExcel() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        window.location.href = urlExcel + "?" + $.param(filtersData.data);
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filtersData.message}`);
    }
}

function openReport(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var wnd = $("#Report").data("kendoWindow");
    loadReport(item.docNumber);
    wnd.open();
}

function loadReport(Id) {
    var objParams = { Subsidiary: "Santa Cruz", DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource({ report: strReport, parameters: objParams });
            viewer.refreshReport();
        } catch (e) {
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        $("#reportViewer1").telerik_ReportViewer({
            serviceUrl: urlService,
            reportSource: { report: strReport, parameters: objParams },
            viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
            scaleMode: telerikReportViewer.ScaleModes.SPECIFIC,
            scale: 1
        });
    }
}

//#endregion