$(function () {
    $(window).resize(function () {
        var width = $('#chartArea').width();
        var height = width * 1.2 / 2;
        for (var i = 0; i < ServerCharts.length; i++) {
            ServerCharts[i](width, height);
        }
    });

    var buttonText = $('#bigButton').text();
    $('#bigButton').hover(function () {
        var self = $(this).find('span');
        $(self).stop().fadeOut(200, function () {
            $(self).text(lastUpdateTime);
            $(self).fadeIn(200);
        })
    }, function () {
        var self = $(this).find('span');
        $(self).stop().fadeOut(200, function () {
            $(self).text(buttonText);
            $(self).fadeIn(200);
        })
    });

    $(window).trigger('resize');
});