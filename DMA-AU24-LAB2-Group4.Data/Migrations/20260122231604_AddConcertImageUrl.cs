using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMA_AU24_LAB2_Group4.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConcertImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Concerts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Concerts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800");

            migrationBuilder.UpdateData(
                table: "Concerts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?w=800");

            migrationBuilder.UpdateData(
                table: "Concerts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1598387993441-a364f854c3e1?w=800");

            migrationBuilder.UpdateData(
                table: "Concerts",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=800");

            migrationBuilder.UpdateData(
                table: "Concerts",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$K3g6XoTau5FJxqGHVdPrS.g1GiqHvfTdL8F7rT4jiYvLlMHxqZpTK");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$PLrJGz8K8Q4FG5xY0F7Yw.3Rjx6H5MQqH7F5V2mN4oP6qR8sT0uWY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Concerts");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "hashed_password_1");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "hashed_password_2");
        }
    }
}
