var tbillRate = 0;
var nft15xlevel = 0;
var nft2xlevel = 0;

$(document).ready(function () {

    fetchData();
    

    $("form").on("submit", function (event) {

        fetchWalletData();

        event.preventDefault();
    });
});

function fetchWalletData() {
    
    var wallet = $("#walletAddress").val();
    var dailyTotal = 0;
    if (wallet != "") {
        $.getJSON("api/rewards/" + wallet, function (data) {
            let i = 1;
            $('#rewardsTable > tbody:last-child').empty();
            $.each(data['data'], function (key, val) {
                //get first 10 only
                //if (i > 10) {
                //    return false;
                //}
                var tvl = parseFloat(val['tv']).toFixed(2);
                var mtvl = parseFloat(val['mtv']).toFixed(2);
                var reward = parseFloat(val['reward']).toFixed(4);
                var time = getTimeFromTimestamp(val['end_time']);
                var tbill1x = parseFloat(val['tbill1x']).toFixed(2);
                var tbill15x = parseFloat(val['tbill15x']).toFixed(2);
                var tbill2x = parseFloat(val['tbill2x']).toFixed(2);
                var tfuel = parseFloat(val['tfuel']).toFixed(2);
                if (i == 1) {
                    $("#tvl").html(tvl);
                    $("#activTbill").html(parseFloat(tbill1x) + parseFloat(tbill15x) + parseFloat(tbill2x));
                    $("#activeTfuel").html(tfuel);
                    nft15xlevel = tbill15x;
                    nft2xlevel = tbill2x;
                }

                $('#rewardsTable > tbody:last-child').append('<tr><td>' + time + '</td><td class="text-end">' + tbill1x + '</td><td class="text-end">' + tbill15x + '</td><td class="text-end">' + tbill2x + '</td><td class="text-end">' + tfuel + '</td><td class="text-end">$' + tvl + '</td><td class="text-end">$' + mtvl + '</td><td class="text-end">' + reward + '</td><td class="text-end">$' + parseFloat(reward * tbillRate).toFixed(4) + '</td></tr>');
                dailyTotal = parseFloat(dailyTotal) + parseFloat(reward);
                i++;
            });
            $("#dayTotal").html(parseFloat(dailyTotal).toFixed(4));

            fetchNFTforWallet();
        });
        
    }

    if (wallet != "") {
        var datesDaily = [];
        var datesDailySum = [];
        var rewardsSum = 0;
        $.getJSON("api/dailyRewards/" + wallet, function (data) {
            let i = 1;
            let rewards = [];
            let rewardsReverse = [];
            $('#dailyRewardsTable > tbody:last-child').empty();


            var totalRewards = 0;
            $.each(data['vals'], function (key, val) {
                //ignore hte header (first record)
                if (i > 1) {
                    rewards.push({
                        date: val[0],
                        tvl: parseFloat(val[1]).toFixed(2),
                        mtvl: parseFloat(val[2]).toFixed(2),
                        tbill1x: parseFloat(val[3]).toFixed(2),
                        tbill15x: parseFloat(val[4]).toFixed(2),
                        tbill2x: parseFloat(val[5]).toFixed(2),
                        tfuel: parseFloat(val[6]).toFixed(2),
                        reward: parseFloat(val[7]).toFixed(4),
                        rewardUSD: parseFloat(val[8]).toFixed(2)
                    });
                    var innerArr = [val[0], parseFloat(val[7]).toFixed(4)];
                    datesDaily.push(innerArr);
                    rewardsSum = parseFloat(rewardsSum) + parseFloat(parseFloat(val[7]).toFixed(4));
                    var innerArr = [val[0], rewardsSum];
                    datesDailySum.push(innerArr);
                    totalRewards = parseFloat(totalRewards) + parseFloat(parseFloat(val[7]));
                }
                i++;
            });
            $("#totalTbill").html(parseFloat(totalRewards).toFixed(4));

            showDailyChart(datesDaily);
            showDailySumChart(datesDailySum);

            //reverse the order to new array
            rewards.slice().reverse().forEach(function (x) {
                rewardsReverse.push(x);
            });

            for (let y = 0; y < rewardsReverse.length; y++) {
                //only first 10
                if (y > 30) {
                    break;
                }
                //console.log(rewardsReverse[y]);
                $('#dailyRewardsTable > tbody:last-child').append('<tr><td>' + rewardsReverse[y].date + '</td><td class="text-end">' + rewardsReverse[y].tbill1x + '</td><td class="text-end">' + rewardsReverse[y].tbill15x + '</td><td class="text-end">' + rewardsReverse[y].tbill2x + '</td><td class="text-end">'
                    + rewardsReverse[y].tfuel + '</td><td class="text-end">$' + rewardsReverse[y].tvl + '</td><td class="text-end">$' + rewardsReverse[y].mtvl + '</td><td class="text-end">' + rewardsReverse[y].reward + '</td><td class="text-end">$' + rewardsReverse[y].rewardUSD + '</td></tr>');
                
            }
        });
    }

}

