//#region Global Variables
var spreadsheet, priceGroups = [], products = [], groupLines = [], idLine = 0, clientsList = [], wndDetail, selectedClients = [], editGroup, editClient, clientsGrid;
const alignCenter = { style: "text-align: center;" };
//#endregion

//#region Events

$(() => setupControls());

$("#action-save-prices").click(onSave);

$("#selected-clients").on("click", ".new-client", onNewClient);

$("#selected-clients").on("click", ".action-delete", onDeleteClient);

$("#save-client").click(onClientSave);

//#endregion

//#region Private Methods

function setupControls() {
    $("#id-line").kendoDropDownList({
        dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Línea ...", dataSource: { transport: { read: { url: urlLines } } },
        change: function (e) {
            spreadsheet.activeSheet().range(kendo.spreadsheet.SHEETREF).clear();
            idLine = +this.value();
            var rows = [], row = ['', '', ''];
            priceGroups.forEach(x => row.push(x.name));
            rows.push(row);
            row = ['Cod. Producto', 'Nombre Producto', 'Costo'];
            $.get(urlProducts, { LineId: idLine }, function (d) {
                groupLines = d.groups;
                products = d.products;

                priceGroups.forEach(x => {
                    var gLine = d.groups.find(i => i.idGroup === x.id);
                    row.push(gLine?.percentage ?? x.percentage ?? 0);
                });
                rows.push(row);

                d.products.forEach(x => rows.push([x.itemCode, x.name, x.cost]));

                spreadsheet.activeSheet().range(0, 0, d.products.length + 2, priceGroups.length + 4).values(rows);
                spreadsheet.activeSheet().range(2, 3, d.products.length, priceGroups.length).formula("$C3*(100+D$2)/100/0.84");
                spreadsheet.activeSheet().range(2, 2, d.products.length, priceGroups.length + 2).format("#,##0.00");

                spreadsheet.activeSheet().range(`R0C0:R0C${priceGroups.length + 3}`).background('#0059B2').color('#FFFFFF').textAlign("right");
                spreadsheet.activeSheet().range(1, 0, 1, 3).background('#0059B2').color('#FFFFFF');
                spreadsheet.activeSheet().range(1, 3, 1, priceGroups.length).background('#BFDFFF');
                spreadsheet.activeSheet().range(0, 0, 2, priceGroups.length + 3).fontSize(12).bold(true);
                spreadsheet.activeSheet().range(2, 2, products.length - 1, 2).background('#BFDFFF');
                spreadsheet.activeSheet().range(2, 4, products.length - 1, priceGroups.length - 1).enable(false);
            });

            $.get(urlGroupsByLine, { LineId: idLine }, function (d) {
                selectedClients = d.clients;
                var ds = getClientsDS(d.clients);
                clientsGrid.setDataSource(ds);

                var ds2 = new kendo.data.DataSource({ data: d.groups });
                editGroup.setDataSource(ds2);
            });
        }
    });

    spreadsheet = $("#spreadsheet").kendoSpreadsheet({
        toolbar: false,
        sheetsbar: false,
        defaultCellStyle: { fontSize: 11 },
        rows: 800,
        sheets: [{ columns: [{ width: 190 }, { width: 270 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }] }]
    }).data("kendoSpreadsheet");

    editClient = $("#edit-client").kendoDropDownList({
        dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains", virtual: {
            itemHeight: 26, valueMapper: function (options) {
                var items = this.dataSource.data();
                var index = items.indexOf(items.find(i => i.code === options.value));
                options.success(index);
            }
        },
        dataSource: { transport: { read: { url: urlClients } } }
    }).data("kendoDropDownList");
    editGroup = $("#edit-group").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Lista...", filter: "contains", dataSource: [] }).data("kendoDropDownList");

    wndDetail = $("#detail-client").kendoWindow({
        visible: false, modal: true, iframe: false, scrollable: true, title: "Agregar Cliente a Lista de Precios", resizable: false, width: 700, actions: ["Close"],
        activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    }).data("kendoWindow");

    clientsGrid = $("#selected-clients").kendoGrid({
        columns: [
            { field: "groupName", title: "Lista", hidden: true, aggregates: ["count"], groupHeaderTemplate: "Lista: #= value #    ( Total: #= count# )" },
            { field: "cardName", title: "Cliente" },
            {
                field: "itemCode", title: " ", attributes: alignCenter, width: 60, sortable: false, headerAttributes: alignCenter,
                headerTemplate: '<a class="action action-link new-client" title="Adicionar Cliente"><i class="fas fa-plus"></i></a>',
                template: '<a class="action action-link action-delete" title="Eliminar Cliente"><i class="fas fa-trash-alt"></i></a>'
            }
        ],
        sortable: true, selectable: "Single, Row", scrollable: { height: "600px" },
        messages: { noRecords: '<div class="w-100 text-center p-3">No hay registros para el criterio de b&uacute;squeda.</div>' },
        dataSource: getClientsDS([])
    }).data("kendoGrid");

    $.get(urlGroups, {}, function (d) {
        if (d.message === '') {
            priceGroups = d.items;

            //sheet tab
            var row = ['', '', '', 'Precio Portal'], row2 = ['Cod. Producto', 'Nombre Producto', 'Costo', ''];
            priceGroups.forEach(x => row.push(x.name));
            spreadsheet.activeSheet().range(0, 0, 2, priceGroups.length + 4).values([row, row2]);
        }
    });

    $.get(urlClients, {}, function (items) {
        clientsList = items;
    });
}

