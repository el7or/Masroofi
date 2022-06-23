using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Masroofi.Core.Migrations
{
    public partial class PushNotificationRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationParent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationSchedule",
                table: "NotificationSchedule");

            migrationBuilder.RenameTable(
                name: "NotificationSchedule",
                newName: "NotificationSchedules");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationSchedules",
                table: "NotificationSchedules",
                column: "NotificationScheduleId");

            migrationBuilder.CreateTable(
                name: "ParentNotifications",
                columns: table => new
                {
                    NotificationParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NotificationScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentNotifications", x => x.NotificationParentId);
                    table.ForeignKey(
                        name: "FK_ParentNotifications_NotificationSchedules_NotificationScheduleId",
                        column: x => x.NotificationScheduleId,
                        principalTable: "NotificationSchedules",
                        principalColumn: "NotificationScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentNotifications_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentNotifications_NotificationScheduleId",
                table: "ParentNotifications",
                column: "NotificationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentNotifications_ParentId",
                table: "ParentNotifications",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParentNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationSchedules",
                table: "NotificationSchedules");

            migrationBuilder.RenameTable(
                name: "NotificationSchedules",
                newName: "NotificationSchedule");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationSchedule",
                table: "NotificationSchedule",
                column: "NotificationScheduleId");

            migrationBuilder.CreateTable(
                name: "NotificationParent",
                columns: table => new
                {
                    NotificationParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NotificationScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationParent", x => x.NotificationParentId);
                    table.ForeignKey(
                        name: "FK_NotificationParent_NotificationSchedule_NotificationScheduleId",
                        column: x => x.NotificationScheduleId,
                        principalTable: "NotificationSchedule",
                        principalColumn: "NotificationScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationParent_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationParent_NotificationScheduleId",
                table: "NotificationParent",
                column: "NotificationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationParent_ParentId",
                table: "NotificationParent",
                column: "ParentId");
        }
    }
}
