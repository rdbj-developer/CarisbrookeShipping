var shipCode;
$(document).ready(function () {
    shipCode = $("#ddlGIReportShipName").val();
    if (isInspector.toLowerCase() == 'false') {
        $("#ddlGIReportShipName").prop('disabled', true);
    } else {
        $("#ddlGIReportShipName").prop('disabled', false);
    }
    loadGrid(shipCode, 'GI');
    $('#GIGrid tbody tr:first-child').trigger('click');
});

function loadDetailGridByShip(ship) {
    var type = "";
    var activeTab = $("ul#custom-content-below-tab li.active a")[0].id
    if (activeTab.includes("gi")) {
        type = "GI";
    } else if (activeTab.includes("si")) { // RDBJ 01/31/2022 added if
        type = "SI";
    }
    // RDBJ 01/31/2022 added else
    else {
        type = "IA";
    }

    loadGrid(ship.value, type);
    $('#' + type + 'DetailsGrid').empty();
    loadDetailGrid(type);
    $('#' + type + 'Grid tbody tr:first-child').trigger('click');
}

function tabChangeLoadGridData(type) {
    shipCode = $("#ddlGIReportShipName").val();//$("#ddlGIReportShipName option:selected").text();
    loadGrid(shipCode, type);
}

function generateTemplateGISIDeleteButton(GISIIAFormID, type) {
    var template = '<div style="padding:5px;line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                '<a class="btn btn-danger" onclick="DeleteGISIDrafts(\'' + GISIIAFormID + '\', \''+ type + '\', this)"> Delete </a>' +
            '</div>';
    return template;
}

function DeleteGISIDrafts(GISIIAFormID, type, ctr) {
    var confirm_result = confirm("Are you sure you want to delete this record?");
    if (confirm_result == true) {
        var url = RootUrl + "Drafts/DeleteGISIIADrafts";//?formId=" + GISIFormID + "&type=" + type;;
        $.ajax({
            url: url,
            data: { GISIIAFormID: GISIIAFormID, type: type },
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

function loadGrid(ship, type) {
    if (ship != null && ship != "" && ship != undefined) {
        //var url = RootUrl + "Drafts/Get" + type + "RDrafts?ship=" + ship; // RDBJ 01/31/2022 Commented this line
        var url = RootUrl + "Drafts/GetGISIIADraftsReportsByShip";   // RDBJ 01/31/2022

        var FormID, AudOrSupr, GPorMaster;
        if (type == "GI") {
            FormID = "UniqueFormID";
            AudOrSupr = "Auditor";
            GPorMaster = "GeneralPreamble";
        } else if (type == "SI") {  // RDBJ 01/31/2022 added if
            FormID = "UniqueFormID";
            AudOrSupr = "Superintended";
            GPorMaster = "Master";
        }
        // RDBJ 01/31/2022 added else
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
            data: { code: ship, type: type },   // RDBJ 01/31/2022
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
                        var id = row[0].cells[0].textContent;
                        var _url = RootUrl + "Drafts/" + type + "RformData?id=" + id;
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
                            template: "#= kendo.toString(kendo.parseDate(UpdatedDate, 'yyyy-MM-dd hh:mm:ss'), 'dd-MM-yyyy HH:mm:ss') #",    // JSL 07/06/2022 set HH instead of hh
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