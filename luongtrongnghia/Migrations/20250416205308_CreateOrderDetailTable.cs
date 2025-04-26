using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace luongtrongnghia.Migrations
{
    public partial class CreateOrderDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),  // Khóa ngoại đến Orders
                    ProductId = table.Column<int>(type: "int", nullable: false), // Khóa ngoại đến Products
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);

                    // Thêm khóa ngoại đến bảng Orders
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId", // Tên khóa ngoại
                        column: x => x.OrderId,            // Cột khóa ngoại
                        principalTable: "Orders",          // Bảng tham chiếu
                        principalColumn: "Id",             // Cột khóa chính trong bảng Orders
                        onDelete: ReferentialAction.Cascade); // Xóa các chi tiết đơn hàng khi xóa đơn hàng

                    // Thêm khóa ngoại đến bảng Products
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId", // Tên khóa ngoại
                        column: x => x.ProductId,               // Cột khóa ngoại
                        principalTable: "Products",             // Bảng tham chiếu
                        principalColumn: "Id",                  // Cột khóa chính trong bảng Products
                        onDelete: ReferentialAction.Restrict); // Không xóa chi tiết khi xóa sản phẩm
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");
        }
    }
}

//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace luongtrongnghia.Migrations
//{
//    public partial class CreateOrderDetailTable : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "OrderDetails",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    OrderId = table.Column<int>(type: "int", nullable: false),
//                    ProductId = table.Column<int>(type: "int", nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Price = table.Column<double>(type: "float", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
//                });
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "OrderDetails");
//        }
//    }
//}
