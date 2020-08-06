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

    public sealed class ManagerDB
    {
        #region Déclaration des variables 
        //Varible du singleton
        private static ManagerDB _instance = null;
        //Permet d'éciter d'avoir deux instance
        private static readonly object _padlock = new object();
        //Variable qui permet de faire la connexions a la base de données
        private DataClassesALERTDataContext _alertDBContext;
        //dictionaire qui contient les informations de configuration
        public Dictionary<String, String> _configurations { get; private set; }

        private NLog.Logger Logger { get; set; }
        #endregion

        #region Constructeur
        /// <summary>
        /// Fonction qui permet de récupérer l'instance de ManagerDB
        /// </summary>
        /// <param name="Logger"></param>
        /// <returns></returns>
        public static ManagerDB getInstance(NLog.Logger Logger = null)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    if (Logger != null)
                    {
                        // Synchronisation des système de log                       
                        _instance = new ManagerDB(Logger);
                    }
                    else
                    {
                        throw new Exception("Le Logger n'est pas initialiser");
                    }
                    
                }
                return _instance;
            }
        }

        private ManagerDB(NLog.Logger Logger)
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

        #endregion

        #region Configuration
        private void GetConfigurationInfo()
        {
            Table<Configuration> configuration = _alertDBContext.GetTable<Configuration>();

            _configurations = (from config in configuration
                               select config).ToDictionary(t => t.Cle, t => t.Value);
        }

        public int GETNombreSequenceMaxRole()
        {
            string value = "";
            int nombreSequenceMaxrole;
            if (_configurations.TryGetValue("NOMBRESEQUENCEMAXROLE", out value))
            {
                // Convertion de la clé qui est en string en int 
                try
                {
                    nombreSequenceMaxrole = Int32.Parse(value);
                }catch(Exception e)
                {
                    throw new Exception("La cle NOMBRESEQUENCEMAXROLE n'est pas un nombre");
                }
               
                return nombreSequenceMaxrole;
            }
            else
            {
                throw new Exception("La cle NOMBRESEQUENCEMAXROLE n'existe pas dans la base de données");
            }


        }

        #endregion

        #region Gestion Secteur
        /// <summary>
        /// Permet de récupérer la lise de tout les secteurs 
        /// </summary>
        /// <returns>List des secteurs</returns>
        public List<Secteur> GETSecteurs()
        {
            Table<Secteur> tableSecteur = _alertDBContext.GetTable<Secteur>();
            List<Secteur> secteurs = (from secteur in tableSecteur select secteur).ToList();
            return secteurs;
        }
        /// <summary>
        /// Permet de trouver un secteur particulier
        /// </summary>
        /// <param name="nomSecteur"></param>
        /// <returns>Retourne le secteur qui porte le nom ou retourne null si pas trouver</returns>
        public Secteur GETSecteurByName(string nomSecteur)
        {
            nomSecteur = nomSecteur.ToUpper();

            Table<Secteur> secteurTable = _alertDBContext.GetTable<Secteur>();
            List<Secteur> secteurTrouvers = (from secteur in secteurTable
                                             where secteur.Nom.Equals(nomSecteur)
                                             select secteur).ToList();
            if (secteurTrouvers.Count != 1)
            {
                Logger.Info("Recherche d'un secteur qui n'est pas connu " + nomSecteur);
                return null;
            }
            return secteurTrouvers.First(); ;
        }
        /// <summary>
        /// Permet de récupérer la liste des secteurs de manière asychrone 
        /// </summary>
        /// <returns>Retourne la liste des secteurs </returns>
        public async Task<List<Secteur>> GETListSecteursAsync()
        {
            List<Secteur> Secteurs = this.GETSecteurs();
            return Secteurs;
        }

        #endregion

        #region Gestion Role
        /// <summary>
        /// Permet de récupérer la lise de tout les rôles 
        /// </summary>
        /// <returns>List des rôles</returns>
        public List<Role> GETRoles()
        {
            Table<Role> tableRole = _alertDBContext.GetTable<Role>();
            List<Role> roles = (from role in tableRole select role).ToList();
            return roles;
        }
        /// <summary>
        /// Permet de trouver un secteur particulier
        /// </summary>
        /// <param name="nomSecteur"></param>
        /// <returns>Retourne le secteur qui porte le nom ou retourne null si pas trouver</returns>
        public Role GETRoleByName(string nomRole)
        {
            nomRole = nomRole.ToUpper();

            Table<Role> roleTable = _alertDBContext.GetTable<Role>();

            List<Role> roleTrouvers = (from role in roleTable
                                             where role.Nom  == nomRole
                                             select role).ToList();
            if (roleTrouvers.Count != 1)
            {
                Logger.Info("Recherche d'un role qui n'est pas connu " + nomRole);
                return null;
            }
            return roleTrouvers.First(); ;
        }
        /// <summary>
        /// Permet de récupérer la liste des secteurs de manière asychrone 
        /// </summary>
        /// <returns>Retourne la liste des secteurs </returns>
        public async Task<List<Role>> GETListRolesAsync()
        {
            List<Role> Roles = this.GETRoles();
            return Roles;
        }

        #endregion

        #region Gestion Type d'alarme
        /// <summary>
        /// Permet de récupérer la lise de tout les types d'alarme
        /// </summary>
        /// <returns>List des Type d'alarme </returns>
        public List<TypeAlarme> GETTypeAlarmes ()
        {
            Table<TypeAlarme> tableTypeAlarme = _alertDBContext.GetTable<TypeAlarme>();
            List<TypeAlarme> typeAlarmes = (from role in tableTypeAlarme select role).ToList();
            return typeAlarmes;
        }

        #endregion


        #region Personne
        public List<Personne> GETPersonnes()
        {
            Table<Personne> tablePersonne = _alertDBContext.GetTable<Personne>();
            List<Personne> personnes = (from personne in tablePersonne select personne).ToList();
            return personnes;
        }

        public async Task<List<Personne>> GETListPersonnesAsync()
        {
            List<Personne> Personnes = this.GETPersonnes();
            return Personnes;
        }
        #endregion

        #region Team

        public List<Team> GETTeams()
        {
            Table<Team> tableTeam = _alertDBContext.GetTable<Team>();
            List<Team> teams = (from team in tableTeam select team).ToList();
            return teams;
        }
        /// <summary>
        /// Permet de trouver un secteur particulier
        /// </summary>
        /// <param name="nomRole"></param>
        /// <returns>Retourne le secteur qui porte le nom ou retourne null si pas trouver</returns>
        public List<Team> GETTeamByRoleNames(string nomRole)
        {
            nomRole = nomRole.ToUpper();

            Table<Team> teamTable = _alertDBContext.GetTable<Team>();
            List<Team> teamTrouvers = (from team in teamTable
                                       where team.Role.Nom.Equals(nomRole)
                                             select team).ToList();

            return teamTrouvers; 
        }

        #endregion


    }
}
