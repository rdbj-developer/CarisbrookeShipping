var dataISM, dataSSP, dataMLC;

// RDBJ 11/30/2021
$(document).ready(function () {
    GetSMSReferenceData();
    GetSSPReferenceData();
    GetMLCRegulationTree();
});
// End RDBJ 11/30/2021

// RDBJ 11/30/2021
function GetSMSReferenceData() {
    var url = AuditRootUrl + "IAFList/GetSMSReferenceData";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            //dataISM = Data;

            //RDBJ 12/01/2021
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
            //End RDBJ 12/01/2021
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End RDBJ 11/30/2021

// RDBJ 11/30/2021
function GetSSPReferenceData() {
    var url = AuditRootUrl + "IAFList/GetSSPReferenceData";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            //dataSSP = Data;

            //RDBJ 12/01/2021
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
            //End RDBJ 12/01/2021
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End RDBJ 11/30/2021

// RDBJ 11/30/2021
function GetMLCRegulationTree() {
    var url = AuditRootUrl + "IAFList/GetMLCRegulationTree";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            //dataMLC = Data;

            //RDBJ 12/01/2021
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
            //End RDBJ 12/01/2021
        },
        error: function (data) {
            console.log(data);
        }
    });
}
// End RDBJ 11/30/2021



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