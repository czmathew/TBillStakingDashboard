﻿let NFTValueInfo = [];
let MyNFTs = [];
totalBalanceUSD = 0;

$(document).ready(function () {
    var nftValueModal = document.getElementById('nftValueModal');
    nftValueModal.addEventListener('shown.bs.modal', function () {
        refreshNFTValueInfo();
    });
    $("#rewardsDownload").click(function (e) {
        
        let url = 'https://api.gpool.io/tbill/rewards-csv?wallet=' + $("#walletAddress").val();
        window.open(url, '_blank');
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
                LastSaleSum += count * parseFloat(NFTInfo.LastSale);
                LastSaleUsdSum += count * parseFloat(NFTInfo.LastSaleUsd);
                Last5salesAvgSum += count * parseFloat(NFTInfo.Last5salesAvg);
                Last5salesAvgUsdSum += count * parseFloat(NFTInfo.Last5salesAvgUsd);
                CurrentSalePriceSum += count * parseFloat(NFTInfo.CurrentSalePrice);
                CurrentSalePriceUsdSum += count * parseFloat(NFTInfo.CurrentSalePriceUsd);

                $('#myNFTsValueTable > tbody:last-child').append('<tr><td>' + val.name + '</td>'
                    + '<td class= "text-end" > ' + count + '</td> '
                    + '<td class= "text-end tableBorderLeft" > ' + parseFloat(NFTInfo.LastSale).toFixed(2) + '</td> '
                    + '<td class= "text-end" > ' + parseFloat(NFTInfo.LastSaleUsd).toFixed(2) + '</td> '
                    + '<td class= "text-end tableBorderLeft" > ' + parseFloat(NFTInfo.Last5salesAvg).toFixed(2) + '</td> '
                    + '<td class= "text-end" > ' + parseFloat(NFTInfo.Last5salesAvgUsd).toFixed(2) + '</td> '
                    + '<td class= "text-end tableBorderLeft" > ' + parseFloat(NFTInfo.CurrentSalePrice).toFixed(2) + '</td> '
                    + '<td class= "text-end" > ' + parseFloat(NFTInfo.CurrentSalePriceUsd).toFixed(2) + '</td> '
                    + '</tr > ');
            }

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
        let has3x = false;
        let has4x = false;
        $.getJSON("api/rewards/" + wallet, function (data) {
            let i = 1;
            $('#rewardsTable > tbody:last-child').empty();
            $.each(data['data'], function (key, val) {
                //get first 10 only
                //if (i > 10) {
                //    return false;
                //}

                if (parseFloat(val['tbill4x']) !== 0 && has4x !== true) {
                    has4x = true;
                }
                if (parseFloat(val['tbill3x']) !== 0 && has3x !== true) {
                    has3x = true;
                }
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
                var tbill3x_gnote = parseFloat(val['tbill3x_gnote']).toFixed(2);
                var tbill3x_tfuel = parseFloat(val['tbill3x_tfuel']).toFixed(2);
                var tbill4x = parseFloat(val['tbill4x']).toFixed(2);
                var tbill4x_gnote = parseFloat(val['tbill4x_gnote']).toFixed(2);
                var tbill4x_tfuel = parseFloat(val['tbill4x_tfuel']).toFixed(2);
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
                    nft3xlevel = tbill3x_tfuel;
                    nft4xlevel = tbill4x_tfuel;

                    nft125xlevelGnote = tbill125x_gnote;
                    nft15xlevelGnote = tbill15x_gnote;
                    nft2xlevelGnote = tbill2x_gnote;
                    nft3xlevelGnote = tbill3x_gnote;
                    nft4xlevelGnote = tbill4x_gnote;
                }

                $('#rewardsTable > tbody:last-child').append('<tr><td>' + time + '</td>'
                    + '<td class="text-end" title="' + tbill1x_tfuel + ' TFuel / ' + tbill1x_gnote + ' gNOTE">' + tbill1x + '</td>'
                    + '<td class="text-end" title="' + tbill125x_tfuel + ' TFuel / ' + tbill125x_gnote + ' gNOTE">' + tbill125x + '</td>'
                    + '<td class="text-end" title="' + tbill15x_tfuel + ' TFuel / ' + tbill15x_gnote + ' gNOTE">' + tbill15x + '</td>'
                    + '<td class="text-end" title="' + tbill2x_tfuel + ' TFuel / ' + tbill2x_gnote + ' gNOTE">' + tbill2x + '</td>'
                    + '<td class="text-end hiddenColumn" title="' + tbill3x_tfuel + ' TFuel / ' + tbill3x_gnote + ' gNOTE">' + tbill3x + '</td>'
                    + '<td class="text-end hiddenColumn" title="' + tbill4x_tfuel + ' TFuel / ' + tbill4x_gnote + ' gNOTE">' + tbill4x + '</td>'
                    + '<td class="text-end">' + tfuel + '</td>'
                    + '<td class="text-end">' + gnote + '</td>'
                    + '<td class="text-end">$' + tvl + '</td>'
                    + '<td class="text-end">$' + mtvl + '</td>'
                    + '<td class="text-end">' + reward + '</td>'
                    + '<td class="text-end">$' + parseFloat(reward * tbillRate).toFixed(4) + '</td></tr>');
                dailyTotal = parseFloat(dailyTotal) + parseFloat(reward);
                i++;
            });
            $("#dayTotal").html('<img height="20" src="/img/tbill.svg" />&nbsp;' + parseFloat(dailyTotal).toFixed(4) + '<br/>' + '$' + parseFloat(dailyTotal * tbillRate).toFixed(2));

            if (has3x === true) {
                $('#rewardsTable td:nth-child(6)').removeClass("hiddenColumn");
                $('#rewardsTable th:nth-child(6)').removeClass("hiddenColumn");
            }
            if (has4x === true) {
                $('#rewardsTable td:nth-child(7)').removeClass("hiddenColumn");
                $('#rewardsTable th:nth-child(7)').removeClass("hiddenColumn");
            }

            fetchNFTforWallet();
        });

    }

    if (wallet != "") {
        $.getJSON("api/my-overview/" + wallet, function (data) {

            if (data['data']['currIlTFuel'] === undefined) {
                $.getJSON("api/my-overview-zeroday/" + wallet, function (data) {

                    var currIl = parseFloat(data['data']['currIlUsd']).toFixed(2);
                    var currIlTfuel = parseFloat(data['data']['currIlTFuel']).toFixed(2);
                    var extraIl = parseFloat(data['data']['extraUsd']).toFixed(2);
                    var extraIlTfuel = parseFloat(data['data']['extraTFuel']).toFixed(2);
                    var currHwm = parseFloat(data['data']['hwm']).toFixed(2);
                    refreshILInfo(data['data']['updateTime'], currIl, currIlTfuel, extraIl, extraIlTfuel, currHwm);
                });
            } else {
                //var realIl = parseFloat(data['data']['realIlUsd']).toFixed(2);
                var currIl = parseFloat(data['data']['currIlUsd']).toFixed(2);
                //var realIlTfuel = parseFloat(data['data']['realIlTFuel']).toFixed(2);
                var currIlTfuel = parseFloat(data['data']['currIlTFuel']).toFixed(2);
                var extraIl = parseFloat(data['data']['extraUsd']).toFixed(2);
                var extraIlTfuel = parseFloat(data['data']['extraTFuel']).toFixed(2);
                var currHwm = parseFloat(data['data']['hwm']).toFixed(2);
                refreshILInfo(data['data']['updateTime'], currIl, currIlTfuel, extraIl, extraIlTfuel, currHwm);

            }


            /* moved to refreshILInfo()
            var updateTime = data['data']['updateTime'];
            //$('#realIl').html('$' + realIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + realIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
            $('#currIl').html('$' + currIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + currIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
            $('#extraIl').html('$' + extraIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + extraIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
            $('#currHwm').html(currHwm.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })+ ' %');

            var batch = data['data']['batch'];
            var snapIl = parseFloat(data['data']['snapIl']).toFixed(2);
            var snapIlUsd = parseFloat(data['data']['snapIlUsd']).toFixed(2);
            var airdroppeedTime = batch ? (new Date(new Date('2022-07-22T17:00:00.000Z').getTime() + (batch - 1) * 10 * 60 * 1000)).toISOString().replace('T', ' ').slice(0, 16) + ' UTC' : 'n/a';

            //$('#ILToBeDropped').html('$' + snapIlUsd.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' (' + snapIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL)<br>@ ' + airdroppeedTime);

            $('#currIlPopover').attr('data-bs-content', 'Last refresh (UTC):<br>' + updateTime + '<br/>Eligible for IL if 90%+ of your HWM remains in LP');
            
            // refresh the currIlPopover popover
            const popoverIL = document.querySelector('#currIlPopover');
            new bootstrap.Popover(popoverIL, { html: true });
            */


            // Projected Amount
            var projectedAmount = parseFloat(data['data']['tbillsProjected']).toFixed(0);
            var timeUntilMint = parseFloat(data['data']['daysLeft']).toFixed(0);
            $('#timeUntilMint').html(timeUntilMint.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + ' (projected ' + projectedAmount.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + ' TBILL)');


        });

    }

    if (wallet != "") {
        var datesDaily = [];
        var datesDailySum = [];
        var rewardsSum = 0;
        $.getJSON("api/dailyRewards/" + wallet, function (data) {
            let has3x = false;
            let has4x = false;
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

                    if (parseFloat(val[8]) !== 0 && has4x !== true) {
                        has4x = true;
                    }
                    if (parseFloat(val[7]) !== 0 && has3x !== true) {
                        has3x = true;
                    }
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
            $("#totalTbill").html('<img height="20" src="/img/tbill.svg" />&nbsp;' + parseFloat(totalRewards).toFixed(4) + '<br/>' + '$' + parseFloat(totalRewardsUsd).toFixed(2));
            $("#daysInLP").html(i - 1);

            showDailyChart(datesDaily);
            showDailySumChart(datesDailySum);
            showTVLChart(dataTVL, dataTbill, dataTfuel);

            //reverse the order to new array
            rewards.slice().reverse().forEach(function (x) {
                rewardsReverse.push(x);
            });

            for (let y = 0; y < rewardsReverse.length; y++) {
                //two years
                if (y > 730) {
                    break;
                }
                //console.log(rewardsReverse[y]);
                $('#dailyRewardsTable > tbody:last-child').append('<tr>'
                    + '<td>' + rewardsReverse[y].date + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].tbill1x + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].tbill125x + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].tbill15x + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].tbill2x + '</td>'
                    + '<td class="text-end hiddenColumn">' + rewardsReverse[y].tbill3x + '</td>'
                    + '<td class="text-end hiddenColumn">' + rewardsReverse[y].tbill4x + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].tfuel + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].gnote + '</td>'
                    + '<td class="text-end">$' + rewardsReverse[y].tvl + '</td>'
                    + '<td class="text-end">$' + rewardsReverse[y].mtvl + '</td>'
                    + '<td class="text-end">' + rewardsReverse[y].reward + '</td>'
                    + '<td class="text-end">$' + rewardsReverse[y].rewardUSD + '</td>'
                    + '</tr>');

            }

            if (has3x === true) {
                $('#dailyRewardsTable td:nth-child(6)').removeClass("hiddenColumn");
                $('#dailyRewardsTable th:nth-child(6)').removeClass("hiddenColumn");
            }
            if (has4x === true) {
                $('#dailyRewardsTable td:nth-child(7)').removeClass("hiddenColumn");
                $('#dailyRewardsTable th:nth-child(7)').removeClass("hiddenColumn");
            }
        });
    }
    if (wallet != "") {

    }
    if (wallet != "") {
        $.getJSON("api/getMyWalletLpStats/" + wallet, function (data) {
            var univ2Hist = [];
            var univ2HistGnote = [];

            $.each(data, function (key, val) {
                if (val.lpName == 'tfuel') {
                    var position = val.Position;
                    var positionTotal = val.PositionTotal;
                    var univ2 = val.Univ2.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 4 });
                    var univ2Total = val.Univ2Total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    var myPct = val.MyPct.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 5 });;

                    $("#lpPosition").html(position + ' / ' + positionTotal);
                    $("#univ2").html(univ2 + ' / ' + univ2Total);

                    $("#lpPct").html(myPct + ' %');


                    $.each(val.Univ2Hist, function (key, val) {
                        //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
                        var innerArr = [val["Item1"], val["Item2"]];
                        univ2Hist.push(innerArr);

                    });
                }

                if (val.lpName == 'gnote') {
                    var position = val.Position;
                    var positionTotal = val.PositionTotal;
                    var univ2 = val.Univ2.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 5 });
                    var univ2Total = val.Univ2Total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 3 });
                    var myPct = val.MyPct.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 5 });;

                    $("#lpPositionGnote").html(position + ' / ' + positionTotal);
                    $("#univ2Gnote").html(univ2 + ' / ' + univ2Total);
                    //$("#univ2").html(univ2 + ' / ' + univ2Total);
                    $("#lpPctGnote").html(myPct + ' %');


                    $.each(val.Univ2Hist, function (key, val) {
                        //var innerArr = [val[0], parseFloat(val[1]).toFixed(4)];
                        var innerArr = [val["Item1"], val["Item2"]];
                        univ2HistGnote.push(innerArr);

                    });
                }

            });

            showUniv2Chart(univ2Hist, univ2HistGnote);


        });
    }

    if (wallet != "") {
        priceTdrop = 0;
        priceTheta = 0;
        $.getJSON("api/getPriceAll", function (data) {
            $.each(data['body'], function (key, val) {
                if (val['_id'] === 'TDROP') {
                    priceTdrop = val['price'];
                }

                if (val['_id'] === 'THETA') {
                    priceTheta = val['price'];
                }

            });

            //get TDrop  balance
            $.getJSON("api/balanceTdrop/" + wallet, function (data) {
                var balance = data.balance;
                var bal = (parseFloat(balance) / 1000000000000000000).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                var balUSD = ((parseFloat(balance) / 1000000000000000000) * priceTdrop).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                
                if (parseFloat(data.balance) > 0) {
                    $("#tdropBlock").show();
                    $("#tdropBalance").html('<img height="20" src="/img/tdrop_flat.png" />&nbsp;' + bal + '<br/>$' + balUSD);
                    updateTotalBalance((parseFloat(balance) / 1000000000000000000) * priceTdrop);
                }
                if (parseFloat(data.staked) > 0) {
                    
                    var staked = (parseFloat(data.staked)).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                    var stakedUSD = ((parseFloat(data.staked)) * priceTdrop).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

                    $("#tdropStakeBlock").show();
                    $("#tdropStakeBalance").html('<img height="20" src="/img/tdrop_flat.png" />&nbsp;' + staked + '<br/>$' + stakedUSD);
                    updateTotalBalance((parseFloat(data.staked)) * priceTdrop);
                }
                
            });

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
                var stakeThetaUSD = (parseFloat(json.ThetaStake) * priceTheta).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                var balanceThetaUSD = (parseFloat(json.Theta) * priceTheta).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

                updateTotalBalance(parseFloat(json.TFuel) * tfuelRate);
                updateTotalBalance(parseFloat(json.TBill) * tbillRate);
                updateTotalBalance(parseFloat(json.TFuelStake) * tfuelRate);
                updateTotalBalance(parseFloat(json.ThetaStake) * priceTheta);
                updateTotalBalance(parseFloat(json.Theta) * priceTheta);
                //console.log(balanceTFuel.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }));
                //console.log(json.TFuel.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }));

                $("#tbillBalance").html('<img height="20" src="/img/tbill.svg" />&nbsp;' + balanceTBill + '<br/>' + '$' + balanceTBillUSD);
                $("#tfuelBalance").html('<img height="20" src="/img/tfuel.svg" />&nbsp;' + balanceTFuel + '<br/>' + '$' + balanceTFuelUSD);
                if (parseFloat(json.Theta) > 0) {
                    $("#thetaBlock").show();
                    $("#thetaBalance").html('<img height="20" src="img/theta.png" />&nbsp;' + balanceTheta + '<br/>$' + balanceThetaUSD);
                }
                if (parseFloat(json.ThetaStake) > 0) {
                    $("#thetaStakeBlock").show();
                    $("#thetaStake").html('<img height="20" src="img/theta.png" />&nbsp;' + stakeTheta + '<br/>$' + stakeThetaUSD);
                }
                if (parseFloat(json.TFuelStake) > 0) {
                    $("#tfuelStakeBlock").show();
                    $("#tfuelStake").html('<img height="20" src="/img/tfuel.svg" />&nbsp;' + stakeTFuel + '<br/>' + '$' + stakeTFuelUSD);
                }
                
            });

        });
    }

    //get gNOTE balance
    if (wallet != "") {
        $.getJSON("api/balanceGnote/" + wallet, function (data) {
            //var json = JSON.parse(data);
            var balance = data.balance;
            var bal = (parseFloat(balance) / 1000000000000000000).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 5 });
            var balanceGnoteUSD = (parseFloat(balance) / 1000000000000000000 * gnoteRate).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            $("#gNoteBalance").html('<img height="20" src="/img/gnote.png" />&nbsp;' + bal + '<br/>' + '$' + balanceGnoteUSD);
            updateTotalBalance(parseFloat(balance) / 1000000000000000000 * gnoteRate);
        });
    }

}

