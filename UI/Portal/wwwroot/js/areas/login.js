var s = $(".form-container");

$("#submit").on("click", function (e) {
    signIn();
});

$("#Password").on("keypress", function (e) {
    if (e.which === 13) {
        signIn();
    }
});

$("#m_login_forget_password").click(function (e) {
    e.preventDefault(), s.removeClass("s-login"), s.removeClass("s-request-user"), s.addClass("s-recovery-password"), s.addClass("step-1");
});

$("#m_login_signup").click(function (e) {
    e.preventDefault(), s.removeClass("s-login"), s.removeClass("s-recovery-password"), s.removeClass("step-1"), s.removeClass("step-2"), s.addClass("s-request-user");
});

$("#send_code_cancel, #change_password_cancel, #user_request_cancel").click(function (e) {
    e.preventDefault(), s.removeClass("s-recovery-password"), s.removeClass("step-1"), s.removeClass("step-2"), s.removeClass("s-request-user"), s.addClass("s-login");
    $(this).closest("form").kendoValidator().data("kendoValidator").hideMessages();
});

$("#send_code_submit").click(function (e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var validator = form.kendoValidator({ messages: { required: "Campo requerido.", email: "No es un E-Mail válido.", checkbox: "" } }).data("kendoValidator");
    if (validator.validate()) {
        $.get(form.attr("action"), { EMail: $("#RecoverEmail").val() }, function (data) {
            if (data.failed) {
                showError(data.message);
            } else {
                s.removeClass("step-1"), s.addClass("step-2");
                $("#CodeGenerated").val(data.message);
                showInfo("Mensaje enviado<br />Por favor revice su email, le hemos enviado un código de restauración.");
            }
        });
    }
});

$("#change_password_submit").click(function (e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var config = {
        rules: {
            code: i => i.is("#CodeSent") ? $.trim(i.val()) === $.trim($("#CodeGenerated").val()) : true,
            confirmpassword: i => i.is("#ConfirmPassword") ? $.trim(i.val()) === $.trim($("#NewPassword").val()) : true,
            minlen: i => i.is("#NewPassword") || i.is("#ConfirmPassword") ? i.val().length > 5 : true
        },
        messages: {
            code: "Código incorrecto",
            confirmpassword: "Las contraseñas no coinciden.",
            minlen: "La Nueva contraseña debe tener al menos 6 caracteres."
        }
    };
    var validator = form.kendoValidator(config).data("kendoValidator");
    if (validator.validate()) {
        $.post(form.attr("action"), { EMail: $("#RecoverEmail").val(), NewPassword: $("#NewPassword").val() }, function (data) {
            if (data.message == "") {
                s.removeClass("s-recovery-password"), s.removeClass("step-1"), s.removeClass("step-2"), s.removeClass("s-request-user"), s.addClass("s-login");
                $("#errors").text("");
                $("#Email, #Password").val("");
                showMessage("Contraseña cambiada correctamente <br /> Ahora ingrese utilizando su nueva contraseña");
            } else {
                showError(data.message);
            }
        });
    }
});

$("#user_request_submit").click(function (e) {
    e.preventDefault();
    var form = $(this).closest("form");
    var validator = form.kendoValidator({ messages: { required: "Campo requerido.", email: "No es un E-Mail válido." } }).data("kendoValidator");
    if (validator.validate()) {
        var obj = form.serializeObject();
        $.post(form.attr("action"), obj, function (data) {
            if (data.succeded) {
                showMessage("Solicitud Procesada <br /> Se le ha enviado un correo con la copia de su solicitud, el resultado de la misma puede tardar hasta 72 horas laborales.");
                s.removeClass("s-recovery-password"), s.removeClass("step-1"), s.removeClass("step-2"), s.removeClass("s-request-user"), s.addClass("s-login");
            } else {
                showError(`${data.title} <br /> ${data.message}`);
                if (data.recover) {
                    s.removeClass("s-login"), s.removeClass("s-request-user"), s.addClass("s-recovery-password"), s.addClass("step-1");
                }
            }
        });
    }
});

