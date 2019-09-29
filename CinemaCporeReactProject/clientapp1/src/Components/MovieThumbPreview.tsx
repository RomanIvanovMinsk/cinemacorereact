import React, { Component, PureComponent } from 'react';
import './MovieThumbPreview.scss';
import LazyLoad from 'react-lazyload';

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
                <LazyLoad
                    height="100%"
                    offsetVertical={400}>
                    <img src={this.props.image} className="image" />
                </LazyLoad>
            </span>

        );
    }
}