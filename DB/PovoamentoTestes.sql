-- Universidade do Minho
-- Laboratorios de Informática IV
-- Grupo: 1

-- ----------------------------------------------------
-- DATABASE USAGE
-- ----------------------------------------------------
USE LI4;

-- ----------------------------------------------------
-- VALUES FOR Users
-- ---------------------------------------------------
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
	('3', 'Library 2', 'HARD','7');
SET IDENTITY_INSERT ConstructionProperties OFF

-- ----------------------------------------------------
-- VALUES FOR BlockProperties (OS TEMPOS ESTAO ALEATORIOS)
-- ---------------------------------------------------
SET IDENTITY_INSERT BlockProperties ON
INSERT INTO BlockProperties (id, name, rarity, timeToAcquire)
	VALUES
	('1', 'Bed', 'RARE','3'),
	('2', 'Bookshelf', 'EPIC','4'),
	('3', 'Cobblestone', 'COMMON','1'),
	('4', 'Crafting Table', 'COMMON','2'),
	('5', 'Dirt', 'COMMON','1'),
	('6', 'Dirt Path', 'RARE','2'),
	('7', 'Glass Pane', 'RARE','1'),
	('8', 'Grass Block', 'EPIC','2'),
	('9', 'Lectern', 'RARE','1'),
	('10', 'Oak Door', 'COMMON','2'),
	('11', 'Oak Fence', 'COMMON','1'),
	('12', 'Oak Log', 'COMMON','3'),
	('13', 'Oak Planks', 'COMMON','4'),
	('14', 'Oak Stairs', 'COMMON','1'),
	('15', 'White Wool', 'RARE','3');
SET IDENTITY_INSERT BlockProperties OFF

-- ----------------------------------------------------
-- VALUES FOR Construction
-- ---------------------------------------------------
SET IDENTITY_INSERT Constructions ON
INSERT INTO Constructions (id, state, idConstructionProperties, idUser)
	VALUES
	('1', 'COMPLETED','1','1'),
	('2', 'COMPLETED','2','5'),
	('3', 'BUILDING','3','2'),
	('4', 'WAITING','2','3'),
	('5', 'WATTING','2','3'),
	('6', 'COMPLETED','1','9');
SET IDENTITY_INSERT Constructions OFF

-- ---------------------------------------------------
-- VALUES FOR BlocksToConstruction
-- ---------------------------------------------------
INSERT INTO BlocksToConstruction (idConstructionProperties, idBlockProperty, quantity)
	VALUES
	('1','15','20'),
	('1','11','10'),
	('1','4','1'),
	
	('2','3','37'),
	('2','7','3'),
	('2','10','1'),
	('2','12','22'),
	('2','13','32'),
	('2','14','50'),
	('2','1','1'),
	
	('3','3','55'),
	('3','2','4'),
	('3','5','21'),
	('3','6','4'),
	('3','7','8'),
	('3','8','2'),
	('3','9','1'),
	('3','10','4'),
	('3','11','18'),
	('3','12','60'),
	('3','13','57'),
	('3','14','70');

-- ----------------------------------------------------
-- VALUES FOR Orders
-- ---------------------------------------------------
SET IDENTITY_INSERT Orders ON
INSERT INTO Orders (id, idUser, orderDate)
	VALUES
    ('1', '1', '2025-01-01 10:30:00'),
    ('2', '2', '2025-01-02 14:45:00'),
    ('3', '3', '2025-01-03 09:15:00'),
    ('4', '4', '2025-01-04 16:00:00'),
    ('5', '5', '2025-01-05 12:00:00'),
    ('6', '6', '2025-01-06 18:20:00'),
    ('7', '7', '2025-01-07 08:00:00');
SET IDENTITY_INSERT Orders OFF

-- ----------------------------------------------------
-- VALUES FOR BlocksInOrder
-- ---------------------------------------------------
INSERT INTO BlocksInOrder (idOrder, idBlockProperty, quantity)
	VALUES
    ('1', '1','3'),
    ('2', '2','1'),
    ('3', '3','1'),
    ('4', '4','1'),
    ('5', '5','1'),
    ('6', '6','23'),
    ('7', '7','10');

-- ----------------------------------------------------
-- VALUES FOR Blocks
-- ---------------------------------------------------
SET IDENTITY_INSERT Blocks ON
INSERT INTO Blocks (id, idBlockProperty, idUser)
	VALUES
	('1', '1', '1'),
	('2', '1', '1'),
	('3', '1', '1'),
    ('4', '2', '2'),
    ('5', '3', '3'),
    ('6', '4', '4'),
    ('7', '5', '4');
SET IDENTITY_INSERT Blocks OFF