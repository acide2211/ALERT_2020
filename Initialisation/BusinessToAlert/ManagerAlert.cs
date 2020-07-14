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
using Alert.Api.Remote.Entities;

namespace BusinessToAlert
{
    public class ManagerAlert
    {
        #region Déclaration des variables 
        //Varible du singleton
        private static ManagerAlert _instance = null;
        //Permet d'éciter d'avoir deux instance
        private static readonly object _padlock = new object();
        // Class qui gère la db
        private ManagerDB _managerDB;
        // Token de connexion
        private LoginResponseBody _loginResponseBody { get; set; }

        private NLog.Logger Logger;
        #endregion
        #region Constructeur
        public static ManagerAlert getInstance(NLog.Logger Logger = null)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    if (Logger != null)
                    {
                        // Synchronisation des système de log                       
                        _instance = new ManagerAlert(Logger);
                    }
                    else
                    {
                        throw new Exception("Le Logger n'est pas initialiser");
                    }

                }
                return _instance;
            }
        }


        private ManagerAlert(NLog.Logger Logger)
        {
            // Synchronisation des système de log
            this.Logger = Logger;
            _managerDB = ManagerDB.getInstance(Logger);

        }
        #endregion
        #region Login
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

        #endregion
        #region CallGroup
        /// <summary>
        /// Fonction qui permet de récupérer tout les call group qui existe dans alert
        /// </summary>
        /// <returns></returns>
        public PagingCollectionDTO<CallGroupDTO> GETCallGroupDTO()
        {
            // Déclarration de la variable de réception
            PagingCollectionDTO<CallGroupDTO> _callGroups = new PagingCollectionDTO<CallGroupDTO>();

            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "callgroups";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddHeader("Content-Type", "application/json");

            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);
            _callGroups = JsonConvert.DeserializeObject<PagingCollectionDTO<CallGroupDTO>>(response.Content);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction GETCallGroupDTO " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers GET API Alert" + response.StatusCode);
            }
            // return
            return _callGroups;
        }

        #region CallGroup POST
        /// <summary>
        /// Fonction qui permet de crée un nouveau Groupe d'appelle
        /// </summary>
        /// <param name="CallGroup"> Object qui contient les informations du groupe d'appelle </param>
        public void POSTCallGroup(CallGroupDTO CallGroup)
        {
            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "/callgroups";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddJsonBody(CallGroup);
            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction POSTCreateCallGroup " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers API Alert" + response.StatusCode);
            }
        }

        /// <summary>
        /// Fonction qui permet de crée un tableau de CallGroup
        /// </summary>
        /// <param name="callGroupDTOs">Object qui contient les informations du groupe d'appelle</param>
        public void CreateCallGroup(List<CallGroupDTO> callGroupDTOs)
        {
            // On crée la liste avec toutes les tâches que l'on va lancer
            List<Task> tasks = new List<Task>();

            foreach (CallGroupDTO item in callGroupDTOs)
            {
                //La on crée une une task qui elle va faire la requete POST aupres de l'api
                Task t = Task.Run(() => POSTCallGroup(item));
                tasks.Add(t);

            }

            // On attend que toute les tache soit bien finie
            Task.WaitAll(tasks.ToArray());
        }
        #endregion

        #region CallGroup PUT
        /// <summary>
        /// Fonction qui permet de modifier un callgroup qui existe déjà
        /// </summary>
        /// <param name="callGroupDTO">Object qui contient les informations du groupe d'appelle</param>
        private void PUTCallGroup(CallGroupDTO callGroupDTO)
        {
            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "/callgroups/" + callGroupDTO.Id;
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddJsonBody(callGroupDTO);

            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction PUTCallGroup " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers API Alert" + response.StatusCode);
            }

        }

        /// <summary>
        /// Fonction qui permet de crée un tableau de CallGroup
        /// </summary>
        /// <param name="callGroupDTOs">Object qui contient les informations du groupe d'appelle</param>
        public void PUTCallGroup(List<CallGroupDTO> callGroupDTOs)
        {
            // On crée la liste avec toutes les tâches que l'on va lancer
            List<Task> tasks = new List<Task>();

            //La on crée une une task qui elle va faire la requete PUT aupres de l'api
            foreach (CallGroupDTO item in callGroupDTOs)
            {
                Task t = Task.Run(() => PUTCallGroup(item));
                tasks.Add(t);

            }

            // On attend que toute les tache soit bien finie
            Task.WaitAll(tasks.ToArray());

        }

        #endregion


        #endregion



    }
}
