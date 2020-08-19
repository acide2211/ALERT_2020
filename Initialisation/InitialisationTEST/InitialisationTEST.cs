using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessToAlert;
using BusinessToDBAlert;
using Initialisation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InitialisationTEST
{

    [TestClass]
    public class InitialisationTEST
    {
        //Variable qui permet de crée les logs
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        ManagerDB managerDB;
        ManagerAlert managerAlert;
        InitialisationControler initialisationControler;

        public InitialisationTEST()
        {
            managerDB = ManagerDB.getInstance(Logger);
            managerAlert = ManagerAlert.getInstance(Logger);
            initialisationControler = new InitialisationControler(managerDB, managerAlert, Logger);
            initialisationForTeam();
        }

      
        public void initialisationForTeam()
        {
            

            // Connexions a Alert pour les Appelles API
            managerAlert.LoginAlertWS();

            // Création des calls groups
            Logger.Info("TEST INITIALISATION  : DEBUT : Creation des call group terminer");
            initialisationControler.CreateCallGroup();
            Logger.Info("TEST INITIALISATION  : FIN : Creation des call group terminer");

            // Liaison entre les call groupe
            Logger.Info("TEST INITIALISATION  :DEBUT : Creation des liaison entre les call group");
            initialisationControler.LiaisonCallGroup();
            Logger.Info("TEST INITIALISATION  : FIN : Creation des liaison entre les call group");

            // Création des utilisateurs dans alert

            Logger.Info("TEST INITIALISATION  : DEBUT : Creation des utilisateurs");
            initialisationControler.CreateUsers();
            Logger.Info("TEST INITIALISATION  : FIN : Creation et update des utilisateurs");

            // Création des TEAM
            Logger.Info("TEST INITIALISATION  : DEBUT : Creation des teams");
            initialisationControler.CreateTeams();
            Logger.Info("TEST INITIALISATION  : FIN :  Creation des teams");

            // USER dans TEAM
            Logger.Info("DEBUT : Creation des user team");
            // initialisationControler.LiaisonTeamPersonne();
        }

        /// <summary>
        /// Méthode de test qui permet de vérifier la liaison des Membre To Team et voir l'optimisation
        /// </summary>

        [TestMethod]
        
        public void testLiaisonMembreToTeam ()
        {
            initialisationControler.LiaisonMemberToTeam();
        }
        
       
    }
}
