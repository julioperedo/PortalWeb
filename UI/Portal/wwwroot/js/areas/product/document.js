//#region GLOBAL VARIABLES
var _maxDate, _minDate, products = [], _lastRecieverGridHeight = null, _docId = -1;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
//#endregion

//#region EVENTS

$(window).resize(setHeights);

$(() => setupControls());

$("#Listado").on("click", ".product", onProductSelected);

$("#Documents").on("click", ".action-new", onEditingDocument);

$("#Documents").on("click", ".action-edit", onEditingDocument);

$("#Documents").on("click", ".action-delete", onDeletingDocument);

$("#Documents").on("click", ".send-email", onOpeningSendingConfig);

$("#Documents").on("click", ".download-files", onDownloadingFiles);

$("#fileList").on("click", ".delete-file", onDeletingFile);

$("#fileList").on("click", ".download-file", onDownloaingFile);

$("#save-document").click(onSavingDocument);

$("#Receivers").on("click", ".action-new", onEditingReceiver);

$("#Receivers").on("click", ".action-edit", onEditingReceiver);

$("#Receivers").on("click", ".action-delete", onDeletingReceiver);

$("#save-receiver").click(onSavingReceiver);

//al seleccionar una imagen en el editor arregla la dirección de una carpeta interna que sino aparece codificada
$(document).on("click", ".k-imagebrowser>.k-filemanager-listview>.k-listview-content>.k-listview-item", fixFolderUrl);

$("#available-docs").on("click", ".list-item", onItemClicked);

$("#available-clients").on("click", ".client-item", onItemClicked2);

$("#available-clients").on("click", ".selector", onAllItemClicked2);

$("#all-receivers").click(onAllItemClicked);

$("#send-email").click(onSendingEmail);

//$("#remove-file").click(onRemovingFile);

$("#open-mail-config").click(onOpenMailConfig);

$("#save-mail-settings").click(onSavingMailSettings);

$("#open-receivers-config").click(onOpenReceiversConfig);

//#endregion

//#region METHODS

