import React from 'react';
import logo from './logo.svg';
import { BrowserRouter, Link, Route } from 'react-router-dom';
import AuthButton from './Components/AuthButton';
import Login from './Pages/Login';
import Register from './Pages/Register';
import PrivateRoute from './Components/PrivateRoute';
import './App.css';

const Public = () => {return  <h3>Public</h3>};
const Private = () => {return (<h3>Protected</h3>)};


class Props {
  array: string[] | undefined;
}

export default class App extends React.Component<any, Props> {
  readonly state = new Props();
  async componentDidMount() {


  }

  render() {
        return (
            <BrowserRouter>
              <div>
                  <AuthButton />
                  <Route path="/" component={Login} />
                  <Route path="/register" component={Register} />
                  <PrivateRoute path='/protected' component={Private} />
              </div>
            </BrowserRouter>     
          
          
     )
    
  }
}

