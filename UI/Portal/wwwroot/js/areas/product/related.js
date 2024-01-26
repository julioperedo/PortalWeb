//#region GLOBAL VARIABLES
var _maxDate, _minDate, products = [], _lastRecieverGridHeight = null, _docId = -1;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
//#endregion

//#region EVENTS

$(() => setupControls());

$("#categories").on("click", ".action-new, .action-edit, .action-delete", onEditCategory);

$("#cancel-category, #cancel-product, #cancel-banner, #cancel-banner-item, #cancel-banner-trigger").click(closeWindow);

$("#save-category").click(onSavingCategory);

$("#products").on("click", ".action-new, .action-edit, .action-delete", onEditProduct);

$("#save-product").click(onSavingProduct);

$("#banners").on("click", ".action-new, .action-edit, .action-delete", onEditBanner);

$("#banners").on("click", ".action-new-item, .action-edit-item, .action-delete-item", onEditBannerItem);

$("#banners").on("click", ".action-new-trigger, .action-edit-trigger, .action-delete-trigger", onEditBannerTrigger);

$("label[for='banner-enabled']").click(e => $("#banner-enabled").getKendoSwitch().toggle());

$("#photo-gallery").click(onGalleryButtonClicked);

$("#photo-remove").click(onPhotoRemoveClicked);

$("#gallery").change(onGalleryChanged);

$("#save-banner").click(onSavingBanner);

$("#save-banner-item").click(onSavingBannerItem);

$("#save-banner-trigger").click(onSavingBannerTrigger);

//#endregion

//#region METHODS

