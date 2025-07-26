// RDBJ 02/08/2022 Added this script

var str_Status_Success = 'Success'; // RDBJ 02/26/2022
var str_Status_Error = 'Error'; // RDBJ 02/26/2022

// RDBJ 02/08/2022
function BindToolTipsForGridText(repGrid, colName, colLength) {
    //$('#' + repGrid).kendoTooltip({   // RDBJ 02/11/2022 commented this line
    $("td[class='tooltipText']").kendoTooltip({ // RDBJ 02/11/2022
        //filter: "td[class='tooltipText']",
        showOn: "mouseenter", // click // JSL 06/13/2022
        //width: 280, // JSL 06/13/2022 commented this line // RDBJ 02/11/2022
        //position: "right",  // JSL 06/13/2022 commented this line
        // JSL 06/13/2022
        width: 480,
        position: "top",
        // End JSL 06/13/2022
        autoHide: true, // RDBJ 02/11/2022
        animation: {
            // RDBJ 02/11/2022
            open: {
                //effects: "slideIn:right",   // JSL 06/13/2022 commented this line
                effects: "fade:in", // JSL 06/13/2022 
                duration: 200
            },
            close: {
                //effects: "slideIn:right",   // JSL 06/13/2022 commented this line
                effects: "fade:in", // JSL 06/13/2022 
                reverse: true,
                duration: 200
            }
            // End RDBJ 02/11/2022
        },
        content: function (e) {
            // RDBJ 02/11/2022
            var target = $(e.target);
            return target.text();
            // End RDBJ 02/11/2022

            // RDBJ 02/11/2022 commented 
            /*
            var dataItem = $('#' + repGrid).data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = dataItem[colName];
            */

            /*
            if (content.length > parseInt(colLength)) {
                return content;
            } else {
                return "";
            }
            */
            // End RDBJ 02/11/2022 commented 
        },
        // RDBJ 02/11/2022 commented 
        /*
        show: function (e) {
            if (this.content.text() != "") {
                $('[role="tooltip"]').css("visibility", "visible");
            }
        },
        hide: function () {
            $('[role="tooltip"]').css("visibility", "hidden");
        }
        */
        // End RDBJ 02/11/2022 commented 
    }).data("kendoTooltip");
}
// End RDBJ 02/08/2022

// RDBJ2 02/26/2022
var CommonServerPostApiCall = function (dictMetadata, controllerName, apiAction, strAction
    , global = true,   // JSL 05/23/2022
) {

    // JSL 05/23/2022 wrapped in if
    if (global)
        $("#lblAutoSave").show();

    setTimeout(function () {
        $.ajax({
            async: false,
            url: RootUrl + controllerName + "/" + apiAction,
            data: { 'jsonmetadata': JSON.stringify(dictMetadata), 'straction': strAction },
            dataType: 'json',
            type: 'POST',
            success: function (data) {
                $("#lblAutoSave").hide();

                PerformClientActionsBasedOnCommonServerCallResponse(data, strAction);
            },
            error: function (data) {
                $("#lblAutoSave").hide();
                console.log(data);
            }
        });
    }, 10);
};
// End RDBJ2 02/26/2022


// RDBJ2 02/26/2022
function PerformClientActionsBasedOnCommonServerCallResponse(data, strAction) {
    switch (strAction) {
        // JSL 11/12/2022
        case str_API_DELETEOTHERSHIPSDATAFROMDATABASE:
            PerformClientDeleteOtherShipsDataFromDatabase(data);
            break;
        // End JSL 11/12/2022
        // RDBJ2 05/10/2022
        case str_API_DELETEDEFICIENCYFILE:
            PerformClientDeleteDeficiencyFile(data);
            break;
        // End RDBJ2 05/10/2022
        // RDBJ 04/18/2022
        case str_API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS:
            PerformClientUpdateDeficienciesShipWhenChangeShipInForms(data);
            break;
        // End RDBJ 04/18/2022
        // RDBJ 04/13/2022
        case str_API_GETMAINSYNCSERVICEDETAILSANDSTATUS:
            PerformClientGetMainSyncServiceDetailsAndStatus(data);
            break;
        // End RDBJ 04/13/2022
        // RDBJ2 04/01/2022
        case str_API_DELETESIRNOTEORSIRADDITIONALNOTE:
            PerformClientDeleteSIRNoteOrSIRAdditionalNote(data);
            break;
        // End RDBJ2 04/01/2022
        // RDBJ2 03/31/2022
        case str_API_UPDATEAUDITNOTEDETAILS:
            PerformClientUpdateAuditNoteDetails(data);
            break;
        // End RDBJ2 03/31/2022
        // RDBJ 02/26/2022
        case str_API_UPDATEMAINSYNCSERVICESETTINGS:
            PerformClientUpdateMainSyncServiceSettings(data);
            break;
        // End RDBJ 02/26/2022
        // RDBJ 02/26/2022
        case str_API_GETMAINSYNCSERVICESETTINGS:
            PerformClientGetMainSyncServiceSettings(data);
            break;
        // End RDBJ 02/26/2022
        default:
        // code block
    }
};
// End RDBJ2 02/26/2022

