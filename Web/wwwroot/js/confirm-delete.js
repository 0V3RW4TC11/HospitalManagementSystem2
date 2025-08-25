function confirmDelete(id, isShown) {
    var deleteSpan = 'deleteSpan_' + id;
    var confirmDeleteSpan = 'confirmDeleteSpan_' + id;

    if (isShown) {
        $('.' + deleteSpan).hide();
        $('.' + confirmDeleteSpan).show();
    }
    else {
        $('.' + deleteSpan).show();
        $('.' + confirmDeleteSpan).hide();
    }
}