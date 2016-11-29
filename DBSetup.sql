-- http://stackoverflow.com/questions/27606518/how-to-drop-all-tables-from-database-with-one-sql-query
DECLARE @sql NVARCHAR(max)=''

SELECT @sql += ' Drop table ' + QUOTENAME(TABLE_SCHEMA) + '.'+ QUOTENAME(TABLE_NAME) + '; '
FROM   INFORMATION_SCHEMA.TABLES
WHERE  TABLE_TYPE = 'BASE TABLE'

Exec Sp_executesql @sql 

CREATE TABLE Preaches 
(
id bigint PRIMARY KEY,
preachType varchar(50) NOT NULL,
preachSentence varchar(MAX) NOT NULL
)

-- http://stackoverflow.com/questions/15800250/add-unique-constraint-to-combination-of-two-columns
--ALTER TABLE Preaches
--ADD CONSTRAINT uq_preachtypeandsentence UNIQUE(preachType, preachSentence);

INSERT INTO Preaches (id, preachType, preachSentence)
VALUES (1, 'mindset', 'You must have the correct mindset'),
(2, 'mindset', 'Think.'),
(8, 'mindset', 'When I make decisions , I have to position myself as a parent.'),
(11, 'skill', 'To me...we all need solid skills. If do not have curiosity how it is built...in future how to innovate?  Thats how China and south Korea and Japan became strong in IT and engineering'),
(3, 'gogiver', 'Chiong your skills and you will achieve law 3.'),
(4, 'gogiver', 'Buy the book'),
(9, 'gogiver', 'Read gogiver ba'),
(10, 'gogiver', 'Read gogiVer...check out third law'),
(12, 'gogiver', 'Whoever master gogiver will have a good future.... So far one out of hundred is able to. Which is still a good ratio coz the person who wants a good future must be wiser and able to grow people may it be at age of 20 30 40 etc'),
(17, 'gogiver', 'give me your bank accounts'),
(5, 'focus', 'Focus is excellence.'),
(6, 'focus', 'FOCUS.'),
(13, 'focus', 'As long as a person is reputated as pleasant , and focus especially in crisis.....Google and many good organizations will find them .... Not they find Google and other firms'),
(7, 'habits', 'Cultivate your habits.'),
(14, 'habits', 'Code code code.....'),
(15, 'none', 'Ah tan tired.. preach later'),
(16, 'none', 'Wealth is people.');

CREATE TABLE Timetable
(
id bigint PRIMARY KEY,
ModuleChar varchar(10) NOT NULL,
ModuleFullName varchar(100) NOT NULL,
ModuleClass varchar(10) NOT NULL,
StartTime time NOT NULL,
EndTime time NOT NULL,
SlotDay int NOT NULL
)

CREATE TABLE Empathies
(
id bigint PRIMARY KEY,
description varchar(MAX) NULL,
url varchar(500) NULL
)

INSERT INTO Empathies (id, description, url)
VALUES (1, NULL, 'https://www.edutopia.org/blog/8-pathways-empathy-in-action-marilyn-price-mitchell');
