import { Component, OnInit, Input, NgModule, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  //@Input() userFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  model: any={}

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register(){
    //console.log(this.model);
    this.accountService.register(this.model).subscribe(responce => {
      console.log(this.model);
      this.cancel(); //Close the form = hide componenets
    }, error =>{
      console.log(error);
    })
  }
  cancel(){
    //console.log('cancelled');
    this.cancelRegister.emit(false);
  }
}
