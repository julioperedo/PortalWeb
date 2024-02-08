//#region GLOBAL DECLARATIONS
const alignCenter = { style: "text-align: center;" }, alignRight = { style: "text-align: right;" };
var permission;
//#endregion

//#region EVENTS

$(() => setupControls());

$(document).ajaxError(onReportError);

$("#listNotes").on("click", ".sale-note", function (e) {
    var wnd = $("#report").data("kendoWindow");
    var grd = $(e.currentTarget).closest(".k-grid").data("kendoGrid");
    var row = $(e.currentTarget).closest("tr");
    grd.select(row);
    var item = grd.dataItem(row);

    loadReport(item.number, item.subsidiary);
    wnd.open().center();
});

$("#resumes").on("click", ".see-details", function (e) {
    $("#resumes .selected").removeClass("selected");
    $(this).closest(".panel").addClass("selected");
    var cardCode = $("#clients").data("kendoDropDownList").value();
    var dataset = this.dataset;
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
});

$("#action-excel").click(function (e) {
    e.preventDefault();
    var year = $("#year").val();
    var clientCode = $("#clients").val();
    if (clientCode !== "" && year !== "") {
        window.location.href = urlExportExcel + "?" + $.param({ CardCode: clientCode, Year: year });
    }
});

$("#action-add").click(function (e) {
    $("#addNoteWindow").data("kendoWindow").center().open();
});

$("#search-button").click(function (e) {
    var validator = $("#search-form").kendoValidator({ messages: { required: "*" } }).data("kendoValidator");
    if (validator.validate()) {
        var number = $("#number").val(), subsidiaries = $("#subsidiaries").data("kendoDropDownList").value(), clientCode = $("#clients").val();
        $.get(urlNote, { "Number": number, "Subsidiary": subsidiaries }, function (data) {
            if (data.message === "") {
                if (data.valid) {
                    var x = data.item;
                    x.noteDate = JSON.toDate(x.noteDate);
                    x.orderDate = JSON.toDate(x.orderDate);
                    $("#note-data").val(JSON.stringify(x));
                    $("#client-name").text(x.clientName);
                    $("#seller-name").text(x.sellerName);
                    $("#client-order").text(x.clientOrder);
                    $("#status-used").data("kendoDropDownList").value("1");

                    $("#order-number").text(x.orderNumber);
                    $("#order-total").text(kendo.toString(x.orderTotal, "N2") + " $Us");
                    $("#order-date").text(kendo.toString(x.orderDate, "dd-MM-yyyy"));
                    $("#order-state").text(x.orderState);

                    $("#note-number").text(x.noteNumber);
                    $("#note-total").text(kendo.toString(x.noteTotal, "N2") + " $Us");
                    $("#note-date").text(kendo.toString(x.noteDate, "dd-MM-yyyy"));

                    var gridItems = $("#note-items").data("kendoGrid");
                    if (gridItems) {
                        gridItems.setDataSource(x.items);
                    } else {
                        $("#note-items").kendoGrid({
                            dataSource: { data: x.items },
                            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
                            columns: [
                                { field: "code", title: "Cod. Item", width: 150 },
                                { field: "name", title: "Detalle" },
                                { field: "quantity", title: "Cantidad", width: 90, attributes: alignRight, headerAttributes: alignRight },
                                { field: "accelerator", title: "Acelerador", width: 110, attributes: alignRight, headerAttributes: alignRight },
                                { field: "availableQuantity", title: "Cant. Disponible", width: 140, attributes: alignRight, headerAttributes: alignRight },
                                {
                                    title: "Acc", width: 60, attributes: alignCenter, template: function (item) {
                                        if (item.accelerator > 0 & item.availableQuantity > 0) {
                                            return `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input acc-used" id="use-${item.id}"><label class="custom-control-label" for="use-${item.id}">&nbsp;</label></div>`;
                                        } else {
                                            return "";
                                        }
                                    }
                                },
                                { title: "Usar", width: 60, attributes: alignCenter, template: item => `<div class="custom-control custom-switch"><input type="checkbox" class="custom-control-input" id="enabled-${item.id}" checked="checked"><label class="custom-control-label" for="enabled-${item.id}">&nbsp;</label></div>` }
                            ]
                        });
                    }
                    $("#addNoteWindow").data("kendoWindow").center();
                    if (clientCode.toLowerCase() === x.clientCode.toLowerCase()) {
                        $("#save-note").removeAttr("disabled");
                    } else {
                        $("#save-note").attr("disabled", "disabled");
                        showInfo(`La nota no pertenece al cliente ${clientCode}`);
                    }
                } else {
                    showInfo(`El código <b>${number}</b> no corresponde a una Nota de Venta válida para <b>${subsidiaries}</b>.`)
                }
            } else {
                showError("Se ha producido un error al traser los datos del servidor.");
            }
        });
    }
});

