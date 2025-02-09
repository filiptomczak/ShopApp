$(document).ready(function () {
    loadDataTable()
})

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/admin/company/getall",
            "type": "GET",
            "datatype": "json",
        },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'city', "width": "20%" },
            { data: 'state', "width": "20%" },

            {
                data: 'id',
                "render": function (data) {
                    return `
                    <div class="w-75 75 btn-group role="group">
                        <a href="/admin/company/update?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>               
                        <a onClick=Delete("/admin/company/delete?id=${data}") class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i></a>
                    </d
                    `
                }
            },
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}
