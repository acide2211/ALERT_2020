﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessToAlert;
using BusinessToDBAlert;
using NLog;

namespace Initialisation
{
    public class Program
    {
        #region Déclaration des variables 
        // Variable qui permet d'interagir avec la dll de la base de données
        
        //Variable qui permet de crée les logs
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        static void Main(string[] args)
        {
            ManagerDB managerDB;
            ManagerAlert managerAlert;
            InitialisationControler initialisationControler;
            try
            {
                Logger.Info("Programme Init");
                managerDB = ManagerDB.getInstance(Logger);
                managerAlert = ManagerAlert.getInstance(Logger);

                initialisationControler = new InitialisationControler(managerDB, managerAlert, Logger);

                // Connexions a Alert pour les Appelles API
                managerAlert.LoginAlertWS();

                // Création des calls groups
                Logger.Info("DEBUT : Creation des call group terminer");
                initialisationControler.CreateCallGroup();
                Logger.Info("FIN : Creation des call group terminer");

                // Liaison entre les call groupe
                Logger.Info("DEBUT : Creation des liaison entre les call group");
                initialisationControler.LiaisonCallGroup();
                Logger.Info("FIN : Creation des liaison entre les call group");

                // Création des utilisateurs dans alert

                Logger.Info("DEBUT : Creation des utilisateurs");
                initialisationControler.CreateUsers();
                Logger.Info("FIN : Creation des utilisateurs");


                Logger.Info("FIN : Initialisation");
                Thread.Sleep(5000);
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur Fatal initialisation impossible.");
            }
            Thread.Sleep(5000);
            System.Console.ReadKey();

        }
    }
}
