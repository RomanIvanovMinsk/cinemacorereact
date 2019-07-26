import { Function } from "@babel/types";

const Auth = {
    isAuthenticated: false,
    authenticate(callback: any) {
        this.isAuthenticated = true       
    },
    signout(callback: any) {
        this.isAuthenticated = false       
    }
}



export default Auth