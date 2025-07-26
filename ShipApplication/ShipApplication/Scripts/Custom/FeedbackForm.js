$(document).ready(function () {
    $(document).on("click", "#btnSubmit", function (e) {
        if ($("#FeedbackForm").valid()) {}
    });
    $("#AttachmentFile").on("change", function (e) {
        if (typeof (FileReader) != "undefined") {
            var reader = new FileReader();
            var fileName = e.target.files[0].name;
            reader.onload = function (e) {
                $("#AttachmentFileName").val(fileName);
                $("#AttachmentPath").val(e.target.result);
            }
            reader.readAsDataURL($(this)[0].files[0]);
        } else {
            alert("This browser does not support FileReader.");
        }
    });

    validateFeedbackForm();
});

function validateFeedbackForm() {
    FeddbackFormValidator = $("#FeedbackForm").validate({
        rules: {
            Title: {
                required: true
            },
            Details: {
                required: true
            },
        },
        messages: {
            Title: "Cannot be blank",
            Details: "Cannot be blank"
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}