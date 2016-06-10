(function () {
    $.validator.addMethod("custom-method", function (value, element, params) {
        return $(element).val().toLowerCase() == "test";
    }, 'error');

    $.validator.unobtrusive.adapters.add("custom", ["other"],
        function (options) {
            options.rules["custom-method"] = options.params;
            options.messages["custom-method"] = options.message;
        });
})();