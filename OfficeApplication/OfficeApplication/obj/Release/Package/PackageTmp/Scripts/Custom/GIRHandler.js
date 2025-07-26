var GIRFormValidator = undefined;
var menuDeficiency;
var itemNumber = "";
var blnUpdateButton = false; //RDBJ 10/07/2021

var oTable; //RDBJ 10/23/2021 This is only for deficiencies
var oSettings; //RDBJ 10/23/2021 This is only for deficiencies

var _Ship, _UniqueFormID;   // RDBJ 03/19/2022

$(document).ready(function () {
    var height = $(window).height();
    $(".leftContent").height(height - 65);
    $(".rightContent").height(height - 65);
    $("#ShipName").val($("#Child option:selected").text());
    _Ship = $("#Child option:selected").val();  // RDBJ 03/19/2022

    $(".GIRData").bind("change", function (e) {
        e.preventDefault();
        $("#ShipName").val($("#Child option:selected").text());
        $("#hdnShip").val($("#Child option:selected").val()); //RDBJ 10/07/2021
        itemNumber = e.target.classList[3];
        setChnageValue(false, false, false, false, false);

        RemoveRequiredStarsIfValues(); //RDBJ 10/08/2021

        //10/07/2021
        if ($("#GIRForm").valid()) {
            GIRAutoSave();
        }
    });
    $('.GIRData').bind('contextmenu', function (e) {
        e.preventDefault();
    });

 
    $(".NavButton").click(function () {
        $("button").removeClass("active"); //RDBJ 10/20/2021
        $(this).addClass("active"); //RDBJ 10/20/2021
        var lblValue = $(this).attr("data-lbl-text");
        var hideSection = $(this).attr("data-lbl-show");
        $(".PageSection").hide();
        $("." + hideSection).show();
        $("#lblSubHeading").text(lblValue);
        $(".leftContent").animate({ scrollTop: 0 }, "fast");
    })

    $("#InserNewDefect").click(function () {
        //var ccPanelDiv = $(".DefectPanel").clone();
        //$(ccPanelDiv).insertAfter(".DefectPanel");
    });
    $("#btnSave").click(function () {
        //RDBJ 10/11/2021 Wrapped in If
        if ($("#GIRForm").valid()) {
            GIRAutoSave();
        }
    });

    $("#Child").change(function () {
        var id = $("#Child").val();
        $("#hdnShip").val($("#Child option:selected").val()); //RDBJ 10/07/2021
        _Ship = $("#Child option:selected").val();  // RDBJ 03/19/2022
        GetGenDescData(id);
        //10/07/2021
        if ($("#GIRForm").valid()) {
            GIRAutoSave(true);  // RDBJ 03/19/2022 Pass true
        }
    });

    ValidateGIRForm();
    bindContextMenu();

    $("#DeficienciesSection").click(function () {
        var url = RootUrl + 'Drafts/_GIR_DeficienciesSection';
        var FormId = $("#UniqueFormID").val(); //RDBJ 09/18/2021

        $.ajax({
            type: "GET",
            url: url + "?id=" + FormId, //RDBJ 09/18/2021 $("#GIRFormID").val(),
        }).done(function (r) {
            $(".DeficienciesSection").html(r);
        }).fail(function (e) {
            console.log(e.responseText);
        });
    });

    //RDBJ 10/07/2021
    $("#btnUpdateShipsData").click(function () {
        if (!blnUpdateButton) {
            $("#btnUpdateShipsData").text("Update Ships Data");
            $("#btnUpdateShipsDataCancel").css("display", "block");
            AllowEditShipGeneralDescription();
            blnUpdateButton = true;
        }
        else {
            $("#btnUpdateShipsData").text("Edit Ships Data");
            $("#btnUpdateShipsDataCancel").css("display", "none");
            GIRShipGeneralDescriptionSave();
        }
    });
    //End RDBJ 10/07/2021

    //RDBJ 10/07/2021
    $("#btnUpdateShipsDataCancel").click(function () {
        $("#btnUpdateShipsData").text("Edit Ships Data");
        $("#btnUpdateShipsDataCancel").css("display", "none");
        DisableEditShipGeneralDescription();
        blnUpdateButton = false;
    });
    //End RDBJ 10/07/2021

    RemoveRequiredStarsIfValues(); //RDBJ 10/08/2021
});

