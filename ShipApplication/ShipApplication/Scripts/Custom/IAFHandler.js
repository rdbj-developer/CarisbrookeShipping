// RDBJ 02/02/2022 Added this script
var childrefe = true;
var active
var count
var isvalid = true;
var ISMNonConformity, ISPSNonConformity, ISMObservation, ISPSObservation, MLCDeficiency;
var typeVal = "";
var i = 0;
var _UniqueFormID   // RDBJ 02/02/2022
    , _Ship // RDBJ 04/18/2022
    ;

$(document).ready(function () {
    autosize(document.getElementsByTagName("textarea")); // RDBJ 01/20/2022

    // RDBJ 02/02/2022
    Numbers = Numbers.split(',');
    if (Numbers != null && Numbers != undefined && Numbers.length > 0) {
        ISMNonConformity = Numbers[0];
        ISPSNonConformity = Numbers[1];
        ISMObservation = Numbers[2];
        ISPSObservation = Numbers[3];
        MLCDeficiency = Numbers[4];
    }
    // RDBJ 02/02/2022

    $("#ShipName").val($("#Child option:selected").text());
    _Ship = $("#Child option:selected").val();    // RDBJ 04/18/2022

    // RDBJ 02/02/2022
    $(".IAFAutoUpdateData").bind("change", function (e) {
        e.preventDefault();
        $("#ShipName").val($("#Child option:selected").val());
        _Ship = $("#Child option:selected").val();    // RDBJ 04/18/2022

        // RDBJ 01/22/2022
        if ($("#IAForm").valid()) {
            IAFAutoSave(true);  // RDBJ 04/18/2022 Pass true
        }
    });
    // RDBJ 02/02/2022

    $("#Child").change(function () {
        //$("#ShipName").val($("#Child option:selected").text());
        $("#ShipName").val($("#Child option:selected").val()); // RDBJ 02/02/2022

        // RDBJ 02/02/2022
        if ($("#IAForm").valid()) {
            IAFAutoSave();
        }
    });

    if (isInspector.toLowerCase() == 'false') {
        $("#Child").prop('disabled', true);
    } else {
        $("#Child").prop('disabled', false);
    }

    $("#txtAuditTypeISM").change(function () {
        if ($("#txtAuditTypeISM").prop('checked') == false) {
            $(".type option:contains('ISM')").hide();
        } else {
            $(".type option:contains('ISM')").show();
        }
    });
    $("#txtAuditTypeISPS").change(function () {
        if ($("#txtAuditTypeISPS").prop('checked') == false) {
            $(".type option:contains('ISPS')").hide();
        } else {
            $(".type option:contains('ISPS')").show();
        }
    });
    $("#txtAuditTypeMLC").change(function () {
        if ($("#txtAuditTypeMLC").prop('checked') == false) {
            $(".type option:contains('MLC')").hide();
        } else {
            $(".type option:contains('MLC')").show();
        }
    });

    // RDBJ 02/02/2022
    $('body').on('focus', "#Date", function () {
        $(this).datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
        });
    });
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
    });

    $("#btnselect").click(function () {
        if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='AuditNote[" + count + "].Reference']").val($("#ISMTree li a.ui-state-active").text());
            }
            else {
                $("input[name='AuditNote[" + count + "].Reference']").val($("#ISMTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }

        if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='AuditNote[" + count + "].Reference']").val($("#SSPTree li a.ui-state-active").text());
            }
            else {
                $("input[name='AuditNote[" + count + "].Reference']").val($("#SSPTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }

        if (typeVal == "MLC-Deficiency") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='AuditNote[" + count + "].Reference']").val($("#MLCTree li a.ui-state-active").text());
            }
            else {
                $("input[name='AuditNote[" + count + "].Reference']").val($("#MLCTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }

        $("#ReferenceModal").modal('hide');
        IAFAutoSave();  // RDBJ 01/24/2022
    });
    // RDBJ 02/02/2022

    ValidateIAFForm();
    AuditTypeListSelection(); // RDBJ 02/02/2022
});

function AuditTypeListSelection() {
    if ($("#txtAuditTypeISM").prop('checked') == false) {
        $(".type option:contains('ISM')").hide();
    } else {
        $(".type option:contains('ISM')").show();
    }

    if ($("#txtAuditTypeISPS").prop('checked') == false) {
        $(".type option:contains('ISPS')").hide();
    } else {
        $(".type option:contains('ISPS')").show();
    }

    if ($("#txtAuditTypeMLC").prop('checked') == false) {
        $(".type option:contains('MLC')").hide();
    } else {
        $(".type option:contains('MLC')").show();
    }
}

function ValidateIAFStep2(ctr) {
    isvalid = true;
    if ($("input[name='AuditNote[" + count + "].Reference']").val() == "") {
        $("input[name='AuditNote[" + count + "].Reference']").parent().append('<label class="error" >Please Select Reference</label>')
        isvalid = false;
    }
    if ($("textarea[name='AuditNote[" + count + "].FullDescription']").val() == "") {
        $("textarea[name='AuditNote[" + count + "].FullDescription']").parent().append('<label class="error" >Please enter Full Description</label>')
        isvalid = false;
    }
    if ($("textarea[name='AuditNote[" + count + "].CorrectiveAction']").val() == "") {
        $("textarea[name='AuditNote[" + count + "].CorrectiveAction']").parent().append('<label class="error" >Please enter Corrective Action</label>')
        isvalid = false;
    }
    if ($("textarea[name='AuditNote[" + count + "].PreventativeAction']").val() == "") {
        $("textarea[name='AuditNote[" + count + "].PreventativeAction']").parent().append('<label class="error" >Please enter Preventative Action</label>')
        isvalid = false;
    }
    if ($("input[name='AuditNote[" + count + "].Rank']").val() == "") {
        $("input[name='AuditNote[" + count + "].Rank']").parent().append('<label class="error" >Please Enter Rank</label>')
        isvalid = false;
    }
    if ($("input[name='AuditNote[" + count + "].Name']").val() == "") {
        $("input[name='AuditNote[" + count + "].Name']").parent().append('<label class="error" >Please Enter Name</label>')
        isvalid = false;
    }
    if ($("input[name='AuditNote[" + count + "].TimeScale']").val() == "") {
        $("input[name='AuditNote[" + count + "].TimeScale']").parent().append('<label class="error" >Please Enter TimeScale</label>')
        isvalid = false;
    }
    if (isvalid) {
        if ($("#IAForm").valid()) {
            if (i < $('.notes').length - 1) {
                i++;
                getAuditNotes();
            }
            else {
                $.notify("You are on the last Audit Note.", "error");
            }
        }
    }
}
function EditNotes() {
    // RDBJ 02/02/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }
    $("#btnStart").trigger('click');
}
function getNumber(ctr) {
    // RDBJ 02/02/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }

    // RDBJ 02/02/2022
    // RDBJ 01/24/2022
    var url = RootUrl + 'InternalAuditForm/GetNumberForNotes';
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        data:
        {
            Ship: $("#Child option:selected").val(),
            UniqueFormID: _UniqueFormID
        },
        success: function (res) {
            Numbers = res.Numbers;
            SetNumberForNotes(Numbers);

            // RDBJ2 03/14/2022 Commented this due to avoid individual audit numbers
            /*
            var number;
            if ($(ctr).val() == 'ISM-Non Conformity') {
                number = ISMNonConformity;
                ISMNonConformity = parseInt(ISMNonConformity) + 1;
                $(ctr).parent().parent().find('input[type=hidden].number').attr('value', (parseInt(number) + 1)); // RDBJ 01/28/2022
                $(ctr).parent().parent().find('label.number').html((parseInt(number) + 1) + ' of  <span class="ISMNonConformity">' + ISMNonConformity + '</span>');
                $(ctr).parent().parent().find('input[type=hidden].number').attr('id', 'ISMNCN');
                $(".ISMNonConformity").text(ISMNonConformity);
            }
            if ($(ctr).val() == 'ISPS-Non Conformity') {
                number = ISPSNonConformity;
                ISPSNonConformity = parseInt(ISPSNonConformity) + 1;
                $(ctr).parent().parent().find('input[type=hidden].number').attr('value', (parseInt(number) + 1)); // RDBJ 01/28/2022
                $(ctr).parent().parent().find('label.number').html(parseInt(number) + 1 + ' of  <span class="ISPSNonConformity">' + ISPSNonConformity + '</span>');
                $(ctr).parent().parent().find('input[type=hidden].number').attr('id', 'ISPSNCN');
                $(".ISPSNonConformity").text(ISPSNonConformity);
            }
            if ($(ctr).val() == 'ISM-Observation') {
                number = ISMObservation;
                ISMObservation = parseInt(ISMObservation) + 1;
                $(ctr).parent().parent().find('input[type=hidden].number').attr('value', (parseInt(number) + 1)); // RDBJ 01/28/2022
                $(ctr).parent().parent().find('label.number').html(parseInt(number) + 1 + ' of  <span class="ISMObservation">' + ISMObservation + '</span>');
                $(ctr).parent().parent().find('input[type=hidden].number').attr('id', 'ISMOBS');
                $(".ISMObservation").text(ISMObservation);
            }
            if ($(ctr).val() == 'ISPS-Observation') {
                number = ISPSObservation;
                ISPSObservation = parseInt(ISPSObservation) + 1;
                $(ctr).parent().parent().find('input[type=hidden].number').attr('value', (parseInt(number) + 1)); // RDBJ 01/28/2022
                $(ctr).parent().parent().find('label.number').html(parseInt(number) + 1 + ' of  <span class="ISPSObservation">' + ISPSObservation + '</span>');
                $(ctr).parent().parent().find('input[type=hidden].number').attr('id', 'ISPSOBS');
                $(".ISPSObservation").text(ISPSObservation);
            }
            if ($(ctr).val() == 'MLC-Deficiency') {
                number = MLCDeficiency;
                MLCDeficiency = parseInt(MLCDeficiency) + 1;
                $(ctr).parent().parent().find('input[type=hidden].number').attr('value', (parseInt(number) + 1)); // RDBJ 01/28/2022
                $(ctr).parent().parent().find('lable.number').html(parseInt(number) + 1 + ' of  <span class="MLCDeficiency">' + MLCDeficiency + '</span>');
                $(ctr).parent().parent().find('input[type=hidden].number').attr('id', 'MLCDef');
                $(".MLCDeficiency").text(MLCDeficiency);
            }
            $(ctr).parent().parent().find('input[type=hidden].number').val(parseInt(parseInt(number) + 1)); // RDBJ 01/28/2022
            */
            // End RDBJ2 03/14/2022 Commented this due to avoid individual audit numbers

            // RDBJ2 03/14/2022
            number = ISMNonConformity;
            ISMNonConformity = parseInt(ISMNonConformity) + 1;
            $(ctr).parent().parent().find('input[type=hidden].number').attr('value', (parseInt(number) + 1));
            $(ctr).parent().parent().find('input[type=hidden].number').attr('id', 'CommonNumber');
            $(ctr).parent().parent().find('label.number').html(parseInt(number) + 1 + ' of  <span class="TotalAuditNumber">' + ISMNonConformity + '</span>');
            // End RDBJ2 03/14/2022

            resetAuditNotesNumberAfterAddOrRemove();

            $(ctr).css("border-color", "#d2d6de");
            IAFAutoSave();
            $("#lblAutoSave").hide();
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
    // RDBJ 01/24/2022
    // End RDBJ 02/02/2022
}
function addNewNotes() {
    // RDBJ 02/02/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }

    $(".notes").css('background-color', '#ffffff!important')
    $(".notes").attr('IsSelected', 'false')

    var count = $(".notes").length;
    var id = createGuid();
    var ddlAuditType;
    if (count > 0) {
        count--;
        ddlAuditType = $('select[name="AuditNote[' + count + '].Type"]');
        if (ddlAuditType.val() == "") {
            ddlAuditType.css("border-color", "red");
            $.notify("Please Select Audit Type from the selection!!", "error");
            return;
        }
        count++;
    }

    $(".AuditNotes").append(
        '<tr class="notes" id="' + id + '" index="' + count + '" IsSelected="true" style="background-color:#17a2b8;">' +
        '<td>' +
        '<input type="hidden" name="AuditNote[' + parseInt(count) + '].NotesUniqueID" value="' + id + '" />' +  // RDBJ 01/22/2022
        '<input type="hidden" name="AuditNote[' + parseInt(count) + '].Ship" value="' + $("#ShipName").val() + '" />' +  // RDBJ 01/22/2022
        '<input type="hidden" class="col-md-12 form-control number" name="AuditNote[' + parseInt(count) + '].Number" /><label  class="col-md-12 form-control number"></label>' +
        '</td>' +
        '<td>' +
        '<select onchange="getNumber(this);" id="AuditNote[' + parseInt(count) + '].Type" class="col-md-12 form-control type IAFAutoUpdateData" name="AuditNote[' + parseInt(count) + '].Type">' +
        '<option></option>' +
        '<option>ISM-Non Conformity</option>' +
        '<option>ISPS-Non Conformity</option>' +
        '<option>ISM-Observation</option>' +
        '<option>ISPS-Observation</option>' +
        '<option>MLC-Deficiency</option>' +
        '</select>' +
        '</td>' +
        '<td colspan="6"><input type="text" onchange="IAFAutoSave()" class="col-md-12 form-control desc IAFAutoUpdateData" name="AuditNote[' + parseInt(count) + '].BriefDescription" placeholder="Give the audit note a short descriptive title." /></td>' +
        '</tr>');

    AuditTypeListSelection();
}
function MoveNote(ctr) {
    // RDBJ 02/02/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }
    var row = $("tr[isselected='true']");

    if (ctr == "up") {
        if ($("tr[isselected='true']")[0] != $('.notes')[0])
            row.insertBefore(row.prev());
    }
    else {
        row.insertAfter(row.next());
    }

}
function ViewPrevious(ctr) {
    //if (ctr == 'n') {
    //    if (!isvalid) {
    //        alert('Current Audit note contain validation error')
    //        return;
    //    }
    //}
    $(".Listsection2").hide();
    $(".Listsection2footer").hide();
    $(".section1").show();
}
function removeNotes() {
    // RDBJ 02/02/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }

    // RDBJ 02/02/2022
    var IAFUFIdOrNoteId;
    var url = RootUrl + "InternalAuditForm/RemoveAuditsOrAuditNotes";
    IAFUFIdOrNoteId = $("tr[isselected='true']").attr("id");

    var obj = {};
    obj.id = IAFUFIdOrNoteId;
    obj.UniqueFormID = _UniqueFormID;   // RDBJ 02/04/2022
    obj.IsAudit = false;

    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Audit Note Deleted Successfully!", "success");
            //$("tr[isselected='true']").remove();
            $("tr[isselected='true']").find("td:eq(0)").find("input[type=hidden].number").removeAttr("id"); // RDBJ 01/28/2022
            $("tr[isselected='true']").css("display", "none");  // RDBJ 01/24/2022
            $("tr[isselected='true']").removeClass("notes");    // RDBJ 01/24/2022
            resetAuditNotesNumberAfterAddOrRemove();    // RDBJ 01/24/2022
            IAFAutoSave();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
    // RDBJ 02/02/2022
}
function createGuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
}

function getAuditNotes() {
    var Ship = $("#Child").val();
    var Location = $("#txtLocation").val();
    var AuditNo = $("#txtAuditNo").val();
    var Date = $("#txtDate").val();
    var Auditor = $("#ddlAuditor").val();
    if ($("#IAForm").valid()) {
        if (i < $('.notes').length) {
            var $detailDiv = $('.Listsection2');
            active = $('.notes')[i].id;
            var desc = $('.notes td:nth-child(3) input')[i].value;//$('.notes')[0].find('input.desc').val();
            if (desc != "") {
                typeVal = $('#' + active).find('select').val();
                if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation") {
                    $("#MLCTree").hide();
                    $("#SSPTree").hide();
                    $("#ISMTree").show();
                }
                if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation") {
                    $("#ISMTree").hide();
                    $("#MLCTree").hide();
                    $("#SSPTree").show();
                }
                if (typeVal == "MLC-Deficiency") {
                    $("#ISMTree").hide();
                    $("#SSPTree").hide();
                    $("#MLCTree").show();
                }
                count = $(".Listsection2 .form2").length;
                var url = RootUrl + 'InternalAuditForm/Details?id=' + active + '&count=' + count;
                var len = $('div.' + active).length;
                if (active != undefined) {
                    if (len >= 1) {
                        count = $('.notes')[i].index
                        $('.Listsection2').show();
                        $('.Listsection2footer').show();
                        $('.form2').hide();
                        $('div.' + active).show();
                        $(".section1").hide();
                    }
                    else {
                        $.get(url, function (data) {
                            $('.Listsection2').show();
                            $('.Listsection2footer').show();
                            $('.form2').hide();
                            $('.Listsection2').append(data);
                            $('div.' + active).show();
                            $(".section1").hide();
                            $('div.' + active).find('.BriefDescription').val(desc)
                            $('div.' + active).find('.ship').val($("#Child").val())
                            if ($('#txtAuditTypeISM').prop('checked') == true)
                                $('div.' + active).find('.ISM').prop('checked', true)
                            if ($('#txtAuditTypeISPS').prop('checked') == true)
                                $('div.' + active).find('.ISPS').prop('checked', true)
                            if ($('#txtAuditTypeMLC').prop('checked') == true)
                                $('div.' + active).find('.MLC').prop('checked', true)

                            $('div.' + active).find('.location').val($("#txtlocation").val())
                            $('div.' + active).find('.Auditor').val($("#ddlAuditor").val())
                            $('div.' + active).find('.Date').val($("#txtDate").val())
                            $('div.' + active).find('.Audit').val($("#ddlAudit").val())
                        });
                    }
                    $('div.' + active).find('.BriefDescription').val(desc)
                    $('div.' + active).find('.ship').val($("#Child").val())
                    if ($('#txtAuditTypeISM').prop('checked') == true)
                        $('div.' + active).find('.ISM').prop('checked', true)
                    if ($('#txtAuditTypeISPS').prop('checked') == true)
                        $('div.' + active).find('.ISPS').prop('checked', true)
                    if ($('#txtAuditTypeMLC').prop('checked') == true)
                        $('div.' + active).find('.MLC').prop('checked', true)

                    $('div.' + active).find('.location').val($("#txtlocation").val())
                    $('div.' + active).find('.Auditor').val($("#ddlAuditor").val())
                    $('div.' + active).find('.Date').val($("#txtDate").val())
                    $('div.' + active).find('.Audit').val($("#ddlAudit").val())
                }
            }
            else {
                bootbox.alert("Enter Description for notes");
            }
        }
    }
}

