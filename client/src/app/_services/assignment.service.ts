import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { Assignment } from '../_models/assignment';
import { AssignmentParams } from '../_models/assignmentParams';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AssignmentService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  assignmentCache = new Map();
  singleAssignmentCache = new Map();
  paginatedResult = signal<PaginatedResult<Assignment[]> | null>(null);
  assignmentParams = signal<AssignmentParams>(new AssignmentParams);

  resetassignmentParams(){
    this.assignmentParams.set(new AssignmentParams);
  }

  getAssignments(groupId: number){
    const response = this.assignmentCache.get(Object.values(this.assignmentParams()).join('-') + "-groupId=" + groupId);

    if (response) return this.setPaginatedResponse(response);
    let params = this.setPaginationHeaders(this.assignmentParams().pageNumber, 
      this.assignmentParams().pageSize)

    if (this.assignmentParams().name) params = params.append("name", this.assignmentParams().name as string);
    if (this.assignmentParams().createdBy) params = params.append("createdBy", this.assignmentParams().createdBy as string);
    params = params.append("minUsers", this.assignmentParams().minUsers);
    params = params.append("maxUsers", this.assignmentParams().maxUsers);
    params = params.append("status", this.assignmentParams().status);
    params = params.append("orderBy", this.assignmentParams().orderBy);

    return this.http.get<Assignment[]>(this.baseUrl + `assignments?groupId=${groupId}`, {observe: 'response', params}).subscribe({
      next: response => {
        this.setPaginatedResponse(response);
        this.assignmentCache.set(Object.values(this.assignmentParams()).join("-") + "-groupId=" + groupId, response);
      }
    });
  }

  getAssignment(id: number){
    const assignment: Assignment = [...this.assignmentCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
      .find((a: Assignment) => a.id == id);

    if (assignment) return of(assignment);

    const assignmentFromSingleCache = this.singleAssignmentCache.get(`assignment-${id}`);
    if (assignmentFromSingleCache) return of(assignmentFromSingleCache);
    
    return this.http.get<Assignment>(this.baseUrl + `assignments/${id}`).pipe(
      tap(assignmentFromApi => {
        this.singleAssignmentCache.set(`assignment-${assignmentFromApi.id}`, assignmentFromApi);
      }
    ));
  }

  createAssignment(groupId: number, model: any){
    return this.http.post<Assignment>(this.baseUrl + `assignments?groupId=${groupId}`, model).pipe(
      tap(() => {
        this.assignmentCache.clear();
        this.getAssignments(groupId);
      })
    );
  }

  editAssignment(assignment: Assignment, model: any){
    return this.http.put<Assignment>(this.baseUrl + `assignments/${assignment.id}`, model).pipe(
      tap(() => {
        this.assignmentCache.clear();
        this.singleAssignmentCache.clear();
        this.getAssignments(assignment.groupId);
      })
    );
  }

  deleteAssignment(assignment: Assignment){
    return this.http.delete(this.baseUrl + `assignments/${assignment.id}`).pipe(
      tap(() => {
        this.assignmentCache.clear();
        this.singleAssignmentCache.clear();
        this.getAssignments(assignment.groupId);
      })
    );
  }

  joinAssignment(assignment: Assignment){
    return this.http.post(this.baseUrl + `assignments/${assignment.id}/join`, {}).pipe(
      tap(() => {
        this.assignmentCache.clear();
        this.singleAssignmentCache.clear();
        this.getAssignments(assignment.groupId);
      })
    );
  }

  leaveAssignment(assignment: Assignment){
    return this.http.post(this.baseUrl + `assignments/${assignment.id}/leave`, {}).pipe(
      tap(() => {
        this.assignmentCache.clear();
        this.singleAssignmentCache.clear();
        this.getAssignments(assignment.groupId);
      })
    );
  }

  completeAssignment(assignment: Assignment){
    return this.http.post(this.baseUrl + `assignments/${assignment.id}/complete`, {}).pipe(
      tap(() => {
        this.assignmentCache.clear();
        this.singleAssignmentCache.clear();
        this.getAssignments(assignment.groupId);
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

  private setPaginatedResponse(response: HttpResponse<Assignment[]>){
    this.paginatedResult.set({
      items: response.body as Assignment[],
      pagination: JSON.parse(response.headers.get("pagination")!)
    })
  }
}
