$(function () {
    //Set flexselect to those select tags that has class flexselect
    $('select.flexselect').flexselect();

    toggleInfo();
});

//Toggle info row
function toggleInfo() {
    //Event for toggle item detail
    $('table').on('click', '.js-toggle-info', function () {
        $this = $(this);
        $this.parents('tr').next('tr').slideToggle(function () {
            $this.children('.icon').toggleClass('fa-chevron-circle-down').toggleClass('fa-chevron-circle-up');
        });
    });
}