// JSL 07/04/2022
function GetDataWhenPageLoad() {
    var dic = {};
    dic["CurrentUserId"] = _CurrentUserDetailsObject.UserGUID;
    CommonServerPostApiCall(dic, "Notifications", "PerformAction", str_API_GETNOTIFICATIONFROMPAGE
        , false // JSL 05/16/2022
    );
}
// End JSL 07/04/2022

// JSL 06/29/2022
function LoadNewNotifications(data) {
    $('#notificationGrid').empty(); // JSL 07/01/2022
    $("#notificationGrid").kendoGrid({
        dataSource: {
            data: data,
            pageSize: 30
        },
        height: 550,
        sortable: true,
        // JSL 07/01/2022
        selectable: true,
        noRecords: true,
        messages: {
            noRecords: "No notification found."
        },
        // End JSL 07/01/2022
        filterable: false,
        //columnMenu: {
        //    componentType: "modern",
        //    columns: {
        //        sort: "asc"
        //    }
        //},
        toolbar: [
            { template: kendo.template($("#template").html()) }
        ],
        search: {
            fields: [
                { name: "ShipName", operator: "contains" },
                { name: "ReportType", operator: "contains" },
                { name: "Deficiency", operator: "contains" },
                { name: "AssignTo", operator: "contains" },
                { name: "CommentsCount", operator: "gte" },
                { name: "InitialActionsCount", operator: "gte" },
                //{ name: "ResolutionsCount", operator: "gte" },    // JSL 07/12/2022 commented
            ]
        },
        pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
        },
        dataBound: function () {
            LetterAvatar.transform();
            BindToolTipsForGridText("notificationGrid", "Deficiency", 120);
        },
        change: function () {
            LetterAvatar.transform();

            // JSL 07/01/2022
            var row = this.select();
            var deficienciesUniqueID = row[0].cells[0].textContent;
            var elementReportType = row[0].cells[2].innerHTML;
            var ReportType = $(elementReportType).attr("data-reporttype");
            openDeficiencyDetails(deficienciesUniqueID, ReportType);
            // End JSL 07/01/2022
        },
        columns:
            [
                // JSL 07/01/2022
                {
                    field: "DeficienciesUniqueID",
                    title: "DeficienciesUniqueID",
                    hidden: true,
                    attributes: { class: "DeficienciesUniqueID" }
                },
                // JSL 07/01/2022
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
                    // JSL 07/12/2022 added class 
                    attributes: {
                        class: "tooltipText",
                    }
                },
                {
                    field: "AssignTo",
                    title: "Assigned To",
                    template: "#=generateTemplateForAssignedTo(AssignTo)#",
                    filterable: false,
                    width: 150
                },
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
                    hidden: true,   // JSL 07/12/2022
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
// End JSL 06/29/2022

// JSL 07/01/2022
function openDeficiencyDetails(deficienciesNoteUniqueID, ReportType) { 
    var _url = "";
    if (ReportType == "GI" || ReportType == "SI")
        _url = RootUrl + "GIRList/DeficienciesDetails?id=" + deficienciesNoteUniqueID;
    else
        _url = RootUrl + "GIRList/InternalAuditDetails?id=" + deficienciesNoteUniqueID;

    window.open(_url, '_blank');
}
// End JSL 07/01/2022

// JSL 06/30/2022
function refreshDataSourceWithoutResetGrid(dataSource) {
    var dataSource = new kendo.data.DataSource({
        data: dataSource
    });

    var grid = $("#notificationGrid").data("kendoGrid");
    grid.setDataSource(dataSource);
}
// End JSL 06/30/2022

function toolbar_click() {
    kendo.alert("Toolbar command is clicked!");
    return false;
}

// JSL 06/29/2022
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
// JSL 06/29/2022

// JSL 06/29/2022
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
// End JSL 06/29/2022

// JSL 06/29/2022
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
// End JSL 06/29/2022

// JSL 06/29/2022
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
// End JSL 06/29/2022

// JSL 07/09/2022
function LoadAssignedToMeGISIIADeficiencies() {
    var url = RootUrl + 'Notifications/LoadAssignedToMeGISIIADeficiencies';
    $.ajax({
        type: 'GET',
        async: false,
        url: url,
        success: function (Data) {
            $('#AssignMeGridList').empty();
            var grid = $('#AssignMeGridList').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                noRecords: true,
                selectable: true,
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
                    $("#AssignMeGridList .k-grid-content").css("min-height", "180px");
                    BindToolTipsForGridText("AssignMeGridList", "Deficiency", 120);
                },
                change: function (e) {
                    var row = this.select();
                    var deficienciesNoteUniqueID = row[0].cells[0].textContent;
                    var ReportType = row[0].cells[2].textContent;
                    openDeficiencyDetails(deficienciesNoteUniqueID, ReportType);
                },
                columns: [
                    {
                        field: "DeficienciesUniqueID",
                        title: "DeficienciesUniqueID",
                        width: "150px",
                        hidden: true,
                    },
                    {
                        field: "ShipName",
                        title: "Ship",
                        width: "150px"
                    },
                    {
                        field: "ReportType",
                        title: "ReportType",
                        width: "150px",
                    },
                    {
                        field: "Deficiency",
                        title: "Deficiency",
                        attributes: { class: "tooltipText" },   // JSL 07/12/2022
                    },
                    {
                        field: "UpdatedDate",
                        title: "Date Time",
                        template: "#= UpdatedDate!=null? kendo.toString(kendo.parseDate(UpdatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy h:mm'):'' #",
                        width: "130px"
                    }
                ]
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End JSL 07/09/2022