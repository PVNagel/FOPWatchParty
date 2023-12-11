using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOPMovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieReports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Sub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserSub = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FopRating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OneOscar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BestQuote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunniestQuote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanRemakeAsNetflixSeries = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieReports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_MovieReports_FopUsers_UserSub",
                        column: x => x.UserSub,
                        principalTable: "FopUsers",
                        principalColumn: "Sub");
                    table.ForeignKey(
                        name: "FK_MovieReports_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieReports_MovieId",
                table: "MovieReports",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieReports_UserSub",
                table: "MovieReports",
                column: "UserSub");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieReports");
        }
    }
}
