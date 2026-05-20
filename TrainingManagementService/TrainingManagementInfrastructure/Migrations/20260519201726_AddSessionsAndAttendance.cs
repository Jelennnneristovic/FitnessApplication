using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingManagementInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionsAndAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_TrainingSession_TrainingSessionId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSession_TrainingPlans_TrainingPlanId",
                table: "TrainingSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainingSession",
                table: "TrainingSession");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSession_TrainingPlanId",
                table: "TrainingSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendance",
                table: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_TrainingSessionId",
                table: "Attendance");

            migrationBuilder.RenameTable(
                name: "TrainingSession",
                newName: "TrainingSessions");

            migrationBuilder.RenameTable(
                name: "Attendance",
                newName: "Attendances");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TrainingSessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TrainingSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TrainingSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TrainingSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Attendances",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainingSessions",
                table: "TrainingSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_TrainingPlanId_StartTime",
                table: "TrainingSessions",
                columns: new[] { "TrainingPlanId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClientId",
                table: "Attendances",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TrainingSessionId_ClientId",
                table: "Attendances",
                columns: new[] { "TrainingSessionId", "ClientId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_TrainingSessions_TrainingSessionId",
                table: "Attendances",
                column: "TrainingSessionId",
                principalTable: "TrainingSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_TrainingPlans_TrainingPlanId",
                table: "TrainingSessions",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_TrainingSessions_TrainingSessionId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_TrainingPlans_TrainingPlanId",
                table: "TrainingSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainingSessions",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_TrainingPlanId_StartTime",
                table: "TrainingSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ClientId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_TrainingSessionId_ClientId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TrainingSessions");

            migrationBuilder.RenameTable(
                name: "TrainingSessions",
                newName: "TrainingSession");

            migrationBuilder.RenameTable(
                name: "Attendances",
                newName: "Attendance");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TrainingSession",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TrainingSession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Attendance",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainingSession",
                table: "TrainingSession",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendance",
                table: "Attendance",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSession_TrainingPlanId",
                table: "TrainingSession",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_TrainingSessionId",
                table: "Attendance",
                column: "TrainingSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_TrainingSession_TrainingSessionId",
                table: "Attendance",
                column: "TrainingSessionId",
                principalTable: "TrainingSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSession_TrainingPlans_TrainingPlanId",
                table: "TrainingSession",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
