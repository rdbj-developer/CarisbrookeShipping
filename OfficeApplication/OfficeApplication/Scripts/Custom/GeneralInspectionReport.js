var dataLoad;
var dataLoadFile;
var ShipReports;
var shipCode;
var width;
var _shipFromURL = getUrlVars()["ship"];  // JSL 05/25/2022
var _formTypeFromURL = getUrlVars()["formtype"];  // JSL 05/25/2022

$(document).ready(function () {
    $("#__ig_wm__").css("display", "none !important"); //RDBJ 04/01/2022 avoid trial version
    try {
        if (screen.width <= 1280) {
            width = "250px";//"100px";
        }
        else {
            width = "450px";//"250px";
        }
    } catch (e) {
        width = "250px";//"100px";
    }

    // JSL 05/25/2022 wrapped in if
    if (IsNullEmptyOrUndefined(_shipFromURL)) {
        shipCode = $("#ddlGIReportShipName option:selected").val(); // RDBJ 03/01/2022 set option:selected
        loadGrid(shipCode, "GI");
        loadDetailGrid("GI");
        $('#GIGrid tbody tr:first-child').trigger('click');

        EditIAFButton("GI"); //RDBJ 11/22/2021
    }
    // Added else
    else {
        $("#ddlGIReportShipName").val(_shipFromURL);
        shipCode = $("#ddlGIReportShipName option:selected").val();
        var tabId = document.querySelector('[id^="' + _formTypeFromURL + '-content-below-tab"]').id;
        $('#' + tabId)[0].click();
    }
    // End JSL 05/25/2022 wrapped in if
    

    //RDBJ 11/22/2021
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
    });

    // RDBJ 01/06/2022
    $('.btnDeleteGISIDrafts').click(function (e) {
        e.stopPropagation();
    });
    // End RDBJ 01/06/2022

    ShowHideDivAddEditAuditSection();   // JSL 12/19/2022

    // JSL 02/18/2023
    $("input, select, option, textarea", "#fstoAddEditDetails").prop('disabled', true);    
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
    // End JSL 02/18/2023
});

function loadDetailGridByShip(ship) {
    var type = "";
    var activeTab = $("ul#custom-content-below-tab li.active a")[0].id
    if (activeTab.includes("gi")) {
        type = "GI";
    }
    //RDBJ 11/22/2021
    else if (activeTab.includes("si")) {
        type = "SI";
    }
    //RDBJ 11/22/2021
    else if (activeTab.includes("ia")) {
        type = "IA";
    }
    else if (activeTab.includes("fsto")) {
        type = "fsto";
    }

    // RDBJ 02/11/2022 wrapped in if to avoid multiple call
    if (type.toUpperCase() == "GI"
        || type.toUpperCase() == "SI"
    ) {
        loadGrid(ship.value, type);
        $('#' + type + 'DetailsGrid').empty();
        loadDetailGrid(type);
        $('#' + type + 'Grid tbody tr:first-child').trigger('click');
    }
    else if (type.toUpperCase() == "IA") {
        InitShipAudits(ship.value); // RDBJ 03/01/2022 pass ship selected value
        $('#DetailsGrid').empty();
        LoadAuditDetail();
        $('#Grid tbody tr:first-child').trigger('click');
    }
    else if (type.toUpperCase() == "FSTO") {
        $("#fstoAddEditDetails").addClass("hide");   // JSL 02/18/2023
        InitShipFSTOAudits(ship.value);
        $('#fstoDetailsGrid').empty();
        //LoadFSTOAuditDetail();
        GetFSTOItemsForEdit();  // JSL 02/17/2023
        $('#fstoGrid tbody tr:first-child').trigger('click');
    }
    
    // RDBJ 02/11/2022 commented 
    /*
    InitShipAudits(ship.value);
    $('#DetailsGrid').empty();
    LoadAuditDetail();
    $('#Grid tbody tr:first-child').trigger('click');
    */
    // End RDBJ 02/11/2022 commented 
    ShowHideDivAddEditAuditSection();   // JSL 01/05/2023
    EditIAFButton(type); //RDBJ 11/22/2021
    EditFSTOButton(type);   // JSL 02/14/2023
}

