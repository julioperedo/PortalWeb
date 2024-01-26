//#region Global Variables
var subsidiaries = [], clients = [], localUser, seeStock, notification;
const containerName = "#products-content", productTemplate = `# var productClass = isNew ? "new-product" : ""; #
    <div class="product" data-id="#=itemCode#" data-show-always="#=showAlways#">
        <div class="product-body">
            <div class="product-image">
                # if($.trim(imageURL) !== "") { #
                <img src="${urlImagesProduct}/#=imageURL#" loading="lazy" onerror="handleImageError(this);" />
                # } else { #
                <img src="${urlNoImage}" loading="lazy" />
                # } #
                # if (isNew) { # <img src="${urlNewImage}" class="new" loading="lazy" /> # } #
            </div>
            <div class="product-desc">
                <input id="productId" name="productId" type="hidden" value="#=id#">
                # if (localUser === "Y" && showTeamOnly) { #
                <div class="local-only">
                    <div>
                        # if ($.trim(link) !== "") { #
                        <a href="#=link#" target="_blank" class="product-code action-link"><strong>#=itemCode#</strong></a>
                        # } else { #
                        <b>#=itemCode#</b>
                        # } #
                        <span>(S&oacute;lo visible por el equipo DMC)</span>
                    </div>
                </div>
                # } else { #
                <div class="product-code">
                    # if ($.trim(link) != "") { #
                    <a href="#=link#" target="_blank" class="action-link"><strong>#=itemCode#</strong></a>
                    # } else { #
                    <b>#=itemCode#</b>
                    # } #
                </div>
                # } #
                <div class="product-name">#=name#</div>
                # if ($.trim(description) != "") { # <div>#=description#</div>	# } #
                # if ($.trim(extraComments) != "") { #	<div>#=extraComments#</div>	# } #
                # if ($.trim(commentaries) != "") { #
                <div class="product-comments">
                    <a class="action-link" data-toggle="collapse" href="\\#product-comments-#=id#" role="button" aria-expanded="false" aria-controls="product-comments-#=id#">Mostrar Comentarios</a>
                    <div id="product-comments-#=id#" class="collapse">#=commentaries#</div>
                </div>
                # } #
                # if ($.trim(consumables) !== "") { # <div>#=consumables#</div>	# } #
                # if ($.trim(warranty) !== "") { # <div>#=warranty#</div> # } #
            </div>
            <div class="product-prices">
                # for (var i = 0; i < prices.length; i++) { #
                #   var price = prices[i]; #
                #   var dataPrice = (price.regular > 0 || price.offer > 0 || $.trim(price.offerDescription) !== "") ? "Y" : "N"; #
                #   var fob = price.subsidiary.toLowerCase() !== "santa cruz" ? "FOB" : ""; #
                <div class="price #=price.subsidiary.toLowerCase().replace(' ', '-')#" data-price="#=dataPrice#" data-subsidiary="#=price.subsidiary#" data-digital="#=(isDigital ? 'Y' : 'N')#">
                    <b>#=prices[i].subsidiary#</b><br>
                    # if (price.offer > 0) { #
                    <div class="offer">Oferta: <span>#=fob# #=kendo.toString(price.offer, "N2")# $Us</span> 
                        #if(price.offerSince) {# <br />Desde: #=kendo.toString(JSON.toDate(price.offerSince), "dd-MM-yyyy")# #}#
                        #if(price.offerUntil) {# <br />Hasta: #=kendo.toString(JSON.toDate(price.offerUntil), "dd-MM-yyyy")# #}#
                    </div>
                    #   if ($.trim(price.offerDescription) !== "") { #
                    <div class="offer-desc">#=price.offerDescription#</div>
                    #   } #
                    # } #
                    # if (price.regular > 0) { #
                    #   var offerClass = price.offer > 0 ? "with-offer" : "", dataPrices = ''; #
                    #   for (var ipr = 0; ipr < price.otherPrices.length; ipr++) { #
                    #      dataPrices += 'data-' + price.otherPrices[ipr].name + '="' + price.otherPrices[ipr].price + '"'; #
                    #   } #
                    <div class="regular #=offerClass#" #=dataPrices#>#=fob# #=kendo.toString(price.regular, "N2")# $Us</div>
                    # } #
                    # if (localUser === "Y" && price.suggested > 0) { #
                    <div class="suggested">Precio sugerido: #=kendo.toString(price.suggested, "N2")# $Us</div>
                    # } #
                    # if (localUser === "Y" && price.volume && price.volume.length > 0) { #
                    #   for (var j = 0; j < price.volume.length; j++) { #
                    #       var vol = price.volume[j]; #
                    <div class="volume-price">
                        <p>
                            #=vol.quantity# items: #=vol.price# $Us
                            # if ($.trim(vol.observations) !== "") { #
                            <br />#=vol.observations#
                            # } #
                        </p>
                    </div>
                    #   } #
                    # } #
                    # if ($.trim(price.observations) !== "") { #
                    # var obsClass = price.regular <= 0 && !(price.offer && price.offer > 0) ? "highlighted" : "" #
                    <div class="observations #=obsClass#">#=price.observations#</div>
                    # } #
                    # if (localUser === "Y" && $.trim(price.commentaries) !== "") { #
                    <div class="commentaries"><span>Comentario Interno:</span> #=price.commentaries#</div>
                    # } #
                    <div class="stock">
                    </div>
                </div>
                # } #
            </div>
        </div>
        <div class="product-footer">
            <a class="add-cart" title="Agregar al carrito"><i class="fa fa-cart-plus"></i> <span>Agregar al carrito</span></a>
            <a class="detail-product" title="Vista Preliminar"><i class="fas fa-window-restore"></i></a>
            # var urlProduct = urlDetailProduct + "?Id=" + id; #
            <a title="Detalle completo del producto" target="_blank" href="#=urlProduct#"><i class="fas fa-external-link-alt"></i></a>
            <a class="add-requirement" title="Agregar requerimiento del producto"><i class="fas fa-plus-circle"></i></a>
        </div>
    </div>`;

//#endregion

//#region Events

$(() => setupControls());

$(document).ajaxError(catchErrors);

$(containerName).on("hide.bs.collapse show.bs.collapse", e => $(e.target).prev().find("i:last-child").toggleClass("fa-angle-up fa-angle-down"));

document.addEventListener('scroll', onScroll, true);

$("#LineList").on("click", ".line-inner", onClickInnerLine);

$("#LineList").on("click", ".submenu-item", onClickSubmenuItem);

$("[name='layouts']").on("change", onChangeLayout);

$('#filter-box').on('shown.bs.collapse', () => $(document).scrollTop(0));

$("#action-excel").click(onExportExcel);

$("#action-table").click(showTable);

$("#show-all").click(showHiddenContent);

$("#action-back").click(onClickBackButton);

$(containerName).on("click", ".product-comments a", e => e.target.innerHTML = e.target.innerHTML === "Mostrar Comentarios" ? "Ocultar Comentarios" : "Mostrar Comentarios");

$(containerName).on("click", ".detail-reserve", seeReservedDetail);

$(containerName).on("click", ".detail-product", seeProductDetail);

