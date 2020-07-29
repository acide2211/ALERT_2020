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

        #endregion

        #region Constante

        #endregion

        #region Constructeur

        public InitialisationControler(ManagerDB managerDB, ManagerAlert managerAlert, NLog.Logger logger)
        {
            // Contrôle des paramètres rentrés
            if (managerDB is null)
            {
                throw new Exception("La variable manager DB est à une valeur null ") ;
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
            List<uint?> callGroupIdDelete = new List<uint?>();
            List<Secteur> secteurs = new List<Secteur>();
            List<Role> roles = new List<Role>();
            List<TypeAlarme> typeAlarmes = new List<TypeAlarme>();

            //Récupération de la liste des call group dans alert
            List<CallGroupDTO> callGroups = _managerAlert.GETCallGroup().Items.ToList(); 

            // Création de la liste des call group a supprimer.
            // Copie de la première liste 

            foreach(CallGroupDTO itemCallGroup in callGroups)
            {
                callGroupIdDelete.Add(itemCallGroup.Id);
            }

            //Récupération de tout les secteurs ;
            secteurs = _managerDB.GETSecteurs();

            //Récupération de tout les rôle ;

            roles = _managerDB.GETRoles();

            //Récupération de tout les type Alarmes
            typeAlarmes = _managerDB.GETTypeAlarmes();

            // Parcour les différent secteur rôle et typeAlarme afin de composer le call group et voir si il existe dans la alert
            foreach(Secteur secteurItem in secteurs)
            {
                foreach (Role roleItem in roles)
                {
                    foreach (TypeAlarme typeAlarmeItem in typeAlarmes)
                    {

                    }
                }
            }


        }

        #endregion
    }
}
