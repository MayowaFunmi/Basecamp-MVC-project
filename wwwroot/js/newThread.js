$(document).ready(function () {
    console.log("is working")
    $('form#startThreadForm').submit(function (e) {
        e.preventDefault();

        // Collect form data
        var projectId = $('#projectId').val();
        var title = $('#title').val();
        
        // Create a JSON object
        var formData = {
            projectId: projectId,
            title: title
        };

        // Send an AJAX POST request
        $.ajax({
            url: $(this).attr("action"),//'/MessageThread/CreateThread', // Update the URL to match your controller action
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(formData),
            success: function (data) {
                console.log("success...")
                if (data.success) {
                    var threadCard = `
                        <div class="col">
                            <div class="card shadow-sm">
                                <div class="card-body">
                                    <p class="card-title">${data.messageThread.title}</p>
                                    <form method="post" action="">
                                        <p class="card-text">
                                            <input type="text" name="content" id="content" class="form-control" placeholder="Write something here ..." />
                                        </p>
                                        <button type="submit" id="threadBtn" class="btn btn-danger" data-bs-dismiss="modal">Start Thread</button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    `;
                }

                // Add the new thread card to the detail page
                $('.newThread').append(threadCard);

                // Optionally, clear the form input
                $('#title').val('');
            },
            error: function (error) {
                // Handle error
                console.error(error);
            }
        });
    });
});