function tabChangeLoadGridData(type) {
    shipCode = $("#ddlGIReportShipName option:selected").val(); // RDBJ 03/01/2022 set option:selected

    // RDBJ 02/11/2022 wrapped in if to avoid multiple call
    if (type.toUpperCase() == "GI"
        || type.toUpperCase() == "SI"   // JSL 12/31/2022 
    ) {
        loadGrid(shipCode, type);
        $('#' + type + 'DetailsGrid').empty();
        loadDetailGrid(type);
        $('#' + type + 'Grid tbody tr:first-child').trigger('click');
    }
    else if (type.toUpperCase() == "IA"){   // JSL 12/31/2022
        InitShipAudits(shipCode);
        $('#DetailsGrid').empty();
        LoadAuditDetail();
        $('#Grid tbody tr:first-child').trigger('click');
    }
    // JSL 12/31/2022
    else if (type.toUpperCase() == "FSTO") {
        $("#fstoAddEditDetails").addClass("hide");   // JSL 02/18/2023
        InitShipFSTOAudits(shipCode);
        $('#fstoDetailsGrid').empty();
        //LoadFSTOAuditDetail();
        GetFSTOItemsForEdit();  // JSL 02/17/2023
        $('#fstoGrid tbody tr:first-child').trigger('click');
    }
    // End JSL 12/31/2022
    ShowHideDivAddEditAuditSection();   // JSL 01/05/2023
    EditIAFButton(type); //RDBJ 11/22/2021
    EditFSTOButton(type);   // JSL 02/14/2023
}

function RemoveFile(ctr) {
    $(ctr).parent().next().next().next().val('false')
    $(ctr).parent().parent().find('.file').remove();
    $(ctr).parent().parent().find('.filename').remove();
    $(ctr).parent().parent().find('.path').remove();
    $(ctr).parent().parent().find('.employeeFile').val('');
    $(ctr).parent().hide();
}
function fileUpload(ctr, count) {
    if (typeof (FileReader) != "undefined") {
        var image_holder = $("#img-holder");
        var reader = new FileReader();
        var notAllowType = ""
        reader.onload = function (e) {
            for (var i = 0; i < ctr.files.length; i++) {
                if (ctr.files[i].type.indexOf('pdf') >= 0 ||
                    ctr.files[i].type.indexOf('image') >= 0 ||
                    ctr.files[i].type.indexOf('document') >= 0 ||
                    ctr.files[i].type.indexOf('xml') >= 0 ||
                    ctr.files[i].type.indexOf('sheet') >= 0) {
                    var picFile = e.target;
                    var data = '<div  class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                        '<a>' + ctr.files[i].name + '</a>' +
                        '<i class="close fa fa-times" onclick="RemoveFile(this)"></i >' +
                        '</div>' +
                        '<input type="hidden" value="' + ctr.files[i].name + '" class="filename" />' +
                        '<input type="hidden" class="path" value="' + picFile.result + '"   class="path" />' +
                        '<input type="hidden" class="isUpload" value="' + "true" + '" class="IsUpload" />';
                    $(ctr).parent().append(data)
                }
                else {
                    notAllowType = notAllowType + " [" + ctr.files[i].name + "] ";
                }
            }
            if (notAllowType != "") {
                $("#modal-default p").text(notAllowType + " files types are not supported")
                $('#modal-default').modal('show');
            }
        }
        image_holder.show();
        reader.readAsDataURL($(ctr)[0].files[0]);
    } else {
        alert("This browser does not support FileReader.");
    }
}

function Download(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Forms/Download?file=" + ctr + "&name=" + _name;;
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
    var url = RootUrl + "Deficiencies/DownloadCommentFile?file=" + ctr + "&name=" + _name;;
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
    var url = RootUrl + "Deficiencies/DownloadInitialActionFile?fileId=" + ctr + "&name=" + _name;;
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
    var url = RootUrl + "Deficiencies/DownloadResolutionFile?fileId=" + ctr + "&name=" + _name;;
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
            template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                '<a onclick="DownloadCommentFile(' + ReportList[i].GIRCommentFileID + ',this)">' + ReportList[i].FileName + '</a>' +
                '</div>';
        }
    }
    return template;
}
function generateTemplateDeficiencies(ReportList) {
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="Download(' + ReportList[i].GIRDeficienciesFileID + ',this)">' + ReportList[i].FileName + '</a>' +
            '</div>';
    }

    return template;
}
function generateTemplateInitialAction(ReportList) {

    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="DownloadInitialActionFile(' + ReportList[i].GIRFileID + ',this)">' + ReportList[i].FileName + '</a>' +
            '</div>';
    }

    return template;
}
function generateTemplateResolution(ReportList) {
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="DownloadResolutionFile(' + ReportList[i].GIRFileID + ',this)">' + ReportList[i].FileName + '</a>' +
            '</div>';
    }

    return template;
}

