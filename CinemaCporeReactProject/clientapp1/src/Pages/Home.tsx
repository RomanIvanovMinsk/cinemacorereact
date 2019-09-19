import React, { Component } from 'react';
import Api, { Movie } from '../Services/Api';
import MovieThumbPreview from '../Components/MovieThumbPreview';
import { CarouselProvider, Slider, Slide, ButtonBack, ButtonNext } from 'pure-react-carousel';
import 'pure-react-carousel/dist/react-carousel.es.css';
import HomeMovieCard from '../Components/HomeMovieCard';
import './Home.scss';
import Loader from '../Components/Loader';


export default class Home extends Component {
    private api: Api;

    public state = {
        movies: [] as Movie[],
        isLoading: true as boolean,
    };

    constructor(props) {
        super(props);

        this.api = new Api();
    }



    async componentDidMount() {
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

    render() {
        return (
            <div className="home">
                {this.state.isLoading ? 
                <div className="loaderContainer">
                <Loader/>
                </div>
                :
                    <CarouselProvider
                        totalSlides={this.state.movies.length}
                        naturalSlideWidth={100}
                        naturalSlideHeight={125}
                        visibleSlides={5}
                    >
                        <Slider>
                            {this.state.movies.map((m, i) => {
                                return (
                                    <Slide index={i}>
                                        <HomeMovieCard key={m.id} movie={m} />
                                    </Slide>
                                );
                            })
                            }
                        </Slider>
                        <ButtonBack>Back</ButtonBack>
                        <ButtonNext>Next</ButtonNext>
                    </CarouselProvider>
                }
            </div>

        );
    }
}