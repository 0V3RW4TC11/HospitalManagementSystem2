export class SearchProvider {
    constructor(url)
    {
        this.url = url;
    }

    async searchAsync(query) {
        if (query.length === 0) {
            return null;
        }

        const requestUrl = `${this.url}${encodeURIComponent(query)}`;
        const response = await fetch(requestUrl);
        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.status}`);
        }

        return response.json();
    }
}