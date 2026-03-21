using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetPharmacy.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastModifiedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Products");
        }
    }
}
