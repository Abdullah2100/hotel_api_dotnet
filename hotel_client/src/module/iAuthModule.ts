import { Guid } from "guid-typescript";

export interface IAuthModule {
    userId :Guid|undefined;
    name: string
    email: string;
    phone: string;
    address: string;
    username: string;
    password: string;
    brithDay: string ;
    imagePath?:File|string|undefined;
}