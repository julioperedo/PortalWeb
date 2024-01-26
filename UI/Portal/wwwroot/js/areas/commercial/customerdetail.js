//#region Variables Globales
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
var showMargin = false;
//#endregion

//#region Eventos

$(() => setupControls());

$("#action-show-all").click(showHideData);

$("#contact-list").on("click", ".client-contact", showContact);

$("#attach-list").on("click", ".download-file", downloadFile);

//#endregion

//#region Métodos Locales

function setupControls() {
    //lista de usuaios en el portal
    $("#portal-users").kendoGrid({
        columns: [
            { field: "name", title: "Nombre" },
            { field: "eMail", title: "Correo Electrónico" },
            { field: "profile", title: "Perfil" },
            { field: "enabled", title: "Habilitado", template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', width: 90, attributes: alignCenter, headerAttributes: alignCenter }
        ],
        mobile: true, sortable: true, selectable: true,
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }
    });

    //lista de contactos
    $("#contact-list").kendoListView({
        dataSource: [],
        selectable: "single",
        change: function (e) {
            var selectedItems = e.sender.select();
            console.log(selectedItems);
        },
        template: kendo.template($("#contact-template").html())
    });

    //lista de direcciones
    $("#address-list").kendoTreeView({
        dataTextField: ["name", "name"],
        change: function (e) {
            var tv = e.sender;
            var item = tv.dataItem(this.select());
            if (!item.hasChildren) {
                var cardCode = $("#Client").data("kendoDropDownList").value();
                $.get(urlAddressData, { CardCode: cardCode, Name: item.name, Type: item.id }, function (data) {
                    if (data.message !== "") {
                        showError("Se ha producido un error al traer los datos del servidor, por favor intente nuevamente.");
                    }
                    loadAddressData(data.item);
                });
            }
        }
    });

    //lista de anexos
    $("#attach-list").kendoGrid({
        dataSource: [],
        columns: [
            { field: "line", title: "No.", width: 70 },
            { field: "path", title: "Vía de Acceso" },
            { field: "fileName", title: "Nombre del archivo" },
            { field: "date", title: "Fecha", format: dateFormat, width: 90, attributes: alignCenter, headerAttributes: alignCenter },
            { field: "enabled", title: " ", template: '<a class="download-file action"><i class="fas fa-download"></i></a>', width: 40, attributes: alignCenter, headerAttributes: alignCenter }
        ],
        mobile: true, sortable: true, selectable: true,
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }
    });

    //lista de correos 
    $("#mail-list").kendoGrid({
        dataSource: [],
        columns: [
            { field: "name", title: "Nombre" },
            { field: "eMail", title: "Correo Electrónico" },
            { field: "typeDesc", title: "Tipo" },
            { field: "logDate", title: "Fecha", format: "{0:dd-MM-yyyy HH:mm}", attributes: alignCenter, headerAttributes: alignCenter },
            { field: "blackList", title: "En Lista Negra", attributes: alignCenter, headerAttributes: alignCenter, template: '# if(blackList) {# <i class="fas fa-check"></i> #} #' }
        ],
        mobile: true, sortable: true, selectable: true,
        noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }
    });

    $("#listYears").kendoGrid({
        dataSource: [],
        mobile: true, sortable: true, selectable: "Single, Row", scrollable: { height: 200 }, noRecords: { template: "No se encontraron registros para el criterio de búsqueda." }, width: 750,
        columns: [
            { field: "year", title: "Año", attributes: alignRight, headerAttributes: alignRight },
            { field: "quantity", title: "# Compras", attributes: alignRight, headerAttributes: alignRight },
            { field: "total", title: "Total $Us", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
            { field: "average", title: "Promedio por mes", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
            { field: "percentageMargin", title: "Margen", format: "{0:N2} %", attributes: alignRight, headerAttributes: alignRight }
        ],
        dataBound: function (e) {
            var grid = e.sender;
            if (showMargin) {
                grid.showColumn("percentageMargin");
            } else {
                grid.hideColumn("percentageMargin");
            }
        },
        detailInit: function (e) {
            $.get(urlMonths, { CardCode: e.data.cardCode, Year: e.data.year }, function (data) {
                if (data.message === "") {
                    data.months.forEach(x => {
                        x.average = x.total / x.quantity;
                        x.percentageMargin = x.taxlessTotal > 0 ? (100 * (x.margin / x.taxlessTotal)) : 0;
                    });
                    $("<div>").appendTo(e.detailCell).kendoGrid({
                        scrollable: false,
                        sortable: true,
                        pageable: false,
                        selectable: true,
                        columns: [
                            { field: "month", title: "Mes", attributes: alignRight, headerAttributes: alignRight },
                            { field: "quantity", title: "# Compras", attributes: alignRight, headerAttributes: alignRight },
                            { field: "total", title: "Total $Us", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                            { field: "average", title: "Promedio por compra", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                            { field: "percentageMargin", title: "Margen", format: "{0:N2} %", attributes: alignRight, headerAttributes: alignRight }
                        ],
                        dataSource: data.months,
                        dataBound: function (e) {
                            var grid = e.sender;
                            if (showMargin) {
                                grid.showColumn("percentageMargin");
                            } else {
                                grid.hideColumn("percentageMargin");
                            }
                        }
                    });
                } else {
                    console.error(data.message);
                    showError('Se ha producido un error al traer los datos del servidor.');
                }
            });
        }
    });
}

function loadDetail(item) {
    item.client.createDate = JSON.toDate(item.client.createDate);
    item.staticts.first = JSON.toDate(item.staticts.first);
    item.staticts.last = JSON.toDate(item.staticts.last);

    var name = item.client.cardName;
    if (item.client.disabled === "Y") name += '&nbsp;&nbsp;&nbsp;<b style="color: #F00;">( Deshabilitado )</b>';
    $("#name").html(name);
    $("#legalName").text(item.client.cardFName);
    $("#nit").text(item.client.nit);
    $("#orders").text(kendo.toString(item.client.ordersBalance, "N2"));
    $("#accountBalance").text(kendo.toString(item.client.balance, "N2"));
    $("#phone1").text(item.client.phone1);
    $("#phone2").text(item.client.phone2);
    $("#fax").text(item.client.fax);
    $("#mobile").text(item.client.cellular);
    $("#email").text(item.client.eMail);
    $("#contactPerson").text(item.client.contactPerson);
    $("#accpac").text(item.client.accpac);
    $("#seller").text(item.client.sellerName);
    $("#creditLimit").text(kendo.toString(item.client.creditLimit, "N2"));
    $("#terms").text(item.client.terms);
    $("#createDate").text(kendo.toString(item.client.createDate, "dd-MM-yyyy"));

    $("#firstInvoice").text(kendo.toString(item.staticts.first, "dd-MM-yyyy"));
    $("#lastInvoice").text(kendo.toString(item.staticts.last, "dd-MM-yyyy"));
    $("#totalInvoice").text(kendo.toString(item.staticts.total, "N2"));
    $("#margin").text(kendo.toString(100 * (item.staticts.margin / item.staticts.taxlessTotal), "N2") + " %");
    $("#averageInvoice").text(kendo.toString(item.staticts.average, "N2"));
    $("#quantityInvoice").text(item.staticts.quantity);
    $("#dueDays").text(item.avgDueDays);

    var extraSA = Enumerable.From(item.extras).Where("$.subsidiary === 'Santa Cruz'").FirstOrDefault();
    if (extraSA) {
        $("#sa-acredited1").text(extraSA.acredited1);
        $("#sa-acredited2").text(extraSA.acredited2);
        $("#sa-acredited3").text(extraSA.acredited3);
        $("#sa-acredited4").text(extraSA.acredited4);
        $("#sa-acredited5").text(extraSA.acredited5);
        $("#sa-acredited6").text(extraSA.acredited6);
        $("#sa-acredited7").text(extraSA.acredited7);
        $("#sa-acredited8").text(extraSA.acredited8);

        $("#sa-ciacredited1").text(extraSA.ciAcredited1);
        $("#sa-ciacredited2").text(extraSA.ciAcredited2);
        $("#sa-ciacredited3").text(extraSA.ciAcredited3);
        $("#sa-ciacredited4").text(extraSA.ciAcredited4);
        $("#sa-ciacredited5").text(extraSA.ciAcredited5);
        $("#sa-ciacredited6").text(extraSA.ciAcredited6);
        $("#sa-ciacredited7").text(extraSA.ciAcredited7);
        $("#sa-ciacredited8").text(extraSA.ciAcredited8);
    }

    var extraIQ = Enumerable.From(item.extras).Where("$.subsidiary === 'Iquique'").FirstOrDefault();
    if (extraIQ) {
        $("#iq-acredited1").text(extraIQ.acredited1);
        $("#iq-acredited2").text(extraIQ.acredited2);
        $("#iq-acredited3").text(extraIQ.acredited3);
        $("#iq-acredited4").text(extraIQ.acredited4);
        $("#iq-acredited5").text(extraIQ.acredited5);
        $("#iq-acredited6").text(extraIQ.acredited6);
        $("#iq-acredited7").text(extraIQ.acredited7);
        $("#iq-acredited8").text(extraIQ.acredited8);

        $("#iq-ciacredited1").text(extraIQ.ciAcredited1);
        $("#iq-ciacredited2").text(extraIQ.ciAcredited2);
        $("#iq-ciacredited3").text(extraIQ.ciAcredited3);
        $("#iq-ciacredited4").text(extraIQ.ciAcredited4);
        $("#iq-ciacredited5").text(extraIQ.ciAcredited5);
        $("#iq-ciacredited6").text(extraIQ.ciAcredited6);
        $("#iq-ciacredited7").text(extraIQ.ciAcredited7);
        $("#iq-ciacredited8").text(extraIQ.ciAcredited8);
    }

    var extraLA = Enumerable.From(item.extras).Where("$.subsidiary === 'Miami'").FirstOrDefault();
    if (extraLA) {
        $("#la-acredited1").text(extraLA.acredited1);
        $("#la-acredited2").text(extraLA.acredited2);
        $("#la-acredited3").text(extraLA.acredited3);
        $("#la-acredited4").text(extraLA.acredited4);
        $("#la-acredited5").text(extraLA.acredited5);
        $("#la-acredited6").text(extraLA.acredited6);
        $("#la-acredited7").text(extraLA.acredited7);
        $("#la-acredited8").text(extraLA.acredited8);

        $("#la-ciacredited1").text(extraLA.ciAcredited1);
        $("#la-ciacredited2").text(extraLA.ciAcredited2);
        $("#la-ciacredited3").text(extraLA.ciAcredited3);
        $("#la-ciacredited4").text(extraLA.ciAcredited4);
        $("#la-ciacredited5").text(extraLA.ciAcredited5);
        $("#la-ciacredited6").text(extraLA.ciAcredited6);
        $("#la-ciacredited7").text(extraLA.ciAcredited7);
        $("#la-ciacredited8").text(extraLA.ciAcredited8);
    }

    var gridUsers = $("#portal-users").data("kendoGrid");
    gridUsers.setDataSource(new kendo.data.DataSource({ data: item.users }));

    var listviewContacts = $("#contact-list").data("kendoListView");
    listviewContacts.setDataSource(new kendo.data.DataSource({ data: item.contacts }));

    var treeviewAddresses = $("#address-list").data("kendoTreeView");
    treeviewAddresses.setDataSource(new kendo.data.HierarchicalDataSource({ data: item.addresses, schema: { model: { id: "type", children: "items" } } }));

    var gridFiles = $("#attach-list").data("kendoGrid");
    item.files.forEach(x => x.date = JSON.toDate(x.date));
    gridFiles.setDataSource(new kendo.data.DataSource({ data: item.files }));

    var gridMails = $("#mail-list").data("kendoGrid");
    item.mailContacts.forEach(x => {
        x.logDate = JSON.toDate(x.logDate);
        x.typeDesc = x.type === 1 ? "Recordatorio de Vencimientos" : (x.type === 2 ? "Copia de Notas de Venta" : (x.type === 3 ? "Ofertas" : "Transferencias de Propiedad"));
    });
    gridMails.setDataSource(new kendo.data.DataSource({ data: item.mailContacts }));

    item.years.forEach(x => x.percentageMargin = x.taxlessTotal > 0 ? (100 * (x.margin / x.taxlessTotal)) : 0);
    var listYears = $("#listYears").data("kendoGrid");
    listYears.setDataSource(new kendo.data.DataSource({ data: item.years }));
}

function loadContactData(item) {
    $(".contact-detail i").removeClass("fa-check").text("");
    if (item) {
        $("#cnt-name").text(item.name);
        $("#cnt-firstName").text(item.firstName);
        $("#cnt-middleName").text(item.middleName);
        $("#cnt-lastName").text(item.lastName);
        $("#cnt-title").text(item.title);
        $("#cnt-position").text(item.position);
        $("#cnt-address").text(item.address);
        $("#cnt-phone1").text(item.phone1);
        $("#cnt-phone2").text(item.phone2);
        $("#cnt-mobile").text(item.cellular);
        $("#cnt-fax").text(item.fax);
        $("#cnt-email").text(item.eMail);
        $("#cnt-identityCard").text(item.identityCard);
        if (item.enabled === "Y") {
            $("#cnt-active").addClass("fa-check");
        }
    }
}

function loadAddressData(item) {
    $(".address-detail span").text("");
    if (item) {
        $("#add-name").text(item.name);
        $("#add-address1").text(item.address1);
        $("#add-address2").text(item.address2);
        $("#add-address3").text(item.address3);
        $("#add-contact").text(item.contact);
        $("#add-country").text(item.country);
        $("#add-state").text(item.state);
        $("#add-city").text(item.city);
        $("#add-tax").text(item.taxCode);
    }
}

function onClientChanged(e) {
    var value = this.value();
    var item = { client: { cardName: "", cardFName: "", nit: "", ordersBalance: 0, balance: 0, phone1: "", phone2: "", fax: "", cellular: "", eMail: "", contactPerson: "", accpac: "", sellerName: "" }, extras: [], users: [] };
    if (value != "") {
        $.get(urlClient, { CardCode: value }, function (data) {
            if (data.message !== "") {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del cliente, por favor intente de nuevo.");
                data.item = item;
            }
            loadDetail(data.item);
        });
    } else {
        loadDetail(item);
    }
}

function clientMapper(options) {
    var items = $("#Client").data("kendoDropDownList").dataSource.data();
    var item = Enumerable.From(items).Where(x => x.code === options.value).FirstOrDefault();
    var index = Enumerable.From(items).IndexOf(item);
    options.success(index);
}

function showHideData(e) {
    var grids = $(".k-grid");
    var e = $(this).find("i");
    $.each(grids, function (i, x) {
        var grd = $(x).data("kendoGrid");
        if (e.hasClass("fa-eye-slash")) {
            grd.hideColumn("percentageMargin");
        } else {
            grd.showColumn("percentageMargin");
        }
    });
    if (e.hasClass("fa-eye-slash")) {
        e.removeClass("fa-eye-slash").addClass("fa-eye");
    } else {
        e.removeClass("fa-eye").addClass("fa-eye-slash");
    }
    $("#margin, #margin-label").toggleClass("d-none");
    showMargin = !showMargin;
}

function showContact(e) {
    var code = $(this).data("id");
    $.get(urlContactData, { Code: code }, function (data) {
        if (data.message !== "") {
            showError("Se ha producido un error al traer los datos del servidor, por favor intente nuevamente.");
        }
        loadContactData(data.data);
    });
}

function downloadFile(e) {
    var grd = $(this).closest(".k-grid").data("kendoGrid");
    var row = $(this).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);
    var cardCode = $("#Client").data("kendoDropDownList").value();
    window.location.href = urlAttachsData + "?" + $.param({ CardCode: cardCode, Line: item.line });
}

//#endregion