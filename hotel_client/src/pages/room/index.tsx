import Header from '../../components/header/header'

import { PencilIcon, XMarkIcon } from '@heroicons/react/16/solid';

import ImageHolder from '../../components/imageHolder';
import RoomsIcon from '../../assets/rooms_icon';
import { enNavLinkType } from '../../module/enNavLinkType';
import { SetStateAction, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { useToastifiContext } from '../../context/toastifyCustom';
import { enMessage } from '../../module/enMessageType';
import { TextInput } from '../../components/input/textInput';
import SubmitButton from '../../components/button/submitButton';
import { enStatus } from '../../module/enState';
import { iImageHolder } from '../../module/IImageHolder';
import { useMutation, useQuery } from '@tanstack/react-query';
import { enApiType } from '../../module/enApiType';
import apiClient from '../../services/apiClient';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../controller/rootReducer';
import { IRoomType } from '../../module/iRoomType';
import { Guid } from 'guid-typescript';
import { generalMessage } from '../../util/generalPrint';
import { IRoomModule } from '../../module/iRoomModule';
import { enStatsu } from '../../module/enStatsu';
import RoomTable from '../../components/tables/roomTable';
import { IAuthModule } from '../../module/iAuthModule';
import { logout } from '../../controller/redux/jwtSlice';
import UserShape from '../../components/user';
import { IUserModule } from '../../module/iUserModule';
import ImagesPlay from '../../components/images';
import { General } from '../../util/general';
import DateFormat from '../../util/dateFormat';
import { Switch } from '@mui/material';

const Room = () => {
  const refreshToken = useSelector((state: RootState) => state.auth.refreshToken)
  const token = useSelector((state: RootState) => state.auth.token)
  const dispatch = useDispatch()

  const { showToastiFy } = useContext(useToastifiContext)

  const [status, setState] = useState<enStatus>(enStatus.none)
  const [isUpdate, setUpdate] = useState<boolean>(false)
  const [isSingle, setSingle] = useState(false)
  const [isDraggable, changeDraggableStatus] = useState(false)
  const [pageNumber, setPageNumber] = useState(1)

  const [isShowingDeleted, setShowingDeleted] = useState<boolean>(false)

  const [thumnailImage, setThumnail] = useState<iImageHolder | undefined>(undefined)
  const [images, setImages] = useState<iImageHolder[] | undefined>(undefined)

  const [imagesHolder, setImagesHolder] = useState<iImageHolder[]>()

  const [deletedImages, setdeletedImagess] = useState<iImageHolder[] | undefined>(undefined)

  const [roomData, setRoomData] = useState<IRoomModule>({
    roomtypeid: undefined,
    bedNumber: 10,
    capacity: 10,
    pricePerNight: 10,
    status: enStatsu.Available,
    images: undefined,
    roomId: undefined,
    createdAt: undefined,
    beglongTo: undefined
  })

  const [userData, setUserHover] = useState<IUserModule | undefined>(undefined)


  const imageRef = useRef<HTMLInputElement>(null);

  const theumnailHolder = useMemo(() => {
    return <ImageHolder
      typeHolder={enNavLinkType.ROOMS}
      src={thumnailImage?.data === undefined ? `${import.meta.env.VITE_MINIO_ENDPOINT}/room/${thumnailImage?.path}` : URL.createObjectURL(thumnailImage.data)}
      style='flex  w-full lg:w-20 '
      isFromTop={true} />
  }, [thumnailImage])

  const handleRoomEditeButton = (roomDataHolder: IRoomModule) => {


    if (roomDataHolder.images) {
      // roomDataHolder.images.map(x => {
      //  if (x != undefined)
      // })
      const thumnailHolder = roomDataHolder.images?.find(x => x.isThumnail === true);
      generalMessage(`\n\n\nthis shown the images ${JSON.stringify(roomDataHolder.images)}\n\n`)

      if (thumnailHolder) {
        generalMessage(`\n\n\nthis shown the images ${thumnailImage?.path}\n\n`)
        setThumnail(thumnailHolder)

      }
      setImages(roomDataHolder.images?.filter(x => x.isThumnail === false))

    }
    setUpdate(true)
    setRoomData({
      isBlock: roomDataHolder.isBlock,
      isDeleted: roomDataHolder.isDeleted,
      roomData: roomDataHolder.roomData,
      user: roomDataHolder.user,
      roomtypeid: roomDataHolder.roomtypeid,
      bedNumber: roomDataHolder.bedNumber,
      capacity: roomDataHolder.capacity,
      pricePerNight: roomDataHolder.pricePerNight,
      status: General.convetNumberToEnStatus((roomDataHolder.status as number)),
      images: roomData.images,
      roomId: roomDataHolder.roomId,
      createdAt: roomDataHolder.createdAt,
      beglongTo: roomDataHolder.beglongTo
    })
  }

  const logoutFn = () => {
    dispatch(logout())
  }

  const updateInput = (value: any, key: string) => {

    setRoomData((prev) => ({
      ...prev,
      [key]: value,
    }));

  };

  const changeIsSingleStatus = async (isSingle: boolean) => {
    setSingle(prev => prev = isSingle)
  }

  const selectImage = async (isSingle: boolean, e: React.MouseEvent) => {

    e.preventDefault()
    await changeIsSingleStatus(isSingle).then(() => {

      imageRef.current?.click();
    })
  }

  const uploadImageDisplayFromSelectInput = async (e: React.ChangeEvent<HTMLInputElement>) => {
    e.preventDefault()

    if (imageRef.current && imageRef.current.files) {

      switch (isSingle) {
        case true: {

          const uploadedFile = imageRef.current.files[0];

          if (thumnailImage) {
            addDeletedFileToDeletedList(thumnailImage);
          }

          const fileExtension = uploadedFile.name.split('.').pop()?.toLowerCase();

          if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
            showToastiFy("You must select a valid image", enMessage.ERROR);
            return;
          }

          const fileNew = new File([uploadedFile], "ffffffff." + uploadedFile.type.split('/')[0], { type: uploadedFile.type });


          setThumnail({
            data: fileNew,
            belongTo: undefined,
            id: undefined,
            isDeleted: false,
            isThumnail: true,
            path: undefined
          })

        } break;

        default: {

          let imagesHolder = [] as iImageHolder[];
          for (let i = 0; i < imageRef.current.files.length; i++) {
            const uploadedFile = imageRef.current.files[i];

            const fileExtension = uploadedFile.name.split('.').pop()?.toLowerCase();

            if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
              showToastiFy("You must select a valid image", enMessage.ERROR);
              return;
            }
            imagesHolder.push({
              data: uploadedFile,
              belongTo: undefined,
              id: undefined,
              isDeleted: false,
              isThumnail: false,
              path: undefined
            })
          }

          setImages(prev => [...(prev || []), ...imagesHolder]);


        } break;
      }


    }
  };


  const draggbleFun = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    changeDraggableStatus(true)
  }

  const draggableOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    changeDraggableStatus(true)
  }

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault()
    changeDraggableStatus(false);
  };

  const handleDropImage = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    changeDraggableStatus(false)
    if (e.dataTransfer.files) {
      if (thumnailImage) {
        addDeletedFileToDeletedList(thumnailImage);
      }

      const file = e.dataTransfer.files[0];
      const fileExtension = file.name.split('.').pop()?.toLowerCase();

      if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
        showToastiFy("You must select a valid image", enMessage.ERROR);
        return;
      }
      const fileNew = new File([file], "ffffffff." + file.type.split('/')[0], { type: file.type });
      setThumnail(prev => prev = {
        data: fileNew,

        belongTo: undefined,
        id: undefined,
        isDeleted: false,
        isThumnail: true,
        path: undefined

      })
    }

  }

  const handleDropImages = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    changeDraggableStatus(false)
    if (e.dataTransfer.files) {

      for (let i = 0; i < e.dataTransfer.files.length; i++) {
        const uploadedFile = e.dataTransfer.files[i];

        const fileExtension = uploadedFile.name.split('.').pop()?.toLowerCase();

        if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
          showToastiFy("You must select a valid image", enMessage.ERROR);
          return;
        }

        setImages(prev => [...(prev || []), ...[
          {
            data: uploadedFile,

            belongTo: undefined,
            id: undefined,
            isDeleted: false,
            isThumnail: false,
            path: undefined

          }
        ]]);

      }

    }
  }


  const addDeletedFileToDeletedList = (image: iImageHolder) => {
    if (image?.id !== undefined) {

      image.isDeleted = true;

      // Update the state
      let deletedImageList = [] as iImageHolder[];
      if (deletedImages === undefined) {
        deletedImageList = [image]
      }
      if (deletedImages !== undefined) {
        deletedImageList = [...deletedImages, image]
      }
      setdeletedImagess(deletedImageList);
    }
  };


  const deleteImage = (index: number) => {
    if ((images && images?.length > 0)) {

      addDeletedFileToDeletedList(images[index]);

      setImages(images.filter(x => x.id !== images[index].id));
    }
    // if (images) {
    // }
  }


  const { data: roomtypes, error } = useQuery({
    queryKey: ['roomTypeNotDleted'],

    queryFn: async () => apiClient({
      enType: enApiType.GET,
      endPoint: import.meta.env.VITE_ROOMTYPE + 'true',
      parameters: true,
      isRequireAuth: true,
      jwtRefresh: refreshToken || ""
      , jwtValue: token || "",
      tryNumber: 1
    }).then((data) => {
      if (data === undefined) return [];
      const roomType = data.data as IRoomType[];
      updateInput(roomType[0].roomTypeID, 'roomtypeid')
      return roomType;
    })
    ,
  });


  const clearData = () => {
    setRoomData(prev => prev = {
      isBlock: undefined,
      isDeleted: null,
      roomData: undefined,
      user: undefined,
      roomtypeid: undefined,
      bedNumber: 0,
      capacity: 0,
      pricePerNight: 0,
      status: enStatsu.Available,
      images: undefined,
      roomId: undefined,
      createdAt: undefined,
      beglongTo: undefined
    })
    setThumnail(undefined)
    setImages(undefined)
  }


  const roomMutaion = useMutation({
    mutationFn: ({ data, endpoint, methodType,
      token, refreshToken
    }: {
      data?: FormData | undefined,
      endpoint: string,
      methodType: enApiType,
      token?: string | null
      refreshToken?: string | null
    }) =>
      apiClient({
        enType: methodType,
        endPoint: endpoint
        , parameters: data,
        isRequireAuth: true,
        isFormData: data != undefined,
        jwtValue: token || "",
        jwtRefresh: refreshToken ?? undefined,
        tryNumber: 1
      }),
    onSuccess: (data) => {
      setState(enStatus.complate)
      showToastiFy(`user ${isUpdate ? "updated" : "created"} Sueccessfuly`, enMessage.SECCESSFUL);
      clearData()
      refetch()
      if (isUpdate)
        setUpdate(false)

    },
    onError: (error) => {
      setState(enStatus.complate);
      if (error != undefined && error !== null) {
        if ((error as any).status === 401) {
          logoutFn()
        } else {
          showToastiFy(error?.message?.toString() || "An unknown error occurred", enMessage.ERROR)

        }

      }
    }

  })

  const validationInput = () => {

    let message = "";
    if (thumnailImage === undefined) {
      message = "Thumnail is required"
    }
    else if (images === undefined || images.length === 0) {
      message = "Images is required"
    }
    else if (roomData.pricePerNight === 0) {
      message = "Price per night is required";
    }
    else if (roomData.roomtypeid === undefined) {
      message = "Room type is required"
    }
    else if (roomData.capacity === 0) {
      message = "Capacity is required"
    }
    else if (roomData.bedNumber === 0) {
      message = "Bed number is required"
    }
    else if (roomData.roomtypeid === undefined) {
      message = "Room type is required"
      return false;
    }

    if (!(message.length === 0)) {

      showToastiFy(message, enMessage.ATTENSTION)
    }

    return message.length === 0;
  }

  const createOrUpdateRoom = async () => {
    let counter = 0;

    if (isUpdate === false) {
      const result = !validationInput();

      if (result) return;
    }

    const formData = new FormData();
    formData.append("status", `${roomData.status}`);

    formData.append("pricePerNight", `${roomData.pricePerNight}`);

    formData.append("capacity", roomData.capacity.toString());

    formData.append("bedNumber", roomData.bedNumber.toString());

    formData.append("status", roomData.status.toString());

    formData.append("roomtypeid", roomData.roomtypeid?.toString() ?? "");


    if (thumnailImage) {

      formData.append(`images[${counter}].id`, thumnailImage.id ? thumnailImage.id.toString() : "");
      formData.append(`images[${counter}].belongTo`, thumnailImage.belongTo ? thumnailImage.belongTo.toString() : "");
      formData.append(`images[${counter}].isDeleted`, thumnailImage.isDeleted ? thumnailImage.isDeleted.toString() : "");
      if (thumnailImage.path !== undefined)
        formData.append(`images[${counter}].fileName`, thumnailImage.path ?? "");
      formData.append(`images[${counter}].isDeleted`, thumnailImage.isDeleted ? thumnailImage.isDeleted.toString() : "");
      formData.append(`images[${counter}].isThumnail`, thumnailImage.isThumnail ? thumnailImage.isThumnail.toString() : "");

      if (thumnailImage.data)
        formData.append(`images[${counter}].data`, thumnailImage.data);
    }


    if (images && images.length > 0) {
      images.forEach((image, index) => {
        counter += 1;
        formData.append(`images[${counter}].id`, image.id ? image.id.toString() : "");
        formData.append(`images[${counter}].belongTo`, image.belongTo ? image.belongTo.toString() : "");
        formData.append(`images[${counter}].isDeleted`, image.isDeleted ? image.isDeleted.toString() : "");
        if (image.path !== undefined)
          formData.append(`images[${counter}].fileName`, image.path ?? "");
        formData.append(`images[${counter}].isThumnail`, image.isThumnail ? image.isThumnail.toString() : "");
        if (image.data)
          formData.append(`images[${counter}].data`, image.data);
      });
    }



    if (isUpdate) {
      // formData.append("roomId", roomData.roomId ? roomData.roomId.toString() : '');
      if (deletedImages) {

        deletedImages.forEach((image, index) => {
          counter += 1;

          formData.append(`images[${counter}].id`, image.id ? image.id.toString() : "");
          formData.append(`images[${counter}].belongTo`, image.belongTo ? image.belongTo.toString() : "");
          formData.append(`images[${counter}].isDeleted`, image.isDeleted ? image.isDeleted.toString() : "");
          if (image.path !== undefined)
            formData.append(`images[${counter}].fileName`, image.path ?? "");
          formData.append(`images[${counter}].isThumnail`, image.isThumnail ? image.isThumnail.toString() : "");
          if (image.data)
            formData.append(`images[${counter}].data`, image.data);
        })
      };

    }

    generalMessage(`Deleted images updated: ${JSON.stringify(deletedImages?.length)}`);

    let endpoint = import.meta.env.VITE_ROOM;

    if (isUpdate && roomData.roomId) {
      endpoint += '/' + roomData.roomId.toString();
    }

    await roomMutaion.mutate({
      data: formData,
      endpoint: endpoint,
      methodType: isUpdate ? enApiType.PUT : enApiType.POST,
      refreshToken: refreshToken,
      token: token
    });

  }

  const { data, error: roomsError, refetch } = useQuery({
    queryKey: ['rooms'],
    queryFn: async () => apiClient({
      enType: enApiType.GET,
      endPoint: import.meta.env.VITE_ROOM + `/${pageNumber}`,
      parameters: undefined,
      isRequireAuth: true,
      jwtValue: token || "",
      jwtRefresh: refreshToken ?? undefined,
      tryNumber: 1
    }),

  }
  );
  const deleteOrUndeleteRoom = async (userId: Guid) => {
    generalMessage(`this shown the deletion function is caled ${import.meta.env.VITE_ROOM + '/' + userId}`)
    await roomMutaion.mutate({
      data: undefined,
      endpoint: import.meta.env.VITE_ROOM + '/' + userId,
      methodType: enApiType.DELETE,
      token: token,
      refreshToken: refreshToken
    });

  };



  useEffect(() => {
    if ((error != undefined && error !== null) || (roomsError != undefined && roomsError !== null)) {
      if ((error as any).status === 401 || (roomsError as any).status == 401) {
        dispatch(logout())
      } else {
        showToastiFy((error?.message?.toString() || roomsError?.message.toString()) || "An unknown error occurred", enMessage.ERROR)
      }
    }
  }, [error])



  return (
    <div className='flex flex-row'>

      <Header index={3} />

      <div className='min-h-screen w-[calc(100%-192px)] ms-[192px] flex flex-col px-2 items-start '>
        <div className='flex flex-row items-center mt-2'>
          <RoomsIcon className='h-8 fill-black group-hover:fill-gray-200 -ms-1' />
          <h3 className='text-2xl ms-1'>Room</h3>
        </div>




        <div className='w-full h-full mt-4 flex flex-col'>
          <div
            onDrag={draggbleFun}
            onDragOver={draggableOver}
            onDrop={handleDropImage}
            onDragLeave={handleDragLeave}
            className='relative mb-3 md:mb-0 '>

            <button
              onClick={(e) => selectImage(true, e)}
              className='group absolute start-2 top-11 hover:bg-gray-600 hover:rounded-sm '>
              <PencilIcon className='h-6 w-6 border-[1px] border-blue-900 rounded-sm group-hover:fill-gray-200  ' />
            </button>
            <h3 className='text-lg'>
              Thumail
            </h3>
            <div className=' h-44 w-44  flex flex-row justify-center items-center border-[2px] rounded-lg mt-2'
            >
              <img
                src={
                  thumnailImage?.data === undefined ?
                    `${import.meta.env.VITE_MINIO_ENDPOINT}/room/${thumnailImage?.path}`
                    : URL.createObjectURL(thumnailImage.data)
                }
                className='flex flex-row h-24 w-24 '
              />
              {
                //theumnailHolder
              }
            </div>

            <input
              multiple={false}
              type="file"
              id="file"
              ref={imageRef}
              onChange={uploadImageDisplayFromSelectInput}
              hidden />
          </div>

          <div className='md:w-3' />

          <div
            onDrag={draggbleFun}
            onDragOver={draggableOver}
            onDrop={handleDropImages}
            onDragLeave={handleDragLeave}

            className=' w-full relative'>
            <button
              onClick={(e) => selectImage(false, e)}
              className='group absolute start-2 top-11 hover:bg-gray-600 hover:rounded-sm '>
              <PencilIcon className='h-6 w-6 border-[1px] border-blue-900 rounded-sm group-hover:fill-gray-200  ' />
            </button>

            <h3 className='text-lg'>
              Images
            </h3>
            <div className={`min-h-44 w-full flex flex-col md:flex-row  border-[2px] rounded-lg mt-2  md:gap-2 px-2 ${images !== undefined && images.length > 0 && 'pt-9'}  justify-center items-center overflow-scroll pb-2`}
            >
              {
                (images != undefined && images.length > 0) ? images.map((data, index) => {
                  return <div
                    className={`w-full lg:w-20 ${index === 0 ? 'mb-1' : index === images.length - 1 ? '' : 'mb-1'} `}
                    key={index}
                  >
                    <ImageHolder
                      deleteFun={() => { deleteImage(index) }}
                      key={index}
                      typeHolder={enNavLinkType.ROOMS}
                      src={data.data === undefined ? `${import.meta.env.VITE_MINIO_ENDPOINT}/room/${data.path}` : URL.createObjectURL(data.data)}
                      style='flex  w-full lg:w-20 '
                      isFromTop={true} />
                  </div>
                }) : <ImageHolder
                  typeHolder={enNavLinkType.ROOMS}
                  src={undefined}
                  style='flex flex-row h-20 w-20'
                  isFromTop={true} />
              }
            </div>

            <input
              multiple={true}
              type="file"
              id="file"
              ref={imageRef}
              onChange={uploadImageDisplayFromSelectInput}
              hidden />
          </div>



        </div>

        <div className='w-full  md:flex md:row md:flex-row md:flex-wrap md:items-center md:gap-[5px] mt-[10px]'>

          {isUpdate &&
            <div className='w-full md:w-[150px] mb-2'>
              <h3 className='text-[10px]'>Room Status</h3>
              <select name="cars" id="cars" className="w-full md:w-[150px] px-2 py-[4px] rounded-sm border border-gray-300 bg-transparent text-[12px]">
                <option className="bg-transparent hover:bg-transparent" value="Available">Available</option>
                <option className="bg-transparent hover:bg-transparent" value="Booked">Booked</option>
                <option className="bg-transparent hover:bg-transparent" value="Under Maintenance">Under Maintenance</option>
              </select>

            </div>
          }

          <div className='w-full md:w-[150px] mb-2'>
            <TextInput
              keyType='pricePerNight'
              value={roomData.pricePerNight}
              onInput={updateInput}
              placeHolder="Price Per Night"
              style=" w-full md:w-[150px]"
            />
          </div>


          {isUpdate &&
            <div className='w-full md:w-[150px] mb-2'>
              <TextInput
                isDisabled={true}
                type='datetime-local'
                keyType='createdAt'
                value={roomData.createdAt}
                onInput={updateInput}
                placeHolder="Created At"
                style="w-full md:w-[150px]"
              />
            </div>
          }


          <div className='w-full md:w-[150px] '>
            <h3 className=' text-[10px]'>RoomType</h3>
            <select
              onChange={(e) => {
                const selectedID = e.target.value;
                updateInput(selectedID as unknown as Guid, 'roomtypeid')
              }}
              name="cars"
              id="cars"
              className="w-full md:w-[150px]  px-2 py-1 mb-2 rounded-sm border border-gray-300 bg-transparent  text-[12px]">
              {roomtypes !== undefined && (roomtypes).map((roomType, index) => {

                return <option key={index} className="bg-transparent hover:bg-transparent" value={roomType.roomTypeID?.toString()}>{roomType.roomTypeName}</option>
              })}
            </select>
          </div>


          <div className='w-full md:w-[100px] mb-2'>
            <TextInput
              keyType='capacity'
              value={roomData.capacity}
              onInput={updateInput}
              placeHolder="Capacity"
              style=" w-full md:w-[100px]"
            />
          </div>


          <div className='w-full md:w-[100px] mb-2'>
            <TextInput
              keyType='bedNumber'
              value={roomData.bedNumber}
              onInput={updateInput}
              placeHolder="BedNumber"
              style="  w-full md:w-[100px]"
            /></div>

          {isUpdate &&
            <div className='w-full md:w-[150px] mb-2'>
              <TextInput
                isDisabled={true}
                keyType='beglongTo'
                value={roomData.beglongTo}
                onInput={updateInput}
                placeHolder="BelongTo"
                style=" w-full md:w-[150px]"
              />
            </div>
          }
        </div>

        <div className='w-full'>

          <SubmitButton
            onSubmit={async () => createOrUpdateRoom()}
            buttonStatus={status}
            placeHolder={isUpdate ? 'update' : 'create'}
            style="text-[10px] bg-mainBg   w-full md:w-44 text-white rounded-[4px] my-2 h-8  hover:opacity-90"
            textstyle='text-[14px]'
          />
          <SubmitButton
            // textstyle='text-black'
            // onSubmit={async () => clearData()}
            onSubmit={async () => { }}
            // buttonStatus={status}
            placeHolder={'reseat'}
            style="text-[10px] bg-white border-[1px]  w-full md:w-44 text-white rounded-[4px] my-2 h-8  hover:opacity-90 md:ms-2"
            textstyle='text-[14px] text-black'
          />
        </div>


        {/* <div className='relative overflow-y-scroll'> */}

        <RoomTable data={data === undefined ? undefined : data.data as unknown as IRoomModule[]}
          setUserHover={setUserHover}
          setRoom={handleRoomEditeButton}
          setRoomImages={setImagesHolder}
          deleteFunc={deleteOrUndeleteRoom} 
          isShwoingDeleted={isShowingDeleted}
           />
        <div>
          <h3>showing the deleted rooms</h3>
          <Switch onChange={() => setShowingDeleted(prev => !prev)} />
        </div>

        <UserShape
          userData={userData ? userData : undefined}
          changeTheButtonStatus={setUserHover}
        />


        <ImagesPlay images={imagesHolder} setImagesToNull={setImagesHolder} />


      </div>
    </div>
  )
}

export default Room
