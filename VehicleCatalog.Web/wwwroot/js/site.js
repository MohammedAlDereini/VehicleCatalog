// Progressive enhancement: turns a native <select> into an accessible,
// type-to-filter combobox (WAI-ARIA "combobox with listbox" pattern).
// Without JavaScript the underlying <select> remains fully functional.
(function () {
    "use strict";

    var MAX_VISIBLE = 1000; // cap rendered options so very broad filters stay snappy

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("[data-combobox]").forEach(enhance);
    });

    function enhance(container) {
        var select = container.querySelector("select");
        if (!select) {
            return;
        }

        var options = Array.prototype.slice
            .call(select.options)
            .filter(function (o) { return o.value !== ""; })
            .map(function (o) { return { value: o.value, label: o.text }; });

        var baseId = select.id || "cb-" + Math.random().toString(36).slice(2);
        var listId = baseId + "-list";
        var inputId = baseId + "-input";

        var input = document.createElement("input");
        input.type = "text";
        input.id = inputId;
        input.className = "combobox-input";
        input.placeholder = "Type or select a make…";
        input.autocomplete = "off";
        input.setAttribute("role", "combobox");
        input.setAttribute("aria-expanded", "false");
        input.setAttribute("aria-controls", listId);
        input.setAttribute("aria-autocomplete", "list");
        input.setAttribute("aria-haspopup", "listbox");

        // Move the "required" obligation onto the visible input; a hidden
        // required <select> would block submission with an unfocusable error.
        input.required = select.required;
        select.required = false;

        var list = document.createElement("ul");
        list.id = listId;
        list.className = "combobox-list";
        list.setAttribute("role", "listbox");
        list.hidden = true;

        var selectedOption = select.options[select.selectedIndex];
        if (selectedOption && selectedOption.value) {
            input.value = selectedOption.text;
        }

        var label = document.querySelector("label[for='" + select.id + "']");
        if (label) {
            label.htmlFor = inputId;
        }

        container.classList.add("is-enhanced");
        container.appendChild(input);
        container.appendChild(list);

        var filtered = options.slice();
        var activeIndex = -1;

        function isOpen() {
            return !list.hidden;
        }

        function open(showAll) {
            render(showAll ? "" : input.value);
            list.hidden = false;
            input.setAttribute("aria-expanded", "true");
        }

        function close() {
            list.hidden = true;
            input.setAttribute("aria-expanded", "false");
            input.removeAttribute("aria-activedescendant");
            activeIndex = -1;
        }

        function render(term) {
            var needle = term.trim().toLowerCase();
            filtered = options.filter(function (o) {
                return needle === "" || o.label.toLowerCase().indexOf(needle) !== -1;
            });

            list.innerHTML = "";
            filtered.slice(0, MAX_VISIBLE).forEach(function (o, i) {
                var li = document.createElement("li");
                li.id = listId + "-opt-" + i;
                li.className = "combobox-option";
                li.setAttribute("role", "option");
                li.setAttribute("aria-selected", String(o.value === select.value));
                li.textContent = o.label;
                li.addEventListener("mousedown", function (e) {
                    e.preventDefault(); // keep focus on the input
                    choose(o);
                });
                list.appendChild(li);
            });

            if (filtered.length === 0) {
                var empty = document.createElement("li");
                empty.className = "combobox-empty";
                empty.textContent = "No matches";
                list.appendChild(empty);
            }

            activeIndex = -1;
            input.removeAttribute("aria-activedescendant");
        }

        function setActive(index) {
            var items = list.querySelectorAll(".combobox-option");
            if (!items.length) {
                return;
            }

            if (index < 0) {
                index = items.length - 1;
            } else if (index >= items.length) {
                index = 0;
            }

            items.forEach(function (el) { el.classList.remove("is-active"); });
            activeIndex = index;
            var el = items[index];
            el.classList.add("is-active");
            el.scrollIntoView({ block: "nearest" });
            input.setAttribute("aria-activedescendant", el.id);
        }

        function choose(option) {
            select.value = option.value;
            input.value = option.label;
            input.setCustomValidity("");
            close();
        }

        input.addEventListener("focus", function () { open(true); });

        input.addEventListener("input", function () {
            select.value = ""; // typing invalidates the previous selection until matched
            input.setCustomValidity("");
            open(false);
        });

        input.addEventListener("keydown", function (e) {
            switch (e.key) {
                case "ArrowDown":
                    e.preventDefault();
                    if (!isOpen()) { open(true); }
                    setActive(activeIndex + 1);
                    break;
                case "ArrowUp":
                    e.preventDefault();
                    if (!isOpen()) { open(true); }
                    setActive(activeIndex - 1);
                    break;
                case "Enter":
                    if (isOpen() && filtered.length) {
                        e.preventDefault();
                        choose(filtered[activeIndex >= 0 ? activeIndex : 0]);
                    }
                    break;
                case "Escape":
                    close();
                    break;
                case "Tab":
                    close();
                    break;
                default:
                    break;
            }
        });

        document.addEventListener("click", function (e) {
            if (!container.contains(e.target)) {
                close();
            }
        });

        var form = select.form;
        if (form) {
            form.addEventListener("submit", function (e) {
                if (input.required && !select.value) {
                    e.preventDefault();
                    input.setCustomValidity("Please select a make from the list.");
                    input.reportValidity();
                    input.focus();
                }
            });
        }
    }
})();
