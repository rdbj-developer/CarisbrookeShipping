var DPRFormValidator = undefined;

$(document).ready(function () {
    ValidateDPRForm();
});

function ValidateDPRForm() {

    $('#txtEstimatedArrivalDateEcoSpeed').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    });
    $('#txtEstimatedArrivalDateFullSpeed').datepicker({
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
    $.validator.addMethod("latexp", function (value, element) {        
        if (value == "")
            return true;
        var latF = parseFloat(value);
        if (isNaN(latF)) return false;
        return latF >= -90 && latF <= 90;
    }, "Invalid latitude");

    $.validator.addMethod("longexp", function (value, element) {
        if (value == "")
            return true;
        var lonF = parseFloat(value);
        if (isNaN(lonF)) return false;
        return lonF >= -180 && lonF <= 180;
    }, "Invalid longitude");


    DPRFormValidator = $("#DPRForm").validate({
        rules: {
            VoyageNo: {
                required: true,
                number: true
            },
            Latitudedd: {
                required: true,
                number: true
            },
            Latitudemm: {
                required: true,
                number: true
            },
            DirectionNS: {
                required: true,
            },
            DirectionEW: {
                required: true,
            },
            Longitudeddd: {
                required: true,
                number: true
            },
            Longitudemm: {
                required: true,
                number: true
            },
            AverageSpeed: {
                required: true,
                number: true
            },
            DistanceMade: {
                required: true,
                number: true
            },
            NextPort: {
                required: true,
            },
            EstimatedArrivalDateEcoSpeed: {
                required: true,
                date: true
            },
            EstimatedArrivalTimeEcoSpeed: {
                required: true,
                time24: true
            },
            EstimatedArrivalDateFullSpeed: {
                required: true,
                date: true
            },
            EstimatedArrivalTimeFullSpeed: {
                required: true,
                time24: true
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
            Sludge: {
                required: true,
                number: true
            },
            DirtyOil: {
                required: true,
                number: true
            },
            Pitch: {
                required: true,
                number: true
            },
            EngineLoad: {
                required: true,
                number: true
            },
            HighCylExhTemp: {
                required: true,
                number: true
            },
            ExhGasTempAftTurboChrg: {
                required: true,
                number: true
            },
            OilCunsum: {
                required: true,
                number: true
            },
            WindDirection: {
                required: true
            },
            WindForce: {
                required: true
            },
            SeaState: {
                required: true
            },
            SwellDirection: {
                required: true
            },
            SwellHeight: {
                required: true,
                number: true
            },
            DraftAft: {
                required: true,
                number: true
            },
            DraftForward: {
                required: true,
                number: true
            },
            CargoType: {
                required: true
            },
        },
        messages: {
            VoyageNo: "Only numbers allowed",
            Latitude: {
                required: "Cannot be blank",
                latexp: "Invalid latitude"
            },
            Longitude: {
                required: "Cannot be blank",
                longexp: "Invalid longitude"
            },
            DirectionNS: {
                required: "Required",
            },
            DirectionEW: {
                required: "Required",
            },
            AverageSpeed: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            DistanceMade: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            NextPort: "Cannot be blank",
            EstimatedArrivalDateEcoSpeed: {
                required: "Cannot be blank",
                date: "Only date or date and time allowed"
            },
            EstimatedArrivalTimeEcoSpeed: {
                required: "Cannot be blank",
            },
            EstimatedArrivalDateFullSpeed: {
                required: "Cannot be blank",
                date: "Only date or date and time allowed"
            },
            EstimatedArrivalTimeFullSpeed: {
                required: "Cannot be blank",
            },
            FuelOil: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            DieselOil: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            SulphurFuelOil: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            SulphurDieselOil: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            FreshWater: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            LubeOil: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            Sludge: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            DirtyOil: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            Pitch: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            EngineLoad: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            HighCylExhTemp: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            ExhGasTempAftTurboChrg: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            OilCunsum: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            WindDirection: "Cannot be blank",
            WindForce: "Cannot be blank",
            SeaState: "Cannot be blank",
            SwellDirection: "Cannot be blank",
            SwellHeight: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            DraftAft: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
            DraftForward: {
                required: "Cannot be blank",
                number: "Only numbers allowed"
            },
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}