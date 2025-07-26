function DisplayServerSettings(ServerObj) {
    $("#txtServerName").val(ServerObj.ServerName);
    $("#txtUserName").val(ServerObj.UserName);
    $("#txtPassword").val(ServerObj.Password);
    if (ServerObj.IsInspector == true) {
        $("#chkbInspector").prop('checked', true);
        $("#chkbInspector").attr('value', 'true');
    } else {
        $("#chkbInspector").prop('checked', false);
        $("#chkbInspector").attr('value', 'false');
    }
    $("#btnServerConnect").text("Server Connected");
    $("#btnServerConnect").attr("disabled", "disabled");
    $("#txtServerName").attr("readonly", "readonly");
    $("#txtUserName").attr("readonly", "readonly");
    $("#txtPassword").attr("readonly", "readonly");
}
function ChangeServerSettings() {
    $("#btnServerConnect").text("Connect Server");
    $("#btnServerConnect").removeAttr("disabled", "disabled");
    $("#txtServerName").removeAttr("readonly", "readonly");
    $("#txtUserName").removeAttr("readonly", "readonly");
    $("#txtPassword").removeAttr("readonly", "readonly");
}
function DisplaySMTPSettings(SMPTObj) {
    $("#txtSMTPServerName").val(SMPTObj.SMPTServerName);
    $("#txtSMTPPort").val(SMPTObj.SMTPPort);
    $("#txtSMTPFromAddress").val(SMPTObj.SMTPFromAddress);
    $("#txtSMTPUserName").val(SMPTObj.SMTPUserName);
    $("#txtSMTPPassword").val(SMPTObj.SMTPPassword);
    if (SMPTObj.IsAuthenticationRequired == true)
        $("#chkIsAuthenticationRequired").attr('checked', true);
    else
        $("#chkIsAuthenticationRequired").attr('checked', false);

    if (SMPTObj.CCEmail != null && SMPTObj.CCEmail != undefined && SMPTObj.CCEmail.length > 0) {
        $("#txtSMTPCC").val(SMPTObj.CCEmail[0]);
        CreateCCEmail(SMPTObj.CCEmail);
    }

    $("#btnSMTPSettings").attr("disabled", "disabled");
    $("#txtSMTPServerName").attr("readonly", "readonly");
    $("#txtSMTPPort").attr("readonly", "readonly");
    $("#txtSMTPFromAddress").attr("readonly", "readonly");
    $("#txtSMTPUserName").attr("readonly", "readonly");
    $("#txtSMTPPassword").attr("readonly", "readonly");
    $("#txtSMTPCC").attr("readonly", "readonly");
    $("#chkIsAuthenticationRequired").attr("disabled", true);
    $("#lnkAddRecipient").hide();
}
function ChangeSMTPSettings() {
    $("#btnSMTPSettings").removeAttr("disabled", "disabled");
    $("#txtSMTPServerName").removeAttr("readonly", "readonly");
    $("#txtSMTPPort").removeAttr("readonly", "readonly");
    $("#txtSMTPFromAddress").removeAttr("readonly", "readonly");
    $("#txtSMTPUserName").removeAttr("readonly", "readonly");
    $("#txtSMTPPassword").removeAttr("readonly", "readonly");
    $("#txtSMTPCC").removeAttr("readonly", "readonly");
    $("#chkIsAuthenticationRequired").removeAttr("disabled");
    $("#lnkAddRecipient").show();
    $(".txtClonedCCEmail").each(function () {
        $(this).removeAttr("readonly", "readonly");
    })
    $(".removeCCEmail").show();
}
function CreateCCEmail(CCEmail) {
    for (var i = 1; i < CCEmail.length; i++) {
        var ccPanelDiv = $(".CCEmailPanel").clone();

        if ($(ccPanelDiv).hasClass("hide")) {
            $(ccPanelDiv).removeClass("hide");
        }

        if ($(ccPanelDiv).hasClass("CCEmailPanel")) {
            $(ccPanelDiv).removeClass("CCEmailPanel").addClass("CloneCCEmailPanel");
        }

        if ($(ccPanelDiv).find(".txtCCEmail").length > 0) {
            $(ccPanelDiv).find(".txtCCEmail").removeClass("txtCCEmail").addClass("txtClonedCCEmail").attr("name", "CCEmail").val(CCEmail[i]).attr("readonly", "readonly");
        }
        $(ccPanelDiv).insertAfter(".CCEmailPanel");
        $(".removeCCEmail").hide();
    }
}
function DownloadSyncService() {

}

//RDBJ 09/17/2021
function SetAppMode(isInternetAvailable) {
    if (isInternetAvailable == "True")
        isInternetAvailable = true;
}
//End RDBJ 09/17/2021

// RDBJ 02/26/2022
function GetMainSyncServiceSettings() {
    var dic = {};

    CommonServerPostApiCall(dic, "Settings", "PerformAction", str_API_GETMAINSYNCSERVICESETTINGS);
}
// End RDBJ 02/26/2022

// RDBJ 02/26/2022
function UpdateMainSyncServiceData() {
    var dic = {};

    dic["IntervalTime"] = $("#ddLocalTimeInterval option:selected").val();
    dic["UseServerTimeInterval"] = $("#chkbUseLocalTimeInterval").is(":checked");
    dic["MainSyncServiceVersion"] = $("#mainSyncServiceVersion").html();
    dic["UpdatedBy"] = _Username;

    CommonServerPostApiCall(dic, "Settings", "PerformAction", str_API_UPDATEMAINSYNCSERVICESETTINGS);
}
// End RDBJ 02/26/2022

// RDBJ 04/13/2022
function GetMainSyncServiceDetailsAndStatus() {
    var dic = {};

    CommonServerPostApiCall(dic, "Settings", "PerformAction", str_API_GETMAINSYNCSERVICEDETAILSANDSTATUS);
}
// End RDBJ 04/13/2022

// JSL 11/12/2022
function ChangeInspectorMode() {
    var value;
    if ($("#chkbInspector").val() == 'true') {
        value = 'false';
    } else {
        value = 'true';
    }
    if (value != "") {
        $("#ajax_loader").show();
        $.ajax({
            type: "POST",
            url: '@Url.Action("ChangeInspector", "Settings")',
            data: { isInspector: value },
            dataType: "json",
            success: function (res) {
                $("#ajax_loader").hide();
                if (res == true) {
                    $.notify("Saved Successfully", "success");
                    $("#chkbInspector").attr('value', value);
                }
                else {
                    $.notify("Error occured!!", "error");
                }
            },
            error: function () {
                $("#ajax_loader").hide();
                $.notify("Error occured!!", "error");
            }
        });
    }
}
// End JSL 11/12/2022

// JSL 11/12/2022
function CleanLocalDBExceptThisShipData() {
    var dic = {};
    CommonServerPostApiCall(dic, "Settings", "PerformAction", str_API_DELETEOTHERSHIPSDATAFROMDATABASE);
}
// End JSL 11/12/2022