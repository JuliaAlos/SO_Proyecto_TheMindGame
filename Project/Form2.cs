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


namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        Socket server;
        string nombre;
        int id;
        public Form2(int id, Socket server, string nombre)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.id = id;
            this.server= server;
            this.nombre = nombre;
        }



        private void Form2_Load(object sender, EventArgs e)
        {
            nform.Text = "Partida " + id.ToString();
            Image miImagen = Image.FromFile("logo.png");
            Imagen3.Image = miImagen;
            Imagen3.SizeMode = PictureBoxSizeMode.CenterImage;
            var random = new Random();
            int num1 = random.Next(1, 100);
            Image Carta1 = Image.FromFile(num1 + ".png");
            Imagen.Image = Carta1;
            Imagen.SizeMode = PictureBoxSizeMode.StretchImage;
            Imagen.Click += new System.EventHandler(this.enviarcarta);
            int num2 = random.Next(1, 100);
            while (num1 == num2) {
                num2 = random.Next(1, 100);
            }
            Image Carta2 = Image.FromFile(num2 + ".png");
            Imagen2.Image = Carta2;
            Imagen2.SizeMode = PictureBoxSizeMode.StretchImage;
            Imagen2.Click += new System.EventHandler(this.enviarcarta);
            Image CartasComp = Image.FromFile("logo.png");
            pictureBox1.Image = CartasComp;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = CartasComp;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Image = CartasComp;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.Image = CartasComp;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.Image = CartasComp;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.Image = CartasComp;
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void enviarcarta(object sender, EventArgs e){
            PictureBox p = (PictureBox)sender;
            Imagen3.Image = p.Image;
            Imagen3.SizeMode = PictureBoxSizeMode.StretchImage;
            p.Visible = false;
        }

        private void Enviar_Click(object sender, EventArgs e)
        {
            string mensaje = "8/" +id+  "/" +nombre+ " : " + frase.Text;
            frase.Text = "";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        public void chat(string mensaje) {
            label1.Text = label1.Text+"\n"+mensaje;
        }

    }
}
