using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersManager.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderStateCorrelationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStateCorrelationId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
