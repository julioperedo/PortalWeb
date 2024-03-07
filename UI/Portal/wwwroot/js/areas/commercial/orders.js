//#region Variables Globales
var _minDate, _maxDate, sellerCode = "", seeAllClients = false, showMargin = false, showAsTable = false;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 0,
    seeMargin = $("#SeeMargin") === "Y", localUser = $("#LocalUser").val() === "Y";
//#endregion

//#region Eventos

$(document).ajaxError(catchError);

$(() => initForm());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$("#action-show-all").click(toggleShowAll);

$(".chk-state").click(changeState);

$(".more-options").click(e => $(e.currentTarget).toggleHtml('Más opciones', 'Menos opciones'));

$("#number").keyup(e => setEnableFilters($(e.target).val() === "", true, false));

$("#action-clean").click(e => setEnableFilters(true, true, true));

$("[name='search-by']").click(changeSearchType);

$("#action-filter").click(filterData);

$("#Listado").on("click", ".transport-id", openTransport);

$("#Listado").on("click", ".attach", openFilesList);

$("body").on("click", ".open-file", openFile);

$("#Listado").on("click", ".comment", openComments);

$("#Listado").on("click", ".doc-number", openReport);

$("#Listado").on("click", ".item-code", openProductStock);

$("body").on("click", ".reserved", openProductReserved);

$("#action-excel").on("click", ExportExcel);

$("body").on("click", ".sale-order", openSaleOrderReport);

$("#action-table-mode").click(changeToTableMode);

//#endregion

//#region Metodos Privados

function initForm() {
    setupControls();
    if (!localUser) {
        filterData();
    } else {
        setGridHeight("Listado", _gridMargin);
    }
}

function toggleShowAll() {
    var grid;
    showMargin = !showMargin;
    $(this).find("i").toggleClass("fa-eye-slash fa-eye");
    $("#Listado").toggleClass("hide-margin show-margin");
    $("#Listado").find(".k-grid").each((i, x) => {
        grid = $(x).data("kendoGrid");
        if (showMargin) {
            if ($(window).width() > 1199) grid.showColumn("margin0100");
        } else {
            grid.hideColumn("margin0100");
        }
        $(x).find("table").attr("style", "");
    });
    grid = $("#Listado").data("kendoGrid");
    if (!showMargin) {
        grid.hideColumn("margin0100");
    } else {
        grid.showColumn("margin0100");
    }
    grid.element.find("table").attr("style", "");
}

