// RDBJ 02/05/2022 added this script

var blnEditButton = false;
var _UniqueFormID = getUrlVars()["id"]; // RDBJ 11/17/2021
var _Ship;  // RDBJ 04/18/2022
var childrefe = true;
var typeVal = "";
var active
var count
var isvalid = true;
var ISMNonConformity, ISPSNonConformity, ISMObservation, ISPSObservation, MLCDeficiency;

var IAFAuditNoteFiles = {
    Files: []
};

// RDBJ 02/05/2021
$(document).ready(function () {
    $('div.form2').show();
    autosize(document.getElementsByTagName("textarea"));
    $('div.form2').hide();
    _Ship = $("#ddlIAFReportShipName option:selected").val();    // RDBJ 04/18/2022

    $("#btnEditIAF").click(function () {
        EditIAFAndAuditNotes();
    });

    $("#btnAddIAFAuditNotes").click(function () {
        AddIAFAuditNotes();
    });

    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
    });

    /*
    $("#btnselect").click(function () {
        if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation")
            $("input[name='Reference']").val($("#ISMTree li.node-selected").text());
        if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation")
            $("input[name='Reference']").val($("#SSPTree li.node-selected").text());
        if (typeVal == "MLC-Deficiency")
            $("input[name='Reference']").val($("#MLCTree li.node-selected").text());

        $("#ReferenceModal").modal('hide');
        $('#modal-AddNewAuditNote').modal('show');
    })
    */

    Numbers = Numbers.split(',');
    if (Numbers != null && Numbers != undefined && Numbers.length > 0) {
        ISMNonConformity = Numbers[0];
        ISPSNonConformity = Numbers[1];
        ISMObservation = Numbers[2];
        ISPSObservation = Numbers[3];
        MLCDeficiency = Numbers[4];
    }

    $("#ddlIAFReportShipName").prop('disabled', true); 

    $("#btnSubmitAuditNote").click(function () {
        ValidateIAFStep2('n');
    });

    if (isInspector.toLowerCase() == 'false' && isDraft.toLowerCase() == 'false') { // RDBJ2 05/10/2022 set && from || 
        $("#ddlIAFReportShipName").prop('disabled', true);
    } else {
        $("#ddlIAFReportShipName").prop('disabled', false);
    }

    $("#ddlIAFReportShipName").change(function () {
        $("#hdnShipName").val($("#ddlIAFReportShipName option:selected").val());
        _Ship = $("#ddlIAFReportShipName option:selected").val();    // RDBJ 04/18/2022
        AuditTypeListSelection();

        // RDBJ 04/18/2022
        if ($("#IAFForm").valid()) {
            IAFAutoSave(true);  // RDBJ 04/18/2022 Pass true
        }
    });

    // RDBJ2 05/10/2022 wrapped in if
    if (isInspector.toLowerCase() == 'true' && isDraft.toLowerCase() == 'true') {
        $(".IAFAutoUpdateData").bind("change", function (e) {
            e.preventDefault();
            if ($("#IAFForm").valid()) {
                IAFAutoSave();
            }
        });
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

    AuditTypeListSelection();
});
// End RDBJ 02/05/2021

// RDBJ 01/28/2022
$(document).on('click', '.notes', function () {
    $(".notes").css('background-color', '#ffffff!important')
    $(".notes").attr('IsSelected', 'false')
    $(this).css('background-color', '#17a2b8!important')
    $(this).attr('IsSelected', 'true')
})
// End RDBJ 01/28/2022

// RDBJ 02/05/2021
$(document).on('input', '#BriefDescription', function () {
    var element = $(this);
    $("input[name='" + element.attr("name") + "']").val(element.val());
});
// End RDBJ 01/28/2022

// RDBJ 11/17/2021
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}
// End RDBJ 11/17/2021

function ViewNotes(ctr
    , element   // RDBJ 01/28/2022
) {
    $(".notes").attr('IsSelected', 'false');
    $('div.form2').hide();
    $('div.' + ctr).show();
    $(".AuditNotes tr").css('background-color', '#ffffff');

    // RDBJ 01/28/2022
    $(element).parent().css('background-color', '#3c8dbc');
    $(element).parent().attr('IsSelected', 'true');
    $('div.' + ctr)[0].scrollIntoView({
        behavior: 'smooth'
    });
    // End RDBJ 01/28/2022
}
function DownloadFile(ctr, data) {

    var url = RootUrl + "InternalAuditForm/Download?file=" + ctr + "&name=" + data;;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        type: "GET",
        success: function () {
            window.location = url;
        }
    });
}

