var auditNoteID = getUrlVars()["id"];// NotesUniqueID guid
var notificationCommentsCounter; //RDBJ 10/21/2021
var notificationResolutionsCounter; //RDBJ 10/21/2021
var userList; // RDBJ 12/21/2021

// JSL 12/03/2022
var fileData = new FormData();  
var _UniqueFormID;
// JSL 12/03/2022
var _strFilePath = '../Images/';    // JSL 12/03/2022
$(document).ready(function () {
    GetAllUsersProfileList(); // RDBJ 12/21/2021

    //RDBJ 10/21/2021
    notificationCommentsCounter = document.getElementById('notificationCommentsCounter');
    notificationResolutionsCounter = document.getElementById('notificationResolutionsCounter');

    //loadNotificationsCount(auditNoteID);  // JSL 06/27/2022 commented this line
    //autoRefresh();    // JSL 06/27/2022 commented this line
    //End RDBJ 10/21/2021

    loadbyDetailtab(auditNoteID);
    //LoadAuditNoteComments(auditNoteID, RootUrl + "Deficiencies/GetAuditDeficiencyComments"); //RDBJ 11/13/2021 Commented this line to avoid multiple call

    var height = $(".content-wrapper").height();
    $(".content").height(height - 150);
    resizeWindow();
    $(window).resize(function () {
        resizeWindow();
    });

    $("#lnkBackToDoc").click(function () {
        //var confirm_result = confirm("Are you sure you want to quit?");
        //if (confirm_result == true) {
        //}
        window.close();
    });

    // JSL 07/04/2022 commented
    /*
    // RDBJ 12/21/2021
    if (vbUserGroup != "5"  // Set 5 for ISM Group Only
        && vbUserGroup != "1"   // Set 1 for Admin Group Only
    ) {
        $("#ddUserList").prop("disabled", true);
        $("#btnRemoveAssignedPerson").remove(); // JSL 07/02/2022
    }
    else {
        $("#ddUserList").prop("disabled", false);
        $("#btnRemoveAssignedPerson").removeClass("hide");  // JSL 07/02/2022
    }
    // End RDBJ 12/21/2021
    */
    // End JSL 07/04/2022 commented

    

    BindLightGallery(); // JSL 12/03/2022
});
function resizeWindow() {
    var windowhheight = $(window).height();
    if (windowhheight < 460) {
        $('body').addClass("fixed");
        $(".content").css('height', 'auto');
        setTimeout(function () {
            $(".content-wrapper").css('height', 'auto');
        }, 900);
    }
    else {
        $('body').removeClass("fixed");
    }
}
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

