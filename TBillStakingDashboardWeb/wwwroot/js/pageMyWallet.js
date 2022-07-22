let NFTValueInfo = [];
let MyNFTs = [];

$(document).ready(function () {
    var nftValueModal = document.getElementById('nftValueModal');
    nftValueModal.addEventListener('shown.bs.modal', function () {
        refreshNFTValueInfo();
    });
});

function refreshNFTValueInfo() {
    $('#myNFTsValueTable > tbody:last-child').empty();
    var countSum = 0;
    var LastSaleSum = 0;
    var LastSaleUsdSum = 0;
    var Last5salesAvgSum = 0;
    var Last5salesAvgUsdSum = 0;
    var CurrentSalePriceSum = 0;
    var CurrentSalePriceUsdSum = 0;


    $.getJSON("api/getNFTValueInfo", function (data) {
        let i = 1;
        

        $.each(data, function (key, val) {
            //if (i == 1) {
            //    alert(val.Name);
            //}
            NFTValueInfo.push(val);
            i++;
        });
        
        $.each(MyNFTs, function (key, val) {
            var count = val.count;
            var NFTInfo = NFTValueInfo.find(x => x.Name === val.name);
            if (typeof NFTInfo !== "undefined") {
                countSum += count;
                LastSaleSum += count * parseFloat(NFTInfo.LastSale) ;
                LastSaleUsdSum += count * parseFloat(NFTInfo.LastSaleUsd);
                Last5salesAvgSum += count * parseFloat(NFTInfo.Last5salesAvg);
                Last5salesAvgUsdSum += count * parseFloat(NFTInfo.Last5salesAvgUsd);
                CurrentSalePriceSum += count * parseFloat(NFTInfo.CurrentSalePrice);
                CurrentSalePriceUsdSum += count * parseFloat(NFTInfo.CurrentSalePriceUsd);
            }
            $('#myNFTsValueTable > tbody:last-child').append('<tr><td>' + val.name + '</td>'
                + '<td class= "text-end" > ' + count + '</td> '
                + '<td class= "text-end tableBorderLeft" > ' + parseFloat(NFTInfo.LastSale).toFixed(2) + '</td> '
                + '<td class= "text-end" > ' + parseFloat(NFTInfo.LastSaleUsd).toFixed(2) + '</td> '
                + '<td class= "text-end tableBorderLeft" > ' + parseFloat(NFTInfo.Last5salesAvg).toFixed(2) + '</td> '
                + '<td class= "text-end" > ' + parseFloat(NFTInfo.Last5salesAvgUsd).toFixed(2) + '</td> '
                + '<td class= "text-end tableBorderLeft" > ' + parseFloat(NFTInfo.CurrentSalePrice).toFixed(2) + '</td> '
                + '<td class= "text-end" > ' + parseFloat(NFTInfo.CurrentSalePriceUsd).toFixed(2) + '</td> '
                +'</tr > ');

        });

        $('#myNFTsValueTable > tbody:last-child').append('<tr><td>TOTAL</td>'
            + '<td class= "text-end"> <strong> ' + countSum + '</strong></td> '
            + '<td class= "text-end tableBorderLeft"> <strong> ' + parseFloat(LastSaleSum).toFixed(2) + '</strong></td> '
            + '<td class= "text-end"> <strong> ' + parseFloat(LastSaleUsdSum).toFixed(2) + '</strong></td> '
            + '<td class= "text-end tableBorderLeft"> <strong> ' + parseFloat(Last5salesAvgSum).toFixed(2) + '</strong></td> '
            + '<td class= "text-end"> <strong> ' + parseFloat(Last5salesAvgUsdSum).toFixed(2) + '</strong></td> '
            + '<td class= "text-end tableBorderLeft"> <strong> ' + parseFloat(CurrentSalePriceSum).toFixed(2) + '</strong></td> '
            + '<td class= "text-end"> <strong> ' + parseFloat(CurrentSalePriceUsdSum).toFixed(2) + '</strong></td> '
            + '</tr > ');
    });
}

