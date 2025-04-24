import { createContext, useEffect, useState } from "react";
import { enMessage } from "../module/enMessageType";
import { ExclamationTriangleIcon, ExclamationCircleIcon } from "@heroicons/react/16/solid";
interface iToastifyCustom {
    showToastiFy: (message: string, messageType: enMessage,isUp?:boolean) => void;
}

const toastifyCustomHolder: iToastifyCustom = {
   showToastiFy: (message: string, messageType: enMessage,isUp:boolean=false) => { },
}
export const useToastifiContext = createContext<iToastifyCustom>(toastifyCustomHolder)

const ToastifyCustom = ({ children }) => {
    const [message, setMessage] = useState<string | undefined>(undefined)
    const [messageType, setMessageType] = useState<enMessage | undefined>(enMessage.ERROR)
    const [isShownMessage, setShownMessage] = useState<boolean>(false)
    const [isUp,setDirection] = useState<boolean>(false)

    useEffect(() => {

    }, [message])

    const showNotify = (message: string, messageType: enMessage,isUp=false) => {
        setMessageType(messageType)
        setMessage(message)
        setShownMessage(true)
        setDirection(isUp)
    }

    const notifyIcon = () => {
        switch (messageType) {
            case enMessage.ERROR: {
                return <ExclamationTriangleIcon className="w-4 h-4 me-1" color="white" />
            }
            case enMessage.ATTENSTION: {
                return <ExclamationCircleIcon className="w-4 h-4 me-1" color="blue" />
            }
                return null;
        }
    }

    useEffect(() => {
        if (isShownMessage === true) {
            setTimeout(() => {
                setShownMessage(false)
            }, 2000); // Allow time for fade-out animation
        }
    }, [isShownMessage])


    return (<useToastifiContext.Provider value={{ showToastiFy: showNotify }}>
        <div className="overflow-hidden relative ">
            <div className={`z-10   transition-all duration-300 ease-in-out fixed
                ${isShownMessage === true ?
                    `${isUp?'top-1':'bottom-1'} opacity-100`
                    : 'bottom-0 opacity-0'
                } end-5 px-8 py-2 
                 rounded-sm
                ${messageType === enMessage.ERROR ? 'bg-red-900' :
                    messageType === enMessage.ATTENSTION ? 'bg-gray-50 ' : 'bg-green-900'} rounded-lg`}>

                <div className=" flex flex-row items-center">
                    {notifyIcon()}
                    <h5 className={`text-black text-sm ${messageType === enMessage.ATTENSTION ? 'text-black' : 'text-white'} ms-2`}>{message}</h5>
                </div>
            </div>

            {
                children
            }
        </div>

    </useToastifiContext.Provider>)
}

export default ToastifyCustom;