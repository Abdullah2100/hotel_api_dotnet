import { Link } from 'react-router-dom'
import { enNavLinkType } from '../../module/enNavLinkType'
import { UsersIcon, } from '@heroicons/react/24/outline'
import { RectangleGroupIcon } from '@heroicons/react/24/outline'
import RoomsIcon from '../../assets/rooms_icon'

interface iNavLinkProps {
  navTo: string,
  name: string,
  navType?:enNavLinkType|undefined,
  isCurrentIndex?:boolean|undefined
}
const NavLink = ({ navTo, name,navType ,isCurrentIndex=false }: iNavLinkProps) => {

  const handleNavImage=()=>{
    switch(navType){
      case enNavLinkType.ROOMS:return (<RoomsIcon className='h-8 group-hover:text-gray-200  text-white' />)
      case enNavLinkType.ROOMTYPE:return (<RectangleGroupIcon className='h-8 group-hover:text-gray-200  text-white'/>)
     default: return ( <UsersIcon className='h-8 group-hover:text-gray-200  text-white'/>)
    }
  }
  return (
    <div >
      <Link
      className={`group flex flex-row justify-start w-44 items-center mt-4  ms-[27px] ps-2 hover:bg-black/10 py-1 ${isCurrentIndex&&'bg-black/10'}`}
      to={navTo}>
        {handleNavImage()}
        <h3 className='text-white group-hover:text-blue-200 text-sm ms-3'>{name}</h3>
      </Link>
      
    </div>
  )
}

export default NavLink
