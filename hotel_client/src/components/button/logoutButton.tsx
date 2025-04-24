import { ArrowLeftEndOnRectangleIcon } from "@heroicons/react/16/solid";
import { useDispatch } from "react-redux";
import { logout } from "../../controller/redux/jwtSlice";

const Logout = ()=>{
    const dispatch = useDispatch();


      const signout = async()=>{
         dispatch(logout())
      }

return(
    <button 
    className="flex flex-row  items-center mt-5 bg-red-700 w-48 ms-[29px] py-1 rounded-[4px] group hover:bg-red-400 ps-4"
    onClick={()=>signout()}>

        <ArrowLeftEndOnRectangleIcon className='h-8 fill-white group-hover:fill-gray-200 group hover:fill-slate-50'/>
        <h3 className='text-white group-hover:text-blue-200 text-sm ms-3 group hover:fill-slate-50'>{'تسجيل الخروج'}</h3>

    </button>
)
}

export default Logout;