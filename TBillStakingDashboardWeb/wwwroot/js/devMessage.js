$(window).on('load', function () {
    if (!sessionStorage.getItem('shown-modal')) {
        $('#warningModal').modal('show');
        sessionStorage.setItem('shown-modal', 'true');
    }
});