$(function () {
    init();
    events();
});

function init() {
    //Set flexselect to those select tags that has class flexselect
    $('select.flexselect').flexselect();

    //Onload calaulate totals for loading datas
    onLoadCalcTotal();
}

function events() {
    //All events
    $('table>tbody').on('change', 'input', lastTrChangeEvent);
    $('table>tbody').on('change', 'select', lastTrChangeEvent);
    $('table>tbody').on('click', '.js-remove', removeRowEvent);
    $('table>tbody').on('change', '.js-qtt,.js-price,.js-book-edition', calcRowTotalEvent);
    $('#btn-submit').on('click', submitEvent);
}

function lastTrChangeEvent() {
    $tr = $(this).parents('tr');

    //Change inputs and selects in last row, create another empty row
    if ($tr.closest("tr").is(":last-child")) {
        //Get new empty row
        $.get('/BookEditionFirstTimes/GetRowPartial', function (data) {
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

//Calcualte row total and total
function calcRowTotalEvent() {
    $tr = $(this).parents('tr');

    calcRowTotal($tr);

    //Calculate invoice total
    calcInvoiceTotal();
}

function calcRowTotal($tr) {
    var total;
    var qtt;
    var price;

    //Get price and qtt
    price = parseFloat($tr.find('.js-price').val());
    qtt = parseInt($tr.find('.js-qtt').val());

    //Get total
    total = price * qtt;
    //Put the total to row total
    $tr.find('.js-row-total').val(total.toLocaleString('en'));
    $tr.find('.js-row-total').attr('data-total', total);
}

function onLoadCalcTotal() {
    //Get all trs in the body
    $trs = $('table>tbody>tr');

    //Go throw all trs and change names
    $trs.each(function (i, tr) {
        calcRowTotal($(tr));
    });

    //Calculate invoice total
    calcInvoiceTotal();
}

function calcInvoiceTotal() {
    var total = parseFloat(0);

    //Get all row tatal texts
    $rowTotals = $('.js-row-total');

    //Go throw all row totals
    $rowTotals.each(function (i, d) {
        //If paresed then sum it with others
        if (parseFloat($(d).attr('data-total'))) {
            total += parseFloat($(d).attr('data-total'));
        }
    });

    //Show invoice total in the table footer
    $('#invoice-total').text(total.toLocaleString('en'));
}

function submitEvent() {
    //Get all trs in the body
    $trs = $('table>tbody>tr');
    //Get 
    $parentForm = $(this).parents('form');

    //Go throw all trs and change names
    $trs.each(function (i, tr) {
        if (!$(tr).closest("tr").is(":last-child")) {
            //Change the name of inputs that mtchs with collection naming
            $(tr).find('.js-book-edition').attr('name', '[' + i + '].BookEditionId');
            $(tr).find('.js-qtt').attr('name', '[' + i + '].Qtt');
            $(tr).find('.js-price').attr('name', '[' + i + '].Price');
            $(tr).find('.js-store').attr('name', '[' + i + '].StoreId');
        } else {
            //If tr is the last remove name on them, avoiding null reference
            $(tr).find('input').removeAttr('name');
            //$(tr).find('.js-qtt').removeAttr('name');
            //$(tr).find('.js-price').removeAttr('name');
            //$(tr).find('.js-store').removeAttr('name');
        }
    });

    //Submit parent form
    $parentForm.submit();
}