import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-managament',
  templateUrl: './user-managament.component.html',
  styleUrls: ['./user-managament.component.css'],
})
export class UserManagamentComponent implements OnInit {
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> =
    new BsModalRef<RolesModalComponent>();
  availableRoles = ['Admin', 'Moderator', 'Member']; // imamo samo tri, pa smo zakucalim inace mogu sam vuci iz baze iz tabele aspNetRoles, u admin kontroleru ih dovuci i pozvato servis npr.

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: (users) => (this.users = users),
    });
  }

  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.username,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles],
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if (!this.arrayEqual(selectedRoles!, user.roles)) {
          this.adminService
            .updateUserRoles(user.username, selectedRoles?.toString()!)
            .subscribe({
              next: (roles) => (user.roles = roles),
            });
        }
      },
    });
    //this.bsModalRef.content!.closeBtnName = 'Close'; // sa ovim ! sam siguran da promjenjiva content nije null ili undefined
  }

  private arrayEqual(arr1: any[], arr2: any[]) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }
}