function setupControls() {
    var filSince = $("#initial-date").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#final-date").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    if (localUser) {
        $("#client").kendoDropDownList({
            dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", virtual: {
                itemHeight: 26, valueMapper: function (options) {
                    var items = this.dataSource.data();
                    var index = items.indexOf(items.find(i => i.code === options.value));
                    options.success(index);
                }
            },
            dataSource: { transport: { read: { url: urlClients } } }
        });
        sellerCode = $("#SellerCode").val();
        seeAllClients = $("#SeeAllClients").val() === "Y";
    }
    $("#seller").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", dataSource: { transport: { read: { url: urlSellers } } }, optionLabel: "Seleccione un Vendedor...", filter: "contains", enable: (seeAllClients | !localUser), value: sellerCode });
    $("#category").kendoDropDownList({ dataTextField: "name", dataValueField: "id", dataSource: { transport: { read: { url: urlCategories } } }, optionLabel: "Seleccione una Categoría...", filter: "contains" });
    $("#subcategory").kendoDropDownList({
        dataSource: { serverFiltering: true, transport: { read: { url: urlSubcategories, data: (e) => ({ CategoryId: e.filter.filters[0].value }) } } },
        cascadeFrom: "category", enable: false, optionLabel: "Seleccione una Subcategoría...", filter: "contains", dataTextField: "name", dataValueField: "id"
    });
    $("#line").kendoDropDownList({ dataTextField: "name", dataValueField: "name", dataSource: { transport: { read: { url: urlLines } } }, optionLabel: "Seleccione una Línea...", filter: "contains" });

    var subsidiaries1 = $("#subsidiary"), warehouses1 = $("#warehouse");
    warehouses1.multipleSelect({ placeholder: "Seleccione al menos un almacén..." });
    subsidiaries1.multipleSelect({
        placeholder: "Seleccione al menos una sucursal...",
        onUncheckAll: () => warehouses1.empty().multipleSelect("refresh").multipleSelect("disable"),
        onCheckAll: () => loadWarehouses(subsidiaries1, warehouses1),
        onClick: (view) => loadWarehouses(subsidiaries1, warehouses1)
    }).multipleSelect("checkAll");

    $("#items-complete").multipleSelect().multipleSelect("checkAll");

    $("#product-manager").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", dataSource: { transport: { read: { url: urlPMs } } }, optionLabel: "Seleccione un G. de Producto...", filter: "contains" });

    $.get(urlSubsidiaries, {}, function (data) {
        $.each(data.items, function (i, obj) {
            subsidiaries1.append(new Option(obj.name, obj.name));
        });
        subsidiaries1.multipleSelect("refresh");
    });

    var customColumns = [
        { title: "Sucursal", width: 120, field: "subsidiary", aggregates: ["count"], groupHeaderTemplate: e => `Sucursal: <span class="text-nowrap">${e.value}&nbsp;( Total: ${e.count},&nbsp;Monto Total: ${kendo.toString(e.total.sum, "N2")} )</span>`, media: "lg" },
        { title: "Almacen", width: 120, field: "warehouse", aggregates: ["count"], groupHeaderTemplate: e => `Almacén: <span class="text-nowrap">${e.value}&nbsp;( Total: ${e.count},&nbsp;Monto Total: ${kendo.toString(e.total.sum, "N2")} )</span>`, media: "lg" },
        { title: "Cliente", width: 250, field: "clientName", aggregates: ["count"], groupHeaderTemplate: e => `Cliente: <span class="text-nowrap">${e.value}&nbsp;( Total: ${e.count},&nbsp;Monto Total: ${kendo.toString(e.total.sum, "N2")} )</span>`, media: "lg" },
        { title: "Vendedor", width: 180, field: "sellerName", aggregates: ["count"], groupHeaderTemplate: e => `Vendedor: <span class="text-nowrap">${e.value}&nbsp;( Total: ${e.count},&nbsp;Monto Total: ${kendo.toString(e.total.sum, "N2")} )</span>`, media: "lg" },
        { title: "Fecha", width: 100, field: "docDate", format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter, media: "lg" },
        { title: "Estado", width: 80, field: "state", media: "lg" },
        { title: "Tipo", width: 120, field: "docType", media: "lg" },
        {
            title: "Número", width: 120, field: "docNumber", media: "lg", attributes: alignCenter, headerAttributes: alignCenter, template: e => {
                var result = e.series === "32" ? e.docNumber : `<a class="doc-number action action-link" data-type="${e.docType}" data-number="${e.docNumber}">${e.docNumber}</a>`;
                if (e.docType === "Nota de Venta" && e.subsidiary.toLowerCase() === "santa cruz") {
                    result += `&nbsp;( <a class="doc-number action action-link" data-type="Factura" data-billtype="${e.series}" data-number="${e.docNumber}">Factura</a> )`;
                }
                return result;
            }
        },
        { title: "O. Compra Cliente", field: "clientOrder", width: 200, media: "lg" },
        { field: "total", aggregates: ["sum"], title: "Total", width: 110, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => kendo.toString(e.total.sum, "N2"), media: "lg" },
        { field: "openAmount", aggregates: ["sum"], title: "Abierto", width: 110, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => kendo.toString(e.openAmount.sum, "N2"), media: "lg" },
        { field: "totalBilled", aggregates: ["sum"], title: "Facturado", width: 110, template: e => kendo.toString(e.total - e.openAmount, "N2"), attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => kendo.toString(e.total.sum - e.openAmount.sum, "N2"), media: "lg" },
        { field: "margin0100", aggregates: ["sum"], title: "Margen", width: 110, format: "{0:N2} %", attributes: alignRight, headerAttributes: alignRight, footerAttributes: alignRight, footerTemplate: e => `${kendo.toString(e.taxlessTotal.sum !== 0 ? 100 * (e.margin.sum / e.taxlessTotal.sum) : 0, "N2")} %`, media: "lg" },
        { field: "authorized", title: "Autorizado", width: 110, attributes: alignCenter, headerAttributes: { "style": "text-align: center; max-width: 90px;" }, template: e => e.authorized ? '<i class="fas fa-check"></i>' : '', media: "lg" },
        { field: "complete", title: "Completo", width: 100, attributes: alignCenter, headerAttributes: { "style": "text-align: center; max-width: 90px;" }, template: e => e.complete ? '<i class="fas fa-check"></i>' : '', media: "lg" },
        {
            field: "relatedDocs", title: "Doc. Relacionados", width: 180, media: "lg", template: function (e) {
                var content = "";
                if (e.relatedDocs.length > 0) {
                    var temp = [];
                    var list = Enumerable.From(e.relatedDocs).OrderBy("$.docType").Select("$").ToArray();
                    list.forEach(x => {
                        var type = x.docType === "Nota de Venta" ? "NV" : (x.docType === "Orden de Venta" ? "OV" : "NE");
                        temp.push(`${x.series === "32" ? x.docNumber : `<a class="doc-number action action-link" data-type="${x.docType}" data-number="${x.docNumber}">${type}-${x.docNumber}</a>`}${e.subsidiary.toLowerCase() === "santa cruz" && x.docType === "Nota de Venta" ? `&nbsp;( <a class="doc-number action action-link" data-type="Factura" data-billtype="${x.series}" data-number="${x.docNumber}">Factura</a> )` : ""}`);
                    });
                    content = temp.join("<br /> ");
                }
                return content;
            }
        },
        { title: " ", field: "header", width: 35, template: e => e.header !== "" || e.footer !== "" ? '<a class="comment action action-link" title="Cabecera y/o Pie de Orden"><i class="far fa-list-alt"></i></a>' : '', media: "lg" },
        { title: " ", firld: "files", width: 35, template: e => e.files.length > 0 && localUser ? '<a class="attach action action-link" title="Archivos adjuntos"><i class="fas fa-paperclip"></i></a>' : '', media: "lg" },
        { title: " ", field: "transport", width: 35, template: e => e.transport.length > 0 ? '<a class="action action-link transport-id" title="Datos de Transporte"><i class="fas fa-road"></i></a>' : '', media: "lg" }
    ];
    if ($(window).width() < 992) {
        customColumns.push({
            field: "id", title: "Detalle", media: "(max-width: 991px)", template: function (e) {
                var content = "";
                if (e.docType === "Orden de Venta") {
                    content = `<div class="row d-xl-none">
                                     <div class="col"><span class="label">Cliente:</span>&nbsp;&nbsp;${e.clientName}</div>
                                   </div>
                                   <div class="row d-xl-none">
                                     <div class="col"><span class="label">Vendedor:</span>&nbsp;&nbsp;${e.sellerName}</div>
                                   </div>
                                   <div class="row d-xl-none">
                                     <div class="col"><span class="label">Sucursal:</span>&nbsp;&nbsp;${e.subsidiary}</div>
                                     <div class="col text-nowrap"><span class="label">Almac&eacute;n:</span>&nbsp;&nbsp;${e.warehouse}</div>
                                   </div>
                                   <div class="row d-xl-none">
                                     <div class="col"><span class="label">Estado:</span>&nbsp;&nbsp;${e.state}</div>
                                     <div class="col text-nowrap"><span class="label">Fecha:</span>&nbsp;&nbsp;${kendo.toString(e.docDate, "dd-MM-yyyy")}</div>
                                   </div>
                                   <div class="row">
                                     <div class="col"><span class="label">N&uacute;mero:</span>&nbsp;&nbsp;<a class="doc-number action action-link" data-type="${e.docType}" data-number="${e.docNumber}">${e.docNumber}</a></div>
                                     <div class="col"><span class="label">Tipo:</span>&nbsp;&nbsp;<span class="text-nowrap">${e.docType}</span></div>
                                     <div class="col"><span class="label">Autorizado:</span>&nbsp;&nbsp;${e.authorized ? 'Si' : 'No'}&nbsp;&nbsp;&nbsp;&nbsp;${localUser ? `<span class="label">Completo:</span>&nbsp;&nbsp;${e.complete ? 'Si' : 'No'}` : ''}</div>
                                   </div>
                                   <div class="row">
                                     <div class="col"><span class="label">Total($us):</span>&nbsp;&nbsp;${kendo.toString(e.total, "N2")}</div>
                                     <div class="col"><span class="label">Abierto ($us):</span>&nbsp;&nbsp;${kendo.toString(e.openAmount, "N2")}</div>
                                     <div class="col"><span class="label">Facturado ($us):</span>&nbsp;&nbsp;${kendo.toString(e.total - e.openAmount, "N2")}</div>
                                     <div class="col"><span class="label margin">Margen:</span>&nbsp;&nbsp;<span class="margin">${kendo.toString(e.margin0100, "N2")} %</span></div>
                                   </div>
                                   <div class="row">
                                     <div class="col"><span class="label">Orden Compra Cliente:</span>&nbsp;&nbsp;${e.clientOrder}</div>
                                     <div class="col">
                                        ${(e.header !== "" || e.footer !== "") && localUser ? '<a class="comment btn btn-danger btn-sm" title="Cabecera y/o Pie">Cabecera y/o Pie <i class="far fa-list-alt"></i></a>&nbsp;&nbsp;&nbsp;&nbsp;' : ''}
                                        ${e.files.length > 0 && localUser ? '<a class="attach btn btn-danger btn-sm" title="Archivos adjuntos">Adjuntos <i class="fas fa-paperclip"></i></a>' : ''}
                                        ${e.transport.length > 0 ? '<a class="btn btn-danger btn-sm transport-id" title="Datos de Transporte">Transporte <i class="fas fa-road"></i></a>' : ''}
                                     </div>
                                   </div>`;
                }
                if (e.docType === "Nota de Venta" || e.docType === "Nota de Entrega") {
                    content = `<div class="row">
                                     <div class="col"><span class="label">N&uacute;mero:</span>&nbsp;&nbsp;${e.series === "32" ? e.docNumber : `<a class="doc-number action action-link" data-type="${e.docType}" data-number="${e.docNumber}">${e.docNumber}</a>`}${e.subsidiary.toLowerCase() === "santa cruz" && e.docType === "Nota de Venta" ? `&nbsp;<a class="doc-number action action-link" data-type="Factura" data-number="${e.docNumber}" data-billtype="${e.series}">Factura</a>` : ""}</div>
                                     <div class="col"><span class="label">Tipo:</span>&nbsp;&nbsp;<span class="text-nowrap">${e.docType}</span></div>
                                      <div class="col d-xl-none"><span class="label">Sucursal:</span>&nbsp;&nbsp;${e.subsidiary}</div>
                                     <div class="col d-xl-none text-nowrap"><span class="label">Almac&eacute;n:</span>&nbsp;&nbsp;${e.warehouse}</div>
                                     <div class="col"><span class="label">Total($us):</span>&nbsp;&nbsp;${kendo.toString(e.total, "N2")}</div>
                                     <div class="col"><span class="label margin">Margen:</span>&nbsp;&nbsp;<span class="margin">${kendo.toString(e.margin0100, "N2")} %</span></div>
                                   </div> `;
                    if ((e.files.length && localUser) || e.transport !== null) {
                        content += `<div class="row">
                                           <div class="col">
                                              ${e.files.length > 0 && localUser ? '<a class="attach btn btn-danger btn-sm" title="Archivos adjuntos">Adjuntos <i class="fas fa-paperclip"></i></a>' : ''}
                                              ${e.transport.length > 0 ? '<a class="btn btn-danger btn-sm transport-id" title="Datos de Transporte">Transporte <i class="fas fa-road"></i></a>' : ''}
                                           </div>
                                        </div>`;
                    }
                }
                if (e.relatedDocs.length > 0) {
                    var ids = Enumerable.From(e.relatedDocs).Select("$.docNumber").ToArray().join();
                    var grouped = Enumerable.From(e.relatedDocs).GroupBy("{ docType: $.docType }", null, function (k, g) {
                        var docNumbers = Enumerable.From(g).Select("{ number: $.docNumber, type: $.series }").Distinct().ToArray();
                        return { type: k.docType, items: docNumbers };
                    }, "$.docType").ToArray();
                    var numbers = '';
                    grouped.forEach(x => {
                        numbers += `<div class="related-doc ${x.type.toLowerCase().replaceAll(" ", "-")}">${x.type}: `;
                        var temp = [];
                        x.items.forEach(i => temp.push(`<a class="doc-number action action-link" data-type="${x.type}" data-number="${i.number}">${i.number}</a>${e.subsidiary.toLowerCase() === "santa cruz" && x.type === "Nota de Venta" ? `&nbsp;( <a class="doc-number action action-link" data-type="Factura" data-number="${i.number}" data-billtype="${i.type}">Factura</a> )` : ""}`));
                        numbers += `${temp.join(", ")}</div>`;
                    });

                    content += `<div class="row">
                                      <div class="col">${numbers}</div>
                                   </div>`;
                }
                return content;
            }, footerTemplate: function (e) {
                var content = "";
                var searchType = $("[name='search-by']:checked").val(), content = "";
                content = `<div class="row">
                                 <div class="col"><span class="label">Total($us):</span>&nbsp;&nbsp;${kendo.toString(e.total.sum, "N2")}</div>
                                 ${searchType === "SO" ? `<div class="col"><span class="label">Abierto ($us):</span>&nbsp;&nbsp;${kendo.toString(e.openAmount.sum, "N2")}</div>` : ``}
                                 ${searchType === "SO" ? `<div class="col"><span class="label">Facturado ($us):</span>&nbsp;&nbsp;${kendo.toString(e.total.sum - e.openAmount.sum, "N2")}</div>` : ``}
                                 <div class="col"><span class="label margin">Margen:</span>&nbsp;&nbsp;<span class="margin">${kendo.toString(e.taxlessTotal.sum !== 0 ? 100 * (e.margin.sum / e.taxlessTotal.sum) : 0, "N2")} %</span></div>
                               </div>`;
                return content;
            }
        });
    }

    $("#Listado").kendoGrid({
        columns: customColumns,
        groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" }, enabled: true },
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([]),
        dataBound: function (e) {
            var grid = e.sender;
            if ($(window).width() > 991) {
                for (var i = 0; i < grid.columns.length; i++) {
                    grid.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    grid.hideColumn($(v).data("field"));
                });
                var orderColumns = ["state", "clientOrder", "openAmount", "totalBilled", "authorized", "complete", "header"];
                if ($("[name='search-by']:checked").val() !== "SO") {
                    orderColumns.forEach(x => grid.hideColumn(x));
                } else {
                    orderColumns.forEach(x => grid.showColumn(x));
                }
                if (!showMargin) grid.hideColumn("margin0100");
            }
            grid.element.find("table").attr("style", "");
            e.sender.element.find(".k-group-col,.k-group-cell").css('width', $(window).width() < 768 ? 1 : 22);
        },
        detailInit: function (e) {
            $.get(urlDetail, { Subsidiary: e.data.subsidiary, Id: e.data.id, State: e.data.state, Type: e.data.docType }, function (data) {
                $("<div>").appendTo(e.detailCell).kendoGrid({
                    scrollable: false, sortable: true, pageable: false, selectable: true,
                    columns: [
                        { field: "itemCode", title: "Cod. Item DMC", width: 160, template: x => localUser ? `<a class="item-code action action-link">${x.itemCode}</a>` : x.itemCode, media: "xl" },
                        { field: "itemName", title: "Descripción", width: 350, media: "xl" },
                        { field: "warehouse", title: "Almacén", width: 120, media: "xl" },
                        { field: "line", title: "Línea", width: 200, media: "xl" },
                        { field: "quantity", title: "Cantidad", width: 90, attributes: alignRight, headerAttributes: alignRight, media: "xl" },
                        { field: "openQuantity", title: "Abierta", width: 90, attributes: alignRight, headerAttributes: alignRight, media: "xl" },
                        { field: "stock", title: "Stock", width: 90, attributes: alignRight, headerAttributes: alignRight, media: "xl" },
                        { field: "price", title: "Precio", width: 90, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, media: "xl" },
                        { field: "itemTotal", title: "Subtotal", width: 90, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight, media: "xl" },
                        { field: "margin0100", title: "Margen", width: 90, format: "{0:p}", attributes: alignRight, headerAttributes: alignRight, media: "xl" },
                        { field: "complete", title: "Completo", width: 90, template: '# if(complete || line === "DMC") {# <i class="fas fa-check"></i> #} #', attributes: alignCenter, headerAttributes: alignCenter, media: "xl" },
                        {
                            media: "(max-width: 1199px)", title: "Detalle", template: function (e) {
                                var content = `<div class="row">
                                                  <div class="col"><span class="label">Cod. Item DMC:</span>&nbsp;&nbsp;${e.itemCode}</div>
                                                  <div class="col-12"><span class="label">Descripci&oacute;n:</span>&nbsp;&nbsp;${e.itemName}</div>
                                                  <div class="col"><span class="label">Almac&eacute;n:</span>&nbsp;&nbsp;${e.warehouse}</div>
                                                  <div class="col text-nowrap"><span class="label">L&iacute;nea:</span>&nbsp;&nbsp;${e.line}</div>
                                                  <div class="col"><span class="label">Cantidad:</span>&nbsp;&nbsp;${e.quantity}</div>
                                                  ${$("[name='search-by']:checked").val() !== "SO" ? `` : `<div class="col"><span class="label">Abierta:</span>&nbsp;&nbsp;${e.openQuantity}</div>`}
                                                  ${localUser & $("[name='search-by']:checked").val() === "SO" ? `<div class="col"><span class="label">Stock:</span>&nbsp;&nbsp;${e.stock}</div>` : ``}
                                                  <div class="col"><span class="label">Precio:</span>&nbsp;&nbsp;${kendo.toString(e.price, "N2")}</div>
                                                  <div class="col"><span class="label">Subtotal:</span>&nbsp;&nbsp;${kendo.toString(e.itemTotal, "N2")}</div>
                                                  <div class="col margin"><span class="label">Margen:</span>&nbsp;&nbsp;${kendo.toString(e.margin0100 * 100, "N2")} %</div>
                                                  ${localUser & $("[name='search-by']:checked").val() === "SO" ? `<div class="col"><span class="label">Completo:</span>&nbsp;&nbsp;${e.complete || e.line === "DMC" ? "Si" : "No"}</div>` : ``}
                                               </div>`;
                                return content;
                            }
                        }
                    ], dataSource: data.items, dataBound: function (e) {
                        var grid = e.sender;
                        if (!localUser) {
                            grid.hideColumn("complete");
                            grid.hideColumn("stock");
                        }
                        if ($("[name='search-by']:checked").val() !== "SO") {
                            grid.hideColumn("openQuantity");
                            grid.hideColumn("stock");
                            grid.hideColumn("complete");
                        }
                        if (!showMargin) grid.hideColumn("margin0100");
                        grid.element.find("table").attr("style", "");
                    },
                    noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }
                });
            });
        }
    });

    $("#Detail").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "", resizable: false, width: 1200, actions: ["Close"], activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    });
    $("#Report").kendoWindow({
        visible: false, width: 1100, title: "", modal: true, activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    });
}

