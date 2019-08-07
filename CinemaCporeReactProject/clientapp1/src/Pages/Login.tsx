import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import Auth from '../auth';
import LoginBackground from '../img/login_background.jpg';
import Button from '@material-ui/core/Button';
import { Input, TextField } from '@material-ui/core';


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
            <div>
                <div style={{
                    position: "absolute",
                    zIndex: -1,
                    display: "block",
                    overflow: "hidden",
                    minHeight: "100vh",
                    height: "100%",
                    width: "100%"
                }}>
                    <img src={LoginBackground} style={{
                        minHeight: "100%",
                        minWidth: "100%",
                        filter: "brightness(50%)"
                    }} />
                </div>
                <div
                    style={{
                        margin: "0 auto -236px",
                        minHeight: "100vh",
                        backgroundColor: "transparent",
                        maxWidth: "450px",
                        display: "block",
                        color: "#333",
                        padding: "0 5%"
                    }}
                >
                    <div style={{
                        padding: "60px 68px 40px",
                        backgroundColor: "rgba(0,0,0,.75)",
                        minHeight: "660px"
                    }}>
                        <div style={{ display: "flex", flexDirection: "column" }}>
                            <h1
                                style={{
                                    color: "white",
                                    fontSize: "32px",
                                    fontWeight: 700,
                                    marginBottom: "28px"
                                }}
                            >Sign In</h1>
                            <TextField label="Email" required margin="normal" variant="filled"
                                style={{
                                    backgroundColor: "#333",
                                    color: "#ffff",
                                }}
                                type="email"
                            />
                            <TextField label="Password" required margin="normal" variant="filled"
                                style={{
                                    backgroundColor: "#333",
                                    color: "#ffff",
                                }} type="password" />
                            <div style={{height: "20px"}}/>
                            <Button color="primary" onClick={this.login} variant="contained">Sign In</Button>

                            <label>New to Netflix? <a href="#" onClick={(e) => {
                                e.preventDefault();
                                this.props.history.push('/register');
                            }}
                            style={{color: "white",
                            textDecoration: "none"}}>Sign up now</a></label>
                        </div>
                    </div>

                </div>
            </div>

        )
    }
}

export default Login;
