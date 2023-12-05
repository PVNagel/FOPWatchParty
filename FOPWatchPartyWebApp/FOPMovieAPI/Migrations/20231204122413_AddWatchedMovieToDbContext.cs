using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOPMovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchedMovieToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatchedMovies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieimdbID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchedMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchedMovies_Movies_MovieimdbID",
                        column: x => x.MovieimdbID,
                        principalTable: "Movies",
                        principalColumn: "imdbID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchedMovies_MovieimdbID",
                table: "WatchedMovies",
                column: "MovieimdbID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchedMovies");
        }
    }
}