function changeState(e) {
    if (e.currentTarget.checked === false) {
        $(e.currentTarget.id === "state-open" ? "#state-close" : "#state-open").prop("checked", true);
    }
}

function changeSearchType(e) {
    if (e.target.parentElement.innerText === "Orden de Venta") {
        $("#state-open, #state-close").closest(".custom-switch").removeClass("d-none");
        $("#items-complete").parent().removeClass("d-none");
        $("label[for='items-complete']").removeClass("d-none");
        $("#client-order").parent().removeClass("d-none");
        $("label[for='client-order']").removeClass("d-none");
    } else {
        $("#state-open, #state-close").closest(".custom-switch").addClass("d-none");
        $("#items-complete").parent().addClass("d-none");
        $("label[for='items-complete']").addClass("d-none");
        $("#client-order").parent().addClass("d-none");
        $("label[for='client-order']").addClass("d-none");
    }
}

function openTransport(e) {
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var items = grd.dataItem(row).transport;

    var content = "", lstContent = [];
    items.forEach(x => {
        lstContent.push(`<div class="form-horizontal">
                             <div class="row">
                                 <span class="control-label col-md-2 font-weight-bold">No. de Guía :</span><div class="col-md-4 control-item"><span>${x.docNumber}</span></div>
                                 <span class="control-label col-md-2 font-weight-bold">Fecha :</span><div class="col-md-4 control-item"><span>${kendo.toString(JSON.toDate(x.date), "dd-MM-yyyy")}</span></div>
                             </div>
                             <div class="row">
                                 <span class="control-label col-md-2 font-weight-bold">Origen :</span><div class="col-md-4 control-item"><span>${x.source}</span></div>
                                 <span class="control-label col-md-2 font-weight-bold">Destino :</span><div class="col-md-4 control-item"><span>${x.destination}</span></div>
                             </div>
                             <div class="row">
                                 <span class="control-label col-md-2 font-weight-bold">Transporte :</span><div class="col-md-4 control-item"><span>${x.transporter}</span></div>
                                 <span class="control-label col-md-2 font-weight-bold">Entregar a :</span><div class="col-md-4 control-item"><span>${x.deliveryTo}</span></div>
                             </div>
                             <div class="row">
                                 <span class="control-label col-md-2 font-weight-bold text-nowrap">Observaciones :</span><div class="col-md-10 control-item"><span>${x.observations}</span></div>
                             </div>
                             <div class="row">
                                 <span class="control-label col-md-2 font-weight-bold">Peso(Kg) :</span><div class="col-md-4 control-item"><span>${kendo.toString(x.weight, "N1")}</span></div>
                                 <span class="control-label col-md-2 font-weight-bold">Cant. Piezas :</span><div class="col-md-4 control-item"><span>${x.quantityPieces}</span></div>
                             </div>
                             <div class="row">
                                 <span class="control-label col-md-2 text-nowrap font-weight-bold">Pago Entrega (Bs) :</span><div class="col-md-4 control-item"><span>${kendo.toString(x.remainingAmount, "N2")}</span></div>
                             </div>
                         </div>`);
    });
    content = lstContent.join("<hr />");
    var wnd = $("#Detail").data("kendoWindow");
    wnd.setOptions({ width: 790, title: "Datos de la Guía" });
    wnd.content(content).open();
}

