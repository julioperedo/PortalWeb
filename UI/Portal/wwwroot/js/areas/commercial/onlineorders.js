//#region GLOBAL DECLARATIONS
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" };
var permission;
//#endregion

//#region EVENTS

$(() => setupControls());

$("body").on("change", ".item-quantity", onQuantityChange);

$("body").on("change", ".price", onPriceChange);

$("body").on("click", ".delete-item", deleteItem);

//#endregion

//#region METHODS

function setupControls() {
    $.get(urlGetOrder, {}, function (d) {
        if (d.message === "") {
            $(".no-items").toggleClass("d-none", d.item.details.length > 0);
            if (d.item.details.length > 0) {
                var subtotal, total = 0, content = '';
                d.item.details.forEach(i => {
                    subtotal = i.quantity * i.price;
                    total += subtotal;
                    content += `<tr>
    <td>
        <span class="item-code">${i.product.itemCode}</span>
        <input type="hidden" class="product-id" value="${i.idProduct}" />
        <input type="hidden" class="detail-id" value="${i.id}" />
        <input type="hidden" class="detail-data" value='${JSON.stringify(i.dataExtra)}' />
    </td>
    <td>${i.product.name}</td>
    <td><input id="subsidiaries_${i.id}" data-subsidiary="${i.idSubsidiary}" data-product="${i.idProduct}" required class="w-100 subsidiary" /></td>
    <td><input class="k-textbox price" type="number" value="${i.price}" step="0.1" min="0" style="width: 80px;" lang="en-us" /></td>
    <td><input class="k-textbox item-quantity" type="number" value="${i.quantity}" min="1" style="width: 80px;" /></td>
    <td class="text-right"><span class="text-right stock">${i.stock}</span> </td>
    <td class="text-right"><span class="subtotal">${kendo.toString(subtotal, "N2")}</span></td>
    <td class="text-center"><span class="fas fa-trash-alt delete-item action action-link" title="Quitar Producto"></span></td>
</tr>`;
                });
                $(".table tbody").append(content);
                $(".table tfoot .total").text(kendo.toString(total, "N2"));
                $(".subsidiary").each(function () {
                    var data = this.dataset;
                    $(this).kendoDropDownList({
                        change: onSubsidiaryChange,
                        dataSource: { transport: { read: { url: urlGetSubsidiaries, data: { ProductId: data.product } } } },
                        ignoreCase: true, optionLabel: "Seleccione una Sucursal...", filter: "contains", dataTextField: "name", dataValueField: "id", value: data.subsidiary
                    });
                });
            }
        } else {
            showError('Se ha producido un error al traer los datos del servidor');
        }
    });
}

function onSubsidiaryChange(e) {
    var idSubsidiary = this.value(), objId = $(this.element).attr("id"), row = $("#" + objId).closest("tr"), detailData = JSON.parse(row.find(".detail-data").val()),
        item = idSubsidiary !== "" ? detailData.find(x => x.id === +idSubsidiary) : { price: 0, stock: 0 }, quantity = parseInt(row.find(".item-quantity").val()),
        subtotal = parseFloat(row.find(".subtotal").text().replace(",", "")), total = parseFloat(row.closest("table").find(".total").text().replace(",", ""));
    row.find(".price").val(item.price);
    row.find(".subtotal").text(kendo.toString(quantity * item.price, "#,##0.00"));
    row.find(".stock").text(item.stock);
    row.closest("table").find(".total").text(kendo.toString(total - subtotal + (quantity * item.price), "N2"));
    validateTempSale();
}

function validateTempSale() {
    var valid = true;
    $.each($(".table tbody tr"), function (i, obj) {
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

//#endregion