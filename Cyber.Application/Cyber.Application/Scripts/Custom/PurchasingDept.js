
function LoadPurchasingDeptReportGrid() {
    var POyear = $("#ddlPoYear").val();
    var month = $("#ddlReportPeriod").val();
    var FleetId = $("#ddlReportFleet").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "PurchasingDept/GetAllPurchasingDeptData",
        data: JSON.stringify({
            // shipCode: shipCode,
            POYear: POyear,
            POMonth: month,
            FleetId: FleetId
        }),
        success: function (response) {
            if (response != null) {
                debugger;
                //response = JSON.parse(response);
                $('#grid').empty();
                $('#grid').kendoGrid({
                    scrollable: true,
                    filterable: true,
                    selectable: true,
                    sortable: true,
                    // detailInit: detailInitList,
                    pageable: {
                        alwaysVisible: true,
                        pageSizes: [5, 10, 20, 100]
                    },
                    dataSource: {
                        data: response,
                        pageSize: 10
                    },
                    dataBound: function () {
                        for (var i = 0; i < this.columns.length; i++) {
                            this.autoFitColumn(i);
                        }
                    },
                    columns: [
                        {
                            field: "ID",
                            hidden: true
                        },
                        {
                            field: "SiteName",
                            title: "Ship"
                        },
                        {
                            field: "Descr",
                            title: "PONumber"
                        },
                        {
                            field: "El1",
                            title: "El1"
                        },
                        {
                            field: "ValueDoc",
                            title: "Value"
                        },
                        {
                            field: "POTotal",
                            title: "PO Total"
                        },
                        {
                            field: "CmpCode",
                            title: "Company Code"
                        },
                        {
                            field: "Vendor_Addr_Name",
                            title: "Vender Name"
                        },
                        {
                            field: "Account_Code",
                            title: "Account_Code"
                        },
                        {
                            field: "Account_Descr",
                            title: "Account_Descr"
                        },
                        {
                            field: "Equip_Name",
                            title: "Equipment Name"
                        },
                        {
                            field: "DocCode",
                            title: "Coda Document"
                        },
                        {
                            field: "DocNum",
                            title: "Coda Number"
                        },
                        {
                            field: "ModDate",
                            title: "ModDate",
                            template: "#= ModDate!=null? kendo.toString(kendo.parseDate(ModDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        }
                    ]
                });
            }
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function PrintReport() {
    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    var htmlStart =
        '<!DOCTYPE html>' +
        '<html>' +
        '<head>' +
        '<link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.common.min.css" />' +
        '<link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.rtl.min.css" />' +
        '<link rel = "stylesheet" href = "https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.silver.min.css" /> ' +
        '<link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.mobile.all.min.css" /> ' +
        '<style>.k-hierarchy-cell, .k-grid-filter{display:none;}</style>' +
        '</head>' +
        '<body onload="window.print()">';

    var htmlEnd =
        '</body>' +
        '</html>';
    var gridElement = $('#grid'), printableContent = '';
    var gridHeader = gridElement.children('.k-grid-header');
    if (gridHeader[0]) {
        var thead = gridHeader.find('thead').clone().addClass('k-grid-header');
        printableContent = gridElement
            .clone()
            .children('.k-grid-header').remove()
            .end()
            .children('.k-grid-content')
            .find('table')
            .first()
            .children('tbody').before(thead)
            .end()
            .end()
            .end()
            .end()[0].innerHTML;
    } else {
        printableContent = gridElement.clone()[0].innerHTML;
    }

    newWin.document.write(htmlStart);
    newWin.document.write(printableContent);
    newWin.document.write(htmlEnd);
    newWin.document.close();
    setTimeout(function () { newWin.close(); }, 5);
}

function ExportFile() {
    var yearValue = $("#ddlPoYear").val();
    var monthvalue = $("#ddlReportPeriod").val();
    var FleetId = $("#ddlReportFleet").val();
    window.location = RootUrl + "PurchasingDept/DownloadPurchasingDept?month=" + monthvalue + "&year=" + yearValue + "&fleetId=" + FleetId;
}