$(function () {
    //Contain all events
    events();

    //Contain all init functions
    init();
});

function init() {
}

function events() {
    //All events
    $('table>tbody').on('change', 'input', lastTrChangeEvent);
    $('table>tbody').on('change', 'select', lastTrChangeEvent);
    $('table>tbody').on('click', '.js-remove', removeRowEvent);
    $('form').on('submit', submitEvent);
}

function lastTrChangeEvent() {
    $tr = $(this).parents('tr');

    //Change inputs and selects in last row, create another empty row
    if ($tr.closest("tr").is(":last-child")) {
        //Get new empty row
        $.get('/StoreTransferInvoices/GetRowPartial', function (data) {
            //Add the new row to table body
            $('table>tbody').append(data);

            //Set flexselect to those select tags that has class flexselect
            $('select.flexselect').flexselect();
        });
    }
}

function removeRowEvent() {
    //Get parent tr
    $tr = $(this).parents('tr');

    //If parent tr is not the last one, then remove the row
    if (!$tr.closest("tr").is(":last-child")) {
        $tr.remove();
    }
}

function submitEvent() {
    //Get all trs in the body
    $trs = $('table>tbody>tr');
    //Get 
    $parentForm=$(this).parents('form');

    //Go throw all trs and change names
    $trs.each(function (i, tr) {
        if (!$(tr).closest("tr").is(":last-child")) {
            //Change the name of inputs that mtchs with collection naming
            $(tr).find('.js-book-edition').attr('name', 'Items[' + i + '].BookEditionId');
            $(tr).find('.js-qtt').attr('name', 'Items[' + i + '].Qtt');
        } else {
            //If tr is the last remove name on them, avoiding null reference
            $(tr).find('.js-book-edition').removeAttr('name');
            $(tr).find('.js-qtt').removeAttr('name');
        }
    });
}