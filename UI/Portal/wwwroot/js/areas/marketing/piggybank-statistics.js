//#region Variables Globales
//var newId = 0;
//const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
var _loaded = false, _minPrize = 0, _options = {
    pivot: {
        rows: [],
        columns: [],
        values: [],
        xfields: [
            { field: "Serial", state: "opened", op: "count" },
            { field: "Usuario", state: "opened" },
            { field: "Tienda", state: "opened" },
            { field: "Ciudad", state: "opened" },
            { field: "Fecha", state: "opened" },
            { field: "Ano", title: "Año", state: "opened" },
            { field: "Trimestre", state: "opened" },
            { field: "Mes", state: "opened" },
            { field: "Dia", state: "opened" },
            { field: "CodCliente", title: "Cod. Cliente", state: "opened" },
            { field: "Cliente", state: "opened" },
            { field: "CodProducto", title: "Cod. Producto", state: "opened" },
            { field: "Producto", state: "opened" },
            { field: "Direccion", state: "opened" },
            { field: "Correo", state: "opened" },
            { field: "Telefono", state: "opened" },
            {
                field: "Puntos", title: "Monedas", op: "sum", formatter: (v) => kendo.toString(+v, "#,##0"),
                styler: function (v, r) {
                    if (+v >= _minPrize && !r.children) {
                        return { style: "background-color: #75c169" };
                    }
                    return {};
                }
            },
            { field: "Usado", state: "opened", op: "sum" },
            { field: "TotalPuntos", title: "Total Monedas", state: "opened", op: "sum" }
        ],
    },
    forzenColumnTitle: '<span style="font-weight:bold">Detalle</span>',
    valueFormatter: function (value) { return kendo.toString(+value, "#,##0"); },
    valuePrecision: 2,
    valueFieldWidth: 100,
    showFooter: true,
    FooterTitle: "TOTAL",
    showColumnsTotals: true,
    i18n: { fields: "Campos", filters: "Filtros", rows: "Filas", columns: "Columnas", values: "Valores", ok: "Aceptar", cancel: "Cancelar" },
    onDblClickRow: function (row) { $(this).pivotgrid("unselectAll"); },
    onBeforeLoad: function (row, param) { resizePivot(); }
};

//#endregion

//#region Eventos

$(() => {
    setupControls();
});


$(window).resize(resizePivot);

$("#action-config").click(configLayoutPivot);

$("#action-excel").click(exportExcel);

//#endregion

//#region Metodos Privados

function setupControls() {
    filterData();
}

function loadPivot(data, minPrize) {
    _minPrize = minPrize;
    var intHeight = $(window).height() - 130;
    data.sort((a, b) => a.date < b.date)
    var items = data.map(function (x) {
        return {
            Serial: x.serialNumber, Usuario: x.userName, Ciudad: x.city, Puntos: x.points, Producto: x.itemName, CodProducto: x.itemCode, CodCliente: x.cardCode, Cliente: x.cardName,
            Fecha: x.date, Tienda: x.storeName, Direccion: x.address, Correo: x.eMail, Telefono: x.phone, Fecha: kendo.toString(x.date, "yyyy-MM-dd"), Ano: x.date.getFullYear(),
            Mes: x.date.getMonth() + '-' + x.date.toLocaleString('default', { month: 'long' }), Dia: x.date.getDate(), Trimestre: "Q" + x.quarter, Usado: x.usedPoints, TotalPuntos: x.totalPoints
        };
    });

    var arrRows = ["Ciudad", "Usuario"], arrColumns = ["Trimestre"], arrValues = [{ field: "Puntos" }, { field: "Usado" }, { field: "TotalPuntos" }]; //, { field: "Serial", op: "count" }];

    if (_loaded) {
        _options = $("#pivot").pivotgrid("options");
    } else {
        _options.pivot.rows = arrRows;
        _options.pivot.columns = arrColumns;
        _options.pivot.values = arrValues;
    }

    $.map(_options.pivot.xfields, function (obj) { obj.state = "opened"; });
    _options.height = intHeight;
    _options.data = items;
    $(".datagrid-row-selected, .datagrid-row-checked").removeClass("datagrid-row-checked").removeClass("datagrid-row-selected");
    $("#action-excel").toggleClass("d-none", items.length <= 0);
    $("#pivot").pivotgrid(_options);
    _loaded = true;
    resizePivot();
}

