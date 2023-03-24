﻿var updatedRow;
var table;
var datatable;
var exportedCols = [];
function showSuccessMessage() {

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toastr-bottom-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "3000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    toastr.success('Saved successfully!')

    //message = 'Saved successfully!'
    //Swal.fire({
    //    icon: 'success',
    //    title: 'Success',
    //    text: message,
    //    customClass: {
    //        confirmButton: "btn btn-primary"
    //    }
    //});

}
function onModalBegin() {
    $('body :submit').attr('disable', 'disable').attr('data-kt-indicator','on');
}
function onModalComplete() {
    $('body :submit').removeAttr('disable').removeAttr('data-kt-indicator');
}
function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}
function onModalSuccess(row) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }
    var newRow = $(row);
    datatable.row.add(newRow).draw();
    KTMenu.init();
    KTMenu.initHandlers();
}

//DataTables
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});

var KTDatatables = function () {
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10,
        });
    }
    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $('.js-datatables').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));
        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }
    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }
    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables');
            if (!table) {
                return;
            }
            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

$(document).ready(function () {
    //select 
    $('.js-select2').select2();

    //DatePicker
    $('.js-datepiker').daterangepicker({
        singleDatePicker: true,
        autoApplay: true,
        drops: 'up',
        maxDate: new Date()
    });

    //Tinymce
    var options = { selector: ".js-tinymce", height: "420" };
    if (KTThemeMode.getMode() === "dark") {
        options["skin"] = "oxide-dark";
        options["content_css"] = "dark";
    }
    tinymce.init(options);

    

    //sweet Alert
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });
    //Handle bootstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');
        modal.find('#ModalLabel').text(btn.data('title'));
        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }
        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showErrorMessage();
            }
        });

        modal.modal('show');
    });
    //Handle Toogle Status
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this);
        bootbox.confirm({
            message: "Are you sure that you need to toggle this item status?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash');
                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
});
//before efit to datatable
//function onModalSuccess(item) {
//    showSuccessMessage();
//    $('#Modal').modal('hide');
//    if (updatedRow === undefined) {
//        $('tbody').append(item);
//    } else {
//        $(updatedRow).replaceWith(item);
//        updatedRow = undefined;
//    }
//    KTMenu.init();
//    KTMenu.initHandlers();
//}