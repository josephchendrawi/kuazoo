twttr.ready(function (twttr) {
    twttr.events.bind('tweet', function (event) {
        $.ajax({
            type:'POST',
            url: $("#_url").val()+"/Home/CheckPostFB",
            data: {id:$("#InventoryItemId").val(),type:"Twitter"},
            dataType:"json",
            success:function(data){
                if(data.success){
                    bootbox.alert("Successfully shared this great deal to your friend!");
                }
            }
        });
    });
});
function shareTwitter() {
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckLimitShare",
        data: {id:$("#InventoryItemId").val(),type:"Twitter"},
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
                document.getElementById("sharetwitterid").click();
            }
        }
    });
}
function shareFB(){
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckLimitShare",
        data: {id:$("#InventoryItemId").val(),type:"Facebook"},
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
                _shareFB();                    
            }
        }
    });
}
function _shareFB() {
        
    var picurl = $("#imagebaseURL").val() + 'thumb-@Model.ImageName';
    FB.ui(
    {
        method: 'share',
        display: 'popup',
        href: document.URL,
        picture:  picurl,
        scrape:true
    },
    function(response){
        if(response && !response.error_code){
                $.ajax({
                type:'POST',
                url: $("#_url").val()+"/Home/CheckPostFB",
                data: {id:$("#InventoryItemId").val(),type:"Facebook"},
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
function pinCount(){
    $.ajax({
        url: 'http://api.pinterest.com/v1/urls/count.json?callback=?',
        data: {
            url: document.URL
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
                        url: $("#_url").val()+"/Home/CheckPostFB",
                        data: {id:$("#InventoryItemId").val(),type:"Pinterest"},
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
function sharePin(){
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckLimitShare",
        data: {id:$("#InventoryItemId").val(),type:"Pinterest"},
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
                _sharePin();                    
            }
        }
    });
}
function _sharePin(){
    var picurl = $("#imagebaseURL").val() + 'thumb-'+$("#ImageName").val();
    var pinUrl = "http://pinterest.com/pin/create/button/?url=";
    pinUrl += encodeURIComponent(document.URL);
    pinUrl += "&media="+picurl;
    pinUrl += "&description="+$("#Name").val();          
    pinCount();
    var win = window.open(pinUrl,'signin', 'height=300,width=665');
    var timer = setInterval(function() {   
        if(win.closed) {  
            clearInterval(timer);  
            //alert('closed');        
            pinCount();
        }  
    }, 1000); 
}
 $(document).ready(function () {
    createCart();
    var $win = $(window),
			isTouch = !!('ontouchstart' in window),
			clickEvent = isTouch ? 'tap' : 'click';
    (function () {
        var $example = $('#dealslide'),
		$frame = $('.frame', $example);
        $frame.mightySlider({
            speed: 1500,
            easing: 'easeOutExpo',
            viewport: 'fill',

            // Navigation options
            navigation: {
                slideSize: '100%',
                keyboardNavBy: 'slides'
            },

            // Dragging
            dragging: {
                swingSpeed: 0.1
            },

            // Pages
            pages: {
                activateOn: 'click'
            },

            // Commands
            commands: {
                pages: 1,
                buttons: 1
            },

            // Cycling
            cycling: {
                cycleBy: 'pages',
                pauseTime: 3000,
                pauseOnHover: 1
            }
        });
    })();

    (function () {
        var $carousel = $('#cover_flow'),
			$frame = $('.frame', $carousel);
        $frame.mightySlider({
            speed: 1000,
            autoScale: 1,
            viewport: 'fill',
            startAt: 0,

            // Navigation options
            navigation: {
                activateOn: clickEvent,
                slideSize: '25.31%'
            },

            // Commands options
            commands: {
                buttons: 1
            },

            // Scrolling options
            scrolling: {
                scrollBy: 1
            },
            // Cycling
            cycling: {
                cycleBy: 'pages',
                pauseTime: 4000,
                pauseOnHover: 1
            }
        });
    })();
});
function multiVariance(){
    if($("#_defaultvar").val().toLowerCase()=="true")
    {
    addToCartVari($("#_defaultvarstr").val());
    }
    else
    {
    $.fancybox.open({
        href :"#varianceList",
        width : 300,
    });
    }
}
function multiVarianceGift(){
    if($("#_defaultvar").val().toLowerCase()=="true")
    {
    addToCartGiftVari($("#_defaultvarstr").val());
    }
    else
    {
    $.fancybox.open({
        href :"#varianceListGift",
        width : 300,
    });
    }
}
function addToCart() {
    $(".loading").css("display","block");
    var id = $("#InventoryItemId").val();
    var name = $("#Name").val();
    var price =$("#Price").val();
    var dis = $("#Discount").val();
    var flashid = $("#FlashDeal_FlashDealId").val();
    if(flashid==null)flashid=0;
    var flash = $("#FlashDeal_Discount").val();
    if (flash != null) dis = parseFloat(dis)+parseFloat(flash);
    var imageurl = $("#ImageUrl").val();
    if (imageurl == null) imageurl = "";
    var prize = $("#Prize_Name").val();
    if (prize == null) prize = "";
    var prizeimageurl = $("#Prize_ImageUrl").val();
    if (prizeimageurl == null) prizeimageurl = "";
    var varieancelist = $("#Variance").val();
    var values = { id: id,variance:'', variancelist:varieancelist, qty: 1,name:name,price:price,dis:dis,imageurl:imageurl,prize:prize,prizeimageurl:prizeimageurl,flashid:flashid,gift:0};
    //console.log(values);
    
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckFreeDealUsed",
        data: {id:id},
        dataType:"json",
        success:function(data){
            if(data.success==0){
                _addToCart(values);
                $(".loading").css("display","none");
                window.location=$("#_url").val()+"/Home/CheckOut"
            }
            else if(data.success==1){
                $.ajax({
                    type:'POST',
                    url: $("#_url").val()+"/Home/BuyFreeDeal",
                    data: {id:id,flashid:flashid,variance:'',qty:1,price:price,disc:dis},
                    dataType:"json",
                    success:function(data){
                $(".loading").css("display","none");
                        window.location=data.returnUrl;
                    }
                });
            }
            else if(data.success==2){
                $(".loading").css("display","none");
                $.fancybox.close();
                bootbox.alert("Deal already used");
            }
            else if(data.success==3){
                $(".loading").css("display","none");
                window.location.replace($('#loginurl').attr('href')+"&popUp=1");
            }
        }
    });
}
function addToCartGift() {
    $(".loading").css("display","block");
    var id = $("#InventoryItemId").val();
    var name = $("#Name").val();
    var price =$("#Price").val();
    var dis = $("#Discount").val();
    var flashid = $("#FlashDeal_FlashDealId").val();
    if(flashid==null)flashid=0;
    var flash = $("#FlashDeal_Discount").val();
    if (flash != null) dis = parseFloat(dis)+parseFloat(flash);
    var imageurl = $("#ImageUrl").val();
    if (imageurl == null) imageurl = "";
    var prize = $("#Prize_Name").val();
    if (prize == null) prize = "";
    var prizeimageurl = $("#Prize_ImageUrl").val();
    if (prizeimageurl == null) prizeimageurl = "";
    var varieancelist = $("#Variance").val();
    var values = { id: id,variance:'', variancelist:varieancelist, qty: 1,name:name,price:price,dis:dis,imageurl:imageurl,prize:prize,prizeimageurl:prizeimageurl,flashid:flashid,gift:1 };
    //console.log(values);
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckFreeDealUsed",
        data: {id:id},
        dataType:"json",
        success:function(data){
            if(data.success==0){
                _addToCart(values);
                $(".loading").css("display","none");
                window.location=$("#_url").val()+"/Home/CheckOut"
            }
            else if(data.success==1){
                $.ajax({
                    type:'POST',
                    url: $("#_url").val()+"/Home/BuyFreeDeal",
                    data: {id:id,flashid:flashid,variance:'',qty:1,price:price,disc:dis},
                    dataType:"json",
                    success:function(data){
                        $(".loading").css("display","none");
                        window.location=data.returnUrl;
                    }
                });
            }
            else if(data.success==2){
                $(".loading").css("display","none");
                $.fancybox.close();
                bootbox.alert("Deal already used");
            }
            else if(data.success==3){
                $(".loading").css("display","none");
                window.location.replace($('#loginurl').attr('href')+"&popUp=1");
            }
        }
    });
}
function addToCartVari(_variance) {
    var id = $("#InventoryItemId").val();
    $(".loading").css("display","block");
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckFreeDealUsed",
        data: {id:id},
        dataType:"json",
        success:function(data){
            if(data.success==0){
                $.ajax({
                    type:'POST',
                    url: $("#_url").val()+"/Home/CheckVarianceLimit",
                    data: {id:id,variance:_variance},
                    dataType:"json",
                    success:function(data){
                    if(data.success){
                        var name = $("#Name").val();
                        //var price =$("#Price").val();
                        var vari = _variance.split('`');
                        var price =vari[1];

                        var dis = vari[2];//$("#Discount").val();
                        var flashid = $("#FlashDeal_FlashDealId").val();
                        if(flashid==null)flashid=0;
                        var flash = $("#FlashDeal_Discount").val();
                        //if (flash != null) dis = parseFloat(dis)+parseFloat(flash);
                        var imageurl = $("#ImageUrl").val();
                        if (imageurl == null) imageurl = "";
                        var prize = $("#Prize_Name").val();
                        if (prize == null) prize = "";
                        var prizeimageurl = $("#Prize_ImageUrl").val();
                        if (prizeimageurl == null) prizeimageurl = "";
                        var varieancelist = $("#Variance").val();
                        var values = { id: id,variance:_variance, variancelist:varieancelist, qty: 1,name:name,price:price,dis:dis,imageurl:imageurl,prize:prize,prizeimageurl:prizeimageurl,flashid:flashid,gift:0};
                        //console.log(values);
                        _addToCart(values);
                        $(".loading").css("display","none");
                        window.location=$("#_url").val()+"/Home/CheckOut"
                    }
                    else{
                        $(".loading").css("display","none");
                        $.fancybox.close();
                        bootbox.alert("Stock Exceed the Limit");
                    }
                }
                });
            }
            else if(data.success==1){
                var vari = _variance.split('`');
                var price =vari[1];
                var dis = vari[2];//$("#Discount").val();
                var flashid = $("#FlashDeal_FlashDealId").val();
                if(flashid==null)flashid=0;
                var flash = $("#FlashDeal_Discount").val();
                if (flash != null) dis = parseFloat(dis)+parseFloat(flash);
                $.ajax({
                    type:'POST',
                    url: $("#_url").val()+"/Home/BuyFreeDeal",
                    data: {id:id,flashid:flashid,variance:_variance,qty:1,price:price,disc:dis},
                    dataType:"json",
                    success:function(data){
                        $(".loading").css("display","none");
                        window.location=data.returnUrl;
                    }
                });
            }
            else if(data.success==2){
                        $(".loading").css("display","none");
                $.fancybox.close();
                bootbox.alert("Deal already used");
            }
            else if(data.success==3){
                        $(".loading").css("display","none");
                window.location.replace($('#loginurl').attr('href')+"&popUp=1");
            }
        }
    });    
}
    
