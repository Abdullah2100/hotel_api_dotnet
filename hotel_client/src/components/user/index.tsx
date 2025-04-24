import { SetStateAction, useState } from "react"
import { IUserModule } from "../../module/iUserModule"
import { XMarkIcon } from "@heroicons/react/24/outline"
import ImageHolder from "../imageHolder"
import { generalMessage } from "../../util/generalPrint"

interface iUserProps {
  userData: IUserModule | undefined,
  changeTheButtonStatus: React.Dispatch<SetStateAction<IUserModule | undefined>>
}

const UserShape = ({ userData = undefined, changeTheButtonStatus }: iUserProps) => {

  return <div className={`fixed  bg-gray-600  bottom-14   flex flex-row   h-screen w-screen top-0 start-0  transition-opacity duration-300 ${userData ? 'opacity-100  z-30' : 'opacity-0  -z-10'}`}>
    <button
      onClick={(e) => {
        e.preventDefault()
        changeTheButtonStatus(undefined)
      }}
      style={{ cursor: 'pointer' }}
      className='absolute z-30'
    >
      <XMarkIcon className='h-7 w-6 text-white absolute top-0' />

    </button>
    {userData && <div className='absolute h-screen w-screen flex flex-row items-center justify-center z-10'>
      <div>
        <ImageHolder
          iconColor='text-white'
          src={`${import.meta.env.VITE_MINIO_ENDPOINT}/user/${userData.imagePath}`}
          style='flex flex-row h-20 w-20'
        />
      </div>
      <div className='me-4 ms-4 flex flex-col flex-nowrap max-w-[200px]'>
        <h4 className='text-white'>
          {userData?.personData.name}
        </h4>
        <h4 className='text-white mt-2'>
          {userData?.personData.phone}

        </h4>
        <h4 className='text-white mt-1'>
          {userData?.personData.email}

        </h4>
      </div>
    </div>}
  </div>
}

export default UserShape;