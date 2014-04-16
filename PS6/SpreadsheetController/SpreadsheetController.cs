using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace SS
{
    public class SpreadsheetController
    {
  /*      private SpreadsheetClientModel scm;
        public List<string> fileList;
        private Form1 form1;
        private LoginWindow logWin;

        private SpreadsheetController()
        {
            scm = new SpreadsheetClientModel();
            fileList = new List<string>();
            logWin = new LoginWindow(scm);

            scm.IncomingLineEvent += MessageReceived;
        }

        private void MessageReceived(string s)
        {
            string[] words = Regex.Split(s, "[esc]");

            if (words[0].Contains("FILELIST"))
            {
                new OpenF(this);

                foreach (string file in words)
                {
                    fileList.Add(file);
                }
            }
            if (words[0].Contains("INVALID"))
            {
                logWin.LoginFailed();
                
            }
        }
        public void OpenSC(string filename)
        {
            scm.SendMessage("OPEN[esc]" + filename + "\n");
            Form1 newForm = new Form1(filename, this);

            newForm.Open(newForm);
        }
 * */
    }

}
