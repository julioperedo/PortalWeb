//#region Variables Globales
var newId = 0;
const alignCenter = { "class": "k-text-center !k-justify-content-center" }, alignRight = { style: "text-align: right;" }, numberFormat = "{0:#,##0.00}", dateFormat = "{0:dd-MM-yyyy}", _gridMargin = 30;
//#endregion

//#region Eventos

$(() => {
    //setupControls();
    //setTimeout(function () { setGridHeight("Listado", _gridMargin) }, 800);
    //filterData(true);
});

//#endregion

//#region Metodos Privados

function setupControls() {
    $("#products").kendoMultiSelect({ filter: "contains", dataTextField: "name", dataValueField: "id", placeholder: "Seleccione los Perfiles" });
}
//#endregion