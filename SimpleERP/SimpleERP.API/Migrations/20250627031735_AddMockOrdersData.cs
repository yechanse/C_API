using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleERP.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMockOrdersData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_CustomerId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "Orders");

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerId", "DeliveredDate", "Notes", "OrderDate", "OrderNumber", "ShippedDate", "ShippingAddress", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 100, new DateTime(2024, 11, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Priority delivery requested", new DateTime(2024, 11, 16, 0, 0, 0, 0, DateTimeKind.Utc), "ORD202412001", new DateTime(2024, 11, 18, 0, 0, 0, 0, DateTimeKind.Utc), "123 Main St, Tech City, TC 12345", "Delivered", 1649.98m },
                    { 2, 101, null, "Office setup order", new DateTime(2024, 11, 21, 0, 0, 0, 0, DateTimeKind.Utc), "ORD202412002", new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc), "456 Business Ave, Corporate City, CC 67890", "Shipped", 899.98m },
                    { 3, 102, null, "Home office upgrade", new DateTime(2024, 11, 26, 0, 0, 0, 0, DateTimeKind.Utc), "ORD202412003", null, "789 Developer Rd, Code City, CD 54321", "Processing", 799.98m },
                    { 4, 100, null, "Additional equipment", new DateTime(2024, 11, 29, 0, 0, 0, 0, DateTimeKind.Utc), "ORD202412004", null, "123 Main St, Tech City, TC 12345", "Pending", 349.98m },
                    { 5, 101, null, "New team member setup", new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "ORD202412005", null, "456 Business Ave, Corporate City, CC 67890", "Processing", 1299.99m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 100, new DateTime(2024, 11, 11, 0, 0, 0, 0, DateTimeKind.Utc), "john@example.com", true, "$2a$11$mock.hash.for.testing.purposes.only", "john_doe" },
                    { 101, new DateTime(2024, 11, 16, 0, 0, 0, 0, DateTimeKind.Utc), "jane@example.com", true, "$2a$11$mock.hash.for.testing.purposes.only", "jane_smith" },
                    { 102, new DateTime(2024, 11, 21, 0, 0, 0, 0, DateTimeKind.Utc), "bob@example.com", true, "$2a$11$mock.hash.for.testing.purposes.only", "bob_wilson" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "TotalPrice", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 1299.99m, 1299.99m },
                    { 2, 1, 3, 7, 349.93m, 49.99m },
                    { 3, 2, 2, 1, 299.99m, 299.99m },
                    { 4, 2, 4, 1, 599.99m, 599.99m },
                    { 5, 3, 5, 2, 799.98m, 399.99m },
                    { 6, 4, 2, 1, 299.99m, 299.99m },
                    { 7, 4, 3, 1, 49.99m, 49.99m },
                    { 8, 5, 1, 1, 1299.99m, 1299.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId1",
                table: "Orders",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId1",
                table: "Orders",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
