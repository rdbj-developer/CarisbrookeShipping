var detailExportPromises = [];
var dataSourceSub
var dataSourceSubDetails
var dataSourceSubExport
var dataSourceSubDeailsExport
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

function exportChildData(code, rowIndex) {

    var url = RootUrl + 'Reports/HierarchyBinding_Details/';
    var id = code;
    if (dataSourceSubExport == "" || dataSourceSubExport == undefined) {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,

            async: false,
            success: function (Data) {
                dataSourceSubExport = new kendo.data.DataSource({
                    data: Data
                });
            }
        });
    }
    dataSourceSubExport.read();
    var deferred = $.Deferred();
    detailExportPromises.push(deferred);
    var obj = [];
    for (var i = 0; i < dataSourceSubExport._data.length; i++) {
        if (dataSourceSubExport._data[i].Code == code) {
            item = {}
            item.ACCOUNT_CODE = dataSourceSubExport._data[i].ACCOUNT_CODE;
            item.ACCOUNT_DESCR = dataSourceSubExport._data[i].ACCOUNT_DESCR;
            item.Total = dataSourceSubExport._data[i].Total;
            obj.push(item);
        }
    }
    var rows = [{
        cells: [
            { value: "ACCOUNT_CODE" },
            { value: "ACCOUNT_DESCR" },
            { value: "Total" }
        ]
    }];
    var exporter = new kendo.ExcelExporter({
        columns: [{
            field: "ACCOUNT_CODE"
        }, {
            field: "ACCOUNT_DESCR"
        }, {
            field: "Total"
        }],
        dataSource: { data: obj }
    });

    exporter.workbook().then(function (book, data) {
        deferred.resolve({
            masterRowIndex: rowIndex,
            sheet: book.sheets[0]
        });
    });
    for (var i = 0; i < obj.length; i++) {
        exportChildDataList(obj[i].ACCOUNT_CODE, rowIndex + 1)
    }
}
function exportChildDataList(code, rowIndex) {
    var dataSource;
    var url = RootUrl + 'Reports/HierarchyBinding_DetailsList/';
    var id = code;
    if (dataSourceSubDeailsExport == "" || dataSourceSubDeailsExport == undefined) {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,
            async: false,
            success: function (Data) {
                dataSourceSubDeailsExport = new kendo.data.DataSource({
                    data: Data
                });
            }
        });
    }
    dataSourceSubDeailsExport.read();
    var deferred = $.Deferred();
    var obj = [];
    for (var i = 0; i < dataSourceSubDeailsExport._data.length; i++) {
        if (dataSourceSubDeailsExport._data[i].AccountCode == id) {
            item = {}
            item.AccountDescription = dataSourceSubDeailsExport._data[i].AccountDescription;
            item.PONO = dataSourceSubDeailsExport._data[i].PONO;
            item.POTITLE = dataSourceSubDeailsExport._data[i].POTITLE;
            item.SupplierName = dataSourceSubDeailsExport._data[i].SupplierName;
            item.InvoiceSent = dataSourceSubDeailsExport._data[i].InvoiceSent;
            item.POTOTALBASECURRENCY = dataSourceSubDeailsExport._data[i].POTOTALBASECURRENCY;
            item.POTOTAL = dataSourceSubDeailsExport._data[i].POTOTAL;
            item.POEXCHRATE = dataSourceSubDeailsExport._data[i].POEXCHRATE;
            item.INVOICENo = dataSourceSubDeailsExport._data[i].INVOICENo;
            item.INVOICEAMOUNTBASECURRENCY = dataSourceSubDeailsExport._data[i].INVOICEAMOUNTBASECURRENCY;
            item.INVOICEAMOUNT = dataSourceSubDeailsExport._data[i].INVOICEAMOUNT;
            item.NETINVOICEAMOUNT = dataSourceSubDeailsExport._data[i].NETINVOICEAMOUNT;
            item.POSTATUS = dataSourceSubDeailsExport._data[i].POSTATUS;
            item.PORECVDATE = dataSourceSubDeailsExport._data[i].PORECVDATE;
            item.FORWARDER_RECVD_DATE = dataSourceSubDeailsExport._data[i].FORWARDER_RECVD_DATE;
            item.PODATE = dataSourceSubDeailsExport._data[i].PODATE;
            item.POCurrency = dataSourceSubDeailsExport._data[i].POCurrency;
            item.INVOICEDATE = dataSourceSubDeailsExport._data[i].INVOICEDATE;
            obj.push(item);
        }
    }
    detailExportPromises.push(deferred);

    var rows = [{
        cells: [
            { value: "ACCOUNT_DESCR" },
            { value: "PONO" },
            { value: "POTITLE" },
            { value: "CURR_CODE" },
            { value: "POSTATUS" },
            { value: "GRANDTOT_BASE" },
            { value: "FINAL_INVOICE_TXT" },
            { value: "NET_INVOICE_AMT_BASE" },
            { value: "PORECVDATE" },
            { value: "FORWARDER_RECVD_DATE" },
            { value: "PODATE" }
        ]
    }];
    var exporter = new kendo.ExcelExporter({
        columns: [{
            field: "AccountDescription"
        }, {
            field: "PONO"
        }, {
            field: "POTITLE"
        }, {
            field: "CURR_CODE"
        }, {
            field: "POSTATUS"
        }, {
            field: "GRANDTOT_BASE"
        }, {
            field: "FINAL_INVOICE_TXT"
        }, {
            field: "NET_INVOICE_AMT_BASE"
        }, {
            field: "PORECVDATE"
        }, {
            field: "FORWARDER_RECVD_DATE"
        }, {
            field: "PODATE"
        }],
        dataSource: { data: obj }
    });

    exporter.workbook().then(function (book, data) {
        deferred.resolve({
            masterRowIndex: rowIndex,
            sheet: book.sheets[0]
        });
    });

}

