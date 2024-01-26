//#region Events

$(function () {
    loadMembers();
});

//#endregion

//#region Methods

function loadMembers() {
    var dptoTempl = kendo.template($("#department-template").html());
    var container = $("#form-content");
    $.get(urlStaff, {}, (data) => {
        data.departments.forEach((d) => {
            d.managers.forEach((m) => { m.photo = urlImages + m.photo; });
            d.members.forEach((m) => { m.photo = urlImages + m.photo; });
            container.append(dptoTempl(d));
        });
    });
}

//#endregion