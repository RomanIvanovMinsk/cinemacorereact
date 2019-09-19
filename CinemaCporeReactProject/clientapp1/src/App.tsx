import React from 'react';
import logo from './logo.svg';
import { BrowserRouter as Router, Link, Route, Switch } from 'react-router-dom';
import AuthButton from './Components/AuthButton';
import Login from './Pages/Login';
import Register from './Pages/Register';
import PrivateRoute from './Components/PrivateRoute';
import './App.css';
import Home from './Pages/Home';
import NotFound from './Pages/NotFound';

const Public = () => { return <h3>Public</h3> };
const Private = () => { return (<h3>Protected</h3>) };


class Props {
  array: string[] | undefined;
}

export default class App extends React.Component<any, Props> {
  readonly state = new Props();
  async componentDidMount() {


  }

  render() {
    return (
      <Router>
        <div>
          
            <AuthButton />
            <ul>
              <li><Link to="/protected">Protected Page</Link></li>
            </ul>
            <Switch>
            <Route path="/" exact component={Home} />
            <Route path="/login" component={Login} />
            <Route path="/register" component={Register} />


            <PrivateRoute path='/protected' component={Private} />
            <Route component={NotFound} />
          </Switch>
        </div>
      </Router>


    )

  }
}

