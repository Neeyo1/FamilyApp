import { Component, inject, OnInit, signal } from '@angular/core';
import { GroupService } from '../../_services/group.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Group } from '../../_models/group';
import { AccountService } from '../../_services/account.service';
import { AssignmentService } from '../../_services/assignment.service';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-assignment-list',
  standalone: true,
  imports: [FormsModule, RouterLink, PaginationModule],
  templateUrl: './assignment-list.component.html',
  styleUrl: './assignment-list.component.css'
})
export class AssignmentListComponent implements OnInit{
  private groupService = inject(GroupService);
  assignmentService = inject(AssignmentService);
  private route = inject(ActivatedRoute);
  private toastrServie = inject(ToastrService);
  private router = inject(Router);
  group = signal<Group | null>(null);
  accountService = inject(AccountService);
  orderByList = [
    {value: 'oldest', display: 'Oldest'}, 
    {value: 'newest', display: 'Newest'},
    {value: 'members', display: 'Members'}, 
    {value: 'membersDesc', display: 'Members desc'}
  ];
  statusList = [
    {value: 'completed', display: 'Completed'}, 
    {value: 'open', display: 'Open'},
    {value: 'all', display: 'All'}
  ];

  ngOnInit(): void {
    this.loadGroup();
  }

  loadGroup(){
    const groupId = Number(this.route.snapshot.paramMap.get("id"));
    if (!groupId) return;

    this.groupService.getGroup(groupId).subscribe({
      next: group => {
        this.group.set(group);
        if (!this.assignmentService.paginatedResult()) this.loadAssignments(this.group()!.id);
      }
    })
  }

  loadAssignments(groupId: number){
    this.assignmentService.getAssignments(groupId);
  }

  resetFilters(){
    this.groupService.resetGroupParams();
    this.loadAssignments(this.group()!.id);
  }

  pageChanged(event: any){
    if (this.assignmentService.assignmentParams().pageNumber != event.page){
      this.assignmentService.assignmentParams().pageNumber = event.page;
      this.loadAssignments(this.group()!.id);
    }
  }
}
