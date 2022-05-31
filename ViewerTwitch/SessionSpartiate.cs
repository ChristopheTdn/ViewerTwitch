using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

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
        private int nbrChatters = 0;
        private string version = "";
        private string newVersion = "";


        // ***************************
        // * CONSTRUCTEUR PAR DEFAUT *
        // ***************************
        public SessionSpartiate()
        {
            // initialisation des constantes de la session
            this.localDir = Fnc_FindLocalDir() + @"\";
            Fnc_DLUpdateSpartiates();
            Fnc_GetVersion();
            Console.WriteLine(version);
            if (version != newVersion) { Fnc_UpdateScript(); }

            this.spartiate = Fnc_ListeSpartiate();
            listeMembreEnLigne = Fnc_ListeMembresEnLigne();

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
            if (listeMembreEnLigne.Count() > 0)
            {
                foreach (string membre in listeMembreEnLigne)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("   > ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("{0}", membre);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("   > ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Aucun membre en ligne.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("");





        }

        // **********************
        // * METHODES DE CLASSE *
        // **********************
        private void Fnc_UpdateScript()
        {
            // renomme l executable
            System.IO.File.Delete(localDir + "OLD_ViewerTwitch");
            System.IO.File.Delete(localDir + "OLD_version");
            System.IO.File.Move(localDir + "ViewerTwitch.exe", localDir + "OLD_ViewerTwitch");
            System.IO.File.Move(localDir + "version.txt", localDir + "OLD_version");


            // copie les nouvelles versions
            System.IO.File.Move(localDir + @"cfg/ViewerTwitch.exe", localDir + "ViewerTwitch.exe");
            System.IO.File.Move(localDir + @"cfg/version.txt", localDir + "version.txt");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("\nMISE A JOUR EFFECTUEE :");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("\nLe script va se relancer.\n");

            System.Threading.Thread.Sleep(5000);

            System.Diagnostics.ProcessStartInfo myInfo = new System.Diagnostics.ProcessStartInfo();
            myInfo.FileName = localDir + "ViewerTwitch.exe";
            System.Diagnostics.Process.Start(myInfo);

            Environment.Exit(0);

        }
        private  string Fnc_FindLocalDir()
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
            using (StreamReader sr = new StreamReader(localDir + @"cfg\spartiates.txt"))
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
        private void Fnc_DLUpdateSpartiates()
        {
            // Dl derniere version du fichier spartiates.txt
            try
            {
                if (!Directory.Exists(localDir + @"cfg\"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(localDir + @"cfg\");
                }
                WebClient webClient = new WebClient();
                webClient.DownloadFile("http://51.178.81.30/ftp/twviewers/spartiates.txt", localDir + @"cfg\spartiates.txt");
                webClient.DownloadFile("http://51.178.81.30/ftp/twviewers/version.txt", localDir + @"cfg\version.txt");
                webClient.DownloadFile("http://51.178.81.30/ftp/twviewers/ViewerTwitch.exe", localDir + @"cfg\ViewerTwitch.exe");

            }
            catch (WebException e)
            {
                // Traitement des erreurs
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" Erreur reseau  :");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("{0}\n Impossible d'obtenir la derniere version des fichiers. Le script va essayer de poursuivre avec les infos qu'il possede.");
            }
        }
        private void Fnc_GetVersion()
        {

                using (StreamReader sr = new StreamReader(localDir + @"\cfg\version.txt"))
                {
                    newVersion = sr.ReadLine();
                }
            try
            {
                using (StreamReader sr = new StreamReader(localDir + @"\version.txt"))
                {
                    version = sr.ReadLine();
                }
            }
            catch
            {
                
            }
            
        }
            private List<string> Fnc_ListeMembresEnLigne()
        {
            // Renvois la liste des chatters d'un channel donné
            Fnc_heureStreamer();
            var url = "https://tmi.twitch.tv/group/user/" + channelViewer.ToLower() + "/chatters";
            var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            List<string> chatters = null;
            // écrit en minuscule tout les pseudos
            List<string> chatterMini = new List<string>();
            try
            {
                using (var response = webrequest.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var source = reader.ReadToEnd();
                    JsonFluxChatters = JsonSerializer.Deserialize<JSONChatters>(source);
                    nbrChatters = JsonFluxChatters.chatter_count;
                    chatters = JsonFluxChatters.ListeChatter(Fnc_ListeSpartiate());
                }
                if (chatters.Count() == 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(" Erreur de requete :");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(" Le serveur ne renvoit pas d'activité. Verifier le pseudo du streamer dans le fichier planning.txt.");
                }
                // passe en misucule le nom des chatter
                if (chatters.Count() > 0)
                {
                    foreach (string membre in chatters)
                    {
                        chatterMini.Add(membre.ToLower());
                    }
                }
            }
            catch (WebException except)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" Erreur reseau  :");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("{0}\n Verifier le nom du streamer dans le fichier planning.txt sur cette plage horaire.", except.Message.ToString());

                Console.WriteLine("\n Le script poursuit son fonctionnement.\n");

            }
            return chatterMini;
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
                        try
                        {
                            channelViewer = words[4].Replace("@", "");
                        }
                        catch
                        {
                            channelViewer = "";
                                }
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

            if (File.Exists(fileName))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Dernière Maj fichier : {0}h{1}mn", DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"));
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
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\n{0}", nbrChatters);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" viewer(s) sur le stream.");               
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0}", listeMembreEnLigne.Count().ToString());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" spartiate(s) sur le stream.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0}", compteurTotal.ToString());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" spartiate(s) au total sur le créneau.\n");
                Console.ForegroundColor = ConsoleColor.White;

                // gestion et affichage des Spartiates Deco durant l'heure

                List <string> spartiateDeco = new List <string>();
                foreach (string spartiate in listeMembreEnLigneHoraire )
                {
                    if (!listeMembreEnLigne.Contains(spartiate))
                    {
                        spartiateDeco.Add(spartiate);
                    }
                }
                if (spartiateDeco.Count() > 0) {
                    Console.Write("Spartiate(s) deconnecté(s) durant le créneau : ");    
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    foreach (string membre in spartiateDeco)
                    {
                        Console.Write(" {0} ", membre.ToLower());                        
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");

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
    
    

