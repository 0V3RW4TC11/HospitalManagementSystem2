import { ResultsBox, Result } from './results-box.js';
import { SearchBox } from './search-box.js';
import { SearchProvider } from './search-provider.js';
import { SelectionBox } from './selection-box.js';

export class DoctorSpecsEditor {
    constructor(
        searchUrl,
        jsonDataId,
        formId,
        containerId)
    {
        this.$jsonData = $(`#${jsonDataId}`);
        this.$container = $(`#${containerId}`);

        $(() => {
            this.#initialize(searchUrl);
            this.#deserializeSpecs();
            $(document).on('click', (event) => this.onResultsUnfocus(event));
            $(`#${formId}`).on('submit', (event) => this.onFormSubmit(event));
        });
    }

    onSearch() {
        this.resultsBox.hide();
        this.searchBox.searchAsync()
            .then(response => this.onResponse(response))
            .catch(error => this.onError(error));
    }

    onResponse(response) {
        this.resultsBox.clear();

        if (!response)
            return;

        if (response.length > 0) {
            response.forEach(item => {
                let result = new Result(
                    item.name,
                    'dropdown-item',
                    () => {
                        this.onAddSelection(item);
                        this.resultsBox.hide();
                    });

                this.resultsBox.add(result);
            });
        }
        else {
            let result = new Result(
                'No results found',
                'dropdown-item text-muted user-select-none',
                null);
            this.resultsBox.add(result);
        }

        this.resultsBox.show();
    }

    onError(error) {
        console.error('Error fetching items:', error);
        let result = new Result(
            'An error occurred',
            'dropdown-item text-danger',
            null);

        this.resultsBox.clear();
        this.resultsBox.add(result);
        this.resultsBox.show();
    }

    onAddSelection(item) {
        this.selectionBox.add(item);
        this.searchBox.clear();
    }

    onResultsUnfocus(event) {
        if (!this.searchBox.$input.is(event.target) &&
            !this.resultsBox.$container.is(event.target) &&
            this.resultsBox.$container.has(event.target).length === 0)
        {
            this.searchBox.clear();
            this.resultsBox.hide();
        }
    }

    onFormSubmit(event) {
        event.preventDefault();

        let specs = [];
        this.selectionBox.$container.children().each(function () {
            specs.push({ id: this.id, name: this.innerText });
        });

        if (this.#validateForm(specs.length)) {
            this.$jsonData.val(JSON.stringify(specs));
            event.target.submit();
        }
    }

    #initialize(searchUrl) {
        let $searchGroup = $('<div>')
            .addClass('form-group')
            .appendTo(this.$container);

        $('<label>')
            .attr('for', 'searchInput')
            .addClass('control-label')
            .text('Specializations:')
            .appendTo($searchGroup);

        let $input = $('<input>')
            .attr('id', 'searchInput')
            .attr('type', 'text')
            .attr('placeholder', 'Search specializations...')
            .addClass('form-control')
            .appendTo($searchGroup);

        let $results = $('<div>')
            .addClass('dropdown-menu w-50')
            .css('max-height', '200px')
            .css('overflow-y', 'auto')
            .appendTo(this.$container);

        let $selectedGroup = $('<div>')
            .addClass('form-group')
            .appendTo(this.$container);

        $('<label>')
            .addClass('control-label')
            .attr('for', 'selectedSpecs')
            .text('Selected:')
            .appendTo($selectedGroup);

        let $selected = $('<ul>')
            .addClass('list-group')
            .attr('id', 'selectedSpecs')
            .appendTo($selectedGroup);

        let searchProvider = new SearchProvider(searchUrl);
        this.searchBox = new SearchBox($input, searchProvider);
        this.resultsBox = new ResultsBox($results);
        this.selectionBox = new SelectionBox($selected);

        $input.on('input', () => this.onSearch());
    }

    #deserializeSpecs() {
        const jsonData = this.$jsonData.val();
        if (!jsonData)
            return;

        const specs = JSON.parse(this.$jsonData.val());
        specs.forEach(s => this.selectionBox.add(s));
    }

    #validateForm(count) {
        if (count <= 0) {
            alert("Please select at least one specialization.");
            return false;
        }

        return true;
    }
}