function setupControls() {

    $.get(urlGetRelatedCategories, {}, function (d) {
        if (d.message === "") {
            $("#categories").kendoGrid({
                sortable: true,
                columns: [
                    { title: "Categoría", field: "category" },
                    { title: "Relacionada", field: "related" },
                    {
                        title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, sticky: true,
                        headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                        template: e => `<a class="action action-link action-edit" data-id="${e.id}"><i class="fas fa-pen" title="Editar"></i></a><a class="action action-link action-delete"><i class="fas fa-trash-alt" title="Eliminar"></i></a>`
                    }
                ],
                sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                dataSource: new kendo.data.DataSource({ data: d.items, schema: { model: { id: "id" } } }),
                dataBound: e => e.sender.element.find("table").attr("style", "")
            });
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer los Categorias relacionadas");
        }
    });

    $.get(urlGetRelatedProducts, {}, function (d) {
        if (d.message === "") {
            $("#products").kendoGrid({
                sortable: true,
                columns: [
                    { title: "Cod. Prod.", field: "productCode", width: 150 },
                    { title: "Producto", field: "productName" },
                    { title: "Cod. Rel.", field: "relatedCode", width: 150 },
                    { title: "Relacionado", field: "relatedName" },
                    {
                        title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, sticky: true,
                        headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                        template: e => `<a class="action action-link action-edit"><i class="fas fa-pen" title="Editar"></i></a><a class="action action-link action-delete"><i class="fas fa-trash-alt" title="Eliminar"></i></a>`
                    }
                ],
                sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                dataSource: new kendo.data.DataSource({ data: d.items, schema: { model: { id: "id" } } }),
                dataBound: e => e.sender.element.find("table").attr("style", "")
            });
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer los Categorias relacionadas");
        }
    });

    $.get(urlGetBanners, {}, function (d) {
        if (d.message === "") {
            var items = d.items;
            items.forEach(function (x) {
                x.initialDate = JSON.toDate(x.initialDate);
                if (x.finalDate) {
                    x.finalDate = JSON.toDate(x.finalDate);
                }
            });
            $("#banners").kendoGrid({
                sortable: true,
                columns: [
                    { title: "Nombre", field: "name" },
                    { title: "Desde", field: "initialDate", format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter, width: 120 },
                    { title: "Hasta", field: "finalDate", format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter, width: 120 },
                    { field: "imageUrl", title: " ", attributes: alignCenter, width: 30, template: e => $.trim(e.imageUrl) !== "" ? '<i class="fas fa-image" title="Tiene Imagen"></i>' : '' },
                    { title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: e => e.enabled ? '<i class="fas fa-check"></i>' : '', field: "enabled" },
                    {
                        title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, sticky: true,
                        headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                        template: e => `<a class="action action-link action-edit"><i class="fas fa-pen" title="Editar"></i></a><a class="action action-link action-delete"><i class="fas fa-trash-alt" title="Eliminar"></i></a>`
                    }
                ],
                sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                dataSource: new kendo.data.DataSource({ data: d.items, schema: { model: { id: "id" } } }),
                dataBound: e => e.sender.element.find("table").attr("style", ""),
                detailInit: function (e) {
                    e.detailCell.append(`<div class="custom-tab">
    <ul class="nav nav-tabs" role="tablist">
        <li class="nav-item"><a class="nav-link active show" data-toggle="tab" href="#tab-items-${e.data.id}" role="tab" aria-selected="true">Items</a></li>
        <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#tab-triggers-${e.data.id}" role="tab">Disparadores</a></li>
    </ul>
    <div class="tab-content">
        <div id="tab-items-${e.data.id}" class="tab-pane active show pt-3" role="tabpanel">
            <div id="items-${e.data.id}"></div>
        </div>
        <div id="tab-triggers-${e.data.id}" class="tab-pane pt-3" role="tabpanel">
            <div id="triggers-${e.data.id}"></div>
        </div>
    </div>
</div>`);
                    $.get(urlGetBannerDetail, { Id: e.data.id }, function (d) {
                        if (d.message === "") {
                            $(`#items-${e.data.id}`).kendoGrid({
                                sortable: true, selectable: "Single, Row",
                                noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                                dataBound: e => e.sender.element.find("table").attr("style", ""),
                                columns: [
                                    { title: "Cod.", field: "productCode", width: 150 },
                                    { title: "Producto", field: "productName" },
                                    { title: "Sucursal", field: "subsidiaryName" },
                                    { title: "Precio", field: "price", format: "{0:N2}", width: 100, attributes: alignRight, headerAttributes: alignRight },
                                    {
                                        title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, sticky: true,
                                        headerTemplate: `<a class="action action-link action-new-item" data-id="${e.data.id}" title="Nuevo"><i class="fas fa-plus"></i></a>`,
                                        template: e => `<a class="action action-link action-edit-item"><i class="fas fa-pen" title="Editar"></i></a><a class="action action-link action-delete-item"><i class="fas fa-trash-alt" title="Eliminar"></i></a>`
                                    }
                                ],
                                dataSource: new kendo.data.DataSource({ data: d.items, schema: { model: { id: "id" } } })
                            });
                            $(`#triggers-${e.data.id}`).kendoGrid({
                                sortable: true, selectable: "Single, Row",
                                noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                                dataBound: e => e.sender.element.find("table").attr("style", ""),
                                columns: [
                                    { title: "Cod.", field: "productCode", width: 150 },
                                    { title: "Producto", field: "productName" },
                                    { title: "Categoría", field: "category" },
                                    { title: "Todos", template: e => $.trim(e.category) === "" && $.trim(e.productName) === "" ? '<i class="fas fa-check"></i>' : '', attributes: alignCenter, headerAttributes: alignCenter, width: 70 },
                                    {
                                        title: " ", attributes: alignCenter, width: 80, sortable: false, headerAttributes: alignCenter, sticky: true,
                                        headerTemplate: `<a class="action action-link action-new-trigger" data-id="${e.data.id}" title="Nuevo"><i class="fas fa-plus"></i></a>`,
                                        template: e => `<a class="action action-link action-edit-trigger"><i class="fas fa-pen" title="Editar"></i></a><a class="action action-link action-delete-trigger"><i class="fas fa-trash-alt" title="Eliminar"></i></a>`
                                    }
                                ],
                                dataSource: new kendo.data.DataSource({ data: d.triggers, schema: { model: { id: "id" } } })
                            });
                        } else {
                            console.error(e.message);
                            showError("Se ha producido un error al traer los datos del servidor");
                        }
                    });
                }
            });
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer los Categorias relacionadas");
        }
    });

    $.get(urlGetCategories, {}, function (d) {
        if (d.message === "") {
            $("#category-name").kendoDropDownList({ dataTextField: "name", dataValueField: "name", optionLabel: "Seleccione una Categoría...", dataSource: { data: d.items } });
            $("#related-name").kendoDropDownList({ dataTextField: "name", dataValueField: "name", optionLabel: "Seleccione una Categoría...", dataSource: { data: d.items } });
            $("#banner-trigger-category").kendoDropDownList({ dataTextField: "name", dataValueField: "name", optionLabel: "Seleccione una Categoría...", dataSource: { data: d.items } });
        } else {
            console.error(d.message);
        }
    });

    $.get(urlGetProducts, {}, function (d) {
        if (d.message === "") {
            $("#product-name").kendoDropDownList({
                dataTextField: "fullName", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", virtual: {
                    itemHeight: 26, valueMapper: function (options) {
                        var items = this.dataSource.data();
                        var index = items.indexOf(items.find(i => i.id === options.value));
                        options.success(index);
                    }
                }, dataSource: { data: d.items }
            });
            $("#related-product-name").kendoDropDownList({
                dataTextField: "fullName", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", virtual: {
                    itemHeight: 26, valueMapper: function (options) {
                        var items = this.dataSource.data();
                        var index = items.indexOf(items.find(i => i.id === options.value));
                        options.success(index);
                    }
                }, dataSource: { data: d.items }
            });
            $("#banner-item-product").kendoDropDownList({
                dataTextField: "fullName", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", virtual: {
                    itemHeight: 26, valueMapper: function (options) {
                        var items = this.dataSource.data();
                        var index = items.indexOf(items.find(i => i.id === options.value));
                        options.success(index);
                    }
                }, dataSource: { data: d.items }
            });
            $("#banner-trigger-product").kendoDropDownList({
                dataTextField: "fullName", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", virtual: {
                    itemHeight: 26, valueMapper: function (options) {
                        var items = this.dataSource.data();
                        var index = items.indexOf(items.find(i => i.id === options.value));
                        options.success(index);
                    }
                }, dataSource: { data: d.items }
            });
        } else {
            console.error(d.message);
        }
    });

    $("#banner-enabled").kendoSwitch({ enabled: true, messages: { checked: "", unchecked: "" } });
    var pickerSince = $("#banner-since").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            pickerUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var pickerUntil = $("#banner-until").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            pickerSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = pickerUntil.max();
    _minDate = pickerSince.min();

    $.get(urlGetSubsidiaries, {}, function (d) {
        if (d.message === "") {
            $("#banner-item-subsidiary").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains", dataSource: { data: d.items } });
        } else {
            console.error(d.message);
        }
    });

    $("#detail-category").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Categoría Relacionada", modal: true, width: 1000, open: onRefreshWindow });
    $("#detail-product").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Producto Relacionado", modal: true, width: 800, open: onRefreshWindow });
    $("#detail-banner").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Banner", modal: true, width: 1000, open: onRefreshWindow });
    $("#detail-banner-item").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Banner Item", modal: true, width: 700, open: onRefreshWindow });
    $("#detail-banner-trigger").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Banner Disparador", modal: true, width: 900, open: onRefreshWindow });
}

