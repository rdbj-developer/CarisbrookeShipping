var AddNewHelpAndSupportFormValidator = undefined; // RDBJ 12/30/2021

$(document).ready(function () {
    $('#ddlShip').prop("disabled", true);
    //autoRefresh(); // RDBJ 12/31/2021
    ValidateAddNewHelpAndSupportFormValidator(); // RDBJ 12/30/2021
    GetHelpAndSupportsList(); // RDBJ 12/30/2021
    SearchDataFromGrid(); // RDBJ 12/31/2021
});

// RDBJ 12/31/2021
function autoRefresh() {
    var intervalId = window.setInterval(function () {
        GetHelpAndSupportsList();
    }, 10000); //set 10 Seconds to refresh this page
};
// End RDBJ 12/31/2021

// RDBJ 12/30/2021
function GetHelpAndSupportsList() {
    var url = RootUrl + "Help/GetHelpAndSupportsList";
    $.ajax({
        type: 'POST',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            data = Data;
            $('#HelpSupportsGrid').empty();
            var grid = $('#HelpSupportsGrid').kendoGrid({
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
                    pageSize: 20
                },
                dataSource: {
                    data: data,
                    //sort: { field: "Date", dir: "desc" }
                },
                dataBound: function (e) {
                    //for (var i = 0; i < this.columns.length; i++) {
                    //    this.autoFitColumn(i);
                    //}

                    $('#HelpSupportsGrid').on("mousedown", "tr[role='row']", function (e) {
                        if (e.which === 3) {
                            $("tr").removeClass("k-state-selected");
                            $(this).toggleClass("k-state-selected");
                        }
                    });
                },
                toolbar: [{ template: kendo.template($("#template").html()) }],
                columns: [
                    {
                        field: "Id",
                        title: "Id",
                        hidden: true,
                    },
                    {
                        field: "ShipId",
                        title: "Ship",
                        width: "12%",
                        attributes: { class: "mainContext" }, // RDBJ 12/31/2021
                    },
                    {
                        field: "Comments",
                        title: "Comments",
                        width: "50%",
                        attributes: { class: "mainContext" }, // RDBJ 12/31/2021
                    },
                    {
                        field: "CreatedBy",
                        title: "Created By",
                        width: "18%",
                        attributes: { class: "mainContext" }, // RDBJ 12/31/2021
                    },
                    // RDBJ 12/31/2021
                    {
                        //field: "IsStatus",
                        //template: "#= templateGenerateStatuscolumn(IsStatus)#",
                        field: "StrStatus",
                        title: "Status",
                        template: "#= templateGenerateStatusStrcolumn(StrStatus)#",
                        width: "10%",
                        attributes: { class: "mainContext" }, // RDBJ 12/31/2021
                    },
                    // End RDBJ 12/31/2021
                    // RDBJ 12/31/2021
                    {
                        //field: "Priority",
                        //template: "#= templateGeneratePrioritycolumn(Priority)#",
                        field: "StrPriority",
                        title: "Priority",
                        template: "#= templateGeneratePriorityStrcolumn(StrPriority)#",
                        width: "10%",
                        attributes: { class: "mainContext" }, // RDBJ 12/31/2021
                    },
                    // End RDBJ 12/31/2021
                    {
                        field: "CreatedDateTime",
                        title: "Created Date",
                        template: "#= CreatedDateTime!=null? kendo.toString(kendo.parseDate(CreatedDateTime, 'yyyy-MM-dd'), 'dd-MM-yyyy HH:mm'):'' #",
                        type: "date",
                        //format: "{0:dd-MM-yyyy HH:mm}",
                        width: "15%",
                        attributes: { class: "mainContext" }, // RDBJ 12/31/2021
                        filterable: {
                            ui: function (element) {
                                element.kendoDatePicker({
                                    format: '{0: dd-MM-yyyy}'
                                })
                            }
                        }
                    },
                ]
            });

            $("#context-menu").show();
            $("#context-menu").kendoContextMenu({
                filter: ".k-grid-content tbody tr[role='row'] td[class='mainContext']",
                select: function (e) {
                    //var row = $(e.target).parent()[0];
                    //var grid = $('#HelpSupportsGrid').data("kendoGrid");

                    var ID = e.target.parentElement.children[0].textContent; // RDBJ 12/31/2021
                    var item = e.item.id; // RDBJ 12/31/2021

                    // RDBJ 12/31/2021
                    switch (item) {
                        case "editDetails":
                            var strShipID = e.target.parentElement.children[1].textContent;
                            var strComments = e.target.parentElement.children[2].textContent;
                            var strStatus = e.target.parentElement.children[4].textContent;
                            var strPriority = e.target.parentElement.children[5].textContent;
                            SetValueAndOpenModalForEdit(ID, strShipID, strComments, strStatus, strPriority);
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
                    // End RDBJ 12/31/2021
                }
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End RDBJ 12/30/2021

// RDBJ 12/30/2021
function ValidateAddNewHelpAndSupportFormValidator() {
    AddNewHelpAndSupportFormValidator = $("#AddNewHelpAndSupportForm").validate({
        rules: {
            txtMessage: {
                required: true
            },
        },
        messages: {
            txtMessage: "Please Enter Details!",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}
// End RDBJ 12/30/2021

// RDBJ 12/30/2021
function SubmitNewHelpAndSupport() {
    $("#lblAutoSave").show();
    if ($("#AddNewHelpAndSupportForm").valid()) {
        var url = RootUrl + 'Help/InsertOrUpdateHelpAndSupport';

        var Modal = {
            Id: $("#hdnId").val(),
            ShipId: $("#ddlShip option:selected").val(),
            Comments: $("#txtMessage").val(),
            IsStatus: $("#ddlStatus option:selected").val(),
            Priority: $("#ddlPriority option:selected").val(),
        };

        $.ajax({
            url: url,
            type: 'POST',
            async: true,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Modal: Modal }),
            success: function (res) {
                closeHelpAndSupportModal();

                $('#HelpSupportsGrid').empty();
                GetHelpAndSupportsList();

                $("#lblAutoSave").hide();
            },
            error: function (err) {
                $("#lblAutoSave").hide();
            }
        });

    }
};
// End RDBJ 12/30/2021

// RDBJ 12/27/2021
function openAddNewHelpAndSupportModal() {
    $('#modal-addNewSupportModal').modal({
        backdrop: 'static',
        keyboard: false
    })
    ResetForm(); // RDBJ 12/31/2021
    $("#lblAddEditTitleModal").html('<i class="fa fa-plus-square" aria-hidden="true"></i> Add New Support'); // RDBJ 12/30/2021
    $('#modal-addNewSupportModal').modal('show');
};
// End RDBJ 12/27/2021

// RDBJ 12/27/2021
function closeHelpAndSupportModal() {
    ResetForm();
    $('#modal-addNewSupportModal').modal('hide');
};
// End RDBJ 12/27/2021

// RDBJ 12/27/2021
function ResetForm() {
    $("#ddlStatus").prop("selectedIndex", 0);
    $("#ddlPriority").prop("selectedIndex", 1);

    $("#txtMessage").val('');
    $("#hdnId").val('');

    AddNewHelpAndSupportFormValidator.resetForm();
};
// End RDBJ 12/27/2021

// RDBJ 12/31/2021 later use if needed
function templateGenerateStatuscolumn(IsStatus) {
    var strReturnTemplate = '<div class="{class}" style="text-align:center; margin: 2px;">';
    strReturnTemplate += '<span class="k-padding">{isStatus}';
    var strClass = "", strStatus = "";

    if (IsStatus == "0") {
        strStatus = 'Open';
        strClass = "bg-red";
    }
    else if (IsStatus == "1") {
        strStatus = 'Closed';
        strClass = "bg-light-green";
    }

    strReturnTemplate += '</span>';
    strReturnTemplate = strReturnTemplate.replace("{class}", strClass);
    strReturnTemplate = strReturnTemplate.replace("{isStatus}", strStatus);
    strReturnTemplate += '</div>';

    return strReturnTemplate;
}
// End RDBJ 12/31/2021 later use if needed

// RDBJ 12/31/2021 later use if needed
function templateGeneratePrioritycolumn(IsPriority) {
    var strReturnTemplate = '<div class="{class}" style="text-align:center; margin: 2px;">';
    strReturnTemplate += '<span class="k-padding">{IsPriority}';
    var strClass = "", strPriority = "";

    if (IsPriority == "1") {
        strPriority = 'Low';
        strClass = "bg-light-green";
    }
    else if (IsPriority == "2") {
        strPriority = 'Medium';
        strClass = "bg-yellow-active";
    }
    else if (IsPriority == "3") {
        strPriority = 'High';
        strClass = "bg-light-orange";
    }
    else if (IsPriority == "4") {
        strPriority = 'Critical';
        strClass = "bg-red";
    }

    strReturnTemplate += '</span>';
    strReturnTemplate = strReturnTemplate.replace("{class}", strClass);
    strReturnTemplate = strReturnTemplate.replace("{IsPriority}", strPriority);
    strReturnTemplate += '</div>';

    return strReturnTemplate;
}
// End RDBJ 12/31/2021 later use if needed

// RDBJ 12/31/2021
function templateGenerateStatusStrcolumn(StrStatus) {
    var strReturnTemplate = '<div class="{class}" style="text-align:center; margin: 2px;">';
    strReturnTemplate += '<span class="k-padding">{isStatus}';
    var strClass = "", strStatus = "";

    if (StrStatus == "Open") {
        strClass = "bg-red";
    }
    else if (StrStatus == "Closed") {
        strClass = "bg-light-green";
    }

    strReturnTemplate += '</span>';
    strReturnTemplate = strReturnTemplate.replace("{class}", strClass);
    strReturnTemplate = strReturnTemplate.replace("{isStatus}", StrStatus);
    strReturnTemplate += '</div>';

    return strReturnTemplate;
}
// End RDBJ 12/31/2021

// RDBJ 12/31/2021
function templateGeneratePriorityStrcolumn(StrPriority) {
    var strReturnTemplate = '<div class="{class}" style="text-align:center; margin: 2px;">';
    strReturnTemplate += '<span class="k-padding">{IsPriority}';
    var strClass = "", strPriority = "";

    if (StrPriority == "Low") {
        strClass = "bg-light-green";
    }
    else if (StrPriority == "Medium") {
        strClass = "bg-yellow-active";
    }
    else if (StrPriority == "High") {
        strClass = "bg-light-orange";
    }
    else if (StrPriority == "Critical") {
        strClass = "bg-red";
    }

    strReturnTemplate += '</span>';
    strReturnTemplate = strReturnTemplate.replace("{class}", strClass);
    strReturnTemplate = strReturnTemplate.replace("{IsPriority}", StrPriority);
    strReturnTemplate += '</div>';

    return strReturnTemplate;
}
// End RDBJ 12/31/2021

// RDBJ 12/31/2021
function SetValueAndOpenModalForEdit(ID, strShipID, strComments, strStatus, strPriority) {
    $("#lblAddEditTitleModal").html('<i class="fa fa-pencil" aria-hidden="true"></i> Edit Support'); // RDBJ 12/30/2021

    $("#hdnId").val(ID);
    setDropDownSelectedIndexByTextValue('ddlShip', strShipID);
    $("#txtMessage").val(strComments);

    if (strStatus != "Open") {
        strStatus = "Close";
    }

    setDropDownSelectedIndexByTextValue('ddlStatus', strStatus);
    setDropDownSelectedIndexByTextValue('ddlPriority', strPriority);

    $('#modal-addNewSupportModal').modal({
        backdrop: 'static',
        keyboard: false
    })

    $('#modal-addNewSupportModal').modal('show');
};
// End RDBJ 12/31/2021

// RDBJ 12/31/2021
function setDropDownSelectedIndexByTextValue(ddlId, textToFind) {
    var dd = document.getElementById(ddlId);
    for (var i = 0; i < dd.options.length; i++) {
        if (dd.options[i].text === textToFind) {
            dd.selectedIndex = i;
            break;
        }
    }
};
// End RDBJ 12/31/2021

// RDBJ 12/31/2021
function SearchDataFromGrid() {
    $("#searchBox").keyup(function () {
        var searchValue = $('#searchBox').val();
        $("#HelpSupportsGrid").data("kendoGrid").dataSource.filter({
            logic: "or",
            filters: [
                {
                    field: "ShipId",
                    operator: "contains",
                    value: searchValue
                },
                {
                    field: "Comments",
                    operator: "contains",
                    value: searchValue
                },
                {
                    field: "CreatedBy",
                    operator: "contains",
                    value: searchValue
                },
                //{
                //    field: "CreatedDateTime",
                //    operator: "contains",
                //    value: searchValue
                //},
                {
                    field: "StrStatus",
                    operator: "contains",
                    value: searchValue
                },
                {
                    field: "StrPriority",
                    operator: "contains",
                    value: searchValue
                }
            ]
        });
    });
}
// End RDBJ 12/31/2021

// RDBJ 12/31/2021
function DeleteHelpAndSupport(ID) {
    $("#lblAutoSave").show();
    var url = RootUrl + 'Help/DeleteHelpAndSupport';
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        dataType: 'json',
        data: { ID: ID },
        success: function (res) {
            $('#HelpSupportsGrid').empty();
            GetHelpAndSupportsList();

            $("#lblAutoSave").hide();
        },
        error: function (err) {
            $("#lblAutoSave").hide();
        }
    });
}
// End RDBJ 12/31/2021