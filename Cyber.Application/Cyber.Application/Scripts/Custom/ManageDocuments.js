$(document).ready(function () {
    $('#btnSubmit').attr('disabled', true);
    $('#btnRemove').attr('disabled', true);
    $('#btnEdit').attr('disabled', true);
    $(".btnAddNew").click(function () {
        $("#lblFileName").hide();
        var btnVal = $(this).val();
        if (btnVal == "AddRoot") {
            AddRootFolder();
        }
        if (btnVal == "AddFolder" || btnVal == "AddWindowsFolder") {
            AddFolder(btnVal);
        }
        if (btnVal == "RemoveObj") {
            RemoveObj();
        }
        if (btnVal == "CancelChanges") {
            CancelChanges();
        }
    });
    $("#btnEdit").click(function () {
        EditDocument();
    })
    $(".AddFileUpload").click(function (e) {
        AddFile(e);
    });
    $(".UpdateFileUpload").click(function (e) {
        UpdateFile(e);
    });
	 $(".UpdateFileFolderUpload").click(function (e) {
        UpdateFileFolder(e);
    });												 
})

function AddRootFolder() {
    $("#hdnClickedType").val("AddRoot");
    $('#btnSubmit').attr('disabled', false);
    $('#btnRemove').attr('disabled', true);
    $('#btnEdit').attr('disabled', true);
    $(".fancytree-node").removeClass("fancytree-active")
    var docID = '81f7f9cd-e822-4956-a1bf-175b24581c48';                 
    var parentId = '00000000-0000-0000-0000-000000000000';
    $("#txtDocumentID").val(docID);
    $("#txtParentID").val(parentId);
    $("#txtNumber").val("");
    $("#txtDocNo").val("");
    $("#txtTitle").val("");
    $("#txtPath").text("");
    $("#txtNumber").removeAttr("readonly");
    $("#txtDocNo").removeAttr("readonly");
    $("#txtTitle").removeAttr("readonly");
    $("#txtPath").removeAttr("readonly");
    $("#txtPath").text("C:\\ProgramData\\Carisbrooke Shipping Ltd\\ISM Dashboard\\Repository\\Documents");
    $('#btnAddNewFolder').prop('disabled', true);
    $('#btnAddNewWindowsFolder').prop('disabled', true);
    $('#btnAddFileUpload').attr('disabled', true);
    $(".AddFileUpload").prop('disabled', true);
}

function AddFolder(clicktype) {
    $("#hdnClickedType").val(clicktype);
    var selectedLength = $(".fancytree-active").length;
    if (selectedLength == 1) {
        $('#btnRemove').attr('disabled', true);
        $('#btnEdit').attr('disabled', true);
        $('#btnSubmit').attr('disabled', false);
        $('#btnAddRootFolder').prop('disabled', true);
        $('#btnAddFileUpload').attr('disabled', true);
        $(".AddFileUpload").prop('disabled', true);
        $("#txtNumber").val("");
        $("#txtDocNo").val("");
        $("#txtTitle").val("");
        $("#txtNumber").removeAttr("readonly");
        $("#txtDocNo").removeAttr("readonly");
        $("#txtTitle").removeAttr("readonly");
        $("#txtPath").prop("readonly", "readonly");
    }
    else {
        alert("Plese select the folder from left side");
    }
}

function AddFile(e) {
    $('#btnRemove').attr('disabled', true);
    var isDisable = $('#btnAddFileUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#hdnClickedType").val("AddFile");
        var selectedLength = $(".fancytree-active").length;
        if (selectedLength == 1) {
            $('#btnEdit').attr('disabled', true);
            $('#btnSubmit').attr('disabled', false);
            $('#btnAddRootFolder').prop('disabled', true);
            $('#btnAddNewFolder').prop('disabled', true);
            $('#btnAddNewWindowsFolder').prop('disabled', true);
            $("#txtNumber").val("");
            $("#txtDocNo").val("");
            $("#txtTitle").val("");
            $("#txtNumber").removeAttr("readonly");
            $("#txtDocNo").removeAttr("readonly");
            $("#txtTitle").removeAttr("readonly");
            $("#txtPath").prop("readonly", "readonly");
        }
        else {
            alert("Plese select the folder from left side");
            e.preventDefault();
        }
    }
}
function UpdateFile(e) {
    $('#btnRemove').attr('disabled', true);
    var isDisable = $('#btnUpdateFileUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#hdnClickedType").val("UpdateDocument");
        var selectedLength = $(".fancytree-active").length;
        if (selectedLength == 1) {
            $('#btnEdit').attr('disabled', true);
            $('#btnSubmit').attr('disabled', false);
            $('#btnAddRootFolder').prop('disabled', true);
            $('#btnAddNewFolder').prop('disabled', true);
            $('#btnAddNewWindowsFolder').prop('disabled', true);
            $("#txtPath").prop("readonly", "readonly");
        }
        else {
            alert("Plese select the file from left side");
            e.preventDefault();
        }
    }
}
function UpdateFileFolder(e) {
    $('#btnRemove').attr('disabled', true);
    var isDisable = $('#btnUpdateFileFolerUpload').attr('disabled');
    if (isDisable == 'disabled') {
        e.preventDefault();
    }
    else {
        $("#hdnClickedType").val("UpdateFileFolder");
        var selectedLength = $(".fancytree-active").length;
        if (selectedLength == 1) {
            $('#btnEdit').attr('disabled', true);
            $('#btnSubmit').attr('disabled', false);
            $('#btnAddRootFolder').prop('disabled', true);
            $('#btnAddNewFolder').prop('disabled', true);
            $('#btnAddNewWindowsFolder').prop('disabled', true);
            //$("#txtPath").prop("readonly", "readonly");
            $("#txtPath").attr("readonly", false); 
        }
        else {
            alert("Plese select the file from left side");
            e.preventDefault();
        }
    }
}
function EditDocument() {
    var selectedLength = $("#DocumentsTree").length;// $(".fancytree-active").length;
    if (selectedLength == 1) {
        $("#hdnClickedType").val("EditDocument");
        $("#txtNumber").removeAttr("readonly");
        $("#txtDocNo").removeAttr("readonly");
        $("#txtTitle").removeAttr("readonly");
       // $("#txtPath").prop("readonly", "readonly");
	     $("#txtPath").removeAttr("readonly");
        $('#btnAddRootFolder').prop('disabled', true);
        $('#btnAddNewFolder').prop('disabled', true);
        $('#btnAddNewWindowsFolder').prop('disabled', true);
        $('#btnAddFileUpload').attr('disabled', true);
        $(".AddFileUpload").prop('disabled', true);
        $('#btnRemove').attr('disabled', true);
        $('#btnSubmit').attr('disabled', false);
        $('#btnUpdateFileUpload').attr('disabled', true);
		$('#btnUpdateFileFolerUpload').attr('disabled', true);								   
        $(".UpdateFileUpload").prop('disabled', true);
		$(".UpdateFileFolderUpload").prop('disabled', true);						
    }
    else {
        alert("Plese select the folder from left side");
        e.preventDefault();
    }
}