function onEditCategory(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr")), wnd = $("#detail-category").getKendoWindow();
    if ($(e.currentTarget).is(".action-new") || $(e.currentTarget).is(".action-edit")) {
        if ($(e.currentTarget).is(".action-new")) item = { id: 0, category: "", related: "" };
        wnd.center().open();
        $("#category-id").val(item.id);
        $("#category-name").getKendoDropDownList().value(item.category);
        $("#related-name").getKendoDropDownList().value(item.related);
    }
    if ($(e.currentTarget).is(".action-delete")) {
        showConfirm(`¿Está seguro que desea eliminar la relación de la categor&iacute;a <b>${item.category}</b> con <b>${item.related}</b>? <br />Una vez realizada la acci&oacute;n no se puede revertir.`, function () {
            $.post(urlDeleteCategory, { Id: item.id }, d => {
                if (d.message === "") {
                    showMessage("Se ha eliminado la relaci&oacute;n.");

                    item = grd.dataSource.get(item.id);
                    grd.dataSource.remove(item);
                    grd.refresh();
                } else {
                    console.error(data.message);
                    showError("Se ha producido un error al eliminar la relaci&oacute;n.");
                }
            });
        });
    }
}

function onSavingCategory(e) {
    e.preventDefault();
    var category = $("#category-name").getKendoDropDownList().value(), related = $("#related-name").getKendoDropDownList().value(), item = { id: $("#category-id").val(), category: category, related: related };
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "" } }).data("kendoValidator");
    if (validator.validate()) {
        $.post(urlSaveCategory, { Item: item }, function (d) {
            if (d.message === "") {
                var grid = $("#categories").getKendoGrid();
                var newItem = { id: d.id, category: category, related: related };
                grid.dataSource.pushUpdate(newItem);
                $("#detail-category").getKendoWindow().close();
            } else {
                console.error(d.message);
                showError(`Se ha producido un error`)
            }
        });
    }
}

