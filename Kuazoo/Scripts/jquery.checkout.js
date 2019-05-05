function createCart() {
    if (localStorage.getItem("kuazoo-cart") == null) {
        var cart = {};
        cart.items = [];
        localStorage.setItem("kuazoo-cart", JSON.stringify(cart));
    }
}
function emptyCart() {
    //localStorage.clear();
    localStorage.removeItem('kuazoo-cart');
    localStorage.removeItem('kuazoo-variance');
    localStorage.removeItem('kuazoo-ship');
    localStorage.removeItem('kuazoo-bill');
    localStorage.removeItem('kuazoo-kpoint');
    localStorage.removeItem('kuazoo-promotion');
    localStorage.removeItem('kuazoo-transaction');
}
function _addToCart(values) {
    //localStorage.removeItem('kuazoo-cart');
    emptyCart();
    createCart();
    var cartValue = localStorage.getItem("kuazoo-cart");
    var cartObject = JSON.parse(cartValue);
    var cartCopy = cartObject;
    var items = cartCopy.items;
//    var check = 0;
//    for (var i = 0; i < items.length; i++) {
//        if (items[i].id == values.id) {
//            items[i].qty = items[i].qty + 1;
//            check = 1;
//            break;
//        }
//    }
//    if (check == 0) {
//        items.push(values);
    //    }
    items.push(values);
    localStorage.setItem("kuazoo-cart", JSON.stringify(cartCopy));
}
function _updateToCart(id,qty,variance) {
    var cartValue = localStorage.getItem("kuazoo-cart");
    var cartObject = JSON.parse(cartValue);
    var cartCopy = cartObject;
    var items = cartCopy.items;
    for (var i = 0; i < items.length; i++) {
        if (items[i].id == id) {
            items[i].qty = qty;
            items[i].variance = variance;
            break;
        }
    }
    localStorage.setItem("kuazoo-cart", JSON.stringify(cartCopy));
}
function _updateToCart2(id, qty, variance, price) {
    var cartValue = localStorage.getItem("kuazoo-cart");
    var cartObject = JSON.parse(cartValue);
    var cartCopy = cartObject;
    var items = cartCopy.items;
    for (var i = 0; i < items.length; i++) {
        if (items[i].id == id) {
            items[i].qty = qty;
            items[i].variance = variance;
            items[i].price = price;
            break;
        }
    }
    localStorage.setItem("kuazoo-cart", JSON.stringify(cartCopy));
}
function _updateToCart3(id, qty, variance, price,disc) {
    var cartValue = localStorage.getItem("kuazoo-cart");
    var cartObject = JSON.parse(cartValue);
    var cartCopy = cartObject;
    var items = cartCopy.items;
    for (var i = 0; i < items.length; i++) {
        if (items[i].id == id) {
            items[i].qty = qty;
            items[i].variance = variance;
            items[i].price = price;
            items[i].dis = disc;
            break;
        }
    }
    localStorage.setItem("kuazoo-cart", JSON.stringify(cartCopy));
}
function getCart() {
    var cartValue = localStorage.getItem("kuazoo-cart");
    var cartObject = JSON.parse(cartValue);
    return cartObject;
}


function addToShipping(values) {
    if (localStorage.getItem("kuazoo-ship") == null) {
        localStorage.setItem("kuazoo-ship", JSON.stringify(values));
    }
    else {
        var cartValue = localStorage.getItem("kuazoo-ship");
        var cartObject = JSON.parse(cartValue);
        var item = cartObject;
        item.gender = values.gender;
        item.fn = values.fn;
        item.ln = values.ln;
        item.ph = values.ph;
        item.ad1 = values.ad1;
        item.ad2 = values.ad2;
        item.city = values.city;
        item.state = values.state;
        item.zip = values.zip;
        item.country = values.country;
        item.gift = values.gift;
        item.note = values.note;

        item.rcemail = values.rcemail;

        localStorage.setItem("kuazoo-ship", JSON.stringify(item));
    }
}
function getShipping() {
    if (localStorage.getItem("kuazoo-ship") != null) {
        var cartValue = localStorage.getItem("kuazoo-ship");
        var cartObject = JSON.parse(cartValue);
        return cartObject;
    }
    else {
        return "";
    }
}


