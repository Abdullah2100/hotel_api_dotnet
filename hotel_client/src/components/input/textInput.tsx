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
  handleWidth?:string|undefined
}

export const TextInput =
  ({
    keyType = '',
    placeHolder,
    isDisabled = false,
    maxLength,
    onInput,
    style = '',
    type = 'text',
    isMultipleLine = false,
    value,
    isRequire = false,
    onFoucseInpur = undefined,
    isHasTitle = true,
    handleWidth=undefined

  }: TextInputProps
  ) => {
    const generalStyle = `px-2 ${isDisabled?'':'border-gray-300 border-[1px] '}  rounded-[3px] text-[12px] focus:rounded-[2px] ${isDisabled?'bg-gray-500 placeholder-gray-400 text-gray-400':'text-black '} `+style;

    // Function to handle input change and call onInput with value and key
    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      e.preventDefault()
      onInput(e.target.value, keyType); // Use name attribute to determine the key
    };



    return (
      <div  className={handleWidth}>
        {isHasTitle &&
        <div>
            <h6 className="text-[10px] mb-[0.5px] ">{placeHolder}</h6>
        </div>
        
          }
        {
          isMultipleLine ?
            <textarea
              name={placeHolder} // Assign the key based on placeholder
              value={value}
              className={generalStyle}
              placeholder={placeHolder}
              maxLength={maxLength}
              disabled={isDisabled}
              onChange={handleChange}
              required={isRequire}
              onFocus={(e)=>{
                 e.preventDefault() ;
                onFoucseInpur;
              }}
            />
            :
            <div>

              <input
                name={placeHolder} // Assign the key based on placeholder
                value={value}
                className={`h-7  ${generalStyle}`}
                placeholder={placeHolder}
                maxLength={maxLength}
                disabled={isDisabled}
                onChange={  handleChange }
                type={type}
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
