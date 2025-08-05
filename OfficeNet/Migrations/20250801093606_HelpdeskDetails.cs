using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeNet.Migrations
{
    /// <inheritdoc />
    public partial class HelpdeskDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HelpDeskDetails",
                columns: table => new
                {
                    IssueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeptID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    SubCategoryID = table.Column<int>(type: "int", nullable: false),
                    EscalationUserId = table.Column<int>(type: "int", nullable: false),
                    EscalationOneUserID = table.Column<int>(type: "int", nullable: false),
                    EscalationOneTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EscalationTwoUserID = table.Column<int>(type: "int", nullable: false),
                    EscalationTwoTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EscalationThree_UserID = table.Column<int>(type: "int", nullable: true),
                    EscalationThreeTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    TentativeClosureHour = table.Column<int>(type: "int", nullable: true),
                    TentativeClosureDay = table.Column<int>(type: "int", nullable: true),
                    IsUploadExcel = table.Column<bool>(type: "bit", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpDeskDetails", x => x.IssueID);
                });

            migrationBuilder.CreateTable(
                name: "HelpdeskAdminUser",
                columns: table => new
                {
                    HdeskAdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueID = table.Column<int>(type: "int", nullable: false),
                    AdminUserID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpdeskAdminUser", x => x.HdeskAdminId);
                    table.ForeignKey(
                        name: "FK_HelpdeskAdminUser_HelpDeskDetails_IssueID",
                        column: x => x.IssueID,
                        principalTable: "HelpDeskDetails",
                        principalColumn: "IssueID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpdeskAdminUser_Users_AdminUserID",
                        column: x => x.AdminUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskAdminUser_AdminUserID",
                table: "HelpdeskAdminUser",
                column: "AdminUserID");

            migrationBuilder.CreateIndex(
                name: "IX_HelpdeskAdminUser_IssueID",
                table: "HelpdeskAdminUser",
                column: "IssueID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HelpdeskAdminUser");

            migrationBuilder.DropTable(
                name: "HelpDeskDetails");
        }
    }
}
