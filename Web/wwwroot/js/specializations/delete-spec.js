async function deleteSpec(specId) {
    if (confirm('Are you sure?')) {
        $.ajax({
            url: '/Specializations/Delete',
            type: 'POST',
            data: { id: specId }
        })
        .done(function () {
            window.location.reload();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            let errorMessage = jqXHR.responseJSON?.message || jqXHR.responseText || errorThrown || 'Unknown error occurred';
            console.error('Error:', errorMessage);
            alert(errorMessage);
        });
    }
}