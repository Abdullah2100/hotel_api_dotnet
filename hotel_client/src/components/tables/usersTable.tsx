import { Dispatch, SetStateAction } from 'react'
import { Switch } from '@mui/material';
import NotFoundComponent from '../notFoundContent';
import { ArrowUturnLeftIcon, PencilIcon, TrashIcon } from '@heroicons/react/16/solid';
import { IAuthModule } from '../../module/iAuthModule';
import { Guid } from 'guid-typescript';
import { IUserModule } from '../../module/iUserModule';
import ImageHolder from '../imageHolder';

interface UserTableProps {
  data?: IUserModule[] | undefined,
  setUser: Dispatch<SetStateAction<IAuthModule>>
  seUpdate: Dispatch<SetStateAction<boolean>>
  deleteFunc: (userId: Guid) => Promise<void>
  makeUserVip: (userId: Guid) => Promise<void>
  isShwoingDeleted: boolean

}

const UersTable = ({
  data,
  setUser,
  seUpdate,
  deleteFunc,
  makeUserVip,
  isShwoingDeleted = false
}: UserTableProps) => {


  const setUserData = (user: IUserModule) => {
    setUser({
      userId: user.userId,
      address: user.personData?.address,
      brithDay: user.brithDay.split('T')[0], //dateHolder.toISOString().split('T')[0],
      email: user.personData?.email,
      name: user.personData?.name,
      password: '',
      phone: user.personData?.phone,
      username: user.userName,
      imagePath: user.imagePath?.toString()
    })
    seUpdate(true)
  }

  return (
    <div className={`overflow-x-auto justify-center ${data === undefined && 'h-48'} `}>
      <table className="min-w-full table-auto border-collapse">
        <thead className="bg-gray-200 text-gray-600">
          <tr>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap"></th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">profileImage</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Name</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Email</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Phone</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Address</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Created At</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Brithday</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">VIP Status</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Operation</th>
          </tr>
        </thead>
        <tbody>
          {data !== undefined && data.length > 0 && data.map((user, index) => (
            user.isDeleted && isShwoingDeleted === false ? undefined : <tr
              key={index}
              className={` ${user.isDeleted ? 'bg-red-500' : 'bg-white'}`}
            >
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{index + 1}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{

                <ImageHolder src={`${import.meta.env.VITE_MINIO_ENDPOINT}/user/${user.imagePath}`}
                  style='flex flex-row h-20 w-20'
                />
              }</td>

              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{user?.personData?.name || ""}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{user?.personData?.email || ""}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{user?.personData?.phone || ""}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{user?.personData?.address || ""}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">
                {user?.personData?.createdAt === undefined ? "" : new Date(user.personData.createdAt).toISOString().split('T')[0]}
              </td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">
                {user?.personData?.createdAt === undefined ? "" : user.brithDay.split('T')[0]}
              </td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">
                <Switch
                  defaultChecked={user.isVip}
                  disabled={user.isDeleted}
                  onChange={() => makeUserVip(user.userId)}
                />
              </td>

              <td className="px-4 py-1   text-left ">
                {user.isDeleted === false ?
                  <div className='flex flex-row justify-between'>

                    <button
                      onClick={() => deleteFunc(user.userId)}
                      className='border-[2px] rounded-[3px] border-red-600 h-7 w-7 flex justify-center items-center'
                    ><TrashIcon className='h-4 w-4 text-red-600 ' /></button>
                    <button
                      onClick={() => setUserData(user)}
                      className='border-[2px] rounded-[3px] border-green-800 h-7 w-7 flex justify-center items-center bg-gray-200'
                    ><PencilIcon className='h-6 w-6 text-green-800' /></button>
                  </div> :
                  <button onClick={() => deleteFunc(user.userId)}>

                    <ArrowUturnLeftIcon
                      className='h-6 w-6 text-white' />
                  </button>
                }
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {data === undefined &&
        <div className='h-20 w-full flex flex-col justify-center items-center  mt-10' >

          <NotFoundComponent />
        </div>
      }

    </div>
  )
}

export default UersTable
