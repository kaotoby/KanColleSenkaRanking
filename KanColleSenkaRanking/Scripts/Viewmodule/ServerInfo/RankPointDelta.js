// --------------------------------------
// Server Rank Point Delta Average Chart
// --------------------------------------

ServerCharts.push(function (width, height) {
    var mixdata = [];
    for (var i = 0; i < rankDeltaData[0].value.length; i++) {
        mixdata.push(rankDeltaData[0].value[i] + rankDeltaData[1].value[i]);
    }
    var max = GetMaxScale(mixdata);
    var scale = GetScaleSpace(0, max);
    max += scale;
    var extMax = Math.floor(max / scale) * scale;
    rankDeltaData[0].color = '#AA985A';
    rankDeltaData[1].color = '#4A432A';

    var chart = new iChart.BarStacked2D({
        render: 'rankPointDeltaAvgChart',
        data: rankDeltaData,
        labels: rankDeltaLable,
        title: {
            text: '日平均戦果増分値',
            font: 'HGrgm',
            fontsize: 24,
            color: '#505050',
            offsety: -15
        },
        padding: '50px',
        width: width,
        height: height * 1.5,
        border: false,
        shadow: true,
        default_mouseover_css: false,
        background_color: false,
        footnote: {
            text: FootNote,
            font: 'HGrgm',
            color: '#505050',
            offsety: 15
        },
        shadow_blur: 3,
        shadow_color: '#1F1E11',
        shadow_offsetx: 1,
        shadow_offsety: 0,
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
            offsetx: 5000 / width,
            offsety: -3000 / height,
            border: false,
            font: 'HGrgm',
            fontsize: 13
        },
        coordinate: {
            grid_color: '#909090',
            axis: {
                width: 0
            },
            scale: [{
                position: 'bottom',
                scale_enable: false,
                end_scale: extMax,
                scale_space: scale
            }],
            width: 600,
            height: 260
        }
    });
    //利用自定义组件构造左侧说明文本
    chart.plugin(new iChart.Custom({
        drawFn: function () {
            //计算位置
            var coo = chart.getCoordinate(),
            x = coo.get('originx'),
            y = coo.get('originy'),
            w = coo.width,
            h = coo.height;
            //在左下侧的位置，渲染一个单位的文字
            chart.target
                .textAlign('start')
                .textBaseline('bottom')
                .textFont('600 14px HGrgm')
                .fillText('順位', x - 40, y - 12, false, '#B5A262')
                .textBaseline('top')
                .fillText('戦果', x + w + 20, y + h + 5, false, '#B5A262');
        }
    }));
    chart.draw();
});