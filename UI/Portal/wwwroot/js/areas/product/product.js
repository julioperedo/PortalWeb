//#region Variables Globales
const marginGrid = 30, alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy}";
var _intId = 0;
//#endregion

//#region Eventos

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", marginGrid));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", marginGrid));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", marginGrid));

$("#Listado").on("click", ".new-product", onNewProduct);

$("#Listado").on("click", ".action-edit", onProductEdit);

$("#Listado").on("click", ".action-delete", onProductDelete);

$("#Listado").on("click", ".action-sync", onProductSync);

$("#Listado").on("click", ".action-prices", onGettingPriceHistory);

$("#action-clean").click(() => cleanFilters());

$("#action-filter").click(() => filterData());

$("#Detail").on("click", ".new-volume-price", onNewVolumePrice);

$("#Detail").on("click", ".edit-volume-price", onVolumePriceEdit);

$("#Detail").on("click", ".delete-volume-price", onVolumePriceDelete);

$("#Detail").on("click", ".cancel-volume-price", onVolumePriceCancelEdit);

$("#Detail").on("click", ".save-volume-price", onVolumePriceSave);

$("#Detail").on("click", ".save-product", onProductSave);

$("#Detail").on("click", "#DeletePhoto", onDeletePhoto);

$("#Detail").on('shown.bs.tab', 'a[data-toggle="tab"]', (e) => $("#Detail").getKendoWindow().center());

$("#action-excel").click(() => ExportExcel());

$("#Detail").on("click", ".new-lot", onNewLot);

$("#Detail").on("click", ".edit-lot", onLotEdit);

$("#Detail").on("click", ".delete-lot", onLotDelete);

$("#Detail").on("click", "#cancel-lot", onLotEditCancel);

