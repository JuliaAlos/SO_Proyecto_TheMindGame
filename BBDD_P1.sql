DROP DATABASE IF EXISTS  bd;
CREATE DATABASE bd;

USE bd;

CREATE TABLE Jugador(
   ID INT NOT NULL,
   Usuario VARCHAR(50) NOT NULL, 
   Psw VARCHAR(50),
   PRIMARY KEY (ID) 

)ENGINE=InnoDB;

CREATE TABLE Partida(
   ID INT NOT NULL,
   Data VARCHAR(50),
   Time INT,
   Winner VARCHAR(50),
   PRIMARY KEY(ID)

)ENGINE=InnoDB;

CREATE TABLE Participantes(
   ID_J INT NOT NULL,
   ID_P INT NOT NULL,
   FOREIGN KEY (ID_J) REFERENCES Jugador(ID),
   FOREIGN KEY (ID_P) REFERENCES Partida(ID)

)ENGINE=InnoDB;

INSERT INTO Jugador VALUES (1, 'Joel', 'Delgar24');
INSERT INTO Jugador VALUES (2, 'Julia', 'pumpkin');
INSERT INTO Jugador VALUES (3, 'Fran', '1234a5b');

INSERT INTO Partida VALUES (1, '13/10/2020-17:23', 20, 'Joel');
INSERT INTO Partida VALUES (2, '13/10/2020-17:50', 15, 'Julia');
INSERT INTO Partida VALUES (3, '13/10/2020-18:34', 19, 'Fran');

INSERT INTO Participantes VALUES (1, 1);
INSERT INTO Participantes VALUES (3, 1);
INSERT INTO Participantes VALUES (1, 2);
INSERT INTO Participantes VALUES (2, 2);
INSERT INTO Participantes VALUES (3, 2);
INSERT INTO Participantes VALUES (2, 3);
INSERT INTO Participantes VALUES (3, 3);

SELECT * FROM Jugador;
SELECT * FROM Partida;
SELECT * FROM Participantes;
