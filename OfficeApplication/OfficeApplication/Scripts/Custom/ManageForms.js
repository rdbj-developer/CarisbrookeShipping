$(document).ready(function () {
    $('#btnSubmitForm').attr('disabled', true);
    $('#btnRemoveForm').attr('disabled', true);
    $('#btnEditForm').attr('disabled', true);
    $(".btnAddNew").click(function () {
        $("#lblFormFileName").hide();
        var btnVal = $(this).val();
        if (btnVal == "RemoveObj") {
            RemoveFormObj();
        }
        if (btnVal == "CancelChanges") {
            CancelFormChanges();
        }
    });
    $("#btnEditForm").click(function () {
        EditForm();
    })
    $(".AddFileUpload").click(function (e) {
        AddFormFile(e);
    });
    $(".UpdateFileUpload").click(function (e) {
        UpdateFormFile(e);
    });
})

function AddFormFile(e) {
    $('#btnRemoveForm').attr('disabled', true);
    var isDisable = $('#btnAddFormFileUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#txtFormID").val(generateGUID());
        $("#txtFormPath").text("C:\\ProgramData\\Carisbrooke Shipping Ltd\\ISM Dashboard\\Repository\\Forms");
        $("#hdnFormClickedType").val("AddFormFile");
        $('#btnEditForm').attr('disabled', true);
        $('#btnSubmitForm').attr('disabled', false);
        $("#txtCode").removeAttr("readonly");
        $("#txtIssue").removeAttr("readonly");
        $("#txtIssueDate").removeAttr("readonly");
        $("#txtAmendment").removeAttr("readonly");
        $("#txtAmendmentDate").removeAttr("readonly");
        $("#chkAllowsNetworkAccess").prop("disabled", false);
        $("#chkCanBeOpened").prop("disabled", false);
        $("#chkHasSavedData").prop("disabled", false);
        $("#chkIsURNBased").prop("disabled", false);
        $("#txtDepartment").removeAttr("readonly");
        $("#txtCategory").removeAttr("readonly");
        $("#txtURN").removeAttr("readonly");
        $("#txtAccessLevel").removeAttr("readonly");
        // $("#txtFormPath").prop("readonly", "readonly");
    }
}
function UpdateFormFile(e) {
    $('#btnRemoveForm').attr('disabled', true);
    var isDisable = $('#btnUpdateFormFileUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#hdnFormClickedType").val("UpdateForm");
        var selectedLength = $(".fancytree-active").length;
        if (selectedLength == 1) {
            $('#btnEditForm').attr('disabled', true);
            $('#btnSubmitForm').attr('disabled', false);
            $('#btnAddFormRootFolder').prop('disabled', true);
            $('#btnAddNewFormFolder').prop('disabled', true);
            $("#txtFormPath").prop("readonly", "readonly");
        }
        else {
            alert("Plese select the file from left side");
            e.preventDefault();
        }
    }
}

function EditForm() {
    var selectedLength = $("#FormsTree").length;// $(".fancytree-active").length;
    if (selectedLength == 1) {
        $("#hdnFormClickedType").val("EditForm");
        $("#txtFormTitle").removeAttr("readonly");
        //$("#txtFormPath").prop("readonly", "readonly");
        $("#txtCode").removeAttr("readonly");
        $("#txtIssue").removeAttr("readonly");
        $("#txtIssueDate").removeAttr("readonly");
        $("#txtAmendment").removeAttr("readonly");
        $("#txtAmendmentDate").removeAttr("readonly");
        $("#chkAllowsNetworkAccess").prop("disabled", false);
        $("#chkCanBeOpened").prop("disabled", false);
        $("#chkHasSavedData").prop("disabled", false);
        $("#chkIsURNBased").prop("disabled", false);
        $("#txtDepartment").removeAttr("readonly");
        $("#txtCategory").removeAttr("readonly");
        $("#txtURN").removeAttr("readonly");
        $("#txtAccessLevel").removeAttr("readonly");
        $('#btnAddFormFileUpload').attr('disabled', true);
        $(".AddFileUpload").prop('disabled', true);
        $('#btnRemoveForm').attr('disabled', true);
        $('#btnSubmitForm').attr('disabled', false);
        $('#btnUpdateFormFileUpload').attr('disabled', true);
        $(".UpdateFileUpload").prop('disabled', true);
    }
    else {
        alert("Plese select the folder from left side");
        e.preventDefault();
    }
}

