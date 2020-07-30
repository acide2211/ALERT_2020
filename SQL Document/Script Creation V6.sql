Create Table Siecle(
 Id int Identity(1,1) not null,
 Numero int, 
 PRIMARY key (Id)
);

Create Table Annee(
 Id int Identity(1,1) not null,
 Annee int,
 PRIMARY key (Id),
 SiecleId int FOREIGN KEY REFERENCES Siecle(Id) not null
);

Create Table Mois(
 Id int Identity(1,1) not null,
 Mois varchar(128),
 Numero int,
 PRIMARY key (Id),
 AnneeId int FOREIGN KEY REFERENCES Annee(Id) not null
);

Create Table JourSemaine(
 Id int Identity(1,1) not null,
 Jour varchar(128),
 Numero int,
 PRIMARY key (Id),

);

Create Table Jour(
 Id int Identity(1,1) not null,
 Numero int,
 PRIMARY key (Id),
 JourSemaineId int FOREIGN KEY REFERENCES JourSemaine(Id) not null,
 MoisId int FOREIGN KEY REFERENCES Mois(Id) not null

);

Create Table Periode(
 Id int Identity(1,1) not null,
 Nom varchar(128),
 PRIMARY key (Id),

);

 /***************************/
/* TypeAlarme */
/****************************/
Create Table TypeAlarme(
 Id int Identity(1,1) not null,
 Libelle varchar (128),
 Description varchar (12),
 PRIMARY key (Id),
 );

 /***************************/
/* Tranche */
/****************************/

Create Table Tranche(
 Id int Identity(1,1) not null,
 Numero int,
 PRIMARY key (Id),
 JourId int FOREIGN KEY REFERENCES Jour(Id) not null,
 PeriodeId int FOREIGN KEY REFERENCES Periode(Id) not null,
 TypeAlarmeId int FOREIGN KEY REFERENCES TypeAlarme(Id) not null
);

/***************************/
/* Secteur */
/****************************/

Create Table Secteur(
 Id int Identity(1,1) not null,
 Nom varchar (128),
 Abreger varchar (128),
 Nombre int,
 ActifAlert bit not null,
 PRIMARY key (Id),
 );
 
 /***************************/
/* Commune */
/****************************/

Create Table Commune(
 Id int Identity(1,1) not null,
 Nom varchar (128),
 CodePostal int,
 PRIMARY key (Id),
 );
/***************************/
/* TypeStation */
/****************************/

Create Table TypeStation(
 Id int Identity(1,1) not null,
 Descrition varchar (128),
 TypeAbreger varchar (12),
 PRIMARY key (Id),
 );

/***************************/
/* Station */
/****************************/
Create Table Station(
 Id int Identity(1,1) not null,
 Nom varchar (128),
 AbregerTBOX varchar(128),
 CodeStation varchar(128),
 Adresse varchar (255),
 TelephoneStation varchar (100),
 Telephoneautomate varchar (100),
 URL varchar (2048),
 Coordonnee varchar (2048), 
 NumeroSecteur int,
 PRIMARY key (Id),
 SecteurId int FOREIGN KEY REFERENCES Secteur(Id) not null,
 CommuneId int FOREIGN KEY REFERENCES Commune(Id) not null,
 TypeStationId int FOREIGN KEY REFERENCES TypeStation(Id) not null
 );
 


/***************************/
/* Alarme */
/****************************/
Create Table Alarme(
 Id int Identity(1,1) not null,
 Code int not null,
 MessageLong varchar (128) not null,
 MessageCourt varchar (128) not null,
 PRIMARY key (Id),
 TypeAlarmeId int FOREIGN KEY REFERENCES TypeAlarme(Id) not null,
 StationId int FOREIGN KEY REFERENCES Station(Id) not null
 );

/***************************/
/* Role */
/****************************/
Create Table Role(
 Id int Identity(1,1) not null,
 Nom varchar(128) not null,
 ActifAlert bit not null,
 NumeroSequence int,
 NumeroGroupe int,
 PRIMARY key (Id),
 );

/***************************/
/* Personne */
/****************************/
Create Table Personne(
 Id int Identity(1,1) not null,
 Nom varchar(128) not null,
 Prenom varchar(128) not null,
 PRIMARY key (Id),
 RoleId int FOREIGN KEY REFERENCES Role(Id) not null,
 );
 
 /***************************/
/* Prioriter */
/****************************/
Create Table Prioriter(
 Id int Identity(1,1) not null,
 Prioriter int not null,
 NumeroSecteur int,
 PRIMARY key (Id),
 RoleId int FOREIGN KEY REFERENCES Role(Id) not null,
 SecteurId int FOREIGN KEY REFERENCES Secteur(Id) not null,
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null
 );
 
/***************************/
/* HorsService */
/****************************/
Create Table HorsService(
 Id int Identity(1,1) not null,
 DateDebut date not null,
 DateFin date not null,
 PRIMARY key (Id),
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null
 );
 
 /***************************/
/* StationMasquee */
/****************************/
Create Table StationMasquee(
 Id int Identity(1,1) not null,
 DateDebut date not null,
 DateFin date not null,
 Comment varchar (1024),
 PRIMARY key (Id),
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null,
 StationId int FOREIGN KEY REFERENCES Station(Id) not null
 );
 
/***************************/
/* AlarmeMasquee */
/****************************/
Create Table AlarmeMasquee(
 Id int Identity(1,1) not null,
 DateDebut date not null,
 DateFin date not null,
 Comment varchar (1024),
 PRIMARY key (Id),
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null,
 AlarmeId int FOREIGN KEY REFERENCES Alarme(Id) not null
 );
 
/***************************/
/* TypeDriver */
/****************************/
Create Table TypeDriver(
 Id int Identity(1,1) not null,
 Numero int not null,
 Name varchar (255),
 PRIMARY key (Id)
 );
 
/***************************/
/* DriverConfig */
/****************************/
Create Table DriverConfig(
 Id int Identity(1,1) not null,
 Prioriter int ,
 AutoAck bit,
 Adresse varchar(1024),
 CountAck int,
 CountTry int,
 Final bit,
 TimeToAck int,
 TimeToRetry int,
 Valide bit,
 PRIMARY key (Id),
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null,
 TypeDriverId int FOREIGN KEY REFERENCES TypeDriver(Id) not null
 );
 
/***************************/
/* ModeIntervention*/
/****************************/
 Create Table ModeIntervention(
 Id int Identity(1,1) not null,
 DateDebut date not null,
 DateFin date not null,
 PRIMARY key (Id),
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null,
 StationId int FOREIGN KEY REFERENCES Station(Id) not null
 );
 
/***************************/
/* Configuration			*/	
/****************************/
 Create Table Configuration(
 Id int Identity(1,1) not null,
 Cle varchar (1024) not null,
 Value varchar(1024) not null,
 PRIMARY key (Id),

 );
 
 /***************************/
/* SecteurPersonne			*/
/****************************/
 Create Table SecteurPersonne(
 Id int Identity(1,1) not null,
 PRIMARY key (Id),
 PersonneId int FOREIGN KEY REFERENCES Personne(Id) not null,
 SecteurId int FOREIGN KEY REFERENCES Secteur(Id) not null
 );