import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { InfoComponent } from './info/info.component';
import { GroupListComponent } from './groups/group-list/group-list.component';
import { GroupDetailComponent } from './groups/group-detail/group-detail.component';
import { authGuard } from './_guards/auth.guard';
import { GroupMembersComponent } from './groups/group-members/group-members.component';
import { AssignmentListComponent } from './assignments/assignment-list/assignment-list.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            {path: 'groups', component: GroupListComponent},
            {path: 'groups/:id', component: GroupDetailComponent},
            {path: 'groups/:id/members', component: GroupMembersComponent},
            {path: 'groups/:id/assignments', component: AssignmentListComponent},
        ]
    },
    {path: 'info', component: InfoComponent},
    {path: '**', component: HomeComponent, pathMatch: 'full'},
];
