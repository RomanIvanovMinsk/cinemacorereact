import React from 'react';
import { Link } from 'react-router-dom';
import './NotFound.scss';
import NotFoundImage from '../img/404-page-not-found.png';

const NotFound = () => {
    return (<h1 className="notFound">
        Requested page is not found.
        <div>
            <Link to="/">Go to main page</Link>
        </div>
        <img src={NotFoundImage} style={{marginTop: 50}}/>
    </h1>)
}

export default NotFound;