function RemoveObj() {
    $("#hdnClickedType").val("RemoveObj");
    var selectedLength = $(".fancytree-active").length;
    if (selectedLength == 1) {
        var r = confirm("Are you sure? you want to delete this?");
        if (r) {
            $("#frmManageDoc").submit();
        }
    }
    else {
        alert("Plese select the folder from left side");
    }
}

function CancelChanges() {
    $("#hdnClickedType").val("CancelChanges");
    $('#btnSubmit').attr('disabled', true);
    $(".fancytree-node").removeClass("fancytree-active");
    $("#txtDocumentID").val("");
    $("#txtParentID").val("");
    $("#txtNumber").val("");
    $("#txtDocNo").val("");
    $("#txtTitle").val("");
    $("#txtPath").text("");
    $('#btnAddRootFolder').prop('disabled', false);
    $('#btnAddNewFolder').prop('disabled', false);
    $('#btnAddNewWindowsFolder').prop('disabled', false);
    $('#btnAddFileUpload').attr('disabled', false);
    $(".AddFileUpload").prop('disabled', false);
    $("#txtNumber").removeAttr("readonly");
    $("#txtDocNo").removeAttr("readonly");
    $("#txtTitle").removeAttr("readonly");
    $("#txtPath").removeAttr("readonly");
    $('#btnRemove').attr('disabled', false);
    $('#btnEdit').attr('disabled', false);
    $(".UpdateFileNew").hide();
	  $(".UpdateFileFolderNew").hide();
    $(".AddFileNew").show();
}

function GetFileData(myFile) {
    var file = myFile.files[0];
    var filename = file.name;
    $("#lblFileName").text("");
    $("#lblFileName").text(filename);
    $("#lblFileName").show();
    //$("#txtTitle").val(filename);
}
function GetFileFolderData(myFolder) {
    //var file = myFile.files[0];
    $("#hdnClickedType").val("UpdateFileFolder");
    $("#txtPath").attr("readonly", false); 
    $("#txtPath").text("");
    $("#txtPath").show();
    $("#btnEdit").attr('disabled', true);
    $('#btnSubmit').attr('disabled', false);
}
function SuccessMsg(ClickedType) {
    if (ClickedType == "AddFolder" || ClickedType == "AddWindowsFolder" || ClickedType == "AddRoot") {
        $.notify("Folder Added Successfully", "success");
    }
    if (ClickedType == "AddFile") {
        $.notify("File Added Successfully", "success");
    }
    if (ClickedType == "RemoveObj") {
        $.notify("Removed Successfully", "success");
    }
    if (ClickedType == "EditDocument") {
        $.notify("Document Updated Successfully", "success");
    }
    if (ClickedType == "UpdateDocument") {
        $.notify("Document Updated Successfully", "success");
    }
	if (ClickedType == "UpdateFileFolder") {
        $.notify("Document Folder Updated Successfully", "success");
    }														   
}

function ErrorMsg(ClickedType) {
    if (ClickedType == "AddFolder" || ClickedType == "AddWindowsFolder"  || ClickedType == "AddRoot") {
        $.notify("Error occured while adding folder", "error");
    }
    if (ClickedType == "AddFile") {
        $.notify("Error occured while adding file", "error");
    }
    if (ClickedType == "RemoveObj") {
        $.notify("Error occured while removing file", "error");
    }
    if (ClickedType == "EditDocument") {
        $.notify("Error occured while updating document", "success");
    }
    if (ClickedType == "UpdateDocument") {
        $.notify("Error occured while updating document", "success");
    }
	 if (ClickedType == "UpdateFileFolder") {
        $.notify("Error occured while updating document", "success");
    }														   
}