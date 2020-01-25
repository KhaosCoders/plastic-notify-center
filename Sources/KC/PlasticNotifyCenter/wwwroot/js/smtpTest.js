(function($) {
    $("#smtpTestForm").submit(function(event) {
        if (!$("#formSmtp")[0].checkValidity() === true) {
            $("#testMailAddressValidator")
                .html("Please fill out the SMTP configuration first.")
                .removeClass("valid-feedback")
                .addClass("invalid-feedback")
                .show();
        } else {
            if (!this.checkValidity() === true) {
                $("#testMailAddressValidator")
                    .html("Please  fill in your email-address.")
                    .removeClass("valid-feedback")
                    .addClass("invalid-feedback")
                    .show();
            } else {
                $("#testMailAddressValidator").hide();
                var testSmtpValidator = $("#testMailAddressValidator");
                $("#smtpTestForm button").prop("disabled", true);
                $.post(
                        "/Setup/SmtpTest",
                        JSON.stringify({
                            Host: $("#smtpHost").val(),
                            EnableSSL: $("#smtpSSL").val() === "on",
                            Port: parseInt($("#smtpPort").val()),
                            SenderMail: $("#smtpFromMail").val(),
                            SenderAlias: $("#smtpFromName").val(),
                            Username: $("#smtpUser").val(),
                            Password: $("#smtpPassword").val(),
                            ToMail: $("#testMailAddress").val()
                        })
                    )
                    .done(function(data) {
                        $("#smtpTestForm button").prop("disabled", false);
                        if (data.state === 0) {
                            testSmtpValidator
                                .html("SMTP test successful =)")
                                .addClass("valid-feedback")
                                .removeClass("invalid-feedback")
                                .show();
                        } else {
                            testSmtpValidator
                                .html(data.message)
                                .removeClass("valid-feedback")
                                .addClass("invalid-feedback")
                                .show();
                        }
                    })
                    .fail(function() {
                        $("#smtpTestForm button").prop("disabled", false);
                        testSmtpValidator
                            .html("API call failed! Please check your input and try again.")
                            .removeClass("valid-feedback")
                            .addClass("invalid-feedback")
                            .show();
                    });
            }
        }
        event.preventDefault();
        event.stopPropagation();
        return false;
    });
})($);