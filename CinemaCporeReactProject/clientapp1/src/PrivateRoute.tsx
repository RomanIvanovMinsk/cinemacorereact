﻿import React, { Component } from 'react';
import { Route, Redirect } from 'react-router-dom';
import Auth from './auth';


const PrivateRoute = ({ ...rest}) => (
    <Route {...rest} render={(props) => (
        Auth.isAuthenticated === true
            ? <Component {...props} />
            : <Redirect to={{
                pathname: '/login',
                state: { from: props.location }
            }} />
    )} />)

export default PrivateRoute;


