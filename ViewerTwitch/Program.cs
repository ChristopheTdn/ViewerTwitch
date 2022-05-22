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
            myTimer.Interval = 0.1 * 60000; 
            myTimer.Enabled = true;

            Console.WriteLine("Le script est lancé");
            ConsoleKeyInfo input;
            do
            {
                input = Console.ReadKey();
            } while (input.Key != ConsoleKey.E);

        }

        // Définition Fonctions Principales
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SessionSpartiate spartiate = new SessionSpartiate();
        }
      
    }
}
