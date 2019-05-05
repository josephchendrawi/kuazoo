
function checkWishlist(id) {
    if (localStorage.getItem("kuazoo-Wishlist") != null) {
        var cartValue = localStorage.getItem("kuazoo-Wishlist");
        var cartObject = JSON.parse(cartValue);
        var cartCopy = cartObject;
        var items = cartCopy.items;
        var check = 0;
        for (var i = 0; i < items.length; i++) {
            if (items[i].id == id) {
                check = 1;
                break;
            }
        }
        if (check == 1) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return false;
    }
}
function addToWishlist(values) {
    if (localStorage.getItem("kuazoo-Wishlist") == null) {
        var cart = {};
        cart.items = [];
        localStorage.setItem("kuazoo-Wishlist", JSON.stringify(cart));
    }
    var cartValue = localStorage.getItem("kuazoo-Wishlist");
    var cartObject = JSON.parse(cartValue);
    var cartCopy = cartObject;
    var items = cartCopy.items;
    var check = 0;
    for (var i = 0; i < items.length; i++) {
        if (items[i].id == values.id) {
            check = 1;
            break;
        }
    }
    if (check == 0) {
        items.push(values);
        localStorage.setItem("kuazoo-Wishlist", JSON.stringify(cartCopy));
        return "true";
    }
    else {
        return "Item has been added to Wishlist.";
    }

}
function removeWishList(id) {
    if (localStorage.getItem("kuazoo-Wishlist") != null) {
        var cartValue = localStorage.getItem("kuazoo-Wishlist");
        var cartObject = JSON.parse(cartValue);
        var cartCopy = cartObject;
        var items = cartCopy.items;
        var check = -1;
        for (var i = 0; i < items.length; i++) {
            if (items[i].id == id) {
                check = i;
                break;
            }
        }
        if (check != -1) {
            items.splice(check, 1);
            localStorage.setItem("kuazoo-Wishlist", JSON.stringify(cartCopy));
            return "true";
        }
        return "";
    }
}
function clearWishlist() {
    localStorage.removeItem('kuazoo-Wishlist');
}
function getWishlist() {
    if (localStorage.getItem("kuazoo-Wishlist") != null) {
        var cartValue = localStorage.getItem("kuazoo-Wishlist");
        var cartObject = JSON.parse(cartValue);
        return cartObject;
    }
    else {
        return "";
    }
}