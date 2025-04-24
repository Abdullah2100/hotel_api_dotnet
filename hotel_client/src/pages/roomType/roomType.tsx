import { useContext, useEffect, useRef, useState } from 'react';
import Header from '../../components/header/header'
import { TextInput } from '../../components/input/textInput'
import { IRoomType } from '../../module/iRoomType';
import { PencilIcon, RectangleGroupIcon } from '@heroicons/react/16/solid';
import SubmitButton from '../../components/button/submitButton';
import { enMessage } from '../../module/enMessageType';
import { useToastifiContext } from '../../context/toastifyCustom';
import { enStatus } from '../../module/enState';
import apiClient from '../../services/apiClient';
import { enApiType } from '../../module/enApiType';
import { notifyManager, useMutation, useQuery } from '@tanstack/react-query';
import { RootState } from '../../controller/rootReducer';
import { useDispatch, useSelector } from 'react-redux';
import RoomTypeTable from '../../components/tables/roomTypeTable';
import ImageHolder from '../../components/imageHolder';
import { generalMessage } from '../../util/generalPrint';
import { Guid } from 'guid-typescript';
import { Switch } from '@mui/material';
import { logout } from '../../controller/redux/jwtSlice';

const RoomType = () => {
  const dispatch = useDispatch();

  const refreshToken = useSelector((state: RootState) => state.auth.refreshToken)
  const token = useSelector((state: RootState) => state.auth.token)

  const { showToastiFy } = useContext(useToastifiContext)
  const [status, setState] = useState<enStatus>(enStatus.none)

  const [isUpdate, setUpdate] = useState<boolean>(false)
  const imageRef = useRef<HTMLInputElement>(null);
  const [imageHolder, setImageHolder] = useState<File | undefined>(undefined);
  const [image, setImage] = useState<string | undefined>(undefined)
  const [isDraggable, changeDraggableStatus] = useState(false)
  const [isShownDeletion, changeShowingDeleteionStatus] = useState(false)

  const [roomType, setRoomType] = useState<IRoomType>({
    roomTypeName: '',
    createdAt: null,
    createdBy: null,
    roomTypeID: null
  });

  const updateInput = (value: any, key: string) => {
    setRoomType((prev) => ({
      ...prev,
      [key]: value,
    }));
  };


    const clearData = () => {
    setRoomType({
      roomTypeName: '',
      createdAt: null,
      createdBy: null,
      roomTypeID: null
    })
    setImage(undefined)
    setImageHolder(undefined)
  }
  const logoutFn = () => {
    dispatch(logout())
  }


  const selectImage = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    imageRef.current?.click();
  }


  const uploadImageDisplayFromSelectInput = async (e: React.ChangeEvent<HTMLInputElement>) => {
    e.preventDefault()
    if (imageRef.current && imageRef.current.files && imageRef.current.files[0]) {
      const uploadedFile = imageRef.current.files[0];
      const fileExtension = uploadedFile.name.split('.').pop()?.toLowerCase();

      // Validate file type
      if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
        showToastiFy("You must select a valid image", enMessage.ERROR);
        return;
      }

      const cachedURL = URL.createObjectURL(uploadedFile);

      const img = new Image();
      img.onload = () => {
        const { naturalHeight, naturalWidth } = img;

        showToastiFy(`Image height: ${naturalHeight}\nImage width: ${naturalWidth}`, enMessage.ERROR);

        //   if (naturalHeight > 40 || naturalWidth > 40) {
        //     showToastiFy("Image must have a minimum height and width of 40px", enMessage.ERROR);
        //     return;
        //   }

        setImage(cachedURL);
        setImageHolder(uploadedFile)
      };

      img.src = cachedURL;
    }
  };



  const { data, error, refetch } = useQuery({

    queryKey: ['roomtypes'],
    queryFn: async () => apiClient({
      enType: enApiType.GET,
      endPoint: import.meta.env.VITE_ROOMTYPE + "false",
      parameters: undefined,
      isRequireAuth: true,
      jwtValue: token || "",
      jwtRefresh: refreshToken || "",
      tryNumber: 1
    }),
  }
  );

  const roomtypeMutaion = useMutation({
    mutationFn: ({ data, endpoint, methodType,
      jwtToken, jwtRefreshToken
    }: {
      data?: FormData | undefined,
      endpoint: string,
      methodType: enApiType,
      jwtToken?: string | null,
      jwtRefreshToken?: string | null
    }) =>
      apiClient({
        enType: methodType,
        endPoint: endpoint
        , parameters: data,
        isRequireAuth: true,
        jwtValue: jwtToken ?? undefined,
        jwtRefresh: jwtRefreshToken ?? undefined,
        isFormData: data != undefined
        , tryNumber: 1
      }),
    onSuccess: (data) => {
      setState(enStatus.complate)
      showToastiFy(`user ${isUpdate ? "updated" : "created"} Sueccessfuly`, enMessage.SECCESSFUL);
      clearData()
      setImage(undefined)
      if (isUpdate)
        setUpdate(false)
      refetch();
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
    if (roomType.roomTypeName.length < 1) {
      message = "name must not be empty";
    }
    else if (image === undefined) {
      message = "image must not be null";
    }
    if (message.length > 0)
      showToastiFy(message, enMessage.ATTENSTION)
    return message.length > 0
  }

  const createOrUpdateRoomType = () => {
    if (!isUpdate) {
      if (validationInput()) {
        return;
      }
    }
    const roomtTypeData = new FormData();

    if (roomType.roomTypeName.length > 0)
      roomtTypeData.append("name", roomType.roomTypeName)

    if (imageHolder !== undefined)
      roomtTypeData.append("image", imageHolder)


    let endPoint = !isUpdate ? import.meta.env.VITE_ROOMTYPE : import.meta.env.VITE_ROOMTYPE + `/${roomType.roomTypeID}`;
    const method = isUpdate ? enApiType.PUT : enApiType.POST;

    roomtypeMutaion.mutate({
      data: roomtTypeData,
      endpoint: endPoint,
      methodType: method,
      jwtToken: token,
      jwtRefreshToken: refreshToken
    })
  }


  const deleteOrUndeleteUser = async (roomtypeid: Guid, isDeleted: boolean) => {
    try {

      await roomtypeMutaion.mutate({
        data: undefined,
        endpoint: import.meta.env.VITE_ROOMTYPE + '/' + roomtypeid,
        methodType: enApiType.DELETE,
        jwtToken: token,
        jwtRefreshToken: refreshToken
      })

      refetch()
      showToastiFy(isDeleted ? "roomtypeDelete Seccessfuly" : "roomtype unDeleted Seccessfuly", enMessage.SECCESSFUL)
    }
    catch (error) {
      showToastiFy(isDeleted ? "could not delete roomtype" : "could not undelete roomtype", enMessage.ERROR)

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

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    changeDraggableStatus(false);
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    changeDraggableStatus(false)
    const file = e.dataTransfer.files[0];
    const fileExtension = file.name.split('.').pop()?.toLowerCase();

    if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
      showToastiFy("You must select a valid image", enMessage.ERROR);
      return;
    }

    const cachedURL = URL.createObjectURL(file);

    const img = new Image();
    img.onload = () => {
      const { naturalHeight, naturalWidth } = img;

      showToastiFy(`Image height: ${naturalHeight}\nImage width: ${naturalWidth}`, enMessage.ERROR);

      //   if (naturalHeight > 40 || naturalWidth > 40) {
      //     showToastiFy("Image must have a minimum height and width of 40px", enMessage.ERROR);
      //     return;
      //   }

      setImage(cachedURL);
      setImageHolder(file)
    };

    img.src = cachedURL;

  }

  useEffect(() => {
    if (error != undefined && error !== null) {
      if ((error as any).status === 401) {
        logoutFn()
      } else {
        showToastiFy(error?.message?.toString() || "An unknown error occurred", enMessage.ERROR)

      }
    }
  }, [error])



  return (
    <div className='flex flex-row'>

      <Header index={2} />

      <div className='min-h-screen w-[calc(100%-192px)] ms-[192px] flex flex-col px-2 items-start  overflow-scroll '>
        <div className='flex flex-row items-center mt-2'>
          <RectangleGroupIcon className='h-8 fill-black group-hover:fill-gray-200 -ms-1' />
          <h3 className='text-2xl ms-1'>RoomType</h3>
        </div>

        <div className='mt-4 flex flex-row flex-wrap gap-1 w-full'>
          <div className='w-full flex flex-col-reverse md:flex-row md:justify-between mb-4'>


            <TextInput
              keyType='roomTypeName'
              value={roomType.roomTypeName}
              onInput={updateInput}
              placeHolder="Name"
              style={`mb-1 w-full`}
              handleWidth='w-full md:w-[200px]'

              maxLength={50}
              isRequire={true}
            />

            <div
              onDrag={draggbleFun}
              onDragOver={draggableOver}
              onDrop={handleDrop}
              onDragLeave={handleDragLeave}
              className={` h-40 w-40 mb-2 md:mb-0 border-[2px] border-dashed border-gray-200 rounded-sm relative ${isDraggable ? 'bg-gray-500' : 'bg-white'}`}>

              <div className='absolute h-full w-full  flex flex-row justify-center items-center '
              >
                <ImageHolder
                  src={image != undefined ? image : roomType?.imagePath ?
                    `${import.meta.env.VITE_MINIO_ENDPOINT}/roomtype/` + roomType.imagePath?.toString() : undefined}
                  style='flex flex-row h-20 w-20 '
                  isFromTop={true} />
              </div>
              <button
                onClick={selectImage}
                className='group absolute end-1 top-2 hover:bg-gray-600 hover:rounded-sm '>
                <PencilIcon className='h-6 w-6 border-[1px] border-blue-900 rounded-sm group-hover:fill-gray-200  ' />
              </button>
              <input
                type="file"
                id="file"
                ref={imageRef}
                onChange={uploadImageDisplayFromSelectInput}
                hidden />
            </div>

          </div>


        </div>
        <div className={'flex flex-row gap-3 mb-2'}>
          <SubmitButton
            onSubmit={async () => createOrUpdateRoomType()}
            // buttonStatus={status}
            placeHolder={isUpdate ? 'update' : 'create'}
            style="text-[10px] bg-mainBg   w-[90px]  text-white rounded-[2px] mt-2 h-6"
          />
          <SubmitButton
            textstyle='text-black'
            onSubmit={async () => clearData()}
            // buttonStatus={status}
            placeHolder={'reseat'}
            style="text-[10px] bg-white border-[1px]   w-[90px]  text-white rounded-[2px] mt-2 h-6"
          />
        </div>
        <RoomTypeTable
          data={data !== undefined ? data.data as unknown as IRoomType[] : undefined}
          setRoomType={setRoomType}
          setUpdate={setUpdate}
          deleteFunc={deleteOrUndeleteUser}
          isShwoingDeleted={isShownDeletion} />

        <h3 className='pt-5'
        >showing deletion roomtype</h3>
        <Switch onChange={() => { changeShowingDeleteionStatus(prev => prev = !prev) }} />
      </div>
    </div>
  )
}

export default RoomType