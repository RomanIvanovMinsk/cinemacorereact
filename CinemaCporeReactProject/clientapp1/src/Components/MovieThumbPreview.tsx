import React, { Component, PureComponent } from 'react';
import './MovieThumbPreview.scss';

class Type {
    image: string = '' 
}

export default class MovieThumbPreview extends PureComponent<Type>{
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <span>
                <img src={this.props.image} className="image" />
            </span>

        );
    }
}