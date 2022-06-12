using System;
using System.IO;
using System.Text.Json;
using System.Net;
using System.Timers;
using System.Diagnostics;

namespace ViewerTwitch
{
    class Program
    {
        static void Main(string[] args)
        {

            // TIMER PRINCIPAL
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e);
            myTimer.Interval = 15 * 60000; // mn * 60000
            myTimer.Enabled = true;

            static Task ActualisePlanning() => new GBot().MainAsync();

            Console.WriteLine("Le script est lancé");
            // Actualise le planning 
            ActualisePlanning();
            System.Threading.Thread.Sleep(5000);

            afficheMenu();

            SessionSpartiate spartiate = new SessionSpartiate();
            ConsoleKeyInfo input;
            do
            {
                input = Console.ReadKey();
                if (input.Key == ConsoleKey.F5)
                {
                    // Actualise le planning 
                    ActualisePlanning();
                    System.Threading.Thread.Sleep(5000);
                    afficheMenu();
                    spartiate = new SessionSpartiate(); }
                if (input.Key == ConsoleKey.F1)
                { 
                    Interract interract = new Interract(input); 
                }
                if (input.Key == ConsoleKey.F2)
                {
                   Interract interract = new Interract(input);
                }

            } while (input.Key != ConsoleKey.Escape);
        }

        // Définition Fonctions Principales
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            try
            {
                static Task ActualisePlanning() => new GBot().MainAsync();
                ActualisePlanning();
                System.Threading.Thread.Sleep(5000);
                afficheMenu();
                SessionSpartiate spartiate = new SessionSpartiate();
            }
            catch (Exception except)
            {
                Console.WriteLine("Probleme d'erreur lors de la requete : {0}", except.ToString());
            }
        }
        private static void afficheMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("\n[F1]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" : Affiche Planning.txt    ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[F2]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" : Ouvre Data Dir   ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[F5]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" : Refresh manuel\n\n");
        }
      
    }
}