// RDBJ 02/05/2021
function EditIAFAndAuditNotes() {
    if (!blnEditButton) {
        $('.checkbox').css('pointer-events', 'auto');
        $('textarea').css('pointer-events', 'auto');
        $('input[type="text"]').css('pointer-events', 'auto');
        $('.notAllowtoEditinEditMode').css('pointer-events', 'none'); //RDBJ 11/18/2021
        $('#RefeData').css('pointer-events', 'auto'); //RDBJ 11/18/2021

        $("#btnEditIAF").text("Done");
        $('#btnAddIAFAuditNotes').css('display', 'block');
        $("#ddlIAFReportShipName").prop('disabled', false); // RDBJ 11/16/2021

        $('.typeInShortList').prop('disabled', false); //RDBJ 11/19/2021 typeInShortList //RDBJ 11/18/2021

        AuditTypeListSelection(); //RDBJ 11/19/2021
        blnEditButton = true;
    }
    else {
        $('.checkbox').css('pointer-events', 'none');
        $('textarea').css('pointer-events', 'none');
        $('input[type="text"]').css('pointer-events', 'none');
        $('#RefeData').css('pointer-events', 'none'); //RDBJ 11/18/2021

        $("#btnEditIAF").text("Edit");
        $('#btnAddIAFAuditNotes').css('display', 'none');
        $("#ddlIAFReportShipName").prop('disabled', true); // RDBJ 11/16/2021

        $('.typeInShortList').prop('disabled', true); //RDBJ 11/18/2021

        blnEditButton = false;
    }
}
// End RDBJ 02/05/2021

// RDBJ 02/05/2021
function AddIAFAuditNotes() {
    AuditTypeListSelection();
    $('#modal-AddNewAuditNote').modal('show');
}
// End RDBJ 02/05/2021

//RDBJ 11/15/2021
$(document).on("click", "#btnCloseReference", function () {
    //RDBJ 11/18/2021 Wrapped in if
    if ($('#btnselect').attr('onclick') == 'SelectReference(99)') {
        $("#ReferenceModal").modal('hide');
        $('#modal-AddNewAuditNote').modal('show');
    } else {
        $("#ReferenceModal").modal('hide');
    }
});
//End RDBJ 11/15/2021

// RDBJ 01/28/2022 Commented old code
/*
//RDBJ 11/15/2021
$(document).on("click", ".RefeData", function () {
    
    //RDBJ 11/16/2021
    if ($(".type option:selected").val() == "") {
        alert("Please select Type and try again!");
        return;
    }
    //End RDBJ 11/16/2021

    $('#modal-AddNewAuditNote').modal('hide');

    $("#btnselect").attr("onclick", "SelectReference(99)"); //RDBJ 11/18/2021
    $("#ReferenceModal").modal('show');
    childrefe = true;
});
*/
// End RDBJ 01/28/2022 Commented old code

function ValidateIAFStep2(ctr) {
    isvalid = true;
    if ($("input[id='txtReference']").val() == "") {
        $("input[id='txtReference']").parent().append('<label class="error" >Please Select Reference</label>')
        isvalid = false;
    }
    if ($("textarea[id='txtFullDescription']").val() == "") {
        $("textarea[id='txtFullDescription']").parent().append('<label class="error" >Please enter Full Description</label>')
        isvalid = false;
    }
    if ($("textarea[id='txtCorrectiveAction']").val() == "") {
        $("textarea[id='txtCorrectiveAction']").parent().append('<label class="error" >Please enter Corrective Action</label>')
        isvalid = false;
    }
    if ($("textarea[id='txtPreventativeAction']").val() == "") {
        $("textarea[id='txtPreventativeAction']").parent().append('<label class="error" >Please enter Preventative Action</label>')
        isvalid = false;
    }
    if ($("input[id='txtRank']").val() == "") {
        $("input[id='txtRank']").parent().append('<label class="error" >Please Enter Rank</label>')
        isvalid = false;
    }
    if ($("input[id='txtName']").val() == "") {
        $("input[id='txtName']").parent().append('<label class="error" >Please Enter Name</label>')
        isvalid = false;
    }
    if ($("input[id='txtTimeScale']").val() == "") {
        $("input[id='txtTimeScale']").parent().append('<label class="error" >Please Enter TimeScale</label>')
        isvalid = false;
    }
    if (isvalid) {
        if ($("#IAFForm").valid()) {
            SubmitAuditNote();
        }
    }
}

