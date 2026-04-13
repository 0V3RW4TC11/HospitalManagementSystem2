import { SearchProvider } from './search-provider.js';

export class SearchBox {
    timeOut;

    constructor(
        $input,
        searchProvider,
        searchDelayMs = 300)
    {
        if (!(searchProvider instanceof SearchProvider)) {
            throw new Error("searchProvider must be an instance of SearchProvider.");
        }

        this.$input = $input;
        this.searchProvider = searchProvider;
        this.searchDelayMs = searchDelayMs;
    }

    async searchAsync() {
        // debounce the search to avoid too many requests
        clearTimeout(this.timeOut);
        await new Promise(resolve => this.timeOut = setTimeout(resolve, this.searchDelayMs));

        // perform the search and return the result
        return await this.searchProvider.searchAsync(this.$input.val().trim());
    }

    clear() {
        this.$input.val('');
    }
}