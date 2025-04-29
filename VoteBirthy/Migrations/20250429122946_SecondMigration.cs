using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteBirthy.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteCasts_Employees_VoterId",
                table: "VoteCasts");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteCasts_VoteOptions_VoteOptionId",
                table: "VoteCasts");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteOptions_Gifts_GiftId",
                table: "VoteOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteOptions_Votes_VoteId",
                table: "VoteOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Employees_BirthdayEmpId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Employees_StartedById",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteOptions",
                table: "VoteOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteCasts",
                table: "VoteCasts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gifts",
                table: "Gifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteOptions",
                table: "VoteOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteCasts",
                table: "VoteCasts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gifts",
                table: "Gifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteCasts_Employees_VoterId",
                table: "VoteCasts",
                column: "VoterId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteCasts_VoteOptions_VoteOptionId",
                table: "VoteCasts",
                column: "VoteOptionId",
                principalTable: "VoteOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteOptions_Gifts_GiftId",
                table: "VoteOptions",
                column: "GiftId",
                principalTable: "Gifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteOptions_Votes_VoteId",
                table: "VoteOptions",
                column: "VoteId",
                principalTable: "Votes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Employees_BirthdayEmpId",
                table: "Votes",
                column: "BirthdayEmpId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Employees_StartedById",
                table: "Votes",
                column: "StartedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteCasts_Employees_VoterId",
                table: "VoteCasts");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteCasts_VoteOptions_VoteOptionId",
                table: "VoteCasts");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteOptions_Gifts_GiftId",
                table: "VoteOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteOptions_Votes_VoteId",
                table: "VoteOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Employees_BirthdayEmpId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Employees_StartedById",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteOptions",
                table: "VoteOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteCasts",
                table: "VoteCasts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gifts",
                table: "Gifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "VoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteOptions",
                table: "VoteOptions",
                column: "VoteOptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteCasts",
                table: "VoteCasts",
                column: "VoteCastId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gifts",
                table: "Gifts",
                column: "GiftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteCasts_Employees_VoterId",
                table: "VoteCasts",
                column: "VoterId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteCasts_VoteOptions_VoteOptionId",
                table: "VoteCasts",
                column: "VoteOptionId",
                principalTable: "VoteOptions",
                principalColumn: "VoteOptionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteOptions_Gifts_GiftId",
                table: "VoteOptions",
                column: "GiftId",
                principalTable: "Gifts",
                principalColumn: "GiftId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteOptions_Votes_VoteId",
                table: "VoteOptions",
                column: "VoteId",
                principalTable: "Votes",
                principalColumn: "VoteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Employees_BirthdayEmpId",
                table: "Votes",
                column: "BirthdayEmpId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Employees_StartedById",
                table: "Votes",
                column: "StartedById",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
