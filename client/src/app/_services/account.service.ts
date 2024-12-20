import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment.development';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl; //'https://localhost:5001/api/';
  //BehaviourSubject daje inicijalnu vrijednost, pomocu njega sirom aplikacije dobijamo informaciju o tome da li je korisnik logovan
  private currentUserSource = new BehaviorSubject<User | null>(null); // moze biti User ili null
  currentUser$ = this.currentUserSource.asObservable(); // dolar naglasava da je u pitanju observable

  constructor(
    private http: HttpClient,
    private presenceService: PresenceService
  ) {} //PresenceService je za potrebe SignalR-a

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role; // JSON role atribut vraca metoda pa mu odmah pristupamo
    Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);
    //localStorage.setItem('user', JSON.stringify(user.username));
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    this.presenceService.createHubConnection(user); // da slusa za potrebe SignalR-a konekcija Hub metode
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
