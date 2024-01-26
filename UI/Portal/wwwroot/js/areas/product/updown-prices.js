//#region Variables Globales
var ddlLines = $("#lines"), spreadsheet, productsCount = 0, roundOptions;
const marginGrid = 45, alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, alignRight2 = { "class": "table-header-cell !k-text-right" };
//#endregion

//#region Eventos

$(() => setupControls());

$("#action-save-prices").click(savePrices);

//#endregion

//#region Métodos Locales

function setupControls() {
    roundOptions = $("#round-options").kendoRadioGroup({
        layout: "horizontal", items: [{ value: "N", label: "Sin redondeo" }, { value: "U", label: "Redondeo hacia arriba" }, { value: "D", label: "Redondeo hacia abajo" }], value: "N",
        change: function (e) {
            spreadsheet.activeSheet().range(4, 5, productsCount, 1).formula(roundBy("E5*((100+$E$4)/100)", e.newValue));
            spreadsheet.activeSheet().range(4, 7, productsCount, 1).formula(roundBy("G5*((100+$G$4)/100)", e.newValue));
            spreadsheet.activeSheet().range(4, 10, productsCount, 1).formula(roundBy("J5*((100+$J$4)/100)", e.newValue));
            spreadsheet.activeSheet().range(4, 12, productsCount, 1).formula(roundBy("L5*((100+$L$4)/100)", e.newValue));
            spreadsheet.activeSheet().range(4, 15, productsCount, 1).formula(roundBy("O5*((100+$O$4)/100)", e.newValue));
            spreadsheet.activeSheet().range(4, 17, productsCount, 1).formula(roundBy("Q5*((100+$Q$4)/100)", e.newValue));
        }
    }).data("kendoRadioGroup");

    $.get(urlLines, {}, function (d) {
        d.forEach((x) => ddlLines.append(new Option(x.name, x.id)));
        ddlLines.multipleSelect({ minimumCountSelected: 5, onUncheckAll: noLineChecked, onCheckAll: lineChanged, onClick: lineChanged });
    });

    spreadsheet = $("#items-list").kendoSpreadsheet({
        toolbar: false,
        sheetsbar: false,
        defaultCellStyle: { fontSize: 11 },
        rows: 2500,
        sheets: [{ columns: [{ width: 150 }, { width: 150 }, { width: 150 }, { width: 150 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }, { width: 75 }] }]
    }).data("kendoSpreadsheet");

    var rows = [];
    rows.push(['', '', '', '', 'Santa Cruz', '', '', '', '', 'Iquique', '', '', '', '', 'Miami', '', '', '', '']);
    rows.push(['', '', '', '', 'Regular', '', 'Oferta', '', 'Volumen', 'Regular', '', 'Oferta', '', 'Volumen', 'Regular', '', 'Oferta', '', 'Volumen']);
    rows.push(['', '', '', '', 'Original', 'Modificado', 'Original', 'Modificado', '', 'Original', 'Modificado', 'Original', 'Modificado', '', 'Original', 'Modificado', 'Original', 'Modificado', '']);
    rows.push(['Linea', 'Categoria', 'Subcategoria', 'Item', 0, '', 0, '', 0, 0, '', 0, '', 0, 0, '', 0, '', 0]);

    spreadsheet.activeSheet().range(0, 0, 4, 19).values(rows);
    spreadsheet.activeSheet().range(0, 4, 1, 5).textAlign("center").merge();
    spreadsheet.activeSheet().range(0, 9, 1, 5).textAlign("center").merge();
    spreadsheet.activeSheet().range(0, 14, 1, 5).textAlign("center").merge();
    spreadsheet.activeSheet().range(1, 4, 1, 2).textAlign("center").merge();
    spreadsheet.activeSheet().range(1, 6, 1, 2).textAlign("center").merge();
    spreadsheet.activeSheet().range(1, 9, 1, 2).textAlign("center").merge();
    spreadsheet.activeSheet().range(1, 11, 1, 2).textAlign("center").merge();
    spreadsheet.activeSheet().range(1, 14, 1, 2).textAlign("center").merge();
    spreadsheet.activeSheet().range(1, 16, 1, 2).textAlign("center").merge();
    spreadsheet.activeSheet().range(2, 4, 1, 15).textAlign("right");
    spreadsheet.activeSheet().range(3, 4, 1, 2).merge();
    spreadsheet.activeSheet().range(3, 6, 1, 2).merge();
    spreadsheet.activeSheet().range(3, 9, 1, 2).merge();
    spreadsheet.activeSheet().range(3, 11, 1, 2).merge();
    spreadsheet.activeSheet().range(3, 14, 1, 2).merge();
    spreadsheet.activeSheet().range(3, 16, 1, 2).merge();
    spreadsheet.activeSheet().range(0, 0, 3, 19).background('#0059B2').color('#FFFFFF');
    spreadsheet.activeSheet().range(3, 0, 1, 4).background('#0059B2').color('#FFFFFF');
    spreadsheet.activeSheet().range(3, 4, 1, 15).background('#BFDFFF');
    spreadsheet.activeSheet().frozenRows(4);
}

