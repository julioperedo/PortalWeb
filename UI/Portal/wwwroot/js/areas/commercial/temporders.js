//#region Variables Globales

//#endregion

//#region Eventos

$(".save-tempsale").click(SaveTempSale);

$(".send-tempsale").click(SendTempSale);

$(".delete-tempsale").click(DeleteTempSale);

$("body").on("change", ".item-quantity", onQuantityChange);

$("body").on("change", ".price", onPriceChange);

$("body").on("click", ".delete-item", deleteItem);

//$("body").on("change", "input", validateShoppingCart);

//#endregion

//#region Metodos Privados

//function SaveCart(e) {
//    e.preventDefault();
//    if (validateShoppingCart()) {
//        var cart = getCart();
//        $.post(urlEditShoppingCart, { Item: cart }, function (data) {
//            if (data.message === "") {
//                $(".shoppingcart").find(".shopping-cart-id").val(data.id);
//                showMessage("Los Items en el Carrito de Compras fueron guardados correctamente.");
//            } else {
//                showError("Se ha producido el siguiente error al intentar guardar el Carrito de Compras: <br />" + data.message);
//            }
//        });
//    } else {
//        showNotification("", "Hay items que requieren atención.", "error");
//    }
//}

//function SendCart(e) {
//    e.preventDefault();
//    if (validateShoppingCart()) {
//        showConfirm("¿Está seguro que ya terminó su selección, no desea agregar más productos?", function () {
//            var cart = getCart();
//            $.post(urlSendShoppingCart, { Item: cart }, function (data, textStatus, jqXHR) {
//                if (data.message === "") {
//                    $(".tab-content table").remove();
//                    $(".no-items").removeClass("d-none");
//                    showMessage("Los Items en el Carrito de Compras fueron guardados y enviados correctamente a su Ejecutivo de Cuenta.");
//                } else {
//                    showError("Se ha producido el siguiente error al intentar guardar y enviar el Carrito de Compras: <br />" + data.message);
//                }
//            });
//        });
//    } else {
//        showNotification("", "Hay items que requieren atención.", "error");
//    }
//}

//function validateShoppingCart(e) {
//    var valid = true;
//    var validator = $("#tabshoppingcart").find("form").kendoValidator({
//        rules: {
//            quantity: function (input) {
//                if (input.is("[type=number]")) {
//                    var row = input.closest("tr"), idSubsidiary = row.find("[id^='subsidiaries-']").getKendoDropDownList().value(), detailData = JSON.parse(row.find(".detail-data").val()),
//                        item = detailData.find(i => i.Id === +idSubsidiary);
//                    return item && item.Stock > 0;
//                }
//                return true;
//            }
//        },
//        messages: { quantity: "Sin Stock", required: "", min: "Debe ser mayor a 0" }
//    }).data("kendoValidator");
//    validator.reset();
//    valid = validator.validate();
//    return valid;
//}

function onSubsidiaryChange(e) {
    var idSubsidiary = this.value(), objId = $(this.element).attr("id"), row = $("#" + objId).closest("tr"), detailData = JSON.parse(row.find(".detail-data").val());
    var item;
    if (idSubsidiary != "") {
        item = Enumerable.From(detailData).Where("$.Id == " + idSubsidiary).FirstOrDefault();
    } else {
        item = { Price: 0, Stock: 0 };
    }
    row.find(".price").val(item.Price);
    var quantity = parseInt(row.find(".item-quantity").val());
    var subtotal = parseFloat(row.find(".subtotal").text().replace(",", ""));
    var total = parseFloat(row.closest("table").find(".total").text().replace(",", ""));
    row.find(".subtotal").text(kendo.toString(quantity * item.Price, "#,##0.00"));
    row.find(".stock").text(item.Stock);
    row.closest("table").find(".total").text(kendo.toString(total - subtotal + (quantity * item.Price), "N2"));
    validateTempSale();
}

function deleteItem(e) {
    var row = $(this).closest("tr"), subtotal = parseFloat(row.find(".subtotal").text().replace(",", "")), total = parseFloat($(this).closest("table").find(".total").text().replace(",", "")),
        itemCode = row.find(".item-code").text(), that = this;
    showConfirm(`¿Está seguro que desea eliminar el item <b>${itemCode}</b>?`, function () {
        $(that).closest("table").find(".total").text(kendo.toString(total - subtotal, "N2"));
        if ($(that).closest("tbody").find("tr").length === 1) { //la fila que estamos removiendo
            $(that).closest(".tempsale").find(".send-tempsale").addClass("disabled");
            if (parseFloat($(that).closest(".tempsale").find(".items-count").val()) <= 0) {
                $(that).closest(".tempsale").find(".save-tempsale, .delete-tempsale").addClass("disabled");
            }
            $(that).closest("table").parent().find(".no-items").removeClass("d-none");
            $(that).closest("table").remove();
        } else {
            $(that).closest("tr").remove();
        }
    });    
}

