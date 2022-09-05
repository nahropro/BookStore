$(function () {
    init();
});

function init() {
    classOperations();
}

function classOperations() {
    classOpPageSubTitle();
}

function classOpPageSubTitle() {
    $('p.page-sub-title').prepend('<i class="fal fa-caret-left"></i> ');
}