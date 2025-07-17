using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comments",
                newSchema: "taskManagement_schema");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                schema: "taskManagement_schema",
                table: "Comments",
                newName: "TodoTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TaskId",
                schema: "taskManagement_schema",
                table: "Comments",
                newName: "IX_Comments_TodoTaskId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "taskManagement_schema",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "taskManagement_schema",
                table: "Comments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TodoTaskId",
                schema: "taskManagement_schema",
                table: "Comments",
                column: "TodoTaskId",
                principalSchema: "taskManagement_schema",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TodoTaskId",
                schema: "taskManagement_schema",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "taskManagement_schema",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "taskManagement_schema",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                schema: "taskManagement_schema",
                newName: "Comments");

            migrationBuilder.RenameColumn(
                name: "TodoTaskId",
                table: "Comments",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TodoTaskId",
                table: "Comments",
                newName: "IX_Comments_TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments",
                column: "TaskId",
                principalSchema: "taskManagement_schema",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
