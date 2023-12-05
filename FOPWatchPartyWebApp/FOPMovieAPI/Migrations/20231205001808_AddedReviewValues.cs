using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOPMovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedReviewValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BestQuote",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CanRemakeAsNetflixSeries",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FunniestQuote",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneOscar",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestQuote",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CanRemakeAsNetflixSeries",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "FunniestQuote",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "OneOscar",
                table: "Movies");
        }
    }
}
