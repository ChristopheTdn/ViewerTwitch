﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ViewerTwitch
{
    public class SessionSpartiate
    {
        // **************
        // * PROPRIETES *
        // **************

        private string localDir = "";
        private List<string> spartiate = new List<string>();
        private List<string> listeMembreEnLigne = new List<string>();
        private List<string> listeMembreEnLigneHoraire = new List<string>();
        private string channelViewer = "";
        private ViewerTwitch.JSONChatters JsonFluxChatters = new JSONChatters();
        private string heureSessionMin = "";
        private string heureSessionMax = "";


        // ***************************
        // * CONSTRUCTEUR PAR DEFAUT *
        // ***************************
        public SessionSpartiate()
        {
            // initialisation des constantes de la session
            this.localDir = Fnc_FindLocalDir() + "\\";
            this.spartiate = Fnc_ListeSpartiate();
            Fnc_heureStreamer(); // channelViewer et heureSession
            Core_Session();
        }

        // **********************
        // *         CORE       *
        // **********************
        private void Core_Session()
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("{0}", heureSessionMin);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" - ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("{0}", heureSessionMax);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("{0}", channelViewer);

            // enregistre les viewers dans le fichier
            Fnc_RecordViewersPoints();
            // renvois sur la console la liste actualisé des joueurs sur le creneau
            foreach (string membre in Fnc_ListeMembresEnLigne())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("   > ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0}", membre);
            }
            Console.WriteLine("");




        }

        // **********************
        // * METHODES DE CLASSE *
        // **********************
        public string Fnc_FindLocalDir()
        {
            // renvois le repertoire locale de l application
            // pour retrouver les fichiers .TXT
            string path = Directory.GetCurrentDirectory();
            return path;
        }

        private List<string> Fnc_ListeSpartiate()
        {
            // Renvois la liste des membres du discord spartiate (spartiates.txt)
            List<string> spartiates = new List<string>();
            using (StreamReader sr = new StreamReader(localDir + "spartiates.txt"))
            {
                string line = null;
                line = sr.ReadLine();
                while (line != null)
                {
                    spartiates.Add(line);
                    line = sr.ReadLine();
                }
            }
            return spartiates;
        }
        private List<string> Fnc_ListeMembresEnLigne()
        {
            // Renvois la liste des chatters d'un channel donné
            Fnc_heureStreamer();
            var url = "https://tmi.twitch.tv/group/user/" + channelViewer.ToLower() + "/chatters";
            var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            List<string> chatters = null;
            try
            {
                using (var response = webrequest.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var source = reader.ReadToEnd();
                    JsonFluxChatters = JsonSerializer.Deserialize<JSONChatters>(source);
                    chatters = JsonFluxChatters.ListeChatter(Fnc_ListeSpartiate());
                }
                if (chatters.Count() == 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(" Erreur de requete :");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" Le serveur ne renvois pas d'activité. Verifier le pseudo du streamer.");
                    Console.WriteLine("Le script poursuit son fonctionnement.");
                }
            }
            catch (WebException except)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" Erreur reseau  :");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("{0}", except);
                Console.WriteLine("Le script poursuit son fonctionnement.");

            }
            return chatters;
        }

        private void Fnc_heureStreamer()
        {
            // renvois un Tableau avec le creneau horaire et le channel du streamer
            // basé sur le fichier "planning.txt"

            channelViewer = "";
            using (StreamReader sr = new StreamReader(localDir + "planning.txt"))
            {
                string line = null;
                line = sr.ReadLine();

                while (line != null)
                {
                    string[] words = line.Split(' ');
                    if (words[0] == DateTime.Now.ToString("HH:00").Replace(":", "h"))
                    {
                        heureSessionMin = words[0];
                        heureSessionMax = words[2];
                        channelViewer = words[4].Replace("@", "");
                    }
                    line = sr.ReadLine();
                }
            }
        }
        private void Fnc_RecordViewersPoints()
        {
            // Enregistre 1 point par Viewer et le sauvegarde dans un fichier Texte

            string path = localDir + @"data\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\";
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            DateTime heure = DateTime.Now;
            TimeSpan ecart = new TimeSpan(1, 0, 0);

            string fileName = path + heure.ToString("HH") + "h00" + "-" + heure.Add(ecart).ToString("HH") + "h00-chatters.txt";

            listeMembreEnLigne = Fnc_ListeMembresEnLigne();

            if (File.Exists(fileName))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" Erreur Fichier  :");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("le fichier pour ce creneau existe deja.");
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line = null;
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        if (line!="")
                        { 
                        listeMembreEnLigneHoraire.Add(line.ToLower());
                        }
                        line = sr.ReadLine();
                    }
                }

            }
            else
            {
                listeMembreEnLigneHoraire.Clear();
            }
            try
            {
                int compteurTotal = 0;
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (string membre in listeMembreEnLigne)
                    {
                        if (!listeMembreEnLigneHoraire.Contains(membre.ToLower()))
                        {
                            writer.WriteLine("{0}", membre.ToLower());
                            compteurTotal++;
                        }
                    }
                    foreach (string membre in listeMembreEnLigneHoraire)
                    {
                        writer.WriteLine("{0}", membre.ToLower());
                        compteurTotal++;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n{0}", listeMembreEnLigne.Count().ToString());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" spartiate(s) sur le stream actuellement.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0}", compteurTotal.ToString());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" spartiate(s) au total sur le créneau.\n");

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" Erreur Fichier  :");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Erreur lors de l ecriture.");
                Console.WriteLine(e.ToString());
            }

        }

    }
}
    
    

