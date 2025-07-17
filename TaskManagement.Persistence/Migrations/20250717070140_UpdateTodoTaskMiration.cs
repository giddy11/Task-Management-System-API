using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTodoTaskMiration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Tasks_TodoTaskId",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_TodoTaskId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "TodoTaskId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "TaskStatus",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "Tasks",
                newSchema: "taskManagement_schema");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "taskManagement_schema",
                table: "Tasks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PriorityStatus",
                schema: "taskManagement_schema",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "taskManagement_schema",
                table: "Tasks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                schema: "taskManagement_schema",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TodoTaskStatus",
                schema: "taskManagement_schema",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TodoTaskAssignee",
                schema: "taskManagement_schema",
                columns: table => new
                {
                    TodoTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoTaskAssignee", x => new { x.TodoTaskId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TodoTaskAssignee_Tasks_TodoTaskId",
                        column: x => x.TodoTaskId,
                        principalSchema: "taskManagement_schema",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoTaskAssignee_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "taskManagement_schema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TodoTaskLabels",
                columns: table => new
                {
                    TodoTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoTaskLabels", x => new { x.TodoTaskId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_TodoTaskLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoTaskLabels_Tasks_TodoTaskId",
                        column: x => x.TodoTaskId,
                        principalSchema: "taskManagement_schema",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedById",
                schema: "taskManagement_schema",
                table: "Tasks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TodoTaskAssignee_UserId",
                schema: "taskManagement_schema",
                table: "TodoTaskAssignee",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoTaskLabels_LabelId",
                table: "TodoTaskLabels",
                column: "LabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                schema: "taskManagement_schema",
                table: "Tasks",
                column: "CreatedById",
                principalSchema: "taskManagement_schema",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                schema: "taskManagement_schema",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TodoTaskAssignee",
                schema: "taskManagement_schema");

            migrationBuilder.DropTable(
                name: "TodoTaskLabels");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedById",
                schema: "taskManagement_schema",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "taskManagement_schema",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TodoTaskStatus",
                schema: "taskManagement_schema",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                schema: "taskManagement_schema",
                newName: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "TodoTaskId",
                table: "Labels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "PriorityStatus",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskStatus",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_TodoTaskId",
                table: "Labels",
                column: "TodoTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Tasks_TodoTaskId",
                table: "Labels",
                column: "TodoTaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
