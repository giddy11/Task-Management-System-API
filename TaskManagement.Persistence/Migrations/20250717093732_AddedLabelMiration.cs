using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedLabelMiration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TodoTaskLabels",
                newName: "TodoTaskLabels",
                newSchema: "taskManagement_schema");

            migrationBuilder.RenameTable(
                name: "Labels",
                newName: "Labels",
                newSchema: "taskManagement_schema");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "taskManagement_schema",
                table: "Labels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                schema: "taskManagement_schema",
                table: "Labels",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                schema: "taskManagement_schema",
                table: "Labels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedById",
                schema: "taskManagement_schema",
                table: "Labels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_Name_Unique",
                schema: "taskManagement_schema",
                table: "Labels",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Users_CreatedById",
                schema: "taskManagement_schema",
                table: "Labels",
                column: "CreatedById",
                principalSchema: "taskManagement_schema",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Users_CreatedById",
                schema: "taskManagement_schema",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CreatedById",
                schema: "taskManagement_schema",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_Name_Unique",
                schema: "taskManagement_schema",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "taskManagement_schema",
                table: "Labels");

            migrationBuilder.RenameTable(
                name: "TodoTaskLabels",
                schema: "taskManagement_schema",
                newName: "TodoTaskLabels");

            migrationBuilder.RenameTable(
                name: "Labels",
                schema: "taskManagement_schema",
                newName: "Labels");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Labels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Labels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldNullable: true);
        }
    }
}
