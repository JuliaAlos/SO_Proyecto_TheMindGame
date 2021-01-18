using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Media;


namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        Socket server;

        string nombre;//Nombre del usuario
        string[] jugadores = new string [3];//Nombres de los otros jugadores
        int id;//Identificador de la partida
        int NumJugadores;

        int num1;//Carta 1 del jugador
        int num2;//Carta 2 del jugador
        int num3;//Carta 3 del jugador
        int cuentaAtras;
        int Nivel;
        SoundPlayer sonido;
        SoundPlayer alarma;
        SoundPlayer flip;
        SoundPlayer win;

        public Form2(int id, Socket server, string nombre, string mensajeCartas,string mensajeJugadores)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.Nivel = 1;
            this.id = id;
            this.server= server;
            this.nombre = nombre;
            this.Cartas(mensajeCartas);
            this.NombresJugadores(mensajeJugadores);
            timer1.Interval = 1000;
            timer1.Start();
            Timer.Text = "60";
            this.cuentaAtras = 60;
            sonido = new SoundPlayer(Application.StartupPath + @"\son\cancion.wav");
            sonido.Play();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            nform.Text = "Partida " + id;
            
            Imagen.Click += new System.EventHandler(this.enviarcarta1);
            Imagen2.Click += new System.EventHandler(this.enviarcarta2);
            Imagen3.Click += new System.EventHandler(this.enviarcarta3);
        }

        private void enviarcarta1(object sender, EventArgs e)
        {   //Mueve la carta y envia un mensaje con la jugada realizada
            string mensaje = "10/" + id + "/" + nombre + "/" + num1;
            PictureBox p = (PictureBox)sender;
            p.Visible = false;
            Imagen4.Image = p.Image;
            Imagen4.SizeMode = PictureBoxSizeMode.StretchImage;
            
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
        private void enviarcarta2(object sender, EventArgs e)
        {   //Mueve la carta y envia un mensaje con la jugada realizada
            string mensaje = "10/" + id + "/" + nombre + "/" + num2;
            PictureBox p = (PictureBox)sender;
            p.Visible = false;
            Imagen4.Image = p.Image;
            Imagen4.SizeMode = PictureBoxSizeMode.StretchImage;
            
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
        private void enviarcarta3(object sender, EventArgs e)
        {   //Mueve la carta y envia un mensaje con la jugada realizada
            string mensaje = "10/" + id + "/" + nombre + "/" + num3;
            PictureBox p = (PictureBox)sender;
            p.Visible = false;
            Imagen4.Image = p.Image;
            Imagen4.SizeMode = PictureBoxSizeMode.StretchImage;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void Enviar_Click(object sender, EventArgs e)
        {   //Envia un mensaje al servidor con la frase a enviar por el chat
            string mensaje = "8/" +id+  "/" +nombre+ " : " + frase.Text;
            frase.Text = "";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        public void NombresJugadores(string text)
        {   //En funcion de los jugadores que hay en la partida
            //introduce sus nombres en el form 
            if (Nivel == 1)
            {
                string[] mensaje = text.Split(',');
                NumJugadores = Convert.ToInt32(mensaje[0]);
                int j = 0;
                
                for (int i = 0; i < NumJugadores; i++)
                {
                    if (mensaje[1 + i] != nombre)
                    {
                        jugadores[j] = mensaje[1 + i];
                        if (j == 0)
                            User1Lbl.Text = jugadores[j];
                            
                        else if (j == 1)
                            User2Lbl.Text = jugadores[j];
                            
                        else
                            User3Lbl.Text = jugadores[j];
                            
                        j++;
                    }
                }
            }
            Image miImagen = Image.FromFile("logo.png");
            if (Nivel >= 1)
            {
                if (NumJugadores > 1)
                {
                    pictureBox1.Image = miImagen;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Visible = true;
                }
                if (NumJugadores > 2)
                {
                    pictureBox4.Image = miImagen;
                    pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox4.Visible = true;
                }
                if (NumJugadores > 3)
                {
                    pictureBox7.Image = miImagen;
                    pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox7.Visible = true;
                }                
                if (Nivel >= 2)
                {
                    if (NumJugadores > 1)
                    {
                        pictureBox2.Image = miImagen;
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox2.Visible = true;
                    }
                    if (NumJugadores > 2)
                    {
                        pictureBox5.Image = miImagen;
                        pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox5.Visible = true;
                    }
                    if (NumJugadores > 3)
                    {
                        pictureBox8.Image = miImagen;
                        pictureBox8.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox8.Visible = true;
                    }
                    if (Nivel >= 3)
                    {
                        if (NumJugadores > 1)
                        {
                            pictureBox3.Image = miImagen;
                            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox3.Visible = true;
                        }
                        if (NumJugadores > 2)
                        {
                            pictureBox6.Image = miImagen;
                            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox6.Visible = true;
                        }
                        if (NumJugadores > 3)
                        {
                            pictureBox9.Image = miImagen;
                            pictureBox9.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox9.Visible = true;
                        }
                    }
                }
            }
            
        }

        public void chat(string mensaje) 
        {
            ChatBox.Items.Insert(0, mensaje);
        }

        public void Cartas(string text)
        {   //A partir del numero de cartas recibidas muestra su imagen corespondente
            Image miImagen = Image.FromFile("logo.png");
            Imagen4.Image = miImagen;
            Imagen4.SizeMode = PictureBoxSizeMode.CenterImage;

            NivelLbl.Text = "Nivel " + Nivel;
            string [] vector =text.Split(',');
            if (Nivel >= 1)
            {
                num1 = Convert.ToInt32(vector[0]);
                Image Carta1 = Image.FromFile(num1 + ".png");
                Imagen.Image = Carta1;
                Imagen.SizeMode = PictureBoxSizeMode.StretchImage;
                
                Imagen.Visible = true;
                if (Nivel >= 2)
                {
                    num2 = Convert.ToInt32(vector[1]);
                    Image Carta2 = Image.FromFile(num2 + ".png");
                    Imagen2.Image = Carta2;
                    Imagen2.SizeMode = PictureBoxSizeMode.StretchImage;
                    
                    Imagen2.Visible = true;
                    if (Nivel == 3)
                    {
                        num3 = Convert.ToInt32(vector[2]);
                        Image Carta3 = Image.FromFile(num3 + ".png");
                        Imagen3.Image = Carta3;
                        Imagen3.SizeMode = PictureBoxSizeMode.StretchImage;
                        
                    }
                }
            }
            
        }
        public void Jugada(string mensaje)
        {
            string [] jugada = mensaje.Split(',');
            int res = Convert.ToInt32(jugada[0]);
            int carta = Convert.ToInt32(jugada[2]);
            if (carta != 101)
            {
                //Cambiamos la carta central
                Image Carta = Image.FromFile(carta + ".png");
                Imagen4.Image = Carta;
                Imagen4.SizeMode = PictureBoxSizeMode.StretchImage;

                //Actualizamos picture box de los otros jugadores en nuetro tablero
                int j = -1;//Localizamos el jugador vector de jugadores
                for (int i = 0; i < jugadores.Length; i++)
                {
                    if (jugadores[i] == jugada[1])
                    {
                        j = i;
                        i = jugadores.Length;//finalizamos bucle
                    }
                }
                if (j == 0)
                {
                    if (pictureBox1.Visible == true)
                        pictureBox1.Visible = false;
                    else if (pictureBox2.Visible == true)
                        pictureBox2.Visible = false;
                    else
                        pictureBox3.Visible = false;
                }
                if (j == 1)
                {
                    if (pictureBox4.Visible == true)
                        pictureBox4.Visible = false;
                    else if (pictureBox5.Visible == true)
                        pictureBox5.Visible = false;
                    else
                        pictureBox6.Visible = false;
                }
                if (j == 2)
                {
                    if (pictureBox7.Visible == true)
                        pictureBox7.Visible = false;
                    else if (pictureBox8.Visible == true)
                        pictureBox8.Visible = false;
                    else
                        pictureBox9.Visible = false;
                }
            }

            if (res == -1){
                timer1.Stop();
                win = new SoundPlayer(Application.StartupPath + @"\son\win.wav");
                win.Play();
                MessageBox.Show("WINNERS");
                this.Close();
                
            }
            else if (res == 1)
            {
                timer1.Stop();
                flip = new SoundPlayer(Application.StartupPath + @"\son\lost.wav");
                flip.Play();
                MessageBox.Show("LOSERS");
                this.Close();
                
            }
        }

        public void NextLevel(string cartas)
        {
            this.BackColor = Color.Purple;
            Nivel++;
            NombresJugadores(" ");
            Image miImagen = Image.FromFile("logo.png");
            Imagen4.Image = miImagen;
            Cartas(cartas);
            timer1.Start();
            Timer.Text = "60";
            this.cuentaAtras = 60;
            sonido = new SoundPlayer(Application.StartupPath + @"\son\cancion.wav");
            sonido.Play();

        }

        public void JugadorDesconectado(string nombre)
        {
            MessageBox.Show(nombre + "se ha desconectado. \n Partida finalizada");
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cuentaAtras--;
            Timer.Text = Convert.ToString(cuentaAtras);
            if (cuentaAtras == 10)
            {
                if (sonido != null)
                    sonido.Stop();
                alarma = new SoundPlayer(Application.StartupPath + @"\son\alarma.wav");
                alarma.Play();
                this.BackColor = Color.Red;
            }
            if (cuentaAtras == 9)
                this.BackColor = Color.Orange;
            if (cuentaAtras == 8)
                this.BackColor = Color.Red;
            if (cuentaAtras == 7)
                this.BackColor = Color.Orange;
            if (cuentaAtras == 6)
                this.BackColor = Color.Red;
            if (cuentaAtras == 5)
                this.BackColor = Color.Orange;
            if (cuentaAtras == 4)
                this.BackColor = Color.Red;
            if (cuentaAtras == 3)
                this.BackColor = Color.Orange;
            if (cuentaAtras == 2)
                this.BackColor = Color.Red;
            if (cuentaAtras == 1)
                this.BackColor = Color.Orange;
            if (cuentaAtras == 0)
            {
                if (alarma != null)
                    alarma.Stop();
                this.BackColor = Color.Purple;
                string mensaje = "10/" + id + "/" + nombre + "/101";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                timer1.Stop();
                flip = new SoundPlayer(Application.StartupPath + @"\son\lost.wav");
                flip.Play();
            }
        }

    }
}
