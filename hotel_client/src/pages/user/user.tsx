import React, { DragEventHandler, useContext, useEffect, useRef, useState } from 'react'
import Header from '../../components/header/header'
import { CameraIcon, UsersIcon } from '@heroicons/react/16/solid'
import { IAuthModule } from '../../module/iAuthModule';
import { TextInput } from '../../components/input/textInput';
import { PasswordInput } from '../../components/input/passwordInput';
import SubmitButton from '../../components/button/submitButton';
import { enStatus } from '../../module/enState';
import UersTable from '../../components/tables/usersTable';
import { notifyManager, useMutation, useQuery } from '@tanstack/react-query';
import apiClient from '../../services/apiClient';
import { enApiType } from '../../module/enApiType';
import NotFoundPage from '../NotFound/notfound';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../controller/rootReducer';
import { generalMessage } from '../../util/generalPrint';
import { useToastifiContext } from '../../context/toastifyCustom';
import { enMessage } from '../../module/enMessageType';
import NotFoundComponent from '../../components/notFoundContent';
import { isHasCapitalLetter, isHasNumber, isHasSmallLetter, isHasSpicalCharacter, isValidEmail } from '../../util/regexValidation';
import { Guid } from 'guid-typescript';
import { Switch } from '@mui/material';
import ImageHolder from '../../components/imageHolder';
import { logout } from '../../controller/redux/jwtSlice';
import { IUserModule } from '../../module/iUserModule';

