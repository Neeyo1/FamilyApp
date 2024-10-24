using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAssignmentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Assigments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserAssignment",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignment", x => new { x.UserId, x.AssignmentId });
                    table.ForeignKey(
                        name: "FK_UserAssignment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAssignment_Assigments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assigments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Assigments_CreatedById",
                table: "Assigments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignment_AssignmentId",
                table: "UserAssignment",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assigments_AspNetUsers_CreatedById",
                table: "Assigments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assigments_AspNetUsers_CreatedById",
                table: "Assigments");

            migrationBuilder.DropTable(
                name: "UserAssignment");

            migrationBuilder.DropIndex(
                name: "IX_Assigments_CreatedById",
                table: "Assigments");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Assigments");
        }
    }
}
