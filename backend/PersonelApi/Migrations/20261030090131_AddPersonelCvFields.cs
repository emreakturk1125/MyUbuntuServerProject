using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonelApi.Migrations
{
    public partial class AddPersonelCvFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CvFileName",
                table: "Personeller",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvOriginalFileName",
                table: "Personeller",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvFileName",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "CvOriginalFileName",
                table: "Personeller");
        }
    }
}
