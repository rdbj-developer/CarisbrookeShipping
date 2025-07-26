var formValidator = undefined;

$(document).ready(function () {
    //TestSendMail();
    validateForm();
});

function TestSendMail() {
    $.ajax({
        type: "POST",
        url: "/Forms/SendMailTest",
        dataType: "json",
        contentType: "application/json",
        success: function (response) {

        },
        error: function (response) {
            console.log(response)
        }
    })
}

function validateForm() {

    $('#txtArrivalDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $('#txtTenderedDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $('#txtPOBDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $('#txtDepartureDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $('#txtCargoDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $("#txtArrivalAlongsideDate").datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $.validator.addMethod("time24", function (value, element) {
        if (value == "")
            return true;
        if (!/^\d{2}:\d{2}$/.test(value)) return false;
        var parts = value.split(':');
        if (parts[0] > 23 || parts[1] > 59) return false;
        return true;
    }, "Invalid time format.");


    $.validator.addMethod("date", function (value, element) {
        return this.optional(element) || moment(value, "dd/mm/yyyy").isValid();
    }, "Please enter a valid date in the format dd/mm/yyyy");

    formvalidator1 = $("#arrivalform").validate({
        rules: {
            VoyageNo: {
                required: true,
                number: true
            },
            PortName: {
                required: true
            },
            ArrivalDate: {
                required: true,
                date: true
            },
            ArrivalTime: {
                required: true,
                time24: true
            },
            AverageSpeed: {
                required: true,
                number: true
            },
            Distance: {
                required: true,
                number: true
            },
            FuelOil: {
                required: true,
                number: true
            },
            DieselOil: {
                required: true,
                number: true
            },
            SulphurFuelOil: {
                required: true,
                number: true
            },
            SulphurDieselOil: {
                required: true,
                number: true
            },
            FreshWater: {
                required: true,
                number: true
            },
            LubeOil: {
                required: true,
                number: true
            },
            DepartureDate: {
                required: true,
                date: true
            },
            DepartureTime: {
                required: true,
                time24: true
            },
            NextPort: {
                required: true
            }
            //,
            //TenderedTime: {
            //    time24: true
            //},
            //POBTime: {
            //    time24: true
            //},
            //ArrivalAlongsideTime: {
            //    time24: true
            //}
        },
        messages: {
            VoyageNo: "Only integers allowed",
            PortName: "Cannot be blank",
            ArrivalDate: "Only date or date and time allowed",
            ArrivalTime: {
                required: "Only date or date and time allowed",
                time24: "Invalid time format",
            },
            AverageSpeed: "Only numbers allowed",
            Distance: "Only numbers allowed",
            FuelOil: "Only numbers allowed",
            DieselOil: "Only numbers allowed",
            SulphurFuelOil: "Only numbers allowed",
            SulphurDieselOil: "Only numbers allowed",
            FreshWater: "Only numbers allowed",
            LubeOil: "Only numbers allowed",
            DepartureDate: "Only date or date and time allowed",
            DepartureTime: {
                required: "Only date or date and time allowed",
                time24: "Invalid time format",
            },
            NextPort: "Cannot be blank"
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });


    $.validator.addClassRules({
        txtClonedCCEmail: {
            required: true,
            email: true
        }
    });
}

$(document).on("click", "#lnkAddRecipient", function (e) {
    var ccPanelDiv = $(".CCEmailPanel").clone();

    if ($(ccPanelDiv).hasClass("hide")) {
        $(ccPanelDiv).removeClass("hide");
    }

    if ($(ccPanelDiv).hasClass("CCEmailPanel")) {
        $(ccPanelDiv).removeClass("CCEmailPanel").addClass("CloneCCEmailPanel");
    }

    if ($(ccPanelDiv).find(".txtCCEmail").length > 0) {
        $(ccPanelDiv).find(".txtCCEmail").removeClass("txtCCEmail").addClass("txtClonedCCEmail").attr("name", "CCEmail");
    }

    $(ccPanelDiv).insertAfter(".CCEmailPanel");
});

$(document).on("click", ".removeCCEmail", function () {
    $(this).parent().remove();
});

$(document).on("click", "#btnSubmit", function (e) {
    if ($("#arrivalform").valid()) {

    }
});