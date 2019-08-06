import React, { Component } from 'react';
import { Route, Redirect, RouteProps } from 'react-router-dom';
import Auth from '../auth';

export interface ProtectedRouteProps extends RouteProps {
}

export default class PrivateRoute extends Route<ProtectedRouteProps>
{
    public render() {
        const isAuthenticated: boolean = Auth.isAuthenticated;
        if (isAuthenticated) {
            return <Route {...this.props} />;
        }

        const renderComponent = () => (<Redirect to={{
            pathname: '/login',
            state: { from: this.props.path }
        }} />);
        return <Route {...this.props} component={renderComponent} />;
    }
}

// const PrivateRoute = ({ ...rest }) => {
//     const isAuthenticated = Auth.isAuthenticated;
//     return (
//         <Route {...rest} render={(props) => {
//             const x = isAuthenticated;
//             console.error(isAuthenticated);
//             return (

//                 isAuthenticated === true
//                     ? <Component {...props} />
//                     : <Redirect to={{
//                         pathname: '/login',
//                         state: { from: props.location }
//                     }} />
//             );
//         }} />)
// };

// export default PrivateRoute;