$("#Detail").on("click", "#save-lot", onLotSave);

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
                    { field: "miami", title: "Miami", attributes: alignRight, headerAttributes: alignRight, width: 80, format: "{0:N2}" },
                    { field: "iquique", title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 80, format: "{0:N2}" }
                ]
            },
            {
                title: "Stock", headerAttributes: alignCenter, columns: [
                    { field: "stockSantaCruz", title: "Santa Cruz", attributes: alignRight, headerAttributes: alignRight, width: 110, template: "#=stockSantaCruz#" },
                    { field: "stockMiami", title: "Miami", attributes: alignRight, headerAttributes: alignRight, width: 110, template: "#=stockMiami#" },
                    { field: "stockIquique", title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 110, template: "#=stockIquique#" }
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
                title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter,
                template: '<a class="action action-link action-sync" title="Sincronizar Producto"><i class="fas fa-sync-alt"></i></a><a class="action action-link action-prices" title="Historial de Precios"><i class="fas fa-dollar-sign"></i></a>'
            },
            {
                field: "itemCode", title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, headerTemplate: '<a class="action action-link new-product" title="Nuevo Producto"><i class="fas fa-plus"></i></a>',
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

function loadGridVolume(panel, items) {
    var grd = panel.find(".k-grid").getKendoGrid();
    var ds = new kendo.data.DataSource({ data: Enumerable.From(items).Where("$.statusType !== 3").ToArray() });
    grd.setDataSource(ds);
}

function loadDetail(panel, item) {
    var template = kendo.template($("#detailTemplate").html());
    if (item.observations === null) { item.observations = ""; }
    panel.find(".card-body").html(template(item));

    panel.find("#Quantity").kendoNumericTextBox({ format: "N0", decimals: 0 });
    panel.find("#Price").kendoNumericTextBox();

    panel.removeClass("d-none");
    $("#Detail").getKendoWindow().center();
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

function loadDetailLot(panel, item) {
    var template = kendo.template($("#detailLotTemplate").html());
    panel.find(".card-body").html(template(item));

    panel.find("#AccEnabled").prop("checked", item.enabled);
    panel.find("#Quantity").kendoNumericTextBox({ format: "N0", decimals: 0 });
    panel.find("#InitialQuantity").kendoNumericTextBox({ format: "N0", decimals: 0 });
    panel.find("#Accelerator").kendoNumericTextBox();
    panel.find("#InitialDate").kendoDatePicker({ value: item.initialDate });
    panel.find("#FinalDate").kendoDatePicker({ value: item.finalDate });
    if (item.id !== 0) {
        panel.find("#InitialQuantity").data("kendoNumericTextBox").enable(false);
        panel.find("#Quantity").data("kendoNumericTextBox").enable(false);
    }
    panel.removeClass("d-none");
    $("#Detail").getKendoWindow().center();
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

function onRefresh(e) {
    $("#ExtraComments").kendoEditor({
        resizable: { content: true },
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        messages: { dialogCancel: "Cancelar", dialogInsert: "Agregar", dialogUpdate: "Aceptar", imageAltTex: "Texto Alternativo", imageWebAddress: "Dirección Imagen", insertImage: "Insertar Imagen", viewHtml: "Ver HTML" }
    });
    $(".volumeprice-subsidiary").each(function (i, obj) {
        var idSubsidiary = $(obj).find(".id-subsidiary").val(), volumeData = JSON.parse($("#Volumen").val());
        $(obj).find(".grid-volume").kendoGrid({
            sortable: true, scrollable: { height: 200 }, selectable: "Single, Row",
            noRecords: { template: "No hay precios por volumen para esta sucursal." },
            columns: [
                { title: "Cantidad", field: "quantity", width: 110, attributes: alignRight, headerAttributes: alignRight },
                { title: "Precio", field: "price", width: 120, attributes: alignRight, headerAttributes: alignRight, format: "{0:N2}" },
                { title: "Comentarios", field: "observations" },
                {
                    title: "", attributes: alignCenter, headerAttributes: alignCenter, width: 50,
                    template: '<i class="fas fa-pen action edit-volume-price" title="Editar Precio por Volúmen"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action delete-volume-price" title="Eliminar Precio por Volúmen"></i>',
                    headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action new-volume-price" title="Nuevo Precio por Volúmen"></i>'
                }
            ],
            dataSource: { data: Enumerable.From(volumeData).Where(`$.idSubsidiary === ${idSubsidiary}`).ToArray() }
        });
    });
    e.sender.element.find("[type='number']").kendoNumericTextBox();

    var accelerators = JSON.parse($("#Accelerators").val());
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
                field: "id", title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, headerTemplate: '<i class="fas fa-plus action new-lot" title="Nuevo Acelerador"></i>',
                template: '<i class="fas fa-pen action edit-lot" title="Editar Acelerador"></i>&nbsp;&nbsp;<i class="fas fa-trash-alt action delete-lot" title="Eliminar Acelerador"></i>'
            }
        ],
        sortable: true, selectable: "Single, Row",
        dataSource: {
            filter: [{ logic: "and", filters: [{ field: "statusType", operator: "neq", value: 3 }] }],
            schema: { model: { id: "id" } },
            data: accelerators
        }
    });

    var maxheight = 0;
    tabs = $('.tab-content.main-tab > .tab-pane');
    $.each(tabs, function () {
        this.classList.add("active"); /* make all visible */
        maxheight = (this.clientHeight > maxheight ? this.clientHeight : maxheight);
        if (!$(this).hasClass("show")) {
            this.classList.remove("active"); /* hide again */
        }
    });
    $.each(tabs, function () {
        $(this).height(maxheight);
    });

    onRefreshWindow(e);
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

function onNewProduct() {
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: urlEdit, data: { Id: 0 } });
    wnd.open();
}

function onProductEdit(e) {
    var grd = $("#Listado").data("kendoGrid");
    var item = grd.dataItem($(e.currentTarget).closest("tr"));

    var wnd = $("#Detail").data("kendoWindow");
    wnd.setOptions({ title: "Detalle de Producto", width: 900 });
    wnd.refresh({ url: urlEdit, data: { Id: item.id } });
    wnd.open();
}

