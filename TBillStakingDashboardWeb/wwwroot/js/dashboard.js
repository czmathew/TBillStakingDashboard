$(document).ready(function () {

    fetchData();

    $("form").on("submit", function (event) {

        fetchData();

        event.preventDefault();
    });
});

function fetchData() {
    $.getJSON("api/rates", function (data) {
        var rate = "";
        $.each(data['rates'], function (key, val) {
            if (val['pair'] == 'tbill_usd') {
                rate = val['rate'];
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
        $("#targetRate").html(parseFloat(targetRate).toFixed(4));
        $("#rebaseRate").html(parseFloat(rebaseRate).toFixed(4));
        $("#noRebaseRangeTop").html(parseFloat(noRebaseRangeTop).toFixed(4));
        $("#noRebaseRangeBottom").html(parseFloat(noRebaseRangeBottom).toFixed(4));

    });

    var wallet = $("#walletAddress").val();
    if (wallet != "") {
        $.getJSON("api/rewards/" + wallet, function (data) {
            let i = 1;
            $('#rewardsTable > tbody:last-child').empty();
            $.each(data['data'], function (key, val) {
                //get first 10 only
                if (i > 10) {
                    return false;
                }
                var tvl = parseFloat(val['tv']).toFixed(2);
                var mtvl = parseFloat(val['mtv']).toFixed(2);
                var reward = parseFloat(val['reward']).toFixed(4);
                var time = getTimeFromTimestamp(val['end_time']);
                var tbill1x = parseFloat(val['tbill1x']).toFixed(2);
                var tbill15x = parseFloat(val['tbill15x']).toFixed(2);
                var tbill2x = parseFloat(val['tbill2x']).toFixed(2);
                var tfuel = parseFloat(val['tfuel']).toFixed(2);

                $('#rewardsTable > tbody:last-child').append('<tr><td>' + time + '</td><td>' + tbill1x + '</td><td>' + tbill15x + '</td><td>' + tbill2x + '</td><td>' + tfuel + '</td><td>' + tvl + '</td><td>' + mtvl + '</td><td>' + reward + '</td></tr>');
                i++;
            });
        });
    }

    if (wallet != "") {
        $.getJSON("api/dailyRewards/" + wallet, function (data) {
            let i = 1;
            let rewards = [];
            let rewardsReverse = [];
            $('#dailyRewardsTable > tbody:last-child').empty();
            $.each(data['vals'], function (key, val) {
                //ignore hte header (first record)
                if (i > 1) {
                    rewards.push({
                        date: val[0], tvl: val[1], mtvl: val[2], tbill1x: val[3], tbill15x: val[4]
                        , tbill2x: val[5], tfuel: val[6], reward: val[7]
                    });
                }
                i++;
            });

            //reverse the order to new array
            rewards.slice().reverse().forEach(function (x) {
                rewardsReverse.push(x);
            });

            for (let y = 0; y < rewardsReverse.length; y++) {
                //only first 10
                if (y > 10) {
                    break;
                }
                //console.log(rewardsReverse[y]);
                $('#dailyRewardsTable > tbody:last-child').append('<tr><td>' + rewardsReverse[y].date + '</td><td>' + rewardsReverse[y].tbill1x + '</td><td>' + rewardsReverse[y].tbill15x + '</td><td>' + rewardsReverse[y].tbill2x + '</td><td>' + rewardsReverse[y].tfuel + '</td><td>' + rewardsReverse[y].tvl + '</td><td>' + rewardsReverse[y].mtvl + '</td><td>' + rewardsReverse[y].reward + '</td></tr>');
            }
        });
    }

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