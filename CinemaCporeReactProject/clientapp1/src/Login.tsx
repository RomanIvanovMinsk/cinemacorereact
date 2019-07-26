import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import Auth from './auth';


class Login extends Component<any> {
    state = {
        redirectToReferrer: false
    }
    login = () => {
        Auth.authenticate( () => {
            this.setState(() => ({
                redirectToReferrer: true
            }))
        })
    }
    render() {
        const { from } = this.props.location.state || { from: { pathname: '/' } }
        const { redirectToReferrer } = this.state

        if (redirectToReferrer === true) {
            return <Redirect to={from} />
        }

        return (
            <div>
                <p>You must log in to view the page</p>
                <button onClick={this.login}>Log in</button>
            </div>
        )
    }
}

export default Login;
