//#region GLOBAL VARIABLES
var _maxDate, _minDate, products = [], _lastRecieverGridHeight = null, _docId = -1;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
//#endregion

//#region EVENTS

$(window).resize(setGridHeight("Listado", gridMargin));

$(() => setupControls());

$("#action-clean").click(onCleaningFilters);

$("#action-filter").click(filterData);

$("#action-excel").click(exportExcel);

$("#ContactList, #WorkList, #RMAList").on("click", ".see-detail", seeDetail);

//#endregion

//#region METHODS

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

    $("#search-type").kendoDropDownList();

    $("#ContactList").kendoGrid({
        columns: [
            { title: "Nombre", field: "name", media: "(min-width: 450px)" },
            { title: "Empresa", width: 160, field: "company", media: "(min-width: 450px)" },
            { title: "Ciudad", field: "city", media: "(min-width: 550px)" },
            { title: "Categoría", width: 160, field: "category", media: "(min-width: 900px)" },
            { title: "Cargo", width: 120, field: "position", media: "(min-width: 1050px)" },
            { title: "Fecha", field: "logDate", width: 90, format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter, media: "(min-width: 450px)" },
            { title: "Hora", field: "logDate", width: 65, format: "{0:HH:mm:ss}", attributes: alignCenter, headerAttributes: alignCenter, media: "(min-width: 1200px)" },
            { width: 35, template: '<a class="action action-link see-detail"><i class="fas fa-pen"></i></a>', attributes: alignCenter, media: "(min-width: 450px)" },
            { media: "(max-width: 450px)", template: `<div class="row"><div class="col see-detail">#=name#<br />#=company#<br />#=city#<br />Fecha: #=kendo.toString(logDate, "dd-MM-yyyy HH:mm:ss")#</div></div>` }
        ],
        groupable: { enabled: false },
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        dataSource: [],
        dataBound: (e) => e.sender.element.find("table").attr("style", "")
    });

    $("#WorkList").kendoGrid({
        columns: [
            { title: "Nombres", field: "firstName", media: "(min-width: 450px)" },
            { title: "Apellidos", field: "lastName", media: "(min-width: 450px)" },
            { title: "F. Nacimiento", width: 100, field: "birthday", media: "(min-width: 450px)" },
            { title: "Estado Civil", width: 160, field: "maritalStatus", media: "(min-width: 900px)" },
            { title: "Ciudad", field: "city", media: "(min-width: 550px)" },            
            { title: "Cargo", width: 120, field: "position", media: "(min-width: 1050px)" },
            { title: "Fecha", field: "logDate", width: 90, format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter, media: "(min-width: 450px)" },
            { title: "Hora", field: "logDate", width: 65, format: "{0:HH:mm:ss}", attributes: alignCenter, headerAttributes: alignCenter, media: "(min-width: 1200px)" },
            { width: 35, template: '<a class="action action-link see-detail"><i class="fas fa-pen"></i></a>', attributes: alignCenter, media: "(min-width: 450px)" },
            { media: "(max-width: 450px)", template: `<div class="row"><div class="col see-detail">#=firstName# #=lastName#<br />F. Nacimiento: #=birthday#<br />#=maritalStatus#<br />#=position#<br />Fecha: #=kendo.toString(logDate, "dd-MM-yyyy HH:mm:ss")#</div></div>` }
        ],
        groupable: { enabled: false },
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        dataSource: [],
        dataBound: (e) => e.sender.element.find("table").attr("style", "")
    });

    $("#RMAList").kendoGrid({
        columns: [
            { title: "Nombre", field: "name", media: "(min-width: 450px)" },
            { title: "Empresa", width: 160, field: "company", media: "(min-width: 450px)" },
            { title: "Ciudad", field: "city", media: "(min-width: 550px)" },
            { title: "Cod. Producto", width: 160, field: "itemCode", media: "(min-width: 900px)" },
            { title: "Marca", width: 120, field: "brand", media: "(min-width: 1050px)" },
            { title: "Fecha", field: "logDate", width: 90, format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter, media: "(min-width: 450px)" },
            { title: "Hora", field: "logDate", width: 65, format: "{0:HH:mm:ss}", attributes: alignCenter, headerAttributes: alignCenter, media: "(min-width: 1200px)" },
            { width: 35, template: '<a class="action action-link see-detail"><i class="fas fa-pen"></i></a>', attributes: alignCenter, media: "(min-width: 450px)" },
            { media: "(max-width: 450px)", template: `<div class="row"><div class="col see-detail">#=name#<br />#=company#<br />#=city#<br />( #=itemCode# ) #=productDesc#<br />Fecha: #=kendo.toString(logDate, "dd-MM-yyyy HH:mm:ss")#</div></div>` }
        ],
        groupable: { enabled: false },
        sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: '<div class="text-center w-100">No se encontraron registros para el criterio de b&uacute;squeda.</div>' },
        dataSource: [],
        dataBound: (e) => e.sender.element.find("table").attr("style", "")
    });

    $("#Detail").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "Detalle del correo", resizable: false, width: 760, actions: ["Close"],
        close: onCloseWindow, activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    });
}

