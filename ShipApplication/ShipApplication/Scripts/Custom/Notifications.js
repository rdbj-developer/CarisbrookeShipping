var notificationsItems; //RDBJ 10/18/2021
var notTblReport; //RDBJ 10/18/2021

//RDBJ 10/14/2021 Added
$(document).ready(function () {
    //autoRefresh();    // JSL 07/05/2022 commented this due to refresh //RDBJ 10/21/2021
    //RDBJ 10/18/2021
    $('#tblNotifications').on('click', 'tr', function () {
        var row = $(this).closest('tr');

        // RDBJ 01/07/2022 wrapped in if
        if ($('#tblNotifications').dataTable().fnGetData(row) != null) {
            var deficienciesNoteUniqueID = $('#tblNotifications').dataTable().fnGetData(row).DeficienciesUniqueID;
            var ReportType = $('#tblNotifications').dataTable().fnGetData(row).ReportType; //RDBJ 10/21/2021
            openDeficiencyDetails(deficienciesNoteUniqueID, ReportType); //RDBJ 10/21/2021 Added ReportType
        }
    });
});

//RDBJ 10/14/2021
function openDeficiencyDetails(deficienciesNoteUniqueID, ReportType) {  //RDBJ 10/21/2021 Added ReportType
    var _url = "";
    //RDBJ 10/21/2021 Wrapped in If
    if (ReportType == "GI" || ReportType == "SI")
        _url = RootUrl + "Deficiencies/DeficienciesDetails?id=" + deficienciesNoteUniqueID;
    else
        _url = RootUrl + "Deficiencies/InternalAuditDetails?id=" + deficienciesNoteUniqueID; //RDBJ 10/21/2021

    window.open(_url, '_blank');
}
//End RDBJ 10/14/2021

//RDBJ 10/16/2021
function autoRefresh() {
    var intervalId = window.setInterval(function () {
        getNotifications();
    }, 5000); //RDBJ 10/18/2021 set 5 Seconds to refresh this page
};
//End RDBJ 10/16/2021

//RDBJ 10/18/2021 Modified //RDBJ 10/16/2021
function getNotifications() {
    var url = RootUrl + 'Notifications/GetNotifications';
    $.ajax({
        url: url,
        type: "POST",
        success: function (res) {
            console.log("Page Call");
            LoadNewNotifications(res.GISINotifications); // JSL 07/05/2022

            // JSL 07/05/2022 commented 
            /*
            notificationsItems = res.GISINotifications; //RDBJ 10/18/2021
            bindNotificationsData(notificationsItems); //RDBJ 10/18/2021
            $("#tblNotifications th").removeClass("deficiencyTextEllipsis");  //RDBJ 10/21/2021
            $("#tblNotifications th").removeClass("shipTextEllipsis"); //RDBJ 10/21/2021
            $('#tblNotifications').DataTable().clear().rows.add(notificationsItems).draw(false); // RDBJ 12/18/2021 set draw(false) //RDBJ 10/22/2021 Auto refresh
            */
            // End JSL 07/05/2022 commented 
        },
        global: false,
        error: function (err) {
            console.log(err);
        }
    });
};
//End RDBJ 10/16/2021

