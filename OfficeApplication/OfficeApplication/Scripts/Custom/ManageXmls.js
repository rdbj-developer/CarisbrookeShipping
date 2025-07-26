$(document).ready(function () {
    $('#btnSubmitXml').attr('disabled', true);
    $('#btnRemoveXml').attr('disabled', true);
    $('#btnEditXml').attr('disabled', true);
    $(".btnAddNewXml").click(function () {
        $("#lblXmlFileName").hide();
        var btnVal = $(this).val();
        if (btnVal == "RemoveObj") {
            RemoveXmlObj();
        }
        if (btnVal == "CancelChanges") {
            CancelXmlChanges();
        }
    });
    $("#btnEditXml").click(function () {
        EditXml();
    })
    $(".AddFileUploadXml").click(function (e) {
        AddXmlFile(e);
    });
    $(".UpdateFileUploadXml").click(function (e) {
        UpdateXmlFile(e);
    });
})

function AddXmlFile(e) {
    $('#btnRemoveXml').attr('disabled', true);
    var isDisable = $('#btnAddXmlFileUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#txtXmlID").val(generateGUID());
        $("#txtXmlPath").text("C:\\ProgramData\\Carisbrooke Shipping Ltd\\ISM Dashboard\\Repository\\Xml");
        $("#hdnXmlClickedType").val("AddFormFile");
        $('#btnEditXml').attr('disabled', true);
        $('#btnSubmitXml').attr('disabled', false);
        $("#chkCanBeOpenedXml").prop("disabled", false);      
    }
}
function UpdateXmlFile(e) {
    $('#btnRemoveXml').attr('disabled', true);
    var isDisable = $('#btnUpdateXmlFileUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#hdnXmlClickedType").val("UpdateForm");
        var selectedLength = $("#XmlsTree .fancytree-active").length;
        if (selectedLength == 1) {
            $('#btnEditXml').attr('disabled', true);
            $('#btnSubmitXml').attr('disabled', false);
            $('#btnAddXmlRootFolder').prop('disabled', true);
            $('#btnAddNewXmlFolder').prop('disabled', true);
            $("#txtXmlPath").prop("readonly", "readonly");
        }
        else {
            alert("Plese select the file from left side");
            e.preventDefault();
        }
    }
}

function EditXml() {
    var selectedLength = $("#XmlsTree").length;// $(".fancytree-active").length;
    if (selectedLength == 1) {
        $("#hdnXmlClickedType").val("EditForm");
        $("#txtXmlTitle").removeAttr("readonly");        
        $("#chkCanBeOpenedXml").prop("disabled", false);
        $('#btnAddXmlFileUpload').attr('disabled', true);
        $(".AddFileUploadXml").prop('disabled', true);
        $('#btnRemoveXml').attr('disabled', true);
        $('#btnSubmitXml').attr('disabled', false);
        $('#btnUpdateXmlFileUpload').attr('disabled', true);
        $(".UpdateFileUploadXml").prop('disabled', true);
    }
    else {
        alert("Plese select the folder from left side");
        e.preventDefault();
    }
}

function RemoveXmlObj() {
    $("#hdnXmlClickedType").val("RemoveObj");
    var selectedLength = $("#XmlsTree .fancytree-active").length;
    if (selectedLength == 1) {
        var r = confirm("Are you sure? you want to delete this?");
        if (r) {
            $("#frmManageXml").submit();
        }
    }
    else {
        alert("Plese select the folder from left side");
    }
}

function CancelXmlChanges() {
    $("#hdnXmlClickedType").val("CancelChanges");
    $('#btnSubmitXml').attr('disabled', true);
    $("#XmlsTree .fancytree-node").removeClass("fancytree-active");

    $("#txtXmlID").val("");
    $("#txtXmlPath").text("");
    $("#txtXmlTitle").val("");
    $("#hdnXmlCreatedDate").val("");
    $("#hdnXmlDownloadPath").val("");
    $("#hdnTypeXml").val("");    
    setXmlCheckboxCheckedValue("#chkCanBeOpenedXml", "false");

    $("#txtXmlTitle").removeAttr("readonly");
    $("#txtXmlPath").removeAttr("readonly");    
    $("#chkCanBeOpenedXml").prop("disabled", false);

    $('#btnAddXmlFileUpload').attr('disabled', false);    
    $('#btnRemoveXml').attr('disabled', true);
    $('#btnEditXml').attr('disabled', true);
    $(".UpdateFileNewXml").hide();
    $(".AddFileNewXml").show();
}

function GetXmlFileData(myFile) {
    var file = myFile.files[0];
    var filename = file.name;
    $("#lblXmlFileName").text("");
    $("#lblXmlFileName").text(filename);
    $("#lblXmlFileName").show();
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
        $.notify("Xml Updated Successfully", "success");
    }
    if (ClickedType == "UpdateForm") {
        $.notify("Xml Updated Successfully", "success");
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

function InitXmlsTree() {
    $("#XmlsTree").fancytree({
        click: function (event, data) {
            var node = data.node;
            var selectedType = node.type;
            $('#btnAddNewXmlFolder').attr('disabled', true);
            $('#btnAddXmlFileUpload').attr('disabled', true);
            $(".AddFileNewXml").hide();
            $('#btnUpdateXmlFileUpload').attr('disabled', false);
            $(".UpdateFileUploadXml").prop('disabled', false);
            $(".UpdateFileNewXml").show();
            $('#btnSubmitXml').attr('disabled', true);
            $('#btnRemoveXml').attr('disabled', false);
            $('#btnEditXml').attr('disabled', false);
            $('#btnAddXmlRootFolder').attr('disabled', false);
            $("#txtXmlID").val(node.data.formid);
            $("#txtXmlPath").text(node.data.path);
            $("#txtXmlTitle").val(node.title);            
            $("#hdnXmlCreatedDate").val(node.data.createddate);
            $("#hdnXmlDownloadPath").val(node.data.downloadpath);
            $("#hdnTypeXml").val(node.data.doctype);
            $("#hdnUploadTypeXml").val(node.data.uploadtype);
            setXmlCheckboxCheckedValue("#chkCanBeOpenedXml", node.data.canbeopened);
            $("#txtXmlID").prop("readonly", "readonly");
            $("#txtXmlTitle").prop("readonly", "readonly");
            $("#txtXmlPath").prop("readonly", "readonly");            
            $("#chkCanBeOpenedXml").prop("disabled", "disabled");      
        },
    });
}

function setXmlCheckboxCheckedValue(obj, value) {
    if (value.toLowerCase() == "true") $(obj).prop('checked', true);
    else $(obj).prop('checked', false);
}