function fetchWalletData() {

    clearMyWalletData();

    var wallet = $("#walletAddress").val();

    //add wallet to URL
    var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?wallet=' + wallet;
    window.history.replaceState(null, null, newurl);

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
                var tbill1x_gnote = parseFloat(val['tbill1x_gnote']).toFixed(2);
                var tbill1x_tfuel = parseFloat(val['tbill1x_tfuel']).toFixed(2);
                var tbill125x = parseFloat(val['tbill125x']).toFixed(2);
                var tbill125x_gnote = parseFloat(val['tbill125x_gnote']).toFixed(2);
                var tbill125x_tfuel = parseFloat(val['tbill125x_tfuel']).toFixed(2);
                var tbill15x = parseFloat(val['tbill15x']).toFixed(2);
                var tbill15x_gnote = parseFloat(val['tbill15x_gnote']).toFixed(2);
                var tbill15x_tfuel = parseFloat(val['tbill15x_tfuel']).toFixed(2);
                var tbill2x = parseFloat(val['tbill2x']).toFixed(2);
                var tbill2x_gnote = parseFloat(val['tbill2x_gnote']).toFixed(2);
                var tbill2x_tfuel = parseFloat(val['tbill2x_tfuel']).toFixed(2);
                var tbill3x = parseFloat(val['tbill3x']).toFixed(2);
                var tbill4x = parseFloat(val['tbill4x']).toFixed(2);
                var tfuel = parseFloat(val['tfuel']).toFixed(2);
                var gnote = parseFloat(val['gnote']).toFixed(4);
                if (i == 1) {
                    $("#tvl").html(tvl);
                    $("#activTbill").html(parseFloat(tbill1x) + parseFloat(tbill125x) + parseFloat(tbill15x) + parseFloat(tbill2x) + parseFloat(tbill3x) + parseFloat(tbill4x));
                    $("#activeTfuel").html(tfuel);
                    $("#activeGnote").html(gnote);
                    nft125xlevel = tbill125x_tfuel;
                    nft15xlevel = tbill15x_tfuel;
                    nft2xlevel = tbill2x_tfuel;

                    nft125xlevelGnote = tbill125x_gnote;
                    nft15xlevelGnote = tbill15x_gnote;
                    nft2xlevelGnote = tbill2x_gnote;
                }

                $('#rewardsTable > tbody:last-child').append('<tr><td>' + time + '</td>'
                    + '<td class="text-end" title="' + tbill1x_tfuel + ' TFuel / ' + tbill1x_gnote + ' gNOTE">' + tbill1x + '</td>'
                    + '<td class="text-end" title="' + tbill125x_tfuel + ' TFuel / ' + tbill125x_gnote + ' gNOTE">' + tbill125x + '</td>'
                    + '<td class="text-end" title="' + tbill15x_tfuel + ' TFuel / ' + tbill15x_gnote + ' gNOTE">' + tbill15x + '</td>'
                    + '<td class="text-end" title="' + tbill2x_tfuel + ' TFuel / ' + tbill2x_gnote + ' gNOTE">' + tbill2x + '</td>'
                    + '<td class="text-end">' + tfuel + '</td>'
                    + '<td class="text-end">' + gnote + '</td>'
                    + '<td class="text-end">$' + tvl + '</td>'
                    + '<td class="text-end">$' + mtvl + '</td>'
                    + '<td class="text-end">' + reward + '</td>'
                    +'<td class="text-end">$' + parseFloat(reward * tbillRate).toFixed(4) + '</td></tr>');
                dailyTotal = parseFloat(dailyTotal) + parseFloat(reward);
                i++;
            });
            $("#dayTotal").html(parseFloat(dailyTotal).toFixed(4) + '<img height="20" src="/img/tbill.svg" /><br/>' + '$' + parseFloat(dailyTotal * tbillRate).toFixed(2));

            fetchNFTforWallet();
        });

    }

    if (wallet != "") {
        $.getJSON("api/my-overview/" + wallet, function (data) {
           
            var realIl = parseFloat(data['data']['realIlUsd']).toFixed(2);
            var currIl = parseFloat(data['data']['currIlUsd']).toFixed(2);
            var realIlTfuel = parseFloat(data['data']['realIlTFuel']).toFixed(2);
            var currIlTfuel = parseFloat(data['data']['currIlTFuel']).toFixed(2);
            var extraIl = parseFloat(data['data']['extraUsd']).toFixed(2);
            var extraIlTfuel = parseFloat(data['data']['extraTFuel']).toFixed(2);
            var updateTime = data['data']['updateTime'];
            $('#realIl').html('$' + realIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + realIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
            $('#currIl').html('$' + currIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + currIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
            $('#extraIl').html('$' + extraIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + extraIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
            
            $('#currIlPopover').attr('data-bs-content', 'Last refresh (UTC):<br>' +updateTime);

            // Projected Amount
            var projectedAmount = parseFloat(data['data']['tbillsProjected']).toFixed(0);
            var timeUntilMint = parseFloat(data['data']['daysLeft']).toFixed(0);
            $('#timeUntilMint').html(timeUntilMint.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + ' (projected ' + projectedAmount.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + ' TBILL)');

            

            // refresh the currIlPopover popover
            const popoverIL = document.querySelector('#currIlPopover');
            new bootstrap.Popover(popoverIL, { html: true });
            
            
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

            let dataTVL = [];
            let dataTbill = [];
            let dataTfuel = [];

            var totalRewards = 0;
            var totalRewardsUsd = 0;
            $.each(data['vals'], function (key, val) {
                //ignore hte header (first record)
                if (i > 1) {
                    rewards.push({
                        date: val[0],
                        tvl: parseFloat(val[1]).toFixed(2),
                        mtvl: parseFloat(val[2]).toFixed(2),
                        tbill1x: parseFloat(val[3]).toFixed(2),
                        tbill125x: parseFloat(val[4]).toFixed(2),
                        tbill15x: parseFloat(val[5]).toFixed(2),
                        tbill2x: parseFloat(val[6]).toFixed(2),
                        tbill3x: parseFloat(val[7]).toFixed(2),
                        tbill4x: parseFloat(val[8]).toFixed(2),
                        tfuel: parseFloat(val[9]).toFixed(2),
                        gnote: parseFloat(val[10]).toFixed(2),
                        reward: parseFloat(val[11]).toFixed(4),
                        rewardUSD: parseFloat(val[12]).toFixed(2)
                    });
                    var innerArr = [val[0], parseFloat(val[11]).toFixed(4)];
                    datesDaily.push(innerArr);
                    rewardsSum = parseFloat(rewardsSum) + parseFloat(parseFloat(val[11]).toFixed(4));
                    var innerArr = [val[0], rewardsSum];
                    datesDailySum.push(innerArr);


                    var innerArr = [val[0], parseFloat(val[1]).toFixed(2)];
                    dataTVL.push(innerArr);
                    var innerArr = [val[0], parseFloat(parseFloat(val[3]) + parseFloat(val[4]) + parseFloat(val[5]) + parseFloat(val[6]) + parseFloat(val[7]) + parseFloat(val[8])).toFixed(2)];
                    dataTbill.push(innerArr);
                    var innerArr = [val[0], parseFloat(val[9]).toFixed(2)];
                    dataTfuel.push(innerArr);

                    totalRewards = parseFloat(totalRewards) + parseFloat(parseFloat(val[11]));
                    totalRewardsUsd = parseFloat(totalRewardsUsd) + parseFloat(parseFloat(val[12]));
                }
                i++;
            });
            $("#totalTbill").html(parseFloat(totalRewards).toFixed(4) + '<img height="20" src="/img/tbill.svg" /><br/>' + '$' + parseFloat(totalRewardsUsd).toFixed(2));
            $("#daysInLP").html(i - 1);

            showDailyChart(datesDaily);
            showDailySumChart(datesDailySum);
            showTVLChart(dataTVL, dataTbill, dataTfuel);

            //reverse the order to new array
            rewards.slice().reverse().forEach(function (x) {
                rewardsReverse.push(x);
            });

            for (let y = 0; y < rewardsReverse.length; y++) {
                //only first 10
                if (y > 100) {
                    break;
                }
                //console.log(rewardsReverse[y]);
                $('#dailyRewardsTable > tbody:last-child').append('<tr><td>' + rewardsReverse[y].date + '</td><td class="text-end">' + rewardsReverse[y].tbill1x + '</td><td class="text-end">' + rewardsReverse[y].tbill125x + '</td><td class="text-end">' + rewardsReverse[y].tbill15x + '</td><td class="text-end">' + rewardsReverse[y].tbill2x + '</td><td class="text-end">'
                    + rewardsReverse[y].tfuel + '</td><td class="text-end">' + rewardsReverse[y].gnote + '</td><td class="text-end">$' + rewardsReverse[y].tvl + '</td><td class="text-end">$' + rewardsReverse[y].mtvl + '</td><td class="text-end">' + rewardsReverse[y].reward + '</td><td class="text-end">$' + rewardsReverse[y].rewardUSD + '</td></tr>');

            }
        });
    }
    if (wallet != "") {
        $.getJSON("api/getBalance/" + wallet, function (data) {
            var json = JSON.parse(data);
            var balanceTheta = json.Theta.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var balanceTFuel = json.TFuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var balanceTBill = json.TBill.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var stakeTheta = json.ThetaStake.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var stakeTFuel = json.TFuelStake.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            var balanceTFuelUSD = (parseFloat(json.TFuel) * tfuelRate).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var balanceTBillUSD = (parseFloat(json.TBill) * tbillRate).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var stakeTFuelUSD = (parseFloat(json.TFuelStake) * tfuelRate).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            //console.log(balanceTFuel.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }));
            //console.log(json.TFuel.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }));

            $("#tbillBalance").html(balanceTBill + '<img height="20" src="/img/tbill.svg" /><br/>' + '$' + balanceTBillUSD);
            $("#tfuelBalance").html(balanceTFuel + '<img height="20" src="/img/tfuel.svg" /><br/>' + '$' + balanceTFuelUSD);
            if (parseFloat(json.Theta) > 0) {
                $("#thetaBlock").show();
                $("#thetaBalance").html(balanceTheta);
            }
            if (parseFloat(json.ThetaStake) > 0) {
                $("#thetaStakeBlock").show();
                $("#thetaStake").html(stakeTheta);
            }
            if (parseFloat(json.TFuelStake) > 0) {
                $("#tfuelStakeBlock").show();
                $("#tfuelStake").html(stakeTFuel + '<img height="20" src="/img/tfuel.svg" /><br/>' + '$' + stakeTFuelUSD);
            }
        });
    }
    if (wallet != "") {
        $.getJSON("api/getMyWalletLpStats/" + wallet, function (data) {
            //var json = JSON.parse(data);
            var position = data.Position;
            var positionTotal = data.PositionTotal;
            var univ2 = data.Univ2.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 4 });
            var univ2Total = data.Univ2Total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var myPct = data.MyPct.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 5 });;

            $("#lpPosition").html(position + ' / ' + positionTotal);
            $("#univ2").html(univ2 + ' / ' + univ2Total);
            $("#lpPct").html(myPct + ' %');

            var univ2Hist = [];
            $.each(data.Univ2Hist, function (key, val) {
                //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
                var innerArr = [val["Item1"], val["Item2"]];
                univ2Hist.push(innerArr);

            });

            showUniv2Chart(univ2Hist);

        });
    }

    //get gNOTE balance
    if (wallet != "") {
        $.getJSON("api/balanceGnote/" + wallet, function (data) {
            //var json = JSON.parse(data);
            var balance = data.balance;
            var bal = (parseFloat(balance)/1000000000000000000).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 5 });
            $("#gNoteBalance").html(bal);

        });
    }


}