function setupControls() {
    var listProducts = $("#Listado").kendoListView({ bordered: false, template: kendo.template($("#template").html()) }).getKendoListView();

    $("#Detail").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Documento de Producto", modal: true, width: 1000, open: onRefreshWindow });
    $("#DetailReceiver").kendoWindow({
        scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Personas de Contacto", modal: true, width: 1200, open: onRefreshWindow,
        close: e => $("#Receivers").data("kendoGrid").setOptions({ height: _lastRecieverGridHeight })
    });
    $("#Emails").kendoWindow({
        scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Enviar Documentos a Clientes", modal: true, width: 1200,
        open: function (e) {
            onRefreshWindow(e);
            e.sender.element.find(".k-listview-content").css("max-height", e.sender.wrapper.height() - 180);
        }
    });
    $("#email-settings").kendoWindow({ scrollable: true, visible: false, actions: ["Close"], resizable: false, title: "Configuraciones correo", modal: true, width: 800, open: onRefreshWindow });

    //Controles del formulario de edición de documentos
    var ddlTypesEdit = $("#typeIdc").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Tipo...", filter: "contains", dataSource: { data: [] } }).data("kendoDropDownList");
    var ddlLinesEdit = $("#idLine").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Línea...", filter: "contains", dataSource: { data: [] } }).data("kendoDropDownList");
    $("#idProduct").kendoDropDownList({
        dataSource: { serverFiltering: true, transport: { read: { url: urlGetProducts, data: getProductFilter } } },
        cascadeFrom: "idLine",
        dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Producto...", filter: "contains",
        virtual: { itemHeight: 26, valueMapper: productMapper }
    });
    $("#releaseDate").kendoDatePicker({ format: "dd-MM-yyyy" });
    $("#enabled").kendoSwitch({ messages: { checked: "", unchecked: "" } });
    $("#description").kendoEditor({
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "insertImage", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        imageBrowser: {
            fileTypes: "*.png,*.gif,*.jpg,*.jpeg", path: "productdocs/",
            transport: {
                read: { url: urlImageRead },
                uploadUrl: urlImageUpload,
                imageUrl: urlImage,
                destroy: { url: urlImageDestroy },
                create: { url: urlImageCreate },
                type: "filebrowser-aspnetmvc"
            }
        }
    });
    $("#files").kendoUpload({
        async: {
            saveUrl: urlSaveFile,
            removeUrl: urlRemoveFile,
            autoUpload: false
        },
        validation: { maxFileSize: 41943040 },
        success: function (e) {
            var filesData = $("#filesData"), data = JSON.parse(filesData.val());
            if (e.operation === "upload") {
                e.files.forEach(x => data.push({ id: _docId--, fileURL: x.name, statusType: 1 }));
            } else {
                e.files.forEach(x => data = data.filter(i => i.fileURL !== x.name || i.statusType !== 1));
            }
            filesData.val(JSON.stringify(data));
        }
    });
    var fileTemplate = `<div class="file-row">
    <div class="file-name">#=fileURL#</div>
    <div class="file-options">
        <a class="action action-link download-file" title="Descargar archivo"><i class="fas fa-download"></i></a> 
        <a class="action action-link delete-file" title="Eliminar archivo" data-id="#=id#"><i class="fas fa-trash"></i></a>
    </div>
</div>`;
    $("#fileList").kendoListView({ bordered: false, template: fileTemplate });


    //Controles del formulario de edición de receptores de correos
    $("#enabled-receiver").kendoSwitch({ messages: { checked: "", unchecked: "" } });
    $("#id-client-receiver").kendoDropDownList({
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", virtual: {
            itemHeight: 26, valueMapper: function (options) {
                var items = this.dataSource.data();
                var index = items.indexOf(items.find(i => i.code === options.value));
                options.success(index);
            }
        },
        dataSource: { transport: { read: { url: urlGetClients } } }
    });

    //Controles de el envío de correos
    var availableClientsTemplate = `<div class="client-group">
    <div>
        <span class="client-title">#= data.value #</span> <span class="selector">Todos</span>
    </div>
    <div class="clients-list">
        # for (var i = 0; i < data.items.length; i++) { #
        <div class="client-item action #if(data.items[i].selected) {# selected #}#" data-id="#=data.items[i].id#">
            #=data.items[i].name# <i></i>
        </div>
        # } #
    </div>
</div>`;
    $("#available-docs").kendoListView({ bordered: false, template: '<div class="list-item action #if(selected) {# selected #}#" data-id="#=id#">#=name# <i></i></div>' });
    $("#available-clients").kendoListView({ bordered: false, template: availableClientsTemplate });
    $("#show-signature").kendoSwitch({ messages: { checked: "", unchecked: "" } });

    //COntroles de configuración de los correos
    $("#mail-header").kendoEditor({
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "insertImage", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        imageBrowser: {
            fileTypes: "*.png,*.gif,*.jpg,*.jpeg", path: "productdocs/",
            transport: {
                read: { url: urlImageRead },
                uploadUrl: urlImageUpload,
                imageUrl: urlImage,
                destroy: { url: urlImageDestroy },
                create: { url: urlImageCreate },
                type: "filebrowser-aspnetmvc"
            }
        }
    });
    $("#mail-footer").kendoEditor({
        tools: [
            "bold", "italic", "underline", "strikethrough", "fontSize", "foreColor", "backColor", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink",
            "unlink", "insertImage", "tableWizard", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "viewHtml", "formatting", "cleanFormatting"
        ],
        imageBrowser: {
            fileTypes: "*.png,*.gif,*.jpg,*.jpeg", path: "productdocs/",
            transport: {
                read: { url: urlImageRead },
                uploadUrl: urlImageUpload,
                imageUrl: urlImage,
                destroy: { url: urlImageDestroy },
                create: { url: urlImageCreate },
                type: "filebrowser-aspnetmvc"
            }
        }
    });

    $.get(urlGetTypes, {}, d => {
        var ds = new kendo.data.DataSource({ data: d });
        ddlTypesEdit.setDataSource(ds);
    });
    $.get(urlGetLines, {}, d => {
        var ds = new kendo.data.DataSource({ data: d });
        ddlLinesEdit.setDataSource(ds);
    });
    $.get(urlGetAvailableProducts, {}, d => {
        products = d;
        var ds = new kendo.data.DataSource({ data: d, group: { field: "lineName", dir: 'asc' }, schema: { model: { id: "id" } } });
        listProducts.setDataSource(ds);
    });

    $("#Documents").kendoGrid({
        sortable: true,
        columns: [
            { title: "Tipo", field: "typeName", width: 250, media: "lg" },
            { title: "Nombre", field: "name", width: 550, media: "lg" },
            { title: "Fecha", field: "releaseDate", width: 90, format: dateFormat, attributes: alignCenter, headerAttributes: alignCenter, media: "lg" },
            { title: "Habilitado", field: "enabled", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: e => e.enabled ? '<i class="fas fa-check"></i>' : '', media: "lg" },
            {
                title: "Detalles", media: "(max-width: 991px)",
                template: e => `<table class="no-format"><tr><td>Tipo:</td><td>${e.typeName}</td></tr><tr><td>Nombre:</td><td>${e.name}</td></tr><tr><td>Fecha:</td><td>${kendo.toString(e.releaseDate, "dd-MM-yyyy")}</td></tr><tr><td>Habilitado:</td><td>${e.enabled ? "Si" : "No"}</td></tr></table>`
            },
            {
                title: " ", attributes: alignCenter, width: 120, sortable: false, headerAttributes: alignCenter, sticky: true,
                headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                template: e => `<a class="download-files action ${(e.listDocumentFiles && e.listDocumentFiles.length > 0 ? "action-link" : "disabled")}" title="Descargar Archivo" ${($.trim(e.fileURL) === "" ? 'disabled="disabled"' : "")}><i class="fas fa-download"></i></a><a class="action action-link send-email" title="Enviar Documento por E-Mail"><i class="fas fa-envelope"></i></a><a class="action action-link action-edit"><i class="fas fa-pen" title="Editar"></i></a><a class="action ${(!_deletePremission ? "disabled" : "action-link")} action-delete" ${(!_deletePremission ? 'disabled="disabled"' : "")}><i class="fas fa-trash-alt" title="Eliminar"></i></a>`
            }
        ],
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([], "Documents"),
        dataBound: e => e.sender.element.find("table").attr("style", "")
    });
    $("#Receivers").kendoGrid({
        sortable: true,
        columns: [
            { title: "Nombre", field: "cardName", width: 250, media: "lg" },
            { title: "Nombre", field: "name", width: 280, media: "lg" },
            { title: "Correo", field: "eMail", width: 240, media: "lg" },
            { title: "Habilitado", field: "enabled", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: e => e.enabled ? '<i class="fas fa-check"></i>' : '', media: "lg" },
            {
                title: " ", attributes: alignCenter, width: 70, sortable: false, headerAttributes: alignCenter, sticky: true,
                headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                template: e => '<a class="action action-link action-edit"><i class="fas fa-pen" title="Editar"></i></a><a class="action action-link action-delete"><i class="fas fa-trash-alt" title="Eliminar"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([], "Receivers"),
        dataBound: e => e.sender.element.find("table").attr("style", "")
    });

    setHeights();
}

function getProductFilter(e) {
    var idLine = 0, filter = "";
    e.filter.filters.forEach(x => {
        if (x.field === "id")
            idLine = x.value;
        else
            filter = x.value;
    });
    return { LineId: idLine, Filter: filter };
}

function productMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.id === options.value));
    options.success(index);
}

