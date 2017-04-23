$(function () {
    $("button.form-retarget").click(function () {
        var button = $(this);
        var form = $(this).closest("form");
        var action = button.data("action");
        if (action) {
            form.attr("action", action)
        }
        var method = button.data("method");
        if (method) {
            form.attr("method", method);
        }
    });
});