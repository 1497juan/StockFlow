
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
  tokenType: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5054/api/Auth';

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap((response) => {
          localStorage.setItem('stockflow_token', response.accessToken);
          localStorage.setItem('stockflow_token_expiration', response.expiresAtUtc);
        }),
      );
  }

  getToken(): string | null {
    return localStorage.getItem('stockflow_token');
  }

  logout(): void {
    localStorage.removeItem('stockflow_token');
    localStorage.removeItem('stockflow_token_expiration');
  }
}
