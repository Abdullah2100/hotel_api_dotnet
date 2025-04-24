import { Guid } from "guid-typescript";
import { iImageHolder } from "./IImageHolder";
import { enStatsu } from "./enStatsu";
import { IRoomType } from "./iRoomType";
import { IUserModule } from "./iUserModule";

export interface IRoomModule {
    roomId?: Guid | undefined;
    status: enStatsu |number;
    pricePerNight: number;
    capacity: number;
    roomtypeid?: Guid | undefined;
    beglongTo?: Guid | undefined;
    bedNumber: number;
    createdAt?: Date | undefined;
    isBlock?: boolean | undefined;
    roomData?: IRoomType | undefined;
    user?: IUserModule | undefined;
    images?: iImageHolder[] | undefined;
    isDeleted?: boolean | null;


}