import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { Group } from '../_models/group';
import { GroupParams } from '../_models/groupParams';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  groupCache = new Map();
  singleGroupCache = new Map();
  paginatedResult = signal<PaginatedResult<Group[]> | null>(null);
  groupParams = signal<GroupParams>(new GroupParams);

  resetGroupParams(){
    this.groupParams.set(new GroupParams);
  }

  getGroups(){
    const response = this.groupCache.get(Object.values(this.groupParams()).join('-'));

    if (response) return this.setPaginatedResponse(response);
    let params = this.setPaginationHeaders(this.groupParams().pageNumber, this.groupParams().pageSize)

    if (this.groupParams().groupName) params = params.append("groupName", this.groupParams().groupName as string);
    if (this.groupParams().owner) params = params.append("owner", this.groupParams().owner as string);
    params = params.append("minMembers", this.groupParams().minMembers);
    params = params.append("maxMembers", this.groupParams().maxMembers);
    params = params.append("orderBy", this.groupParams().orderBy);

    return this.http.get<Group[]>(this.baseUrl + "groups", {observe: 'response', params}).subscribe({
      next: response => {
        this.setPaginatedResponse(response);
        this.groupCache.set(Object.values(this.groupParams()).join("-"), response);
      }
    });
  }

  getGroup(id: number){
    const group: Group = [...this.groupCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
      .find((g: Group) => g.id == id);

    if (group) return of(group);

    const groupFromSingleCache = this.singleGroupCache.get(`group-${id}`);
    if (groupFromSingleCache) return of(groupFromSingleCache);
    
    return this.http.get<Group>(this.baseUrl + `groups/${id}`).pipe(
      tap(groupFromApi => {
        this.singleGroupCache.set(`group-${groupFromApi.id}`, groupFromApi);
      }
    ));
  }

  createGroup(model: any){
    return this.http.post<Group>(this.baseUrl + "groups", model).pipe(
      tap(() => {
        this.groupCache.clear();
        this.getGroups();
      })
    );
  }

  editGroup(groupId: number, model: any){
    return this.http.put<Group>(this.baseUrl + `groups/${groupId}`, model).pipe(
      tap(() => {
        this.groupCache.clear();
        this.singleGroupCache.clear();
        this.getGroups();
      })
    );
  }

  deleteGroup(groupId: number){
    return this.http.delete(this.baseUrl + `groups/${groupId}`).pipe(
      tap(() => {
        this.groupCache.clear();
        this.singleGroupCache.clear();
        this.getGroups();
      })
    );
  }

  addMember(userKnownAs: string, groupId: number){
    return this.http.post(this.baseUrl + `groups/${groupId}/members/add?userKnownAs=${userKnownAs}`, {}).pipe(
      tap(() => {
        this.groupCache.clear();
        this.singleGroupCache.clear();
        this.getGroups();
      })
    );
  }

  removeMember(userKnownAs: string, groupId: number){
    return this.http.post(this.baseUrl + `groups/${groupId}/members/remove?userKnownAs=${userKnownAs}`, {}).pipe(
      tap(() => {
        this.groupCache.clear();
        this.singleGroupCache.clear();
        this.getGroups();
      })
    );
  }

  leaveGroup(groupId: number){
    return this.http.post(this.baseUrl + `groups/members/${groupId}/leave`, {}).pipe(
      tap(() => {
        this.groupCache.clear();
        this.singleGroupCache.clear();
        this.getGroups();
      })
    );
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams();

    if (pageNumber && pageSize){
      params = params.append("pageNumber", pageNumber);
      params = params.append("pageSize", pageSize);
    }

    return params;
  }

  private setPaginatedResponse(response: HttpResponse<Group[]>){
    this.paginatedResult.set({
      items: response.body as Group[],
      pagination: JSON.parse(response.headers.get("pagination")!)
    })
  }
}