function openFilesList(e) {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.setOptions({ width: 600, title: "Adjuntos" });
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var content = "";
    item.files.forEach(x => content += `<a href="#" data-path="${x.path}" data-name="${x.fileName}" data-ext="${x.fileExt}" class="open-file action-link"><span>${x.fileName}.${x.fileExt}</span></a><br />`);
    wnd.content(content);
    wnd.center().open();
}

function openFile(e) {
    var data = e.currentTarget.dataset;
    window.location.href = urlDownloadFile + "?" + $.param({ FilePath: data.path, FileName: data.name, FileExt: data.ext });
}

function openComments(e) {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.setOptions({ width: 600, title: "Comentarios" });
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var content = (item.header !== "" ? `<h5>Iniciales</h5><p>${item.header}</p>` : "") + (item.footer !== "" ? `<h5>Finales</h5><p>${item.footer}</p>` : "");
    wnd.content(content);
    wnd.center().open();
}

function openProductStock(e) {
    var itemCode = $(e.currentTarget).text();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var parentGrid = $(e.currentTarget).closest(".k-grid").parent().closest(".k-grid").data("kendoGrid");
    var parentRow = $(e.currentTarget).closest(".k-grid").parent().closest("tr").prev();
    parentGrid.select(parentRow);
    var parentItem = parentGrid.dataItem(parentRow);

    var lstItemCodes = ["FLETES", "ENVIO", "DMCSERVICIOS"];
    if (lstItemCodes.indexOf(itemCode) == -1) {
        var wnd = $("<div>").attr("id", setSafeName("detail-stock-" + itemCode)).kendoWindow({ width: 980, title: "Detalle Stock" }).data("kendoWindow");
        wnd.refresh({ url: urlGetStock, data: { Subsidiary: parentItem.subsidiary, Warehouse: item.warehouse, ItemCode: itemCode } });
        wnd.open().center();
    }
}

