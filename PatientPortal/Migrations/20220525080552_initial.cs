using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientPortal.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    PatientId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci"),
                    Sex = table.Column<string>(type: "enum('male','female','others')", nullable: false, collation: "utf8_general_ci"),
                    Age = table.Column<int>(type: "int", nullable: false),
                    BloodType = table.Column<string>(type: "enum('A','B','AB','O')", nullable: false, collation: "utf8_general_ci"),
                    PastHistory = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, collation: "utf8_general_ci"),
                    City = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8_general_ci"),
                    State = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci"),
                    Reports = table.Column<byte[]>(type: "blob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient", x => x.PatientId);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "donor",
                columns: table => new
                {
                    DonorId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci"),
                    Sex = table.Column<string>(type: "enum('male','female','others')", nullable: false, collation: "utf8_general_ci"),
                    Age = table.Column<int>(type: "int", nullable: false),
                    BloodType = table.Column<string>(type: "enum('A','B','AB','O')", nullable: false, collation: "utf8_general_ci"),
                    PastHistory = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, collation: "utf8_general_ci"),
                    City = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8_general_ci"),
                    State = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci"),
                    PatientRelation = table.Column<string>(type: "enum('father','mother','brother','sister','husband','wife','son','daughter','others')", nullable: false, collation: "utf8_general_ci"),
                    FamilyPatientId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donor", x => x.DonorId);
                    table.ForeignKey(
                        name: "FamilyPatient",
                        column: x => x.FamilyPatientId,
                        principalTable: "patient",
                        principalColumn: "PatientId");
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "swap",
                columns: table => new
                {
                    SwapId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DonorId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swap", x => x.SwapId);
                    table.ForeignKey(
                        name: "DonorId",
                        column: x => x.DonorId,
                        principalTable: "donor",
                        principalColumn: "DonorId");
                    table.ForeignKey(
                        name: "PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "PatientId");
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "DonorId_UNIQUE",
                table: "donor",
                column: "DonorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FamilyPatient_idx",
                table: "donor",
                column: "FamilyPatientId");

            migrationBuilder.CreateIndex(
                name: "PatientId_UNIQUE",
                table: "patient",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "DonorId_idx",
                table: "swap",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "PatientId_idx",
                table: "swap",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "SwapId_UNIQUE",
                table: "swap",
                column: "SwapId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "swap");

            migrationBuilder.DropTable(
                name: "donor");

            migrationBuilder.DropTable(
                name: "patient");
        }
    }
}