$(containerName).on("click", ".action-see-order", showProviderOrders);

$("#search-products").click(filterData);

$(containerName).on("click", ".add-cart", addToCart);

$(containerName).on("click", ".add-requirement", addRequirement);

$("#cart-additem").click(addItemToCart);

$("body").on("click", ".sale-order", showSaleOrder);

$("#save-request").click(onSavingRequest);

$(containerName).on("click", '[name="options"]', onListSelected);

$(containerName).on("click", ".see-clients", onShowingClients);

//#endregion

//#region Methods

function setupControls() {
    setTimeout(_ => $("#layouts .btn-info").first().addClass("active"), 600);
    localUser = $("#LocalUser").val();
    seeStock = $("#SeeStock").val();
    $("#Lines").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Línea ...", dataSource: { transport: { read: { url: urlGetLines } } } });
    $("#Category").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Categoría...", dataSource: { transport: { read: { url: urlGetCategories } } } });
    $("#SubCategory").kendoDropDownList({
        autoBind: false, cascadeFrom: "Category", dataTextField: "name", dataValueField: "id", enable: false, optionLabel: "Seleccione un Subcategoría...",
        dataSource: { transport: { read: { url: urlGetSubcategories, data: e => { return { CategoryId: $("#Category").val() }; } } }, serverFiltering: true }
    });
    $("#cart-subsidiary").kendoDropDownList({
        dataTextField: "name", dataValueField: "id",
        change: function (e) {
            var value = this.value();
            var data = JSON.parse($("#cart-pricedata").val());
            var selected = data.find(x => x.id === value);
            $("#cart-price").text(kendo.toString(selected.price, "N2"));
        }
    });
    $("#Detail").kendoWindow({ refresh: onRefreshWindow, scrollable: true, visible: false, width: 1180, actions: ["Close"], resizable: false, title: "Detalle Producto", modal: true });
    $("#CartDetail").kendoWindow({ refresh: onRefreshWindow, visible: false, width: 800, actions: ["Close"], resizable: false, title: "Agregar al carrito", modal: true });
    var ms = $("#ItemFilter").magicSuggest({ hideTrigger: true, placeholder: "Escriba su criterio de búsqueda, separado por comas si desea ingresar más de uno", minChars: 50, allowFreeEntries: true });
    $(ms).on('keydown', function (e, m, v) {
        if (v.keyCode == 13) filterData();
    });
    setToolbar("I");

    notification = $("#notification").kendoNotification({
        position: { pinned: true, top: 60, right: 30 },
        autoHideAfter: 10000,
        stacking: "down",
        templates: [{ type: "error", template: $("#errorTemplate").html() }, { type: "success", template: $("#successTemplate").html() }]
    }).data("kendoNotification");

    $("#Report").kendoWindow({
        visible: false, width: 1100, title: "", modal: true, activate: function (e) {
            var wnd = this;
            setTimeout(() => wnd.center(), 300);
        }
    });

    //Setea controles del formulario para agregar slicitudes de productos
    $("#RequestDetail").kendoWindow({
        title: "Agregar Solicitud de Producto", visible: false, scrollable: true, modal: true, width: 750, iframe: false,
        activate: function (e) {
            var wnd = this;
            setTimeout(() => {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });

    var ddlSubsidiaries = $("#IdSubsidiary").kendoDropDownList({ dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione una Sucursal...", filter: "contains" }).data("kendoDropDownList"),
        ddlClients = $("#CardCode").kendoDropDownList({
            dataTextField: "name", dataValueField: "code", optionLabel: "Seleccione un Cliente...", filter: "contains",
            dataSource: { data: [] }, virtual: {
                itemHeight: 45, valueMapper: function (options) {
                    var items = clients || [];
                    var index = items.findIndex(x => x.code === options.value);
                    options.success(index);
                }
            },
            template: '<span class="k-state-default">#: data.name.replace(data.code + " - ", "") #<p>#: data.code #</p></span>'
        }).data("kendoDropDownList");

    //Llamadas de datos
    $.get(urlGetSubsidiaries, {}, data => {
        if (data.message === "") {
            var /*select = $("#Subsidiary"),*/ select2 = $("#AvailableIn");
            subsidiaries = data.items;
            data.items.forEach(x => {
                //select.append(new Option(x.name, x.name));
                select2.append(new Option(x.name, x.name));
            });
            //select.multipleSelect({ selectAll: false });
            select2.multipleSelect({
                selectAll: false, width: 155, allSelected: "Todos", placeholder: "Disponible en",
                onClick: function (e) {
                    var value = e.value.toLowerCase().split(" ").join("-");
                    $("#products-content").toggleClass("available-" + value);
                }
            });
            setToolbar("I");
            if (localUser === "Y") ddlSubsidiaries.setDataSource(data.items);
        } else {
            showError(`Se ha producido el siguiente error al traer los datos del servidor: <br /> ${data.message}`);
        }
    });
    $.get(urlGetAvailableLines, {}, data => {
        if (data.message === "") {
            $("#LineList").kendoListView({ dataSource: data.items, template: kendo.template($("#templateLine").html()) });
        } else {
            showError(`Se ha producido el siguiente error al traer los datos del servidor: <br /> ${data.message}`);
        }
    });
    if (localUser === "Y") {
        $.get(urlClients, {}, d => ddlClients.setDataSource(d));
    }

    $("#products-content").toggleClass("with-requirement", $("#ProductRequirements").val() === "Y");
}

function filterData() {
    var intLineId = $("#Lines").data("kendoDropDownList").value();
    var ms = $("#ItemFilter").magicSuggest();
    $("#ItemFilter").blur();
    var category = $("#Category").data("kendoDropDownList"), subcategory = $("#SubCategory").data("kendoDropDownList");
    var strFilter = ms.getValue().join().replaceAll("'", ""), strCategory = category.value() !== "" ? category.text() : "", strSubCategory = subcategory.value() !== "" ? subcategory.text() : "", strAvailable = ""/*$("#Subsidiary").multipleSelect("getSelects").join()*/;
    $('#filter-box').collapse("hide");
    if (intLineId !== "" | strCategory != "" | strFilter !== "") {
        loadLineContent(intLineId, strCategory, strSubCategory, strFilter, strAvailable);
        cleanFilters();
    } else {
        $("#divContent").html("<p>Debe seleccionar al menos un criterio de b&uacute;squeda aparte del Stock.</p>");
    }
}

function loadLineContent(idLine, category, subcategory, filter, available) {
    $("#show-all").find("i").removeClass("far fa-square").addClass("fas fa-check-square");
    $("#show-all").attr("data-selected", "N");

    $("#LastSearch").val(JSON.stringify({ IdLine: idLine, Category: category, Subcategory: subcategory, Filter: filter, /*Available: available,*/ OpenBox: false }));
    var data = {};
    if (idLine && idLine != "-1") {
        data.idLine = idLine;
    }
    if (category) {
        data.Category = category;
    }
    if (subcategory) {
        data.Subcategory = subcategory;
    }
    if (filter) {
        data.Filter = filter;
    }
    data._ = new Date();
    var localUser = $("#LocalUser").val() === "Y", seeStock = $("#SeeStock").val() === "Y", seeProviderOrders = $("#SeeProviderOrders").val() === "Y";
    $(containerName).empty();
    $.get(urlLine, data, data => {
        if (data.message === "") {
            if (data.items && data.items.length > 0) {
                data.items.forEach(x => {
                    var card = $("<div>").addClass("card");
                    $(containerName).append(card);
                    card.append(`<div class="card-header" id="headingLine-${x.id}"><h5 class="mb-0"><button class="d-flex align-items-center justify-content-between btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#contentLine-${x.id}" aria-expanded="false" aria-controls="contentLine-${x.id}">${x.name}<i class="fas fa-angle-up"></i></button></h5></div>`);

                    var lineContent = $("<div>").addClass("col"), lineImage = "", classLabel = "", lineHeader, managerContent = "";
                    if ($.trim(x.imageURL) != "") {
                        lineImage = `<img src="${urlImagesLine}${x.imageURL}" class="d-none d-sm-inline" />`;
                        classLabel = "d-sm-none";
                    }
                    if (x.manager) {
                        managerContent += `<img src="../images/staff/${x.manager.photo}" class="d-none d-sm-inline float-right" /><div class="float-right"><span class="font-weight-bold">${x.manager.name}</span><br /><span class="font-italic">${x.manager.position}</span><br /><a href="mailto:${x.manager.email}" class="action-link">${x.manager.email}</a><br /><span>Interno: ${x.manager.phone}</span></div>`;
                    }
                    var usedGroups = '';
                    if (x.usedGroups.length > 0) {
                        usedGroups += '<div class="col-12 text-right mb-2"><div class="btn-group btn-group-toggle" data-toggle="buttons">';
                        x.usedGroups.forEach(u => usedGroups += `<label class="btn btn-warning ${(u == "Portal" ? "active" : "")}"><input type="radio" name="options" id="op-${u}"> ${u}</label>`);
                        usedGroups += '</div>&nbsp;&nbsp;&nbsp;<a class="see-clients action action-link">Ver clientes</a></div>';
                    }
                    lineHeader = `<div class="row line-header" data-idline="${x.id}">
                        <div class="col-12 col-sm-4 col-md-3 col-lg-2">${lineImage}<br class="d-sm-none" /><br class="d-sm-none" /><span class="${classLabel}">${x.name}</span></div><div class="d-none d-sm-inline-block col-sm-4 col-md-4 col-lg-5"><p>${x.description}</p><p>${x.header}</p></div><div class="d-none d-sm-inline-block col-sm-4 col-md-5 col-lg-5 text-right">${managerContent}</div>
                        ${usedGroups}                        
                    </div>`;
                    lineContent.append(lineHeader);

                    var lineBody = $("<div>").addClass("col");
                    lineContent.append($("<div>").addClass("line-body").addClass("row").append(lineBody));

                    if (x.categories && x.categories.length > 0) {
                        x.categories.forEach(cat => {
                            var cardCat = $("<div>").addClass("card");
                            lineBody.append(cardCat);
                            cardCat.append(`<div class="card-header" id="headingCategory-${x.id}-${cat.id}"><h5 class="mb-0"><button class="d-flex align-items-center justify-content-between btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#contentCategory-${x.id}-${cat.id}" aria-expanded="false" aria-controls="contentCategory-${x.id}-${cat.id}">${cat.name}<i class="fas fa-angle-up"></i></button></h5></div>`);

                            var catContent = $("<div>").addClass("col");

                            if (cat.subcategories && cat.subcategories.length > 0) {
                                cat.subcategories.forEach(sub => {
                                    var cardSub = $("<div>").addClass("card");
                                    catContent.append(cardSub);
                                    cardSub.append(`<div class="card-header" id="headingSubcategory-${x.id}-${cat.id}-${sub.id}"><h5 class="mb-0"><button class="d-flex align-items-center justify-content-between btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#contentSubcategory-${x.id}-${cat.id}-${sub.id}" aria-expanded="false" aria-controls="contentSubcategory-${x.id}-${cat.id}-${sub.id}">${sub.name}<i class="fas fa-angle-up"></i></button></h5></div>`);

                                    var subContent = $("<div>").addClass("col").addClass("subcategory"), template = kendo.template(productTemplate); //kendo.template($("#product-template").html());

                                    var productCodes = [];
                                    if (sub.products && sub.products.length > 0) {
                                        sub.products.forEach(product => {
                                            $(subContent).append(template(product));
                                            productCodes.push(`'${product.itemCode}'`);
                                        });
                                    }

                                    cardSub.append($("<div>").addClass("collapse").addClass("show").attr("id", `contentSubcategory-${x.id}-${cat.id}-${sub.id}`).attr("aria-labelledby", `headingSubcategory-${x.id}-${cat.id}-${sub.id}`).append($("<div>").addClass("card-body").append($("<div>").addClass("row").append(subContent))));

                                    var tempCodes = [];
                                    while (productCodes.length > 0) {
                                        if (productCodes.length > 50) {
                                            tempCodes = productCodes.slice(0, 50);
                                            productCodes = productCodes.slice(50, productCodes.length);
                                        } else {
                                            tempCodes = productCodes;
                                            productCodes = [];
                                        }
                                        $.get(urlGetStock, { ItemCodes: tempCodes.join(), _: new Date() }, d => {
                                            if (d.message === "") {
                                                d.items.forEach(item => {
                                                    var obj = $(`[data-id="${item.itemCode}"]`);
                                                    if (obj.length > 0) {
                                                        obj.find(".product-prices .price").each(function () {
                                                            var ids = this.dataset;
                                                            if (localUser || ids.price === "Y") {
                                                                if (item.subsidiaries && item.subsidiaries.length > 0) {
                                                                    var subsidiary = item.subsidiaries.find(e => e.name === ids.subsidiary);
                                                                    if (subsidiary) {
                                                                        var classStock = subsidiary.name === "Iquique" ? "primary" : "", classAvailable = subsidiary.name !== "Iquique" ? "primary" : "", stockTotal = 0;
                                                                        var stockContent = $(this).find(".stock");
                                                                        if (subsidiary.items && subsidiary.items.length > 0) {
                                                                            var columnName = "$.stock"; //subsidiary.name === "Iquique" ? "$.stock" : "$.available2";
                                                                            stockTotal = Enumerable.From(subsidiary.items).Select(columnName).Sum();
                                                                            if (stockTotal < 1) {
                                                                                $(this).find(".offer, .offer-desc").remove();
                                                                                $(this).find(".regular").removeClass("with-offer");
                                                                            } else {
                                                                                obj.addClass(`available-${subsidiary.name.toLowerCase().replace(' ', '-')}`);
                                                                            }
                                                                            var stockDetail = "";
                                                                            subsidiary.items.forEach(warehouse => {
                                                                                if (localUser || seeStock) {
                                                                                    var reservedClass = warehouse.reserved > 0 && localUser ? "detail-reserve action" : "", notForClientsClass = warehouse.clientVisible ? "" : `class="not-for-clients"`;
                                                                                    if (localUser) {
                                                                                        var arrivalDate = warehouse.arrivalDate != null ? `<br />(Fecha estimada de arribo: ${kendo.toString(JSON.toDate(warehouse.arrivalDate), "dd-MM-yyyy")})` : "";
                                                                                        stockDetail += `<tr ${notForClientsClass}><td>${warehouse.warehouse}${arrivalDate}</td><td class="text-right ${classStock}">${warehouse.stock}</td><td class="text-right ${reservedClass}">${warehouse.reserved}</td><td class="text-right ${classAvailable}">${warehouse.available2}</td><td class="text-right ${(seeProviderOrders && warehouse.requested > 0 ? "action action-see-order" : "")}">${warehouse.requested}</td></tr>`;
                                                                                    } else {
                                                                                        if (warehouse.clientVisible) {
                                                                                            stockDetail += `<tr ${notForClientsClass}><td>${warehouse.warehouse}</td><td class="text-right ${classAvailable}">${warehouse.available2}</td></tr>`;
                                                                                        }                                                                                        
                                                                                    }
                                                                                } else {
                                                                                    if (warehouse.clientVisible) {
                                                                                        var percentage = warehouse.percentage != null ? `${kendo.toString(warehouse.percentage, "N0")}` : `n/a`;
                                                                                        stockDetail += `<tr><td>${warehouse.warehouse}</td><td class="text-right">${percentage}</td></tr>`;
                                                                                    } else {
                                                                                        if (warehouse.arrivalDate != null) {
                                                                                            stockDetail += `<tr><td colspan="2">Fecha estimada de arribo: ${kendo.toString(JSON.toDate(warehouse.arrivalDate), "dd-MM-yyyy")}</td</tr>`;
                                                                                        }                                                                                        
                                                                                    }                                                                                    
                                                                                }
                                                                            });
                                                                            if (localUser || seeStock) {
                                                                                if (localUser) {
                                                                                    stockContent.append(`<div class="stock-detail"><table style="width: 100%;"><tr><td></td><td class="text-right ${classStock}">Stock</td><td class="text-right">Reserva</td><td class="text-right ${classAvailable}">Disponible</td><td class="text-right">Pedido</td></tr>${stockDetail}</table></div>`);
                                                                                } else {
                                                                                    stockContent.append(`<div class="stock-detail"><table style="width: 100%;"><tr><td></td><td class="text-right ${classAvailable}">Disponible</td></tr>${stockDetail}</table></div>`);
                                                                                }
                                                                            } else {
                                                                                stockContent.append(`<div class="stock-detail"><table style="width: 100%;"><tr><td></td><td class="text-right">Stock</td></tr>${stockDetail}</table></div>`);
                                                                            }
                                                                        } else {
                                                                            $(this).find(".offer, .offer-desc").remove()
                                                                            $(this).find(".regular").removeClass("with-offer");
                                                                            if (ids.price === "N") {
                                                                                $(this).remove();
                                                                            } else {
                                                                                $(this).find(".stock").append(ids.digital === "Y" ? 'Stock Disponible' : `Sin Stock`);
                                                                            }
                                                                        }
                                                                    } else {
                                                                        $(this).find(".offer, .offer-desc").remove();
                                                                        $(this).find(".regular").removeClass("with-offer");
                                                                        if (ids.price === "N") {
                                                                            $(this).remove();
                                                                        } else {
                                                                            $(this).find(".stock").append(ids.digital === "Y" ? 'Stock Disponible' : `Sin Stock`);
                                                                        }
                                                                    }
                                                                } else {
                                                                    if (ids.price === "N") {
                                                                        $(this).remove();
                                                                    } else {
                                                                        $(this).find(".offer, .offer-desc").remove();
                                                                        $(this).find(".regular").removeClass("with-offer");
                                                                        $(this).find(".stock").append(ids.digital === "Y" ? 'Stock Disponible' : `Sin Stock`);
                                                                    }
                                                                }
                                                            } else {
                                                                $(this).remove();
                                                            }
                                                        });
                                                        var removeCards = function (div, className) {
                                                            if (div.find(`${className}.offer`).length === 0) {
                                                                var subcategory = div.closest(".card");
                                                                div.remove();
                                                                if (subcategory.find(".product").length === 0) {
                                                                    var category = subcategory.parent().closest(".card");
                                                                    subcategory.remove();
                                                                    if (category.find(".product").length === 0) {
                                                                        var line = category.parent().closest(".card");
                                                                        category.remove();
                                                                        if (line.find(".product").length === 0) {
                                                                            line.remove();
                                                                            var xxx = "Ahora si Rosita trae ese culito";
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (idLine === "-10") {
                                                            removeCards(obj, "");
                                                        }
                                                        if (idLine === "-11") {
                                                            removeCards(obj, ".santa-cruz ");
                                                        }
                                                        if (idLine === "-12") {
                                                            removeCards(obj, ".miami ");
                                                        }
                                                        if (idLine === "-13") {
                                                            removeCards(obj, ".iquique ");
                                                        }
                                                    }
                                                });
                                                if (productCodes.length === 0) {
                                                    setTimeout(() => setProductHeight(), 600);
                                                }
                                            } else {
                                                notification.show({ title: "Error de conección", message: "No se puede llegar al servidor remoto." }, "error");
                                            }
                                        });
                                    }
                                });
                            }
                            cardCat.append($("<div>").addClass("collapse").addClass("show").attr("id", `contentCategory-${x.id}-${cat.id}`).attr("aria-labelledby", `headingCategory-${x.id}-${cat.id}`).append($("<div>").addClass("card-body").append($("<div>").addClass("row").append(catContent))));
                        });
                    }

                    if ($.trim(x.footer) !== "") {
                        lineContent.append(`<div class="row line-footer"><div class="col">${x.footer}</div></div>`)
                    }

                    card.append($("<div>").addClass("collapse").addClass("show").attr("id", `contentLine-${x.id}`).attr("aria-labelledby", `headingLine-${x.id}`).append($("<div>").addClass("card-body").append($("<div>").addClass("row").append(lineContent))));
                });
                setToolbar("P");
                setTimeout(() => setProductHeight(), 500);
            } else {
                setToolbar("I");
                $(containerName).append(`<div class="no-items">No se encontraron resultados para el criterio de b&uacute;squeda.</div>`);
            }
        } else {
            setToolbar("I");
            $(containerName).append(`<div class="no-items">No se encontraron resultados para el criterio de b&uacute;squeda.</div>`);
            showError(`Se ha producido el siguiente error al traer los datos del servidor: <br /> ${data.message}`);
        }
    });
}

function loadOpenBoxes() {
    var template = kendo.template(`<div class="product open-box">
                                       <div class="product-body">
                                           <div class="product-image">
                                               <img src="#=imageUrl#" />
                                           </div>
                                           <div class="product-desc">
                                               <input id="productId" name="productId" type="hidden" value="#=id#">
                                               <div class="product-code">
                                                   # if ($.trim(link) !== "") { #
                                                   <a href="#=link#" target="_blank" class="action-link"><strong>#=itemCode#</strong></a>
                                                   # } else { #
                                                   <strong>#=itemCode#</strong>
                                                   # } #
                                               </div>
                                               <div class="product-name">#=productName#</div>
                                               # if ($.trim(productComments) !== "") { #
                                               <div class="product-comments">
                                                   <a class="action-link" data-toggle="collapse" href="\\\\\#product-comments-#=id#" role="button" aria-expanded="false" aria-controls="product-comments-#=id#">Mostrar Comentarios</a>
                                                   <div id="product-comments-#=id#" class="collapse">#=productComments#</div>
                                               </div>
                                               # } #
                                               <div>#=comments#</div>
                                           </div>
                                           <div class="product-prices">
                                               <div class="#=subsidiary.toLowerCase().replace(' ', '-')#">
                                                   <b>#=subsidiary#</b><br>
                                                   Precio: <b><span class="price">#=kendo.toString(price,"N2")#</span> $us</b><br>
                                                   Unidades disponibles: <span class="quantity">#=quantity#</span>
                                               </div>
                                           </div>
                                       </div>
                                       <div class="product-footer">
                                           <a class="add-cart" title="Agregar al carrito"><i class="fa fa-cart-plus"></i> <span>Agregar al carrito</span></a>
                                       </div>
                                   </div>`);
    $("#LastSearch").val(JSON.stringify({ IdLine: 0, Category: "", Subcategory: "", Filter: "", Available: "", OpenBox: true }));
    $(containerName).empty();
    $.get(urlOpenBoxes, {}, data => {
        if (data.message === "") {
            data.items.forEach(x => {
                x.imageUrl = $.trim(x.imageUrl) === "" ? urlNoImage : `${urlImagesProduct}/${x.imageUrl}`;
                $(containerName).append(template(x));
                $(containerName).scrollTop(0);
            });
            $(".product-comments a").click(e => e.target.text = e.target.text === "Mostrar Comentarios" ? "Ocultar Comentarios" : "Mostrar Comentarios");
            setToolbar("OP");
            setTimeout(() => setProductHeight(), 300);
        } else {
            showError(`Se ha producido el siguiente error al traer los datos del servidor: <br /> ${data.message}`);
            setToolbar("I");
        }
    });
}

function cleanFilters() {
    $("#Lines").getKendoDropDownList().value("");
    $("#Category").getKendoDropDownList().value("");
    //$("#Subsidiary").multipleSelect("uncheckAll");
    var ms = $("#ItemFilter").magicSuggest();
    ms.clear();
    ms.collapse();
}

function setToolbar(type) {
    $("#action-excel, #action-table").toggleClass("d-none", type === "I" || type === "OP");
    $(".ms-parent.InToolbox").toggleClass("d-none", type === "I" || type === "OP");
    $("#show-all").toggleClass("d-none", type === "I" || type === "OP");
    $("#action-filters").toggleClass("d-none", type !== "I");
    $("#action-back").toggleClass("d-none", type === "I");
}

function setProductHeight() {
    var subcategories = $(".subcategory");
    if (subcategories.length > 0) {
        subcategories.each(function () {
            var maxHeight = 0;
            $(this).find(".product-body").each(function () {
                $(this).find(".product-desc").css("min-height", "");
                var height = +$(this).outerHeight().toFixed(0) + 1;
                maxHeight = height > maxHeight ? height : maxHeight;
            });
            if (!$(containerName).hasClass("old-list")) {
                $(this).find(".product-body").each(function () {
                    var height = $(this).outerHeight();
                    var imgHeight = $(this).find(".product-image").outerHeight();
                    var descHeight = $(this).find(".product-desc").outerHeight();
                    var innerHeight = $("#products-content").hasClass("list") ? (imgHeight > descHeight ? imgHeight : descHeight) : descHeight;
                    $(this).find(".product-desc").css("min-height", Math.round((innerHeight + maxHeight - height) * 100) / 100);
                });
            }
        });
    } else {
        var maxHeight = 0;
        $(".product.open-box .product-body").each(function () {
            $(this).find(".product-desc").css("min-height", "");
            var height = +$(this).outerHeight().toFixed(0) + 1;
            maxHeight = height > maxHeight ? height : maxHeight;
        });
        $(".product.open-box .product-body").each(function () {
            var height = $(this).outerHeight();
            var imgHeight = $(this).find(".product-image").outerHeight();
            var descHeight = $(this).find(".product-desc").outerHeight();
            var innerHeight = $("#products-content").hasClass("list") ? (imgHeight > descHeight ? imgHeight : descHeight) : descHeight;
            $(this).find(".product-desc").css("min-height", Math.round((innerHeight + maxHeight - height) * 100) / 100);
        });
    }
}

function loadReport(Id, Subsidiary, Report) {
    var objParams = { Subsidiary: Subsidiary, DocNumber: Id, User: $.trim($(".user-info > .user-name").first().text()) }, strReport = "SaleOrder.trdp";
    if (Report === "Note") {
        strReport = "SaleNote.trdp";
    }
    if (Report === "Delivery") {
        strReport = "DeliveryNote.trdp";
        objParams.SearchType = 1;
    }
    if (Report === "Bill") {
        strReport = "Bill.trdp";
    }
    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource({ report: strReport, parameters: objParams });
            viewer.refreshReport();
        } catch (e) {
            console.error(`Error al recargar el reporte: ${e}`);
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        try {
            $("#reportViewer1").telerik_ReportViewer({
                serviceUrl: urlService,
                reportSource: { report: strReport, parameters: objParams },
                viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
                scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
            });
        } catch (e) {
            console.error(`Error al cargar el reporte: ${e}`);
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    }
}

function catchErrors(event, jqxhr, settings, thrownError) {
    if (jqxhr.responseText) {
        if (jqxhr.responseText.includes("Document/refresh with ID") || jqxhr.responseText.includes("not found. Expired")) {
            var viewer = $("#reportViewer1").data("telerik_ReportViewer");
            if (viewer) viewer.refreshReport();
        }
    }
}

function onScroll(event) {
    var scrollDistance = $(event.target).scrollTop();
    if (scrollDistance > 51) {
        $(".tool-box").addClass("fixed");
    } else {
        $(".tool-box").removeClass("fixed");
    }
}

function onChangeLayout(e) {
    $(containerName).removeClass("grid").removeClass("list").removeClass("old-list").addClass($(e.target).parent().find("input").val());
    setProductHeight();
}

function onClickInnerLine(e) {
    var intID = $(this).find(".lineId").val(), intWContainer = $(this).closest(".k-listview").width(), parent = $(this).parent(), intW = parent.width(), intCols = Math.floor(intWContainer / intW),
        submenu = parent.find(".submenu"), isHidden = submenu.hasClass("d-none"), intH = "auto";
    $(".submenu").addClass("d-none");
    $(".k-listview .line").height("auto");
    if (submenu.length > 0) {
        if (!isHidden) {
            submenu.addClass("d-none");
        } else {
            submenu.removeClass("d-none");
            intH = $(this).parent().height();
        }
    } else {
        if (intID == "-1") {
            //var wnd = $("#Detail").data("kendoWindow");
            var wnd = $("<div>").kendoWindow({ activate: onRefreshWindow, scrollable: true, visible: false, width: 1180, actions: ["Close"], resizable: false, title: "Staff", modal: true }).data("kendoWindow");
            var container = $("<div>");
            var dptoTempl = kendo.template($("#department-template").html());
            $.get(urlStaff, {}, data => {
                data.departments.forEach(d => {
                    d.managers.forEach(m => { m.photo = urlImagesStaff + m.photo; });
                    d.members.forEach(m => { m.photo = urlImagesStaff + m.photo; });
                    container.append(dptoTempl(d));
                });
                wnd.content(container);
                setTimeout(() => wnd.open().center(), 300);
            });
        } else if (intID == "-3") {
            $('#filter-box').collapse("hide");
            loadOpenBoxes();
            cleanFilters();
        } else {
            // Hides filter panel
            $('#filter-box').collapse("hide");
            loadLineContent(intID, null, null, "", "");
            cleanFilters();
        }
    }
    var lstLines = $(".k-listview .line");
    var intPos = lstLines.index(parent);
    var intRow = Math.floor(intPos / intCols);
    $.each(lstLines, function (i, obj) {
        var intCurrent = Math.floor(i / intCols);
        if (intRow == intCurrent) {
            $(obj).height(intH);
        } else {
            $(obj).height("auto");
        }
    });
}

function onClickSubmenuItem(e) {
    // Hides filter panel
    $('#filter-box').collapse("hide");
    loadLineContent($(this).attr("id"), null, null, "", "");
    cleanFilters();
}

function onExportExcel(e) {
    e.preventDefault();
    var items = [];
    $.each($(".product:visible"), (i, o) => {
        if ($(o).find(".local-only").length === 0) {
            items.push($(o).find("#productId").val());
        }
    });
    if (items.length > 0) {
        var ids = items.join(",");
        var exportGrouped = function (e) {
            var withDetail = $("#export-detail").prop("checked"), withTransit = $("#export-transit").prop("checked");
            window.location.href = urlExportExcel + "?" + $.param({ Ids: ids, WithDetail: withDetail, WithTransit: withTransit });
        }
        var message = `<p>¿Desea exportar con la descripci&oacute;n de los productos?</p>
<input type="checkbox" id="export-detail" /><label for="export-detail" class="control-label">&nbsp;Exportar con detalle</label><br />
${(localUser === 'Y' ? `<input type="checkbox" id="export-transit" /><label for="export-transit" class="control-label">&nbsp;Exportar con tr&aacute;nsito</label>` : '')}`;
        showConfirm(message, exportGrouped);
    }
}

function showTable(e) {
    e.preventDefault();
    var items = [];
    $.each($(".product:visible"), (i, o) => {
        items.push($(o).find("#productId").val());
    });
    if (items.length > 0) {
        var ids = items.join(",");
        $.get(urlGetItems, { Ids: ids }, data => {
            if (data.message === "") {
                if (data.lines.length > 0) {
                    var table = $("<table>").addClass("copy-table");
                    data.lines.forEach(l => {
                        table.append(`<tr class="line-header"><td colspan="9">${l.name}</td></tr>`);
                        l.categories.forEach(c => {
                            table.append(`<tr class="category-header"><td colspan="9">${c.name}</td></tr>`);
                            c.subcategories.forEach(s => {
                                table.append(`<tr class="subcategory-header"><td colspan="9">${s.name}</td></tr>`);
                                table.append(`<tr class="item-header"><td rowspan="2">Item</td><td rowspan="2">Nombre</td><td rowspan="2">Garantía</td><td colspan="2" class="text-center">Santa Cruz</td><td colspan="2" class="text-center">Iquique</td><td colspan="2" class="text-center">Miami</td></tr>`);
                                table.append(`<tr class="item-header"><td class="text-right text-nowrap">Precio ($US)</td><td class="text-right">Stock<td class="text-right text-nowrap">Precio ($US)</td><td class="text-right">Stock<td class="text-right text-nowrap">Precio ($US)</td><td class="text-right">Stock</tr>`);
                                s.products.forEach(p => {
                                    var priceSC = "", priceIQ = "", priceMI = "", stockSC = "", stockIQ = "", stockMI = "", price, stockItems;
                                    price = p.prices.find(x => x.subsidiary.toLowerCase() === "santa cruz");
                                    if (price) {
                                        if (price.offer > 0) {
                                            priceSC += `<span class="offer">${kendo.toString(price.offer, "N2")}</span><span class="regular with-offer">${kendo.toString(price.regular, "N2")}</span>`;
                                        } else {
                                            if (price.regular > 0) priceSC += kendo.toString(price.regular, "N2");
                                        }
                                        if (price.volume && price.volume.length > 0) {
                                            priceSC += `<br /><table><thead><tr><td class="text-right">Cantidad</td><td class="text-right">Precio</td></tr></thead>`;
                                            price.volume.forEach(v => priceSC += `<tr><td class="text-right">${v.quantity}</td><td class="text-right">${kendo.toString(v.price, "N2")}</td></tr>`);
                                            priceSC += `</table>`;
                                        }
                                    }
                                    price = p.prices.find(x => x.subsidiary.toLowerCase() === "iquique");
                                    if (price) {
                                        if (price.offer > 0) {
                                            priceIQ += `<span class="offer">${kendo.toString(price.offer, "N2")}</span><span class="regular with-offer">${kendo.toString(price.regular, "N2")}</span>`;
                                        } else {
                                            if (price.regular > 0) priceIQ += kendo.toString(price.regular, "N2");

                                        }
                                        if (price.volume && price.volume.length > 0) {
                                            priceIQ += `<br /><table><thead><tr><td class="text-right">Cantidad</td><td class="text-right">Precio</td></tr></thead>`;
                                            price.volume.forEach(v => priceIQ += `<tr><td class="text-right">${v.quantity}</td><td class="text-right">${kendo.toString(v.price, "N2")}</td></tr>`);
                                            priceIQ += `</table>`;
                                        }
                                    }
                                    price = p.prices.find(x => x.subsidiary.toLowerCase() === "miami");
                                    if (price) {
                                        if (price.offer > 0) {
                                            priceMI += `<span class="offer">${kendo.toString(price.offer, "N2")}</span><span class="regular with-offer">${kendo.toString(price.regular, "N2")}</span>`;
                                        } else {
                                            if (price.regular > 0) priceMI += kendo.toString(price.regular, "N2");
                                        }
                                        if (price.volume && price.volume.length > 0) {
                                            priceMI += `<br /><table><thead><tr><td class="text-right">Cantidad</td><td class="text-right">Precio</td></tr></thead>`;
                                            price.volume.forEach(v => priceMI += `<tr><td class="text-right">${v.quantity}</td><td class="text-right">${kendo.toString(v.price, "N2")}</td></tr>`);
                                            priceMI += `</table>`;
                                        }
                                    }
                                    stockItems = data.stockItems.filter(x => x.itemCode === p.itemCode & x.subsidiary.toLowerCase() === "santa cruz");
                                    if (stockItems && stockItems.length > 0) {
                                        stockSC += `<table><thead><tr><td></td><td class="text-right">Stock</td><td class="text-right">Reserva</td><td class="text-right">Disponible</td><td class="text-right">Tránsito</td></tr></thead>`;
                                        stockItems.forEach(s => {
                                            stockSC += `<tr><td>${s.warehouse}</td><td class="text-right">${s.stock}</td><td class="text-right">${s.reserved}</td><td class="text-right">${s.available2}</td><td class="text-right">${s.requested}</td></tr>`;
                                        });
                                        stockSC += "</table>";
                                    }
                                    stockItems = data.stockItems.filter(x => x.itemCode === p.itemCode & x.subsidiary.toLowerCase() === "iquique");
                                    if (stockItems && stockItems.length > 0) {
                                        stockIQ += `<table><thead><tr><td></td><td class="text-right">Stock</td><td class="text-right">Reserva</td><td class="text-right">Disponible</td><td class="text-right">Tránsito</td></tr></thead>`;
                                        stockItems.forEach(s => {
                                            stockIQ += `<tr><td>${s.warehouse}</td><td class="text-right">${s.stock}</td><td class="text-right">${s.reserved}</td><td class="text-right">${s.available2}</td><td class="text-right">${s.requested}</td></tr>`;
                                        });
                                        stockIQ += "</table>";
                                    }
                                    stockItems = data.stockItems.filter(x => x.itemCode === p.itemCode & x.subsidiary.toLowerCase() === "miami");
                                    if (stockItems && stockItems.length > 0) {
                                        stockMI += `<table><thead><tr><td></td><td class="text-right">Stock</td><td class="text-right">Reserva</td><td class="text-right">Disponible</td><td class="text-right">Tránsito</td></tr></thead>`;
                                        stockItems.forEach(s => {
                                            stockMI += `<tr><td>${s.warehouse}</td><td class="text-right">${s.stock}</td><td class="text-right">${s.reserved}</td><td class="text-right">${s.available2}</td><td class="text-right">${s.requested}</td></tr>`;
                                        });
                                        stockMI += "</table>";
                                    }
                                    table.append(`<tr><td>${p.itemCode}</td><td>${p.name}</td><td>${p.warranty}</td><td class="text-right">${priceSC}</td><td class="text-right">${stockSC}</td><td class="text-right">${priceIQ}</td><td class="text-right">${stockIQ}</td><td class="text-right">${priceMI}</td><td class="text-right">${stockMI}</td></tr>`);
                                });
                            });
                        });
                    });

                    $("<div>").append(table).kendoWindow({
                        visible: true,
                        scrollable: true, modal: true, width: 1400, iframe: false,
                        activate: function (e) {
                            var wnd = this;
                            setTimeout(() => {
                                onRefreshWindow(e);
                                wnd.center();
                            }, 300);
                        }
                    });
                }
            } else {
                console.error(`Se he producido el siguiente error al traer los datos del servidor: ${data.message}`);
                showError("Se he producido el siguiente error al traer los datos del servidor");
            }
        });
    }
}

function showHiddenContent(e) {
    e.preventDefault();
    $(this).find("i").toggleClass("fas fa-check-square far fa-square");
    $(this).attr("data-selected", $(this).attr("data-selected") === "Y" ? "N" : "Y");
    $(".volume-price, .commentaries").toggleClass("d-none");
    setProductHeight();
}

function onClickBackButton(e) {
    $(containerName).empty();
    setToolbar("I");
}

function seeReservedDetail(e) {
    var value = +this.textContent;
    if (value > 0 && localUser === "Y") {
        var itemCode = $.trim($(this).closest(".product").find(".product-code").text());
        var subsidiary = $(this).closest(".price").data("subsidiary");
        var warehouse = $.trim($(this).closest("tr").find("td").first().text());

        $.get(urlReservedItems, { Subsidiary: subsidiary, Warehouse: warehouse, ItemCode: itemCode }, function (data) {
            if (data.message === "") {
                var extraTitle = subsidiary.toLowerCase() === "iquique" ? "<th>Autorizado</th><th>Correlativo</th>" : "";
                var content = `<table class="table"><thead class="thead-light"><tr><th scope="col">Ejecutivo</th><th scope="col">Cliente</th><th scope="col">Orden</th><th scope="col">Fecha</th><th scope="col" class="text-right">Cantidad</th><th scope="col" class="text-right">Precio</th>${extraTitle}<th>&nbsp;</th></thead><tbody>`;
                $.each(data.items, function (i, obj) {
                    var extraData = subsidiary.toLowerCase() === "iquique" ? `<td class="text-center">${obj.authorized === "Y" ? '<i class="fas fa-check"></i>' : ''}</td><td>${obj.correlative}</td>` : "";
                    content += `<tr><td>${obj.sellerName}</td><td><strong>${obj.clientCode}</strong> - ${obj.clientName}</td><td><a class="sale-order action action-link" data-subsidiary="${obj.subsidiary}" data-number="${obj.docNum}">${obj.docNum}</a></td><td>${kendo.toString(JSON.toDate(obj.docDate), "dd-MM-yyyy")}</td><td class="text-right">${obj.quantity}</td><td class="text-right">${obj.price !== null ? kendo.toString(obj.price, "N2") : ""}</td>${extraData}<td>`;
                    if (obj.hasFiles) {
                        $.each(obj.files, function (j, oFile) {
                            content += `<a href="#" title="${oFile}" class="open-file" data-subsidiary="${obj.subsidiary}" data-code="${obj.docEntry}" data-file="${oFile}"><span class="glyphicon glyphicon-paperclip"></span></a>&nbsp;&nbsp;&nbsp;`;
                        });
                    }
                    content += `</td></tr>`;
                });
                content += '</tbody></table>';

                var wnd = $("#Detail").data("kendoWindow");
                wnd.setOptions({ title: "Detalle Reservas" });
                wnd.content(content);
                wnd.open().center();
            } else {
                showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
            }
        });
    }
}

function seeProductDetail(e) {
    var productId = $(e.target).closest(".product").find("#productId").val();
    $("<div>").kendoWindow({
        visible: true,
        scrollable: true, modal: true, width: 1180, iframe: false,
        content: { url: urlGetDetailPopup, data: { Id: productId } },
        activate: function (e) {
            var wnd = this;
            setTimeout(() => {
                onRefreshWindow(e);
                wnd.center();
            }, 300);
        }
    });
}

function addToCart(e) {
    var productId = $(e.currentTarget).closest(".product").find("#productId").val();
    var productCode = $(e.currentTarget).closest(".product").find(".product-code").text(), productName = $(e.currentTarget).closest(".product").find(".product-name").text();
    var bolOpenBox = $(e.currentTarget).closest(".product").hasClass("open-box");
    var price = 0, max = 0;
    if (bolOpenBox) {
        var objSelected = $(e.currentTarget).closest(".product").find(".product-prices div");
        price = objSelected.find("span.price").text().replace(",", "");
        max = objSelected.find("span.quantity").text().replace(",", "");
    }

    var wnd = $("#CartDetail").data("kendoWindow");
    var ddl = $("#cart-subsidiary").data("kendoDropDownList");
    $("#cart-idproduct").val(productId);
    $("#cart-openbox").val(bolOpenBox ? "Y" : "N");
    $("#cart-quantity").val("1");
    $("#cart-quantity").removeAttr("max");
    $("#cart-itemcode").text(productCode);
    $("#cart-itemname").text(productName);

    if (bolOpenBox) {
        var data = [{ Id: 1, Name: "Santa Cruz", Price: price, Max: max }];
        var ds = new kendo.data.DataSource({ data: data });
        ddl.setDataSource(ds);
        ddl.select(0);
        $("#cart-price").text(kendo.toString(price, "N2"));
        $("#cart-pricedata").val(JSON.stringify(data));
        $("#cart-quantity").attr("max", max);
        wnd.center().open();
    } else {
        $.get(urlGetSubsidiariesByProduct, { ProductId: productId }, data => {
            var ds = new kendo.data.DataSource({ data: data });
            ddl.setDataSource(ds);
            ddl.select(0);
            if (data.length > 0) {
                $("#cart-price").text(kendo.toString(data[0].price, "N2"));
            }
            $("#cart-pricedata").val(JSON.stringify(data));
            wnd.center().open();
        });
    }
}

function addItemToCart(e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var validator = form.kendoValidator().data("kendoValidator");
    if (validator.validate()) {
        var idProduct = $("#cart-idproduct").val(), openBox = $("#cart-openbox").val() === "Y", priceData = JSON.parse($("#cart-pricedata").val()), idSubsidiary = $("#cart-subsidiary").data("kendoDropDownList").value(), item,
            productCode = $("#cart-itemcode").text(), quantity = $("#cart-quantity").val(), url;
        item = priceData.find(x => x.id === idSubsidiary);
        url = $("#LocalUser").val() === "Y" ? urlAddItemTS : urlAddItemSC;
        $.post(url, { IdSubsidiary: idSubsidiary, IdProduct: idProduct, Price: item.price, Quantity: quantity, OpenBox: openBox, Warehouse: "" }, function (data) {
            if (data.message === "") {
                $("#CartDetail").data("kendoWindow").close();
                notification.show({ title: "Item Agregado", message: `Producto (<strong>${productCode}</strong>) agregado correctamente.` }, "success");
            } else {
                notification.show({ title: "Error", message: data.message }, "error");
            }
        });
    }
}

function showSaleOrder(e) {
    var item = e.currentTarget.dataset;

    var wnd = $("#Report").getKendoWindow();
    wnd.title("Orden de Venta");
    loadReport(item.number, item.subsidiary, "Order");
    wnd.open().center();
}

function showProviderOrders(e) {
    var value = +this.textContent;
    if (value > 0) {
        var itemCode = $.trim($(this).closest(".product").find(".product-code").text()), subsidiary = $(this).closest(".price").data("subsidiary"), warehouse = $.trim($(this).closest("tr").find("td").first().text());

        $.get(urlGetProviderOrders, { Subsidiary: subsidiary, Warehouse: warehouse, ItemCode: itemCode }, function (d) {
            if (d.message === "") {
                var content = "";
                if (d.items && d.items.length > 0) {
                    content = `<table class="table"><thead class="thead-light"><tr><th scope="col">#</th><th scope="col">Proveedor</th><th scope="col" class="text-center">Fecha</th><th scope="col">Referencia</th><th scope="col" class="text-right">Cantidad</th><th scope="col" class="text-right">Cantidad Abierta</th><th scope="col" class="text-right">Precio</th></thead><tbody>`;
                    d.items.forEach(x => content += `<tr><td>${x.docNumber}</td><td>${x.providerName}</td><td class="text-center">${kendo.toString(JSON.toDate(x.docDate), "dd-MM-yyyy")}</td><td>${$.trim(x.referenceOrder)}</td><td class="text-right">${x.quantity}</td><td class="text-right">${x.openQuantity}</td><td class="text-right">${kendo.toString(x.price, "N2")}</td></tr>`);
                    content += '</tbody></table>';
                }

                var wnd = $("#Detail").data("kendoWindow");
                wnd.setOptions({ title: "Detalle Ordenes a Proveedor" });
                wnd.content(content);
                wnd.open().center();
            } else {
                showError(`Se ha producido el siguiente error al traer los datos: ${d.message}`);
            }
        });
    }
}

function addRequirement(e) {
    var productId = $(e.currentTarget).closest(".product").find("#productId").val(), productCode = $.trim($(e.currentTarget).closest(".product").find(".product-code").text()),
        productName = $(e.currentTarget).closest(".product").find(".product-name").text();

    $("#ProductName").text(`( ${productCode} ) ${productName}`);
    $("#IdProduct").val(productId);
    $("#CardCode").data("kendoDropDownList").value("");
    $("#IdSubsidiary").data("kendoDropDownList").value(1);
    $("#Quantity").val(1);
    $("#Description").val("");
    $("#RequestDetail").data("kendoWindow").open();
}

function onSavingRequest(e) {
    e.preventDefault(e);
    var form = $(this).closest("form");
    var validator = form.kendoValidator({ messages: { required: "" } }).data("kendoValidator");
    if (validator.validate()) {
        var productId = $("#IdProduct").val(), clientCode = $("#CardCode").data("kendoDropDownList").value(), subsidiaryId = $("#IdSubsidiary").data("kendoDropDownList").value(), quantity = $("#Quantity").val(),
            comments = $("#Description").val(), productName = $("#ProductName").text(), item = { Id: 0, IdProduct: productId, IdSubsidiary: subsidiaryId, CardCode: clientCode, Quantity: quantity, Description: comments };

        $.post(urlAddRequest, { Item: item }, function (d) {
            if (d.message === "") {
                notification.show({ title: "Solicitud guardada", message: `Producto (<strong>${productName}</strong>) agregado correctamente.` }, "success");
                $("#RequestDetail").data("kendoWindow").close();
            } else {
                console.error(d.message);
                notification.show({ title: "Error", message: "Se produjo un error al guardar los datos, intente nuevamente por favor." }, "error");
            }
        });
    }
}

function handleImageError(image) {
    image.src = urlNoImage;
}

function onListSelected(e) {
    var name = e.currentTarget.id.replace("op-", "").toLowerCase();
    $(e.currentTarget).closest(".card-body").find(".regular").each((i, obj) => {
        var d = $(obj).data();
        $(obj).text(kendo.toString(d[name], "N2") + " $Us");
    });
}

function onShowingClients(e) {
    var sel = $(this).parent().find(".active input"), d = $(this).closest(".line-header").data(), name = sel[0].id.replace("op-", ""),
        wnd = $("<div>").kendoWindow({ title: `Clientes ${name}`, width: 500, close: function (e) { this.destroy(); } }).data("kendoWindow"), wndContent = '';

    $.get(urlClientsByGroup, { ListName: name.toLowerCase(), LineId: d.idline }, function (d) {
        if (d.message == "") {
            if (d.items.length > 0) {
                wndContent = '<div class="w-100 p-3">';
                d.items.forEach((x) => wndContent += `<div>( <b>${x.code}</b> ) ${x.name}</div>`);
                wndContent += '</div>';
            } else {
                wndContent = `<div class="w-100 p-3">${(name == "Portal" ? "No es necesario subscribir clientes en esta lista." : "No existen clientes subscritos a esta lista.")}</div>`;
            }
        } else {
            wndContent = '<div class="w-100 p-3">Se ha producido un error al traer la lista de clientes.</div>';
        }
        wnd.content(wndContent);
        wnd.center().open();
    });

}

//#endregion