import React from 'react'
import { BrowserRouter, withRouter } from 'react-router-dom';
import Auth from './auth';


const AuthButton = withRouter(({ history }) => (
    Auth.isAuthenticated ? (
        <p>
            Welcome! <button onClick={() => {
                 Auth.signout(() => history.push('/'))
            }}>Sign out</button>
        </p>
    ) : (
            <p>You are not logged in.</p>
        )
))

export default AuthButton;