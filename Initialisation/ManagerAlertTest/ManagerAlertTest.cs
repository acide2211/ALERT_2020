using System;
using System.Collections.Generic;
using Alert.Api.Remote.Entities;
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
            PagingCollectionDTO<CallGroupDTO> pagingCollectionDTOs = _managerAlert.GetCallGroupDTO();
            List<CallGroupDTO> callgroups = pagingCollectionDTOs.Items.ToList();

        }
    }
}