const User = () => {
  const dispatch = useDispatch()


  const { showToastiFy } = useContext(useToastifiContext)
  const imageRef = useRef<HTMLInputElement>(null)
  const [image, setImage] = useState<string | undefined>(undefined)

  const refreshToken = useSelector((state: RootState) => state.auth.refreshToken)
  const token = useSelector((state: RootState) => state.auth.token)

  const [status, setState] = useState<enStatus>(enStatus.none)
  const [page, setPage] = useState<number>(1)
  const [isNoData, setNoData] = useState<boolean>(false)
  const [isShowingDeleted, setShowingDeleted] = useState<boolean>(false)
  const [isDraggable, changeDraggableStatus] = useState(false)

  const [userHolder, setUser] = useState<IAuthModule>({
    userId: undefined,
    name: '',
    email: '',
    phone: '',
    address: '',
    username: '',
    password: '',
    brithDay: (new Date()).toISOString().split('T')[0],
    imagePath: undefined
  });

  const [isUpdate, setUpdate] = useState<boolean>(false)

  const updateInput = (value: any, key: string) => {
    setUser((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  const { data, error, refetch } = useQuery({
    queryKey: ['users'],
    queryFn: async () => apiClient({
      enType: enApiType.GET,
      endPoint: import.meta.env.VITE_USERS + `${page}`,
      parameters: undefined,
      isRequireAuth: true,
      jwtValue: token || "",
      jwtRefresh: refreshToken || "",
      tryNumber: 1
    }),
  }
  );
  const logoutFn = () => {
    dispatch(logout())
  }


  const userMutaion = useMutation({
    mutationFn: ({
      data,
      endpoint,
      methodType,
      jwtToken,
      jwtRerreshToken
    }: {
      data?: FormData | undefined,
      endpoint: string,
      methodType: enApiType,
      jwtToken?: string | null,
      jwtRerreshToken?: string | null
    }) =>
      apiClient({
        enType: methodType,
        endPoint: endpoint
        , parameters: data,
        isRequireAuth: true,
        jwtValue: jwtToken ?? undefined,
        jwtRefresh: jwtRerreshToken ?? undefined,
        isFormData: data != undefined,
        tryNumber: 1
      }),
    onSuccess: (data) => {
      setState(enStatus.complate)
      showToastiFy(`user ${isUpdate ? "updated" : "created"} Sueccessfuly`, enMessage.SECCESSFUL);
      setUser({
        userId: undefined,
        address: '',
        password: '',
        email: '',
        name: '',
        phone: '',
        username: '',
        brithDay: (new Date()).toISOString().split('T')[0]
      })
      setImage(undefined)
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

    let validationMessage = "";
    if (userHolder.name.trim().length < 1) {
      validationMessage = "name mustn't be empty"
    }
    else if (userHolder.email.trim().length < 1) {
      validationMessage = "email must not be empty"
    }
    else if (userHolder.brithDay === undefined) {
      validationMessage = "brithday must not be empty"
    }
    else if (userHolder.phone.length < 1) {
      validationMessage = "phone must not be empty"
    }
    else if (userHolder.username.length < 1) {
      validationMessage = "username must not be empty"
    }
    else if (userHolder.password.length < 1) {
      validationMessage = "password must not be empty"
    }
    else if (!isValidEmail(userHolder.email)) {
      validationMessage = "write valide email"
    }
    else if (userHolder.phone.length < 10) {
      validationMessage = "phone must atleast 10 numbers";
    }
    else if (!isHasCapitalLetter(userHolder.password)) {
      validationMessage = " password must contain 2 capital character"
    }
    else if (!isHasSmallLetter(userHolder.password)) {

      validationMessage = "password must contain 2 small character"
    }
    else if (!isHasSpicalCharacter(userHolder.password)) {
      validationMessage = "password must contain 2 special character";
    }
    else if (!isHasNumber(userHolder.password)) {
      validationMessage = " password must contain 2 number"
    }

    if (validationMessage.length > 0)
      showToastiFy(validationMessage, enMessage.ATTENSTION)

    return validationMessage.length > 0;
  }


  const createOrUpdateUser = async () => {
    if (!isUpdate)
      if (validationInput()) {
        return;
      }
    setState(enStatus.loading)
    const formData = new FormData();
    formData.append("name", userHolder.name);
    formData.append("email", userHolder.email);
    formData.append("phone", userHolder.phone);
    formData.append("userName", userHolder.username);
    formData.append("password", userHolder.password);
    formData.append("brithDay", new Date(userHolder.brithDay).toISOString());
    formData.append("isVip", "false");

    if (userHolder.address !== '')
      formData.append("address", userHolder.address ?? "");


    if (userHolder.imagePath !== undefined)
      formData.append("imagePath", userHolder.imagePath);


    let endPoint = isUpdate ? import.meta.env.VITE_USER + `/${userHolder.userId}` : import.meta.env.VITE_USER;

    await userMutaion.mutate({
      data: formData,
      endpoint: endPoint,
      methodType: isUpdate ? enApiType.PUT : enApiType.POST,
      jwtToken: token,
      jwtRerreshToken: refreshToken
    })

  };


  const deleteOrUndeleteUser = async (userId: Guid) => {
    let endpoint = import.meta.env.VITE_USER

    await userMutaion.mutate({
      data: undefined,
      endpoint: endpoint + '/' + userId,
      methodType: enApiType.DELETE,
      jwtToken: token,
      jwtRerreshToken: refreshToken
    });

  };


  const makeUserVip = async (userId: Guid) => {
    let endpoint = import.meta.env.VITE_USER

    await userMutaion.mutate({
      data: undefined,
      endpoint: endpoint + '/' + userId,
      methodType: enApiType.POST,
      jwtToken: token,
      jwtRerreshToken: refreshToken
    });

  };


  const selectImage = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    imageRef.current?.click();
  }

  const uploadImageDisplay = async (e: React.ChangeEvent<HTMLInputElement>) => {
    e.preventDefault()
    if (imageRef.current && imageRef.current.files && imageRef.current.files[0]) {

      const uploadedFile = imageRef.current.files[0];
      const fileExtension = uploadedFile.name.split('.').pop()?.toLowerCase();
      if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
        showToastiFy("you must select valide image ", enMessage.ERROR)
        return;
      }
      const cachedURL = URL.createObjectURL(uploadedFile);
      setImage(cachedURL);
      setUser((prev) => ({
        ...prev,
        ['imagePath']: uploadedFile,
      }));
    }
  }


  const draggbleFun = (e: React.DragEvent) => {
    e.preventDefault();
    changeDraggableStatus(true)
  }

  const draggableOver = (e: React.DragEvent) => {
    e.preventDefault();
    changeDraggableStatus(true)
  }
  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault()
    changeDraggableStatus(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    changeDraggableStatus(false)
    const file = e.dataTransfer.files[0];
    const fileExtension = file.name.split('.').pop()?.toLowerCase();
    if (!['png', 'jpg', 'jpeg'].includes(fileExtension || '')) {
      showToastiFy("you must select valide image ", enMessage.ERROR)
      return;
    }
    const cachedURL = URL.createObjectURL(file);
    setImage(cachedURL);
    setUser((prev) => ({
      ...prev,
      ['imagePath']: file,
    }));

  }




  useEffect(() => {
    if (error) {
      setNoData(true)
      showToastiFy(error.message, enMessage.ERROR);
    }
    if (data) {
      setNoData(false)
    }
  }, [error, data])



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

      <Header index={1} />

      <div className='min-h-screen w-[calc(100%-192px)] ms-[192px] flex flex-col px-2 items-start  overflow-scroll '>
        <div className='flex flex-row items-center mt-2'>
          <UsersIcon className='h-8 fill-black group-hover:fill-gray-200 -ms-1' />
          <h3 className='text-2xl ms-1'>Users</h3>
        </div>
        <div className='relative'>
          <div
            onDrag={draggbleFun}
            onDragOver={draggableOver}
            onDrop={handleDrop}
            onDragLeave={handleDragLeave}

            className={`h-20 w-20 ${isDraggable ? 'bg-gray-500' : 'bg-green-400'} rounded-full mt-4 flex flex-row items-center justify-center  overflow-hidden
          `}>

            <ImageHolder
              src={image != undefined ? image : userHolder?.imagePath ?
                `${import.meta.env.VITE_MINIO_ENDPOINT}/user/` + userHolder.imagePath?.toString() : undefined}
              style='flex flex-row h-20 w-20 '
              isFromTop={true} />


          </div>
          <button
            onClick={selectImage}
            className='absolute bg-gray-300 rounded-full p-1 end-1 -bottom-1'>
            <CameraIcon className='h-4 w-4' />
          </button>

          <input
            type="file"
            id="file"
            ref={imageRef}
            onChange={uploadImageDisplay}
            hidden />
        </div>


        <div className='mt-4 flex flex-row flex-wrap gap-1'>



          <TextInput

            keyType='name'
            value={userHolder.name}
            onInput={updateInput}
            placeHolder="name"
            style={`mb-1 w-full md:w-[200px]`}
            maxLength={50}
            isRequire={true}
          />
          <TextInput
            isDisabled={isUpdate}
            keyType='email'
            value={userHolder.email}
            onInput={updateInput}
            placeHolder="email"
            style={`mb-1  ${isUpdate && 'text-gray-400'} w-full md:w-[200px]`}
            maxLength={100}
            isRequire={true}

          />

          <TextInput
            keyType='brithDay'
            value={userHolder.brithDay}
            onInput={updateInput}
            placeHolder="2020/01/20"
            type="date"
            style="mb-1 w-full md:w-[200px]"
            isRequire={true}

          />

          <TextInput
            keyType='phone'
            value={userHolder.phone}
            onInput={updateInput}
            placeHolder="735501225"
            style="mb-1 w-full md:w-[200px]"
            isRequire={true}
            maxLength={10}
            type='number'
          />
          <TextInput
            keyType='username'
            value={userHolder.username}
            onInput={updateInput}
            placeHolder="username"
            style="mb-1 w-full md:w-[200px]"
            isRequire={true}
            maxLength={50}

          />
          <PasswordInput
            keyType='password'
            value={userHolder.password}
            onInput={updateInput}
            placeHolder="*****"

            isRequire={true}
            canShowOrHidePassowrd={true}
            maxLength={8}
          />

          <div className='w-full'>

            <TextInput
              keyType='address'
              value={userHolder.address}
              onInput={updateInput}
              placeHolder="address"
              style="h-16 w-full"
              isRequire={true}
              isMultipleLine={true}
            />

          </div>

          <SubmitButton
            onSubmit={() => createOrUpdateUser()}
            buttonStatus={status}
            placeHolder={isUpdate ? 'update' : 'create'}
            style="text-[10px] bg-mainBg w-[120px] text-white rounded-[2px] mt-2 h-6"
          />

        </div>
        <h3 className='text-2xl font-bold mt-2'>users Data : </h3>

        <div className="overflow-x-auto   w-full mt-4 mb-5">
          <UersTable
            data={data !== undefined ? (data.data as IUserModule[]) : undefined}
            setUser={setUser}
            seUpdate={setUpdate}
            makeUserVip={makeUserVip}
            deleteFunc={deleteOrUndeleteUser}
            isShwoingDeleted={isShowingDeleted}
          />
        </div>
        <div>
          <h3>showing the deleted user</h3>
          <Switch onChange={() => setShowingDeleted(prev => !prev)} />
        </div>
      </div>
    </div>
  )
}

export default User