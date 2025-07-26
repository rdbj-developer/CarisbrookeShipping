var ShipAudits;
var AuditDeficiencyComments;
var AuditDeficiencyCommentFiles;
var HideAndShow = false; //RDBJ 11/22/2021
var typeVal = ""; //RDBJ 11/22/2021
var ISMNonConformity, ISPSNonConformity, ISMObservation, ISPSObservation, MLCDeficiency; //RDBJ 11/22/2021
var Numbers; //RDBJ 11/22/2021
var UniqueFormID; //RDBJ 11/23/2021
var NotesUniqueID; //RDBJ 11/25/2021
var rowAuditIndex; //RDBJ 11/24/2021
var IAFFormValidator = undefined; //RDBJ 11/25/2021
var IAFNotesFormValidator = undefined; //RDBJ 11/25/2021
var IAFRowCount; //RDBJ 12/03/2021

// JSL 02/14/2023
var FSTORowCount; 
var AddNewFSTOFormValidator = undefined;
var fileData = new FormData();
var _strFilePath = '../Images/';
// End JSL 02/14/2023

// JSL 02/18/2023
var EditFSTOFormValidator = undefined;
// End JSL 02/18/2023

$(document).ready(function () {

    // RDBJ 01/20/2022
    if (UserGroupID == "1") {
        $("#btnRemoveAudit").css("display", "initial");
        $("#btnRemoveDeficiencies").css("display", "initial");
    }
    else {
        $("#btnRemoveAudit").css("display", "none"); 
        $("#btnRemoveDeficiencies").css("display", "none");
    }
    // End RDBJ 01/20/2022

    var shipCode = $("#ddlGIReportShipName").val();

    // RDBJ 03/01/2022 commented to avoid call from load
    /*
    InitShipAudits(shipCode);
    LoadAuditDetail();
    */
    // End RDBJ 03/01/2022 commented to avoid call from load

    //RDBJ 11/22/2021
    AuditTypeListSelection();
    $("#txtAuditTypeISM").change(function () {
        AuditTypeListSelection();
    });

    $("#txtAuditTypeISPS").change(function () {
        AuditTypeListSelection();
    });

    $("#txtAuditTypeMLC").change(function () {
        AuditTypeListSelection();
    });
    //End RDBJ 11/22/2021

    $(".AuditAutoUpdate").bind("change", function (e) {
        if (HideAndShow) {
            //RDBJ 11/25/2021 Wrapped in validator
            if ($("#IAFForm").valid())
                IAFAutoSave();
        }
    });

    $(".FSTOAutoUpdate").bind("change", function (e) {
        SubmitFSTO();
    });

    ValidateIAFForm(); //RDBJ 11/25/2021
    ValidateIAFNotesFormValidator(); //RDBJ 11/25/2021
    ValidateAddNewFSTOFormValidator();  // JSL 02/14/2023
    ValidateEditFSTOFormValidator();  // JSL 02/18/2023
});

//RDBJ 11/22/2021
$(document).on("click", ".RefeData", function () {
    if ($(".type option:selected").val() == "") {
        //alert("Please select Type and try again!");
        $.notify("Please select Audit Note Type and try again!", "error");
        return;
    }
    $("#ReferenceModal").modal('show');
    childrefe = true;
});
//End RDBJ 11/22/2021