$(document).on('click', '.notes', function () {
    $(".notes").css('background-color', '#ffffff!important')
    $(".notes").attr('IsSelected', 'false')
    $(this).css('background-color', '#17a2b8!important')
    $(this).attr('IsSelected', 'true')
})
$(document).on("click", ".RefeData", function () {
    $("#ReferenceModal").modal('show');
    childrefe = true;
});

$(document).on("click", "#btnPrevious", function (e) {
    if ($("#IAForm").valid()) {
        if (i > 0) {
            i--;
            getAuditNotes();
        } else {
            $(".Listsection2").hide();
            $(".Listsection2footer").hide();
            $(".section1").show();
        }
    }
});

$(document).on("click", "#btnStart", function (e) {
    if ($("#IAForm").valid()) {
        i = 0;
        getAuditNotes();
    }
});

function RemoveFile() {
    $(".filelist[isselected='true'] input.IsActive").val(false);
    $(".filelist[isselected='true']").css('display', 'none');

    IAFAutoSave();  // RDBJ 02/05/2022
}
function selectFile(ctr) {
    if ($(ctr).closest('li').attr('IsSelected') == undefined || $(ctr).closest('li').attr('IsSelected') == "false") {
        $(ctr).closest('li').addClass('active');
        $(ctr).closest('li').attr('IsSelected', 'true');
        $("#btnremovefile").removeClass('disabled')
        $("#btnopenfile").removeClass('disabled')
        $(ctr).text('unselect')
    }
    else {
        $(ctr).closest('li').removeClass('active');
        $(ctr).closest('li').attr('IsSelected', 'false');
        $(ctr).text('select')
    }
    if ($(".filelist[isselected='true']").length == 0) {
        $("#btnremovefile").addClass('disabled')
        $("#btnopenfile").addClass('disabled')
    }
}
function fileUpload(ctr) {
    if (typeof (FileReader) != "undefined") {
        var reader = new FileReader();
        var notAllowType = "";
        var fileistoobig = "";

        var data = "";
        var current = $('.notes')[i].id;
        reader.onload = function (e) {
            for (var i = 0; i < ctr.files.length; i++) {
                if (ctr.files[i].type.indexOf('pdf') >= 0 ||
                    ctr.files[i].type.indexOf('image') >= 0 ||
                    ctr.files[i].type.indexOf('document') >= 0 ||
                    ctr.files[i].type.indexOf('xml') >= 0 ||
                    ctr.files[i].type.indexOf('sheet') >= 0) {

                    if (ctr.files[i].size > 4000000) {
                        fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] ";
                    } else {
                        var l = $('li[notesid=' + active + ']').length;
                        l = parseInt(l);
                        var picFile = e.target;
                        data = '<div class="filelist" ><div class="col-md-2">' +
                            '<input type="hidden" class="IsActive" name="AuditNote[' + count + '].AuditNotesAttachment.[' + l + '].IsActive" value="true" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment.[' + l + '].StorePath" value="' + picFile.result + '" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment.[' + l + '].FileName" value="' + ctr.files[i].name + '" />' +
                            '<a onclick="selectFile(this)">Select</a>' +
                            '</div>' +
                            '<div class="col-md-10">' +
                            '"' + ctr.files[i].name + '"' +
                            '</div></div>';
                        data = '<li class="list-group-item filelist" notesid="' + active + '"><div class="row"><div class="col-md-2">' +
                            '<input type="hidden" class="IsActive" name="AuditNote[' + count + '].AuditNotesAttachment[' + l + '].IsActive" value="true" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment[' + l + '].StorePath" value="' + picFile.result + '" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment[' + l + '].FileName" value="' + ctr.files[i].name + '" />' +
                            '<label onclick="selectFile(this)">Select</label></div > <div class="col-md-10">' + ctr.files[i].name + '</div></div ></li > ';
                        $("#fileinfo_" + count).append(data);
                        IAFAutoSave();
                    }
                }
                else {
                    notAllowType = notAllowType + " [" + ctr.files[i].name + "] ";
                }
            }
            if (notAllowType != "") {
                $("#modal-default p").text(notAllowType + " files types are not supported")
                $('#modal-default').modal('show');
            } else if (fileistoobig != "") {
                $("#modal-default p").text(fileistoobig + " File must be smaller than 4.0 MB")
                $('#modal-default').modal('show');
            }
        }
        reader.readAsDataURL($(ctr)[0].files[0]);
    } else {
        alert("This browser does not support FileReader.");
    }
}

