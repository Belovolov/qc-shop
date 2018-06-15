$(document).ready(function () {
    

    $('.click-overlay').on('mouseenter', function () {
        $(this).fadeTo(200, 1)
    })

    $('.click-overlay').on('mouseleave', function () {
        $(this).fadeTo(200, 0)
    })

    $('.click-overlay').on('click', function () {
        var baseUrl = $('base').attr('href');
        window.location.href = baseUrl + $(this).attr('data-link')
    })
})