function onEditProduct(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr")), wnd = $("#detail-product").getKendoWindow();
    if ($(e.currentTarget).is(".action-new") || $(e.currentTarget).is(".action-edit")) {
        if ($(e.currentTarget).is(".action-new")) item = { id: 0, idProduct: 0, idRelated: 0, productCode: "", productName: "", relatedCode: "", relatedName: "" };
        wnd.center().open();
        $("#product-id").val(item.id);
        $("#product-name").getKendoDropDownList().value(item.idProduct);
        $("#related-product-name").getKendoDropDownList().value(item.idRelated);
    }
    if ($(e.currentTarget).is(".action-delete")) {
        showConfirm(`¿Está seguro que desea eliminar la relación del producto <b>${item.productCode}</b> con <b>${item.relatedCode}</b>? <br />Una vez realizada la acci&oacute;n no se puede revertir.`, function () {
            $.post(urlDeleteProduct, { Id: item.id }, d => {
                if (d.message === "") {
                    showMessage("Se ha eliminado la relaci&oacute;n.");

                    item = grd.dataSource.get(item.id);
                    grd.dataSource.remove(item);
                    grd.refresh();
                } else {
                    console.error(data.message);
                    showError("Se ha producido un error al eliminar la relaci&oacute;n.");
                }
            });
        });
    }
}

function closeWindow(e) {
    e.preventDefault();
    $(e.currentTarget).closest(".k-window-content").getKendoWindow().close();
}

function onSavingProduct(e) {
    e.preventDefault();
    var product = $("#product-name").getKendoDropDownList().dataItem(), relatedProduct = $("#related-product-name").getKendoDropDownList().dataItem(),
        item = { id: $("#product-id").val(), idProduct: product.id, idRelated: relatedProduct.id };
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "" } }).data("kendoValidator");
    if (validator.validate()) {
        $.post(urlSaveProduct, { Item: item }, function (d) {
            if (d.message === "") {
                var grid = $("#products").getKendoGrid(), insert = +item.id === 0, ds = grid.dataSource;
                if (!insert) {
                    item = ds.get(item.id);
                    item.idProduct = product.id;
                    item.idRelated = relatedProduct.id;
                }
                else
                    item.id = d.id;

                item.productCode = product.itemCode;
                item.productName = product.name;
                item.relatedCode = relatedProduct.itemCode;
                item.relatedName = relatedProduct.name;

                ds.pushUpdate(item);
                $("#detail-product").getKendoWindow().close();
            } else {
                console.error(d.message);
                showError(`Se ha producido un error`)
            }
        });
    }
}

