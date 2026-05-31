$(function () {
    $("#PasswordVisibilityButton").click(function (e) {
        let button = $(this);
        let passwordInput = button.parent().find("input");
        if (!passwordInput) {
            return;
        }

        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
        }
        else {
            passwordInput.attr("type", "password");
        }

        let icon = button.find("i");
        if (icon) {
            icon.toggleClass("fa-eye-slash").toggleClass("fa-eye");
        }
    });

    $("form.needs-validation").on("submit", function (e) {
        let form = $(this);
        let submitter = e.originalEvent && e.originalEvent.submitter ? $(e.originalEvent.submitter) : form.find("button[type='submit']:focus");
        if (!submitter.length) {
            submitter = form.find("button[type='submit'][name='Action'][value='Login']").first();
        }

        if (!submitter.length || submitter.attr("name") !== "Action" || submitter.val() !== "Login") {
            return;
        }

        if (typeof form.valid === "function" && !form.valid()) {
            return;
        }

        if (form.data("loginSubmitting")) {
            e.preventDefault();
            return false;
        }

        form.data("loginSubmitting", true);

        if (!form.find("input[type='hidden'][name='Action'][value='Login']").length) {
            $("<input>", {
                type: "hidden",
                name: "Action",
                value: "Login"
            }).appendTo(form);
        }

        submitter
            .data("original-html", submitter.html())
            .prop("disabled", true)
            .attr("aria-busy", "true")
            .html("<span class=\"spinner-border spinner-border-sm me-2\" role=\"status\" aria-hidden=\"true\"></span>" + $.trim(submitter.text()));
    });

});
