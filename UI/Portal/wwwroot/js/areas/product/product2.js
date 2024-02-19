//#region Variables Globales
const marginGrid = 30, alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy}",
    priceVolumeTemplate = `<div class="row">
        <input id="pv-id-#:idSubsidiary#" type="hidden" value="#:id#" class="id" />
        <label for="pv-quantity-#:idSubsidiary#" class="col col-sm-2">Cantidad</label>
        <div class="col col-sm-4">
            <input id="pv-quantity-#:idSubsidiary#" name="pv-quantity-#:idSubsidiary#" type="number" value="#:quantity#" min="0" class="quantity form-control" />
        </div>
        <label for="pv-price-#:idSubsidiary#" class="col col-sm-2">Precio</label>
        <div class="col col-sm-4">
            <input id="pv-price-#:idSubsidiary#" name="pv-price-#:idSubsidiary#" class="price form-control" type="number" value="#:price#" min="0" />
        </div>
    </div>
    <div class="row">
        <label for="pv-observations-#:idSubsidiary#" class="col-12 col-sm-2">Observaciones</label>
        <div class="col col-sm-10">
            <textarea name="pv-observations-#:idSubsidiary#" class="form-control observations" id="pv-observations-#:idSubsidiary#" rows="3" cols="20" data-val="true" data-val-length-max="255" data-val-length="No debe exceder los 255 caracteres.">#:observations#</textarea>
        </div>
    </div>
    <div class="row">
        <div class="col text-right mt-3 mb-3">
            <button class="per-update btn btn-light cancel-volume-price" role="button" type="button">Cancelar</button>&nbsp;
            <button class="per-update btn btn-secondary save-volume-price" role="button" type="button">Guardar</button>
        </div>
    </div>`,
    lotTemplate = `<div class="row">
        <input id="Id" name="Id" type="hidden" value="#:id#" />
        <input id="IdProduct" name="IdProduct" type="hidden" value="#:idProduct#" />
        <label for="InitialQuantity" class="control-label col-2">Cantidad Inicial</label>
        <div class="col-4">
            <input id="InitialQuantity" name="InitialQuantity" type="number" value="#:initialQuantity#" min="0" />
        </div>
        <label for="Quantity" class="control-label col-2">Cantidad</label>
        <div class="col-4">
            <input id="Quantity" name="Quantity" type="number" value="#:quantity#" min="0" />
        </div>
    </div>
    <div class="row">
        <label for="InitialDate" class="control-label col-2">Desde</label>
        <div class="col-4">
            <input id="InitialDate" name="InitialDate" type="date" />
        </div>
        <label for="FinalDate" class="control-label col-2">Hasta</label>
        <div class="col-4">
            <input id="FinalDate" name="FinalDate" type="date" />
        </div>
    </div>
    <div class="row">
        <label for="Accelerator" class="control-label col-2">Acelerador</label>
        <div class="col-4">
            <input id="Accelerator" name="Accelerator" type="number" value="#:accelerator#" min="0" />
        </div>
        <div class="col-4">
            <div class="custom-control custom-switch">
                <input id="AccEnabled" name="AccEnabled" type="checkbox" class="custom-control-input selected" />
                <label class="custom-control-label" for="AccEnabled"> Habilitado</label>
            </div>
        </div>
    </div>
    <div class="row">
        <label for="Excluded" class="control-label col-2">Excluidos</label>
        <div class="col-9">
            <select id="Excluded" class="w-100"></select>
        </div>
    </div>
    <div class="row">
        <div class="col text-right pr-3 pb-3 pt-4">
            <button tabindex="0" class="per-update btn btn-light" id="cancel-lot" role="button" aria-disabled="false" type="button" data-role="button">Cancelar</button>&nbsp;&nbsp;
            <button tabindex="0" class="per-update btn btn-secondary" id="save-lot" role="button" aria-disabled="false" type="button" data-role="button">Guardar</button>
        </div>
    </div>`;
var _intId = 0, _minDate, _maxDate, _clients = [];
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", marginGrid));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", marginGrid));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", marginGrid));

$("#Listado").on("click", ".action-edit, .new-product", onProductEdit);

$("#Listado").on("click", ".action-delete", onProductDelete);

$("#Listado").on("click", ".action-sync", onProductSync);

$("#Listado").on("click", ".action-prices", onGettingPriceHistory);

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData());

$("#Detail").on("click", ".new-offer, .edit-offer, .delete-offer", onOfferEdit);

$("#Detail").on("click", ".cancel-offer, .save-offer", onOfferSave);

$("#Detail").on("click", ".new-volume-price, .edit-volume-price, .delete-volume-price", onEditVolumePrice);

$("#Detail").on("click", ".cancel-volume-price, .save-volume-price", onVolumePriceSave);

$("#save-product").click(onProductSave);

$("#cancel-product, #back-list").click(onProductCancel);

$("#Detail").on("click", "#DeletePhoto", onDeletePhoto);

$("#action-excel").click(() => ExportExcel());

$("#Detail").on("click", ".new-lot, .edit-lot, .delete-lot", onLotEdit);

$("#Detail").on("click", "#cancel-lot, #save-lot", onLotSave);

//#endregion

//#region Métodos Locales

