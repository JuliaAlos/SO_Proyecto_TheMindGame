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
        public Form1()
        {
            InitializeComponent();
        }

        private void Conectar_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            } 
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
                    // Quiere saber la longitud
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
                        MessageBox.Show("Hola " +Usuario.Text);
                    }
                    else
                    {
                        MessageBox.Show(Usuario.Text + " no existe");
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

        private void Registro_botton_Click_1(object sender, EventArgs e)
        {
            try
            {

                if ((Usuario.Text != "") && (Contraseña.Text != ""))
                {
                    // Quiere saber la longitud
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
    }
}