function clearMyWalletData() {
    nft125xlevel = 0;
    nft15xlevel = 0;
    nft2xlevel = 0;
    nft2xlevelGnote = 0;
    nft2xlevelGnote = 0;
    nft2xlevelGnote = 0;
    $("#thetaBalance").html('');
    $("#tfuelBalance").html('');
    $("#thetaStake").html('');
    $("#tfuelStakeBlock").hide();
    $("#tfuelStake").html('');
    $("#thetaBlock").hide();
    $("#tbillBalance").html('');
    $("#thetaStakeBlock").hide();
    $("#gNoteBalance").html('');

    $("#dayTotal").html('');
    $("#totalTbill").html('');
    $("#tvl").html('');
    $("#activTbill").html('');
    $("#activeTfuel").html('');
    $("#gnoteTfuel").html('');
    $("#daysInLP").html('');

    $("#nft2xlabel").html(0 + ' / ' + 0);
    $("#nft15xlabel").html(0 + ' / ' + 0);
    $("#nft125xlabel").html(0 + ' / ' + 0);

    $("#nft2xlabelGnote").html(0 + ' / ' + 0);
    $("#nft15xlabelGnote").html(0 + ' / ' + 0);
    $("#nft125xlabelGnote").html(0 + ' / ' + 0);


    $('#progress2x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress15x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress125x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress2xGnote').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress15xGnote').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress125xGnote').css('width', 0 + '%').attr('aria-valuenow', 0);

    MyNFTs = [];
}

