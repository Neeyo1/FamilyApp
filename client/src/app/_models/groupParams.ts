export class GroupParams{
    groupName?: string;
    owner?: string;
    minMembers = 1;
    maxMembers = 100;
    orderBy = "oldest";
    pageNumber = 1;
    pageSize = 10;
}