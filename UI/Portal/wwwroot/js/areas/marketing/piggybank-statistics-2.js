//#region Global Variables
var _items, _months, _products, _clients, _tvMonths, _tvProducts, _selMonth = [], _selProduct = [];
//#endregion

//#region Events

$(() => {
    setupControls();
});

//#endregion

//#region Private Methods

function setupControls() {
    _tvMonths = $("#months-list").kendoTreeView({
        checkboxes: { checkChildren: true },
        dataSource: getHDS([], "months"),
        dataTextField: ["year", "monthDesc"],
        check: function (e) {
            var item = e.sender.dataItem(e.node), parent;
            if (item.checked) {
                if (item.hasChildren) {
                    var month = _months.find((x) => x.year == item.year);
                    month.months.forEach(function (m) {
                        if (!_selMonth.find((x) => x == `${item.year}-${m.month}`)) {
                            _selMonth.push(`${item.year}-${m.month}`);
                        }
                    });
                } else {
                    parent = e.sender.dataItem(e.sender.parent(e.node));
                    _selMonth.push(`${parent.year}-${item.month}`);
                }
            } else {
                if (item.hasChildren) {
                    var month = _months.find((x) => x.year == item.year);
                    month.months.forEach((x) => _.remove(_selMonth, (m) => m == `${item.year}-${x.month}`));
                } else {
                    parent = e.sender.dataItem(e.sender.parent(e.node));
                    _.remove(_selMonth, (m) => m == `${parent.year}-${item.month}`);
                }
            }
            loadCharts();
        }
    }).data("kendoTreeView");
    _tvProducts = $("#product-list").kendoTreeView({
        checkboxes: { checkChildren: true },
        dataSource: getHDS([], "months"),
        dataTextField: ["line", "code"],
        template: (e) => e.item.id ? `${e.item.code} - ${e.item.name}` : e.item.line,
        check: function (e) {
            var item = e.sender.dataItem(e.node);
            if (item.checked) {
                if (item.hasChildren) {
                    var tempProds = _products.filter((x) => x.line == item.line);
                    tempProds.forEach(function (p) {
                        if (!_selProduct.find((x) => x == p.itemCode)) {
                            _selProduct.push(p.itemCode);
                        }
                    });
                } else {
                    _selProduct.push(item.code);
                }
            } else {
                if (item.hasChildren) {
                    var tempProds = _products.filter((x) => x.line == item.line);
                    tempProds.forEach((x) => _.remove(_selProduct, (p) => p == x.itemCode));
                } else {
                    _.remove(_selProduct, (p) => p == item.code);
                }
            }
            loadCharts();
        }
    }).data("kendoTreeView");
    filterData();
}

function filterData() {
    $.ajaxSetup({ cache: false });

    var newItem = function (x) {
        var d = JSON.toDate(x.date), prod = _products.find((p) => $.trim(p.itemCode.toLowerCase()) == $.trim(x.itemCode.toLowerCase()));
        if (!prod) console.log("Item malo", x.itemCode);
        return {
            id: x.id, idUser: x.idUser, date: d, quarter: Math.floor((d.getMonth() + 3) / 3), cardCode: x.cardCode, itemCode: x.itemCode,
            points: x.points, userName: x.userName, storeName: x.storeName, city: x.city, month: `${d.getFullYear()}-${d.getMonth() + 1}`,
            line: prod?.line ?? ''
        };
    };

    $.get(urlFilter, {}, function (d) {
        var data = Enumerable.From(d.products).GroupBy("$.line").Select("{ line: $.Key(), products: $.Select('{ id: $.id, code: $.itemCode, name: $.name }').ToArray() }").ToArray();
        _tvProducts.setDataSource(getHDS(data, "products"));
        _products = d.products;

        _clients = d.clients;

        _items = d.items.map((x) => newItem(x));
        //console.log(_items);
        var data = Enumerable.From(_items).Select("{ year: $.date.getFullYear(), month: $.date.getMonth() + 1, monthDesc: $.date.toLocaleString('es-BO', { month: 'long' }).toTitleCase() }")
            .Distinct("$.year, $.month").OrderBy("$.year, $.month").ToArray();

        var g = Enumerable.From(data).GroupBy("$.year").Select("{ year: $.Key(), months: $.Select('{ month: $.month, monthDesc: $.monthDesc }').ToArray() }").ToArray();
        _months = g;
        _tvMonths.setDataSource(getHDS(g, "months"));
        //_tvMonths.expand(".k-item");  
    });
}

function getHDS(items, fieldName) {
    return new kendo.data.HierarchicalDataSource({
        data: items,
        schema: { model: { children: fieldName } }
    });
}

