var notificationsItems; //RDBJ 10/18/2021
var notTblReport; //RDBJ 10/18/2021

//RDBJ 10/14/2021 Added
$(document).ready(function () {
    //autoRefresh(); //RDBJ 10/21/2021
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

    LoadAssignedToMeGISIIADeficiencies(); // RDBJ 12/21/2021
});

//RDBJ 10/14/2021
function openDeficiencyDetails(deficienciesNoteUniqueID, ReportType) {  //RDBJ 10/21/2021 Added ReportType
    var _url = "";
     //RDBJ 10/21/2021 Wrapped in If
    if (ReportType == "GI" || ReportType == "SI")
        _url = RootUrl + "GIRList/DeficienciesDetails?id=" + deficienciesNoteUniqueID;
    else
        _url = RootUrl + "GIRList/InternalAuditDetails?id=" + deficienciesNoteUniqueID; //RDBJ 10/21/2021

    window.open(_url, '_blank');
}
//End RDBJ 10/14/2021

//RDBJ 10/16/2021
function autoRefresh() {
    var intervalId = window.setInterval(function () {
        getNotifications();
        LoadAssignedToMeGISIIADeficiencies();
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
            notificationsItems = res.GISINotifications; //RDBJ 10/18/2021
            bindNotificationsData(notificationsItems); //RDBJ 10/18/2021
            $("#tblNotifications th").removeClass("deficiencyTextEllipsis");  //RDBJ 10/21/2021
            $("#tblNotifications th").removeClass("shipTextEllipsis"); //RDBJ 10/21/2021
            $('#tblNotifications').DataTable().clear().rows.add(notificationsItems).draw(false); // RDBJ 12/18/2021 set draw(false) //RDBJ 10/22/2021 Auto refresh
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
        "order": [[8, "desc"]], // RDBJ 12/18/2021 set UpdatedDate Descending
        retrieve: true,
        "language": {
            "lengthMenu": "Show _MENU_ Notifications",
            "emptyTable": "Notifications Not Availabe!"
        },
        //"columnDefs" : [{"targets":8, "type":"dd/MM/yyyy"}],
        //columnDefs: [{
        //    target: 8, //index of column
        //    type: 'datetime-moment'
        //}],
        //"columnDefs": [
        //    //non-date fields
        //    {
        //        "searchable": true,
        //        "orderable": true,
        //        "targets": [1, 2, 3, 4, 5, 6, 7],
        //        "type": 'natural'
        //    },
        //    //date-fields
        //    {
        //        "searchable": true,
        //        "orderable": true,
        //        "targets": 8,
        //        "type": 'date'
        //    }
        //],
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
            // RDBJ 12/21/2021
            {
                "data": "AssignTo",
                title: "Assign To",
                width: "15%",
                className: "textCenter",
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
                type: "date", // RDBJ 01/07/2022 commented this line due to sorting not working
                "render": function (value) {
                    if (value === null) return "";
                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));
                    return moment(dt).fromNow(); //(dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
                },
                "orderable": true,
            }
        ]
    });
};
//End RDBJ 10/18/2021

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
                    //BindToolTipsForGridText("AssignMeGridList", "Deficiency", 100);   // RDBJ 02/08/2022
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
                        //attributes: { class: "tooltipText" },   // RDBJ 02/08/2022
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