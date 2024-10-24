using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToUserAssignmentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_AspNetUsers_UserId",
                table: "UserAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_Assigments_AssignmentId",
                table: "UserAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignment",
                table: "UserAssignment");

            migrationBuilder.RenameTable(
                name: "UserAssignment",
                newName: "UserAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignment_AssignmentId",
                table: "UserAssignments",
                newName: "IX_UserAssignments_AssignmentId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "UserAssignments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignments",
                table: "UserAssignments",
                columns: new[] { "UserId", "AssignmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_AspNetUsers_UserId",
                table: "UserAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_Assigments_AssignmentId",
                table: "UserAssignments",
                column: "AssignmentId",
                principalTable: "Assigments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_AspNetUsers_UserId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_Assigments_AssignmentId",
                table: "UserAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignments",
                table: "UserAssignments");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "UserAssignments");

            migrationBuilder.RenameTable(
                name: "UserAssignments",
                newName: "UserAssignment");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignments_AssignmentId",
                table: "UserAssignment",
                newName: "IX_UserAssignment_AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignment",
                table: "UserAssignment",
                columns: new[] { "UserId", "AssignmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_AspNetUsers_UserId",
                table: "UserAssignment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_Assigments_AssignmentId",
                table: "UserAssignment",
                column: "AssignmentId",
                principalTable: "Assigments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
