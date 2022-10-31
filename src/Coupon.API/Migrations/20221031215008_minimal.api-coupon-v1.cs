using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coupon.API.Migrations
{
    public partial class minimalapicouponv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percent = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Created", "IsActive", "Name", "Percent", "Updated" },
                values: new object[] { 1, new DateTime(2022, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "10OFF", 10, null });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Created", "IsActive", "Name", "Percent", "Updated" },
                values: new object[] { 2, new DateTime(2022, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "20OFF", 10, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
