var DCRFormValidator = undefined;

$(document).ready(function () {
    ValidateDCRForm();
});


function ValidateDCRForm() {

    $('#txtETCDate').datepicker({
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

    DCRFormValidator = $("#DCRForm").validate({
        rules: {
            VoyageNo: {
                required: true,
                number: true
            },
            PortName: {
                required: true
            },
            NoOfGangs: {
                number: true
            },
            NoOfShips: {
                number: true
            },
            QuantityOfCargoLoaded: {
                required: true,
                number: true
            },
            TotalCargoLoaded: {
                required: true,
                number: true
            },
            CargoRemaining: {
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
            Sludge: {
                required: true,
                number: true
            },
            DirtyOil: {
                required: true,
                number: true
            },
            ETCDate: {
                required: true,
                date: true
            },
            ETCTime: {
                required: true,
                time24: true
            },
            NextPort: {
                required: true
            },
            CargoType: {
                required: true
            },
        },
        messages: {
            VoyageNo: "Only integers allowed",
            PortName: "Cannot be blank",
            NoOfGangs: "Only numbers allowed",
            NoOfShips: "Only numbers allowed",
            QuantityOfCargoLoaded: "Only numbers allowed",
            TotalCargoLoaded: "Only numbers allowed",
            CargoRemaining: "Only numbers allowed",
            FuelOil: "Only numbers allowed",
            DieselOil: "Only numbers allowed",
            SulphurFuelOil: "Only numbers allowed",
            SulphurDieselOil: "Only numbers allowed",
            Sludge: "Only numbers allowed",
            DirtyOil: "Only numbers allowed",
            ETCDate: "Only date or date and time allowed",
            ETCTime: {
                required: "Only date or date and time allowed",
                time24: "Invalid time format",
            },
            NextPort: "Cannot be blank",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}