using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SaaS_DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlanType = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    BillingInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    TrialDays = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddonName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    BillingType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanAddons_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UserRoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TrialEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancellationReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanAddonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    AddedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionAddons_PlanAddons_PlanAddonId",
                        column: x => x.PlanAddonId,
                        principalTable: "PlanAddons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionAddons_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, "role for managing things...", "Admin" },
                    { 2, "Just a usual subscriber", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAddons_PlanId",
                table: "PlanAddons",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddons_PlanAddonId",
                table: "SubscriptionAddons",
                column: "PlanAddonId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddons_SubscriptionId",
                table: "SubscriptionAddons",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleName",
                table: "UserRoles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserRoleId",
                table: "Users",
                column: "UserRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SubscriptionAddons");

            migrationBuilder.DropTable(
                name: "PlanAddons");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
