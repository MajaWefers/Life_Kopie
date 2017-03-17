using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Life_Kopie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Variablen für Zellen und Spielfeld, die per Textbox eingestellt werden können
        int zelle_breite = 15;
        int zelle_hoehe = 15;
        static int reihen = 20;
        static int spalten = 20;
        static int timer_interval = 50;

        //Variablen zur Steuerung
        static bool erste_gen = true;
        static bool stop = false;
        static int zaehler = 0;

        int[,] gen_akt = new int[reihen, spalten];
        int[] akt_lebendig = new int[5];

        Panel Spielfeld = new Panel();
        Panel Steuerung = new Panel();
        Panel Gleiter = new Panel();
        Panel U = new Panel();
        Panel Oktagon = new Panel();

        Label lblCounter = new Label();

        Button btnStart = new Button();
        Button btnNeu = new Button();

        private void Form1_Load(object sender, EventArgs e)
        {
            btnStart.Click += new EventHandler(btnStart_Click);
            btnNeu.Click   += new EventHandler(btnNeu_Click);
            timer1.Tick += new EventHandler(timer1_Tick);

            this.AutoScaleDimensions = new SizeF(3F, 6.5F);
            this.Text    = "Game of Life";

            Spielfeld_erstellen();
            Zellen_erstellen();
            
            //Buttons auf Panel "Steuerung" positionieren
            btnStart.Location   = new Point(20, 0);
            btnStart.Size       = new Size(51, 23);
            btnStart.Text       = "Start";
            Steuerung.Controls.Add(btnStart);

            btnNeu.Location    = new Point(20, 31);
            btnNeu.Size        = new Size(51, 23);
            btnNeu.Text        = "Neu";
            Steuerung.Controls.Add(btnNeu);

            //Vorzubelegende Figuren
            Oktagon_erstellen();
            Gleiter_erstellen();
            U_erstellen();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = timer_interval;

            timer1.Stop();

            int[,] gen_neu = new int[reihen, spalten];

            //Erste Generation setzen
            if (erste_gen)
            {
                Erste_Generation_speichern();
                erste_gen = false;
            }

            Farbe_setzen_1_0();

            Naechste_Gen(gen_akt, gen_neu);
            lblCounter.Text = Convert.ToString(zaehler);

            if (gen_akt == gen_neu)
            {
                timer1.Stop();
            }
            else
            {
                gen_akt = gen_neu;
                timer1.Start();
            }
        }

        private void Erste_Generation_speichern()
        {

            for (int r = 0; r < gen_akt.GetLength(0); r++)
            {
                for (int c = 0; c < gen_akt.GetLength(1); c++)
                {
                    Point pt = new Point(r * zelle_breite, c * zelle_hoehe);
                    //Prüfe die Zelle an dieser Stelle
                    Control zelle = Spielfeld.GetChildAtPoint(pt);
                    {
                        if (zelle.BackColor == Color.Black)
                        {
                            gen_akt[r, c] = 1;
                        }
                        else
                        {
                            gen_akt[r, c] = 0;
                        }
                    }
                } //column
            } //row
        }

        private void Farbe_setzen_1_0()
        {
            for (int r = 0; r < gen_akt.GetLength(0); r++)
            {
                for (int c = 0; c < gen_akt.GetLength(1); c++)
                {
                    Point pt = new Point(r * zelle_breite, c * zelle_hoehe);
                    Control zelle = Spielfeld.GetChildAtPoint(pt);

                    if (gen_akt[r, c] == 1)
                    {
                        zelle.BackColor = Color.Black;
                    }
                    else
                    {
                        zelle.BackColor = Color.White;
                    }
                }
            }
        }

        public static void Naechste_Gen(int[,] gen_akt, int[,] gen_neu)
        {
            for (int r = 0; r < gen_akt.GetLength(0); r++)
            {
                for (int c = 0; c < gen_akt.GetLength(1); c++)
                {
                    int reihe_oben = r;
                    int reihe_unten = r;
                    int spalte_links = c;
                    int spalte_rechts = c;

                    //Sonderbedingung für Ränder, damit Zellen am gegenüberliegenden Rand wieder auftauchen
                    if (r == 0)
                    {
                        reihe_oben = gen_akt.GetLength(0);
                    }
                    if (r == gen_akt.GetLength(0) - 1)
                    {
                        reihe_unten = -1;
                    }
                    if (c == 0)
                    {
                        spalte_links = gen_akt.GetLength(1);
                    }
                    if (c == gen_akt.GetLength(1) - 1)
                    {
                        spalte_rechts = -1;
                    }

                    //Leben und sterben
                    if (gen_akt[r, c] == 1)
                    {
                        //sterben
                        if (gen_akt[reihe_oben - 1, spalte_links - 1] + gen_akt[reihe_oben - 1, c] + gen_akt[reihe_oben - 1, spalte_rechts + 1]
                             + gen_akt[r, spalte_links - 1] + gen_akt[r, spalte_rechts + 1]
                             + gen_akt[reihe_unten + 1, spalte_links - 1] + gen_akt[reihe_unten + 1, c] + gen_akt[reihe_unten + 1, spalte_rechts + 1]
                             < 2
                             ||
                               gen_akt[reihe_oben - 1, spalte_links - 1] + gen_akt[reihe_oben - 1, c] + gen_akt[reihe_oben - 1, spalte_rechts + 1]
                             + gen_akt[r, spalte_links - 1] + gen_akt[r, spalte_rechts + 1]
                             + gen_akt[reihe_unten + 1, spalte_links - 1] + gen_akt[reihe_unten + 1, c] + gen_akt[reihe_unten + 1, spalte_rechts + 1]
                             > 3)
                        {
                            gen_neu[r, c] = 0;
                        }

                        //weiter leben
                        else
                        {
                            gen_neu[r, c] = 1;
                        }
                    }

                    if (gen_akt[r, c] == 0)
                    {
                        //neues Leben
                        if (gen_akt[reihe_oben - 1, spalte_links - 1] + gen_akt[reihe_oben - 1, c] + gen_akt[reihe_oben - 1, spalte_rechts + 1]
                             + gen_akt[r, spalte_links - 1] + gen_akt[r, spalte_rechts + 1]
                             + gen_akt[reihe_unten + 1, spalte_links - 1] + gen_akt[reihe_unten + 1, c] + gen_akt[reihe_unten + 1, spalte_rechts + 1]
                             == 3)
                        {
                            gen_neu[r, c] = 1;
                        }
                    }
                } //column
            } //row

            zaehler++;

        } //Naechste_Gen



        //Buttons und Click-Events
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (stop == false)
            {
                timer1.Start();
                btnStart.Text = "Pause";
                stop = true;
            }
            else
            {
                timer1.Stop();
                btnStart.Text = "Start";
                stop = false;
            }
        }

        private void btnNeu_Click(object sender, EventArgs e)
        {
            //Variablen zurücksetzen
            timer1.Stop();
            erste_gen = true;
            stop = false;
            btnStart.Text = "Start";
            zaehler = 0;
            lblCounter.Text = "";

            //Spielfeld leeren
            foreach (Control zelle in Spielfeld.Controls)
            {
                zelle.BackColor = Color.White;
            }

            //Zurücksetzen des Abfrage-Arrays
            for (int r = 0; r < gen_akt.GetLength(0); r++)
            {
                for (int c = 0; c < gen_akt.GetLength(1); c++)
                {
                    gen_akt[c, r] = 0;
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button akt_zelle = (Button)sender;
            if (akt_zelle.BackColor == Color.White)
            {
                akt_zelle.BackColor = Color.Black;
            }
            else
            {
                akt_zelle.BackColor = Color.White;
            }
        }



        //Spielfelder und Zellen erstellen
        private void Spielfeld_erstellen()
        {
            this.Controls.Add(Spielfeld);
            Spielfeld.Location = new Point(20, 20); //Startpunkt links oben
            lblCounter.Location = new Point(0, 0);
            lblCounter.AutoSize = true;
            Spielfeld.Controls.Add(lblCounter);
            Spielfeld.Size = new Size(zelle_breite * reihen, zelle_breite * spalten);
            Spielfeld.Visible = true;

            this.Controls.Add(Steuerung);
            Steuerung.Location = new Point(400, 20); //Startpunkt links oben
            Steuerung.Size = new Size(200,200);
            Steuerung.Visible = true;
        }

        private void Zellen_erstellen()
        {
            for (int r = 0; r < reihen; r++)
            {
                for (int c = 0; c < spalten; c++)
                {
                    Button zelle = new Button();
                    zelle.Location = new Point(r * zelle_breite, c * zelle_hoehe);
                    zelle.Size = new Size(zelle_breite, zelle_hoehe);
                    zelle.Click += button_Click;
                    zelle.FlatStyle = FlatStyle.Flat;
                    zelle.BackColor = Color.White;
                    Spielfeld.Controls.Add(zelle);
                }
            }
        }



        //Vorzubelegende Formen
        private void U_erstellen()
        {
            this.Controls.Add(U);
            U.Location = new Point(340, 185);
            U.Size = new Size(zelle_breite * 3, zelle_hoehe * 7);
            U.Visible = true;

            for (int r = 0; r < 7; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Button zelle = new Button();
                    //CellObject testzelle = new CellObject();
                    //testzelle.IsAlive = true;
                    zelle.Location = new Point(c * zelle_breite, r * zelle_hoehe);
                    zelle.Size = new Size(zelle_breite, zelle_hoehe);
                    zelle.Click += button_Click;
                    zelle.FlatStyle = FlatStyle.Flat;
                    if (r == 0 && c == 0 ||
                        r == 0 && c == 1 ||
                        r == 0 && c == 2 ||
                        r == 1 && c == 0 ||
                        r == 1 && c == 2 ||
                        r == 2 && c == 0 ||
                        r == 2 && c == 2 ||
                        r == 4 && c == 0 ||
                        r == 4 && c == 2 ||
                        r == 5 && c == 0 ||
                        r == 5 && c == 2 ||
                        r == 6 && c == 0 ||
                        r == 6 && c == 1 ||
                        r == 6 && c == 2)
                    {
                        zelle.BackColor = Color.Black;
                    }
                    else
                    {
                        zelle.BackColor = Color.White;
                    }
                    U.Controls.Add(zelle);
                }
            }
        }

        private void Gleiter_erstellen()
        {
            this.Controls.Add(Gleiter);
            Gleiter.Location = new Point(340, 125);
            Gleiter.Size = new Size(zelle_breite * 3, zelle_hoehe * 3);
            Gleiter.Visible = true;

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Button zelle = new Button();
                    zelle.Location = new Point(c * zelle_breite, r * zelle_hoehe);
                    zelle.Size = new Size(zelle_breite, zelle_hoehe);
                    zelle.Click += button_Click;
                    zelle.FlatStyle = FlatStyle.Flat;
                    if (r == 0 && c == 1 ||
                        r == 1 && c == 2 ||
                        r == 2 && c == 0 ||
                        r == 2 && c == 1 ||
                        r == 2 && c == 2)
                    {
                        zelle.BackColor = Color.Black;
                    }
                    else
                    {
                        zelle.BackColor = Color.White;
                    }
                    Gleiter.Controls.Add(zelle);
                }
            }
        }

        private void Oktagon_erstellen()
        {
            this.Controls.Add(Oktagon);
            Oktagon.Location = new Point(340, 20);
            Oktagon.Size = new Size(zelle_breite * 6, zelle_hoehe * 6);
            Oktagon.Visible = true;

            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 6; c++)
                {
                    Button zelle = new Button();
                    zelle.Location = new Point(c * zelle_breite, r * zelle_hoehe);
                    zelle.Size = new Size(zelle_breite, zelle_hoehe);
                    zelle.Click += button_Click;
                    zelle.FlatStyle = FlatStyle.Flat;
                    if (r == 0 && c == 1 ||
                        r == 0 && c == 4 ||
                        r == 1 && c == 0 ||
                        r == 1 && c == 2 ||
                        r == 1 && c == 3 ||
                        r == 1 && c == 5 ||
                        r == 2 && c == 1 ||
                        r == 2 && c == 4 ||
                        r == 3 && c == 1 ||
                        r == 3 && c == 4 ||
                        r == 4 && c == 0 ||
                        r == 4 && c == 2 ||
                        r == 4 && c == 3 ||
                        r == 4 && c == 5 ||
                        r == 5 && c == 1 ||
                        r == 5 && c == 4)
                    {
                        zelle.BackColor = Color.Black;
                    }
                    else
                    {
                        zelle.BackColor = Color.White;
                    }
                    Oktagon.Controls.Add(zelle);
                }
            }
        }
    }
}

public partial class CellObject : Button
{
    private bool isAlive = false;

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        set
        {
            isAlive = value;
        }
    }
}