function setHeights() {
    $("#Listado .k-listview-content").height($(window).height() - 104);
}

function onProductSelected(e) {
    $(e.currentTarget).closest(".k-listview-content").find(".product").removeClass("selected");
    $(e.currentTarget).addClass("selected");
    var idProduct = e.currentTarget.dataset.id;

    $.get(urlFilter, { IdProduct: idProduct }, d => {
        if (d.message === "") {
            loadGrid(d.documents, "Documents");
            loadGrid(d.receivers, "Receivers");
        } else {
            showError(d.message);
        }
    });
}

function fixFolderUrl() {
    var input = $("#k-editor-image-url")[0];
    if (input && /%2F/.test(input.value)) input.value = input.value.replace(/%2F/g, "/").replace(/%20/g, " ").replace("productdocs/", "");
}

function loadGrid(items, gridName) {
    var grd = $("#" + gridName).data("kendoGrid");
    grd.setDataSource(getDataSource(items, gridName));
    //setGridHeight("Documents", gridMargin);
}

function getDataSource(items, gridName) {
    if (gridName === "Documents") items.forEach(x => x.releaseDate = JSON.toDate(x.releaseDate));
    var ds = new kendo.data.DataSource({ data: items, sort: [{ field: "id", dir: "asc" }], schema: { model: { id: "id" } } });
    return ds;
}

