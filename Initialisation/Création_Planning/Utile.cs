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

        List<JourSemaine> jourSemaines;
        public Utile(ManagerDB managerDB, NLog.Logger logger)
        {
            if (managerDB != null)
            {
                _managerDB = managerDB;
            }
            else
            {
                _managerDB = ManagerDB.getInstance();
            }

            // Création des Jours de l'année
            List<Annee> annees = _managerDB.GETListAnnee();

            // Récupération des Roles 
            List<Role> roles = _managerDB.GETListRoles();

            // Récupération des jours de la semaine 
            jourSemaines = _managerDB.GETListJourSeamaine();

            JourSemaine test;

            test = (from jourSemaine in jourSemaines
                    where jourSemaine.Numero.Equals(1)
                    select jourSemaine).First();




            // Parcour des années pour récupérer les mois

            foreach (Annee annee in annees)
            {
                List<Mois> moiss = annee.Mois.ToList();

                // Parcour des roles
                foreach (Role roleItem in roles)
                {
                    Console.WriteLine( "Role " + roleItem.Nom);
                    // Parcour des mois pour crée les jours si il n'existe pas

                    foreach (Mois moisItem in moiss)
                    {
                        if (moisItem.Jour.Count == 0)
                        {
                            logger.Debug("Le mois n'est pas initialiser");
                            logger.Debug("Initialisation du mois");

                            switch (moisItem.Numero)
                            {
                                case 1:
                                case 3:
                                case 5:
                                case 7:
                                case 8:
                                case 10:
                                case 12: CreateJour(roleItem, moisItem, 31);
                                    break;

                            }

                           // Console.WriteLine("Mois : " + moisItem.Numero );
                        }

                        //Console.WriteLine("Mois : " + moisItem.Numero + "Role " + roleItem.Nom);
                    }



                }


            }
        }

        public void CreateJour(Role role, Mois mois, int nombreJour)
        {
            Console.WriteLine("Role " + role.Nom);
            mois.Jour.Clear();

            int? dayOfWeek;
            for (int jourIndex = 1; jourIndex <= nombreJour; jourIndex++)
            {
                dayOfWeek = CalculJourByDate(mois.Annee.Annee1, mois.Numero, jourIndex);
                Jour jour = new Jour();
                jour.JourSemaineId = (from jourSemaine in jourSemaines
                                      where jourSemaine.Numero.Equals(dayOfWeek)
                                      select jourSemaine.Id).First();
                jour.Numero = jourIndex;
                jour.Tranche.Clear();


                List<Tranche> tranches = new List<Tranche>();
                for(int indexTranche = 1; indexTranche <= 96; indexTranche ++)
                {
                    Tranche tranche = new Tranche();
                    tranche.Jour = jour;
                    tranche.NumeroTranche = indexTranche;
                    tranche.Role = role;
                    jour.Tranche.Add(tranche);
                }



                mois.Jour.Add(jour);
            }
            //_managerDB.InsertMois(mois);
            _managerDB.SubmitChangesDB();
            Console.WriteLine();
        }

        /// <summary>
        /// Fonction qui permet de calculer le jour de la semaine, Utilisé l’algorithme de Mike Keith avec rajout de 1 pour que le lundi soit le premier jour de la semaine
        /// </summary>
        /// <param name="annee"> Année</param>
        /// <param name="mois"> Mois</param>
        /// <param name="jour"> Jour</param>
        /// <returns>
        /// 1 = Lundi
        /// 2 = Mardi
        /// 3 = Mercredi
        /// 4 = Jeudi
        /// 5 = Vendredi
        /// 6 = Samedi
        /// 7 = Dimanche
        /// </returns>
        public int? CalculJourByDate(int? annee, int? mois, int? jour)
        {
            int? dayOfWeek;
            int? z;

            
            if (mois < 3)
            {
                z = annee - 1;
            }
            else
            {
                z = annee;
            }
            dayOfWeek = ((23 * mois) / 9) + jour + 4 + annee + (z / 4) - (z / 100 ) + (z / 400 ) - 2;
            dayOfWeek = dayOfWeek % 7;
            return dayOfWeek + 1;

        }

    }
}
