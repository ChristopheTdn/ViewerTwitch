using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ViewerTwitch
{
    public class GBot
    {
        private readonly DiscordSocketClient _client;
        public List<string> spartiateMembers = new List<string>();
        private string _arg = ""; 

        public GBot()
            {
                DiscordSocketConfig config = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.All | GatewayIntents.GuildMembers,
                    AlwaysDownloadUsers = true
                };

                
                _client = new DiscordSocketClient(config);
                // Subscribing to client events, so that we may receive them whenever they're invoked.
                _client.Ready += ReadyAsync;
            }
         public async Task MainAsync(string arg)
            {
                _arg = arg;
                string token = getToken();
                // Tokens should be considered secret data, and never hard-coded.
                await _client.LoginAsync(TokenType.Bot, token);
                // Different approaches to making your token a secret is by putting them in local .json, .yaml, .xml or .txt files, then reading them on startup.
                await _client.StartAsync();

                // Block the program until it is closed.
                await Task.Delay(10000);
            }

            // The Ready event indicates that the client has opened a
            // connection and it is now safe to access the cache.
         private async Task ReadyAsync()
         {
            if (_arg == "Read")
            {
                ulong channelID = Fnc_GetChannelID();

                var channel = _client.GetChannel(channelID) as SocketTextChannel;
                if (channel == null) return;
                var messages = await channel.GetMessagesAsync(10).FlattenAsync();
                string message = parseMsg(messages.Last().ToString());
                sauvegardePlanning(message);
                getListeUser();
            }
            if (_arg == "Write")
            {
                Ecrire_Discord();
            }
        }

        private ulong Fnc_GetChannelID()
        {
            ulong ChannelID=0;
            DayOfWeek jour = DateTime.Now.DayOfWeek;
            if (DateTime.Now.Hour <2 )
                {
                jour = DateTime.Now.AddDays(-1).DayOfWeek; 
                }

            switch (jour)
            {
                case DayOfWeek.Monday:
                    ChannelID = 979855578144858163;
                    break;
                case DayOfWeek.Tuesday:
                    ChannelID = 979855690879361035;
                    break;
                case DayOfWeek.Wednesday:
                    ChannelID = 979855775193264128;
                    break;
                case DayOfWeek.Thursday:
                    ChannelID = 979855851340857414;
                    break;
                case DayOfWeek.Friday:
                    ChannelID = 979856098855120916;
                    break;
                case DayOfWeek.Saturday:
                    ChannelID = 979856179645796382;
                    break;
                case DayOfWeek.Sunday:
                    ChannelID = 979856260759420978;
                    break;
            }
            return ChannelID;
        }
        private ulong Fnc_GetChannelID_Write()
        {
            ulong ChannelID = 0;
            DayOfWeek jour = DateTime.Now.DayOfWeek;
            if (DateTime.Now.Hour < 2)
            {
                jour = DateTime.Now.AddDays(-1).DayOfWeek;
            }

            switch (jour)
            {
                case DayOfWeek.Monday:
                    ChannelID = 997927665367535656;
                    break;
                case DayOfWeek.Tuesday:
                    ChannelID = 997934036599197757;
                    break;
                case DayOfWeek.Wednesday:
                    ChannelID = 997942533722226818;
                    break;
                case DayOfWeek.Thursday:
                    ChannelID = 997943662564626432;
                    break;
                case DayOfWeek.Friday:
                    ChannelID = 997949515258675250;
                    break;
                case DayOfWeek.Saturday:
                    ChannelID = 985535043285942292;
                    break;
                case DayOfWeek.Sunday:
                    ChannelID = 979862708251951184;
                    break;
            }
            return ChannelID;
        }



        private string getToken()
            {
                StreamReader ?reader;
                reader = File.OpenText("tokenSP.txt");
                string token = reader.ReadLine();
                return token;
            }
         private string parseMsg(string message)
            {
                string[] messageCut = message.Split("\n");
                
                string messageParse = "";


            foreach (string line in messageCut)
                {
                string[] lineCut = line.Split(' ');
                string name = "";

                // gestion des problemes de mise en forme dans la liste (espace en trop)
                List <string> lineClean = new List<string>();
                foreach (string word in lineCut)
                {
                    if (word=="")
                    {
                        continue;
                    }
                    else
                    {
                        lineClean.Add(word);
                    }
                }

                if (lineClean.Count >= 5 && lineClean[4]!="")
                    { 
                        name = lineClean[4];
                        if (name.Substring(0, 1) == "<")
                        {
                            ulong id = ulong.Parse(lineClean[4].Replace("<", "").Replace(">", "").Replace("@", ""));
                            var nick = _client.GetGuild(951887546273640598).GetUser(id).Nickname;
                            if (nick != null)
                            {
                                name = "@" + nick;
                            }
                            else
                            {
                                name = "@" + _client.GetUserAsync(id).Result.Username.ToString();
                            }
                        }
                    }
                    string lineParse = lineClean[0] + " " + lineClean[1] + " " + lineClean[2] + " " + lineClean[3] + " " + name;
                    messageParse += lineParse + '\n';
                }
                return messageParse;
         }
        private void sauvegardePlanning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Running process GBot...");
            StreamWriter writer = File.CreateText("planning.txt");
            writer.WriteLine(message);
            writer.Close();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" > ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\"planning.txt\" actualisé et sauvegardé...");
        }
        private void getListeUser()
        {
            // dl derniere version du fichier spartiates.txt
            try
            {
                StreamWriter writer = File.CreateText("spartiates.txt");
                foreach (var user in _client.Guilds.ToList()[0].Users.ToList())
                {
                    writer.WriteLine(user.DisplayName);
                }
                writer.Close();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(" > ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\"spartiates.txt\" actualisé et sauvegardé...");
            }
            catch (Exception e)
            {
                // Traitement des erreurs
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" Erreur reseau  :");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("{0}\n Impossible d'obtenir la dernière version du fichier spartiates.txt. Le script va essayer de poursuivre avec les infos qu'il possede.");
            }
        }
        private void Ecrire_Discord()
        {
            ulong channelID_Write = Fnc_GetChannelID_Write();
            string messageDiscord = "";
            using (StreamReader sr = new StreamReader("discord.txt"))
            {
                messageDiscord=sr.ReadToEnd();
            }
                _client.GetGuild(951887546273640598).GetTextChannel(channelID_Write).SendMessageAsync(messageDiscord);

        }
    }


}



