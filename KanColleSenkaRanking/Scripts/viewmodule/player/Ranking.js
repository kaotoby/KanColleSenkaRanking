// -----------------------------
// Player Ranking Chart
// -----------------------------

PlayerCharts.push(function (width, height) {
    var max = GetMaxScale(rankingData[0].value);
    var min = GetMinScale(rankingData[0].value);
    var scale = GetScaleSpace(min, max);
    var from0 = min == 1;
    max += scale;
    min -= scale
    var extMax = Math.floor(max / scale) * scale;
    var extMin = Math.ceil(min / scale) * scale;
    var newMax = 2 * extMax - extMin;
    if (from0) {
        extMax += scale;
        newMax += scale;
    }
    var chartdata = [{
        color: '#1F1E11',
        line_width: 2.5,
        value: []
    }];
    for (var i = 0; i < rankingData[0].value.length; i++) {
        chartdata[0].value.push(2 * extMax - rankingData[0].value[i]);
    }
    var chart = new iChart.LineBasic2D({
        render: "rankingChart",
        data: chartdata,
        align: 'center',
        title: {
            text: '順位履歴',
            font: 'HGrgm',
            fontsize: 24,
            color: '#505050',
            offsety: -10
        },
        border: false,
        width: width,
        height: height * 1.2,
        padding: '50px',
        shadow: true,
        shadow_color: '#B5A262',
        shadow_blur: 4,
        shadow_offsetx: 0,
        shadow_offsety: 0,
        background_color: false,
        animation: true,
        animation_duration: 600,
        tip: {
            enable: true,
            shadow: true,
            listeners: {
                //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                parseText: function (tip, name, value, text, i) {
                    var time;
                    if (rankLable[i] == '') {
                        if (i == 0) {
                            time = (parseInt(rankLable[i + 1]) - 1) + "日 15時"
                        } else {
                            time = parseInt(rankLable[i - 1]) + "日 15時"
                        }
                    } else {
                        time = rankLable[i] + "日 3時"
                    }
                    var val = 2 * extMax - value;
                    return '<span>' + time + '</span>' +
                    '<br/><span class="cl-brown font-bold h4 text-center">' + val + '位</span>';
                }
            }
        },
        crosshair: {
            enable: false
        },
        sub_option: {
            smooth: true,
            label: false,
            hollow_color: '#B5A262',
            point_size: 10
        },
        listeners: {
            /**
            * d:相当于data[0],即是一个线段的对象
            * v:相当于data[0].value
            * x:计算出来的横坐标
            * x:计算出来的纵坐标
            * j:序号 从0开始
            */
            parsePoint: function (d, v, x, y, j) {
                var val = 2 * extMax - v;
                val = Math.round(val * 10) / 10;
                if (val == min + scale + 0.1)
                    return { ignored: true };
            }
        },
        coordinate: {
            width: 640,
            height: 260,
            striped_factor: 0.18,
            grid_color: '#C0C0C0',
            axis: {
                color: '#808080',
                width: [0, 0, 2, 2]
            },
            scale: [{
                position: 'left',
                start_scale: extMax,
                end_scale: newMax,
                scale_space: -scale,
                scale_enable: false,
                label: { color: '#505050', fontsize: 13, fontweight: 600 },
                listeners: {
                    parseText: function (text, originx, originy, index, last) {
                        return { text: text == '0' ? '1位' : text + '位' }
                    }
                }
            }, {
                position: 'bottom',
                label: { color: '#505050', fontsize: 13, fontweight: 600 },
                scale_enable: false,
                labels: rankLable
            }]
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
            //在左上侧的位置，渲染一个单位的文字
            chart.target.textAlign('start')
            .textBaseline('bottom')
            .textFont('600 14px HGrgm')
            .fillText('順位', x - 40, y - 12, false, '#B5A262')
            .textBaseline('top')
            .fillText('(日)', x + w + 10, y + h + 6, false, '#B5A262');
        }
    }));
    chart.draw();
});