//RDBJ 10/18/2021
function bindNotificationsData(notificationsItems) {
    //Load datatable
    notTblReport = $("#tblNotifications");
    notTblReport.DataTable({
        "data": notificationsItems,
        "order": [[7, "desc"]], // RDBJ 12/18/2021 set UpdatedDate Descending
        retrieve: true,
        "language": {
            "lengthMenu": "Show _MENU_ Notifications",
            "emptyTable": "Notifications Not Availabe!"
        },
        "columns": [
            {
                data: "DeficienciesUniqueID",
                title: "ID",
                visible: false,
            },
            //RDBJ 10/21/2021 Commented
            /*
            {
                "data": "Ship",
                title: "Ship",
                width: "12%",
            },
            */
            //RDBJ 10/21/2021
            {
                "data": "ShipName",
                title: "Ship",
                width: "12%",
                className: "shipTextEllipsis",
                "render": function (value, type, full, meta) {
                    return '<span data-toggle="tooltip" title="' + value + '">' + value + '</span>';
                }
            },
            {
                "data": "ReportType",
                title: "ReportType",
                width: "10%",
                className: "textCenter",
                "render": function (value) {
                    if (value === null) return "";

                    var rtnValue = "";
                    var toolTip = "";

                    if (value === "GI") {
                        rtnValue = "GIR";
                        toolTip = "General Inspection Report";
                    }
                    else if (value === "SI") {
                        rtnValue = "SIR";
                        toolTip = "Superintended Inspection Report";
                    }
                    else {
                        rtnValue = "IAF";
                        toolTip = "Internal Audit Form";
                    }

                    return '<span data-toggle="tooltip" title="' + toolTip + '">' + rtnValue + '</span>';;
                },
            },
            //End RDBJ 10/21/2021
            {
                "data": "Deficiency",
                title: "Deficiencies / Actionable Items",
                //width: "40%",
                className: "deficiencyTextEllipsis", //RDBJ 10/21/2021
                //RDBJ 10/21/2021 added render
                "render": function (value, type, full, meta) {
                    return '<span data-toggle="tooltip" title="' + value + '">' + value + '</span>';
                }
            },
            {
                "data": "CommentsCount",
                title: "New Comments",
                width: "10%",
                className: "textCenter",
            },
            {
                "data": "InitialActionsCount",
                title: "New Initial Actions",
                width: "10%",
                className: "textCenter",
            },
            {
                "data": "ResolutionsCount",
                title: "New Resolutions",
                width: "10%",
                className: "textCenter",
            },
            {
                "data": "UpdatedDate",
                title: "Recent",
                width: "15%",
                className: "textCenter", //RDBJ 10/21/2021
                //type: "date", // RDBJ 01/07/2022 commented this line due to sorting not working
                "render": function (value) {
                    if (value === null) return "";
                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));
                    return moment(dt).fromNow(); //(dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
                }
            }
        ]
    });
};
//End RDBJ 10/18/2021

// JSL 07/05/2022
function LoadNewNotifications(data) {
    $('#notificationGrid').empty();
    $("#notificationGrid").kendoGrid({
        dataSource: {
            data: data,
            pageSize: 30
        },
        height: 550,
        sortable: true,
        selectable: true,
        noRecords: true,
        messages: {
            noRecords: "No notification found."
        },
        filterable: true,
        toolbar: [
            { template: kendo.template($("#template").html()) }
        ],
        search: {
            fields: [
                { name: "ShipName", operator: "contains" },
                { name: "ReportType", operator: "contains" },
                { name: "Deficiency", operator: "contains" },
                //{ name: "AssignTo", operator: "contains" },
                { name: "CommentsCount", operator: "gte" },
                { name: "InitialActionsCount", operator: "gte" },
                //{ name: "ResolutionsCount", operator: "gte" },    // JSL 07/18/2022 commented
            ]
        },
        pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
        },
        dataBound: function () {
            LetterAvatar.transform();
        },
        change: function () {
            LetterAvatar.transform();

            var row = this.select();
            var deficienciesUniqueID = row[0].cells[0].textContent;
            var elementReportType = row[0].cells[2].innerHTML;
            var ReportType = $(elementReportType).attr("data-reporttype");
            openDeficiencyDetails(deficienciesUniqueID, ReportType);
        },
        columns:
            [
                {
                    field: "DeficienciesUniqueID",
                    title: "DeficienciesUniqueID",
                    hidden: true,
                    attributes: { class: "DeficienciesUniqueID" }
                },
                {
                    field: "ShipName",
                    title: "Ship",
                    filterable: false,
                    template: "#=generateTemplateForShip(ShipName)#",
                    width: 140
                },
                {
                    field: "ReportType",
                    title: "Form",
                    filterable: false,
                    template: "#=generateTemplateToSetAvatarForFormType(ReportType)#",
                    width: 80
                },
                {
                    field: "Deficiency",
                    title: "Details",
                    filterable: false,
                    attributes: {
                    }
                },
                //{
                //    field: "AssignTo",
                //    title: "Assigned To",
                //    template: "#=generateTemplateForAssignedTo(AssignTo)#",
                //    filterable: false,
                //    width: 150
                //},
                {
                    field: "CommentsCount",
                    title: "Comments",
                    filterable: false,
                    width: 120
                },
                {
                    field: "InitialActionsCount",
                    title: "InitialActions",
                    filterable: false,
                    width: 120
                },
                {
                    field: "ResolutionsCount",
                    title: "Resolutions",
                    filterable: false,
                    hidden: true,   // JSL 07/18/2022
                    width: 120
                },
                {
                    field: "UpdatedDate",
                    title: "Recent",
                    template: "#=generateTemplateToSetRecentTime(UpdatedDate)#",
                    filterable: false,
                    width: 140
                }
            ]
    });
}
// End JSL 07/05/2022