$("#enable-request-submit").click(e => {
    var email = $.trim($("#Email").val());
    $.get(urlEnableUser, { EMail: email }, data => {
        if (data.message === "") {
            $("#submit").closest(".row").removeClass("d-none");
            $("#enable-request-submit").closest(".row").addClass("d-none");
            showMessage("Su usuario ha sido habilitado, por favor intente ingresar nuevamente.");
        } else {
            showError(data.message);
        }
    });
});

function signIn() {
    var objValRules = {
        rules: {
            client: i => i.is("#CardCode") && !$("#clients").hasClass("d-none") ? $.trim(i.val()) !== "" : true,
            profile: i => i.is("#IdProfile") && !$("#profiles").hasClass("d-none") ? $.trim(i.val()) !== "" : true
        },
        messages: {
            client: "Debe seleccionar un cliente.",
            profile: "Debe seleccionar un perfil.",
            required: "Campo requerido."
        }
    };
    var form = $("#submit").closest("form");
    var validator = form.kendoValidator(objValRules).data("kendoValidator");
    if (validator.validate()) {
        var email = $.trim($("#Email").val()), password = $.trim($("#Password").val()), CarCode = $("#CardCode").val(), IdProfile = $("#IdProfile").val();

        if ($.trim(email) !== "" & $.trim(password) !== "") {
            var objData;
            if ($("#clients").hasClass("d-none") & $("#profiles").hasClass("d-none")) {
                objData = { email: email, password: password };
            } else {
                IdProfile = $.trim(IdProfile) === "" ? null : +IdProfile;
                objData = { email: email, password: password, cardCode: CarCode, idProfile: IdProfile };
            }
            $.get(urlLogin, objData, function (data, textStatus, jqXHR) {
                $("#errors").text(data.message);
                if (data.message === "") {
                    if (data.logued) {
                        showLoading();
                        window.location.href = returnUrl;
                    } else {
                        if (data.clients || data.hasProfiles) {
                            $(".m-login__wrapper").addClass("form-extended");
                        }
                        if (data.clients) {
                            if (data.clients.length > 0) {
                                $("#clients").removeClass("d-none");
                                var dsClient = new kendo.data.DataSource({ data: data.clients });
                                var ddlClient = $("#CardCode").data("kendoDropDownList");
                                ddlClient.setDataSource(dsClient);
                            }

                        } else {
                            $("#clients").addClass("d-none");
                        }
                        if (data.hasProfiles) {
                            $("#profiles").removeClass("d-none");
                            if (data.profiles) {
                                var dsProfile = new kendo.data.DataSource({ data: data.profiles });
                                var ddlProfile = $("#IdProfile").data("kendoDropDownList");
                                ddlProfile.setDataSource(dsProfile);
                            }
                        } else {
                            $("#profiles").addClass("d-none");
                        }
                    }
                } else {
                    if (data.message.includes("deshabilitado")) {
                        $("#submit").closest(".row").addClass("d-none");
                        $("#enable-request-submit").closest(".row").removeClass("d-none");
                    }
                }
            });
        }
    }
}

function onClientChanged(e) {
    if (!$("#profiles").hasClass("d-none")) {
        var strCardCode = this.value();
        var strEmail = $.trim($("#Email").val()), strPassword = $.trim($("#Password").val());
        $.get(urlProfiles, { email: strEmail, password: strPassword, cardCode: strCardCode }, function (data, textStatus, jqXHR) {
            $("#errors").text(data.message);
            if (data.message === "") {
                if (data.items.length > 0) {
                    var dataSource = new kendo.data.DataSource({ data: data.items });
                    var ddl = $("#IdProfile").data("kendoDropDownList");
                    ddl.setDataSource(dataSource);
                }
            }
        });
    }
}