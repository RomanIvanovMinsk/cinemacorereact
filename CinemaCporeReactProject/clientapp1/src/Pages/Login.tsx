import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import Auth from '../auth';


class Login extends Component<any> {
    state = {
        redirectToReferrer: false
    }
    public login = (e) => {
        e.preventDefault();
        const x = Auth;
        Auth.authenticate().then(x => {
            this.setState({
                redirectToReferrer: true
            })
        });
    }
    render() {
        const { from } = this.props.location.state || { from: { pathname: '/' } }
        const { redirectToReferrer } = this.state

        if (redirectToReferrer === true) {
            return <Redirect to={from} />
        }

        return (
            <div style={{ display: "flex", flexDirection: "column" }}>
                <p>You must log in to view the page</p>
                <input type="email" />
                <input type="password" />
                <button onClick={this.login}>Log in</button>
            </div>
        )
    }
}

export default Login;