function openProductReserved(e) {
    var x = e.currentTarget.dataset;

    var table = $(e.currentTarget).closest(".table").parent().find(".items-reserved");
    var wnd = $("#detail-stock-" + setSafeName(x.itemcode)).data("kendoWindow");

    table.empty();
    $.get(urlReservedItems, { Subsidiary: x.subsidiary, Warehouse: x.warehouse, ItemCode: x.itemcode }, function (data) {
        if (data.message === "") {
            var extraTitle = x.subsidiary.toLowerCase() === "iquique" ? "<th>Autorizado</th><th>Correlativo</th>" : "";
            var content = `<thead class="thead-light"><tr><th scope="col">Ejecutivo</th><th scope="col">Cliente</th><th scope="col">Orden</th><th scope="col">Fecha</th><th scope="col" class="text-right">Cantidad</th><th scope="col" class="text-right">Precio</th>${extraTitle}<th>&nbsp;</th></thead><tbody>`;
            $.each(data.items, function (i, obj) {
                var extraData = x.subsidiary.toLowerCase() === "iquique" ? `<td class="text-center">${obj.authorized === "Y" ? '<i class="fas fa-check"></i>' : ''}</td><td>${obj.correlative}</td>` : "";
                content += `<tr><td>${obj.sellerName}</td><td><strong>${obj.clientCode}</strong> - ${obj.clientName}</td><td><a class="sale-order action action-link" data-subsidiary="${obj.subsidiary}" data-number="${obj.docNum}">${obj.docNum}</a></td><td>${kendo.toString(JSON.toDate(obj.docDate), "dd-MM-yyyy")}</td><td class="text-right">${obj.quantity}</td><td class="text-right">${obj.price !== null ? kendo.toString(obj.price, "N2") : ""}</td>${extraData}<td>`;
                if (obj.hasFiles) {
                    $.each(obj.files, function (j, oFile) {
                        content += `<a href="#" title="${oFile}" class="open-file" data-subsidiary="${obj.subsidiary}" data-code="${obj.docEntry}" data-file="${oFile}"><span class="glyphicon glyphicon-paperclip"></span></a>&nbsp;&nbsp;&nbsp;`;
                    });
                }
                content += `</td></tr>`;
            });
            content += '</tbody>';

            table.append(content);
            setTimeout(() => wnd.center(), 100);
        } else {
            showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
        }
    });
}