function addToBilling(values) {
    if (localStorage.getItem("kuazoo-bill") == null) {
        localStorage.setItem("kuazoo-bill", JSON.stringify(values));
    }
    else {
        var cartValue = localStorage.getItem("kuazoo-bill");
        var cartObject = JSON.parse(cartValue);
        var item = cartObject;
        item.payment = values.payment;
        item.cc = values.cc;
        item.ccv = values.ccv;
        item.month = values.month;
        item.year = values.year;
        item.gender = values.gender;
        item.fn = values.fn;
        item.ln = values.ln;
        item.ph = values.ph;
        item.ad1 = values.ad1;
        item.ad2 = values.ad2;
        item.city = values.city;
        item.state = values.state;
        item.zip = values.zip;
        item.country = values.country;
        item.email = values.email;
        item.pass = values.pass;

        localStorage.setItem("kuazoo-bill", JSON.stringify(item));
    }
}
function getBilling() {
    if (localStorage.getItem("kuazoo-bill") != null) {
        var cartValue = localStorage.getItem("kuazoo-bill");
        var cartObject = JSON.parse(cartValue);
        return cartObject;
    }
    else {
        return "";
    }
}



function addToKPoint(values) {
    if (localStorage.getItem("kuazoo-kpoint") == null) {
        localStorage.setItem("kuazoo-kpoint", JSON.stringify(values));
    }
    else {
        var cartValue = localStorage.getItem("kuazoo-kpoint");
        var cartObject = JSON.parse(cartValue);
        var item = cartObject;
        item.point = values.point;
        localStorage.setItem("kuazoo-kpoint", JSON.stringify(item));
    }
}
function clearKPoint() {
    localStorage.removeItem('kuazoo-kpoint');
}
function getKPoint() {
    if (localStorage.getItem("kuazoo-kpoint") != null) {
        var cartValue = localStorage.getItem("kuazoo-kpoint");
        var cartObject = JSON.parse(cartValue);
        return cartObject;
    }
    else {
        return "";
    }
}


function clearPromotion() {
    localStorage.removeItem('kuazoo-promotion');
}
function addToPromotion(values) {
    if (localStorage.getItem("kuazoo-promotion") == null) {
        localStorage.setItem("kuazoo-promotion", JSON.stringify(values));
    }
    else {
        var cartValue = localStorage.getItem("kuazoo-promotion");
        var cartObject = JSON.parse(cartValue);
        var item = cartObject;
        item.id = values.id;
        item.code = values.code;
        item.type = values.type;
        item.typename = values.typename;
        item.value = values.value;
        localStorage.setItem("kuazoo-promotion", JSON.stringify(item));
    }
}
function getPromotion() {
    if (localStorage.getItem("kuazoo-promotion") != null) {
        var cartValue = localStorage.getItem("kuazoo-promotion");
        if (cartValue != "" && cartValue != "{}") {
            var cartObject = JSON.parse(cartValue);
            return cartObject;
        }
        else {
            return "";
        }
    }
    else {
        return "";
    }
}




function addToSubscribe(values) {
    if (localStorage.getItem("kuazoo-subscribe") == null) {
        localStorage.setItem("kuazoo-subscribe", JSON.stringify(values));
    }
    else {
        var cartValue = localStorage.getItem("kuazoo-subscribe");
        var cartObject = JSON.parse(cartValue);
        var item = cartObject;
        item.flag = values.flag;
        localStorage.setItem("kuazoo-subscribe", JSON.stringify(item));
    }
}
function clearSubscribe() {
    localStorage.removeItem('kuazoo-subscribe');
}
function getSubscribe() {
    if (localStorage.getItem("kuazoo-subscribe") != null) {
        var cartValue = localStorage.getItem("kuazoo-subscribe");
        var cartObject = JSON.parse(cartValue);
        return cartObject;
    }
    else {
        return "";
    }
}



function clearTransaction() {
    localStorage.removeItem('kuazoo-transaction');
}
function addToTransaction(values) {
    if (localStorage.getItem("kuazoo-transaction") == null) {
        localStorage.setItem("kuazoo-transaction", JSON.stringify(values));
    }
    else {
        var cartValue = localStorage.getItem("kuazoo-transaction");
        var cartObject = JSON.parse(cartValue);
        var item = cartObject;
        item.id = values.id;
        localStorage.setItem("kuazoo-transaction", JSON.stringify(item));
    }
}
function getTransaction() {
    if (localStorage.getItem("kuazoo-transaction") != null) {
        var cartValue = localStorage.getItem("kuazoo-transaction");
        if (cartValue != "" && cartValue != "{}") {
            var cartObject = JSON.parse(cartValue);
            return cartObject;
        }
        else {
            return "";
        }
    }
    else {
        return "";
    }
}

