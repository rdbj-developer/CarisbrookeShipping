var isInternetAvailable = false;
var isDraft = 'true';
var appMode = "offline";

var oTable; // RDBJ 01/15/2022 This is only for deficiencies
var oSettings; // RDBJ 01/15/2022 This is only for deficiencies

var _Ship, _UniqueFormID;   // RDBJ 04/18/2022

$(document).ready(function () {
    autosize(document.getElementsByTagName("textarea")); // RDBJ 01/20/2022
    
    if (isInspector.toLowerCase() == 'false') {
        $("#ddlGIReportShipName").prop('disabled', true);
    } else {
        $("#ddlGIReportShipName").prop('disabled', false);
    }

    _Ship = $("#ddlGIReportShipName option:selected").val();  // RDBJ 04/18/2022

    $(".PageSection").hide();
    ValidateGIRForm();

    var height = $(window).height();
    $(".leftContent").height(height - 65);  // RDBJ 02/26/2022 set 65 // RDBJ 02/19/2022
    $(".rightContent").height(height - 65); // RDBJ 02/26/2022 set 65 // RDBJ 02/19/2022

    $(".section1").show();
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',   // JSL 07/06/2022
        autoclose: true
    });

    $('#Date').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });

    $('.txtDateRaised').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy', // RDBJ 01/15/2022
    });

    $('.txtDateClosed').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy', // RDBJ 01/15/2022
    });

    $('body').on('focus', ".txtDateRaised,.txtDateClosed", function () {
        $(this).datepicker({
            format: 'dd/mm/yyyy'
        });
    });

    Info();
    $(".SIRData").bind("change", function (e) {
        if (isInspector == 'True') {
            e.preventDefault();
            setChnageValue(false, false);

            RemoveRequiredStarsIfValues(); //RDBJ 10/11/2021

            //RDBj 10/11/2021 Wrapped in If
            if ($("#SIRForm").valid()) {
                SIRAutoSave();
            }
        }
    });

    // RDBJ 04/18/2022
    $("#ddlGIReportShipName").change(function () {
        _Ship = $("#ddlGIReportShipName option:selected").val();

        if ($("#SIRForm").valid()) {
            SIRAutoSave(true);
        }
    });
    // End RDBJ 04/18/2022

    $('.SIRData').bind('contextmenu', function (e) {
        e.preventDefault();
    });
    $(".NavButton").click(function () {
        $("button").removeClass("active"); // RDBJ 02/19/2022
        $(this).addClass("active"); // RDBJ 02/19/2022
        var lblValue = $(this).attr("data-lbl-text");
        var hideSection = $(this).attr("data-lbl-show");
        $(".PageSection").hide();
        $("." + hideSection).show();
        $("#lblSubHeading").text(lblValue);
        document.querySelector('.leftContent').scrollIntoView({
            behavior: 'smooth'
        });
    })

    $("#section19").click(function () {
        var url = RootUrl + 'Forms/_SIR_DeficienciesSection';
        var FormId = $("#UniqueFormID").val();
        $.ajax({
            type: "GET",
            url: url + "?id=" + FormId,
        }).done(function (r) {
            $(".DefSectionContent").html(r);
        }).fail(function (e) {
            console.log(e.responseText);
        });
    });

    jQuery(document).on('click focus', '.SectionDiv textarea', function (e) { // RDBJ 01/20/2022 change input[type=text] to textarea
        $('[data-toggle=popover]').popover('hide');    // RDBJ 02/19/2022
        var isNoteAvailable = false;
        //$(".material-icons").remove();    // RDBJ 04/02/2022
        $("#idAddNote").remove();    // RDBJ 04/02/2022
        var no = $(this).parent().parent().children()[0].textContent.trim();
        if (no != undefined && no != null) {
            $('.notetable > tbody  > tr')
                .not('.hide')   // RDBJ 04/02/2022
                .each(function () {
                var tr = $(this);
                var td = $(tr).find("td:eq(1)").find("input").val();
                //var td = $(tr).find("td:eq(1)").find("textarea").val(); // RDBJ2 04/02/2022 commented this line // RDBJ 01/20/2022
                if (no == td) {
                    isNoteAvailable = true;
                }
                console.log(td);
            });
        }
        if (isNoteAvailable) {
            $(this).parent().next().html('<i id="idViewNote" data-toggle="tooltip" data-placement="left" title="View note" class="material-icons" onclick="viewNote(this)">note</i>')
        }
        else {
            //$(this).parent().next().html('<i id="idAddNote" data-toggle="tooltip" data-placement="left" title="Add a note" class="material-icons" onclick="addNote(this)">note_add</i>') // RDBJ 02/18/2022 commented this line
            $(this).parent().next().html('<i id="idAddNote" data-toggle="tooltip" data-placement="left" title="Add a Note/Corrective" class="material-icons" onclick="addNote(this);">note_add</i>')    // RDBJ 02/18/2022
        }
        Info()
    });

    jQuery(document).on('focusout', '.PageSection .form-control', function (e) {
        //$(this).parent().next().html('')
    });
    bindContextMenu();

    $("#btnSave").click(function () {
        //RDBj 10/11/2021 Wrapped in If
        if ($("#SIRForm").valid()) {
            SIRAutoSave();
        }
    });

    RemoveRequiredStarsIfValues(); //RDBJ 10/11/2021
});
function SIRAutoSave(
    IsUpdateShipForDeficiencies = false    // RDBJ 04/18/2022
) {
    var url = RootUrl + 'Forms/SIRAutoSave';
    $("#lblAutoSave").show();
    var form = $("#SIRForm");
    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        async: true,
        success: function (res) {
            console.log(res);
            $("#SIRFormID").val(res.SIRFormID);
            $("#OfficeSIRFormID").val(res.OfficeSIRFormID);
            $("#lblAutoSave").hide();
            $("#UniqueFormID").val(res.UniqueFormID);
            _UniqueFormID = res.UniqueFormID;   // RDBJ 04/18/2022
            $("#FormVersion").val(res.FormVersion);

            // RDBJ 04/18/2022
            if (IsUpdateShipForDeficiencies) {
                UpdateDeficienciesShipWhenChangeFormShip();
            }
            // End RDBJ 04/18/2022
        },
        error: function (err) {
            console.log(err);
            $("#lblAutoSave").hide();
        }
    });
}
function bindContextMenu() {
    if (isInspector == 'True') {
        menuDeficiency = $("#menu").kendoContextMenu({
            target: ".addDeficiencies",
            //filter: "tr",
            animation: {
                open: { effects: "fadeIn" },
                duration: 500
            },
            select: function (e) {
                $("#lblAutoSave").show();

                // RDBJ 01/15/2022
                if ($("#UniqueFormID").val() == "") {
                    $("#lblAutoSave").hide();
                    $.notify("Please add General Details first in Inspection Details!", "error");
                    return;
                }

                if ($("#UniqueFormID").val() != "") {
                    var url = RootUrl + 'Forms/AddGIRDeficiencies';
                    var itemno = "";
                    var deficienciesTd = $(e.target).closest('tr').find('td.clsDeficiencies');
                    var deficiencies = $(deficienciesTd).find('.SIRData').val();
                    var section = "";
                    itemno = $(e.target).closest('tr').find('td.clsItemNo').text().trim();
                    section = $($(e.target).parent().closest('.clsSection')[0]).find('.SectionHead u').text();
                    if (deficiencies == "") {
                        $.notify("Please enter comment", "error");
                        $(deficienciesTd).find('.SIRData').focus();
                        $("#lblAutoSave").hide();
                        return false;
                    }
                    var Modal = {
                        GIRFormID: $("#SIRFormID").val(),
                        OfficeGIRFormID: $("#OfficeSIRFormID").val(),
                        ItemNo: itemno,
                        Deficiency: deficiencies,
                        Section: section,
                        ReportType: "SI",
                        Priority: 12, // RDBJ 01/15/2022
                        Ship: $("#ddlGIReportShipName option:selected").val(),
                        UniqueFormID: $("#UniqueFormID").val(),
                        DateRaised: $("#Date").val(),   // JSL 10/15/2022
                    }

                    $.ajax({
                        url: url,
                        type: 'POST',
                        async: true,
                        data: Modal,
                        success: function (data) {
                            console.log(data);

                            // RDBJ 02/18/2022
                            if (data.DeficienciesUniqueID != "") {
                                $(e.target).closest('tr').find('td.notes_column').attr("data-deficiencies-id", data.DeficienciesUniqueID);
                            }

                            $("#FormVersion").val(data.FormVersion);    // RDBJ 02/19/2022

                            $.notify("Actionable Item Added Successfully", "success");  // RDBJ 02/18/2022

                            $("#lblAutoSave").hide();
                            itemNumber = "";
                        },
                        error: function (err) {
                            console.log(err);
                            $("#lblAutoSave").hide();
                        }
                    });
                }
                else {
                    $.notify("Please save form data", "error");
                }
                $("#lblAutoSave").hide();
            }
        }).data('kendoContextMenu');
    }
    else {
        $("#menu").hide();
    }
}
function setChnageValue(notes, additionNotes, defiChange) {
    $("#NotesChanged").val(notes);
    $("#AdditionalNotesChanged").val(additionNotes);
    $("#DeficienciesChanged").val(defiChange);

}
function printDiv() {
    var className = $(".PageSection:Visible").attr("class");
    className = className.replace("PageSection", "");
    className = className.replace("SectionDiv", "");
    className = className.replace("   clsSection", ""); // RDBJ 01/15/2022
    className = className.trim();

    // RDBJ 02/18/2022 set hide header and footer of Section 19
    $("#tblActionableItems_length").addClass("aviodPrint");
    $("#tblActionableItems_filter").addClass("aviodPrint");
    $("#tblActionableItems_info").addClass("aviodPrint");
    $("#tblActionableItems_paginate").addClass("aviodPrint");
    // End RDBJ 02/18/2022 set hide header and footer of Section 19

    $(".SectionComplete").hide();
    $(".aviodPrint").hide();
    $("#lblSuperintended").html("<strong>Superintended: </strong>" + $("#Superintended option:selected").text());
    $("#lblMaster").html("<strong>Master: </strong>" + $("#Master").val());
    $("#lblship").html("<strong>Ship :</strong>" + $("#ShipName option:selected").text());
    $("#lblport").html("<strong>Port :</strong>" + $("#Port").val());
    $("#lbldate").html("<strong>Date :</strong>" + $("#SuperintendedInspectionReport_Date").val());
    $("input[type=text]").each(function () {
        var str = $(this).val();
        $(this).attr('value', str);
    });
    $("textarea").each(function () {
        var str = $(this).val();
        $(this).text("");
        $(this).append(str);

        // RDBJ 01/20/2022
        //var val = this.innerHTML;
        //if (val != "") {
        //    $(this).css("width", "800px");
        //    this.removeAttribute("rows");
        //    this.addEventListener('focus', function () {
        //        autosize(this);
        //    });
        //    autosize(this);
        //}
        // End RDBJ 01/20/2022
    });

    // RDBJ 01/20/2022 this is only for DeficienciesSections
    $(".txtAreaDeficiencies").each(function () {
        var str = $(this).val();
        $(this).text("");
        $(this).append(str);
        var val = this.innerHTML;
        if (val != "") {
            $(this).css("width", "400px");
            this.removeAttribute("rows");
            this.addEventListener('focus', function () {
                autosize(this);
            });
            autosize(this);
        }
    });

    $("select").each(function (i) {
        var objSelected = $(this).find('option:selected');
        objSelected.attr('selected', 'selected');
    });

    // RDBJ 01/15/2022 this is only for DeficienciesSections
    oTable = $('#tblActionableItems').DataTable();
    oSettings = oTable.settings();

    oSettings[0]._iDisplayLength = oSettings[0].fnRecordsTotal();
    oTable.draw();

    $(".txtDateRaised").each(function (i) {
        $(this).addClass('txtSetDateWidthForPrint');
    });

    $(".txtDateClosed").each(function (i) {
        $(this).addClass('txtSetDateWidthForPrint');
    });

    //End RDBJ 01/15/2022

    // JSL 09/09/2022
    $('.clsDeficiencies').removeClass('text-center');
    $('.clsDeficiencies').addClass('text-left');
    $('.clsDeficiencies').attr('style', 'padding: 5px !important');

    $('textarea').replaceWith(function () {
        return '<span>' + $(this).text() + '</span>';
    });

    $('input[type="text"]').replaceWith(function () {
        return '<span>' + $(this).val() + '</span>';
    });
    // End JSL 09/09/2022

    var divToPrint = "";
    $(".PageSection").show();
    divToPrint = document.getElementById("forntpage").innerHTML;
    divToPrint += document.getElementById('SIRContent').innerHTML;

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
    $(".SectionComplete").show();
    $(".aviodPrint").show();
    newWin.document.close();
    $(".PageSection").hide();
    $("." + className).show();
    setTimeout(function () {
        newWin.close();

        // RDBJ 02/18/2022 set hide header and footer of Section 19
        $("#tblActionableItems_length").removeClass("aviodPrint");
        $("#tblActionableItems_filter").removeClass("aviodPrint");
        $("#tblActionableItems_info").removeClass("aviodPrint");
        $("#tblActionableItems_paginate").removeClass("aviodPrint");
        // End RDBJ 02/18/2022 set hide header and footer of Section 19

        // RDBJ 01/15/2022
        $(".txtDateRaised").each(function (i) {
            $(this).removeClass('txtSetDateWidthForPrint');
        });

        $(".txtDateClosed").each(function (i) {
            $(this).removeClass('txtSetDateWidthForPrint');
        });
        // End RDBJ 01/15/2022

        // RDBJ 01/20/2022 revert css changes
        //$("textarea").each(function () {
        //    var str = $(this).val();
        //    $(this).text("");
        //    $(this).append(str);
        //    var val = this.innerHTML;
        //    if (val != "") {
        //        $(this).css("width", "");
        //        this.removeAttribute("rows");
        //        this.addEventListener('focus', function () {
        //            autosize(this);
        //        });
        //        autosize(this);
        //    }
        //});
        // End RDBJ 01/20/2022

        // RDBJ 01/15/2022 this is only for DeficienciesSections
        $(".txtAreaDeficiencies").each(function () {
            $(this).css("width", "100%");
            this.addEventListener('focus', function () {
                autosize(this);
            });
            autosize(this);
        });

        // RDBJ 01/15/2022 this is only for DeficienciesSections
        oSettings[0]._iDisplayLength = 100;
        oTable.draw();
        // End RDBJ 01/15/2022
        location.reload();  // 09/09/2022
    }, 1000);
}