function loadWarehouses(subsidiaries, warehouses) {
    var objSelected = subsidiaries.multipleSelect("getSelects");
    if (objSelected && objSelected.length > 0) {
        var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
        $.get(urlWarehouses, { Subsidiary: strData }, function (data) {
            if (data.message !== "") {
                showError(data.message);
                warehouses.multipleSelect("disable");
            } else {
                warehouses.empty();
                if (data.items.length > 0) {
                    $.each(data.items, function (i, obj) {
                        warehouses.append(new Option(obj.name, obj.name));
                    });
                    warehouses.multipleSelect("enable");
                } else {
                    warehouses.multipleSelect("disable");
                }
                warehouses.multipleSelect("refresh");
            }
            warehouses.multipleSelect("uncheckAll");
        });
    } else {
        warehouses.multipleSelect("disable");
    }
}

function getFilters() {
    var message = "", clientCode = $(`#client`).val(), docNumber = $(`#number`).val(), initialDate = $(`#initial-date`).data("kendoDatePicker").value(), finalDate = $(`#final-date`).data("kendoDatePicker").value(),
        sellerCode = $(`#seller`).val(), state = $(`#state-open`).prop("checked") && $(`#state-close`).prop("checked") ? "" : $(`#state-open`).prop("checked") ? "O" : "C",
        productManager = $.trim($(`#product-manager`).val()) !== "" ? $(`#product-manager`).data("kendoDropDownList").text() : "",
        clientOrder = $(`#client-order`).val(), itemCode = $(`#product`).val(), category = $.trim($(`#category`).val()) !== "" ? $("#category").data("kendoDropDownList").text() : "",
        subcategory = $.trim($(`#subcategory`).val()) !== "" ? $(`#subcategory`).data("kendoDropDownList").text() : "", line = $(`#line`).val(),
        subsidiary = Enumerable.From($(`#subsidiary`).multipleSelect('getSelects')).Select("$").ToArray().join(),
        warehouse = Enumerable.From($(`#warehouse`).multipleSelect('getSelects')).Select("$").ToArray().join(),
        complete = Enumerable.From($(`#items-complete`).multipleSelect('getSelects')).Select("$").ToArray().join(), searchType = $("[name='search-by']:checked").val();

    if (initialDate) {
        initialDate = initialDate.toISOString();
    } else {
        if ($.trim(docNumber) === "") {
            if ((searchType === "SO" && (state === "" || state === "C")) || searchType === "SN" || searchType === "DN") {
                message += "- Fecha Inicial<br />";
            }
        }
    }
    if (finalDate) {
        finalDate = finalDate.toISOString();
    } else {
        if ($.trim(docNumber) === "") {
            if ((searchType === "SO" && (state === "" || state === "C")) || searchType === "SN" || searchType === "DN") {
                message += "- Fecha Final<br />";
            }
        }
    }

    return {
        message: message,
        data: {
            ClientCode: clientCode, DocNumber: docNumber, InitialDate: initialDate, FinalDate: finalDate, SellerCode: sellerCode, State: state, Complete: complete, ClientOrder: clientOrder, ItemCode: itemCode, Category: category,
            Subcategory: subcategory, Line: line, Subsidiary: subsidiary, Warehouse: warehouse, ProductManager: productManager, SearchType: searchType
        }
    };
}

