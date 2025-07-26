var deficienciesID = getUrlVars()["id"];/*new defunique Guid id*/
var notificationCommentsCounter; //RDBJ 10/26/2021
var notificationInitialActionsCounter; //RDBJ 10/26/2021
var notificationResolutionsCounter; //RDBJ 10/26/2021
var _ReportType;    // RDBJ2 05/10/2022
isInspector = isInspector.toLowerCase();    // JSL 07/09/2022
var fileData = new FormData();  // JSL 11/08/2022
var _strFilePath = '../Images/';    // JSL 12/03/2022
$(document).ready(function () {
    
    loadbyDetailtab(deficienciesID);
    loadComment(deficienciesID);

    var height = $(".content-wrapper").height();
    $(".content").height(height - 160);
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

    //RDBJ 10/26/2021
    notificationCommentsCounter = document.getElementById('notificationCommentsCounter');
    notificationInitialActionsCounter = document.getElementById('notificationInitialActionsCounter');
    notificationResolutionsCounter = document.getElementById('notificationResolutionsCounter');

    loadNotificationsCount(deficienciesID);
    autoRefresh();
    //End RDBJ 10/26/2021

    // JSL 07/09/2022
    if (!JSON.parse(isInspector)) {
        $("#DeficiencyPriority").attr("disabled", true);
        $("#chkIsClosed").attr("disabled", true);
        $("#initialActionForm").addClass("hide");   // JSL 07/18/2022
    }
    else {
        $("#DeficiencyPriority").attr("disabled", false);
        $("#chkIsClosed").attr("disabled", false);
        $("#initialActionForm").removeClass("hide");    // JSL 07/18/2022
    }
    // End JSL 07/09/2022

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
function RemoveFile(ctr, value) {   // JSL 11/08/2022 added value
    $(ctr).parent().next().next().next().val('false')
    $(ctr).parent().parent().find('.file').remove();
    $(ctr).parent().parent().find('.filename').remove();
    $(ctr).parent().parent().find('.path').remove();
    $(ctr).parent().parent().find('.employeeFile').val('');
    $(ctr).parent().hide();

    // JSL 11/08/2022
    fileData.delete(value);
    // End JSL 11/08/2022
}

// RDBJ2 05/10/2022
function RemoveDeficienciesFile(ctr) {
    var DeficienciesFileID = $(ctr).attr("data-id");

    var dic = {};

    dic["DeficienciesFileID"] = DeficienciesFileID;
    dic["DeficienciesUniqueID"] = deficienciesID;
    dic["FormType"] = _ReportType;

    CommonServerPostApiCall(dic, "Deficiencies", "PerformAction", str_API_DELETEDEFICIENCYFILE);
}
// End RDBJ2 05/10/2022

function fileUpload(ctr, count) {
    // JSL 01/08/2023
    var image_holder = $("#img-holder");
    var notAllowType = ""
    var fileistoobig = ""; //RDBJ 10/30/2021
    var fileNametoobig = ""; // JSL 01/08/2023

    for (var i = 0; i < ctr.files.length; i++) {
        if (ctr.files[i].type.indexOf('pdf') >= 0 ||
            ctr.files[i].type.indexOf('image') >= 0 ||
            ctr.files[i].type.indexOf('document') >= 0 ||
            ctr.files[i].type.indexOf('xml') >= 0 ||
            ctr.files[i].type.indexOf('sheet') >= 0) {
            //RDBJ 10/30/2021 validation for file size and wrapped in if
            if (ctr.files[i].size > 2000000) {
                fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] </br>";
            }
            // JSL 01/08/2023
            else if (ctr.files[i].name.length > 40) {
                fileNametoobig = fileNametoobig + " [" + ctr.files[i].name + "] </br>";
            }
            // End JSL 01/08/2023
            else {
                fileData.append(ctr.files[i].name, ctr.files[i]);   // JSL 11/08/2022

                var data = '<div class="individual-tag"> <div  class="chip pink lighten-2 white-text waves-effect waves-effect file" > ' +
                    '<a>' + ctr.files[i].name + '</a>' +
                    '<i class="close fa fa-times" onclick="RemoveFile(this, \'' + ctr.files[i].name + '\')"></i >' +   // JSL 11/08/2022 added ctr.files[i].name
                    '</div>' +
                    '<input type="hidden" value="' + ctr.files[i].name + '" class="filename" />' +
                    '<input type="hidden" class="isUpload" value="' + "true" + '" class="IsUpload" /> </div>';
                if (ctr.className == "resultionfile") {
                    $("#resoultion-list").append(data);
                }
                else if (ctr.className == "employeeFile") {
                    $("#comment-list").append(data);
                }
                else if (ctr.className == "Intifile") {
                    $("#intial-list").append(data);
                }
            }
        }
        else {
            notAllowType = notAllowType + " [" + ctr.files[i].name + "] </br>";
        }
    }

    var blnIsNeedTOShowErrorMessage = false;    // JSL 01/08/2023 

    if (notAllowType != "") {
        $("#modal-default p#fileTypeError").html("<strong>File types are not supported : </strong></br>" + notAllowType + "");
        //$('#modal-default').modal('show');  // JSL 01/08/2023 commented
        blnIsNeedTOShowErrorMessage = true; // JSL 01/08/2023
    }
    //RDBJ 10/30/2021 added else if
    if (fileistoobig != "") {
        $("#modal-default p#fileSizeError").html("<strong>File must be smaller than 2.0 MB : </strong></br>" + fileistoobig + "");
        //$('#modal-default').modal('show');  // JSL 01/08/2023 commented
        blnIsNeedTOShowErrorMessage = true; // JSL 01/08/2023
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
        var notAllowType = ""
        var fileistoobig = ""; //RDBJ 10/30/2021
        reader.onload = function (e) {
            for (var i = 0; i < ctr.files.length; i++) {
                if (ctr.files[i].type.indexOf('pdf') >= 0 ||
                    ctr.files[i].type.indexOf('image') >= 0 ||
                    ctr.files[i].type.indexOf('document') >= 0 ||
                    ctr.files[i].type.indexOf('xml') >= 0 ||
                    ctr.files[i].type.indexOf('sheet') >= 0) {
                    //RDBJ 10/30/2021 validation for file size and wrapped in if
                    if (ctr.files[i].size > 2000000) {
                        fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] ";
                    } else {
                        fileData.append(ctr.files[i].name, ctr.files[i]);   // JSL 11/08/2022

                        //var picFile = e.target; // JSL 11/08/2022 commented this line
                        var data = '<div class="individual-tag"> <div  class="chip pink lighten-2 white-text waves-effect waves-effect file" > ' +
                            '<a>' + ctr.files[i].name + '</a>' +
                            '<i class="close fa fa-times" onclick="RemoveFile(this, \'' + ctr.files[i].name + '\')"></i >' +   // JSL 11/08/2022 added ctr.files[i].name
                            '</div>' +
                            '<input type="hidden" value="' + ctr.files[i].name + '" class="filename" />' +
                            //'<input type="hidden" class="path" value="' + picFile.result + '"   class="path" />' +     // JSL 11/08/2022 commented this line
                            '<input type="hidden" class="isUpload" value="' + "true" + '" class="IsUpload" /> </div>';
                        if (ctr.className == "resultionfile") {
                            $("#resoultion-list").append(data);
                        }
                        else if (ctr.className == "employeeFile") {
                            $("#comment-list").append(data);
                        }
                        else if (ctr.className == "Intifile") {
                            $("#intial-list").append(data);
                        }
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
            //RDBJ 10/30/2021 added else if
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
function Download(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/Download?file=" + ctr + "&name=" + _name;;
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
function DownloadCommentFile(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/DownloadCommentFile?CommentFileID=" + ctr + "&name=" + _name;
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
function DownloadInitialActionFile(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/DownloadInitialActionFile?IntActFileID=" + ctr + "&name=" + _name; //RDBJ 09/18/2021 changed parameter name fileId
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
function DownloadResolutionFile(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/DownloadResolutionFile?ResolutionFileID=" + ctr + "&name=" + _name; //RDBJ 09/18/2021 changed parameter name fileId
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
function generateTemplate(ReportList) {
    var template = "";
    if (ReportList && ReportList.length > 0) {
        for (var i = 0; i < ReportList.length; i++) {
            // JSL 07/05/2022 wrapped in if
            if (JSON.parse(!IsNullEmptyOrUndefined(ReportList[i].IsResolution))) {
                template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                    '<a onclick="DownloadResolutionFile(\'' + ReportList[i].CommentFileUniqueID + '\',this)">' + ReportList[i].FileName + '</a>' +
                    '</div>';
            }
            // End JSL 07/05/2022 wrapped in if
            else {
                template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                    '<a onclick="DownloadCommentFile(\'' + ReportList[i].CommentFileUniqueID + '\', this)">' + ReportList[i].FileName + '</a>' +
                    '</div>';
            }
        }
    }
    return template;
}
function generateTemplateDeficiencies(ReportList) {
    // JSL 12/10/2022 uncommented
    // JSL 12/03/2022 commented
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div class="chip pink lighten-2 white-text waves-effect waves-effect file">' +   // RDBJ2 05/10/2022 removed style="line-height: unset;"
            //'<a class="scroll" href="#img_' + ReportList[i].GIRDeficienciesFileID + '">' + ReportList[i].FileName + '</a>';   // JSL 12/10/2022 commented
            '<a class="scroll" href="javascript:void(0);" data-id="' + ReportList[i].GIRDeficienciesFileID + '" onclick="moveToAttachment(this);">' + ReportList[i].FileName + '</a>';  // JSL 12/10/2022
        template = template + '<i data-toggle="tooltip" data-placement="bottom" data-original-title="Download" style="padding-left: 10px;" class="close fa fa-download" onclick="Download(' + ReportList[i].GIRDeficienciesFileID + ', this)"></i>'; // JSL 12/10/2022

        // JSL 05/12/2022 commented this due to restriction
        // RDBJ2 05/10/2022
        /*
        if (isInspector.toLowerCase() == 'true')
            template = template + '<i style="padding-left: 10px;" class="close fa fa-trash" data-id="' + ReportList[i].GIRDeficienciesFileID + '" onclick="RemoveDeficienciesFile(this)"></i>'; // RDBJ2 05/10/2022
        */
        // End RDBJ2 05/10/2022
        // End JSL 05/12/2022 commented this due to restriction
        template = template + '</div>';

        // JSL 12/10/2022
        var strFileDataId = ReportList[i].GIRDeficienciesFileID;
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

        var html = '<div id="img_' + strFileDataId + '" class="item">' +
            '<a class="' + strClassNameForGallery + '" data-src="' + strPathForView + '" href="' + strFilePathAndName + '">' +
            '<img class="imgClass img" src="' + strPathForView + '" alt="' + strFileName + '" />' +
            '</a>';
            '</div>';
        $("#defAttachment").append(html);
        // End JSL 12/10/2022
    }
    // End JSL 12/03/2022 commented
    // End JSL 12/10/2022 uncommented

    // JSL 12/10/2022 commented
    /*
    // JSL 12/03/2022
    var template = '<div class="container-fluid box" id="defFiles-gallery">';
    for (var i = 0; i < ReportList.length; i++) {
        var strFileDataId = ReportList[i].GIRDeficienciesFileID;
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
            '<a class="btn btn-success downloadFileBtn" onclick="Download(' + strFileDataId + ',this)" data-toggle="tooltip" data-placement="bottom" data-original-title="Download ' + strFileName + '" >' +
            '<i style="padding-right:5px" class="fa fa-download"></i>' + strFileName +
            '</a >';
        
        //if (isInspector.toLowerCase() == 'true') {
        //    html += '<button class="btn btn-danger" data-id="' + strFileDataId + '" data-toggle="tooltip" data-placement="bottom" data-original-title="Delete" onclick="RemoveDeficienciesFile(this)" style="margin-left: 5px;"><i class="fa fa-trash-o" aria-hidden="true"></i></button>';
        //}
        
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
function generateTemplateInitialAction(ReportList) {

    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="DownloadInitialActionFile(\'' + ReportList[i].IniActFileUniqueID + '\',this)">' + ReportList[i].FileName + '</a>' + //RDBJ 09/18/2021 Changed ReportList[i].GIRFileID
            '</div>';
    }

    return template;
}
function generateTemplateResolution(ReportList) {
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="DownloadResolutionFile(\'' + ReportList[i].ResolutionFileUniqueID + '\',this)">' + ReportList[i].FileName + '</a>' + //RDBJ 09/18/2021 Changed ReportList[i].GIRFileID
            '</div>';
    }

    return template;
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
function AddResolution(ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = RootUrl + "Deficiencies/AddDeficienciesResolution";
    var resolution = $(ctr).parent().parent().find('.resolution').val();
    var obj = {};
    obj.Name = name;
    obj.Resolution = resolution;
    //obj.DeficienciesID = deficienciesID;
    obj.DeficienciesUniqueID = deficienciesID;
    obj.UniqueFormID = $("#UniqueFormID").val();
    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {

        var GIRDeficienciesResolutionFiles = new Object();
        GIRDeficienciesResolutionFiles.GIRResolutionID = 0;
        GIRDeficienciesResolutionFiles.NoteID = 0;
        //GIRDeficienciesResolutionFiles.DeficienciesID = deficienciesID;
        GIRDeficienciesResolutionFiles.FileName = $(this).next().val();
        GIRDeficienciesResolutionFiles.StorePath = $(this).next().next().val();
        GIRDeficienciesResolutionFiles.IsUpload = $(this).next().next().next().val();
        GIRDeficienciesResolutionFiles.UniqueFormID = $("#UniqueFormID").val();
        filedata.push(GIRDeficienciesResolutionFiles);
    });
    obj.GIRDeficienciesResolutionFiles = filedata;

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        async: true,
        url: url,
        data: JSON.stringify(obj),
        success: function (Data) {

            $.notify("Resolution Added Successfully", "success");
            var url = RootUrl + "Deficiencies/GetDeficienciesResolution";
            loadResolution(deficienciesID, url)
            //var childGrid = $(ctr).parent().parent().parent().parent().parent().find('.resoultionList')
            $("#resolutionGrid").empty();
            $("#resolutionGrid").kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                pageable: {
                    pageSize: 5
                },
                dataSource: {
                    data: dataLoad.options.data

                },
                dataBound: function (e) {
                    $("#resolutionGrid .k-grid-content").css("min-height", "170px");
                    //RDBJ 10/28/2021
                    var rows = e.sender.tbody.children();
                    for (var j = 0; j < rows.length; j++) {
                        var row = $(rows[j]);
                        var isNew = $(row).find('td:eq(4)').text();
                        if (isNew == "1") {
                            row.css("background-color", "rgb(165, 229, 255)");
                            row.css("color", "black");
                        }
                    }
                    //End RDBJ 10/28/2021
                },
                columns: [
                    {
                        field: "Name",
                        title: "Name",
                        width: "150px"
                    },
                    {
                        field: "Resolution",
                        title: "Resolution"
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
                        template: "#=generateTemplateResolution(GIRDeficienciesResolutionFiles)#",
                        width: "130px"
                    },
                    //RDBJ 10/28/2021
                    {
                        field: "isNew",
                        title: "New",
                        hidden: true
                    }
                ]
            });

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
function tabChangeLoadGridData(type, element) {
    if (type == "Resolution") {
        loadbyResolutiontab();
    }
    else if (type == "InitialActions") {
        loadbyInitialActiontab();
    }
    //RDBJ 10/26/2021
    else if (type == "Comments") {
        loadComment(deficienciesID);
    }
    //RDBJ 10/26/2021
    else if (type == "Details") {
        loadbyDetailtab(deficienciesID);
    }
    
    //RDBJ 10/30/2021
    if (element.querySelector("span") != null)
        //RDBJ 10/26/2021
        if (element.querySelector("span").innerText != "")
            openAndSeenNotificationsUpdateStatus(type);
}
function loadComment(id) {
    var url = RootUrl + "Deficiencies/GetDeficienciesNote";
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: id },
        success: function (data) {
            var grid = $('#commentGrid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                noRecords: true,
                messages: {
                    noRecords: "No record found."
                },
                pageable: {
                    pageSize: 5
                },
                dataSource: {
                    data: data,
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

                        //RDBJ 10/26/2021
                        var isNew = $(row).find('td:eq(4)').text();
                        if (isNew == "1") {
                            row.css("background-color", "rgb(165, 229, 255)");
                            row.css("color", "black");
                        }
                        //End RDBJ 10/26/2021
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
                        field: 'GIRDeficienciesCommentFile',
                        title: 'Files',
                        template: "#=generateTemplate(GIRDeficienciesCommentFile)#",
                        attributes: { style: "white-space: normal !important; text- overflow: unset; !important" },  // JSL 11/08/2022
                        //width: "130px"
                    },
                    //RDBJ 10/26/2021
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
function loadDeficienciesInitialActions(id, url) {
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
function loadResolution(id, url) {
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
function loadFile(id, url) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: id },
        success: function (Data) {
            dataLoadFile = new kendo.data.DataSource({
                data: Data
            });

        },
        error: function (data) {
            console.log(data);
        }
    });
}
function AddNotes(ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = RootUrl + "Deficiencies/AddDeficienciesNote";
    var comment = $(ctr).parent().parent().find('.comment').val();

    // JSL 11/08/2022
    if (IsNullEmptyOrUndefined(comment)) {
        $.notify("Please add comment!", "error");
        return;
    }
    // End JSL 11/08/2022

    // JSL 11/08/2022 commented
    /*
    var obj = {};
    obj.UserName = name;
    obj.Comment = comment;
    //obj.DeficienciesID = deficienciesID;
    obj.DeficienciesUniqueID = deficienciesID;
    obj.UniqueFormID = $("#UniqueFormID").val();
    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {
        var GIRDeficienciesCommentFile = new Object();
        GIRDeficienciesCommentFile.GIRCommentFileID = 0;
        GIRDeficienciesCommentFile.NoteID = 0;
        //GIRDeficienciesCommentFile.DeficienciesID = deficienciesID;
        GIRDeficienciesCommentFile.DeficienciesUniqueID = deficienciesID;
        GIRDeficienciesCommentFile.FileName = $(this).next().val();
        GIRDeficienciesCommentFile.StorePath = $(this).next().next().val();
        GIRDeficienciesCommentFile.IsUpload = $(this).next().next().next().val();
        GIRDeficienciesCommentFile.UniqueFormID = $("#UniqueFormID").val();
        filedata.push(GIRDeficienciesCommentFile);
    });
    obj.GIRDeficienciesCommentFile = filedata;
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: JSON.stringify(obj),
        success: function (Data) {

            $.notify("Comment Added Successfully", "success");
            var url = RootUrl + "Deficiencies/GetDeficienciesNote";
            loadComment(deficienciesID, url)
            $(ctr).parent().parent().find('.comment').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();
            $(ctr).parent().parent().find('.employeeFile').val('');
        },
        error: function (Data) {
            console.log(Data);
        }
    });
    */
    // End JSL 11/08/2022 commented

    // JSL 11/08/2022
    // Adding one more key to FormData object  
    fileData.append("UserName", name);
    fileData.append("Comment", comment);
    fileData.append("DeficienciesUniqueID", deficienciesID);
    fileData.append("UniqueFormID", $("#UniqueFormID").val());

    $.ajax({
        type: 'POST',
        contentType: false,
        processData: false,
        async: false,
        url: url,
        data: fileData,
        success: function (Data) {
            $.notify("Comment Added Successfully", "success");
            var url = RootUrl + "Deficiencies/GetDeficienciesNote";
            loadComment(deficienciesID, url)
            $(ctr).parent().parent().find('.comment').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();
            $(ctr).parent().parent().find('.employeeFile').val('');

            fileData = new FormData();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
    // JSL 11/08/2022
}

function AddInitialDetails(ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = RootUrl + "Deficiencies/AddDeficienciesInitialActions";
    var description = $(ctr).parent().parent().find('.description').val();

    // JSL 11/08/2022
    if (IsNullEmptyOrUndefined(description)) {
        $.notify("Please add comment!", "error");
        return;
    }
    // End JSL 11/08/2022

    // JSL 11/08/2022 commented
    /*
    var obj = {};
    obj.Name = name;
    obj.Description = description;
    //obj.DeficienciesID = deficienciesID;
    obj.DeficienciesUniqueID = deficienciesID;
    obj.UniqueFormID = $("#UniqueFormID").val();
    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {
        var GIRDeficienciesInitialActionsFiles = new Object();
        GIRDeficienciesInitialActionsFiles.GIRInitialID = 0;
        GIRDeficienciesInitialActionsFiles.NoteID = 0;
        //GIRDeficienciesInitialActionsFiles.DeficienciesID = deficienciesID;
        GIRDeficienciesInitialActionsFiles.FileName = $(this).next().val();
        GIRDeficienciesInitialActionsFiles.StorePath = $(this).next().next().val();
        GIRDeficienciesInitialActionsFiles.IsUpload = $(this).next().next().next().val();
        GIRDeficienciesInitialActionsFiles.UniqueFormID = $("#UniqueFormID").val();
        filedata.push(GIRDeficienciesInitialActionsFiles);
    });
    obj.GIRDeficienciesInitialActionsFiles = filedata;

    */
    // End JSL 11/08/2022 commented

    // JSL 11/08/2022
    // Adding one more key to FormData object  
    fileData.append("Name", name);
    fileData.append("Description", description);
    fileData.append("DeficienciesUniqueID", deficienciesID);  
    fileData.append("UniqueFormID", $("#UniqueFormID").val());
    // End JSL 11/08/2022

    $.ajax({
        type: 'POST',
        //contentType: 'application/json',  // JSL 11/08/2022 commented
        //dataType: 'json', // JSL 11/08/2022 commented
        contentType: false, // JSL 11/08/2022
        processData: false, // JSL 11/08/2022
        async: true,
        url: url,
        //data: JSON.stringify(obj),  // JSL 11/08/2022 commented
        data: fileData,  // JSL 11/08/2022
        success: function (Data) {
            $.notify("Initial Details Added Successfully", "success");
            var url = RootUrl + "Deficiencies/GetDeficienciesInitialActions";
            loadDeficienciesInitialActions(deficienciesID, url);
            $("#initialActionGrid").kendoGrid({
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
                    $("#initialActionGrid .k-grid-content").css("min-height", "170px");
                    //RDBJ 10/28/2021
                    var rows = e.sender.tbody.children();
                    for (var j = 0; j < rows.length; j++) {
                        var row = $(rows[j]);
                        var isNew = $(row).find('td:eq(4)').text();
                        if (isNew == "1") {
                            row.css("background-color", "rgb(165, 229, 255)");
                            row.css("color", "black");
                        }
                    }
                    //End RDBJ 10/28/2021
                },
                columns: [
                    {
                        field: "Name",
                        title: "Name",
                        width: "150px"
                    },
                    {
                        field: "Description",
                        title: "Description"
                    },
                    {
                        field: "CreatedDate",
                        title: "Date Time",
                        template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy h:mm'):'' #",
                        width: "130px"
                    },
                    {
                        field: 'GIRDeficienciesInitialActionsFiles',
                        title: 'Files',
                        template: "#=generateTemplateInitialAction(GIRDeficienciesInitialActionsFiles)#",
                        attributes: { style: "white-space: normal !important; text- overflow: unset; !important" },  // JSL 11/08/2022
                        //width: "130px"    // JSL 11/08/2022 commented this line
                    },
                    //RDBJ 10/28/2021
                    {
                        field: "isNew",
                        title: "New",
                        hidden: true
                    }
                ]
            });

            $(ctr).parent().parent().find('.description').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();
            $(ctr).parent().parent().find('.Intifile').val('');

            fileData = new FormData();  // JSL 11/08/2022
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}

function loadbyInitialActiontab() {
    var url = RootUrl + "Deficiencies/GetDeficienciesInitialActions";
    loadDeficienciesInitialActions(deficienciesID, url);
    //var childGrid = $(ctr).parent().parent().parent().parent().find('.initialDetailsList');
    $("#initialActionGrid").kendoGrid({
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
        pageable: {
            pageSize: 5
        },
        dataBound: function (e) {
            $("#initialActionGrid .k-grid-content").css("min-height", "170px");

            //RDBJ 10/26/2021
            var rows = e.sender.tbody.children();
            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);
                var isNew = $(row).find('td:eq(4)').text();
                if (isNew == "1") {
                    row.css("background-color", "rgb(165, 229, 255)");
                    row.css("color", "black");
                }
            }
            //End RDBJ 10/26/2021

            BindToolTipsForGridText("initialActionGrid", "Description", 120);   // RDBJ 02/11/2022
        },
        columns: [
            {
                field: "Name",
                title: "Name",
                width: "150px"
            },
            {
                field: "Description",
                title: "Description",
                attributes: { class: "tooltipText" },   // RDBJ 02/11/2022
            },
            {
                field: "CreatedDate",
                title: "Date Time",
                template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy h:mm'):'' #",
                width: "130px"
            },
            {
                field: 'GIRDeficienciesInitialActionsFiles',
                title: 'Files',
                template: "#=generateTemplateInitialAction(GIRDeficienciesInitialActionsFiles)#",
                attributes: { style: "white-space: normal !important; text- overflow: unset; !important" },  // JSL 11/08/2022
                //width: "130px"    // JSL 11/08/2022 commented this line
            },
            //RDBJ 10/26/2021
            {
                field: "isNew",
                title: "New",
                hidden: true
            }
        ]
    });
}
function loadbyResolutiontab() {
    var url = RootUrl + "Deficiencies/GetDeficienciesResolution";
    loadResolution(deficienciesID, url);
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
        dataBound: function(e) {
            $("#resolutionGrid .k-grid-content").css("min-height", "170px");

            //RDBJ 10/26/2021
            var rows = e.sender.tbody.children();
            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);
                var isNew = $(row).find('td:eq(4)').text();
                if (isNew == "1") {
                    row.css("background-color", "rgb(165, 229, 255)");
                    row.css("color", "black");
                }
            }
            //End RDBJ 10/26/2021

            BindToolTipsForGridText("resolutionGrid", "Resolution", 120);   // RDBJ 02/11/2022
        },
        columns: [
            {
                field: "Name",
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
                template: "#=generateTemplateResolution(GIRDeficienciesResolutionFiles)#",
                width: "130px"
            }
            ,
            //RDBJ 10/26/2021
            {
                field: "isNew",
                title: "New",
                hidden: true
            }
        ]
    });
}
//function loadbyDetailtab(id, ctr) {
//    var url = RootUrl + "Deficiencies/GetDeficienciesResolution";
//    loadDeficienciesInitialActions(id, url);
//    var childGrid = $(ctr).parent().parent().parent().parent().find('.resoultionList');
//    $(childGrid).kendoGrid({
//        scrollable: true,
//        sortable: true,
//        resizable: true,
//        filterable: true,
//        dataSource: {
//            data: dataLoad.options.data
//        },
//        columns: [
//            {
//                field: "Name",
//                title: "Name",
//                width: "150px"
//            },
//            {
//                field: "Resolution",
//                title: "Resolution"
//            },
//            {
//                field: "CreatedDate",
//                title: "Date Time",
//                template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy h:mm'):'' #",
//                width: "130px"
//            },
//            {
//                field: 'GIRDeficienciesResolutionFiles',
//                title: 'Files',
//                template: "#=generateTemplateResolution(GIRDeficienciesResolutionFiles)#",
//                width: "130px"
//            }
//        ]
//    });
//}


function loadbyDetailtab(id) {
    var url = RootUrl + "Deficiencies/GetDeficienciesByDeficienciesID";
    //loadDeficienciesInitialActions(id, url);
    $.ajax({
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: { id : id},
        success: function (response) {
            var data = response.details; //RDBJ 10/26/2021
            var isInspector = response.isInspector; //RDBJ 10/26/2021
            console.log(data);
            _ReportType = data.ReportType;  // RDBJ2 05/10/2022

            $("#UniqueFormID").val(data.UniqueFormID);
            var def = "", section = "", date = "",nowDate="",closedDate = "";
            if (data.Deficiency != null) {
                def = data.Deficiency;
            }
            if (data.Section != null) {
                section = data.Section;
            }
            if (data.DateRaised != null) {
                var dateString = data.DateRaised.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = ('0' + (currentTime.getMonth() + 1)).slice(-2);
                var day = ('0' + (currentTime.getDate())).slice(-2); //RDBJ 10/12/2021 Removed + 1
                var year = currentTime.getFullYear();
                var result = year + "-" + month + "-" + day; //+ " " + currentTime.getHours() +":"+ currentTime.getMinutes(); // RDBJ 11/29/2021 Commented this
                date = result;
            }
            if (data.DateClosed != null) {
                var dateString = data.DateClosed.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = ('0' + (currentTime.getMonth() + 1)).slice(-2);
                var day = ('0' + (currentTime.getDate())).slice(-2); //RDBJ 10/12/2021 Removed + 1
                var year = currentTime.getFullYear();
                var result = year + "-" + month + "-" + day; //+ " " + currentTime.getHours() + ":" + currentTime.getMinutes(); // RDBJ 11/29/2021 Commented this
                closedDate = result;
            }
            var detailData = '<div class="details">' +
                '<table class="table table-bordered">' +
                '<tbody>' +
                '<tr>' +
                '<td style="width: 180px;">Section Number</td>' +
                '<td>' + section + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 180px;">Deficiency</td>' + // RDBJ 12/21/2021 Change Deficiency from Comment
                '<td>' + def + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 180px;">Date Raised</td>' +
                '<td>' + date + '</td>' +
                '</tr >' +
                // JSL 07/09/2022
                '<tr>' +
                //'<td style="width: 250px;">Priority</td>' +   // JSL 07/11/202 commented
                // JSL 07/11/2022
                '<td style="width: 285px; vertical-align: bottom;">Priority : ' +
                '<table class="table table-borderless" style="width: 100% !important; margin-bottom: 0 !important;">' +
                '<tbody>' +
                '<tr>' +
                '<td class="col-md-2" style="width: 100px; vertical-align: middle; border-right: 1px solid #f4f4f4; padding: 0; border-top: none;"> Due Date :</td>' +
                '<td style="width: 200px; vertical-align: middle; border-top: none;">' +
                '<div class="has-feedback" style="width: 180px;">' +
                '<input type="text" class="form-control datepicker txtDueDate" onchange="AddPriority()" autocomplete="off" readonly/>' +
                '<span class="form-control-feedback arrivalCalendarAddon">' +
                '<i class="fa fa-calendar"></i>' +
                '</span>' +
                '</div>' +
                '</td>' +
                '</tr>' +
                '</tbody>' +
                '</table>' +
                '</td>' +
                // End JSL 07/11/2022
                '<td style="vertical-align: bottom;">' +
                '<table class="table table-borderless" style="width: 100% !important; margin-bottom: 0 !important;">' +
                '<tbody>' +
                '<tr>' +
                '<td class="col-md-2" style="width: 170px; vertical-align: middle; border-right: 1px solid #f4f4f4; padding: 0; border-top: none;">Select number of Weeks :</td>' +
                '<td style="border-top: none;">' +
                '<select class="DeficiencyPriority form-control" id="DeficiencyPriority" onchange="AddPriority()" style="width:auto;">' +
                '<option value="12">12</option>' +
                '<option value="8">8</option>' +
                '<option value="4">4</option>' +
                '</select>' +
                '</td>' +
                '</tr>' +
                '</tbody>' +
                '</table>' +
                '</td>' +
                '</tr>' +
                // End JSL 07/09/2022
                '<tr>' +
                '<td style="width: 180px;">Date Closed</td>' +
                '<td>' + closedDate + '</td>' +
                '</tr >' +
                '<tr>' +
                '<td style="width: 180px;">Files</td>' +
                '<td style="width: 155vh;">' + generateTemplateDeficiencies(data.GIRDeficienciesFile) + '</td>' + // JSL 12/03/2022 added style
                '</tr >';
            //RDBJ 10/26/2021 wrapped in if for inspector
            if (isInspector == true)
            {
                detailData += '<tr>' +
                    '<td style="width: 250px;";>Is closed</td>' +
                    '<td><input onclick = "updateDefiStatus(this)"  type = "checkbox" class="chkbx" id="chkIsClosed" /></td>' +
                    '</tr >';
            }
            //RDBJ 11/9/2021 added else
            else {
                detailData += '<tr>' +
                    '<td style="width: 250px;";>Is closed</td>' +
                    '<td><input onclick="return false;" type = "checkbox" class="chkbx" id="chkIsClosed" /></td>' +
                    '</tr >';
            }
            detailData += '</tbody></table></div>';

            if (data.ReportType == "GI") {
                var headerData = '<div class="header-inner-common">' +
                    '<ul class="headerdetails">' +
                    //'<li><span><b>Ship Name</b></span><span>' + data.Ship + '</span></li>' +    // JSL 06/13/2022 commented this line
                    '<li><span><b>Ship Name</b></span><span>' + data.ShipName + '</span></li>' +    // JSL 06/13/2022 
                    '<li><span><b>Type</b></span><span>General Inspection Report</span></li>' +
                    '<li><span><b>Deficiencies Number</b></span><span>' + data.No + '</span></li>' +
                    '<li><span><b>Section Number</b></span><span>' + section + '</span></li>' +
                    '</ul>' +
                    //'<a class="back-btn" href="' + RootUrl + 'Deficiencies/Index" id = "lnkBackToDoc"> Back</a>' +  // JSL 07/18/2022 commented this line
                    '<a class="back-btn" href="return false;" onclick="closeThisWindow();"> Back</a>' +  // JSL 07/18/2022
                    '</div>';
            }
            else {
                var headerData = '<div class="header-inner-common">' +
                    '<ul class="headerdetails">' +
                    //'<li><span><b>Ship Name</b></span><span>' + data.Ship + '</span></li>' +    // JSL 06/13/2022 commented this line
                    '<li><span><b>Ship Name</b></span><span>' + data.ShipName + '</span></li>' +    // JSL 06/13/2022 
                    '<li><span><b>Type</b></span><span>SuperIntended Inspection Report</span></li>' +
                    '<li><span><b>Actionable Item</b></span><span>' + data.No + '</span></li>' +
                    '<li><span><b>Section Number</b></span><span>' + section + '</span></li>' +
                    '</ul>' +
                    //'<a class="back-btn" href="' + RootUrl + 'Deficiencies/Index" id = "lnkBackToDoc"> Back</a>' +  // JSL 07/18/2022 commented this line
                    '<a class="back-btn" href="return false;" onclick="closeThisWindow();"> Back</a>' +  // JSL 07/18/2022
                    '</div>';
            }
            $("#detailGrid").html(''); //RDBJ 10/26/2021
            $("#detailGrid").append(detailData);
            $("#divHeaderDetail").html(''); //RDBJ 10/26/2021
            $("#divHeaderDetail").append(headerData);
            $("#chkIsClosed").prop("checked", data.IsClose);

            //RDBJ 10/30/2021
            if (data.Priority != 0)
                $("#DeficiencyPriority").val(data.Priority);
            else
                $("#DeficiencyPriority").val(12);

            //$.notify("Comment Added Successfully", "success");

            // RDBJ 02/28/2022
            if (data.DueDate != null) {
                var dateString = data.DueDate.substr(6);
                var currentTime = new Date(parseInt(dateString));
                var month = ('0' + (currentTime.getMonth() + 1)).slice(-2);
                var day = ('0' + (currentTime.getDate())).slice(-2); //RDBJ 10/12/2021 Removed + 1
                var year = currentTime.getFullYear();
                var result = day + "/" + month + "/" + year; // + " " + currentTime.getHours() + ":" + currentTime.getMinutes(); // RDBJ 11/29/2021 Commented this
                $('.txtDueDate').val(result);
            }
            // End RDBJ 02/28/2022
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}

//RDBJ 10/26/2021
function autoRefresh() {
    var intervalId = window.setInterval(function () {
        loadNotificationsCount(deficienciesID);
    }, 5000);
};
//End RDBJ 10/26/2021

//RDBJ 10/26/2021
function loadNotificationsCount(defId) {
    var url = RootUrl + 'Notifications/GetCountOfNotificationsById';
    $.ajax({
        contentType: 'application/json',
        async: false,
        dataType: 'json',
        url: url,
        type: 'GET',
        data: { id: defId, formType: "GISI" }, //RDBJ 10/26/2021 Added formType
        success: function (res) {
            setNotificationsCountForCommenteInitialActionsResolutions(res);
        },
        global: false,
        error: function (err) {
            console.log(err);
        }
    });
};
//End RDBJ 10/26/2021

//RDBJ 10/26/2021
function setNotificationsCountForCommenteInitialActionsResolutions(data) {
    //this is for the Comments Tab
    if (data.totalNewComments == 0
        && (IsNullEmptyOrUndefined(data.totalNewResolutions) || data.totalNewResolutions == 0)    // JSL 07/09/2022 added
    ) {
        notificationCommentsCounter.classList.remove("notification");
        notificationCommentsCounter.innerHTML = "";
    }
    else {
        // JSL 07/09/2022
        var numberOfCount = data.totalNewComments;
        if (!IsNullEmptyOrUndefined(data.totalNewResolutions)) {
            numberOfCount = parseInt(numberOfCount) + parseInt(data.totalNewResolutions);
        }
        // End JSL 07/09/2022

        notificationCommentsCounter.classList.add("notification");
        notificationCommentsCounter.innerHTML = numberOfCount;
    }

    //this is for the Initial Actions tab
    if (data.totalNewInitialActions == 0) {
        notificationInitialActionsCounter.classList.remove("notification");
        notificationInitialActionsCounter.innerHTML = "";
    }
    else {
        notificationInitialActionsCounter.classList.add("notification");
        notificationInitialActionsCounter.innerHTML = data.totalNewInitialActions;
    }

    // JSL 07/09/2022 commented
    /*
    //RDBJ 10/26/2021 this is for the Resolution tab
    if (data.totalNewResolutions == 0) {
        notificationResolutionsCounter.classList.remove("notification");
        notificationResolutionsCounter.innerHTML = "";
    }
    else {
        notificationResolutionsCounter.classList.add("notification");
        notificationResolutionsCounter.innerHTML = data.totalNewResolutions;
    }
    */
    // End JSL 07/09/2022 commented
};
//End RDBJ 10/26/2021

//RDBJ 10/26/2021
function openAndSeenNotificationsUpdateStatus(section) {
    var url = RootUrl + 'Notifications/openAndSeenNotificationsUpdateStatusById';
    $.ajax({
        contentType: 'application/json',
        async: false,
        dataType: 'json',
        url: url,
        type: 'GET',
        data: { id: deficienciesID, section: section, formType: "GISI" }, //RDBJ 10/21/2021 Added formType
        success: function (res) {
            if (res == "true")
                loadNotificationsCount(deficienciesID);
        },
        global: false,
        error: function (err) {
            console.log(err);
        }
    });
};
//End RDBJ 10/26/2021

//RDBJ 10/26/2021
function updateDefiStatus(ctr) {
    var did = deficienciesID;
    var url = RootUrl + "Deficiencies/UpdateDeficienciesData";
    var obj = {};
    obj.id = did;
    obj.isClose = $(ctr).prop('checked');
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Updated Successfully", "success");
            loadbyDetailtab(deficienciesID);
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
//End RDBJ 10/26/2021

//RDBJ 10/30/2021
function AddPriority(ctr) {
    // JSL 07/09/2022
    if (!JSON.parse(isInspector)) {
        return;
    }
    // End JSL 07/09/2022
    var val = ctr.value;
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: RootUrl + "Deficiencies/UpdateDeficiencyPriority",
        data: { DeficienciesUniqueID: deficienciesID, PriorityWeek: parseInt(val) },
        success: function (data) {
            $.notify("Priority Changed Successfully", "success");
        },
        error: function (data) {
            console.log(data);
        }
    });
}
//End RDBJ 10/30/2021

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