function onDownloadingFiles(e) {
    e.preventDefault();

    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);

    window.location.href = urlDownloadFiles + "?" + $.param({ Id: item.id });
}

function onEditingDocument(e) {
    e.preventDefault();

    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item, wnd = $("#Detail").data("kendoWindow"), isNew = e.currentTarget.classList.contains("action-new");
    if (isNew) {
        grd.clearSelection();
        item = { id: 0, name: "", description: "", typeIdc: 0, releaseDate: new Date(), idLine: 0, idProduct: 0, fileURL: "", enabled: true, listDocumentFiles: [] };
        var sel = $("div.product.selected");
        if (sel.length > 0) {
            var d = sel.data();
            item.idProduct = d.id;
            item.idLine = d.idline;
        }
    } else {
        var row = $(e.currentTarget).closest("tr");
        grd.select(row);
        item = grd.dataItem(row);
    }

    $("#id").val(item.id);
    $("#name").val(item.name);
    $("#description").getKendoEditor().value(formatHTMLSafe(item.description));
    $("#typeIdc").getKendoDropDownList().value(item.typeIdc);
    $("#releaseDate").getKendoDatePicker().value(item.releaseDate);
    $("#idLine").getKendoDropDownList().value(item.idLine);
    $("#idProduct").getKendoDropDownList().value(item.idProduct);
    $("#fileList").getKendoListView().setDataSource(new kendo.data.DataSource({ data: item.listDocumentFiles, schema: { model: { id: "id" } } }));
    $("#filesData").val(JSON.stringify(item.listDocumentFiles));
    $("#enabled").getKendoSwitch().check(item.enabled);
    $("#files").data("kendoUpload").clearAllFiles();

    //$("#file-wrapper").toggleClass("d-none", item.fileURL !== "");
    //$("#file-name").closest(".input-group").toggleClass("d-none", item.fileURL === "");

    wnd.center().open();
}

