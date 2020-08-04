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
        /// <summary>
        /// Attribut qui permet de faire les interaction avec la base de donnée
        /// </summary>
        private readonly ManagerDB _managerDB;
        /// <summary>
        /// Attribut qui permet de faire les interaction avec le programme ALERT
        /// </summary>
        private readonly ManagerAlert _managerAlert;
        /// <summary>
        /// Attribut qui permet de faire les log
        /// </summary>
        private readonly NLog.Logger _logger;


        /// <summary>
        /// Liste des CallGroup qui devrons être supprimer d'alert
        /// </summary>
        private List<CallGroupDTO> callGroupDeletes = new List<CallGroupDTO>(); 
        /// <summary>
        /// Liste des CallGroup qui devrons être ajouter dans alert
        /// </summary>
        private List<CallGroupDTO> callGroupNew = new List<CallGroupDTO>();
        /// <summary>
        /// Liste des CallGroup qui devrons être ajouter dans alert
        /// </summary>
        private List<CallGroupDTO> callGroupUpdate = new List<CallGroupDTO>();
        /// <summary>
        /// Liste des Call Groups qui sont connu dans alert.
        /// </summary>
        private List<CallGroupDTO> callGroups;

        /// <summary>
        /// Liste des User qui sont connu dans alert.
        /// </summary>
        private List<UserDTO> users;

        private List<UserDTO> usersNew = new List<UserDTO>();

        private List<UserDTO> usersUpdate = new List<UserDTO>();

        /// <summary>
        /// Liste des Roles qui sont dans la DB
        /// </summary>
        private List<Role> rolesDB = new List<Role>();

        private List<Personne> personnes = new List<Personne>();

        #endregion

        #region Constante

        private readonly int NOMBRESEQUENCEMAXROLE; 

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

            this.NOMBRESEQUENCEMAXROLE = _managerDB.GETNombreSequenceMaxRole();


        }
        #endregion

        #region Création des Call Group

        /// <summary>
        /// Méthode qui permet de rechrche dans la liste des CallGroup qui vienne d'alert si 
        /// Le Call Group existe déja alors il est retirer de la liste des CallGroup à Supprimer
        /// Si il n'est pas dans la liste alors on l'ajoutera dans la liste des CallGroup à Créee
        /// </summary>
        /// <param name="searchCallGroupName"></param>
        /// <returns></returns>
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

                                        trouverCallGroup = SearchCallGroupByName(searchCallGroupName);

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

                                trouverCallGroup = SearchCallGroupByName(searchCallGroupName);

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

                                trouverCallGroup = SearchCallGroupByName(searchCallGroupName);

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

        #region Méthode Outils Call Group

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

        /// <summary>
        /// Méthode qui permet de chercher dans la liste de call Group qui est dans ALERT
        /// Si un autre CallGroup porte se nom ou pas 
        /// </summary>
        /// <param name="i">indexe de possition dans   la liste CallGroups</param>
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

        /// <summary>
        /// Fonction qui permet de crée un Call Group avec comme nom le passez en paramètre
        /// et ajout dans une liste d'ajout des callGroups
        /// </summary>
        /// <param name="searchCallGroupName"> Nom du call group </param>
        private void AddNewCallGroupList(string searchCallGroupName)
        {
            CallGroupDTO callGroupDTO = new CallGroupDTO();
            callGroupDTO.Name = searchCallGroupName;

            callGroupNew.Add(callGroupDTO);
            _logger.Debug("Ajout du call group dans la liste a ajouter");
            _logger.Debug("Nom : " + callGroupDTO.Name);
        }

        #endregion

        #endregion

        #region Liaison des Call Group

        public void LiaisonCallGroup ()
        {
            string callGroupName;
            string callGroupNameNext;
            string roleName;
            Role roleNext= null;
            CallGroupDTO callGroupNext = null;
            int? numeroSequence;
            //Remise à zero de la liste des updates 
            callGroupUpdate = new List<CallGroupDTO>();

            // Récupération de la liste des CallGroup
            callGroups = _managerAlert.GETCallGroup().Items.ToList();

            //Récupération de la liste des Roles depuis la DB pour connaitre la séquence
            rolesDB = _managerDB.GETRoles();

            // Pacours de la CallList
            foreach (CallGroupDTO callGroupItem in callGroups)
            {
                _logger.Debug("LiaisonCallGroup : " + callGroupItem.Name);

                callGroupNext = null;
                callGroupName = callGroupItem.Name;

                // Parcour de la liste des Role pour connaitre la séquence qu'il à
                for (int i = 0; i < rolesDB.Count; i++)
                {
                    roleName = rolesDB[i].Nom;

                    if(callGroupName.Contains(roleName))
                    {
                        numeroSequence = rolesDB[i].NumeroSequence;

                        // Une liaison doit être établie
                        if(numeroSequence != NOMBRESEQUENCEMAXROLE)
                        {
                            //Recherche du role ayant le numéro de sequence supérieur

                            roleNext = null;
                            for (int j = 0; roleNext == null & j < rolesDB.Count; j++)
                            {
                                if ( rolesDB[j].NumeroSequence == numeroSequence +1)
                                {
                                    roleNext = rolesDB[j];
                                }
                            }

                            int indexStartCallGroup = callGroupName.IndexOf(roleName);
                            callGroupNameNext = callGroupName.Substring(0,indexStartCallGroup) + roleNext.Nom;

                            //Recherche du nouveau callGroup

                            bool trouverCallGroup = false;
                            // Recherche Le groupe suivant avec le nom correct
                            for (int indexCallGroup = 0; trouverCallGroup == false & indexCallGroup < callGroups.Count; indexCallGroup++)
                            {
                                if (callGroups[indexCallGroup].Name.Equals(callGroupNameNext))
                                {
                                    callGroupNext = callGroups[indexCallGroup];
                                    trouverCallGroup = true;
                                }
                            }
                            // Si on a pas trouver avec le callGroup et Nom Role on cherche juste par le role Name
                            if(trouverCallGroup == false)
                            {
                                for (int indexCallGroup = 0; trouverCallGroup == false & indexCallGroup < callGroups.Count; indexCallGroup++)
                                {
                                    if (callGroups[indexCallGroup].Name.Equals(roleNext.Nom))
                                    {
                                        callGroupNext = callGroups[indexCallGroup];
                                        trouverCallGroup = true;
                                    }
                                }
                            }

                            if(callGroupItem.ReliefGroupId != callGroupNext.Id)
                            {
                                callGroupItem.ReliefGroupId = callGroupNext.Id;
                                callGroupUpdate.Add(callGroupItem);
                            }                           
                        }
                        
                    }
                }


            }

            // Modification des CallGroup a Update
            
            _managerAlert.ManagerCallGroups(callGroupUpdate, EnumHTMLVerbe.PUT);

        }

        #region Outils Pour Liaison des Call Group



        #endregion
        #endregion

        #region Création des Utilisateurs

        public void CreateUsers()
        {

            UserDTO user = null;
            bool userTrouver;
            // Récupération des User qui son dans alert

            users = _managerAlert.GETUsers().Items.ToList();

            // Récupération des Personne qui sont dans la DB

            personnes = _managerDB.GETPersonnes();

            foreach(Personne personneItem in personnes)
            {
                userTrouver = false;
                // Recherche si on trouver un user qui a le même nom et prenom
                for(int i = 0; userTrouver == false & i < users.Count; i++)
                {
                    user = users[i];
                    if(user.Name.Equals(personneItem.Nom) & user.FirstName.Equals(personneItem.Prenom))
                    {
                        userTrouver = true;
                    }

                }

                // Controle si on a trouver user
                if (userTrouver == false)
                {
                    // Création d'un user depuis une personne;
                    user = new UserDTO();
                    usersNew.Add(user);

                    user.Name = personneItem.Nom;
                    user.FirstName = personneItem.Prenom;

                    // Mettre la personne en actif
                    user.StatusId = 1;

                    if(user.Numbers == null || user.Numbers.Count == 0)
                    {
                        user.Numbers = new List<NumberDTO>();

                        foreach (DriverConfig personneDriverConfig in personneItem.DriverConfig)
                        {
                            NumberDTO number = new NumberDTO();
                            number.AckAutoDefault = Convert.ToInt16(personneDriverConfig.AutoAck);
                            number.Address = personneDriverConfig.Adresse;
                            number.CountAck = Convert.ToInt16(personneDriverConfig.CountAck);
                            number.CountTry = Convert.ToInt16(personneDriverConfig.CountTry);
                            number.DriverId = Convert.ToUInt32(personneDriverConfig.TypeDriver.Numero);
                            number.DriverName = personneDriverConfig.TypeDriver.Name;
                            number.Final = personneDriverConfig.Final;
                            number.Id = Convert.ToUInt32(personneDriverConfig.Prioriter);
                            number.TimeToAck = Convert.ToInt16(personneDriverConfig.TimeToAck);
                            number.TimeToRetry = Convert.ToInt16(personneDriverConfig.TimeToRetry);
                            number.Valid = personneDriverConfig.Valide;
                            user.Numbers.Add(number);
                        }

                    }
                    else
                    {
                        if (user.Numbers.Count != personneItem.DriverConfig.Count)
                        {
                            List<NumberDTO> NumberDTODelete = new List<NumberDTO>();
                            foreach (NumberDTO userDTONumberItem in user.Numbers)
                            {

                                bool trouver = false;
                                int i = 0;
                                while (i < personneItem.DriverConfig.Count && trouver == false)
                                {

                                    if (userDTONumberItem.DriverId == personneItem.DriverConfig[i].TypeDriver.Numero)
                                    {
                                        trouver = true; ;
                                    }
                                    i++;
                                }

                                if (trouver == false)
                                {
                                    NumberDTODelete.Add(userDTONumberItem);
                                }
                            }

                            // Suppression des élémement de trop

                            foreach (NumberDTO numberDTOItem in NumberDTODelete)
                            {
                                user.Numbers.Remove(numberDTOItem);
                            }

                        }
                    }


                }else
                {
                    // L'user existe dans alert et dans la base de données on va controler les driver info si ils sont correct
                    user.Numbers = new List<NumberDTO>();

                    foreach (DriverConfig personneDriverConfig in personneItem.DriverConfig)
                    {
                        NumberDTO number = new NumberDTO();
                        number.AckAutoDefault = Convert.ToInt16(personneDriverConfig.AutoAck);
                        number.Address = personneDriverConfig.Adresse;
                        number.CountAck = Convert.ToInt16(personneDriverConfig.CountAck);
                        number.CountTry = Convert.ToInt16(personneDriverConfig.CountTry);
                        number.DriverId = Convert.ToUInt32(personneDriverConfig.TypeDriver.Numero);
                        number.DriverName = personneDriverConfig.TypeDriver.Name;
                        number.Final = personneDriverConfig.Final;
                        number.Id = Convert.ToUInt32(personneDriverConfig.Prioriter);
                        number.TimeToAck = Convert.ToInt16(personneDriverConfig.TimeToAck);
                        number.TimeToRetry = Convert.ToInt16(personneDriverConfig.TimeToRetry);
                        number.Valid = personneDriverConfig.Valide;
                        user.Numbers.Add(number);
                    }
                    usersUpdate.Add(user);

                }
            }


            _managerAlert.ManagerUser(usersNew,EnumHTMLVerbe.POST);
            _managerAlert.ManagerUser(usersUpdate, EnumHTMLVerbe.PUT);



        }

        #endregion

    }
}