function updateTotalBalance(val) {
    totalBalanceUSD += val;
    $("#totalBalance").html('$' + totalBalanceUSD.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
}

function refreshILInfo(updateTime, currIl, currIlTfuel, extraIl, extraIlTfuel, currHwm) {

    //$('#realIl').html('$' + realIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + realIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
    $('#currIl').html('$' + currIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + currIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
    $('#extraIl').html('$' + extraIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '<br>' + extraIlTfuel.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL');
    $('#currHwm').html(currHwm.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' %');

    //var batch = data['data']['batch'];
    //var snapIl = parseFloat(data['data']['snapIl']).toFixed(2);
    //var snapIlUsd = parseFloat(data['data']['snapIlUsd']).toFixed(2);
    //var airdroppeedTime = batch ? (new Date(new Date('2022-07-22T17:00:00.000Z').getTime() + (batch - 1) * 10 * 60 * 1000)).toISOString().replace('T', ' ').slice(0, 16) + ' UTC' : 'n/a';

    //$('#ILToBeDropped').html('$' + snapIlUsd.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' (' + snapIl.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TFUEL)<br>@ ' + airdroppeedTime);

    //$('#currIlPopover').attr('data-bs-content', 'Last refresh (UTC):<br>' + updateTime + '<br/>Eligible for IL if 90%+ of your HWM remains in LP');
    $('#currIlPopover').attr('data-bs-content', 'Data from Jan 16th Snapshot. Eligible for IL if 90%+ of your HWM remains in LP');


    // refresh the currIlPopover popover
    const popoverIL = document.querySelector('#currIlPopover');
    new bootstrap.Popover(popoverIL, { html: true });
}

function clearMyWalletData() {
    totalBalanceUSD = 0;
    nft125xlevel = 0;
    nft15xlevel = 0;
    nft2xlevel = 0;
    nft3xlevel = 0;
    nft4xlevel = 0;
    nft125xlevelGnote = 0;
    nft15xlevelGnote = 0;
    nft2xlevelGnote = 0;
    nft3xlevelGnote = 0;
    nft4xlevelGnote = 0;
    $("#thetaBalance").html('');
    $("#tfuelBalance").html('');
    $("#thetaStake").html('');
    $("#tfuelStakeBlock").hide();
    $("#tfuelStake").html('');
    $("#thetaBlock").hide();
    $("#tdropBlock").hide();
    $("#tbillBalance").html('');
    $("#thetaStakeBlock").hide();
    $("#tdropStakeBlock").hide();
    $("#gNoteBalance").html('');
    $("#totalBalance").html('');

    $("#dayTotal").html('');
    $("#totalTbill").html('');
    $("#tvl").html('');
    $("#activTbill").html('');
    $("#activeTfuel").html('');
    $("#gnoteTfuel").html('');
    $("#daysInLP").html('');

    $("#nft4xlabel").html(0 + ' / ' + 0);
    $("#nft3xlabel").html(0 + ' / ' + 0);
    $("#nft2xlabel").html(0 + ' / ' + 0);
    $("#nft15xlabel").html(0 + ' / ' + 0);
    $("#nft125xlabel").html(0 + ' / ' + 0);

    $("#nft4xlabelGnote").html(0 + ' / ' + 0);
    $("#nft3xlabelGnote").html(0 + ' / ' + 0);
    $("#nft2xlabelGnote").html(0 + ' / ' + 0);
    $("#nft15xlabelGnote").html(0 + ' / ' + 0);
    $("#nft125xlabelGnote").html(0 + ' / ' + 0);


    $('#progress4x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress3x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress2x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress15x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress125x').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress4xGnote').css('width', 0 + '%').attr('aria-valuenow', 0);
    $('#progress3xGnote').css('width', 0 + '%').attr('aria-valuenow', 0);
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
    var nft3xsum = 0;
    var nft4xsum = 0;

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
            } else if (Multiplier == "3x") {
                nft3xsum += parseFloat(TbillAmount);
            } else if (Multiplier == "4x") {
                nft4xsum += parseFloat(TbillAmount);
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

        if (nft4xsum !== 0) {
            $("#nft4xBlock").css({ display: "block" });
            $("#nft4xGnoteBlock").css({ display: "block" });
        }
        if (nft3xsum !== 0) {
            $("#nft3xBlock").css({ display: "block" });
            $("#nft3xGnoteBlock").css({ display: "block" });
        }

        $("#nft4xlabel").html(nft4xlevel + ' / ' + nft4xsum);
        $("#nft3xlabel").html(nft3xlevel + ' / ' + nft3xsum);
        $("#nft2xlabel").html(nft2xlevel + ' / ' + nft2xsum);
        $("#nft15xlabel").html(nft15xlevel + ' / ' + nft15xsum);
        $("#nft125xlabel").html(nft125xlevel + ' / ' + nft125xsum);

        $("#nft4xlabelGnote").html(nft4xlevelGnote + ' / ' + nft4xsum);
        $("#nft3xlabelGnote").html(nft3xlevelGnote + ' / ' + nft3xsum);
        $("#nft2xlabelGnote").html(nft2xlevelGnote + ' / ' + nft2xsum);
        $("#nft15xlabelGnote").html(nft15xlevelGnote + ' / ' + nft15xsum);
        $("#nft125xlabelGnote").html(nft125xlevelGnote + ' / ' + nft125xsum);

        var prct4x = parseFloat(parseFloat(nft4xlevel) / nft4xsum * 100).toFixed(1);
        var prct3x = parseFloat(parseFloat(nft3xlevel) / nft3xsum * 100).toFixed(1);
        var prct2x = parseFloat(parseFloat(nft2xlevel) / nft2xsum * 100).toFixed(1);
        var prct15x = parseFloat(parseFloat(nft15xlevel) / nft15xsum * 100).toFixed(1);
        var prct125x = parseFloat(parseFloat(nft125xlevel) / nft125xsum * 100).toFixed(1);

        $('#progress4x').css('width', prct4x + '%').attr('aria-valuenow', prct4x);
        $('#progress3x').css('width', prct3x + '%').attr('aria-valuenow', prct3x);
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
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            },
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

function showUniv2Chart(data, dataGnote) {

    var options = {
        series: [{
            name: 'Uni-v2 TFuel',
            type: 'column',
            data: data
        }
            //, {
            //name: 'Uni-v2 gNOTE',
            //type: 'line',
            //data: dataGnote
            //}
        ],
        chart: {
            height: 350,
            type: 'line',
        },
        stroke: {
            width: [2, 2]
        },
        title: {
            text: 'Uni-v2'
        },
        xaxis: {
            type: 'datetime'
        },
        yaxis: [{
            title: {
                text: 'uni-v2 TFuel',
            }, labels: {
                formatter: function (val) {
                    /*    return (val / 1).toFixed(2);*/
                    return val.toFixed(4);
                }
            },
            min: 0,

        }
            //, {
            //    opposite: true,
            //    title: {
            //        text: 'Uni-v2 gNOTE'
            //    }, labels: {
            //        formatter: function (val) {
            //            return (val).toFixed(5);
            //        }
            //    },
            //    //min: TbillSupplyMin * 0.98,
            //    //max: TbillSupplyMax * 1.02
            //}
        ],

        theme: {
            mode: 'dark'
        },
        tooltip: {
            enabled: true,
            shared: true,
            x: {
                format: 'dd/MM/yy'
            }
        }
        //,
        //colors: [ '#4ECDC4']
    };

    var chart = new ApexCharts(document.querySelector("#chartUniv2"), options);
    $("#chartUniv2").empty();
    chart.render();
}
