-- Universidade do Minho
-- Laboratorios de Inform�tica IV
-- Grupo: 1

-- ----------------------------------------------------
-- DATABASE USAGE
-- ----------------------------------------------------
USE LI4;

-- ----------------------------------------------------
-- VALUES FOR Users
-- ---------------------------------------------------
DELETE FROM Users;
SET IDENTITY_INSERT Users ON
INSERT INTO Users (id, username, email, userPassword)
	VALUES
    ('1', 'Paulo', 'paulo@example.com', 'password1'),
    ('2', 'Maria', 'maria@example.com', 'password2'),
    ('3', 'Joao', 'joao@example.com', 'password3'),
    ('4', 'Ana', 'ana@example.com', 'password4'),
    ('5', 'Carlos', 'carlos@example.com', 'password5'),
    ('6', 'Beatriz', 'beatriz@example.com', 'password6'),
    ('7', 'Luis', 'luis@example.com', 'password7'),
    ('8', 'Sara', 'sara@example.com', 'password8'),
    ('9', 'Miguel', 'miguel@example.com', 'password9'),
    ('10', 'Ines', 'ines@example.com', 'password10');
SET IDENTITY_INSERT Users OFF

-- ----------------------------------------------------
-- VALUES FOR ConstructionProperties
-- ---------------------------------------------------
SET IDENTITY_INSERT ConstructionProperties ON
INSERT INTO ConstructionProperties (id, name, dificulty, nStages)
	VALUES
	('1', 'Pillager Tent', 'LOW','3'),
	('2', 'Small House 3', 'MEDIUM','5'),
	('3', 'Library 2', 'HIGH','7');
SET IDENTITY_INSERT ConstructionProperties OFF

-- ----------------------------------------------------
-- VALUES FOR BlockProperties (OS TEMPOS ESTAO ALEATORIOS)
-- ---------------------------------------------------
SET IDENTITY_INSERT BlockProperties ON
INSERT INTO BlockProperties (id, name, rarity, timeToAcquire)
	VALUES
	('1', 'Yellow Bed', 'RARE','3'),
	('2', 'Bookshelf', 'EPIC','4'),
	('3', 'Cobblestone', 'COMMON','1'),
	('4', 'Crafting Table', 'COMMON','2'),
	('5', 'Dirt', 'COMMON','1'),
	('6', 'Glass Pane', 'RARE','1'),
	('7', 'Grass Block', 'EPIC','2'),
	('8', 'Lectern', 'RARE','1'),
	('9', 'Oak Door', 'COMMON','2'),
	('10', 'Oak Fence', 'COMMON','1'),
	('11', 'Oak Log', 'COMMON','3'),
	('12', 'Oak Planks', 'COMMON','4'),
	('13', 'Oak Stairs', 'COMMON','1'),
	('14', 'White Wool', 'RARE','3');
SET IDENTITY_INSERT BlockProperties OFF

-- ----------------------------------------------------
-- VALUES FOR Construction
-- ---------------------------------------------------
DELETE FROM Constructions;
SET IDENTITY_INSERT Constructions ON
INSERT INTO Constructions (id, state, idConstructionProperties, idUser)
    VALUES
    ('1', 'COMPLETED','1','1'),
    ('2', 'COMPLETED','2','5'),
    ('3', 'BUILDING','3','2'),
    ('4', 'WAITING','2','3'),
    ('5', 'WAITING','2','3'),
    ('6', 'BUILDING','1','9'),
    ('7', 'WAITING', '1', '2'),
    ('8', 'BUILDING', '2', '4'),
    ('9', 'WAITING', '3', '5'),
    ('10', 'COMPLETED', '1', '7'),
    ('11', 'BUILDING', '2', '8'),
    ('12', 'WAITING', '3', '9'),
    ('13', 'COMPLETED', '1', '10'),
    ('14', 'BUILDING', '2', '3'),
    ('15', 'WAITING', '3', '6'),
    ('16', 'COMPLETED', '1', '1'),
    ('17', 'BUILDING', '2', '2'),
    ('18', 'WAITING', '3', '4'),
    ('19', 'COMPLETED', '1', '5'),
    ('20', 'BUILDING', '2', '6'),
    ('21', 'WAITING', '3', '7'),
    ('22', 'COMPLETED','1','8'),
    ('23', 'BUILDING','2','9'),
    ('24', 'WAITING','3','10'),
    ('25', 'COMPLETED','1','4'),
    ('26', 'BUILDING','2','5'),
    ('27', 'WAITING','3','1'),
    ('28', 'COMPLETED','1','2'),
    ('29', 'BUILDING','2','3'),
    ('30', 'WAITING','3','4'),
    ('31', 'COMPLETED','1','6'),
    ('32', 'BUILDING','2','7'),
    ('33', 'WAITING','3','8'),
    ('34', 'COMPLETED','1','9'),
    ('35', 'BUILDING','2','10'),
    ('36', 'WAITING','3','1'),
    ('37', 'COMPLETED','1','3'),
    ('38', 'BUILDING','2','4'),
    ('39', 'WAITING','3','5'),
    ('40', 'COMPLETED','1','7');