function loadbyDetailtab(id, ctr) {
    var url = RootUrl + "Forms/GetAuditNotesById";
    //loadDeficienciesInitialActions(id, url);
    $.ajax({
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: id },
        success: function (data) {
            // JSL 12/21/2022
            var onChangeAddPriority = '', onChangeAssignUser = '', onClickAssignUser = '', onClickUpdateStatus = '';
            if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
                if (_CurrentUserDetailsObject.UserGroup != '8') {
                    onChangeAddPriority = 'onchange="AddPriority()"';
                    onChangeAssignUser = 'onchange="AddAssignToUser(this);"';
                    onClickAssignUser = 'onclick="AddAssignToUser(this, true);"';
                    onClickUpdateStatus = 'onclick="updateStatus(this)"';
                }
            }
            // End JSL 12/21/2022

            _UniqueFormID = data.UniqueFormID;  // JSL 12/03/2022
            console.log(data);
            var typeNumberTitle = "", BriefDescription = "", auditType = "", date = "", nowDate = "", closedDate = "", timeScale = "";
            if (data.BriefDescription != null) {
                BriefDescription = data.BriefDescription;
            }
            if (data.Type != null) {
                auditType = data.Type;
            }
            if (data.CreatedDate != null) {
                var dateString = data.CreatedDate.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = ('0' + (currentTime.getMonth() + 1)).slice(-2);
                var day = ('0' + (currentTime.getDate())).slice(-2);
                var year = currentTime.getFullYear();
                var result = year + "-" + month + "-" + day + " " + currentTime.getHours() + ":" + currentTime.getMinutes();
                date = result;
            }
            if (data.DateClosed != null) {
                var dateString = data.DateClosed.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = ('0' + (currentTime.getMonth() + 1)).slice(-2);
                var day = ('0' + (currentTime.getDate())).slice(-2);
                var year = currentTime.getFullYear();
                var result = year + "-" + month + "-" + day + " " + currentTime.getHours() + ":" + currentTime.getMinutes();
                closedDate = result;
            }

            // RDBJ 12/21/2021
            //var userListHTML = '<select class="form-control" onchange="AddAssignToUser(this);" style="width:auto;" id="ddUserList"><option value=""> Select Inspector </option>';   // JSL 12/21/2022 commented
            var userListHTML = '<select class="form-control" ' + onChangeAssignUser + ' style="width:auto;" id="ddUserList"><option value=""> Select Inspector </option>';
            $.each(userList, function (index, value) {
                //$("#ddUserList").append($("<option></option>").val(value.UserID).html(value.UserName));
                if (data.AssignTo == value.UserID) {
                    userListHTML += '<option value="' + value.UserID + '" selected>' + value.UserName + '</option>';
                }
                else {
                    userListHTML += '<option value="' + value.UserID + '">' + value.UserName + '</option>';
                }
            })
            userListHTML += "</select>";
            // End RDBJ 12/21/2021

            var detailData = '<div class="details">' +
                '<table class="table table-bordered" style="width: 100%;!important">' +
                '<tbody>' +
                '<tr>' +
                '<td style="width: 250px;">Type</td>' +
                '<td>' + auditType + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;">Reference</td>' +
                '<td>' + data.Reference + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;">Brief Description</td>' +
                '<td>' + BriefDescription + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;">Full Description</td>' +
                '<td>' + data.FullDescription + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;">Date Raised</td>' +
                '<td>' + date + '</td>' +
                '</tr >' +
                // JSL 07/09/2022
                '<tr>' +
                '<td style="width: 300px; vertical-align: bottom;">Priority : ' +
                '<table class="table table-borderless" style="width: 100% !important; margin-bottom: 0 !important;">' +
                '<tbody>' +
                '<tr>' +
                '<td class="col-md-2" style="width: 100px; vertical-align: middle; border-right: 1px solid #f4f4f4; padding: 0; border-top: none;"> Time Scale :</td>' +
                '<td style="width: 200px; vertical-align: middle; border-top: none;">' +
                '<div class="has-feedback" style="width: 180px;">' +
                //'<input type="text" class="form-control datepicker txtTimeScale" onchange="AddPriority()" autocomplete="off" />' +  // JSL 12/21/2022 commented
                '<input type="text" class="form-control datepicker txtTimeScale" ' + onChangeAddPriority + ' autocomplete="off" />' +  // JSL 12/21/2022
                '<span class="form-control-feedback arrivalCalendarAddon">' +
                '<i class="fa fa-calendar"></i>' +
                '</span>' +
                '</div>' +
                '</td>' +
                '</tr>' +
                '</tbody>' +
                '</table>' +
                '<td style="vertical-align: bottom;">' +
                '<table class="table table-borderless" style="width: 100% !important; margin-bottom: 0 !important;">' +
                '<tbody>' +
                '<tr>' +
                '<td class="col-md-2" style="width: 170px; vertical-align: middle; border-right: 1px solid #f4f4f4; padding: 0; border-top: none;">Select number of Weeks :</td>' +
                '<td style="border-top: none;">' +
                //'<select class="DeficiencyPriority form-control" id="DeficiencyPriority" onchange="AddPriority(this)" style="width:auto;">' +   // JSL 12/21/2022 commented
                '<select class="DeficiencyPriority form-control" id="DeficiencyPriority" ' + onChangeAddPriority + ' style="width:auto;">' +   // JSL 12/21/2022
                '<option value="12">12</option>' +
                '<option value="8">8</option>' +
                '<option value="4">4</option>' +
                '</select>' +
                '</td>' +
                '</tr>' +
                '<tr>' +
                '</tr>' +
                '</tbody>' +
                '</table>' +
                '</td>' +
                '</tr>' +
                // End JSL 07/09/2022
                '<tr>' +
                '<td style="width: 250px;">Date Closed</td>' +
                '<td>' + closedDate + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;";>Files</td>' +
                '<td style="width: 155vh;">' + generateTemplateDeficiencies(data.AuditNotesAttachment) + '</td>' +    // JSL 12/03/2022 added style
                '</tr >' +
                // RDBJ 12/21/2021
                '<tr>' +
                '<td style="width: 250px;";>Assign to: </td>' +
                //'<td>' + userListHTML + '</td>' + // JSL 07/02/2022 commented this line
                // JSL 07/02/2022
                '<td>' +
                '<div class="col-sm-12">' +
                '<div style="display: flex;">' +
                userListHTML +
                //'<button id="btnRemoveAssignedPerson" class="btn btn-danger hide" style="margin-left: 5px;" onclick="AddAssignToUser(this, true);"><i class="fa fa-trash-o" aria-hidden="true"></i></button>' + // JSL 12/21/2022 commented
                '<button id="btnRemoveAssignedPerson" class="btn btn-danger hide" style="margin-left: 5px;" ' + onClickAssignUser + '><i class="fa fa-trash-o" aria-hidden="true"></i></button>' + // JSL 12/21/2022
                '</div>' +
                '</div>' +
                '</td>' +
                // End JSL 07/02/2022
                '</tr >' +
                // End RDBJ 12/21/2021
                '<tr>' +
                '<td style="width: 250px;";>Is Resolved</td>' +
                //'<td><input onclick="updateStatus(this)" type="checkbox" class="chkbx" id="chkIsClosed" /></td>' +  // JSL 12/21/2022 commented
                '<td><input ' + onClickUpdateStatus + ' type="checkbox" class="chkbx" id="chkIsClosed" /></td>' +  // JSL 12/21/2022
                '</tr >' +
                '</tbody>' +
                '</table>' +
                '</div>';
            if (auditType.includes("-Non")) {
                typeNumberTitle = "NCN \'s";
            } else if (auditType.includes("-Obs")) {
                typeNumberTitle = "OBS \'s";
            } else {
                typeNumberTitle = "MLC \'s";
            }
            var headerData = '<div class="header-inner-common">' +
                '<ul class="headerdetails">' +
                //'<li><span><b>Ship Name</b></span><span>' + data.Ship + '</span></li>' +    // JSL 06/13/2022 commented this line
                '<li><span><b>Ship Name</b></span><span>' + data.ShipName + '</span></li>' +    // JSL 06/13/2022
                '<li><span><b>Form Type</b></span><span>Internal Audit Form Report</span></li>' +
                '<li><span><b>' + typeNumberTitle + ' No</b></span><span>' + data.Number + '</span></li>' +
                '<li><span><b>Type</b></span><span>' + auditType + '</span></li>' +
                '</ul>' +
                //'<a class="back-btn" href="' + RootUrl + 'Forms/GeneralInspectionReport" id = "lnkBackToDoc"> Back</a>' +   // JSL 07/18/2022 commented this line
                '<a class="back-btn" href="return false;" onclick="closeThisWindow();"> Back</a>' +  // JSL 07/18/2022
                '</div>';
            $("#detailGrid").html('');
            $("#detailGrid").append(detailData);  
            $("#divHeaderDetail").html('');
            $("#divHeaderDetail").append(headerData);
            $("#chkIsClosed").prop("checked", data.isResolved);

            var step2AData = '<div class="step2A">' +
                '<table class="table table-bordered" style="width: 100% !important">' +
                '<tbody>' +
                '<tr>' +
                '<td style="width: 250px;">Corrective Action(s)</td>' +
                '<td>' + data.CorrectiveAction + '</td>' +
                '</tr >' +
                '</tbody>' +
                '</table>' +
                '</div>';
            $("#step2AGrid").html('');
            //$("#step2AGrid").append(step2AData); // RDBJ2 02/23/2022 Commented this line
            $('#txtCorrectiveAction').val(data.CorrectiveAction); // RDBJ2 02/23/2022
            var step2BData = '<div class="step2B">' +
                '<table class="table table-bordered" style="width: 100% !important">' +
                '<tbody>' +
                '<tr>' +
                '<td style="width: 250px;">Preventative Action(s)</td>' +
                '<td>' + data.PreventativeAction + '</td>' +
                '</tr >' +
                '</tbody>' +
                '</table>' +
                '</div>';
            $("#step2BGrid").html('');
            //$("#step2BGrid").append(step2BData); // RDBJ2 02/23/2022 Commented this line
            $('#txtPreventiveAction').val(data.PreventativeAction); // RDBJ2 02/23/2022

            if (data.TimeScale) {
                var dateString = data.TimeScale.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = ('0' + (currentTime.getMonth() + 1)).slice(-2);
                var day = ('0' + (currentTime.getDate())).slice(-2);
                var year = currentTime.getFullYear();
                //var result = year + "-" + month + "-" + day + " " + currentTime.getHours() + ":" + currentTime.getMinutes(); // RDBJ2 02/23/2022 Commented hh:mm
                var result = day + "/" + month + "/" + year;    // RDBJ2 02/23/2022
                timeScale = result;
            }
            var responsibleData = '<div class="Responsible">' +
                '<table class="table table-bordered" style="width: 100% !important">' +
                '<tbody>' +
                '<tr>' +
                '<td style="width: 250px;">Rank:</td>' +
                '<td>' + data.Rank + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;">Name:</td>' +
                '<td>' + data.Name + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 250px;">TimeScale:</td>' +
                '<td>' + timeScale + '</td>' +
                '</tr >' +
                '</tbody>' +
                '</table>' +
                '</div>';
            $("#responsibleGrid").html('');
            //$("#responsibleGrid").append(responsibleData); // RDBJ2 02/23/2022 Commented this line

            // RDBJ2 02/23/2022
            $('#txtRank').val(data.Rank); 
            $('#txtName').val(data.Name);
            $('.txtTimeScale').val(timeScale);
            // End RDBJ2 02/23/2022

            //RDBJ 11/13/2021
            if (data.Priority != 0)
                $("#DeficiencyPriority").val(data.Priority);
            else
                $("#DeficiencyPriority").val(12);
            //End RDBJ 11/13/2021

            // JSL 07/13/2022
            $('.datepicker').datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
            });
            // End JSL 07/13/2022

            SetRestrictionForSomeUserToModify();    // JSL 12/21/2022
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
function updateStatus(ctr) {
    // JSL 12/21/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/21/2022

    var NoteID = auditNoteID;
    var url = RootUrl + "Deficiencies/UpdateAuditDeficiencies";
    var obj = {};
    obj.id = NoteID;
    obj.isClose = $(ctr).prop('checked');
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Updated Successfully", "success");
            loadbyDetailtab(auditNoteID);
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
function generateTemplateDeficiencies(ReportList) {
    // JSL 12/10/2022 uncommented
    // JSL 12/03/2022 commented
    var template = "";
    var html = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div class="chip pink lighten-2 white-text waves-effect waves-effect file">' +   // JSL 05/10/2022 removed style="line-height: unset;"
            //'<a onclick="Download(\'' + ReportList[i].NotesFileUniqueID + '\',this)">' + ReportList[i].FileName + '</a>';   // JSL 12/10/2022 commented
            '<a class="scroll" href="javascript:void(0);" data-id="' + ReportList[i].NotesFileUniqueID + '" onclick="moveToAttachment(this);">' + ReportList[i].FileName + '</a>';  // JSL 12/10/2022

        // JSL 05/12/2022 wrapped in if
        if (vbUserGroup == "1")
            template = template + '<i data-toggle="tooltip" data-placement="bottom" data-original-title="Delete" style="padding-left: 10px;" class="close fa fa-trash" data-id="' + ReportList[i].AuditNotesAttachmentId + '" onclick="RemoveDeficienciesFile(this)"></i>'; // JSL 05/10/2022

        template = template + '<i data-toggle="tooltip" data-placement="bottom" data-original-title="Download" style="padding-left: 10px;" class="close fa fa-download" onclick="Download(' + ReportList[i].NotesFileUniqueID + ', this)"></i>'; // JSL 12/10/2022

        template = template + '</div>';

        // JSL 12/10/2022
        var strFileDataId = ReportList[i].NotesFileUniqueID;
        var strFileDataDeleteId = ReportList[i].AuditNotesAttachmentId;
        var strFileName = ReportList[i].FileName;
        var strFilePathAndName = _strFilePath + ReportList[i].StorePath;
        var strPathForView = strFilePathAndName;

        var strFileNameToCheck = strFilePathAndName.split("/").pop();
        var strClassNameForGallery = 'itemImage';

        var blnIsImage = /\.(jpe?g|png|gif|bmp)$/i.test(strFileNameToCheck);
        if (!blnIsImage) {
            strPathForView = _strFilePath + "default_document_image.jpg";
            strClassNameForGallery = "";
        }

        html = html + '<div id="img_' + strFileDataId + '" class="item">' +
            '<a class="' + strClassNameForGallery + '" data-src="' + strPathForView + '" href="' + strFilePathAndName + '">' +
            '<img class="imgClass img" src="' + strPathForView + '" alt="' + strFileName + '" />' +
            '</a>' +
            '</div>';
        // End JSL 12/10/2022
    }
    $("#defAttachment").html('');   // JSL 12/21/2022
    $("#defAttachment").append(html);
    // End JSL 12/03/2022 commented
    // End JSL 12/10/2022 uncommented

    // JSL 12/10/2022 commented
    /*
    // JSL 12/03/2022
    var template = '<div class="container-fluid box" id="defFiles-gallery">';
    for (var i = 0; i < ReportList.length; i++) {
        var strFileDataId = ReportList[i].NotesFileUniqueID;
        var strFileDataDeleteId = ReportList[i].AuditNotesAttachmentId;
        var strFileName = ReportList[i].FileName;
        var strFilePathAndName = _strFilePath + ReportList[i].StorePath;
        var strPathForView = strFilePathAndName;

        var strFileNameToCheck = strFilePathAndName.split("/").pop();
        var strClassNameForGallery = 'itemImage';

        var blnIsImage = /\.(jpe?g|png|gif|bmp)$/i.test(strFileNameToCheck);
        if (!blnIsImage) {
            strPathForView = _strFilePath + "default_document_image.jpg";
            strClassNameForGallery = "";
        }

        var html = '<div class="item">' +
            '<a class="' + strClassNameForGallery + '" data-src="' + strPathForView + '" href="' + strFilePathAndName + '" download>' +
            '<img class="imgClass img" src="' + strPathForView + '" alt="' + strFileName + '" />' +
            '</a>';

        html += '<div style="text-align:center; display: flex;">' +
            '<a class="btn btn-success downloadFileBtn" onclick="Download(\'' + strFileDataId + '\',this)" data-toggle="tooltip" data-placement="bottom" data-original-title="Download ' + strFileName + '" >' +
            '<i style="padding-right:5px" class="fa fa-download"></i>' + strFileName +
            '</a >';

        if (vbUserGroup == "1") {
            html += '<button class="btn btn-danger" data-id="' + strFileDataDeleteId + '" data-toggle="tooltip" data-placement="bottom" data-original-title="Delete" onclick="RemoveDeficienciesFile(this)" style="margin-left: 5px;"><i class="fa fa-trash-o" aria-hidden="true"></i></button>';
        }
        html += '</div >' +
            '</div>';

        template += html;
    }
    template += '</div>';
    // End JSL 12/03/2022
    */
    // End JSL 12/10/2022 commented

    return template;
}
function generateTemplate(ReportList) {
    var template = "";
    if (ReportList && ReportList.length > 0) {
        for (var i = 0; i < ReportList.length; i++) {
            template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                '<a onclick="DownloadCommentFile(\'' + ReportList[i].CommentFileUniqueID + '\',this)">' + ReportList[i].FileName + '</a>' +
                '</div>';
        }
    }
    return template;
}
function tabChangeLoadGridData(type, element) {
    if (type == "Resolution") {
        loadbyResolutiontab();
    }
    //else if (type == "InitialActions") {
    //    loadbyInitialActiontab();
    //}
    else if (type == "Details") {
        loadbyDetailtab(auditNoteID);
    }
    //RDBJ 11/13/2021 added else if for Load Comments
    else if (type == "Comments") {
        LoadAuditNoteComments(auditNoteID, RootUrl + "Deficiencies/GetAuditDeficiencyComments");
    }

    //RDBJ 11/13/2021
    if (element.querySelector("span") != null) {
        //RDBJ 10/21/2021
        if (element.querySelector("span").innerText != "") {
            // JSL 06/27/2022
            let numberOfNewRow = parseInt(element.querySelector("span").innerText);
            $('tr[role="row"]:nth-child(-n+' + numberOfNewRow + ')').addClass("fadeOutBackGroundColourOfNotification"); // JSL 07/01/2022 added tr[role="row"]
            // JSL 06/27/2022

            openAndSeenNotificationsUpdateStatus(type);
        }
    }
}

