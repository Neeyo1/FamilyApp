import { Component, inject, OnInit, signal } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { GroupService } from '../../_services/group.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Group } from '../../_models/group';
import { Member } from '../../_models/member';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-group-members',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './group-members.component.html',
  styleUrl: './group-members.component.css'
})
export class GroupMembersComponent implements OnInit{
  accountService = inject(AccountService);
  private groupService = inject(GroupService);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  group = signal<Group | null>(null);
  members = signal<Member[]>([]);
  model: any = {};

  ngOnInit(): void {
    this.loadGroup();
  }

  loadGroup(){
    const groupId = Number(this.route.snapshot.paramMap.get("id"));
    if (!groupId) return;

    this.groupService.getGroup(groupId).subscribe({
      next: group => {
        this.group.set(group);
        this.members.set(this.group()!.members);
      }
    })
  }

  addMember(){
    this.groupService.addMember(this.model.knownAs, this.group()!.id).subscribe({
      next: () => this.loadGroup(),
      error: error => this.toastr.error(error.error)
    });
  }

  removeMember(userKnownAs: string){
    this.groupService.removeMember(userKnownAs, this.group()!.id).subscribe({
      next: () => this.loadGroup(),
      error: error => this.toastr.error(error.error)
    });
  }
}
