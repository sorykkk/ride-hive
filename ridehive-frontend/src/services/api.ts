// API configuration and endpoints
export class ApiConfig {
    // env is set in vite.config.ts to be dependent on the mode vite runs
    static readonly BASE_URL = import.meta.env.VITE_API_BASE_URL;

    // API Endpoints
    static readonly ENDPOINTS = {
        // define here all endpoints
        // example:
        /*
        WEATHER: {
            ANALYZE: '/WeatherForecast/analyze',
            GET_FORECAST: '/WeatherForecast'
        }
        */
    } as const;

    // Build full URL
    static getUrl(endpoint: string): string {
        return `${this.BASE_URL}${endpoint}`;
    }

    // Environment info
    static get isDevelopment(): boolean {
        return import.meta.env.DEV;
    }

    static get isProduction(): boolean {
        return import.meta.env.PROD;
    }

    static get appVersion(): string {
        return import.meta.env.VITE_APP_VERSION || '1.0.0';
    }
}

// Type-safe API Client
export class ApiClient {
    static async post<T>(endpoint: string, data: any): Promise<T> {
        try {
            console.log(`Making Post request to: ${ApiConfig.getUrl(endpoint)}`);

            const response = await fetch(ApiConfig.getUrl(endpoint), {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });

            if(!response.ok) {
                const errorText = await response.text();
                throw new Error(`Api Error ${response.status} ${response.statusText} - ${errorText}`);
            }

            return response.json();
        } catch(error) {
            console.error('API Request failed: ', error);
            if(error instanceof TypeError && error.message.includes('fetch')) {
                throw new Error('Cannot connect to server. Make sure the backend is running on ' + ApiConfig.BASE_URL);
            }
            throw error;
        }
    }

    static async get<T>(endpoint: string): Promise<T> {
        try {
            console.log(`Making GET request to: ${ApiConfig.getUrl(endpoint)}`);

            const response = await fetch(ApiConfig.getUrl(endpoint));

            if(!response.ok) {
                const errorText = await response.text();
                throw new Error(`API Error: ${response.status} ${response.statusText} - ${errorText}`);
            }

            return response.json();
        } catch(error) {
            console.error('API Request failed: ', error);
            if(error instanceof TypeError && error.message.includes('fetch')) {
                throw new Error('Cannot connect to server. Make sure the backend is running on ' + ApiConfig.BASE_URL);
            }
            throw error;
        }
    }
}
