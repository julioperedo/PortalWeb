//#region GLOBAL DECLARATIONS
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" };
//#endregion

//#region EVENTS

$(() => setupControls());

$(document).ajaxError(onReportError);

$("#listNotes").on("click", ".sale-note", openSaleNote);

$("#resumes").on("click", ".see-details", seeDetails);

//#endregion

//#region METHODS

function setupControls() {
    $("#report").kendoWindow({ visible: false, width: 1100, title: "Nota de Venta", modal: true });
    loadDetail($("#clients").val());
}

function loadDetail(clientCode, year) {
    cleanDetail();
    var resumeDiv = $("#resumes");
    $.get(urlDetail, { CardCode: clientCode }, function (data) {
        if (data.message === "") {
            var tempYear = `<div class="panel slide">
                                <div class="">
                                    <div class="text-center details-title">
                                        <div>
                                            <span class="small-label">año</span><br />
                                            <span class="year-title">#=year#</span><br />
                                        </div>                    
                                        <a class="see-details" data-type="N" data-year="#=year#"><span class="small-label action">ver detalle</span></a>
                                    </div>
                                    <div class="text-right details">
                                        <div>
                                            <span class="year-values">#=kendo.toString(amount, "N2")#</span><br />
                                            <span class="small-label">monto acumulado</span>
                                        </div>
                                        <div>
                                            <span class="year-values">#=kendo.toString(points, "N0")#</span><br />
                                            <span class="small-label">puntos acumulados</span>
                                        </div>
                                        <div>
                                            <span class="year-values">#=status#</span><br />
                                            <span class="small-label">status</span>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
            var template = kendo.template(tempYear);

            var titlesDiv = $("<div>").addClass("width-panel");
            var divName = `<div class="panel available width-panel">
                                <div class="row">
                                    <div class="col text-center">
                                        <div>                                                
                                            <span class="points-title">${kendo.toString(data.availablePoints, "N0")}</span><br />                                       
                                            <span class="small-label">puntos disponibles</span>
                                        </div>
                                    </div>                                       
                                </div>
                            </div>`;
            titlesDiv.append(divName);

            if (data.claimedPoints > 0) {
                var divUsed = `<div class="panel  width-panel">
                                   <div class="row">
                                       <div class="col text-center">
                                            <div>
                                                <span class="small-label">puntos usados</span>
                                                <span class="points-title">${kendo.toString(data.claimedPoints, "N0")}</span>                                                
                                            </div>
                                            <a class="see-details" data-type="UP" data-year="0"><span class="small-label action">ver detalle</span></a>
                                       </div>                                       
                                   </div>
                               </div>`;
                titlesDiv.append(divUsed);
            }
            resumeDiv.append(titlesDiv);

            var slider = $("<div>").attr("id", "years-slider").addClass("slider-2");
            data.years.forEach((x) => {
                slider.append(template(x));
            });
            slider.append(`<div onclick="prev()" class="control-btn prev"><i class="fas fa-arrow-left"></i></div>
               <div onclick="next()" class="control-btn next"><i class="fas fa-arrow-right"></i></div>`);
            resumeDiv.append(slider);

            $("#action-excel").removeClass("d-none");
        } else {
            showError(`Se ha producido el siguiente error al intentar traer los datos: <br /> ${data.message}.`);
        }
    });
}

function loadGridNotes(items) {
    items.forEach((x) => { x.date = JSON.toDate(x.date); });

    $("#notesContainer").removeClass("d-none");
    $("#awardsContainer").addClass("d-none");

    var grd = $("#listNotes").data("kendoGrid");
    if (grd) {
        var ds = new kendo.data.DataSource({ data: items, aggregate: [{ field: "itemPoints", aggregate: "sum" }] });
        grd.setDataSource(ds);
    } else {
        $("#listNotes").kendoGrid({
            dataSource: { data: items, aggregate: [{ field: "Amount", aggregate: "sum" }, { field: "ItemPoints", aggregate: "sum" }] },
            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
            columns: [
                { field: "subsidiary", title: "Sucursal" },
                { field: "number", title: "No. Nota", template: '# if(id) { # <a class="sale-note action action-link">#=number#</a> # } else { # <span>Canje Puntos</span>  # } #' },
                { field: "date", title: "Fecha", format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter },
                { field: "amount", title: "Monto", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                { field: "itemPoints", title: "Puntos", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight }
            ]
        });
    }
}

function loadGridAwards(items) {
    items.forEach((x) => { x.claimDate = JSON.toDate(x.claimDate); });

    $("#notesContainer").addClass("d-none");
    $("#awardsContainer").removeClass("d-none");

    var grd = $("#listAwards").data("kendoGrid");
    if (grd) {
        var ds = new kendo.data.DataSource({ data: items });
        grd.setDataSource(ds);
    } else {
        $("#listAwards").kendoGrid({
            dataSource: { data: items },
            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
            columns: [
                { field: "claimDate", title: "Fecha", format: "{0:dd-MM-yyyy}", width: 100, attributes: alignCenter, headerAttributes: alignCenter },
                { field: "quantity", title: "Cantidad", width: 80, attributes: alignRight, headerAttributes: alignRight },
                { field: "award", title: "Premio" },
                { field: "points", title: "Puntos Usados", width: 120, format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight }
            ]
        });
    }
}

function cleanDetail() {
    $("#resumes").empty();
    loadGridNotes([]);
    $("#notesContainer").addClass("d-none");
    loadGridAwards([]);
    $("#awardsContainer").addClass("d-none");

    $("#action-excel").addClass("d-none");
    $("#action-add").addClass("d-none");
}

function loadReport(Number, Subsidiary) {
    var user = $.trim($(".user-info > .user-name").first().text());
    var report = { report: "SaleNote.trdp", parameters: { Subsidiary: Subsidiary, DocNumber: Number, User: user } };

    var viewer = $("#reportViewer1").data("telerik_ReportViewer");
    if (viewer) {
        try {
            viewer.reportSource(report);
            viewer.refreshReport();
        } catch (e) {
            showInfo("El servidor está ocupado, espere un momento y vuelva a intentar.");
        }
    } else {
        $("#reportViewer1").telerik_ReportViewer({
            serviceUrl: urlService,
            reportSource: report,
            viewMode: telerikReportViewer.ViewModes.INTERACTIVE,
            scaleMode: telerikReportViewer.ScaleModes.FIT_PAGE_WIDTH
        });
    }
}

function openSaleNote(e) {
    var wnd = $("#report").data("kendoWindow"), grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid"), row = $(e.currentTarget).closest("tr"), item = grd.dataItem(row);
    grd.select(row);
    loadReport(item.number, item.subsidiary);
    wnd.open().center();
}

function seeDetails(e) {
    $("#resumes .selected").removeClass("selected");
    $(this).closest(".panel").addClass("selected");
    var cardCode = $("#clients").val(), dataset = this.dataset;
    if (dataset.type === "N") {
        $.get(urlNotes, { CardCode: cardCode, Year: dataset.year }, function (data) {
            if (data.message === "") {
                loadGridNotes(data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor");
            }
        });
    } else {
        $.get(urlAwards, { CardCode: cardCode }, function (data) {
            if (data.message === "") {
                loadGridAwards(data.items);
            } else {
                console.error(data.message);
                showError("Se ha producido un error al traer los datos del servidor");
            }
        });
    }
}

function prev() {
    document.getElementById('years-slider').scrollLeft -= 270;
}

function next() {
    document.getElementById('years-slider').scrollLeft += 270;
}

//#endregion