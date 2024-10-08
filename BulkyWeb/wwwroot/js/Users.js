var dataTable;

$(document).ready(function () {
    loadUsers();
});

function loadUsers() {
    dataTable = $('#UsersTable').DataTable({
        "ajax": { url: '/admin/user/GetUsers' },

        //column names should match api data 
        "columns": [
            { data: 'name' },
            { data: 'email' },
            { data: 'phoneNumber' },
            { data: 'company.name' },
            { data: 'role' },
            {
                data: {
                    id: 'id', lockoutEnd: 'lockoutEnd'
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockedDate = new Date(data.lockoutEnd).getTime();

                    if (lockedDate > today) {
                        //user is locked
                        return `<div class="text-center" >
                        <a onclick=LockUnlockUser('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px">
                              <i class="bi bi-lock-fill"></i> UnLock
                        </a>
                         <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-dark text-white" style="cursor:pointer; width:150px;">
                                     <i class="bi bi-pencil-square"></i> Permission
                        </a>
                        </div>`
                       
                    } else {
                       return `<div class="text-center" >
                        <a onclick=LockUnlockUser('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px">
                              <i class="bi bi-lock-fill"></i> Lock
                        </a>
                         <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-dark text-white" style="cursor:pointer; width:150px;">
                                     <i class="bi bi-pencil-square"></i> Permission
                        </a>
                        </div>`
                    }
                   

                }

            }
        ]

    });
}


function LockUnlockUser(id) {
   
            $.ajax({
                url: '/Admin/User/LockUnLock',
                type: 'POST',
                data: JSON.stringify(id),
                contentType : 'application/json',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                   
                }


            })
        
}

