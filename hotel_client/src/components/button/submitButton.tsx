import { Button, CircularProgress } from "@mui/material";
import { enStatus } from "../../module/enState";

interface submitButtonProps {
    placeHolder: string;
    onSubmit: () => Promise<void>;
    buttonStatus?: enStatus | undefined;
    style?: string | undefined
    textstyle?:string |undefined
}

const SubmitButton = (
    {
        onSubmit,
        placeHolder,
        buttonStatus = enStatus.none,
        style = undefined,
        textstyle
    }: submitButtonProps) => {
    return (<button
        onClick={(e)=>{
            e.preventDefault();
            buttonStatus !== enStatus.loading ?onSubmit() : undefined
        }}
       // color="primary"
        className={style}
    >
        {
            buttonStatus === enStatus.loading ?
                <CircularProgress sx={{ color: 'white' }} size={10} className="mt-1" />
                : 
               <h3 className={textstyle}>

                  {placeHolder} 
               </h3>
        }
    </button>)

}
export default SubmitButton;