import { AssignmentMember } from "./assignmentMember";
import { Member } from "./member";
import { Reaction } from "./reaction";

export interface Assignment{
    id: number;
    groupId: number;
    name: string;
    description: string;
    createdAt: Date;
    endsAt: Date;
    completed: boolean;
    maxUsers: number;
    createdBy: Member;
    membersCount: number;
    members: Member[];
    usersAssigned: AssignmentMember[];
    reactions: Reaction[];
}