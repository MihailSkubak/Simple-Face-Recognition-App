using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Simple_Face_Recognition_App
{
    public partial class Form2 : Form
    {
        string h;
        static string ip;
        static bool curtainsB, mistakeCurtains;
        static System.Media.SoundPlayer player;

        public Form2()
        {
            InitializeComponent();
        }
        static Label l;
        static bool Mistake;
        static void Connect(string Mes)
        {
            try
            {
                TcpClient tc = new TcpClient(ip, 80);
                ASCIIEncoding asen = new ASCIIEncoding();
                NetworkStream ns = tc.GetStream();
                StreamWriter sw = new StreamWriter(ns);
                byte[] ba = asen.GetBytes(Mes);
                ns.Write(ba, 0, ba.Length);
                mistakeCurtains = true;
            }
            catch
            {
                MessageBox.Show("Server connection error!");
                Mistake = true;
                mistakeCurtains = false;
            }
            if (!Mistake)
            {
                MessageBox.Show("Connected to  " + ip + " !");
                Mistake = true;
            }
        }
        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string words = "";
            if (e.Result.Confidence > 0.5) { l.Text = e.Result.Text; words = e.Result.Text; }

            if (words == "Google request")
            {
                Process.Start("https://www.google.com/search?q=" + "The weather in Warsaw");
            }
            if (words == "Open Youtube") { Process.Start("https://www.youtube.com/"); }

            if (words == "Open Google") { Process.Start("http://google.com"); }

            if (words == "turn on the light") { Connect("/LED=OFF1/r"); }
            if (words == "turn off the light") { Connect("/LED=ON1/r"); }

            if (words == "turn on the lamp") { Connect("/LED=OFF2/r"); }
            if (words == "turn off the lamp") { Connect("/LED=ON2/r"); }

            if (words == "turn on the outlet") { Connect("/LED=OFF3/r"); }
            if (words == "turn off the outlet") { Connect("/LED=ON3/r"); }

            if (words == "turn on the iron") { Connect("/LED=OFF5/r"); }
            if (words == "turn off the iron") { Connect("/LED=ON5/r"); }

            if (words == "turn on everything") { Connect("/LED=OFF4/r"); }
            if (words == "turn off everything") { Connect("/LED=ON4/r"); }

            if (words == "open curtains")
            {
                if (curtainsB == false)
                {
                    Connect("/LED=OFF6/r");
                    if (mistakeCurtains == true)
                    {
                        curtainsB = true;
                    }
                    if (mistakeCurtains == false)
                    {
                        curtainsB = false;
                    }
                }
            }
            if (words == "close curtains")
            {
                if (curtainsB == true)
                {
                    Connect("/LED=ON6/r");
                    if (mistakeCurtains == true)
                    {
                        curtainsB = false;
                    }
                    if (mistakeCurtains == false)
                    {
                        curtainsB = true;
                    }
                }
            }
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            l = TextFromSpeech;

            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-us");// ru-ru
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);


            Choices numbers = new Choices();
            numbers.Add(new string[] {"Open Google", "Open Youtube", "turn on the light", "turn off the light",
                "open curtains", "close curtains","turn on the lamp","turn off the lamp","turn on the outlet",
                "turn off the outlet","turn on the iron","turn off the iron","turn on everything",
                "turn off everything","Google request" });

            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(numbers);


            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);

            sre.RecognizeAsync(RecognizeMode.Multiple);//Single
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ip = textBox2.Text;
            Mistake = false;
            Connect(" ");
        }
    }
}
