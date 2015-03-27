$(function () {
    $(window).resize(function () {
        var width = $('#chartArea').width();
        var height = width * 1.2 / 2;
        for (var i = 0; i < PlayerCharts.length; i++) {
            PlayerCharts[i](width, height);
        }
    });
    $(window).trigger('resize');
});