function RemoveFile() {
    $(".filelist[isselected='true'] input.IsActive").val(false);
    $(".filelist[isselected='true']").css('display', 'none');

    for (var i = 0; i < $(".filelist[isselected='true']").length; i++) {
        var fileName = $(".filelist[isselected='true'] div.col-md-10")[i].innerHTML;
        $.each(IAFAuditNoteFiles.Files, function (i) {
            if (IAFAuditNoteFiles.Files[i].FileName === fileName) {
                IAFAuditNoteFiles.Files.splice(i, 1);
                return false;
            }
        });
    }
}
function selectFile(ctr) {
    if ($(ctr).closest('li').attr('IsSelected') == undefined || $(ctr).closest('li').attr('IsSelected') == "false") {
        $(ctr).closest('li').addClass('active');
        $(ctr).closest('li').attr('IsSelected', 'true');
        $("#btnremovefile").removeClass('disabled')
        $("#btnopenfile").removeClass('disabled')
        $(ctr).text('Unselect')
    }
    else {
        $(ctr).closest('li').removeClass('active');
        $(ctr).closest('li').attr('IsSelected', 'false');
        $(ctr).text('Select')
    }
    if ($(".filelist[isselected='true']").length == 0) {
        $("#btnremovefile").addClass('disabled')
        $("#btnopenfile").addClass('disabled')
    }
}
function fileUpload(ctr) {
    if (typeof (FileReader) != "undefined") {
        var reader = new FileReader();
        var notAllowType = ""
        var fileistoobig = ""; //RDBJ 11/16/2021

        var data = "";
        reader.onload = function (e) {
            for (var i = 0; i < ctr.files.length; i++) {
                //RDBJ 11/16/2021 wrapped in if and put validation for files
                if (ctr.files[i].type.indexOf('pdf') >= 0 ||
                    ctr.files[i].type.indexOf('image') >= 0 ||
                    ctr.files[i].type.indexOf('document') >= 0 ||
                    ctr.files[i].type.indexOf('xml') >= 0 ||
                    ctr.files[i].type.indexOf('sheet') >= 0) {

                    //RDBJ 11/16/2021 validation for file size and wrapped in if
                    if (ctr.files[i].size > 4000000) {
                        fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] ";
                    } else {
                        var picFile = e.target;

                        data = '<li class="list-group-item filelist"><div class="row"><div class="col-md-2">' +
                            '<label onclick="selectFile(this)">Select</label></div > <div class="col-md-10">' + ctr.files[i].name + '</div></li > ';

                        IAFAuditNoteFiles.Files.push({
                            "FileName": ctr.files[i].name,
                            "StorePath": picFile.result,
                        });
                    }
                }

                $("#fileinfo_").append(data)
            }
            if (notAllowType != "") {
                $("#modal-default p").text(notAllowType + " files types are not supported")
                $('#modal-default').modal('show');
            }
            //RDBJ 11/16/2021 added else if
            else if (fileistoobig != "") {
                $("#modal-default p").text(fileistoobig + " File must be smaller than 4.0 MB")
                $('#modal-default').modal('show');
            }
        }
        reader.readAsDataURL($(ctr)[0].files[0]);
    } else {
        alert("This browser does not support FileReader.");
    }
}
//End RDBJ 11/15/2021

