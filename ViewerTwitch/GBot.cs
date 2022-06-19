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
    internal class GBot
    {
         private readonly DiscordSocketClient _client;


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
         public async Task MainAsync()
            {

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
            ulong channelID = Fnc_GetChannelID();

            var channel = _client.GetChannel(channelID) as SocketTextChannel;
            if (channel == null) return;
            var messages = await channel.GetMessagesAsync(10).FlattenAsync();
            string message = parseMsg(messages.Last().ToString());
            sauvegardePlanning(message);

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



        private string getToken()
            {
                StreamReader reader = File.OpenText("tokenSP.txt");
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

                if (lineCut.Length >= 5 && lineCut[4]!="")
                    { 
                        name = lineCut[4];
                        if (name.Substring(0, 1) == "<")
                        {
                            ulong id = ulong.Parse(lineCut[4].Replace("<", "").Replace(">", "").Replace("@", ""));
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
                    string lineParse = lineCut[0] + " " + lineCut[1] + " " + lineCut[2] + " " + lineCut[3] + " " + name;
                    messageParse += lineParse + '\n';
                }
                return messageParse;
         }
        private void sauvegardePlanning(string message)
        {
            StreamWriter writer = File.CreateText("planning.txt");
            writer.WriteLine(message);
            writer.Close();
            Console.WriteLine("planning.txt actualisé et sauvegardé...");
        }
    }
}


