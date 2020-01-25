(function($) {
    var versionPrefix = "CM Version: ";
    var progressbar = $("#progressbar");

    $("#startSetupBtn").click(function(event) {
        $("#setupIntro").hide("fast");
        $("#setup1").show("fast");
        setProgress(10);
        $("#baseUrl").val(findBaseUrl());
        event.preventDefault();
        event.stopPropagation();
        return false;
    });

    $(".needs-validation").each(function(index) {
        var form = $(this);
        form.submit(function(event) {
            indicateLoading();
            // reset old errors
            $(".form-control").each(function() {
                this.setCustomValidity("");
            });
            // validate form
            if (form[0].checkValidity() === true) {
                // submit form
                eval(form.attr("id") + "_submit()");
            } else {
                indicateError();
            }
            form.addClass("was-validated");
            event.preventDefault();
            event.stopPropagation();
            return false;
        });
    });

    $("#userMode").change(function() {
        updateUserModeInfo();
    });

    function updateUserModeInfo() {
        switch ($("#userMode").val()) {
            case "PncUserMode":
                $("#userModeOptionHelp").html(
                    "<b>Build-In:</b> Manage users and groups inside PlasticNotifyCenter."
                );
                break;
            case "PlasticUserMode":
                $("#userModeOptionHelp").html(
                    '<b>Import Plastic Users:</b> Import users defined in Plastic SCM into PlasticNotifyCenter (only users with a license are imported). <i><i class="fas fa-key"></i> Needs a paid licence</i>'
                );
                break;
            case "LDAPUserMode":
                $("#userModeOptionHelp").html(
                    '<b>LDAP:</b> Import domain users and groups using a LDAP request. <i><i class="fas fa-key"></i> Needs a paid licence</i>'
                );
                break;
            case "ADUserMode":
                $("#userModeOptionHelp").html(
                    '<b>Active Directory:</b> Import all users and group of the current domain. <i><i class="fas fa-key"></i> Needs a paid licence</i>'
                );
                break;
        }
    }
    updateUserModeInfo();

    function formUrl_submit() {
        replaceBaseUrl();

        $("#setup1").hide("fast");
        $("#setup2").show("fast");
        setProgress(25);
        doneLoading();
    }

    function formTriggers_submit() {
        $("#setup2").hide("fast");
        $("#setup3").show("fast");
        setProgress(45);
        doneLoading();
    }

    function formSmtp_submit() {
        $("#setup3").hide("fast");
        $("#setup4").show("fast");
        setProgress(90);
        doneLoading();
    }

    function formAdminUser_submit() {
        var adminPwValidator = $("#adminPwValidator");
        var adminPw2Validator = $("#adminPw2Validator");
        var adminPw = $("#adminPw").val();
        var adminPw2 = $("#adminPw2").val();
        if (adminPw.length < 6) {
            $("#adminPw")[0].setCustomValidity("API error");
            adminPwValidator
                .html("Password is too short! The minimum length is 6 charaters.")
                .removeClass("valid-feedback")
                .addClass("invalid-feedback")
                .show();
            indicateError();
            return;
        }
        adminPwValidator
            .addClass("valid-feedback")
            .removeClass("invalid-feedback")
            .hide();

        if (adminPw !== adminPw2) {
            $("#adminPw")[0].setCustomValidity("API error");
            adminPw2Validator
                .html("The input is not the same")
                .removeClass("valid-feedback")
                .addClass("invalid-feedback")
                .show();
            indicateError();
            return;
        }
        adminPw2Validator
            .addClass("valid-feedback")
            .removeClass("invalid-feedback")
            .hide();

        saveSetup()
            .done(function(data) {
                if (data.state === 0) {
                    $("#setup4").hide("fast");
                    $("#setup5").show("fast");
                    setProgress(100);
                    doneLoading();
                } else {
                    indicateError();
                    $("#adminPw")[0].setCustomValidity("API error");
                    adminPwValidator
                        .html(data.message)
                        .removeClass("valid-feedback")
                        .addClass("invalid-feedback")
                        .show();
                }
            })
            .fail(function() {
                indicateError();
                adminPw2Validator
                    .html("API call failed! Please check your input and try again.")
                    .removeClass("valid-feedback")
                    .addClass("invalid-feedback")
                    .show();
            });
    }

    function saveSetup() {
        indicateLoading();
        return $.post(
            "/Setup/CompleteSetup",
            JSON.stringify({
                BaseUrl: $("#baseUrl").val(),
                AdminPw: $("#adminPw").val(),
                AdminEmail: $("#adminEmail").val(),
                AdminUsername: $("#adminUser").val(),
                Smtp: {
                    Name: $("#smtpName").val(),
                    Host: $("#smtpHost").val(),
                    EnableSSL: $("#smtpSSL").val() === "on",
                    Port: parseInt($("#smtpPort").val()),
                    SenderMail: $("#smtpFromMail").val(),
                    SenderAlias: $("#smtpFromName").val(),
                    Username: $("#smtpUser").val(),
                    Password: $("#smtpPassword").val()
                }
            })
        );
    }

    function validatePw(password) {
        return $.post("/Setup/ValidatePW", JSON.stringify({
            Password: password
        }));
    }

    function setProgress(progress) {
        progressbar.attr("aria-valuenow", progress).width(progress + "%");
    }

    function indicateLoading() {
        progressbar
            .removeClass("bg-danger")
            .addClass("progress-bar-animated")
            .addClass("progress-bar-striped");
    }

    function doneLoading() {
        progressbar
            .removeClass("progress-bar-animated")
            .removeClass("progress-bar-striped");
    }

    function indicateError() {
        doneLoading();
        progressbar.addClass("bg-danger");
    }

    function findBaseUrl() {
        var url = window.location.toString();
        return url.substr(0, url.indexOf("/", url.indexOf("://") + 3));
    }
})($);