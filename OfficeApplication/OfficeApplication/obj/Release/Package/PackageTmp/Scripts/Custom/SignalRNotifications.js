// JSL 06/25/2022 added this file

// JSL 06/25/2022
$(document).ready(function () {
    var hub = $.connection.notificationsHub;
    $.connection.hub.start()
        .done(function () {
            //console.log("Hub Connected!");
        })
        .fail(function () {
            console.log("Could not Connect!");
        });

    hub.client.broadcaastNotificationForForms = function (data) {
        ShowNotificationForForms(data);
    }

    hub.client.broadcaastNotificationForDeficiencies = function (data) {
        ShowNotificationForDeficiencies(data);
    }

    // JSL 06/30/2022
    hub.client.broadcaastNotificationForToster = function (data) {
        ShowNotificationForToster(data);
    }
    // End JSL 06/30/2022

    hub.client.notify = function (message) {
        $('#resultMessage').html(message);
    }
});
// End JSL 06/25/2022

// JSL 06/25/2022
function ShowNotificationForForms(data) {
    var dic = {};
    dic["Status"] = str_Status_Success;
    dic["CurrentUserNotifications"] = data;
    PerformClientGetNotification(dic);
}
// End JSL 06/25/2022

// JSL 06/25/2022
function ShowNotificationForDeficiencies(data) {
    var jsonData = JSON.parse(data);
    var notificationCounter = document.getElementById('NewnotificationCounter');

    if (!IsNullEmptyOrUndefined(notificationCounter)) {
        _AllDeficienciesNotificationsData = jsonData;

        if (_AllDeficienciesNotificationsData.length > 0) {
            notificationCounter.classList.add("notification-counter");
            notificationCounter.innerHTML = jsonData.length;
            
            // Bind Or Update Grid here
        }
        else {
            notificationCounter.classList.remove("notification-counter");
            notificationCounter.innerHTML = "";
        }

        // JSL 06/27/2022
        // Show notification for DeficienciesDetails or InternalAuditDetails page window.location.href
        var pageURL = window.location.href;
        if (pageURL.includes("DeficienciesDetails") || pageURL.includes("InternalAuditDetails")) {
            ShowDeficienciesNotifications();
        }
        // End JSL 06/27/2022

        // JSL 06/29/2022
        if (pageURL.includes("NewNotification")) {
            //refreshDataSourceWithoutResetGrid(_AllDeficienciesNotificationsData);
            LoadNewNotifications(_AllDeficienciesNotificationsData);
        }
        // End JSL 06/29/2022
    }
}
// End JSL 06/25/2022

// JSL 06/30/2022
function ShowNotificationForToster(data) {
    var jsonData = JSON.parse(data);

    $.each(jsonData, function (index, item) {
        SendNotificationToUser(item);
    });
}
// End JSL 06/30/2022

// JSL 06/30/2022
function SendNotificationToUser(data) {
    var strShipName = ''
        , strReportType = ''
        , strNumber = ''
        , strDeficiency = ''
        , strPriority = ''
        , strDataType = ''
        , strCreatedDateTime = '';

    strShipName = data.ShipName;

    if (data.ReportType) {
        if (data.ReportType == 'GI') {
            strReportType = "General Inspection";
        }
        else if (data.ReportType == 'SI') {
            strReportType = "Supretendent Inspection";
        }
        else {
            strReportType = "Internal Audit";
        }
    }
    
    strNumber = data.Number;
    strDeficiency = data.Deficiency;
    strPriority = data.Priority;
    strDataType = data.DataType;
    strCreatedDateTime = moment(data.CreatedDateTime).fromNow();

    $.notify.addStyle('foo', {
        html:
            '<div class="toast" role="alert" aria-live="assertive" aria-atomic="true">' +
            '<div class="toast-header">' +
            '<img src="/OfficeApplication/Images/office.png" class="rounded mr-2" style="width: 20px;" alt="...">' +
            '<strong class="mr-auto"> ' + strShipName + ' - ' + strReportType + ' </strong>' +
            '<small style="padding-bottom: 5px;">' + strCreatedDateTime + '</small>' +
            '<button type="button" class="ml-2 mb-1 close no" data-dismiss="toast" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>' +
            '<div class="toast-body">' +
            'New <strong class="mr-auto">' + strDataType + ' </strong>' + 'for deficiency number <strong class="mr-auto">' + strNumber + ' </strong>' +
            '</div>' +
            '</div>'
    });

    $.notify({
    }, {
        style: 'foo',
        autoHide: true,
        clickToHide: false,
        position: "right bottom"
    });
}
// End JSL 06/30/2022