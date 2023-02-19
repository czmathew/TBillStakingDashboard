$(window).on('load', function () {
    if (!sessionStorage.getItem('shown-modal-tbillPause')) {
        $('#tbillPauseModal').modal('show');
        sessionStorage.setItem('shown-modal-tbillPause', 'true');
    }
});