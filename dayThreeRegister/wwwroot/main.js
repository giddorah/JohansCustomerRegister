$(function () {
    $.fn.editable.defaults.mode = 'inline';

});

$("#countCustomers").click(function () {
    $.ajax({
        url: '/api/values/countcustomers',
        method: 'GET'
    })
        .done(function (result) {
            $("#status").html('<div class="alert alert-primary alert-dismissible fade show" role="alert">' + result + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
        });
});

$("#checkLog").click(function () {
    $.ajax({
        url: '/api/log/checklog',
        method: 'GET'
    })
        .done(function (result) {
            let concatinatedMessage = "";

            $.each(result, function (index, item) {
                concatinatedMessage += item + "<br/> ";
            });

            $("#status").html('<div class="alert alert-primary alert-dismissible fade show" role="alert">' + concatinatedMessage + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
        });
});

$("#getOne").click(function () {
    let idNumber = $("#idNumber").val();
    let showBrief = $('#summary').prop('checked');

    $.ajax({
        url: '/api/values/getUsingId',
        method: 'GET',
        data: { id: idNumber, brief: showBrief }
    })
        .done(function (result) {
            $("#status").html('<div class="alert alert-success alert-dismissible fade show" role="alert">' + result + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

        })

        .fail(function (xhr, status, error) {
            $("#status").html('<div class="alert alert-danger alert-dismissible fade show" role="alert">' + xhr.responseText + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
        });
});

$("#getAll").click(function () {
    $.ajax({
        url: '/api/values/getallcustomers',
        method: 'GET'
    })
        .done(function (result) {

            let generatedResult = '<table class="table table-dark table-striped"><thead><tr><th scope="col">#</th><th scope="col">First Name</th><th scope="col">Last Name</th><th scope="col">Gender</th><th scope="col">Email</th><th scope="col">Age</th><th scope="col">Address</th><th scope="col">Delete</th></tr></thead><tbody>';

            $.each(result, function (index, item) {
                generatedResult += '<tr id="customerNumber' + item.id + '">' +
                    '<th scope="row">' + item.id + '</th>' +
                    '<td>' + '<span class="edit" data-name="firstName" data-type="text" data-pk="' + item.id + '" data-title="Enter First Name">' + item.firstName + '</span></td>' +
                    '<td>' + '<span class="edit" data-name="lastName" data-type="text" data-pk="' + item.id + '" data-title="Enter Last Name">' + item.lastName + '</span></td>' +
                    '<td>' + '<span class="edit" data-name="gender" data-type="text" data-pk="' + item.id + '" data-title="Enter Gender">' + item.gender + '</span></td>' +
                    '<td>' + '<span class="edit" data-name="email" data-type="text" data-pk="' + item.id + '" data-title="Enter Email">' + item.email + '</span></td>' +
                    '<td>' + '<span class="edit" data-name="age" data-type="text" data-pk="' + item.id + '" data-title="Enter Age">' + item.age + '</span></td>' +
                    '<td>' + '<span class="address" id="' + item.id + '" data-title="Adress"><button class="btn btn-info">A</button></span></td>' +
                    '<td>' + '<span class="delete" id="' + item.id + '" data-title="Delete"><button class="btn btn-danger">X</button></span></td>' +
                    '</tr>';
            });
            generatedResult += "</tbody ></table ><hr />";

            $("#dataTable").html(generatedResult);

            $(".edit").editable({
                type: 'text',
                url: '/api/values/editcustomer',
                emptyMessage: '<em>Please write something.</em>',
                success: function (response) {
                    $("#status").html('<div class="alert alert-success alert-dismissible fade show" role="alert">' + response + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
                    $().alert('close');
                },
                error: function (response) {
                    $("#status").html('<div class="alert alert-danger alert-dismissible fade show" role="alert">' + response.responseText + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
                    $().alert('close');
                }
            });

            $(".delete").click(function () {
                let deleteIdNumber = this.id;
                $.ajax({
                    url: '/api/values/deleteacustomer',
                    method: 'POST',
                    data: { id: deleteIdNumber }
                }).done(function (response) {
                    $("#getAll").click();
                    $("#status").html('<div class="alert alert-warning alert-dismissible fade show" role="alert">' + response + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

                });
            });
            $(".address").click(function () {
                let idforcustomer = this.id;
                let tableContents = "";

                $.ajax({
                    url: '/api/values/showcustomeradresses',
                    method: 'GET',
                    data: { id: idforcustomer }
                }).done(function (result) {
                    if (result.length === 0) {
                        tableContents = "<h2>No known addresses for this customer.</h2>";
                    }
                    else {
                        $.each(result, function (index, item) {
                            tableContents += '<tr><th scope="row">#' + item.id
                                + '</th><td><span href="#" class="editAddress" data-name="streetName" data-type="text" data-pk="' + item.id + '" data-title="Enter Street Name">' + item.streetName + '</span></td>'
                                + '</th><td><span href="#" class="editAddress" data-name="number" data-type="text" data-pk="' + item.id + '" data-title="Enter Street Number">' + item.number + '</span></td>'
                                + '</th><td><span href="#" class="editAddress" data-name="postalCode" data-type="text" data-pk="' + item.id + '" data-title="Enter Postal Code">' + item.postalCode + '</span></td>'
                                + '</th><td><span href="#" class="editAddress" data-name="area" data-type="text" data-pk="' + item.id + '" data-title="Enter Area Name">' + item.area + '</span></td>'
                                + '</th><td class="delete" id=' + item.id + '><button class="btn btn-danger">X</button></td>'
                                + '</td></tr>';
                        });
                    }

                }).fail(function () {
                    tableContents = "An error has occured during the process.";
                }).always(function () {
                    let modalContents = '<div class="modal-dialog modal-lg" role="document">'
                        + '<div class="modal-content">'
                        + '<div class="modal-header">'
                        + '<h5 class="modal-title" id="exampleModalLabel">Known Addresses</h5>'
                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close">'
                        + '<span aria-hidden="true">&times;</span>'
                        + '</button>'
                        + '</div>'
                        + '<div class="modal-body">'
                        + '<table class="table">'
                        + '<tbody>';
                    modalContents += tableContents;
                    modalContents += '<span id="addressStatus"></span></tbody></table><button class="btn btn-secondary text-center" id="addAnotherAddress">Add Address</button><div id="editArea"></div></div><div class="modal-footer">'
                        + '<button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>'
                        + '</div>'
                        + '</div>'
                        + '</div>';

                    $("#exampleModal").html(modalContents);
                    $(".delete").click(function () {
                        let idforbutton = this.id;

                        $.ajax({
                            url: '/api/values/deleteaddress',
                            method: 'POST',
                            data: { addressId: idforbutton, custId: idforcustomer }
                        })
                            .done(function (result) {
                                $("#addressStatus").html('<div class="alert alert-danger alert-dismissible fade show" role="alert">' + result + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

                                
                            });
                    });

                    $("#addAnotherAddress").click(function () {
                        let editAddressHTML = '<input id="cStreetName" placeholder="Street Name"/><input id="cNumber" placeholder="Street Number"/><input id="cPostalCode" placeholder="Postal Code"/><input id="cArea" placeholder="City/Area"/><br/><button id="saveAddress" class="btn btn-success">Save</button>';
                        $("#editArea").html(editAddressHTML);

                        $("#saveAddress").click(function () {
                            let cStreetName = $("#cStreetName").val();
                            let cNumber = $("#cNumber").val();
                            let cPostalCode = $("#cPostalCode").val();
                            let cArea = $("#cArea").val();

                            $.ajax({
                                url: '/api/values/createaddress',
                                method: 'POST',
                                data: { StreetName: cStreetName, Number: cNumber, PostalCode: cPostalCode, Area: cArea, custId: idforcustomer }
                            })
                                .done(function (result) {
                                    $("#addressStatus").html('<div class="alert alert-success alert-dismissible fade show" role="alert">' + result + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

                                    
                                })
                                .fail(function (xhr, status, error) {
                                    let errorMessages = xhr.responseJSON;
                                    let concatinatedErrorMessages = "";
                                    $.each(errorMessages, function (index, item) {

                                        concatinatedErrorMessages += item[0] + " ";
                                    });
                                    $("#addressStatus").html('<div class="alert alert-danger alert-dismissible fade show" role="alert">' + concatinatedErrorMessages + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

                                    
                                });

                        });
                    });

                    $(".editAddress").editable({
                        type: 'text',
                        url: '/api/values/editaddress',
                        success: function (response) {
                            $("#addressStatus").html('<div class="alert alert-success alert-dismissible fade show" role="alert">' + response + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
                        },
                        fail: function (response) {
                            $("#addressStatus").html('<div class="alert alert-danger alert-dismissible fade show" role="alert">' + response + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');
                        }
                    });

                    $('#exampleModal').modal(show = true);
                });
            });
        })

        .fail(function (xhr, status, error) {
            $("#status").html(xhr.responseText);

        });
});

$("#createNew").click(function () {
    let cfirstName = $("#cfirstName").val();
    let clastName = $("#clastName").val();
    let cgender = $("#cgender").val();
    let cemail = $("#cemail").val();
    let cage = $("#cage").val();

    $.ajax({
        url: '/api/values/createcustomer',
        method: 'POST',
        data: { FirstName: cfirstName, LastName: clastName, Gender: cgender, Email: cemail, Age: cage }
    })
        .done(function (result) {
            $("#status").html('<div class="alert alert-success alert-dismissible fade show" role="alert">' + result + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

            $("#getAll").click();
        })

        .fail(function (xhr, status, error) {
            let errorMessages = xhr.responseJSON;
            let concatinatedErrorMessages = "";

            $.each(errorMessages, function (index, item) {
                concatinatedErrorMessages += item[0] + " ";
            });
            $("#status").html('<div class="alert alert-warning alert-dismissible fade show" role="alert">' + concatinatedErrorMessages + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

        });
});

$("#seedCustomers").click(function () {

    $.ajax({
        url: '/api/values/seedcustomers',
        method: 'GET'
    })
        .done(function (result) {
            $("#status").html('<div class="alert alert-success alert-dismissible fade show" role="alert">' + result + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>');

            $("#getAll").click();
        })

        .fail(function (xhr, status, error) {
            $("#status").html('<div class="alert alert-warning alert-dismissible fade show" role="alert">' + xhr.responseText + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></h2>'); 
        });
});