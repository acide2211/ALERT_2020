/* Configuration */
INSERT INTO [dbo].[Configuration]([Cle],[Value]) VALUES ('URL','http://192.168.81.131/AlertWS/api/v1/');
INSERT INTO [dbo].[Configuration]([Cle],[Value]) VALUES ('LOGIN','ALERT');
INSERT INTO [dbo].[Configuration]([Cle],[Value]) VALUES ('PASSWORD','');
INSERT INTO [dbo].[Configuration]([Cle],[Value]) VALUES ('NOMBRESEQUENCEMAXROLE','7');

/*Type d'alarme */

INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('INCONNU','INCONNU');
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Alarme technique de toute urgence','Type_1')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Alarme technique','Type_2')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Alarme technique la veille d un WE ou JF','Type_3')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Alarme technique pdt heures de services','Type_4')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Alarme technique local','Type_5')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Intrusion répétée ou avec plainte','IN_URG')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Intrusion','IN_NORM')
INSERT INTO [dbo].[TypeAlarme] ([Libelle],[Description]) VALUES ('Incendie','INC')
/* Création des secteur */

INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('PLAINE COCKERILL','PC',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('BASSE MEUSE','BM',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('OUVEA','OUV',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('PLATEAU DE HERVE','PH',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('HESBAYE','HES',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('CONDROZ','CON',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('ARDENNES','ARD',1,1);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('MAINTENANCE MECANIQUE','MME',0,0);
INSERT INTO [dbo].[Secteur]([Nom],[Abreger],[Nombre],[ActifAlert]) VALUES ('MAINTENANCE ELECTRIQUE','MEL',0,0);

/* Création des role */

INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('PREMIERE-LIGNE',1,1,1);
INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('CHEF',1,2,2);
INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('CONTREMAITRE',1,3,2);
INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('SPO',1,4,3);
INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('REFERENT',1,5,3);
INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('DIRECTION',1,6,4);
INSERT INTO [dbo].[Role] ([Nom],[ActifAlert],[NumeroSequence],[NumeroGroupe]) VALUES ('NON-AVERTI',1,7,4);

/*Création des Personne */
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('SERAING_6' ,'SPO' ,(select Id from dbo.Role where Nom like 'SPO'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('GSM SERVICE' ,'S1' ,(select Id from dbo.Role where Nom like 'PREMIERE-LIGNE'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('GSM GARDE' ,'S1' ,(select Id from dbo.Role where Nom like 'PREMIERE-LIGNE'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('PIRRERA' ,'BERNARD' ,(select Id from dbo.Role where Nom like 'CHEF'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('SIRAGUSA' ,'NICOLAS' ,(select Id from dbo.Role where Nom like 'CHEF'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('SCHEPERS' ,'PASCAL' ,(select Id from dbo.Role where Nom like 'CHEF'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('RADOUX' ,'ANDREAS' ,(select Id from dbo.Role where Nom like 'CHEF'));
INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('SCHWANEN' ,'DOMINIQUE' ,(select Id from dbo.Role where Nom like 'CHEF'));


INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('REMACLE' ,'STEPHANE' ,(select Id from dbo.Role where Nom like 'CONTREMAITRE'))


INSERT INTO [dbo].[Personne] ([Nom] ,[Prenom] ,[RoleId]) VALUES ('DIRECTION' ,'S1-S2-S3-S4-S5P-S5S' ,(select Id from dbo.Role where Nom like 'DIRECTION'))





