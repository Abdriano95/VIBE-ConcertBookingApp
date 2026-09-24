using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMA_AU24_LAB2_Group4.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedCustomerPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$14iUvy7JDmzV14JpiRZmA.czCYnNgnrGnEG9XNuKzxyIaV/jWRTDi");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$uxpheqSZiEIy10yyL7a8GOhXV4r7XXPyHik89046f3h3OI.Ldwspe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
