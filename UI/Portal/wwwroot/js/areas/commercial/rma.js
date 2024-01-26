//#region Variables Globales
var _minDate, _maxDate, pageLoad = true, localUser, userCode, statuses, technicians, clients, countries, cities, locations, callTypes, priorities, problemTypes, origins, users, solutionStatuses, tempId = 0;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, perActivities = $("#permission").val();
//#endregion

//#region Eventos

$(document).ajaxError((event, jqxhr, settings, thrownError) => catchError(event, jqxhr, settings, thrownError));

$(() => setupControls());

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#FilId").keyup(e => setEnableFilters($(e.currentTarget).val() === ""));

$("#action-filter").click(() => filterData());

$("#action-clean").click(() => cleanFilters());

$("#action-excel").click(() => exportExcel());

$("#Listado").on("click", ".action-new", e => openDetail());

$("#Listado").on("click", ".action-edit", e => openDetail(e));

$("#Listado").on("click", ".action-delete", e => deleteServiceCall(e));

$("body").on("click", ".see-detail", e => openDetail(e));

$("body").on("click", ".attach", e => openFile(e));

$("body").on("click", ".action-new-activity", e => openActivity(e, true, true));

$("body").on("click", ".action-edit-activity", e => openActivity(e, false, true));

$("body").on("click", ".action-delete-activity", e => onDeleteActivity(e));

$("body").on("click", ".action-new-repair", e => openRepair(e, true));

$("body").on("click", ".action-edit-repair", e => openRepair(e, false));

$("body").on("click", ".action-delete-repair", e => onDeleteRepair(e));

$("body").on("click", ".save-repair", e => saveRepair(e));

$("body").on("click", ".product-card", e => getProductCardData(e, true, true));

$("body").on("keypress", ".product-card-search input", e => getProductCardData(e, e.keyCode === 13, false));

$("body").on("input", ".product-card-search input", e => loadInputsProductCard(e.currentTarget.dataset.code, null));

$("body").on("click", ".other-serial-number", e => getOtherServiceCallsFromSerial(e));

$("body").on('shown.bs.tab', 'a[data-toggle="tab"]', e => $(e.currentTarget).closest(".my-window").data("kendoWindow").center());

$("body").on("click", ".related-doc", e => openRelatedDoc(e));

$("body").on("click", ".save-activity", e => saveActivity(e));

$("body").on("click", ".photo-camera", e => openCamera(e));

$("body").on("click", ".photo-gallery", e => openGallery(e));

$("body").on("change", ".gallery", e => onGalleryFileSelected(e));

$("body").on("click", ".delete-file", e => deleteFile(e));

$("#take-photo-ok").click(e => onTakePhoto(e));

$("#take-photo-cancel").click(e => onCancelTekePhoto(e));

$("body").on("click", ".save-call-service", e => saveServiceCall(e));

//#endregion

//#region Métodos Locales