function onEditBanner(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr")), wnd = $("#detail-banner").getKendoWindow();
    if ($(e.currentTarget).is(".action-new") || $(e.currentTarget).is(".action-edit")) {
        if ($(e.currentTarget).is(".action-new")) item = { id: 0, imageUrl: "", name: "", enabled: true, initialDate: new Date(), finalDate: null };

        $("#banner-id").val(item.id);
        $("#banner-name").val(item.name);
        $("#banner-enabled").getKendoSwitch().check(item.enabled);
        $("#banner-since").getKendoDatePicker().value(item.initialDate);
        $("#banner-until").getKendoDatePicker().value(item.finalDate);
        $("#banner-url").attr("src", $.trim(item.imageUrl) === "" ? urlNoImage : `${urlImages}promobanner/${item.imageUrl}`);
        $("#old-photo").val(item.imageUrl);
        setTimeout(() => wnd.center().open(), 100);
    }
    if ($(e.currentTarget).is(".action-delete")) {
        showConfirm(`¿Está seguro que desea eliminar el banner <b>${item.name}</b>? <br />Una vez realizada la acci&oacute;n no se puede revertir.`, function () {
            $.post(urlDeleteBanner, { Id: item.id }, d => {
                if (d.message === "") {
                    showMessage("Se ha eliminado el banner.");

                    item = grd.dataSource.get(item.id);
                    grd.dataSource.remove(item);
                    grd.refresh();
                } else {
                    console.error(data.message);
                    showError("Se ha producido un error al eliminar el banner.");
                }
            });
        });
    }
}

function onGalleryButtonClicked(e) {
    $("#gallery").val(null);
    $("#gallery").click();
}

function onPhotoRemoveClicked(e) {
    $("#banner-url").attr("src", urlNoImage);
    $("#new-photo").val("");
}

function onGalleryChanged(e) {
    if (this.files && this.files[0]) {
        var FR = new FileReader();
        FR.addEventListener("load", function (e) {
            $("#banner-url").attr("src", e.target.result);
            if (e.target.result !== "") {
                $.post(urlSaveImageBase64, { ImageBase64: e.target.result }, function (data) {
                    if (data.message !== "") showError(data.message);
                    $("#new-photo").val(data.fileName);
                });
            }
        });
        FR.readAsDataURL(this.files[0]);
    }
}

function onSavingBanner(e) {
    e.preventDefault();
    var item = {
        id: +$("#banner-id").val(), name: $("#banner-name").val(), enabled: $("#banner-enabled").getKendoSwitch().check(), initialDate: kendo.toString($("#banner-since").getKendoDatePicker().value(), "yyyy-MM-dd"),
        finalDate: kendo.toString($("#banner-until").getKendoDatePicker().value(), "yyyy-MM-dd"), imageUrl: $("#new-photo").val() || $("#old-photo").val()
    };
    $.post(urlSaveBanner, { Item: item }, function (d) {
        if (d.message === "") {
            var grid = $("#banners").getKendoGrid(), ds = grid.dataSource;
            item.id = d.id;
            ds.pushUpdate(item);
            $(e.currentTarget).closest(".k-window-content").getKendoWindow().close();
        } else {
            console.error(d.message);
            showError("Se ha producido un error al guardar el banner.");
        }
    });
}

