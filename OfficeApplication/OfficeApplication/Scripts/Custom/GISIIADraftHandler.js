//RDBJ 12/03/2021 Added Script

var shipCode;
$(document).ready(function () {
    shipCode = $("#ddlGIReportShipName").val();
    loadGrid(shipCode, 'GI');
});

// RDBJ 12/03/2021
function loadDetailGridByShip(ship) {
    var type = "";
    var activeTab = $("ul#custom-content-below-tab li.active a")[0].id
    if (activeTab.includes("gi")) {
        type = "GI";
    } else if (activeTab.includes("si")) { // RDBJ 01/23/2022 added if
        type = "SI";
    }
    // RDBJ 01/23/2022 added else
    else {
        type = "IA";
    }

    loadGrid(ship.value, type);
}
// End RDBJ 12/03/2021

// RDBJ 12/03/2021
function tabChangeLoadGridData(type) {
    shipCode = $("#ddlGIReportShipName").val();
    loadGrid(shipCode, type);
}
// End RDBJ 12/03/2021

// RDBJ 12/03/2021
function generateTemplateGISIDeleteButton(GISIIAFormID, type) {
    var template = '<div style="padding:5px;line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
        '<a class="btn btn-danger" onclick="DeleteGISIIADrafts(\'' + GISIIAFormID + '\', \'' + type + '\', this)" style="padding: 0px 16px !important"> Delete </a>' +  // RDBJ 01/05/2022 Set style
        '</div>';
    return template;
}
// End RDBJ 12/03/2021

// RDBJ 12/03/2021
function DeleteGISIIADrafts(GISIFormID, type, ctr) {
    //$('#deleteModal').modal('show');
    //$('#deleteButton').html('<a class="btn btn-danger" onclick="ConfirmDeleteGISIIADrafts(\'' + GISIFormID + '\', \'' + type + '\', \'' + ctr +'\')">Delete</a>');

    var confirm_result = confirm("Are you sure you want to delete this record?");
    if (confirm_result == true) {
        var url = RootUrl + "Drafts/DeleteGISIIADrafts";//?formId=" + GISIFormID + "&type=" + type;;
        $.ajax({
            url: url,
            data: { GISIFormID: GISIFormID, type: type },
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            type: "GET",
            success: function () {
                $.notify(type + " Draft Deleted Successfully.", "success");
                ctr.parentElement.parentElement.parentElement.remove();
                //window.location = RootUrl + "Drafts/";
            }
        });
    }   
}
// End RDBJ 12/03/2021

// RDBJ 12/03/2021
function ConfirmDeleteGISIIADrafts(GISIFormID, type, ctr) {
    var url = RootUrl + "Drafts/DeleteGISIIADrafts";
    $.ajax({
        url: url,
        data: { GISIFormID: GISIFormID, type: type },
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        type: "GET",
        success: function () {
            $.notify(type + " Draft Deleted Successfully.", "success");
            ctr.parentElement.parentElement.parentElement.remove();
            $('#deleteModal').modal('hide');
            //window.location = RootUrl + "Drafts/";
        }
    });
}
// End RDBJ 12/03/2021

// RDBJ 12/03/2021
function loadGrid(ship, type) {
    if (ship != null && ship != "" && ship != undefined) {
        var url = RootUrl + "Forms/GetGISIDraftsReportsByShip";

        var FormID, AudOrSupr, GPorMaster;
        if (type == "GI") {
            FormID = "UniqueFormID";
            AudOrSupr = "Auditor";
            GPorMaster = "GeneralPreamble";
        } else if (type == "SI") {  // RDBJ 01/23/2022 added if
            FormID = "UniqueFormID";
            AudOrSupr = "Superintended";
            GPorMaster = "Master";
        }
        // RDBJ 01/23/2022 added else
        else {
            FormID = "UniqueFormID";
            AudOrSupr = "Auditor";
            GPorMaster = "Type";
        }

        $.ajax({
            type: 'GET',
            dataType: 'json',
            async: false,
            url: url,
            data: { code: ship, type: type },
            success: function (Data) {
                data = Data;
                $('#' + type + 'RDraftsGrid').empty();
                var grid = $('#' + type + 'RDraftsGrid').kendoGrid({
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
                    change: function () {
                        var row = this.select();
                        var FormID = row[0].cells[0].textContent;
                        debugger;
                        var _url;
                        if (type == "GI")
                            _url = RootUrl + "GIRList/Index?id=" + FormID;
                        else if (type == "SI") {
                            _url = RootUrl + "SIRList/DetailsView?id=" + FormID;
                        }
                        else {
                            _url = RootUrl + "IAFList/DetailsView?id=" + FormID;
                        }
                        window.open(_url, '_blank');
                    },
                    dataSource: {
                        data: data
                    },
                    dataBound: function () {
                        $('#' + type + 'RDraftsGrid .k-grid-content').css("min-height", "220px"); //Superintended
                        if (type == "GI") {
                            $('#' + type + 'RDraftsGrid thead [data-field=Auditor] .k-link').html("Auditor");
                            $('#' + type + 'RDraftsGrid thead [data-field=GeneralPreamble] .k-link').html("General Preamble");
                        } else {
                            $('#' + type + 'RDraftsGrid thead [data-field=Superintended] .k-link').html("Superintendent");
                            $('#' + type + 'RDraftsGrid thead [data-field=Master] .k-link').html("Master");
                        }
                        for (var i = 0; i < this.columns.length; i++) {
                        }
                    },

                    columns: [
                        {
                            field: FormID,
                            title: "GIRFormID",
                            hidden: true,
                        },
                        {
                            field: "Ship",
                            title: "Ship",
                            hidden: true,
                        },
                        {
                            field: "ShipName",
                            title: "Ship",
                            width: "70px",
                        },
                        {
                            field: AudOrSupr,
                            title: "Auditor",
                            width: "140px",
                        },
                        {
                            field: "Location",
                            title: "Location",
                            width: "160px",
                        },
                        {
                            field: "Date",
                            title: "Date",
                            width: "60px",
                        },
                        {
                            field: GPorMaster,
                            title: "General Preamble",
                            width: "150px",
                        },
                        {
                            field: "UpdatedDate",
                            title: "Last Edited",
                            width: "90px",
                            template: "#= kendo.toString(kendo.parseDate(UpdatedDate, 'yyyy-MM-dd hh:mm:ss'), 'dd-MM-yyyy hh:mm:ss') #",
                        },
                        {
                            field: FormID,
                            title: "Delete",
                            filterable: false,
                            template: "#=generateTemplateGISIDeleteButton(" + FormID + ", '" + type + "')#",
                            width: "65px"
                        },
                    ]
                });
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
    else {
        alert("Ship Not Available");
    }
}
// End RDBJ 12/03/2021