$(function () {
    $.fn.editable.defaults.mode = 'inline';

});

$("#countCustomers").click(function () {
    $.ajax({
        url: '/api/values/countcustomers',
        method: 'GET'
    })
        .done(function (result) {
            $('#status').text(result);
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
            $("#status").text(result);
        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
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
                    '<td>' + '<a href="#" class="edit" data-name="firstName" data-type="text" data-pk="' + item.id + '" data-title="Enter First Name">' + item.firstName + '</a></td>' +
                    '<td>' + '<a href="#" class="edit" data-name="lastName" data-type="text" data-pk="' + item.id + '" data-title="Enter Last Name">' + item.lastName + '</a></td>' +
                    '<td>' + '<a href="#" class="edit" data-name="gender" data-type="text" data-pk="' + item.id + '" data-title="Enter Gender">' + item.gender + '</a></td>' +
                    '<td>' + '<a href="#" class="edit" data-name="email" data-type="text" data-pk="' + item.id + '" data-title="Enter Email">' + item.email + '</a></td>' +
                    '<td>' + '<a href="#" class="edit" data-name="age" data-type="text" data-pk="' + item.id + '" data-title="Enter Age">' + item.age + '</a></td>' +
                    '<td>' + '<a href="#" class="address" id="' + item.id + '" data-title="Adress"><button class="btn btn-info">A</button></a></td>' +
                    '<td>' + '<a href="#" class="delete" id="' + item.id + '" data-title="Delete"><button class="btn btn-danger">X</button></a></td>' +
                    '</tr>';
            });
            generatedResult += "</tbody ></table ><hr />";

            $("#dataTable").html(generatedResult);

            $(".edit").editable({
                type: 'text',
                url: '/api/values/editcustomer',
                emptyMessage: '<em>Please write something.</em>'
            });

            $(".delete").click(function () {
                let deleteIdNumber = this.id;
                $.ajax({
                    url: '/api/values/deleteacustomer',
                    method: 'POST',
                    data: { id: deleteIdNumber }
                }).done(function () {
                    $("#getAll").click();
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
                                + '</th><td><a href="#" class="editAddress" data-name="streetName" data-type="text" data-pk="' + item.id + '" data-title="Enter Street Name">' + item.streetName + '</a></td>'
                                + '</th><td><a href="#" class="editAddress" data-name="number" data-type="text" data-pk="' + item.id + '" data-title="Enter Street Number">' + item.number + '</a></td>'
                                + '</th><td><a href="#" class="editAddress" data-name="postalCode" data-type="text" data-pk="' + item.id + '" data-title="Enter Postal Code">' + item.postalCode + '</a></td>'
                                + '</th><td><a href="#" class="editAddress" data-name="area" data-type="text" data-pk="' + item.id + '" data-title="Enter Area Name">' + item.area + '</a></td>'
                                + '</th><td class="delete" id=' + item.id + '><button class="btn btn-danger">X</button></td>'
                                + '</td></tr>';
                        });
                    }

                }).fail(function () {
                    tableContents = "An error has occured during the process.";
                }).always(function () {
                    let modalContents = '<div class="modal-dialog" role="document">'
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
                    modalContents += '</tbody></table><button class="btn btn-secondary text-center" id="addAnotherAddress">Add Address</button><div id="editArea"></div></div><div class="modal-footer">'
                        + '<span id="addressStatus"></span><button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>'
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
                                $("#addressStatus").html("<strong style='color:red'>" + result + "</strong>");
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

                                    $("#addressStatus").html("<strong style='color:green'>" + result + "</strong>");
                                })
                                .fail(function (xhr, status, error) {
                                    let errorMessages = xhr.responseJSON;
                                    let concatinatedErrorMessages = "";
                                    $.each(errorMessages, function (index, item) {
                                        
                                        concatinatedErrorMessages += item[0] + " ";
                                    });

                                    $("#addressStatus").html("<strong style='color:red'>" + concatinatedErrorMessages + "</strong>");
                                });

                        });
                    });

                    $(".editAddress").editable({
                        type: 'text',
                        url: '/api/values/editaddress',
                        success: function (response, newValue) {
                            if (response.status === 'error') {
                                console.log(response);
                            }
                            else {
                                console.log(response);
                            }
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
            $("#status").text(result);
            $("#getAll").click();
        })

        .fail(function (xhr, status, error) {
            let errorMessages = xhr.responseJSON;
            let concatinatedErrorMessages = "";

            $.each(errorMessages, function (index, item) {
                concatinatedErrorMessages += item[0] + " ";
            });

            $("#status").text(concatinatedErrorMessages);
        });
});

$("#seedCustomers").click(function () {

    $.ajax({
        url: '/api/values/seedcustomers',
        method: 'GET'
    })
        .done(function (result) {
            $("#status").text(result);
            $("#getAll").click();
        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
        });
});