function ValidateIAFForm() {

    validator = $("#IAForm").validate({
        rules: {
            "InternalAuditForm.ShipName": {
                required: true
            },
            "InternalAuditForm.Location": {
                required: true
            },
            "InternalAuditForm.AuditNo": {
                required: true
            },
            "InternalAuditForm.Date": {
                required: true,
                //date: true // RDBJ 01/20/2022
            },
            "InternalAuditForm.Auditor": {
                required: true,
            },
            "InternalAuditForm.AuditType": {
                required: true,
            },
        },
        messages: {
            "InternalAuditForm.ShipName": "Please Select Ship",
            "InternalAuditForm.Location": "Enter Location.",
            "InternalAuditForm.AuditNo": "Enter AuditNo",
            "InternalAuditForm.Date": "Please Enter Date",
            "InternalAuditForm.Auditor": "Please Select Auditor",
            "InternalAuditForm.AuditType": "Please You must select at least one!",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });

    $.validator.addMethod("InternalAuditForm.AuditType", function (value, elem, param) {
        return $(".InternalAuditForm.AuditType:checkbox:checked").length > 0;
    }, "You must select at least one!");
}

function printDiv() {
    $("input[type=text]").each(function () {
        var str = $(this).val();
        $(this).attr('value', str);
    });
    $("input[type=checkbox]").each(function () {
        var str = $(this).val();
        $(this).attr('checked', str);
    });
    $("textarea").each(function () {
        var str = $(this).val();
        $(this).text("");
        $(this).append(str);
    });

    $(".Listsection2").show();
    $(".AttachmentsDiv").hide();

    var divToPrint = "";
    // JSL 09/10/2022 commented
    /*
    $("#IAFFormPrint .form2").show();
    divToPrint = $("#IAFFormPrint").html();
    $("#IAFFormPrint .form2").hide();
    */
    //   divToPrint = document.getElementById("IAFFormPrint").innerHTML;
    // End JSL 09/10/2022 commented

    // JSL 09/10/2022
    $('textarea').replaceWith(function () {
        return '<span>' + $(this).text() + '</span>';
    });

    $('input[type="text"]').replaceWith(function () {
        return '<span>' + $(this).val() + '</span>';
    });

    $(".aviodPrint").hide();
    $("#IAForm .form2").show();
    divToPrint = $("#IAForm").html();
    $("#IAForm .form2").hide();
    // End JSL 09/10/2022

    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    var style = "@@media print {.breakDivPage { page-break-after: always; page-break-inside:auto   } }";
    style += "@@page {size: auto;margin: 5%;}";
    newWin.document.write('<html><head><title></title>');
    newWin.document.write('<style>' + style + '</style>');
    newWin.document.write('<link href="../Content/bootstrap/dist/css/bootstrap.css" rel="stylesheet" />');
    newWin.document.write('<link href="../Content/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />');
    newWin.document.write('<link rel="stylesheet" href="../Content/Custom/GIRForm.css" type="text/css" />');
    newWin.document.write('<link href="../Content/AdminLTE.min.css" rel="stylesheet" />');
    newWin.document.write('<link href="../Content/_all-skins.min.css" rel="stylesheet" />');
    newWin.document.write('<link href="../Content/Custom/SiteCustom.css" rel="stylesheet" />');
    newWin.document.write('</head><body onload="window.print()">');
    newWin.document.write(divToPrint);
    newWin.document.write('</body></html>');
    newWin.document.close();
    setTimeout(function () {
        newWin.close();
        location.reload();  // JSL 09/10/2022
    }, 5000);

    $(".AttachmentsDiv").show();
}

// RDBJ RDBJ 02/02/2022
function GetAuditNotesNumber() {
    var url = RootUrl + 'InternalAuditForm/GetNumberForNotes';
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        data:
        {
            Ship: $("#Child option:selected").val(),
            UniqueFormID: _UniqueFormID
        },
        success: function (res) {
            Numbers = res.Numbers;
            SetNumberForNotes(Numbers);
            $("#lblAutoSave").hide();
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
}
// End RDBJ 02/02/2022

// RDBJ RDBJ 02/02/2022
function SetNumberForNotes(Numbers) {
    if (Numbers != null && Numbers != undefined && Numbers.length > 0) {
        ISMNonConformity = Numbers[0];
        ISPSNonConformity = Numbers[1];
        ISMObservation = Numbers[2];
        ISPSObservation = Numbers[3];
        MLCDeficiency = Numbers[4];
    }
}
// End RDBJ 02/02/2022

// RDBJ RDBJ 02/02/2022
function HideAndShowTreeListInReferenceModal(typeValue) {
    if (typeValue == "ISM-Non Conformity" || typeValue == "ISM-Observation") {
        $("#MLCTree").hide();
        $("#SSPTree").hide();
        $("#ISMTree").show();
    }
    if (typeValue == "ISPS-Non Conformity" || typeValue == "ISPS-Observation") {
        $("#ISMTree").hide();
        $("#MLCTree").hide();
        $("#SSPTree").show();
    }
    if (typeValue == "MLC-Deficiency") {
        $("#ISMTree").hide();
        $("#SSPTree").hide();
        $("#MLCTree").show();
    }
}
// End RDBJ 02/02/2022

// RDBJ RDBJ 02/02/2022
function IAFAutoSave(
    IsUpdateShipForDeficiencies = false    // RDBJ 04/18/2022
) {
    $("#lblAutoSave").show();
    var url = RootUrl + 'InternalAuditForm/IAFAutoSave';
    var form = $("#IAForm");
    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        async: true,
        success: function (res) {
            $("#UniqueFormID").val(res.UniqueFormID);
            $("#FormVersion").val(res.FormVersion);   // RDBJ 02/04/2022

            // RDBJ 01/24/2022 wrapped in if
            if (_UniqueFormID == undefined) {
                _UniqueFormID = res.UniqueFormID;
                GetAuditNotesNumber();
            }
            $("#lblAutoSave").hide();

            // RDBJ 04/18/2022
            if (IsUpdateShipForDeficiencies) {
                UpdateDeficienciesShipWhenChangeFormShip();
            }
            // End RDBJ 04/18/2022
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
}
// End RDBJ 02/02/2022

// RDBJ RDBJ 02/02/2022
function resetAuditNotesNumberAfterAddOrRemove() {
    var startNumber = parseInt(500);

    // RDBJ2 03/14/2022 Commented this section due to avoid individual
    /*
    $("input[id=ISMNCN]").each(function () {
        startNumber = startNumber + 1;
        $(this).val(startNumber);
        $(this).parent().find("label").html(startNumber + ' of  <span class="ISMNonConformity">' + ISMNonConformity + '</span>');
    });
    $(".ISMNonConformity").text(startNumber);

    startNumber = parseInt(500);
    $("input[id=ISPSNCN]").each(function () {
        startNumber = startNumber + 1;
        $(this).val(startNumber);
        $(this).parent().find("label").html(startNumber + ' of  <span class="ISPSNonConformity">' + ISPSNonConformity + '</span>');
    });
    $(".ISPSNonConformity").text(startNumber);

    startNumber = parseInt(500);
    $("input[id=ISMOBS]").each(function () {
        startNumber = startNumber + 1;
        $(this).val(startNumber);
        $(this).parent().find("label").html(startNumber + ' of  <span class="ISMObservation">' + ISMObservation + '</span>');
    });
    $(".ISMObservation").text(startNumber);

    startNumber = parseInt(500);
    $("input[id=ISPSOBS]").each(function () {
        startNumber = startNumber + 1;
        $(this).val(startNumber);
        $(this).parent().find("label").html(startNumber + ' of  <span class="ISPSObservation">' + ISPSObservation + '</span>');
    });
    $(".ISPSObservation").text(startNumber);

    startNumber = parseInt(500);
    $("input[id=MLCDef]").each(function () {
        startNumber = startNumber + 1;
        $(this).val(startNumber);
        $(this).parent().find("label").html(startNumber + ' of  <span class="MLCDeficiency">' + MLCDeficiency + '</span>');
    });
    $(".MLCDeficiency").text(startNumber);
    */
    // End RDBJ2 03/14/2022 Commented this section due to avoid individual

    // RDBJ2 03/14/2022
    $("input[id=CommonNumber]").each(function () {
        startNumber = startNumber + 1;
        $(this).val(startNumber);
        $(this).parent().find("label").html(startNumber + ' of  <span class="TotalAuditNumber">' + startNumber + '</span>');
    });
    $(".TotalAuditNumber").text(startNumber - 500); // JSL 05/27/2022 added -500
    // End RDBJ2 03/14/2022
}
// End RDBJ 02/02/2022

// RDBJ 02/02/2022 commented this below function
/*
$(document).ready(function () {
    addNewNotes()

    $("#Child").change(function () {
        var url = RootUrl + "InternalAuditForm/getNextNumber";
        $.ajax({
            type: 'POST',
            dataType: 'json',
            async: false,
            url: url,
            data: { ship: $(this).val() },
            success: function (Data) {
                if (Data != null && Data != undefined && Data.length > 0) {
                    ISMNonConformity = Data[0].split(':')[1];
                    ISPSNonConformity = Data[1].split(':')[1];
                    ISMObservation = Data[2].split(':')[1];
                    ISPSObservation = Data[3].split(':')[1];
                    MLCDeficiency = Data[4].split(':')[1];
                }
            }
        });
    });

    $("#Child").trigger('change');

    $('body').on('focus', "#Date", function () {
        $(this).datepicker({
            format: 'dd/mm/yyyy', // RDBJ 01/20/2022 set small mm
            autoclose: true,
        });
    });

    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy', // RDBJ 11/15/2021 set small mm
        autoclose: true,
    });

    $("#btnselect").click(function () {
        if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation")
            $("input[name='AuditNote[" + count + "].Reference']").val($("#ISMTree li.node-selected").text());
        if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation")
            $("input[name='AuditNote[" + count + "].Reference']").val($("#SSPTree li.node-selected").text());
        if (typeVal == "MLC-Deficiency")
            $("input[name='AuditNote[" + count + "].Reference']").val($("#MLCTree li.node-selected").text());
        $("#ReferenceModal").modal('hide');
    })

    AuditTypeListSelection();
});
*/
// RDBJ 02/02/2022 commented this below function

// RDBJ 04/18/2022
function UpdateDeficienciesShipWhenChangeFormShip() {
    var dic = {};

    dic["UniqueFormID"] = _UniqueFormID;
    dic["Ship"] = _Ship;
    dic["FormType"] = 'IA';

    CommonServerPostApiCall(dic, "Forms", "PerformAction", str_API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS);
}
// End RDBJ 04/18/2022