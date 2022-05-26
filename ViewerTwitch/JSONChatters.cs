using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewerTwitch
{

    public class JSONChatters
    {
        // Constructeurs
        public _Links _links { get; set; }
        public int chatter_count { get; set; }
        public Chatters chatters { get; set; }

        // Methodes
        public List<string> ListeChatter(List<string> listeSpartiate)
        {
            List<string> _listePresent = new List<string>();
            foreach (string membre in listeSpartiate)
            {
                if (chatters.viewers.Contains(membre.ToLower()) ||
                    chatters.broadcaster.Contains(membre.ToLower()) ||
                    chatters.vips.Contains(membre.ToLower()) ||
                    chatters.moderators.Contains(membre.ToLower()) ||
                    chatters.staff.Contains(membre.ToLower()) ||
                    chatters.global_mods.Contains(membre.ToLower()) ||
                    chatters.admins.Contains(membre.ToLower()))

                { _listePresent.Add(membre); }
            }
            return _listePresent;
        }
    }

    // Classes  JSON
    public class _Links
    {
    }
    public class Chatters
    {
        public string[] broadcaster { get; set; }
        public string[] vips { get; set; }
        public string[] moderators { get; set; }
        public object[] staff { get; set; }
        public object[] admins { get; set; }
        public object[] global_mods { get; set; }
        public string[] viewers { get; set; }
    }

}

