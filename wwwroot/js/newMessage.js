$(document).ready(function () {
    console.log("is working")

    $("#message").submit(function (e) {
        e.preventDefault();

        var threadId = $("#threadId").val();
        var content = $("#content").val();
        console.log(threadId + "" + content)
        var formData = {
            threadId: threadId,
            content: content
        };

        $.ajax({
            url: `/Project/CreateMessage`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (data) {
                console.log(data);
            },
            error: function (xhr, status, error) {
                console.error("Error: " + error);
            }
        })
    })
});