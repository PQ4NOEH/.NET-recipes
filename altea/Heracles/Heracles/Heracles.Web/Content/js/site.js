(function (window, document, $, undefined) {
    window.i18n.init({
        lng: window.lng,
        resGetPath: "/Content/lng/site-__lng__.json",
        getAsync: true,
        fallbackLng: false,
        useCookie: false
    }).always(function () {
        var form = document.getElementById("login-form");
        var fields = form.getElementsByTagName("fieldset")[0];
        var spinner = document.getElementById("login-spinner");
        var validation = document.getElementById("login-validation");

        var username = document.getElementById("Username");
        var password = document.getElementById("Password");
        var remember = document.getElementById("Remember");

        form.addEventListener("submit", function(event) {
            event.preventDefault();

            fields.disabled = true;
            spinner.style.display = "block";
            validation.innerText = "";

            if (username.value.trim() === "" || password.value === "") {
                validation.innerText = i18n.t("login-empty-data-error");
                fields.disabled = false;
                spinner.style.display = "";
                return;
            }

            $.ajax("/", {
                async: true,
                type: "POST", 
                data: {
                    Username: username.value,
                    Password: password.value,
                    Remember: remember.checked
                },
                dataType: "json",

                success: function(result) {
                    if (result.status) {
                        window.location.reload();
                    } else {
                        validation.innerText = result.message;
                        fields.disabled = false;
                        spinner.style.display = "";
                    }
                },

                error: function() {
                    validation.innerText = i18n.t("login-error");
                    fields.disabled = false;
                    spinner.style.display = "";
                }
            });
        }, true);
    });
})(window, document, jQuery);
