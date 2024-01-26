//#region Global Variables

var _minDate, _maxDate;
var _latitude = -17.8100395202637;
var _longitude = -63.2070960998535;
var mymap;

//#endregion

//#region Events

$(function () {
    setMapHeight();
    setTimeout(function () {
        setupControls();
        filterData(true);
    }, 900);
});

$(window).resize(function () { setMapHeight(); });

$("#action-filter").click(function (e) { filterData(false); });

//#endregion

//#region Methods

function setupControls() {

    $("#Guard").kendoDropDownList({ dataSource: { transport: { read: { url: urlGetGuards } } }, dataTextField: "name", dataValueField: "id", optionLabel: "Seleccione un Guardia ..." });

    var today = new Date(), since = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0, 0, 0), until = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
    var filSince = $("#Since").kendoDateTimePicker({
        value: since, format: "d/M/yyyy HH:mm", interval: 30, change: function (e) {
            var startDate = this.value();
            if (startDate === null) this.value("");
            filUntil.min(startDate ? startDate : _minDate);
        }
    }).data("kendoDateTimePicker");

    var filUntil = $("#Until").kendoDateTimePicker({
        value: until, format: "d/M/yyyy HH:mm", interval: 30, change: function (e) {
            var endDate = this.value();
            if (endDate === null) this.value("");
            filSince.max(endDate ? endDate : _maxDate);
        }
    }).data("kendoDateTimePicker");

    _maxDate = filUntil.max();
    _minDate = filSince.min();

    $("#resumePoints").kendoGrid({
        dataSource: [],
        sortable: true,
        selectable: true,
        groupable: false,
        pageable: false,
        noRecords: { template: '<div class="w-100 text-center p-3">No hay resultados para el criterio de b&uacute;squeda.</div>' },
        columns: [
            { field: "position", title: "#", width: 25 },
            { field: "date", title: "Fecha", width: 150 },
            { field: "type", title: "Tipo", width: 110 }
        ],
        change: function (e) {
            var selectedRows = this.select();
            for (var i = 0; i < selectedRows.length; i++) {
                var item = this.dataItem(selectedRows[i]);
                mymap.eachLayer(function (layer) {
                    if (layer._leaflet_id === item.id) {
                        layer.openPopup();
                    }
                });
            }
        }
    });

    mymap = L.map('map').setView([_latitude, _longitude], 13);

    //L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token=pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw', {
    //    id: 'mapbox/streets-v11',
    //    tileSize: 512,
    //    zoomOffset: -1
    //}).addTo(mymap);
    L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(mymap);

}

function setMapHeight() {
    var content = $("#map"), margins = 35;
    var newHeight = content.height() + $(window).height() - $(".container-fluid .card").height() - margins;
    content.height(newHeight);
    var grid = $("#resumePoints").data("kendoGrid");
    if (grid) {
        grid.setOptions({ height: newHeight });
    }
}

function getFilters() {
    var message = "", guardId = $("#Guard").data("kendoDropDownList").value(), initialDate = $("#Since").data("kendoDateTimePicker").value(), finalDate = $("#Until").data("kendoDateTimePicker").value();
    if (initialDate) initialDate = kendo.toString(initialDate, "yyyy-MM-dd HH:mm");
    if (finalDate) finalDate = kendo.toString(finalDate, "yyyy-MM-dd HH:mm");
    if (initialDate === "" & finalDate === "" & guardId === "") {
        message = "Debe escoger algún criterio de búsqueda.";
    }
    return { message: message, data: { GuardId: guardId, Since: initialDate, Until: finalDate, _: new Date() } };
}

function filterData(pageLoad) {
    var filtersData = getFilters();
    if (filtersData.message === "") {
        mymap.eachLayer(function (layer) { //clear all layers
            if (layer._leaflet_id !== 24) { //except the map
                mymap.removeLayer(layer);
            }
        });
        $.get(urlGetPoints, filtersData.data, function (data) {
            if (data.message !== "") {
                showError(`Se ha producido el siguiente error al traer los datos: ${data.message}`);
            } else {
                if (data.items) {
                    var customIcon = L.Icon.extend({ options: {} });
                    var anchorBig = [11, 28], anchorSmall = [8, 16];
                    var pinIcon = new customIcon({ iconUrl: urlPinIcon, iconAnchor: anchorSmall }), traceIcon = new customIcon({ iconUrl: urlTraceIcon, iconAnchor: anchorBig }), routeIcon = new customIcon({ iconUrl: urlRouteIcon, iconAnchor: anchorBig });
                    var lines = [], items = [];
                    data.items.forEach((x, i) => {
                        var date = JSON.toDate(x.date);
                        var info = `<strong># ${(i + 1)}</strong>` +
                            `<br /><strong>Fecha:</strong> ${kendo.toString(date, "dd/MM/yyyy HH:mm:ss")}` +
                            `<br /><strong>Latitud:</strong> ${x.latitude}` +
                            `<br /><strong>Longitud:</strong> ${x.longitude}` +
                            `<br /><strong>Altitud:</strong> ${kendo.toString(x.altitude, "#,##0.00")} msnm` +
                            `<br /><strong>Precisión:</strong> ${kendo.toString(x.accuracy, "#,##0.00")} m` +
                            `<br /><strong>Fuente:</strong> ${x.provider}`;
                        var icon = x.type === "A" ? pinIcon : (x.type === "C" ? routeIcon : traceIcon);
                        lines.push([x.latitude, x.longitude]);

                        var m = L.marker([x.latitude, x.longitude], { icon: icon }).addTo(mymap).bindPopup(info).openPopup();
                        items.push({ position: i + 1, date: kendo.toString(date, "dd/MM/yyyy HH:mm:ss"), type: x.type == "A" ? "Automático" : "Punto de Control", id: m._leaflet_id });
                    });
                    L.polyline(lines, { color: "red", weight: 1 }).addTo(mymap);
                    loadGrid(items);
                }
            }
        });
    } else {
        if (!pageLoad) {
            showInfo(`Se deben ingresar los siguientes campos: <br />${filtersData.message}`);
        }
    }
    setMapHeight();
}

function loadGrid(items) {
    var grd = $("#resumePoints").getKendoGrid();
    var ds = new kendo.data.DataSource({ data: items });
    grd.setDataSource(ds);
}

//#endregion