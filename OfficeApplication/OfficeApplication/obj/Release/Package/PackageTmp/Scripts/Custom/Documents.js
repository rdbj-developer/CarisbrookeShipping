var AllDocuments;

$(document).ready(function () {

    $('#txtSearch').keyup(function (e) {
        if (e.keyCode == 13) {
            // SearchDocuments(true);
        }
        else {
            SearchDocuments();
        }
    });


    GetAllDocuments();
    var height = $(".content-wrapper").height();
    $(".content").height(height - 190);
    resizeWindow();
    $(window).resize(function () {
        resizeWindow();
    });
});

function resizeWindow() {
    var windowhheight = $(window).height();
    if (windowhheight < 460) {
        $('body').addClass("fixed");
        $(".content").css('height', 'auto');
        setTimeout(function () {
            $(".content-wrapper").css('height', 'auto');
        }, 900);
    }
    else {
        $('body').removeClass("fixed");
    }
}

function GetSubCategories(DocumentID) {
    var Categories = AllDocuments.filter(function (x) {
        return x.ParentID == DocumentID;
    })
    CreateDocList(Categories);
}

function SearchDocuments() {
    var existingString = $("#txtSearch").val();
    if (existingString != "" && existingString.length > 0) {
        var searchResult = AllDocuments.filter(function (el) {
            return el.Title.toLowerCase().indexOf(existingString.toLowerCase()) > -1 && el.Type != "FOLDER";
        })
        DisplaySearch(searchResult);
    }
    else {
        var CategoriesAll = AllDocuments.filter(function (x) {
            return x.ParentID == "00000000-0000-0000-0000-000000000000";
        });
        CreateDocList(CategoriesAll);
    }
}

function GetUniqueArrays(searchResult) {
    var arr = [];
    for (var i = 0; i < searchResult.length; i++) {
        if (!arr.includes(searchResult[i].ParentID)) {
            arr.push(searchResult[i].ParentID);
        }
    }
    return arr;
}

function DisplaySearch(searchResult) {
    if (searchResult != null && searchResult.length > 0) {

        var uniques = GetUniqueArrays(searchResult);
        var MainFiles = [];

        if (uniques != null && uniques.length > 0) {
            for (b in uniques) {
                var AllFiles = searchResult.filter(function (el) {
                    return el.ParentID == uniques[b];
                });
                var parentID = uniques[b];
                var HeadTitle;
                var ParentDocumentID;

                for (var i = 0; i < 5; i++) {
                    var ParentFolder = AllDocuments.filter(function (el) {
                        return el.DocumentID == parentID;
                    });
                    if (ParentFolder != null && ParentFolder != undefined && ParentFolder.length > 0) {
                        parentID = ParentFolder[0].ParentID;
                    }
                    if (parentID == "00000000-0000-0000-0000-000000000000") {
                        HeadTitle = ParentFolder[0].Title;
                        ParentDocumentID = ParentFolder[0].DocumentID;
                        break;
                    }
                }

                if (AllFiles != null && AllFiles != undefined && AllFiles.length > 0) {
                    for (var j = 0; j < AllFiles.length; j++) {
                        AllFiles[j].ParentID = ParentDocumentID;
                        MainFiles.push(AllFiles[j]);
                    }
                }
            }
        }
        DisplaySearchedFiles(MainFiles);
    }
}

function EditViewofRiskassessmentForm(path) {
    var id = path.substring(path.lastIndexOf('?') + 4);
    var _url = RootUrl + "Forms/RAformData?id=" + id;
    window.open(_url, '_blank');
}
function OpenCybersecurityRA() {
    var _url = RootUrl + "Documents/CybersecurityRisksAssessmentView";
    window.open(_url, '_blank');
}