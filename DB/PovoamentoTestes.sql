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
	('Casa pequena', '1','3'),
	('Casa media', '2','5'),
	('Casa grande', '3','10');

-- ----------------------------------------------------
-- VALUES FOR PropriedadeBloco
-- ---------------------------------------------------
INSERT INTO PropriedadeBloco (nome, raridade, tempoParaAdquirir)
	VALUES
	('Porta de carvalho', 'Comum','10'),
	('Tabuas de carvalho', 'Comum','5'),
	('Cerca de carvalho', 'Raro','12'),
	('Escadas de carvalho', 'Comum','8'),
	('Vidro', 'Epico','30'),
	('Pedregulho', 'Raro','20'),
	('Laje de Carvalho', 'Comum','8');

-- ----------------------------------------------------
-- VALUES FOR Construcao
-- ---------------------------------------------------
INSERT INTO Construcao (estado, idPropriedadeConstrucao, idUtilizador)
	VALUES
	('2','1','1'),
	('4','2','5'),
	('7','3','2'),
	('2','2','3'),
	('0','2','3'),
	('3','1','9');

-- ----------------------------------------------------
-- VALUES FOR BlocoProduzirConstrucao
-- ---------------------------------------------------
INSERT INTO BlocoProduzirConstrucao (idPropriedadeConstrucao, idPropriedadeBloco, quantidade)
	VALUES
	('1','2','20'),
	('2','2','30'),
	('2','1','1'),
	('3','2','45'),
	('3','1','2'),
	('3','5','20'),
	('3','6','30');

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