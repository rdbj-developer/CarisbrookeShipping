var ShipAudits;
var AuditDeficiencyComments;
var AuditDeficiencyCommentFiles;
$(document).ready(function () {
    LoadAuditShips();
    //LoadAuditList();
    LoadAuditDetail();
});

function LoadAuditShips() {
    var url = RootUrl + "Deficiencies/GetAuditShips";
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
                detailTemplate: kendo.template($("#ReportTemplate").html()),
                detailInit: InitShipAudits,
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
function InitShipAudits(e) {
    var detailRow = e.detailRow;
    var code = e.data.Ship;
    var url = RootUrl + "Deficiencies/GetShipAudits";
    locadShipAudits(code, url);
    detailRow.find(".ShipAuditsList").kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        selectable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100]
        },
        dataSource: {
            data: ShipAudits.options.data,
            pageSize: 10
        },
        resizable: true,
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
}
function locadShipAudits(code, url) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        data: { code: code },
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
    var url = RootUrl + "Deficiencies/GetAuditLists";
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
        debugger
        var AuditID = e.currentTarget.children[0].textContent
        var Subject = e.currentTarget.children[1].textContent
        if (AuditID != "" && Subject != "") {
            var url = RootUrl + "Deficiencies/GetAuditDetails/" + parseInt(AuditID);
            $.ajax({
                type: 'GET',
                dataType: 'json',
                async: false,
                url: url,
                data: { id: AuditID },
                success: function (Data) {
                    data = Data;
                    $('#DetailsGrid').empty();
                    var grid = $('#DetailsGrid').kendoGrid({
                        scrollable: true,
                        sortable: true,
                        resizable: true,
                        selectable: true,
                        dataSource: {
                            data: data
                        },
                        dataBound: function () {
                            for (var i = 0; i < this.columns.length; i++) {
                                //  this.autoFitColumn(i);
                            }
                        },
                        detailTemplate: kendo.template($("#CommentsTemplate").html()),
                        detailInit: InitCommentsTemplate,
                        //change: function () {
                        //    var row = this.select();
                        //    var id = row[0].cells[2].textContent;
                        //    var type = row[0].cells[4].textContent;
                        //    var no = row[0].cells[3].textContent;
                        //    var _url;
                        //    if (type == "GI")
                        //        _url = RootUrl + "GIRList/Index?id=" + id + "&isDefectSection=" + true;
                        //    else
                        //        _url = RootUrl + "SIRList/DetailsView?id=" + id + "&isDefectSection=" + true + "&No=" + no;
                        //    window.open(_url, '_blank');
                        //},
                        columns: [
                            {
                                field: "NoteID",
                                title: "NoteID",
                                hidden: true,
                                attributes: { class: "NoteID" }
                            },
                             {
                                 field: "Type",
                                 title: "Type",
                             },
                             {
                                 field: "Deficiency",
                                 title: "Deficiency",
                             },
                            {
                                field: "Reference",
                                title: Subject == "ISM" ? "SMS References" : Subject == "ISPS" ? "SSP References" : Subject == "MLC" ? "MLC References" : "References",
                            },
                            {
                                field: "DueDate",
                                title: "DueDate",
                            },
                             {
                                 field: "IsResolved",
                                 title: "Resolved?",
                                 template: '<input onclick="updateStatus(this)"  type="checkbox" #= IsResolved ? \'checked="checked"\' : "" # class="chkbx"  />',
                                 width: "120px"
                             },
                        ]
                    });
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}
function updateStatus(ctr) {
    var NoteID = $(ctr).parent().parent().find('.NoteID').text();
    var url = RootUrl + "Deficiencies/UpdateAuditDeficiencies";
    var obj = {};
    obj.id = NoteID;
    obj.isClose = $(ctr).prop('checked');
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        data: obj,
        success: function (Data) {
            $.notify("Updated Successfully", "success");
            LoadAuditShips();
            LoadAuditDetail();
        },
        error: function (Data) {
            console.log(Data);
        }
    });
}

function AddComment(id, ctr) {
    var name = $(ctr).parent().parent().find('.name').val();
    var url = RootUrl + "Deficiencies/AddAuditDeficiencyComments";
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
            var url = RootUrl + "Deficiencies/GetAuditDeficiencyComments";
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
    var url = RootUrl + "Deficiencies/GetAuditDeficiencyComments";
    var urlFile = RootUrl + "Deficiencies/GetAuditDeficiencyCommentFiles";
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
function generateTemplate(ReportList) {
    var template = "";
    for (var i = 0; i < ReportList.length; i++) {
        template = template + '<div style="line-height: unset;" class="chip pink lighten-2 white-text waves-effect waves-effect file">' +
            '<a onclick="DownloadCommentFile(' + ReportList[i].CommentFileID + ',this)">' + ReportList[i].FileName + '</a>' +
            '</div>';
    }
    return template;
}
function DownloadCommentFile(ctr, data) {
    var _name = $(data).text();
    var url = RootUrl + "Deficiencies/DownloadCommentFile?CommentFileID=" + ctr + "&name=" + _name;;
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
    if (typeof (FileReader) != "undefined") {
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
    }
}
function RemoveFile(ctr) {
    $(ctr).parent().next().next().next().val('false')
    $(ctr).parent().hide();
}