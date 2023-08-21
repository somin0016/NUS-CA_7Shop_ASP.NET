function updateCartQuantity(productId) {
    let quantityInput = document.getElementById('quantityInput-' + productId);
    let newQuantity = parseInt(quantityInput.value);
    let price = parseFloat(quantityInput.dataset.price);
    let itemTotal = newQuantity * price;

    fetch('/ShoppingCart/CartUpdate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            productId: productId,
            quantity: newQuantity
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success == true) {
                let cartItems = document.querySelectorAll('.cart-item-detail');
                let cartTotal = 0;
                let cartQty = 0;

                cartItems.forEach(function (item) {
                    let itemPrice = parseFloat(item.querySelector('.item-price').innerHTML.replace('$ ', ''));
                    let itemQuantity = parseInt(item.querySelector('.quantity-input').value);
                    let itemTotalPrice = itemPrice * itemQuantity;
                    cartTotal += itemTotalPrice;
                    cartQty += itemQuantity;
                });

                document.getElementById('cartTotal').innerHTML = '$ ' + cartTotal.toFixed(2);
                document.getElementById('cartQty').innerHTML = cartQty;
            }
        })
        .catch(error => console.error(error));
}