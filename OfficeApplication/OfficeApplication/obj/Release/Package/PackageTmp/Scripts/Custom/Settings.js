// RDBJ 02/24/2022 Added this script

$(document).ready(function () {
    GetMainSyncServiceData();

    $(".updateData").bind("change", function (e) {
        UpdateMainSyncServiceData();
    });
});

// RDBJ 02/24/2022
$(document).on('click', '#btnDownloadMainSyncService', function () {
    window.location = RootUrl + 'Admin/DownloadMainSyncServiceSetup';
});
// End RDBJ 02/24/2022

// RDBJ 02/24/2022
function GetMainSyncServiceData() {
    var dic = {};

    CommonServerPostApiCall(dic, "Settings", "PerformAction", str_API_GETMAINSYNCSERVICESETTINGS);
}
// End RDBJ 02/24/2022

// RDBJ 02/24/2022
function UpdateMainSyncServiceData() {
    var dic = {};

    dic["IntervalTime"] = $("#mainSyncServiceVersionTimeInterval option:selected").val();
    dic["UseServerTimeInterval"] = $("#mainSyncServiceUseServerTimeInterval").is(":checked");
    dic["MainSyncServiceVersion"] = $("#mainSyncServiceVersion").html();
    dic["UpdatedBy"] = _Username;

    CommonServerPostApiCall(dic, "Settings", "PerformAction", str_API_UPDATEMAINSYNCSERVICESETTINGS);
}
// End RDBJ 02/24/2022