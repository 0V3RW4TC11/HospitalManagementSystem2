export class SelectionBox {
    constructor($container)
    {
        this.$container = $container;
    }

    add(item) {
        if (!this.#isExisting(item.id)) {
            const $li = $('<li>')
                .attr('id', item.id)
                .addClass('list-group-item d-flex justify-content-between align-items-center')
                .text(item.name)

            const $removeBtn = $('<button>')
                .attr('type', 'button')
                .addClass('btn-close')
                .attr('aria-label', 'Close')
                .on('click', () => $(`#${item.id}`).remove());

            $li.append($removeBtn);

            this.$container.append($li);
        }
    }

    reset() {
        this.$container.empty();
    }

    #isExisting(itemId) {
        return this.$container.find(`#${itemId}`).length > 0;
    }
}