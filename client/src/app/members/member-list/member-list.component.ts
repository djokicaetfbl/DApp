import { Component, ViewEncapsulation } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent {
  //members$: Observable<Member[]> | undefined; // $ - kako bi oznacio da je nesto Observable
  members: Member[] = [];
  pagination: Pagination | undefined;
  pageNumber = 1;
  pageSize = 5;
  userParams: UserParams | undefined;
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  constructor(
    private memberService: MembersService /*private accountService: AccountService*/
  ) {
    this.userParams = this.memberService.getUserParams();
  }

  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }

  ngOnInit(): void {
    this.loadMembers();
    //this.members$ = this.memberService.getMembers();
  }

  loadMembers() {
    console.log('Djole: ' + this.userParams?.gender);
    if (this.userParams) {
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: (response) => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        },
      });
    }
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      // moramo provjeriti jer je userParams udenfined
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
