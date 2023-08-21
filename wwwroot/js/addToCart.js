function addToCart(productId) {
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/ShoppingCart/AddItem");
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status != 200) return;

            let data = JSON.parse(this.responseText);
            if (data.success == true) {
                document.getElementById("cartQty").innerHTML = data.totalQty || 0;
            }
        }
    };

    let data = {
        "ProductId": productId,
    };

    xhr.send(JSON.stringify(data));
}
