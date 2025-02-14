var dataTable;


$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    var status = urlParams.get('status');
    loadDataTable(status);
    $.fn.dataTable.ext.errMode = function (obj, param, err) {
        var tableId = obj.sTableId;
        console.log('Handling DataTable issue of Table ' + tableId);
    };
});


function loadDataTable(status) {
    dataTable = $('#tblBookings').DataTable({
        "ajax": {
            url: '/booking/getall' + (status === null ? '' : ('?status=' + status))
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phone', "width": "10%" },
            { data: 'email', "width": "15%" },
            { data: 'status', "width": "10%" },
            { data: 'checkInDate', "width": "10%" },
            { data: 'nights', "width": "10%" },
            { data: 'totalCost', render: $.fn.dataTable.render.number(',', '.', 2), "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                        <a href="/booking/BookingDetails?bookingId=${data}" class="btn btn-outline-warning mx-2">
                            <i class="bi bi-pencil-square"></i> Details
                        </a>
                    </div>`
                }
            }
        ]
    });
}