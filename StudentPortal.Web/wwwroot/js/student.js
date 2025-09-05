$(document).ready(function () {
    let table = $('#studentsTable').DataTable({
        ajax: {
            url: '/Students/GetStudents',
            type: 'GET',
            dataSrc: 'data'
        },
        columns: [
            { data: 'name' },
            { data: 'email' },
            { data: 'phone' },
            { data: 'subscribed', render: d => d ? "Yes" : "No" },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <button class="btn btn-sm btn-primary edit-btn" data-id="${data}">Edit</button>
                        <button class="btn btn-sm btn-danger delete-btn" data-id="${data}">Delete</button>
                    `;
                }
            }
        ]
    });

    // Add Student
    $.get('/Students/Add', function (html) {
        $('#modalTitle').text('Add Student');
        $('#modalContent').html(html);
        $('#studentModal').modal('show');
        bindForm('/Students/Add'); // ✅ Matches [HttpPost] Add
    });

    // Edit Student
    $.get('/Students/Edit/' + id, function (html) {
        $('#modalTitle').text('Edit Student');
        $('#modalContent').html(html);
        $('#studentModal').modal('show');
        bindForm('/Students/Edit'); // ✅ Matches [HttpPost] Edit
    });


    // Delete Student
    $('#studentsTable').on('click', '.delete-btn', function () {
        if (!confirm("Are you sure you want to delete this student?")) return;
        let id = $(this).data('id');
        $.post('/Students/Delete/' + id, $('form').serialize(), function (res) {
            if (res.success) {
                table.ajax.reload();
                alert(res.message);
            } else {
                alert(res.message);
            }
        });
    });

    // Reusable form binding
    function bindForm(url) {
        $('#studentForm').submit(function (e) {
            e.preventDefault();
            $.ajax({
                url: url,
                type: 'POST',
                data: $(this).serialize(),
                success: function (res) {
                    if (res.success) {
                        $('#studentModal').modal('hide');
                        table.ajax.reload();
                        alert(res.message);
                    } else {
                        console.log(res.errors);
                        alert("Validation failed.");
                    }
                }
            });
        });
    }

});
