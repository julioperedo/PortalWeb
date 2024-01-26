//#region Global Variables

const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, dateFormat = "{0:dd-MM-yyyy HH:mm:ss}", _gridMargin = 20;

//#endregion

//#region Events

$(() => initPage());

$(window).resize(function () { setGridHeight("Listado", _gridMargin); });

$("#action-clean").click(cleanFilters);

$("#action-filter").click(filterData);

$("#action-excel").click(exportExcel);

//#endregion

//#region Methods

function initPage() {
    setupControls();
    setGridHeight("Listado", _gridMargin);
}

function setupControls() {
    var config = { dataTextField: "Text", dataValueField: "Value", optionLabel: "Cualquiera", dataSource: [{ Text: "Si", Value: "Y" }, { Text: "No", Value: "N" }] };
    $("#Enabled").kendoDropDownList(config);
    $("#Stock").kendoDropDownList(config);
    $("#Comments").kendoDropDownList(config);
    $("#Transit").kendoDropDownList(config);
    $("#Price").kendoDropDownList(config);
    $("#ShowInWeb").kendoDropDownList(config);
    $("#WithImage").kendoDropDownList(config);
    $("#WithLink").kendoDropDownList(config);
    $("#WithRotation").kendoDropDownList(config);
    $("#Manager").kendoDropDownList({ dataTextField: "name", dataValueField: "shortName", optionLabel: "Todos", dataSource: { transport: { read: { url: urlGetPMs } } } });

    $("#Listado").kendoGrid({
        columns: [
            { title: "Line", hidden: true, field: "line", aggregates: ["count"], groupHeaderTemplate: "Línea: #= value #    ( Total: #= count# )" },
            { title: "Category", hidden: true, field: "category", aggregates: ["count"], groupHeaderTemplate: "Categoría: #= value #    ( Total: #= count# )" },
            { title: "Sub Category", hidden: true, field: "subCategory", aggregates: ["count"], groupHeaderTemplate: "Subcategoría: #= value #    ( Total: #= count# )" },
            { title: "Item", width: 170, footerTemplate: "Total : #= count#", field: "itemCode", aggregates: ["count"] },
            { title: "Nombre", field: "name", width: 230 },
            { title: "Product Manager", hidden: true, field: "productManager", aggregates: ["count"], groupHeaderTemplate: "PM: #= value #    ( Total: #= count# )" },
            { title: "Habilitado", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: '# if(enabled) {# <i class="fas fa-check"></i> #} #', field: "enabled" },
            { title: "En Web", attributes: alignCenter, headerAttributes: alignCenter, width: 100, template: '# if(showInWeb) {# <i class="fas fa-check"></i> #} #', field: "showInWeb" },
            { title: "Link", attributes: alignCenter, headerAttributes: alignCenter, width: 80, template: '# if(link && link !== "") {# <i class="fas fa-check"></i> #} #', field: "link" },
            { title: "Comentarios", attributes: alignCenter, headerAttributes: alignCenter, width: 120, template: '# if(commentaries && commentaries !== "") {# <i class="fas fa-check"></i> #} #', field: "commentaries" },
            { title: " ", attributes: alignCenter, width: 30, template: '# if(imageURL !== null && $.trim(imageURL) !== "") {# <i class="fas fa-image"></i> #} #', field: "imageURL" },
            {
                title: "Precios", headerAttributes: { style: "text-align: center;", colSpan: 3 },
                columns: [
                    { title: "Central", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "priceSantaCruz", format: "{0:N2}" },
                    { title: "LA", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "priceMiami", format: "{0:N2}" },
                    { title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 100, field: "priceIquique", format: "{0:N2}" }
                ]
            },
            {
                title: "Stock", headerAttributes: { style: "text-align: center;", colSpan: 3 },
                columns: [
                    { title: "Central", attributes: alignRight, headerAttributes: alignRight, width: 100, template: '# if(santaCruz) {# #=santaCruz# #} # # if(reservedSantaCruz) {# (-#=reservedSantaCruz#) #} #', field: "santaCruz" },
                    { title: "LA", attributes: alignRight, headerAttributes: alignRight, width: 100, template: '# if(miami) {# #=miami# #} # # if(reservedMiami) {# (-#=reservedMiami#) #} #', field: "miami" },
                    { title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 100, template: '# if(iquique) {# #=iquique# #} # # if(reservedIquique) {# (-#=reservedIquique#) #} #', field: "iquique" }
                ]
            },
            {
                title: "Tránsito", headerAttributes: { style: "text-align: center;", colSpan: 3 },
                columns: [
                    { title: "Central", attributes: alignRight, headerAttributes: alignRight, width: 80, field: "transitSantaCruz" },
                    { title: "LA", attributes: alignRight, headerAttributes: alignRight, width: 80, field: "transitMiami" },
                    { title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 80, field: "transitIquique" }
                ]
            },
            {
                title: "Rotación", headerAttributes: { style: "text-align: center;", colSpan: 3 },
                columns: [
                    { title: "Central", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "rotationSantaCruz" },
                    { title: "LA", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "rotationMiami" },
                    { title: "Iquique", attributes: alignRight, headerAttributes: alignRight, width: 90, field: "rotationIquique" }
                ]
            }
        ],
        sortable: true, selectable: "Single, Row", noRecords: { template: '<div class="w-100 text-center p-3">No hay resultados para el criterio de búsqueda.</div>' },
        dataSource: getDatasource([])
    });

}