function loadAuditNoteResolution(id, url) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: id },
        success: function (Data) {
            dataLoad = new kendo.data.DataSource({
                data: Data
            });

        },
        error: function (data) {
            console.log(data);
        }
    });
}
function loadbyResolutiontab() {
    var url = RootUrl + "IAFList/GetAuditNoteResolution";
    loadAuditNoteResolution(auditNoteID, url);
    //var childGrid = $(ctr).parent().parent().parent().parent().find('.resoultionList');
    $("#resolutionGrid").kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        noRecords: true,
        pageable: {
            pageSize: 5
        },
        messages: {
            noRecords: "No record found."
        },
        dataSource: {
            data: dataLoad.options.data
        },
        dataBound: function (e) {
            $("#resolutionGrid .k-grid-content").css("min-height", "170px");
            //RDBJ 10/21/2021
            var rows = e.sender.tbody.children();
            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);
                var isNew = $(row).find('td:eq(4)').text();
                if (isNew == "1") {
                    row.css("background-color", "rgb(165, 229, 255)");
                    row.css("color", "black");
                }
            }
            //End RDBJ 10/21/2021

            BindToolTipsForGridText("resolutionGrid", "Resolution", 120);   // RDBJ 02/11/2022
        },
        columns: [
            {
                field: "UserName",
                title: "Name",
                width: "150px"
            },
            {
                field: "Resolution",
                title: "Resolution",
                attributes: { class: "tooltipText" },   // RDBJ 02/11/2022
            },
            {
                field: "CreatedDate",
                title: "Date Time",
                template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy h:mm'):'' #",
                width: "130px"
            },
            {
                field: 'GIRDeficienciesResolutionFiles',
                title: 'Files',
                template: "#=generateTemplateResolution(AuditNoteResolutionsFiles)#",
                //width: "130px"
            },
            //RDBJ 10/21/2021
            {
                field: "isNew",
                title: "New",
                hidden: true
            }
        ]
    });
}