// JSL 07/05/2022
function generateTemplateForShip(value) {
    var retTemplate = "";
    if (!IsNullEmptyOrUndefined(value)) {
        var html = "";
        html += '<div style="display: flex;">';
        html += '<div>';
        html += '<i style="font-size: 25px; padding-right: 5px;" class="fa fa-ship" aria-hidden="true"></i>';
        html += '</div>';
        html += '<div>';
        html += '<span data-toggle="tooltip" title="' + value + '">' + value + '</span>';
        html += '</div>';
        html += '</div>';

        retTemplate = html;
    }
    return retTemplate;
}
// End JSL 07/05/2022


// JSL 07/05/2022
function generateTemplateToSetAvatarForFormType(value) {
    var retTemplate = "";
    var strReportType = value;  // JSL 07/01/2022
    if (!IsNullEmptyOrUndefined(value)) {
        if (value === "GI") {
            value = "General Inspection Report";
        }
        else if (value === "SI") {
            value = "Superintended Inspection Report";
        }
        else {
            value = "Internal Audit Form";
        }
        //retTemplate += '<span>' + value + '</span>';
        retTemplate += '<img class="formTypeCircle" data-reporttype="' + strReportType + '" data-toggle="tooltip" title="' + value + '" avatar="' + value + '">';   // JSL 07/01/2022 added data-reporttype
    }
    return retTemplate;
}
// End JSL 07/05/2022

// JSL 07/05/2022
function generateTemplateForAssignedTo(value) {
    var retTemplate = "";
    if (!IsNullEmptyOrUndefined(value)) {
        var html = "";
        html += '<div style="display: flex;" data-toggle="tooltip" title="' + value + '">';
        html += '<div>';
        html += '<i style="font-size: 25px; padding-right: 5px;" class="fa fa-user-circle-o" aria-hidden="true"></i>';
        html += '</div>';
        html += '<div class="deficiencyTextEllipsis">';
        html += '<span style="overflow: hidden;">' + value + '</span>';
        html += '</div>';
        html += '</div>';

        retTemplate = html;
    }
    return retTemplate;
}
// End JSL 07/05/2022

// JSL 07/05/2022
function generateTemplateToSetRecentTime(value) {
    var retTemplate = "";
    if (!IsNullEmptyOrUndefined(value)) {
        var recentTime = moment(value).fromNow();
        var html = "";
        html += '<div class="deficiencyTextEllipsis" data-toggle="tooltip" title="' + recentTime + '">';
        html += '<span style="overflow: hidden;">' + recentTime + '</span>';
        html += '</div>';

        retTemplate = html;
    }
    return retTemplate;
}
// End JSL 07/05/2022
