export interface Government {
  id: string;
  name: string;
  cities?: City[];
}

export interface City {
  id: string;
  name: string;
  governmentId: string;
  governmentName?: string;
}
