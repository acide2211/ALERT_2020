using BusinessToDBAlert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Création_Planning
{
    class Program
    {
        //Variable qui permet de crée les logs


        static void Main(string[] args)
        {   
            NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
            ManagerDB managerDB;
           
            managerDB = ManagerDB.getInstance(Logger);
            Utile utile = new Utile(managerDB, Logger);

            Console.WriteLine("Jour : " + utile.CalculJourByDate(2019, 07, 4));
            //utile.CreateJour();
            Console.ReadKey();
        }
    }
}
