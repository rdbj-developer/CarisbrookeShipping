// RDBJ 02/08/2022 Added this script

var str_Status_Success = 'Success'; // RDBJ 02/24/2022
var str_Status_Error = 'Error'; // RDBJ 02/24/2022

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

// RDBJ2 02/23/2022
var CommonServerPostApiCall = function (dictMetadata, controllerName, apiAction, strAction
    , global = true,   // JSL 05/16/2022
) {
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
// End RDBJ2 02/23/2022


// RDBJ2 02/23/2022
function PerformClientActionsBasedOnCommonServerCallResponse(data, strAction){
    switch (strAction) {
        // JSL 07/23/2022
        case str_API_GETFORMSPERSONLIST:
            PerformClientGetFormsPersonList(data);
            break;
        // End JSL 07/23/2022
        // JSL 07/23/2022
        case str_API_API_ADDNEW_UPDATE_DELETE_FORMSPERSON:
            PerformClientAddNewOrUpdateOrDeleteFormPerson(data);
            break;
        // End JSL 07/23/2022
        // JSL 05/10/2022
        case str_API_DELETEDEFICIENCYFILE:
            PerformClientDeleteDeficiencyFile(data);
            break;
        // End JSL 05/10/2022
        // JSL 05/01/2022
        case str_API_OPENANDSEENNOTIFICATION:
            PerformClientOpenAndSeenNotification(data);
            break;
        // End JSL 05/01/2022
        // JSL 04/30/2022
        case str_API_GETNOTIFICATION:
            PerformClientGetNotification(data);
            break;
        // End JSL 04/30/2022
        // JSL 07/04/2022
        case str_API_GETNOTIFICATIONFROMPAGE:
            PerformClientGetNotificationForPage(data);
            break;
        // End JSL 07/04/2022
        // RDBJ 04/02/2022
        case str_API_DELETESIRNOTEORSIRADDITIONALNOTE:
            PerformClientDeleteSIRNoteOrSIRAdditionalNote(data);
            break;
        // End RDBJ 04/02/2022
        // RDBJ 03/19/2022
        case str_API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS:
            PerformClientUpdateDeficienciesShipWhenChangeShipInForms(data);
            break;
        // End RDBJ 03/19/2022
        // RDBJ 02/24/2022
        case str_API_UPDATEMAINSYNCSERVICESETTINGS:
            PerformClientUpdateMainSyncServiceSettings(data);
            break;
        // End RDBJ 02/24/2022
        // RDBJ 02/24/2022
        case str_API_GETMAINSYNCSERVICESETTINGS:
            PerformClientGetMainSyncServiceSettings(data);
            break;
        // End RDBJ 02/24/2022
        case str_API_UPDATEAUDITNOTEDETAILS:
            PerformClientUpdateAuditNoteDetails(data);
            break;
        default:
        // code block
    }
};
// End RDBJ2 02/23/2022

// RDBJ 02/24/2022
function PerformClientActionNotifyStatusSuccessOrError(data) {
    // RDBJ 02/24/2022 Moved from CommonServerPostApiCall to Success PerformClient
    if (data.Status == str_Status_Success) {
        $.notify("Performed Action done Successfully!", "success");
    }
    else {
        $.notify("Something went wrong!", "error");
    }
    // End RDBJ 02/24/2022 Moved from CommonServerPostApiCall to Success PerformClient
}
// End RDBJ 02/24/2022

// JSL 07/23/2022
function PerformClientAddNewOrUpdateOrDeleteFormPerson(data) {
    PerformClientActionNotifyStatusSuccessOrError(data);
    LoadFormsPersonGrid();
}
// End JSL 07/23/2022

// JSL 07/23/2022
function PerformClientGetFormsPersonList(data) {
    if (data.Status == str_Status_Success) {
        var lstFormsPerson;
        if (!IsNullEmptyOrUndefined(data.FormsPersonList)) {
            lstFormsPerson = JSON.parse(data.FormsPersonList);
        }
        
        BindFormsPersonGrid(lstFormsPerson);
    }
}
// End JSL 07/23/2022

// JSL 05/10/2022
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
        
        $(element).parent().parent().remove();    // JSL 12/03/2022 added .parent() and remove() from hide()
        $.notify("Performed Action done Successfully!", "success");
    }
}
// End JSL 05/10/2022

