import React, { useContext, useState } from 'react'
import { TextInput } from '../../components/input/textInput'
import SubmitButton from '../../components/button/submitButton'
import { Link } from 'react-router-dom'
import { IAuthModule } from '../../module/iAuthModule';
import { useMutation } from '@tanstack/react-query';
import apiClient from '../../services/apiClient';
import { enApiType } from '../../module/enApiType';
import { generalMessage } from '../../util/generalPrint';
import { useForm, } from 'react-hook-form';
import { enStatus } from '../../module/enState';
import { isHasCapitalLetter, isHasNumber, isHasSmallLetter, isHasSpicalCharacter, isValidEmail } from '../../util/regexValidation';
import { useToastifiContext } from '../../context/toastifyCustom';
import { enMessage } from '../../module/enMessageType';
import { PasswordInput } from '../../components/input/passwordInput';
import { useDispatch } from 'react-redux';
import { setTokens } from '../../controller/redux/jwtSlice';
import { AuthResult } from '../../module/iAuthResult';
import { ISingupModle } from '../../module/iSingupModle';

const SignUp = () => {
    const dispatcher = useDispatch()
    const { showToastiFy } = useContext(useToastifiContext)
    const [isPasswordFocuse, setPasswordFocuse] = useState<boolean | undefined>(undefined)
    const [status, setState] = useState<enStatus>(enStatus.none)
    const { handleSubmit } = useForm();
    const [userAuth, setUser] = useState<ISingupModle>({
        name: 'asdf',
        email: 'asda@gmail.com',
        phone: '7755012257',
        address: 'sadf',
        username: 'asdf',
        password: 'asAS12#$',
    });

    // Update input field in the userAuth state by key
    const updateInput = (value: any, key: string) => {
        setUser((prev) => ({
            ...prev,
            [key]: value,
        }));
    };

    const singup = useMutation({
        mutationFn: (userData: any) =>
            apiClient({
                enType: enApiType.POST,
                endPoint: import.meta.env.VITE_SINGUP,
                prameters: userData


            }),
        onSuccess: (data) => {
            setState(enStatus.complate)
            const result = data.data as unknown as AuthResult;
            dispatcher(setTokens({ accessToken: result.accessToken, refreshToken: result.refreshToken }))
        },
        onError: (error) => {
            setState(enStatus.complate);

            if (error.response) {
                // Extract error message from the server response
                const errorMessage = error?.response || "An error occurred";
                showToastiFy(errorMessage, enMessage.ERROR);
            } else if (error.request) {
                // Handle network errors or no response received
                const requestError = "No response received from server";
                showToastiFy(requestError, enMessage.ERROR);
            } else {
                // Handle other unknown errors
                const unknownError = error.message || "An unknown error occurred";
                showToastiFy(unknownError, enMessage.ERROR);
            }
        }

    })

    const validationInput = () => {

        let validationMessage = "";
        if (userAuth.name.trim().length < 1) {
            validationMessage = "name mustn't be empty"
        }
        else if (userAuth.email.trim().length < 1) {
            validationMessage = "email must not be empty"
        }
        else if (userAuth.phone.length < 1) {
            validationMessage = "phone must not be empty"
        }
        else if (userAuth.username.length < 1) {
            validationMessage = "username must not be empty"
        }
        else if (userAuth.password.length < 1) {
            validationMessage = "password must not be empty"
        }
        else if (!isValidEmail(userAuth.email)) {
            validationMessage = "write valide email"
        }
        else if (userAuth.phone.length < 10) {
            validationMessage = "phone must atleast 10 numbers";
        }
        else if (!isHasCapitalLetter(userAuth.password)) {
            validationMessage = " password must contain 2 capital character"
        }
        else if (!isHasSmallLetter(userAuth.password)) {

            validationMessage = "password must contain 2 small character"
        }
        else if (!isHasSpicalCharacter(userAuth.password)) {
            validationMessage = "password must contain 2 special character";
        }
        else if (!isHasNumber(userAuth.password)) {
            validationMessage = " password must contain 2 number"
        }

        if (validationMessage.length > 0)
            showToastiFy(validationMessage, enMessage.ATTENSTION)

        return validationMessage.length > 0;
    }
    const onSubmit = async () => {
        if (validationInput()) {
            return;
        }
        setState(enStatus.loading)
        const data = {
            "name": userAuth.name,
            "email": userAuth.email,
            "phone": userAuth.phone,
            "address": userAuth.address,
            "userName": userAuth.username,
            "password": userAuth.password,
        }
        await singup.mutate(data)

    };



    return (
        <div className='h-screen w-screen flex flex-col items-center justify-center '
        >

            <TextInput

                keyType='name'
                value={userAuth.name}
                onInput={updateInput}
                placeHolder="name"
                style="mb-1"
                maxLength={50}
                isRequire={true}
            />
            <TextInput
                keyType='email'
                value={userAuth.email}
                onInput={updateInput}
                placeHolder="email"
                style="mb-1"
                maxLength={100}
                isRequire={true}

            />

            
            <TextInput
                keyType='address'
                value={userAuth.address}
                onInput={updateInput}
                placeHolder="Yemen Sanaa"
                style="mb-1 "
                isRequire={true}

            />
            <TextInput
                keyType='phone'
                value={userAuth.phone}
                onInput={updateInput}
                placeHolder="735501225"
                style="mb-1"
                isRequire={true}
                maxLength={10}
                type='number'
            />
            <TextInput
                keyType='username'
                value={userAuth.username}
                onInput={updateInput}
                placeHolder="username"
                style="mb-1"
                isRequire={true}
                maxLength={50}

            />



            <PasswordInput
                keyType='password'
                value={userAuth.password}
                onInput={updateInput}
                placeHolder="*****"

                isRequire={true}
                canShowOrHidePassowrd={true}
                maxLength={8}
            />





            {<SubmitButton
                onSubmit={() => onSubmit()}
                buttonStatus={status}
                placeHolder={'Sign Up'}
                // onSubmit={() => { }}
                style="text-[10px] bg-mainBg w-[200px]  text-white rounded-[4px] mt-5 h-6"
            />}
            <div className="w-[200px] justify-end">
                <Link to={'/login'} className="text-[8px] text-black hover:text-blue-600">
                    Go to login
                </Link>
            </div>
        </div>
    );
};

export default SignUp;

/*
"{\"type\":\"https://tools.ietf.org/html/rfc9110#section-15.5.1\",
\"title\":\"One or more validation errors occurred.\",\"status\":400,\"errors\":
{\"phone\":[\"The field phone must be a string or array type with a minimum length of '10'.\"]},\"traceId\":\"00-c4559e5ca9da0cd344902a038a99e6ee-0fc0ef973a456a37-00\"}"

*/