function LoadArrivalReportGrid() {

    var shipCode = $("#ddlShip").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "Admin/GetAllArrivalReport",
        data: JSON.stringify({
            // shipCode: shipCode,
            shipName: shipCode
        }),
        success: function (response) {
            if (response != null) {
                //response = JSON.parse(response);
                $('#arrivalReportGrid').empty();
                $('#arrivalReportGrid').kendoGrid({
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
                            field: "ShipName",
                            title: "Ship"
                        },
                        {
                            field: "ReportCreated",
                            title: "Report Created",
                            template: "#= ReportCreated!=null? kendo.toString(kendo.parseDate(ReportCreated, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "CreatedBy",
                            title: "Submitted By"
                        },
                        {
                            field: "VoyageNo",
                            title: "Voyage No"
                        },
                        {
                            field: "PortName",
                            title: "Port"
                        },
                        {
                            field: "ArrivalDate",
                            title: "Arrival Date",
                            template: "#= ArrivalDate!=null? kendo.toString(kendo.parseDate(ArrivalDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "ArrivalTime",
                            title: "Arrival Time"
                        },
                        {
                            field: "TenderedDate",
                            title: "Tendered Date",
                            template: "#= TenderedDate!=null? kendo.toString(kendo.parseDate(TenderedDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "TenderedTime",
                            title: "Tendered Time"
                        },
                        {
                            field: "POBDate",
                            title: "POB Date",
                            template: "#= POBDate!=null? kendo.toString(kendo.parseDate(POBDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "POBTime",
                            title: "POB Time"
                        },
                        {
                            field: "TugsNo",
                            title: "No Of Tugs Used"
                        },
                        {
                            field: "chkAnchorOn",
                            title: "OnAnchor"
                        },
                        {
                            field: "ArrivalAlongsideDate",
                            title: "ArrivalAlongSide Date",
                            template: "#= ArrivalAlongsideDate!=null? kendo.toString(kendo.parseDate(ArrivalAlongsideDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "AverageSpeed",
                            title: "Average Speed"
                        },
                        {
                            field: "Distance",
                            title: "Distance Made"
                        },
                        {
                            field: "FuelOil",
                            title: "FuelOil"
                        },
                        {
                            field: "DieselOil",
                            title: "DieselOil"
                        },
                        {
                            field: "SulphurFuelOil",
                            title: "SulphurFuelOil"
                        },
                        {
                            field: "SulphurDieselOil",
                            title: "SulphurDieselOil"
                        },
                        {
                            field: "FreshWater",
                            title: "FreshWater"
                        },
                        {
                            field: "LubeOil",
                            title: "LubeOil"
                        },
                        {
                            field: "CargoDate",
                            title: "Cargo Date",
                            template: "#= CargoDate!=null? kendo.toString(kendo.parseDate(CargoDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "CargoTime",
                            title: "Cargo Time"
                        },
                        {
                            field: "DepartureDate",
                            title: "ETCDeparture Date",
                            template: "#= DepartureDate!=null? kendo.toString(kendo.parseDate(DepartureDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "NextPort",
                            title: "NextPort"
                        },
                        {
                            field: "Remarks",
                            title: "Remarks"
                        },
                        {
                            field: "ToEmail",
                            title: "ToEmail"
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

function LoadDepartureReportGrid() {

    var shipCode = $("#ddlShipDepart").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "Admin/GetAllDepartureReport",
        data: JSON.stringify({
            // shipCode: shipCode,
            shipName: shipCode
        }),
        success: function (response) {
            if (response != null) {
                //response = JSON.parse(response);
                $('#departureReportGrid').empty();
                $('#departureReportGrid').kendoGrid({
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
                            field: "ShipName",
                            title: "Ship"
                        },
                        {
                            field: "ReportCreated",
                            title: "Report Created",
                            template: "#= ReportCreated!=null? kendo.toString(kendo.parseDate(ReportCreated, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "CreatedBy",
                            title: "Submitted By"
                        },
                        {
                            field: "VoyageNo",
                            title: "Voyage No"
                        },
                        {
                            field: "PortName",
                            title: "Port"
                        },
                        {
                            field: "DateCargoOperations",
                            title: "Date Cargo Operations",
                            template: "#= DateCargoOperations!=null? kendo.toString(kendo.parseDate(DateCargoOperations, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "TimeCargoOperations",
                            title: "Time Cargo Operations"
                        },
                        {
                            field: "CargoOnBoard",
                            title: "CargoOnBoard"
                        },
                        {
                            field: "CargoLoaded",
                            title: "CargoLoaded"
                        },
                        {
                            field: "POBDate",
                            title: "POB Date",
                            template: "#= POBDate!=null? kendo.toString(kendo.parseDate(POBDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "POBTime",
                            title: "POB Time"
                        },
                        {
                            field: "DepartureDate",
                            title: "ETCDeparture Date",
                            template: "#= DepartureDate!=null? kendo.toString(kendo.parseDate(DepartureDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "DepartureTime",
                            title: "Departure Time"
                        },
                        {
                            field: "POffDate",
                            title: "POffDate",
                            template: "#= POffDate!=null? kendo.toString(kendo.parseDate(POffDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "POffTime",
                            title: "POffTime"
                        },
                        {
                            field: "NoOfTugs",
                            title: "NoOfTugs"
                        },
                        {
                            field: "FuelOil",
                            title: "FuelOil"
                        },
                        {
                            field: "DieselOil",
                            title: "DieselOil"
                        },
                        {
                            field: "SulphurFuelOil",
                            title: "SulphurFuelOil"
                        },
                        {
                            field: "SulphurDieselOil",
                            title: "SulphurDieselOil"
                        },
                        {
                            field: "NextPort",
                            title: "NextPort"
                        },
                        {
                            field: "ETADate",
                            title: "ETA Date",
                            template: "#= ETADate!=null? kendo.toString(kendo.parseDate(ETADate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "ETATime",
                            title: "ETA Time"
                        },
                        {
                            field: "IntendedRoute",
                            title: "IntendedRoute"
                        },
                        {
                            field: "Remarks",
                            title: "Remarks"
                        },
                        {
                            field: "ToEmail",
                            title: "ToEmail"
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

function LoadDailyCargoReportGrid() {

    var shipCode = $("#ddlShipDailyCargo").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "Admin/GetAllDailyCargoReport",
        data: JSON.stringify({
            // shipCode: shipCode,
            shipName: shipCode
        }),
        success: function (response) {
            if (response != null) {
                //response = JSON.parse(response);
                $('#dailyCargoReportGrid').empty();
                $('#dailyCargoReportGrid').kendoGrid({
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
                            field: "ShipName",
                            title: "Ship"
                        },
                        {
                            field: "ReportCreated",
                            title: "Report Created",
                            template: "#= ReportCreated!=null? kendo.toString(kendo.parseDate(ReportCreated, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "CreatedBy",
                            title: "Submitted By"
                        },
                        {
                            field: "VoyageNo",
                            title: "Voyage No"
                        },
                        {
                            field: "PortName",
                            title: "Port"
                        },
                        {
                            field: "NoOfGangs",
                            title: "NoOfGangs Employed"
                        },
                        {
                            field: "NoOfShips",
                            title: "NoOfShipsCranesInUse"
                        },
                        {
                            field: "QuantityOfCargoLoaded",
                            title: "QuantityOfCargoLoaded"
                        },
                        {
                            field: "TotalCargoLoaded",
                            title: "TotalCargoLoaded"
                        },
                        {
                            field: "CargoRemaining",
                            title: "CargoRemaining"
                        },
                        {
                            field: "FuelOil",
                            title: "FuelOil"
                        },
                        {
                            field: "DieselOil",
                            title: "DieselOil"
                        },
                        {
                            field: "SulphurFuelOil",
                            title: "SulphurFuelOil"
                        },
                        {
                            field: "SulphurDieselOil",
                            title: "SulphurDieselOil"
                        },
                        {
                            field: "Sludge",
                            title: "Sludge"
                        },
                        {
                            field: "DirtyOil",
                            title: "DirtyOil"
                        },
                        {
                            field: "ETCDate",
                            title: "ETC Date",
                            template: "#= ETCDate!=null? kendo.toString(kendo.parseDate(ETCDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "ETCTime",
                            title: "ETC Time"
                        },
                        {
                            field: "NextPort",
                            title: "NextPort"
                        },
                        {
                            field: "Remarks",
                            title: "Remarks"
                        },
                        {
                            field: "ToEmail",
                            title: "ToEmail"
                        },
                        {
                            field: "CargoType",
                            title: "CargoType"
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

function LoadDailyPositionReportGrid() {

    var shipCode = $("#ddlShipPostion").val();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "cache-control": "no-cache"
        },
        url: RootUrl + "Admin/GetAllDailyPositionReport",
        data: JSON.stringify({
            // shipCode: shipCode,
            shipName: shipCode
        }),
        success: function (response) {
            if (response != null) {
                //response = JSON.parse(response);
                $('#dailyPositionReportGrid').empty();
                $('#dailyPositionReportGrid').kendoGrid({
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
                            field: "ShipName",
                            title: "Ship"
                        },
                        {
                            field: "ReportCreated",
                            title: "Report Created",
                            template: "#= ReportCreated!=null? kendo.toString(kendo.parseDate(ReportCreated, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "CreatedBy",
                            title: "Submitted By"
                        },
                        {
                            field: "VoyageNo",
                            title: "Voyage No"
                        },
                        {
                            field: "Latitude",
                            title: "Latitude"
                        },
                        {
                            field: "Longitude",
                            title: "Longitude"
                        },
                        {
                            field: "chkAnchored",
                            title: "OnAnchor",
                            template: '<input type="checkbox" #= chkAnchored ? "checked=checked" : "" # disabled="disabled" ></input>'
                        },
                        {
                            field: "AverageSpeed",
                            title: "Average Speed"
                        },
                        {
                            field: "DistanceMade",
                            title: "Distance Made"
                        },
                        {
                            field: "NextPort",
                            title: "NextPort"
                        },
                        {
                            field: "EstimatedArrivalDateEcoSpeed",
                            title: "EstimatedArrivalDateEcoSpeed",
                            template: "#= EstimatedArrivalDateEcoSpeed!=null? kendo.toString(kendo.parseDate(EstimatedArrivalDateEcoSpeed, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "EstimatedArrivalTimeEcoSpeed",
                            title: "EstimatedArrivalTimeEcoSpeed"
                        },
                        {
                            field: "EstimatedArrivalDateFullSpeed",
                            title: "EstimatedArrivalDateFullSpeed",
                            template: "#= EstimatedArrivalDateFullSpeed!=null? kendo.toString(kendo.parseDate(EstimatedArrivalDateFullSpeed, 'yyyy-MM-dd'), 'dd/MM/yyyy'):'' #"
                        },
                        {
                            field: "EstimatedArrivalTimeFullSpeed",
                            title: "EstimatedArrivalTimeFullSpeed"
                        },
                        {
                            field: "FuelOil",
                            title: "FuelOil"
                        },
                        {
                            field: "DieselOil",
                            title: "DieselOil"
                        },
                        {
                            field: "SulphurFuelOil",
                            title: "SulphurFuelOil"
                        },
                        {
                            field: "SulphurDieselOil",
                            title: "SulphurDieselOil"
                        },
                        {
                            field: "FreshWater",
                            title: "FreshWater"
                        },
                        {
                            field: "LubeOil",
                            title: "LubeOil"
                        },
                        {
                            field: "Sludge",
                            title: "Sludge"
                        },
                        {
                            field: "DirtyOil",
                            title: "DirtyOil"
                        },
                        {
                            field: "Pitch",
                            title: "Pitch"
                        },
                        {
                            field: "EngineLoad",
                            title: "EngineLoad"
                        },
                        {
                            field: "HighCylExhTemp",
                            title: "HighCylExhTemp"
                        },
                        {
                            field: "ExhGasTempAftTurboChrg",
                            title: "ExhGasTempAftTurboChrg"
                        },
                        {
                            field: "OilCunsum",
                            title: "OilCunsum"
                        },
                        {
                            field: "WindDirection",
                            title: "WindDirection"
                        },
                        {
                            field: "WindForce",
                            title: "WindForce"
                        },
                        {
                            field: "SeaState",
                            title: "SeaState"
                        },
                        {
                            field: "SwellDirection",
                            title: "SwellDirection"
                        },
                        {
                            field: "SwellHeight",
                            title: "SwellHeight"
                        },
                        {
                            field: "DraftAft",
                            title: "DraftAft"
                        },
                        {
                            field: "DraftForward",
                            title: "DraftForward"
                        },
                        {
                            field: "Remarks",
                            title: "Remarks"
                        },
                        {
                            field: "ToEmail",
                            title: "ToEmail"
                        },
                        {
                            field: "CargoType",
                            title: "CargoType"
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