// RDBJ 01/15/2022
function SetDeficienciesDataForPrintAndPrintIt() {
    var url = RootUrl + 'Forms/_SIR_DeficienciesSection';
    var FormId = $("#UniqueFormID").val();
    $.ajax({
        type: "GET",
        url: url + "?id=" + FormId
    }).done(function (r) {
        $(".DefSectionContent").html(r);
        printDiv();
    }).fail(function (e) {
        console.log(e.responseText);
    });
}
// End RDBJ 01/15/2022

// RDBJ2 04/02/2022
function createGuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
}
// End RDBJ2 04/02/2022

function removeNote(ctr, isFromNotes) {
    // RDBJ 01/20/2022
    if ($("#UniqueFormID").val() == "") {
        $("#lblAutoSave").hide();
        $.notify("Please add General Details first in Inspection Details!", "error");
        return;
    }

    // RDBJ2 04/02/2022
    var NotesUniqueID = $(ctr).attr("data-id");

    var dic = {};
    dic["NotesUniqueID"] = NotesUniqueID;
    dic["UniqueFormID"] = $("#UniqueFormID").val();
    dic["IsSIRAdditionalNote"] = !isFromNotes;

    CommonServerPostApiCall(dic, "Forms", "PerformAction", str_API_DELETESIRNOTEORSIRADDITIONALNOTE);
    // End RDBJ2 04/02/2022

    // RDBJ2 04/02/2022 Commented
    /*
    $(ctr).closest('tr').remove();
    if (isFromNotes) {
        setChnageValue(true, false);
    }
    else {
        setChnageValue(false, true);
    }
    SIRAutoSave();
    */
    // End RDBJ2 04/02/2022 Commented
}
function onChangeNoteValue() {
    // RDBJ 01/20/2022
    if ($("#UniqueFormID").val() == "") {
        $("#lblAutoSave").hide();
        $.notify("Please add General Details first in Inspection Details!", "error");
        return;
    }
    if (isInspector == 'True') {
        setChnageValue(true, false);
        SIRAutoSave();
    }
}
function onChangeAdditionalNoteValue() {
    // RDBJ 01/20/2022
    if ($("#UniqueFormID").val() == "") {
        $("#lblAutoSave").hide();
        $.notify("Please add General Details first in Inspection Details!", "error");
        return;
    }
    if (isInspector == 'True') {
        setChnageValue(false, true);
        SIRAutoSave();
    }
}
function addNote(ctr) {
    // RDBJ 01/20/2022
    if ($("#UniqueFormID").val() == "") {
        $("#lblAutoSave").hide();
        $.notify("Please add General Details first in Inspection Details!", "error");
        return;
    }

    // RDBJ 02/18/2022
    $(ctr).attr("data-toggle", "popover");
    $(ctr).attr("title", "Add Corrective Action");  // RDBJ 02/19/2022

    $("[data-toggle=popover]").popover({
        trigger: 'manual',
        placement: "left",
        html: true,
        title: "Add Corrective Action",
        content: function () {
            return $('#popover-corrective-action-content').html();
        },
    });
    // RDBJ 02/18/2022

    if (isInspector == 'True') {
        var no = $(ctr).parent().parent().children()[0].textContent.trim();
        var DeficienciesUniqueID = $(ctr).parent().parent().children()[4].getAttribute("data-deficiencies-id"); // RDBJ 02/18/2022
        var Section = $($(ctr).parent().closest('.clsSection')[0]).find('.SectionHead u').text(); // RDBJ 02/18/2022

        // RDBJ 02/18/2022
        if (DeficienciesUniqueID != ""
            && DeficienciesUniqueID != null
        ) {
            $("#hdnDeficienciesUniqueID").val(DeficienciesUniqueID);    // RDBJ 02/18/2022
            $(ctr).prop('popShown', true).popover('show');
        }
        else {
            CheckDeficiencyAddedOrNotByRightClickContextMenu(no, Section)
                .done(function (data) {
                    var dicRetMetadata = data.dicRetMetadata;
                    if (dicRetMetadata != undefined
                        && dicRetMetadata.IsDeficiencyExist == "true"
                        && dicRetMetadata.IsDeficiencyInitialActionExist == "false"
                    ) {
                        $("#hdnDeficienciesUniqueID").val(dicRetMetadata.DeficienciesUniqueID);    // RDBJ 02/18/2022
                        $(ctr).prop('popShown', true).popover('show');
                    }
                    else {
                        addNoteSection(no);
                        $(ctr).parent().parent().children()[4].innerHTML = '<i id="idViewNote" data-toggle="tooltip" data-placement="left" title="View note" class="material-icons" onclick="viewNote(this)">note</i>';
                    }
                })
                .fail(function (x) {
                    // Tell the user something bad happened
                });
        }
        // End RDBJ 02/18/2022
    }
}