function Download(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/DownloadAuditFile?file=" + ctr + "&name=" + _name;;
    $.ajax({
        url: url,
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        type: "GET",
        async: false,
        success: function () {
            window.location = url;
        }
    });
}


function AddComment(ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = RootUrl + "Deficiencies/AddAuditDeficiencyComments";
    var comment = $(ctr).parent().parent().find('.comment').val();

    // JSL 12/03/2022
    if (IsNullEmptyOrUndefined(comment)) {
        $.notify("Please add comment!", "error");
        return;
    }
    // End JSL 12/03/2022

    /*
    var obj = {};
    obj.UserName = name;
    obj.Comment = comment;
    //obj.AuditNoteID = auditNoteID;
    obj.NotesUniqueID = auditNoteID;

    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {
        var Audit_Deficiency_Comments_Files = new Object();
        Audit_Deficiency_Comments_Files.CommentFileID = 0;
        Audit_Deficiency_Comments_Files.CommentsID = 0;
        Audit_Deficiency_Comments_Files.AuditNoteID = auditNoteID;
        Audit_Deficiency_Comments_Files.FileName = $(this).next().val();
        Audit_Deficiency_Comments_Files.StorePath = $(this).next().next().val();
        Audit_Deficiency_Comments_Files.IsUpload = $(this).next().next().next().val();
        filedata.push(Audit_Deficiency_Comments_Files);
    });
    obj.AuditDeficiencyCommentsFiles = filedata;

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: JSON.stringify(obj),
        success: function (Data) {
            $.notify("Comment Added Successfully", "success");
            LoadAuditNoteComments(auditNoteID, RootUrl + "Deficiencies/GetAuditDeficiencyComments");
            $(ctr).parent().parent().find('.comment').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
    */

    // JSL 12/03/2022
    // Adding one more key to FormData object  
    fileData.append("UserName", name);
    fileData.append("Comment", comment);
    fileData.append("NotesUniqueID", auditNoteID);
    fileData.append("UniqueFormID", _UniqueFormID);

    $.ajax({
        type: 'POST',
        contentType: false,
        processData: false,
        async: false,
        url: url,
        data: fileData,
        success: function (Data) {
            $.notify("Comment Added Successfully", "success");
            LoadAuditNoteComments(auditNoteID, RootUrl + "Deficiencies/GetAuditDeficiencyComments");
            $(ctr).parent().parent().find('.comment').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();

            fileData = new FormData();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
    // End JSL 12/03/2022
}
function LoadAuditNoteComments(id, url) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { NotesUniqueID: id },
        success: function (Data) {
            var grid = $('#commentGrid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                noRecords: true,
                pageable: {
                    pageSize: 5
                },
                messages: {
                    noRecords: "No record found."
                },
                dataSource: {
                    data: Data,
                },
                dataBound: function (e) {
                    $("#commentGrid .k-grid-content").css("min-height", "170px");
                    var rows = e.sender.tbody.children();
                    var dataItem = e.sender.dataItem(row);
                    for (var j = 0; j < rows.length; j++) {
                        var row = $(rows[j]);
                        var isExpired = $(row).find('td:eq(8)').text();
                        if (isExpired == "true") {
                            row.css("background-color", "#da3b3b");
                            row.css("color", "white");
                        }

                        //RDBJ 10/21/2021
                        var isNew = $(row).find('td:eq(4)').text();
                        if (isNew == "1") {
                            row.css("background-color", "rgb(165, 229, 255)");
                            row.css("color", "black");
                        }
                        //End RDBJ 10/21/2021
                    }

                    BindToolTipsForGridText("commentGrid", "Comment", 120);   // RDBJ 02/11/2022
                },
                columns: [
                    {
                        field: "UserName",
                        title: "Name",
                        width: "150px"
                    },
                    {
                        field: "Comment",
                        title: "Comment",
                        attributes: { class: "tooltipText" },   // RDBJ 02/11/2022
                    },
                    {
                        field: "CreatedDate",
                        title: "Date Time",
                        template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy h:mm'):'' #",
                        width: "130px"
                    },
                    {
                        field: 'AuditDeficiencyCommentsFiles',
                        title: 'Files',
                        template: "#=generateTemplate(AuditDeficiencyCommentsFiles)#",
                        //width: "130px"
                        attributes: { style: "white-space: normal !important; text- overflow: unset; !important" }, // JSL 12/03/2022
                    },
                    //RDBJ 10/21/2021
                    {
                        field: "isNew",
                        title: "New",
                        hidden: true
                    }
                ]
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function DownloadCommentFile(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/DownloadCommentFile?CommentFileID=" + ctr + "&name=" + _name;;
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
function LoadCommentFiles(id, url) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: id },
        success: function (Data) {
            AuditDeficiencyCommentFiles = new kendo.data.DataSource({
                data: Data
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function fileUpload(ctr, count) {
    // JSL 01/08/2023
    var image_holder = $("#img-holder");
    var notAllowType = "";
    var fileistoobig = ""; // JSL 05/16/2022
    var fileNametoobig = ""; // JSL 01/08/2023

    for (var i = 0; i < ctr.files.length; i++) {
        if (ctr.files[i].type.indexOf('pdf') >= 0 ||
            ctr.files[i].type.indexOf('image') >= 0 ||
            ctr.files[i].type.indexOf('document') >= 0 ||
            ctr.files[i].type.indexOf('xml') >= 0 ||
            ctr.files[i].type.indexOf('sheet') >= 0) {
            // JSL 05/16/2022 validation for file size and wrapped in if
            if (ctr.files[i].size > 2000000) {
                fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] </br>";
            }
            // JSL 01/08/2023
            else if (ctr.files[i].name.length > 40) {
                fileNametoobig = fileNametoobig + " [" + ctr.files[i].name + "] </br>";
            }
            // End JSL 01/08/2023
            else {
                fileData.append(ctr.files[i].name, ctr.files[i]);   // JSL 12/03/2022
                var data = '<div class="individual-tag"> <div class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                    '<a>' + ctr.files[i].name + '</a>' +
                    '<i class="close fa fa-times" onclick="RemoveFile(this)"></i >' +
                    '</div>' +
                    '<input type="hidden" value="' + ctr.files[i].name + '" class="filename" />' +
                    '<input type="hidden" class="isUpload" value="' + "true" + '" class="IsUpload" /> </div>';
                $(ctr).parent().append(data);
            }

        }
        else {
            notAllowType = notAllowType + " [" + ctr.files[i].name + "] </br>";
        }
    }

    var blnIsNeedTOShowErrorMessage = false;    // JSL 01/08/2023 

    if (notAllowType != "") {
        // JSL 01/08/2023 commented
        /*
        $("#modal-default p").text(notAllowType + " files types are not supported")
        $('#modal-default').modal('show');
        */
        // End JSL 01/08/2023 commented

        // JSL 01/08/2023
        $("#modal-default p#fileTypeError").html("<strong>File types are not supported : </strong></br>" + notAllowType + "");
        blnIsNeedTOShowErrorMessage = true;
        // End JSL 01/08/2023
    }
    // JSL 05/16/2022 added else if
    if (fileistoobig != "") {
        // JSL 01/08/2023 commented
        /*
        $("#modal-default p").text(fileistoobig + " File must be smaller than 2.0 MB")
        $('#modal-default').modal('show');
        */
        // End JSL 01/08/2023 commented

        // JSL 01/08/2023
        $("#modal-default p#fileSizeError").html("<strong>File must be smaller than 2.0 MB : </strong></br>" + fileistoobig + "");
        blnIsNeedTOShowErrorMessage = true;
        // End JSL 01/08/2023
    }

    // JSL 01/08/2023
    if (fileNametoobig != "") {
        $("#modal-default p#fileNameError").html("<strong>File name must be smaller than 40 Character : </strong></br>" + fileNametoobig + "");
        blnIsNeedTOShowErrorMessage = true; // JSL 01/08/2023
    }

    if (blnIsNeedTOShowErrorMessage) {
        $('#modal-default').modal('show');
    }
    // End JSL 01/08/2023

    image_holder.show();
    // End JSL 01/08/2023

    // JSL 01/08/2023 commented
    /*
    if (typeof (FileReader) != "undefined") {
        var image_holder = $("#img-holder");
        var reader = new FileReader();
        var notAllowType = "";
        var fileistoobig = ""; // JSL 05/16/2022
        reader.onload = function (e) {
            for (var i = 0; i < ctr.files.length; i++) {
                if (ctr.files[i].type.indexOf('pdf') >= 0 ||
                    ctr.files[i].type.indexOf('image') >= 0 ||
                    ctr.files[i].type.indexOf('document') >= 0 ||
                    ctr.files[i].type.indexOf('xml') >= 0 ||
                    ctr.files[i].type.indexOf('sheet') >= 0) {
                    // JSL 05/16/2022 validation for file size and wrapped in if
                    if (ctr.files[i].size > 2000000) {
                        fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] ";
                    }
                    else {
                        fileData.append(ctr.files[i].name, ctr.files[i]);   // JSL 12/03/2022

                        //var picFile = e.target; // JSL 12/03/2022 commented
                        var data = '<div class="individual-tag"> <div class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                            '<a>' + ctr.files[i].name + '</a>' +
                            '<i class="close fa fa-times" onclick="RemoveFile(this)"></i >' +
                            '</div>' +
                            '<input type="hidden" value="' + ctr.files[i].name + '" class="filename" />' +
                            //'<input type="hidden" class="path" value="' + picFile.result + '"   class="path" />' +    // JSL 12/03/2022 commented
                            '<input type="hidden" class="isUpload" value="' + "true" + '" class="IsUpload" /> </div>';
                        $(ctr).parent().append(data);
                    }
                    
                }
                else {
                    notAllowType = notAllowType + " [" + ctr.files[i].name + "] ";
                }
            }
            if (notAllowType != "") {
                $("#modal-default p").text(notAllowType + " files types are not supported")
                $('#modal-default').modal('show');
            }
            // JSL 05/16/2022 added else if
            else if (fileistoobig != "") {
                $("#modal-default p").text(fileistoobig + " File must be smaller than 2.0 MB")
                $('#modal-default').modal('show');
            }
        }
        image_holder.show();
        reader.readAsDataURL($(ctr)[0].files[0]);
    } else {
        alert("This browser does not support FileReader.");
    }
    */
    // End JSL 01/08/2023 commented
}
function RemoveFile(ctr) {
    $(ctr).parent().next().next().next().val('false')
    $(ctr).parent().hide();
}

// JSL 05/10/2022
function RemoveDeficienciesFile(ctr) {
    // JSL 12/10/2022 wrapped in if
    var confirm_result = confirm("Are you sure you want to delete this Form?");
    if (confirm_result == true) {
        var DeficienciesFileID = $(ctr).attr("data-id");

        var dic = {};

        dic["DeficienciesFileID"] = DeficienciesFileID;
        dic["DeficienciesUniqueID"] = auditNoteID;
        dic["FormType"] = "IA";

        CommonServerPostApiCall(dic, "Deficiencies", "PerformAction", str_API_DELETEDEFICIENCYFILE);
    }
}
// End JSL 05/10/2022

function AddResolution(ctr) {
    var url = RootUrl + "IAFList/AddAuditNoteResolutions";
    var resolution = $(ctr).parent().parent().find('.resolution').val();
    var name = $(ctr).parent().parent().find('.name').val();
    var obj = {};
    obj.UserName = name;
    obj.Resolution = resolution;
    //obj.AuditNoteID = auditNoteID;
    obj.NotesUniqueID = auditNoteID;
    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {
        var Audit_Note_Resolutions_Files = new Object();
        Audit_Note_Resolutions_Files.CommentFileID = 0;
        Audit_Note_Resolutions_Files.ResolutionID = 0;
        Audit_Note_Resolutions_Files.AuditNoteID = auditNoteID;
        Audit_Note_Resolutions_Files.FileName = $(this).next().val();
        Audit_Note_Resolutions_Files.StorePath = $(this).next().next().val();
        Audit_Note_Resolutions_Files.IsUpload = $(this).next().next().next().val();
        filedata.push(Audit_Note_Resolutions_Files);
    });
    obj.AuditNoteResolutionsFiles = filedata;

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: JSON.stringify(obj),
        success: function (Data) {
            $.notify("Resolution Added Successfully", "success");
            loadbyResolutiontab();
            $(ctr).parent().parent().find('.resolution').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();
            $(ctr).parent().parent().find('.resultionfile').val('');
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
function generateTemplateResolution(ReportList) {
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="DownloadResolutionFile(\'' + ReportList[i].ResolutionFileUniqueID + '\',this)">' + ReportList[i].FileName + '</a>' +
            '</div>';
    }

    return template;
}
function DownloadResolutionFile(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "IAFList/DownloadAuditNoteResolutionFile?ResolutionFileID=" + ctr + "&name=" + _name;;
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

//RDBJ 10/21/2021
function autoRefresh() {
    var intervalId = window.setInterval(function () {
        loadNotificationsCount(auditNoteID);
    }, 5000);
};
//End RDBJ 10/21/2021

//RDBJ 10/21/2021
function loadNotificationsCount(audNotId) {
    var url = RootUrl + 'Notifications/GetCountOfNotificationsById';
    $.ajax({
        contentType: 'application/json',
        async: false,
        dataType: 'json',
        url: url,
        type: 'GET',
        data: { id: audNotId, formType: "IAF" },
        success: function (res) {
            setNotificationsCountForCommenteInitialActionsResolutions(res);
        },
        global: false,
        error: function (err) {
            console.log(err);
        }
    });
};
//End RDBJ 10/21/2021

//RDBJ 10/21/2021
function setNotificationsCountForCommenteInitialActionsResolutions(data) {
    //this is for the Comments Tab
    if (data.totalNewComments == 0) {
        notificationCommentsCounter.classList.remove("notification");
        notificationCommentsCounter.innerHTML = "";
    }
    else {
        notificationCommentsCounter.classList.add("notification");
        notificationCommentsCounter.innerHTML = data.totalNewComments;
    }

    //this is for the Resolution tab
    if (data.totalNewResolutions == 0) {
        notificationResolutionsCounter.classList.remove("notification");
        notificationResolutionsCounter.innerHTML = "";
    }
    else {
        notificationResolutionsCounter.classList.add("notification");
        notificationResolutionsCounter.innerHTML = data.totalNewResolutions;
    }
};
//End RDBJ 10/21/2021

// JSL 06/27/2022
function ShowDeficienciesNotifications() {
    // JSL 12/21/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/21/2022

    let obj = _AllDeficienciesNotificationsData.find(data => data.DeficienciesUniqueID === auditNoteID);

    if (IsNullEmptyOrUndefined(obj)) {
        obj = {
            CommentsCount: 0
            , InitialActionsCount: 0
            , ResolutionsCount: 0
        };
    }

    setNotificationsCountForCommenteInitialActionsResolutionsFromSignalR(obj);
}
// End JSL 06/27/2022

// JSL 06/27/2022
function setNotificationsCountForCommenteInitialActionsResolutionsFromSignalR(data) {
    //this is for the Comments Tab
    if (IsNullEmptyOrUndefined(data.CommentsCount) || data.CommentsCount == 0
        && (IsNullEmptyOrUndefined(data.ResolutionsCount) || data.ResolutionsCount == 0)    // JSL 07/09/2022 added
    ) {
        notificationCommentsCounter.classList.remove("notification");
        notificationCommentsCounter.innerHTML = "";
    }
    else {
        // JSL 07/09/2022
        var numberOfCount = data.CommentsCount;
        if (!IsNullEmptyOrUndefined(data.ResolutionsCount)) {
            numberOfCount = parseInt(numberOfCount) + parseInt(data.ResolutionsCount);
        }
        // End JSL 07/09/2022

        notificationCommentsCounter.classList.add("notification");
        notificationCommentsCounter.innerHTML = data.CommentsCount;
    }

    // JSL 07/09/2022 commented
    /*
    //this is for the Resolution tab
    if (IsNullEmptyOrUndefined(data.ResolutionsCount) || data.ResolutionsCount == 0) {
        notificationResolutionsCounter.classList.remove("notification");
        notificationResolutionsCounter.innerHTML = "";
    }
    else {
        notificationResolutionsCounter.classList.add("notification");
        notificationResolutionsCounter.innerHTML = data.ResolutionsCount;
    }
    */
    // End JSL 07/09/2022 commented
};
// End JSL 06/27/2022

//RDBJ 10/21/2021
function openAndSeenNotificationsUpdateStatus(section) {
    // JSL 12/21/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/21/2022

    var url = RootUrl + 'Notifications/openAndSeenNotificationsUpdateStatusById';
    $.ajax({
        contentType: 'application/json',
        async: false,
        dataType: 'json',
        url: url,
        type: 'GET',
        data: { id: auditNoteID, section: section, formType: "IAF" },
        success: function (res) {
            //if (res == "true")    // JSL 06/27/2022 commented this line
            //    loadNotificationsCount(auditNoteID);  // JSL 06/27/2022 commented this line
        },
        global: false,
        error: function (err) {
            console.log(err);
        }
    });
};
//End RDBJ 10/21/2021

//RDBJ 11/13/2021
function AddPriority(ctr) {
    // JSL 12/21/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/21/2022

    var val = ctr.value;
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: RootUrl + "IAFList/UpdateIAFAuditNotePriority",
        data: { NotesUniqueID: auditNoteID, PriorityWeek: parseInt(val) },
        success: function (data) {
            $.notify("Priority Changed Successfully", "success");
        },
        error: function (data) {
            console.log(data);
        }
    });
}
//End RDBJ 11/13/2021

// RDBJ 12/21/2021
function GetAllUsersProfileList() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: RootUrl + "Deficiencies/GetAllUsersProfileList",
        success: function (data) {
            userList = data;
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End RDBJ 12/21/2021


//RDBJ 12/21/2021
function AddAssignToUser(ctr
    , blnIsRemoveAssignedPerson = false // JSL 07/02/2022
) {
    // JSL 12/21/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/21/2022

    var val = ctr.value;
    var userName = $("#ddUserList option:selected").html();
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: RootUrl + "Deficiencies/UpdateDeficiencyAssignToUser",
        data: {
            DeficienciesUniqueID: auditNoteID
            , AssignTo: val
            , blnIsIAF: "true"
            , blnIsNeedToDelete: blnIsRemoveAssignedPerson // JSL 07/02/2022
        },
        success: function (data) {
            // JSL 07/02/2022 wrapped in if
            if (blnIsRemoveAssignedPerson) {
                $.notify("Removed assigned person " + userName + " from deficiency successfully.", "success");
                $("#ddUserList").prop('selectedIndex', 0);
            }
            // End JSL 07/02/2022 wrapped in if
            else
                $.notify("Deficiency Assign To " + userName + " Successfully", "success");
        },
        error: function (data) {
            console.log(data);
        }
    });
}
//End RDBJ 12/21/2021

// RDBJ2 02/23/2022
function UpdateAuditNoteDetails() {
    var dic = {};

    dic["NotesUniqueID"] = auditNoteID;
    dic["CorrectiveActions"] = $("#txtCorrectiveAction").val();
    dic["PreventativeActions"] = $("#txtPreventiveAction").val();
    dic["Rank"] = $("#txtRank").val();
    dic["Name"] = $("#txtName").val();
    dic["TimeScale"] = $(".txtTimeScale").val();

    CommonServerPostApiCall(dic, "IAFList", "PerformAction", str_API_UPDATEAUDITNOTEDETAILS);
}
// End RDBJ2 02/23/2022

// JSL 12/03/2022
function BindLightGallery() {
    //lightGallery(document.getElementById('defFiles-gallery'), {   // JSL 12/10/2022 commented
    lightGallery(document.getElementById('defAttachment'), {    // JSL 12/10/2022
        selector: '.itemImage',
        plugins: [lgZoom, lgThumbnail, lgRotate, lgFullscreen],
        thumbnail: true,
    });
}
// End JSL 12/03/2022

// JSL 12/10/2022
function moveToAttachment(element) {
    $('div[id^="img_"]').removeClass('blink');
    var dataId = $(element).attr("data-id");
    let e = document.getElementById("img_" + dataId);

    e.classList.add("blink");
    e.scrollIntoView({
        block: 'start',
        behavior: 'smooth',
        inline: 'start'
    });
}
// End JSL 12/10/2022

// JSL 12/21/2022
function SetRestrictionForSomeUserToModify() {
    // JSL 07/04/2022
    if (vbUserGroup == "5"  // Set 5 for ISM Group Only
        || vbUserGroup == "7"  // Set 7 for Technical Group Only
        || vbUserGroup == "1"   // RDBJ 12/20/2021 Set 1 for Admin Group Only
    ) {
        $("#ddUserList").prop("disabled", false);
        $("#btnRemoveAssignedPerson").removeClass("hide");  // JSL 07/02/2022
    }
    else {
        $("#ddUserList").prop("disabled", true);
        $("#btnRemoveAssignedPerson").remove(); // JSL 07/02/2022
    }
    // JSL 07/04/2022

    // RDBJ2 02/23/2022
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
    });
    // End RDBJ2 02/23/2022

    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            $("#commentForm").remove();
        }
        else {
            // RDBJ2 02/23/2022
            $(".updateAuditNote").bind("change", function (e) {
                UpdateAuditNoteDetails();
            });

            $(".txtTimeScale").bind("change", function (e) {
                var val = $(this).val();
                $('.txtTimeScale').val(val);
                UpdateAuditNoteDetails();
            });
    // End RDBJ2 02/23/2022
        }
    }
}
// End JSL 12/21/2022