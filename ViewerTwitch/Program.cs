using System;
using System.IO;
using System.Text.Json;
using System.Net;
using System.Timers;

namespace ViewerTwitch
{
    class Program
    {
        static void Main(string[] args)
        {

            // TIMER PRINCIPAL
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e);
            myTimer.Interval = 20 * 60000; // mn * 60000
            myTimer.Enabled = true;

            Console.WriteLine("Le script est lancé");
            SessionSpartiate spartiate = new SessionSpartiate();
            ConsoleKeyInfo input;
            do
            {
                input = Console.ReadKey();
                if (input.Key == ConsoleKey.R)
                { spartiate = new SessionSpartiate(); }
            } while (input.Key != ConsoleKey.Escape);

        }

        // Définition Fonctions Principales
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                SessionSpartiate spartiate = new SessionSpartiate();
            }
            catch (Exception except)
            {
                Console.WriteLine("Probleme d'erreur lors de la requete : {0}", except.ToString());
            }
        }
      
    }
}
