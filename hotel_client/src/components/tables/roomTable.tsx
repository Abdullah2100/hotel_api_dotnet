import { Dispatch, SetStateAction } from 'react'

import NotFoundComponent from '../notFoundContent';

import { Guid } from 'guid-typescript';

import { IRoomModule } from '../../module/iRoomModule';

import DateFormat from '../../util/dateFormat';
import { ArrowUturnLeftIcon, PencilIcon, PhotoIcon, TrashIcon } from '@heroicons/react/24/outline';
import { generalMessage } from '../../util/generalPrint';
import { General } from '../../util/general';
import { IUserModule } from '../../module/iUserModule';
import { iImageHolder } from '../../module/IImageHolder';

interface RoomTableProps {
  data?: IRoomModule[] | undefined,
  setRoom: (roomData:IRoomModule)=>void;
  setUserHover: Dispatch<SetStateAction<IUserModule | undefined>>
  setRoomImages: Dispatch<SetStateAction<iImageHolder[] | undefined>>
   deleteFunc: (roomId: Guid) => Promise<void>
  isShwoingDeleted: boolean

}

const RoomTable = ({
  data,
  setRoom,
  setUserHover,
  setRoomImages,
   deleteFunc,
  isShwoingDeleted = false
}: RoomTableProps) => {

  data?.forEach((x)=>{
    generalMessage(`this the data from api ${
       JSON.stringify(x)
    }`)
  })
   

  return (
    <div className={`w-full pb-5 overflow-auto`}>
      <table className="w-full  table-auto border-collapse">
        <thead className="bg-gray-200 text-gray-600">
          <tr>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap"></th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Owner by</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Status</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">RoomType</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Price Per Night</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">bedNumber</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Created At</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Images</th>
            <th className="px-4 py-2 border-b text-left whitespace-nowrap">Operations</th>
          </tr>
        </thead>
        <tbody>
          {data !== undefined && data.length > 0 && data.map((Room, index) => (
            Room.isDeleted && isShwoingDeleted === false ? undefined : <tr
              key={index}
              className={` ${Room.isDeleted ? 'bg-red-500' : 'bg-white'}`}
            >
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{index + 1}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">
                <button
                  className='text-blue-600'
                  onClick={(e) => {
                    e.preventDefault()
                    setUserHover(Room?.user)
                  }}
                  style={{ cursor: 'pointer' }}
                >
                  {
                    Room?.user?.personData.name || ""
                  }
                </button>


              </td>
              <td className={`px-4 py-2 border-b text-left whitespace-nowrap  ${General.handlingEnStatusColor((Room.status as number))}`}>{General.convetNumberToEnStatus((Room.status as number))}</td>


              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{Room.roomData?.roomTypeName}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{Room.pricePerNight}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">{Room.bedNumber}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap ">{DateFormat.toStringDate(Room.createdAt)}</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap cursor-pointer">{
                <button 
                onClick={(e)=>{
                  e.preventDefault();
                  setRoomImages(Room.images)
                }}
                className='cursor-pointer w-full h-14'>
                  <PhotoIcon className='h-6 w-6 text-blue-700 ms-4' />
                </button>
              }</td>
              <td className="px-4 py-2 border-b text-left whitespace-nowrap">
                {Room.isDeleted === false ?
                  <div className='flex flex-row justify-between'>

                    <button
                       onClick={() => { deleteFunc(Room.roomId as Guid) }}
                      className='border-[2px] rounded-[3px] border-red-600 h-7 w-7 flex justify-center items-center cursor-pointer'
                    ><TrashIcon className='h-4 w-4 text-red-600 ' /></button>
                    <button
                      onClick={() => setRoom(Room)}
                      className='border-[2px] rounded-[3px] border-green-800 h-7 w-7 flex justify-center items-center bg-white cursor-pointer'
                    ><PencilIcon className='h-6 w-6 text-green-800' /></button>
                  </div>
                  :
                  <button
                   onClick={() => { deleteFunc(Room.roomId as Guid) }}
                  >

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

export default RoomTable
