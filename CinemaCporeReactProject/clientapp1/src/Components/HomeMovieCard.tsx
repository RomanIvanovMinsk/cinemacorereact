import React, { Component, PureComponent } from 'react'
import { Movie } from '../Services/Api';
import MovieThumbPreview from './MovieThumbPreview';
import './HomeMovieCard.scss';

class Type {
    constructor() {
        this.movie = new Movie();
    }

    movie: Movie;
    style?: React.CSSProperties = {}
}

export default class HomeMovieCard extends PureComponent<Type>{
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <span className="card" style={this.props.style}>
                <MovieThumbPreview image={this.props.movie.image} />
                <div className="title">
                    <label>{this.props.movie.title}</label>
                </div>
                <div className="genries">
                    {this.props.movie.genries.join(', ') || "_"}
                </div>
                <button className="button">
                    Купить билет
                </button>
            </span>
        );
    }
}