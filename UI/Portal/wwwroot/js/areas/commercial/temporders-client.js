//#region Variables Globales

//#endregion

//#region Eventos

$(() => setupControls());

$(".send-cart").click(SendCart);

$(".delete-cart").click(DeleteCart);

$("body").on("change", ".item-quantity", onQuantityChange);

$("body").on("click", ".delete-item", deleteItem);

$("body").on("change", "input", validateShoppingCart);

$("body").on("click", ".add-cart", addItem);

$("body").on("click", ".add-promo", addPromo);

//#endregion

//#region Metodos Privados

function setupControls() {
    loadRelatedProducts(5);
    loadBanner();
}

function loadRelatedProducts(quantity) {
    $.get(urlGetRelatedProducts, { Quantity: quantity }, function (d) {
        if (d.message === "") {
            if (d.items && d.items.length > 0) {
                var placeholder = $("#col-related");
                placeholder.removeClass("d-none");
                d.items.forEach(function (v, i) {
                    var imageUrl = $.trim(v.imageURL) !== "" ? `${urlImages}products/${v.imageURL}` : urlNoImage;
                    var content = `<div class="product"><img src="${imageUrl}" alt="" /><div><b>${v.itemCode}</b><br />${v.name}<br /><button class="btn btn-outline-secondary add-cart" data-id="${v.id}" data-code="${v.itemCode}" data-name="${v.name}" data-prices="${JSON.stringify(v.prices).replaceAll('"', '&quot;')}"><i class="fas fa-cart-plus"></i> Agregar</button></div></div>`;
                    placeholder.append(content);
                });
            }
        } else {
            console.error(d.message);
        }
    });
}

function loadBanner() {
    $.get(urlGetBanner, {}, function (d) {
        if (d.message === "") {
            if (d.item) {
                var placeholder = $("#col-promos");
                placeholder.removeClass("d-none");
                var data = JSON.stringify(d.item.items);
                placeholder.append(`<a class="add-promo" data-items='${data}'><img src="${urlImages}promobanner/${d.item.imageUrl}" alt="" /></a>`);
            }
        } else {
            console.error(d.message);
        }
    });
}

function SaveCart(e) {
    e.preventDefault();
    var cart = getCart();
    $.post(urlEditShoppingCart, { Item: cart }, function (data) {
        if (data.message === "") {
            $(".shoppingcart").find(".shopping-cart-id").val(data.id);
        } else {
            showError("Se ha producido el siguiente error al intentar guardar el Carrito de Compras: <br />" + data.message);
        }
    });
}

function SendCart(e) {
    e.preventDefault();
    if (validateShoppingCart()) {
        showConfirm("¿Está seguro que ya terminó su selección, no desea agregar más productos?", function () {
            var cart = getCart();
            $.post(urlSendShoppingCart, { Item: cart }, function (data, textStatus, jqXHR) {
                if (data.message === "") {
                    $(".tab-content table").remove();
                    $(".no-items").removeClass("d-none");
                    showMessage("Los Items en el Carrito de Compras fueron guardados y enviados correctamente a su Ejecutivo de Cuenta.");
                } else {
                    showError("Se ha producido el siguiente error al intentar guardar y enviar el Carrito de Compras: <br />" + data.message);
                }
            });
        });
    } else {
        showNotification("", "Hay items que requieren atención.", "error");
    }
}

function DeleteCart(e) {
    e.preventDefault();
    showConfirm("¿Está seguro que desea eliminar los datos de su carrito?", function () {
        var id = $("#shopping-cart-id").val();
        if (id !== "0") {
            $.post(urlDeleteShoppingCart, { Id: id }, function (data) {
                if (data.message === "") {
                    $(".tab-content table").remove();
                    $(".no-items").removeClass("d-none");
                    showMessage("Los Items en el Carrito de Compras fueron eliminados.");
                } else {
                    showError("Se ha producido el siguiente error al intentar eliminar el Carrito de Compras: <br />" + data.message);
                }
            });
        }
    });
}

function getCart() {
    var cart = {
        Id: $("#shopping-cart-id").val(), Name: $("#shoppingcart-name").val(), Address: $("#shoppingcart-address").val(), Commentaries: $("#shoppingcart-comments").val()
        , ClientSaleNote: $("#shoppingcart-clientsalenote").val(), SellerCode: $("#sellerCode").val(), WithDropShip: $("#shoppingcart-dropship").prop("checked"), ListSaleDetails: []
    };
    $.each($(".shoppingcart tbody tr"), function (i, obj) {
        var quantity, idproduct, price, idSubsidiary;
        idproduct = $(obj).find(".product-id").val();
        quantity = $(obj).find(".item-quantity").val();
        price = $(obj).find(".price").text().replace(",", "");
        idSubsidiary = $(obj).find("[id^='subsidiaries-']").getKendoDropDownList().value();
        cart.ListSaleDetails.push({ IdProduct: idproduct, Quantity: quantity, Warehouse: "", Price: price, IdSubsidiary: idSubsidiary });
    });
    return cart;
}

