// 触摸焦点图
for(var i=0; i<$(".scroll_wrap").length;i++){
    fnZonePic(i);
};

function fnZonePic(iNum){
    var parScroll = $(".scroll_wrap").eq(iNum);
    var oScroll = $(".scroll").eq(iNum);
    var aScroll = $(".scroll").eq(iNum).find("li");
    var aScrollIcon = $(".scroll_bar").eq(iNum).find("li");
    var iNow = 0;

    for(var i=0; i<aScroll.length ; i++){
        fnPic(i);
    }

    function fnPic(num){
    // 触摸开始
    aScroll[num].addEventListener('touchstart',function(event){
            event.preventDefault(); //滚动时阻止浏览器滚动或缩放
        if (event.targetTouches.length == 1){
            touch = event.targetTouches[0];
            startX = touch.clientX ;       
            iLeft = oScroll[0].offsetLeft ;
        }
    },false);
    // 触摸移动
    aScroll[num].addEventListener('touchmove', function(event) {
            event.preventDefault(); //滚动时阻止浏览器滚动或缩放

          // 如果这个元素的位置内只有一个手指的话
          if (event.targetTouches.length == 1) {
            touch = event.targetTouches[0];
            x = Number(touch.pageX); //页面触点X坐标  
            oScroll[0].style.left = iLeft+x-startX+"px" ; 
          }
    }, false);
    // 触摸结束
    aScroll[num].addEventListener('touchend',function(event){
            //运动
            if( x-startX > 50){     // 向右
                if( iNow >0 ){
                    iNow-- ;
                }
            }else if( x-startX < -50){  // 向左
                if( iNow < aScroll.length-1 ){
                    iNow++ ;
                }        
            }
            // 既不向右，也不向左
            oScroll.animate({"left":-320*iNow});  
            fnIcon(iNow);     
    },false);

    // function fnIcon(a){
    //     for( var j=0; j<aScroll.length ; j++ ){
    //         aScrollIcon[j].className = "" ;
    //     }
    //     aScrollIcon[a].className = "active" ;
    // }

    function fnIcon(iNum){
        aScrollIcon.eq(iNum).addClass("active").siblings().removeClass("active");
    }
    // console.//Log(iNum)

    }
}


// 顶部二级滑动导航
fnSecond_nav();
function fnSecond_nav(){
    var oSecond_nav = $(".second_nav");
    var aScroll = $(".second_nav_btn a");
    var bottomArrow = $(".bottom_arrow");
    var topArrow = $(".top_arrow");

    if(aScroll.length<3){
        aScroll.css({"padding":"0px","width":"160px"});
        bottomArrow.css("display","none");
    }
    if(aScroll.length==3){
        aScroll.css({"padding":"0px","width":"106px"});
        bottomArrow.css("display","none");
    }

    bottomArrow.click(function(){
        aScroll.css({"padding":"0px","width":"106px"});
        oSecond_nav.css("height","auto");
        oSecond_nav.css("padding-bottom","35px");
        // bottomArrow.css("width","100%");

        if(oSecond_nav.css("height","auto")){
            bottomArrow.hide();
            topArrow.show();
        }
    });

    topArrow.click(function(){
        oSecond_nav.css({"height":"35px","padding-bottom":"0px"});
        // bottomArrow.css("width","30px");
        aScroll.css({"width":"auto","padding":"0px 16px"});

        if(oSecond_nav.css("height","35px")){
            topArrow.hide();
            bottomArrow.show();
        }
    });

    aScroll.click(function(){
        var iThis = $(this).index();
        aScroll.eq(iThis).addClass("active").siblings().removeClass("active");
        // console.//Log(iThis)
    });
}


/*园区美图*/
// fnScrollPicUl3();
// function fnScrollPicUl3(){
//     var parScroll = $("#fnScrollPicUl3");
//     var oScroll = parScroll.find(".picture_scrul");
//     var aScroll = parScroll.find(".picture_scrul ul");
//     var oScrollIcon=$(".pictureNav");
//     var aScrollIcon = $(".pictureNav a");
//     var iNow = 0;

//     for(var i=0; i<aScroll.length ; i++){
//         fnPic(i);
//     }
//     function fnPic(num){
//     // 触摸开始
//     aScroll[num].addEventListener('touchstart',function(event){
//         if (event.targetTouches.length == 1){
//             touch = event.targetTouches[0];
//             startX = touch.clientX ;       
//             iLeft = oScroll[0].offsetLeft ;
//         }
//     },false);
//     // 触摸移动
//     aScroll[num].addEventListener('touchmove', function(event) {
//           // 如果这个元素的位置内只有一个手指的话
//           if (event.targetTouches.length == 1) {
//             touch = event.targetTouches[0];
//             x = Number(touch.pageX); //页面触点X坐标  
//             oScroll[0].style.left = iLeft+x-startX+"px" ; 
//           }
//     }, false);
//     // 触摸结束
//     aScroll[num].addEventListener('touchend',function(event){
//             //运动
//             if( x-startX > 50){     // 向右
//                 if( iNow >0 ){
//                     iNow-- ;
//                 }
//             }else if( x-startX < -50){  // 向左
//                 if( iNow < aScroll.length-1 ){
//                     iNow++ ;
//                 }        
//             }
//             // 既不向右，也不向左
//             oScroll.animate({"left":-300*iNow});  
//             fnIcon(iNow);     
//     },false);
//     function fnIcon(a){
//         for( var j=0; j<aScroll.length ; j++ ){
//             aScrollIcon[j].className = "" ;
//         }
//         aScrollIcon[a].className = "active" ;
//         $(".pictureNav em").animate({"left":106*a});
//     }
//     }
// }