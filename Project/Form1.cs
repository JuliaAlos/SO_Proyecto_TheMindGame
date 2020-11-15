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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        string nombre;
        public Form1()
         
        {
            InitializeComponent();
        }

        private void Desconectar_Click(object sender, EventArgs e)
        {

            try
            {
                
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/");
                server.Send(msg);
                // Se terminó el servicio. 
                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
                this.timer1.Stop();


            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error al cerrar conexion");
                return;
            } 
        }

        private void Login_Click(object sender, EventArgs e)
        {
            try
            {

                if ((Usuario.Text != "") && (Contraseña.Text != ""))
                {
                    //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                    //al que deseamos conectarnos
                    IPAddress direc = IPAddress.Parse("192.168.56.102");
                    IPEndPoint ipep = new IPEndPoint(direc, 9070);


                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    server.Connect(ipep);//Intentamos conectar el socket
                    string mensaje = "2/" + Usuario.Text + "/" + Contraseña.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    if (mensaje == "0")
                    {
                        this.BackColor = Color.Green;
                        MessageBox.Show("Hola " +Usuario.Text);
                        nombre = Usuario.Text;
                        this.timer1.Start();
                     }

                     else
                     {
                        MessageBox.Show(Usuario.Text + " no existe");
                        msg = System.Text.Encoding.ASCII.GetBytes("0/");
                        server.Send(msg);
                        // Se terminó el servicio. 
                        // Nos desconectamos
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                    }
                }

                else
                {
                    MessageBox.Show("Error en los campos de los datos");
                }
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error con la conexion");
                return;
            }

        }

        private void Registro_botton_Click_1(object sender, EventArgs e)
        {
            try
            {

                if ((Usuario.Text != "") && (Contraseña.Text != ""))
                {
                    //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                    //al que deseamos conectarnos
                    IPAddress direc = IPAddress.Parse("192.168.56.102");
                    IPEndPoint ipep = new IPEndPoint(direc, 9070);


                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(ipep);//Intentamos conectar el socket

                    string mensaje = "1/" + Usuario.Text + "/" + Contraseña.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    if (mensaje == "0")
                    {
                        MessageBox.Show(Usuario.Text + " registrado correctamente");
                    }
                    else
                    {
                        MessageBox.Show(Usuario.Text + " ya existe");

                        msg = System.Text.Encoding.ASCII.GetBytes("0/");
                        server.Send(msg);
                        // Se terminó el servicio. 
                        // Nos desconectamos
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                    }
                }

                else
                {
                    MessageBox.Show("Error en los campos de los datos");
                }
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error con la peticion");
                return;
            }
        }

        private void Consultar_Click(object sender, EventArgs e)
        {
            try
            {

                if (Rapido.Checked)
                {
                    // Quiere saber quien ha ganado mas rapido
                    string mensaje = "3/";
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    MessageBox.Show("El mas ràpido ha sido: " + mensaje);
                }
                else if (Ganadores.Checked)
                {
                    // Quiere saber quien ha ganado contra Joel
                    string mensaje = "4/";
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    string[] jugador = mensaje.Split('/');
                    string ganadores = "Los que ganaron contra Joel son: ";
                    for (int i = 0; i < jugador.Length; i++)
                    {
                        ganadores = ganadores + jugador[i] + ", ";
                    }
                    MessageBox.Show(ganadores);


                }
                else if (Viciado.Checked)
                {
                    // Quiere saber quien ha jugado mas partidas
                    string mensaje = "5/";
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    MessageBox.Show("El jugador que mas partidas ha jugado es. "+mensaje);
                }
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error con la peticion");
                return;
            } 
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000;
            // Quiere saber quien está conectado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("/7");
            server.Send(msg);

            //Recibimos la respuesta del servidor
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            string respuesta= Encoding.ASCII.GetString(msg2).Split('\0')[0];
            //MessageBox.Show(respuesta);
            string[] vector=respuesta.Split('/');
            int i;
            for (i = 0; i < vector.Length; i++)
            {
                ConectadosGrid[0,i].Value = vector[i];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConectadosGrid.Columns.Add("Usuario","Usuario");
            ConectadosGrid.RowCount = 10;
            ConectadosGrid.ColumnHeadersVisible = true;
            ConectadosGrid.RowHeadersVisible = false;
            ConectadosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            int i;
            for (i = 0; i < 10; i++){

                ConectadosGrid[0, i].Value = "";
            }
        }
    }
}
