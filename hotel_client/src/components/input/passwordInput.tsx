import { EyeIcon, EyeSlashIcon } from "@heroicons/react/16/solid";
import { FocusEventHandler, forwardRef, useState } from "react";

interface TextInputProps {
    keyType: string;
    placeHolder: string;
    isDisabled?: boolean;
    isMultipleLine?: boolean;
    maxLength?: number;
    onInput: (value: any, key: string) => void;
    style?: string;
    type?: React.HTMLInputTypeAttribute;
    value: any;
    isRequire?: boolean | undefined;
    onFoucseInpur?: FocusEventHandler<HTMLInputElement | HTMLTextAreaElement> | undefined
    isHasTitle?: boolean | undefined
    canShowOrHidePassowrd?: boolean | undefined
}

export const PasswordInput =
    ({
        keyType = '',
        placeHolder,
        isDisabled = false,
        maxLength,
        onInput,
        style = '',

        value,
        isRequire = false,
        onFoucseInpur = undefined,
        isHasTitle = true
        , canShowOrHidePassowrd = undefined,

    }: TextInputProps
    ) => {
        const [passwordType, setType] = useState<React.HTMLInputTypeAttribute>('password')
        const generalStyle = `px-2 ${isDisabled?'':'border-gray-300 border-[1px] '}  rounded-[3px] text-[12px] focus:rounded-[2px] ${isDisabled?'bg-gray-500 placeholder-gray-400 text-gray-400':'text-black '} `+style;

        // Function to handle input change and call onInput with value and key
        const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
            onInput(e.target.value, keyType); // Use name attribute to determine the key
        };



        return (
            <div >
                {isHasTitle &&
                    <h6 className="text-[10px]  mb-[0.5px]">{keyType}</h6>}
                {
                    <div className="relative">
                        {
                            canShowOrHidePassowrd === true &&
                            <div className="absolute top-1 end-3 z-30">

                                {passwordType === 'text' ?
                                    <button
                                        onClick={(e) =>{
e.preventDefault();
setType('password');
                                        } }
                                        className=" h-5 w-5 "
                                        >
                                        <EyeIcon fontSize={5} />
                                    </button> :
                                    <button
                                        onClick={(e) =>{
                                            e.preventDefault();
                                            setType("text")
                                        }}

                                        className="h-5 w-5"
                                        >
                                        <EyeSlashIcon />
                                    </button>
                                }
                            </div>
                        }
                        <input
                            name={placeHolder} // Assign the key based on placeholder
                            value={value}
                            className={`h-7 ${generalStyle}`}
                            placeholder={placeHolder}
                            maxLength={maxLength}
                            disabled={isDisabled}
                            onChange={
                                (e)=>{
                                  e.preventDefault();
                                  handleChange;
                                }
                                }
                            type={passwordType}
                            required={isRequire}
                            onFocus={(e)=>{
                                e.preventDefault() ;
                               onFoucseInpur;
                             }}
                        />
                    </div>
                }
            </div>

        );
    };