function fetchNFTforWallet() {
    //fetch NFT info for wallet
    var data = $("#walletAddress").val();
    var jsonData = {
        "walletAddress": $("#walletAddress").val()
    }
    var nft125xsum = 0;
    var nft15xsum = 0;
    var nft2xsum = 0;

    $('#myNFTsTable > tbody:last-child').empty();

    $.post("api/getNFTforWallet/", jsonData, function (data, textStatus) {
        var jsonObj = jQuery.parseJSON(data);
        $.each(jsonObj, function (key, val) {
            var Name = val['Name'];
            var ImageURL = val['ImageURL'];
            var Multiplier = val['Multiplier'];
            var TbillAmount = val['TbillAmount'];
            var BoostPercentage = val['BoostPercentage'];
            var Edition = val['Edition'];
            if (Multiplier == "2x") {
                nft2xsum += parseFloat(TbillAmount);
            } else if (Multiplier == "1.5x") {
                nft15xsum += parseFloat(TbillAmount);
            } else if (Multiplier == "1.25x" && !Name.includes("Sticker")) {
                nft125xsum += parseFloat(TbillAmount);
            }
            $('#myNFTsTable > tbody:last-child').append('<tr><td><img height="30" src="' + ImageURL + '" /></td><td>' + Name + '</td><td class="text-end">' + Multiplier + '</td><td class="text-end">' + TbillAmount + '</td><td class="text-end">' + BoostPercentage + '</td><td class="text-end">' + Edition + '</td></tr>');

            // count each NFT, if it's already in the array, then add 1 to count
            if (Edition != 0) {
                if (MyNFTs.findIndex(x => x.name === val['Name']) < 0) {
                    MyNFTs.push({ name: val['Name'], count: 1 });
                } else {
                    MyNFTs.find(x => x.name === val['Name']).count = MyNFTs.find(x => x.name === val['Name']).count + 1;
                }
            }
        });

        $("#nft2xlabel").html(nft2xlevel + ' / ' + nft2xsum);
        $("#nft15xlabel").html(nft15xlevel + ' / ' + nft15xsum);
        $("#nft125xlabel").html(nft125xlevel + ' / ' + nft125xsum);

        $("#nft2xlabelGnote").html(nft2xlevelGnote + ' / ' + nft2xsum);
        $("#nft15xlabelGnote").html(nft15xlevelGnote + ' / ' + nft15xsum);
        $("#nft125xlabelGnote").html(nft125xlevelGnote + ' / ' + nft125xsum);

        var prct2x = parseFloat(parseFloat(nft2xlevel) / nft2xsum * 100).toFixed(1);
        var prct15x = parseFloat(parseFloat(nft15xlevel) / nft15xsum * 100).toFixed(1);
        var prct125x = parseFloat(parseFloat(nft125xlevel) / nft125xsum * 100).toFixed(1);

        $('#progress2x').css('width', prct2x + '%').attr('aria-valuenow', prct2x);
        $('#progress15x').css('width', prct15x + '%').attr('aria-valuenow', prct15x);
        $('#progress125x').css('width', prct125x + '%').attr('aria-valuenow', prct125x);

    }, "json");
}

