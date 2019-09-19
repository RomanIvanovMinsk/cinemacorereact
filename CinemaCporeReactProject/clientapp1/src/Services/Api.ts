export default class Api {
  public getCurrentMovies(): Promise<Movie[]> {
    const m = new Movie();
    m.id = "0";
    m.image = "https://picsum.photos/200/300";
    m.title = "Форсаж: Хоббс и Шоу";
    m.genries = ["боевик", "приключения"];

    const m1 = new Movie();
    m1.id = "1";
    m1.image = "https://picsum.photos/200/300";
    m1.title = "Тайна печати дракона";
    m.genries = ["приключения"];

    return Promise.resolve([m, m1, m, m1, m, m, m1,m1]);
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