function RemoveFormObj() {
    $("#hdnFormClickedType").val("RemoveObj");
    var selectedLength = $(".fancytree-active").length;
    if (selectedLength == 1) {
        var r = confirm("Are you sure? you want to delete this?");
        if (r) {
            $("#frmManageForm").submit();
        }
    }
    else {
        alert("Plese select the folder from left side");
    }
}

function CancelFormChanges() {
    $("#hdnFormClickedType").val("CancelChanges");
    $('#btnSubmitForm').attr('disabled', true);
    $(".fancytree-node").removeClass("fancytree-active");

    $("#txtFormID").val("");
    $("#txtCode").val("");
    $("#txtFormPath").text("");
    $("#txtIssue").val("");
    $("#txtIssueDate").val("");
    $("#txtFormTitle").val("");
    $("#txtAmendment").val("");
    $("#txtAmendmentDate").val("");
    $("#txtDepartment").val("");
    $("#txtCategory").val("");
    $("#txtURN").val("");
    $("#txtAccessLevel").val("");
    $("#hdnFormCreatedDate").val("");
    $("#hdnFormDownloadPath").val("");
    $("#hdnType").val("");
    setCheckboxCheckedValue("#chkAllowsNetworkAccess", "false");
    setCheckboxCheckedValue("#chkCanBeOpened", "false");
    setCheckboxCheckedValue("#chkHasSavedData", "false");
    setCheckboxCheckedValue("#chkIsURNBased", "false");

    $("#txtFormTitle").removeAttr("readonly");
    $("#txtFormPath").removeAttr("readonly");
    $("#txtCode").removeAttr("readonly");
    $("#txtIssue").removeAttr("readonly");
    $("#txtIssueDate").removeAttr("readonly");
    $("#txtAmendment").removeAttr("readonly");
    $("#txtAmendmentDate").removeAttr("readonly");
    $("#txtDepartment").removeAttr("readonly");
    $("#txtCategory").removeAttr("readonly");
    $("#txtURN").removeAttr("readonly");
    $("#txtAccessLevel").removeAttr("readonly");
    $("#chkAllowsNetworkAccess").prop("disabled", false);
    $("#chkCanBeOpened").prop("disabled", false);
    $("#chkHasSavedData").prop("disabled", false);
    $("#chkIsURNBased").prop("disabled", false);

    $('#btnAddFormFileUpload').attr('disabled', false);
    // $(".AddFileUpload").prop('disabled', false);
    $('#btnRemoveForm').attr('disabled', true);
    $('#btnEditForm').attr('disabled', true);
    $(".UpdateFileNew").hide();
    $(".AddFileNew").show();
}

function GetFormFileData(myFile) {
    var file = myFile.files[0];
    var filename = file.name;
    $("#lblFormFileName").text("");
    $("#lblFormFileName").text(filename);
    $("#lblFormFileName").show();
}

function SuccessMsg(ClickedType) {
    if (ClickedType == "AddFolder" || ClickedType == "AddRoot") {
        $.notify("Folder Added Successfully", "success");
    }
    if (ClickedType == "AddFormFile") {
        $.notify("File Added Successfully", "success");
    }
    if (ClickedType == "RemoveObj") {
        $.notify("Removed Successfully", "success");
    }
    if (ClickedType == "EditForm") {
        $.notify("Form Updated Successfully", "success");
    }
    if (ClickedType == "UpdateForm") {
        $.notify("Form Updated Successfully", "success");
    }
}

function ErrorMsg(ClickedType) {
    if (ClickedType == "AddFormFolder" || ClickedType == "AddFormRoot") {
        $.notify("Error occured while adding folder", "error");
    }
    if (ClickedType == "AddFormFile") {
        $.notify("Error occured while adding file", "error");
    }
    if (ClickedType == "RemoveFormObj") {
        $.notify("Error occured while removing file", "error");
    }
    if (ClickedType == "EditForm") {
        $.notify("Error occured while updating form", "success");
    }
    if (ClickedType == "UpdateForm") {
        $.notify("Error occured while updating form", "success");
    }
}

