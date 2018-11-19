Maak binnen deze map een map aan met als naam de naam van het versienummer
-------------
VERSIE NUMMER
Versie nummers moet zijn van formaat:
x.x.x.xxx

hierbij is x getal tussen 0 en 9
dus na 2.0.2.999 komt dus  2.0.3.001
en na 2.0.9.xxx komt 2.1.0.xxx als de minor nummer wilt ophogen
--------------
WELKE BESTANDEN? 
in map zet je alleen de bestanden die je wilt vervangen
dll's en scripts, en andere bestanden
--------------
DATABASE
Voor database updates: Binnen de map met versienummer zet je een mapje met naam DB
In dit mapje zet je de bestanden die moeten worden geupdate
--------------
FULLVERSION 
Voor aanmaken van nieuwe sites met bitplate
Binnen deze map bevindt zich ook de map met naam FUllVersion
Hierin staat weer een map met naam van versienummer
In deze map staat laatste complete versie van bitplate
Binnen deze map staat ook weer een map DB met daarin laatste versie van database
CREATETABLES.sql
INSERTDEFAULTS.sql
(Create Database en Create User hoeven er niet in, die worden in code aangemaakt)

