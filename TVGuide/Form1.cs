using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVGuide
{
    public partial class Form1 : Form
    {
        public static string ScreenScrape(string url)
        {
            return new System.Net.WebClient().DownloadString(url);
        }
        public Form1()
        {
            InitializeComponent();
            List<DateTime> weekdays = new List<DateTime>();
            weekdays.Add(DateTime.Now);
            weekdays.Add(DateTime.Now.AddDays(1));
            weekdays.Add(DateTime.Now.AddDays(2));
            weekdays.Add(DateTime.Now.AddDays(3));
            weekdays.Add(DateTime.Now.AddDays(-1));
            weekdays.Add(DateTime.Now.AddDays(-2));
            weekdays.Sort();
            foreach (var v in weekdays)
            {
                comboBoxDay.Items.Add(v.ToString("yyyy-MM-dd"));
            }

        }
        string[] progPartArray;
        string programPart;
        string all;
        string date;
        private void comboBoxDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            date = comboBoxDay.Text;
            all = ScreenScrape("http://www.tv.nu/" + date);
            int start = all.IndexOf("<li class=\"channel-schedule");
            int end = all.LastIndexOf("</li>			</ol>");
            int length = end - start;
            programPart = all.Substring(start, length);
            progPartArray = programPart.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            List<String> channels = new List<string>();
            foreach (var v in progPartArray)
            {
                if (v.StartsWith("data-slug="))
                {
                    var name = v.Substring(v.IndexOf("="));
                    channels.Add(name);
                }
            }
            List<String> cleanNames = new List<string>();
            foreach (var v in channels)
            {
                int s = 2;
                int nd = v.Length - 2;
                int l = nd - s;
                var name = v.Substring(s, l);
                cleanNames.Add(name);
                comboBoxChannels.Items.Add(name);
            }
        }
        List<string> ids = new List<string>();
        string progId;
        string[] programsInChannel2;
        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            ids.Clear();
            List<string> programList = new List<string>();
            var channel = comboBoxChannels.Text;
            date = comboBoxDay.Text;
            string url = "http://www.tv.nu/kanal/" + channel + '/' + date; 
            string all2 = ScreenScrape(url);
            progPartArray = all2.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);           
            foreach (var v in progPartArray)
            {
                if (v.StartsWith("data-id="))
                {
                    string id = v.Substring(v.IndexOf("="));
                    string cleanId = id.Substring(2, id.Length - 2).Trim();
                    string cleanId2 = cleanId.Remove(cleanId.Length - 1);
                    
                    if (cleanId2.Length>5)
                    ids.Add(cleanId2);
                }
            }
            var templist = ids;
            string[] sep2 = new string[] { "data-id=\"" };
            string[] programsInChannel = all2.Split(sep2, StringSplitOptions.None);
            programsInChannel2 = programsInChannel.Where(w => w != programsInChannel[0]).ToArray();
            foreach (var p in programsInChannel2)
            {
                int s1 = p.IndexOf("data-title=\"") + 12;
                int e1 = p.IndexOf("data-start-time=\"");
                int l1 = e1 - s1;
                string programName = p.Substring(s1, l1).Trim().Remove((p.Substring(s1, l1).Trim()).Length - 1);
                var convertedProgramName = Encoding.UTF8.GetString(Encoding.Default.GetBytes(programName));
                int timeStart = e1 + 28;
                string time = p.Substring(timeStart, 5).Trim();
               
                programList.Add(time + " " + convertedProgramName);
                
            }
            foreach (var prog in programList)
            {
                listView1.Items.Add(prog);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (listView1.SelectedItems.Count > 0)
            {
               
                int index = listView1.SelectedIndices[0];
                progId = ids[index];
                if (!Char.IsNumber(progId[progId.Length - 1]) && !Char.IsLetter(progId[progId.Length - 1]))
                    progId = progId.Remove(progId.Length - 1);
                string detailInfo = ScreenScrape("http://www.tv.nu/p/" + progId);
                int start = detailInfo.IndexOf("Om programmet");
                int end = detailInfo.IndexOf("Visas Ã¤ven pÃ¥ tv");
                int length = end - start;
                string temp = detailInfo.Substring(start, length);
                int start1 = temp.IndexOf("<p class=\"indent-bottom--small\">");
                int end1 = temp.IndexOf("<a href");
                int length1 = end1 - start1;
                string temp1 = temp.Substring(start1, length1).Trim();
                int s = temp1.IndexOf("small\">") + 7;
                int ed = temp1.IndexOf("</p>");
                int l = ed - s;
                string info = temp1.Substring(s, l).Trim();
                var convertedInfo = Encoding.UTF8.GetString(Encoding.Default.GetBytes(info));
                textBox1.Text = convertedInfo;
               
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //listView1.SelectedIndices.Clear();
        }
    }
}