function setupControls() {
    $("#FilEnabled").kendoDropDownList();
    $("#FilOffers").kendoDropDownList();
    $("#FilLine").kendoDropDownList({
        dataSource: { transport: { read: { url: urlLines } } },
        optionLabel: "Seleccione una Línea...",
        filter: "contains",
        dataTextField: "name",
        dataValueField: "id"
    });
    $("#FilCategory").kendoDropDownList({
        dataSource: { serverFiltering: true, transport: { read: { url: urlCategories, data: function (e) { return { LineId: $("#FilLine").val() } } } } },
        cascadeFrom: "FilLine",
        optionLabel: "Seleccione una Categoría...",
        filter: "contains",
        dataTextField: "name",
        dataValueField: "name"
    });
    $("#Listado").kendoGrid({
        columns: [
            { field: "line", title: "Line", hidden: true, aggregates: ["count"], groupHeaderTemplate: "Línea: #= value #    ( Total: #= count# )" },
            { field: "category", title: "Category", hidden: true, aggregates: ["count"], groupHeaderTemplate: "Categoría: #= value #    ( Total: #= count# )" },
            { field: "itemCode", title: "Item", width: 150 },
            { field: "name", title: "Nombre", width: 255 },
            { field: "enabled", title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 90, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #' },
            { field: "isDigital", title: "Digital", attributes: alignCenter, headerAttributes: alignCenter, width: 70, template: '# if(isDigital) {# <i class="fas fa-check"></i> #} #' },
            {
                title: "Precios", headerAttributes: alignCenter, columns: [
                    { field: "santaCruz", title: "Santa Cruz", attributes: alignRight, headerAttributes: alignRight, width: 90, format: "{0:N2}" },
                    { field: "miami", title: "Miami", attributes: alignRight, headerAttributes: alignRight, width: 75, format: "{0:N2}" },
                    { field: "iquique", title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 80, format: "{0:N2}" }
                ]
            },
            {
                title: "Stock", headerAttributes: alignCenter, columns: [
                    { field: "stockSantaCruz", title: "Santa Cruz", attributes: alignRight, headerAttributes: alignRight, width: 105, template: "#=stockSantaCruz#" },
                    { field: "stockMiami", title: "Miami", attributes: alignRight, headerAttributes: alignRight, width: 105, template: "#=stockMiami#" },
                    { field: "stockIquique", title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 105, template: "#=stockIquique#" }
                ]
            },
            {
                title: "Mostrar", headerAttributes: alignCenter, columns: [
                    { field: "showAlways", title: "Siempre", attributes: alignCenter, headerAttributes: alignCenter, width: 70, template: '# if(showAlways) {# <i class="fas fa- check"></i> #} #' },
                    { field: "showInWeb", title: "en Web", attributes: alignCenter, headerAttributes: alignCenter, width: 70, template: '# if(showInWeb) {# <i class="fas fa-check"></i> #} #' },
                    { field: "showTeamOnly", title: "Sólo DMC", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(showTeamOnly) {# <i class="fas fa-check"></i> #} #' }
                ]
            },
            { field: "imageURL", title: " ", attributes: alignCenter, width: 30, template: '# if(imageURL != null && $.trim(imageURL) != "") {# <i class="fas fa-image" title="Tiene Imagen"></i> #} #' },
            {
                title: " ", attributes: alignCenter, width: 60, sortable: false, headerAttributes: alignCenter,
                template: '<a class="action action-link action-sync" title="Sincronizar Producto"><i class="fas fa-sync-alt"></i></a><a class="action action-link action-prices" title="Historial de Precios"><i class="fas fa-dollar-sign"></i></a>'
            },
            {
                field: "itemCode", title: " ", attributes: alignCenter, width: 60, sortable: false, headerAttributes: alignCenter, sticky: true,
                headerTemplate: '<a class="action action-link new-product" title="Nuevo Producto"><i class="fas fa-plus"></i></a>',
                template: '<a class="action action-link action-edit" title="Editar Producto"><i class="fas fa-pen"></i></a><a class="action action-link action-delete" title="Eliminar Producto"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", scrollable: { height: "200px" },
        messages: { noRecords: '<div class="w-100 text-center p-3">No hay registros para el criterio de b&uacute;squeda.</div>' },
        dataSource: {
            group: [
                { field: "line", dir: "asc", aggregates: [{ field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }] },
                { field: "category", dir: "asc", aggregates: [{ field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }] }
            ],
            aggregate: [{ field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }],
            schema: { data: "Data", total: "Total", errors: "Errors", model: { id: "id" } },
            data: []
        }
    });
    var ms = $("#FilProduct").magicSuggest({ hideTrigger: true, placeholder: "Escriba su criterio de búsqueda, separado por comas si desea ingresar más de uno" });
    $(ms).on('keydown', function (e, m, v) {
        if (v.keyCode == 13) {
            filterData();
        }
    });
    setGridHeight("Listado", marginGrid);

    //COntroles del Formulario
    $("#Line").kendoDropDownList({ dataSource: { transport: { read: { url: urlSAPLines } } }, optionLabel: "Seleccione una Línea...", filter: "contains", dataTextField: "name", dataValueField: "name" });
    $("#Category").kendoDropDownList({ dataSource: { transport: { read: { url: urlSAPCategories } } }, optionLabel: "Seleccione una Categoría...", filter: "contains", dataTextField: "name", dataValueField: "name" });
    $("#SubCategory").kendoDropDownList({
        dataSource: { serverFiltering: true, transport: { read: { url: urlSAPSubcategories, data: getCategoryValue } } },
        cascadeFrom: "Category", optionLabel: "Seleccione una subcategoría...", filter: "contains", dataTextField: "name", dataValueField: "name"
    });
    $("#ExtraComments").kendoEditor({
        resizable: { content: true },
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        messages: { dialogCancel: "Cancelar", dialogInsert: "Agregar", dialogUpdate: "Aceptar", imageAltTex: "Texto Alternativo", imageWebAddress: "Dirección Imagen", insertImage: "Insertar Imagen", viewHtml: "Ver HTML" }
    });

    $.get(urlClients, {}, (d) => _clients = d);
}

function cleanFilters() {
    $("#FilProduct").val("");
    $("#FilEnabled").data("kendoDropDownList").value("");
    $("#FilOffers").data("kendoDropDownList").value("");
    $("#FilLine").data("kendoDropDownList").value("");
    $("#FilCategory").data("kendoDropDownList").value("");
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
    setGridHeight("Listado", marginGrid);
}

function filterData() {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        $.get(urlFilter, filtersData.data, function (data) {
            if (data.message !== "") {
                showError(data.message);
                loadGrid([]);
            } else {
                loadGrid(data.items);
            }
        });
    } else {
        showInfo(`Debe seleccionar los siguientes campos: <br />${filtersData.message}`);
    }
}

function getDataSource(items) {
    return new kendo.data.DataSource({
        data: items, pageSize: 500, group: [
            { field: "line", dir: "asc", aggregates: [{ field: "line", aggregate: "count" }] },
            { field: "category", dir: "asc", aggregates: [{ field: "category", aggregate: "count" }] }
        ]
    });
}

function getFilters() {
    $("#FilProduct").blur();
    var product = $("#FilProduct").magicSuggest().getValue().join(), enabled = $("#FilEnabled").data("kendoDropDownList").value(), offers = $("#FilOffers").data("kendoDropDownList").value(),
        line = $("#FilLine").data("kendoDropDownList").value(), category = $("#FilCategory").data("kendoDropDownList").value(), message = "";
    if (enabled === "" && product === "" && offers === "" && line === "" && category === "") {
        message += "- Al menos un criterio de búsqueda.";
    }
    return { message: message, data: { Name: product, LineId: line, CategoryId: category, Enabled: enabled, Offer: offers } };
}

function onProductEdit(e) {
    loadProduct($(e.currentTarget).hasClass("action-edit") ? $("#Listado").data("kendoGrid").dataItem($(e.currentTarget).closest("tr")) : { editable: true, enabled: true, line: "", category: "", subcategory: "", showInWeb: true });
}

function onProductCancel(e) {
    e.preventDefault();
    $("#Detail").addClass("d-none");
    $("#Listado").removeClass("d-none");
    $("#page-title").text("Configuración de Productos");
    $("#back-list").addClass("d-none");
    $("#action-excel, #toggle-filter-box").removeClass("d-none");
}

function loadProduct(item) {
    $("#filter-box").collapse('hide');
    $("#Listado").addClass("d-none");
    $("#Detail").removeClass("d-none")
    $("#page-title").text("Detalle de Producto");
    $("#back-list").removeClass("d-none");
    $("#action-excel, #toggle-filter-box").addClass("d-none");

    var ddlLine = $("#Line").getKendoDropDownList(), ddlCategory = $("#Category").getKendoDropDownList(), ddlSubcategory = $("#SubCategory").getKendoDropDownList();
    $("#Id").val(item.id);
    $("#ItemCode").val(item.itemCode);
    $("#Name").val(item.name);
    $("#Editable").prop("checked", item.editable);
    $("#Enabled").prop("checked", item.enabled);
    $("#ShowInWeb").prop("checked", item.showInWeb);
    $("#ShowAlways").prop("checked", item.showAlways);
    $("#ShowTeamOnly").prop("checked", item.showTeamOnly);
    $("#IsDigital").prop("checked", item.isDigital);
    $("#Description").val(item.description);
    $("#Commentaries").val(item.commentaries);
    $("#Link").val(item.link);
    $("#Warranty").val(item.warranty);
    $("#Consumables").val(item.consumables);
    ddlLine.value(item.line);
    ddlCategory.value(item.category);
    ddlSubcategory.value(item.subCategory);
    $("#ExtraComments").getKendoEditor().value(formatHTMLSafe(item.extraComments));
    $("#ImageURL").val(item.imageURL);
    $("#Photo").attr("src", $.trim(item.imageURL) === "" ? urlNoImage : `${urlImages}${$.trim(item.imageURL)}`);
    $("#DeletePhoto").toggleClass("d-none", $.trim(item.imageURL) === "");

    $("#ItemCode, #Name, #Enabled, #Description, #Commentaries, #Warranty, #Consumables, #DeletePhoto").attr("disabled", !item.editable);
    ddlLine.enable(item.editable);
    ddlCategory.enable(item.editable);
    ddlSubcategory.enable(item.editable);
    $("#files").getKendoUpload().enable(item.editable);

    var ulPrices = $("#tab-prices").find("ul"), contentPrices = $("#tab-prices").find(".tab-content"),
        ulVolumePrices = $("#tab-volume-prices").find("ul"), contentVolumePrices = $("#tab-volume-prices").find(".tab-content");
    ulPrices.empty();
    contentPrices.empty();
    ulVolumePrices.empty();
    contentVolumePrices.empty();
    $.get(urlEdit, { Id: item.id, ItemCode: item.itemCode }, function (d) {
        if (d.message === "") {
            d.prices.forEach((v, i) => {
                var clsActive = i === 0 ? 'active show' : '', attActive = i === 0 ? 'aria-selected="true"' : '', stockContent = "", fifoContent = "";
                ulPrices.append(`<li class="nav-item"><a class="nav-link ${clsActive}" data-toggle="tab" href="#tab-price-${v.idSubsidiary}" role="tab" ${attActive}>${v.subsidiary}</a></li>`);

                if (v.stockDetail.length > 0) {
                    stockContent = `<table class="table table-condensed table-striped w-100 mt-4" style="font-size: 0.9em;">
                <thead><tr><th>Almac&eacute;n</th><th class="text-right">Stock</th><th class="text-right">Reserva</th><th class="text-right">Pedido</th></tr></thead>`;
                    v.stockDetail.forEach(sd => {
                        stockContent += `<tr><td>${sd.warehouse}</td><td class="text-right">${sd.stock}</td><td class="text-right">${sd.reserve}</td><td class="text-right">${sd.transit}</td></tr>`;
                    });
                    stockContent += '</table>';
                }
                if (v.fifoDetail.length > 0) {
                    fifoContent = `<table class="table table-condensed table-striped w-100 mt-4" style="font-size: 0.9em;">
                <thead><tr><th>Almac&eacute;n</th><th class="text-right">Stock</th><th class="text-right">Costo</th><th class="text-right">Total</th><th class="text-center">Fecha</th></tr></thead>`;
                    v.fifoDetail.forEach(fd => {
                        fifoContent += `<tr>
                    <td>${fd.warehouse}</td>
                    <td class="text-right">${fd.stock}</td>
                    <td class="text-right">${fd.realCost ? kendo.toString(fd.realCost, "N2") : ""}</td>
                    <td class="text-right">${fd.total ? kendo.toString(fd.total, "N2") : ""}</td>
                    <td class="text-center">${fd.date ? kendo.toString(JSON.toDate(fd.date), "dd-MM-yyyy") : ""}</td>
                </tr>`;
                    });
                    fifoContent += '</table>';
                }

                contentPrices.append(`<div class="price-subsidiary tab-pane ${clsActive} pt-3" id="tab-price-${v.idSubsidiary}" role="tabpanel">
    <input type="hidden" class="id-subsidiary" value="${v.idSubsidiary}" />
    <input type="hidden" class="id" value="${v.id}" />
    <div class="row">
        <div class="col-4 col-md-2"><label for="regular-price-${v.idSubsidiary}" class="control-label">Regular</label></div>
        <div class="col-8 col-md-4"><input id="regular-price-${v.idSubsidiary}" type="number" value="${v.regular ?? ""}" min="0" class="regular-price form-control" /></div>
        <div class="col-4 col-md-2"><label for="client-suggested-${v.idSubsidiary}" class="control-label">Sugerido Cliente</label></div>
        <div class="col-8 col-md-4"><input id="client-suggested-${v.idSubsidiary}" type="number" value="${v.clientSuggested ?? ""}" class="client-suggested form-control" /></div>
    </div>
    <div class="row">
        <div class="col-4 col-md-2"><label for="observations-${v.idSubsidiary}" class="control-label">Observaciones</label></div>
        <div class="col-8 col-md-10"><input id="observations-${v.idSubsidiary}" type="text" value="${v.observations ?? ""}" class="form-control observations" /></div>
    </div>
    <div class="row">
        <div class="col-4 col-md-2"><label for="commentaries-${v.idSubsidiary}" class="control-label">Comentarios Internos</label></div>
        <div class="col-8 col-md-10"><input id="commentaries-${v.idSubsidiary}" type="text" value="${v.commentaries ?? ""}" class="form-control commentaries" /></div>
    </div>
    <div class="row">
        <div class="col pt-3 pb-3">
            <h5>Precios de Oferta</h5>
            <div class="price-offers"></div>
            <div class="card detail-offer d-none mt-2">
                <div class="card-header">Detalle</div>
                <div class="card-body">
                    <input type="hidden" class="id-offer" />
                    <input type="hidden" class="id-subsidiary" />
                    <div class="row">
                        <div class="col-6"><h5>Detalle Oferta</h5></div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <label for="po-price-${v.idSubsidiary}">Precio</label>
                            <input type="number" class="po-price form-control" id="po-price-${v.idSubsidiary}" min="0" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="po-description-${v.idSubsidiary}">Descripci&oacute;n</label>
                            <textarea type="number" class="po-description form-control" id="po-description-${v.idSubsidiary}" rows="3"></textarea>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label for="po-since-${v.idSubsidiary}" class="mr-2">Desde</label>
                            <input id="po-since-${v.idSubsidiary}" type="date" class="po-since" />
                        </div>
                        <div class="col">
                            <label for="po-until-${v.idSubsidiary}" class="mr-2">Hasta</label>
                            <input id="po-until-${v.idSubsidiary}" type="date" class="po-until" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input selected po-enabled" id="po-enabled-${v.idSubsidiary}" />
                                <input name="po-enabled-${v.idSubsidiary}" type="hidden" value="false" />
                                <label class="custom-control-label" for="po-enabled-${v.idSubsidiary}"> Habilitado</label>
                            </div>
                        </div>
                        <div class="col">
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input selected po-only-with-stock" id="po-only-with-stock-${v.idSubsidiary}" />
                                <input name="po-only-with-stock-${v.idSubsidiary}" type="hidden" value="false" />
                                <label class="custom-control-label" for="po-only-with-stock-${v.idSubsidiary}"> Deshabilitar sin stock</label>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col text-right pb-2">
                            <button class="btn btn-light cancel-offer">Cancelar Edici&oacute;n</button>
                            <button class="btn btn-secondary save-offer">Guardar Oferta</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-12 col-sm-6">${stockContent}</div>
        <div class="col-12 col-sm-6">${fifoContent}</div>
    </div>
</div>`);

            });

            var filSince = $(".po-since").kendoDatePicker({
                format: "d/M/yyyy", change: function (e) {
                    var startDate = this.value();
                    if (startDate === null) this.value("");
                    filUntil.min(startDate ? startDate : _minDate);
                }
            }).data("kendoDatePicker");
            var filUntil = $(".po-until").kendoDatePicker({
                format: "d/M/yyyy", change: function (e) {
                    var endDate = this.value();
                    if (endDate === null) this.value("");
                    filSince.max(endDate ? endDate : _maxDate);
                }
            }).data("kendoDatePicker");

            _maxDate = filUntil.max();
            _minDate = filSince.min();

            $(".price-subsidiary").each(function (i, obj) {
                var id = +$(obj).find(".id").val(), offerItems = d.prices.find(x => x.id === id).offers;
                offerItems.forEach(x => {
                    if (x.since) x.since = JSON.toDate(x.since);
                    if (x.until) x.until = JSON.toDate(x.until);
                });
                $(obj).find(".price-offers").kendoGrid({
                    sortable: true, scrollable: { height: 200 }, selectable: "Single, Row",
                    noRecords: { template: '<div class="w-100 p-2">No hay precios de oferta para esta sucursal.</div>' },
                    columns: [
                        { title: "Precio", field: "price", width: 130, attributes: alignRight, headerAttributes: alignRight, format: "{0:N2}" },
                        { title: "Desde", field: "since", width: 120, format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter },
                        { title: "Hasta", field: "until", width: 120, format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter },
                        { field: "onlyWithStock", title: "Eliminar sin stock", attributes: alignCenter, headerAttributes: alignCenter, width: 140, template: e => e.onlyWithStock ? '<i class="fas fa-check"></i>' : '' },
                        { field: "enabled", title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 110, template: e => e.enabled ? '<i class="fas fa-check"></i>' : '' },
                        {
                            title: "", attributes: alignCenter, headerAttributes: alignCenter, headerAttributes: alignCenter, width: 60,
                            template: '<i class="fas fa-pen action action-link edit-offer" title="Editar Precio de Oferta"></i><i class="fas fa-trash-alt action action-link delete-offer" title="Eliminar Precio de Oferta"></i>',
                            headerTemplate: '<i class="fas fa-plus action action-link new-offer" title="Nuevo Precio de Oferta"></i>'
                        }
                    ],
                    dataSource: {
                        filter: [{ logic: "and", filters: [{ field: "statusType", operator: "neq", value: 3 }] }],
                        schema: { model: { id: "id" } },
                        data: offerItems
                    }
                });
            });

            if (d.externalSite !== "") {
                ulPrices.append(`<li class="nav-item"><a class="nav-link" data-toggle="tab" href="#tab-price-external" role="tab">${d.externalSite}</a></li>`);
                contentPrices.append(`<div class="price-external-site tab-pane" id="tab-price-external" role="tabpanel">
    <input type="hidden" id="id-external" value="${d.externalPrice.id}" />
    <table style="width: 100%">
        <tr>
            <td class="control-label" style="width: 100px;"><label for="external-price">Precio</label></td>
            <td><input type="number" value="${d.externalPrice.price}" min="0" class="form-control" id="external-price" /></td>
        </tr>
        <tr>
            <td class="control-label"><label for="external-price-commentaries">Comentario</label></td>
            <td><input type="text" class="form-control" value="${d.externalPrice.commentaries}" id="external-price-commentaries" /></td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="custom-control custom-switch">
                    <input type="checkbox" class="custom-control-input selected" value="${d.externalPrice.showAlways}" id="external-price-show-always" />
                    <label class="custom-control-label" for="external-price-show-always"> Mostrar Siempre</label>
                </div>
            </td>
        </tr>
    </table>
</div>`);
            }

            d.volumePrices.forEach((v, i) => {
                var clsActive = i === 0 ? 'active show' : '', attActive = i === 0 ? 'aria-selected="true"' : '', stockContent = "", fifoContent = "";

                ulVolumePrices.append(`<li class="nav-item"><a class="nav-link ${clsActive}" data-toggle="tab" href="#tab-volumeprice-${v.id}" role="tab" ${attActive}>${v.name}</a></li>`);

                if (v.stockDetail.length > 0) {
                    stockContent = `<table class="table table-condensed table-striped w-100 mt-4" style="font-size: 0.9em;">
                <thead><tr><th>Almac&eacute;n</th><th class="text-right">Stock</th><th class="text-right">Reserva</th><th class="text-right">Pedido</th></tr></thead>`;
                    v.stockDetail.forEach(sd => {
                        stockContent += `<tr><td>${sd.warehouse}</td><td class="text-right">${sd.stock}</td><td class="text-right">${sd.reserve}</td><td class="text-right">${sd.transit}</td></tr>`;
                    });
                    stockContent += '</table>';
                }
                if (v.fifoDetail.length > 0) {
                    fifoContent = `<table class="table table-condensed table-striped w-100 mt-4" style="font-size: 0.9em;">
                <thead>
                    <tr>
                        <th>Almac&eacute;n</th>
                        <th class="text-right">Stock</th>
                        <th class="text-right">Costo</th>
                        <th class="text-right">Total</th>
                        <th class="text-center">Fecha</th>
                    </tr>
                </thead>`;
                    v.fifoDetail.forEach(fd => {
                        fifoContent += `<tr>
                    <td>${fd.warehouse}</td>
                    <td class="text-right">${fd.stock}</td>
                    <td class="text-right">${fd.realCost ? kendo.toString(fd.realCost, "N2") : ""}</td>
                    <td class="text-right">${fd.total ? kendo.toString(fd.total, "N2") : ""}</td>
                    <td class="text-center">${fd.date ? kendo.toString(JSON.toDate(fd.date), "dd-MM-yyyy") : ""}</td>
                </tr>`;
                    });

                    fifoContent += '</table>';
                }
                contentVolumePrices.append(`<div class="volumeprice-subsidiary tab-pane ${clsActive}" id="tab-volumeprice-${v.id}" role="tabpanel">
    <input type="hidden" value="${v.id}" class="id-subsidiary" />
    <div class="grid-volume" id="grid-volume-${v.id}"></div>
    <div class="card d-none detail-volume" style="margin-top: 8px;">
        <div class="card-header">Detalle</div>
        <div class="card-body">
        </div>
    </div>
    <div class="row">
        <div class="col-12 col-sm-6">${stockContent}</div>
        <div class="col-12 col-sm-6">${fifoContent}</div>
    </div>
</div>`);
            });

            $(".volumeprice-subsidiary").each(function (i, obj) {
                var idSubsidiary = +$(obj).find(".id-subsidiary").val(), volumeItems = d.volumePrices.find(x => x.id === idSubsidiary).items;
                $(obj).find(".grid-volume").kendoGrid({
                    sortable: true, scrollable: { height: 200 }, selectable: "Single, Row",
                    noRecords: { template: '<div class="w-100 p-2">No hay precios por volumen para esta sucursal.</div>' },
                    columns: [
                        { title: "Cantidad", field: "quantity", width: 110, attributes: alignRight, headerAttributes: alignRight },
                        { title: "Precio", field: "price", width: 120, attributes: alignRight, headerAttributes: alignRight, format: "{0:N2}" },
                        { title: "Comentarios", field: "observations" },
                        {
                            title: "", attributes: alignCenter, headerAttributes: alignCenter, width: 60,
                            template: '<i class="fas fa-pen action action-link edit-volume-price" title="Editar Precio por Volúmen"></i><i class="fas fa-trash-alt action action-link delete-volume-price" title="Eliminar Precio por Volúmen"></i>',
                            headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link new-volume-price" title="Nuevo Precio por Volúmen"></i>'
                        }
                    ],
                    dataSource: { filter: [{ logic: "and", filters: [{ field: "statusType", operator: "neq", value: 3 }] }], schema: { model: { id: "id" } }, data: volumeItems }
                });
            });

            var accelerators = d.accelerators ?? [];
            accelerators.forEach((x) => {
                x.initialDate = JSON.toDate(x.initialDate);
                x.finalDate = JSON.toDate(x.finalDate);
            });
            $("#AcceleratorLots").kendoGrid({
                columns: [
                    { title: "Cantidad Inicial", field: "initialQuantity", attributes: alignRight, headerAttributes: alignRight },
                    { title: "Cantidad", field: "quantity", attributes: alignRight, headerAttributes: alignRight },
                    { title: "Acelerador", field: "accelerator", attributes: alignRight, headerAttributes: alignRight },
                    { title: "Desde", field: "initialDate", format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter },
                    { title: "Hasta", field: "finalDate", format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter },
                    { title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled" },
                    {
                        field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action action-link new-lot" title="Nuevo Acelerador"></i>',
                        template: '<i class="fas fa-pen action action-link edit-lot" title="Editar Acelerador"></i><i class="fas fa-trash-alt action action-link delete-lot" title="Eliminar Acelerador"></i>'
                    }
                ],
                sortable: true, selectable: "Single, Row",
                noRecords: { template: '<div class="w-100 p-3">No hay aceleradores para este producto.</div>' },
                dataSource: {
                    filter: [{ logic: "and", filters: [{ field: "statusType", operator: "neq", value: 3 }] }],
                    schema: { model: { id: "id" } },
                    data: accelerators
                }
            });
        } else {
            showError(`Se ha producido un error al traer los datos del servidor`);
        }
    });
}

function onProductDelete(e) {
    var grd = $("#Listado").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr")), filters = getFilters();
    if (item.editable) {
        showConfirm(`¿Está seguro que desea eliminar el producto <b>${item.itemCode}</b>?`, () => {
            $.post(urlDelete, { Id: item.id, Filters: filters.data }, function (data) {
                if (data.message === "") {
                    loadGrid(data.items);
                    showMessage(`Se ha eliminado el producto <b>${item.itemCode}</b> correctamente.`);
                } else {
                    showError(data.message);
                }
            });
        });
    } else {
        showInfo(`No se puede eliminar el Producto <b>${item.itemCode}</b>, est&aacute; ligado a un producto en el SAP.`);
    }
}

function onProductSync(e) {
    var grd = $("#Listado").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row), filters = getFilters();

    $.post(urlSync, { Id: item.id, Filters: filters.data }, function (data) {
        if (data.error) {
            showError(data.message);
        } else {
            loadGrid(data.items);
            if (data.message === "") {
                showMessage(`Se ha sincronizado el Item: <b>${item.itemCode}</b> exitosamente.`);
            } else {
                showInfo(data.message);
            }
        }
    });
}

function onProductSave(e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var product = form.serializeObject();
        if (!product.Editable) {
            product.ItemCode = $("#ItemCode").val();
            product.Name = $("#Name").val();
            product.Description = $("#Description").val();
            product.Commentaries = $("#Commentaries").val();
            product.Warranty = $("#Warranty").val();
            product.Consumables = $("#Consumables").val();
            product.Line = $("#Line").getKendoDropDownList().value();
            product.Category = $("#Category").getKendoDropDownList().value();
            product.SubCategory = $("#SubCategory").getKendoDropDownList().value();
        }

        product.Prices = [];
        product.Volumen = [];
        product.Offers = [];

        $.each($(".price-subsidiary"), function (i, obj) {
            var price = {
                Id: $(obj).find(".id").val(), IdSubsidiary: $(obj).find(".id-subsidiary").val(), Regular: $(obj).find(".regular-price").val(),
                ClientSuggested: $(obj).find(".client-suggested").val(), Observations: $(obj).find(".observations").val(), Commentaries: $(obj).find(".commentaries").val()
            };
            product.Prices.push(price);
            var tempOffers = $(obj).find(".price-offers").getKendoGrid().dataSource.data().map(x => { return { Id: x.id, IdProduct: x.idProduct, IdSubsidiary: x.idSubsidiary, Price: x.price, Description: x.description, Enabled: x.enabled, Since: x.since ? kendo.toString(x.since, "yyyy-MM-dd") : "", Until: x.until ? kendo.toString(x.until, "yyyy-MM-dd") : "", OnlyWithStock: x.onlyWithStock, StatusType: x.statusType } });
            product.Offers.push(...tempOffers);
        });

        if ($("#id-external").length > 0) {
            product.ExternalPrice = { Id: $("#id-external").val(), Price: $("#external-price").val(), Commentaries: $("#external-price-commentaries").val(), ShowAlways: $("#external-price-show-always").prop("checked") };
        }

        $.each($(".volumeprice-subsidiary"), function (i, obj) {
            var tempVolume = $(obj).find(".grid-volume").getKendoGrid().dataSource.data().map(x => { return { Id: x.id, IdProduct: x.idProduct, IdSubsidiary: x.idSubsidiary, Price: x.price, Quantity: x.quantity, Observations: x.observations, StatusType: x.statusType } });
            product.Volumen.push(...tempVolume);
        });

        var accItems = $("#AcceleratorLots").data("kendoGrid").dataSource.data();
        product.Lots = accItems.map((x) => ({ Id: x.id, IdProduct: x.idProduct, InitialQuantity: x.initialQuantity, Quantity: x.quantity, Accelerator: x.accelerator, InitialDate: x.initialDate, FinalDate: x.finalDate, Enabled: x.enabled, ListAcceleratorLotExcludeds: x.listAcceleratorLotExcludeds.map((y) => ({ Id: y.id, CardCode: y.cardCode, StatusType: y.statusType })), StatusType: x.statusType }));

        var filters = getFilters();
        console.log(product);
        $.post(urlEdit, { Item: product, Filters: filters.data }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);

                $("#Detail, #back-list").addClass("d-none");
                $("#Listado, #toggle-filter-box").removeClass("d-none");
                $("#page-title").text("Configuración de Productos");

                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });

    }
}

function onOfferEdit(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr"));//, offers = JSON.parse($("#Offers").val());
    if ($(e.currentTarget).hasClass("delete-offer")) {
        showConfirm(`¿Está seguro que desea eliminar este precio de oferta?`, () => {
            if (item) {
                if (item.id > 0) {
                    item.statusType = 3;
                    grd.dataSource.pushUpdate(item);
                } else {
                    grd.dataSource.remove(item);
                }
            }
        });
    }
    if ($(e.currentTarget).hasClass("new-offer") || $(e.currentTarget).hasClass("edit-offer")) {
        var idProduct = $("#Id").val(), idSubsidiary = $(e.currentTarget).closest(".price-subsidiary").find(".id-subsidiary").val();
        var panel = $(e.currentTarget).closest(".price-subsidiary").find(".detail-offer");
        if ($(e.currentTarget).hasClass("new-offer")) {
            item = { id: getId(), idProduct: idProduct, idSubsidiary: idSubsidiary, price: 0, description: "", enabled: true, since: null, until: null, onlyWithStock: false };
        }
        panel.find(".id-offer").val(item.id);
        panel.find(".id-subsidiary").val(item.idSubsidiary);
        panel.find(".po-price").val(item.price);
        panel.find(".po-description").val(item.description);
        panel.find(".po-since.k-input-inner").getKendoDatePicker().value(item.since);
        panel.find(".po-until.k-input-inner").getKendoDatePicker().value(item.until);
        panel.find(".po-enabled").prop("checked", item.enabled);
        panel.find(".po-only-with-stock").prop("checked", item.onlyWithStock);

        panel.removeClass("d-none");
        $("#save-product").attr("disabled", true);
    }
}

function onOfferSave(e) {
    e.preventDefault();
    if ($(e.currentTarget).hasClass("cancel-offer")) {
        $(e.currentTarget).closest(".detail-offer").addClass("d-none");
    } else {
        var p = $(e.currentTarget).closest(".detail-offer"), id = p.find(".id-offer").val(), grid = p.prev().getKendoGrid(), item;
        item = {
            id: id,
            idSubsidiary: p.find(".id-subsidiary").val(),
            price: +p.find(".po-price").val(),
            description: p.find(".po-description").val(),
            since: p.find(".po-since.k-input-inner").getKendoDatePicker().value(),
            until: p.find(".po-until.k-input-inner").getKendoDatePicker().value(),
            enabled: p.find(".po-enabled").prop("checked"),
            onlyWithStock: p.find(".po-only-with-stock").prop("checked"),
            statusType: id < 1 ? 1 : 2
        };
        grid.dataSource.pushUpdate(item);
        p.addClass("d-none");
    }
    $("#save-product").removeAttr("disabled");
}

function onEditVolumePrice(e) {
    var grid = $(this).closest(".k-grid").data("kendoGrid"), item = grid.dataItem($(this).closest("tr")), panel = $(this).closest(".volumeprice-subsidiary").find(".detail-volume");
    if ($(e.currentTarget).hasClass("delete-volume-price")) {
        showConfirm(`¿Está seguro que desea eliminar este precio por volumen?`, () => {
            if (item) {
                if (item.id > 0) {
                    item.statusType = 3;
                    grid.dataSource.pushUpdate(item);
                } else {
                    grid.dataSource.remove(item);
                }
            }
        });
    }
    if ($(e.currentTarget).hasClass("new-volume-price") || $(e.currentTarget).hasClass("edit-volume-price")) {
        if ($(e.currentTarget).hasClass("new-volume-price")) {
            item = { id: getId(), idProduct: $("#Id").val(), idSubsidiary: $(this).closest(".volumeprice-subsidiary").find(".id-subsidiary").val(), quantity: 0, price: 0, observations: "" };
        }
        var template = kendo.template(priceVolumeTemplate);
        if (item.observations === null) { item.observations = ""; }
        panel.find(".card-body").html(template(item));
        panel.find("#Quantity").kendoNumericTextBox({ format: "N0", decimals: 0 });
        panel.find("#Price").kendoNumericTextBox();

        panel.removeClass("d-none");
        $("#save-product").attr("disabled", true);
    }
}

function onVolumePriceSave(e) {
    var panel = $(this).closest(".card");
    if ($(e.currentTarget).hasClass("save-volume-price")) {
        var p = $(e.currentTarget).closest(".detail-volume"), id = p.find(".id").val(), grid = p.prev().getKendoGrid(), item;
        item = {
            id: id,
            idSubsidiary: $(e.currentTarget).closest(".volumeprice-subsidiary").find(".id-subsidiary").val(),
            quantity: +p.find(".quantity").val(),
            price: +p.find(".price").val(),
            observations: p.find(".observations").val(),
            statusType: id < 1 ? 1 : 2
        };
        grid.dataSource.pushUpdate(item);
    }
    panel.find(".card-body").empty();
    panel.addClass("d-none");
    $("#save-product").removeAttr("disabled");
}

function onLotEdit(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr"));
    if ($(e.currentTarget).hasClass("delete-lot")) {
        showConfirm("¿Está seguro que desea eliminar este Lote con acelerador?", function () {
            if (item) {
                if (item.id > 0) {
                    item.statusType = 3;
                    grd.dataSource.pushUpdate(item);
                } else {
                    grd.dataSource.remove(item);
                }
            }
        });
    }
    if ($(e.currentTarget).hasClass("new-lot") || $(e.currentTarget).hasClass("edit-lot")) {
        var panel = $(e.currentTarget).closest(".kbyte-detail").find(".card");
        if ($(e.currentTarget).hasClass("new-lot")) {
            item = { id: 0, idProduct: $("#Id").val(), initialQuantity: 0, quantity: 0, accelerator: 0, initialDate: null, finalDate: null, enabled: true };
        }
        var template = kendo.template(lotTemplate);
        panel.find(".card-body").html(template(item));

        $("#AccEnabled").prop("checked", item.enabled);
        var txtQuantity = $("#Quantity").kendoNumericTextBox({ format: "N0", decimals: 0 }).data("kendoNumericTextBox");
        var txtInitQuantity = $("#InitialQuantity").kendoNumericTextBox({ format: "N0", decimals: 0, change: (e) => txtQuantity.value(e.sender.value()) }).data("kendoNumericTextBox");
        $("#Accelerator").kendoNumericTextBox();
        $("#InitialDate").kendoDatePicker({ value: item.initialDate });
        $("#FinalDate").kendoDatePicker({ value: item.finalDate });
        if (item.id != 0) {
            txtInitQuantity.enable(false);
        }
        txtQuantity.enable(false);

        $("#Excluded").kendoMultiSelect({
            dataTextField: "name", dataValueField: "code", dataSource: _clients, filter: "contains", placeholder: "Clientes...", downArrow: true,
            value: item.listAcceleratorLotExcludeds?.filter((x) => x.statusType != 3).map((x) => x.cardCode) ?? [],
            noDataTemplate: '<div class="kd-nodata-wrapper">No hay resultados para el critrerio de búsqueda</div>',
            virtual: {
                itemHeight: 26, valueMapper: function (options) {
                    var items = this.dataSource.data();
                    var indexs = [];
                    options.value.forEach((x) => indexs.push(items.indexOf(items.find(i => i.code === x))));
                    options.success(indexs);
                }
            },
        });

        panel.removeClass("d-none");
        $("#save-product").attr("disabled", true);
    }
}

function onLotSave(e) {
    e.preventDefault();
    var panel = $(this).closest(".kbyte-detail").find(".card");
    if (e.currentTarget.id === "save-lot") {
        var item = panel.serializeToObject(), grd = $("#AcceleratorLots").data("kendoGrid");
        item = toCamel(item);
        item.enabled = item.accEnabled;
        if (+item.id === 0) item.id = getId();
        item.statusType = item.id > 0 ? 2 : 1;
        item.enabled = item.accEnabled;
        item.listAcceleratorLotExcludeds = [];

        let newItem = (id, cardCode, status) => ({ id: id, cardCode: cardCode, statusType: status }),
            oldItem = grd.dataItem(grd.select()), sel = $("#Excluded").data("kendoMultiSelect").value();
        oldItem.listAcceleratorLotExcludeds.forEach(function (o) {
            if (sel.includes(o.cardCode)) {
                item.listAcceleratorLotExcludeds.push(newItem(o.id, o.cardCode, o.statusType));
            } else {
                if (o.id > 0) item.listAcceleratorLotExcludeds.push(newItem(o.id, o.cardCode, 3));
            }
        });
        sel.forEach(function (s) {
            if (!oldItem.listAcceleratorLotExcludeds.find((x) => x.cardCode == s)) item.listAcceleratorLotExcludeds.push(newItem(getId(), s, 1));
        });
        console.log('item', item);
        grd.dataSource.pushUpdate(item);
    }
    panel.find(".card-body").empty();
    panel.addClass("d-none");
    $("#save-product").removeAttr("disabled");
}

function ExportExcel() {
    var filterData = getFilters();
    if (filterData.message === "") {
        window.location.href = `${urlExcel}?${$.param(filterData.data)}`;
    } else {
        showInfo(`Los siguientes campos son necesarios <br />${filterData.message}`);
    }
}

function getId() {
    _intId -= 1;
    return _intId;
}

function getCategoryValue(e) {
    return { Category: $("#Category").val() };
}

function toCamel(o) {
    var newO, origKey, newKey, value
    if (o instanceof Array) {
        return o.map(function (value) {
            if (typeof value === "object") {
                value = toCamel(value)
            }
            return value
        })
    } else {
        newO = {}
        for (origKey in o) {
            if (o.hasOwnProperty(origKey)) {
                newKey = (origKey.charAt(0).toLowerCase() + origKey.slice(1) || origKey).toString()
                value = o[origKey]
                if (value instanceof Array || (value !== null && value.constructor === Object)) {
                    value = toCamel(value)
                }
                newO[newKey] = value
            }
        }
    }
    return newO
}

function toTitle(o) {
    var newO, origKey, newKey, value
    if (o instanceof Array) {
        return o.map(function (value) {
            if (typeof value === "object") {
                value = toTitle(value)
            }
            return value
        })
    } else {
        newO = {}
        for (origKey in o) {
            if (o.hasOwnProperty(origKey)) {
                newKey = (origKey.charAt(0).toUpperCase() + origKey.slice(1) || origKey).toString()
                value = o[origKey]
                if (value instanceof Array || (value !== null && value.constructor === Object)) {
                    value = toTitle(value)
                }
                newO[newKey] = value
            }
        }
    }
    return newO
}

function onSelect(e) {
    var strExt = e.files[0].extension.toLowerCase();
    var ValidExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp"];

    if (Enumerable.From(ValidExtensions).Where(`$ === '${strExt}'`).Count() === 0) {
        e.preventDefault();
        showInfo("Debe ser una imagen.");
    }
}

function onUploadSucceded(e) {
    var files = e.files;
    if (e.operation == "upload") {
        $.each(files, function (i, file) {
            $("#Photo").attr("src", urlImages + file.name);
            $("#DeletePhoto").hide();
            $("#NewImageURL").val(file.name);
        });
    } else {
        var ImageURL = $("#ImageURL").val();
        $("#NewImageURL").val("");
        if (ImageURL === "") {
            $("#Photo").attr("src", urlNoImage);
            $("#DeletePhoto").hide();
        } else {
            $("#Photo").attr("src", urlImages + ImageURL);
            $("#DeletePhoto").show();
        }
    }
}

function onGettingPriceHistory(e) {
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var wnd = $("#HistoryDetail").data("kendoWindow");
    $.get(urlHistory, { Id: item.id }, function (data) {
        $.each(data.prices, function (i, obj) {
            obj.logDate = JSON.toDate(obj.logDate);
        });
        $.each(data.volume, function (i, obj) {
            obj.logDate = JSON.toDate(obj.logDate);
        });
        $("#prices-history-grid").kendoGrid({
            sortable: true, scrollable: { height: 200 }, selectable: "Single, Row",
            noRecords: { template: '<div class="w-100 text-center p-3">No hay histórico de precios para este producto.</div>' },
            columns: [
                { title: "Sucursal", width: "120px", field: "sudsidiary.name" },
                { title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: "80px", field: "regular", format: "{0:N2}" },
                { title: "Oferta", attributes: alignRight, headerAttributes: alignRight, width: "80px", field: "offer", format: "{0:N2}" },
                { title: "Oferta Desc.", width: "250px", field: "offerDescription" },
                { title: "Sug. Cliente", attributes: alignRight, headerAttributes: alignRight, width: "100px", field: "clientSuggested", format: "{0:N2}" },
                { title: "Observaciones", width: "250px", field: "observations" },
                { title: "Comentarios", width: "250px", field: "commentaries" },
                { title: "Usuario", width: "150px", field: "userName" },
                { title: "Fecha", width: 140, field: "logDate", format: "{0:dd/MM/yyyy HH:mm}" },
                { title: "Acción", width: 100, field: "logAction" }
            ],
            dataSource: { data: data.prices }
        });
        $("#volume-prices-history-grid").kendoGrid({
            sortable: true, scrollable: { height: 200 }, selectable: "Single, Row",
            noRecords: { template: '<div class="w-100 text-center p-3">No hay histórico de precios por volumen para este producto.</div>' },
            columns: [
                { title: "Sucursal", width: "120px", field: "subsidiary.name" },
                { title: "Cantidad", attributes: alignRight, headerAttributes: alignRight, width: "80px", field: "quantity" },
                { title: "Precio", attributes: alignRight, headerAttributes: alignRight, width: "80px", field: "price", format: "{0:N2}" },
                { title: "Observaciones", width: "250px", field: "observations" },
                { title: "Usuario", width: "150px", field: "userName" },
                { title: "Fecha", width: 140, field: "logDate", format: "{0:dd/MM/yyyy HH:mm}" },
                { title: "Acción", width: 100, field: "logAction" }
            ],
            dataSource: { data: data.volume }
        });
        wnd.open().center();
    });
}

function onDeletePhoto(e) {
    $("#Photo").attr("src", urlNoImage);
    $("#DeletePhoto").hide();
    $("#NewImageURL").val("None");
}

//#endregion