$(document).ready(function () {
    setTimeout(function () {
        var deliveryMsg = document.createElement('div')
        $(deliveryMsg).text('Free delivery for orders above 50$')
        $(deliveryMsg).addClass('delivery-message')

        var close = document.createElement('span')
        $(close).html('&#10761')
        $(close).addClass('my-times')

        $(close).on('click', () => {
            console.log('close click')
            $(deliveryMsg).slideUp(500)
        })

        $(deliveryMsg).append(close)
        $('body').prepend(deliveryMsg)
        $(deliveryMsg).slideDown(500)
    }, 2000)
})