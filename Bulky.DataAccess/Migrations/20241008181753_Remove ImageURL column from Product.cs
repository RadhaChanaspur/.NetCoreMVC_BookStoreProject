using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageURLcolumnfromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDue",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "PaymentDue",
                table: "Orders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageURL",
                value: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
