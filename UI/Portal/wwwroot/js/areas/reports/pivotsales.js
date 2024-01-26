//#region Global Variables

var _minDate, _maxDate;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy HH:mm:ss}", _gridMargin = 30;
var _loaded = false, _reconfigured = false, localUser = false, permissions;
var _minSC, _minIQ, _minMI;
var _options = {
    pivot: {
        rows: ["Fecha", "Cliente", "Factura"],
        columns: ["Sucusrsal"],
        values: [
            { field: "Total" },
            { field: "MargenPorcentage" },
            { field: "Cantidad" }
        ],
        xfields: [
            { field: "Cliente", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
            { field: "GerenteProducto", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
            { field: "Fecha", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
            { field: "Factura", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
            { field: "Item", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
            { field: "Sucursal", state: $("#ColapsedRows").prop("checked") ? "closed" : "opened" },
            {
                field: "MargenPorcentage", title: "Margen %", op: "avg", formatter: function (value) { return kendo.toString(+value, "#,##0.00") + " %" },
                styler: function (value, row) {
                    if (value != 0 & ((/SANTA CRUZ/g.test(this.field) & value < _minSC) | (/IQUIQUE/g.test(this.field) & value < _minIQ) | (/MIAMI/g.test(this.field) & value < _minMI))) {
                        return { class: "low-margin" };
                    }
                    return {};
                }
            },
            { field: "Cantidad", op: "sum", formatter: function (value) { return kendo.toString(+value, "#,##0"); } },
            { field: "Total", op: "sum", formatter: function (value) { return kendo.toString(+value, "#,##0.00"); } }
        ]
    },
    forzenColumnTitle: "<span style=\"font-weight:bold\">Detalle</span>",
    valueFormatter: function (value) { return kendo.toString(+value, "#,##0.00"); },
    valuePrecision: 2,
    valueFieldWidth: 100,
    showFooter: true,
    FooterTitle: "TOTAL",
    showColumnsTotals: true,
    i18n: { fields: "Campos", filters: "Filtros", rows: "Filas", columns: "Columnas", values: "Valores", ok: "Aceptar", cancel: "Cancelar" },
    onDblClickRow: function (row) {
        $(this).pivotgrid("unselectAll");
    },
    onLoadSuccess: function (row, data) {
        var options = $(this).pivotgrid("options");
        var pivotType = localUser ? $("#PivotType").getKendoDropDownList().value() : "V";
        if (pivotType === "C" | options.pivot.redrawed === true) {
            if (localUser) $("#PivotType").getKendoDropDownList().value("C");
            var rows, columns, values;
            if (typeof options.pivot.rows[0] === "object") {
                rows = Enumerable.From(options.pivot.rows).Select("$.field").ToArray().join();
            } else {
                rows = options.pivot.rows.join();
            }
            if (typeof options.pivot.columns[0] === "object") {
                columns = Enumerable.From(options.pivot.columns).Select("$.field").ToArray().join();
            } else {
                columns = options.pivot.columns.join();
            }
            if (typeof options.pivot.values[0] === "object") {
                values = Enumerable.From(options.pivot.values).Select("$.field").ToArray().join();
            } else {
                values = options.pivot.values.join();
            }
            $.post(urlUpdate, { Rows: rows, Columns: columns, Values: values }, function (data) { if (data.message !== "") { showError(data.message); } });
        }
    }
};

//#endregion

//#region Events

$(() => setupControls());

$(window).resize(resizePivot);

$("#action-config").click(configLayoutPivot);

$("#action-clean").click(cleanFilters);

$("#action-filter").click(filterData);

$("#action-excel").click(exportExcel);

//#endregion

//#region Methods

function setupControls() {
    localUser = $("#local-user").val() === "Y";
    permissions = localUser ? JSON.parse($("#permissions").val()) : {};
    var today = new Date(), initDate = new Date(today.getFullYear(), today.getMonth(), 1);
    var filSince = $("#Since").kendoDatePicker({
        format: "d/M/yyyy", value: initDate, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDatePicker");
    var filUntil = $("#Until").kendoDatePicker({
        format: "d/M/yyyy", value: today, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDatePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    $("#Subsidiary").multipleSelect({
        placeholder: "Seleccione una Sucursal",
        onUncheckAll: () => { $("#Storehouse").empty().multipleSelect("refresh").multipleSelect("disable"); },
        onCheckAll: () => { loadStoreHouses(); },
        onClick: (view) => { loadStoreHouses(); }
    });
    $("#Storehouse").multipleSelect({ placeholder: "Seleccione algún Almacén" }).multipleSelect("disable");

    $("#Category").multipleSelect({ placeholder: "Seleccione alguna Categoría", onClick: function (view) { loadSubcategories(); }, onCheckAll: function () { loadSubcategories(); }, onUncheckAll: function () { loadSubcategories(); } });
    $("#SubCategory").multipleSelect({ placeholder: "Seleccione alguna Subcategoría" });
    $("#Line").multipleSelect({ placeholder: "Seleccione alguna Línea" });
    if (localUser) {
        $("#Client").multipleSelect({ placeholder: "Seleccione algún Cliente", filter: true, selectAll: false });
        $("#SalesMan").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", optionLabel: "Seleccione un Valor ...", value: permissions.SellerCode, enable: permissions.SeeAllClients, dataSource: { transport: { read: { url: urlSellers } } } });
        $("#GP").kendoDropDownList({ dataTextField: "name", dataValueField: "name", optionLabel: "Seleccione un Valor ...", dataSource: { transport: { read: { url: urlPManagers } } } });

        $("#PivotType").kendoDropDownList({
            dataTextField: "Text", dataValueField: "Value", value: permissions.SeeAllClients & permissions.SeeMargin ? "C" : "V", enable: permissions.SeeAllClients & permissions.SeeMargin,
            dataSource: [{ Text: "Custom", Value: "C", Selected: true }, { Text: "Ventas", Value: "V" }, { Text: "Gerentes de Producto", Value: "GP" }, { Text: "Vendedores", Value: "VS" }]
        });
    }

    $.get(urlGetSubsidiaries, {}, (data) => {
        if (data.message === "") {
            var ddl = $("#Subsidiary");
            data.items.forEach((x) => {
                ddl.append(new Option(x.name, x.id, false, false));
            });
            ddl.multipleSelect("refresh");
        } else {
            showError(`Ha ocurrido el siguiente error al traer las Sucursales: <br />${data.message}.`);
        }
    });
    $.get(urlLines, {}, (data) => {
        if (data) {
            var ddl = $("#Line");
            data.forEach((x) => {
                ddl.append(new Option(x.name, x.id, false, false));
            });
            ddl.multipleSelect("refresh");
        }
    });
    $.get(urlCategories, {}, (data) => {
        if (data) {
            var ddl = $("#Category");
            data.forEach((x) => {
                ddl.append(new Option(x.name, x.id, false, false));
            });
            ddl.multipleSelect("refresh");
        }
    });
    if (localUser) {
        $.get(urlClients, {}, (data) => {
            if (data) {
                var ddl = $("#Client");
                data.forEach((x) => ddl.append(new Option(x.name, x.code, false, false)));
                ddl.multipleSelect("refresh");
            }
        });
    }

    $.extend($.fn.pivotgrid.defaults.operators, {
        avg: function (rows, field) {
            //var opts = $(this).pivotgrid("options");
            var v = 0, margen = 0, total = 0;
            if (field === "MargenPorcentage") {
                $.map(rows, function (row) {
                    margen += parseFloat(row.Margen) || 0;
                    total += parseFloat(row.TotalSinImpuestos) || 0;
                });
                if (total > 0) {
                    v = margen / total * 100;
                }
            } else {
                if (rows.length > 0) {
                    $.map(rows, function (row) { v += parseFloat(row[field]) || 0; });
                    v = v / rows.length;
                }
            }
            return v;
        },
        countDistinct: function (rows, field) {
            return Enumerable.From(rows).Select("$." + field).Distinct().Count();
        }
    });

}

function loadStoreHouses() {
    $("#Storehouse").empty();
    var objSelected = $("#Subsidiary").multipleSelect("getSelects", "text");
    if (objSelected && objSelected.length > 0) {
        var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
        $.get(urlWarehouses, { Subsidiary: strData }, (data) => {
            if (data.message !== "") {
                showError(data.message);
                $("#Storehouse").multipleSelect("disable");
            } else {
                if (data.items.length > 0) {
                    $.each(data.items, function (i, obj) {
                        $("#Storehouse").append(new Option(obj.name, obj.name, true, true));
                    });
                    $("#Storehouse").multipleSelect("enable");
                } else {
                    $("#Storehouse").multipleSelect("disable");
                }
            }
            $("#Storehouse").multipleSelect("refresh");
        });
    } else {
        $("#Storehouse").multipleSelect("refresh").multipleSelect("disable");
    }
}

function loadSubcategories() {
    $("#SubCategory").empty();
    var objSelected = $("#Category").multipleSelect("getSelects", "text");
    if (objSelected && objSelected.length > 0) {
        var strData = Enumerable.From(objSelected).Select(function (x) { return `'${x}'` }).ToArray().join();
        $.get(urlSubcategories, { Categories: strData }, (data) => {
            if (data.message !== "") {
                showError(data.message);
                $("#SubCategory").multipleSelect("disable");
            } else {
                if (data.items.length > 0) {
                    $.each(data.items, function (i, obj) {
                        $("#SubCategory").append(new Option(obj.name, obj.name, false, false));
                    });
                    $("#SubCategory").multipleSelect("enable");
                } else {
                    $("#SubCategory").multipleSelect("disable");
                }
            }
            $("#SubCategory").multipleSelect("refresh");
        });
    } else {
        $("#SubCategory").multipleSelect("refresh").multipleSelect("disable");
    }
}

function getFilters() {
    var strRegionals = "", strWarehouses = "", strCategories = "", strSubcategories = "", strLines = "", strClients = "", strSalesMan = "", strProductManager = "", strItemCode;
    var objSince = $("#Since").data("kendoDatePicker").value();
    if (objSince) {
        objSince = objSince.toISOString();
    }
    var objUntil = $("#Until").data("kendoDatePicker").value();
    if (objUntil) {
        objUntil = objUntil.toISOString();
    }
    var objSelected = $("#Subsidiary").multipleSelect("getSelects", "text");
    if (objSelected && objSelected.length > 0) {
        strRegionals = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
    }
    objSelected = $("#Storehouse").multipleSelect("getSelects");
    if (objSelected && objSelected.length > 0) {
        strWarehouses = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
    }
    objSelected = $("#Category").multipleSelect("getSelects", "text");
    if (objSelected && objSelected.length > 0) {
        strCategories = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
    }
    objSelected = $("#SubCategory").multipleSelect("getSelects", "text");
    if (objSelected && objSelected.length > 0) {
        strSubcategories = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
    }
    objSelected = $("#Line").multipleSelect("getSelects", "text");
    if (objSelected && objSelected.length > 0) {
        strLines = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
    }
    if (localUser) {
        objSelected = $("#Client").multipleSelect("getSelects", "value");
        if (objSelected && objSelected.length > 0) {
            strClients = Enumerable.From(objSelected).Select(x => `'${x}'`).ToArray().join();
        }
        if ($("#SalesMan").getKendoDropDownList().value() !== "") {
            strSalesMan = $("#SalesMan").getKendoDropDownList().text();
        }
        if ($("#GP").getKendoDropDownList().value() !== "") {
            strProductManager = $("#GP").getKendoDropDownList().text();
        }
    } else {
        strClients = `'${$("#Client").val()}'`;
    }
    strItemCode = $("#ItemCode").val();

    return {
        InitialDate: objSince, FinalDate: objUntil, Regionals: strRegionals, Warehouses: strWarehouses, Categories: strCategories, SubCategories: strSubcategories, Lines: strLines,
        Clients: strClients, SalesMan: strSalesMan, ProductManager: strProductManager, ItemCode: strItemCode
    };
}

function loadPivot(items) {
    var intHeight = $(window).height() - 130, strType = localUser ? $("#PivotType").getKendoDropDownList().value() : "CL";
    var arrRows = [], arrColumns = [], arrValues = [];
    if (strType === "C") {
        //Custom
        if (_loaded) {
            _options = $("#pivot").pivotgrid("options");
        } else {
            if (_UserConfig !== "") {
                _UserConfig = _UserConfig.replace("&#241;", "ñ");
                var parts = _UserConfig.split(";");
                if (parts[0] != null && $.trim(parts[0]) != "") {
                    arrRows = parts[0].split(",");
                }
                if (parts[1] != null && $.trim(parts[1]) != "") {
                    arrColumns = parts[1].split(",");
                }
                if (parts[2] != null && $.trim(parts[2]) != "") {
                    arrValues = Enumerable.From(parts[2].split(",")).Select("{ field: $ }").ToArray();
                }
                _options.pivot.rows = arrRows;
                _options.pivot.columns = arrColumns;
                _options.pivot.values = arrValues;
            }
        }
    } else if (strType === "V") {
        // Ventas
        arrValues = [{ field: "Total", op: "sum" }];
        arrRows = ["Sucursal", "Cliente"];
        arrColumns = ["Mes2"];

        _options.pivot.rows = arrRows;
        _options.pivot.columns = arrColumns;
        _options.pivot.values = arrValues;

    } else if (strType === "CL") {
        // Clientes
        arrValues = [{ field: "Total", op: "sum" }];
        arrRows = ["Linea"];
        arrColumns = ["Mes2"];

        _options.pivot.rows = arrRows;
        _options.pivot.columns = arrColumns;
        _options.pivot.values = arrValues;

    } else if (strType == "GP") {
        // Gerentes de Producto
        arrValues = [{ field: "Cantidad", op: "sum" }, { field: "Total", op: "sum" }, { field: "MargenPorcentage", op: "avg" }];
        arrRows = ["GerenteProducto", "Linea"];

        _options.pivot.rows = arrRows;
        _options.pivot.columns = arrColumns;
        _options.pivot.values = arrValues;
    } else {
        // Vendedores
        arrValues = [{ field: "Total", op: "sum" }];
        if (permissions.SeeMargin) {
            arrValues.push({ field: "MargenPorcentage", op: "avg" });
        }
        arrColumns = ["Mes2"];
        arrRows = ["Vendedor", "Cliente"];

        _options.pivot.rows = arrRows;
        _options.pivot.columns = arrColumns;
        _options.pivot.values = arrValues;
    }

    var collapsed = $("#ColapsedRows").prop("checked");
    $.map(_options.pivot.xfields, function (obj) { obj.state = collapsed ? "closed" : "opened"; });
    _options.height = intHeight;
    _options.data = items;
    $(".datagrid-row-selected, .datagrid-row-checked").removeClass("datagrid-row-checked").removeClass("datagrid-row-selected");

    $("#action-excel").toggleClass("d-none", items.length <= 0);

    $("#pivot").pivotgrid(_options);
    _loaded = true;
}

function filterData(e) {
    var filters = getFilters();
    $("#pivot-container").empty();

    $.ajaxSetup({ cache: false });
    $.get(urlFilter, filters, function (data) {
        $('#filter-box').collapse("hide");
        _minSC = +$("#MinSantaCruz").val(), _minIQ = +$("#MinIquique").val(), _minMI = +$("#MinMiami").val();
        loadPivot(data);
    });
}

function exportExcel(e) {
    var filters = getFilters();
    window.location.href = urlExport + "?" + $.param(filters);
}

function cleanFilters(e) {
    $("#Subsidiary").multipleSelect("uncheckAll");
    $("#Category").multipleSelect("uncheckAll");
    $("#Line").multipleSelect("uncheckAll");
    var objDate = new Date();
    $("#Until").data("kendoDatePicker").value(objDate);
    objDate.setDate(1);
    $("#Since").data("kendoDatePicker").value(objDate);

    loadPivot([]);
}

function configLayoutPivot(e) {
    e.preventDefault();
    $('#pivot').pivotgrid('layout');
}

function resizePivot(e) {
    var intHeight = $(window).height() - $(".navbar").height() - $(".toolbar").height() - 50;
    $("#pivot").pivotgrid("resize", { height: intHeight });
}

//#endregion