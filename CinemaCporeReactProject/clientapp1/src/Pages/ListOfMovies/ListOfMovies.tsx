import React, { Component } from 'react';
import "./ListOfMovies.scss";
import { Cinema } from '../../Types/Cinema';
import CinemaComboBox from '../../Components/CinemaComboBox/CinemaComboBox';
import DateComboBox from '../../Components/DateComboBox/DateComboBox';
import SessionsComboBox from '../../Components/SessionsComboBox/SessionsComboBox';
import FormatComboBox from '../../Components/FormatComboBox/FormatComboBox';
import Api, { Movie } from '../../Services/Api';
import HomeMovieCard from '../../Components/HomeMovieCard';
import InfiniteScroll from 'react-infinite-scroller';
import Loader from '../../Components/Loader';
import LazyLoad from 'react-lazyload';




export default class ListOfMovies extends Component {
    state = {
        cinemas: [] as Array<Cinema>,
        dates: [] as Array<Date>,
        movies: [] as Array<Movie>,
        isLoading: false as Boolean,
    };

    private api: Api;

    constructor(props) {
        super(props);
        const now = new Date();
        this.state.dates = [
            now,
            new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1),
            new Date(now.getFullYear(), now.getMonth(), now.getDate() + 2),
            new Date(now.getFullYear(), now.getMonth(), now.getDate() + 3),
            new Date(now.getFullYear(), now.getMonth(), now.getDate() + 4),
            new Date(now.getFullYear(), now.getMonth(), now.getDate() + 5),
            new Date(now.getFullYear(), now.getMonth(), now.getDate() + 6),
        ];

        this.api = new Api();
    }





    private async refreshPage() {
        try {
            const movies = await this.api.getCurrentMovies();
            this.setState({ movies: movies });
        }
        catch (e) {
            console.error(e);
        }
        finally {
            this.setState({ isLoading: false });
        }
    }

    componentDidMount() {
        const cinemas = [
            {
                id: '1',
                name: "Galileo",
                address: "..., Minsk"
            } as Cinema,
            {
                id: '2',
                name: "Arena City",
                address: "..., Minsk"
            } as Cinema,
            {
                id: '3',
                name: "Voka Cinema",
                address: "..., Minsk"
            } as Cinema
        ]
        this.setState({ cinemas: cinemas });
        this.refreshPage();
    }

    loadMore = async (page) => {
        const newMovies = await this.api.getMovies(page, 10);
        const movies = this.state.movies;
        const newArrayOfMovies = movies.concat(newMovies);
        this.setState({ movies: newArrayOfMovies });
    }

    render() {
        return (
            <div className="listOfMovies">
                <div style={{ display: "flex", flexDirection: "row", alignContent: "center" }}>
                    <h1 style={{ flex: 1 }}>Афиша кино</h1>
                    <label style={{ alignSelf: "center", alignContent: "flex-endf" }}>Сейчас в кино</label>
                </div>
                <div style={{ display: 'flex', flexDirection: "row", flexWrap: "wrap" }}>
                    <CinemaComboBox Cinemas={this.state.cinemas} style={{ flex: 1 }} itemSelected={this.cinemaSelected} />
                    <DateComboBox Dates={this.state.dates} style={{ flex: 1 }} itemSelected={this.dateSelected} />
                    <SessionsComboBox style={{ flex: 1 }} />
                    <FormatComboBox style={{ flex: 1 }} />
                </div>
                {/* <div style={{ display: "flex", flexDirection: "row", flexWrap: "wrap", justifyContent: "space-evenly" }}>
                    {this.state.movies.map(m => {
                        return (
                            <HomeMovieCard key={m.id} movie={m} style={{ margin: 10 }} />
                        );
                    })}
                </div> */}
                <InfiniteScroll pageStart={0}
                    loadMore={this.loadMore}
                    initialLoad={false}
                    hasMore={true}
                    loader={<Loader key="-1" />}
                    thresh
                >
                    <div style={{ display: "flex", flexDirection: "row", flexWrap: "wrap", justifyContent: "space-evenly" }}>
                        {this.state.movies.map(m => {
                            return (
                                
                                <HomeMovieCard key={m.id} movie={m} style={{ margin: 10 }} />
                                
                            );
                        })}
                    </div>
                </InfiniteScroll>
            </div >
        );
    }

    private cinemaSelected = (cinema: Cinema) => {
        this.refreshPage();
    }

    private dateSelected = (d: Date) => {
        this.refreshPage();
    }
}