// RDBJ 01/05/2022
function generateTemplateGISIDeleteButton(GISIFormID, type) {
    var template = '<div style="padding:1px;line-height: unset;" class="white-text waves-effect waves-effect file">' +
        '<a class="btn btn-danger btnDeleteGISIDrafts" onclick="DeleteGISIDrafts(\'' + GISIFormID + '\', \'' + type + '\', this)" style="padding: 0px 16px !important"> Delete </a>' +
        '</div>';
    return template;
}
// ENd RDBJ 01/05/2022

function detailInit(e) {
    var detailRow = e.detailRow;
    var id = e.data.DeficienciesID;
    detailRow.find(".tabstrip").kendoTabStrip({
        animation: {
            open: { effects: "fadeIn" }
        }
    });
    var url = RootUrl + "Forms/GetDeficienciesNote";
    var urlFile = RootUrl + "Forms/GetDeficienciesFiles";
    loadComment(id, url);
    loadFile(id, urlFile);
    detailRow.find(".orders").kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        dataSource: {
            data: dataLoad.options.data
        },
        resizable: true,
        columns: [
            {
                field: "UserName",
                title: "Name",
                width: "150px"
            },
            {
                field: "Comment",
                title: "Comment"
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
                width: "130px"
            }
        ]
    });
}

