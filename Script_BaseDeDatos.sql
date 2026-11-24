-- Crear base de datos
CREATE DATABASE Vacaciones_BD;
GO

USE Vacaciones_BD;
GO

-- Tabla: Departamentos
CREATE TABLE Departamentos (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100)
);

-- Tabla: Empresas
CREATE TABLE Empresas (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    Direccion NVARCHAR(200),
    Telefono NVARCHAR(20),
    Logo VARBINARY(MAX)
);

-- Tabla: SubEmpresas
CREATE TABLE SubEmpresas (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    IdEmpresa INT,
    Logo VARBINARY(MAX),
    FOREIGN KEY (IdEmpresa) REFERENCES Empresas(Id)
);

-- Tabla: Empleados
CREATE TABLE Empleados (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    ApellidoPaterno NVARCHAR(100),
    ApellidoMaterno NVARCHAR(100),
    FechaIngreso DATE,
    IdEmpresa INT,
    IdDepartamento INT,
    Correo NVARCHAR(150),
    Contraseña NVARCHAR(100),
    DiasVacacionesTotales INT,
    DiasDisponibles INT,
    FOREIGN KEY (IdEmpresa) REFERENCES Empresas(Id),
    FOREIGN KEY (IdDepartamento) REFERENCES Departamentos(Id)
);

-- Tabla: MotivosAusencia
CREATE TABLE MotivosAusencia (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100)
);

-- Tabla: Permisos
CREATE TABLE Permisos (
    Id INT PRIMARY KEY IDENTITY,
    IdEmpleado INT,
    IdTipoPermiso INT,
    IdEmpresa INT,
    FechaGeneracion DATE,
    FechaInicio DATE,
    FechaFinal DATE,
    FOREIGN KEY (IdEmpleado) REFERENCES Empleados(Id),
    FOREIGN KEY (IdTipoPermiso) REFERENCES MotivosAusencia(Id),
    FOREIGN KEY (IdEmpresa) REFERENCES Empresas(Id)
);

-- Tabla: Roles
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100)
);

-- Tabla: UsuarioRol
CREATE TABLE UsuarioRol (
    Id INT PRIMARY KEY IDENTITY,
    IdUsuario INT,
    IdRol INT,
    IdEmpresa INT,
    IdSubEmpresa INT,
    FOREIGN KEY (IdUsuario) REFERENCES Empleados(Id),
    FOREIGN KEY (IdRol) REFERENCES Roles(Id),
    FOREIGN KEY (IdEmpresa) REFERENCES Empresas(Id),
    FOREIGN KEY (IdSubEmpresa) REFERENCES SubEmpresas(Id)
);
