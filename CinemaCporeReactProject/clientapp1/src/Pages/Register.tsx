import React, { Component, CSSProperties } from 'react';
import { Redirect } from 'react-router-dom';
import Auth from '../auth';
import LoginBackground from '../img/login_background.jpg';
import Button from '@material-ui/core/Button';
import { Input, TextField } from '@material-ui/core';

const styles = {
    background: {
        background: {
            position: "absolute",
            zIndex: -1,
            display: "block",
            overflow: "hidden",
            minHeight: "100vh",
            height: "100%",
            width: "100%"
        } as CSSProperties,
        image: {
            minHeight: "100%",
            minWidth: "100%",
            filter: "brightness(50%)"
        } as CSSProperties,


    },
    form: {
        formHeader: {
            color: "white",
            fontSize: "32px",
            fontWeight: 700,
            marginBottom: "28px"
        } as CSSProperties,
        formBackground: {
            padding: "60px 68px 40px",
            backgroundColor: "rgba(0,0,0,.75)",
            minHeight: "660px"
        } as CSSProperties,
        form: {
            margin: "0 auto -236px",
            minHeight: "100vh",
            backgroundColor: "transparent",
            maxWidth: "450px",
            display: "block",
            color: "#333",
            padding: "0 5%"
        } as CSSProperties,
        textField: {
            backgroundColor: "#333",
            color: "#ffff",
        } as CSSProperties,
    },

}

class Register extends Component<any> {
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
                <div style={styles.background.background}>
                    <img src={LoginBackground} style={styles.background.image} />
                </div>
                <div style={styles.form.form}>
                    <div style={styles.form.formBackground}>
                        <div style={{ display: "flex", flexDirection: "column" }}>
                            <h1 style={styles.form.formHeader}>Register </h1>
                            <TextField label="Email" required margin="normal" variant="filled"
                                style={styles.form.textField}
                                type="email"
                            />
                            <TextField label="Password" required margin="normal" variant="filled"
                                style={styles.form.textField} type="password" />
                            <div style={{ height: "20px" }} />
                            <Button color="primary" onClick={this.login} variant="contained">Register</Button>

                            <label>Already have an account? <a href="#" onClick={(e) => {
                                e.preventDefault();
                                this.props.history.push('/login');
                            }}
                                style={{
                                    color: "white",
                                    textDecoration: "none"
                                }}>Sign In</a></label>
                        </div>
                    </div>

                </div>
            </div>

        )
    }
}

export default Register;
