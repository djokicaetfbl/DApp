import {
  Directive,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]', // *appHasRole = '["Admin", "Thing"]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  user: User = {} as User;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService
  ) {
    // kad koristimo pipe ne potrebe da vrsimo unsubsribe
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        } else {
          console.log('NE UCITA USER-a');
          this.viewContainerRef.clear(); // If there's no user, clear the view
        }
      },
    });
  }
  //zbog asingronog izvrsavanja desava se raceCondition pa je bolje logiku iz ngOnInit prebaciti u posebnu metodu, koja se izvrsiti nakon sto se izvrsi subscribe user-a
  ngOnInit(): void {
    // console.log('AAA: ' + this.user.username);
    // console.log('DD: ' + this.user.roles);
    if (this.user.roles.some((r) => this.appHasRole.includes(r))) {
      //ako je moja rola onda prikazi sadrzaj
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

  // By moving the view rendering logic (updateView()) inside the subscription, you ensure the roles are checked after the user data is fetched.
  // This avoids potential race conditions between the directive's initialization and the currentUser$ observable's response.

  private updateView() {
    if (this.user) {
      console.log('CCC: ' + this.user.username);
      console.log('DRRRD: ' + this.user.roles);
    } else {
      console.log('UNDEFFFINED');
    }

    if (this.user && this.user.roles.some((r) => this.appHasRole.includes(r))) {
      //ako je moja rola onda prikazi sadrzaj
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
