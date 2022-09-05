var nahro = {
    //Delete that requests api and with confirm box and take necessary actinos
    //Collection parameters
    //url, meesage, successMessage, failMessage, parent
    //success and fail callback
    daleteApi:function (d, success = null, fail = null) {
        bootbox.confirm(d.message, function (result) {
            if (result) {
                $.ajax({
                    url: d.url,
                    method: 'delete'
                }).done(function () {
                    d.parent.remove();  //If succeded, delete the selected tr in the table;

                    //Show successMessage and call success function
                    toastr.success(d.successMessage);
                    success();
                    }).fail(function () {
                    //Show failMessage and call fail function
                    toastr.error(d.failMessage);
                    fail();
                });
            }
        });
    }
}