function setSafeName(name) {
    return name.replace("(", "").replace(")", "").replace("#", "").replace("/", "");
}

function setEnableFilters(enabled, cleanFilters, cleanNumber) {
    var objSince = $("#initial-date").data("kendoDatePicker"), objUntil = $("#final-date").data("kendoDatePicker"), objClient = $("#client").data("kendoDropDownList"), objSeller = $("#seller").data("kendoDropDownList"), stateOpen = $("#state-open"), stateClose = $("#state-close"),
        pm = $("#product-manager").data("kendoDropDownList"), complete = $("#items-complete"), clientOrder = $("#client-order"), product = $("#product"), line = $("#line").data("kendoDropDownList"), category = $("#category").data("kendoDropDownList"),
        subcategory = $("#subcategory").data("kendoDropDownList"), subsidiary = $("#subsidiary"), warehouse = $("#warehouse");

    objSince.enable(enabled);
    objUntil.enable(enabled);
    if (localUser) objClient.enable(enabled);
    pm.enable(enabled);
    line.enable(enabled);
    category.enable(enabled);
    subcategory.enable(enabled);

    if (enabled) {
        objSeller.enable(seeAllClients | !localUser);
        stateOpen.removeAttr("disabled");
        stateClose.removeAttr("disabled");
        complete.multipleSelect("enable");
        clientOrder.removeAttr("disabled");
        product.removeAttr("disabled");
        subsidiary.multipleSelect("enable");
        warehouse.multipleSelect("enable");
    } else {
        objSeller.enable(enabled);
        stateOpen.attr("disabled", "disabled");
        stateClose.attr("disabled", "disabled");
        complete.multipleSelect("disable");
        clientOrder.attr("disabled", "disabled");
        product.attr("disabled", "disabled");
        subsidiary.multipleSelect("disable");
        warehouse.multipleSelect("disable");
    }

    if (cleanNumber) {
        $("#number").val("");
    }
    if (cleanFilters) {
        complete.multipleSelect("checkAll");
        subsidiary.multipleSelect("uncheckAll");
        warehouse.multipleSelect("uncheckAll");
        objSince.value("");
        objUntil.value("");
        if (localUser) {
            objClient.text("");
            objClient.value("");
        }
        if (seeAllClients | !localUser) objSeller.value("");
        pm.value("");
        line.value("");
        category.value("");
        subcategory.value("");
        stateOpen.prop("checked", true);
        stateClose.prop("checked", false);
        clientOrder.val("");
        product.val("");
    }
}

