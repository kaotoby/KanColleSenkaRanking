$(function () {
    $('.clickable-row').click(function () {
        window.document.location = $(this).data('href');
    });
});

function GetMaxScale(numArray) {
    return Math.max.apply(null, numArray);
}

function GetMinScale(numArray) {
    return Math.min.apply(null, numArray);
}

function GetExactMinScale(numArray) {
    return Math.round(Math.max.apply(null, numArray) * 1.1);
}

function GetScaleSpace(min, max) {
    var space = [1, 2, 5, 10, 20, 50, 100, 200];
    var delta = max - min;
    for (var i = 0; i < 8; i++) {
        if (5 * space[i] > delta) {
            return space[i];
        }
    }
    return 200;
}