using System;
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
        private string channelViewer = "";
        private ViewerTwitch.TwChatters JsonFluxChatters = new TwChatters();
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
            foreach (string membre in Fnc_ListeMembresEnLigne())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("   > ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0}", membre);
            }
            Console.WriteLine("");
            Fnc_RecordViewersPoints();



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
            using (var response = webrequest.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var source = reader.ReadToEnd();
                JsonFluxChatters = JsonSerializer.Deserialize<TwChatters>(source);
                chatters = JsonFluxChatters.ListeChatter(Fnc_ListeSpartiate());
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
            string fileName = path+ DateTime.Now.ToString("HH")+"h"+DateTime.Now.ToString("mm") +"mn"+ DateTime.Now.ToString("ss")+"s"+@"-chatters.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (string membre in Fnc_ListeMembresEnLigne())
                    {
                        writer.WriteLine("{0}", membre);
                    }
                    
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }

        }
    }
}
    
    