function showTVLChart(dataTVL, dataTbill, dataTFuel) {

    var options = {
        series: [{
            name: 'TVL',
            data: dataTVL
        }, {
            name: 'TFuel',
            data: dataTFuel
        }, {
            name: 'TBill',
            data: dataTbill
        }],
        chart: {
            type: 'line',
            stacked: false,
            height: 350,
            //group: 'myWallet',
            //id: 'tvl',
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
            text: 'Total Value Locked',
            align: 'left'
        },
        yaxis: {
            labels: {
                formatter: function (val) {
                    return (val / 1000).toFixed(0) + ' K';
                },
                minWidth: 40
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
                    return (val / 1).toFixed(2)
                }
            }
        },
        stroke: {
            width: [3, 3, 3]
        },
        theme: {
            mode: 'dark'
        },
        colors: ['#85bb65', '#e75c10', '#1d2c3b']
    };

    var chart = new ApexCharts(document.querySelector("#chartTVL"), options);
    $("#chartTVL").empty();
    chart.render();
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
            height: 350,
            //group: 'myWallet',
            //id: 'daily'
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
                minWidth: 40
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
            //id: 'rewards',
            //group: 'myWallet',
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
                minWidth: 40
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

function showUniv2Chart(data) {

    var options = {
        series: [{
            name: 'Uni-v2',
            type: 'column',
            data: data
        }],
        chart: {
            height: 350,
            type: 'line',
        },
        stroke: {
            width: [0]
        },
        title: {
            text: 'Uni-v2'
        },
        xaxis: {
            type: 'datetime'
        },
        yaxis: [{
            title: {
                text: 'uni-v2',
            }, labels: {
                formatter: function (val) {
                    /*    return (val / 1).toFixed(2);*/
                    return val.toFixed(4);
                }
            },
            min: 0,

        }],
        
        theme: {
            mode: 'dark'
        }
        //,
        //colors: [ '#4ECDC4']
    };

    var chart = new ApexCharts(document.querySelector("#chartUniv2"), options);
    $("#chartUniv2").empty();
    chart.render();
}
