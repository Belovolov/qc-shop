$('.click-overlay').on('mouseenter', function () {
    $(this).fadeTo(200, 1)
})

$('.click-overlay').on('mouseleave', function () {
    $(this).fadeTo(200, 0)
})

$('.click-overlay').on('click', function () {
    window.location.href = $(this).attr('data-link')
})