// RDBJ 02/18/2022
function CheckDeficiencyAddedOrNotByRightClickContextMenu(itemno, section) {
    var url = RootUrl + 'Deficiencies/CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu';
    var dicMetadata = {};
    dicMetadata["ItemNo"] = itemno;
    dicMetadata["Section"] = section;
    dicMetadata["ReportType"] = "SI";
    dicMetadata["Ship"] = $("#ddlGIReportShipName option:selected").val();
    dicMetadata["UniqueFormID"] = $("#UniqueFormID").val();

    return $.ajax({
        url: url,
        type: 'POST',
        async: true,
        data: { dicMetadata: dicMetadata },
        success: function (res) {
            console.log(res.dicRetMetadata);
            retDicRetMetadata = res.dicRetMetadata;
            $("#lblAutoSave").hide();
        },
        error: function (err) {
            console.log(err);
            $("#lblAutoSave").hide();
        }
    });
}
// End RDBJ 02/18/2022

function viewNote(ctr) {
    var no = $(ctr).parent().parent().children()[0].textContent.trim();
    viewAdditionalNote(no)
}
function Info() {
    $('[data-toggle="tooltip"]').tooltip();
}
function GoBackTOSection(ctr) {
    //var no = $(ctr).parent().parent().children().children().val()
    var no = $(ctr).parent().parent().find("td:eq(1)").find("input").val();
    $(".PageSection").hide();
    no = no.split('-')[0];
    $(".section" + no).show();
}
function addNoteSection(ctr) {
    var len = $('.notelist').length;
    len = parseInt(len);

    var id = createGuid();  // RDBJ2 04/02/2022

    $(".section20 .notetable").append(
        "<tr class='notelist' id='" + id + "'>" +   // RDBJ2 04/02/2022 Added id
        "<td width='2%' class='v-al-m text-center aviodPrint'>" +
        "<input type='hidden' name='SIRNote[" + len + "].NotesUniqueID' value='" + id + "'>" + // RDB2 04/02/2022
        "<a href='#' onclick='removeNote(this,true)' data-id='" + id + "' >" + // RDBJ2 04/02/2022 Added data-id
        "<span class='glyphicon glyphicon-remove removeIcon'></span>" +
        "</a>" +
        "</td>" +
        "<td width='18%' class='v-al-m text-center'>" +
        "<input type = 'text' class= 'col-md-12 form-control SIRData' onchange='onChangeNoteValue()' value='" + ctr + "'  name='SIRNote[" + len + "].Number'/>" +
        "</td>" +
        "<td width='80%' class='v-al-m text-center'>" +
        //"<input type = 'text' autofocus class= 'col-md-12 form-control SIRData' onchange='onChangeNoteValue()' name='SIRNote[" + len + "].Note' />" + // RDBJ 01/20/2022 commented this line
        "<textarea autofocus class= 'col-md-12 form-control SIRData' onchange='onChangeNoteValue()' name='SIRNote[" + len + "].Note'></textarea>" + // RDBJ 01/20/2022
        "</td > " +
        "<td class='notes_column aviodPrint'><i data-toggle='tooltip' data-placement='left' title='Go back to section' class='glyphicon glyphicon-arrow-right' onclick='GoBackTOSection(this)'></i></td>" +
        "</tr> ")
    $(".PageSection").hide();
    $(".section20").show();
    Info();
    setChnageValue(true, false);
}
function SetAppMode(isInternetAvailable) {
    if (isInternetAvailable == "True")
        isInternetAvailable = true;
    if (isInternetAvailable == true)
        appMode = "online";
    $("#appMode").val(appMode);
}

