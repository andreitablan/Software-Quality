import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  constructor(private httpClient: HttpClient) {}

  sendLetter(letter: string) {
    return this.httpClient
      .post<Boolean>('http://localhost:5135/api/memory/letter', letter)
      .subscribe();
  }
}
