$(window).on('load', function () {
    if (!sessionStorage.getItem('shown-modal-newDomain')) {
        $('#newDomainModal').modal('show');
        sessionStorage.setItem('shown-modal-newDomain', 'true');
    }
});