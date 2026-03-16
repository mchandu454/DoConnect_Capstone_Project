using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoConnect.API.Migrations
{
    /// <inheritdoc />
    public partial class ContextUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppImages_Answers_AnswerId",
                table: "AppImages");

            migrationBuilder.DropForeignKey(
                name: "FK_AppImages_Questions_QuestionId",
                table: "AppImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppImages",
                table: "AppImages");

            migrationBuilder.RenameTable(
                name: "AppImages",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_AppImages_QuestionId",
                table: "Images",
                newName: "IX_Images_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AppImages_AnswerId",
                table: "Images",
                newName: "IX_Images_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "ImageId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Password", "Role", "Username" },
                values: new object[] { 1, "Admin@123", "Admin", "admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Answers_AnswerId",
                table: "Images",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "AnswerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Questions_QuestionId",
                table: "Images",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Answers_AnswerId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Questions_QuestionId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "AppImages");

            migrationBuilder.RenameIndex(
                name: "IX_Images_QuestionId",
                table: "AppImages",
                newName: "IX_AppImages_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_AnswerId",
                table: "AppImages",
                newName: "IX_AppImages_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppImages",
                table: "AppImages",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppImages_Answers_AnswerId",
                table: "AppImages",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "AnswerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppImages_Questions_QuestionId",
                table: "AppImages",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
