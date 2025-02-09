$(document).ready(function () {
    loadDataTable()
})

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/admin/user/getall",
            "type": "GET",
            "datatype":"json",
        },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'email', "width": "20%" },
            { data: 'company.name', "width": "20%" },
            { data: 'role', "width": "20%" },

            {
                data: { id: 'id', lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout <= today) {
                        return `
                            <div class="text-center">
                                <a onclick=lockUnlock('${data.id}') class="btn btn-success text-white" style="width:100px;"
                                    <i class="bi bi-unlock-fill"></i>UnLocked
                                </a>

                                <a href="/admin/user/roleManagement?userId=${data.id}" class="btn btn-warning text-white" style="width:100px;"
                                    <i class="bi bi-pencil-square"></i>Permission
                                </a>

                            </div>
                        `
                    } else {
                        return `
                            <div class="text-center">
                                <a onclick=lockUnlock('${data.id}') class="btn btn-danger text-white" style="width:100px;"
                                    <i class="bi bi-lock-fill"></i>Locked
                                </a>

                                <a href="/admin/user/roleManagement?userId=${data.id}" class="btn btn-warning text-white" style="width:100px;"
                                    <i class="bi bi-pencil-square"></i>Permission
                                </a>

                          </div>`
                    }
                    
                }
            },
        ]
    });
}

function lockUnlock(userId) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(userId),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        },
    })
}