export default class Api {
  static movieId: number = 0;
  public getCurrentMovies(): Promise<Movie[]> {
    return this.getMovies(0, 20);
  }

  public getMovies(page: number, results: number): Promise<Movie[]> {
    if (results <= 0) results = 10;
    const array = new Array<Movie>();
    for (let i = 0; i < results; i++) {
      array.push(this.generateMovie());
    }
    return Promise.resolve(array);
  }

  private generateMovie(): Movie {
    const m = new Movie();
    m.id = (Api.movieId++).toString();
    m.image = "https://picsum.photos/200/300?q=" + Math.random();

    const coin = this.getRandomInt(2);
    switch (coin) {
      case 0:
        m.title = "Форсаж: Хоббс и Шоу";
        m.genries = ["боевик", "приключения"];
        break;
      case 1:
        m.title = "Тайна печати дракона";
        m.genries = ["приключения"];
        break;
    }
    return m;
  }

  private getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
  }
}

export class Movie {
  constructor() {
    this.title = "";
    this.id = "";
    this.image = "";
  }

  public id: string;
  public image: string;
  public title: string;
  public genries: string[] = [];
}