function validateShoppingCart(e) {
    var valid = true;
    var validator = $("#tabshoppingcart").find("form").kendoValidator({
        rules: {
            quantity: function (input) {
                if (input.is("[type=number]")) {
                    var row = input.closest("tr"), idSubsidiary = row.find("[id^='subsidiaries-']").getKendoDropDownList().value(), detailData = JSON.parse(row.find(".detail-data").val()),
                        item = detailData.find(i => i.id === +idSubsidiary);
                    return item && (item.stock > 0 || item.isDigital);
                }
                return true;
            }
        },
        messages: { quantity: "Sin Stock", required: "", min: "Debe ser mayor a 0" }
    }).data("kendoValidator");
    validator.reset();
    valid = validator.validate();
    return valid;
}

function onSubsidiaryChange(e) {
    var idSubsidiary = this.value(), objId = $(this.element).attr("id"), row = $("#" + objId).closest("tr"), detailData = JSON.parse(row.find(".detail-data").val()), item, quantity, subtotal, total, price;
    if (idSubsidiary !== "") {
        item = Enumerable.From(detailData).Where(x => x.id === +idSubsidiary).FirstOrDefault();
    } else {
        item = { price: 0, stock: 0 };
    }
    quantity = parseInt(row.find(".item-quantity").val());
    subtotal = parseFloat(row.find(".subtotal").text().replace(",", ""));
    total = parseFloat(row.closest("table").find(".total").text().replace(",", ""));
    price = item.price;
    row.find(".price").text(kendo.toString(price, "#,##0.00"));
    row.find(".subtotal").text(kendo.toString(quantity * price, "#,##0.00"));
    row.closest("table").find(".total").text(kendo.toString(total - subtotal + (quantity * price), "N2"));
    //validateShoppingCart();
    SaveCart(e);
}

function onQuantityChange(e) {
    var row = $(this).closest("tr"), quantity = parseInt($(this).val()), price = parseFloat(row.find(".price").text().replace(",", "")), subtotal = parseFloat(row.find(".subtotal").text().replace(",", "")),
        total = parseFloat($(this).closest("table").find(".total").text().replace(",", "")), idSubsidiary = row.find(":input.item-subsidiary").val();
    if (idSubsidiary && idSubsidiary !== "") {
        var detailData = JSON.parse(row.find(".detail-data").val());
        //var item = Enumerable.From(detailData).Where("$.Id === " + idSubsidiary).FirstOrDefault();
        var item = Enumerable.From(detailData).Where(x => x.id === +idSubsidiary).FirstOrDefault();
        price = item.price;
    }
    row.find(".price").text(kendo.toString(price, "#,##0.00"));
    row.find(".subtotal").text(kendo.toString(quantity * price, "#,##0.00"));
    $(this).closest("table").find(".total").text(kendo.toString(total - subtotal + (quantity * price), "N2"));
    SaveCart(e);
}

function addItem(e) {
    var ds = e.currentTarget.dataset, id = ds.id, code = ds.code, name = ds.name, prices = JSON.parse(ds.prices), idSubsidiary = prices[0].id, price = +prices[0].price;
    var rows = $("#tabshoppingcart table tbody");
    $.post(urlAddItem, { IdProduct: id, Quantity: 1, Price: price, OpenBox: false, IdSubsidiary: idSubsidiary, Warehouse: "" }, function (d) {
        if (d.message === "") {
            $(e.currentTarget).closest(".product").remove();
            var content = `<tr>
                       <td class="pt-4">
                           <span class="item-code">${code}</span>
                           <input type="hidden" class="product-id" value="${id}">
                           <input type="hidden" class="detail-id" value="${d.id}">
                           <input type="hidden" class="detail-data" value="${ds.prices.replaceAll('"', '&quot;')}">
                       </td>
                       <td class="pt-4"><span class="item-name">${name}</span></td>
                       <td style="width: 95px;">
                           <input id="subsidiaries-${d.id}" />
                       </td>
                       <td class="text-right pt-4"><span class="price">${kendo.toString(price, "N2")}</span></td>
                       <td>
                           <input id="item-quantity-${d.id}" name="item-quantity-${d.id}" class="k-textbox item-quantity" type="number" value="1" min="1" style="width: 80px;" required>
                           <span class="k-invalid-msg" data-for="item-quantity-${d.id}"></span>
                       </td>
                       <td class="text-right pt-4"><span class="subtotal">${kendo.toString(price, "N2")}</span></td>
                       <td class="text-center pt-4"><a class="delete-item action action-link" title="Quitar Producto"><i class="fas fa-trash-alt"></i></a></td>
                   </tr>`;
            rows.append(content);
            $(`#subsidiaries-${d.id}`).kendoDropDownList({
                change: onSubsidiaryChange,
                dataSource: { transport: { read: { url: urlGetSubsidiariesByProduct, data: { ProductId: id } } } },
                ignoreCase: true, optionLabel: "Seleccione una Sucursal...", value: 1, filter: "contains", dataTextField: "name", dataValueField: "id"
            });
            loadRelatedProducts(1);
            var total = parseFloat($("#tabshoppingcart").find(".total").text().replace(",", ""));
            $("#tabshoppingcart").find(".total").text(kendo.toString(total + price, "N2"));
        } else {
            console.error(d.message);
            showError(`Ha ocurrido un error al intentar agregar el item al carrito.<br />${d.message}`);
        }
    });
}

