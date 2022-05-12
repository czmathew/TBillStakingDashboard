$(document).ready(function () {
    calculate();

    $("#calculate").click(function (event) {
        event.preventDefault();
        calculate();
    });
});



function calculate() {
    var firstInvest = parseFloat($("#firstInvestment").val());
    var monthlyInvest = parseFloat($("#monthlyInvestment").val());
    var multi2x = parseFloat($("#2xMulti").val());
    var tfuelPrice = parseFloat($("#tfuelPrice").val());
    var tbillPrice = parseFloat($("#tbillPrice").val());
    var compound = parseFloat($("#compound").val());

    if (isNaN(monthlyInvest)) {
        monthlyInvest = 0;
    }
    if (isNaN(firstInvest)) {
        firstInvest = 0;
    }
    if (isNaN(multi2x)) {
        multi2x = 0;
    }
    if (tfuelPrice == "" || tfuelPrice == "" | isNaN(tfuelPrice) || isNaN(tfuelPrice)) {
        return;
    }

    //alert(compound);

    var years = 10;
    var yearsInLP = [];

    for (let i = 0; i < years; i++) {
        if (i == 0) {
            var calcTbill = firstInvest / 2 / tbillPrice;
            var calcTfuel = firstInvest / 2 / tfuelPrice;
            var usedMulti2x = (multi2x > calcTbill) ? 1 : (1 - ((calcTbill - multi2x) / calcTbill));
            var apr = (0.75 * usedMulti2x) + (0.5 * (1 - usedMulti2x));
            var apy = Math.pow((1 + (apr / compound)), (compound)) - 1;
            let year = new yearInLP(firstInvest, 0, calcTfuel, calcTbill, usedMulti2x, apr, apy);
            yearsInLP.push(year);
        } else {
            var prevYear = yearsInLP[i - 1];
            var x = prevYear.apy;
            var val = (prevYear.val + (12 * monthlyInvest)) * (1 + prevYear.apy);
            var netGain = val - prevYear.val;
            var calcTbill = val / 2 / tbillPrice;
            var calcTfuel = val / 2 / tfuelPrice;
            var usedMulti2x = (multi2x > calcTbill) ? 1 : (1 - ((calcTbill - multi2x) / calcTbill));
            var apr = (0.75 * usedMulti2x) + (0.5 * (1 - usedMulti2x));
            var apy = Math.pow((1 + (apr / compound)), (compound)) - 1;
            let year = new yearInLP(val, netGain, calcTfuel, calcTbill, usedMulti2x, apr, apy);
            yearsInLP.push(year);
        }
    }
    var yearsArray = [];
    $('#calcTable > tbody:last-child').empty();
    $.each(yearsInLP, function (i, val) {

        var valu = (val.val).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var netGain = (val.netGain).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var calcTfuel = (val.calcTfuel).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var calcTbill = (val.calcTbill).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var multi2x = (val.usedMulti2x * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var apr = (val.apr * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var apy = (val.apy*100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        $('#calcTable > tbody:last-child').append('<tr><td>' + i + '</td><td class="text-end">$' + valu + '</td><td class="text-end">$' + netGain + '</td><td class="text-end">' + calcTfuel + '</td><td class="text-end">' + calcTbill + '</td><td class="text-end">' + multi2x + '%</td><td class="text-end">' + apr + '%</td><td class="text-end">' + apy + '%</td></tr>');
        var innerArr = [i, val.val];
        yearsArray.push(innerArr);
    });


    showCalcChart(yearsArray);

      
   

}

function showCalcChart(data) {

    var options = {
        series: [{
            name: 'Daily',
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
        //title: {
        //    text: 'Value',
        //    align: 'left'
        //},
        stroke: {
            width: [3]
        },
        
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val / 1).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });;
                },
            },
            title: {
                text: 'Value'
            },
        },
        xaxis: {
        },
        tooltip: {
            enabled: true,
            shared: false,
            y: {
                formatter: function (val) {
                    return (val / 1).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                }
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartCalc"), options);
    $("#chartCalc").empty();
    chart.render();
}

class yearInLP {
    constructor(val, netGain, calcTfuel, calcTbill, usedMulti2x, apr, apy) {
        this.val = val;
        this.netGain = netGain;
        this.calcTfuel = calcTfuel;
        this.calcTbill = calcTbill;
        this.usedMulti2x = usedMulti2x;
        this.apr = apr;
        this.apy = apy;
    }
}