function getDatasource(items) {
    var ds = new kendo.data.DataSource({
        data: items,
        group: [
            { field: "productManager", dir: "asc", aggregates: [{ field: "productManager", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }, { field: "subCategory", aggregate: "count" }, { field: "itemCode", aggregate: "count" }] },
            { field: "line", dir: "asc", aggregates: [{ field: "productManager", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }, { field: "subCategory", aggregate: "count" }, { field: "itemCode", aggregate: "count" }] },
            { field: "category", dir: "asc", aggregates: [{ field: "productManager", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }, { field: "subCategory", aggregate: "count" }, { field: "itemCode", aggregate: "count" }] },
            { field: "subCategory", dir: "asc", aggregates: [{ field: "productManager", aggregate: "count" }, { field: "line", aggregate: "count" }, { field: "category", aggregate: "count" }, { field: "subCategory", aggregate: "count" }, { field: "itemCode", aggregate: "count" }] }
        ],
        aggregate: [
            { field: "productManager", aggregate: "count" },
            { field: "line", aggregate: "count" },
            { field: "category", aggregate: "count" },
            { field: "subCategory", aggregate: "count" },
            { field: "itemCode", aggregate: "count" }
        ]
    });
    return ds;
}

function loadResults(items) {
    var grd, ds;
    grd = $("#Listado").data("kendoGrid");
    ds = getDatasource(items);
    grd.setDataSource(ds);

    var margin = _gridMargin;
    if (items && items.length > 0) {
        $('#filter-box').collapse("hide");
        $("#action-excel").removeClass("d-none");
        margin -= 20;
    } else {
        $("#action-excel").addClass("d-none");
    }
    setTimeout(() => { setGridHeight("Listado", margin) }, 200);
}

function cleanFilters(e) {
    $("#Enabled").getKendoDropDownList().value("");
    $("#Stock").getKendoDropDownList().value("");
    $("#Transit").getKendoDropDownList().value("");
    $("#Price").getKendoDropDownList().value("");
    $("#Manager").getKendoDropDownList().value("");
    $("#Comments").getKendoDropDownList().value("");
    $("#ShowInWeb").getKendoDropDownList().value("");
    $("#WithImage").getKendoDropDownList().value("");
    $("#WithLink").getKendoDropDownList().value("");
    loadResults([]);
}

function filterData(e) {
    e.preventDefault();
    var strEnabled = $("#Enabled").val(), strStock = $("#Stock").val(), strTransit = $("#Transit").val(), strPrice = $("#Price").val(), strComments = $("#Comments").val(), strManager = "", showInWeb = $("#ShowInWeb").val(),
        withImage = $("#WithImage").val(), withLink = $("#WithLink").val(), withRotation = $("#WithRotation").val();
    if ($("#Manager").getKendoDropDownList().value() != "") { strManager = $("#Manager").getKendoDropDownList().text(); }
    var lstTitle = [];
    if (strEnabled == "Y") { lstTitle.push("Habilitados"); }
    if (strEnabled == "N") { lstTitle.push("Deshabilitados"); }
    if (strStock == "Y") { lstTitle.push("Con Stock"); }
    if (strStock == "N") { lstTitle.push("Sin Stock"); }
    if (strTransit == "Y") { lstTitle.push("Con Tránsito"); }
    if (strTransit == "N") { lstTitle.push("Sin Tránsito"); }
    if (strPrice == "Y") { lstTitle.push("Con Precio"); }
    if (strPrice == "N") { lstTitle.push("Sin Precio"); }
    if (showInWeb == "Y") { lstTitle.push("Visible en Web"); }
    if (showInWeb == "N") { lstTitle.push("No visible en Web"); }
    if (strManager != "") { lstTitle.push("De " + strManager); }
    if (withImage !== "") { lstTitle.push(withImage === "Y" ? "Con Imagen" : "Sin Imagen"); }
    if (withLink !== "") { lstTitle.push(withLink === "Y" ? "Con Vínculo" : "Sin Vínculo"); }
    if (WithRotation !== "") { lstTitle.push(WithRotation === "Y" ? "Con Rotación" : "Sin Rotación"); }
    $.get(urlFilter, { Enabled: strEnabled, WithStock: strStock, InTransit: strTransit, WithPrice: strPrice, WithCommentaries: strComments, ProductManager: strManager, InWeb: showInWeb, WithImage: withImage, WithLink: withLink, WithRotation: withRotation }, function (data) {
        if (data.message === "") {
            loadResults(data.items);
            if (lstTitle.length > 0) {
                $("#title").text(" - " + lstTitle.join(", "));
            } else {
                $("#title").text("");
            }
            if (data.items.length === 0) {
                showInfo("No existen resultados para ese criterio de búsqueda.");
            }
        } else {
            showError(data.message);
        }
    });
}

function exportExcel(e) {
    e.preventDefault();
    var strEnabled = $("#Enabled").val(), strStock = $("#Stock").val(), strTransit = $("#Transit").val(), strPrice = $("#Price").val(), strComments = $("#Comments").val(), strManager = "", showInWeb = $("#ShowInWeb").val(),
        withImage = $("#WithImage").val(), withLink = $("#WithLink").val(), withRotation = $("#WithRotation").val();
    if ($("#Manager").getKendoDropDownList().value() !== "") { strManager = $("#Manager").getKendoDropDownList().text(); }
    window.location.href = urlExport + "?" + $.param({ Enabled: strEnabled, WithStock: strStock, InTransit: strTransit, WithPrice: strPrice, WithCommentaries: strComments, ProductManager: strManager, InWeb: showInWeb, WithImage: withImage, WithLink: withLink, WithRotation: withRotation });
}

//#endregion