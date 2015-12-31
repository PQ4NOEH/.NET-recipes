window.ALTEA_PROMISES.push(function(window, document, $, undefined) {
    "use strict";

    if (window.admin === undefined) {
        window.admin = {};
    }

    var api = Object.create({
        createUser: "/Admin/Users/Create"
    });

    var dom = Object.create({
        createIcon: document.getElementById("adm-bx-usr-add"),
        mpdId: "adm-add-usr",
        mpdContent: null
    });

    function createPopup() {
        if (window.mpd.created()) {
            window.mpd.destroy();
        }

        dom.mpdContent = window.mpd.create(
            dom.mpdId,
            "500px",
            i18n.t("admin.adduser"),
            { closebtn: true }
        );

        var roleContainer = document.createElement("div");
        roleContainer.id = "adm-usr-add-rlc";
        dom.mpdContent.appendChild(roleContainer);

        for (var i = 0, l = window.model.roles.length; i < l; i++) {
            var role = document.createElement("button");
            role.className = "adm-usr-add-rle input " + (i === 0 ? "input-bl input-nh" : "input-gr");
            role.dataset.id = i;
            role.innerText = i18n.t("roles." + window.model.roles[i]);
            role.style.width = (100 / l) + "%";
            roleContainer.appendChild(role);
        }

        var firstName = document.createElement("input");
        firstName.id = "adm-usr-add-fn";
        firstName.className = "input";
        firstName.placeholder = i18n.t("FirstName");
        dom.mpdContent.appendChild(firstName);

        var lastName = document.createElement("input");
        lastName.id = "adm-usr-add-ln";
        lastName.className = "input";
        lastName.placeholder = i18n.t("LastName");
        dom.mpdContent.appendChild(lastName);

        var mail = document.createElement("input");
        mail.id = "adm-usr-add-ma";
        mail.className = "input";
        mail.placeholder = i18n.t("MailAddress");
        mail.type = "email";
        dom.mpdContent.appendChild(mail);

        var passContainer = document.createElement("div");
        passContainer.id = "adm-usr-add-pc";
        dom.mpdContent.appendChild(passContainer);

        var password = document.createElement("input");
        password.id = "adm-usr-add-pw";
        password.className = "input";
        password.placeholder = i18n.t("Password");
        password.type = "password";
        passContainer.appendChild(password);

        var passwordCheckbox = document.createElement("div");
        passwordCheckbox.id = "adm-usr-add-pwc";
        passContainer.appendChild(passwordCheckbox);

        var passwordMini = document.createElement("input");
        passwordMini.id = "adm-usr-add-pwm";
        passwordMini.name = "adm-usr-add-pwm";
        passwordMini.className = "mini";
        passwordMini.type = "checkbox";
        passwordCheckbox.appendChild(passwordMini);

        var passwordMiniLabel = document.createElement("label");
        passwordMiniLabel.id = "adm-usr-add-pwl";
        passwordMiniLabel.className = "option";
        passwordMiniLabel.htmlFor = "adm-usr-add-pwm";
        passwordMiniLabel.placeholder = i18n.t("admin.randompassword");
        passwordCheckbox.appendChild(passwordMiniLabel);

        var passwordLabel = document.createElement("label");
        passwordLabel.id = "adm-usr-add-pwlt";
        passwordLabel.htmlFor = "adm-usr-add-pwm";
        passwordLabel.innerText = i18n.t("admin.randompassword");
        passwordCheckbox.appendChild(passwordLabel);

        var fromId, toId;

        if (Object.keys(window.model.languages).length !== 1) {
            var languageFrom = document.createElement("select");
            languageFrom.id = "adm-usr-add-lfrm";
            languageFrom.className = "input clearfix";
            languageFrom.placeholder = i18n.t("LanguageFrom");
            var defaultFrom = document.createElement("option");
            defaultFrom.selected = true;
            defaultFrom.disabled = true;
            defaultFrom.value = "";
            defaultFrom.innerText = i18n.t("LanguageFrom");
            languageFrom.appendChild(defaultFrom);
            dom.mpdContent.appendChild(languageFrom);

            for (var language in window.model.languages) {
                var option = document.createElement("option");
                option.value = language;
                option.innerText = i18n.t("languages." + language);
                languageFrom.appendChild(option);
            }

            var languageTo = document.createElement("select");
            languageTo.id = "adm-usr-add-lto";
            languageTo.className = "input clearfix";
            languageTo.placeholder = i18n.t("LanguageTo");
            var defaultTo = document.createElement("option");
            defaultTo.selected = true;
            defaultTo.disabled = true;
            defaultTo.value = "";
            defaultTo.innerText = i18n.t("LanguageTo");
            languageTo.appendChild(defaultTo);
            dom.mpdContent.appendChild(languageTo);

            $(languageFrom).selectize({
                onChange: function(value) {
                    languageTo.selectize.clearOptions();
                    languageTo.selectize.clear();

                    for (var language in window.model.languages[value]) {
                        languageTo.selectize.addOption({
                            value: language,
                            text: i18n.t("languages." + language)
                        });
                    }

                    fromId = value;
                }
            });

            $(languageTo).selectize({
                onChange: function(value) {
                    toId = value;
                }
            });
        } else {
            fromId = Object.keys(window.model.languages)[0];
            toId = window.model.languages[fromId][0];
        }

        var create = document.createElement("button");
        create.id = "adm-usr-add-cte";
        create.className = "input input-bl";
        create.innerText = i18n.t("Create");
        create.disabled = true;
        dom.mpdContent.appendChild(create);

        $(passwordMini).on("change", function() {
            if (this.checked) {
                password.value = "************************";
                password.readOnly = true;
            } else {
                password.value = "";
                password.readOnly = false;
            }
        });

        $(".adm-usr-add-rle").on("click", function() {
            var roles = document.getElementsByClassName("adm-usr-add-rle");
            for (var i = 0, l = roles.length; i < l; i++) {
                roles[i].classList.remove("input-bl", "input-nh");
                roles[i].classList.add("input-gr");
            }

            this.classList.add("input-bl", "input-nh");
            this.classList.remove("input-gr");
        });

        function checkEnableCreate() {
            create.disabled = firstName.value.trim().length === 0
                || lastName.value.trim().length === 0
                || !mail.validity.valid
                || password.value.trim().length === 0;
        }

        $(firstName).on("input", checkEnableCreate);
        $(lastName).on("input", checkEnableCreate);
        $(mail).on("input", checkEnableCreate);
        $(password).on("input", checkEnableCreate);
        $(passwordMini).on("change", checkEnableCreate);

        function createUser() {
            var mailValue = mail.value;

            $.ajax({
                async: true,
                type: "POST",
                url: api.createUser,
                data: {
                    firstName: firstName.value.trim(),
                    lastName: lastName.value.trim(),
                    mail: mailValue,
                    password: password.value,
                    autoPassword: passwordMini.checked,
                    role: window.model.roles[parseInt($(".adm-usr-add-rle.input-bl")[0].dataset.id)],
                    from: fromId,
                    to: toId
                },

                success: function(result) {
                    if (result === null) {
                        createNoty("warning", "center", i18n.t("error-processing"), 1500, ["click"], {
                            callback: {
                                afterClose: function() {
                                    $(".adm-usr-add-rle").prop("disabled", false);
                                    firstName.disabled = false;
                                    lastName.disabled = false;
                                    mail.disabled = false;
                                    password.disabled = false;
                                    passwordMini.disabled = false;
                                    create.disabled = false;
                                    window.mpd.unblock();
                                }
                            }
                        });
                    } else {
                        while (dom.mpdContent.firstChild) {
                            dom.mpdContent.removeChild(dom.mpdContent.firstChild);
                        }

                        var pUser = document.createElement("p");
                        pUser.className = "adm-add-usr-p";
                        pUser.innerHTML = i18n.t("admin.usercreated").replace("{0}", result);
                        dom.mpdContent.appendChild(pUser);

                        var pMail = document.createElement("p");
                        pMail.className = "adm-add-usr-p";
                        pMail.innerHTML = i18n.t("admin.usercreatedmail").replace("{0}", mailValue);
                        dom.mpdContent.appendChild(pMail);

                        var finish = document.createElement("button");
                        finish.className = "input input-bl";
                        finish.innerText = i18n.t("Close");
                        dom.mpdContent.appendChild(finish);

                        window.mpd.resize();

                        $(finish).one("click", function() {
                            if (!window.model.index) {
                                window.location.refresh();
                            } else {
                                window.mpd.unblock();
                                window.mpd.destroy();
                            }
                        });
                    }
                },

                error: function() {
                    createNoty("error", "center", i18n.t("internal-error"), 1500, ["click"], {
                        callback: {
                            afterClose: function() {
                                $(".adm-usr-add-rle").prop("disabled", false);
                                firstName.disabled = false;
                                lastName.disabled = false;
                                mail.disabled = false;
                                password.disabled = false;
                                passwordMini.disabled = false;
                                create.disabled = false;
                                window.mpd.unblock();
                            }
                        }
                    });
                }
            });
        }

        $(create).on("click", function() {
            $(".adm-usr-add-rle").prop("disabled", true);
            firstName.disabled = true;
            lastName.disabled = true;
            mail.disabled = true;
            password.disabled = true;
            passwordMini.disabled = true;
            create.disabled = true;
            window.mpd.block();

            createUser();
        });
    }

    $(dom.createIcon).on("click", function() {
        createPopup();
        window.mpd.show();
    });
});