function onEditBannerItem(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr")), wnd = $("#detail-banner-item").getKendoWindow();
    if ($(e.currentTarget).is(".action-new-item") || $(e.currentTarget).is(".action-edit-item")) {
        if ($(e.currentTarget).is(".action-new-item")) item = { id: 0, idPromo: +e.currentTarget.dataset.id, idProduct: 0, idSubsidiary: 0, price: 0 };

        $("#banner-item-id").val(item.id);
        $("#banner-id").val(item.idPromo);
        $("#banner-item-product").getKendoDropDownList().value(item.idProduct);
        $("#banner-item-subsidiary").getKendoDropDownList().value(item.idSubsidiary);
        $("#banner-item-price").val(item.price);
        setTimeout(() => wnd.center().open(), 100);
    }
    if ($(e.currentTarget).is(".action-delete-item")) {
        showConfirm(`¿Está seguro que desea eliminar el item <b>${item.productCode}</b> en <b>${item.subsidiaryName}</b>? <br />Una vez realizada la acci&oacute;n no se puede revertir.`, function () {
            $.post(urlDeleteBannerItem, { Id: item.id }, d => {
                if (d.message === "") {
                    showMessage("Se ha eliminado el item.");

                    item = grd.dataSource.get(item.id);
                    grd.dataSource.remove(item);
                    grd.refresh();
                } else {
                    console.error(data.message);
                    showError("Se ha producido un error al eliminar el item.");
                }
            });
        });
    }
}

function onSavingBannerItem(e) {
    e.preventDefault();
    var product = $("#banner-item-product").getKendoDropDownList().dataItem(), subsidiary = $("#banner-item-subsidiary").getKendoDropDownList().dataItem(),
        item = {
            id: +$("#banner-item-id").val(), idPromo: +$("#banner-id").val(), idProduct: product.id, productCode: product.itemCode, productName: product.name, idSubsidiary: subsidiary.id,
            subsidiaryName: subsidiary.name, price: +$("#banner-item-price").val()
        };
    $.post(urlSaveBannerItem, { Item: item }, function (d) {
        if (d.message === "") {
            var grid = $(`#items-${item.idPromo}`).getKendoGrid(), ds = grid.dataSource;
            item.id = d.id;
            ds.pushUpdate(item);
            $(e.currentTarget).closest(".k-window-content").getKendoWindow().close();
        } else {
            console.error(d.message);
            showError("Se ha producido un error al guardar el item.");
        }
    });
}

function onEditBannerTrigger(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr")), wnd = $("#detail-banner-trigger").getKendoWindow();
    if ($(e.currentTarget).is(".action-new-trigger") || $(e.currentTarget).is(".action-edit-trigger")) {
        if ($(e.currentTarget).is(".action-new-trigger")) item = { id: 0, idPromo: +e.currentTarget.dataset.id, idProduct: 0, category: "" };

        $("#banner-trigger-id").val(item.id);
        $("#banner-id").val(item.idPromo);
        $("#banner-trigger-product").getKendoDropDownList().value(item.idProduct);
        $("#banner-trigger-category").getKendoDropDownList().value(item.category);
        setTimeout(() => wnd.center().open(), 100);
    }
    if ($(e.currentTarget).is(".action-delete-trigger")) {
        showConfirm(`¿Est&aacute; seguro que desea eliminar el disparador? <br />Una vez realizada la acci&oacute;n no se puede revertir.`, function () {
            $.post(urlDeleteBannerTrigger, { Id: item.id }, d => {
                if (d.message === "") {
                    showMessage("Se ha eliminado el disparador.");

                    item = grd.dataSource.get(item.id);
                    grd.dataSource.remove(item);
                    grd.refresh();
                } else {
                    console.error(data.message);
                    showError("Se ha producido un error al eliminar el disparador.");
                }
            });
        });
    }
}

function onSavingBannerTrigger(e) {
    e.preventDefault();
    var product = $("#banner-trigger-product").getKendoDropDownList().dataItem(), cat = $("#banner-trigger-category").getKendoDropDownList(),
        item = { id: +$("#banner-trigger-id").val(), idPromo: +$("#banner-id").val(), idProduct: product?.id, productCode: product?.itemCode, productName: product?.name, category: cat.value() };
    $.post(urlSaveBannerTrigger, { Item: item }, function (d) {
        if (d.message === "") {
            var grid = $(`#triggers-${item.idPromo}`).getKendoGrid(), ds = grid.dataSource;
            item.id = d.id;
            ds.pushUpdate(item);
            $(e.currentTarget).closest(".k-window-content").getKendoWindow().close();
        } else {
            console.error(d.message);
            showError("Se ha producido un error al guardar el trigger.");
        }
    });
}

//#endregion