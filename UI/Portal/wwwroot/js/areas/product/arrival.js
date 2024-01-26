//#region Global Variables
const _gridMargin = 30, _alignCenter = { "class": "k-text-center !k-justify-content-center" }, _alignRight = { style: "text-align: right;" }, _alignRight2 = { "class": "table-header-cell !k-text-right" };
var _gridItems;
//#endregion

//#region Events

$(function () {
    setupControls();
    filterData();
});

$(window).resize(() => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('hidden.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$('#filter-box').on('shown.bs.collapse', () => setGridHeight("Listado", _gridMargin));

$("#action-clean").click(cleanFilters);

$("#action-filter").click(filterData);

//#endregion

//#region Private Methods

function setupControls() {

    _gridItems = $("#Listado").kendoGrid({
        dataBound: e => {
            var grid = e.sender;
            for (var i = 0; i < grid.columns.length; i++) {
                grid.showColumn(i);
            }
            $("div.k-group-indicator").each((i, v) => grid.hideColumn($(v).data("field")));
            grid.element.find("table").attr("style", "");
        },
        groupable: { enabled: true, messages: { empty: "Arrastre un encabezado de columna y colóquela aquí para agrupar por esa columna" } },
        pageable: { enabled: true },
        noRecords: { "template": "No existen resultados para el criterio de búsqueda." },
        scrollable: true, sortable: true, selectable: true,
        columns: [
            { field: "line", title: "Línea", hidden: true, width: 120, aggregates: ["count"], groupHeaderTemplate: "Línea: #=value#    (Total: #=count#)" },
            { field: "category", title: "Categoría", hidden: true, width: 120, aggregates: ["count"], groupHeaderTemplate: "Categoría: #=value#    (Total: #=count#)" },
            { field: "subcategory", title: "Subcategoría", width: 120, aggregates: ["count"], groupHeaderTemplate: "Subcategoría: #= value #    (Total: #= count#)" },
            { field: "itemCode", title: "Item", width: 160, groupable: false },
            { field: "itemName", title: "Descripción", width: 300, groupable: false },
            { field: "stock", title: "Cantidad", width: 90, attributes: _alignRight, headerAttributes: _alignRight, template: (e) => _isLocal || e.stock <= 50 ? e.stock : "50+" },
            { field: "arrivalDate", title: "F. Est. Arribo", width: 100, attributes: _alignCenter, headerAttributes: _alignCenter, format: "{0:dd-MM-yyyy}" }
        ],
        dataSource: getDataSource([])
    }).data("kendoGrid");

}

function cleanFilters() {
    $("#filter-product").val("");
}

function loadGrid(items) {
    var ds = getDataSource(items);
    _gridItems.setDataSource(ds);
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
    } else {
        $("#action-excel").addClass("d-none");
    }
    setGridHeight("Listado", _gridMargin);
}

function filterData() {
    $.get(urlFilter, { Filter: $("#filter-product").val() }, function (d) {
        if (d.message != "") {
            showError(d.message);
            loadGrid([]);
        } else {
            loadGrid(d.items);
        }
    });
}

function getDataSource(items) {
    items.forEach((x) => {
        x.date = JSON.toDate(x.date);
        x.arrivalDate = JSON.toDate(x.arrivalDate);
    });
    var ds = new kendo.data.DataSource({
        data: items,
        pageSize: 500,
        aggregate: [
            { aggregate: "count", field: "line" },
            { aggregate: "count", field: "category" },
            { aggregate: "count", field: "subcategory" }
        ],
        group: [
            { field: "line", dir: "asc", aggregates: [{ field: "line", aggregate: "count" }] },
            { field: "category", dir: "asc", aggregates: [{ field: "category", aggregate: "count" }] },
            { field: "subcategory", dir: "asc", aggregates: [{ field: "subcategory", aggregate: "count" }] }
        ]
    });
    return ds;
}

//#endregion