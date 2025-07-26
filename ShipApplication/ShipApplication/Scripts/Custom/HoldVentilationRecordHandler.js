var HVRFormValidator = undefined;

$(document).ready(function () {
    initControls();
    $('#txtQuantity').keypress(function (event) {
        if (((event.which != 46 || (event.which == 46 && $(this).val() == '')) ||
            $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    }).on('paste', function (event) {
        event.preventDefault();
    });

});
$(document).on("click", "#btnSubmit", function (e) {
    var isValid = true;
    $("input[type=text]").each(function () {
        var element = $(this);
        if (element.val() == "") isValid = false;
        else {
            isValid = true;
            return false;
        } 
    });
    if (!isValid) {
        $.notify("Please fill some value in any field before submit form.", "warn");
        return false;
    }
});

$(document).on('click', '.records', function () {
    $(".records").css('background-color', '#ffffff!important');
    $(".records").attr('IsSelected', 'false');
    $(this).css('background-color', '#17a2b8!important');
    $(this).attr('IsSelected', 'true');
});

function initControls() {
    $('.datepicker').datepicker({
        autoclose: true
    });
    $('.time').inputmask('99:99', { 'placeholder': '00:00' });
}

function addNewRecord() {
    $(".records").css('background-color', '#ffffff !important');
    $(".records").attr('IsSelected', 'false');

    var count = $(".records").length;
    $(".RecordSheet").append('<tr class="records" index="' + parseInt(count) + '" IsSelected="true">'
        + '    <td width="9%">'
        + '        <div class="form-group has-feedback">'
        + '            <input type="text" autocomplete="off" class="col-md-12 form-control datepicker" name="HoldVentilationRecordList[' + parseInt(count) + '].HVRDate" />'
        + '            <span class="form-control-feedback arrivalCalendarAddon">'
        + '                <i class="fa fa-calendar"></i>'
        + '            </span>'
        + '        </div>'
        + '    </td>'
        + '    <td width="6%">'
        + '        <input type="text" class="form-control time" name="HoldVentilationRecordList[' + parseInt(count) + '].HVRTime" placeholder="HH:MM" title="Only time allowed" />'
        + '    </td>'
        + '    <td width="8%">'
        + '        <input type="text" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].OUTDryBulb" />'
        + '    </td>'
        + '    <td width="8%">'
        + '        <input type="text" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].OUTWetBulb" />'
        + '    </td>'
        + '    <td width="8%">'
        + '        <input type="text" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].OUTDewPOint" />'
        + '    </td>'
        + '    <td width="8%">'
        + '        <input type="text" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].HODryBulb" />'
        + '    </td>'
        + '    <td width="8%">'
        + '        <input type="text" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].HOWetBulb" />'
        + '    </td>'
        + '    <td width="8%">'
        + '        <input type="text" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].HODewPOint" />'
        + '    </td>'
        + '    <td width="2%" class="text-center">'
        + '        <input type="checkbox" name="HoldVentilationRecordList[' + parseInt(count) + '].IsVentilation" value="true" />'
        + '        <input type="hidden" name="HoldVentilationRecordList[' + parseInt(count) + '].IsVentilation" value="false" />'
        + '    </td>'
        + '    <td width="3%">'
        + '        <input type="text" id="txtCargo" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].SeaTemp" />'
        + '    </td>'
        + '    <td width="5%">'
        + '        <input type="text" id="txtCargo" class="col-md-12 form-control" name="HoldVentilationRecordList[' + parseInt(count) + '].Remarks" />'
        + '    </td>'
        + '</tr>');
    initControls();
}

function removeRecord() {
    $("tr[IsSelected='true']").remove();
}
