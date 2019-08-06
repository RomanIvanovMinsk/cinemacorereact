export default class BaseClient{
    protected transformOptions(options:RequestInit): Promise<RequestInit>{
        return Promise.resolve(options);
    }
}