function onCleaningFilters(e) {
    $("#search-type").data("kendoDropDownList").value("");
    $("#initial-date").data("kendoDatePicker").value("");
    $("#final-date").data("kendoDatePicker").value("");
}

function loadGrid(listName, items) {
    items.forEach(x => {
        x.logDate = JSON.toDate(x.logDate);
        if (listName === "RMAList") x.saleDate = JSON.toDate(x.saleDate);
    });
    $(".k-grid").addClass("d-none");
    $(`#${listName}`).removeClass("d-none");
    var grd = $(`#${listName}`).data("kendoGrid");
    var ds = new kendo.data.DataSource({ data: items });
    grd.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
    } else {
        $("#action-excel").addClass("d-none");
    }
    setTimeout(() => setGridHeight("Listado", gridMargin), 300);
}

function filterData(e) {
    var filterData = getFilters();
    $("#last-search").val(filterData.data.Type);
    var gridName = filterData.data.Type === "C" ? "ContactList" : (filterData.data.Type === "W" ? "WorkList" : "RMAList");
    if (filterData.message === "") {
        $.get(urlFilter, filterData.data, function (data) {
            if (data.message === "") {
                loadGrid(gridName, data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor.");
            }
        });
    } else {
        showInfo(`Se deben ingresar los siguientes campos: <br />${filterData.message}`);
    }
}

function getFilters() {
    var message = "", initialDate = $(`#initial-date`).data("kendoDatePicker").value(), finalDate = $(`#final-date`).data("kendoDatePicker").value(),
        type = $(`#search-type`).data("kendoDropDownList").value();

    if (initialDate || finalDate) {
        if (initialDate) initialDate = kendo.toString(initialDate, "yyyy-MM-dd");
        if (finalDate) finalDate = finalDate.toString(initialDate, "yyyy-MM-dd");
    } else {
        message += `Debe seleccionar al menos una fecha.`;
    }

    return {
        message: message,
        data: { Type: type, Since: initialDate, Until: finalDate }
    };
}

function seeDetail(e) {
    var wnd = $("#Detail").data("kendoWindow"), gridName, template, contactTemplate, workTemplate, rmaTemplate, grd, row, item, content, type;
    type = $("#last-search").val();
    contactTemplate = `<div>
    <dl class="row">
        <dt class="col-sm-2">Nombre</dt><dd class="col-sm-10">#=name#</dd>
        <dt class="col-sm-2">E-Mail</dt><dd class="col-sm-10">#=eMail#</dd>
        <dt class="col-sm-2">Empresa</dt><dd class="col-sm-10">#=company#</dd>
        <dt class="col-sm-2">Catego&iacute;a</dt><dd class="col-sm-10">#=category#</dd>
        <dt class="col-sm-2">Cargo</dt><dd class="col-sm-10">#=position#</dd>
        <dt class="col-sm-2">Ciudad</dt><dd class="col-sm-10">#=city#</dd>
        <dt class="col-sm-2">Direcci&oacute;n</dt><dd class="col-sm-10">#=address#</dd>
        <dt class="col-sm-2">Tel&eacute;fono</dt><dd class="col-sm-10">#=phone#</dd>
        <dt class="col-sm-2">NIT</dt><dd class="col-sm-10">#=nit#</dd>
        <dt class="col-sm-2">Tipo de Cliente</dt><dd class="col-sm-10">#=clientType#</dd>
        <dt class="col-sm-2">Mensaje</dt><dd class="col-sm-10">#=message#</dd>
        <dt class="col-sm-2">Fecha</dt><dd class="col-sm-10">#=kendo.toString(logDate, "dd-MM-yyyy HH:mm:ss")#</dd>
    </dl>
</div>`;
    workTemplate = `<div>
    <dl class="row">
        <dt class="col-sm-3">Nombre</dt><dd class="col-sm-9">#=firstName# #=lastName#</dd>
        <dt class="col-sm-3">Fecha Nac.</dt><dd class="col-sm-9">#=birthday#</dd>
        <dt class="col-sm-3">Estado Civil</dt><dd class="col-sm-9">#=maritalStatus#</dd>
        <dt class="col-sm-3">No. Doc.</dt><dd class="col-sm-9">#=identitynDoc#</dd>
        <dt class="col-sm-3">Ciudad</dt><dd class="col-sm-9">#=city#</dd>
        <dt class="col-sm-3">Direcci&oacute;n</dt><dd class="col-sm-9">#=address#</dd>
        <dt class="col-sm-3">Celular</dt><dd class="col-sm-9">#=cellphone#</dd>
        <dt class="col-sm-3">Tel&eacute;fono</dt><dd class="col-sm-9">#=phone#</dd>
        <dt class="col-sm-3">E-Mail</dt><dd class="col-sm-9">#=eMail#</dd>
        <dt class="col-sm-3">Puesto Vacante</dt><dd class="col-sm-9">#=meetRequirements#</dd>
        <dt class="col-sm-3">Experiencia (a&ntilde;os)</dt><dd class="col-sm-9">#=experience#</dd>
        <dt class="col-sm-3">Idiomas</dt><dd class="col-sm-9">#=languages#</dd>
        <dt class="col-sm-3">Pasatiempos</dt><dd class="col-sm-9">#=hobbies#</dd>
        <dt class="col-sm-3">Descr&iacute;bete</dt><dd class="col-sm-9">#=aboutYourself#</dd>
        <dt class="col-sm-3">Referencias</dt><dd class="col-sm-9">#=references#</dd>
        <dt class="col-sm-3">¿Por qu&eacute; nosotros?</dt><dd class="col-sm-9">#=whyUs#</dd>
        <dt class="col-sm-3">Logros</dt><dd class="col-sm-9">#=achievements#</dd>
        <dt class="col-sm-3">Motivo salida</dt><dd class="col-sm-9">#=leavingReason#</dd>
        <dt class="col-sm-3">Celular</dt><dd class="col-sm-9">#=cellphone#</dd>
        <dt class="col-sm-3">Experiencia</dt><dd class="col-sm-9">#=laboralExperience#</dd>
        <dt class="col-sm-3">Puesto</dt><dd class="col-sm-9">#=position#</dd>
        <dt class="col-sm-3">Pretensi&oacute;n salarial</dt><dd class="col-sm-9">#=salaryPretension#</dd>
        <dt class="col-sm-3">Disp. Viaje</dt><dd class="col-sm-9">#=travelAvailability#</dd>
        <dt class="col-sm-3">Adjunto</dt><dd class="col-sm-9"># if($.trim(linkCV) !== "") {# <a href="#=linkCV#" target="_blank">#=linkCV#</a> #} #</dd>
        <dt class="col-sm-3">Fecha</dt><dd class="col-sm-9">#=kendo.toString(logDate, "dd-MM-yyyy HH:mm:ss")#</dd>
    </dl>
</div>`;
    rmaTemplate = `<div>
    <dl class="row">
        <dt class="col-sm-2">Nombre</dt><dd class="col-sm-10">#=name#</dd>        
        <dt class="col-sm-2">Empresa</dt><dd class="col-sm-10">#=company#</dd>
        <dt class="col-sm-2">Ciudad</dt><dd class="col-sm-10">#=city#</dd>
        <dt class="col-sm-2">Direcci&oacute;n</dt><dd class="col-sm-10">#=address#</dd>
        <dt class="col-sm-2">Celular</dt><dd class="col-sm-10">#=cellphone#</dd>
        <dt class="col-sm-2">Tel&eacute;fono</dt><dd class="col-sm-10">#=phone#</dd>
        <dt class="col-sm-2">E-Mail</dt><dd class="col-sm-10">#=eMail#</dd>
        <dt class="col-sm-2">Producto</dt><dd class="col-sm-10">( #=itemCode# ) #=productDesc#</dd>
        <dt class="col-sm-2">Marca</dt><dd class="col-sm-10">#=brand#</dd>
        <dt class="col-sm-2">Serial</dt><dd class="col-sm-10">#=serialNumber#</dd>
        <dt class="col-sm-2">Nota de Venta</dt><dd class="col-sm-10">#=saleNote#</dd>        
        <dt class="col-sm-2">Fecha Nota</dt><dd class="col-sm-10">#=kendo.toString(saleDate, "dd-MM-yyyy")#</dd>
        <dt class="col-sm-2">Falla</dt><dd class="col-sm-10">#=failure#</dd>
        <dt class="col-sm-2">Tipo</dt><dd class="col-sm-10"># if(type === "DI") {# Distribuidor #} else {# Usuario Final #} #</dd>
        <dt class="col-sm-2">Fecha</dt><dd class="col-sm-10">#=kendo.toString(logDate, "dd-MM-yyyy HH:mm:ss")#</dd>
    </dl>
</div>`;
    switch (type) {
        case "C":
            gridName = "ContactList";
            template = kendo.template(contactTemplate);
            break;
        case "W":
            gridName = "WorkList";
            template = kendo.template(workTemplate);
            break;
        default:
            gridName = "RMAList";
            template = kendo.template(rmaTemplate);
    }
    grd = $(`#${gridName}`).data("kendoGrid");
    row = $(e.currentTarget).closest("tr");
    item = grd.dataItem(row);
    grd.select(row);
    content = template(item);
    wnd.content(content).open();
}

function exportExcel(e) {
    var type = $("#last-search").val(), gridName = type === "C" ? "ContactList" : (type === "W" ? "WorkList" : "RMAList");
    $(`#${gridName}`).data("kendoGrid").saveAsExcel();
}

//#endregion