//RDBJ 11/16/2021
function getNumber(ctr) {
    // RDBJ 01/24/2022
    var url = RootUrl + 'InternalAuditForm/GetNumberForNotes';
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        data:
        {
            Ship: $("#ddlIAFReportShipName option:selected").val(),
            UniqueFormID: _UniqueFormID
        },
        success: function (res) {
            Numbers = res.Numbers;
            SetNumberForNotes(Numbers);

            var number;
            // RDBJ2 03/14/2022 Commented this due to avoid individual audit numbers
            /*
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
                $(ctr).parent().parent().find('label.number').html(parseInt(number) + 1 + ' of  <span class="MLCDeficiency">' + MLCDeficiency + '</span>');
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
            IAFAutoSave(); // RDBJ 01/28/2022
            $(ctr).css("border-color", "#d2d6de");
            $("#lblAutoSave").hide();
            IAFAutoSave(); // RDBJ 01/28/2022
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
    AuditTypeListSelection();
    typeVal = $(ctr).val();
    HideAndShowTreeListInReferenceModal(typeVal); //RDBJ 11/18/2021
}

//RDBJ 11/18/2021
function HideAndShowTreeListInReferenceModal(typeValue) {
    //RDBJ 11/16/2021
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
    //End RDBJ 11/16/2021
}
//End RDBJ 11/18/2021

function AuditTypeListSelection() {
    if ($("#txtAuditTypeISM").prop('checked') == false) {
        $(".type option:contains('ISM')").hide();
        $(".typeInShortList option:contains('ISM')").hide(); //RDBJ 11/19/2021
    } else {
        $(".type option:contains('ISM')").show();
        $(".typeInShortList option:contains('ISM')").show(); //RDBJ 11/19/2021
    }

    if ($("#txtAuditTypeISPS").prop('checked') == false) {
        $(".type option:contains('ISPS')").hide();
        $(".typeInShortList option:contains('ISPS')").hide(); //RDBJ 11/19/2021
    } else {
        $(".type option:contains('ISPS')").show();
        $(".typeInShortList option:contains('ISPS')").show(); //RDBJ 11/19/2021
    }

    if ($("#txtAuditTypeMLC").prop('checked') == false) {
        $(".type option:contains('MLC')").hide();
        $(".typeInShortList option:contains('MLC')").hide(); //RDBJ 11/19/2021
    } else {
        $(".type option:contains('MLC')").show();
        $(".typeInShortList option:contains('MLC')").show(); //RDBJ 11/19/2021
    }
}
//End RDBJ 11/16/2021

//RDBJ 11/16/2021
function SubmitAuditNote() {
    $("#lblAutoSave").show();

    var url = RootUrl + 'IAFList/AddIAFAuditNote';

    var Modal = {
        UniqueFormID: _UniqueFormID,
        Number: $("#hdnAuditNoteNumber").val(),
        Type: $(".type option:selected").val(),
        BriefDescription: $("#txtBriefDescription").val(),
        Reference: $("#txtReference").val(),
        FullDescription: $("#txtFullDescription").val(),
        CorrectiveAction: $("#txtCorrectiveAction").val(),
        PreventativeAction: $("#txtPreventativeAction").val(),
        Rank: $("#txtRank").val(),
        Name: $("#txtName").val(),
        TimeScale: $("#txtTimeScale").val(),
        Ship: $("#ddlIAFReportShipName option:selected").val(),
        AuditNotesAttachment: IAFAuditNoteFiles.Files,
    };

    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ Modal: Modal }),
        success: function (res) {
            IAFAuditNoteFiles.Files = [];
            $("#lblAutoSave").hide();
            window.location.reload(); //RDBJ 11/17/2021
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });

}
//End RDBJ 11/16/2021

//RDBJ 11/18/2021
function SelectReferenceFromAuditNote(noteIndex) {
    var value = $("select[name='AuditNote[" + noteIndex + "].Type']").val();
    typeVal = value;
    HideAndShowTreeListInReferenceModal(value);

    $("#btnselect").attr("onclick", "SelectReference(" + noteIndex + ")");
    $("#ReferenceModal").modal('show');
}
//End RDBJ 11/18/2021

//RDBJ 11/18/2021
function SelectReference(noteIndex) {
    if (noteIndex == 99) {
        if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='Reference']").val($("#ISMTree li a.ui-state-active").text());
            }
            else {
                $("input[name='Reference']").val($("#ISMTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }
            
        if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='Reference']").val($("#SSPTree li a.ui-state-active").text());
            }
            else {
                $("input[name='Reference']").val($("#SSPTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }
            
        if (typeVal == "MLC-Deficiency") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='Reference']").val($("#MLCTree li a.ui-state-active").text());
            }
            else {
                $("input[name='Reference']").val($("#MLCTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }

        $("#ReferenceModal").modal('hide');
        $('#modal-AddNewAuditNote').modal('show');
    }
    else {
        
        if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#ISMTree li a.ui-state-active").text());
            }
            else {
                $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#ISMTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }
            
        if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#SSPTree li a.ui-state-active").text());
            }
            else {
                $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#SSPTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }
            
        if (typeVal == "MLC-Deficiency") {
            // JSL 05/22/2022 wrrapped in if
            if (IsLatestReferencesFound) {
                $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#MLCTree li a.ui-state-active").text());
            }
            else {
                $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#MLCTree li.node-selected").text());
            }
            // End JSL 05/22/2022 wrrapped in if
        }

        // RDBJ 01/28/2022
        /*
        if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation")
            $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#ISMTree li a.ui-state-active").text());
        if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation")
            $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#SSPTree li a.ui-state-active").text());
        if (typeVal == "MLC-Deficiency")
            $("input[name='AuditNote[" + noteIndex + "].Reference']").val($("#MLCTree li a.ui-state-active").text());
        */
        // End RDBJ 01/28/2022

        $("#ReferenceModal").modal('hide');
    }

    $("#btnselect").attr("onclick", "SelectReference(99)");

    IAFAutoSave();  // RDBJ 01/28/2022
}
//End RDBJ 11/18/2021

