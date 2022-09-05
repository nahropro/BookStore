$(function () {
    //Event for remove item
    $('table').on('click', '.js-remove-edition', function () {
        id = $(this).attr('data-id');   //Get the Id of the row that stored in the tag
        $tr = $(this).parents('tr');    //Get the paret tr of the selected row
        
        nahro.daleteApi({
            url: '/api/BookEditions/' + id,
            message: 'دڵنیای له‌ سڕینه‌وه‌ی ئه‌م چاپه‌‌‌؟',
            successMessage: 'كاره‌كه‌ سه‌ركه‌وتووبوو',
            failMessage: 'هه‌ڵه‌یه‌ك ڕوویدا',
            parent: $tr
        });
    });
});