using BusinessToDBAlert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alert.Api.Remote.REST.Entities;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Alert.Api.Remote.REST.Clients.Converters;

namespace BusinessToAlert
{
    public class ManagerAlert
    {
        #region Déclaration des variables 
        // Class qui gère la db
        private ManagerDB _managerDB;
        // Token de connexion
        private LoginResponseBody _loginResponseBody { get; set; }


        private NLog.Logger Logger;
        #endregion

        public ManagerAlert(NLog.Logger Logger)
        {
            // Synchronisation des système de log
            this.Logger = Logger;
            _managerDB = ManagerDB.getInstance(Logger);

        }

        public void LoginAlertWS()
        {
            string Login = _managerDB._configurations["LOGIN"];
            string Password = _managerDB._configurations["PASSWORD"];

            string url = _managerDB._configurations["URL"] + "auth/login";
            Console.WriteLine("URL = " + url);
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", "grant_type=password&username=" + Login + "&password=&" + Password, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _loginResponseBody = JsonConvert.DeserializeObject<LoginResponseBody>(response.Content, new LoginResponseBodyConverter());
                 Logger.Info("Login Réussi");
            }
            else
            {
                throw (new Exception("Problème lors de la connexion"));
            }

        }
    }
}