// RDBJ 02/26/2022
function PerformClientActionNotifyStatusSuccessOrError(data) {
    if (data.Status == str_Status_Success) {
        $.notify("Performed Action done Successfully!", "success");
    }
    else {
        $.notify("Something went wrong!", "error");
    }
}
// End RDBJ 02/26/2022

// RDBJ2 05/10/2022
function PerformClientDeleteDeficiencyFile(data) {
    if (data.Status == str_Status_Success) {
        var element;
        // JSL 06/04/2022
        if (JSON.parse(data.IsDeleteFromSection)) {
            element = document.querySelector('[data-id="' + data.DeficienciesFileUniqueID + '"]');
            $("#FormVersion").val(data.FormVersion);
        }
        else {
            element = document.querySelector('[data-id="' + data.DeficienciesFileID + '"]');
        }
        // End JSL 06/04/2022

        $(element).parent().hide();
        $.notify("Performed Action done Successfully!", "success");
    }
}
// End RDBJ2 05/10/2022

// RDBJ 04/18/2022
function PerformClientUpdateDeficienciesShipWhenChangeShipInForms(data) {
    if (data.Status == str_Status_Success) {
        $("#FormVersion").val(data.FormVersion);
    }
};
// End RDBJ 04/18/2022

// RDBJ 04/13/2022
function PerformClientGetMainSyncServiceDetailsAndStatus(data) {
    if (data.Status == str_Status_Success) {
        var blnIsServiceRunningStatus = (data.ServiceStatus == 'Running') ? true : false;
        $('#mainSyncServiceStatus').html(changeStatusTemplate(blnIsServiceRunningStatus, data.IsNeedToShowNotificationForLatestMainSyncService, data.InstalledDate, data.InstalledVersion, data.LatestVersion, data.ServiceStatus));
        
        var showNotificationForMainSyncService = document.getElementById('showNotificationForMainSyncService');
        var showNotificationForMainSyncServiceInSettingPage = document.getElementById('showNotificationForMainSyncServiceInSettingPage');
        if (data.IsNeedToShowNotificationForLatestMainSyncService == "true") {
            showNotificationForMainSyncService.classList.add("notification-counter");
            showNotificationForMainSyncService.innerHTML = '<i class="bell fa fa-bell"></i>';

            // RDBJ 04/18/2022 wrapped in if
            if (showNotificationForMainSyncServiceInSettingPage != null) {
                showNotificationForMainSyncServiceInSettingPage.classList.add("notification-counter");
                showNotificationForMainSyncServiceInSettingPage.innerHTML = '<i class="bell fa fa-bell"></i>';
            }
        }
        else {
            showNotificationForMainSyncService.classList.remove("notification-counter");
            showNotificationForMainSyncService.innerHTML = "";

            // RDBJ 04/18/2022 wrapped in if
            if (showNotificationForMainSyncServiceInSettingPage != null) {
                showNotificationForMainSyncServiceInSettingPage.classList.remove("notification-counter");
                showNotificationForMainSyncServiceInSettingPage.innerHTML = "";
            }
        }
    }
};
// End RDBJ 04/13/2022

// RDBJ 04/13/2022
function changeStatusTemplate(isRunning, blnIsNeedToShowNotificationForLatestMainSyncService, lastupdatedate, runningVersion, latestVersion, serviceStatus) {
    var strToolTip = "";
    var strClassSuccessOrDanger = '';   // RDBJ 04/16/2022
    var strClassSpinOrPause = '';   // RDBJ 04/16/2022
    var strTemplate = '<div style="">';

    // RDBJ 04/17/2022
    if (isRunning) {

        // RDBJ 04/18/2022 wrapped in if
        if (JSON.parse(blnIsNeedToShowNotificationForLatestMainSyncService))    // JSL 05/19/2022 set Json.parse to parse bool value
            strClassSuccessOrDanger = 'label-warning';
        // RDBJ 04/18/2022 added else
        else
            strClassSuccessOrDanger = 'label-success';

        strClassSpinOrPause = 'fa fa-spinner fa-spin';
        strToolTip = serviceStatus + " since " + lastupdatedate;
    }
    else {
        strClassSuccessOrDanger = 'label-danger';
        strClassSpinOrPause = 'fa fa-pause';
    }
    // End RDBJ 04/17/2022

    if (strToolTip == "") {
        strToolTip = 'Please install latest version(' + latestVersion + ') of service on this Ship.';
    }

    strTemplate += '<small data-placement="left" data-toggle="tooltip" title="' + strToolTip + '" class="label ' + strClassSuccessOrDanger + ' clsLable" style="font-size: 15px !important;"><i class="' + strClassSpinOrPause + '"></i> ' + runningVersion + ' ' + serviceStatus + '</small>';

    /*
    if (isRunning)
        strTemplate += '<small data-placement="left" data-toggle="tooltip" title="Running ' + strToolTip + '" class="label label-success clsLable"><i class="fa fa-spinner fa-spin"></i> ' + runningVersion + ' Running</small>';
    else {
        if ((downloadDate != null && downloadDate != undefined) && (lastupdatedate == null)) {
            strTemplate += '<small data-placement="left" data-toggle="tooltip" title="Old version is running" class="label label-warning clsLable"><i class="fa fa-spinner fa-spin"></i> 1.0 Running</small>';
        }
        else {
            if (strToolTip != "") {
                strToolTip = 'data-placement="left" data-toggle="tooltip" title="Stop ' + strToolTip + '"';
            }
            else {
                strToolTip = 'data-placement="left" data-toggle="tooltip" title="Please install latest version of this service on this Ship"';
            }
            strTemplate += '<small ' + strToolTip + ' class="label label-danger clsLable"><i style="font-size:10px" class="fa fa-pause"></i> Stop</small>';
        }
    }
    */

    strTemplate += '</div>';
    return strTemplate;
}
// End RDBJ 04/13/2022