//RDBJ 11/28/2021
function textAreaAutoHeight() {
    $("textarea").each(function () {
        var val = this.innerHTML;
        if (val != "") {
            this.removeAttribute("rows");
            this.addEventListener('focus', function () {
                autosize(this);
            });
            autosize(this);
        }
    }).on("input", function () {
        this.style.height = "auto";
        this.style.height = (this.scrollHeight) + "px";
    });
}
//End RDBJ 11/28/2021

function bindContextMenu() {
    //alert("call");
    menuDeficiency = $("#menu").kendoContextMenu({
        target: ".addDeficiencies",
        //filter: "tr",
        animation: {
            open: { effects: "fadeIn" },
            duration: 500
        },
        select: function (e) {
            $("#lblAutoSave").show();

            // RDBJ 12/18/2021
            if ($("#UniqueFormID").val() == "") {
                $("#lblAutoSave").hide();
                $.notify("Please add General Details first in General Section!", "error");
                return;
            }

            if ($("#UniqueFormID").val() != "00000000-0000-0000-0000-000000000000") { //RDBJ 09/18/2021 changed condition if ($("#GIRFormID").val() != "0") {
                var url = RootUrl + 'GIRList/AddGIRDeficiencies';
                var itemno = "";
                var deficienciesTd = $(e.target).closest('tr').find('td.clsDeficiencies');
                var deficiencies = $(deficienciesTd).find('.GIRData').val();
                var section = "";
                var multipleRowClassName = $(e.target).closest('tr').parent().closest('.clsMultipleRow')[0] == undefined ? "" : $(e.target).closest('tr').parent().closest('.clsMultipleRow')[0].className;

                if (multipleRowClassName == "clsMultipleRow") {
                    if (itemNumber != "") {
                        itemno = $($(e.target).closest('tr').parent().closest('.clsDeficiencies')[0]).parent().find('td.clsItemNo').text().trim() + itemNumber;
                    }
                    else {
                        itemno = $($(e.target).closest('tr').parent().closest('.clsDeficiencies')[0]).parent().find('td.clsItemNo').text().trim();
                    }
                    section = $($($(e.target).parent().closest('.clsMultipleRow')[0]).parent().closest('.clsSection')[0]).find('.SectionHead u').text();
                }
                else {
                    itemno = $(e.target).closest('tr').find('td.clsItemNo').text().trim();
                    section = $($(e.target).parent().closest('.clsSection')[0]).find('.SectionHead u').text();
                }
                if (deficiencies == "") {
                    $.notify("Please enter comment", "error");
                    $(deficienciesTd).find('.GIRData').focus();
                    $("#lblAutoSave").hide();
                    return false;
                }
                var Modal = {
                    GIRFormID: $("#GIRFormID").val(),
                    UniqueFormID: $("#UniqueFormID").val(), //RDBJ 09/18/2021
                    ItemNo: itemno,
                    Deficiency: deficiencies,
                    Section: section,
                    ReportType: "GI", //RDBJ 11/02/2021
                    Priority: 12, //RDBJ 11/02/2021
                    Ship: $("#Child option:selected").val(),
                    DateRaised: $("#Date").val(),   // JSL 10/15/2022
                }

                $.ajax({
                    url: url,
                    type: 'POST',
                    async: true,
                    data: Modal,
                    success: function (data) {
                        console.log(data);

                        $("#FormVersion").val(data.FormVersion);    // RDBJ 02/19/2022
                        $.notify("Deficiency Added Successfully", "success");  // RDBJ 02/19/2022

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

function SetAppMode(isInternetAvailable) {
    if (isInternetAvailable == "True")
        isInternetAvailable = true;
    if (isInternetAvailable == true)
        appMode = "online";
    $("#appMode").val(appMode);
}

function ValidateGIRForm() {

    $('#Date').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });

    $('.txtDateRaised').datepicker({
        autoclose: true
    });

    $('.txtDateClosed').datepicker({
        autoclose: true
    });

    $('body').on('focus', ".txtDateRaised,.txtDateClosed", function () {
        $(this).datepicker({
            format: 'dd/mm/yyyy'
        });
    });

    GIRFormValidator = $("#GIRForm").validate({
        rules: {
            Ship: {
                required: true
            },
            Port: {
                required: true
            },
            Inspector: {
                required: true
            },
            Date: {
                required: true
                //date: true
            },
            //RDBJ 11/02/2021
            dateRaised: {
                required: true
            },
            txtDeficiency: {
                required: true
            },
            //End RDBJ 11/02/2021
        },
        messages: {
            Ship: "Please Select Ship",
            Port: "Enter Port.",
            Inspector: "Please Select Inspector",
            Date: "Please Enter Date",
            txtDeficiency: "Please Enter Deficiency", //RDBJ 11/02/2021
            dateRaised: "Please Enter Date", //RDBJ 11/02/2021
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}

$(document).on("click", "#btnSubmit", function (e) {
    if ($("#GIRForm").valid()) {

    }
});

function GIRAutoSave(
    IsUpdateShipForDeficiencies = false    // RDBJ 03/19/2022
) {
    var url = RootUrl + 'GIRList/GIRAutoSave';
    $("#lblAutoSave").show();
    console.log("Input : " + $("#GIRFormID").val());
    var form = $("#GIRForm");
    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        async: true,
        success: function (res) {
            console.log(res);
            $("#GIRFormID").val(res);
            $("#UniqueFormID").val(res.UniqueFormID);
            _UniqueFormID = res.UniqueFormID;   // RDBJ 03/19/2022
            $("#FormVersion").val(res.FormVersion);
            $("#lblAutoSave").hide();

            // RDBJ 03/19/2022
            if (IsUpdateShipForDeficiencies) {
                UpdateDeficienciesShipWhenChangeFormShip(); 
            }
            // End RDBJ 03/19/2022
        },
        error: function (err) {
            console.log(err);
            $("#lblAutoSave").hide();
        }
    });
}

function printDiv() {
    var className = $(".PageSection:Visible").attr("class");
    className = className.replace("PageSection", "");
    className = className.trim();
    $(".SectionComplete").hide();
    $(".aviodPrint").hide();
    $(".PageSection").show();
    $("#lblInspector").html("<strong>Inspector: </strong>" + $("#Inspector option:selected").text());
    $("#lblship").html("<strong>Ship :</strong>" + $("#Child option:selected").text());
    $("#lblport").html("<strong>Port :</strong>" + $("#Port").val());
    $("#lbldate").html("<strong>Date :</strong>" + $("#Date").val());
    $("input[type=text]").each(function () {
        var str = $(this).val();
        $(this).attr('value', str);
    });
    $("textarea").each(function () {
        var str = $(this).val();
        $(this).text("");
        $(this).append(str);

        //RDBJ 10/23/2021
        var val = this.innerHTML;
        if (val != "") {
            $(this).css("width", "800px");
            this.removeAttribute("rows");
            this.addEventListener('focus', function () {
                autosize(this);
            });
            autosize(this);
        }
        //End RDBJ 10/23/2021
    });

    //RDBJ 10/23/2021 this is only for DeficienciesSections
    $("select").each(function (i) {
        var objSelected = $(this).find('option:selected');
        objSelected.attr('selected', 'selected');
    });

    //RDBJ 10/23/2021 this is only for DeficienciesSections
    oTable = $('#tblDeficiencies').DataTable();
    oSettings = oTable.settings();

    oSettings[0]._iDisplayLength = oSettings[0].fnRecordsTotal();
    oTable.draw();

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

    // RDBJ 01/06/2022
    $(".txtDateRaised").each(function (i) {
        $(this).addClass('txtSetDateWidthForPrint');
    });

    $(".txtDateClosed").each(function (i) {
        $(this).addClass('txtSetDateWidthForPrint');
    });
    // End RDBJ 01/06/2022

    //End RDBJ 10/23/2021

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
    divToPrint = document.getElementById("forntpage").innerHTML;
    divToPrint += document.getElementById('GIRContent').innerHTML;

    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    var style = "@media print {.breakDivPage { page-break-after: always; page-break-inside:auto;   } }";
    style += "@page {size: auto;margin: 5%;}";
    newWin.document.write('<html><head><title></title>');
    newWin.document.write('<link href="../Content/bootstrap/dist/css/bootstrap.css" rel="stylesheet" />');
    newWin.document.write('<link href="../Content/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />');
    newWin.document.write('<link rel="stylesheet" href="../Content/Custom/GIRForm.css" type="text/css" />');
    newWin.document.write('<link href="../Content/AdminLTE.min.css" rel="stylesheet" />');
    newWin.document.write('<link href="../Content/_all-skins.min.css" rel="stylesheet" />');
    newWin.document.write('<link href="../Content/Custom/SiteCustom.css" rel="stylesheet" />');
    newWin.document.write('<style>' + style + '</style>');
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

        // RDBJ 01/06/2022
        $(".txtDateRaised").each(function (i) {
            $(this).removeClass('txtSetDateWidthForPrint');
        });

        $(".txtDateClosed").each(function (i) {
            $(this).removeClass('txtSetDateWidthForPrint');
        });
        // End RDBJ 01/06/2022

        //RDBJ 10/23/2021 revert css changes
        $("textarea").each(function () {
            var str = $(this).val();
            $(this).text("");
            $(this).append(str);
            var val = this.innerHTML;
            if (val != "") {
                $(this).css("width", "");
                this.removeAttribute("rows");
                this.addEventListener('focus', function () {
                    autosize(this);
                });
                autosize(this);
            }
        });
        //End RDBJ 10/23/2021

        // RDBJ 01/06/2022 this is only for DeficienciesSections
        $(".txtAreaDeficiencies").each(function () {
            $(this).css("width", "100%");
            this.addEventListener('focus', function () {
                autosize(this);
            });
            autosize(this);
        });

        //RDBJ 10/23/2021 this is only for DeficienciesSections
        oSettings[0]._iDisplayLength = 100;
        oTable.draw();
        //End RDBJ 10/23/2021
        location.reload();  // 09/09/2022
    }, 5000);
}

function GetGenDescData(id) {
    var url = RootUrl + 'GIRList/GIRGetGeneralDescriptionByShip';
    $.ajax({
        type: 'POST',
        url: url,
        data: { Shipvalue: id },
        dataType: 'json',
        success: function (data) {
            SetGenDescData(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed...');
        }
    });
}

//RDBJ 10/07/2021 Update all properties with CSShipsModal
function SetGenDescData(data) {
    // RDBJ 01/10/2022 commented below 3 lines
    /*
    $("input[name='CSShipsModal.ClassificationSocietyId']").val(data.ClassificationSocietyId);
    $("input[name='CSShipsModal.FlagStateId']").val(data.FlagStateId);
    $("input[name='CSShipsModal.PortOfRegistryId']").val(data.PortOfRegistryId);
    */

    // RDBJ 01/10/2022
    $("#ClassificationSocietyId").val(data.ClassificationSocietyId);
    $("#FlagStateId").val(data.FlagStateId);
    $("#PortOfRegistryId").val(data.PortOfRegistryId);

    $("#hdnClassificationSocietyId").val(data.ClassificationSocietyId);
    $("#hdnFlagStateId").val(data.FlagStateId);
    $("#hdnPortOfRegistryId").val(data.PortOfRegistryId);
    // End RDBJ 01/10/2022

    $("input[name='CSShipsModal.IMONumber']").val(data.IMONumber);
    $("input[name='CSShipsModal.SummerDeadweight']").val(data.SummerDeadweight);
    $("input[name='CSShipsModal.Lightweight']").val(data.Lightweight);
    $("input[name='CSShipsModal.Beam']").val(data.Beam);
    $("input[name='CSShipsModal.SummerDraft']").val(data.SummerDraft);
    $("input[name='CSShipsModal.BowThruster']").val(data.BowThruster);
    $("input[name='Noofholds']").val(data.Noofholds);
    $("input[name='Containers']").val(data.Containers);
    $("input[name='Cargohandlingequipment']").val(data.Cargohandlingequipment);

    $("input[name='CSShipsModal.BuildYear']").val(data.BuildYear);
    $("input[name='Classofvessel']").val(data.Classofvessel);
    $("input[name='CSShipsModal.MMSI']").val(data.MMSI);
    $("input[name='CSShipsModal.CallSign']").val(data.CallSign);
    $("input[name='CSShipsModal.GrossTonnage']").val(data.GrossTonnage);
    $("input[name='CSShipsModal.NetTonnage']").val(data.NetTonnage);
    $("input[name='CSShipsModal.LOA']").val(data.LOA);
    $("input[name='CSShipsModal.LBP']").val(data.LBP);
    $("input[name='CSShipsModal.BHP']").val(data.BHP);
    $("input[name='Nomoveablebulkheads']").val(data.Nomoveablebulkheads);
    $("input[name='Cargocapacity']").val(data.Cargocapacity);
}

//RDBJ 10/07/2021 Update all properties with CSShipsModal
function SetGenDescDataEmpty() {
    $("input[name='CSShipsModal.ClassificationSocietyId']").val("");
    $("input[name='CSShipsModal.FlagStateId']").val("");
    $("input[name='CSShipsModal.PortOfRegistryId']").val("");
    $("input[name='CSShipsModal.IMONumber']").val("");
    $("input[name='CSShipsModal.SummerDeadweight']").val("");
    $("input[name='CSShipsModal.Lightweight']").val("");
    $("input[name='CSShipsModal.Beam']").val("");
    $("input[name='CSShipsModal.SummerDraft']").val("");
    $("input[name='CSShipsModal.BowThruster']").val("");
    //$("input[name=Noofholds]").val("");
    //$("input[name=Containers]").val("");
    //$("input[name=Cargohandlingequipment]").val("");
    $("input[name='CSShipsModal.BuildYear']").val("");
    $("input[name='Classofvessel']").val("");
    $("input[name='CSShipsModal.MMSI']").val("");
    $("input[name='CSShipsModal.CallSign']").val("");
    $("input[name='CSShipsModal.GrossTonnage']").val("");
    $("input[name='CSShipsModal.NetTonnage']").val("");
    $("input[name='CSShipsModal.LOA']").val("");
    $("input[name='CSShipsModal.LBP']").val("");
    $("input[name='CSShipsModal.BHP']").val("");
    //$("input[name=Nomoveablebulkheads]").val("");
    //$("input[name=Cargocapacity]").val("");
}

function setChnageValue(safe, crewdoc, restandwork, deficiency, Photos) {
    $("#SafeMiningChanged").val(safe);
    $("#CrewDocsChanged").val(crewdoc);
    $("#RestAndWorkChanged").val(restandwork);
    $("#DeficienciesChanged").val(deficiency);
    $("#PhotosChanged").val(Photos);
}

//RDBJ 10/07/2021
function AllowEditShipGeneralDescription() {
    // RDBJ 01/10/2022 commented below 3 lines
    /*
    $("input[name='CSShipsModal.ClassificationSocietyId']").removeAttr('readonly');
    $("input[name='CSShipsModal.FlagStateId']").removeAttr('readonly');
    $("input[name='CSShipsModal.PortOfRegistryId']").removeAttr('readonly');
    */

    // RDBJ 01/10/2022
    $("select[name='ddlClassificationSocietyId']").attr('disabled', false);
    $("select[name='ddlFlagStateId']").attr('disabled', false);
    $("select[name='ddlPortOfRegistryId']").attr('disabled', false);
    // End RDBJ 01/10/2022

    $("input[name='CSShipsModal.IMONumber']").removeAttr('readonly');
    $("input[name='CSShipsModal.SummerDeadweight']").removeAttr('readonly');
    $("input[name='CSShipsModal.Lightweight']").removeAttr('readonly');
    $("input[name='CSShipsModal.Beam']").removeAttr('readonly');
    $("input[name='CSShipsModal.SummerDraft']").removeAttr('readonly');
    $("input[name='CSShipsModal.BowThruster']").removeAttr('readonly');
    //$("input[name=Noofholds]").removeAttr('readonly');
    //$("input[name=Containers]").removeAttr('readonly');
    //$("input[name=Cargohandlingequipment]").removeAttr('readonly');
    $("input[name='CSShipsModal.BuildYear']").removeAttr('readonly');
    $("input[name='Classofvessel']").removeAttr('readonly');
    $("input[name='CSShipsModal.MMSI']").removeAttr('readonly');
    $("input[name='CSShipsModal.CallSign']").removeAttr('readonly');
    $("input[name='CSShipsModal.GrossTonnage']").removeAttr('readonly');
    $("input[name='CSShipsModal.NetTonnage']").removeAttr('readonly');
    $("input[name='CSShipsModal.LOA']").removeAttr('readonly');
    $("input[name='CSShipsModal.LBP']").removeAttr('readonly');
    $("input[name='CSShipsModal.BHP']").removeAttr('readonly');
    //$("input[name=Nomoveablebulkheads]").removeAttr('readonly');
    //$("input[name=Cargocapacity]").removeAttr('readonly');
}
//End RDBJ 10/07/2021

//RDBJ 10/07/2021
function DisableEditShipGeneralDescription() {
    // RDBJ 01/10/2022 commented below 3 lines
    /*
    $("input[name='CSShipsModal.ClassificationSocietyId']").attr('readonly', true);
    $("input[name='CSShipsModal.FlagStateId']").attr('readonly', true);
    $("input[name='CSShipsModal.PortOfRegistryId']").attr('readonly', true);
    */

    // RDBJ 01/10/2022
    $("select[name='ddlClassificationSocietyId']").attr('disabled', true);
    $("select[name='ddlFlagStateId']").attr('disabled', true);
    $("select[name='ddlPortOfRegistryId']").attr('disabled', true);
    // End RDBJ 01/10/2022

    $("input[name='CSShipsModal.IMONumber']").attr('readonly', true);
    $("input[name='CSShipsModal.SummerDeadweight']").attr('readonly', true);
    $("input[name='CSShipsModal.Lightweight']").attr('readonly', true);
    $("input[name='CSShipsModal.Beam']").attr('readonly', true);
    $("input[name='CSShipsModal.SummerDraft']").attr('readonly', true);
    $("input[name='CSShipsModal.BowThruster']").attr('readonly', true);
    //$("input[name=Noofholds]").attr('readonly', true);
    //$("input[name=Containers]").attr('readonly', true);
    //$("input[name=Cargohandlingequipment]").attr('readonly', true);
    $("input[name='CSShipsModal.BuildYear']").attr('readonly', true);
    $("input[name='Classofvessel']").attr('readonly', true);
    $("input[name='CSShipsModal.MMSI']").attr('readonly', true);
    $("input[name='CSShipsModal.CallSign']").attr('readonly', true);
    $("input[name='CSShipsModal.GrossTonnage']").attr('readonly', true);
    $("input[name='CSShipsModal.NetTonnage']").attr('readonly', true);
    $("input[name='CSShipsModal.LOA']").attr('readonly', true);
    $("input[name='CSShipsModal.LBP']").attr('readonly', true);
    $("input[name='CSShipsModal.BHP']").attr('readonly', true);
    //$("input[name=Nomoveablebulkheads]").attr('readonly', true);
    //$("input[name=Cargocapacity]").attr('readonly', true);
}
//End RDBJ 10/07/2021

//RDBJ 10/07/2021
function GIRShipGeneralDescriptionSave() {
    var url = RootUrl + 'GIRList/GIRShipGeneralDescriptionSave';
    $("#lblAutoSave").show();
    var form = $("#GIRForm");
    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        async: true,
        success: function (res) {
            if ($("#GIRForm").valid()) {
                GIRAutoSave();
            }
            DisableEditShipGeneralDescription();
            blnUpdateButton = false;
            $("#lblAutoSave").hide();
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
}
//End RDBJ 10/07/2021

//RDBJ 10/08/2021
function RemoveRequiredStarsIfValues() {
    if ($("#Port").val() == "")
        $("#lblErrorPort").css("display", "block");
    else
        $("#lblErrorPort").css("display", "none");

    if ($("#Date").val() == "")
        $("#lblErrorDate").css("display", "block");
    else
        $("#lblErrorDate").css("display", "none");
}
//End RDBJ 10/08/2021

// RDBJ 01/06/2022
function SetDeficienciesDataForPrintAndPrintIt() {
    var url = RootUrl + 'Drafts/_GIR_DeficienciesSection';
    var FormId = $("#UniqueFormID").val();
    $.ajax({
        type: "GET",
        url: url + "?id=" + FormId
    }).done(function (r) {
        $(".DeficienciesSection").html(r);
        printDiv();
    }).fail(function (e) {
        console.log(e.responseText);
    });
}
// End RDBJ 01/06/2022

// RDBJ 01/03/2022
function ddlOnChangeSetValueCommonFunction(elementId) {
    $('#hdn' + elementId).val($('#' + elementId + ' option:selected').val());
}
// End RDBJ 01/03/2022

// RDBJ 03/19/2022
function UpdateDeficienciesShipWhenChangeFormShip() {
    var dic = {};

    dic["UniqueFormID"] = _UniqueFormID;
    dic["Ship"] = _Ship;
    dic["FormType"] = 'GI';

    CommonServerPostApiCall(dic, "Forms", "PerformAction", str_API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS);
}
// End RDBJ 03/19/2022