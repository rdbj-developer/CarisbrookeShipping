$(document).ready(function () {
    $("#btnAddUser").click(function () {
        $("#UsersList").hide();
        $("#AddUserForm").show();
    });
    $("#btnCancel").click(function () {
        $("#AddUserForm").hide();
        $("#UsersList").show();
    })
    LoadUsersGrid();
});
function LoadUsersGrid() {
    var url = RootUrl + "Admin/GetAllUsers";
    $.ajax({
        type: 'GET',
        dataType: 'json',
        async: false,
        url: url,
        success: function (Data) {
            data = Data;
            $('#UsersGrid').empty();
            var grid = $('#UsersGrid').kendoGrid({
                scrollable: true,
                sortable: true,
                resizable: true,
                filterable: true,
                height: $(window).height() - $(".tab-content").height()-180,
                pageable: {
                    pageSizes: [5, 10, 20, 100]
                },
                dataSource: {
                    data: data,
                    pageSize: 10
                },
                dataBound: function () {
                    for (var i = 0; i < this.columns.length; i++) {
                    }
                },
                columns: [
                    {
                        field: "UserID",
                        title: "UserID",
                        hidden: true,
                    },
                    {
                        field: "Email",
                        title: "Email",
                    },
                    {
                        field: "UserGroupName",
                        title: "User Group",
                    },
                    {
                        field: "UserName",
                        title: "User Name",
                    }
                    //{
                    //    field: "UserRoleName",
                    //    title: "User Role",
                    //},
                ]
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}