using Alert.Api.Remote.Entities;
using BusinessToAlert;
using BusinessToDBAlert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initialisation
{
    public class InitialisationControler
    {
        #region Variables

        private readonly ManagerDB _managerDB;
        private readonly ManagerAlert _managerAlert;
        private readonly NLog.Logger _logger;
        private List<CallGroupDTO> callGroupDeletes = new List<CallGroupDTO>();
        private List<CallGroupDTO> callGroupNew = new List<CallGroupDTO>();
        private List<CallGroupDTO> callGroups;

        #endregion

        #region Constante

        #endregion

        #region Constructeur

        public InitialisationControler(ManagerDB managerDB, ManagerAlert managerAlert, NLog.Logger logger)
        {
            // Contrôle des paramètres rentrés
            if (managerDB is null)
            {
                throw new Exception("La variable manager DB est à une valeur null ");
            }
            if (managerAlert is null)
            {
                throw new Exception("La variable manager Alert est à une valeur null ");
            }
            if (logger is null)
            {
                throw new Exception("La variable logger DB est à une valeur null ");
            }

            // Assignation des valeurs pour le bon fonctionnement du controleur
            this._managerDB = managerDB;
            this._managerAlert = managerAlert;
            this._logger = logger;



        }
        #endregion

        #region Création des Call Group

        public void CreateCallGroup()
        {
            
            List<Secteur> secteurs = new List<Secteur>();
            List<Role> roles = new List<Role>();
            List<TypeAlarme> typeAlarmes = new List<TypeAlarme>();
            callGroupNew = new List<CallGroupDTO>();
            callGroupDeletes = new List<CallGroupDTO>();

            string searchCallGroupName;
            bool trouverCallGroup;
            bool trouverCallGroupDelete;
            bool premierPassageSecteur;

            //Récupération de la liste des call group dans alert
            callGroups = _managerAlert.GETCallGroup().Items.ToList();

            // Création de la liste des call group a supprimer.
            // Copie de la première liste 

            foreach (CallGroupDTO itemCallGroup in callGroups)
            {
                callGroupDeletes.Add(itemCallGroup);
            }

            //Récupération de tout les secteurs ;
            secteurs = _managerDB.GETSecteurs();

            //Récupération de tout les rôle ;

            roles = _managerDB.GETRoles();

            //Récupération de tout les type Alarmes
            typeAlarmes = _managerDB.GETTypeAlarmes();

            // Parcour les différent secteur rôle et typeAlarme afin de composer le call group et voir si il existe dans la alert

            foreach (Secteur secteurItem in secteurs)
            {
                premierPassageSecteur = false;
                if (secteurItem.ActifAlert == true)
                {

                    foreach (Role roleItem in roles)
                    {
                        if (roleItem.ActifAlert == true)
                        {
                            if (roleItem.NumeroGroupe == 1)
                            {
                                foreach (TypeAlarme typeAlarmeItem in typeAlarmes)
                                {
                                    if (!typeAlarmeItem.Description.Equals("INCONNU"))
                                    {
                                        searchCallGroupName = secteurItem.Abreger + "_" + roleItem.Nom + "_" + typeAlarmeItem.Description;

                                        // Recherche dans la liste des callGroups si le nom de call group est trouver
                                        trouverCallGroup = false;

                                        for (int i = 0; trouverCallGroup == false & i < callGroups.Count; i++)
                                        {
                                            if (callGroups[i].Name.Equals(searchCallGroupName))
                                            {
                                                trouverCallGroup = true;
                                                trouverCallGroupDelete = false;
                                                // Retirer des callGroup que l'on doit supprimer
                                                for (int j = 0; trouverCallGroupDelete == false & j < callGroupDeletes.Count; j++)
                                                {
                                                    if (callGroupDeletes[j].Id == callGroups[i].Id)
                                                    {
                                                        callGroupDeletes.RemoveAt(j);
                                                        trouverCallGroupDelete = true;
                                                        _logger.Debug("Suppression de la liste de suppression call group");
                                                        _logger.Debug("Call group Id" + callGroups[i].Id + " Nom : " + callGroups[i].Name );
                                                    }

                                                }
                                            }
                                        }
                                        // Le call group n'existe pas dans alert donc on initialise un callgroup et on l'ajoute dans la liste a ajouter
                                        if (trouverCallGroup == false)
                                        {
                                            AddNewCallGroupList( searchCallGroupName);
                                        }

                                    }
                                }

                            }
                            if (roleItem.NumeroGroupe == 2)
                            {
                                // premierPassageRole = true;

                                searchCallGroupName = secteurItem.Abreger + "_" + roleItem.Nom;

                                // Recherche dans la liste des callGroups si le nom de call group est trouver
                                trouverCallGroup = false;

                                for (int i = 0; trouverCallGroup == false & i < callGroups.Count; i++)
                                {
                                    if (callGroups[i].Name.Equals(searchCallGroupName))
                                    {
                                        trouverCallGroup = true;
                                        trouverCallGroupDelete = false;
                                        // Retirer des callGroup que l'on doit supprimer
                                        for (int j = 0; trouverCallGroupDelete == false & j < callGroupDeletes.Count; j++)
                                        {
                                            if (callGroupDeletes[j].Id == callGroups[i].Id)
                                            {
                                                callGroupDeletes.RemoveAt(j);
                                                trouverCallGroupDelete = true;
                                                _logger.Debug("Suppression de la liste de suppression call group");
                                                _logger.Debug("Call group Id" + callGroups[i].Id + " Nom : " + callGroups[i].Name);
                                            }

                                        }
                                    }

                                }

                                // Le call group n'existe pas dans alert donc on initialise un callgroup et on l'ajoute dans la liste a ajouter
                                if (trouverCallGroup == false)
                                {
                                    AddNewCallGroupList( searchCallGroupName);

                                }

                            }

                            if (roleItem.NumeroGroupe == 3)
                            {
                                searchCallGroupName = secteurItem.Abreger + "_" + roleItem.Nom;

                                // Recherche dans la liste des callGroups si le nom de call group est trouver
                                trouverCallGroup = false;

                                for (int i = 0; trouverCallGroup == false & i < callGroups.Count; i++)
                                {
                                    if (callGroups[i].Name.Equals(searchCallGroupName))
                                    {
                                        trouverCallGroup = true;
                                        trouverCallGroupDelete = false;
                                        // Retirer des callGroup que l'on doit supprimer
                                        for (int j = 0; trouverCallGroupDelete == false & j < callGroupDeletes.Count; j++)
                                        {
                                            if (callGroupDeletes[j].Id == callGroups[i].Id)
                                            {
                                                callGroupDeletes.RemoveAt(j);
                                                trouverCallGroupDelete = true;
                                                _logger.Debug("Suppression de la liste de suppression call group");
                                                _logger.Debug("Call group Id" + callGroups[i].Id + " Nom : " + callGroups[i].Name);
                                            }

                                        }
                                    }

                                }

                                // Le call group n'existe pas dans alert donc on initialise un callgroup et on l'ajoute dans la liste a ajouter
                                if (trouverCallGroup == false)
                                {
                                    AddNewCallGroupList( searchCallGroupName);

                                }

                            }
                        }
                    }

                }
            }

            // Pour le group 4
            foreach (Role roleItem in roles)
            {
                if (roleItem.NumeroGroupe == 4)
                {
                    searchCallGroupName = roleItem.Nom;

                    // Recherche dans la liste des callGroups si le nom de call group est trouver
                    trouverCallGroup = false;

                   // Recherche si le GroupByName est connu d'alert ou pas et vérifier si il faudrait faire des suppressions
                    trouverCallGroup = SearchCallGroupByName(searchCallGroupName);
                    // Le call group n'existe pas dans alert donc on initialise un callgroup et on l'ajoute dans la liste a ajouter
                    if (trouverCallGroup == false)
                    {
                        AddNewCallGroupList(searchCallGroupName);

                    }
                }
            }

            _managerAlert.ManagerCallGroups(callGroupNew, EnumHTMLVerbe.POST);
            _managerAlert.ManagerCallGroups(callGroupDeletes, EnumHTMLVerbe.DELETE);
          //  Console.ReadKey();


        }

        private bool SearchCallGroupByName(string searchCallGroupName)
        {
            bool trouverCallGroup = false;   
            
            for (int i = 0; trouverCallGroup == false & i < callGroups.Count; i++)
            {
                if (callGroups[i].Name.Equals(searchCallGroupName))
                {
                    trouverCallGroup = true;
                   
                    // Retirer des callGroup que l'on doit supprimer
                    SearchAddDeleteCallGroupList(i);
                }

            }
            return trouverCallGroup;
        }

        private void SearchAddDeleteCallGroupList(int i)
        {
            bool trouverCallGroupDelete = false;
            for (int j = 0; trouverCallGroupDelete == false & j < callGroupDeletes.Count; j++)
            {
                if (callGroupDeletes[j].Id == callGroups[i].Id)
                {
                    callGroupDeletes.RemoveAt(j);
                    trouverCallGroupDelete = true;
                    _logger.Debug("Suppression de la liste de suppression call group");
                    _logger.Debug("Call group Id" + callGroups[i].Id + " Nom : " + callGroups[i].Name);
                }

            }

            
        }

        private void AddNewCallGroupList(string searchCallGroupName)
        {
            CallGroupDTO callGroupDTO = new CallGroupDTO();
            callGroupDTO.Name = searchCallGroupName;

            callGroupNew.Add(callGroupDTO);
            _logger.Debug("Ajout du call group dans la liste a ajouter");
            _logger.Debug("Nom : " + callGroupDTO.Name);
        }

        #endregion
    }
}