function updateDefiStatus(ctr) {
    // JSL 12/19/2022
    if (_CurrentUserDetailsObject.UserGroup == '8') {
        //$.notify("You are not autorized person.", "error");
        return;
    }
    // End JSL 12/19/2022

    var did = $(ctr).parent().parent().find('.DeficienciesUniqueID').text();
    var url = RootUrl + "Forms/UpdateDeficienciesData";
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
            var shipCode = $("#ddlGIReportShipName").val();
            var type = "";
            var activeTab = $("ul#custom-content-below-tab li.active a")[0].id
            if (activeTab.includes("gi")) {
                type = "GI";
            } else {
                type = "SI";
            }

            loadGrid(shipCode, type);
            loadDetailGrid(type);
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
function loadGrid(ship, reptype) {
    var url = RootUrl + "Deficiencies/GetShipGISIReports";
    //locadShipReports(ship, url, reptype);

    // RDBJ 01/05/2022
    var blnNeedShowDelete = false;
    if (UserGroupID == "1") {
        blnNeedShowDelete = true;
    }
    // End RDBJ 01/05/2022

    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: { code: ship, type: reptype },
        success: function (Data) {
            data = Data;
            $('#' + reptype + 'Grid').empty();
            var grid = $('#' + reptype + 'Grid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                selectable: true,
                noRecords: true,
                messages: {
                    noRecords: "No record found."
                },
                pageable: {
                    pageSize: 10
                },
                dataSource: {
                    data: data,
                    //sort: { field: "Date", dir: "desc" }
                },
                dataBound: function (e) {
                    $('#' + reptype + 'Grid .k-grid-content').css("min-height", "220px");
                    if (reptype == "GI") {
                        $('#' + reptype + 'Grid thead [data-field=Deficiencies] .k-link').html("No. of Def.");
                        $('#' + reptype + 'Grid thead [data-field=Auditor] .k-link').html("Auditor");
                    } else {
                        $('#' + reptype + 'Grid thead [data-field=Deficiencies] .k-link').html("Act. Items");
                        $('#' + reptype + 'Grid thead [data-field=Auditor] .k-link').html("Superintended");
                    }
                    
                    var rows = e.sender.tbody.children();
                    var dataItem = e.sender.dataItem(row);
                    for (var j = 0; j < rows.length; j++) {
                        var row = $(rows[j]);
                        var isExpired = $(row).find('td:eq(8)').text();
                        if (isExpired == "true") {
                            row.css("color", "#da3b3b");
                        }
                    }
                    for (var i = 0; i < this.columns.length; i++) {
                        this.autoFitColumn(i);
                    }
                    setTimeout(function () {
                        if (reptype != 'IA') {
                            var gridData = $('#' + reptype + 'Grid').data("kendoGrid");
                            gridData.autoFitColumn("Date");
                            gridData.autoFitColumn("Location");
                            gridData.autoFitColumn("Auditor");
                            gridData.autoFitColumn("Deficiencies");
                            gridData.autoFitColumn("OpenDeficiencies");
                        }
                    }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5
                    $('#' + reptype + 'Grid').on("mousedown", "tr[role='row']", function (e) {
                        if (e.which === 3) {
                            $("tr").removeClass("k-state-selected");
                            $(this).toggleClass("k-state-selected");
                            //$(this).trigger('click');
                        }
                    });
                },
                columns: [
                    {
                        field: "UniqueFormID",
                        title: "FormID",
                        hidden: true,
                    },
                    {
                        field: "Ship",
                        title: "Ship",
                        hidden: true,
                    },
                    {
                        field: "Type",
                        title: "Type",
                        hidden: true,
                    },
                    {
                        field: "Date",
                        title: "Date",
                        width: "10%",
                        template: "#= Date!=null? kendo.toString(kendo.parseDate(Date, 'yyyy-MM-dd'), 'dd-MMM-yyyy'):'' #", // RDBJ 04/01/2022
                        attributes: { class: "mainContext" },
                    },
                    {
                        field: "Location",
                        title: "Location",
                        width: "40%",
                        attributes: { class: "mainContext" },
                    },
                    {
                        field: "Auditor",
                        title: "Auditor",
                        width: "40%",
                        attributes: { class: "mainContext" },
                    },
                    {
                        field: "Deficiencies",
                        title: "No. of Def.",
                        width: "15%",
                        attributes: { class: "mainContext" },
                    },
                    {
                        field: "OpenDeficiencies",
                        title: "Outstanding",
                        width: "15%",
                        attributes: { class: "mainContext" },
                    },
                    {
                        field: "isExpired",
                        title: "is Expired",
                        hidden: true,
                    },
                    {
                        field: "FormID",
                        title: "GIRFormID",
                        hidden: true,
                    },
                    // RDBJ 01/05/2022
                    {
                        field: "UniqueFormID",
                        title: "Delete",
                        filterable: false,
                        template: "#=generateTemplateGISIDeleteButton(UniqueFormID, '" + reptype + "')#",
                        //width: "65px",
                        width: "15%",
                        attributes: { class: "mainContext" },
                        hidden: !blnNeedShowDelete,
                    },
                    // End RDBJ 01/05/2022
                ]
            });
            setTimeout(function () {
                if (reptype != 'IA') {
                    var gridData = $('#' + reptype + 'Grid').data("kendoGrid");
                    gridData.autoFitColumn("Date");
                    gridData.autoFitColumn("Location");
                    gridData.autoFitColumn("Auditor");
                    gridData.autoFitColumn("Deficiencies");
                    gridData.autoFitColumn("OpenDeficiencies");

                    // RDBJ 01/17/2022 added for loop
                    for (var i = 0; i < gridData.columns.length; i++) {
                        gridData.autoFitColumn(i);
                    }
                }
            }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5
            BindOpenFormContextMenuByGridId();    // JSL 02/14/2023 wrapped code in function
        },
        error: function (data) {
            console.log(data);
        }
    });
}

// JSL 02/14/2023
function BindOpenFormContextMenuByGridId(strIdWithHash = "") {
    // JSL 02/14/2023
    if (IsNullEmptyOrUndefined(strIdWithHash)) {
        strIdWithHash = "#GIGrid, #SIGrid, #Grid";
    }
    // End JSL 02/14/2023
    
    $("#context-menu").show();
    $("#context-menu").kendoContextMenu({
        target: strIdWithHash,  // JSL 02/14/2023
        filter: ".k-grid-content tbody tr[role='row'] td[class='mainContext']",
        select: function (e) {
            var row = $(e.target).parent()[0];
            //var grid = $("#" + reptype + "Grid").data("kendoGrid");   // JSL 02/14/2023 commented due to not used
            var FormID = e.target.parentElement.children[0].textContent
            if (FormID && FormID != "") {
                var ship = e.target.parentElement.children[1].textContent;
                var type = "";
                if (e.target.parentElement.children[2] != undefined)
                    type = e.target.parentElement.children[2].textContent;
                var _url;
                if (type == "GI")
                    _url = RootUrl + "GIRList/Index?id=" + FormID;
                else if (type == "SI") {
                    var no = row.cells[4].textContent;
                    _url = RootUrl + "SIRList/DetailsView?id=" + FormID; //RDBJ 09/24/2021 Removed  + "&No=" + no;
                }
                else {
                    _url = RootUrl + "IAFList/DetailsView?id=" + FormID;
                }
                window.open(_url, '_blank');
            }
        }
    });
}
// End JSL 02/14/2023


