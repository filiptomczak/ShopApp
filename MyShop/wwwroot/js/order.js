$(document).ready(function () {
    var filter = document.getElementById("status").getAttribute('data-value');
    loadDataTable(filter)
})

function loadDataTable(filter) {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/admin/order/getall?filter="+filter,
            "type": "GET",
            "datatype":"json",
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'appUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "10%" },

            {
                data: 'id',
                "render": function (data) {
                    return `
                    <div class="w-75 75 btn-group role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>  </a>
                    </d
                    `
                }
            },
        ]
    });
}
