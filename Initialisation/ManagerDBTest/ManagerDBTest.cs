using System;
using System.Collections.Generic;
using BusinessToDBAlert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagerDBTest
{
    [TestClass]
    public class ManagerDBTest
    {
        
        //Variable qui permet de crée les logs
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private ManagerDB _managerDB;

        public ManagerDBTest()
        {
            _managerDB = ManagerDB.getInstance(Logger);
        }

        [TestMethod]
        public void TestGetSecteurs()
        {
            List<Secteur> secteurs = _managerDB.GetSecteurs();
            Assert.IsNotNull(secteurs);
        }
        [TestMethod]
        public void TestGetSecteursByName()
        {
            string name = "OUVEA";
            Secteur secteur = _managerDB.GetSecteurByName(name);
            Assert.IsNotNull(secteur);

            name = "plateau de herves";
            secteur = _managerDB.GetSecteurByName(name);
            Assert.IsNull(secteur);
        }

        [TestMethod]
        public void TestGetRoles()
        {
            List<Role> roles = _managerDB.GetRoles();
            Assert.IsNotNull(roles);
        }

        [TestMethod]       
        public void TestGetRoleByName()
        {
            string name;

            name = "ContreMaitre";
            Role role = _managerDB.GetRoleByName(name);
            Assert.IsNotNull(role);
        }
    }
}
