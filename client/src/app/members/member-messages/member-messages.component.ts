import { NgForm } from '@angular/forms';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { Pagination } from '../../_models/pagination';
//import { ConfirmService } from '../_services/confirm.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  // messages: Message[] = [];
  // pagination: Pagination;
  // container = 'Unread';
  // pageNumber = 1;
  // pageSize = 5;
  loading = false;
  messageContent: string;

  constructor(private messageService: MessageService) { }
  //constructor() { }
  //constructor(private messageService: MessageService, private confirmService: ConfirmService) { }

  ngOnInit(): void {
    //this.loadMessages();
  }

  // loadMessages() {
  //   this.loading = true;
  //   this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
  //     this.messages = response.result;
  //     this.pagination = response.pagination;
  //     this.loading = false;
  //   })
  // }

  deleteMessage(id: number) {
    // this.messageService.deleteMessage(id).subscribe(() => {
    //   this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    // })

    // this.confirmService.confirm('Confirm delete message', 'This cannot be undone').subscribe(result => {
    //   if (result) {
    //     this.messageService.deleteMessage(id).subscribe(() => {
    //       this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    //     })
    //   }
    // })
  }

  // pageChanged(event: any) {
  //   this.pageNumber = event.page;
  //   this.loadMessages();
  // }

  sendMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message => {
      this.messages.push(message);
      this.messageForm.reset();
    })
  }

  // sendMessage() {
  //   console.log(this.messageContent);
  //   this.loading = true;
  //   this.messageService.sendMessage(this.username, this.messageContent).then(() => {
  //     this.messageForm.reset();
  //   }).finally(() => this.loading = false);
  // }
}