function setupControls() {
    userCode = $("#user-code").val();
    localUser = $("#local-user").val() === "Y";
    var filSince = $("#FilSince").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#FilUntil").kendoDatePicker({
        format: "d/M/yyyy", change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    filterData();

    var filState = $("#FilState"), filBrand = $("#FilBrand"), filLine = $("#FilLine");
    filState.multipleSelect({ placeholder: "Selecciona al menos un Estado..." });
    filBrand.multipleSelect({ placeholder: "Selecciona al menos una Marca..." });
    filLine.multipleSelect({ placeholder: "Selecciona al menos una Línea..." });
    $.get(urlStatuses, {}, function (data) {
        var codes = [];
        statuses = data;
        data.forEach((i) => {
            filState.append(new Option(i.name, i.id));
            if (i.id !== -1) codes.push(i.id);
        });
        filState.multipleSelect("refresh");
        filState.multipleSelect("setSelects", codes);
    });
    $.get(urlBrands, {}, function (data) {
        data.forEach((i) => filBrand.append(new Option(i.name, i.name)));
        filBrand.multipleSelect("refresh");
    });
    $.get(urlLines, {}, function (data) {
        data.forEach((i) => filLine.append(new Option(i.name, i.name)));
        filLine.multipleSelect("refresh");
    });
    $.get(urlTechnicians, {}, d => technicians = d);
    $.get(urlClients, {}, d => clients = d);
    $.get(urlCities, {}, d => cities = d);
    $.get(urlCountries, {}, d => countries = d);
    $.get(urlLocations, {}, d => locations = d);
    $.get(urlCallTypes, {}, d => callTypes = d);
    $.get(urlOrigins, {}, d => origins = d);
    $.get(urlProblemTypes, {}, d => problemTypes = d);
    $.get(urlGetUsers, {}, d => users = d);
    $.get(urlGetSolutionStatuses, {}, d => solutionStatuses = d);
    priorities = [{ id: 0, name: "Baja" }, { id: 1, name: "Media" }, { id: 2, name: "Alta" }];

    if (localUser) {
        $("#FilClient").kendoDropDownList({ dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", dataSource: { transport: { read: { url: urlClients } } }, virtual: { itemHeight: 26, valueMapper: clientMapper } });
        $("#FilTechnician").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Técnico...", filter: "contains", dataSource: { transport: { read: { url: urlTechnicians } } } });
        $("#FilProductManager").kendoDropDownList({ dataTextField: "name", dataValueField: "name", optionLabel: "Seleccione un G. de Producto...", filter: "contains", dataSource: { transport: { read: { url: urlPManagers } } } });
    }

    $("#Listado").kendoGrid({
        columns: [
            { title: "Número", width: 90, field: "code", attributes: alignRight, headerAttributes: alignRight },
            { title: "Estado", width: 150, field: "status", aggregates: ["count"], groupHeaderTemplate: 'Estado: #= value #    ( Total: #= count# )' },
            { title: "Técnico", field: "technician", aggregates: ["count"], groupHeaderTemplate: 'Técnico: #= value #    ( Total: #= count# )', width: 230 },
            { title: "F. Ingreso", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "createDate", format: dateFormat },
            { title: "F. Cierre", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "closeDate", format: dateFormat },
            { title: "D. Abierto", width: 90, field: "openDays", attributes: alignRight, headerAttributes: alignRight },
            { title: "Cliente", field: "clientName", width: 220, attributes: { class: "text-nowrap" }, aggregates: ["count"], groupHeaderTemplate: 'Cliente: #= value #    ( Total: #= count# )' },
            { title: "Usuario Final", field: "finalUser", width: 220, attributes: { class: "text-nowrap" }, aggregates: ["count"], groupHeaderTemplate: 'Usuario Final: # if(value) {# #= value # #} #    ( Total: #= count# )' },
            { title: "Ciudad", field: "city", width: 120 },
            { title: "Producto", width: 180, field: "itemCode", aggregates: ["count"], groupHeaderTemplate: 'Producto: #= value #    ( Total: #= count# )' },
            { title: "Marca", width: 120, field: "brand", aggregates: ["count"], groupHeaderTemplate: 'Marca: #= value #    ( Total: #= count# )' },
            { title: "F. Compra", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "purchaseDate", format: dateFormat },
            { title: "Garantía", width: 90, field: "warranty", aggregates: ["count"], groupHeaderTemplate: 'Garantía: #= value #    ( Total: #= count# )' },
            {
                title: " ", attributes: alignCenter, width: 50, sortable: false, headerAttributes: alignCenter, sticky: true, headerTemplate: '<i class="fas fa-plus action action-link action-new" title="Nuevo"></i>',
                template: function (e) {
                    var t = '<i class="fas fa-pen action action-link action-edit" title="Editar"></i>';
                    if (e.portalId > 0) {
                        t += '&nbsp;&nbsp;<i class="fas fa-trash-alt action action-link action-delete" title="Eliminar"></i>';
                    }
                    return t;
                }
            }
        ],
        groupable: { messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" }, enabled: true },
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
        dataSource: getDataSource([]),
        excel: { fileName: "RMA.xlsx" },
        dataBound: function (e) {
            var grid = e.sender;
            for (var i = 0; i < grid.columns.length; i++) {
                grid.showColumn(i);
            }
            $("div.k-group-indicator").each(function (i, v) {
                grid.hideColumn($(v).data("field"));
            });
            grid.element.find("table").attr("style", "");
        },
        //detailInit: function (e) {
        //    $.get(urlHistory, { Id: e.data.id }, function (data) {
        //        if (data.message === "") {
        //            data.items.forEach((x) => {
        //                x.createDate = JSON.toDate(x.createDate);
        //                x.updateDate = JSON.toDate(x.updateDate);
        //            });
        //            $("<div>").appendTo(e.detailCell).kendoGrid({
        //                scrollable: false,
        //                sortable: true,
        //                pageable: false,
        //                selectable: true,
        //                columns: [
        //                    { field: "order", title: "Número", width: 100, attributes: alignRight, headerAttributes: alignRight },
        //                    { field: "updateDate", title: "Fecha", format: "{0:dd-MM-yyyy HH:mm}", width: 150, attributes: alignCenter, headerAttributes: alignCenter },
        //                    { field: "stateName", title: "Estado", width: 200 },
        //                    { field: "subject", title: "Descripción" }
        //                ],
        //                dataSource: data.items
        //            });
        //        } else {
        //            console.error(data.message);
        //            showError('Se ha producido un error al traer los datos del servidor.');
        //        }
        //    });
        //}
    });

    $("#Report").kendoWindow({ visible: false, width: 1100, title: "Producto", modal: true });
    $("#detail").kendoWindow({
        visible: false,
        scrollable: true, modal: true, width: 1100, iframe: false,
        close: onCloseWindow,
        activate: function (e) {
            var wnd = this;
            setTimeout(() => {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });
    $("#detailCamera").kendoWindow({ visible: false, width: 346, title: "Sacar fotografía", modal: true });
}

function clientMapper(options) {
    var items = this.dataSource.data();
    var index = items.indexOf(items.find(i => i.code === options.value));
    options.success(index);
}

function getDataSource(items) {
    $.each(items, function (i, obj) {
        obj.createDate = JSON.toDate(obj.createDate);
        if (obj.admissionDate !== null) obj.admissionDate = JSON.toDate(obj.admissionDate);
        if (obj.closeDate !== null) obj.closeDate = JSON.toDate(obj.closeDate);
        obj.purchaseDate = JSON.toDate(obj.purchaseDate);
        if (obj.clientPurchaseDate !== null) obj.clientPurchaseDate = JSON.toDate(obj.clientPurchaseDate);
        if (obj.deliveredDate !== null) obj.deliveredDate = JSON.toDate(obj.deliveredDate);
    });
    var ds = new kendo.data.DataSource({
        data: items,
        aggregate: [
            { aggregate: "count", field: "technicianName" },
            { aggregate: "count", field: "clientName" },
            { aggregate: "count", field: "finalUser" }
        ],
        //group: [{ field: "technicianName", dir: "asc", aggregates: [{ field: "technicianName", aggregate: "count" }] }],
        sort: [{ field: "id", dir: "asc" }],
        schema: { model: { id: "code" } }
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
    var id = $("#FilId").val(), objTech = $("#FilTechnician").data("kendoDropDownList"), initialDate = $(`#FilSince`).data("kendoDatePicker").value(), finalDate = $(`#FilUntil`).data("kendoDatePicker").value(), technician = "", stateSelected = $("#FilState").multipleSelect("getSelects")
        , stateCode = Enumerable.From(stateSelected).Select(function (x) { return `'${x}'` }).ToArray().join(), message = "", brandSelected = $("#FilBrand").multipleSelect("getSelects"), brandCode = Enumerable.From(brandSelected).Select(x => `'${x}'`).ToArray().join()
        , lineSelected = $("#FilLine").multipleSelect("getSelects"), lineCode = Enumerable.From(lineSelected).Select(x => `'${x}'`).ToArray().join(), productManager = "", serial = $.trim($("#FilSerial").val()), product = $.trim($("#FilProduct").val());
    if (localUser) {
        technician = objTech.value();
        clientCode = $("#FilClient").data("kendoDropDownList").value();
        productManager = $("#FilProductManager").data("kendoDropDownList").value();
    } else {
        clientCode = $("#FilClient").val();
    }
    if (initialDate) {
        initialDate = initialDate.toISOString();
    } else {
        if ($.trim(stateCode).indexOf("-1") >= 0 && serial === "") {
            message += "- Fecha Inicial<br />";
        }
    }
    if (finalDate) {
        finalDate = finalDate.toISOString();
    } else {
        if ($.trim(stateCode).indexOf("-1") >= 0 && serial === "") {
            message += "- Fecha Final<br />";
        }
    }
    return {
        message: message,
        data: { Id: id, ClientCode: clientCode, InitialDate: initialDate, FinalDate: finalDate, Technician: technician, StateCodes: stateCode, Brands: brandCode, Lines: lineCode, ProductManager: productManager, Serial: serial, Product: product }
    };
}

function filterData() {
    hideNotification();
    var filter = { message: "" };
    if (pageLoad) {
        if (localUser) {
            filter = { data: { StateCodes: "0" }, message: "" };
        } else {
            filter = { data: { StateCodes: "0", ClientCode: $("#FilClient").val() }, message: "" };
        }
        pageLoad = false;
    } else {
        filter = getFilters();
    }
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
    hideNotification();
    $("#FilId").val("");
    $(`#FilSince`).data("kendoDatePicker").value("");
    $(`#FilUntil`).data("kendoDatePicker").value("");
    if (localUser) {
        $("#FilClient").data("kendoDropDownList").value("");
        $("#FilTechnician").data("kendoDropDownList").value("");
        $("#FilProductManager").data("kendoDropDownList").value("");
    }
    $("#FilState").multipleSelect("uncheckAll");
    $("#FilLine").multipleSelect("uncheckAll");
    $("#FilBrand").multipleSelect("uncheckAll");
    $("#FilProduct").val("");
    $("#FilSerial").val("");
    setEnableFilters(true);
}

function setEnableFilters(enabled) {
    var objSince = $("#FilSince").data("kendoDatePicker"), objUntil = $("#FilUntil").data("kendoDatePicker"), objClient = $("#FilClient").data("kendoDropDownList"), objTechnician = $("#FilTechnician").data("kendoDropDownList"), objProductManager = $("#FilProductManager").data("kendoDropDownList");
    objSince.enable(enabled);
    objUntil.enable(enabled);
    if (localUser) {
        objClient.enable(enabled);
        objTechnician.enable(enabled);
        objProductManager.enable(enabled);
    }
    if (enabled) {
        $("#FilState").multipleSelect("enable");
        $("#FilLine").multipleSelect("enable");
        $("#FilBrand").multipleSelect("enable")
    } else {
        $("#FilState").multipleSelect("uncheckAll").multipleSelect("disable");
        $("#FilLine").multipleSelect("uncheckAll").multipleSelect("disable");
        $("#FilBrand").multipleSelect("uncheckAll").multipleSelect("disable")

        if (localUser) {
            objClient.value("");
            objTechnician.value("");
            objProductManager.value("");
        }
        objSince.value("");
        objUntil.value("");
        $("#FilProduct").val("");
        $("#FilSerial").val("");
    }
}

function loadReport(Id, Subsidiary, Report) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    if (Report === "Note") {
        strReport = "SaleNote.trdp";
    }
    if (Report === "Delivery") {
        strReport = "DeliveryNote.trdp";
        objParams.SearchType = 2;
    }
    if (Report === "Bill") {
        strReport = "Bill.trdp";
    }
    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource({ report: strReport, parameters: objParams });
            viewer.refreshReport();
        } catch (e) {
            console.error(`Error al recargar el reporte: ${e}`);
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        try {
            $("#reportViewer1").telerik_ReportViewer({
                serviceUrl: urlService,
                reportSource: { report: strReport, parameters: objParams },
                viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
                scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
            });
        } catch (e) {
            console.error(`Error al cargar el reporte: ${e}`);
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    }
}

function getFileButton(edit, id, fileName, url, line, enableDelete) {
    if (edit) {
        return `<div class="btn-group btn-file" role="group">
                    <button type="button" class="btn btn-outline-secondary ${url !== '' ? 'attach' : ''}" title="Descargar archivo" data-url="${url}" ${url !== '' ? '' : 'disabled'}>${fileName}</button>
                    <button type="button" class="btn btn-outline-danger delete-file" title="Eliminar archivo" data-id="${id}" data-name="${fileName}" data-existing="${line === 0 ? 'N' : 'Y'}" data-line="${line}" ${line === 0 || enableDelete ? '' : 'disabled'}><i class="fas fa-times"></i></button>
                </div>`;
    } else {
        return `<a class="attach action action-link" data-url="${url}">${fileName}</a>`
    }
}

function openDetail(e) {
    var item, modal = true;
    hideNotification();
    if (e) {
        $(".custom-tab li:first-child a").tab("show");
        var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr");
        item = grd.dataItem(row);
        grd.select(row);
    } else {
        item = {
            code: "0", openDays: 0, finalUserPhone: "", finalUserEMail: "", country: "", callType: "", priority: 1, originCode: "", problemTypeCode: "", city: "", cityCode: "", createDate: new Date(), clientCode: "", clientName: "",
            itemCode: "", itemName: "", serialNumber: "", resolution: "", resolutionDate: "", room: "", startDate: "", state: "", statusCode: "-3", subject: "", technicianCode: "", admissionDate: "", finalUser: "", deliveryDate: "",
            deliveredBy: "", externalService: "", externalServiceAddress: "", externalServiceTechnician: "", externalServiceNumber: "", guideNumber: "", transport: "", stateCode: "", locationCode: "", countedPieces: 0,
            priorCountedPieces: 0, diffCountedPieces: 0, purchaseDate: "", reportedBy: "", receivedBy: "", refNV: "", comments: "", warranty: "", city2: "", location: "", street: "", brand: "", countryCode: "BO", listServiceCallFiles: []
        };
    }

    if (item.fileName) {
        if (typeof item.fileName === "string") {
            item.fileName = item.fileName.split("; ");
        }
    } else {
        item.fileName = [];
    }

    var template = kendo.template($("#template-service-call-edit").html());
    var contentRMA = template(item);

    var wnd = $("<div>").addClass("my-window").kendoWindow({ visible: false, modal: modal, iframe: false, title: "", resizable: false, width: 1100, actions: ["Close"], close: onCloseWindow, open: onRefreshWindow }).data("kendoWindow");
    wnd.content(contentRMA);
    if (item.code === "0") $(`#save-button-${item.code}`).attr("disabled", "disabled");
    $(`#create-date-${item.code}`).kendoDateTimePicker({ format: "dd-MM-yyyy HH:mm", componentType: "modern", culture: "es-BO", value: item.createDate });
    $(`#close-date-${item.code}`).kendoDateTimePicker({ format: "dd-MM-yyyy HH:mm", componentType: "modern", culture: "es-BO", value: item.closeDate });
    $(`#purchase-date-${item.code}`).kendoDatePicker({ format: "dd-MM-yyyy", culture: "es-BO", value: item.purchaseDate });
    $(`#status-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Estado...", dataSource: { data: statuses } });
    $(`#technician-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Técnico...", dataSource: { data: technicians } });
    $(`#client-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", dataSource: { data: clients }, virtual: { itemHeight: 26, valueMapper: clientMapper } });
    $(`#city-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Ciudad...", dataSource: { data: cities } });
    $(`#call-type-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Tipo...", dataSource: { data: callTypes } });
    $(`#priority-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Prioridad...", dataSource: { data: priorities } });
    $(`#origin-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Origen...", dataSource: { data: origins } });
    $(`#problem-type-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Tipo de Problema...", dataSource: { data: problemTypes } });
    $(`#country-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un País...", filter: "contains", dataSource: { data: countries } });
    $(`#state-${item.code}`).kendoDropDownList({
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Departamento...", cascadeFrom: `country-${item.code}`, enable: false,
        dataSource: {
            serverFiltering: true, transport: {
                read: {
                    url: urlStates, data: (e) => {
                        return { Country: e.filter ? e.filter.filters[0].value : "BO" }
                    }
                }
            }
        }
    });
    $(`#location-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Locación...", filter: "contains", dataSource: { data: locations } });
    $(`#delivery-date-${item.code}`).kendoDateTimePicker({ format: "dd-MM-yyyy HH:mm", componentType: "modern", culture: "es-BO", value: item.deliveredDate });
    $(`#counted-pieces-${item.code}`).kendoNumericTextBox({
        format: "n0", change: function () {
            var value = this.value(), lastValue = $(`#prior-${this.element.attr("id")}`).getKendoNumericTextBox().value();
            $(`#diff-${this.element.attr("id")}`).getKendoNumericTextBox().value(value - lastValue);
        }
    });
    $(`#prior-counted-pieces-${item.code}`).kendoNumericTextBox({ format: "n0" });
    $(`#diff-counted-pieces-${item.code}`).kendoNumericTextBox({ format: "n0", restrictDecimals: true, decimals: 0, min: 0 });

    $.get(urlDetail, { Id: item.code }, function (data) {
        if (data.message === "") {
            var editable = +item.statusCode !== -1;
            data.activities.forEach(x => x.activityDate = JSON.toDate(x.activityDate));
            item.listServiceCallActivitys = data.activities;
            $("#activitiesList_" + item.code).kendoGrid({
                columns: [
                    { title: "Cod.", field: "code", width: 60, attributes: alignRight, headerAttributes: alignRight },
                    { title: "Actividad", field: "activityType", width: 130 },
                    { title: "Fecha", field: "activityDate", format: "{0:dd-MM-yyyy}", width: 80, attributes: alignCenter, headerAttributes: alignCenter },
                    { title: "Hora", field: "activityDate", format: "{0:HH:mm}", width: 65, attributes: alignCenter, headerAttributes: alignCenter },
                    { title: "Tratado por", field: "treatedBy", width: 180 },
                    { title: "Cerrado", field: "closed", width: 80, attributes: alignCenter, headerAttributes: alignCenter, template: '# if(closed === "Y") {# <i class="fas fa-check"></i> #} #' },
                    { title: "Contenido", field: "notes", attributes: { class: "text-truncate" } },
                    {
                        title: " ", attributes: alignCenter, headerAttributes: alignCenter, width: 45,
                        headerTemplate: () => editable ? '<i class="fas fa-plus action action-link action-new-activity" title="Nuevo"></i>' : '',
                        template: function (e) {
                            var result = "";
                            if (perActivities > 0) {
                                if (perActivities > 1) {
                                    result = `<i class="fas fa-pen action action-link action-edit-activity" title="Editar" data-editable="${editable ? "Y" : "N"}"></i>`;
                                }
                                if (perActivities > 3 && e.id !== 0 && editable) {
                                    result += '&nbsp;&nbsp;<i class="fas fa-trash action action-link action-delete-activity" title="Eliminar"></i>';
                                }
                            }
                            return result;
                        }
                    }
                ],
                sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                dataSource: { data: data.activities, filter: [{ field: "statusType", operator: "neq", value: "3" }] }
            });

            data.repairs.forEach(x => {
                x.createDate = JSON.toDate(x.createDate);
                x.updateDate = JSON.toDate(x.updateDate);
            });
            item.listServiceCallSolutions = data.repairs;
            $("#repairList_" + item.code).kendoGrid({
                columns: [
                    { title: "Cod.", field: "code", width: 60, attributes: alignRight, headerAttributes: alignRight/*, template: '<a class="repair action action-link">#=id#</a>'*/ },
                    { title: "Solución", field: "subject" },
                    { title: "F. Creación", field: "createDate", format: "{0:dd-MM-yyyy}", width: 100, attributes: alignCenter, headerAttributes: alignCenter },
                    { title: "Propietario", field: "owner", width: 180 },
                    { title: "F. Actualización", field: "updateDate", format: "{0:dd-MM-yyyy}", width: 130, attributes: alignCenter, headerAttributes: alignCenter },
                    { title: "Estado", field: "status", width: 100 },
                    { title: " ", field: "attachment", width: 30, attributes: alignCenter, headerAttributes: alignCenter, template: '# if(attachment) {# <a class="attach action action-link" title="Archivos adjuntos"><i class="fas fa-paperclip"></i></a> #} #' },
                    {
                        title: " ", attributes: alignCenter, headerAttributes: alignCenter, width: 45,
                        headerTemplate: () => editable ? '<i class="fas fa-plus action action-link action-new-repair" title="Nuevo"></i>' : '',
                        template: function (e) {
                            var result = "";
                            if (perActivities > 0) {
                                if (perActivities > 1) {
                                    result = `<i class="fas fa-pen action action-link action-edit-repair" title="Editar" data-editable="${editable ? "Y" : "N"}"></i>`;
                                }
                                if (perActivities > 3 && e.id !== 0 && editable) {
                                    result += '&nbsp;&nbsp;<i class="fas fa-trash action action-link action-delete-repair" title="Eliminar"></i>';
                                }
                            }
                            return result;
                        }
                    }
                ],
                sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                dataSource: data.repairs
            });

            $("#historyList_" + item.code).kendoGrid({
                scrollable: false,
                sortable: true,
                pageable: false,
                selectable: true,
                columns: [
                    { field: "order", title: "Número", width: 100, attributes: alignRight, headerAttributes: alignRight },
                    { field: "updateDate", title: "Fecha", format: "{0:dd-MM-yyyy HH:mm}", width: 150, attributes: alignCenter, headerAttributes: alignCenter },
                    { field: "status", title: "Estado", width: 200 },
                    { field: "subject", title: "Descripción" }
                ],
                dataSource: data.history,
                noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' }
            });

            data.costs.forEach(x => x.postingDate = JSON.toDate(x.postingDate));
            $("#costsList_" + item.code).kendoGrid({
                scrollable: true,
                sortable: true,
                pageable: false,
                selectable: true,
                columns: [
                    { field: "docTypeDesc", title: "Clase doc.", width: 100 },
                    { field: "docNumber", title: "No. doc.", width: 100, attributes: alignCenter, headerAttributes: alignCenter, template: e => (e.docType === 15 || e.docType === 17) && e.canceled === "N" ? `<a class="action action-link related-doc">${e.docNumber}</a>` : e.docNumber },
                    { field: "postingDate", title: "Fecha", format: "{0:dd-MM-yyyy}", width: 90, attributes: alignCenter, headerAttributes: alignCenter },
                    { field: "itemCode", title: "No. artículo", width: 180 },
                    { field: "itemName", title: "Desc. artículo", width: 300 },
                    { field: "transfered", title: "Transferido", attributes: alignCenter, headerAttributes: alignCenter, template: e => e.transfered === "Y" ? '<i class="fas fa-check"></i>' : '', width: 100 },
                    { field: "transToTec", title: "Transf. al Técnico", width: 140, attributes: alignRight, headerAttributes: alignRight },
                    { field: "delivered", title: "Entregado", width: 90, attributes: alignRight, headerAttributes: alignRight },
                    { field: "retFromTec", title: "Dev. del Tec.", width: 100, attributes: alignRight, headerAttributes: alignRight },
                    { field: "returned", title: "Devuelto", width: 100, attributes: alignRight, headerAttributes: alignRight },
                    { field: "bill", title: "Facturado", template: e => e.bill === "Y" ? '<i class="fas fa-check"></i>' : '', width: 100, attributes: alignCenter, headerAttributes: alignCenter },
                    { field: "qtyToBill", title: "Cat. a Facturar", width: 100, attributes: alignRight, headerAttributes: alignRight },
                    { field: "qtyToInv", title: "Cat. a Inv.", width: 90, attributes: alignRight, headerAttributes: alignRight },
                    { field: "canceled", title: "Anulado", width: 100, attributes: alignCenter, headerAttributes: alignCenter, template: e => e.canceled === "Y" ? '<i class="fas fa-check"></i>' : '' }
                ],
                dataSource: data.costs,
                noRecords: { template: '<div class="p-3 w-100 text-center">No se encontraron registros para el criterio de búsqueda.</div>' },
                dataBound: function (e) {
                    var dataItems = e.sender.dataSource.view();
                    for (var j = 0; j < dataItems.length; j++) {
                        var canceled = dataItems[j].get("canceled");
                        var row = e.sender.tbody.find("[data-uid='" + dataItems[j].uid + "']");
                        if (canceled === "Y") {
                            row.addClass("canceled");
                        }
                    }
                }
            });
            $(`#data-${item.code}`).val(JSON.stringify(item));
        } else {
            console.error(data.message);
            showError("Se ha producido un error al traer los datos del servidor.");
        }
    });

    wnd.center().open();
}

function exportExcel() {
    hideNotification();
    $("#Listado").data("kendoGrid").saveAsExcel();
}

function getOtherServiceCallsFromSerial(e) {
    hideNotification();
    var data = e.currentTarget.dataset, code = data.code, serial = $(`#serial-number-${code}`).val();
    if ($.trim(serial) !== "") {
        $.get(urlOtherRMAs, { Id: code, Serial: serial }, d => {
            if (d.message === "") {
                d.items.forEach(x => {
                    x.createDate = JSON.toDate(x.createDate);
                    x.closeDate = JSON.toDate(x.closeDate);
                });
                var wnd = $("<div>").kendoWindow({ visible: false, modal: false, iframe: false, title: "Listado de Casos de RMA para un serial", resizable: false, width: 1200, actions: ["Close"], close: onCloseWindow }).data("kendoWindow");
                wnd.content(`<div id="rma-cases-${data.id}"></div>`);
                $(`#rma-cases-${data.id}`).kendoGrid({
                    columns: [
                        { title: "Número", width: 90, field: "code", attributes: alignRight, headerAttributes: alignRight, template: '<a class="see-detail action action-link" title="Ver Detalle">#=code#</a>' },
                        { title: "Técnico", field: "technician", width: 230 },
                        { title: "F. Ingreso", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "createDate", format: dateFormat },
                        { title: "F. Cierre", attributes: alignCenter, headerAttributes: alignCenter, width: 100, field: "closeDate", format: dateFormat },
                        { title: "D. Abierto", width: 90, field: "openDays", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight },
                        { title: "Cliente", field: "clientName", width: 220, attributes: { class: "text-nowrap" } },
                        { title: "Piezas Contadas", field: "countedPieces", format: "{0:N0}", width: 140, attributes: alignRight, headerAttributes: alignRight },
                        { title: "Estado", width: 150, field: "status" }
                    ],
                    sortable: true, selectable: "Single, Row", noRecords: { template: "No se encontraron registros de otros RMAs para este producto." },
                    dataSource: d.items
                });
                wnd.center().open();
            } else {
                console.error(d);
                showError("Se ha producido un error al traer los datos del servidor");
            }
        });
    }
}

function openActivity(e, isNew, edit) {
    hideNotification();
    var item, rmaId = $(e.currentTarget).closest(".detail-rma").find(".id").text(), grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    if (isNew) {
        grd.clearSelection();
        item = { id: 0, code: getTempId(), notes: "", details: "", activityType: "", assignedBy: "", assignedByCode: 1, treatedBy: "", contact: "", telephone: "", cardCode: $(e.currentTarget).closest(".detail-rma").find(".card-code").val(), activityDate: new Date() };
    } else {
        var row = $(e.currentTarget).closest("tr");
        grd.select(row);
        item = grd.dataItem(row);
    }

    if (!edit) {
        item.notes = item.notes !== null ? item.notes.replaceAll("\r", "<br />") : "";
        item.details = item.details !== null ? item.details.replaceAll("\r", "<br />") : "";
    } else {
        item.notes = item.notes !== null ? item.notes.replaceAll("<br />", "\r",) : "";
        item.details = item.details !== null ? item.details.replaceAll("<br />", "\r") : "";
    }
    var fileName = "", attachment = "", detailClass = edit ? "" : "bg-data", activityType, activityDate, assignedBy, treatedBy, contact, telephone, subject, closed, details, notes,
        lbType = "Actividad", lbDate = "Fecha", lbAssignedBy = "Asignado por", lbTreatedBy = "Tratado por", lbContact = "Persona de contacto", lbTelephone = "No. de Teléfono", lbSubject = "Asunto", lbClosed = "Cerrado", lbDetails = "Detalle", lbNotes = "Notas", labelClass = edit ? "label-edit" : "", saveRow = "";
    if (item.attachment) {
        var separator = item.attachment.indexOf("/") >= 0 ? "/" : "\\";
        if (item.attachment.indexOf(";") >= 0) {
            var files = item.attachment.split(";"), temp = [], line = 1;
            files.forEach(x => {
                fileName = Enumerable.From(x.split(separator)).LastOrDefault();
                temp.push(getFileButton(edit, item.code, fileName, x, line, false));
                line += 1;
            });
            attachment = temp.join(edit ? "&nbsp;&nbsp;&nbsp;" : "<br />");
        } else {
            fileName = Enumerable.From(item.attachment.split(separator)).LastOrDefault();
            attachment = getFileButton(edit, item.code, fileName, item.attachment, 1, false);
        }
    }
    if (edit) {
        attachment = `<div id="files-content-${item.code}" class="files-content mb-2">${attachment}</div>
                      <button type="button" class="btn btn-secondary photo-gallery" title="Escoger un archivo" data-id="${item.code}">
                        <i class="fa fa-image"></i> Escoger un Archivo
                      </button>
                      <input id="gallery-${item.code}" type="file" class="d-none gallery" data-id="${item.code}"><input type="hidden" id="data-files-${item.code}" value="" />`;
        activityType = `<input id="activity-type-${item.code}" class="form-control w-100" value="${item.activityTypeCode}" required>`;
        activityDate = `<input id="activity-date-${item.code}" type="datetime" required disabled="disabled">`;
        assignedBy = `<input id="assigned-by-${item.code}" class="w-100" value="${item.assignedByCode}" required>`;
        treatedBy = `<input id="treated-by-${item.code}" class="w-100" value="${item.treatedByCode}" required>`;
        contact = `<input id="activity-contact-${item.code}" class="w-100" value="${item.contactCode}">`;
        telephone = `<input id="telephone-${item.code}" class="form-control" value="${$.trim(item.telephone)}">`;
        subject = `<input id="subject-${item.code}" class="w-100" value="${item.subjectCode}" required>`;
        closed = `<input id="closed-${item.code}" class="closed" type="checkbox">`;
        details = `<input id="details-${item.code}" class="form-control" type="text" value="${$.trim(item.details)}">`;
        notes = `<textarea id="notes-${item.code}" class="form-control" rows="15">${$.trim(item.notes)}</textarea>`;
        saveRow = `<div class="row"><div class="col text-right"><button type="button" class="btn btn-primary save-activity" data-id="${item.code}" data-rmaId="${rmaId}">Guardar Actividad</button></div></div>`;
        lbType = `<label for="activity-type-${item.code}">${lbType}</label>`;
        lbDate = `<label for="activity-date-${item.code}">${lbDate}</label>`;
        lbAssignedBy = `<label for="assigned-by-${item.code}">${lbAssignedBy}</label>`;
        lbTreatedBy = `<label for="treated-by-${item.code}">${lbTreatedBy}</label>`;
        lbContact = `<label for="activity-contact-${item.code}">${lbContact}</label>`;
        lbTelephone = `<label for="telephone-${item.code}">${lbTelephone}</label>`;
        lbSubject = `<label for="subject-${item.code}">${lbSubject}</label>`;
        lbClosed = `<label for="closed-${item.code}">${lbClosed}</label>`;
        lbDetails = `<label for="details-${item.code}">${lbDetails}</label>`;
        lbNotes = `<label for="notes-${item.code}">${lbNotes}</label>`;
    } else {
        activityType = item.activityType;
        activityDate = kendo.toString(item.activityDate, "dd-MM-yyyy HH:mm");
        assignedBy = item.assignedBy;
        treatedBy = item.treatedBy;
        contact = item.contact;
        telephone = item.telephone;
        subject = item.subject || "";
        closed = item.closed === "Y" ? "Si" : "No";
        details = item.details;
        notes = item.notes;
    }

    var content = `<form id="form-activity-${item.code}">
                      <input type="hidden" id="card-code-${item.code}" value="${item.cardCode}"><input type="hidden" id="attachment-code-${item.code}" value="${item.attachmentCode}">
                      <div class="content-data">
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">N&uacute;mero</div><div class="col-4 ${detailClass} font-weight-bold ${labelClass}">${item.code}</div>
                           <div class="col-2 font-weight-bold ${labelClass}">${lbType}</div><div class="col-4 ${detailClass}">${activityType}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">${lbDate}</div><div class="col-10 ${detailClass}">${activityDate}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">${lbAssignedBy}</div><div class="col-4 ${detailClass}">${assignedBy}</div>
                           <div class="col-2 font-weight-bold ${labelClass}">${lbTreatedBy}</div><div class="col-4 ${detailClass}">${treatedBy}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">${lbContact}</div><div class="col-4 ${detailClass}">${contact}</div>
                           <div class="col-2 font-weight-bold ${labelClass}">${lbTelephone}</div><div class="col-4 ${detailClass}">${telephone}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">${lbSubject}</div><div class="col-4 ${detailClass}">${subject}</div>
                           <div class="col-2 font-weight-bold ${labelClass}">${lbClosed}</div><div class="col-4 ${detailClass}">${closed}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">${lbDetails}</div><div class="col-10 ${detailClass}">${details}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">${lbNotes}</div><div class="col-10 ${detailClass}">${notes}</div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold ${labelClass}">Adjunto</div><div class="col-10 ${detailClass}">${attachment}</div>
                         </div>${saveRow}
                      </div>
                   </form>`;

    var w = $("#detail").data("kendoWindow");
    w.content(content).open();

    if (edit) {
        var data = [{ id: "C", name: "Llamada teléfonica" }, { id: "E", name: "Nota" }, { id: "M", name: "Reunión" }, { id: "N", name: "Otra" }, { id: "P", name: "Campaña" }, { id: "T", name: "Tarea" }];
        $(`#activity-date-${item.code}`).kendoDateTimePicker({ format: "dd-MM-yyyy HH:mm", componentType: "modern", culture: "es-BO", value: item.activityDate });
        $(`#closed-${item.code}`).kendoSwitch({ checked: item.closed === "Y", messages: { checked: "", unchecked: "" } });
        $(`#activity-type-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", dataSource: data, optionLabel: "Seleccione un Tipo ..." });
        $(`#subject-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", dataSource: { transport: { read: { url: urlGetSubjects } } }, optionLabel: "Seleccione un Titulo ..." });
        $(`#assigned-by-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Técnico...", filter: "contains", dataSource: { data: users }, enable: false });
        $(`#treated-by-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Técnico...", filter: "contains", dataSource: { data: users } });
        $(`#activity-contact-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Contacto...", filter: "contains", dataSource: { transport: { read: { url: urlGetContacts, data: { CardCode: item.cardCode } } } } });
    }
}

function saveActivity(e) {
    var form = $(e.currentTarget).closest("form"), rmaId, id, activityType, activityTypeCode, activityDate, assignedBy, assignedByCode, treatedBy, treatedByCode, contact, contactCode, telephone, subject, subjectCode, closed, details, notes, cardCode, attachment = "", filesData, files = [],
        ddlActivityType, ddlAssignedBy, ddlTreatedBy, ddlContact, ddlSubject, dtpDate, swClose, item, data;
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        id = e.currentTarget.dataset.id;
        rmaId = e.currentTarget.dataset.rmaid;
        data = JSON.parse($(`#data-${rmaId}`).val());

        data.listServiceCallActivitys.forEach(x => x.activityDate = JSON.toDate(x.activityDate));
        item = data.listServiceCallActivitys.find(x => x.code === id);

        ddlActivityType = $(`#activity-type-${id}`).data("kendoDropDownList");
        ddlAssignedBy = $(`#assigned-by-${id}`).data("kendoDropDownList");
        ddlTreatedBy = $(`#treated-by-${id}`).data("kendoDropDownList");
        ddlContact = $(`#activity-contact-${id}`).data("kendoDropDownList");
        ddlSubject = $(`#subject-${id}`).getKendoDropDownList();
        dtpDate = $(`#activity-date-${id}`).data("kendoDateTimePicker");
        swClose = $(`#closed-${id}`).data("kendoSwitch");

        activityTypeCode = ddlActivityType.value();
        activityType = ddlActivityType.text();
        assignedBy = ddlAssignedBy.text();
        assignedByCode = ddlAssignedBy.value();
        treatedByCode = ddlTreatedBy.value();
        treatedBy = ddlTreatedBy.text();
        contactCode = ddlContact.value();
        contact = contactCode === "" ? "" : ddlContact.text();
        activityDate = dtpDate.value();
        closed = swClose.check() ? "Y" : "N";
        telephone = $(`#telephone-${id}`).val();
        subject = ddlSubject.text();
        subjectCode = ddlSubject.value();
        details = $(`#details-${id}`).val();
        notes = $(`#notes-${id}`).val();
        cardCode = $(`#card-code-${id}`).val();
        //attachment = $(`#new-file-${id}`).val();
        filesData = $(`#data-files-${id}`).val();
        if (filesData !== "") {
            files = JSON.parse(filesData);
            var oldFiles = (item === null || $.trim(item.attachment) === "") ? [] : item.attachment.split(";");
            files.forEach((o, i) => {
                if (o.action === "D" && o.existing === "Y") {
                    var old = oldFiles.find(x => x.indexOf(o.name) >= 0);
                    if (old) {
                        oldFiles.remove(old);
                    }
                }
                if (o.action === "I" && o.existing === "N") {
                    oldFiles.push(urlFiles + o.name);
                }
            });
            attachment = oldFiles.join(";");
        }

        if (item) {
            $.extend(item, { activityType: activityType, activityTypeCode: activityTypeCode, activityDate: activityDate, attachment: attachment, assignedBy: assignedBy, assignedByCode: assignedByCode, treatedBy: treatedBy, treatedByCode: treatedByCode, contactCode: contactCode, contact: contact, telephone: telephone, subject: subject, subjectCode: subjectCode, closed: closed, details: details, notes: notes, cardCode: cardCode });
            item.statusType = item.id === 0 ? 1 : 2;
        } else {
            item = { code: id, activityType: activityType, activityTypeCode: activityTypeCode, activityDate: activityDate, attachment: attachment, assignedBy: assignedBy, assignedByCode: assignedByCode, treatedBy: treatedBy, treatedByCode: treatedByCode, contactCode: contactCode, contact: contact, telephone: telephone, subject: subject, subjectCode: subjectCode, closed: closed, details: details, notes: notes, cardCode: cardCode, statusType: 1 };
            data.listServiceCallActivitys.push(item);
        }
        var grid = $(`#activitiesList_${rmaId}`).getKendoGrid(), ds = new kendo.data.DataSource({ data: data.listServiceCallActivitys, filter: [{ field: "statusType", operator: "neq", value: 3 }] });
        grid.setDataSource(ds);

        $(`#data-${rmaId}`).val(JSON.stringify(data));
        $("#detail").data("kendoWindow").close();
    }
}

function onDeleteActivity(e) {
    hideNotification();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row),
        rmaId = $(e.currentTarget).closest(".detail-rma").find(".id").text();
    grd.select(row);

    var message = item.code.indexOf("TEMP") >= 0 ? `¿Está seguro que desea eliminar la actividad No. <b>TEMP-${item.id}</b>?` : `Sólo eliminará la modificación que no ha subido al SAP ¿Está seguro?`;
    showConfirm(message, function (e) {
        var data = JSON.parse($(`#data-${rmaId}`).val());
        var temp = data.listServiceCallActivitys.find(x => x.id === item.id);
        if (+temp.id > 0) {
            var old = $.extend({}, temp, { id: 0, statusType: 0 });
            data.listServiceCallActivitys.push(old);
            temp.statusType = 3;
        } else {
            if (+temp.code < 0) {
                var index = data.listServiceCallActivitys.findIndex(x => x.id === item.id);
                data.listServiceCallActivitys.splice(index, 1);
            } else {
                temp.id = 0, temp.statusType = 0;
            }
        }
        data.listServiceCallActivitys.forEach(x => x.activityDate = JSON.toDate(x.activityDate));
        var ds = ds = new kendo.data.DataSource({ data: data.listServiceCallActivitys, filter: [{ field: "statusType", operator: "neq", value: 3 }] });
        grd.setDataSource(ds);
        $(`#data-${rmaId}`).val(JSON.stringify(data));
        showNotification("", `Se eliminó la actividad No. <b>${item.code}</b>.`, "success");
    });
}

function openRepair(e, isNew) {
    hideNotification();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), item, rmaId = $(e.currentTarget).closest(".detail-rma").find(".id").text(), editable;
    if (isNew) {
        grd.clearSelection();
        item = { id: 0, code: getTempId(), itemCode: $(`#item-code-${rmaId}`).val(), status: "", statusCode: -3, updateDate: new Date(), description: "", attachment: "", updatedBy: "", updatedByCode: 1, subject: "", symptom: "", cause: "" };
        editable = "Y";
    } else {
        var dataset = e.currentTarget.dataset;
        var row = $(e.currentTarget).closest("tr");
        grd.select(row);
        item = grd.dataItem(row);
        editable = dataset.editable;
    }

    item.description = item.description.replaceAll("\r", "<br />");
    var fileName = "", attachment = "";
    if (item.attachment) {
        var separator = item.attachment.indexOf("/") >= 0 ? "/" : "\\";
        if (item.attachment.indexOf(";") >= 0) {
            var files = item.attachment.split(";"), temp = [], line = 1;
            files.forEach(x => {
                fileName = Enumerable.From(x.split(separator)).LastOrDefault();
                temp.push(getFileButton(edit, item.code, fileName, x, line, false));
                line += 1;
            });
            attachment = temp.join(edit ? "&nbsp;&nbsp;&nbsp;" : "<br />");
        } else {
            fileName = Enumerable.From(item.attachment.split(separator)).LastOrDefault();
            attachment = getFileButton(edit, item.code, fileName, item.attachment, 1, false);
        }
    }

    var content = '';
    if (editable === "Y") {
        attachment = `<div id="files-content-${item.code}" class="files-content mb-2">${attachment}</div>
                      <button type="button" class="btn btn-secondary photo-gallery" title="Escoger un archivo" data-id="${item.code}">
                        <i class="fa fa-image"></i> Escoger un Archivo
                      </button>
                      <input id="gallery-${item.code}" type="file" class="d-none gallery" data-id="${item.code}"><input type="hidden" id="data-files-${item.code}" value="" />`;
        content = `<form>
                      <div class="content-data">
                         <div class="row"><div class="col-2 font-weight-bold">N&uacute;mero</div><div class="col-4 font-weight-bold">${item.code}</div></div>
                         <div class="row">
                           <div class="col-2 font-weight-bold"><label for="sol-itemcode-${item.code}">Art&iacute;culo</label></div><div class="col-4"><input id="sol-itemcode-${item.code}" class="form-control" value="${item.itemCode}" disabled></div>
                           <div class="col-2 font-weight-bold"><label for="sol-status-${item.code}">Estado</label></div><div class="col-4"><input id="sol-status-${item.code}" class="w-100" value="${item.statusCode}" required></div>
                         </div>
                         <div class="row">
                           <div class="col-2 font-weight-bold"><label for="sol-update-date-${item.code}">Actualizado el</label></div><div class="col-4"><input id="sol-update-date-${item.code}" disabled></div>
                           <div class="col-2 font-weight-bold"><label for="sol-updated-by-${item.code}">Actualizado por</label></div><div class="col-4"><input id="sol-updated-by-${item.code}" class="w-100" value="${item.updatedByCode}"></div>
                         </div>
                         <div class="row"><div class="col-2 font-weight-bold"><label for="sol-subject-${item.code}">Soluci&oacute;n</label></div><div class="col-10"><input id="sol-subject-${item.code}" type="text" class="form-control" value="${$.trim(item.subject)}"></div></div>
                         <div class="row"><div class="col-2 font-weight-bold"><label for="sol-symptom-${item.code}">S&iacute;ntoma</label></div><div class="col-10"><input id="sol-symptom-${item.code}" type="text" class="form-control" value="${$.trim(item.symptom)}" required></div></div>
                         <div class="row"><div class="col-2 font-weight-bold"><label for="sol-cause-${item.code}">Causa</label></div><div class="col-10"><input id="sol-cause-${item.code}" type="text" class="form-control" value="${$.trim(item.cause)}"></div></div>
                         <div class="row"><div class="col-2 font-weight-bold"><label for="sol-description-${item.code}">Descripci&oacute;n</label></div><div class="col-10"><input id="sol-description-${item.code}" type="text" class="form-control" value="${$.trim(item.description)}"></div></div>
                         <div class="row"><div class="col-2 font-weight-bold">Adjunto</div><div class="col-10">${attachment}</div></div>
                         <div class="row">
                            <div class="col text-right mt-2"><button class="btn btn-primary save-repair" data-code="${item.code}" data-rmaId="${rmaId}">Guardar Reparación</button></div>
                         </div>
                      </div>
                   </form> `;
    } else {
        content = `<div class="content-data">
                      <div class="row"><div class="col-2 font-weight-bold">N&uacute;mero</div><div class="col-4 bg-data font-weight-bold">${item.code}</div></div>
                      <div class="row">
                        <div class="col-2 font-weight-bold">Art&iacute;culo</div><div class="col-4 bg-data">${item.itemCode}</div>
                        <div class="col-2 font-weight-bold">Estado</div><div class="col-4 bg-data">${item.status}</div>
                      </div>
                      <div class="row">
                        <div class="col-2 font-weight-bold">Actualizado el</div><div class="col-4 bg-data">${kendo.toString(item.updateDate, "dd-MM-yyyy")}</div>
                        <div class="col-2 font-weight-bold">Actualizado por</div><div class="col-4 bg-data">${item.updatedBy}</div>
                      </div>
                      <div class="row"><div class="col-2 font-weight-bold">Soluci&oacute;n</div><div class="col-10 bg-data">${$.trim(item.subject)}</div></div>
                      <div class="row"><div class="col-2 font-weight-bold">S&iacute;ntoma</div><div class="col-10 bg-data">${$.trim(item.symptom)}</div></div>
                      <div class="row"><div class="col-2 font-weight-bold">Causa</div><div class="col-10 bg-data">${$.trim(item.cause)}</div></div>
                      <div class="row"><div class="col-2 font-weight-bold">Comentarios</div><div class="col-10 bg-data">${$.trim(item.description)}</div></div>
                      <div class="row"><div class="col-2 font-weight-bold">Adjunto</div><div class="col-10 bg-data">${attachment}</div></div>
                   </div> `;
    }

    var w = $("#detail").data("kendoWindow");
    w.content(content);

    if (editable === "Y") {
        $(`#sol-update-date-${item.code}`).kendoDatePicker({ format: "dd-MM-yyyy", culture: "es-BO", value: item.updateDate });
        $(`#sol-updated-by-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Técnico...", filter: "contains", dataSource: { data: users }, enable: false });
        $(`#sol-status-${item.code}`).kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Status...", filter: "contains", dataSource: { data: solutionStatuses } });
    }

    w.open();
}

function saveRepair(e) {
    e.preventDefault();

    var form = $(e.currentTarget).closest("form"), validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var buttonData = e.currentTarget.dataset, code = buttonData.code, rmaId = buttonData.rmaid, data = JSON.parse($(`#data-${rmaId}`).val()), item, attachment = null, filesData, files = [];
        if (data.listServiceCallSolutions && data.listServiceCallSolutions.length > 0) {
            data.listServiceCallSolutions.forEach(x => {
                if (x.createDate) x.createDate = JSON.toDate(x.createDate);
                if (x.updateDate) x.updateDate = JSON.toDate(x.updateDate);
            });
        }
        item = data.listServiceCallSolutions.find(x => x.code === code);
        var newDate = new Date();
        if (!item) {
            item = { id: code, code: code, statusType: 1, itemCode: $(`#sol-itemcode-${code}`).val(), createDate: newDate, owner: "manager", ownerCode: 1, attachment: "" };
            data.listServiceCallSolutions.push(item);
        } else {
            if (item.statusType !== 1) {
                item.statusType = 2;
                if (item.id === 0) item.id = getTempId();
            }
        }

        filesData = $(`#data-files-${code}`).val();
        if (filesData !== "") {
            files = JSON.parse(filesData);
            var oldFiles = (item === null || $.trim(item.attachment) === "") ? [] : item.attachment.split(";");
            files.forEach((o, i) => {
                if (o.action === "D" && o.existing === "Y") {
                    var old = oldFiles.find(x => x.indexOf(o.name) >= 0);
                    if (old) {
                        oldFiles.remove(old);
                    }
                }
                if (o.action === "I" && o.existing === "N") {
                    oldFiles.push(urlFiles + o.name);
                }
            });
            attachment = oldFiles.join(";");
        }

        var objStatus = $(`#sol-status-${code}`).getKendoDropDownList(), objUpdatedBy = $(`#sol-updated-by-${code}`).getKendoDropDownList();

        item.updateDate = newDate;
        item.status = objStatus.text();
        item.statusCode = objStatus.value();
        item.updatedBy = objUpdatedBy.text();
        item.updatedByCode = objUpdatedBy.value();
        item.subject = $(`#sol-subject-${code}`).val();
        item.symptom = $(`#sol-symptom-${code}`).val();
        item.cause = $(`#sol-cause-${code}`).val();
        item.description = $(`#sol-description-${code}`).val();
        item.attachment = attachment;

        var grid = $(`#repairList_${rmaId}`).getKendoGrid(), ds = new kendo.data.DataSource({ data: data.listServiceCallSolutions, filter: [{ field: "statusType", operator: "neq", value: 3 }] });
        grid.setDataSource(ds);

        $(`#data-${rmaId}`).val(JSON.stringify(data));
        $("#detail").data("kendoWindow").close();
    }
}

function onDeleteRepair(e) {
    hideNotification();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row),
        rmaId = $(e.currentTarget).closest(".detail-rma").find(".id").text();
    grd.select(row);

    var message = item.code.indexOf("TEMP") >= 0 ? `¿Está seguro que desea eliminar la reparación No. <b>TEMP-${item.id}</b>?` : `Sólo eliminará la modificación que no ha subido al SAP ¿Está seguro?`;
    showConfirm(message, function (e) {
        var data = JSON.parse($(`#data-${rmaId}`).val());
        var temp = data.listServiceCallSolutions.find(x => x.id === item.id);
        if (+temp.id > 0) {
            var old = $.extend({}, temp, { id: 0, statusType: 0 });
            data.listServiceCallSolutions.push(old);
            temp.statusType = 3;
        } else {
            if (+temp.code < 0) {
                var index = data.listServiceCallSolutions.findIndex(x => x.id === item.id);
                data.listServiceCallSolutions.splice(index, 1);
            } else {
                temp.id = 0, temp.statusType = 0;
            }
        }
        data.listServiceCallSolutions.forEach(x => {
            x.createDate = JSON.toDate(x.createDate);
            if (x.updateDate) x.updateDate = JSON.toDate(x.updateDate);
        });
        var ds = ds = new kendo.data.DataSource({ data: data.listServiceCallSolutions, filter: [{ field: "statusType", operator: "neq", value: 3 }] });
        grd.setDataSource(ds);
        $(`#data-${rmaId}`).val(JSON.stringify(data));
        showNotification("", `Se eliminó la reparación No. <b>${item.id}</b>.`, "success");
    });
}

function openRelatedDoc(e) {
    hideNotification();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    var wnd = $("#Report").data("kendoWindow");
    wnd.title(item.docTypeDesc);
    loadReport(item.docNumber, "Santa Cruz", item.docType === 15 ? "Delivery" : "");
    wnd.open().center();
}

function getProductCardData(e, isValid, isButton) {
    if (isValid) {
        var code, serial;
        if (isButton) {
            var d = e.currentTarget.dataset;
            code = d.code;
            serial = $(`#serial-number-${code}`).val();
        } else {
            serial = $(e.currentTarget).val();
            code = e.currentTarget.dataset.code;
        }
        if ($.trim(serial) !== "") {
            $.get(urlGetProductCard, { SerialNumber: serial }, function (d) {
                if (d.message === "") {
                    var i;
                    if (d.exists) {
                        i = d.item;
                        i.purchaseDate = JSON.toDate(i.purchaseDate);
                    } else {
                        showError("Este producto no tiene tarjeta de producto creada.");
                    }
                    loadInputsProductCard(code, i);
                } else {
                    console.error(d);
                    showError("Se ha producido un error al traer los datos del servidor");
                }
            });
        }
    }
}

function loadInputsProductCard(code, item) {
    if (item) {
        $(`#save-button-${code}`).removeAttr("disabled");
    } else {
        $(`#save-button-${code}`).attr("disabled", "disabled");
    }
    $(`#item-code-${code}`).val(item?.itemCode ?? "");
    $(`#item-name-${code}`).val(item?.itemName ?? "");
    $(`#brand-${code}`).val(item?.brand ?? "");
    $(`#ref-nv-${code}`).val(item ? `NV-${item.refNV}` : "");
    $(`#purchase-date-${code}`).data("kendoDatePicker").value(item?.purchaseDate ?? "");
    $(`#warranty-${code}`).val(item ? (item.warranty === "Y" ? "SI" : "NO") : "");
    $(`#client-${code}`).data("kendoDropDownList").value(item?.clientCode ?? "");
    $(`#final-user-${code}`).val(item?.clientName ?? "");
    $(`#prior-counted-pieces-${code}`).getKendoNumericTextBox().value(item?.lastCountedPieces ?? 0);
    $(`#city-${code}`).getKendoDropDownList().value(item?.cityCode ?? "");
    $(`#reported-by-${code}`).val(item?.reportedBy ?? "");
    $(`#final-user-phone-${code}`).val(item?.phone ?? "");
    $(`#final-user-email-${code}`).val(item?.eMail ?? "");
    $(`#external-service-${code}`).val(item?.externalService ?? "");
    $(`#external-service-technician-${code}`).val(item?.externalServiceTechnician ?? "");
    $(`#external-service-address-${code}`).val(item?.address ?? "");
}

function getTempId() {
    tempId -= 1;
    return tempId;
}

function deleteServiceCall(e) {
    hideNotification();
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);

    var message = item.sapCode != null ? `Sólo eliminará la modificación que no ha subido al SAP ¿Está seguro?` : `¿Está seguro que desea eliminar el RMA No. <b>${item.code}</b>?`;
    showConfirm(message, function (e) {
        $.post(urlDeleteServiceCall, { Code: item.portalId }, function (d) {
            if (d.message === "") {
                var ds = grd.dataSource;
                if (item.code.indexOf("TEMP") >= 0) {
                    ds.remove(item);
                } else {
                    var temp = ds.get(item.code);
                    temp.portalId = 0;
                }
                grd.setDataSource(ds);
                showNotification("", `Se eliminó la llamada de servicio No. <b>${item.code}</b>.`, "success");
            } else {
                console.error(d.message);
                showNotification("", `Se produjo el siguiente error al intentar eliminar la llamada de servicio`, "error");
            }
        });
    });
}

function saveServiceCall(e) {
    e.preventDefault();
    var form = $(e.currentTarget).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var objCountry, objState, objCity, objLocation, objClient, objStatus, objTechnician, objPriority, objCallType, objOrigin, objProblemType;
        var code = e.currentTarget.dataset.code;
        var item = JSON.parse($(`#data-${code}`).val());

        objCountry = $(`#country-${code}`).data("kendoDropDownList");
        objState = $(`#state-${code}`).data("kendoDropDownList");
        objCity = $(`#city-${code}`).data("kendoDropDownList");
        objLocation = $(`#location-${code}`).data("kendoDropDownList");
        objClient = $(`#client-${code}`).data("kendoDropDownList");
        objStatus = $(`#status-${code}`).data("kendoDropDownList");
        objTechnician = $(`#technician-${code}`).data("kendoDropDownList");
        objPriority = $(`#priority-${code}`).getKendoDropDownList();
        objCallType = $(`#call-type-${code}`).getKendoDropDownList();
        objProblemType = $(`#problem-type-${code}`).getKendoDropDownList();
        objOrigin = $(`#origin-${code}`).getKendoDropDownList();

        item.problemTypeCode = objProblemType.value();
        item.originCode = objOrigin.value();
        item.priority = objPriority.value();
        item.callType = objCallType.value();
        item.city = objCity.text();
        item.cityCode = objCity.value();
        item.countryCode = objCountry.value();
        item.country = item.countryCode === "" ? "" : objCountry.text();
        item.stateCode = objState.value();
        item.state = item.stateCode === "" ? "" : objState.text();
        item.locationCode = objLocation.value();
        item.location = item.locationCode === "" ? "" : objLocation.text();
        item.creationDate = $(`#create-date-${code}`).data("kendoDateTimePicker").value();
        if (item.creationDate) {
            item.creationDate = item.creationDate.toISOString();
            item.startDate = item.creationDate;
        }
        item.clientCode = objClient.value();
        item.clientName = objClient.text();
        item.itemCode = $(`#item-code-${code}`).val();
        item.itemName = $(`#item-name-${code}`).val();
        item.serialNumber = $(`#serial-number-${code}`).val();
        item.brand = $(`#brand-${code}`).val();
        item.resolution = $(`#resolution-${code}`).val();
        //pendiente resolutionDate
        item.state = $(`#state-${code}`).val();
        item.street = $(`#street-${code}`).val();
        item.room = $(`#room-${code}`).val();
        item.city2 = $(`#city2-${code}`).val();
        item.statusCode = objStatus.value();
        item.status = item.statusCode === "" ? "" : objStatus.text();
        item.subject = $(`#subject-${code}`).val();
        item.technicianCode = objTechnician.value();
        item.technician = objTechnician.text();
        item.assignee = objTechnician.text();
        item.assigneeCode = objTechnician.value();
        //admissionDate es igual a creationDate
        item.finalUser = $(`#final-user-${code}`).val();
        item.finalUserPhone = $(`#final-user-phone-${code}`).val();
        item.finalUserEMail = $(`#final-user-email-${code}`).val();
        item.deliveryDate = $(`#delivery-date-${code}`).data("kendoDateTimePicker").value();
        if (item.deliveryDate) item.deliveryDate = item.deliveryDate.toISOString();
        item.deliveredBy = $(`#delivered-by-${code}`).val();
        item.externalService = $(`#external-service-${code}`).val();
        item.externalServiceAddress = $(`#external-service-address-${code}`).val();
        item.externalServiceNumber = $(`#external-service-number-${code}`).val();
        item.externalServiceTechnician = $(`#external-service-technician-${code}`).val();
        item.guideNumber = $(`#guide-number-${code}`).val();
        item.transport = $(`#transport-${code}`).val();
        item.countedPieces = $(`#counted-pieces-${code}`).data("kendoNumericTextBox").value();
        item.priorCountedPieces = $(`#prior-counted-pieces-${code}`).data("kendoNumericTextBox").value();
        item.diffCountedPieces = $(`#diff-counted-pieces-${code}`).data("kendoNumericTextBox").value();
        item.purchaseDate = $(`#purchase-date-${code}`).data("kendoDatePicker").value();
        if (item.purchaseDate) item.purchaseDate = item.purchaseDate.toISOString();
        item.reportedBy = $(`#reported-by-${code}`).val();
        item.receivedBy = $(`#received-by-${code}`).val();
        item.refNV = $(`#ref-nv-${code}`).val();
        item.comments = $(`#comments-${code}`).val();
        item.warranty = $(`#warranty-${code}`).val();

        var filesData = $(`#data-files-${code}`).val();
        if (filesData !== "") {
            var files = JSON.parse(filesData);
            if (!item.listServiceCallFiles) item.listServiceCallFiles = [];
            files.forEach((o, i) => {
                if (o.action === "D" && o.existing === "Y") {
                    var temp = item.listServiceCallFiles.find(x => x.fileName === o.name);
                    if (temp) temp.statusType = 3;
                }
                if (o.action === "I" && o.existing === "N") {
                    item.listServiceCallFiles.push({ fileName: o.name, statusType: 1 });
                }
            });
        }

        $.post(urlSaveServiceCall, { Item: item, Filters: filesData.data }, function (d) {
            if (d.message === "") {
                filterData();
                $(e.currentTarget).closest(".my-window").data("kendoWindow").close();
            } else {
                console.error(d.message);
                showError("Se ha producido un error al guardar la Llamada de Servicio.");
            }
        });
    }
}

//#endregion

//#region Métodos de Archivos

function openCamera(e) {
    var id = e.currentTarget.dataset.id;
    Webcam.set({ width: 320, height: 240, dest_width: 320, dest_height: 240, crop_width: 320, crop_height: 240 });
    Webcam.attach('#my_camera');
    $("#take-photo-ok").attr("data-id", id);
    $("#detailCamera").data("kendoWindow").open();
}

function openGallery(e) {
    var id = e.currentTarget.dataset.id;
    $(`#gallery-${id}`).val(null);
    $(`#gallery-${id}`).click();
}

function onGalleryFileSelected(e) {
    e.preventDefault();
    var id = e.currentTarget.dataset.id;
    if (e.currentTarget.files && e.currentTarget.files[0]) {
        var name = e.currentTarget.files[0].name;
        var FR = new FileReader();
        FR.addEventListener("load", function (e) {
            saveImage(e.target.result, name, id);
        });
        FR.readAsDataURL(e.currentTarget.files[0]);
    }
}

function onTakePhoto(e) {
    e.preventDefault();
    var id = e.currentTarget.dataset.id;
    Webcam.snap(function (data_uri) {
        var fileName = `Photo-${userCode}-${kendo.toString(new Date(), "yyyyMMdd-HHmmss")}.jpg`;
        saveImage(data_uri, fileName, id);
        Webcam.reset();
        $("#detailCamera").data("kendoWindow").close();
    });
}

function onCancelTekePhoto(e) {
    e.preventDefault();
    Webcam.reset();
    $("#detailCamera").data("kendoWindow").close();
}

function deleteFile(e) {
    var d = e.currentTarget.dataset;
    var filesData = $(`#data-files-${d.id}`).val();
    var files = filesData === "" ? [] : JSON.parse(filesData);
    var button = $(e.currentTarget).closest(".btn-file");
    showConfirm(`¿ Está seguro que desea eliminar el archivo ${d.name} ?`, e => {
        var item = Enumerable.From(files).Where(x => x.name === d.name).FirstOrDefault();
        if (item) {
            item.action = "D";
        } else {
            files.push({ name: d.name, action: "D", existing: d.existing, line: d.line });
        }
        $(`#data-files-${d.id}`).val(JSON.stringify(files));
        button.remove();
    });
}

function saveImage(image, name, id) {
    $.post(urlSaveFile, { FileBase64: image, FileName: name }, function (data) {
        if (data.message === "") {
            var filesData = $(`#data-files-${id}`).val();
            var files = filesData === "" ? [] : JSON.parse(filesData);
            files.push({ name: name, action: "I", existing: "N", line: 0 });
            $(`#data-files-${id}`).val(JSON.stringify(files));
            $(`#files-content-${id}`).append(`&nbsp;&nbsp;&nbsp;${getFileButton(true, id, name, "", 0, true)}`);
        } else {
            console.log(data.message);
            showNotification("", "Se produjo un error al guardar el archivo.", "error");
        }
    });
}

function openFile(e) {
    hideNotification();
    var path = $(e.currentTarget).attr("data-url");
    if (!path) {
        var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
        var row = $(e.currentTarget).closest("tr");
        grd.select(row);
        var item = grd.dataItem(row);
        path = item.attachment;
    }
    path = path.replaceAll(";", "");
    window.location.href = urlDownloadFile + "?" + $.param({ FilePath: path });
}

//#endregion