function onQuantityChange(e) {
    var row = $(this).closest("tr"), quantity = parseInt($(this).val()), price = parseFloat(row.find(".price").val()), subtotal = parseFloat(row.find(".subtotal").text().replace(",", "")),
        total = parseFloat($(this).closest("table").find(".total").text().replace(",", ""));
    row.find(".subtotal").text(kendo.toString(quantity * price, "#,##0.00"));
    $(this).closest("table").find(".total").text(kendo.toString(total - subtotal + (quantity * price), "N2"));
}

function onPriceChange(e) {
    var row = $(this).closest("tr"), quantity = parseInt(row.find(".item-quantity").val()), price = parseFloat($(this).val()), subtotal = parseFloat(row.find(".subtotal").text().replace(",", "")),
        total = parseFloat($(this).closest("table").find(".total").text().replace(",", ""));
    row.find(".subtotal").text(kendo.toString(quantity * price, "#,##0.00"));
    $(this).closest("table").find(".total").text(kendo.toString(total - subtotal + (quantity * price), "N2"));
}

function SaveTempSale() {
    var items = getTempSaleItems();
    $.post(urlEdit, { Items: items }, function (data) {
        if (data.message === "") {
            $("#tempsale").find(".tempsale-id").val(data.id);
            showMessage("Los Items en el Carrito de Compras fueron guardados correctamente.");
        } else {
            showError("Se ha producido el siguiente error al intentar guardar el Carrito de Compras: <br />" + data.message);
        }
    });
}

function DeleteTempSale() {
    showConfirm("¿Está seguro que desea limpiar los items del carrito?", () => {
        $.post(urlDelete, function (data) {
            if (data.message === "") {
                $(".tempsale table").remove();
                $(".no-items").removeClass("d-none");
                $(".save-tempsale, .send-tempsale, .delete-tempsale").addClass("disabled");
                showMessage("Los Items en el Carrito se limpiaron correctamente.");
            } else {
                showError("Se ha producido el siguiente error al intentar limpiar el Carrito de Compras: <br />" + data.message);
            }
        });
    });
}

function SendTempSale() {
    if (validateTempSale()) {
        showConfirm('¿Est&aacute; seguro que desea enviar el correo y limpiar el carrito?<br /><input type="checkbox" id="send-comments" /><label for="send-comments" class="control-label">&nbsp;Enviar con comentarios</label>', () => {
            var items = getTempSaleItems();
            var sendComments = $("#send-comments").prop("checked");
            $.post(urlSend, { Items: items, WithComments: sendComments }, function (data) {
                if (data.message === "") {
                    $(".tempsale table").remove();
                    $(".no-items").removeClass("d-none");
                    $(".save-tempsale, .send-tempsale, .delete-tempsale").addClass("disabled");
                    showMessage("Se enviaron los datos carrito correctamente.");
                } else {
                    showError("Se ha producido el siguiente error al intentar enviar el Carrito de Compras: <br />" + data.message);
                }
            });
        });
    } else {
        showError("Existen Items que requieren atenci&oacute;n.")
    }
}

function getTempSaleItems() {
    var items = [];
    $.each($(".tempsale tbody tr"), function (i, obj) {
        var id, quantity, idproduct, warehouse, price, idsubsidiary;
        id = $(obj).find(".detail-id").val();
        idproduct = $(obj).find(".product-id").val();
        quantity = $(obj).find(".item-quantity").val();
        price = $(obj).find(".price").val();
        idsubsidiary = $(obj).find("[id^='subsidiaries_']").getKendoDropDownList().value();
        items.push({ id: id, idProduct: idproduct, idSubsidiary: idsubsidiary, quantity: quantity, price: price, warehouse: "" });
    });
    return items;
}

function validateTempSale() {
    var valid = true;
    $.each($("#tempsale tbody tr"), function (i, obj) {
        $(obj).find("*").removeClass("k-invalid");
        if ($(obj).find(".item-quantity").val() <= 0) {
            $(obj).find(".item-quantity").addClass("k-invalid");
            valid = false;
        }
        if ($(obj).find(".price").val() <= 0) {
            $(obj).find(".price").addClass("k-invalid");
            valid = false;
        }
        if ($(obj).find("[id^='subsidiaries_']").getKendoDropDownList().value() == "") {
            $(obj).find(".item-subsidiary").parent().find(".k-dropdown-wrap").addClass("k-invalid");
            valid = false;
        }
    });
    return valid;
}

//#endregion