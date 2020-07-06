using System;
using System.Collections.Generic;
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
        private DataClassesALERTDataContext _AlertDBContext;
        //dictionaire qui contient les informations de configuration
        private Dictionary<String, String> _private;
        #endregion

        public AlertManager()
        {
        }

    }
}
