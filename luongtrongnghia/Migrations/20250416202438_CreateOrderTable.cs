using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace luongtrongnghia.Migrations
{
    public partial class CreateOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo bảng Orders
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),  // Khóa ngoại UserId
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    // Thiết lập khóa ngoại
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId", // Tên khóa ngoại
                        column: x => x.UserId, // Cột khóa ngoại
                        principalTable: "Users", // Bảng tham chiếu
                        principalColumn: "Id", // Cột khóa chính trong bảng Users
                        onDelete: ReferentialAction.Cascade); // Hành động khi xóa: xóa các đơn hàng khi xóa người dùng
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa bảng Orders khi rollback
            migrationBuilder.DropTable(name: "Orders");
        }
    }
}

//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace luongtrongnghia.Migrations
//{
//    public partial class CreateOrderTable : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "Orders",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    UserId = table.Column<int>(type: "int", nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Orders", x => x.Id);
//                });
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "Orders");
//        }
//    }
//}