function loadDetailGrid(repType) {
    if (repType != "IA") {
        $("#" + repType + "Grid tbody").on("click", "tr", function (e) {

            // JSL 12/19/2022
            var blnIsVisitor = '';
            if (_CurrentUserDetailsObject.UserGroup == '8') {
                blnIsVisitor = 'disabled = true';
            }
            // End JSL 12/19/2022

            var FormID = e.currentTarget.children[0].textContent;
            var ship = e.currentTarget.children[1].textContent;
            var Type = repType;// "GI";
            if (e.currentTarget.children.length > 1 && e.currentTarget.children[2]) {
                Type = e.currentTarget.children[2].textContent;
            }
            if (ship != "" && FormID != "" && Type != "") {
                var url = RootUrl + "Deficiencies/GetGISIReportsDeficiencies";
                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    async: false,
                    url: url,
                    data: { ship: ship, UniqueFormID: FormID, Type: Type },
                    success: function (Data) {
                        $("#info").html("Ship <strong>" + ship + "</strong>" + " " + Data.length + " Deficiencies data.");
                        data = Data;
                        $('#' + repType + 'DetailsGrid').empty();
                        var grid = $('#' + repType + 'DetailsGrid').kendoGrid({
                            scrollable: true,
                            sortable: true,
                            resizable: true,
                            filterable: true,
                            selectable: true,
                            noRecords: true,
                            messages: {
                                noRecords: "No record found."
                            },
                            pageable: {
                                //pageSize: 5   // JSL 07/04/2022 commented 
                                alwaysVisible: true,    // JSL 07/04/2022
                                pageSizes: [5, 10, 20, 100] // JSL 07/04/2022
                            },
                            dataSource: {
                                data: data
                                , pageSize: 10  // JSL 07/04/2022
                            },
                            dataBound: function () {
                                $('#' + repType + 'DetailsGrid .k-grid-content').css("min-height", "130px");
                                if (repType == "GI") {
                                    $('#' + repType + 'DetailsGrid thead [data-field=Number] .k-link').html("Def. No.");
                                } else {
                                    $('#' + repType + 'DetailsGrid thead [data-field=Number] .k-link').html("Act. No.");
                                }
                                setTimeout(function () {
                                    if (repType != 'IA') {
                                        var gridDataDetail = $('#' + repType + 'DetailsGrid').data("kendoGrid");
                                        //if (gridDataDetail.dataSource.data().length > 0) {
                                        gridDataDetail.autoFitColumn("Number");
                                        gridDataDetail.autoFitColumn("Section");
                                        //gridDataDetail.autoFitColumn("Deficiency");
                                        gridDataDetail.autoFitColumn("Inspector");
                                        gridDataDetail.autoFitColumn("CreatedDate");
                                        gridDataDetail.autoFitColumn("Port");
                                        gridDataDetail.autoFitColumn("IsColse");
                                        gridDataDetail.autoFitColumn("GIRDeficienciesFile");
                                        // RDBJ 01/25/2022
                                        gridDataDetail.autoFitColumn("AssignTo");
                                        gridDataDetail.autoFitColumn("UpdatedDate");
										// End RDBJ 01/25/2022
                                        //}
                                    }
                                }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5

                                BindToolTipsForGridText(repType + "DetailsGrid", "Deficiency", 70);   // RDBJ 02/08/2022
                            },
                            //detailTemplate: kendo.template($("#template").html()),
                            //detailInit: detailInit,
                            change: function () {
                                var row = this.select();
                                //var FormID = row[0].cells[1].textContent;
                                var defID = row[0].cells[0].textContent;
                                var deficienciesUniqueID = row[0].cells[0].textContent; // RDBJ 12/16/2021 Update index number after added Updated Date column
                              //  var uniq = row[0].cells[4].textContent;
                                if (FormID && FormID != "") {
                                    var _url = RootUrl + "GIRList/DeficienciesDetails?id=" + deficienciesUniqueID;
                                    window.open(_url, '_blank');
                                }
                            },
                            columns: [
                                // RDBJ 12/21/2021 set at 0 index
                                {
                                    field: "DeficienciesUniqueID",
                                    title: "DeficienciesUniqueID",
                                    hidden: true,
                                    attributes: { class: "DeficienciesUniqueID" }
                                },
                                {
                                    field: "GIRFormID",
                                    title: "GIRFormID",
                                    hidden: true,
                                    attributes: { class: "GIRFormID" }
                                },
                                {
                                    field: "Number",
                                    title: "Def. No.",
                                    width: "20%"
                                },
                                {
                                    field: "ReportType",
                                    title: "Report Type",
                                    width: "120px",
                                    hidden: true,
                                },
                                {
                                    field: "UniqueFormID",
                                    title: "UniqueFormID",
                                    hidden: true,
                                },
                                {
                                    field: "Section",
                                    title: "Section",
                                    width: "40%"

                                },
                                {
                                    field: "Deficiency",
                                    title: "Description",
                                    width: width,
                                    attributes: { class: "tooltipText" },   // RDBJ 02/08/2022
                                },
                                // RDBJ 12/21/2021
                                {
                                    field: "AssignTo",
                                    title: "Assign To",
                                    width: "25%",
                                },
                                // End RDBJ 12/21/2021
                                {
                                    field: "Inspector",
                                    title: "Created By",
                                    width: "25%",
                                    hidden: true, // RDBJ 12/14/2021
                                },
                                {
                                    field: "CreatedDate",
                                    title: "Created Date",
                                    template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy'):'' #",
                                    width: "20%",
                                    hidden: true, // RDBJ 12/14/2021
                                },
                                {
                                    field: "Port",
                                    title: "Location",
                                    width: "20%",
                                    hidden: true, // RDBJ 12/14/2021
                                },
                                // RDBJ 12/16/2021
                                {
                                    field: "UpdatedDate",
                                    title: "Last Update",
                                    template: "#= UpdatedDate!=null? kendo.toString(kendo.parseDate(UpdatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy'):'' #",
                                    width: "20%",
                                },
                                {
                                    field: "IsColse",
                                    title: "Is Closed?",
                                    type: "boolean",
                                    template: '<input onclick="updateDefiStatus(this)"  type="checkbox" #= IsColse ? \'checked="checked"\' : "" # class="chkbx" ' + blnIsVisitor + '  />',   // JSL 12/19/2022 added blnIsVisitor
                                    width: "18%",
                                    //filterable: { ui: boolFilterTemplate }
                                },
                                {
                                    field: 'GIRDeficienciesFile',
                                    title: 'Files',
                                    template: "#=generateTemplateDeficiencies(GIRDeficienciesFile)#",
                                    width: "20%"
                                },
                                
                            ]
                        });
                        setTimeout(function () {
                            if (repType != 'IA') {
                                var gridDataDetail = $('#' + repType + 'DetailsGrid').data("kendoGrid");
                                //if (gridDataDetail.dataSource.data().length > 0) {
                                gridDataDetail.autoFitColumn("Number");
                                gridDataDetail.autoFitColumn("Section");
                                //gridDataDetail.autoFitColumn("Deficiency");
                                gridDataDetail.autoFitColumn("Inspector");
                                gridDataDetail.autoFitColumn("CreatedDate");
                                gridDataDetail.autoFitColumn("Port");
                                gridDataDetail.autoFitColumn("IsColse");
                                gridDataDetail.autoFitColumn("GIRDeficienciesFile");
                                // RDBJ 01/25/2022
                                gridDataDetail.autoFitColumn("AssignTo");
                                gridDataDetail.autoFitColumn("UpdatedDate");
								// End RDBJ 01/25/2022
                                //}
                            }
                        }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5
                        $('#context-menu').hide();
                        $("#child-menu").show();
                        $("#child-menu").kendoContextMenu({
                            target: "#GIDetailsGrid",
                            filter: "td",
                            select: function (e) {
                                //var row = $(e.target).parent()[0];
                                var id = e.target.parentElement.children[4].textContent
                                var section = e.target.parentElement.children[5].textContent
                                var _url = RootUrl + "GIRList/Index?id=" + id + "&isDefectSection=" + true + "&section=" + section;
                                window.open(_url, '_blank');
                            }
                        });
                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }
            else if (e.currentTarget.className == "k-master-row" || e.currentTarget.className == "k-alt k-master-row") {
                if (e.target.className != "k-icon k-i-expand" && e.target.className != "k-icon k-i-collapse") {
                    LoadAllDeficiencies(e);
                }
            }
        });
    }
}
function LoadAllDeficiencies(e) {
    var name = e.currentTarget.children[1].textContent;
    var url = RootUrl + "Forms/GIRDetails";
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: { code: name },
        success: function (Data) {
            $("#info").show();
            $("#info").html("Ship <strong>" + name + "</strong>" + " " + Data.length + " Deficiencies data.");
            data = Data;
            $('#DetailsGrid').empty();
            var grid = $('#DetailsGrid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                selectable: true,
                dataSource: {
                    data: data
                },
                dataBound: function () {
                    for (var i = 0; i < this.columns.length; i++) {
                        //  this.autoFitColumn(i);
                    }
                },
                detailTemplate: kendo.template($("#template").html()),
                detailInit: detailInit,
                change: function () {
                    var row = this.select();
                    var id = row[0].cells[2].textContent;
                    var type = row[0].cells[4].textContent;
                    var no = row[0].cells[3].textContent;
                    var _url;
                    if (type == "GI")
                        _url = RootUrl + "GIRList/Index?id=" + id + "&isDefectSection=" + true;
                    else
                        _url = RootUrl + "SIRList/DetailsView?id=" + id + "&isDefectSection=" + true + "&No=" + no;
                    window.open(_url, '_blank');
                },
                columns: [
                    {
                        field: "DeficienciesID",
                        title: "DeficienciesID",
                        hidden: true,
                        attributes: { class: "DeficienciesID" }
                    },
                    {
                        field: "GIRFormID",
                        title: "GIRFormID",
                        hidden: true,
                        attributes: { class: "GIRFormID" }
                    },
                    {
                        field: "Number",
                        title: "Deficiency No",
                        width: "150px"
                    },
                    {
                        field: "ReportType",
                        title: "Report Type",
                        width: "120px"

                    },
                    {
                        field: "Deficiency",
                        title: "Description",
                        //width: "50px"
                    },
                    {
                        field: "IsColse",
                        title: "Is Closed?",
                        template: '<input onclick="updateDefiStatus(this)"  type="checkbox" #= IsColse ? \'checked="checked"\' : "" # class="chkbx"  />',
                        width: "120px"
                    },
                    {
                        field: 'GIRDeficienciesFile',
                        title: 'Files',
                        template: "#=generateTemplateDeficiencies(GIRDeficienciesFile)#",
                        width: "200px"
                    },
                ]

            });

        },
        error: function (data) {
            console.log(data);
        }
    });
}
function loadComment(id, url) {
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
function AddNotes(id, ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = RootUrl + "Forms/AddDeficienciesNote";
    var comment = $(ctr).parent().parent().find('.comment').val();
    var obj = {};
    obj.UserName = name;
    obj.Comment = comment;
    obj.DeficienciesID = id;
    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {
        var GIRDeficienciesCommentFile = new Object();
        GIRDeficienciesCommentFile.GIRCommentFileID = 0;
        GIRDeficienciesCommentFile.NoteID = 0;
        GIRDeficienciesCommentFile.DeficienciesID = id;
        GIRDeficienciesCommentFile.FileName = $(this).next().val();
        GIRDeficienciesCommentFile.StorePath = $(this).next().next().val();
        GIRDeficienciesCommentFile.IsUpload = $(this).next().next().next().val();
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
            var url = RootUrl + "Forms/GetDeficienciesNote";
            loadComment(id, url)
            var childGrid = $(ctr).parent().parent().parent().parent().parent().find('.orders')
            $(childGrid).kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                dataSource: {
                    data: dataLoad.options.data
                },
                columns: [
                    {
                        field: "UserName",
                        title: "Name",
                        width: "150px"
                    },
                    {
                        field: "Comment",
                        title: "Comment"
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
                        width: "130px"
                    }
                ]
            });

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
}

