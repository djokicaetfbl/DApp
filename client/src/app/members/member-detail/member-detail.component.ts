import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    MemberMessagesComponent,
  ], // dodajemo import-e module jer je u pitanju standalone komponenta
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent; // nas ViewChild ce sad da bude konstruisan odmah zahvaljujuci ovom static: true
  member: Member = {} as Member; // | undefined; // na ovja nacin smo definicali Member kao prazan objekat
  images: GalleryItem[] | undefined = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  /**Kada koristite static: true, Angular će pokušati da pronađe referencu na element odmah nakon što se konstruktor komponente izvrši, ali pre nego što se dogodi ngOnInit.
To znači da će memberTabs biti dostupan već u ngOnInit metodi, čak i ako je deo statičkog sadržaja stranice (npr. nije uklonjen ili dodat dinamički pomoću *ngIf, *ngFor, itd.).
Koristite ovu opciju kada želite da pristupite elementu ili komponenti u ngOnInit ili ranije, i kada je element prisutan u DOM-u bez obzira na eventualne promene tokom prikazivanja. */

  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presenceService: PresenceService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection(); // zatvri messageHub kad se napusti kompinenta!
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => (this.member = data['member']), // ovdje objekat dobijam pomocu: memberDetailedResolver, koji je prikacen na rutu: members/:username koja ucitava ovu MemberDetailComponent - u
    });

    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });

    this.getImages();
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find((x) => x.heading === heading)!.active = true; // sa ovim ! sam rekao typescript-u da ovo nije undefined i da ignorise upozorenje i slusa mene :D
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
      // this.loadMessages();
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  // loadMessages() { // sad poruke dobijamo preko singalR-a unutar onTabActivated metode!
  //   if (this.member?.userName) {
  //     this.messageService.getMessageThread(this.member.userName).subscribe({
  //       next: (messages) => (this.messages = messages),
  //     });
  //   }
  // }

  // loadMember() { // nema potrebe za loadMember jer sad se koristi memberDetailedResolver, pa se member dobija kao objekat iz njega
  //   var username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;
  //   this.memberService.getMember(username).subscribe({
  //     next: (member) => {
  //       (this.member = member);
  //     },
  //   });
  // }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      if (this.images) {
        this.images.push(
          new ImageItem({
            src: photo.url,
            thumb: photo.url,
          })
        );
      }
    }
  }
}
