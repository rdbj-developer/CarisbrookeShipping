$(document).ready(function () {
    LoadAllShipPCName();
    //setInterval(function () {
    //    LoadAllShipPCName();
    //}, 30000); //30000
});
// var shipCode = $("#ddlShip").val();

function LoadAllShipPCName() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "Admin/GetAllSiteInfo",
        data: JSON.stringify({
            // shipCode: shipCode,
            shipName: $("#ddlShip option:selected").text()
        }),
        success: function (response) {
            if (response != null) {
                $('#grid').empty();
                $('#grid').kendoGrid({
                    scrollable: true,
                    filterable: true,
                    //selectable: true,
                    sortable: true,
                    //persistSelection: true,
                    resizable: true,
                    toolbar: [
                        { template: kendo.template($("#template").html()) }
                    ],
                    pageable: {
                        alwaysVisible: true,
                        pageSizes: [5, 10, 20, 100]
                    },
                    dataSource: {
                        data: response,
                        pageSize: 10
                    },
                    columns: [
                        { selectable: true, locked: true, width: 150},
                        {
                            field: "Id",
                            title: "Id",
                            hidden: true,
                            locked: true,
                            attributes: {
                                "class": "clsId"
                            }
                        },
                        {
                            field: "PCUniqueId",
                            title: "PCUniqueId",
                            hidden: true,
                            locked:true,
                            attributes: {
                                "class": "clsPCId"
                               
                            }
                        },
                        {
                            field: "OFSRunningVersion",
                            title: "OFSRunningVersion",
                            hidden: true
                        },
                        {
                            field: "MSSRunningVersion",
                            title: "MSSRunningVersion",
                            hidden: true
                        },
                        {
                            field: "Name",
                            title: "Ship Name",
                            width: "110px",
                            locked: true,
                        },
                        {
                            field: "ShipCode",
                            title: "Ship Code",
                            width: "110px",
                            locked: true,
                            attributes: {
                                "class": "clsShipCode"
                            }
                        },
                        {
                            field: "PCName",
                            title: "PC Name",
                            locked: true,
                            attributes: {
                                "class": "clsPCName"
                            }
                        },
                        {
                            field: "EventDate",
                            title: "Event Date",
                            template: "#= EventDate!=null? kendo.toString(kendo.parseDate(EventDate, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm'):'' #"
                        },
                        {
                            field: "DownloadDate",
                            title: "Download Date",
                            template: "#= DownloadDate!=null? kendo.toString(kendo.parseDate(DownloadDate, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm'):'' #"
                        },
                        {
                            field: "IsRunningOpenFileService",
                            title: "OpenFileService",
                            template: '#= changeStatusTemplate(IsRunningOpenFileService,EventDate,kendo.toString(kendo.parseDate(OFSLastUpdateDate,"yyyy-MM-dd  HH:mm:ss"), "dd/MM/yyyy HH:mm"),OFSRunningVersion)#'
                            , filterable: false
                            , sortable: false
                        },
                        {
                            field: "IsRunningMainSyncServices",
                            title: "MainSyncService",
                            template: '#= changeStatusTemplate(IsRunningMainSyncServices,DownloadDate,kendo.toString(kendo.parseDate(MSSLastUpdateDate,"yyyy-MM-dd HH:mm:ss"), "dd/MM/yyyy HH:mm"),MSSRunningVersion)#'
                            , filterable: false
                            , sortable: false
                        },
                        {
                            field: "OSName",
                            title: "Operating System"
                        },
                        {
                            field: "OSVersion",
                            title: "OS Version"
                        },
                        {
                            field: "OSArchitecture",
                            title: "OSArchitecture"
                        },
                        {
                            field: "ProductID",
                            title: "Product ID"
                        },
                        {
                            field: "UpdatePatch",
                            title: "Update Patch"
                        },
                        {
                            field: "UpdateInstallDate",
                            title: "Update Install Date"
                        },
                        {
                            field: "IsBlocked",
                            title: " ",
                            template: "#= changeActionTemplate(IsBlocked)#"
                            , filterable: false
                            , sortable: false
                        },
                        {
                            field: "IsMainPC",
                            title: "IsMainPC",
                            hidden: true
                        },
                        
                    ],
                    dataBound: function () {
                        for (var i = 0; i < this.columns.length; i++) {
                            this.autoFitColumn(i);
                        }
                        var data = this.dataSource.data();
                        $.each(data, function (i, row) {
                            if (row.IsMainPC != null && row.IsMainPC == true) {
                                // grid.select(row);
                                //$('tr[data-uid="' + row.uid + '"] ').css("background-color", "#f6eec7");
                                $('tr[data-uid="' + row.uid + '"] ').find(".k-checkbox").prop("checked", true);
                                $('tr[data-uid="' + row.uid + '"] ').toggleClass("k-state-selected");

                            }
                        });
                        $('[data-toggle="tooltip"]').tooltip();
                    }                    
                });
                $(".k-grid-refresh").after("<span style='background-color:#1984c8;width:15px;height:15px;margin-left: 15px;border: 1px solid'> &nbsp;&nbsp;&nbsp;&nbsp</span> - Main PC for Ship");

                //var grid = $("#grid").data("kendoGrid");
                //var columns = grid.columns;

                //var hiddenFields = [];
                //for (var i = 0; i < columns.length; ++i) {
                //    if (columns[i].hidden) {
                //        console.log(columns[i].attributes);
                //        hiddenFields.push(columns[i].field);
                        
                //    }
                //}
                //console.log(hiddenFields);
            }
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function changeActionTemplate(IsBlocked) {
    var strTEmplate = "<div style='text-align:center;'>";
    if (IsBlocked) {
        strTEmplate += "&nbsp;&nbsp;&nbsp;&nbsp;<button class='remove btn btn-primary btn-sm delete' style='padding: 5px 5px;'>Delete Logs</button>&nbsp;&nbsp;&nbsp;<button class='remove btn btn-warning btn-sm unblock' style='padding: 5px 5px;'>UnBlock</button>";
    }
    else
        strTEmplate += "<button class='remove btn btn-primary btn-sm delete' style='padding: 5px 5px;'>Delete Logs</button>&nbsp;&nbsp;&nbsp;<button class='remove btn btn-danger btn-sm block' style='padding: 5px 5px;'>Block</button>";
    strTEmplate += "</div>";
    return strTEmplate;
}

function changeStatusTemplate(isRunning, downloadDate, lastupdatedate, runningVersion) {
    var strToolTip = "";
    if (lastupdatedate != null) {
        strToolTip = " since " + lastupdatedate;
    }
    var strTemplate = '<div style="text-align:center;">';
    if (isRunning)
        strTemplate += '<small data-placement="left" data-toggle="tooltip" title="Running ' + strToolTip + '" class="label label-success clsLable"><i class="fa fa-spinner fa-spin"></i> ' + runningVersion + ' Running</small>';
    else {
        if ((downloadDate != null && downloadDate != undefined) && (lastupdatedate == null)) {
            strTemplate += '<small data-placement="left" data-toggle="tooltip" title="Old version is running" class="label label-warning clsLable"><i class="fa fa-spinner fa-spin"></i> 1.0 Running</small>';
        }
        else {
            if (strToolTip != "") {
                strToolTip = 'data-placement="left" data-toggle="tooltip" title="Stop ' + strToolTip + '"';
            }
            else {
                strToolTip = 'data-placement="left" data-toggle="tooltip" title="Please install latest version of this service on this Ship"';
            }
            strTemplate += '<small ' + strToolTip + ' class="label label-danger clsLable"><i style="font-size:10px" class="fa fa-pause"></i> Stop</small>';
        }
    }
    strTemplate += '</div>';
    return strTemplate;
}
$("#grid").on("click", "button.remove", function (e) {
    e.preventDefault();
    var ShipCode = "";
    var Id, PCUniqueId, PCName = "";
    var row = $("#grid")
    Id = $($(row).find('td.clsId')[0]).text();
    PCUniqueId = $($(row).find('td.clsPCId')).text();
    ShipCode = $($(row).find('td.clsShipCode')[0]).text().trim();
    PCName = $($(row).find('td.clsPCName')).text().trim();    
    if ($(e.target).hasClass("block") || $(e.target).hasClass("unblock")) {
        var isBlockPC = false;
        if ($(e.target).hasClass("block")) {
            isBlockPC = true;
        }
        var kendoWindow = $("<div />").kendoWindow({
            title: "Block/UnBlock PC Confirmation",
            resizable: false,
            modal: true
        });
        kendoWindow.data("kendoWindow")
            .content($("#blockpc-confirmation").html())
            .center().open();

        kendoWindow.find(".blockpc-confirm,.blockpc-cancel")
            .click(function () {
                if ($(this).hasClass("blockpc-confirm")) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        headers: {
                            "cache-control": "no-cache"
                        },
                        url: RootUrl + "Admin/UpdateBlockPCRecords",
                        data: JSON.stringify({
                            ShipCode: ShipCode, Id: Id, PCName: PCName, PCUniqueId: PCUniqueId, "IsBlocked": isBlockPC
                        }),
                        success: function (response) {
                            if (response.result == "Success") {
                                $.notify("Record Blocked/UnBlocked Successfully !!!", "success");
                                LoadAllShipPCName();
                            }
                            else {
                                $.notify("Failed to Block/UnBlock Record!!!", "error");
                            }
                        },
                        error: function (response) {

                        }
                    });
                }
                kendoWindow.data("kendoWindow").close();
            })
            .end()
    }
    else {
        var kendoWindow = $("<div />").kendoWindow({
            title: "Delete Logs Confirmation",
            resizable: false,
            modal: true
        });
        kendoWindow.data("kendoWindow")
            .content($("#delete-confirmation").html())
            .center().open();

        kendoWindow.find(".delete-confirm,.delete-cancel")
            .click(function () {
                if ($(this).hasClass("delete-confirm")) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        headers: {
                            "cache-control": "no-cache"
                        },
                        url: RootUrl + "Admin/GetAllDeletePCRecord",
                        data: JSON.stringify({
                            ShipCode: ShipCode, Id: Id, PCName: PCName, PCUniqueId: PCUniqueId
                        }),
                        success: function (response) {
                            if (response.result == "Success") {
                                $.notify("Record Deleted Successfully !!!", "success");
                                LoadAllShipPCName();
                            }
                            else {
                                $.notify("Failed to Record Delete !!!", "error");
                            }
                        },
                        error: function (response) {

                        }
                    });
                }
                kendoWindow.data("kendoWindow").close();
            })
            .end()
    }
});
$("#grid").on("click", ".k-checkbox", function (e) {
    var row = $("#grid");
    var Id, PCUniqueId, PCName = "";
    Id = $($(row).find('td.clsId')[0]).text();
    PCUniqueId = $($(row).find('td.clsPCId')).text();
    ShipCode = $($(row).find('td.clsShipCode')[0]).text().trim();
    PCName = $($(row).find('td.clsPCName')).text().trim();
    var IsMainPC = this.checked;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "Admin/UpdateMainPCRecords",
        data: JSON.stringify({
            ShipCode: ShipCode, Id: Id, PCName: PCName, PCUniqueId: PCUniqueId, "IsMainPC": IsMainPC
        }),
        success: function (response) {
            if (response.result == "Success") {
                $.notify("Record Marked as Main PC Successfully !!!", "success");
                LoadAllShipPCName();
            }
            else {
                $.notify("Failed to Mark Record!!!", "error");
            }
        },
        error: function (response) {

        }
    });
    //var kendoWindow = $("<div />").kendoWindow({
    //    title: "Main PC mark Confirmation",
    //    resizable: false,
    //    modal: true
    //});
    //kendoWindow.data("kendoWindow")
    //    .content($("#mainpc-confirmation").html())
    //    .center().open();

    //kendoWindow.find(".mainpc-confirm,.mainpc-cancel")
    //    .click(function () {
    //        if ($(this).hasClass("mainpc-confirm")) {
    //            $.ajax({
    //                type: "POST",
    //                contentType: "application/json; charset=utf-8",
    //                dataType: "json",
    //                headers: {
    //                    "cache-control": "no-cache"
    //                },
    //                url: RootUrl + "Admin/UpdateMainPCRecords",
    //                data: JSON.stringify({
    //                    ShipCode: ShipCode, Id: Id, PCName: PCName, PCUniqueId: PCUniqueId, "IsMainPC": IsMainPC
    //                }),
    //                success: function (response) {
    //                    if (response.result == "Success") {
    //                        $.notify("Record Marked as Main PC Successfully !!!", "success");
    //                        LoadAllShipPCName();
    //                    }
    //                    else {
    //                        $.notify("Failed to Mark Record!!!", "error");
    //                    }
    //                },
    //                error: function (response) {

    //                }
    //            });
    //        }
    //        else {
    //            e.preventDefault();
    //        }
    //        kendoWindow.data("kendoWindow").close();
    //    })
    //    .end()
});