using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ViewerTwitch
{
    public class Interract
    {
        private string localDir = "";
        

        // ***************************
        // * CONSTRUCTEUR PAR DEFAUT *
        // ***************************
        public Interract()
        {
           localDir = Fnc_FindLocalDir() + @"\";
            Core_Session();
        }
        public Interract(ConsoleKeyInfo input)
        {
            localDir = Fnc_FindLocalDir() + @"\";
            if (input.Key == ConsoleKey.F1)
            { OpenPlanningTxt(); }
            if (input.Key == ConsoleKey.F2)
            { openDataDir(); }
        }

        // **********************
        // *         CORE       *
        // **********************
        private void Core_Session()
        {
        }


        private void OpenPlanningTxt()
        {
            string rep = Directory.GetCurrentDirectory();
            try
            {
                Process.Start("notepad.exe", rep + @"\planning.txt");

            }
            catch { }
        }

        private void openDataDir()
        {
            //ouvre le repertoire DATA
            string link = localDir + "data";
            Process.Start("explorer.exe", link);
        }
        private string Fnc_FindLocalDir()
        {
            // renvois le repertoire locale de l application
            // pour retrouver les fichiers .TXT
            string path = Directory.GetCurrentDirectory();
            return path;
        }
    }
}
