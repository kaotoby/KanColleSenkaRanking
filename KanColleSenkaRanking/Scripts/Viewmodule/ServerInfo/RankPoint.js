// -----------------------------
// Server Rank Point Chart
// -----------------------------

ServerCharts.push(function (width, height) {
    var startColor = 10;
    rankPointData[0].color = '#B5A262';
    rankPointData[1].color = '#808080';
    rankPointData[2].color = '#606060';
    rankPointData[3].color = '#404040';
    rankPointData[4].color = '#202020';
    var data = rankPointData.slice(0);

    var chart = new iChart.Area2D({
        render: 'rankPointChart',
        data: data,
        title: {
            text: '戦果値',
            font: 'HGrgm',
            fontsize: 24,
            color: '#505050',
            offsety: -10
        },
        width: width,
        height: height,
        padding: '20px',
        border: false,
        background_color: false,
        footnote: {
            text: FootNote,
            font: 'HGrgm',
            color: '#505050',
            offsety: 10
        },
        legend: {
            enable: true,
            row: 1,//设置在一行上显示，与column配合使用
            column : 'max',
            valign:'top',
            background_color: false,
            offsetx: -3000 / width,
            offsety: -3000 / height,
            border : false,
            font: 'HGrgm',
            fontsize: 13
        },
        tip: {
            enable: true,
            shadow: true,
            listeners: {
                //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                parseText: function (tip, name, value, text, i) {
                    var prefix;
                    if (name == '1位') {
                        prefix = '<span class="h5 cl-brown">' + topPlayerName[i];
                    } else {
                        prefix = '<span class="h5">' + name;
                    }
                    return prefix + ' <span class="font-bold">' + value + '</span></span>';
                }
            }
        },
        tipMocker: function (tips, idx) {
            //tips:线段的提示文本的数组、i:数据点的索引
            var time;
            var tipString = '';
            if (rankPointLable[idx] == '') {
                if (idx == 0) {
                    time = (parseInt(rankPointLable[idx + 1]) - 1) + "日 15時"
                } else {
                    time = rankPointLable[idx - 1] + "日 15時"
                }
            } else {
                time = rankPointLable[idx] + "日 3時"
            }
            tipString = '<span>' + time + '</span>';
            for (var i = 0; i < tips.length; i++) {
                tipString += '<br/>' + tips[i];
            }

            return '<div class="text-right">' + tipString + '</div>';
        },
        crosshair: {
            enable: true,
            line_color: '#505050',
            line_width: 1
        },
        sub_option: {
            label: false,
            point_size: 7
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
                if ((v * 10) % 10 != 0)
                    return { ignored: true };
            }
        },
        coordinate: {
            axis: {
                width: [0, 0, 2, 1]
            },
            grid_color: '#C0C0C0',
            scale: [{
                position: 'left',
                scale_enable: false,//禁用小横线
                label: { color: '#505050', fontsize: 13 }
            }, {
                position: 'bottom',
                start_scale: 1,
                labels: rankPointLable,
                label: { color: '#505050', fontweight: 600 }
            }]
        }
    });
    //利用自定义组件构造左上侧单位
    chart.plugin(new iChart.Custom({
        drawFn: function () {
            //计算位置
            var coo = chart.getCoordinate(),
            x = coo.get('originx'),
            y = coo.get('originy');
            w = coo.width,
            h = coo.height;
            //在左上侧的位置，渲染一个单位的文字
            chart.target.textAlign('start')
            .textBaseline('bottom')
            .textFont('600 14px HGrgm')
            .fillText('戦果', x - 40, y - 12, false, '#B5A262')
            .textBaseline('top')
            .fillText('(日)', x + w + 10, y + h + 6, false, '#B5A262');
        }
    }));
    chart.draw();
});