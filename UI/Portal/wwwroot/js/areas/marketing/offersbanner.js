//#region Variables Globales
var _minDate, _maxDate, notification;
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30, objConfig = { messages: { required: "*" } };
//#endregion

//#region Eventos

$(() => {
    setupControls();
    getOffers();
});

$(window).resize(() => setListViewHeight("Listado", _gridMargin));

$("#Listado").on("click", ".product-item", onItemClicked);

$("#Listado").on("click", ".selector", onAllItemsLineClicked);

$("#select-all").click(onAllItemsClicked);

$("#chk-sa, #chk-la, #chk-iq").click(onSubsidiaryChanged);

$("[name='search-by']").click(onChangeType);

$("#generate-file").click(generateFile);

//#endregion

//#region Metodos Privados

function setupControls() {
    var template = `<div class="line-group">
    <div>
        <span class="line-title">#= data.value #</span> <span class="selector">Todos</span>
    </div>
    <table class="products-list">
        # for (var i = 0; i < data.items.length; i++) { #
        <tr class="product-item action" data-id="#=data.items[i].id#">
            <td><img src="../../images/products/#=data.items[i].imageURL#" /></td>
            <td>
                <span class="item-code">#= data.items[i].itemCode #</span> #if(data.items[i].showTeamOnly){# <span class="text-danger bg-warning">Tomar en cuenta que s&oacute;lo es visible para DMC</span> #}# <br />
                <span class="name">#= data.items[i].name #</span><br />
                # for (var j = 0; j < data.items[i].listPrices.length; j++) { #
                    <span class="price">Precio #=data.items[i].listPrices[j].sudsidiary.name#: #=kendo.toString(data.items[i].listPrices[j].regular, "N2")#</span><span class="ml-4 price">Oferta: #=kendo.toString(data.items[i].listPrices[j].offer, "N2")#</span><br />
                # } #
            </td>
            <td><i></i></td>
        </tr>
        # } #
    </table>
</div>`;
    $("#Listado").kendoListView({ bordered: false, template: template });
    $("#export-type").kendoDropDownList();
    setListViewHeight("Listado", _gridMargin);
}

function getOffers() {
    $.get(urlGetOffers, {}, function (d) {
        if (d.message === "") {
            loadList(d.items);
        } else {
            showError('Se ha producido un error al traer los datos de las ofertas.');
        }
    });
}

function getNewProducts() {
    $.get(urlGetNewProducts, {}, function (d) {
        if (d.message === "") {
            loadList(d.items);
        } else {
            showError('Se ha producido un error al traer los datos de los productos nuevos.');
        }
    });
}

function getAllProducts() {
    $.get(urlGetAllProducts, {}, function (d) {
        if (d.message === "") {
            loadList(d.items);
        } else {
            showError('Se ha producido un error al traer los datos de los productos.');
        }
    });
}

function loadList(items) {
    var list = $("#Listado").data("kendoListView");
    var ds = new kendo.data.DataSource({ data: items, group: { field: "line", dir: 'asc' } });
    list.setDataSource(ds);
}

function onItemClicked(e) {
    $(e.currentTarget).toggleClass("selected");
    $("#generate-file").prop("disabled", $(".product-item.selected").length === 0 || !($("#chk-sa").prop("checked") || $("#chk-iq").prop("checked") || $("#chk-la").prop("checked")));
}

function onAllItemsLineClicked(e) {
    var group = $(e.currentTarget).closest(".line-group");
    group.find(".product-item").toggleClass("selected", group.find(".product-item").length !== group.find(".product-item.selected").length);
    $("#generate-file").prop("disabled", $(".product-item.selected").length === 0 || !($("#chk-sa").prop("checked") || $("#chk-iq").prop("checked") || $("#chk-la").prop("checked")));
}

function onAllItemsClicked(e) {
    var group = $("#Listado");
    group.find(".product-item").toggleClass("selected", group.find(".product-item").length !== group.find(".product-item.selected").length);
    $("#generate-file").prop("disabled", $(".product-item.selected").length === 0 || !($("#chk-sa").prop("checked") || $("#chk-iq").prop("checked") || $("#chk-la").prop("checked")));
}

function onSubsidiaryChanged(e) {
    $("#generate-file").prop("disabled", $(".product-item.selected").length === 0 || !($("#chk-sa").prop("checked") || $("#chk-iq").prop("checked") || $("#chk-la").prop("checked")));
}

function onChangeType(e) {
    if (e.currentTarget.id === "by-new") {
        getNewProducts();
    }
    if (e.currentTarget.id === "by-offer") {
        getOffers();
    }
    if (e.currentTarget.id === "by-all") {
        getAllProducts();
    }
}

function generateFile(e) {
    var ids = $(".product-item.selected").map((i, x) => x.dataset.id).toArray().join(), subsidiaries = $(".chk-subsidiary:checked").map((i, x) => `'${x.id.replace("chk-", "")}'`).toArray().join(),
        type = $("#export-type").data("kendoDropDownList").value(), validate = $("[name='search-by']:checked").val() === "A" ? "N" : "Y";
    
    window.location.href = `${urlGenerateFile}?${$.param({ Codes: ids, Subsidiaries: subsidiaries, ExportType: type, ValidateStock: validate }) }`;
}

//#endregion