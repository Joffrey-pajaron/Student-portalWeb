// site-helpers.js
function getCsrfToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value;
}

// Generic AJAX POST with AntiForgeryToken
function ajaxJson(url, data, onSuccess, onError) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: onSuccess,
        error: function (xhr) {
            if (onError) onError(xhr);
            else showError("Server error: " + xhr.status);
        }
    });
}

// Load a partial view into modal
function loadPartial(url, modalSelector) {
    $.get(url, function (res) {   // ✅ GET is correct here
        $(modalSelector + " .modal-content").html(res);
        $(modalSelector).modal("show");
    });
}


// Bind form submission (AJAX inside modal)
function bindFormSubmit(modalSelector, table) {
    $(modalSelector).find("form").on("submit", function (e) {
        e.preventDefault();
        let form = $(this);
        ajaxJson(form.attr("action"), form.serialize(),
            function (res) {
                if (res.success) {
                    showSuccess(res.message);
                    $(modalSelector).modal("hide");
                    if (table) table.ajax.reload();
                } else if (res.errors) {
                    // Show validation errors inside form
                    $(".field-validation-error").text("");
                    for (const [field, message] of Object.entries(res.errors)) {
                        $(`[data-valmsg-for="${field}"]`).text(message);
                    }
                }
            });
    });
}

// SweetAlert-based notifications
function showSuccess(message) {
    Swal.fire({ icon: 'success', title: 'Success!', text: message, timer: 2000, showConfirmButton: false });
}

function showError(message) {
    Swal.fire({ icon: 'error', title: 'Error!', text: message });
}