function loadCharts() {
    var items = _items.filter((i) => _selMonth.includes(i.month) && _selProduct.includes(i.itemCode));
    console.log("items", items);

    if (items.length > 0) {
        //Resumen por ciudad
        var resumeCity = Enumerable.From(items).GroupBy("$.city", "$",
            (k, g) => Object.assign({}, { name: k, y: Enumerable.From(g.source).Sum("$.points") })
        ).ToArray();
        Highcharts.chart('resume-by-city', {
            credits: false,
            chart: { type: 'pie' },
            title: { text: 'Resumen de monedas por ciudad' },
            tooltip: { pointFormat: 'Total: <b>{point.y:,f} ( {point.percentage:.2f} % )</b>' },
            plotOptions: {
                series: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: [
                        { enabled: true, distance: 20, style: { fontSize: '0.9em' } },
                        {
                            enabled: true,
                            distance: -40,
                            format: '{point.y:,f}',
                            style: { textOutline: 'none', opacity: 0.7, style: { fontSize: '1em' } },
                            filter: { operator: '>', property: 'percentage', value: 4 }
                        }
                    ]
                }
            },
            series: [
                { name: 'Monedas', colorByPoint: true, data: resumeCity }
            ]
        });

        //Recouento de items por mes
        var monthCats = Enumerable.From(items).Select((i) => `${i.date.getFullYear()}<br />${i.date.toLocaleString('es-BO', { month: 'long' }).toTitleCase()}`).Distinct().ToArray();
        var monthSerieNames = Enumerable.From(_products).Where((p) => _selProduct.includes(p.itemCode)).Select("$.line").Distinct().ToArray();
        var monthSeries = monthSerieNames.map(function (s) {
            var serieData = [];
            var monthsIds = Enumerable.From(items).Select((i) => `${i.date.getFullYear()}-${i.date.getMonth() + 1}`).Distinct().ToArray();
            monthsIds.forEach(function (m) {
                serieData.push(Enumerable.From(items).Where(`$.line == '${s}' && $.month == '${m}'`).Sum("$.points"));
            });
            return { name: s, data: serieData };
        });
        Highcharts.chart('resume-by-month', {
            credits: false,
            chart: { type: 'column' },
            title: { text: 'Productos escaneados' },
            xAxis: {
                categories: monthCats,
                crosshair: true,
                accessibility: { description: 'Meses' }
            },
            yAxis: { min: 0, title: { text: '' } },
            plotOptions: { column: { pointPadding: 0.2, borderWidth: 0 } },
            series: monthSeries
        });

        //Top 10 skus más vendidos (no terminado)
        var topSkus = Enumerable.From(items).GroupBy("$.itemCode", "$", (k, g) => Object.assign({}, { name: k, y: g.source.length })
        ).OrderByDescending("$.y").Take(10).ToArray();
        var topSkusSeries = topSkus.map((x) => Object.assign({}, { name: x.name, data: [x.y] }));
        console.log("TOP", topSkus, topSkusSeries);
        Highcharts.chart('top-skus', {
            credits: false,
            chart: { type: 'bar' },
            title: { text: 'Productos más vendidos (TOP 10)' },
            xAxis: { categories: [''], crosshair: true },
            yAxis: { min: 0, title: { text: '' } },
            plotOptions: { column: { pointPadding: 0.2, borderWidth: 0 } },
            series: topSkusSeries
        });


        //Top 10 usuarios
        var topUsers = Enumerable.From(items).GroupBy("$.userName", "$",
            (k, g) => Object.assign({}, { name: k, points: Enumerable.From(g.source).Sum("$.points") })
        ).OrderByDescending("$.points").Take(10).ToArray();
        var serieTopUsers = topUsers.map((x) => Object.assign({}, { name: x.name, data: [x.points] }));
        Highcharts.chart('resume-top-by-user', {
            credits: false,
            chart: { type: 'bar' },
            title: { text: 'TOP 10 Usuarios' },
            xAxis: { categories: [''], crosshair: true },
            yAxis: { min: 0, title: { text: '' } },
            tooltip: { valueSuffix: ' monedas' },
            plotOptions: { column: { pointPadding: 0.2, borderWidth: 0 } },
            series: serieTopUsers
        });

        //Resumen clientes
        var getClientName = function (code) {
            var client = _clients.find((x) => x.cardCode == code);
            return client?.cardName ?? '';
        }
        var resumeClients = Enumerable.From(items).GroupBy("$.cardCode", "$",
            (k, g) => Object.assign({}, { name: getClientName(k), y: g.source.length })).OrderByDescending("$.y").ToArray(); //[getClientName(k), g.source.length]).ToArray();
        console.log("Resume Clients", resumeClients);

        Highcharts.chart('resume-top-by-client', {
            credits: false,
            chart: { type: 'pie', height: 600 },
            title: { text: 'Resumen de items por Cliente' },
            tooltip: { pointFormat: 'Total: <b>{point.y:,f} ( {point.percentage:.2f} % )</b>' },
            plotOptions: {
                series: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    showInLegend: true,
                    dataLabels: [
                        //{ enabled: true, distance: 20, style: { fontSize: '0.9em' } },
                        {
                            enabled: true,
                            distance: -40,
                            format: '{point.y:,f}',
                            style: { textOutline: 'none', opacity: 0.7, style: { fontSize: '1.5em' } },
                            filter: { operator: '>', property: 'percentage', value: 4 }
                        }
                    ]
                }
            },
            series: [
                { name: 'Total', colorByPoint: true, data: resumeClients }
            ]
        });

    }
}

//#endregion