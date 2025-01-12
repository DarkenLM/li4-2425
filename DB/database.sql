-- Universidade do Minho
-- Laboratorios de Informática IV
-- Grupo: 1

-- ----------------------------------------------------
-- DATABASE SETUP
-- ----------------------------------------------------
-- Drop the database if it exists (use cautiously in development)
--IF DB_ID('LI4') IS NOT NULL
--BEGIN
--	USE master;
--	DROP DATABASE LI4;
--END;

-- Create the database
CREATE DATABASE LI4;

-- Switch to the newly created database
USE LI4;

-- ----------------------------------------------------
-- TABLE LI4.Utilizador
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Utilizador')
BEGIN
	CREATE TABLE Utilizador(
		id INT IDENTITY(1,1) PRIMARY KEY,
		username VARCHAR(40) NOT NULL UNIQUE,
		email VARCHAR(70) NOT NULL UNIQUE,
		palavraPasse VARCHAR(45) NOT NULL
	)
END

-- ----------------------------------------------------
-- TABLE LI4.PropriedadeConstrucao
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='PropriedadeConstrucao')
BEGIN
	CREATE TABLE PropriedadeConstrucao(
		id INT IDENTITY(1,1) PRIMARY KEY,
		nome VARCHAR(50) NOT NULL,
		dificuladade VARCHAR(6) NOT NULL,
		estagio INT NOT NULL
	)
END
-- ----------------------------------------------------
-- TABLE LI4.PropriedadeBloco
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='PropriedadeBloco')
BEGIN
	CREATE TABLE PropriedadeBloco(
		id INT IDENTITY(1,1) PRIMARY KEY,
		nome VARCHAR(40) NOT NULL,
		raridade VARCHAR(7) NOT NULL,
		tempoParaAdquirir INT NOT NULL,
	)
END

-- ----------------------------------------------------
-- TABLE LI4.Construcao
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Construcao')
BEGIN
	CREATE TABLE Construcao(
		id INT IDENTITY(1,1) PRIMARY KEY,
		estado VARCHAR(9) NOT NULL,
		idPropriedadeConstrucao INT NOT NULL FOREIGN KEY REFERENCES PropriedadeConstrucao(id),
		idUtilizador INT NOT NULL FOREIGN KEY REFERENCES Utilizador(id)
	)
END

-- ----------------------------------------------------
-- TABLE LI4.BlocoProduzirConstrucao
-- 
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='BlocoProduzirConstrucao')
BEGIN
	CREATE TABLE BlocoProduzirConstrucao(
		idPropriedadeConstrucao INT NOT NULL FOREIGN KEY REFERENCES PropriedadeConstrucao(id),
		idPropriedadeBloco INT NOT NULL FOREIGN KEY REFERENCES PropriedadeBloco(id),
		quantidade INT NOT NULL,
		CONSTRAINT PK_BlocoProduzirConstrucao PRIMARY KEY (idPropriedadeConstrucao, idPropriedadeBloco)
	)
END

-- ----------------------------------------------------
-- TABLE LI4.Encomenda
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Encomenda')
BEGIN
	CREATE TABLE Encomenda(
		id INT IDENTITY(1,1) PRIMARY KEY,
		idUtilizador INT NOT NULL FOREIGN KEY REFERENCES Utilizador(id),
		dataEncomenda DATETIME NOT NULL,
	)
END

-- ----------------------------------------------------
-- TABLE LI4.Bloco
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Bloco')
BEGIN
	CREATE TABLE Bloco(
		id INT IDENTITY(1,1) PRIMARY KEY,
		idPropriedadeBloco INT NOT NULL FOREIGN KEY REFERENCES PropriedadeBloco(id),
		idEncomenda INT NOT NULL FOREIGN KEY REFERENCES Encomenda(id)
	)
END