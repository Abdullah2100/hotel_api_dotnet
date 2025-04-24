

import { Guid } from "guid-typescript";

export interface iImageHolder {
    id?: Guid | undefined;
    belongTo?: Guid | undefined;
    path?: string | undefined;

    isDeleted?: boolean | undefined;
    isThumnail?: boolean | undefined;
    data?: File | undefined;
}

// 