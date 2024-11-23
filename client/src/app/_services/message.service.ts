import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { group } from '@angular/animations';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) {}

  /*
    The accessTokenFactory in this context is a function used to generate an access token when establishing a SignalR connection with a hub. 
    In your Angular code, it’s part of the configuration for connecting to the SignalR hub.

    Here’s a breakdown of what it does:

    HubConnectionBuilder: Creates a new connection to a SignalR hub.
    .withUrl(...): Specifies the hub URL for the connection. In your case, it’s this.hubUrl + 'message?user=' + otherUsername, which appends the otherUsername to the URL.
    accessTokenFactory: This is a function that returns the access token (user.token in this case). 
    SignalR will call this function whenever it needs to add the token to the connection headers, making it easier to secure the connection with an authorization token.
    The accessTokenFactory is essential when you need to authenticate users on the SignalR hub, allowing the server to validate each request with the provided token. 
    This setup is typically used in applications where the server verifies each connection request for security purposes.
   */

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('ReciveMessageThread', (messages) => {
      // mora se podudarati naziv sa OnConnectedAsync u MessageHub.cs
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some((x) => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe({
          next: (messages) => {
            messages.forEach((message) => {
              if (!message.dateRead) {
                message.dateRead = new Date(Date.now());
              }
            });
            this.messageThreadSource.next([...messages]);
          },
        });
      }
    });

    // mora se podudarati naziv sa OnConnectedAsync u MessageHub.cs
    this.hubConnection.on('NewMessage', (message) => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: (messages) => {
          this.messageThreadSource.next([...messages, message]);
        },
      });
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection?.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(
      this.baseUrl + 'messages',
      params,
      this.http
    );
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'messages/thread/' + username
    );
  }

  async sendMessage(username: string, content: string) {
    // return this.http.post<Message>(this.baseUrl + 'messages', {
    //   recipientUsername: username,
    //   content,
    // });

    // u invoke ide tacan naziv metode iz MessageHub.cs
    return this.hubConnection
      ?.invoke('SendMessage', {
        recipientUserName: username,
        content,
      })
      .catch((error) => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
