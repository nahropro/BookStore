var currentSlideIndexs=[];
var slideCount = [];
var backBtns;
var nextBtns;

$(function () {
    initObjectsFormSlide();
    initFormSlide();
    eventsSlideForms();
});

function initObjectsFormSlide() { 
    backBtns = $('.form-slide-action-back');
    nextBtns = $('.form-slide-action-next');
}

function initFormSlide() {
    $('.form-slides').each(function (index,formSlide) {
        //Get slide counts
        slideCount[$(formSlide).attr('id')] = $(formSlide).find('.slide').length;
       
        //If has initial slide index get it and put it in current-slide-index otherwise show first slide
        gotoSlide(($(formSlide).attr('initial-slide-index') ? $(formSlide).attr('initial-slide-index'):0), $(formSlide));
    });
}

function eventsSlideForms() {
    backBtns.on('click', backSlide);
    nextBtns.on('click', nextSlide);
}

function gotoSlide(index,$formSlide) {
    currentSlideIndexs[$formSlide.attr('id')]=index;    //set current-slide-index to passed index parameter based on id
   
    $('#' + $formSlide.attr('id')+'.form-slides>.slide').hide();  //Hide all slides for first load
    $('#' + $formSlide.attr('id') +'.form-slides>.slide[slide-order=' + index + ']').show(); //Show the previous slide

    showControlBtns($formSlide);  //ReRender control buttons
}

function backSlide() {
    var formSlideId = $(this).attr('control-form-slide-for');
    var $fromSlide = $('#' + formSlideId);

    if (currentSlideIndexs[formSlideId] > 0) {
        currentSlideIndexs[formSlideId]--;    //Decrese slide index

        $('#' + formSlideId + '.form-slides>.slide').hide();  //Hide all slides for first load
        $('#' + formSlideId + '.form-slides>.slide[slide-order=' + currentSlideIndexs[formSlideId] + ']').show(); //Show the previous slide

        showControlBtns($fromSlide);  //ReRender control buttons
    }
}

function nextSlide() {
    var formSlideId = $(this).attr('control-form-slide-for');
    var $fromSlide = $('#' + formSlideId);

    if (currentSlideIndexs[formSlideId] < slideCount[formSlideId] - 1) {
        currentSlideIndexs[formSlideId]++;    //Increase slide index

        $('#' + formSlideId + '.form-slides>.slide').hide();  //Hide all slides for first load
        $('#' + formSlideId + '.form-slides>.slide[slide-order=' + currentSlideIndexs[formSlideId] + ']').show(); //Show the previous slide

        showControlBtns($fromSlide);  //ReRender control buttons
    }
}

function showControlBtns($formSlide) {
    if (currentSlideIndexs[$formSlide.attr('id')]==0) {
        //If at first slide
        $('.form-slide-action-back[control-form-slide-for=' + $formSlide.attr('id') +']').attr("disabled", true);
        $('.form-slide-action-next[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", false);
        $('.form-slide-action-submit[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", true);
    }
    else if (currentSlideIndexs[$formSlide.attr('id')] == slideCount[$formSlide.attr('id')]-1) {
        //If at last slide
        $('.form-slide-action-back[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", false);
        $('.form-slide-action-next[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", true);
        $('.form-slide-action-submit[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", false);
    }
    else {
        //If at middle slides
        $('.form-slide-action-back[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", false);
        $('.form-slide-action-next[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", false);
        $('.form-slide-action-submit[control-form-slide-for=' + $formSlide.attr('id') + ']').attr("disabled", true);
    }
}