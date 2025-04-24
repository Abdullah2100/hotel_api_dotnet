import { Guid } from "guid-typescript";
import { IPersonModule } from "./iPersonModule";

export interface IUserModule {
    userId: Guid,
    personID: Guid,
    brithDay: string
    isVip: boolean,
    userName: string,
    isDeleted:boolean,
    personData: IPersonModule,
    imagePath:string|null
}