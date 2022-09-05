var invoiceTotal = 0;
var remains = 0;
var cash = 0;
var loan = 0;
var $total;
var $remains;
var $cash;
var $loan;
var $submit;
var $cashGetRemainsBtn;
var $loanGetRemainsBtn;

$(function () {
    initPageObjects();
    initPage();
    pageEvents();
});

function initPageObjects() {
    $total = $('#Total');
    $remains = $('#Remains');
    $cash = $('#Cash');
    $loan = $('#Loan');
    $submit = $('#submit-btn');
    $cashGetRemainsBtn = $('#cash-get-remains');
    $loanGetRemainsBtn = $('#loan-get-remains');
}

function initPage() {
    //Set flexselect to those select tags that has class flexselect
    $('select.flexselect').flexselect();

    calcInvoiceTotal(); //Calculate invoice total
}

function pageEvents() {
    $('table>tbody').on('change', 'input,select', lastTrChangeEvent);
    $('table>tbody').on('change', '.js-amount', calcInvoiceTotal);
    $('table>tbody').on('click', '.js-remove', removeRowEvent);
    $cash.on('change', amountChange);
    $loan.on('change', amountChange);
    $('form').on('submit', submitEvent);
    $cashGetRemainsBtn.on('click', cashGetRemains);
    $loanGetRemainsBtn.on('click', loanGetRemains);
}

function cashGetRemains() {
    //Put the remains in chash field
    $cash.val(cash+remains);

    //Recalculate remains
    amountChange();
}

function loanGetRemains() {
    //Put the remains in loan field
    $loan.val(loan+remains);

    //Recalculate remains
    amountChange();
}

function lastTrChangeEvent() {
    $tr = $(this).parents('tr');

    //Change inputs and selects in last row, create another empty row
    if ($tr.closest("tr").is(":last-child")) {
        //Get new empty row
        $.get('/SpendInvoices/GetItemRow', function (data) {
            //Add the new row to table body
            $('table>tbody').append(data);

            //Set flexselect to those select tags that has class flexselect
            $('select.flexselect').flexselect();
        });
    }
}

function calcInvoiceTotal() {
    var total = parseFloat(0);

    //Get all row tatal texts
    $rowTotals = $('.js-amount');

    //Go throw all row totals
    $rowTotals.each(function (i, d) {
        //If paresed then sum it with others
        if (parseFloat($(d).val())) {
            total += parseFloat($(d).val());
        }
    });

    //Set invoiceTotal to a globale variable
    invoiceTotal = total;

    //Set total to total and remains fileld in second slide
    $total.val(total);
    amountChange(); //Calculate remains

    //Show invoice total in the table footer
    $('#invoice-total').text(total.toLocaleString('en'));
}

function removeRowEvent() {
    //Get parent tr
    $tr = $(this).parents('tr');

    //If parent tr is not the last one, then remove the row
    if (!$tr.closest("tr").is(":last-child")) {
        $tr.remove();
        calcInvoiceTotal(); //Recalculate total
    }
}

function amountChange() {
    //reset cash and loan
    cash = 0;
    loan = 0;

    //Get cash and loan
    if (parseFloat($cash.val())) {
        cash = parseFloat($cash.val());
    }

    if (parseFloat($loan.val())) {
        loan = parseFloat($loan.val());
    }

    //Calculate remains
    remains = invoiceTotal - (cash + loan);

    //Show remains in remains field
    $remains.val(remains);
}

function submitEvent() {
    //Get all trs in the body
    $trs = $('table>tbody>tr');

    //Get parent form
    $parentForm = $(this).parents('form');

    //Go throw all trs and change names
    $trs.each(function (i, tr) {
        if (!$(tr).closest("tr").is(":last-child")) {
            //Change the name of inputs that mtchs with collection naming
            $(tr).find('.js-item-type').attr('name', 'Items[' + i + '].TypeId');
            $(tr).find('.js-amount').attr('name', 'Items[' + i + '].Amount');
            $(tr).find('.js-note').attr('name', 'Items[' + i + '].Note');
        } else {
            //If tr is the last remove name on them, avoiding null reference
            $(tr).find('.js-spend-type').removeAttr('name');
            $(tr).find('.js-amount').removeAttr('name');
            $(tr).find('.js-note').removeAttr('name');
        }
    });
}