var tbillRate = 0;
var tfuelRate = 0;
var nft125xlevel = 0;
var nft15xlevel = 0;
var nft2xlevel = 0;

$(document).ready(function () {

    //enable popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl, { html: true }) //, { html: true } - enable HTML in content
    })

    //check if we are on MyWallet page, if so, the wallet data will be fetched after rate
    fetchData(window.location.pathname.includes("MyWallet"));


    $("form").on("submit", function (event) {

        fetchWalletData();

        event.preventDefault();
    });

    $('#nftStats').DataTable({
        "paging": false,
        "searching": false,
        "autoWidth": false,
        columnDefs: [
            { orderable: false, targets: 0 },
            { width: "10", targets: 0 }
        ],
        order: [[2, 'desc']],
        "language": {
            "info": ""
        }
    });
});








function fetchData(fetchWallet) {
    $.getJSON("api/rates", function (data) {
        var rate = "";
        var rateTFuel = "";
        var rateGnote = "";
        $.each(data['rates'], function (key, val) {
            if (val['pair'] == 'tbill_usd') {
                rate = val['rate'];
            } else if (val['pair'] == 'tfuel_usd') {
                rateTFuel = val['rate'];
            } else if (val['pair'] == 'gnote_usd') {
                rateGnote = val['rate'];
            }
        });
        var targetRate = data['targetRate'];
        var rebaseRate = data['rebaseRate'];
        var noRebaseRangeTop = data['noRebaseRange']['top'];
        var noRebaseRangeBottom = data['noRebaseRange']['bottom'];
        var lpTokenRate = data['lpTokenRate'];

        if (rebaseRate > noRebaseRangeTop) {
            $("#rebaseRate").addClass("text-success");
        } else if (rebaseRate < noRebaseRangeBottom) {
            $("#rebaseRate").addClass("text-danger");
        }
        //rebaseRate = 0.65;
        const rebaseDays = 2;
        const inRange = rebaseRate >= targetRate * 0.95 && rebaseRate <= targetRate * 1.05;
        const nextAvg = inRange ? rebaseRate : (targetRate - rebaseRate) / rebaseDays + rebaseRate;
        const ratio = inRange ? 1 : nextAvg / rebaseRate;
        const priceAfterRebase = parseFloat(rate) * ratio;
        const nextRebase = (1 - (1 / ratio)) * -100;

        //const nextTBills = 1 / ratio; 

        $("#tbillRate").html(parseFloat(rate).toFixed(4));
        $("#tbillRateTop").html(parseFloat(rate).toFixed(4));
        $("#tbillTfuelRatio").html('Ratio:' + parseFloat(parseFloat(rate) / parseFloat(rateTFuel)).toFixed(4));
        tbillRate = parseFloat(rate).toFixed(4);
        tfuelRate = parseFloat(rateTFuel).toFixed(4);
        $("#targetRate").html(parseFloat(targetRate).toFixed(4));
        $("#rebaseRate").html(parseFloat(rebaseRate).toFixed(4));
        $("#noRebaseRangeTop").html(parseFloat(noRebaseRangeTop).toFixed(4));
        $("#noRebaseRangeBottom").html(parseFloat(noRebaseRangeBottom).toFixed(4));
        $("#tfuelPrice").html(parseFloat(rateTFuel).toFixed(4));
        $("#tfuelPriceTop").html(parseFloat(rateTFuel).toFixed(4));
        $("#gnotePriceTop").html(parseFloat(rateGnote).toFixed(4));
        $("#lpTokenRate").html('$ ' + parseFloat(lpTokenRate).toFixed(2));

        $("#nextRebaseRate").html(parseFloat(priceAfterRebase).toFixed(4));
        $("#nextRebaseTbillChange").html('&nbsp;(TBill change: ' + parseFloat(nextRebase).toFixed(2) + ' %)');

        if (fetchWallet) {
            fetchWalletData();
            //refresh wallet data every 10 minutes
            setInterval(fetchWalletData, 600000);
        }

        if (window.location.pathname.includes("Calc")) {
            $("#tfuelPrice").val(tfuelRate);
            $("#tbillPrice").val(parseFloat(targetRate).toFixed(4));
            calculate();
        }
    });



}

function getNFTSales(sel) {
    //alert(sel.value);
    var nftName = sel.value;

    if (nftName != "") {
        getNFTSalesAPICall(nftName);
    }
}

function changeNFTselect(nftName) {
    document.getElementById('nftSelect').value = nftName;
    getNFTSalesAPICall(nftName);
}

function getNFTSalesAPICall(nftName) {
    if (nftName != "") {
        var jsonData = {
            "name": nftName
        }

        $.post("api/getNFTSales/", jsonData, function (data, textStatus) {
            var jsonObj = jQuery.parseJSON(data);

            var datesPrices = [];
            jsonObj.forEach(async (value) => {
                //sum = await sumFunction(sum, value);
                var innerArr = [value["Item1"], value["Item2"]];
                datesPrices.push(innerArr);

            });
            refreshNFTSalesChart(datesPrices, nftName);
        });
    }
}

function refreshNFTSalesChart(data, name) {

    var optionsChartNFTSold = {
        series: [{
            name: name,
            data: data
        }],
        chart: {
            type: 'line',
            stacked: false,
            height: 300,
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            },
            toolbar: {
                autoSelected: 'zoom'
            },
            animations: {
                enabled: false
            }
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: name,
            align: 'left'
        },
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val / 1).toFixed(0);
                },
            },
            title: {
                text: 'Price TFuel'
            },
            //logarithmic: false,
            //logBase: 5,
            //tickAmount: 6,
            //min: 100
        },
        xaxis: {
            type: 'datetime',
        },
        tooltip: {
            enabled: true,
            shared: true,
            y: {
                formatter: function (val) {
                    return (val / 1).toFixed(0)
                }
            }
        },
        theme: {
            mode: 'dark'
        },
        stroke: {
            width: 3
        }
    };




    var chartNFTSold = new ApexCharts(document.querySelector("#chartNFTSold"), optionsChartNFTSold);
    $("#chartNFTSold").empty();
    chartNFTSold.render();


}

//source https://stackoverflow.com/questions/847185/convert-a-unix-timestamp-to-time-in-javascript
function getTimeFromTimestamp(timestamp) {
    let unix_timestamp = timestamp
    // Create a new JavaScript Date object based on the timestamp
    // multiplied by 1000 so that the argument is in milliseconds, not seconds.
    var date = new Date(unix_timestamp * 1000);
    // Hours part from the timestamp
    var hours = date.getHours();
    // Minutes part from the timestamp
    var minutes = "0" + date.getMinutes();
    // Seconds part from the timestamp
    var seconds = "0" + date.getSeconds();

    // Will display time in 10:30:23 format
    return formattedTime = hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);

}