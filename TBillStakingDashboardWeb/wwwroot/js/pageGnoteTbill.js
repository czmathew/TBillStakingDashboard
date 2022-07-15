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
            height: 350
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
