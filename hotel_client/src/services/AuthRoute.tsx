import { Navigate, Route } from 'react-router-dom'
import {  useSelector } from "react-redux";
import { RootState } from "../controller/rootReducer";

const AuthRoute = ({ Page }) => {
    const userAuth =   useSelector((state:RootState) => state.auth.refreshToken)
    

    if (userAuth!==null)
        return <Navigate to={'/'} />

    return (<Page />)
}

export default AuthRoute
        
    