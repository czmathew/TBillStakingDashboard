$(document).ready(function () {
    refreshCharts()
});

function refreshCharts() {

    $.getJSON("api/getDailyRates/", function (data) {
        var ratioDaily = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            var innerArr = [val["Item1"], val["Item4"]];
            ratioDaily.push(innerArr);
        });

        showDailyRateChart(ratioDaily);
        
    });

    $.getJSON("api/getDailyLPToken/", function (data) {
        var ratioDaily = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            var innerArr = [val["Item1"], val["Item2"]];
            ratioDaily.push(innerArr);
        });

        showDailyLPTokenChart(ratioDaily);

    });
}

function showDailyLPTokenChart(data) {

    var options = {
        series: [{
            name: 'LP Token',
            data: data
        }],
        
        chart: {
            type: 'line',
            height: 350
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: 'LP Token',
            align: 'left'
        },
        yaxis: {
            labels: {
                formatter: function (val) {
                    return val;
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
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartDailyLPToken"), options);
    $("#chartDailyLPToken").empty();
    chart.render();
}


function showDailyRateChart(data) {

    var options = {
        series: [{
            name: 'Tbill / Tfuel ratio',
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
            height: 350
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: 'Tbill / Tfuel ratio',
            align: 'left'
        },
        yaxis: {
            labels: {
                formatter: function (val) {
                    return val;
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
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartDailyRates"), options);
    $("#chartDailyRates").empty();
    chart.render();
}
