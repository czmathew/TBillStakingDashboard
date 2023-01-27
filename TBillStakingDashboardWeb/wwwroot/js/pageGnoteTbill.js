$(document).ready(function () {
    refreshChartsGnote()
});

function refreshChartsGnote() {

    $.getJSON("api/getGnoteTbillPrice/", function (data) {
        var dailyPrice = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            var innerArr = [val["Item1"], val["Item2"]];
            dailyPrice.push(innerArr);
        });

        showDailyGnoteTbillPrice(dailyPrice);
        
    });

    $.getJSON("api/getDailyRatesGnote/360", function (data) {
        var dailyGnoteUSDPrice = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            if (val["Item2"] != '0') {
                var innerArr = [val["Item1"], val["Item2"]];
            }
            dailyGnoteUSDPrice.push(innerArr);
        });

        showDailyGnoteUSDPrice(dailyGnoteUSDPrice);

    });

   
}

function showDailyGnoteUSDPrice(data) {

    var options = {
        series: [{
            name: 'gNOTE / USD',
            data: data
        }],
        //chart: {
        //    type: 'area',
        //    stacked: false,
        //    height: 350,
        //    zoom: {
        //        type: 'x',
        //        enabled: true,
        //        autoScaleYaxis: true
        //    },
        //    toolbar: {
        //        autoSelected: 'zoom'
        //    }
        //},
        chart: {
            type: 'line',
            height: 350,
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            }
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: 'gNOTE / USD',
            align: 'left'
        },
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val).toFixed(3);
                },
            },
            //title: {
            //    text: 'TBILL'
            //},
        },
        xaxis: {
            type: 'datetime',
        },
        stroke: {
            width: [3]
        },
        tooltip: {
            enabled: true,
            shared: false,
            y: {
                formatter: function (val) {
                    return (val).toFixed(4)
                }
            },
            x: {
                format: 'dd/MM/yy HH:mm'
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartGnoteUSDPrice"), options);
    $("#chartGnoteUSDPrice").empty();
    chart.render();
}

function showDailyGnoteTbillPrice(data) {

    var options = {
        series: [{
            name: 'gNOTE / TBill',
            data: data
        }],
        //chart: {
        //    type: 'area',
        //    stacked: false,
        //    height: 350,
        //    zoom: {
        //        type: 'x',
        //        enabled: true,
        //        autoScaleYaxis: true
        //    },
        //    toolbar: {
        //        autoSelected: 'zoom'
        //    }
        //},
        chart: {
            type: 'line',
            height: 350,
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            }
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: 'gNOTE / TBill',
            align: 'left'
        },
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val).toFixed(3);
                },
            },
            //title: {
            //    text: 'TBILL'
            //},
        },
        xaxis: {
            type: 'datetime',
        },
        stroke: {
            width: [3]
        },
        tooltip: {
            enabled: true,
            shared: false,
            y: {
                formatter: function (val) {
                    return (val).toFixed(4)
                }
            },
            x: {
                format: 'dd/MM/yy HH:mm'
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartGnoteTbillPrice"), options);
    $("#chartGnoteTbillPrice").empty();
    chart.render();
}
