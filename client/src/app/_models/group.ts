import { Member } from "./member";

export interface Group{
    id: number;
    groupName: string;
    createdAt: Date;
    owner: Member;
    membersCount: number;
    members: Member[];
}