function getDataSource(items) {
    $.each(items, function (i, obj) {
        obj.docDate = JSON.toDate(obj.docDate);
    });
    var grouping = [
        { field: "subsidiary", dir: "asc", aggregates: [{ field: "subsidiary", aggregate: "count" }, { field: "total", aggregate: "sum" }, { field: "openAmount", aggregate: "sum" }] },
        { field: "warehouse", dir: "asc", aggregates: [{ field: "warehouse", aggregate: "count" }, { field: "total", aggregate: "sum" }] }
    ], agg = [
        { aggregate: "sum", field: "total" },
        { aggregate: "sum", field: "openAmount" },
        { aggregate: "count", field: "subsidiary" },
        { aggregate: "count", field: "warehouse" },
        { aggregate: "count", field: "clientName" },
        { aggregate: "count", field: "sellerName" },
        { aggregate: "sum", field: "taxlessTotal" },
        { aggregate: "sum", field: "margin" }
    ], sorting = [{ field: "subsidiary", dir: "asc" }, { field: "warehouse", dir: "asc" }, { field: "docDate", dir: "asc" }, { field: "docNumber", dir: "asc" }], schema = { model: { id: "id" } }, ds;
    ds = new kendo.data.DataSource({ data: items, aggregate: agg, group: grouping, sort: sorting, schema: schema });
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
    setTimeout(() => setGridHeight("Listado", _gridMargin), 300);

}

function openReport(e) {
    var data = e.currentTarget.dataset;
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var wnd = $("#Report").data("kendoWindow");
    loadReport(data.number, item.subsidiary, data.type, data.billtype);
    wnd.open();
}

function openSaleOrderReport(e) {
    var item = e.currentTarget.dataset;

    var wnd = $("#Report").getKendoWindow();
    wnd.title("Orden de Venta");
    loadReport(item.number, item.subsidiary, "Order");
    wnd.open().center();
}

function loadReport(Id, Subsidiary, Report, Series) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    if (Report === "Nota de Venta") {
        strReport = "SaleNote.trdp";
    }
    if (Report === "Nota de Entrega") {
        strReport = "DeliveryNote.trdp";
        objParams.SearchType = 2;
    }
    if (Report === "Factura") {
        switch (Series) {
            case "93":
                strReport = "ElectronicBill.trdp";
                break;
            case "32":
                strReport = "RentReceipt.trdp";
                break;
            default:
                strReport = "Bill.trdp";
        }
    }
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

function ExportExcel() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        var message = `<p class="font-weight-bold">¿Desea exportar una tabla agrupada o una tabla plana sin agrupaciones?</p>
                       <div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input" id="export-detailed"><label class="custom-control-label" for="export-detailed">Exportar Detallado</label></div>`;
        showConfirm(message, function (e) {
            var detailed = $("#export-detailed").prop("checked");
            var data = filtersData.data;
            data.Detailed = detailed;
            window.location.href = urlExcel + "?" + $.param(data);
        });
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filtersData.message}`);
    }
}

function filterData() {
    var filterData = getFilters();
    if (filterData.message === "") {
        $.get(urlFilter, filterData.data, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor.");
            }
        });
    } else {
        showInfo(`Se deben ingresar los siguientes campos: <br />${filterData.message}`);
    }
}

function changeToTableMode(e) {
    $(e.currentTarget).find("i").toggleClass("fa-table fa-th-list");
    showAsTable = !showAsTable;
}

//#endregion