const noLineChecked = () => spreadsheet.activeSheet().range(4, 0, productsCount, 16).value("");

function lineChanged(e) {
    var sheet = spreadsheet.activeSheet();
    sheet.range(4, 0, productsCount, 16).value("");
    $.get(urlProducts, { LineIds: ddlLines.multipleSelect('getSelects').join() }, function (d) {
        if (d.message == "") {
            if (d.items.length > 0) {
                var rows = [], cont = 0;
                //d.items.forEach((x) => rows.push([x.line, x.category, x.subcategory, x.itemCode, x.priceSA, '', x.offerSA, '', x.priceIQ, '', x.offerIQ, '', x.priceLA, '', x.offerLA, '']));
                var xxx = Enumerable.From(d.items)
                    .GroupBy("{ line: $.line + ' ' + $.category }", null, function (key, g) {
                        var result = { line: key.line }, cont = 0;
                        g.source.forEach((x) => {
                            if (cont++ < 11) rows.push([x.line, x.category, x.subcategory, x.itemCode, x.priceSA, '', x.offerSA, '', 'Sin Preview', x.priceIQ, '', x.offerIQ, '', 'Sin Preview', x.priceLA, '', x.offerLA, '', 'Sin Preview']);
                        });
                        return result;
                    }, "$.line + ' ' + $.category").ToArray();

                sheet.range(4, 0, rows.length, 19).values(rows);

                sheet.range(4, 5, rows.length, 1).formula(roundBy("E5*((100+$E$4)/100)", roundOptions.value()));
                sheet.range(4, 7, rows.length, 1).formula(roundBy("G5*((100+$G$4)/100)", roundOptions.value()));
                sheet.range(4, 10, rows.length, 1).formula(roundBy("J5*((100+$J$4)/100)", roundOptions.value()));
                sheet.range(4, 12, rows.length, 1).formula(roundBy("L5*((100+$L$4)/100)", roundOptions.value()));
                sheet.range(4, 15, rows.length, 1).formula(roundBy("O5*((100+$O$4)/100)", roundOptions.value()));
                sheet.range(4, 17, rows.length, 1).formula(roundBy("Q5*((100+$Q$4)/100)", roundOptions.value()));
                sheet.range(4, 4, rows.length, 15).format("#,##0.00");
                sheet.range(4, 0, rows.length, 20).enable(false);

                productsCount = rows.length;
            }
        } else {
            showError('Se ha producido iun error al traer los datos del servidor.');
        }
    });
}

function roundBy(value, type) {
    var newValue = value;
    if (type == "U") newValue = `ROUNDUP(${value}, 0)`;
    if (type == "D") newValue = `ROUNDDOWN(${value}, 0)`;
    return newValue;
}

function savePrices(e) {
    e.preventDefault();
    var sheet = spreadsheet.activeSheet(), scr = sheet.range("E4").value(), sco = sheet.range("G4").value(), scv = sheet.range("I4").value(), iqr = sheet.range("J4").value(),
        iqo = sheet.range("L4").value(), iqv = sheet.range("N4").value(), lar = sheet.range("O4").value(), lao = sheet.range("Q4").value(), lav = sheet.range("S4").value(),
        lineIds = ddlLines.multipleSelect('getSelects').join();

    $.post(urlSave, { LineIds: lineIds, RegularSA: scr, OfferSA: sco, VolumeSA: scv, RegularIQ: iqr, OfferIQ: iqo, VolumeIQ: iqv, RegularLA: lar, OfferLA: lao, VolumeLA: lav, RoundType: roundOptions.value() }, function (d) {
        if (d.message == "") {
            showMessage("Se han guardado los precios correctamente");
        } else {
            console.error(d.message);
            showError("Se ha producido un error al guardar los precios");
        }
    });
}

//#endregion