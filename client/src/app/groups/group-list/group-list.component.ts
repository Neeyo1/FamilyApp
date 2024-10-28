import { Component, inject, OnInit } from '@angular/core';
import { GroupService } from '../../_services/group.service';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-group-list',
  standalone: true,
  imports: [FormsModule, PaginationModule, RouterLink],
  templateUrl: './group-list.component.html',
  styleUrl: './group-list.component.css'
})
export class GroupListComponent implements OnInit{
  groupService = inject(GroupService);
  orderByList = [
    {value: 'oldest', display: 'Oldest'}, 
    {value: 'newest', display: 'Newest'},
    {value: 'members', display: 'Members'}, 
    {value: 'membersDesc', display: 'Members desc'}
  ];

  ngOnInit(): void {
    if (!this.groupService.paginatedResult()) this.loadGroups();
  }

  loadGroups(){
    this.groupService.getGroups();
  }

  resetFilters(){
    this.groupService.resetGroupParams();
    this.loadGroups();
  }

  pageChanged(event: any){
    if (this.groupService.groupParams().pageNumber != event.page){
      this.groupService.groupParams().pageNumber = event.page;
      this.loadGroups();
    }
  }
}
