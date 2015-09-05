/*
 * Abstract   ：iScroll插件调用
 * Author     ：蔡应	
 * Time       ：2014年2月13日11:50:54
 */
 

var oIScrollBox = $(".scroll_wrap")[0]; 
	
	/*设置iScrollBox高度*/
	oIScrollBox.style.height = "171px" ;
	window.onresize = function(){
		oIScrollBox.style.height = "171px";		
	}
	
	/*调用iScroll插件*/
	$(function(){
		var myscroll= new iScroll("iScrollBox",{fixedScrollbar:true});
	})
		
	
	
