using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryPolaris.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeliveryPartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryPartner");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryPartner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location_Lat = table.Column<double>(type: "float", nullable: false),
                    Location_Lon = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPartner", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPartner_Name",
                table: "DeliveryPartner",
                column: "Name",
                unique: true);
        }
    }
}