$("#save-note").click(function (e) {
    var data = JSON.parse($("#note-data").val());
    data.orderDate = JSON.toDate(data.orderDate);
    data.noteDate = JSON.toDate(data.noteDate);

    var note = { Subsidiary: $("#subsidiaries").data("kendoDropDownList").text(), Number: data.noteNumber, CardCode: data.clientCode, NoteDate: kendo.toString(data.noteDate, "yyyy-MM-dd"), Amount: data.noteTotal, Terms: data.terms, IdStatus: $("#status-used").data("kendoDropDownList").value(), AcceleratorPeriod: $("#accelerator-extra").val(), Items: [] };

    data.items.forEach(x => {
        var chkEnabled = $("#note-items").find(`#enabled-${x.id}`);
        var used = chkEnabled && chkEnabled.prop("checked");
        var item = { IdProduct: x.id, Quantity: x.quantity, Total: x.total, AcceleratedQuantity: 0, AcceleratedTotal: 0, Accelerator: 0, ExtraPoints: 0, IdLot: 0, Enabled: used };
        var chk = $("#note-items").find(`#use-${x.id}`);
        if (chk && chk.prop("checked")) {
            item.Accelerator = x.accelerator;
            item.AcceleratedQuantity = x.availableQuantity > x.quantity ? x.quantity : x.availableQuantity;
            item.AcceleratedTotal = x.price * item.AcceleratedQuantity;
            item.ExtraPoints = item.AcceleratedTotal * (x.accelerator - 1);
            item.IdLot = x.idLot;
            item.RemainQuantity = x.availableQuantity - item.AcceleratedQuantity;
        }
        note.Items.push(item);
    });

    $.post(urlSaveNote, { Item: note }, data => {
        if (data.message === "") {
            showInfo("Se ha guardado la Nota correctamente.");
            var status = data.status;
            $("#currentAmount").text(` $US ${kendo.toString(status.amount, "N2")}`);
            $("#currentPoints").text(kendo.toString(status.points, "N0"));
            $("#currentState").text(status.status.name);
            $("#currentRate").text(kendo.toString(status.status.points, "N2"));

            loadGridNotes(data.items);
            cleanFormAdd();
            $("#addNoteWindow").data("kendoWindow").close();
        } else {
            console.error(data.message);
            showError("Se ha producido un error al guardar la Nota.");
        }
    });
});

//#endregion

//#region METHODS

function setupControls() {
    permission = JSON.parse($("#kbytes-data").val());
    var urlData = permission.AllClients > 0 ? urlClients : urlSellerClients;
    $("#clients").kendoDropDownList({
        dataSource: { transport: { read: { url: urlData } } },
        dataTextField: "name",
        dataValueField: "code",
        optionLabel: "Seleccione un cliente",
        filter: "contains",
        virtual: {
            itemHeight: 26,
            valueMapper: function (options) {
                var items = this.dataSource.data(), index = items.indexOf(items.find(i => i.code === options.value));
                options.success(index);
            }
        },
        change: function (e) {
            var value = this.value();
            if (value !== "") {
                loadDetail(value);
            } else {
                cleanDetail();
            }
        }
    });

    $("#subsidiaries").kendoDropDownList({
        dataSource: { transport: { read: { url: urlSubsidiaries } } },
        dataTextField: "name",
        dataValueField: "name",
        optionLabel: "Seleccione una Sucursal",
        filter: "contains"
    });

    $("#status-used").kendoDropDownList({
        dataSource: { transport: { read: { url: urlStatus } } },
        dataTextField: "name",
        dataValueField: "id"
    });

    $("#report").kendoWindow({ visible: false, width: 1100, title: "Nota de Venta", modal: true });

    $("#addNoteWindow").kendoWindow({ visible: false, width: 1200, title: "Adicionar Orden/Nota de Venta", modal: true });
}