function InitFormsTree() {
    $("#FormsTree").fancytree({
        click: function (event, data) {
            var node = data.node;
            var selectedType = node.type;
            $('#btnAddNewFormFolder').attr('disabled', true);
            $('#btnAddFormFileUpload').attr('disabled', true);
            $(".AddFileNew").hide();
            $('#btnUpdateFormFileUpload').attr('disabled', false);
            $(".UpdateFileUpload").prop('disabled', false);
            $(".UpdateFileNew").show();
            $('#btnSubmitForm').attr('disabled', true);
            $('#btnRemoveForm').attr('disabled', false);
            $('#btnEditForm').attr('disabled', false);
            $('#btnAddFormRootFolder').attr('disabled', false);
            $("#txtFormID").val(node.data.formid);
            $("#txtCode").val(node.data.code);
            $("#txtFormPath").text(node.data.path);
            $("#txtIssue").val(node.data.issue);
            if (node.data.issuedate !== "") {
                $("#txtIssueDate").datepicker("update", parseDate(node.data.issuedate));
            }            
            $("#txtFormTitle").val(node.title);
            $("#txtAmendment").val(node.data.amendment);
            if (node.data.amendmentdate !== "") {
                $("#txtAmendmentDate").datepicker("update", parseDate(node.data.amendmentdate));
            }            
            $("#txtURN").val(node.data.urn);
            $("#txtDepartment").val(node.data.department);
            $("#txtCategory").val(node.data.category);
            $("#txtAccessLevel").val(node.data.accesslevel);
            $("#hdnFormCreatedDate").val(node.data.createddate);
            $("#hdnFormDownloadPath").val(node.data.downloadpath);
            $("#hdnType").val(node.data.doctype);
            $("#hdnUploadType").val(node.data.uploadtype);
            setCheckboxCheckedValue("#chkAllowsNetworkAccess", node.data.allowsnetworkaccess);
            setCheckboxCheckedValue("#chkCanBeOpened", node.data.canbeopened);
            setCheckboxCheckedValue("#chkHasSavedData", node.data.hassaveddata);
            setCheckboxCheckedValue("#chkIsURNBased", node.data.isurnbased);
            $("#txtFormID").prop("readonly", "readonly");
            $("#txtCode").prop("readonly", "readonly");
            $("#txtFormTitle").prop("readonly", "readonly");
            $("#txtFormPath").prop("readonly", "readonly");
            $("#txtIssue").prop("readonly", "readonly");
            $("#txtIssueDate").prop("readonly", "readonly");
            $("#txtAmendment").prop("readonly", "readonly");
            $("#txtAmendmentDate").prop("readonly", "readonly");
            $("#chkAllowsNetworkAccess").prop("disabled", "disabled");
            $("#chkCanBeOpened").prop("disabled", "disabled");
            $("#chkHasSavedData").prop("disabled", "disabled");
            $("#chkIsURNBased").prop("disabled", "disabled");
            $("#txtDepartment").prop("readonly", "readonly");
            $("#txtCategory").prop("readonly", "readonly");
            $("#txtAccessLevel").prop("readonly", "readonly");
            $("#txtURN").prop("readonly", "readonly");

        },        
    });
}

//Parse dd/mm/yyyy string  to date
function parseDate(dateString) {
    var parts = dateString.split("/");
    return new Date(parseInt(parts[2], 10),
        parseInt(parts[1], 10) - 1,
        parseInt(parts[0], 10));
}

function setCheckboxCheckedValue(obj, value) {
    if (value.toLowerCase() == "true") $(obj).prop('checked', true);
    else $(obj).prop('checked', false);
}

function BindFormsTree() {
    $("#ajax_loader").show();
    $.ajax({
        type: "GET",
        url: '@Url.Action("FormsTreeView", "Admin")?sectionType=' + $("#optSectionType").val(),
        async: true,
        cache: false,
        dataType: "html",
        success: function (result) {
            $("#FormsTree").html('@Html.Raw("' + result + '")');
            CancelChanges();
            try {
                var objTree = $(".sampletree").fancytree();
                if (objTree != undefined && objTree != null) {
                    // using default options
                    $("#FormsTree").fancytree("destroy");
                }
                InitFormsTree();
            } catch (e) {

            }

            $("#ajax_loader").hide();
        },
        error: function (err) {
            $("#ajax_loader").hide();
        }
    });
}