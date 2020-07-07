using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessToDBAlert
{
    /***Classe qui permet de faire la gestion de la base de données
     * 
     */

    public class ManagerDB
    {
        #region Déclaration des variables 
        //Variable qui permet de faire la connexions a la base de données
        private DataClassesALERTDataContext _alertDBContext;
        //dictionaire qui contient les informations de configuration
        public Dictionary<String, String> _configurations { get; private set; }

        private NLog.Logger Logger;
        #endregion

        public ManagerDB()
        {
            // Creation de la connexions avec la base de données

            _alertDBContext = new DataClassesALERTDataContext();

      

            //Récupération de la configuration

            GetConfigurationInfo();

         

        }
        public ManagerDB(NLog.Logger Logger)
        {
            // Synchronisation des système de log
            this.Logger = Logger;

            // Creation de la connexions avec la base de données

            _alertDBContext = new DataClassesALERTDataContext();

            Logger.Info("Connexions a la base de données");

            //Récupération de la configuration

            GetConfigurationInfo();

            Logger.Info("Récupération des informations de configuration");
           

            
            //try
            //{
            //    Logger.Info("Base de données");
            //    System.Console.ReadKey();
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex, "Goodbye cruel world");
            //}
        }

        private void GetConfigurationInfo()
        {
            Table<Configuration> configuration = _alertDBContext.GetTable<Configuration>();

            _configurations = (from config in configuration
                              select config).ToDictionary(t => t.Cle, t => t.Value);
        }

    }
}
