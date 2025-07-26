// JSL 05/22/2022 Added this script

var dataISM, dataSSP, dataMLC;  // JSL 05/22/2022

// JSL 05/22/2022
$(document).ready(function () {
    GetSMSReferenceData();
    GetSSPReferenceData();
    GetMLCRegulationTree();
});
// End JSL 05/22/2022

// JSL 05/22/2022
function GetSMSReferenceData() {
    var url = RootUrl + "InternalAuditForm/GetSMSReferenceData";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            //dataISM = Data;

            $("#ISMTree").igTree({
                dataSource: Data, //JSON Array defined above
                bindings: {
                    textKey: "Reference",
                    valueKey: "Id",
                    childDataProperty: "Nodes",
                    bindings: {
                        textKey: "Reference",
                        valueKey: "Id"
                    }
                }
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End JSL 05/22/2022

// JSL 05/22/2022
function GetSSPReferenceData() {
    var url = RootUrl + "InternalAuditForm/GetSSPReferenceData";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            //dataSSP = Data;

            $("#SSPTree").igTree({
                dataSource: Data, //JSON Array defined above
                bindings: {
                    textKey: "Reference",
                    valueKey: "Id",
                    childDataProperty: "Nodes",
                    bindings: {
                        textKey: "Reference",
                        valueKey: "Id"
                    }
                }
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End JSL 05/22/2022

// JSL 05/22/2022
function GetMLCRegulationTree() {
    var url = RootUrl + "InternalAuditForm/GetMLCRegulationTree";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            //dataMLC = Data;

            $("#MLCTree").igTree({
                dataSource: Data, //JSON Array defined above
                bindings: {
                    textKey: "Reference",
                    valueKey: "Id",
                    childDataProperty: "Nodes",
                    bindings: {
                        textKey: "Reference",
                        valueKey: "Id"
                    }
                }
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End JSL 05/22/2022


/*
function format_for_treeview(data, arr) {
        for (var key in data) {
            if (Array.isArray(data[key]) || data[key].toString() === "[object Object]") {
                // when data[key] is an array or object
                var nodes = [];
                var completedNodes = format_for_treeview(data[key], nodes);
                arr.push({
                    //text: data[key].Number + " " + data[key].Reference,
                    text: data[key].Reference,
                    //text: data[key],
                    nodes: completedNodes
                });
            } else {
                // when data[key] is just strings or integer values
                if (data[key].Number != undefined) {
                    arr.push({
                        //text: data[key].Number + ". " + data[key].Reference,
                        text: data[key],//.Reference,
                        //text: key + " : " + data[key]
                        //text: data[key]
                    });
                }
            }
        }
        return arr;
    }

    $("#ISMTree").treeview({
        color: "#428bca",
        levels: 1,
        expandIcon: 'glyphicon glyphicon-chevron-right',
        collapseIcon: 'glyphicon glyphicon-chevron-down',
        data: format_for_treeview(dataISM, [])
    });

    $("#SSPTree").treeview({
        color: "#428bca",
        levels: 1,
        expandIcon: 'glyphicon glyphicon-chevron-right',
        collapseIcon: 'glyphicon glyphicon-chevron-down',
        data: format_for_treeview(dataSSP, [])
    });

    $("#MLCTree").treeview({
        color: "#428bca",
        levels: 1,
        expandIcon: 'glyphicon glyphicon-chevron-right',
        collapseIcon: 'glyphicon glyphicon-chevron-down',
        data: format_for_treeview(dataMLC, [])
    });
 */