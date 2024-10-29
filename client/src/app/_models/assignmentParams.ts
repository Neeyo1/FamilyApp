export class AssignmentParams{
    name?: string;
    createdBy?: string;
    minUsers = 1;
    maxUsers = 10;
    status = "all";
    orderBy = "oldest";
    pageNumber = 1;
    pageSize = 10;
}