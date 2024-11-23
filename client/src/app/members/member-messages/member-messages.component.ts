import { CommonModule } from '@angular/common';
import { Component, Input, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
//import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule],
  standalone: true,
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  // @Input() messages: Message[] = []; ovo nam ne treba vise jer koristimo signalR
  messageContent = '';

  constructor(public messageService: MessageService) {}

  ngOnInit(): void {}

  sendMessage() {
    if (!this.username) return;
    this.messageService
      .sendMessage(this.username, this.messageContent)
      // .subscribe({
      // next: (message) => {
      // this.messages.push(message); ovo nam ne treba vise jer koristimo signalR
      // this.messageForm?.reset();
      // },
      // });
      .then(() => {
        this.messageForm?.reset();
      });
  }
}
