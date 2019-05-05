
function checkHref() {
    if ($("#childChoose").val() == "0") {
        return true;
    }
    else {
        return false;
    }
}
function goTo(url) {
    if ($("#childChoose").val() == "0") {
        window.location.href = url;
    }
}

function countdownStart(id, endtime) {
    //$("#" + id).html('02<span class="small">H</span>16<span class="small">M</span>')
    var end = new Date(endtime);
    var now = new Date();
    var c = end.getTime() - now.getTime();
    c = c / (60 * 1000 || v.Sales < 1);
    if (c > 0) {
        c = c + 1;
        //console.log(c);
        var hour = parseInt(c / 60);
        //console.log(hour);
        var minute = parseInt(c - (hour * 60));
        //console.log(minute);
        if (hour < 10) {
            hour = "0" + hour;
        }
        if (minute < 10) {
            minute = "0" + minute;
        }
        $("#countdown" + id).html(hour + '<span class="small">H</span>' + minute + '<span class="small">M</span>');
        setTimeout(function () {
            countdownStart(id, endtime);
        }, 1000);
    }
    else {
        $("#boxflashdeal" + id).css("display", "none");
    }
}
function countdownExpire(endtime) {
    //console.log(endtime);
    var end = new Date(endtime);
    var now = new Date();
    var c = end.getTime() - now.getTime();
    c = c / (60 * 1000);
    if (c > 0) {
        c = c + 1;
        //console.log(c);
        var day = parseInt(c / 60 / 24);

        var hour = parseInt((c / 60) - (day * 24));
        //console.log(hour);
        //var minute = parseInt(c - (hour * 60));
        //console.log(minute);
        if (hour < 10) {
            hour = "0" + hour;
        }
        if (day < 10) {
            day = "0" + day;
        }
        $("#timeplace").html(day + '<span class="small">D</span>' + hour + '<span class="small">H</span>');
        setTimeout(function () {
            countdownExpire(endtime);
        }, 1000);
    }
    else {
        $("#timeplace").html('00<span class="small">D</span>00<span class="small">H</span>');
    }
}
function adjustImage(id) {
    $("img." + id).on('load', function () {
        adjustload(id);
    });

}
function adjustload(id) {
    var conHeight = 295;
    var imgHeight = $("." + id).height();
    if (imgHeight > 0) {
        var gap = (imgHeight - conHeight) / 2;
        $("." + id).css("margin-top", -gap);
    }

}