// RDBJ2 04/01/2022
function PerformClientDeleteSIRNoteOrSIRAdditionalNote(data) {
    if (data.Status == str_Status_Success) {
        $("#FormVersion").val(data.FormVersion);
        $("#" + data.NotesUniqueID).css("display", "none");
        $("#" + data.NotesUniqueID).addClass("hide");   // RDBJ 04/02/2022

        $.notify("Performed Action done Successfully!", "success");
    }
};
// End RDBJ2 04/01/2022

// RDBJ2 02/26/2022
function PerformClientUpdateMainSyncServiceSettings(data) {
    if (data.Status == str_Status_Success) {
        PerformClientActionNotifyStatusSuccessOrError(data);
    }
};
// End RDBJ2 02/26/2022

// RDBJ2 02/26/2022
function PerformClientGetMainSyncServiceSettings(data) {
    if (data.Status == str_Status_Success) {
        if (data.MainSyncSettings != null) {
            var mainSyncServiceSettings = JSON.parse(data.MainSyncSettings);

            $('#lblServerTimeInterval').html(mainSyncServiceSettings.ServerSettings.IntervalTime + " Mins.");
            $('#lblServerUpdatedBy').html(mainSyncServiceSettings.ServerSettings.UpdatedBy);
            $('#lblServerUpdatedDate').html(mainSyncServiceSettings.ServerSettings.UpdatedDate);

            $('#ddLocalTimeInterval').val(mainSyncServiceSettings.LocalSettings.IntervalTime);
            $('#lblLocalUpdatedBy').html(mainSyncServiceSettings.LocalSettings.UpdatedBy);
            $('#lblLocalUpdatedDate').html(mainSyncServiceSettings.LocalSettings.UpdatedDate);

            $('#mainSyncServiceVersion').html(mainSyncServiceSettings.ServerSettings.MainSyncServiceVersion);

            // RDBJ 03/08/2022 wrapped in if
            if (mainSyncServiceSettings.LocalSettings != null) {
                let boolStatus = (mainSyncServiceSettings.LocalSettings.UseServerTimeInterval.toLowerCase() === 'true');
                $("#chkbUseLocalTimeInterval").prop("checked", boolStatus);
            }
        }
    }
};
// End RDBJ2 02/26/2022

// RDBJ2 03/31/2022
function PerformClientUpdateAuditNoteDetails(data) {
    PerformClientActionNotifyStatusSuccessOrError(data);
};
// End RDBJ2 03/31/2022

// JSL 11/12/2022
function PerformClientDeleteOtherShipsDataFromDatabase(data) {
    PerformClientActionNotifyStatusSuccessOrError(data);
};
// End JSL 11/12/2022

// JSL 07/05/2022
function IsNullEmptyOrUndefined(strValue) {
    var blnReturn = false;
    if (typeof (strValue) == "undefined"
        || strValue == "undefined"
        || strValue == null
        || strValue == ""
        || strValue.length == 0
        || strValue == -1) {
        blnReturn = true;
    }
    return blnReturn;
};
// End JSL 07/05/2022

// JSL 07/05/2022
function IsNullEmptyOrUndefined_Triple(strValue) {
    var blnReturn = false;
    if (typeof (strValue) === "undefined"
        || strValue === "undefined"
        || strValue === null
        || strValue === ""
        || strValue.length === 0
        || strValue === -1) {
        blnReturn = true;
    }
    return blnReturn;
}
// End JSL 07/05/2022

// JSL 07/18/2022
function closeThisWindow() {
    window.close();
}
// End JSL 07/18/2022