// JSL 04/30/2022
function PerformClientGetNotification(data) {
    if (data.Status == str_Status_Success) {
        var ISMDashBoardNotification = document.getElementById('ISMDashBoardNotification');

        // JSL 12/19/2022
        if (IsNullEmptyOrUndefined(ISMDashBoardNotification)) {
            return;
        }
        // End JSL 12/19/2022

        $('#notificationdisplay').empty();
        if (data.CurrentUserNotifications != "") {
            var lstNotifications = JSON.parse(data.CurrentUserNotifications);
            
            if (lstNotifications.length == 0) {
                ISMDashBoardNotification.classList.remove("notification-counter");
                ISMDashBoardNotification.innerHTML = "";

                $('#notificationdisplay').append($('<div class="nothing"><i class="fa fa-child stick"></i><div class="cent">Looks Like your all caught up!</div></div>'));
            }
            else {
                ISMDashBoardNotification.classList.add("notification-counter");
                ISMDashBoardNotification.innerHTML = lstNotifications.length;
                var strDisplayNotificationHTML = '<div class="cont">';

                $.each(lstNotifications, function (index, item) {
                    //$('#notificationdisplay').append($('<li>New contact : ' + item.UniqueDataId + ' (' + item.DataType + ')</li>'));

                    var strClassIsRead = ''
                        , strClassOrStyleIsDraft
                        , strTitle
                        , strDetailsURL
                        , dtCreatedOrReadDate
                        , strShipCode   // JSL 05/25/2022
                        ;

                    strDetailsURL = item.DetailsURL;    // JSL 05/25/2022
                    strShipCode = item.ShipCode;    // JSL 05/25/2022
                    strTitle = item.Title;

                    if (!JSON.parse(item.IsRead)) {
                        strClassIsRead = 'new';
                    }

                    if (JSON.parse(item.IsDraft)) {
                        strClassOrStyleIsDraft = 'bg-warning text-dark';
                    }
                    else {
                        strClassOrStyleIsDraft = 'bg-info text-dark';

                        // JSL 05/25/2022
                        var strFormType = '';
                        if (strTitle.includes("General")) {
                            strFormType = 'gi';
                        }
                        else if (strTitle.includes("Superintended")) {
                            strFormType = 'si';
                        }
                        else if (strTitle.includes("Internal")) {
                            strFormType = 'ia';
                        }

                        if (!IsNullEmptyOrUndefined(strFormType)) {
                            strDetailsURL += '?ship=' + strShipCode + '&formtype=' + strFormType;
                        }
                        // End JSL 05/25/2022
                    }
                    
                    if (item.ReadDateTime == "") {
                        var dtVaue = item.CreatedDateTime;
                        dtCreatedOrReadDate = moment(dtVaue).fromNow();
                    }
                    else {
                        var dtVaue = item.ReadDateTime;
                        dtCreatedOrReadDate = 'Seen : ';
                        dtCreatedOrReadDate += moment(dtVaue).fromNow();
                    }

                    // JSL 07/12/2022
                    strDisplayNotificationHTML += '<div class="parentSection_' + item.UniqueId + '">';
                    strDisplayNotificationHTML += '<div class="success" data-closable="slide-out-right" style="float: right; margin-top: 5px; margin-right: 5px;">';
                    strDisplayNotificationHTML += '<button data-id="' + item.UniqueId + '" style="border-radius: 15px; padding: 1px 6px !important;" class="btn btn-danger closeNotification" aria-label="Dismiss alert" type="button" data-close>';
                    strDisplayNotificationHTML += '<i class="fa fa-times" aria-hidden="true"></i>';
                    strDisplayNotificationHTML += '</button>';
                    strDisplayNotificationHTML += '</div>';
                    // End JSL 07/12/2022

                    strDisplayNotificationHTML += '<div class="sec ' + strClassIsRead + ' ' + strClassOrStyleIsDraft + '" data-id="' + item.UniqueId + '" data-detailsurl="' + strDetailsURL + '">';
                    strDisplayNotificationHTML += '<div class="profCont">';
                    strDisplayNotificationHTML += '<img class="profile" avatar="' + strTitle + '">';
                    strDisplayNotificationHTML += '</div>';
                    strDisplayNotificationHTML += '<div class="txt">' + strTitle + ' - ' + item.ShipName + '</div>';
                    strDisplayNotificationHTML += '<div class="txt"> Created by - ' + item.CreatedByPerson + '</div>';
                    strDisplayNotificationHTML += '<div class="txt"> Form Date - ' + moment(item.DataDate).format('DD-MM-YYYY') + '</div>';
                    strDisplayNotificationHTML += '<div class="txt sub">' + dtCreatedOrReadDate + '</div>';
                    strDisplayNotificationHTML += '</div>';
                    strDisplayNotificationHTML += " </div>";
                });

                strDisplayNotificationHTML += " </div>";

                $('#notificationdisplay').append($(strDisplayNotificationHTML));
                LetterAvatar.transform();

                $('div.sec').click(function () {
                    var row = $(this);

                    var NotDataId = row.attr("data-id");
                    var NotDetailsURL = row.attr("data-detailsurl");

                    var dic = {};
                    dic["NotDataId"] = NotDataId;
                    dic["NotDetailsURL"] = NotDetailsURL;
                    dic["CurrentUserID"] = _CurrentUserDetailsObject.UserGUID;

                    CommonServerPostApiCall(dic, "Notifications", "PerformAction", str_API_OPENANDSEENNOTIFICATION);
                });

                // JSL 07/12/2022
                $('button.closeNotification').on('click', function () {
                    var row = $(this);
                    var NotDataId = row.attr("data-id");

                    var dic = {};
                    dic["NotDataId"] = NotDataId;
                    dic["NotDetailsURL"] = "";
                    dic["CurrentUserID"] = _CurrentUserDetailsObject.UserGUID;

                    CommonServerPostApiCall(dic, "Notifications", "PerformAction", str_API_OPENANDSEENNOTIFICATION);
                });
                // End JSL 07/12/2022
            }
        }
    }
}
// End JSL 04/30/2022

