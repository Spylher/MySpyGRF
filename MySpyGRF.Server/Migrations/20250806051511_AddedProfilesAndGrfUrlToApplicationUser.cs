using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySpyGRF.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedProfilesAndGrfUrlToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrfUrl",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilesJson",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrfUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilesJson",
                table: "AspNetUsers");
        }
    }
}
