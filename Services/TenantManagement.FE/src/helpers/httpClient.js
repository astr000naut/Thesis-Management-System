import { useAuthStore } from "@/stores";
import axios from "axios";

//axios.defaults.headers.common['Client-Domain'] = window.location.hostname;

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
    if (err.code === "ERR_NETWORK") {
        return Promise.reject("Không thể kết nối đến Server. Vui lòng thử lại sau.");
    };

    const response = err.response;
    if ([401, 403].includes(response.status))
    {
        const { loginInfo, logout, refreshToken } = useAuthStore();
        if (loginInfo?.refreshToken) {
            await refreshToken();
            const newToken = useAuthStore().loginInfo?.accessToken;
            if (newToken) {
                response.config.headers['Authorization'] = `Bearer ${newToken}`;
                return axios(response.config);
            } else {
                logout();
            }
        } else {
            logout();
        }
    } else {
        // get error message from body or default to response status
        const error = err.message;;
        return Promise.reject(error);
    }
}

