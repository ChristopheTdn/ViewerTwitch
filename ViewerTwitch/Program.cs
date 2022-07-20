using System;
using System.IO;
using System.Text.Json;
using System.Net;
using System.Timers;
using System.Diagnostics;
using System.Reflection;
using System.Windows;


namespace ViewerTwitch
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // TIMER PRINCIPAL
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e);
            myTimer.Interval = 10 * 60000; // mn * 60000
            myTimer.Enabled = true;

            // TIMER PRINCIPAL
            System.Timers.Timer myTimer2 = new System.Timers.Timer();
            myTimer2.Elapsed += (sender, e) => OnTimedEvent_ECRIRE(sender, e);
            myTimer2.Interval = 1 * 60000; // mn * 60000
            myTimer2.Enabled = true;

            static Task ActualisePlanning() => new GBot().MainAsync("Read");
            
            Assembly execAssembly = Assembly.GetCallingAssembly();
            AssemblyName name = execAssembly.GetName();
            Console.Title = (string.Format("{0}   {1} v.{2}.{3}.{4}",
                Environment.NewLine,
                name.Name.ToString(),
                name.Version.Major.ToString(),
                name.Version.Minor.ToString(),
                name.Version.Build.ToString()
                ));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(string.Format("{0}  {1} v.{2}.{3}.{4} {0}",
                Environment.NewLine,
                name.Name.ToString().ToUpper(),
                name.Version.Major.ToString(),
                name.Version.Minor.ToString(),
                name.Version.Build.ToString()
                ));
            Console.ForegroundColor = ConsoleColor.White;

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
                static Task ActualisePlanning() => new GBot().MainAsync("Read");
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
        private static void OnTimedEvent_ECRIRE(object source, ElapsedEventArgs e)
        {

            try
            {
                int minute = DateTime.Now.Minute;
                if (minute == 59)
                {

                Console.WriteLine("> Message Discord Transmis.");
                static Task EcritPresence() => new GBot().MainAsync("Write");
                EcritPresence();
                }
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
