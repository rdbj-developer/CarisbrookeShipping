jQuery.support.cors = true;

$(document).ready(function () {
    $('#txtSearch').keyup(function (e) {
        if (e.keyCode != 13) {
            var value = $(this).val().toLowerCase();
            $(".divFormsList .dynamic-box").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        }
    }); 
});
function serchForm() {
    var value = $('#txtSearch').val().toLowerCase();
    $(".divFormsList .dynamic-box").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
}					  
function openFormDocument(path,type) {
    $.ajax({
        type: "POST",
        crossDomain: true,
        contentType: "application/json; charset=utf-8",
        headers: {
            "cache-control": "no-cache"
        },
        url: url + "openfile",
        data: JSON.stringify({
            value: path
        }),
        success: function (response) {
            if (response != null && response != "" && response.OpenFileResult != null && response.OpenFileResult != "") {
                if (type == "RWHS" || type == "RWH") {
                    if (response.OpenFileResult != "") {
                        var filename = path.substring(path.lastIndexOf('/') + 46);
                        var downloadpath = "C:/ProgramData/Carisbrooke Shipping Ltd/ISM Dashboard/Repository/Saved Forms/RWHS/" + filename;
                        openFormDocument(downloadpath);
                    }
                }
                else {
                    $.notify(response.OpenFileResult, "error");
                }
                //Ajax call
            }
        },
        error: function (response) {
            if (response.statusCode != 200)
                $.notify("Make sure local service is running and InfoPath setup is installed.", "error");
                if (type == "document") {
                    window.location = RootUrl + 'Forms/CreatePDF?path=' + path;
                } else {
                    $("#confirmServiceModal").modal("show");
                }
          //  var servicePath = "D:/DotNetProjects/Ship_Projects/OfficeApplication/OfficeApplication/Service/CarisbrookeOpenFileService.zip";
          //  window.location = RootUrl + 'Forms/CreatePDF?path=' + servicePath;
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
            id
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

//$(document).on('mouseenter', '.showIsuue', function () {
//   $("#boxIssue").addClass('hvrbox');
//    $(".hvrbox-layer_top").removeClass('hidden');
//}).on('mouseleave', '.showIsuue', function () {
//    $("#boxIssue").removeClass('hvrbox');
//    $(".hvrbox-layer_top").addClass('hidden');
//});