function loadDetail(clientCode) {
    cleanDetail();
    var resumeDiv = $("#resumes");
    $.get(urlDetail, { CardCode: clientCode }, function (data) {
        if (data.message === "") {
            var tempYear = `<div class="panel width-panel height-panel">
                                <div class="row">
                                    <div class="col text-center details-title">
                                        <div>
                                            <span class="small-label">año</span><br />
                                            <span class="year-title">#=year#</span><br />
                                        </div>                    
                                        <a class="see-details" data-type="N" data-year="#=year#"><span class="small-label action">ver detalle</span></a>
                                    </div>
                                    <div class="col text-right details">
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

            var titlesDiv = $("<div>").addClass("float-left").addClass("width-panel").addClass("height-panel");
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

            data.years.forEach((x) => {
                resumeDiv.append(template(x));
            });

            $("#action-excel").removeClass("d-none");
            if (permission.AddNote > 0) {
                $("#action-add").removeClass("d-none");
            }
        } else {
            showError(`Se ha producido el siguiente error al intentar traer los datos: <br /> ${data.message}.`);
        }
    });
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

function loadGridNotes(items) {
    items.forEach((x) => { x.date = JSON.toDate(x.date); });

    $("#notesContainer").removeClass("d-none");
    $("#awardsContainer").addClass("d-none");

    var grd = $("#listNotes").data("kendoGrid");
    if (grd) {
        var ds = new kendo.data.DataSource({ data: items });
        grd.setDataSource(ds);
    } else {
        var gridColumns = [
            { field: "subsidiary", title: "Sucursal" },
            { field: "number", title: "No. Nota", template: '# if(id) { # <a class="sale-note action action-link">#=number#</a> # } else { # <span>Canje Puntos</span>  # } #' },
            { field: "date", title: "Fecha", format: "{0:dd-MM-yyyy}", attributes: alignCenter, headerAttributes: alignCenter },
            { field: "amount", title: "Monto", format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
            { field: "status", title: "Status" },
            { field: "statusUsed", title: "Status Aplicado" }
        ];

        if (permission.ExtraPoints > 0) {
            gridColumns.splice(7, 0, { field: "points", title: "Puntos", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight },
                { field: "extraPoints", title: "Puntos Acelerador", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight },
                { field: "extraPointsPeriod", title: "Puntos Ace. (Periodo)", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight },
                { field: "acceleratorPeriod", title: "Acelerador (Periodo)", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight }
            );
        } else {
            gridColumns.splice(7, 0, { field: "itemPoints", title: "Puntos", format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight });
        }

        var gridConfig = {
            dataSource: { data: items },
            sortable: true, selectable: true, pageable: false, noRecords: { template: "No hay registros para el criterio de búsqueda" },
            columns: gridColumns,
            dataBound: (e) => e.sender.element.find("table").attr("style", "")
        };

        if (permission.SeeDetail > 0) {
            //gridConfig.detailTemplate = kendo.template($('#detailOrder').html());
            gridConfig.detailInit = function (e) {
                var id = e.data.id;
                $.get(urlNoteItems, { NoteId: id }, function (data) {
                    if (data.message === "") {
                        $("<div>").appendTo(e.detailCell).kendoGrid({
                            scrollable: false,
                            sortable: true,
                            pageable: false,
                            selectable: true,
                            columns: [
                                { field: "itemCode", title: "Item", width: 150 },
                                { field: "itemName", title: "Descripción", width: 250 },
                                { field: "line", title: "Línea", width: 200 },
                                { field: "quantity", title: "Cantidad", width: 90, attributes: alignRight, headerAttributes: alignRight },
                                { field: "total", title: "Subtotal", width: 90, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                                { field: "acceleratedQuantity", title: "Cant. Acelerador", width: 130, attributes: alignRight, headerAttributes: alignRight },
                                { field: "acceleratedTotal", title: "Subtotal Acelerador", width: 150, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                                { field: "accelerator", title: "Acelerador", width: 120, format: "{0:N2}", attributes: alignRight, headerAttributes: alignRight },
                                { field: "extraPoints", title: "Puntos Extra", width: 130, format: "{0:N0}", attributes: alignRight, headerAttributes: alignRight }
                            ],
                            dataSource: data.items,
                            noRecords: { template: "No hay items con acelerador" },
                            dataBound: (e) => e.sender.element.find("table").attr("style", "")
                        });
                    } else {
                        showError(`Se ha producido el siguiente error al traer los datos: ${data.message}.`);
                    }
                });
            };
        }
        $("#listNotes").kendoGrid(gridConfig);
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

function cleanFormAdd() {
    $("#note-data").val("");
    $("#client-name").text("");
    $("#seller-name").text("");
    $("#client-order").text("");
    $("#status-used").data("kendoDropDownList").value("1");
    $("#accelerator-extra").val("0");

    $("#order-number").text("");
    $("#order-total").text("");
    $("#order-date").text("");
    $("#order-state").text("");

    $("#note-number").text("");
    $("#note-total").text("");
    $("#note-date").text("");

    $("#note-items").data("kendoGrid").setDataSource([]);
}

//#endregion