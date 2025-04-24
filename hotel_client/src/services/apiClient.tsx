import axios from "axios";
import { enApiType } from "../module/enApiType";
import { generalMessage } from "../util/generalPrint";

interface ApiClientProps {
    enType: enApiType;
    endPoint: string;
    parameters?: any;
    jwtValue?: string;
    jwtRefresh?: string;
    isFormData?: boolean;
    isRequireAuth?: boolean;
    tryNumber?: number;
}

export default async function apiClient({
    enType,
    parameters,
    endPoint,
    jwtValue = '',
    jwtRefresh = '',
    isFormData = false,
    isRequireAuth = false,
    tryNumber = 0,
}: ApiClientProps) {
    const fullUrl = import.meta.env.VITE_BASE_URL + endPoint;
    const token = tryNumber === 1 ? jwtValue : jwtRefresh;
    const headers = handleHeaders(isRequireAuth, isFormData, token);

 
    try {
        const response = await axios({
            url: fullUrl,
            method: enType.toString(),
            data: parameters,
            headers: headers,
        });

         generalMessage(`this shown the token from user ${token}`)
        return response;
    } catch (error) {
        if (error?.response?.status === 401 && tryNumber === 1) {
            // Retry with refresh token
            return apiClient({
                enType,
                parameters,
                endPoint,
                jwtValue,
                jwtRefresh,
                isFormData,
                isRequireAuth,
                tryNumber: 2, // Retry with refresh token
            });
        } else {
            // Return the error for handling by the caller
            throw error;
        }
    }
}

function handleHeaders(isRequireAuth: boolean, isFormData: boolean, jwtValue: string) {
    const headers: Record<string, string> = {
        "Content-Type": isFormData ? "multipart/form-data" : "application/json",
    };

    if (isRequireAuth && jwtValue) {
        headers["Authorization"] = `Bearer ${jwtValue}`;
    }

    return headers;
}