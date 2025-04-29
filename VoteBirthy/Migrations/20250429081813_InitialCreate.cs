using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteBirthy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "Gifts",
                columns: table => new
                {
                    GiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifts", x => x.GiftId);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    VoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BirthdayEmpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.VoteId);
                    table.ForeignKey(
                        name: "FK_Votes_Employees_BirthdayEmpId",
                        column: x => x.BirthdayEmpId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_Employees_StartedById",
                        column: x => x.StartedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoteOptions",
                columns: table => new
                {
                    VoteOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteOptions", x => x.VoteOptionId);
                    table.ForeignKey(
                        name: "FK_VoteOptions_Gifts_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gifts",
                        principalColumn: "GiftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoteOptions_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "VoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteCasts",
                columns: table => new
                {
                    VoteCastId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteCasts", x => x.VoteCastId);
                    table.ForeignKey(
                        name: "FK_VoteCasts_Employees_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoteCasts_VoteOptions_VoteOptionId",
                        column: x => x.VoteOptionId,
                        principalTable: "VoteOptions",
                        principalColumn: "VoteOptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoteCasts_VoterId",
                table: "VoteCasts",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "UQ_OneVotePerUserPerVote",
                table: "VoteCasts",
                columns: new[] { "VoteOptionId", "VoterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoteOptions_GiftId",
                table: "VoteOptions",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteOptions_VoteId",
                table: "VoteOptions",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_StartedById",
                table: "Votes",
                column: "StartedById");

            migrationBuilder.CreateIndex(
                name: "UQ_OneOpenVotePerEmpYr",
                table: "Votes",
                columns: new[] { "BirthdayEmpId", "StartDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoteCasts");

            migrationBuilder.DropTable(
                name: "VoteOptions");

            migrationBuilder.DropTable(
                name: "Gifts");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
