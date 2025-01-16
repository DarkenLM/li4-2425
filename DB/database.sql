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

-- Create User
--CREATE LOGIN batman WITH PASSWORD = '554456asryf74sgr4f8ysyf4564ffhdsdfghz465shrd4hsdf56';

-- Create the database
CREATE DATABASE LI4;

-- Switch to the newly created database
USE LI4;

-- Create User for Login
CREATE USER batman FOR LOGIN batman;
ALTER ROLE db_owner ADD MEMBER batman;

-- ----------------------------------------------------
-- TABLE LI4.Users
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Users')
BEGIN
	CREATE TABLE Users(
		id INT IDENTITY(1,1) PRIMARY KEY,
		username VARCHAR(40) NOT NULL UNIQUE,
		email VARCHAR(70) NOT NULL UNIQUE,
		userPassword VARCHAR(45) NOT NULL
	)
END

-- ----------------------------------------------------
-- TABLE LI4.ConstructionProperties
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='ConstructionProperties')
BEGIN
	CREATE TABLE ConstructionProperties(
		id INT IDENTITY(1,1) PRIMARY KEY,
		name VARCHAR(50) NOT NULL,
		dificulty VARCHAR(8) NOT NULL,
		nStages INT NOT NULL
	)
END

-- ----------------------------------------------------
-- TABLE LI4.BlockProperties
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='BlockProperties')
BEGIN
	CREATE TABLE BlockProperties(
		id INT IDENTITY(1,1) PRIMARY KEY,
		name VARCHAR(40) NOT NULL,
		rarity VARCHAR(7) NOT NULL,
		timeToAcquire INT NOT NULL,
	)
END

-- ----------------------------------------------------
-- TABLE LI4.Constructions
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Constructions')
BEGIN
	CREATE TABLE Constructions(
		id INT IDENTITY(1,1) PRIMARY KEY,
		state VARCHAR(10) NOT NULL,
		idConstructionProperties INT NOT NULL FOREIGN KEY REFERENCES ConstructionProperties(id),
		idUser INT NOT NULL FOREIGN KEY REFERENCES Users(id)
	)
END

-- ----------------------------------------------------
-- TABLE LI4.BlocksToConstruction
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='BlocksToConstruction')
BEGIN
	CREATE TABLE BlocksToConstruction(
		idConstructionProperties INT NOT NULL FOREIGN KEY REFERENCES ConstructionProperties(id),
		idBlockProperty INT NOT NULL FOREIGN KEY REFERENCES BlockProperties(id),
		quantity INT NOT NULL,
		CONSTRAINT PK_BlocksToConstruction PRIMARY KEY (idConstructionProperties, idBlockProperty)
	)
END

-- ----------------------------------------------------
-- TABLE LI4.Orders
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Orders')
BEGIN
	CREATE TABLE Orders(
		id INT IDENTITY(1,1) PRIMARY KEY,
		idUser INT NOT NULL FOREIGN KEY REFERENCES Users(id),
		orderDate DATETIME NOT NULL,
	)
END

-- ----------------------------------------------------
-- TABLE LI4.BlocksInOrder
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='BlocksInOrder')
BEGIN
	CREATE TABLE BlocksInOrder(
		idOrder INT NOT NULL FOREIGN KEY REFERENCES Orders(id),
		idBlockProperty INT NOT NULL FOREIGN KEY REFERENCES BlockProperties(id),
		quantity INT NOT NULL,
		CONSTRAINT PK_BlocksInOrder PRIMARY KEY (idOrder, idBlockProperty)
	)
END

-- ----------------------------------------------------
-- TABLE LI4.Blocks
-- ----------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Blocks')
BEGIN
	CREATE TABLE Blocks(
		id INT IDENTITY(1,1) PRIMARY KEY,
		idBlockProperty INT NOT NULL FOREIGN KEY REFERENCES BlockProperties(id),
		idUser INT NOT NULL FOREIGN KEY REFERENCES Users(id),
	)
END