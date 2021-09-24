using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sentencer
{
    public partial class Form1 : Form
    {
        private Rectangle richTextBox1OriginalRect;
        private Rectangle richTextBox2OriginalRect;
        private Rectangle button1OriginalRect;
        private Rectangle button2OriginalRect;
        private Rectangle button3OriginalRect;
        private Rectangle button4OriginalRect;
        private Rectangle button5OriginalRect;
        private Rectangle button7OriginalRect;

        private Size formOriginalSize;

        //string upper_string_todisplay = File.ReadAllText(@"C:\Temp\upper.txt",Encoding.UTF8);
        // Loading the two books next to the Application
        string upper_string_todisplay = File.ReadAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "upper.txt"), Encoding.UTF8);
        string lower_string_todisplay = File.ReadAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "lower.txt"), Encoding.UTF8);

        // Initializing the two Sentence pointers
        int upper_sentence_to_be_displayed = 1;
        int lower_sentence_to_be_displayed = 1;

        // this function takes two strings as parameters: the string that goes into the upper cell of the kindle page, and the one that goes into the lower cell
        // wwhich is the 3rd. The second middle cell is a gray filled cell acting as a separator
        // This function builds a HTML section that is a page in the mobi book
        static string add_page(string upper, string lower)
        {
            string buildup;
            buildup = string.Concat("<span style='font-size:11.0pt;line-height:107%;font-family:\"Calibri\",sans-serif;", Environment.NewLine, "mso-ascii-theme-font:minor-latin;mso-fareast-font-family:Calibri;mso-fareast-theme-font:", Environment.NewLine, "minor-latin;mso-hansi-theme-font:minor-latin;mso-bidi-font-family:Arial;", Environment.NewLine, "mso-bidi-theme-font:minor-bidi;mso-ansi-language:HU;mso-fareast-language:EN-US;", Environment.NewLine, "mso-bidi-language:AR-SA'><br clear = all style='mso-special-character:line-break;", Environment.NewLine, "page-break-before:always'> ", Environment.NewLine, " </span> ", Environment.NewLine, "", Environment.NewLine, " <p class=MsoNormal><o:p>&nbsp;</o:p></p>", Environment.NewLine, "", Environment.NewLine, "<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0", Environment.NewLine, " style='border-collapse:collapse;border:none;mso-border-alt:solid windowtext .5pt;", Environment.NewLine, " mso-yfti-tbllook:1184;mso-padding-alt:0cm 5.4pt 0cm 5.4pt'>", Environment.NewLine, " <tr style='mso-yfti-irow:0;mso-yfti-firstrow:yes;height:226.6pt'> ", Environment.NewLine, " <td width=483 valign=middle style='width:362.4pt;border:solid windowtext 1.0pt;", Environment.NewLine, "  mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;height:226.6pt'> ", Environment.NewLine, " <p class=MsoNormal align = center style='margin-bottom:0cm;margin-bottom:.0001pt;", Environment.NewLine, "  text-align:center;line-height:normal'><o:p>&nbsp;</o:p></p>", Environment.NewLine, "  <p class=MsoNormal align = center style='margin-bottom:0cm;margin-bottom:.0001pt;", Environment.NewLine, "  text-align:center;line-height:normal'>", upper, "<o:p></o:p></p>", Environment.NewLine, "  </td>", Environment.NewLine, " </tr>", Environment.NewLine, " <tr style='mso-yfti-irow:1'> ", Environment.NewLine, " <td width=483 valign=middle style='width:362.4pt;border:solid windowtext 1.0pt;", Environment.NewLine, "  border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;", Environment.NewLine, "  background:#404040;mso-background-themecolor:text1;mso-background-themetint:", Environment.NewLine, "  191;padding:0cm 5.4pt 0cm 5.4pt'> ", Environment.NewLine, " <p class=MsoNormal align = center style='margin-bottom:0cm;margin-bottom:.0001pt;", Environment.NewLine, "  text-align:center;line-height:normal'><o:p>&nbsp;</o:p></p>", Environment.NewLine, "  </td>", Environment.NewLine, " </tr>", Environment.NewLine, " <tr style='mso-yfti-irow:2;mso-yfti-lastrow:yes;height:254.4pt'> ", Environment.NewLine, " <td width=483 valign=middle style='width:362.4pt;border:solid windowtext 1.0pt;", Environment.NewLine, "  border-top:none;mso-border-top-alt:solid windowtext .5pt;mso-border-alt:solid windowtext .5pt;", Environment.NewLine, "  padding:0cm 5.4pt 0cm 5.4pt;height:254.4pt'> ", Environment.NewLine, " <p class=MsoNormal align = center style='margin-bottom:0cm;margin-bottom:.0001pt;", Environment.NewLine, "  text-align:center;line-height:normal'><o:p>&nbsp;</o:p></p>", Environment.NewLine, "  <p class=MsoNormal align = center style='margin-bottom:0cm;margin-bottom:.0001pt;", Environment.NewLine, "  text-align:center;line-height:normal'>", lower, "<o:p></o:p></p>", Environment.NewLine, "  </td>", Environment.NewLine, " </tr>", Environment.NewLine, "</table>", Environment.NewLine, "", Environment.NewLine, "<p class=MsoNormal><o:p>&nbsp;</o:p></p>", Environment.NewLine);
            return buildup;

        }

        //This function returns a string, which when appended to the RAW HTML closes the book
        static string end_book()
        {
            string buildup;
            buildup = string.Concat("", Environment.NewLine, "", Environment.NewLine, "</div>", Environment.NewLine, "", Environment.NewLine, "</body>", Environment.NewLine, "", Environment.NewLine, "</html>", Environment.NewLine, "");
            return buildup;

        }

        // This function locates the nth sentence in a long string
        private static string Sentence_locator(int nth, string text)
        {
            int sentence_count = 1;
            int i = 0;
            string temp, sentence_to_return = "";
            bool a = false;


            while ((i < text.Length) && (sentence_count < nth))  // while ((i <text.Length) | (sentence_count <nth))
            {
                a = (i < text.Length);
                a = (sentence_count < nth);
                a = ((i < text.Length) && (sentence_count < nth));

                if ((text[i] == '.') | (text[i] == '?') | (text[i] == '!'))
                {
                    sentence_count++;
                }
                i++;
            }

            try
            {

                while ((i < text.Length) ^ ((text[i] == '.') || (text[i] == '?') || (text[i] == '!')))
                {
                    sentence_to_return += text[i]; i++;

                }
                sentence_to_return += text[i];
            }
            catch (IndexOutOfRangeException e)
            {
                sentence_to_return = "End of the book.";
            }

            // resultString = Regex.Replace(subjectString, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
            temp = Regex.Replace(sentence_to_return, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
            return temp;
        }

        public void TextMove_MoveOnWithBoth()
        {
            richTextBox1.Text = Sentence_locator(++upper_sentence_to_be_displayed, upper_string_todisplay);
            richTextBox2.Text = Sentence_locator(++lower_sentence_to_be_displayed, lower_string_todisplay);
            //Displaying the sentence locations in the statusbar
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);
            toolStripStatusLabel1.BackColor = Color.SeaShell;
        }

        public void TextMove_MoveOnWithUpper()
        {
            richTextBox1.Text = Sentence_locator(++upper_sentence_to_be_displayed, upper_string_todisplay);
            //Displaying the sentence locations in the statusbar
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);
            toolStripStatusLabel1.BackColor = Color.SeaShell;
        }

        public void TextMove_MoveOnWithLower()
        {
            richTextBox2.Text = Sentence_locator(++lower_sentence_to_be_displayed, lower_string_todisplay);
            //Displaying the sentence locations in the statusbar
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);
            toolStripStatusLabel1.BackColor = Color.SeaShell;
        }

        public void TextMove_StepBackWithUpper()
        {
            richTextBox1.Text = Sentence_locator(--upper_sentence_to_be_displayed, upper_string_todisplay);
            //Displaying the sentence locations in the statusbar
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);
            toolStripStatusLabel1.BackColor = Color.SeaShell;
        }

        public void TextMove_StepBackWithLower()
        {
            richTextBox2.Text = Sentence_locator(--lower_sentence_to_be_displayed, lower_string_todisplay);
            //Displaying the sentence locations in the statusbar
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);
            toolStripStatusLabel1.BackColor = Color.SeaShell;
        }

        public Form1()
        {
            InitializeComponent();
            // Pick up the book where it was left off. 
            // An ini file stores the sentence number pair numbers. this file is updated upon program exit
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "sentencer_config.txt"));
            string useFullScreenStr = data["last_displayed_sentence_numbers"]["upper"];
            upper_sentence_to_be_displayed = int.Parse(useFullScreenStr);
            useFullScreenStr = data["last_displayed_sentence_numbers"]["lower"];
            lower_sentence_to_be_displayed = int.Parse(useFullScreenStr);
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);

            // Beaconing start into logfile
            File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}", "-", "\t", DateTime.Now.ToString(), "\t", "Sentencer Started ", Environment.NewLine));

            //Deleting resudie converion files if there is any at strtup
            File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "Példa.htm"));
            File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "példa.mobi"));

            if (Directory.Exists(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "Példa_files"))) Directory.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "Példa_files"), true);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formOriginalSize = this.Size;

            richTextBox1OriginalRect = new Rectangle(richTextBox1.Location.X, richTextBox1.Location.Y, richTextBox1.Width, richTextBox1.Height);
            richTextBox2OriginalRect = new Rectangle(richTextBox2.Location.X, richTextBox2.Location.Y, richTextBox2.Width, richTextBox2.Height);
            button1OriginalRect = new Rectangle(button1.Location.X, button1.Location.Y, button1.Width, button1.Height);
            button2OriginalRect = new Rectangle(button2.Location.X, button2.Location.Y, button2.Width, button2.Height);
            button3OriginalRect = new Rectangle(button3.Location.X, button3.Location.Y, button3.Width, button3.Height);
            button4OriginalRect = new Rectangle(button4.Location.X, button4.Location.Y, button4.Width, button4.Height);
            button5OriginalRect = new Rectangle(button5.Location.X, button5.Location.Y, button5.Width, button5.Height);
            button7OriginalRect = new Rectangle(button7.Location.X, button7.Location.Y, button7.Width, button7.Height);


            //Displaying the first sentences
            richTextBox1.Text = Sentence_locator(upper_sentence_to_be_displayed, upper_string_todisplay);
            richTextBox2.Text = Sentence_locator(lower_sentence_to_be_displayed, lower_string_todisplay);

            // Anchor the button to the bottom right corner of the form
            // According to the help: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.anchor?redirectedfrom=MSDN&view=net-5.0#System_Windows_Forms_Control_Anchor
            button6.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            richTextBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            richTextBox2.Anchor = (AnchorStyles.Left | AnchorStyles.Right);


            // Moving focus to the button to move both sentences.
            button1.Select();
        }

        private void resizeChildrenControls()
        {
            resizeControl(richTextBox1OriginalRect, richTextBox1);
            resizeControl(richTextBox2OriginalRect, richTextBox2);
            resizeControl(button1OriginalRect, button1);
            resizeControl(button2OriginalRect, button2);
            resizeControl(button3OriginalRect, button3);
            resizeControl(button4OriginalRect, button4);
            resizeControl(button5OriginalRect, button5);
            resizeControl(button7OriginalRect, button7);
        }

        private void resizeControl(Rectangle originalControlRect, Control control)
        {
            float xRatio = (float)(this.Width) / (formOriginalSize.Width);
            float yRatio = (float)(this.Height) / (formOriginalSize.Height);

            int newX = (int)(originalControlRect.X * xRatio);
            int newY = (int)(originalControlRect.Y * yRatio);
            int newWidth = (int)(originalControlRect.Width * xRatio);
            int newHeight = (int)(originalControlRect.Height * yRatio);

            control.Location = new Point(newX, newY);
            control.Size = new Size(newWidth, newHeight);

        }

        // Move on with both texts, Numpad 5 and j does the same. 
        private void button1_Click(object sender, EventArgs e)
        {
            TextMove_MoveOnWithBoth();
        }

        // Step back with upper text, Numpad 7 and u does the same
        private void button2_Click(object sender, EventArgs e)
        {
            TextMove_StepBackWithUpper();
        }

        // Step on with upper sentence, Numpad 9 and i does the same
        private void button3_Click(object sender, EventArgs e)
        {
            TextMove_MoveOnWithUpper();
        }

        // Step back with lower text, NumPad 1 and n does the same
        private void button4_Click(object sender, EventArgs e)
        {
            TextMove_StepBackWithLower();
        }

        // Step on with lower sentence, Numpad3 and m doeas the same
        private void button5_Click(object sender, EventArgs e)
        {
            TextMove_MoveOnWithLower();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 106)  // j key
                TextMove_MoveOnWithBoth();

            //Exporting to mobi
            if (e.KeyChar == 112)  // p key
            {
                toolStripStatusLabel1.Text = "Exporting to mobi..";
                string str_path_RAWHTMLfolder = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "RAW");
                string str_path_RAWHTMLfolder_for_convert = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen");

                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(str_path_RAWHTMLfolder, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(str_path_RAWHTMLfolder, str_path_RAWHTMLfolder_for_convert));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(str_path_RAWHTMLfolder, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(str_path_RAWHTMLfolder, str_path_RAWHTMLfolder_for_convert), true);

                //Adding a page to the temp HTML

                //File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "Példa.htm"), add_page((Sentence_locator(upper_sentence_to_be_displayed, upper_string_todisplay)), Sentence_locator(lower_sentence_to_be_displayed, lower_string_todisplay)) );

                //Ekkor jövök rá, hogy a file írásakor az ékezetes betűket rosszul írja a c#
                //Viszatérő baj és találtam rá megoldást: 
                //https://stackoverflow.com/questions/23855276/preserve-special-characters-when-writing-all-lines-to-text-file
                // In notepad++ I just had to convert the source HTM to UTF8 BOM, then it worked immediately. 

                //  Open the Readlog
                var str_ReadLogfilecontent = File.ReadLines(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"));
                foreach (var line_to_be_processed in str_ReadLogfilecontent)
                {
                    if (line_to_be_processed[0] == '+')
                    {
                        string[] splitstring = Regex.Split(line_to_be_processed, @"\t"); // [2] and [4] hold the correct upper and lower values
                        File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "Példa.htm"), add_page((Sentence_locator(Int32.Parse(splitstring[2]), upper_string_todisplay)), Sentence_locator(Int32.Parse(splitstring[4]), lower_string_todisplay)));
                    }
                }

                //Adding trailing close
                File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen", "Példa.htm"), string.Concat("</div>", Environment.NewLine, Environment.NewLine, "</body>", Environment.NewLine, Environment.NewLine, "</html>", Environment.NewLine));

                //Running the conversion
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kindlegen");
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C .\\kindlegen .\\Példa.htm -c0 -o példa.mobi";
                process.StartInfo = startInfo;
                process.Start();

            }

            if (e.KeyChar == 117)  // u key
            {
                TextMove_StepBackWithUpper();
            }

            if (e.KeyChar == 105)  // i key
            {
                TextMove_MoveOnWithUpper();
            }

            if (e.KeyChar == 110)  // n key
            {
                TextMove_StepBackWithLower();
            }

            if (e.KeyChar == 109)  // m key
            {
                TextMove_MoveOnWithLower();
            }

            //Button to note a matching pair Numpad 6 for marking a matching pair, k does the same
            if (e.KeyChar == 107)  // k key
            {
                // Adding a new line into the Logfile in "+	2021. 03. 22. 22:13:33	10 --> 9" format
                File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", "+", "\t", DateTime.Now.ToString(), "\t", upper_sentence_to_be_displayed.ToString(), "\t", "-->", "\t", lower_sentence_to_be_displayed.ToString(), Environment.NewLine));
                toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}      | Pair created! ", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);


                // Adding a new line into the logfile in actual sentences format. Not used as it messes up the logfile with the \r\n-s. 
                //File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "+", "\t", DateTime.Now.ToString(), "\t", Sentence_locator(upper_sentence_to_be_displayed, upper_string_todisplay), " --> ", Sentence_locator(lower_sentence_to_be_displayed, lower_string_todisplay), Environment.NewLine));

                // Turning the StritStatuslabel to green, but this green is too green, and for other buttonpresses I'd need to change it back, so this feature is unused
                // A nice list of available colors to use: 
                // http://www.flounder.com/csharp_color_table.htm

                toolStripStatusLabel1.BackColor = Color.Lime;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Marking the current displayed sentence locator numbers into the ini file.
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "sentencer_config.txt"));
            data["last_displayed_sentence_numbers"]["upper"] = upper_sentence_to_be_displayed.ToString();
            data["last_displayed_sentence_numbers"]["lower"] = lower_sentence_to_be_displayed.ToString();
            parser.WriteFile(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "sentencer_config.txt"), data);

            // Beaconing exit into logfile
            File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}", "-", "\t", DateTime.Now.ToString(), "\t", "Sentencer Exiting..", Environment.NewLine));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Numpad 5 for advancing both texts. 
            if (e.KeyCode == Keys.NumPad5) TextMove_MoveOnWithBoth();

            //Numpad 7 for stepping back the upper text
            if (e.KeyCode == Keys.NumPad7)
            {
                TextMove_StepBackWithUpper();
            }

            //Numpad 9 for advancing the upper text
            if (e.KeyCode == Keys.NumPad9)
            {
                TextMove_MoveOnWithUpper();
            }

            //Numpad 1 for stepping back lower text
            if (e.KeyCode == Keys.NumPad1)
            {
                TextMove_StepBackWithLower();
            }

            //Numpad 3 for advancing lower text
            if (e.KeyCode == Keys.NumPad3)
            {
                TextMove_MoveOnWithLower();
            }

            //Numpad 6 for marking a matching pair, k does the same
            if (e.KeyCode == Keys.NumPad6)
            {

                // Adding a new line into the Logfile in "+	2021. 03. 22. 22:13:33	10 --> 9" format
                File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", "+", "\t", DateTime.Now.ToString(), "\t", upper_sentence_to_be_displayed.ToString(), "\t", "-->", "\t", lower_sentence_to_be_displayed.ToString(), Environment.NewLine));
                toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}      | Pair created! ", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);


                // Adding a new line into the logfile in actual sentences format. Not used as it messes up the logfile with the \r\n-s. 
                //File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "+", "\t", DateTime.Now.ToString(), "\t", Sentence_locator(upper_sentence_to_be_displayed, upper_string_todisplay), " --> ", Sentence_locator(lower_sentence_to_be_displayed, lower_string_todisplay), Environment.NewLine));

                // Turning the StritStatuslabel to green, but this green is too green, and for other buttonpresses I'd need to change it back, so this feature is unused
                // A nice list of available colors to use: 
                // http://www.flounder.com/csharp_color_table.htm

                toolStripStatusLabel1.BackColor = Color.Lime;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to Sentencer! \n\nThis minimalist app needs 2 text files next to itself. \nupper.txt gets displayed in the upper lines, lower.txt gets displayed on the low.\nUse Numpad 5,1,3,7,9 or j,n,m,u,i to navigate in the texts or use the buttons.\n\n Last sentencer textfile positions are saved upon exit\n\nCheers, \nAttila Czibere\n2019-09-30\n");
        }

        //Button to note a matching pair Numpad 6 for marking a matching pair, k does the same
        private void button7_Click(object sender, EventArgs e)
        {
            // Adding a new line into the Logfile in "+	2021. 03. 22. 22:13:33	10 --> 9" format
            File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", "+", "\t", DateTime.Now.ToString(), "\t", upper_sentence_to_be_displayed.ToString(), "\t", "-->", "\t", lower_sentence_to_be_displayed.ToString(), Environment.NewLine));
            toolStripStatusLabel1.Text = string.Format("Upper text sentence no: {0}    |  Lower text sentence no: {1}      | Pair created! ", upper_sentence_to_be_displayed, lower_sentence_to_be_displayed);


            // Adding a new line into the logfile in actual sentences format. Not used as it messes up the logfile with the \r\n-s. 
            //File.AppendAllText(Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ReadLog.txt"), string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "+", "\t", DateTime.Now.ToString(), "\t", Sentence_locator(upper_sentence_to_be_displayed, upper_string_todisplay), " --> ", Sentence_locator(lower_sentence_to_be_displayed, lower_string_todisplay), Environment.NewLine));

            // Turning the StritStatuslabel to green, but this green is too green, and for other buttonpresses I'd need to change it back, so this feature is unused
            // A nice list of available colors to use: 
            // http://www.flounder.com/csharp_color_table.htm

            toolStripStatusLabel1.BackColor = Color.Lime;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            resizeChildrenControls();
        }
    }
}
