import { useAuthStore } from "@/stores";
import axios from "axios";


export const httpClient = {
    get: request('GET'),
    post: request('POST'),
    put: request('PUT'),
    delete: request('DELETE')
};

function request(method) {
    return (url, body) => {
        const headers = authHeader();
        if (body) {
            headers['Content-Type'] = 'application/json';
            body = JSON.stringify(body);
        }

        return axios({
            method: method,
            url: url,
            headers: headers,
            data: body
        }).then((response) => response.data)
        .catch((err) => handleError(err));
    }
}

// helper functions

function authHeader() {

    const { loginInfo } = useAuthStore();
    const isLoggedIn = !!loginInfo?.accessToken;
    if (isLoggedIn) {
        return { Authorization: `Bearer ${loginInfo.accessToken}` };
    } else {
        return {};
    }
}

async function handleError(err) {
    const response = err.response;
    if ([401, 403].includes(response.status))
    {
        const { loginInfo, logout, refreshToken } = useAuthStore();
        if (loginInfo?.refreshToken) {
            await refreshToken();
            response.config.headers['Authorization'] = `Bearer ${useAuthStore().loginInfo.accessToken}`;
            return axios(response.config);
        } else {
            logout();
        }
    } else {
        // get error message from body or default to response status
        const error = (data && data.message) || response.status;
        return Promise.reject(error);
    }
}