function onDeletingDocument(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el documento <b>${item.name}</b> del producto <b>${item.productName}</b>? <br />Una vez realizada la acci&oacute;n no se puede revertir.`, function () {
        $.post(urlDelete, { Id: item.id }, d => {
            if (d.message === "") {
                showMessage("Se ha eliminado el documento.");

                item = grd.dataSource.get(item.id);
                grd.dataSource.remove(item);
                grd.refresh();
            } else {
                console.error(data.message);
                showError("Se ha producido un error al eliminar el documento.");
            }
        });
    });
}

function onSavingDocument(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "" } }).data("kendoValidator");
    if (validator.validate()) {
        var item = form.serializeObject();
        item.enabled = $("#enabled").getKendoSwitch().check();
        item.listDocumentFiles = JSON.parse($("#filesData").val());
        $.post(urlEdit, { Item: item }, function (d) {
            if (d.message === "") {
                showMessage("Se realizaron los cambios correctamente.");
                loadGrid(d.items, "Documents");

                var prod = products.find(x => x.id === +item.idProduct);
                if (!prod) {
                    var fullName = $("#idProduct").getKendoDropDownList().text(), code, name;
                    var i = fullName.indexOf(" - ");
                    code = fullName.substring(0, i + 1);
                    name = fullName.substring(i + 3);
                    products.push({ id: item.idProduct, idLine: item.idLine, itemCode: code, lineName: $("#idLine").getKendoDropDownList().text(), name: name });
                    var list = $("#Listado").getKendoListView();
                    var ds = new kendo.data.DataSource({ data: products, group: { field: "lineName", dir: 'asc' } });
                    list.setDataSource(ds);
                }
                $("#Listado .product").removeClass("selected");
                $("#Listado .product[data-id='" + item.idProduct + "']").addClass("selected");
                $("#Detail").data("kendoWindow").close();
            } else {
                console.error(data.message);
                showError("Se ha producido un error al guardar el documento.");
            }
        });
    }
}

function onItemClicked(e) {
    $(e.currentTarget).toggleClass("selected");
    $("#send-email").prop("disabled", $("#available-docs .list-item.selected").length === 0 || $("#available-clients .client-item.selected").length === 0);
}

function onItemClicked2(e) {
    $(e.currentTarget).toggleClass("selected");
    $("#send-email").prop("disabled", $("#available-docs .list-item.selected").length === 0 || $("#available-clients .client-item.selected").length === 0);
}

function onAllItemClicked2(e) {
    var group = $(e.currentTarget).closest(".client-group");
    group.find(".client-item").toggleClass("selected", group.find(".client-item").length !== group.find(".client-item.selected").length);
    $("#send-email").prop("disabled", $("#available-docs .list-item.selected").length === 0 || $("#available-clients .client-item.selected").length === 0);
}

function onAllItemClicked(e) {
    var group = $("#available-clients");
    group.find(".client-item").toggleClass("selected", group.find(".client-item").length !== group.find(".client-item.selected").length);
    $("#send-email").prop("disabled", $("#available-docs .list-item.selected").length === 0 || $("#available-clients .client-item.selected").length === 0);
}

function onDeletingFile(e) {
    var data = e.currentTarget.dataset, items = JSON.parse($("#filesData").val()), item, lv = $("#fileList").getKendoListView(),
        row = lv.dataSource.get(data.id), id = +data.id;

    item = items.find(x => x.id === id);
    if (item === 1) {
        items = items.filter(x => x.id !== id);
    } else {
        item.statusType = 3;
    }
    lv.dataSource.remove(row);
    $("#filesData").val(JSON.stringify(items));
}

function onDownloaingFile(e) {
    window.location.href = urlDownloadFile + "?" + $.param({ FileName: $(e.currentTarget).closest(".file-row").find(".file-name").text() });
}

function onOpeningSendingConfig(e) {
    var btn = $(e.currentTarget), grd = btn.closest(".k-grid").data("kendoGrid"), row = btn.closest("tr"), item = grd.dataItem(row), wnd = $("#Emails").data("kendoWindow");
    grd.select(row);
    $("#send-email").prop("disabled", true);

    $.get(urlConfigMail, { Id: item.id, IdProduct: item.idProduct }, d => {
        if (d.message === "") {
            $("#available-docs").getKendoListView().setDataSource(new kendo.data.DataSource({ data: d.documents }));
            $("#available-clients").getKendoListView().setDataSource(new kendo.data.DataSource({ data: d.receivers, group: { field: "cardName", dir: "asc" } }));
            $("#show-signature").data("kendoSwitch").check(true);
            wnd.open();
        } else {
            console.error(data.message);
            showError("Se ha producido un error al traer la configuración de los Correos.");
        }
    });
}

function onSendingEmail(e) {
    var docs = [], clients = [], withSignature = false, subject = $("#email-subject").val();
    $("#available-docs .list-item.selected").each((i, v) => docs.push(+v.dataset.id));
    $("#available-clients .client-item.selected").each((i, v) => clients.push(+v.dataset.id));
    withSignature = $("#show-signature").data("kendoSwitch").check();
    $.get(urlSendMail, { Subject: subject, DocumentIds: docs.join(), ClientsIds: clients.join(), WithSignature: withSignature }, d => {
        if (d.message === "") {
            showMessage("Correo enviado exitosamente");
            $("#Emails").data("kendoWindow").close();
        } else {
            console.error(d.message);
            showError("Se ha producido un error al traer la configuración de los Correos.");
        }
    });
}

function onOpenReceiversConfig(e) {
    $.get(urlAllReceivers, {}, function (d) {
        if (d.message === "") {
            loadGrid(d.items, "Receivers");
            $("#receiver-form").addClass("d-none");
            $("#DetailReceiver").data("kendoWindow").center().open();
        } else {
            showError("Se ha producido un error al traer los datos del servidor, por favor intente de nuevo y si el problema persiste comun&iacute;quese con soporte t&eacute;cnico.");
        }
    });
}

function onEditingReceiver(e) {
    e.preventDefault();

    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item, isNew = e.currentTarget.classList.contains("action-new");
    if (isNew) {
        grd.clearSelection();
        item = { id: 0, name: "", eMail: "", cardCode: "", cardName: "", enabled: true };
        var sel = $("div.product.selected");
        if (sel.length > 0) {
            var d = sel.data();
            item.idProduct = d.id;
            item.idLine = d.idline;
        }
    } else {
        var row = $(e.currentTarget).closest("tr");
        grd.select(row);
        item = grd.dataItem(row);
    }

    $("#id-receiver").val(item.id);
    $("#name-receiver").val(item.name);
    $("#email-receiver").val(item.eMail);
    $("#id-client-receiver").getKendoDropDownList().value(item.cardCode);
    $("#enabled-receiver").getKendoSwitch().check(item.enabled);

    _lastRecieverGridHeight = !_lastRecieverGridHeight ? $(e.currentTarget).closest(".k-grid").height() : _lastRecieverGridHeight;
    console.log(_lastRecieverGridHeight);
    grd.setOptions({ height: $("#DetailReceiver").height() - 250 });
    $("#receiver-form").removeClass("d-none");
}

function onDeletingReceiver(e) {
    e.preventDefault();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item = grd.dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el subscriptor <b>${item.name}</b>?`, function () {
        $.post(urlDeleteReceiver, { Id: item.id }, d => {
            if (d.message === "") {
                showMessage("Se ha eliminado el subscriptor.");

                //var grd = $("#Receiver").getKendoGrid();
                item = grd.dataSource.get(item.id);
                grd.dataSource.remove(item);
                grd.refresh();
            } else {
                console.error(data.message);
                showError("Se ha producido un error al eliminar el subscriptor.");
            }
        });
    });
}