// JSL 07/04/2022
function PerformClientGetNotificationForPage(data) {
    if (data.Status == str_Status_Success) {
        if (!IsNullEmptyOrUndefined(data.CurrentUserNotifications)) {
            ShowNotificationForForms(data.CurrentUserNotifications)
        }

        if (!IsNullEmptyOrUndefined(data.DeficienciesNotificationsList)) {
            ShowNotificationForDeficiencies(data.DeficienciesNotificationsList)
        }
    }
}
// End JSL 07/04/2022

// JSL 05/01/2022
function PerformClientOpenAndSeenNotification(data) {
    if (data.Status == str_Status_Success) {
        if (data.NotDetailsURL != "") {
            var _url = RootUrl + data.NotDetailsURL;
            window.open(_url, '_blank');
        }
        else {
            $('.parentSection_' + data.NotDataId).animate({
                //padding: "0px",
                //'margin-left': '-10px',
                //'font-size': "0px",
                transition: "1s",
                left: "0"
            }, 500, function () {
                $('.parentSection_' + data.NotDataId).remove();
            });
        }
    }
}
// End JSL 05/01/2022

// RDBJ 04/02/2022
function PerformClientDeleteSIRNoteOrSIRAdditionalNote(data) {
    if (data.Status == str_Status_Success) {
        $("#FormVersion").val(data.FormVersion);
        $("#" + data.NotesUniqueID).css("display", "none");
        $("#" + data.NotesUniqueID).addClass("hide");

        $.notify("Performed Action done Successfully!", "success");
    }
};
// End RDBJ 04/02/2022

function PerformClientUpdateDeficienciesShipWhenChangeShipInForms(data) {
    if (data.Status == str_Status_Success) {
        $("#FormVersion").val(data.FormVersion);
    }
};

// RDBJ2 02/24/2022
function PerformClientUpdateMainSyncServiceSettings(data) {
    if (data.Status == str_Status_Success) {
        PerformClientActionNotifyStatusSuccessOrError(data);
    }
};
// End RDBJ2 02/24/2022

// RDBJ2 02/24/2022
function PerformClientGetMainSyncServiceSettings(data) {
    if (data.Status == str_Status_Success) {
        let boolStatus = (data.UseServerTimeInterval.toLowerCase() === 'true');
        
        $('#mainSyncServiceVersionTimeInterval').val(data.IntervalTime);
        $('#mainSyncServiceUpdatedBy').html(data.UpdatedBy);
        $('#mainSyncServiceUpdatedDate').html(data.UpdatedDate);
        $('#mainSyncServiceVersion').html(data.MainSyncServiceVersion);
        $("#mainSyncServiceUseServerTimeInterval").prop("checked", boolStatus);
    }
};
// End RDBJ2 02/24/2022

// RDBJ2 02/23/2022
function PerformClientUpdateAuditNoteDetails(data) {
    PerformClientActionNotifyStatusSuccessOrError(data);  // RDBJ 02/24/2022
};
// End RDBJ2 02/23/2022

// JSL 05/25/2022
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
// End JSL 05/25/2022

// JSL 05/25/2022
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
// End JSL 05/25/2022

// JSL 07/18/2022
function closeThisWindow() {
    window.close();
}
// End JSL 07/18/2022

// JSL 07/23/2022
function setDropDownSelectedIndexByTextValue(ddlId, textToFind) {
    var dd = document.getElementById(ddlId);
    for (var i = 0; i < dd.options.length; i++) {
        if (dd.options[i].text === textToFind) {
            dd.selectedIndex = i;
            break;
        }
    }
};
// End JSL 07/23/2022

// JSL 07/23/2022
function GetAllPersonList(intPersonType) {
    var dic = {};
    dic["PersonType"] = intPersonType;
    CommonServerPostApiCall(dic, "Admin", "PerformAction", str_API_GETFORMSPERSONLIST);
}
// End JSL 07/23/2022