function LoadAuditShips(ship) {
    var url = AuditRootUrl + "Deficiencies/GetAuditShips";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { code: ship },
        success: function (Data) {
            data = Data;
            $('#Grid').empty();
            var grid = $('#Grid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                selectable: true,
                //detailTemplate: kendo.template($("#ReportTemplate").html()),
                //detailInit: InitShipAudits,
                pageable: {
                    alwaysVisible: true,
                    pageSizes: [5, 10, 20, 100]
                },
                dataSource: {
                    data: data,
                    pageSize: 10
                },
                dataBound: function () {
                    for (var i = 0; i < this.columns.length; i++) {
                        // this.autoFitColumn(i);
                    }
                },
                columns: [
                    {
                        field: "IAFId",
                        title: "IAFId",
                        hidden: true,
                    },
                    {
                        field: "Ship",
                        title: "Ship",
                        hidden: true,
                    },
                    {
                        field: "ShipName",
                        title: "ShipName",
                    },
                    {
                        field: "OpenISMNCNs",
                        title: "Open ISM NCNs",
                    },
                    {
                        field: "OpenISMOBS",
                        title: "Open ISM OBS",
                    },
                    {
                        field: "OpenISPSNCN",
                        title: "Open ISPS NCN",
                    },
                    {
                        field: "OpenISPSOBS",
                        title: "Open ISPS OBS",
                    },
                    {
                        field: "OpenMLCNCNs",
                        title: "Open MLC NCNs",
                    },
                    {
                        field: "OpenMLCOBS",
                        title: "Open MLC OBS",
                    }
                ]
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function InitShipAudits(shipCode
    , IsAddedNewAudit = false   // JSL 04/20/2022
) {
    //var detailRow = e.detailRow;
    //var code = e.data.Ship;
    var code = shipCode;
    var url = AuditRootUrl + "Deficiencies/GetShipAudits";
    locadShipAudits(code, url
        , IsAddedNewAudit   // JSL 04/20/2022
    );

    // JSL 12/19/2022
    var blnIsVisitor = '';
    if (_CurrentUserDetailsObject.UserGroup == '8') {
        blnIsVisitor = 'disabled = true';
    }
    // End JSL 12/19/2022

    //detailRow.find(".ShipAuditsList").kendoGrid({
    $('#Grid').empty();
    var grid = $('#Grid').kendoGrid({
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
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
        },
        dataSource: {
            data: ShipAudits.options.data,
            pageSize: 10,
            //sort: { field: "AuditDate", dir: "desc" }
        },
        dataBound: function (e) {
            $("#Grid .k-grid-content").css("min-height", "220px");
            //for (var i = 0; i < this.columns.length; i++) {
            //    this.autoFitColumn(i);
            //}
            setTimeout(function () {
                var gridDataDetail = $('#Grid').data("kendoGrid");
                gridDataDetail.autoFitColumn("Subject");
                gridDataDetail.autoFitColumn("Type");
                gridDataDetail.autoFitColumn("AuditDate");
                gridDataDetail.autoFitColumn("Location");
                gridDataDetail.autoFitColumn("Auditor");
                gridDataDetail.autoFitColumn("NCN");
                gridDataDetail.autoFitColumn("OutstandingNCN");
                gridDataDetail.autoFitColumn("OBS");
                gridDataDetail.autoFitColumn("OutstandingOBS");
                // RDBJ 01/24/2022
                gridDataDetail.autoFitColumn("MLC");
                gridDataDetail.autoFitColumn("OutstandingMLC");
                // End RDBJ 01/24/2022
            }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5

            $('#Grid').on("mousedown", "tr[role='row']", function (e) {
                if (e.which === 3) {
                    $("tr").removeClass("k-state-selected");
                    $(this).toggleClass("k-state-selected");
                    //$(this).trigger('click');
                }
            });
        },
        resizable: true,
        columns: [
            {
                field: "UniqueFormID",
                title: "UniqueFormID",
                hidden: true,
                attributes: { class: "UniqueFormID" },
            },
            {
                field: "InternalAuditFormId",
                title: "InternalAuditFormId",
                hidden: true,
            },
            {
                field: "Subject",
                title: "Audit", // RDBJ 01/24/2022 set Audit from Subject
                width: 70,
                attributes: { class: "mainContext" },
            },
            {
                field: "Type",
                title: "Type",
                width: 70,
                //RDBJ 11/24/2021
                template: function (data) {
                    var auditType;
                    if (data.Type == "1")
                        auditType = "Internal";
                    else if (data.Type == "2")  // RDBJ 04/05/2022 set condition
                        auditType = "External";

                    return auditType;
                },
                //End RDBJ 11/24/2021
                attributes: { class: "mainContext" },
            },
            {
                template: '<input onclick="updateAdditionalAndCloseStatus(this, true)" type="checkbox" #= Extra ? \'checked="checked"\' : "" # class="chkbx" ' + blnIsVisitor + ' />',  // JSL 12/19/2022 added blnIsVisitor
                title: "Extra?",
                width: 60, //80, // RDBJ 01/19/2022 reduce the width
                attributes: { class: "mainContext" },
            },
            {
                field: "AuditDate",
                title: "AuditDate",
                width: 100,
                template: "#= AuditDate!=null? kendo.toString(kendo.parseDate(AuditDate, 'yyyy-MM-dd'), 'dd-MMM-yyyy'):'' #", // RDBJ 04/01/2022
                attributes: { class: "mainContext" },
            },
            {
                field: "Location",
                title: "Location",
                width: 70,
                attributes: { class: "mainContext" },
            },
            {
                field: "Auditor",
                title: "Auditor",
                width: 100,
                attributes: { class: "mainContext" },
            },

            // RDBJ 02/11/2022 re-order columns // RDBJ 01/19/2022 re-order columns
            {
                field: "NCN",
                title: "No. Of NCN's",
                width: 50,
                attributes: { class: "mainContext" },
            },
            {
                field: "OBS",
                title: "No. Of OBS's",
                width: 50,
                attributes: { class: "mainContext" },
            },
            {
                template: '<input onclick="updateAdditionalAndCloseStatus(this, false)" type="checkbox" #= Closed ? \'checked="checked"\' : "" # class="chkbx" ' + blnIsVisitor + ' />', // JSL 12/19/2022 added blnIsVisitor
                title: "Closed?",
                width: 80,
                attributes: { class: "mainContext" },
            },
            {
                field: "OutstandingNCN",
                title: "Open NCN's",
                width: 50,
                attributes: { class: "mainContext" },
            },
            {
                field: "OutstandingOBS",
                title: "Open OBS's",
                width: 50,
                attributes: { class: "mainContext" },
            },
            // End RDBJ 02/11/2022 re-order columns // End RDBJ 01/19/2022 re-order columns
            {
                field: "OutstandingMLC",
                title: "Open MLC's",
                width: 50,
                attributes: { class: "mainContext" },
            },
            {
                field: "MLC",
                title: "No. Of MLC's",
                width: 50,
                attributes: { class: "mainContext" },
            },
            // RDBJ 01/24/2022

            // RDBJ 01/24/2022

            //RDBJ 11/23/2021
            {
                field: "AuditTypeISM",
                title: "AuditTypeISM",
                hidden: true,
            },
            {
                field: "AuditTypeISPS",
                title: "AuditTypeISPS",
                hidden: true,
            },
            {
                field: "AuditTypeMLC",
                title: "AuditTypeMLC",
                hidden: true,
            },
            //End RDBJ 11/23/2021
        ]
    });
    setTimeout(function () {
        var gridDataDetail = $('#Grid').data("kendoGrid");
        gridDataDetail.autoFitColumn("Subject");
        gridDataDetail.autoFitColumn("Type");
        gridDataDetail.autoFitColumn("AuditDate");
        gridDataDetail.autoFitColumn("Location");
        gridDataDetail.autoFitColumn("Auditor");
        gridDataDetail.autoFitColumn("NCN");
        gridDataDetail.autoFitColumn("OutstandingNCN");
        gridDataDetail.autoFitColumn("OBS");
        gridDataDetail.autoFitColumn("OutstandingOBS");
        // RDBJ 01/24/2022
        gridDataDetail.autoFitColumn("MLC");
        gridDataDetail.autoFitColumn("OutstandingMLC");
        // End RDBJ 01/24/2022
    }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5
    BindOpenFormContextMenuByGridId("#Grid");   // JSL 02/14/2023
}
function locadShipAudits(code, url
    , IsAddedNewAudit = false   // JSL 04/20/2022
) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: {
            code: code
            , blnIsAddedNewAudit: IsAddedNewAudit
        },
        success: function (Data) {
            ShipAudits = new kendo.data.DataSource({
                data: Data
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function LoadAuditList() {
    var url = AuditRootUrl + "Deficiencies/GetAuditLists";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            data = Data;
            $('#Grid').empty();
            var grid = $('#Grid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                selectable: true,
                //filterable: true,
                pageable: {
                    alwaysVisible: true,
                    pageSizes: [5, 10, 20, 100]
                },
                dataSource: {
                    data: data,
                    pageSize: 10
                },
                dataBound: function () {
                    for (var i = 0; i < this.columns.length; i++) {
                        // this.autoFitColumn(i);
                    }
                },
                columns: [
                    {
                        field: "InternalAuditFormId",
                        title: "InternalAuditFormId",
                        hidden: true,

                    },
                    {
                        field: "Subject",
                        title: "Subject",
                        width: 70
                    },
                    {
                        field: "Type",
                        title: "Type",
                        width: 70
                    },
                    { template: '<input type="checkbox" #= Extra ? \'checked="checked"\' : "" # class="chkbx" />', title: "Extra?", width: 50 },
                    {
                        field: "AuditDate",
                        title: "AuditDate",
                        width: 100
                    },
                    {
                        field: "Location",
                        title: "Location",
                        width: 70
                    },
                    {
                        field: "Auditor",
                        title: "Auditor",
                        width: 100
                    },
                    {
                        field: "NCN",
                        title: "NCN's",
                        width: 50
                    },
                    {
                        field: "OBS",
                        title: "OBS's",
                        width: 50
                    },
                    { template: '<input type="checkbox" #= Closed ? \'checked="checked"\' : "" # class="chkbx" />', title: "Closed?", width: 50 },
                ]
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function LoadAuditDetail() {
    $("#Grid tbody").on("click", "tr", function (e) {
        var AuditID = e.currentTarget.children[0].textContent
        var Subject = e.currentTarget.children[2].textContent   // RDBJ 02/11/2022 set index 2
        UniqueFormID = e.currentTarget.children[0].textContent // RDBJ 11/23/2021 remove local declare and set global
        rowAuditIndex = parseInt($(this).index(), 10) + 1; //RDBJ 11/24/2021
        HideAndShow = false;
        //RDBJ 11/23/2021
        if (e.currentTarget.children[15].textContent == "false") {  // RDBJ 01/28/2022 set 15 index
            $(".type option:contains('ISM')").hide();
        } else {
            $(".type option:contains('ISM')").show();
        }

        if (e.currentTarget.children[16].textContent == "false") {  // RDBJ 01/28/2022 set 16 index
            $(".type option:contains('ISPS')").hide();
        } else {
            $(".type option:contains('ISPS')").show();
        }

        if (e.currentTarget.children[17].textContent == "false") {  // RDBJ 01/28/2022 set 17 index
            $(".type option:contains('MLC')").hide();
        } else {
            $(".type option:contains('MLC')").show();
        }
        //End RDBJ 11/23/2021

        GetAndSetIAFFormData(UniqueFormID); //RDBJ 11/24/2021

        // JSL 12/19/2022
        var blnIsVisitor = '';
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            blnIsVisitor = 'disabled = true';
        }
        // End JSL 12/19/2022

        var url = AuditRootUrl + "Deficiencies/GetAuditDetails/" + UniqueFormID;
        $.ajax({
            type: 'GET',
            dataType: 'json',
            async: false,
            url: url,
            success: function (Data) {
                data = Data;
                $('#DetailsGrid').empty();
                var grid = $('#DetailsGrid').kendoGrid({
                    scrollable: true,
                    sortable: true,
                    resizable: true,
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
                        $('#DetailsGrid .k-grid-content').css("min-height", "130px");

                        setTimeout(function () {
                            var gridDataDetail = $('#DetailsGrid').data("kendoGrid");
                            gridDataDetail.autoFitColumn("Type");
                            //gridDataDetail.autoFitColumn("Deficiency");
                            //gridDataDetail.autoFitColumn("Reference");
                            gridDataDetail.autoFitColumn("DueDate");
                            gridDataDetail.autoFitColumn("IsResolved");
                            // RDBJ 01/24/2022
                            gridDataDetail.autoFitColumn("AssignTo");
                            gridDataDetail.autoFitColumn("Number");
							// End RDBJ 01/24/2022
                        }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5

                        BindToolTipsForGridText("DetailsGrid", "Deficiency", 45);   // RDBJ 02/08/2022
                        BindToolTipsForGridText("DetailsGrid", "Reference", 45);   // RDBJ 02/11/2022
                    },
                    change: function () {
                        var row = this.select();
                        var FormID = row[0].cells[1].textContent;
                        var defID = row[0].cells[0].textContent;
                        var notesUniqueID = row[0].cells[0].textContent; // RDBJ 12/16/2021 Update index number after added Updated Date column
                        
                        NotesUniqueID = notesUniqueID; //RDBJ 11/25/2021
                        GetAndSetIAFFormNoteData(NotesUniqueID); //RDBJ 11/25/2021

                        if (FormID && FormID != "") {
                            var _url = RootUrl + "GIRList/InternalAuditDetails?id=" + notesUniqueID;
                            window.open(_url, '_blank');
                        }
                    },
                    //detailTemplate: kendo.template($("#CommentsTemplate").html()),
                    //detailInit: InitCommentsTemplate,
                    columns: [
                        {
                            field: "NotesUniqueID",
                            title: "NotesUniqueID",
                            hidden: true,
                            attributes: { class: "NotesUniqueID" } //RDBJ 11/13/2021
                        },
                        {
                            field: "NoteID",
                            title: "NoteID",
                            hidden: true,
                            //attributes: { class: "NoteID" } //RDBJ 11/13/2021 Commented this line
                        },
                        {
                            field: "InternalAuditFormId",
                            title: "InternalAuditFormId",
                            hidden: true,
                            attributes: { class: "InternalAuditFormId" }
                        },
                        // RDBJ 01/24/2022
                        {
                            field: "Number",
                            title: "Number",
                            width: "60px"
                        },
                        // End RDBJ 01/24/2022
                        {
                            field: "Type",
                            title: "Type",
                        },
                        {
                            field: "Deficiency",
                            title: "Brief Description",
                            attributes: { class: "tooltipText" },   // RDBJ 02/08/2022
                        },
                        {
                            field: "Reference",
                            title: Subject == "ISM" ? "SMS References" : Subject == "ISPS" ? "SSP References" : Subject == "MLC" ? "MLC References" : "References",
                            attributes: { class: "tooltipText" },   // RDBJ 02/11/2022
                        },
                        {
                            field: "IsResolved",
                            title: "Resolved?",
                            template: '<input onclick="updateStatus(this)"  type="checkbox" #= IsResolved ? \'checked="checked"\' : "" # class="chkbx" ' + blnIsVisitor + ' />',    // JSL 12/19/2022 added blnIsVisitor
                            width: "120px"
                        },
                        // RDBJ 12/21/2021
                        {
                            field: "AssignTo",
                            title: "Assign To",
                            width: "25%",
                        },
                        // End RDBJ 12/21/2021
                        {
                            field: "DueDate",
                            title: "DueDate",
                        },
                        // RDBJ 12/16/2021
                        {
                            field: "UpdatedDate",
                            title: "Last Update",
                            template: "#= UpdatedDate!=null? kendo.toString(kendo.parseDate(UpdatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy'):'' #",
                            width: "20%",
                        },
                    ]
                });
                setTimeout(function () {
                    var gridDataDetail = $('#DetailsGrid').data("kendoGrid");
                    gridDataDetail.autoFitColumn("Type");
                    //gridDataDetail.autoFitColumn("Deficiency");
                    //gridDataDetail.autoFitColumn("Reference");
                    gridDataDetail.autoFitColumn("DueDate");
                    gridDataDetail.autoFitColumn("IsResolved");
                    // RDBJ 01/24/2022
                    gridDataDetail.autoFitColumn("AssignTo");
                    gridDataDetail.autoFitColumn("Number");
					// End RDBJ 01/24/2022
                }, 3000);   // JSL 07/13/2022 set 3 secs instead of 1.5
            },
            error: function (data) {
                console.log(data);
            }
        });
        HideAndShow = true;
    });
}
function updateStatus(ctr) {
    // JSL 12/19/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/19/2022

    var UpdateAuditDeficiencies = $(ctr).parent().parent().find('.NotesUniqueID').text(); //RDBJ 11/13/2021 Set class NotesUniqueID
    var url = AuditRootUrl + "Deficiencies/UpdateAuditDeficiencies";
    var obj = {};
    obj.id = UpdateAuditDeficiencies; //RDBJ 11/13/2021 set UpdateAuditDeficiencies
    obj.isClose = $(ctr).prop('checked');
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Updated Successfully", "success");
            InitShipAudits($("#ddlGIReportShipName").val());
            LoadAuditDetail();
            $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").trigger('click');  // JSL 10/15/2022
            //LoadAuditShips($("#ddlGIReportShipName").val());
            //LoadAuditDetail();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}

function AddComment(id, ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = AuditRootUrl + "Deficiencies/AddAuditDeficiencyComments";
    var comment = $(ctr).parent().parent().find('.comment').val();
    var obj = {};
    obj.UserName = name;
    obj.Comment = comment;
    obj.AuditNoteID = id;

    var filedata = new Array();
    $(ctr).parent().parent().find('.file').each(function () {
        var Audit_Deficiency_Comments_Files = new Object();
        Audit_Deficiency_Comments_Files.CommentFileID = 0;
        Audit_Deficiency_Comments_Files.CommentsID = 0;
        Audit_Deficiency_Comments_Files.AuditNoteID = id;
        Audit_Deficiency_Comments_Files.FileName = $(this).next().val();
        Audit_Deficiency_Comments_Files.StorePath = $(this).next().next().val();
        Audit_Deficiency_Comments_Files.IsUpload = $(this).next().next().next().val();
        filedata.push(Audit_Deficiency_Comments_Files);
    });
    obj.AuditDeficiencyCommentsFiles = filedata;

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: JSON.stringify(obj),
        success: function (Data) {
            $.notify("Comment Added Successfully", "success");
            var url = AuditRootUrl + "Deficiencies/GetAuditDeficiencyComments";
            LoadAuditDeficiencyComments(id, url)
            var childGrid = $(ctr).parent().parent().parent().parent().parent().find('.CommentsGrid')
            $(childGrid).kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                dataSource: {
                    data: AuditDeficiencyComments.options.data
                },
                columns: [
                    {
                        field: "UserName",
                        title: "UserName",
                    },
                    {
                        field: "Comment",
                        title: "Comment"
                    },
                    {
                        field: "CreatedDate",
                        title: "Date Time",
                        template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'MM/dd/yyyy h:mm'):'' #",
                    },
                    {
                        field: 'AuditDeficiencyCommentsFiles',
                        title: 'Files',
                        template: "#=generateTemplate(AuditDeficiencyCommentsFiles)#",
                        width: "130px"
                    }
                ]
            });
            $(ctr).parent().parent().find('.comment').val('');
            $(ctr).parent().parent().find('.filename').remove();
            $(ctr).parent().parent().find('.file').remove();
            $(ctr).parent().parent().find('.path').remove();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
function InitCommentsTemplate(e) {
    var detailRow = e.detailRow;
    var id = e.data.NoteID;
    detailRow.find(".CommentsTab").kendoTabStrip({
        animation: {
            open: { effects: "fadeIn" }
        }
    });
    var url = AuditRootUrl + "Deficiencies/GetAuditDeficiencyComments";
    var urlFile = AuditRootUrl + "Deficiencies/GetAuditDeficiencyCommentFiles";
    LoadAuditDeficiencyComments(id, url);
    LoadCommentFiles(id, urlFile);
    detailRow.find(".CommentsGrid").kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        dataSource: {
            data: AuditDeficiencyComments.options.data
        },
        resizable: true,
        columns: [
            {
                field: "UserName",
                title: "UserName",
            },
            {
                field: "Comment",
                title: "Comment"
            },
            {
                field: "CreatedDate",
                title: "Date Time",
                template: "#= CreatedDate!=null? kendo.toString(kendo.parseDate(CreatedDate, 'yyyy-MM-dd'), 'MM/dd/yyyy h:mm'):'' #",
            },
            {
                field: 'AuditDeficiencyCommentsFiles',
                title: 'Files',
                template: "#=generateTemplate(AuditDeficiencyCommentsFiles)#",
                width: "130px"
            }
        ]
    });
}
function LoadAuditDeficiencyComments(id, url) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { NoteID: id },
        success: function (Data) {
            AuditDeficiencyComments = new kendo.data.DataSource({
                data: Data
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function LoadCommentFiles(id, url) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: id },
        success: function (Data) {
            AuditDeficiencyCommentFiles = new kendo.data.DataSource({
                data: Data
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function DownloadCommentFile(ctr, data) {
    var _name = $(data).text();
    var url = AuditRootUrl + "Deficiencies/DownloadCommentFile?CommentFileID=" + ctr + "&name=" + _name;;
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

function fileUpload(ctr, count) {
    // JSL 02/14/2023
    var image_holder = $("#img-holder");
    var notAllowType = "";
    var fileistoobig = ""; 
    var fileNametoobig = ""; 

    for (var i = 0; i < ctr.files.length; i++) {
        if (ctr.files[i].type.indexOf('pdf') >= 0 ||
            ctr.files[i].type.indexOf('document') >= 0 ||
            ctr.files[i].type.indexOf('xml') >= 0 ||
            ctr.files[i].type.indexOf('sheet') >= 0) {
            if (ctr.files[i].size > 2000000) {
                fileistoobig = fileistoobig + " [" + ctr.files[i].name + "] </br>";
            }
            else if (ctr.files[i].name.length > 40) {
                fileNametoobig = fileNametoobig + " [" + ctr.files[i].name + "] </br>";
            }
            else {
                // JSL 02/18/2023
                var UniqueID = CreateNewGuid();    
                fileData = new FormData();

                fileData.append('UniqueID', UniqueID);
                fileData.append('UniqueFormID', UniqueFormID);
                fileData.append('ReportType', 'FSTO');
                // End JSL 02/18/2023

                fileData.append(ctr.files[i].name, ctr.files[i]);   

                // JSL 02/18/2023
                $.ajax({
                    url: RootUrl + 'Deficiencies/UploadFSTOFiles',
                    type: "POST",
                    async: true,
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: fileData,
                    success: function (data) {
                        if (data.Status == 'Success') {
                            var strFileName = data.AttachmentName;
                            var strFileUniqueID = data.UniqueID;

                            $.notify(strFileName + " attachment uploaded successfully!", "success");

                            var data = '<div data-id="dvFSTO_' + strFileUniqueID + '" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
                                '<a data-toggle="tooltip" data-placement="bottom" data-original-title="Download ' + strFileName + '" onclick="DownloadFSTOFile(\'' + strFileUniqueID + '\')">' + strFileName + '</a>' +
                                '<i data-toggle="tooltip" data-placement="bottom" data-original-title="Delete" class="close fa fa-trash btnDeleteFSTOAttachment" data-id="' + strFileUniqueID + '" onclick="RemoveFSTOFile(this)"></i >' +
                                '</div>';
                            $("#fstoAttachmentList").append(data);
                            BindDeletePopConfirm(".btnDeleteFSTOAttachment", "left");
                            fileData = undefined;
                        }
                    },
                    error: function (err) {
                        alert(err.statusText);
                    }
                });
                // End JSL 02/18/2023
            }
        }
        else {
            notAllowType = notAllowType + " [" + ctr.files[i].name + "] </br>";
        }
    }

    var blnIsNeedTOShowErrorMessage = false;    
    if (notAllowType != "") {
        $("#modal-default p#fileTypeError").html("<strong>File types are not supported : </strong></br>" + notAllowType + "");
        blnIsNeedTOShowErrorMessage = true;
    }

    if (fileistoobig != "") {
        $("#modal-default p#fileSizeError").html("<strong>File must be smaller than 2.0 MB : </strong></br>" + fileistoobig + "");
        blnIsNeedTOShowErrorMessage = true;
    }

    if (fileNametoobig != "") {
        $("#modal-default p#fileNameError").html("<strong>File name must be smaller than 40 Character : </strong></br>" + fileNametoobig + "");
        blnIsNeedTOShowErrorMessage = true; 
    }

    if (blnIsNeedTOShowErrorMessage) {
        $('#modal-default').modal('show');
    }
    image_holder.show();
    // End JSL 02/14/2023


    // JSL 02/14/2023 commented old
    /*if (typeof (FileReader) != "undefined") {
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
    }*/
    // End JSL 02/14/2023 commented old
}
function RemoveFile(ctr
    , value    // JSL 02/14/2023
) {
    $(ctr).parent().next().next().next().val('false')
    $(ctr).parent().hide();

    fileData.delete(value); // JSL 02/14/2023
}

//RDBJ 11/25/2021
function ValidateIAFForm() {

    IAFFormValidator = $("#IAFForm").validate({
        rules: {
            txtLocation: {
                required: true
            },
            ddlAuditNo: {
                required: true
            },
            txtAuditDate: {
                required: true
            },
            ddlAuditor: {
                required: true
            },
            AuditNoteType: {
                required: true
            },
        },
        messages: {
            txtLocation: "Please Enter Location.",
            ddlAuditNo: "Please Select Audit No",
            txtAuditDate: "Please Select Date",
            ddlAuditor: "Please Select Auditor",
            AuditNoteType: "Please Audit Note Type",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
//End RDBJ 11/25/2021

//RDBJ 11/25/2021
function ValidateIAFNotesFormValidator() {

    IAFNotesFormValidator = $("#IAFNotesForm").validate({
        rules: {
            Reference: {
                required: true
            },
            txtDueDate: {
                required: true
            },
        },
        messages: {
            Reference: "Please Select Reference",
            txtDueDate: "Please Select Date",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
//End RDBJ 11/25/2021

//RDBJ 11/24/2021
function GetAndSetIAFFormData(UniqueFormID) {
    var url = AuditRootUrl + "IAFList/GetAuditDetails/" + UniqueFormID;
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            data = Data;

            //RDBJ 11/24/2021
            $('#ddpAuditType').val(data.AuditType).change();
            $('#ddlAuditNo').val(data.AuditNo).change();

            if ($("#ddlAuditor option[value='" + data.Auditor + "']").length == 0) {
                /*
                var ddlAuditor = document.getElementById('ddlAuditor');
                ddlAuditor.options[ddlAuditor.options.length] = new Option(data.Auditor, data.Auditor);
                */

                var o = new Option(data.Auditor, data.Auditor);
                /// jquerify the DOM object 'o' so we can use the html method
                $(o).html(data.Auditor);
                $("#ddlAuditor").append(o);
                $('#ddlAuditor').val(data.Auditor).change();
            }
            else
                $('#ddlAuditor').val(data.Auditor).change();

            $("#txtAuditTypeISM").prop("checked", data.AuditTypeISM);
            $("#txtAuditTypeISPS").prop("checked", data.AuditTypeISPS);
            $("#txtAuditTypeMLC").prop("checked", data.AuditTypeMLC);
            $("#ckbAuditAdditional").prop("checked", data.IsAdditional);
            $("#ckbAuditClosed").prop("checked", data.IsClosed);

            $("#txtLocation").val(data.Location);
            $("#txtAuditDate").datepicker("setDate", parseJsonDate(data.Date));

            $("#hdnIAFFormSavedAsDraft").val(data.SavedAsDraft);    // RDBJ 01/23/2022
            //End RDBJ 11/24/2021

            $("#btnAddAudit").attr("disabled", true);
            $("#btnAddAudit").text("Auto Update Mode");
            HideAndShow = true;

            $("#btnAddDeficiencies").attr("disabled", false); //RDBJ 11/25/2021
            $("#btnRemoveDeficiencies").attr("disabled", true); //RDBJ 11/25/2021

            // JSL 05/25/2022 wrapped in if
            if (!IsNullEmptyOrUndefined(IAFFormValidator)) {
                IAFFormValidator.resetForm(); //RDBJ 11/25/2021
            }

            // JSL 05/25/2022 wrapped in if
            if (!IsNullEmptyOrUndefined(IAFNotesFormValidator)) {
                IAFNotesFormValidator.resetForm(); //RDBJ 11/25/2021
            }
        },
        error: function (data) {
            console.log(data);
        }
    });
}
//End RDBJ 11/24/2021

//RDBJ 11/25/2021
function GetAndSetIAFFormNoteData(NotesUniqueID) {
    var url = AuditRootUrl + "Forms/GetAuditNotesById";
    $.ajax({
        contentType: 'application/json',
        dataType: 'json',
        async: false,
        url: url,
        data: { id: NotesUniqueID },
        success: function (Data) {
            data = Data;
            
            $("#hdnAuditNoteNumber").val(data.Number);
            $('.type').val(data.Type).change();
            $("#txtBriefDescription").val(data.BriefDescription);
            $("#txtReference").val(data.Reference);
            $("#txtDueDate").datepicker("setDate", parseJsonDate(data.TimeScale));
            $("#ckbAuditNoteResolved").prop("checked", data.isResolved);

            // JSL 10/15/2022
            $("#btnAddDeficiencies").addClass("hide");   
            $("#btnClearAndAddNewDeficiencies").removeClass("hide");
            // End JSL 10/15/2022

            $("#btnAddDeficiencies").attr("disabled", true);
            $("#btnRemoveDeficiencies").attr("disabled", false);

            IAFNotesFormValidator.resetForm();
        },
        error: function (data) {
            console.log(data);
        }
    });
}
//End RDBJ 11/25/2021

//RDBJ 11/24/2021
function parseJsonDate(jsonDateString) {
    if (jsonDateString == undefined)
        return "";
    else
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}
//End RDBJ 11/24/2021

//RDBJ 11/24/2021
function IAFAutoSave() {
    $("#lblAutoSave").show();
    var url = RootUrl + 'IAFList/IAFAutoSave';
    var txtAuditTypeISM, txtAuditTypeISPS, txtAuditTypeMLC, IsAdditional, IsClosed;

    if ($("#txtAuditTypeISM").prop('checked') == true)
        txtAuditTypeISM = true;
    else
        txtAuditTypeISM = false;

    if ($("#txtAuditTypeISPS").prop('checked') == true)
        txtAuditTypeISPS = true;
    else
        txtAuditTypeISPS = false;

    if ($("#txtAuditTypeMLC").prop('checked') == true)
        txtAuditTypeMLC = true;
    else
        txtAuditTypeMLC = false;

    if ($("#ckbAuditAdditional").prop('checked') == true)
        IsAdditional = true;
    else
        IsAdditional = false;

    if ($("#ckbAuditClosed").prop('checked') == true)
        IsClosed = true;
    else
        IsClosed = false;

    var Modal = {
        "InternalAuditForm.UniqueFormID": UniqueFormID,
        "InternalAuditForm.AuditType": $("#ddpAuditType option:selected").val(),
        "InternalAuditForm.AuditTypeISM": txtAuditTypeISM,
        "InternalAuditForm.AuditTypeISPS": txtAuditTypeISPS,
        "InternalAuditForm.AuditTypeMLC": txtAuditTypeMLC,
        "InternalAuditForm.Location": $("#txtLocation").val(),
        "InternalAuditForm.AuditNo": $("#ddlAuditNo option:selected").val(),
        "InternalAuditForm.Date": $("#txtAuditDate").val(),
        "InternalAuditForm.Auditor": $("#ddlAuditor option:selected").val(),
        "InternalAuditForm.IsAdditional": IsAdditional,
        "InternalAuditForm.IsClosed": IsClosed,
        "InternalAuditForm.ShipName": $("#ddlGIReportShipName option:selected").val(),
        "InternalAuditForm.SavedAsDraft": $("#hdnIAFFormSavedAsDraft").val(),
    };
    
    //RDBJ 11/19/2021
    $.ajax({
        url: url,
        type: 'POST',
        data: Modal,
        async: true,
        success: function (res) {
            InitShipAudits($("#ddlGIReportShipName option:selected").val());
            $('#DetailsGrid').empty();
            LoadAuditDetail();
            $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").trigger('click'); //RDBJ 11/24/2021

            $("#lblAutoSave").hide();
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
    //End RDBJ 11/19/2021
}
//End RDBJ 11/24/2021

//RDBJ 11/24/2021
function updateAdditionalAndCloseStatus(ctr, isAdditional) {
    // JSL 12/19/2022
    if (!IsNullEmptyOrUndefined(_CurrentUserDetailsObject)) {
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            return;
        }
    }
    else {
        return;
    }
    // End JSL 12/19/2022

    var IAFUFId = $(ctr).parent().parent().find('.UniqueFormID').text();
    var url = RootUrl + "IAFList/UpdateAdditionalAndCloseStatus";

    var obj = {};
    obj.id = IAFUFId;
    obj.IsAdditionalAndClosedStatus = $(ctr).prop('checked');
    obj.IsAdditionalAndClosed = isAdditional;

    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Updated Successfully", "success");
            var shipCode = $("#ddlGIReportShipName").val();

            InitShipAudits(shipCode);
            LoadAuditDetail();
            $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").trigger('click');  // JSL 10/15/2022
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
//End RDBJ 11/24/2021

//RDBJ 11/22/2021
function EditIAFButton(formType) {
    $("#btnAddIAFDone").css("display", "none"); //RDBJ 11/26/2021
    $("#btnEditIAFDone").css("display", "none"); //RDBJ 11/26/2021

    if (formType == "IA") {
        HideAndShowEditButtonIfIAFListNullOrNot(); // RDBJ 12/03/2021

        $('#btnAddIAF').css('display', 'block'); //RDBJ 11/24/2021
        $("#btnAddAudit").text("Auto Update Mode"); //RDBJ 11/23/2021

        GetNumberForNotes();    // RDBJ 01/27/2022
    }
    else {
        $('#btnEditIAF').css('display', 'none');
        $("#btnEditIAF").text("Edit"); //RDBJ 11/23/2021

        $('#btnAddIAF').css('display', 'none'); //RDBJ 11/24/2021
        $("#btnAddAudit").text("Add Audit"); //RDBJ 11/23/2021

        $('#SectionAuditNote').css('display', 'none'); //RDBJ 11/23/2021
        $('#SectionAuditNotes').css('display', 'none'); //RDBJ 11/23/2021
    }
}
//End RDBJ 11/22/2021

//RDBJ 11/22/2021
function HideAndShowAuditAndAuditNotesSection(editMode) {
    if (HideAndShow) {
        //$("#btnEditIAF").text("Edit/Update Done"); //RDBJ 11/23/2021
        //$("#btnEditIAF").removeClass("btn-primary"); //RDBJ 11/23/2021
        //$("#btnEditIAF").addClass("btn-success"); //RDBJ 11/23/2021

        $('#SectionAuditNote').css('display', 'block');
        $('#SectionAuditNotes').css('display', 'block');
        HideAndShow = false; //RDBJ 11/23/2021
        $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").trigger('click'); //RDBJ 11/24/2021
    } else {
        $('#SectionAuditNote').css('display', 'block');
        $('#SectionAuditNotes').css('display', 'block');

        //$("#btnEditIAF").text("Edit"); //RDBJ 11/23/2021
        //$("#btnEditIAF").removeClass("btn-success"); //RDBJ 11/23/2021
        //$("#btnEditIAF").addClass("btn-primary"); //RDBJ 11/23/2021
        HideAndShow = true; //RDBJ 11/23/2021
    }

    //RDBJ 11/24/2021
    if (editMode) {
        $("#btnEditIAF").css("display", "none"); //RDBJ 11/26/2021
        $("#btnEditIAFDone").css("display", "block"); //RDBJ 11/26/2021
        $("#btnAddIAF").css("display", "none"); //RDBJ 11/26/2021

        // RDBJ 01/29/2022
        if (rowAuditIndex == "") {
            $('#Grid tbody tr:first-child').trigger('click');
        }

        //$("#btnEditIAF").text("Edit/Update Done");
        //$("#btnEditIAF").removeClass("btn-primary");
        //$("#btnEditIAF").addClass("btn-success");

        $("#btnRemoveAudit").attr("disabled", false);
        $("#btnAddAudit").attr("disabled", true);
        $("#btnAddAudit").text("Auto Update Mode");
        HideAndShow = true;

        //ResetAuditForm() //RDBJ 11/25/2021
        ResetAuditNoteForm();   // JSL 10/15/2022
    }
    else {
        $("#btnAddIAF").css("display", "none"); //RDBJ 11/26/2021
        $("#btnAddIAFDone").css("display", "block"); //RDBJ 11/26/2021
        $("#btnEditIAF").css("display", "none"); //RDBJ 11/26/2021

        //$("#btnEditIAF").text("Edit");
        //$("#btnEditIAF").removeClass("btn-success");
        //$("#btnEditIAF").addClass("btn-primary");

        $("#btnRemoveAudit").attr("disabled", true);
        $("#btnAddAudit").attr("disabled", false);
        $("#btnAddAudit").text("Add Audit");
        HideAndShow = false;
        $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").removeClass("k-state-selected");
        $("#DetailsGrid").data('kendoGrid').dataSource.data([]);

        UniqueFormID = undefined; //RDBJ 11/25/2021

        ResetAuditForm();
    }
    //End RDBJ 11/24/2021
}
//End RDBJ 11/22/2021

//RDBJ 11/26/2021
function HideAndShowAuditAndAuditNotesSectionAddEditDone(AddEditModeDone) {
    if (AddEditModeDone)
    {
        $("#btnAddIAF").css("display", "block");
        $("#btnAddIAFDone").css("display", "none");
        $("#btnEditIAF").css("display", "block");
        $("#btnEditIAFDone").css("display", "none");
        HideAndShow = true;
    }
    else
    {
        $("#btnEditIAF").css("display", "block");
        $("#btnEditIAFDone").css("display", "none");
        $("#btnAddIAF").css("display", "block");
        $("#btnAddIAFDone").css("display", "none");
        HideAndShow = false;
    }

    HideAndShowEditButtonIfIAFListNullOrNot(); // RDBJ 12/03/2021

    $('#SectionAuditNote').css('display', 'none');
    $('#SectionAuditNotes').css('display', 'none');
}
//End RDBJ 11/26/2021

// RDBJ 12/03/2021
function HideAndShowEditButtonIfIAFListNullOrNot() {
    IAFRowCount = $('#Grid tr').length;

    if (parseInt(IAFRowCount) > 1) {
        $('#btnEditIAF').css('display', 'block');
    }
    else {
        $('#btnEditIAF').css('display', 'none');
    }
}
// End RDBJ 12/03/2021

//RDBJ 11/22/2021
function getNumber(ctr) {
    //SetNumberForNotes(Numbers); // RDBJ 01/27/2022 commented this line //RDBJ 11/23/2021
    GetNumberForNotes();    // RDBJ 01/27/2022
    var number;
    if ($(ctr).val() == 'ISM-Non Conformity') {
        number = ISMNonConformity
        ISMNonConformity = parseInt(ISMNonConformity) + 1
        $(ctr).parent().parent().find('.number').html(parseInt(number) + 1 + ' of  <span class="ISMNonConformity">' + ISMNonConformity + '</span>')
        $(".ISMNonConformity").text(ISMNonConformity)
    }
    if ($(ctr).val() == 'ISPS-Non Conformity') {
        number = ISPSNonConformity
        ISPSNonConformity = parseInt(ISPSNonConformity) + 1
        $(ctr).parent().parent().find('.number').html(parseInt(number) + 1 + ' of  <span class="ISPSNonConformity">' + ISPSNonConformity + '</span>')
        $(".ISPSNonConformity").text(ISPSNonConformity)
    }
    if ($(ctr).val() == 'ISM-Observation') {
        number = ISMObservation
        ISMObservation = parseInt(ISMObservation) + 1
        $(ctr).parent().parent().find('.number').html(parseInt(number) + 1 + ' of  <span class="ISMObservation">' + ISMObservation + '</span>')
        $(".ISMObservation").text(ISMObservation)
    }
    if ($(ctr).val() == 'ISPS-Observation') {
        number = ISPSObservation
        ISPSObservation = parseInt(ISPSObservation) + 1
        $(ctr).parent().parent().find('.number').html(parseInt(number) + 1 + ' of  <span class="ISPSObservation">' + ISPSObservation + '</span>')
        $(".ISPSObservation").text(ISPSObservation)
    }
    if ($(ctr).val() == 'MLC-Deficiency') {
        number = MLCDeficiency
        MLCDeficiency = parseInt(MLCDeficiency) + 1
        $(ctr).parent().parent().find('.number').html(parseInt(number) + 1 + ' of  <span class="MLCDeficiency">' + MLCDeficiency + '</span>')
        $(".MLCDeficiency").text(MLCDeficiency)
    }

    $(ctr).parent().parent().find('input.number').val(parseInt(number) + 1)
    $(ctr).css("border-color", "#d2d6de");

    //AuditTypeListSelection();
    typeVal = $(ctr).val();
    HideAndShowTreeListInReferenceModal(typeVal);
}
//End RDBJ 11/22/2021

//RDBJ 11/22/2021
function HideAndShowTreeListInReferenceModal(typeValue) {
    if (typeValue == "ISM-Non Conformity" || typeValue == "ISM-Observation") {
        $("#MLCTree").hide();
        $("#SSPTree").hide();
        $("#ISMTree").show();
    }
    if (typeValue == "ISPS-Non Conformity" || typeValue == "ISPS-Observation") {
        $("#ISMTree").hide();
        $("#MLCTree").hide();
        $("#SSPTree").show();
    }
    if (typeValue == "MLC-Deficiency") {
        $("#ISMTree").hide();
        $("#SSPTree").hide();
        $("#MLCTree").show();
    }
}

function AuditTypeListSelection() {
    if ($("#txtAuditTypeISM").prop('checked') == false) {
        $(".type option:contains('ISM')").hide();
    } else {
        $(".type option:contains('ISM')").show();
    }

    if ($("#txtAuditTypeISPS").prop('checked') == false) {
        $(".type option:contains('ISPS')").hide();
    } else {
        $(".type option:contains('ISPS')").show();
    }

    if ($("#txtAuditTypeMLC").prop('checked') == false) {
        $(".type option:contains('MLC')").hide();
    } else {
        $(".type option:contains('MLC')").show();
    }
}
//End RDBJ 11/22/2021

//RDBJ 11/22/2021
function SelectReference() {
    if (typeVal == "ISM-Non Conformity" || typeVal == "ISM-Observation")
        $("input[name='Reference']").val($("#ISMTree li a.ui-state-active").text()); //RDBJ 12/01/2021
        //$("input[name='Reference']").val($("#ISMTree li.node-selected").text()); //RDBJ 12/01/2021 commented
    if (typeVal == "ISPS-Non Conformity" || typeVal == "ISPS-Observation")
        $("input[name='Reference']").val($("#SSPTree li a.ui-state-active").text()); //RDBJ 12/01/2021
        //$("input[name='Reference']").val($("#SSPTree li.node-selected").text());  //RDBJ 12/01/2021 commented
    if (typeVal == "MLC-Deficiency")
        $("input[name='Reference']").val($("#MLCTree li a.ui-state-active").text()); //RDBJ 12/01/2021
        //$("input[name='Reference']").val($("#MLCTree li.node-selected").text());  //RDBJ 12/01/2021 commented

    $("#ReferenceModal").modal('hide');
}
//End RDBJ 11/22/2021

//RDBJ 11/23/2021
function SetNumberForNotes(Numbers) {
    //RDBJ 11/22/2021
    if (Numbers != null && Numbers != undefined && Numbers.length > 0) {
        ISMNonConformity = Numbers[0];
        ISPSNonConformity = Numbers[1];
        ISMObservation = Numbers[2];
        ISPSObservation = Numbers[3];
        MLCDeficiency = Numbers[4];
    }
    //End RDBJ 11/22/2021
}
//End RDBJ 11/23/2021

//RDBJ 11/23/2021
function SubmitAudits() {
    $("#lblAutoSave").show();
    //RDBJ 11/25/2021 wrapped in form validator
    if ($("#IAFForm").valid()) {
        var url = AuditRootUrl + "IAFList/SubmitIAForm";
        var txtAuditTypeISM, txtAuditTypeISPS, txtAuditTypeMLC, IsAdditional, IsClosed;

        if ($("#txtAuditTypeISM").prop('checked') == true)
            txtAuditTypeISM = true;
        else
            txtAuditTypeISM = false;

        if ($("#txtAuditTypeISPS").prop('checked') == true)
            txtAuditTypeISPS = true;
        else
            txtAuditTypeISPS = false;

        if ($("#txtAuditTypeMLC").prop('checked') == true)
            txtAuditTypeMLC = true;
        else
            txtAuditTypeMLC = false;

        //RDBJ 11/24/2021
        if ($("#ckbAuditAdditional").prop('checked') == true)
            IsAdditional = true;
        else
            IsAdditional = false;

        if ($("#ckbAuditClosed").prop('checked') == true)
            IsClosed = true;
        else
            IsClosed = false;
        //End RDBJ 11/24/2021

        var Modal = {
            AuditType: $("#ddpAuditType option:selected").val(), //RDBJ 11/24/2021
            AuditTypeISM: txtAuditTypeISM,
            AuditTypeISPS: txtAuditTypeISPS,
            AuditTypeMLC: txtAuditTypeMLC,
            Location: $("#txtLocation").val(),
            AuditNo: $("#ddlAuditNo option:selected").val(),
            Date: $("#txtAuditDate").val(),
            Auditor: $("#ddlAuditor option:selected").val(),
            IsAdditional: IsAdditional, //RDBJ 11/24/2021
            IsClosed: IsClosed, //RDBJ 11/24/2021
            ShipName: $("#ddlGIReportShipName option:selected").val(),
            SavedAsDraft: false,    // RDBJ 01/23/2022
        };

        $.ajax({
            url: url,
            type: 'POST',
            async: true,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Modal: Modal }),
            success: function (res) {
                ResetAuditForm(); //RDBJ 11/24/2021

                InitShipAudits($("#ddlGIReportShipName option:selected").val()
                    , true  // JSL 04/20/2022
                );
                $('#DetailsGrid').empty();
                LoadAuditDetail();
                $('#Grid tbody tr:first-child').trigger('click');

                $("#lblAutoSave").hide();
            },
            error: function (err) {
                $("#lblAutoSave").hide();
            }
        });
    }

    
}
//End RDBJ 11/23/2021

//RDBJ 11/24/2021
function ResetAuditForm() {
    $("#ddpAuditType").prop("selectedIndex", 0); //RDBJ 11/24/2021
    $("#ddlAuditNo").prop("selectedIndex", 1); // RDBJ 01/28/2022 set 1st selected to avoid null value //RDBJ 11/24/2021
    $("#ddlAuditor").prop("selectedIndex", 0); //RDBJ 11/24/2021

    $("#txtAuditTypeISM").prop("checked", false);
    $("#txtAuditTypeISPS").prop("checked", false);
    $("#txtAuditTypeMLC").prop("checked", false);
    $("#ckbAuditAdditional").prop("checked", false);
    $("#ckbAuditClosed").prop("checked", false);

    $("#txtLocation").val('');
    $("#txtAuditDate").val('');

    IAFFormValidator.resetForm(); //RDBJ 11/25/2021
    AuditTypeListSelection(); //RDBJ 11/25/2021

    ResetAuditNoteForm();   // JSL 10/15/2022
}
//End RDBJ 11/24/2021

//RDBJ 11/24/2021
function RemoveAuditsOrAuditNotes(IsAudit) {
    var IAFUFIdOrNoteId;
    var url = RootUrl + "IAFList/RemoveAuditsOrAuditNotes";

    if (IsAudit)
        IAFUFIdOrNoteId = UniqueFormID;
    else
        IAFUFIdOrNoteId = NotesUniqueID;

    var obj = {};
    obj.id = IAFUFIdOrNoteId;
    obj.IsAudit = IsAudit;

    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Deleted Successfully", "error");

            var shipCode = $("#ddlGIReportShipName").val();

            InitShipAudits(shipCode);
            LoadAuditDetail();

            if (IsAudit) {
                $('#Grid tbody tr:first-child').trigger('click');
            }
            else {
                $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").trigger('click'); //RDBJ 11/24/2021

                ClearAndResetFormAuditNote();   // JSL 10/15/2022
            }
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}
//End RDBJ 11/24/2021

//RDBJ 11/23/2021
function SubmitAuditNote() {
    $("#lblAutoSave").show();
    //RDBJ 11/25/2021 wrapped in form validator
    if ($("#IAFNotesForm").valid()) {
        if (UniqueFormID == undefined) {
            alert("Please Select Audit From Upper List");
            return;
        }

        var url = AuditRootUrl + 'IAFList/AddIAFAuditNote';
        var isResolved;

        if ($("#ckbAuditNoteResolved").prop('checked') == true)
            isResolved = true;
        else
            isResolved = false;

        var Modal = {
            UniqueFormID: UniqueFormID,
            Number: $("#hdnAuditNoteNumber").val(),
            Type: $(".type option:selected").val(),
            BriefDescription: $("#txtBriefDescription").val(),
            Reference: $("#txtReference").val(),
            TimeScale: $("#txtDueDate").val(),
            isResolved: isResolved,
            Ship: $("#ddlGIReportShipName option:selected").val(),
        };

        $.ajax({
            url: url,
            type: 'POST',
            async: true,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Modal: Modal }),
            success: function (res) {

                InitShipAudits($("#ddlGIReportShipName option:selected").val());
                $('#DetailsGrid').empty();
                LoadAuditDetail();
                $("#Grid tbody tr:nth-child(" + rowAuditIndex + ")").trigger('click'); //RDBJ 11/24/2021

                ResetAuditNoteForm();   // JSL 10/15/2022

                GetNumberForNotes();    // RDBJ 01/27/2022

                $("#lblAutoSave").hide();
            },
            error: function (err) {
                $("#lblAutoSave").hide();
            }
        });
    }
}
//End RDBJ 11/23/2021

// RDBJ 01/27/2022
function GetNumberForNotes() {
    var url = RootUrl + 'IAFList/GetNumberForNotes';
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        data:
        {
            Ship: $("#ddlGIReportShipName option:selected").val(),
            UniqueFormID: UniqueFormID, // RDBJ 01/22/2022
        },
        success: function (res) {
            Numbers = res.Numbers;
            SetNumberForNotes(Numbers);

            $("#lblAutoSave").hide();
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
}
// End RDBJ 01/27/2022

// JSL 10/15/2022
function ResetAuditNoteForm() {
    $("#hdnAuditNoteNumber").val('');
    //$(".type option:selected").val('');
    $(".type").prop("selectedIndex", 0); //RDBJ 11/24/2021
    $("#txtBriefDescription").val('');
    $("#txtReference").val('');
    $("#txtDueDate").val('');
    $("#ckbAuditNoteResolved").prop("checked", false);

    IAFNotesFormValidator.resetForm();
}
// End JSL 10/15/2022

// JSL 10/15/2022
function ClearAndResetFormAuditNote() {
    ResetAuditNoteForm();
    $("#btnClearAndAddNewDeficiencies").addClass("hide");
    $("#btnAddDeficiencies").removeClass("hide");
    $("#btnAddDeficiencies").attr("disabled", false);
}
// End JSL 10/15/2022

// JSL 12/31/2022
function InitShipFSTOAudits(shipCode) {
    var code = shipCode;
    var url = AuditRootUrl + "Deficiencies/GetFSTOAuditData";

    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: {
            code: code
        },
        success: function (Data) {
            ShipAudits = new kendo.data.DataSource({
                data: Data
            });

            BindShipFSTOAuditsGrid();
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End JSL 12/31/2022

// JSL 12/31/2022
function BindShipFSTOAuditsGrid() {
    var blnNeedShowDelete = false;
    if (_CurrentUserDetailsObject.UserGroup == "1" || _CurrentUserDetailsObject.UserGroup == "5") {
        blnNeedShowDelete = true;
    }

    $('#fstoGrid').empty();

    var grid = $('#fstoGrid').kendoGrid({
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
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
        },
        dataSource: {
            data: ShipAudits.options.data,
            pageSize: 10,
            //sort: { field: "AuditDate", dir: "desc" }
        },
        dataBound: function (e) {
            $("#fstoGrid .k-grid-content").css("min-height", "220px");
            //for (var i = 0; i < this.columns.length; i++) {
            //    this.autoFitColumn(i);
            //}
            setTimeout(function () {
                var gridDataDetail = $('#fstoGrid').data("kendoGrid");
                /*gridDataDetail.autoFitColumn("UserName");
                gridDataDetail.autoFitColumn("TravelStartedOn");
                gridDataDetail.autoFitColumn("EmbarkedOn");
                gridDataDetail.autoFitColumn("DisembarkedOn");
                gridDataDetail.autoFitColumn("TravelEndedOn");
                gridDataDetail.autoFitColumn("Location");
                gridDataDetail.autoFitColumn("CompletedDays");
                gridDataDetail.autoFitColumn("PurposeOfVisit");*/
            }, 3000);   

            $('#fstoGrid').on("mousedown", "tr[role='row']", function (e) {
                if (e.which === 3) {
                    $("tr").removeClass("k-state-selected");
                    $(this).toggleClass("k-state-selected");
                    //$(this).trigger('click');
                }
            });

            BindToolTipsForGridText("fstoGrid", "Location", 120);
            BindToolTipsForGridText("fstoGrid", "PurposeOfVisit", 120);
        },
        resizable: true,
        columns: [
            {
                field: "UniqueFormID",
                title: "UniqueFormID",
                hidden: true,
                attributes: { class: "fstoUniqueFormID" },
            },
            {
                field: "UserGUID",
                title: "UserGUID",
                hidden: true,
            },
            {
                field: "UserName",
                title: "Inspector", 
                width: 150,
            },
            {
                field: "TravelStartedOn",
                title: "Travel Started",
                width: 120,
                template: "#= TravelStartedOn!=null? kendo.toString(kendo.parseDate(TravelStartedOn, 'yyyy-MM-dd'), 'dd-MMM-yyyy'):'' #",
                filterable: {
                    ui: "datepicker"
                },
                hidden: true,   // JSL 02/21/2023
            },
            {
                field: "EmbarkedOn",
                title: "Embarked",
                width: 120,
                template: "#= EmbarkedOn!=null? kendo.toString(kendo.parseDate(EmbarkedOn, 'yyyy-MM-dd'), 'dd-MMM-yyyy'):'' #",
                filterable: {
                    ui: "datepicker"
                },
                hidden: true,   // JSL 02/21/2023
            },
            {
                field: "DisembarkedOn",
                title: "Disembarked",
                width: 120,
                template: "#= DisembarkedOn!=null? kendo.toString(kendo.parseDate(DisembarkedOn, 'yyyy-MM-dd'), 'dd-MMM-yyyy'):'' #",
                filterable: {
                    ui: "datepicker"
                },
                hidden: true,   // JSL 02/21/2023
            },
            {
                field: "TravelEndedOn",
                title: "Travel Ended",
                width: 120,
                template: "#= TravelEndedOn!=null? kendo.toString(kendo.parseDate(TravelEndedOn, 'yyyy-MM-dd'), 'dd-MMM-yyyy'):'' #",
                filterable: {
                    ui: "datepicker" 
                },
                hidden: true,   // JSL 02/21/2023
            },
            {
                field: "Location",
                title: "Location",
                width: 200,
            },
            {
                field: "CompletedDays",
                title: "Days",
                width: 70,
            },
            {
                field: "PurposeOfVisit",
                title: "Purpose Of Visit",
                attributes: { class: "tooltipText" },   // JSL 02/18/2023
                width: 410,
            },
            // JSL 02/17/2023
            /*{
                field: 'FSTOInspectionAttachments',
                title: 'Files',
                template: "#=generateTemplateForFSTOFiles(FSTOInspectionAttachments)#",
                width: "50%"
            },*/
            {
                field: "UniqueFormID",
                title: "Delete",
                filterable: false,
                template: "#=generateTemplateForFSTODeleteButton(UniqueFormID, 'FSTO')#",
                //width: "65px",
                width: 90,
                hidden: !blnNeedShowDelete,
            },
            // End JSL 02/17/2023
        ]
    });
    BindDeletePopConfirm(".btnDeleteFSTO", "left"); // JSL 02/17/2023
    //BindContextMenuByGridId("#fstoGrid"); // JSL 02/17/2023 commented  // JSL 02/14/2023
}
// End JSL 12/31/2022

// JSL 02/17/2023
function generateTemplateForFSTODeleteButton(UniqueFormID, type) {
    var template = '<div style="padding:1px;line-height: unset;" class="white-text waves-effect waves-effect file">' +
        '<a class="btn btn-danger btnDeleteFSTO" onclick="DeleteFSTO(\'' + UniqueFormID + '\', \'' + type + '\', this)" style="padding: 0px 16px !important"> Delete </a>' +
        '</div>';
    return template;
}
// End JSL 02/17/2023

// JSL 02/17/2023
function DeleteFSTO(strUniqueFormID, strFormType) {
    var dic = {};

    dic["UniqueFormID"] = strUniqueFormID;
    dic["CurrentUserID"] = _CurrentUserDetailsObject.UserGUID;
    dic["FormType"] = strFormType;

    CommonServerPostApiCall(dic, "Deficiencies", "PerformAction", str_API_DELETEFSTO);
}
// End JSL 02/17/2023

// JSL 02/17/2023
function generateTemplateForFSTOFiles(lstFSTOAttachment, blnIsGenerateForGrid = false) {
    var template = "";
    for (var i = 0; i < lstFSTOAttachment.length; i++) {
        var strUniqueID = lstFSTOAttachment[i].UniqueID;
        var strAttachmentName = lstFSTOAttachment[i].AttachmentName;
        template = template + '<div data-id="dvFSTO_' + strUniqueID + '" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +   
            '<a data-toggle="tooltip" data-placement="bottom" data-original-title="Download ' + strAttachmentName + '" onclick="DownloadFSTOFile(\'' + strUniqueID + '\')">' + strAttachmentName + '</a>';  

        if (!blnIsGenerateForGrid && (_CurrentUserDetailsObject.UserGroup == "1" || _CurrentUserDetailsObject.UserGroup == "5")) {
            template = template + '<i style="padding-left: 10px;" data-toggle="tooltip" data-placement="bottom" data-original-title="Delete" class="close fa fa-trash btnDeleteFSTOAttachment" data-id="' + strUniqueID + '" onclick="RemoveFSTOFile(this)"></i>';
        }

        //template = template + '<i data-toggle="tooltip" data-placement="bottom" data-original-title="Download" style="padding-left: 10px;" class="close fa fa-download" onclick="DownloadFSTOFile(\'' + lstFSTOAttachment[i].UniqueID + '\')"></i>'; 
        template = template + '</div>';
    }
    return template;
}
// End JSL 02/17/2023

// JSL 02/17/2023
function RemoveFSTOFile(ctr) {
    var FileUniqueID = $(ctr).attr("data-id");
    var dic = {};

    dic["FileUniqueID"] = FileUniqueID;
    dic["CurrentUserID"] = _CurrentUserDetailsObject.UserGUID;
    dic["FormType"] = "FSTO";

    CommonServerPostApiCall(dic, "Deficiencies", "PerformAction", str_API_DELETEFSTOFILE);
}
// End JSL 02/17/2023

// JSL 02/17/2023
function DownloadFSTOFile(fileId, element) {
    var url = RootUrl + "Deficiencies/DownloadFSTOFile?fileId=" + fileId;
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
// End JSL 02/17/2023

// JSL 12/31/2022
function LoadFSTOAuditDetail() {
    $("#fstoGrid tbody").on("click", "tr", function (e) {
        var Subject = e.currentTarget.children[2].textContent   
        UniqueFormID = e.currentTarget.children[0].textContent 
        rowAuditIndex = parseInt($(this).index(), 10) + 1; 
        HideAndShow = false;
        
        if (e.currentTarget.children[15].textContent == "false") {  
            $(".type option:contains('ISM')").hide();
        } else {
            $(".type option:contains('ISM')").show();
        }

        if (e.currentTarget.children[16].textContent == "false") { 
            $(".type option:contains('ISPS')").hide();
        } else {
            $(".type option:contains('ISPS')").show();
        }

        if (e.currentTarget.children[17].textContent == "false") { 
            $(".type option:contains('MLC')").hide();
        } else {
            $(".type option:contains('MLC')").show();
        }

        //GetAndSetIAFFormData(UniqueFormID);

        var blnIsVisitor = '';
        if (_CurrentUserDetailsObject.UserGroup == '8') {
            blnIsVisitor = 'disabled = true';
        }

        var url = AuditRootUrl + "Deficiencies/GetAuditDetails/" + UniqueFormID;
        $.ajax({
            type: 'GET',
            dataType: 'json',
            async: false,
            url: url,
            success: function (Data) {
                data = Data;

                //BindFSTOAuditDetailGrid(data, Subject, blnIsVisitor);
            },
            error: function (data) {
                console.log(data);
            }
        });
        HideAndShow = true;
    });
}
// End JSL 12/31/2022

// JSL 12/31/2022
function BindFSTOAuditDetailGrid(data, Subject, blnIsVisitor) {
    $('#fstoDetailsGrid').empty();
    var grid = $('#fstoDetailsGrid').kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        selectable: true,
        noRecords: true,
        messages: {
            noRecords: "No record found."
        },
        pageable: {
            //pageSize: 5   
            alwaysVisible: true,    
            pageSizes: [5, 10, 20, 100] 
        },
        dataSource: {
            data: data
            , pageSize: 10  
        },
        dataBound: function () {
            $('#fstoDetailsGrid .k-grid-content').css("min-height", "130px");

            setTimeout(function () {
                var gridDataDetail = $('#fstoDetailsGrid').data("kendoGrid");
                gridDataDetail.autoFitColumn("Type");
                gridDataDetail.autoFitColumn("DueDate");
                gridDataDetail.autoFitColumn("IsResolved");
                gridDataDetail.autoFitColumn("AssignTo");
                gridDataDetail.autoFitColumn("Number");
            }, 3000);   

            BindToolTipsForGridText("fstoDetailsGrid", "Deficiency", 45);   
            BindToolTipsForGridText("fstoDetailsGrid", "Reference", 45);   
        },
        change: function () {
            var row = this.select();
            var FormID = row[0].cells[1].textContent;
            var defID = row[0].cells[0].textContent;
            var notesUniqueID = row[0].cells[0].textContent;

            NotesUniqueID = notesUniqueID; 
            //GetAndSetIAFFormNoteData(NotesUniqueID); 

            if (FormID && FormID != "") {
                var _url = RootUrl + "GIRList/InternalAuditDetails?id=" + notesUniqueID;
                window.open(_url, '_blank');
            }
        },
        //detailTemplate: kendo.template($("#CommentsTemplate").html()),
        //detailInit: InitCommentsTemplate,
        columns: [
            {
                field: "NotesUniqueID",
                title: "NotesUniqueID",
                hidden: true,
                attributes: { class: "NotesUniqueID" } 
            },
            {
                field: "NoteID",
                title: "NoteID",
                hidden: true,
            },
            {
                field: "InternalAuditFormId",
                title: "InternalAuditFormId",
                hidden: true,
                attributes: { class: "InternalAuditFormId" }
            },
            {
                field: "Number",
                title: "Number",
                width: "60px"
            },
            {
                field: "Type",
                title: "Type",
            },
            {
                field: "Deficiency",
                title: "Brief Description",
                attributes: { class: "tooltipText" },
            },
            {
                field: "Reference",
                title: Subject == "ISM" ? "SMS References" : Subject == "ISPS" ? "SSP References" : Subject == "MLC" ? "MLC References" : "References",
                attributes: { class: "tooltipText" },
            },
            {
                field: "IsResolved",
                title: "Resolved?",
                template: '<input onclick="updateStatus(this)"  type="checkbox" #= IsResolved ? \'checked="checked"\' : "" # class="chkbx" ' + blnIsVisitor + ' />',
                width: "120px"
            },
            {
                field: "AssignTo",
                title: "Assign To",
                width: "25%",
            },
            {
                field: "DueDate",
                title: "DueDate",
            },
            {
                field: "UpdatedDate",
                title: "Last Update",
                template: "#= UpdatedDate!=null? kendo.toString(kendo.parseDate(UpdatedDate, 'yyyy-MM-dd'), 'dd-MM-yyyy'):'' #",
                width: "20%",
            },
        ]
    });
    setTimeout(function () {
        var gridDataDetail = $('#fstoDetailsGrid').data("kendoGrid");
        gridDataDetail.autoFitColumn("Type");
        gridDataDetail.autoFitColumn("DueDate");
        gridDataDetail.autoFitColumn("IsResolved");
        gridDataDetail.autoFitColumn("AssignTo");
        gridDataDetail.autoFitColumn("Number");
    }, 3000);   
}
// End JSL 12/31/2022

// JSL 02/14/2023
function EditFSTOButton(formType) {
    AllowEditFSTO(false);   // JSL 02/21/2023
    if (formType.toUpperCase() == "FSTO"
        && (_CurrentUserDetailsObject.UserGroup == "1" || _CurrentUserDetailsObject.UserGroup == "5")   // JSL 02/18/2023
    ) {
        $('#btnAddNewFSTO').removeClass("hide"); 
        $("#btnEditFSTO").removeClass("hide");
    }
    else {
        $('#btnAddNewFSTO').addClass("hide");
        $("#btnEditFSTO").addClass("hide");
    }
}
// End JSL 02/14/2023

// JSL 02/18/2023
function AllowEditFSTO(blnEditFSTOOnOff) {
    if (_CurrentUserDetailsObject.UserGroup == "1" || _CurrentUserDetailsObject.UserGroup == "5")
    {
        if (blnEditFSTOOnOff) {
            $("input, select, option, textarea", "#fstoAddEditDetails").prop('disabled', false);
            $("#dvUploadFSTOAttachment").removeClass("hide");
            $("#btnEditFSTO").addClass("hide");
            $("#btnEditFSTODone").removeClass("hide");
        }
        else {
            $("input, select, option, textarea", "#fstoAddEditDetails").prop('disabled', true);
            $("#dvUploadFSTOAttachment").addClass("hide");
            $("#btnEditFSTODone").addClass("hide");
            $("#btnEditFSTO").removeClass("hide");
        }
    }
}
// End JSL 02/18/2023

// JSL 02/14/2023
function BindContextMenuByGridId(strIdWithHash) {
    $("#fsto-context-menu").show();
    $("#fsto-context-menu").kendoContextMenu({
        target: strIdWithHash,
        filter: ".k-grid-content tbody tr[role='row'] td[class='mainContext']",
        select: function (e) {
            var ID = e.target.parentElement.children[0].textContent;
            var item = e.item.id;

            switch (item) {
                case "editDetails":
                    var strUserID = e.target.parentElement.children[1].textContent;
                    var TravelStartedOn = e.target.parentElement.children[3].textContent;
                    var EmbarkedOn = e.target.parentElement.children[4].textContent;
                    var DisembarkedOn = e.target.parentElement.children[5].textContent;
                    var TravelEndedOn = e.target.parentElement.children[6].textContent;
                    var strLocation = e.target.parentElement.children[7].textContent;
                    var strPurposeOfVisit = e.target.parentElement.children[9].textContent;

                    var objFSTOInspection = {
                        ID: ID,
                        strUserID: strUserID,
                        strTravelStartedOn: TravelStartedOn,
                        strEmbarkedOn: EmbarkedOn,
                        strDisembarkedOn: DisembarkedOn,
                        strTravelEndedOn: TravelEndedOn,
                        strLocation: strLocation,
                        strPurposeOfVisit: strPurposeOfVisit
                    };

                    SetValueAndOpenModalForEdit(objFSTOInspection);
                    break;
                case "deleteDetails":
                    var kendoWindow = $("<div />").kendoWindow({
                        width: "300px",
                        height: "100px",
                        title: "Delete ?",
                        resizable: false,
                        modal: true
                    });
                    kendoWindow.data("kendoWindow")
                        .content($("#update-confirmation").html())
                        .center().open();

                    kendoWindow.find(".update-confirm,.update-cancel")
                        .click(function () {
                            if ($(this).hasClass("update-confirm")) {
                                DeleteHelpAndSupport(ID);
                            }
                            kendoWindow.data("kendoWindow").close();
                        })
                        .end();
                    break;
                default:
                    break;
            };
        }
    });
}
// End JSL 02/14/2023

// JSL 02/18/2023
function ValidateEditFSTOFormValidator() {
    EditFSTOFormValidator = $("#EditFSTOForm").validate({
        rules: {
            txtPurposeOfVisit: {
                required: true
            },
        },
        messages: {
            txtPurposeOfVisit: "Please enter purpose of visit!",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
// End JSL 02/18/2023

// JSL 02/14/2023
function ValidateAddNewFSTOFormValidator() {
    AddNewFSTOFormValidator = $("#AddNewFSTOForm").validate({
        rules: {
            txtAddNewPurposeOfVisit: {
                required: true
            },
        },
        messages: {
            txtAddNewPurposeOfVisit: "Please enter purpose of visit!",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
// End JSL 02/14/2023

// JSL 02/14/2023
function openAddNewFSTOModal() {
    $('#modal-addNewFSTOModal').modal({
        backdrop: 'static',
        keyboard: false
    })
    ResetFSTOForm();
    $("#hdnFSTOId").val(CreateNewGuid());   // JSL 02/17/2023
    $("#lblAddEditTitleModal").html('<i class="fa fa-plus-square" aria-hidden="true"></i> New FSTO');
    $('#modal-addNewFSTOModal').modal('show');
};
// End JSL 02/14/2023

// JSL 02/14/2023
function closeFSTOModal() {
    ResetFSTOForm();
    $('#modal-addNewFSTOModal').modal('hide');
};
// End JSL 02/14/2023

// JSL 02/14/2023
function ResetFSTOForm() {
    $("#hdnFSTOId").val('');    // JSL 02/17/2023

    //$("#ddlAddNewShip").prop("selectedIndex", 0);   // JSL 02/18/2023 commented
    $("#ddlAddNewShip").val($("#ddlGIReportShipName option:selected").val());   // JSL 02/21/2023 added actual ship selected   // JSL 02/18/2023
    $("#ddlAddNewUsers").prop("selectedIndex", 0);

    $("#txtAddNewLocation").val('');
    $("#txtAddNewPurposeOfVisit").val('');

    AddNewFSTOFormValidator.resetForm();
};
// End JSL 02/14/2023

// JSL 02/14/2023
function SetValueAndOpenModalForEdit(objFSTOInspection) {
    // JSL 02/18/2023
    $("#fstoAddEditDetails").removeClass("hide");
    EditFSTOFormValidator.resetForm();
    // End JSL 02/18/2023

    var ID = objFSTOInspection.UniqueFormID;
    var strUserID = objFSTOInspection.UserGUID;
    var strShipCode = objFSTOInspection.ShipCode;   // JSL 02/17/2023
    var dtTravelStartedOn = new Date(Date.parse(objFSTOInspection.TravelStartedOn));
    var dtTravelEndedOn = new Date(Date.parse(objFSTOInspection.TravelEndedOn)); 
    var dtEmbarkedOn = new Date(Date.parse(objFSTOInspection.EmbarkedOn)); 
    var dtDisembarkedOn = new Date(Date.parse(objFSTOInspection.DisembarkedOn)); 

    $('#dateTravelStarted').attr('value', moment(dtTravelStartedOn).format('YYYY-MM-DD'));
    $('#dateTravelEnded').attr('value', moment(dtTravelEndedOn).format('YYYY-MM-DD'));
    $('#dateEmbarked').attr('value', moment(dtEmbarkedOn).format('YYYY-MM-DD'));
    $('#dateDisembarked').attr('value', moment(dtDisembarkedOn).format('YYYY-MM-DD'));

    var strLocation = objFSTOInspection.Location;
    var strPurposeOfVisit = objFSTOInspection.PurposeOfVisit;

    $("#lblAddEditTitleModal").html('<i class="fa fa-pencil" aria-hidden="true"></i> Edit FSTO');

    //$("#hdnFSTOId").val(ID);  // JSL 02/18/2023 commented
    $("#ddlShip").val(strShipCode);
    $("#ddlUsers").val(strUserID);

    $("#txtFSTOLocation").val(strLocation);
    $("#txtPurposeOfVisit").val(strPurposeOfVisit);

    // JSL 02/18/2023
    $("#fstoAttachmentList").empty();
    if (!IsNullEmptyOrUndefined(objFSTOInspection.FSTOInspectionAttachments)) {
        var filesList = objFSTOInspection.FSTOInspectionAttachments;
        var htmlOfAttachment = generateTemplateForFSTOFiles(filesList);
        $("#fstoAttachmentList").html(htmlOfAttachment);

        BindDeletePopConfirm(".btnDeleteFSTOAttachment", "left");
    }
    // End JSL 02/18/2023

    // JSL 02/17/2023 commented
    /*$('#modal-addNewFSTOModal').modal({
        backdrop: 'static',
        keyboard: false
    })

    $('#modal-addNewFSTOModal').modal('show');*/
    // End JSL 02/17/2023 commented
};
// End JSL 02/14/2023

// JSL 02/17/2023
function GetFSTOItemsForEdit() {
    $("#fstoGrid tbody").on("click", "tr", function (e) {
        UniqueFormID = e.currentTarget.children[0].textContent
        rowAuditIndex = parseInt($(this).index(), 10) + 1; 

        var dic = {};

        dic["UniqueFormID"] = UniqueFormID;
        dic["FormType"] = "FSTO";

        CommonServerPostApiCall(dic, "Deficiencies", "PerformAction", str_API_GETFSTODETAILSBYID);
    });
}
// End JSL 02/17/2023

// JSL 02/18/2023
function SubmitNewFSTO() {
    if ($("#AddNewFSTOForm").valid()) {
        var obj = CreateFSTOObject("AddNew");
        InsertOrUpdateFSTO(obj);
    }
}
// End JSL 02/18/2023

// JSL 02/18/2023
function SubmitFSTO() {
    if ($("#EditFSTOForm").valid()) {
        var obj = CreateFSTOObject();
        obj["IsActionForUpdate"] = "true";
        InsertOrUpdateFSTO(obj);
    }
}
// End JSL 02/18/2023

// JSL 02/18/2023
function CreateFSTOObject(strAddOrEditElement = "") {   // passed default null to get value for the Edit
    var obj = {};
    if (!IsNullEmptyOrUndefined(strAddOrEditElement))
        obj["UniqueFormID"] = $('#hdnFSTOId').val();
    else
        obj["UniqueFormID"] = UniqueFormID;
    
    obj["UserGUID"] = $('#ddl' + strAddOrEditElement + 'Users option:selected').val();
    obj["ShipCode"] = $('#ddl' + strAddOrEditElement + 'Ship option:selected').val();

    obj["TravelStartedOn"] = $('#date' + strAddOrEditElement + 'TravelStarted').val();
    obj["EmbarkedOn"] = $('#date' + strAddOrEditElement + 'Embarked').val();
    obj["DisembarkedOn"] = $('#date' + strAddOrEditElement + 'Disembarked').val();
    obj["TravelEndedOn"] = $('#date' + strAddOrEditElement + 'TravelEnded').val();

    obj["Location"] = $('#txt' + strAddOrEditElement + 'FSTOLocation').val();
    obj["PurposeOfVisit"] = $('#txt' + strAddOrEditElement + 'PurposeOfVisit').val();

    obj["CurrentUserID"] = _CurrentUserDetailsObject.UserGUID;

    return obj;
}
// End JSL 02/18/2023

// JSL 02/18/2023
function InsertOrUpdateFSTO(objFSTODataForInsertOrUpdate) {
    if (_CurrentUserDetailsObject.UserGroup == "1" || _CurrentUserDetailsObject.UserGroup == "5") {
        $("#lblAutoSave").show();
        var dic = {};
        dic = objFSTODataForInsertOrUpdate;
        CommonServerPostApiCall(dic, "Deficiencies", "PerformAction", str_API_INSERTORUPDATEFSTO);
    }
}
// End JSL 02/18/2023