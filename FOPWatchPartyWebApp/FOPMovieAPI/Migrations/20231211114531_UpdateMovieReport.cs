using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOPMovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMovieReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieReports_FopUsers_UserSub",
                table: "MovieReports");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieReports_Movies_MovieId",
                table: "MovieReports");

            migrationBuilder.DropIndex(
                name: "IX_MovieReports_MovieId",
                table: "MovieReports");

            migrationBuilder.DropIndex(
                name: "IX_MovieReports_UserSub",
                table: "MovieReports");

            migrationBuilder.DropColumn(
                name: "UserSub",
                table: "MovieReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserSub",
                table: "MovieReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieReports_MovieId",
                table: "MovieReports",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieReports_UserSub",
                table: "MovieReports",
                column: "UserSub");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieReports_FopUsers_UserSub",
                table: "MovieReports",
                column: "UserSub",
                principalTable: "FopUsers",
                principalColumn: "Sub");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieReports_Movies_MovieId",
                table: "MovieReports",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "MovieId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
