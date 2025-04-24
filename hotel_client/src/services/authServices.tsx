import axios from "axios"
import { generalMessage } from "../util/generalPrint";
import { IAuthModule } from "../module/iAuthModule";
import { AuthResult } from "../module/iAuthResult";
import { enStatus } from "../module/enState";

export const isValidToken = async (token: string) => {
    let isValid = false;
    await axios.post('/auth-validation', {
        tokenPram: token
    }).then((result) => {
        isValid = true;
    })
        .catch((error) => {

        })
    return isValid;
}

export const signUpNewUser = async (userData: IAuthModule, changeState: (status: enStatus) => void) => {
    let isSignup = false;
    changeState(enStatus.loading)
    await axios.post(
        `${import.meta.env.BASE_URL}${import.meta.env.LOGIN_USER}`,
        {
            name: userData.username,
            email: userData.email,
            phone: userData.phone,
            address: userData.address,
            userName: userData.username,
            password: userData.password,
            brithDay: userData.brithDay
        })
        .then((data) => {
            changeState(enStatus.complate)

            if (data !== undefined) {
                var result = data as unknown as AuthResult;

            }
        })
        .catch((error) => {
            
            changeState(enStatus.complate)

        });


}