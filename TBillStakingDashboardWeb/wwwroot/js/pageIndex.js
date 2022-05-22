$(document).ready(function () {
    refreshCharts();

    $('input[type=radio][name=optionsPeriod]').change(function () {
        refreshTbillPriceChart(this.value);

        refreshTVLandRewards(this.value);

        refreshDailyLPToken(this.value);

        refreshRebaseChart(this.value);

        refreshDailyRates(this.value);
    });
});



function refreshCharts() {

    refreshDailyRates(31);

    refreshDailyLPToken(31);

    refreshTbillPriceChart(31);

    refreshTVLandRewards(31);
    refreshRebaseChart(31);



}

function refreshDailyLPToken(days) {
    $.getJSON("api/getDailyLPToken/"+days, function (data) {
        var ratioDaily = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            var innerArr = [val["Item1"], val["Item2"]];
            ratioDaily.push(innerArr);
        });

        showDailyLPTokenChart(ratioDaily);

    });

}

function refreshDailyRates(days) {
    $.getJSON("api/getDailyRates/" + days, function (data) {
        var ratioDaily = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            var innerArr = [val["Item1"], val["Item4"]];
            ratioDaily.push(innerArr);
        });

        showDailyRateChart(ratioDaily);

    });
}

function refreshRebaseChart(days) {
    var datesRebases = [];
    var datesTbillSupply = [];
    var TbillSupplyMin = null;
    var TbillSupplyMax = 0;
    //var sourcearrayRebase = @Html.Raw(Json.Serialize(Model.RebaseList));
    $.getJSON("api/getRebaseStats/" + days, function (data) {
        $.each(data, function (key, value) {
            var innerArr = [value["Item1"], value["Item4"]];
            datesRebases.push(innerArr);
            var innerArrSupply = [value["Item1"], value["Item2"]];
            datesTbillSupply.push(innerArrSupply);
            if (!TbillSupplyMin || parseFloat(value["Item2"]) < parseFloat(TbillSupplyMin)) {
                TbillSupplyMin = parseFloat(value["Item2"]);
            }
            if (parseFloat(value["Item2"]) > parseFloat(TbillSupplyMax)) {
                TbillSupplyMax = parseFloat(value["Item2"]);
            }
        });
        
    

        var optionsRebase = {
            series: [{
                name: 'Rebase %',
                type: 'column',
                data: datesRebases
            }, {
                name: 'TBILL supply',
                type: 'line',
                data: datesTbillSupply
            }],
            chart: {
                height: 350,
                type: 'line',
            },
            stroke: {
                width: [0, 4]
            },
            title: {
                text: 'Rebase / TBILL Supply'
            },
            xaxis: {
                type: 'datetime'
            },
            yaxis: [{
                title: {
                    text: 'Rebase %',
                }, labels: {
                    formatter: function (val) {
                        /*    return (val / 1).toFixed(2);*/
                        return val.toFixed(2);
                    }
                },
                min: -20,
                max: 20

            }, {
                opposite: true,
                title: {
                    text: 'TBILL supply'
                }, labels: {
                    formatter: function (val) {
                        return (val / 1000000).toFixed(2) + ' M';
                    }
                },
                min: TbillSupplyMin * 0.98,
                max: TbillSupplyMax * 1.02
            }],
            plotOptions: {
                bar: {
                    colors: {
                        ranges: [{
                            from: -100,
                            to: 0,
                            color: '#dc3545'
                        }, {
                            from: 0,
                            to: 100,
                            color: '#28a745'
                        }]
                    },
                    columnWidth: '80%',
                }
            },
            theme: {
                mode: 'dark'
            },
            colors: ['#28a745', '#4ECDC4']
        };

    
        var chartRebase = new ApexCharts(document.querySelector("#chartRebase"), optionsRebase);
        $("#chartRebase").empty();
        chartRebase.render();

    });

}

function refreshTbillPriceChart(days) {
    $.getJSON("api/getTbillPrice/"+days, function (data) {
        var tbill = [];
        var target = [];
        var rebase = [];
        $.each(data, function (key, val) {
            //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
            var innerArr = [val["Item1"], val["Item2"]];
            tbill.push(innerArr);
            var innerArr = [val["Item1"], val["Item3"]];
            target.push(innerArr);
            var innerArr = [val["Item1"], val["Item4"]];
            rebase.push(innerArr);
        });

        showDailyTbillChart(tbill, target, rebase);
    });
}