function addToCartGiftVari(_variance) {
    var id = $("#InventoryItemId").val();
    $(".loading").css("display","block");
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/CheckFreeDealUsed",
        data: {id:id},
        dataType:"json",
        success:function(data){
            if(data.success==0){
                $.ajax({
                    type:'POST',
                    url: $("#_url").val()+"/Home/CheckVarianceLimit",
                    data: {id:id,variance:_variance},
                    dataType:"json",
                    success:function(data){
                    if(data.success){
                        var name = $("#Name").val();
                        //var price =$("#Price").val();
                        var vari = _variance.split('`');
                        var price =vari[1];
        
                        var dis = vari[2];//$("#Discount").val();
                        var flashid = $("#FlashDeal_FlashDealId").val();
                        if(flashid==null)flashid=0;
                        var flash = $("#FlashDeal_Discount").val();
                        //if (flash != null) dis = parseFloat(dis)+parseFloat(flash);
                        var imageurl = $("#ImageUrl").val();
                        if (imageurl == null) imageurl = "";
                        var prize = $("#Prize_Name").val();
                        if (prize == null) prize = "";
                        var prizeimageurl = $("#Prize_ImageUrl").val();
                        if (prizeimageurl == null) prizeimageurl = "";
                        var varieancelist = $("#Variance").val();
                        var values = { id: id,variance:_variance, variancelist:varieancelist, qty: 1,name:name,price:price,dis:dis,imageurl:imageurl,prize:prize,prizeimageurl:prizeimageurl,flashid:flashid,gift:1 };
                        //console.log(values);
                        _addToCart(values);
                        $(".loading").css("display","none");
                        window.location=$("#_url").val()+"/Home/CheckOut"
                    }
                    else{
                        $(".loading").css("display","none");
                        $.fancybox.close();
                        bootbox.alert("Stock Exceed the Limit");
                    }
                }
                });
            }
            else if(data.success==1){
                var vari = _variance.split('`');
                var price =vari[1];
                var dis = vari[2];//$("#Discount").val();
                var flashid = $("#FlashDeal_FlashDealId").val();
                if(flashid==null)flashid=0;
                var flash = $("#FlashDeal_Discount").val();
                if (flash != null) dis = parseFloat(dis)+parseFloat(flash);
                $.ajax({
                    type:'POST',
                    url: $("#_url").val()+"/Home/BuyFreeDeal",
                    data: {id:id,flashid:flashid,variance:_variance,qty:1,price:price,disc:dis},
                    dataType:"json",
                    success:function(data){
                        $(".loading").css("display","none");
                        window.location=data.returnUrl;
                    }
                });
            }
            else if(data.success==2){
                $(".loading").css("display","none");
                $.fancybox.close();
                bootbox.alert("Deal already used");
            }
            else if(data.success==3){
                $(".loading").css("display","none");
                window.location.replace($('#loginurl').attr('href')+"&popUp=1");
            }
        }
    });
}
function _addToWishList(){
    if($("#isAu").val()==0)
    {
        window.location.replace($('#loginurl').attr('href')+"&popUp=1");

//        $("#wishlistBtn").attr("disabled", "disabled");
//        $("#wishlistBtn").addClass("disabled");
//        var wishlist = {id:$("#InventoryItemId").val()};
//        var result = addToWishlist(wishlist);
//        if(result=="true"){
//                    bootbox.alert("Successfully added to wish list!");
//        }
//        else{
//                    bootbox.alert(result);
//        }
    }
    else
    {

        $("#wishlistBtn").attr("disabled", "disabled");
        $("#wishlistBtn").addClass("disabled");
    $.ajax({
        type:'POST',
        url: $("#_url").val()+"/Home/WishlistAdd",
        data: {id:$("#InventoryItemId").val()},
        dataType:"json",
        success:function(data){
            if(data.success==true){
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
 var mapopen = 0;
function locationstart(lat, lng) {
    if (mapopen == 0) {
        setTimeout(function () {
            mapopen = 1;
            initializeEditMap(lat, lng);
        }, 1000);
    }
}
var map;

var infowindow = new google.maps.InfoWindow();

function initializeEditMap(lat, lng) {

    geocoder = new google.maps.Geocoder();
    var loc = new google.maps.LatLng(lat, lng);
    var mapOptions = {
        zoom: 16,
        center: loc,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map"),
				        mapOptions);

    marker = new google.maps.Marker({
        map: map,
        draggable: true,
        animation: google.maps.Animation.DROP,
        position: loc
    });



}
function reviewPopUp(){
        $.fancybox.open({
            href: $("#_url").val()+'/Home/Review/'+$("#InventoryItemId").val(),
            type: 'iframe',
            padding: 0,
            width:658,
            afterClose: function () {
                var reviewmsg = $("#reviewMsg").val();
                if(reviewmsg=="0"){
                var id = $("#InventoryItemId").val();
                    $.ajax({
                    type:'GET',
                    url: $("#_url").val()+"/Home/ReviewCount",
                    data: {id:id},
                    dataType:"json",
                    success:function(data){
                        if(data.result!=true){
                            var html='<div class="reviewcount"><div class="ratingStar"></div> ';
                            html += data.result.Rating.toFixed(1) +'/5</div>';
                            html += '<div class="reviewdata"><a onclick="reviewPopUp()">'+data.result.ReviewCount+' Reviews</a></div>';
                            $("#reviewdetail").html(html);
                        }
                        else{
                            bootbox.alert(data.error);
                        }
                    }
                    });    
                }
                else{
                    window.location = $("#_url").val()+"/Home/Index";
                }
            }
        });
    }
    function shareEmail(){
        var height=445;
        if($("#isAu").val()==1)
        {
            height=325;
        }
        $.fancybox.open({
            href: $("#_url").val()+'/Home/ShareEmail/'+$("#InventoryItemId").val(),
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