SET IDENTITY_INSERT Constructions OFF;

-- ----------------------------------------------------
-- VALUES FOR ConstructionStages
-- ---------------------------------------------------
INSERT INTO ConstructionStages (idConstructionProperties, stage, time)
	VALUES
	('1','1','5'),
	('1','2','10'),
	('1','3','2'),

	('2','1','5'),
	('2','2','10'),
	('2','3','8'),
	('2','4','6'),
	('2','5','10'),
	
	('3','1','5'),
	('3','2','15'),
	('3','3','10'),
	('3','4','8'),
	('3','5','3'),
	('3','6','4'),
	('3','7','10');

-- ---------------------------------------------------
-- VALUES FOR BlocksToConstruction
-- ---------------------------------------------------
INSERT INTO BlocksToConstruction (idConstructionProperties, idBlockProperty, stage, quantity)
	VALUES
	-- Construction 1
	('1', '4', '1', '1'),
	('1', '11', '2', '10'),
	('1', '15', '3', '20'),
	-- Construction 1

	-- Construction 2
	('2', '12', '1', '22'),
	('2', '13', '2', '9'),
	('2', '3', '3', '37'),
	('2', '13', '4', '26'),
	('2', '14', '4', '48'),
	('2', '10', '5', '1'),
	('2', '1', '5', '1'),
	('2', '7', '5', '3'),
	-- Construction 2

	-- Construction 3
	('3', '3', '1', '35'),

	('3', '12', '2', '32'),

	('3', '3', '3', '40'),

	('3', '12', '4', '15'),

	('3', '13', '5', '56'),

	('3', '12', '6', '13'),
	('3', '14', '6', '54'),

	('3', '8', '7', '10'),
	('3', '11', '7', '11'),
	('3', '2', '7', '4'),
	('3', '14', '7', '4'),
	('3', '13', '7', '1'),
	('3', '7', '7', '8'),
	('3', '10', '7', '4'),
	('3', '9', '7', '1');
	-- Construction 3

-- ----------------------------------------------------
-- VALUES FOR Orders
-- ---------------------------------------------------
SET IDENTITY_INSERT Orders ON
INSERT INTO Orders (id, idUser, orderDate, delivered)
	VALUES
    ('1', '1', '2025-01-01 10:30:00', 'false'),
    ('2', '2', '2025-01-02 14:45:00', 'false'),
    ('3', '3', '2025-01-03 09:15:00', 'false'),
    ('4', '4', '2025-01-04 16:00:00', 'false'),
    ('5', '5', '2025-01-05 12:00:00', 'false'),
    ('6', '6', '2025-01-06 18:20:00', 'false'),
    ('7', '7', '2025-01-07 08:00:00', 'false');
SET IDENTITY_INSERT Orders OFF

-- ----------------------------------------------------
-- VALUES FOR BlocksInOrder
-- ---------------------------------------------------
INSERT INTO BlocksInOrder (idOrder, idBlockProperty, quantity)
	VALUES
    ('1', '1','3'),
	('1', '2', '50'),
    ('2', '2','1'),
    ('3', '3','1'),
    ('4', '4','1'),
    ('5', '5','1'),
    ('6', '6','23'),
    ('7', '7','10');

-- ----------------------------------------------------
-- VALUES FOR Blocks
-- ---------------------------------------------------
INSERT INTO Blocks (quantity, idBlockProperty, idUser)
	VALUES
	('14', '1', '1'),
	('2', '2', '1'),
	('6', '3', '1'),
    ('52', '3', '3'),
    ('12', '4', '4'),
	('47', '3', '2'),
	('10', '7', '2'),
	('3', '10', '2'),
	('40', '12', '2'),
	('60', '13', '2'),
	('80', '14', '2'),
	('10', '1', '2'),
    ('17', '5', '4');