function refreshTVLandRewards(days) {
    var datesTvl = [];
    var datesTF = [];
    var datesTB = [];
    var datesRewards = [];

    var Httpreq = new XMLHttpRequest(); 
    Httpreq.open("GET", "api/getDailyTBillStats/"+days, false);
    Httpreq.send(null);
    Httpreq.responseText;

    var json_obj = JSON.parse(Httpreq.responseText);

    json_obj.forEach((el) => {
        var innerArr = [el["Date"], el["TvLocked"]];
        datesTvl.push(innerArr);
        var innerArr2 = [el["Date"], el["TfuelLocked"]];
        datesTF.push(innerArr2);
        var innerArr3 = [el["Date"], el["TbillLocked"]];
        datesTB.push(innerArr3);
        var innerArr4 = [el["Date"], el["Rewards"]];
        datesRewards.push(innerArr4);
    });

    var options = {
        series: [{
            name: 'TVL',
            data: datesTvl
        }, {
            name: 'TFuel',
            data: datesTF
        }, {
            name: 'TBill',
            data: datesTB
        }],
        chart: {
            type: 'line',
            stacked: false,
            height: 350,
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            },
            toolbar: {
                autoSelected: 'zoom'
            }
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        stroke: {
            width: [3,3,3]
        },
        title: {
            text: 'Total Value Locked',
            align: 'left'
        },
        //fill: {
        //    type: 'gradient',
        //    gradient: {
        //        shadeIntensity: 1,
        //        inverseColors: false,
        //        opacityFrom: 0.5,
        //        opacityTo: 0,
        //        stops: [0, 90, 100]
        //    },
        //},
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val / 1000000).toFixed(0) + ' M';
                },
            },
            title: {
                text: 'Value'
            },
        },
        xaxis: {
            type: 'datetime',
        },
        tooltip: {
            enabled: true,
            shared: true,
            y: {
                formatter: function (val) {
                    return (val / 1000000).toFixed(2) + ' M'
                }
            },
            x: {
                format: 'dd/MM/yy HH:mm'
            }
        },
        theme: {
            mode: 'dark'
        },
        colors: ['#85bb65', '#e75c10', '#1d2c3b']
    };

    var optionsRewards = {
        series: [{
            name: 'Rewards',
            data: datesRewards
        }],
        chart: {
            type: 'area',
            stacked: false,
            height: 350,
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            },
            toolbar: {
                autoSelected: 'zoom'
            }
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: 'Rewards (USD)',
            align: 'left'
        },
        //fill: {
        //    type: 'gradient',
        //    gradient: {
        //        shadeIntensity: 1,
        //        inverseColors: false,
        //        opacityFrom: 0.5,
        //        opacityTo: 0,
        //        stops: [0, 90, 100]
        //    },
        //},
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val / 1).toFixed(0);
                },
            },
            title: {
                text: 'Value'
            },
        },
        xaxis: {
            type: 'datetime',
        },
        tooltip: {
            enabled: true,
            shared: false,
            y: {
                formatter: function (val) {
                    return (val / 1).toFixed(0)
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

    var chart = new ApexCharts(document.querySelector("#chartTVL"), options);
    $("#chartTVL").empty();
    chart.render();

    var chartRewards = new ApexCharts(document.querySelector("#chartRewards"), optionsRewards);
    $("#chartRewards").empty();
    chartRewards.render();
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
            },
            x: {
                format: 'dd/MM/yy HH:mm'
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
            },
            x: {
                format: 'dd/MM/yy HH:mm'
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

function showDailyTbillChart(tbill, target, rebase) {

    var options = {
        series: [
            {
                name: 'Tbill price',
                data: tbill
            },
            {
                name: '24hr oracle',
                data: rebase
            },
            {
                name: 'Target price',
                data: target
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
            text: 'Tbill price',
            align: 'left'
        },
        tickAmount: 6,
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val).toFixed(4)
                },
            },
            axisTicks: {
                show: true,
                borderType: 'solid',
                color: '#78909C',
                width: 6,
                offsetX: 0,
                offsetY: 0
            }
            //title: {
            //    text: 'TBILL'
            //},
        },
        xaxis: {
            type: 'datetime',
        },
        stroke: {
            width: [3, 3, 3]
        },
        tooltip: {
            enabled: true,
            shared: true,
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
        },
        colors: ['#4ECDC4', '#FEB019', '#4CAF50']
        
    };

    var chart = new ApexCharts(document.querySelector("#chartTbillPrice"), options);
    $("#chartTbillPrice").empty();
    chart.render();
}
