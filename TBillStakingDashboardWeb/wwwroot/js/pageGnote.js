$(document).ready(function () {
    //refreshChartsGnote(31);

    //$('input[type=radio][name=optionsPeriod]').change(function () {
    //    refreshChartsGnote(this.value);
    //});

    $.getJSON("api/gnoteStats", function (data) {
        let CurrentGnote_Tfuel = (parseFloat(data.currentGnote_Tfuel)).toLocaleString(undefined, { minimumFractionDigits: 4, maximumFractionDigits: 4 });
        $('#CurrentGnote_Tfuel').html(CurrentGnote_Tfuel);
        let CurrentTfuel_Gnote = (parseFloat(data.currentTfuel_Gnote)).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        $('#CurrentTfuel_Gnote').html(CurrentTfuel_Gnote);

    });
});


