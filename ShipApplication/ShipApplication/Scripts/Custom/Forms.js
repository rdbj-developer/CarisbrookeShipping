jQuery.support.cors = true;

$(document).ready(function () {
    if ($('#txtSearch').val() != "") {
        serchForm();
    }
    $('#txtSearch').keyup(function (e) {
        if (e.keyCode != 13) {
            serchForm();
        }
    });
});
function serchForm() {
    var value = $('#txtSearch').val().toLowerCase();
    $(".divFormsList .dynamic-box").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
}
function openFormDocument(path, type, originPath) {
    var data = path;
    if (originPath != null && originPath != "" && originPath != undefined) {
        data = originPath;
    }
    console.log(data);
    $.ajax({
        type: "POST",
        crossDomain: true,
        contentType: "application/json; charset=utf-8",
        headers: {
            "cache-control": "no-cache"
        },
        url: url + "openfile",
        data: JSON.stringify({
            value: data
        }),
        success: function (response) {
            if (response != null && response != "" && response.OpenFileResult != null && response.OpenFileResult != "") {
                if (type == "SCSI" || type == "BSI" || type == "CO") {                    
                    openFormDocument(path);
                }
                else if (type == "RWH") {
                    $.notify("This form needs to be opened from Main communication PC.", "warning");                    
                }
                else {
                    $.notify(response.OpenFileResult, "error");
                }               
            }
        },
        error: function (response) {
            if (response.statusCode != 200)
                $.notify("Make sure local service is running and InfoPath setup is installed.", "error");
            if (type == "document") {
                window.location = RootUrl + 'Documents/CreatePDF?path=' + path;
            } else {
                $("#confirmServiceModal").modal("show");
            }
            console.log(response);
        }
    });
}
function openFolder(path) {
    $.ajax({
        type: "POST",
        crossDomain: true,
        contentType: "application/json; charset=utf-8",
        headers: {
            "cache-control": "no-cache"
        },
        url: url + "openfolder",
        data: JSON.stringify({
            value: path
        }),
        success: function (response) {
            if (response != null && response != "" && response.OpenFileResult != null && response.OpenFileResult != "") {
                    $.notify(response.OpenFileResult, "error");                
            }

        },
        error: function (response) {
            if (response.statusCode != 200)
                $.notify("Make sure local service is running and InfoPath setup is installed.", "error");          
            console.log(response);
        }
    });
}
function ViewInfoPDF(path) {

    var id = path.substring(path.lastIndexOf('?') + 4);

    $.ajax({
        type: "POST",
        url: RootUrl + "Documents/ViewPDFPath",
        dataType: "json",
        contentType: "application/json",
        async: false,
        data: JSON.stringify({
            "id": id
        }),
        success: function (response) {
            var type = "document";
            openFormDocument(response, type);
        },
        error: function (response) {

        }
    })
}

$(document).on('click', '#btnService', function () {
    window.location = RootUrl + 'Documents/DownloadServiceSetup';
    $("#confirmServiceModal").modal("hide");
});

function openForMainPCDocument(path, type, isLocalRequest, isFolderRequest) {
    if (isLocalRequest != "True") {
        $.notify("This form needs to be opened from Main communication PC.", "warning");     
        return false;
    }
    if (isFolderRequest) {
        openFolder(path);
    }
    else {
        openFormDocument(path, type);
    }
}
