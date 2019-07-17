import React from 'react';
import logo from './logo.svg';
import './App.css';
import { SampleDataClient, WeatherForecast, SampleData2Client } from './ApiClient.g';

class Props {
  data: WeatherForecast[] | undefined;
  array: string[] | undefined;
}

export default class App extends React.Component<any, Props> {
  readonly state = new Props();
  async componentDidMount() {
    const host = "http://localhost:50342";
    const data: WeatherForecast[] = await new SampleDataClient(host).weatherForecasts();
    this.setState({
      data: data
    });
    const array: string[] = await new SampleData2Client(host).weatherForecasts();
    this.setState({
      array: array
    });

  }

  render() {

    return (

      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <p>
            Edit <code>src/App.tsx</code> and save to reload.
        </p>
          <a
            className="App-link"
            href="https://reactjs.org"
            target="_blank"
            rel="noopener noreferrer"
          >
            Learn React
        </a>
          <label>{JSON.stringify(this.state.data)}</label>
          <label>{JSON.stringify(this.state.array)}</label>
        </header>
      </div>
    );
  }
}