function getClientsDS(items) {
    return new kendo.data.DataSource({
        group: [{ field: "groupName", dir: "asc", aggregates: [{ field: "groupName", aggregate: "count" }] }],
        aggregate: [{ field: "groupName", aggregate: "count" }],
        schema: { model: { id: "id" } },
        data: items
    });
}

function onSave(e) {
    e.preventDefault();

    var updatedGroups = [], updatedProducts = [], updatedPrices = [];

    spreadsheet.activeSheet().range(1, 3, 1, priceGroups.length).forEachCell(function (r, c, p) {
        var pGroup = priceGroups.at(c - 3), idGroup = pGroup.id, groupL = groupLines.find(x => x.idGroup === idGroup);
        //if (p.value != pGroup.percentage) {
            if (!groupL) groupL = { id: 0, idGroup: idGroup, idLine: idLine, percentage: 0 };
            groupL.percentage = p.value;
            updatedGroups.push(groupL);
        //}
    });

    spreadsheet.activeSheet().range(2, 2, products.length - 1, 2).forEachCell(function (r, c, p) {
        var prod = products.at(r - 2);
        if (c === 2) {
            if (prod.cost != p.value) {
                updatedProducts.push({ id: prod.id, cost: p.value });
            }
        }
        if (c === 3) {
            if (prod.price != p.value) {
                updatedPrices.push({ id: prod.idPrice ?? 0, regular: p.value, idSudsidiary: 1, idProduct: prod.id });
            }
        }
    });

    $.post(urlSave, { Groups: updatedGroups, Products: updatedProducts, Prices: updatedPrices }, function (d) {
        if (d.message == '') {
            showMessage('Se guardaron los datos correctamente.');
        } else {
            console.error(d.message);
            showError('Se ha producido un error al guardar los datos.');
        }
    });
}

function onNewClient(e) {
    e.preventDefault();
    if ($("#id-line").data("kendoDropDownList").value() != "") {
        wndDetail.open();
    }
}

function onClientSave(e) {
    e.preventDefault();
    var config = {
        messages: { required: "", customRule1: "Este cliente ya está asignado a alguna lista" },
        rules: {
            customRule1: function (input) {
                var code = editClient.value(), id = editGroup.value(), item = selectedClients.find(x => x.cardCode == code);
                return item == undefined;
            }
        }
    };

    var form = $(this).closest("form"), validator = form.kendoValidator(config).data("kendoValidator");
    if (validator.validate()) {
        var code = editClient.value(), id = editGroup.value();
        $.post(urlSaveClient, { CardCode: code, GroupLineId: id }, function (d) {
            if (d.message == "") {
                var item = { cardCode: code, cardName: editClient.text(), idGroupLine: id, groupName: editGroup.text() };
                selectedClients.push(item);
                clientsGrid.dataSource.add(item);
                wndDetail.close();
                showMessage("Se ha asignado el Cliente a la Lista correctamente");
            } else {
                console.error(d.message);
                showError("Se ha producido un error al asignar el Cliente a la Lista");
            }
        });
    }
}

function onDeleteClient(e) {
    e.preventDefault();
    var item = clientsGrid.dataItem($(e.currentTarget).closest("tr"));
    showConfirm(`¿Está seguro que desea eliminar el Cliente <b>${item.cardName}</b>?`, function () {
        $.post(urlDeleteClient, { CardCode: item.cardCode, GroupLineId: item.idGroupLine }, function (d) {
            if (d.message == "") {
                var index = selectedClients.findIndex(x => x.cardCode == item.cardCode);
                selectedClients.splice(index, 1);
                clientsGrid.dataSource.remove(item);
                showMessage("Se ha eliminado la asignación del Cliente correctamente.");
            } else {
                console.error(d.message);
                showError("Se ha producido un error al eliominar la asignación del Cliente a la Lista");
            }
        });
    });
}

//#endregion