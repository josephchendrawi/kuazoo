
$(document).ready(function () {
//    $(".dealbox .boxprizeribbon").hover(function () {
//        surprize = 1;
//        $(this).parent().parent().parent().find(".dealboxa").css("z-index", "4");
//        $(this).parent().parent().parent().find(".dealboxa").parent().append("<span class='dealboxsurprizebg'></span");
//        $(this).parent().parent().parent().find(".dealboxa").parent().append("<span class='dealboxsurprize'></span");
//        $(this).parent().parent().parent().find(".dealboxsurprizebg").css("opacity", "1");
//        $(this).parent().parent().parent().find(".dealboxsurprizebg").css("filter", "alpha(opacity=100)");
//        $(this).parent().parent().parent().find(".boxdisc").css("display", "none");
//        $(this).parent().parent().parent().find(".boxtext").css("display", "none");
//        $(this).parent().parent().parent().find(".boxoffbeforeflashdeal").css("display", "none");
//        $(this).parent().parent().parent().find(".boxoffbigflashdeal").css("display", "none");
//        $(this).parent().parent().parent().find(".boxflashdealfooter").css("display", "none");
//        $(this).parent().parent().parent().find(".boxbarbg").parent().prepend("<span class='dealboxsurprizetext'>When the bar is full <br/>a Winner will be announced</span>");
//        $(this).parent().parent().parent().find(".boxbarbg").parent().append("<span class='dealboxsurprizetext'>Every purchase moves the bar by one.</span>");
//    }, function () {
//        $(".dealbox .dealboxsurprize").hover(function () {
//            $(this).parent().parent().parent().find(".dealboxa").hover(function () {
//            });
//        }, function () {
//            $(this).parent().parent().parent().find(".dealboxsurprizebg").remove();
//            $(this).parent().parent().parent().find(".boxdisc").css("display", "block");
//            $(this).parent().parent().parent().find(".boxtext").css("display", "block");
//            $(this).parent().parent().parent().find(".boxoffbeforeflashdeal").css("display", "block");
//            $(this).parent().parent().parent().find(".boxoffbigflashdeal").css("display", "block");
//            $(this).parent().parent().parent().find(".boxflashdealfooter").css("display", "table-cell");
//            $(this).parent().parent().parent().find(".dealboxsurprizetext").remove();
//            $(this).parent().parent().parent().find(".dealboxa").css("z-index", "2");
//            $(this).parent().parent().parent().find(".dealboxsurprize").remove();
//        });
//    });
});
twttr.ready(function (twttr) {
    twttr.events.bind('tweet', function (event) {
        var id = $("#twitterChoose").val();
            $("#twitterChoose").val("");
        $.ajax({
            type:'POST',
            url : $("#_url").val()+"/Home/CheckPostFB",
            data: {id:id,type:"Twitter"},
            dataType:"json",
            success:function(data){
                if(data.success){
                    bootbox.alert("Successfully shared this great deal to your friend!");
                }
            }
        });
    });
});
function shareTwitter(id) {
    $("#childChoose").val(1);
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckLimitShare",
        data: {id:id,type:"Twitter"},
        dataType:"json",
        success:function(data){
            var check=false;
            if(data.success && data.login){
                check=true;
            }
            else if(data.success == false && data.login==true){
                check=false;
                bootbox.alert(data.error);
            }
            else if(data.success== false && data.login==false){
                check=true;
            }
            if(check){
            $("#twitterChoose").val(id);
                document.getElementById("sharetwitterid").click();
            }
            $("#childChoose").val(0);
        }
    });
}
function shareFB(id){
    $("#childChoose").val(1);
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckLimitShare",
        data: {id:id,type:"Facebook"},
        dataType:"json",
        success:function(data){
    $("#childChoose").val(0);
            var check=false;
            if(data.success && data.login){
                check=true;
            }
            else if(data.success == false && data.login==true){
                check=false;
                bootbox.alert(data.error);
            }
            else if(data.success== false && data.login==false){
                check=true;
            }
            if(check){
                _shareFB(id);                    
            }
        }
    });
}
function _shareFB(id) {
    var imgname=  $("#inventoryimgname"+id).val();
    var url = $("#inventoryurl"+id).val(); 
    var picurl = $("#imagebaseURL").val() + 'thumb-'+imgname;
    FB.ui(
    {
        method: 'share',
        display: 'popup',
        href: url,
        picture:  picurl,
        scrape:true
    },
    function(response){
        if(response && !response.error_code){
                $.ajax({
                type:'POST',
                url : $("#_url").val()+"/Home/CheckPostFB",
                data: {id:id,type:"Facebook"},
                dataType:"json",
                success:function(data){
                    if(data.success){
                        bootbox.alert("Successfully shared this great deal to your friend!");
                    }
                }
            });
        }
    });
}
var pincount=0;
function pinCount(id){
    var url = $("#inventoryurl"+id).val(); 
    $.ajax({
        url: 'http://api.pinterest.com/v1/urls/count.json?callback=?',
        data: {
            url: url
        },
        success: function(data) {
            if(pincount==0){
                pincount = data.count;
            }
            else{
                console.log(data.count);
                if(pincount==data.count){
                    pincount=0;
                }
                else{
                    pincount=0;
                        $.ajax({
                        type:'POST',
                        url : $("#_url").val()+"/Home/CheckPostFB",
                        data: {id:id,type:"Pinterest"},
                        dataType:"json",
                        success:function(data){
                            if(data.success){
                                bootbox.alert("Successfully shared this great deal to your friend!");
                            }
                        }
                    });
                }
            }
        },
        dataType: 'jsonp'
    });
}
function sharePin(id){
    $("#childChoose").val(1);
    $.ajax({
        type:'POST',
        url :$("#_url").val()+"/Home/CheckLimitShare",
        data: {id:id,type:"Pinterest"},
        dataType:"json",
        success:function(data){
    $("#childChoose").val(0);
            var check=false;
            if(data.success && data.login){
                check=true;
            }
            else if(data.success == false && data.login==true){
                check=false;
                bootbox.alert(data.error);
            }
            else if(data.success== false && data.login==false){
                check=true;
            }
            if(check){
                _sharePin(id);                    
            }
        }
    });
}
function _sharePin(id){
    var url = $("#inventoryurl"+id).val(); 
    var name = $("#inventoryname"+id).val();
    var imgname=  $("#inventoryimgname"+id).val();
    var picurl = $("#imagebaseURL").val() + 'thumb-'+imgname;
    console.log(url);
    var pinUrl = "http://pinterest.com/pin/create/button/?url=";
    pinUrl += encodeURIComponent(url);
    pinUrl += "&media="+picurl;
    pinUrl += "&description="+name;          
    pinCount(id);
    var win = window.open(pinUrl,'signin', 'height=300,width=665');
    var timer = setInterval(function() {   
        if(win.closed) {  
            clearInterval(timer);     
            pinCount(id);
        }  
    }, 1000); 
}
function addWishList(e,id){
    $("#childChoose").val(1);
    if($("#isAu").val()==0)
    {
        window.location.replace($('#loginurl').attr('href')+"&popUp=1");

//        var wishlist = {id:id};
//        var result = addToWishlist(wishlist);
//        if(result=="true"){
//            $("#inventoryfav"+id).addClass("active");
//                bootbox.alert("Successfully added to wish list!",function(){$("#childChoose").val(0);});
//        }
//        else{
//                bootbox.alert(result,function(){$("#childChoose").val(0);});
//        }
    }
    else
    {
    $.ajax({
        type:'POST',
        url : $("#_url").val()+"/Home/WishlistAdd",
        data: {id:id},
        dataType:"json",
        success:function(data){
            $("#childChoose").val(0);
            if(data.success==true){
                $("#inventoryfav"+id).addClass("active");
                bootbox.alert("Successfully added to wish list!");
            }
            else{
                if(data.error=="Unauthorized access."){
                        
                }
                else{
                bootbox.alert(data.error);
                }
            }
        }
    });
    }
}
function shareEmail(id) {
    $("#emailMsg").val("");
    $("#childChoose").val(1);
    setTimeout(function(){
    $("#childChoose").val(0);},1000);
    var height=445;
    if($("#isAu").val()==1)
    {
        height=325;
    }
    $.fancybox.open({
        href: $("#_url").val()+'/Home/ShareEmail/'+id,
        type: 'iframe',
        padding: 0,
        width: 600,
        maxHeight:height,
        afterClose: function() {
            var msg= $("#emailMsg").val();
            if(msg!=""){
                bootbox.alert(msg);
            }
        }
    });
}

