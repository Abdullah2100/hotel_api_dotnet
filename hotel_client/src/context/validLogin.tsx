import { createContext, ReactNode, useEffect, useState } from "react";
import { isValidToken } from "../services/authServices";

interface iUserAuthReq{
    isLogin:boolean,
    hasValidToken:boolean,
 
}
const userAuth:iUserAuthReq={
hasValidToken:false,
isLogin:false
}

interface UseAuthUserProps {
  children: ReactNode; // Type for the children prop
}

export const userAuthContext = createContext<iUserAuthReq>(userAuth);

const UseAuthUser = ({children}:UseAuthUserProps)=>{
    const accessToken = localStorage.getItem('access_token')||'';
    const [isLogin,changeLoginState] = useState((accessToken!==undefined&&accessToken?.length>0)||false);
    const [hasValidToken,chageTokenValidation] = useState(false);
    
    //async function validateToken(token:string){

   // }
   useEffect(()=>{
    if(isLogin){
       isValidToken(accessToken).then(()=>{
        chageTokenValidation(true)
       })
       .catch(()=>{
        chageTokenValidation(false)
       })
    }

   },[])


    return (
        <userAuthContext.Provider value={{isLogin,hasValidToken}}>
            {children}
        </userAuthContext.Provider>
    )

}

export default UseAuthUser;
