using BusinessToDBAlert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Création_Planning
{
    public class Utile
    {
        /// <summary>
        /// Attribut qui permet de faire les interaction avec la base de donnée
        /// </summary>
        private readonly ManagerDB _managerDB;

        /// <summary>
        /// Attribut qui permet de faire les log
        /// </summary>
        private readonly NLog.Logger _logger;
        public Utile(ManagerDB managerDB,  NLog.Logger logger)
        {

        }
    }
}
