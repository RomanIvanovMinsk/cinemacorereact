﻿import { Function } from "@babel/types";

class Authentication{
    public isAuthenticated: boolean = false;

    public authenticate(): Promise<any> {
        this.isAuthenticated = true;
        return Promise.resolve();       
    }

    public signOut() : Promise<any> {
        this.isAuthenticated = false;
        return Promise.resolve();
    }
}

const Auth = new Authentication();

export default Auth;
