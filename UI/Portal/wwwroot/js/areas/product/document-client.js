//#region GLOBAL VARIABLES
var _maxDate, _minDate, products = [];
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, gridMargin = 30, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}";
//#endregion

//#region EVENTS

$(window).resize(setHeights);

$(() => setupControls());

$("#Listado").on("click", ".product", onProductSelected);

$("#Documents").on("click", ".download-files", onDownloadingFiles);

//$("#Documents").on("click", ".send-email", onOpeningSendingConfig);

//#endregion

//#region METHODS

function setupControls() {
    var productTemplate = `<div class="line">
        <h5>#= data.value #</h5>
        <div class="product-list">
            # for (var i = 0; i < data.items.length; i++) { #
                <div class="product" data-id="#=data.items[i].id#" data-idLine="#=data.items[i].idLine#">
                    <span class="item-code">#= data.items[i].itemCode #</span><br />
                    <span class="name">#= data.items[i].name #</span>
                </div>
            # } #
        </div>
    </div>`;
    var listProducts = $("#Listado").kendoListView({ bordered: false, template: kendo.template(productTemplate) }).getKendoListView();

    var docTemplate = `<div class="doc-type">
        <h5>#= data.value #</h5>
        <table>
            <colgroup><col><col style="width:150px"><col style="width:30px"></colgroup>
        # for (var i = 0; i < data.items.length; i++) { #
            <tr>
                <td>#= data.items[i].name #</td>
                <td><b>Fecha:</b> #=kendo.toString(JSON.toDate(data.items[i].releaseDate), "dd-MM-yyyy")#</td>
                <td><a class="action action-link download-files" data-id="#=data.items[i].id#"><i class="fas fa-download"></i></a></td>
            </tr>
        # } #
        </table>
    </div>`;
    //docTemplate = `<div><h5>#= data.value #</h5></div>`;
    $("#Documents").kendoListView({ bordered: false, template: kendo.template(docTemplate) });

    $.get(urlGetAvailableProducts, {}, d => {
        products = d;
        var ds = new kendo.data.DataSource({ data: d, group: { field: "lineName", dir: 'asc' }, schema: { model: { id: "id" } } });
        listProducts.setDataSource(ds);
    });

    setHeights();
}

function setHeights() {
    $("#Listado .k-listview-content").height($(window).height() - 104);
}

function onProductSelected(e) {
    $(e.currentTarget).closest(".k-listview-content").find(".product").removeClass("selected");
    $(e.currentTarget).addClass("selected");
    var idProduct = e.currentTarget.dataset.id, productName = $(e.currentTarget).find(".name").text();
    $("#product-name").text(productName);

    $.get(urlFilter, { IdProduct: idProduct }, function(d) {
        if (d.message === "") {
            var docView = $("#Documents").getKendoListView();
            var ds = new kendo.data.DataSource({ data: d.items, group: { field: "typeName", dir: 'asc' }, schema: { model: { id: "id" } } });
            docView.setDataSource(ds);
        } else {
            showError(d.message);
        }
    });
}

function onDownloadingFiles(e) {
    e.preventDefault();
    var id = e.currentTarget.dataset.id;
    window.location.href = urlDownloadFiles + "?" + $.param({ Id: id });
}

//#endregion