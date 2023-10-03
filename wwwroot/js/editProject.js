$(document).ready(function () {
    $("form").submit(function (event) {
        event.preventDefault();

        var name = $("#Name").val();
        var description = $("#Description").val();
        var projectId = $("#ProjectId").val();

        var formData = {
            Name: name,
            Description: description,
            ProjectId: projectId
        };

        $.ajax({
            url: $(this).attr("action"),
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
}) 