function InitReportDetails(e) {
    var detailRow = e.detailRow;
    var code = e.data.Ship;
    var url = RootUrl + "Deficiencies/GetShipGISIReports";
    locadShipReports(code, url);
    detailRow.find(".ShipGISIReports").kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        selectable: true,
        dataSource: {
            data: ShipReports.options.data
        },
        dataBound: function (e) {
            var rows = e.sender.tbody.children();
            var dataItem = e.sender.dataItem(row);
            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);
                var isExpired = $(row).find('td:eq(7)').text();
                if (isExpired == "true") {
                    row.css("color", "#da3b3b");
                }
            }
        },
        resizable: true,
        columns: [
            {
                field: "FormID",
                title: "FormID",
                hidden: true,
            },
            {
                field: "Ship",
                title: "Ship",
                hidden: true,
            },
            {
                field: "Type",
                title: "Type",
                hidden: true,
            },
            {
                field: "Date",
                title: "Date",
            },
            {
                field: "Location",
                title: "Location",
            },
            {
                field: "Deficiencies",
                title: "Deficiencies",
            },
            {
                field: "OpenDeficiencies",
                title: "OpenDeficiencies",
            },
            {
                field: "isExpired",
                title: "is Expired",
                hidden: true,
            },
        ],
        change: function () {
            //var row = this.select();
            //var id = row[0].cells[0].textContent;
            //var type = row[0].cells[2].textContent;
            //var no = row[0].cells[4].textContent;
            //var _url;
            //if (type == "GI")
            //    _url = RootUrl + "GIRList/Index?id=" + id;
            //else
            //    _url = RootUrl + "SIRList/DetailsView?id=" + id + "&No=" + no;
            //window.open(_url, '_blank');
        }
    });
    //$("#context-menu").show();
    //$("#context-menu").kendoContextMenu({
    //    target: detailRow.find(".ShipGISIReports"),
    //    filter: "td",
    //    select: function (e) {
    //        var row = $(e.target).parent()[0];
    //        var grid = $("#Grid").data("kendoGrid");
    //        var FormID = e.target.parentElement.children[0].textContent
    //        if (FormID && FormID != "") {
    //            var ship = e.target.parentElement.children[1].textContent;
    //            var type = "";
    //            if (e.target.parentElement.children[2] != undefined)
    //                type = e.target.parentElement.children[2].textContent;
    //            var _url;
    //            if (type == "GI")
    //                _url = RootUrl + "GIRList/Index?id=" + FormID;
    //            else {
    //                var no = row.cells[4].textContent;
    //                _url = RootUrl + "SIRList/DetailsView?id=" + FormID + "&No=" + no;
    //            }
    //            window.open(_url, '_blank');
    //        }
    //    }
    //});
}

