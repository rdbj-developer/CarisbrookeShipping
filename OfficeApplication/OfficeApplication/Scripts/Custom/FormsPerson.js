var AddNewFormsPersonFormValidator = undefined;

$(document).ready(function () {
    ValidateAddNewFormsPersonFormValidator();
    LoadFormsPersonGrid();
});

function LoadFormsPersonGrid() {
    var dic = {};
    CommonServerPostApiCall(dic, "Admin", "PerformAction", str_API_GETFORMSPERSONLIST);
}

function BindFormsPersonGrid(data) {
    $('#FormsPersonGrid').empty();
    var grid = $('#FormsPersonGrid').kendoGrid({
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: false,
        selectable: true,
        noRecords: true,
        messages: {
            noRecords: "No record found."
        },
        pageable: {
            pageSizes: [5, 10, 20, 100]
        },
        dataSource: {
            data: data,
            pageSize: 5
        },
        dataBound: function () {
            $('#FormsPersonGrid').on("mousedown", "tr[role='row']", function (e) {
                if (e.which === 3) {
                    $("tr").removeClass("k-state-selected");
                    $(this).toggleClass("k-state-selected");
                }
            });
        },
        columns: [
            {
                field: "Id",
                title: "Id",
                hidden: true,
            },
            {
                field: "UniqueId",
                title: "UniqueId",
                hidden: true,
            },
            {
                field: "PersonName",
                title: "Person",
                attributes: { class: "mainContext" },
            },
            {
                field: "PersonType",
                title: "Type",
                template: "#= templateGeneratePersonTypeStrcolumn(PersonType)#",
                attributes: { class: "mainContext" },
            },
            {
                field: "CreatedDateTime",
                title: "Created On",
                template: "#= CreatedDateTime!=null? kendo.toString(kendo.parseDate(CreatedDateTime, 'yyyy-MM-dd'), 'dd-MM-yyyy HH:mm'):'' #",
                type: "date",
                width: "15%",
                attributes: { class: "mainContext" },
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
            var ID = e.target.parentElement.children[1].textContent; 
            var item = e.item.id; 
            switch (item) {
                case "editDetails":
                    var strPersonName = e.target.parentElement.children[2].textContent;
                    var strPersonType = e.target.parentElement.children[3].textContent;
                    SetValueAndOpenModalForEdit(ID, strPersonName, strPersonType);
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
                                DeleteFormsPerson(ID);
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

function templateGeneratePersonTypeStrcolumn(PersonType) {
    var strReturnTemplate = '';

    if (PersonType == "1") {
        strReturnTemplate += "Auditor";
    }
    
    return strReturnTemplate;
}

function openAddNewFormsPersonModal() {
    $('#modal-addNewFormsPerson').modal({
        backdrop: 'static',
        keyboard: false
    })
    ResetForm(); 
    $("#lblAddEditTitleModal").html('<i class="fa fa-plus-square" aria-hidden="true"></i> Add New Person');
    $('#modal-addNewFormsPerson').modal('show');
};

function closeFormsPersonModal() {
    ResetForm();
    $('#modal-addNewFormsPerson').modal('hide');
};

function ValidateAddNewFormsPersonFormValidator() {
    AddNewFormsPersonFormValidator = $("#AddNewFormsPersonForm").validate({
        rules: {
            txtPersonName: {
                required: true
            },
        },
        messages: {
            txtPersonName: "Please Enter Person Name!",
        },
        errorPlacement: function (error, element) {
            $(error).insertAfter($(element).parent().find(".form-control-feedback"));
        }
    });
}

function SubmitNewFormsPerson() {
    if ($("#AddNewFormsPersonForm").valid()) {
        var dic = {
            UniqueId: $("#hdnId").val(),
            PersonName: $("#txtPersonName").val(),
            PersonType: $("#ddPersonType option:selected").val(),
            CurrentUserID: _CurrentUserDetailsObject.UserGUID,
        };

        CommonServerPostApiCall(dic, "Admin", "PerformAction", str_API_API_ADDNEW_UPDATE_DELETE_FORMSPERSON);
    }
};

function DeleteFormsPerson(ID) {
    var dic = {
        UniqueId: ID,
        IsDelete: true,
        CurrentUserID: _CurrentUserDetailsObject.UserGUID,
    };

    CommonServerPostApiCall(dic, "Admin", "PerformAction", str_API_API_ADDNEW_UPDATE_DELETE_FORMSPERSON);
};

function ResetForm() {
    $("#txtPersonName").val('');
    $("#ddPersonType").prop("selectedIndex", 0);
    $("#hdnId").val('');

    AddNewFormsPersonFormValidator.resetForm();
};

function SetValueAndOpenModalForEdit(ID, strPersonName, strPersonType) {
    $("#lblAddEditTitleModal").html('<i class="fa fa-pencil" aria-hidden="true"></i> Edit Person');
    $("#hdnId").val(ID);
    $("#txtPersonName").val(strPersonName);
    setDropDownSelectedIndexByTextValue('ddPersonType', strPersonType);

    $('#modal-addNewFormsPerson').modal({
        backdrop: 'static',
        keyboard: false
    })

    $('#modal-addNewFormsPerson').modal('show');
};
