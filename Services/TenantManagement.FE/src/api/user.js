export const userApi = (baseUrl) => {
    return {
        authenticate: baseUrl + '/users/authenticate',
        refreshToken: baseUrl + '/users/refresh-token',
        test: baseUrl + '/users'
    }
};