function locadShipReports(code, url, repType) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { code: code, type: repType },
        success: function (Data) {
            ShipReports = new kendo.data.DataSource({
                data: Data
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function loadbyDetailtab(id, ctr) {
    var url = RootUrl + "Forms/GetDeficienciesResolution";
    loadDeficienciesInitialActions(id, url);
    var childGrid = $(ctr).parent().parent().parent().parent().find('.resoultionList');
    $(childGrid).kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        dataSource: {
            data: dataLoad.options.data
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
            }
        ]
    });
}

// RDBJ 01/05/2022
function DeleteGISIDrafts(GISIFormID, type, ctr) {
    var confirm_result = confirm("Are you sure you want to delete this Form?");
    if (confirm_result == true) {
        var url = RootUrl + "Drafts/DeleteGISIIADrafts";//?formId=" + GISIFormID + "&type=" + type;;
        $.ajax({
            url: url,
            data: { GISIFormID: GISIFormID, type: type },
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            type: "GET",
            success: function () {
                $.notify(type + " Form is Deleted Successfully.", "success");
                ctr.parentElement.parentElement.parentElement.remove();
                //window.location = RootUrl + "Drafts/";
            }
        });
    }
}
// End RDBJ 01/05/2022

// RDBJ 01/10/2022
function boolFilterTemplate(input) {
    input.kendoDropDownList({
        dataSource: {
            data: [
                { text: "True", value: true },
                { text: "False", value: false }
            ]
        },
        dataTextField: "text",
        dataValueField: "value",
        valuePrimitive: true,
        optionLabel: "All"
    });
}
// End RDBJ 01/10/2022

// JSL 05/25/2022
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
// End JSL 05/25/2022

// JSL 12/19/2022
function ShowHideDivAddEditAuditSection() {
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            $("#divAddEditAuditSection").remove();
            $("#divAddEditFSTOSection").remove();   // JSL 02/14/2023
        }
        else {
            $("#divAddEditAuditSection").css("display", "flex");
            $("#divAddEditFSTOSection").css("display", "flex"); // JSL 02/14/2023
        }
    }
    else {
        $("#divAddEditAuditSection").remove();
        $("#divAddEditFSTOSection").remove();   // JSL 02/14/2023
    }
}
// End JSL 12/19/2022