function multiVariance(id){
    $("#childChoose").val(1);
    if($("#VarianceDefault"+id).val().toLowerCase()=="true"){
        addToCartVari($("#VarianceDefaultStr"+id).val(),id);
    }
    else{
        $.fancybox.open({
            href :"#varianceList"+id,
            width : 300,
            afterClose: function () {
        $("#childChoose").val(0);

            }
        });
    }
}
function addToCart(id) {
    $(".loading").css("display", "block");
    $("#childChoose").val(1);
    createCart();
        var invid = $("#InventoryItemId"+id).val();
    var name = $("#Name"+id).val();
    var price =$("#Price"+id).val();
    var dis = $("#Discount"+id).val();
    var flashid = $("#FlashDeal_FlashDealId"+id).val();
    if(flashid==null)flashid=0;
    var flash = $("#FlashDeal_Discount"+id).val();
    //if (flash != null) dis = flash;
    var imageurl = $("#ImageUrl"+id).val();
    if (imageurl == null) imageurl = "";
    var prize = $("#Prize_Name"+id).val();
    if (prize == null) prize = "";
    var prizeimageurl = $("#Prize_ImageUrl"+id).val();
    if (prizeimageurl == null) prizeimageurl = "";
    var varieancelist = $("#Variance"+id).val();
    var values = { id: invid,variance:'', variancelist:varieancelist, qty: 1,name:name,price:price,dis:dis,imageurl:imageurl,prize:prize,prizeimageurl:prizeimageurl,flashid:flashid,gift:0};
    // console.log(values);
    $.ajax({
        type: 'POST',
        url: $("#_url").val() + "/Home/CheckFreeDealUsed",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (data.success == 0) {
                _addToCart(values);
                $(".loading").css("display", "none");
                window.location = $("#_url").val() + "/Home/CheckOut";
            }
            else if (data.success == 1) {
                $.ajax({
                    type: 'POST',
                    url: $("#_url").val() + "/Home/BuyFreeDeal",
                    data: { id: id, flashid: flashid, variance: '', qty: 1, price: price, disc: dis },
                    dataType: "json",
                    success: function (data) {
                        $(".loading").css("display", "none");
                        window.location = data.returnUrl;
                    }
                });
            }
            else if (data.success == 2) {
                $(".loading").css("display", "none");
                $.fancybox.close();
                bootbox.alert("Deal already used");
            }
            else if (data.success == 3) {
                $(".loading").css("display", "none");
                window.location.replace($('#loginurl').attr('href') + "&popUp=1");
            }
        }
    });
}
function addToCartVari(_variance,id) {
    $("#childChoose").val(1);
    $(".loading").css("display", "block");
    $.ajax({
        type: 'POST',
        url: $("#_url").val() + "/Home/CheckFreeDealUsed",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            if (data.success == 0) {
                $.ajax({
                    type: 'POST',
                    url: $("#_url").val() + "/Home/CheckVarianceLimit",
                    data: { id: id, variance: _variance },
                    dataType: "json",
                    success: function (data) {
                        if (data.success) {
                            createCart();
                            var invid = $("#InventoryItemId" + id).val();
                            var name = $("#Name" + id).val();
                            //var price =$("#Price"+id).val();
                            var vari = _variance.split('`');
                            var price = vari[1];

                            var dis = vari[2]; //$("#Discount"+id).val();
                            var flashid = $("#FlashDeal_FlashDealId" + id).val();
                            if (flashid == null) flashid = 0;
                            var flash = $("#FlashDeal_Discount" + id).val();
                            //if (flash != null) dis = flash;
                            var imageurl = $("#ImageUrl" + id).val();
                            if (imageurl == null) imageurl = "";
                            var prize = $("#Prize_Name" + id).val();
                            if (prize == null) prize = "";
                            var prizeimageurl = $("#Prize_ImageUrl" + id).val();
                            if (prizeimageurl == null) prizeimageurl = "";
                            var varieancelist = $("#Variance" + id).val();
                            var values = { id: invid, variance: _variance, variancelist: varieancelist, qty: 1, name: name, price: price, dis: dis, imageurl: imageurl, prize: prize, prizeimageurl: prizeimageurl, flashid: flashid, gift: 0 };
                            // console.log(values);
                            _addToCart(values);
                            $(".loading").css("display", "none");
                            window.location = $("#_url").val() + "/Home/CheckOut";
                        }
                        else {
                            $(".loading").css("display", "none");
                            $.fancybox.close();
                            bootbox.alert("Stock Exceed the Limit");
                        }
                    }
                });
            }
            else if (data.success == 1) {
                var vari = _variance.split('`');
                var price = vari[1];
                var dis = vari[2]; //$("#Discount").val();
                var flashid = $("#FlashDeal_FlashDealId").val();
                if (flashid == null) flashid = 0;
                var flash = $("#FlashDeal_Discount").val();
                if (flash != null) dis = parseFloat(dis) + parseFloat(flash);
                $.ajax({
                    type: 'POST',
                    url: $("#_url").val() + "/Home/BuyFreeDeal",
                    data: { id: id, flashid: flashid, variance: _variance, qty: 1, price: price, disc: dis },
                    dataType: "json",
                    success: function (data) {
                        $(".loading").css("display", "none");
                        window.location = data.returnUrl;
                    }
                });
            }
            else if (data.success == 2) {
                $(".loading").css("display", "none");
                $.fancybox.close();
                bootbox.alert("Deal already used");
            }
            else if (data.success == 3) {
                $(".loading").css("display", "none");
                window.location.replace($('#loginurl').attr('href') + "&popUp=1");
            }
        }
    });
}