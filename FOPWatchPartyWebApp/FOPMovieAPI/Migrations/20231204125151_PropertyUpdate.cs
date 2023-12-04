using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOPMovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class PropertyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FOPrating",
                table: "Movies",
                newName: "FopRating");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FopRating",
                table: "Movies",
                newName: "FOPrating");
        }
    }
}
