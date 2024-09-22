var dataTable;

$(document).ready(function () {
    loadProducts();
});

function loadProducts() {
    dataTable = $('#ProductsTable').DataTable({
       "ajax": {url : '/admin/product/GetProducts'},

        //column names should match api data 
        "columns": [
            { data: 'title' },
            { data: 'isbn' },
            { data: 'author' },
            { data: 'listPrice' },
            { data: 'category.name' },
            {
                data: 'id',
                "render": function (data) {
                   return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     <a onClick=deleteProduct("/admin/product/delete?id=${data}") class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                    
                } 

            }
        ]

    });
}


function deleteProduct (url){
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
    })
}

