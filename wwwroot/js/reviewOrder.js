function reviewOrder(orderId, productId, rating)
{
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Order/Review");
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status != 200) return;

            let data = JSON.parse(this.responseText);
            if (data.success == true) {
                let allStars = document.getElementById(`${data.orderId}-${data.productId}`);
                let stars = allStars.querySelectorAll('.star');
                stars.forEach(star => {
                    star.classList.remove('checked');
                })
                let reviewedProduct = document.getElementById(`${data.orderId}-${data.productId}-${data.rating}`)
                reviewedProduct.classList.add('checked');
            }
        }
    };

    let data = {
        "OrderId": orderId,
        "ProductId": productId,
        "ReviewRating": rating
    };

    xhr.send(JSON.stringify(data));
}