function onProductDelete(e) {
    var grd = $("#Listado").data("kendoGrid");
    var item = grd.dataItem($(e.currentTarget).closest("tr"));
    var filters = getFilters();
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
    var grd = $("#Listado").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    var item = grd.dataItem(row);
    var filters = getFilters();

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
        product.Prices = [];
        var lstVolumen = JSON.parse($("#Volumen").val());
        product.Volumen = Enumerable.From(lstVolumen).Where("$.statusType !== 0").ToArray();

        $.each($(".price-subsidiary"), function (i, obj) {
            var price = {
                Id: $(obj).find("#id").val(), IdSubsidiary: $(obj).find("#id-subsidiary").val(), Regular: $(obj).find("#regular-price").data("kendoNumericTextBox").value(), Offer: $(obj).find("#offer-price").data("kendoNumericTextBox").value(),
                ClientSuggested: $(obj).find("#client-suggested").data("kendoNumericTextBox").value(), OfferDescription: $(obj).find("#offer-description").val(), Observations: $(obj).find("#observations").val(),
                Commentaries: $(obj).find("#commentaries").val(), Volumen: []
            };
            product.Prices.push(price);
        });

        if ($("#id-external").length > 0) {
            product.ExternalPrice = { Id: $("#id-external").val(), Price: $("#external-price").data("kendoNumericTextBox").value(), Commentaries: $("#external-price-commentaries").val(), ShowAlways: $("#external-price-show-always").prop("checked") };
        }

        var accGrd = $("#AcceleratorLots").data("kendoGrid");
        var accItems = accGrd.dataSource.data();
        product.Lots = accItems.map((x) => { return { Id: x.id, IdProduct: x.idProduct, InitialQuantity: x.initialQuantity, Quantity: x.quantity, Accelerator: x.accelerator, InitialDate: x.initialDate, FinalDate: x.finalDate, Enabled: x.enabled, StatusType: x.statusType } }); //$("#AcceleratorLots").data("kendoGrid").dataSource.data();

        var filters = getFilters();

        $.post(urlEdit, { Item: product, Filters: filters.data }, function (data) {
            if (data.message === "") {
                loadGrid(data.items);
                //cleanFilters();
                $("#Detail").data("kendoWindow").close();
                showMessage("Se realizaron los cambios correctamente.");
            } else {
                showError(data.message);
            }
        });
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
                { title: "Fecha", width: 110, field: "logDate", format: "{0:dd/MM/yyyy HH:mm}" },
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
                { title: "Fecha", width: 110, field: "logDate", format: "{0:dd/MM/yyyy HH:mm}" },
                { title: "Acción", width: 100, field: "logAction" }
            ],
            dataSource: { data: data.volume }
        });
        wnd.open().center();
    });
}

function onNewVolumePrice(e) {
    var item = { id: getId(), idProduct: $("#Id").val(), idSubsidiary: $(this).closest(".volumeprice-subsidiary").find(".id-subsidiary").val(), quantity: 0, price: 0, observations: "" };
    var panel = $(this).closest(".volumeprice-subsidiary").find(".detail-volume");
    loadDetail(panel, item);
    $(".save-product").attr("disabled", true);
}

function onVolumePriceEdit(e) {
    var grid = $(this).closest(".k-grid").data("kendoGrid");
    var item = grid.dataItem($(this).closest("tr"));
    var panel = $(this).closest(".volumeprice-subsidiary").find(".detail-volume");
    loadDetail(panel, item);
    $(".save-product").attr("disabled", true);
}

function onVolumePriceDelete(e) {
    var grid = $(this).closest(".k-grid").data("kendoGrid");
    var item = grid.dataItem($(this).closest("tr"));
    var panel = $(this).closest(".volumeprice-subsidiary");
    var items = JSON.parse($("#Volumen").val());
    var price = Enumerable.From(items).Where(`$.id === ${item.id}`).FirstOrDefault();

    showConfirm(`¿Está seguro que desea eliminar este precio por volumen?`, () => {
        if (price) {
            if (price.id > 0) {
                price.statusType = 3;
            } else {
                items = items.filter((v, i, a) => v.id !== price.id);
            }
            $("#Volumen").val(JSON.stringify(items));
            loadGridVolume(panel, Enumerable.From(items).Where(`$.idSubsidiary === ${item.idSubsidiary}`).ToArray());
        }
    });
}

function onVolumePriceCancelEdit(e) {
    var panel = $(this).closest(".card");
    panel.addClass("d-none");
    panel.find(".card-body").empty();
    $("#Detail").getKendoWindow().center();
    $(".save-product").removeAttr("disabled");
}