function fetchNFTforWallet() {
    //fetch NFT info for wallet
    var data = $("#walletAddress").val();
    var jsonData = {
        "walletAddress": $("#walletAddress").val()
    }
    var nft15xsum = 0;
    var nft2xsum = 0;

    $('#myNFTsTable > tbody:last-child').empty();

    $.post("api/getNFTforWallet/", jsonData, function (data, textStatus) {
        var jsonObj = jQuery.parseJSON(data);
        $.each(jsonObj, function (key, val) {
            var Name = val['Name'];
            var Multiplier = val['Multiplier'];
            var TbillAmount = val['TbillAmount'];
            var BoostPercentage = val['BoostPercentage'];
            var Edition = val['Edition'];
            if (Multiplier = "2x") {
                nft2xsum += parseFloat(TbillAmount);
            } else if (Multiplier = "1.5x") {
                nft15xsum += parseFloat(TbillAmount);
            }
            $('#myNFTsTable > tbody:last-child').append('<tr><td>' + Name + '</td><td class="text-end">' + Multiplier + '</td><td class="text-end">' + TbillAmount + '</td><td class="text-end">' + BoostPercentage + '</td><td class="text-end">' + Edition + '</td></tr>');

        });

        $("#nft2xlabel").html(nft2xlevel + ' / ' + nft2xsum);
        $("#nft15xlabel").html(nft15xlevel + ' / ' + nft15xsum);

        var prct2x = parseFloat(parseFloat(nft2xlevel) / nft2xsum * 100).toFixed(1);
        var prct15x = parseFloat(parseFloat(nft15xlevel) / nft15xsum * 100).toFixed(1);

        $('#progress2x').css('width', prct2x + '%').attr('aria-valuenow', prct2x);
        $('#progress15x').css('width', prct15x + '%').attr('aria-valuenow', prct15x);

    }, "json");
}

function showDailyChart(data) {

    var options = {
        series: [{
            name: 'Daily',
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
            type: 'bar',
            height: 350
        },
        dataLabels: {
            enabled: false
        },
        markers: {
            size: 0,
        },
        title: {
            text: 'Daily rewards',
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
                    return (val / 1).toFixed(4);
                },
            },
            title: {
                text: 'TBILL'
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
                    return (val / 1).toFixed(4)
                }
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartDaily"), options);
    $("#chartDaily").empty();
    chart.render();
}

function showDailySumChart(data) {

    var options = {
        series: [{
            name: 'Total rewards',
            data: data
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
            text: 'Rewards in total',
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
                    return (val / 1).toFixed(2);
                },
            },
            title: {
                text: 'TBILL'
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
                    return (val / 1).toFixed(2)
                }
            }
        },
        theme: {
            mode: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartDailySum"), options);
    $("#chartDailySum").empty();
    chart.render();
}


function fetchData() {
    $.getJSON("api/rates", function (data) {
        var rate = "";
        var rateTFuel = "";
        $.each(data['rates'], function (key, val) {
            if (val['pair'] == 'tbill_usd') {
                rate = val['rate'];
            } else if (val['pair'] == 'tfuel_usd') {
                rateTFuel = val['rate'];
            }
        });
        var targetRate = data['targetRate'];
        var rebaseRate = data['rebaseRate'];
        var noRebaseRangeTop = data['noRebaseRange']['top'];
        var noRebaseRangeBottom = data['noRebaseRange']['bottom'];

        if (rebaseRate > noRebaseRangeTop) {
            $("#rebaseRate").addClass("text-success");
        } else if (rebaseRate < noRebaseRangeBottom) {
            $("#rebaseRate").addClass(".text-danger");
        }

        $("#tbillRate").html(parseFloat(rate).toFixed(4));
        $("#tbillRateTop").html(parseFloat(rate).toFixed(4));
        $("#tbillTfuelRatio").html('Ratio:' + parseFloat(parseFloat(rate) / parseFloat(rateTFuel)).toFixed(4));
        tbillRate = parseFloat(rate).toFixed(4);
        $("#targetRate").html(parseFloat(targetRate).toFixed(4));
        $("#rebaseRate").html(parseFloat(rebaseRate).toFixed(4));
        $("#noRebaseRangeTop").html(parseFloat(noRebaseRangeTop).toFixed(4));
        $("#noRebaseRangeBottom").html(parseFloat(noRebaseRangeBottom).toFixed(4));
        $("#tfuelPrice").html(parseFloat(rateTFuel).toFixed(4));
        $("#tfuelPriceTop").html(parseFloat(rateTFuel).toFixed(4));

    });

    

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