function onSavingReceiver(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator({ messages: { required: "", email: "no es un correo electrónico válido" } }).data("kendoValidator");
    if (validator.validate()) {
        var t = form.serializeObject(),
            item = { id: t["id-receiver"], cardCode: t["id-client-receiver"], name: t["name-receiver"], eMail: t["email-receiver"], enabled: $("#enabled-receiver").getKendoSwitch().check() };

        $.post(urlEditReceiver, { Item: item }, d => {
            if (d.message === "") {
                showMessage("Se realizaron los cambios correctamente.");
                item.cardName = $("#id-client-receiver").getKendoDropDownList().text();
                var grd = $("#Receivers").data("kendoGrid");
                grd.dataSource.pushUpdate(item);
                $("#receiver-form").addClass("d-none");
                grd.setOptions({ height: _lastRecieverGridHeight });
            } else {
                console.error(d.message);
                showError("Se ha producido un error al guardar el subscriptor.");
            }
        });
    }
}

function onOpenMailConfig(e) {
    var wnd = $("#email-settings").data("kendoWindow");
    $.get(urlGetMailConfig, {}, function (d) {
        if (d.message === "") {
            $("#mail-settings-id").val(d.item.id);
            $("#mail-header").data("kendoEditor").value(d.item.header);
            $("#mail-footer").data("kendoEditor").value(d.item.footer);
            wnd.open();
        } else {
            console.error(data.message);
            showError("Se ha producido un error al traer la configuración de los Correos.");
        }
    });
}

function onSavingMailSettings(e) {
    var item = { Id: $("#mail-settings-id").val(), Header: $("#mail-header").data("kendoEditor").value(), Footer: $("#mail-footer").data("kendoEditor").value() };
    $.post(urlSaveMailConfig, { Item: item }, function (d) {
        if (d.message === "") {
            showMessage("Datos guardados exitosamente.");
            $("#email-settings").data("kendoWindow").close();
        } else {
            console.error(data.message);
            showError("Se ha producido un error al traer la configuración de los Correos.");
        }
    });
}

//#endregion