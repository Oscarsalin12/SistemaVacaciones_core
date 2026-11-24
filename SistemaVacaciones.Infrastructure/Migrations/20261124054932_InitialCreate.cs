using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVacaciones.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaveDep = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreDep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NombreEmcargado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rfc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MotivosAusencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMotivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BitDescuentoDias = table.Column<bool>(type: "bit", nullable: false),
                    BitDescuentoHoras = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosAusencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasenia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BitAdministrador = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubEmpresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpresa_madre = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rfc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Sucursal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubEmpresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubEmpresas_Empresas_IdEmpresa_madre",
                        column: x => x.IdEmpresa_madre,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumEmpleado = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaIDeIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AniosTrabajados = table.Column<int>(type: "int", nullable: false),
                    MesesTrabajados = table.Column<int>(type: "int", nullable: false),
                    intDiasVacaciones = table.Column<int>(type: "int", nullable: false),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    DiasDisponibles = table.Column<int>(type: "int", nullable: false),
                    IdDepartamento = table.Column<int>(type: "int", nullable: false),
                    DiasLaboralesPorSemana = table.Column<int>(type: "int", nullable: false),
                    UsuarioEmpleado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HorasPorDia = table.Column<int>(type: "int", nullable: false),
                    HorasRestantesDelDia = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empleados_Departamentos_IdDepartamento",
                        column: x => x.IdDepartamento,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Empleados_SubEmpresas_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "SubEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosEmpresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idEmpresa = table.Column<int>(type: "int", nullable: false),
                    idRol = table.Column<int>(type: "int", nullable: false),
                    idSubEmpresa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosEmpresas", x => x.id);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpresas_Empresas_idEmpresa",
                        column: x => x.idEmpresa,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpresas_Roles_idRol",
                        column: x => x.idRol,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpresas_SubEmpresas_idSubEmpresa",
                        column: x => x.idSubEmpresa,
                        principalTable: "SubEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosEmpresas_Usuarios_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdTipoPermiso = table.Column<int>(type: "int", nullable: false),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasDisponibles = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CantidadHorasSolicitadas = table.Column<int>(type: "int", nullable: false),
                    HorasDisponibles = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permisos_Empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permisos_MotivosAusencia_IdTipoPermiso",
                        column: x => x.IdTipoPermiso,
                        principalTable: "MotivosAusencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permisos_SubEmpresas_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "SubEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_IdDepartamento",
                table: "Empleados",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_IdEmpresa",
                table: "Empleados",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_IdEmpleado",
                table: "Permisos",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_IdEmpresa",
                table: "Permisos",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_IdTipoPermiso",
                table: "Permisos",
                column: "IdTipoPermiso");

            migrationBuilder.CreateIndex(
                name: "IX_SubEmpresas_IdEmpresa_madre",
                table: "SubEmpresas",
                column: "IdEmpresa_madre");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpresas_idEmpresa",
                table: "UsuariosEmpresas",
                column: "idEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpresas_idRol",
                table: "UsuariosEmpresas",
                column: "idRol");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpresas_idSubEmpresa",
                table: "UsuariosEmpresas",
                column: "idSubEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEmpresas_idUsuario",
                table: "UsuariosEmpresas",
                column: "idUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "UsuariosEmpresas");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "MotivosAusencia");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "SubEmpresas");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
