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

    public enum EnumHTMLVerbe
    {
        POST,
        GET,
        PUT,
        DELETE
    }
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
                Logger.Error("Login Echoué");
                Logger.Error("Responde Status Code " + response.StatusCode);
                Logger.Error("Response body " + response.Content);
                throw (new Exception("Problème lors de la connexion"));
            }

        }

        #endregion
        #region CallGroup GET
        /// <summary>
        /// Fonction qui permet de récupérer tout les call group qui existe dans alert
        /// </summary>
        /// <returns></returns>
        public PagingCollectionDTO<CallGroupDTO> GETCallGroup()
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
        #endregion
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

            Logger.Debug("Insert la fonction POSTCreateCallGroup " + CallGroup.Name);
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
        #region CallGroup DELETE

        private void DELETECallGroup(CallGroupDTO callGroup)
        {
            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "callgroups/" + callGroup.Id.ToString();
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.DELETE);
            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);

            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction GETCallGroupDTO " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers GET API Alert" + response.StatusCode);
            }

            Logger.Debug("Delete la fonction DELETECallGroup " + callGroup.Name);


        }
        #endregion


        /// <summary>
        /// Fonction qui permet depuis un tableau de CallGroup de faire l'action définie dans l'énumération
        /// </summary>
        /// <param name="callGroupDTOs"></param>
        /// <param name="enumHTML"></param>
        public void ManagerCallGroups(List<CallGroupDTO> callGroupDTOs, EnumHTMLVerbe enumHTML)
        {
            // On crée la liste avec toutes les tâches que l'on va lancer
            List<Task> tasks = new List<Task>();

            #region CallGroup
            //La on crée une une task qui elle va faire la requete PUT aupres de l'api
            foreach (CallGroupDTO item in callGroupDTOs)
            {
                Task t = null;
                switch (enumHTML)
                {
                    case EnumHTMLVerbe.POST:
                        t = Task.Run(() => POSTCallGroup(item));
                        break;
                    case EnumHTMLVerbe.GET:
                        break;
                    case EnumHTMLVerbe.PUT:
                        t = Task.Run(() => PUTCallGroup(item));
                        break;
                    case EnumHTMLVerbe.DELETE:
                        t = Task.Run(() => DELETECallGroup(item));
                        break;
                    default:
                        break;
                }

                tasks.Add(t);

            }
            #endregion

            // On attend que toute les tache soit bien finie
            Task.WaitAll(tasks.ToArray());

        }

        #region User GET

        #endregion
        public PagingCollectionDTO<UserDTO> GETUsers()
        {

            // Déclarration de la variable de réception
            PagingCollectionDTO<UserDTO> _users = new PagingCollectionDTO<UserDTO>();

            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "users";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddHeader("Content-Type", "application/json");

            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);
            _users = JsonConvert.DeserializeObject<PagingCollectionDTO<UserDTO>>(response.Content);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction GETUserDTO " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers GET API Alert" + response.StatusCode);
            }
            // return
            return _users;

        }

        private void POSTUser(UserDTO UserDTO)
        {
            string url = _managerDB._configurations["URL"] + "/users";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddJsonBody(UserDTO);
            IRestResponse response = client.Execute(request);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction POSTCreateUsers " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers API Alert" + response.StatusCode);
            }

            Logger.Debug("Insert la fonction POSTCreateUsers " + UserDTO.Name);

        }

        private void PUTUser(UserDTO userDTO)
        {
            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "/users/" + userDTO.Id;
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddJsonBody(userDTO);

            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction PUTUser " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers API Alert" + response.StatusCode);
            }

        }


        public void ManagerUser(List<UserDTO> usersNew, EnumHTMLVerbe enumHTML)
        {
            // On crée la liste avec toutes les tâches que l'on va lancer
            List<Task> tasks = new List<Task>();

            #region User
            //La on crée une une task qui elle va faire la requete PUT aupres de l'api
            foreach (UserDTO item in usersNew)
            {
                Task t = null;
                switch (enumHTML)
                {
                    case EnumHTMLVerbe.POST:
                        t = Task.Run(() => POSTUser(item));
                        break;
                    case EnumHTMLVerbe.GET:
                        break;
                    case EnumHTMLVerbe.PUT:
                        t = Task.Run(() => PUTUser(item));
                        break;
                    case EnumHTMLVerbe.DELETE:
                      //  t = Task.Run(() => DELETECallGroup(item));
                        break;
                    default:
                        break;
                }

                tasks.Add(t);

            }
            #endregion

            // On attend que toute les tache soit bien finie
            Task.WaitAll(tasks.ToArray());

        }

        #region Team 


        public PagingCollectionDTO<TeamDTO> GETTeamByCallGroup(uint? callGroupId)
        {
            // Déclarration de la variable de réception
            PagingCollectionDTO<TeamDTO> _teams = new PagingCollectionDTO<TeamDTO>();

            // Construction de la requête
            string url = _managerDB._configurations["URL"] + "callgroups/" + callGroupId + "/teams";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddHeader("Content-Type", "application/json");

            // Attende de la réponse de la requête
            IRestResponse response = client.Execute(request);
            _teams = JsonConvert.DeserializeObject<PagingCollectionDTO<TeamDTO>>(response.Content);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction GETTeamByCallGroup " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers GET API Alert" + response.StatusCode);
            }
            
            return _teams;
        }

        private void POSTTeam(TeamDTO item)
        {
            string url = _managerDB._configurations["URL"] + "/teams";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", _loginResponseBody.TokenType + " " + _loginResponseBody.AccessToken);
            request.AddJsonBody(item);
            IRestResponse response = client.Execute(request);

            // Controle si la réponse a été refusée par le serveur
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Error("Erreur dans la fonction POSTTeam " + response.StatusCode);
                throw new Exception("Erreur lors de la réquête vers API Alert" + response.StatusCode);
            }

            Logger.Debug("Insert la fonction POSTTeam " + item.Name);
        }

        public void ManagerTeams(List<TeamDTO> teamNewDTOs, EnumHTMLVerbe enumHTML)
        {
            // On crée la liste avec toutes les tâches que l'on va lancer
            List<Task> tasks = new List<Task>();

            #region Team
            //La on crée une une task qui elle va faire la requete PUT aupres de l'api
            foreach (TeamDTO item in teamNewDTOs)
            {
                Task t = null;
                switch (enumHTML)
                {
                    case EnumHTMLVerbe.POST:
                        t = Task.Run(() => POSTTeam(item));
                        break;
                    case EnumHTMLVerbe.GET:
                        break;
                    case EnumHTMLVerbe.PUT:
                        //    t = Task.Run(() => PUTUser(item));
                        break;
                    case EnumHTMLVerbe.DELETE:
                        //  t = Task.Run(() => DELETECallGroup(item));
                        break;
                    default:
                        break;
                }

                tasks.Add(t);

            }
            #endregion

            // On attend que toute les tache soit bien finie
            Task.WaitAll(tasks.ToArray());
        }



        #endregion



    }
}
