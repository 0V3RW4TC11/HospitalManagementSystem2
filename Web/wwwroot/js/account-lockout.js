function toggleAccountLockout(accountId, isLockedOut) {
    $.ajax({
        url: '/Account/SetAccountLockout',
        type: 'POST',
        data: { id: accountId, enabled: !isLockedOut },
        headers: {
            // Add CSRF token for security
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function () {
            // Toggle button text and style
            const button = $(`button[onclick*="${accountId}"]`);
            if (isLockedOut) {
                button.text('Disable').removeClass('btn-success').addClass('btn-danger');
                button.attr('onclick', `toggleAccountLockout('${accountId}', false)`);
            } else {
                button.text('Enable').removeClass('btn-danger').addClass('btn-success');
                button.attr('onclick', `toggleAccountLockout('${accountId}', true)`);
            }
        },
        error: function (xhr) {
            // Handle error (e.g., show message from ModelState)
            let errorMessage = 'An error occurred.';
            if (xhr.responseJSON && xhr.responseJSON['']) {
                errorMessage = xhr.responseJSON[''][0].errors[0].errorMessage;
            }
            alert(errorMessage);
        }
    });
}