function onVolumePriceSave(e) {
    var form = $(this).closest(".card-body");
    var panel = $(this).closest(".volumeprice-subsidiary");
    var items = JSON.parse($("#Volumen").val());
    var data = {
        id: +form.find("#Id").val(), idProduct: +form.find("#IdProduct").val(), idSubsidiary: +form.find("#IdSubsidiary").val(), observations: form.find("#Observations").val(),
        quantity: form.find("#Quantity").getKendoNumericTextBox().value(), price: form.find("#Price").getKendoNumericTextBox().value()
    };

    var config = {
        rules: { price: i => i.is("#Price") ? i.val() > 0 : true, quantity: i => i.is("#Quantity") ? i.val() > 0 : true },
        messages: { quantity: "Debe ser mayor a cero.", price: "Debe ser mayor a cero." }
    };
    var validator = form.kendoValidator(config).data("kendoValidator");
    if (validator.validate()) {
        var item = Enumerable.From(items).Where(`$.id === ${data.id} && $.idSubsidiary === ${data.idSubsidiary}`).FirstOrDefault();
        if (!item) {
            item = data;
            item.statusType = 1;
            items.push(item);
        } else {
            data.statusType = item.id > 0 ? 2 : 1;
            item = Object.assign(item, data);
        }

        loadGridVolume(panel, Enumerable.From(items).Where(`$.idSubsidiary === ${data.idSubsidiary}`).ToArray());
        panel.find(".card-body").empty();
        panel.find(".card").addClass("d-none");
        $("#Detail").getKendoWindow().center();
        $("#Volumen").val(JSON.stringify(items));
        $(".save-product").removeAttr("disabled");
    }
}

function onDeletePhoto(e) {
    $("#Photo").attr("src", urlNoImage);
    $("#DeletePhoto").hide();
    $("#NewImageURL").val("None");
}

function onNewLot(e) {
    var panel = $(e.currentTarget).closest(".kbyte-detail").find(".card");
    var item = { id: 0, idProduct: $("#Id").val(), initialQuantity: 0, quantity: 0, accelerator: 0, initialDate: null, finalDate: null, enabled: true };

    loadDetailLot(panel, item);
    $(".save-product").attr("disabled", true);
}

function onLotEdit(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var item = grd.dataItem($(e.currentTarget).closest("tr"));
    var panel = $(e.currentTarget).closest(".kbyte-detail").find(".card");
    loadDetailLot(panel, item);
    $(".save-product").attr("disabled", true);
}

function onLotDelete(e) {
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    showConfirm("¿Está seguro que desea eliminar este Lote con acelerador?", () => {
        var ds = grd.dataSource;
        var item = grd.dataItem($(e.currentTarget).closest("tr"));
        var beItem = ds.get(item.id);
        beItem.statusType = 3;
        grd.setDataSource(ds);
    });
}

function onLotEditCancel(e) {
    var panel = $(this).closest(".kbyte-detail").find(".card");
    panel.find(".card-body").empty();
    panel.addClass("d-none");
    $("#Detail").getKendoWindow().center();
    $(".save-product").removeAttr("disabled");
}

function onLotSave(e) {
    var panel = $(this).closest(".kbyte-detail").find(".card");
    var item = panel.serializeToObject();
    item = toCamel(item);
    item.enabled = item.accEnabled;
    var grd = $("#AcceleratorLots").data("kendoGrid");
    var ds = grd.dataSource;
    if (+item.id === 0) {
        item.statusType = 1;
        item.id = getId();
        ds.add(item);
    } else {
        var beItem = ds.get(item.id);
        beItem.initialQuantity = item.initialQuantity;
        beItem.quantity = item.quantity;
        beItem.accelerator = item.accelerator;
        beItem.initialDate = item.initialDate;
        beItem.finalDate = item.finalDate;
        beItem.enabled = item.accEnabled;
        beItem.statusType = beItem.id > 0 ? 2 : 1;
    }
    grd.setDataSource(ds);

    panel.find(".card-body").empty();
    panel.addClass("d-none");
    $("#Detail").getKendoWindow().center();
    $(".save-product").removeAttr("disabled");
}

//#endregion