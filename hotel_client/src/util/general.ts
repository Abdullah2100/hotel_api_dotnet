import { enStatsu } from "../module/enStatsu";

export class General{

    static convetNumberToEnStatus(statusNumber:number){
        switch(statusNumber){
            case 0:return enStatsu.Available;
            case 1:return enStatsu.Booked;
            default :return enStatsu.UnderMaintenance
        }
    }

    static handlingEnStatusColor(statusNumber:number){
         switch(statusNumber){
            case 0:return 'bg-green-500';
            case 1:return 'bg-red-500';
            default :return 'bg-orange-500'
        }
    }
}