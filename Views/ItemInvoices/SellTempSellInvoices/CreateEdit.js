$(function () {
    //Contain all events
    events();

    //Contain all init functions
    init();
});

function init() {
    //Onload calaulate totals for loading datas
    calcTotalAll();
}

function events() {
    //All events
    $('table>tbody').on('change', 'input', lastTrChangeEvent);
    $('table>tbody').on('change', 'select', lastTrChangeEvent);
    $('table>tbody').on('click', '.js-remove', removeRowEvent);
    $('table>tbody').on('change', '.js-qtt,.js-price,.js-book-edition', calcRowTotalEvent);
    $('form').on('submit', submitEvent);

    //This getPrice is set in the main page, that means this page require retriving parice of items
    if (getPrice) {
        $('table>tbody').on('change', 'select.js-book-edition', getPriceOnBookEditionChangeEvent);
    }
}

function getPriceOnBookEditionChangeEvent() {
    $select = $(this);  //Get select bookedition select list
    $price = $(this).parents('tr').find('.js-price');   //Get price input to this row
    bookEditionId = $select.val();  //Get selected id

    //Get the selected book-edition and put price into price input
    $.get('/api/BookEditions/' + bookEditionId, function (data) {
        //Check if discharge price show dischanger price otherwise show sell price
        $price.val(data.DischargePrice);
    });
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

function calcTotalAll() {
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

function lastTrChangeEvent() {
    $tr = $(this).parents('tr');

    //Change inputs and selects in last row, create another empty row
    if ($tr.closest("tr").is(":last-child")) {
        //Get new empty row
        $.get('/SellTempSellInvoices/GetRowPartial', function (data) {
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
        $tr.remove();   //Remove the row
        calcTotalAll(); //Recalculate total
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
            $(tr).find('.js-price').attr('name', 'Items[' + i + '].Price');
        } else {
            //If tr is the last remove name on them, avoiding null reference
            $(tr).find('.js-book-edition').removeAttr('name');
            $(tr).find('.js-qtt').removeAttr('name');
            $(tr).find('.js-price').removeAttr('name');
        }
    });
}