function loadCharts(data) {
    var width = $(window).width() - $("#sidebar").width() - 100;
    loadChartCityDistribution(data);
    loadChartCityTimeDistribution(data, width);
}

function loadChartCityDistribution(data, width) {
    var lstGrouped = Enumerable.From(data).GroupBy("{ city: $.city }", null,
        function (key, g) {
            return { category: key.city, value: g.source.reduce((acc, cur) => acc + cur.points, 0) };
        },
        "$.city"
    ).ToArray();
    lstGrouped.sort((a, b) => b.value - a.value);

    $("#chartPerCity").kendoChart({
        title: { position: "bottom", text: "Distribución de monedas por Ciudad" },
        legend: { visible: false },
        chartArea: { width: width },
        seriesDefaults: { type: "pie", startAngle: 120 },
        series: [{
            data: lstGrouped,
            //aggregate: "sum",
            labels: {
                visible: true,
                background: "transparent",
                position: "outsideEnd",
                template: "#=category#: #=kendo.toString(value, 'N0')#"
            }
        }],
        tooltip: {
            visible: true,
            template: "#= category #: #=kendo.toString(value, 'N0')#"
        }
    });
}

function loadChartCityTimeDistribution(data, width) {
    //data.forEach((x) => x.date = kendo.toString(x.date, "dd-MM-yyyy"));
    var series = [], dates = Enumerable.From(data).Select("kendo.toString($.date, 'dd-MM-yyyy')").Distinct().ToArray();
    series = Enumerable.From(data).GroupBy("{ city: $.city }", null,
        function (key, g) {
            var serieData = [];
            dates.forEach((d) => {
                serieData.push(Enumerable.From(g).Where((i) => kendo.toString(i.date, "dd-MM-yyyy") == d).Sum("$.points"));
            });
            return { name: key.city, data: serieData };
        },
        "$.city"
    ).ToArray();

    $("#timeDistribution").kendoChart({
        title: {
            text: "Registro de monedas por día"
        },
        legend: {
            position: "bottom"
        },
        chartArea: { background: "", width: width },
        seriesDefaults: {
            type: "line",
            style: "smooth"
        },
        series: series,
        valueAxis: {
            labels: {
                //format: "{0}%"
            },
            line: {
                visible: false
            },
            axisCrossingValue: -10
        },
        categoryAxis: {
            categories: dates,
            majorGridLines: {
                visible: false
            },
            labels: {
                rotation: "auto"
            }
        },
        tooltip: {
            visible: true,
            //format: "{0}%",
            template: "#=series.name# <br /> Fecha: #=category# <br /> Cantidad: #= value #"
        }
    });
}

function filterData() {
    $.ajaxSetup({ cache: false });

    var newItem = function (x) {
        return {
            id: x.id, idUser: x.idUser, date: JSON.toDate(x.date), quarter: x.quarter, cardCode: x.cardCode, cardName: x.cardName, itemCode: x.itemCode, itemName: x.itemName,
            points: x.points, userName: x.userName, storeName: x.storeName, city: x.city, serialNumber: x.serialNumber, address: x.address, email: x.email, phone: x.phone,
            usedPoints: 0, totalPoints: x.points, idPrize: 0, prizeName: '', quantity: 0
        };
    };
    var newItemClaimed = function (x) {
        return {
            id: x.id, idUser: x.idUser, date: JSON.toDate(x.date), quarter: x.quarter, cardCode: '', cardName: '', itemCode: '', itemName: '',
            points: 0, userName: x.userName, storeName: x.storeName, city: x.city, serialNumber: '', address: x.address, email: x.email, phone: x.phone,
            usedPoints: x.points, totalPoints: -x.points, idPrize: x.idPrize, quantity: x.quantity, prizeName: x.prizeName
        };
    };

    $.get(urlFilter, {}, function (d) {
        var items = d.items.map((x) => newItem(x));
        loadCharts(items);
        d.claimed.forEach(function (x) {
            items.push(newItemClaimed(x));
        });
        loadPivot(items, d.minPrize);
    });
}

function configLayoutPivot(e) {
    e.preventDefault();
    $('#pivot').pivotgrid('layout');
}

function resizePivot(e) {
    var intHeight = $(window).height() - $("h3.title").closest(".row").height() - 150; //- $(".navbar").height() - $(".toolbar").height() - 50;
    $("#pivot").pivotgrid("resize", { height: intHeight });
}

function exportExcel(e) {
    window.location.href = urlExport;
}

//#endregion