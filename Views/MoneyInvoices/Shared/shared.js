var $amount;
var $discount;
var $total;

$(function () {
    initObjects();
    initEvents();
    init();
});

function init() {
    //Set flexselect to those select tags that has class flexselect
    $('select.flexselect').flexselect();

    getTotalEvent();
    toggleInfo();
}

function toggleInfo() {
    //Event for toggle item detail
    $('table').on('click', '.js-toggle-info', function () {
        $this = $(this);
        $this.parents('tr').next('tr').slideToggle(function () {
            $this.children('.icon').toggleClass('fa-chevron-circle-down').toggleClass('fa-chevron-circle-up');
        });
    });
}

function initObjects() {
    $amount = $('#Amount');
    $discount = $('#Discount');
    $total = $('#Total');
}

function initEvents() {
    $amount.change(getTotalEvent);
    $discount.change(getTotalEvent);
}

function getTotalEvent() {
    $total.val(($amount.val()*1)+($discount.val()*1));
}