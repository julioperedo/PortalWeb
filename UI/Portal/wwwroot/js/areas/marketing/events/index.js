//#region Events

$(() => initForm());

$(".next").click((e) => loadNextEvents());

$(".box-container").on("click", "a", (e) => openEvent(e));

//#endregion

//#region Methods

function initForm() {
    setupControls();
    loadNextEvents();
}

function setupControls() {
    $("#Detail").kendoWindow({ visible: false, modal: true, scrollable: true, title: "Detalle de Evento", width: 1000, actions: ["Close"], refresh: onRefreshWindow, iframe: false });
}

function loadNextEvents() {
    var next = +$("#next-event").val();
    if (next > 0) {
        $.get(urlNext, { Initial: next }, function (data) {
            $("#next-event").val(data.next);
            $.each(data.items, function (i, obj) {
                var div, divBox, divInner, h4, p, hidden, spDate;
                p = $("<span>").text(obj.description);
                obj.date = JSON.toDate(obj.date);
                spDate = $("<span>").addClass("date");
                if (obj.date.getHours() === 0 && obj.date.getMinutes() === 0) {
                    spDate.text(kendo.toString(obj.date, "dd/MM/yyyy"));
                } else {
                    spDate.text(kendo.toString(obj.date, "dd/MM/yyyy HH:mm"));
                }
                if (obj.detail) {
                    var url = urlDetail + "/" + obj.id;
                    var link = $("<a>").attr("href", url).text(obj.name);
                    h4 = $("<h4>").html(link);
                } else {
                    h4 = $("<h4>").text(obj.name);
                }
                hidden = $("<input>").attr("type", "hidden").val(obj.id);
                divBox = $("<div>").addClass("box").addClass("color-" + ((i + 1) % 5));
                divInner = $("<div>").addClass("inner-box");
                divInner.append(hidden);
                divInner.append(h4);
                divInner.append(spDate);
                divInner.append("<br />");
                divInner.append(p);
                div = $("<div>").addClass("col-12").addClass("col-sm-6").addClass("col-md-4").addClass("col-lg-4");
                divBox.append(divInner);
                div.append(divBox);
                $(".box-container").append(div);
            });
            if (data.next == 0) {
                $(".next").remove();
            }
        });
    }
}

function openEvent(e) {
    e.preventDefault();
    var wnd = $("#Detail").data("kendoWindow");
    wnd.refresh({ url: e.currentTarget.href });
    setTimeout(() => wnd.open().center(), 400);
}

//#endregion