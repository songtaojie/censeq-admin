$(function () {
    var toastEl = $("#errorToast");
    if (toastEl.length > 0) {
        var toast = new bootstrap.Toast(toastEl[0], { autohide: true, delay: 3000 });
        toast.show();
    }
});
