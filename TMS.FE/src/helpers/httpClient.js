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
    return (url, body, opts) => {
        const headers = authHeader();

        if (body) {
            headers['Content-Type'] = 'application/json';
            body = JSON.stringify(body);
        }

        return axios({
            method: method,
            url: url,
            headers: headers,
            data: body,
            withCredentials: true,
            ...opts
        }).then((response) => response.data)
        .catch((err) => handleError(err));
    }
}

// helper functions

function authHeader() {
    return {};
}

async function handleError(err) {
    if (err.code === "ERR_NETWORK") {
        return Promise.reject("Không thể kết nối đến Server. Vui lòng thử lại sau.");
    };

    const response = err.response;
    if ([401, 403].includes(response.status))
    {
        const { logout, refreshToken } = useAuthStore();
        const refreshTokenResult = await refreshToken();
        if (refreshTokenResult) {
            var resendResult = await axios(response.config);
            return resendResult.data;
        } else {
            logout();
        }
    } else {
        // get error message from body or default to response status
        const error = err.message;;
        return Promise.reject(error);
    }
}

