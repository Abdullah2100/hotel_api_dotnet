import { Guid } from "guid-typescript"

export interface IRoomType {
    roomTypeID?: Guid|null,
    roomTypeName: string,
    createdBy?:Guid|null,
    createdAt?:Date |null;
    imagePath?:string |null;
    isDeleted?:boolean |null;
}