function addAdditionalNoteSection() {
    // RDBJ 01/20/2022
    if ($("#UniqueFormID").val() == "") {
        $("#lblAutoSave").hide();
        $.notify("Please add General Details first in Inspection Details!", "error");
        return;
    }
    var len = $('.noteAdditionallist').length;
    len = parseInt(len);
    var id = createGuid();  // RDBJ2 04/02/2022
    $(".section20 .noteAdditionallisttable").append(
        "<tr class='noteAdditionallist' id='" + id + "'>" + // RDBJ2 04/02/2022 Added id
        "<td width='2%' class='v-al-m text-center aviodPrint'>" +
        "<a href='#' onclick='removeNote(this,false)' data-id='" + id + "' >" + // RDBJ2 04/02/2022 Added data-id
        "<span class='glyphicon glyphicon-remove removeIcon'></span>" +
        "</a>" +
        "</td>" +
        "<td width='18%' class='v-al-m text-center'>" +
        "<input type='hidden' name='SIRAdditionalNote[" + len + "].NotesUniqueID' value='" + id + "'>" + // RDB2 04/02/2022
        "<input type = 'text' class= 'col-md-12 form-control SIRData' onchange='onChangeAdditionalNoteValue()'  name='SIRAdditionalNote[" + len + "].Number'/>" +
        "</td>" +
        "<td width='70%' class='v-al-m text-center'>" +
        //"<input type = 'text' autofocus class= 'col-md-12 form-control SIRData' onchange='onChangeAdditionalNoteValue()' name='SIRAdditionalNote[" + len + "].Note' />" + // RDBJ 01/20/2022 commented this line
        "<textarea autofocus class= 'col-md-12 form-control SIRData' onchange='onChangeAdditionalNoteValue()'  name='SIRAdditionalNote[" + len + "].Note'></textarea>" + // RDBJ 01/20/2022
        "</td > " +
        "</tr>");
    setChnageValue(false, true);
}
function ValidateGIRForm() {
    GIRFormValidator = $("#SIRForm").validate({
        rules: {
            "SuperintendedInspectionReport.ShipName": {
                required: true
            },
            "SuperintendedInspectionReport.Port": {
                required: true
            },
            "SuperintendedInspectionReport.Superintended": {
                required: true
            },
            "SuperintendedInspectionReport.Master": {
                required: true
            },
            "SuperintendedInspectionReport.Date": {
                required: true,
                //date: true
            },
            // RDBJ 01/15/2022
            dateRaised: {
                required: true
            },
            txtDeficiency: {
                required: true
            },
            // End RDBJ 01/15/2022
        },
        messages: {
            "SuperintendedInspectionReport.ShipName": "Please Select Ship",
            "SuperintendedInspectionReport.Port": "Enter Port.",
            "SuperintendedInspectionReport.Superintended": "Please Select Superintended",
            "SuperintendedInspectionReport.Master": "Please enter Master",
            "SuperintendedInspectionReport.Date": "Please Enter Date",
            txtDeficiency: "Please Enter Deficiency", // RDBJ 01/15/2022
            dateRaised: "Please Enter Date", // RDBJ 01/15/2022
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
function viewAdditionalNote(no) {
    $('.notetable > tbody  > tr').each(function () {
        var tr = $(this);
        var td = $(tr).find("td:eq(1)").find("input").val();
        if (no == td) {
            $(".PageSection").hide();
            $(".section20").show();
            //$(tr).find("td:eq(2)").find("input").click(); // RDBJ 01/20/2022 commented this line
            $(tr).find("td:eq(2)").find("textarea").click(); // RDBJ 01/20/2022
        }
    });
}
$(document).on("click", "#btnSubmit", function (e) {
    if ($("#SIRForm").valid()) {
        SIRAutoSave(); // RDBJ 01/15/2022
    }
});

function onSIRFormSubmit() {
    if (!$(document.activeElement).is(':submit')) return false;
    else return true;
}
//RDBJ 10/11/2021
function RemoveRequiredStarsIfValues() {
    if ($("#Port").val() == "")
        $("#lblErrorPort").css("display", "inline");
    else
        $("#lblErrorPort").css("display", "none");

    if ($("#Master").val() == "")
        $("#lblErrorMaster").css("display", "inline");
    else
        $("#lblErrorMaster").css("display", "none");
}
//End RDBJ 10/11/2021

// RDBJ 02/18/2022
function SubmitCorrectiveAction() {
    var url = RootUrl + "Deficiencies/AddDeficienciesInitialActions";
    var Description = $('.popover-content').find('#txtCorrectiveDescription').text();   // JSL 05/19/2022 changed with html to text

    // JSL 11/13/2022
    if (IsNullEmptyOrUndefined(Description)) {
        $.notify("Please add comment!", "error");
        return;
    }
    // End JSL 11/13/2022

    var obj = {};
    obj.Name = "";
    obj.Description = Description;
    obj.DeficienciesUniqueID = $('#hdnDeficienciesUniqueID').val();
    obj.UniqueFormID = $("#UniqueFormID").val();

    // JSL 11/13/2022 commented
    /*
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        async: true,
        url: url,
        data: JSON.stringify(obj),
        success: function (data) {
            $.notify("Corrective Action Added Successfully", "success");
            $("#FormVersion").val(data.FormVersion); // RDBJ 02/20/2022
            $("[data-deficiencies-id='" + obj.DeficienciesUniqueID + "']").attr("data-deficiencies-id", "");
            $('[data-toggle=popover]').popover('hide');
        },
        error: function (data) {
            console.log(data);
        }
    });
    */
    // End JSL 11/13/2022 commented

    // JSL 11/13/2022
    var fileData = new FormData();
    fileData.append("Name", "");
    fileData.append("Description", Description);
    fileData.append("DeficienciesUniqueID", $('#hdnDeficienciesUniqueID').val());
    fileData.append("UniqueFormID", $("#UniqueFormID").val());

    $.ajax({
        type: 'POST',
        contentType: false,
        processData: false,
        async: true,
        url: url,
        data: fileData,
        success: function (data) {
            $.notify("Corrective Action Added Successfully", "success");
            $("#FormVersion").val(data.FormVersion);
            $("[data-deficiencies-id='" + obj.DeficienciesUniqueID + "']").attr("data-deficiencies-id", "");
            $('[data-toggle=popover]').popover('hide');
        },
        error: function (data) {
            console.log(data);
        }
    });
    // End JSL 11/13/2022
}
// End RDBJ 02/18/2022

// RDBJ 02/18/2022
function ClosePopoverCorrectiveAction() {
    $('[data-toggle=popover]').popover('hide');
}
// End RDBJ 02/18/2022

// RDBJ 04/18/2022
function UpdateDeficienciesShipWhenChangeFormShip() {
    var dic = {};

    dic["UniqueFormID"] = _UniqueFormID;
    dic["Ship"] = _Ship;
    dic["FormType"] = 'SI';

    CommonServerPostApiCall(dic, "Forms", "PerformAction", str_API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS);
}
// End RDBJ 04/18/2022