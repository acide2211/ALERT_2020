﻿using Alert.Api.Remote.Entities;
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

        private List<TeamDTO> teamDTOs;

        private List<TeamDTO> teamNewDTOs = new List<TeamDTO>();




        /// <summary>
        /// Liste des Roles qui sont dans la DB
        /// </summary>
        private List<Role> rolesDB = new List<Role>();

        private List<Personne> personnes = new List<Personne>();

        private List<Team> teamDB = new List<Team>();



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
                                            AddNewCallGroupList(searchCallGroupName);
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
                                    AddNewCallGroupList(searchCallGroupName);

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
                                    AddNewCallGroupList(searchCallGroupName);

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

        public void LiaisonCallGroup()
        {
            string callGroupName;
            string callGroupNameNext;
            string roleName;
            Role roleNext = null;
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

                    if (callGroupName.Contains(roleName))
                    {
                        numeroSequence = rolesDB[i].NumeroSequence;

                        // Une liaison doit être établie
                        if (numeroSequence != NOMBRESEQUENCEMAXROLE)
                        {
                            //Recherche du role ayant le numéro de sequence supérieur

                            roleNext = null;
                            for (int j = 0; roleNext == null & j < rolesDB.Count; j++)
                            {
                                if (rolesDB[j].NumeroSequence == numeroSequence + 1)
                                {
                                    roleNext = rolesDB[j];
                                }
                            }

                            int indexStartCallGroup = callGroupName.IndexOf(roleName);
                            callGroupNameNext = callGroupName.Substring(0, indexStartCallGroup) + roleNext.Nom;

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
                            if (trouverCallGroup == false)
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

                            if (callGroupItem.ReliefGroupId != callGroupNext.Id)
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

            foreach (Personne personneItem in personnes)
            {
                userTrouver = false;
                // Recherche si on trouver un user qui a le même nom et prenom
                for (int i = 0; userTrouver == false & i < users.Count; i++)
                {
                    user = users[i];
                    if (user.Name.Equals(personneItem.Nom) & user.FirstName.Equals(personneItem.Prenom))
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

                    if (user.Numbers == null || user.Numbers.Count == 0)
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


                }
                else
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


            _managerAlert.ManagerUser(usersNew, EnumHTMLVerbe.POST);
            _managerAlert.ManagerUser(usersUpdate, EnumHTMLVerbe.PUT);



        }

        #endregion

        #region Création des Teams

        #endregion
        public void CreateTeams()
        {
            callGroups = _managerAlert.GETCallGroup().Items.ToList();
            rolesDB = _managerDB.GETRoles();
            Role roleCurrent = null;

            teamNewDTOs = new List<TeamDTO>();

            string callGroupName;
            bool trouverRole = false;
            bool trouverTeam = false;

            // Parcour tous les call group pour trouver leur role

            foreach (CallGroupDTO callGroupItem in callGroups)
            {
                callGroupName = callGroupItem.Name;

                // Recherche dans quel groupe il est 
                trouverRole = false;

                for (int roleIndex = 0; trouverRole == false & roleIndex < rolesDB.Count; roleIndex++)
                {
                    if (callGroupName.Contains(rolesDB[roleIndex].Nom))
                    {
                        roleCurrent = rolesDB[roleIndex];
                        trouverRole = true;
                    }

                }

                if (trouverRole == false)
                {
                    throw new Exception("Impossible de trouver un nom de role dans le callGroup, vérifier la DB");
                }

                teamDB = _managerDB.GETTeamByRoleNames(roleCurrent.Nom);

                if (teamDB == null | teamDB.Count == 0)
                {
                    throw new Exception("Il n'y a pas de Team Lier au rôle veuilliez corriger la base de données");
                }

                // On récupérer les teams qui sont déjà dans le call group

                teamDTOs = _managerAlert.GETTeamByCallGroup(callGroupItem.Id).Items.ToList();


                foreach (Team teamDBItem in teamDB)
                {
                    trouverTeam = false;
                    // recherche si dans le call group les team exist déjà.



                    for (int teamDTOindex = 0; trouverTeam == false && teamDTOindex < teamDTOs.Count; teamDTOindex++)
                    {
                        if (teamDTOs[teamDTOindex].Name.Equals(teamDBItem.Nom))
                        {
                            trouverTeam = true;
                        }
                    }

                    if (trouverTeam == false)
                    {
                        TeamDTO teamDTO = new TeamDTO();

                        teamDTO.Name = teamDBItem.Nom;
                        teamDTO.Color = teamDBItem.ColorTeam;
                        teamDTO.CallGroupId = callGroupItem.Id;

                        teamNewDTOs.Add(teamDTO);

                    }
                }

            }

            // Ajout des teams dans la base de données
            _managerAlert.ManagerTeams(teamNewDTOs, EnumHTMLVerbe.POST);

        }
        public void LiaisonTeamPersonne()
        {
            bool trouverMember;
            bool trouverUser = false;


            string nameSecteur;

            List<TeamDTO> teamDTOs;

            List<MemberDTO> memberDTOs;
            List<MemberDTO> membersDelete = new List<MemberDTO>();
            List<MemberDTO> membersNew = new List<MemberDTO>();

            UserDTO userDTO = null;
            // Récuperation du dernier CallGroup

            callGroups = _managerAlert.GETCallGroup().Items.ToList();

            //Retrouver le Abreger du secteur

            nameSecteur = callGroups[0].Name.Substring(0, callGroups[0].Name.IndexOf('_'));

            //Recherche Secteur
            Secteur secteur = _managerDB.GETSecteurByAbreger(nameSecteur);

            // Rechrche pour un call group

            teamDTOs = _managerAlert.GETTeamByCallGroup(callGroups[0].Id).Items.ToList();

            //Recherche des members connu dans alert

            memberDTOs = _managerAlert.GETMemberByCallGroupId(callGroups[0].Id).Items.ToList();

            //Retrouver le Abreger du secteur

            nameSecteur = callGroups[0].Name.Substring(0, callGroups[0].Name.IndexOf('_'));

            //Recherche le RoleDB par TeamDB

            Role role = _managerDB.GETListRoleByTeamName(teamDTOs[0].Name);

            // Récuperation des personnes par la prioriter qui sont dans le bon role et le bon secteur

            List<Prioriter> prioriters = _managerDB.GETListPrioriterByRoleAndSecteur(role.Id, secteur.Id);

            // Recherche des membres a delete du call group

            foreach (MemberDTO itemMember in memberDTOs)
            {
                //Retrouver l'opérateur derrière le membre
                try
                {
                    userDTO = _managerAlert.GETUserById(itemMember.Id);

                    trouverMember = false;

                    for (int i = 0; trouverMember == false && i < prioriters.Count; i++)
                    {
                        if (userDTO.Name == prioriters[i].Personne.Nom && userDTO.FirstName == prioriters[i].Personne.Nom)
                        {
                            trouverMember = true;
                        }
                    }

                    if (trouverMember == false)
                    {
                        membersDelete.Add(itemMember);
                    }
                }
                catch (Exception)
                {


                }

            }

            // Recherche des membres à ajouter
            users = _managerAlert.GETUsers().Items.ToList();
            foreach (Prioriter itemPropriter in prioriters)
            {
                trouverUser = false;

                for (int i = 0; trouverUser == false && i < users.Count; i++)
                {
                    try
                    {
                        userDTO = users[i];

                        if (itemPropriter.Personne.Nom == userDTO.Name && itemPropriter.Personne.Prenom == userDTO.FirstName)
                        {
                            trouverUser = true;
                            MemberDTO memberNew = new MemberDTO();
                            memberNew.Id = userDTO.Id;
                            membersNew.Add(memberNew);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            if (trouverUser == false)
            {
                _logger.Error("Aucun user trouver");
            }

            teamDTOs[0].Members = membersNew;

            _managerAlert.ManagerMemberByCallGroup(membersDelete, callGroups[0], EnumHTMLVerbe.DELETE);

            _managerAlert.ManagerTeams(teamDTOs, EnumHTMLVerbe.PUT);




        }



       public void LiaisonMemberToTeam()
        {
            callGroups = _managerAlert.GETCallGroup().Items.ToList();
            //LiaisonMemberToTeamCallGroup(callGroups[0]);
            foreach (CallGroupDTO itemCallGroup in callGroups)
            {
                Console.WriteLine("Debut :" + itemCallGroup.Name);


                //Debug
                if (itemCallGroup.Name.Equals("DIRECTION") || itemCallGroup.Name.Equals("NON-AVERTI"))
                {
                    Console.WriteLine("Debug :" + itemCallGroup.Name);
                    LiaisonMemberToTeamCallGroup(itemCallGroup);
                }
                else
                {
                    LiaisonMemberToTeamCallGroup(itemCallGroup);
                }

                Console.WriteLine("FIN : " + itemCallGroup.Name);
            }
        }

        public void LiaisonMemberToTeamCallGroup(CallGroupDTO callGroup)
        {
            List<TeamDTO> teamDTOs;
            List<MemberDTO> membersDelete = new List<MemberDTO>();
            List<MemberDTO> membersNew = new List<MemberDTO>();
            string teamDTOName;


            users = _managerAlert.GETUsers().Items.ToList();
            string nameSecteur = null;
            //Retrouver Abreger du secteur
            try
            {
                nameSecteur = callGroup.Name.Substring(0, callGroup.Name.IndexOf('_'));
            }
            catch (Exception ex)
            {
                if (callGroup.Name.IndexOf('_') == -1)
                {
                    nameSecteur = callGroup.Name;
                    nameSecteur = "PC";

                }
            }

            //Recherche Secteur
            Secteur secteur = _managerDB.GETSecteurByAbreger(nameSecteur);

            //Recherche des Teams du call group
            teamDTOs = _managerAlert.GETTeamByCallGroup(callGroup.Id).Items.ToList();


            for (int indexTeamDTO = 0; indexTeamDTO < teamDTOs.Count; indexTeamDTO++)
            {
               // membersNew = new List<MemberDTO>();
                membersDelete = new List<MemberDTO>();

                //Recherche le RoleDB par TeamDB

                Role role = _managerDB.GETListRoleByTeamName(teamDTOs[indexTeamDTO].Name);

                // Récuperation des personnes par la prioriter qui sont dans le bon role et le bon secteur


                List<Prioriter> prioriters = _managerDB.GETListPrioriterByRoleSecteurTeamId(role.Id, secteur.Id, teamDTOs[indexTeamDTO].Name);
                // Permet de supprimer les membre de la team
                teamDTOs[indexTeamDTO].Members = new List<MemberDTO>();
                _managerAlert.ManagerTeams(teamDTOs, EnumHTMLVerbe.PUT);

                //Mise en place des membres dans la team par rapport à la Prioriter

                // On regarde dans la table TeamDB si le spo doit être un membre
                teamDTOName = teamDTOs[indexTeamDTO].Name;

                //Recherche les Informations de la TeamDB
                Team teamDB = _managerDB.GETTeamByTeamNames(teamDTOName);

                //Permet de mettre le spo en premier si il y a besoin
                if (teamDB.ActifSPOPosition == 1)
                {
                    // Rechercher l'utilisateur SPO

                    membersNew.Add(searchMemberDTOByUserNameAndFirstName(secteur.Abreger, "SPO", users));

                }

                prioriters = prioriters.OrderBy(prioriter => prioriter.Prioriter1).ToList();

                foreach (Prioriter itemPropriter in prioriters)
                {
                    membersNew.Add(searchMemberDTOByUserNameAndFirstName(itemPropriter.Personne.Nom, itemPropriter.Personne.Prenom, users));

                    //Permet de mettre les personnes en repli
                    if (teamDB.ActifSPOPosition == 1 || itemPropriter.Prioriter1 != 1)
                    {
                        membersNew.Last().Relief = true;

                    }


                }


                //Permet de mettre le spo en dernier si il y a besoin
                if (teamDB.ActifSPOPosition == 2)
                {
                    // Rechercher l'utilisateur SPO

                    membersNew.Add(searchMemberDTOByUserNameAndFirstName(secteur.Abreger, "SPO", users));
                    if (membersNew.Count > 1)
                    {
                        membersNew.Last().Relief = true;

                    }

                }
                teamDTOs[indexTeamDTO].Members = membersNew;
                _managerAlert.ManagerTeams(teamDTOs, EnumHTMLVerbe.PUT);


            }


        }

        public MemberDTO searchMemberDTOByUserNameAndFirstName(string name, string firstName, List<UserDTO> users = null)
        {
            // Si on ne passe pas la liste des users ou que celle-ci est vide alors on va la récupérer
            if (users == null || users.Count == 0)
            {
                users = _managerAlert.GETUsers().Items.ToList();

                if (users.Count == 0)
                {
                    throw new Exception("Impossible de chercher un user dans la liste d'alert car celle-ci est vide");
                }
            }

            bool trouverUser = false;
            UserDTO userDTO = null;

            for (int i = 0; trouverUser == false && i < users.Count; i++)
            {

                userDTO = users[i];

                if (name == userDTO.Name && firstName == userDTO.FirstName)
                {
                    trouverUser = true;
                    MemberDTO memberNew = new MemberDTO();
                    memberNew.Id = userDTO.Id;
                    return memberNew;
                }

            }

            if (trouverUser == false)
            {
                throw new Exception("Impossible de trouver un user dans alert qui porte le nom et prenom suivant veuilliez controller qu'il existe" + name + " " + firstName);
            }

            return null;

        }
    }
}
