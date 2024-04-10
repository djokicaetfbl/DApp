import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  //currentUser$: Observable<User | null> = of(null); // ovo je observable of null :D

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {} // da je i u templejtu (.html-u) dostupan nam accountService

  ngOnInit(): void {
    //this.currentUser$ = this.accountService.currentUser$;
  }

  // getCurrentUser() {
  //   this.accountService.currentUser$.subscribe({
  //     next: (user) => (this.loggedIn = !!user), // turn our user object to boolean, ako postoji logovan korisnik vrati true, ako ne postoji vrati false
  //     error: (error) => console.log(error),
  //   });
  // }

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl('/members'),
      //error: (error) => this.toastr.error(error.error), // nema potrebe ovdje naglasavati error jer imamo interceptor
      //complete: () => console.log('Completed request!'),
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
