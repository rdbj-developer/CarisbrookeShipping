var DepartureFormValidator = undefined;

$(document).ready(function () {
    ValidateDepartureForm();
});


function ValidateDepartureForm() {

    $('#txtDateCargoOperations').datepicker({
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

    $('#txtPOffDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });

    $('#txtETADate').datepicker({
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


    DepartureFormValidator = $("#DepartureForm").validate({
        rules: {
            VoyageNo: {
                required: true,
                number: true
            },
            PortName: {
                required: true
            },
            DepartureDate: {
                required: true,
                date: true
            },
            DepartureTime: {
                required: true,
                time24: true
            },
            DraftFWD: {
                required: true,
                number: true
            },
            DraftAFT: {
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
            NextPort: {
                required: true
            },
            ETADate: {
                required: true,
                date: true
            },
            ETATime: {
                required: true,
                time24: true
            },
            IntendedRoute: {
                required: true
            },
            TimeCargoOperations: {
                time24: true
            },
            POBTime: {
                time24: true
            },
            POffTime: {
                time24: true
            },
        },
        messages: {
            VoyageNo: "Only integers allowed",
            PortName: "Cannot be blank",
            DepartureDate: "Only date or date and time allowed",
            DepartureTime: {
                required: "Only date or date and time allowed",
                time24: "Invalid time format",
            },
            DraftFWD: "Only numbers allowed",
            DraftAFT: "Only numbers allowed",
            FuelOil: "Only numbers allowed",
            DieselOil: "Only numbers allowed",
            SulphurFuelOil: "Only numbers allowed",
            SulphurDieselOil: "Only numbers allowed",
            NextPort: "Cannot be blank",
            ETADate: "Only date or date and time allowed",
            ETATime: {
                required: "Only date or date and time allowed",
                time24: "Invalid time format",
            },
            IntendedRoute: "Cannot be blank",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