function deleteItem(e) {
    var row = $(this).closest("tr"), subtotal = parseFloat(row.find(".subtotal").text().replace(",", "")), total = parseFloat($(this).closest("table").find(".total").text().replace(",", "")),
        itemCode = row.find(".item-code").text(), detailId = row.find(".detail-id").val(), that = this;
    showConfirm(`¿Está seguro que desea eliminar de su carrito el item <b>${itemCode}</b>?`, function () {
        $.post(urlDeleteItem, { Id: detailId }, function (d) {
            if (d.message === "") {
                $(that).closest("table").find(".total").text(kendo.toString(total - subtotal, "N2"));
                if ($(that).closest("tbody").find("tr").length === 1) { //la fila que estamos removiendo
                    $(that).closest(".shoppingcart").find(".send-cart").addClass("disabled");
                    $(that).closest("table").remove();
                    $(".no-items").removeClass("d-none");
                } else {
                    $(that).closest("tr").remove();
                }
            } else {
                console.error(d.message);
                showError(`Se ha producido un error al intentar eliminar el item <b>${itemCode}</b>`);
            }
        });
    });
}

function addPromo(e) {
    var items = JSON.parse(e.currentTarget.dataset.items), rows = $("#tabshoppingcart table tbody");
    console.log(items);
    items.forEach(function (i) {
        console.log(i);
        $.post(urlAddItem, { IdProduct: i.id, Quantity: 1, Price: i.price, OpenBox: false, IdSubsidiary: i.idSubsidiary, Warehouse: "" }, function (d) {
            if (d.message === "") {
                       var content = `<tr>
                       <td class="pt-4">
                           <span class="item-code">${i.productCode}</span>
                           <input type="hidden" class="product-id" value="${i.id}">
                           <input type="hidden" class="detail-id" value="${d.id}">
                           
                       </td>
                       <td class="pt-4"><span class="item-name">${i.productName}</span></td>
                       <td style="width: 95px;">
                           <input id="subsidiaries-${d.id}" />
                       </td>
                       <td class="text-right pt-4"><span class="price">${kendo.toString(i.price, "N2")}</span></td>
                       <td>
                           <input id="item-quantity-${d.id}" name="item-quantity-${d.id}" class="k-textbox item-quantity" type="number" value="1" min="1" style="width: 80px;" required>
                           <span class="k-invalid-msg" data-for="item-quantity-${d.id}"></span>
                       </td>
                       <td class="text-right pt-4"><span class="subtotal">${kendo.toString(i.price, "N2")}</span></td>
                       <td class="text-center pt-4"><a class="delete-item action action-link" title="Quitar Producto"><i class="fas fa-trash-alt"></i></a></td>
                   </tr>`;
            rows.append(content);
            $(`#subsidiaries-${d.id}`).kendoDropDownList({
                change: onSubsidiaryChange,
                dataSource: { transport: { read: { url: urlGetSubsidiariesByProduct, data: { ProductId: i.id } } } },
                ignoreCase: true, optionLabel: "Seleccione una Sucursal...", value: 1, filter: "contains", dataTextField: "name", dataValueField: "id"
            });
            var total = parseFloat($("#tabshoppingcart").find(".total").text().replace(",", ""));
            $("#tabshoppingcart").find(".total").text(kendo.toString(total + i.price, "N2"));
            } else {
                console.error(d.message);
                showError(`Ha ocurrido un error al intentar agregar el item al carrito.<br />${d.message}`);
            }
        });
    });

}

//#endregion