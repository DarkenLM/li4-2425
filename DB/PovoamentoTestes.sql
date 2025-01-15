-- Universidade do Minho
-- Laboratorios de Informática IV
-- Grupo: 1

-- ----------------------------------------------------
-- DATABASE USAGE
-- ----------------------------------------------------
USE LI4;

-- ----------------------------------------------------
-- VALUES FOR Utilizador
-- ---------------------------------------------------
INSERT INTO Utilizador (username, email, palavraPasse)
	VALUES
    ('Paulo', 'paulo@example.com', 'password1'),
    ('Maria', 'maria@example.com', 'password2'),
    ('Joao', 'joao@example.com', 'password3'),
    ('Ana', 'ana@example.com', 'password4'),
    ('Carlos', 'carlos@example.com', 'password5'),
    ('Beatriz', 'beatriz@example.com', 'password6'),
    ('Luis', 'luis@example.com', 'password7'),
    ('Sara', 'sara@example.com', 'password8'),
    ('Miguel', 'miguel@example.com', 'password9'),
    ('Ines', 'ines@example.com', 'password10');

-- ----------------------------------------------------
-- VALUES FOR PropriedadeConstrucao
-- ---------------------------------------------------
INSERT INTO PropriedadeConstrucao (nome, dificuladade, estagio)
	VALUES
	('Pillager Tent', '1','3'),
	('Small House 3', '2','5'),
	('Library 2', '3','7');

-- ----------------------------------------------------
-- VALUES FOR PropriedadeBloco (OS TEMPOS ESTAO ALEATORIOS)
-- ---------------------------------------------------
INSERT INTO PropriedadeBloco (nome, raridade, tempoParaAdquirir)
	VALUES
	('Bed', 'RARE','3'),
	('Bookshelf', 'EPIC','4'),
	('Cobblestone', 'COMMON','1'),
	('Crafting Table', 'COMMON','2'),
	('Dirt', 'COMMON','1'),
	('Dirt Path', 'RARE','2'),
	('Glass Pane', 'RARE','1'),
	('Grass Block', 'EPIC','2'),
	('Lectern', 'RARE','1'),
	('Oak Door', 'COMMON','2'),
	('Oak Fence', 'COMMON','1'),
	('Oak Log', 'COMMON','3'),
	('Oak Planks', 'COMMON','4'),
	('Oak Stairs', 'COMMON','1'),
	('White Wool', 'RARE','3');

-- ----------------------------------------------------
-- VALUES FOR Construcao
-- ---------------------------------------------------
INSERT INTO Construcao (estado, idPropriedadeConstrucao, idUtilizador)
	VALUES
	('2','1','1'),
	('2','2','5'),
	('1','3','2'),
	('0','2','3'),
	('0','2','3'),
	('2','1','9');

-- ----------------------------------------------------
-- VALUES FOR BlocoProduzirConstrucao
-- ---------------------------------------------------
INSERT INTO BlocoProduzirConstrucao (idPropriedadeConstrucao, idPropriedadeBloco, quantidade)
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
-- VALUES FOR Encomenda
-- ---------------------------------------------------
INSERT INTO Encomenda (idUtilizador, dataEncomenda)
	VALUES
    ('1', '2025-01-01 10:30:00'),
    ('2', '2025-01-02 14:45:00'),
    ('3', '2025-01-03 09:15:00'),
    ('4', '2025-01-04 16:00:00'),
    ('5', '2025-01-05 12:00:00'),
    ('6', '2025-01-06 18:20:00'),
    ('7', '2025-01-07 08:00:00');

-- ----------------------------------------------------
-- VALUES FOR Bloco
-- ---------------------------------------------------
INSERT INTO Bloco (idPropriedadeBloco, idEncomenda)
	VALUES
	('1', '1'),
    ('2', '2'),
    ('3', '3'),
    ('4', '4'),
    ('5', '4'); 