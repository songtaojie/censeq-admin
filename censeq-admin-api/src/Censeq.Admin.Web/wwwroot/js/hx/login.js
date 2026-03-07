$(function () {
    $("#PasswordVisibilityButton").click(function (e) {
        let icon = $(this);
        let passwordInput = icon.parent().find("input");
        if (!passwordInput) {
            return;
        }

        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
        }
        else {
            passwordInput.attr("type", "password");
        }

        icon.toggleClass("fa-eye-slash").toggleClass("fa-eye");
    });

    $("input").on("blur", function () {
        $(this).valid(); // 失去焦点立即验证
    });
});
