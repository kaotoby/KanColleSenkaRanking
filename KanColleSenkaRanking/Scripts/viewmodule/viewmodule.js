$(function () {
    var $sidebar = $('#sidebar').find('.list-group-item');
    var $affix = $('#affix');

    $(window).resize(function () {
        //Side Bar Adjust
        $affix.width($affix.parent().width());
        var padding = 10;
        do {
            $sidebar.css({ padding: padding + 'px 13px' });
            padding--;
        } while ($('#sidebar').offset().top + $affix.height() > $(window).height() && padding >= 2);
        if (padding == 1) {
            $sidebar.css({ padding: '3px 13px' });
            $affix.removeClass('affix');
        } else {
            $affix.addClass('affix');
        }
    });

    //Clickable Item
    $('.clickable').click(function (e) {
        e.stopPropagation();
        window.document.location = $(this).data('href');
    });

    //Side Bar Text Effect
    if ($.support.transition) {
        $sidebar.each(function (idx, elem) {
            var text = $(elem).text().replace(/ /g, '');
            $(elem).contents().filter(function () {
                return (this.nodeType == 3);
            }).remove();
            for (var i = 0; i < text.length; i++) {
                $(elem).append('<span class="trans">' + text[i] + '</span>');
            }
        });

        $sidebar.hover(function () {
            $(this).find('.trans').each(function (index, elem) {
                $(elem).transition({ rotateY: 90, delay: index * 50 }, 200, 'ease', function () {
                    $(elem).css({ rotateY: 270 })
                        .transition({ rotateY: 360 }, 200, 'ease', function () {
                            $(elem).css({ rotateY: 0 });
                        });
                });
            });
        }, function myfunction() {
            $(this).find('.trans').transitionStop().css({ rotateY: 0 });
        });
    }

    $(document).ready(function () {
        $(window).trigger('resize');
        $affix.hide();
        $affix.css({ visibility: 'visible' });
        $affix.slideDown(600).animate({ opacity: 1 }, { queue: false, duration: 400 });
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