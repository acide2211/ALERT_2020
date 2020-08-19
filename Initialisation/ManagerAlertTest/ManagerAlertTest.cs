using System;
using System.Collections.Generic;

using BusinessToAlert;
using BusinessToDBAlert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ManagerAlertTest
{
    [TestClass]
    public class ManagerAlertTest
    {
        //Variable qui permet de crée les logs
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private ManagerAlert _managerAlert;

        public ManagerAlertTest()
        {
            _managerAlert = ManagerAlert.getInstance(Logger);
        }

        [TestMethod]
        public void TestGetCallGroupDTO()
        {
            _managerAlert.LoginAlertWS();
            // Récupération des callGroup dans alert
            PagingCollectionDTO<CallGroupDTO> pagingCollectionDTOs = _managerAlert.GETCallGroup();
            List<CallGroupDTO> callgroups = pagingCollectionDTOs.Items.ToList();

            Assert.IsNotNull(callgroups);
            Assert.AreNotEqual(callgroups.Count, 0);

        }

        /// <summary>
        /// Methode de test qui permet de vérifier si on arrive bien a crée un CallGroup
        /// </summary>
        [TestMethod]
        public void CreationCallGroup()
        {
            PagingCollectionDTO<CallGroupDTO> pagingCollectionDTOs;
            int nombreCallGroupAvant, nombreCallGroupApres;
            CallGroupDTO createcallGroup = new CallGroupDTO(); ;
            List<CallGroupDTO> callGroups = new List<CallGroupDTO>();

            _managerAlert.LoginAlertWS();

            pagingCollectionDTOs = _managerAlert.GETCallGroup();
            nombreCallGroupAvant = pagingCollectionDTOs.Items.ToList().Count;

            createcallGroup = new CallGroupDTO();
            createcallGroup.Id = null;
            createcallGroup.Name = "Nouveau";

            callGroups.Add(createcallGroup);

            _managerAlert.ManagerCallGroups(callGroups, EnumHTMLVerbe.POST);

            pagingCollectionDTOs = _managerAlert.GETCallGroup();
            nombreCallGroupApres = pagingCollectionDTOs.Items.ToList().Count;
           
            Assert.AreNotEqual(nombreCallGroupAvant, nombreCallGroupApres);

        }

        /// <summary>
        /// Methode de test qui permet de vérifier la suppression du dernier call group qui est dans alert
        /// </summary>
        [TestMethod]
        public void DeleteCallGroup()
        {
            PagingCollectionDTO<CallGroupDTO> pagingCollectionDTOs;
            int nombreCallGroupAvant, nombreCallGroupApres;
            CallGroupDTO deleteCallGroup = new CallGroupDTO(); ;
            List<CallGroupDTO> callGroups = new List<CallGroupDTO>();

            _managerAlert.LoginAlertWS();

            pagingCollectionDTOs = _managerAlert.GETCallGroup();
            callGroups = pagingCollectionDTOs.Items.ToList();
            nombreCallGroupAvant = callGroups.Count;

            if(nombreCallGroupAvant == 0)
            {
                Assert.Fail("Il n'y a pas de callgroup dans alert");
            }

            deleteCallGroup = callGroups.Last();
           
            callGroups.Add(deleteCallGroup);

            _managerAlert.ManagerCallGroups(callGroups, EnumHTMLVerbe.DELETE);

            pagingCollectionDTOs = _managerAlert.GETCallGroup();
            nombreCallGroupApres = pagingCollectionDTOs.Items.ToList().Count;

            Assert.AreNotEqual(nombreCallGroupAvant, nombreCallGroupApres);

        }
    }
}