//RDBJ 11/18/2021
function IAFAutoSave(
    IsUpdateShipForDeficiencies = false    // RDBJ 04/18/2022
) {
    if (isInspector.toLowerCase() == 'false' && isDraft.toLowerCase() == 'false')
        return;

    var url = RootUrl + 'InternalAuditForm/IAFAutoSave';

    $("#lblAutoSave").show();
    var IAFForm = $("#IAFForm");
    //RDBJ 11/19/2021
    $.ajax({
        url: url,
        type: 'POST',
        data: IAFForm.serialize(),
        async: true,
        success: function (res) {
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
    //End RDBJ 11/19/2021
}
//End RDBJ 11/18/2021

// RDBJ 01/27/2022
function createGuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
}
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
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
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
function addNewNotes() {
    // RDBJ 01/22/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }

    $(".notes").css('background-color', '#ffffff!important');
    $(".notes").attr('IsSelected', 'false');

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
        '<tr class="notes" id="' + id + '" index="' + count + '">' +
        '<td width="2%"><p class="m-5 text-center cursor-pointer" title="Delete Notes" onclick="removeNotes(\'' + id + '\', this)"><i class="fa fa-fw fa-remove"></i></p></td>' +
        '<td width="10%">' +
        '<input type="hidden" name="AuditNote[' + parseInt(count) + '].NotesUniqueID" value="' + id + '" />' +  // RDBJ 01/22/2022
        '<input type="hidden" name="AuditNote[' + parseInt(count) + '].Ship" value="' + $("#ddlIAFReportShipName option:selected").val() + '" />' +  // RDBJ 01/22/2022
        '<input type="hidden" class="col-md-12 form-control number" name="AuditNote[' + parseInt(count) + '].Number" /><label  class="col-md-12 form-control number"></label>' +
        '</td>' +
        '<td width="15%">' +
        '<select onchange="getNumber(this);" id="AuditNote[' + parseInt(count) + '].Type" class="col-md-12 form-control type IAFAutoUpdateData" name="AuditNote[' + parseInt(count) + '].Type">' +
        '<option></option>' +
        '<option>ISM-Non Conformity</option>' +
        '<option>ISPS-Non Conformity</option>' +
        '<option>ISM-Observation</option>' +
        '<option>ISPS-Observation</option>' +
        '<option>MLC-Deficiency</option>' +
        '</select>' +
        '</td>' +
        '<td width="65%" colspan="5"><input type="text" onchange="IAFAutoSave()" class="col-md-12 form-control BriefDescription IAFAutoUpdateData" id="BriefDescription" name="AuditNote[' + parseInt(count) + '].BriefDescription" placeholder="Give the audit note a short descriptive title." /></td>' +
        '<td width="2%" class="cursor-pointer" onclick="ViewNotes(\'' + id + '\', this)"><p class="m-5 text-center"><i class="fa fa-fw fa-th-list"></i></p></td>' +
        '</tr>');

    var url = RootUrl + 'InternalAuditForm/EditStep2Details?id=' + id + '&count=' + count;
    $.get(url, function (data) {
        $('.Listsection2').show();
        $('.form2').hide();
        $('.Listsection2').append(data);
        $('div.' + id).show();
    });
    AuditTypeListSelection();
}
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
function EditNotes() {
    // RDBJ 01/22/2022
    if ($("#UniqueFormID").val() == "") {
        $.notify("Please add Audit Details first!", "error");
        return;
    }
    $("#btnStart").trigger('click');
}
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
function removeNotes(IAFUFIdOrNoteId, ctr) {
    var url = RootUrl + "InternalAuditForm/RemoveAuditsOrAuditNotes";

    var obj = {};
    obj.id = IAFUFIdOrNoteId;
    obj.UniqueFormID = _UniqueFormID;
    obj.IsAudit = false;

    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $(ctr).parent().next().find("input[type=hidden].number").removeAttr("id"); // RDBJ 01/28/2022
            $(ctr).parent().parent().css("display", "none");
            $(ctr).parent().parent().removeClass("notes");
            $('div.' + IAFUFIdOrNoteId).hide();

            resetAuditNotesNumberAfterAddOrRemove();
            IAFAutoSave();

            $.notify("Audit Note Deleted Successfully!", "success");
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
function MoveNote(ctr) {
    // RDBJ 01/22/2022
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
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
function SetNumberForNotes(Numbers) {
    if (Numbers != null && Numbers != undefined && Numbers.length > 0) {
        ISMNonConformity = Numbers[0];
        ISPSNonConformity = Numbers[1];
        ISMObservation = Numbers[2];
        ISPSObservation = Numbers[3];
        MLCDeficiency = Numbers[4];
    }
}
// End RDBJ 01/27/2022

// RDBJ 01/27/2022
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
// End RDBJ 01/27/2022

// RDBJ 01/28/2022
function fileUploadEdit(noteId, count, ctr) {
    if (typeof (FileReader) != "undefined") {
        var reader = new FileReader();
        var notAllowType = "";
        var fileistoobig = "";

        var data = "";
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
                        var l = $('li[notesid=' + noteId + ']').length;
                        l = parseInt(l);
                        var picFile = e.target;
                        data = '<div class="filelist" ><div class="col-md-2">' +
                            '<input type="hidden" class="IsActive" name="AuditNote[' + count + '].AuditNotesAttachment.[' + l + '].IsActive" value="true" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment.[' + l + '].StorePath" value="' + picFile.result + '" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment.[' + l + '].FileName" value="' + ctr.files[i].name + '" />' +
                            '<a onclick="selectFileEdit(this)">Select</a>' +
                            '</div>' +
                            '<div class="col-md-10">' +
                            '"' + ctr.files[i].name + '"' +
                            '</div></div>';
                        data = '<li class="list-group-item filelist" notesid="' + noteId + '"><div class="row"><div class="col-md-2">' +
                            '<input type="hidden" class="IsActive" name="AuditNote[' + count + '].AuditNotesAttachment[' + l + '].IsActive" value="true" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment[' + l + '].StorePath" value="' + picFile.result + '" />' +
                            '<input type="hidden" name="AuditNote[' + count + '].AuditNotesAttachment[' + l + '].FileName" value="' + ctr.files[i].name + '" />' +
                            '<label onclick="selectFileEdit(this)">Select</label></div > <div class="col-md-10">' + ctr.files[i].name + '</div></div ></li > ';
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
// End RDBJ 01/28/2022

// RDBJ 01/28/2022
function RemoveFileEdit() {
    $(".filelist[isselected='true'] input.IsActive").val(false);
    $(".filelist[isselected='true']").css('display', 'none');
    IAFAutoSave();
    $("button[id^='btnremovefile']").addClass('disabled');
    $("button[id^='btnopenfile']").addClass('disabled');
}
// End RDBJ 01/28/2022

// RDBJ 01/28/2022
function selectFileEdit(ctr) {
    if ($(ctr).closest('li').attr('IsSelected') == undefined || $(ctr).closest('li').attr('IsSelected') == "false") {
        $(ctr).closest('li').addClass('active');
        $(ctr).closest('li').attr('IsSelected', 'true');
        //$("#btnremovefile").removeClass('disabled');
        $("button[id^='btnremovefile']").removeClass('disabled');
        //$("#btnopenfile").removeClass('disabled');
        $("button[id^='btnopenfile']").removeClass('disabled');
        $(ctr).text('unselect');
    }
    else {
        $(ctr).closest('li').removeClass('active');
        $(ctr).closest('li').attr('IsSelected', 'false');
        $(ctr).text('select');
    }
    if ($(".filelist[isselected='true']").length == 0) {
        $("#btnremovefile").addClass('disabled');
        $("#btnopenfile").addClass('disabled');
    }
}
// End RDBJ 01/28/2022

// RDBJ 01/28/2022
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
    $("#IAFForm .form2").show();
    divToPrint = $("#IAFForm").html();
    $("#IAFForm .form2").hide();
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
// End RDBJ 01/28/2022

// RDBJ 04/18/2022
function UpdateDeficienciesShipWhenChangeFormShip() {
    var dic = {};

    dic["UniqueFormID"] = _UniqueFormID;
    dic["Ship"] = _Ship;
    dic["FormType"] = 'IA';

    CommonServerPostApiCall(dic, "Forms", "PerformAction", str_API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS);
}
// End RDBJ 04/18/2022