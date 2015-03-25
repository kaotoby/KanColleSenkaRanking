// -----------------------------
// Player Rank Point Delta Chart
// -----------------------------

PlayerCharts.push(function (width, height) {
    var mixdata = [];
    var avg = 0.0;
    var count = 0;
    for (var i = 0; i < rankPointDeltaData[0].value.length; i++) {
        var sum = 0;
        for (var a = 0; a < 2; a++) {
            var v = rankPointDeltaData[a].value[i];
            if ((v * 10) % 10 != 0) {
                rankPointDeltaData[a].value[i] = 0;
            } else {
                sum += v;
            }
        }
        avg += sum;
        count++;
        mixdata.push(sum)
    }
    if (rankPointDeltaData[0].value.length == 0) {
        avg = 0;
    } else {
        avg = avg / count;
        avg = Math.round(avg * 10) / 10;
    }

    var max = GetMaxScale(mixdata);
    var scale = GetScaleSpace(0, max);
    max += scale;
    var extMax = Math.floor(max / scale) * scale;
    rankPointDeltaData[0].color = '#AA985A';
    rankPointDeltaData[1].color = '#4A432A';
    var chart = new iChart.ColumnStacked2D({
        render: 'rankPointDeltaChart',
        data: rankPointDeltaData,
        labels: rankPointDeltaLable,
        title: {
            text: '戦果増分値',
            font: 'HGrgm',
            fontsize: 24,
            color: '#505050'
        },
        border: false,
        shadow: true,
        footnote: {
            text: FootNote,
            font: 'HGrgm',
            color: '#505050',
        },
        shadow_blur: 3,
        shadow_color: '#1F1E11',
        shadow_offsetx: 1,
        shadow_offsety: 0,
        width: width,
        height: height,
        default_mouseover_css: false,
        background_color: false,
        sub_option: {
            label: { color: '#f9f9f9', fontsize: 10, fontweight: 600 },
            border: {
                radius: '3 3 3 3'
            }
        },
        label: { color: '#505050', fontsize: 13, fontweight: 600 },
        legend: {
            enable: true,
            row: 1,//设置在一行上显示，与column配合使用
            column: 'max',
            valign: 'top',
            background_color: false,
            offsetx: -6000 / width,
            offsety: -2000 / height,
            border: false,
            font: 'HGrgm',
            fontsize: 13
        },
        column_width: 80,
        coordinate: {
            grid_color: '#909090',
            axis: {
                width: 0
            },
            scale: [{
                position: 'left',
                scale_enable: false,
                end_scale: extMax,
                scale_space: scale,
                label: { color: '#505050', fontsize: 13, fontweight: 600 }
            }],
            width: '80%',
            height: '76%'
        }
    });
    //利用自定义组件构造左上侧单位
    chart.plugin(new iChart.Custom({
        drawFn: function () {
            //计算位置
            var coo = chart.getCoordinate(),
            x = coo.get('originx'),
            y = coo.get('originy'),
            w = coo.width,
            h = coo.height,
            S = coo.getScale('left'),
            H = (avg - S.start) * h / S.distance,
            Y = chart.y + h - H;
            //在左上侧的位置，渲染一个单位的文字
            chart.target
                .textAlign('start')
                .textBaseline('bottom')
                .textFont('600 14px HGrgm')
                .fillText('戦果', x - 40, y - 12, false, '#B5A262')
                .textBaseline('top')
                .fillText('日', x + w + 5, y + h + 5, false, '#B5A262');
            if (avg != 0) {
                chart.target
                    .globalAlpha(0.2)
                    .box(x, Y, w, H, 0, '#505050')
                    .globalAlpha(1)
                    .textAlign('start')
                    .textBaseline('middle')
                    .textFont('600 12px Verdana')
                    .fillText('日平均', x + w, Y - 15, false, '#B5A262')
                    .fillText(avg, x + w, Y, false, '#B5A262');
            }
        }
    }));
    chart.draw();
});