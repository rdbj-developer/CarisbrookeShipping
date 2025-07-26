var dataLoad;
var dataLoadFile;
var ShipReports;
var shipCode;
var width;
$(document).ready(function () {
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
    shipCode = $("#ddlGIReportShipName").val();
    loadGrid(shipCode, "GI");
    loadDetailGrid("GI");
    $('#GIGrid tbody tr:first-child').trigger('click');
});

function DownloadDeficienciesFile(ctr, data) {  // RDBJ 02/11/2022 rename function due to duplicate in another file
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

function generateTemplateDeficiencies(ReportList) {
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="Download(' + ReportList[i].GIRDeficienciesFileID + ', this);">' + ReportList[i].FileName + '</a>' +
            //'<a onclick="DownloadDeficienciesFile(' + ReportList[i].GIRDeficienciesFileID + ', this);" title="' + ReportList[i].FileName + '"><i class="fa fa-download"></i></a>' + // RDBJ 02/11/2022
            '</div>';
    }
    return template;
}

function loadDetailGridByShip(ship) {
    var type = "";
    var activeTab = $("ul#custom-content-below-tab li.active a")[0].id
    if (activeTab.includes("gi")) {
        type = "GI";
    }
    // RDBJ 03/08/2022
    else if (activeTab.includes("si")) {
        type = "SI";
    }
    // RDBJ 03/08/2022
    else {
        type = "IA";
    }

    // RDBJ 02/11/2022 wrapped in if to avoid multiple call
    if (type.toUpperCase() != "IA") {
        loadGrid(ship.value, type);
        $('#' + type + 'DetailsGrid').empty();
        loadDetailGrid(type);
        $('#' + type + 'Grid tbody tr:first-child').trigger('click');
    }
    else {
        InitShipAudits(ship.value);
        $('#DetailsGrid').empty();
        LoadAuditDetail();
        $('#Grid tbody tr:first-child').trigger('click');
    }
}
function tabChangeLoadGridData(type) {
    shipCode = $("#ddlGIReportShipName").val();

    // RDBJ 02/11/2022 wrapped in if to avoid multiple call
    if (type.toUpperCase() != "IA") {
        loadGrid(shipCode, type);
        $('#' + type + 'DetailsGrid').empty();
        loadDetailGrid(type);
        $('#' + type + 'Grid tbody tr:first-child').trigger('click');
    }
    else {
        InitShipAudits(shipCode);
        $('#DetailsGrid').empty();
        LoadAuditDetail();
        $('#Grid tbody tr:first-child').trigger('click');
    }
}
function detailInit(e) {
    var detailRow = e.detailRow;
    var id = e.data.DeficienciesID;
    detailRow.find(".tabstrip").kendoTabStrip({
        animation: {
            open: { effects: "fadeIn" }
        }
    });
    var url = RootUrl + "Deficiencies/GetDeficienciesNote";
    var urlFile = RootUrl + "Deficiencies/GetDeficienciesFiles";
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

//function updateStatus(ctr) {
//    var did = $(ctr).parent().parent().find('.DeficienciesID').text();
//    var url = RootUrl + "Deficiencies/UpdateDeficienciesData";
//    var obj = {};
//    obj.id = did;
//    obj.isClose = $(ctr).prop('checked');
//    $.ajax({
//        type: 'POST',
//        dataType: 'json',
//        async: false,
//        url: url,
//        data: obj,
//        success: function (Data) {
//            $.notify("Updated Successfully", "success");
//            var shipCode = $("#ddlGIReportShipName").val();
//            var type = "";
//            var activeTab = $("ul#custom-content-below-tab li.active a")[0].id
//            if (activeTab.includes("gi")) {
//                type = "GI";
//            } else {
//                type = "SI";
//            }

//            loadGrid(shipCode, type);
//            loadDetailGrid(type);
//        },
//        error: function (Data) {
//            console.log(Data);
//        }
//    });
//}
function loadGrid(ship, reptype) {
    var url = RootUrl + "Deficiencies/GetShipGISIReports";
    locadShipReports(ship, url, reptype);
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
                // JSL 06/27/2022 added pageable
                pageable: {
                    //pageSize: 10,    // JSL 06/27/2022 commented this line
                    alwaysVisible: true,
                    pageSizes: [5, 10, 20, 100]
                },
                dataSource: {
                    data: data,
                    pageSize: 10  // JSL 06/27/2022
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
                            //row.css("background-color", "#da3b3b");
                            row.css("color", "#da3b3b");
                        }
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
                    }, 3000);   // JSL 07/18/2022 set 3 secs instead of 1.5
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
                }
            }, 3000);   // JSL 07/18/2022 set 3 secs instead of 1.5
            $("#context-menu").show();
            $("#context-menu").kendoContextMenu({
                
                filter: ".k-grid-content tbody tr[role='row'] td[class='mainContext']",
                select: function (e) {
                    var row = $(e.target).parent()[0];
                    var grid = $("#" + reptype + "Grid").data("kendoGrid");
                    var FormID = e.target.parentElement.children[0].textContent
                    if (FormID && FormID != "") {
                        //var ship = e.target.parentElement.children[1].textContent;
                        var type = "";
                        if (e.target.parentElement.children[2] != undefined)
                            type = e.target.parentElement.children[2].textContent;
                        var _url;
                        if (type == "GI")
                            _url = RootUrl + "Drafts/GIRformData?id=" + FormID;
                        else if (type == "SI") {
                            var no = row.cells[4].textContent;
                            _url = RootUrl + "Drafts/SIRformData?id=" + FormID + "&No=" + no;
                        }
                        else {
                            _url = RootUrl + "Drafts/IARformData?id=" + FormID;
                        }
                        window.open(_url, '_blank');
                    }
                }
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function loadDetailGrid(repType) {
    $("#" + repType + "Grid tbody").on("click", "tr", function (e) {
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
                success: function (response) {
                    var Data = response.deficiencies; //RDBJ 10/26/2021
                    var isInspector = response.isInspector; //RDBJ 10/26/2021

                    var templateIsClose = ''; //RDBJ 11/9/2021

                    //RDBJ 11/9/2021 added if else
                    if (isInspector == true) {
                        templateIsClose = '<input onclick="updateDefiStatus(this)"  type="checkbox" #= IsClose ? \'checked="checked"\' : "" # class="chkbx"  />';
                    }
                    else {
                        templateIsClose = '<input onclick="return false;"  type="checkbox" #= IsClose ? \'checked="checked"\' : "" # class="chkbx"  />';
                    }
                    //$("#info").show(); //Hide Show Alert
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
                        dataSource: {
                            data: data
                            , pageSize: 10  // JSL 06/27/2022
                        },
                        // JSL 06/27/2022 added pageable
                        pageable: {
                            //pageSize: 5,    // JSL 06/27/2022 commented this line
                            alwaysVisible: true,
                            pageSizes: [5, 10, 20, 100]
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
                                    gridDataDetail.autoFitColumn("Number");
                                    gridDataDetail.autoFitColumn("Section");
                                    //gridDataDetail.autoFitColumn("Deficiency");
                                    gridDataDetail.autoFitColumn("Inspector");
                                    gridDataDetail.autoFitColumn("CreatedDate");
                                    gridDataDetail.autoFitColumn("Port");
                                    gridDataDetail.autoFitColumn("IsColse");
                                    gridDataDetail.autoFitColumn("GIRDeficienciesFile");
                                }
                            }, 3000);   // JSL 07/18/2022 set 3 secs instead of 1.5

                            BindToolTipsForGridText(repType + "DetailsGrid", "Deficiency", 70);   // RDBJ 02/08/2022
                        },
                        //detailTemplate: kendo.template($("#template").html()),
                        //detailInit: detailInit,
                        change: function () {
                            //var row = this.select();
                            //var id = row[0].cells[2].textContent;
                            //var type = row[0].cells[4].textContent;
                            //var no = row[0].cells[3].textContent;
                            //var section = row[0].cells[5].textContent;
                            //var _url;
                            //if (type == "GI")
                            //    //_url = RootUrl + "Drafts/GIRformData?id=" + id + "&isDefectSection=" + true;
                            //    _url = RootUrl + "Drafts/GIRformData?id=" + id + "&isDefectSection=" + true + "&section=" + section;

                            //else
                            //    _url = RootUrl + "Drafts/SIRformData?id=" + id + "&isDefectSection=" + true + "&No=" + no;
                            //window.open(_url, '_blank');
                            var row = this.select();
                            //var FormID = row[0].cells[1].textContent;
                            var defID = row[0].cells[0].textContent;
                            var deficienciesUniqueID = row[0].cells[11].textContent; //RDBJ 10/26/2021 set 11
                            if (FormID && FormID != "") {
                                var _url = RootUrl + "Deficiencies/DeficienciesDetails?id=" + deficienciesUniqueID;
                                window.open(_url, '_blank');
                            }
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
                            {
                                field: "Inspector",
                                title: "Created By",
                                width: "25%"
                            },
                            {
                                field: "CreatedDate",
                                title: "Created Date",
                                template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy'):'' #",
                                width: "20%"
                            },
                            {
                                field: "Port",
                                title: "Location",
                                width: "25%"
                            },
                            //RDBJ 10/26/2021
                            {
                                field: "IsColse",
                                title: "Is Closed?",
                                //template: '<input onclick="updateDefiStatus(this)"  type="checkbox" #= IsClose ? \'checked="checked"\' : "" # class="chkbx"  />', //RDBJ 11/9/2021 Comment this line
                                template: templateIsClose,  //RDBJ 11/9/2021 added templateIsClose
                                width: "18%",
                                //hidden: !isInspector, //RDBJ 11/9/2021 comment this line
                            },
                            {
                                field: 'GIRDeficienciesFile',
                                title: 'Files',
                                template: "#=generateTemplateDeficiencies(GIRDeficienciesFile)#",
                                width: "20%",
                                attributes: { class: "avoidClick" }, // RDBJ 02/11/2022
                            },
                            {
                                field: "DeficienciesUniqueID",
                                title: "DeficienciesUniqueID",
                                hidden: true,
                                attributes: { class: "DeficienciesUniqueID" }, //RDBJ 10/26/2021
                            },
                            {
                                field: "UniqueFormID",
                                title: "UniqueFormID",
                                hidden: true,
                            }
                        ]
                    });
                    setTimeout(function () {
                        if (repType != 'IA') {
                            var gridDataDetail = $('#' + repType + 'DetailsGrid').data("kendoGrid");
                            gridDataDetail.autoFitColumn("Number");
                            gridDataDetail.autoFitColumn("Section");
                            //gridDataDetail.autoFitColumn("Deficiency");
                            gridDataDetail.autoFitColumn("Inspector");
                            gridDataDetail.autoFitColumn("CreatedDate");
                            gridDataDetail.autoFitColumn("Port");
                            gridDataDetail.autoFitColumn("IsColse");
                            gridDataDetail.autoFitColumn("GIRDeficienciesFile");
                        }
                    }, 3000);   // JSL 07/18/2022 set 3 secs instead of 1.5

                    // RDBJ 02/11/2022 commented
                    /*
                    $('#context-menu').hide();
                    $("#child-menu").show();
                    $("#child-menu").kendoContextMenu({
                        target: "#GIDetailsGrid,#SIDetailsGrid",
                        filter: "td",

                        //select: function (e) {
                        //    var row = $(e.target).parent()[0];
                        //    var grid = $("#" + reptype + "Grid").data("kendoGrid");
                        //    var FormID = e.target.parentElement.children[0].textContent
                        //    if (FormID && FormID != "") {
                        //        //var ship = e.target.parentElement.children[1].textContent;
                        //        var type = "";
                        //        if (e.target.parentElement.children[2] != undefined)
                        //            type = e.target.parentElement.children[2].textContent;
                        //        var _url;
                        //        if (type == "GI")
                        //            _url = RootUrl + "Drafts/GIRformData?id=" + FormID;
                        //        else if (type == "SI") {
                        //            var no = row.cells[4].textContent;
                        //            _url = RootUrl + "Drafts/SIRformData?id=" + FormID + "&No=" + no;
                        //        }
                        //        else {
                        //            _url = RootUrl + "Drafts/IARformData?id=" + FormID;
                        //        }
                        //        window.open(_url, '_blank');
                        //    }
                        //}

                        select: function (e) {
                            var type = "";
                            //var row = $(e.target).parent()[0];
                            var id = e.target.parentElement.children[11].textContent
                            var section = e.target.parentElement.children[4].textContent
                            if (e.target.parentElement.children[3] != undefined)
                                type = e.target.parentElement.children[3].textContent;
                            var _url = "";
                            if (type == "GI") {
                                _url = RootUrl + "Drafts/GIRformData?id=" + id + "&isDefectSection=" + true + "&section=" + section;
                            }
                            else {
                                _url = RootUrl + "Drafts/SIRformData?id=" + id + "&isDefectSection=" + true + "&section=" + section;
                            }
                            window.open(_url, '_blank');
                        }
                    });
                    */
                    // End RDBJ 02/11/2022 commented
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
                        _url = RootUrl + "Drafts/GIRformData?id=" + id + "&isDefectSection=" + true;
                    else
                        _url = RootUrl + "Drafts/DetailsView?id=" + id + "&isDefectSection=" + true + "&No=" + no;
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
                        template: '<input onclick="updateStatus(this)"  type="checkbox" #= IsColse ? \'checked="checked"\' : "" # class="chkbx"  />',
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
                    //row.css("background-color", "#da3b3b");
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
    $("#context-menu").show();
    $("#context-menu").kendoContextMenu({
        target: detailRow.find(".ShipGISIReports"),
        filter: "td",
        select: function (e) {
            var row = $(e.target).parent()[0];
            var grid = $("#Grid").data("kendoGrid");
            var FormID = e.target.parentElement.children[0].textContent
            if (FormID && FormID != "") {
                var ship = e.target.parentElement.children[1].textContent;
                var type = "";
                if (e.target.parentElement.children[2] != undefined)
                    type = e.target.parentElement.children[2].textContent;
                var _url;
                if (type == "GI")
                    _url = RootUrl + "GIRList/Index?id=" + FormID;
                else {
                    var no = row.cells[4].textContent;
                    _url = RootUrl + "SIRList/DetailsView?id=" + FormID + "&No=" + no;
                }
                window.open(_url, '_blank');
            }
        }
    });
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

//RDBJ 10/26/2021
function updateDefiStatus(ctr) {
    var did = $(ctr).parent().parent().find('.DeficienciesUniqueID').text();
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
//End RDBJ 10/26/2021