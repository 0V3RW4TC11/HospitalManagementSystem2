export class ResultsBox {
    constructor($container)
    {
        this.$container = $container;
    }

    add(result) {
        if (!(result instanceof Result)) {
            throw new Error("result must be an instance of Result.");
        }

        const $a = $('<a href=#>')
            .addClass(result.className)
            .text(result.name);

        if (result.onClick) {
            $a.on('click', result.onClick);
        }

        this.$container.append($a);
    }

    clear() {
        this.$container.empty();
    }

    hide() {
        this.$container.removeClass('show');
    }

    show() {
        this.$container.addClass('show');
    }
}

export class Result {
    constructor(name, className, onClick) {
        if (onClick && typeof onClick !== 'function') {
            throw new Error("onClick must be a function.");
        }

        this.name = name;
        this.className = className;
        this.onClick = onClick;
    }
}