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
            myTimer.Interval = 20 * 60000; // mn * 60000
            myTimer.Enabled = true;

            Console.WriteLine("Le script est lancé");
            Console.WriteLine("\n[F1]: Ouvre Planning.txt  [F2]: Ouvre Data Dir   [F5] : Refresh manuel\n");
            SessionSpartiate spartiate = new SessionSpartiate();
            ConsoleKeyInfo input;
            do
            {
                input = Console.ReadKey();
                if (input.Key == ConsoleKey.F5)
                { spartiate = new SessionSpartiate(); }
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
        private static void OpenPlanningTxt()
        {
            string rep = Directory.GetCurrentDirectory();
            try
            {
                Process.Start("notepad.exe",rep + @"\planning.txt");
                
            }
            catch { }   
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("\n[F1]: Ouvre Planning.txt  [F2]: Ouvre Data Dir  [F5] : Refresh manuel\n");
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
