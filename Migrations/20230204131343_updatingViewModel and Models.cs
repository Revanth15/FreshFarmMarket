using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshFarmMarket.Migrations
{
    /// <inheritdoc />
    public partial class updatingViewModelandModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "aboutMe",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "creditCardNo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "deliveryAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "fullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "imageURL",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "mobileNo",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aboutMe",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "creditCardNo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "deliveryAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "fullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "imageURL",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "mobileNo",
                table: "AspNetUsers");
        }
    }
}
