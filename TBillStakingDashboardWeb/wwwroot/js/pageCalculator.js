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
    var multi125x = parseFloat($("#125xMulti").val());
    var multi15x = parseFloat($("#15xMulti").val());
    var multi2x = parseFloat($("#2xMulti").val());
    var multi3x = parseFloat($("#3xMulti").val());
    var multi4x = parseFloat($("#4xMulti").val());
    var tfuelPrice = parseFloat($("#tfuelPrice").val());
    var tbillPrice = parseFloat($("#tbillPrice").val());
    var compound = parseFloat($("#compound").val());

    if (isNaN(monthlyInvest)) {
        monthlyInvest = 0;
    }
    if (isNaN(firstInvest)) {
        firstInvest = 0;
    }
    if (isNaN(multi4x)) {
        multi4x = 0;
    }
    if (isNaN(multi3x)) {
        multi3x = 0;
    }
    if (isNaN(multi2x)) {
        multi2x = 0;
    }
    if (isNaN(multi15x)) {
        multi15x = 0;
    }
    if (isNaN(multi125x)) {
        multi125x = 0;
    }
    if (tfuelPrice == "" || tfuelPrice == "" | isNaN(tfuelPrice) || isNaN(tfuelPrice)) {
        return;
    }

    //alert(compound);

    var years = 10;
    var yearsInLP = [];

    for (let i = 0; i <= years; i++) {
        if (i == 0) {
            var calcTbill = firstInvest / 2 / tbillPrice;
            var calcTfuel = firstInvest / 2 / tfuelPrice;
            var amountMulti4x = (calcTbill > multi4x) ? multi4x : calcTbill;
            var amountMulti3x = (calcTbill > (multi4x + multi3x)) ? multi3x : (calcTbill - multi4x);
            var amountMulti2x = (calcTbill > (multi4x + multi3x + multi2x)) ? multi2x : (calcTbill - multi4x - multi3x);
            var amountMulti15x = (calcTbill > (multi4x + multi3x + multi2x + multi15x)) ? multi15x : (calcTbill - multi4x - multi3x - multi2x);
            var amountMulti125x = (calcTbill > (multi4x + multi3x + multi2x + multi15x + multi125x)) ? multi125x : (calcTbill - multi4x - multi3x - multi2x - multi15x);
            var amountRest = (calcTbill - multi4x - multi3x - multi2x - multi15x - multi125x);

            //make sure it's not negative
            amountMulti3x = amountMulti3x < 0 ? 0 : amountMulti3x;
            amountMulti2x = amountMulti2x < 0 ? 0 : amountMulti2x;
            amountMulti15x = amountMulti15x < 0 ? 0 : amountMulti15x;
            amountMulti125x = amountMulti125x < 0 ? 0 : amountMulti125x;
            amountRest = amountRest < 0 ? 0 : amountRest;

            var apr = (1.25 * amountMulti4x / calcTbill)
                + (1 * amountMulti3x / calcTbill)
                + (0.75 * amountMulti2x / calcTbill)
                + (0.625 * amountMulti15x / calcTbill)
                + (0.5625 * amountMulti125x / calcTbill)
                + (0.5 * amountRest / calcTbill);
            var apy = Math.pow((1 + (apr / compound)), (compound)) - 1;
            let year = new yearInLP(firstInvest, 0, calcTfuel, calcTbill, amountMulti125x / calcTbill, amountMulti15x / calcTbill, amountMulti2x / calcTbill, amountMulti3x / calcTbill, amountMulti4x / calcTbill, apr, apy);
            yearsInLP.push(year);
        } else {
            var prevYear = yearsInLP[i - 1];
            var x = prevYear.apy;
            var val = (prevYear.val + (12 * monthlyInvest)) * (1 + prevYear.apy);
            var netGain = val - prevYear.val;
            var calcTbill = val / 2 / tbillPrice;
            var calcTfuel = val / 2 / tfuelPrice;
            //var usedMulti2x = (multi2x > calcTbill) ? 1 : (1 - ((calcTbill - multi2x) / calcTbill));

            var amountMulti4x = (calcTbill > multi4x) ? multi4x : calcTbill;
            var amountMulti3x = (calcTbill > (multi4x + multi3x)) ? multi3x : (calcTbill - multi4x);
            var amountMulti2x = (calcTbill > (multi4x + multi3x + multi2x)) ? multi2x : (calcTbill - multi4x - multi3x);
            var amountMulti15x = (calcTbill > (multi4x + multi3x + multi2x + multi15x)) ? multi15x : (calcTbill - multi4x - multi3x - multi2x);
            var amountMulti125x = (calcTbill > (multi4x + multi3x + multi2x + multi15x + multi125x)) ? multi125x : (calcTbill - multi4x - multi3x - multi2x - multi15x);
            var amountRest = (calcTbill - multi4x - multi3x - multi2x - multi15x - multi125x);
            //make sure it's not negative
            amountMulti3x = amountMulti3x < 0 ? 0 : amountMulti3x;
            amountMulti2x = amountMulti2x < 0 ? 0 : amountMulti2x;
            amountMulti15x = amountMulti15x < 0 ? 0 : amountMulti15x;
            amountMulti125x = amountMulti125x < 0 ? 0 : amountMulti125x;
            amountRest = amountRest < 0 ? 0 : amountRest;
            var apr = (1.25 * amountMulti4x / calcTbill)
                + (1 * amountMulti3x / calcTbill)
                + (0.75 * amountMulti2x / calcTbill)
                + (0.625 * amountMulti15x / calcTbill)
                + (0.5625 * amountMulti125x / calcTbill)
                + (0.5 * amountRest / calcTbill);

            //var apr = (0.75 * usedMulti2x) + (0.5 * (1 - usedMulti2x));
            var apy = Math.pow((1 + (apr / compound)), (compound)) - 1;
            let year = new yearInLP(val, netGain, calcTfuel, calcTbill, amountMulti125x / calcTbill, amountMulti15x / calcTbill, amountMulti2x / calcTbill, amountMulti3x / calcTbill, amountMulti4x / calcTbill, apr, apy);
            yearsInLP.push(year);
        }
    }

    //populate table header

    $('#calcTable > thead').empty();
    $('#calcTable > thead').append('<tr>'
        + '          <th> Year</th >'
        + '          <th class="text-end">Value</th>'
        + '          <th class="text-end">Net gain</th>'
        + '          <th class="text-end">Calcualted TFuel</th>'
        + '          <th class="text-end">Calculated TBill</th>'
        + (multi125x != 0 ? '          <th class="text-end">TBill covered<br/>with 1.25x mult.</th>' : '')
        + (multi15x != 0 ? '          <th class="text-end">TBill covered<br/>with 1.5x mult.</th>' : '')
        + (multi2x != 0 ? '          <th class="text-end">TBill covered<br/>with 2x mult.</th>' : '')
        + (multi3x != 0 ? '          <th class="text-end">TBill covered<br/>with 3x mult.</th>' : '')
        + (multi4x != 0 ? '          <th class="text-end">TBill covered<br/>with 4x mult.</th>' : '')
        + '          <th class="text-end">APR</th>'
        + '          <th class="text-end">APY</th>'
        + '      </tr >');

    var yearsArray = [];
    $('#calcTable > tbody:last-child').empty();
    $.each(yearsInLP, function (i, val) {

        var valu = (val.val).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var netGain = (val.netGain).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var calcTfuel = (val.calcTfuel).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var calcTbill = (val.calcTbill).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var usedMulti125x = (val.usedMulti125x * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var usedMulti15x = (val.usedMulti15x * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var usedMulti2x = (val.usedMulti2x * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var usedMulti3x = (val.usedMulti3x * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var usedMulti4x = (val.usedMulti4x * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var apr = (val.apr * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var apy = (val.apy * 100).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        //$('#calcTable > tbody:last-child').append('<tr><td>' + i + '</td><td class="text-end">$' + valu + '</td><td class="text-end">$' + netGain + '</td><td class="text-end">' + calcTfuel + '</td><td class="text-end">' + calcTbill + '</td><td class="text-end">' + multi2x + '%</td><td class="text-end">' + apr + '%</td><td class="text-end">' + apy + '%</td></tr>');
        $('#calcTable > tbody:last-child').append('<tr><td>' + i + '</td>'
            + '<td class="text-end">$' + valu + '</td>'
            + '<td class="text-end">$' + netGain + '</td>'
            + '<td class="text-end">' + calcTfuel + '</td>'
            + '<td class="text-end">' + calcTbill + '</td>'
            + (multi125x != 0 ? '<td class="text-end">' + usedMulti125x + '%</td>' : '')
            + (multi15x != 0 ? '<td class="text-end">' + usedMulti15x + '%</td>' : '')
            + (multi2x != 0 ? '<td class="text-end">' + usedMulti2x + '%</td>' : '')
            + (multi3x != 0 ? '<td class="text-end">' + usedMulti3x + '%</td>' : '')
            + (multi4x != 0 ? '<td class="text-end">' + usedMulti4x + '%</td>' : '')
            + '<td class="text-end">' + apr + '%</td>'
            + '<td class="text-end">' + apy + '%</td></tr>');
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
    constructor(val, netGain, calcTfuel, calcTbill, usedMulti125x, usedMulti15x, usedMulti2x, usedMulti3x, usedMulti4x, apr, apy) {
        this.val = val;
        this.netGain = netGain;
        this.calcTfuel = calcTfuel;
        this.calcTbill = calcTbill;
        this.usedMulti125x = usedMulti125x;
        this.usedMulti15x = usedMulti15x;
        this.usedMulti2x = usedMulti2x;
        this.usedMulti3x = usedMulti3x;
        this.usedMulti4x = usedMulti4x;
        this.apr = apr;
        this.apy = apy;
    }
}