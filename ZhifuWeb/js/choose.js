$(".jian").click(function () {
    var mark = $(this).attr('mark');
    var i = parseInt($(this).siblings('em').html());
    var price = parseFloat($(this).attr('price'));
    if (isNaN(i) || isNaN(price) || i == undefined || price == undefined) {
        location.reload();
        return false;
    }
    if (i === 0) {
        return false;
    } else {
        i = i - 1;
        $(this).siblings('em').html(i);
        $(this).siblings('input').val(i);
        $('i[mark=' + mark + ']').html(i * price);
        getzhongjia();
    }
});
$('.jia').click(function () {
    var mark = $(this).attr('mark');
    var i = parseInt($(this).siblings('em').html());
    var price = parseFloat($(this).attr('price'));
    if (isNaN(i) || isNaN(price) || i == undefined || price == undefined) {
        location.reload();
        return false;
    }
    
    if (i === 100) {
        return false;
    } else {
        i = i + 1;
        $(this).siblings('em').html(i);
        $(this).siblings('input').val(i);
        $('i[mark=' + mark + ']').html(i * price);
        getzhongjia();
    }
});
$('.ticket_btn_box_right a').click(function () {
    var inputs = $('input');
    var num = 0;
    for (var i = 0; i < inputs.length; i++) {
        num = num + parseInt(inputs.eq(i).val());
    }
    if (num > 0) {
        $('form:first').submit();
    } else {
        alert('请至少选择一张票!');
    }
});
function getzhongjia() {
    var total = 0, num = 0;
    for (var i = 0; i < $(".count").length; i++) {
        total = total + $('.count').eq(i).html() * $('.count').eq(i).attr('price');
        num = num + parseInt($('.count').eq(i).html());
    }
    $(".zhongjia").html(total);
    $('.piaoshu').html(num);
}
$(function() {
    getzhongjia();
});
