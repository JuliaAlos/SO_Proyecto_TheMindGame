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
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        string nombre;
        string[] jugadores = new string [4];
        int numSelecionados=0;
        int[] filasGrid = new int[3];
        List<Form2> formularios = new List<Form2>();

     

        delegate void DelegadoParaEscribir(string mensaje); 
        public Form1()
         
        {
            InitializeComponent();

        }

        public void AbrirForm2(int id) {
            int count = formularios.Count();
            Form2 f2 = new Form2(count, id, server, nombre);
            formularios.Add(f2);
            f2.ShowDialog();
            
        }

        public void Listaconectados(string mensaje){
            string[] vector = mensaje.Split(',');
            int numero = Convert.ToInt32(vector[0]);
            for (int i = 1; i < 10; i++){
                if (i <= numero)
                    ConectadosGrid[0, i - 1].Value = vector[i];
                else
                    ConectadosGrid[0, i - 1].Value = "";
            }
        }

        public void CambiarColor(string mensaje) {
            this.BackColor = Color.Green;
        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                int numform = Convert.ToInt32(trozos[1]);
                string mensaje = trozos[2].Split('\0')[0];

                switch (codigo)
                {
                    case 1:     //Registrar
                        if (mensaje == "0")
                        {
                            MessageBox.Show(Usuario.Text + " registrado correctamente");
                            DelegadoParaEscribir color = new DelegadoParaEscribir(CambiarColor);
                            this.Invoke(color, new object[] { mensaje });
                            nombre = Usuario.Text;
                        }
                        else
                        {
                            if(mensaje=="1")
                                MessageBox.Show(Usuario.Text + " ya existe");

                            else
                                MessageBox.Show("Se ha alcanzado el maximo de usuarios");

                            // Se terminó el servicio. 
                            // Nos desconectamos
                            atender.Abort();
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                        }
                        break;

                    case 2:     //Login
                        if (mensaje == "0")
                        {
                            DelegadoParaEscribir color = new DelegadoParaEscribir(CambiarColor);
                            this.Invoke(color, new object[] {mensaje});
                            MessageBox.Show("Hola " + Usuario.Text);
                            nombre = Usuario.Text;
                        }
                        else
                        {
                            if (mensaje == "1")
                                MessageBox.Show("Usuario o contraseña incorrecta");

                            else if (mensaje == "2")
                                MessageBox.Show("Se ha alcanzado el maximo de usuarios");

                            else
                                MessageBox.Show(Usuario.Text + " ya tiene una sesion abierta");
                            // Se terminó el servicio. 
                            // Nos desconectamos
                            atender.Abort();
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                        }

                        break;

                    case 3:     //El mas rapido
                        MessageBox.Show("El mas ràpido ha sido: " + mensaje);
                        break;

                    case 4:     //Los que ganaron a Joel
                        string[] jugador = mensaje.Split(',');
                        string ganadores = jugador[0];
                        for (int i = 1; i < jugador.Length - 1; i++)
                        {
                            ganadores = ganadores + ", " + jugador[i];
                        }
                        MessageBox.Show("Los que ganaron contra Joel son: " + ganadores);
                        break;

                    case 5:
                        MessageBox.Show("El jugador que mas partidas ha jugado es. " + mensaje);
                        break;

                    case 6:
                        DelegadoParaEscribir delegado = new DelegadoParaEscribir(Listaconectados);
                        ConectadosGrid.Invoke(delegado,new object[] {mensaje});
                        break;

                    case 7:
                        if (mensaje == "-1")
                            MessageBox.Show("Lo sentimos, no se ha podido crear la partida");
                        else
                        {
                            string[] partida = mensaje.Split(',');
                            string pregunta = "Quieres unirte a la partida " + partida[0] + " con " + partida[1];
                            DialogResult result1 = MessageBox.Show(pregunta,"Nueva partida",MessageBoxButtons.YesNo);
                            if (result1 == DialogResult.Yes)
                                mensaje = "7/0/"+partida[0]+","+ nombre+",YES";
                            else
                                mensaje = "7/0/" + partida[0] + ","+nombre+ ",NO";
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);
                        }
                        break;

                    case 8:
                        trozos = mensaje.Split(',');
                        int ID;
                        if (trozos[0] == "1")
                        {
                            ID = Convert.ToInt32(trozos[1]);
                            ThreadStart ts = delegate { AbrirForm2(ID); };
                            Thread T = new Thread(ts);
                            T.Start();
                        }
                        else
                            MessageBox.Show("No tienes suficientes amigos");
                        break;
                    case 9:
                        formularios[numform].chat(mensaje);
                        break;
                }

            }
        }

        
        private void Desconectar_Click(object sender, EventArgs e)
        {

            try
            {
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/" + nombre);
                server.Send(msg);

                // Se terminó el servicio. 
                // Nos desconectamos
                this.BackColor = Color.Gray;
                atender.Abort();
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
                    //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                    //al que deseamos conectarnos
                    IPAddress direc = IPAddress.Parse("192.168.56.102");
                    IPEndPoint ipep = new IPEndPoint(direc, 9020);
                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(ipep);//Intentamos conectar el socket
                    //pongo en marcha el thread que atendrá los mensajes del servidor
                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("2/0/" + Usuario.Text + "/" + Contraseña.Text);
                    server.Send(msg);
                    
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
                    IPEndPoint ipep = new IPEndPoint(direc, 9020);
                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(ipep);//Intentamos conectar el socket
                    //pongo en marcha el thread que atendrá los mensajes del servidor
                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("1/0/" + Usuario.Text + "/" + Contraseña.Text);
                    server.Send(msg);
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
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("3/0/");
                    server.Send(msg);

                }
                else if (Ganadores.Checked)
                {
                    // Quiere saber quien ha ganado contra Joel
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("4/0/");
                    server.Send(msg);

                    


                }
                else if (Viciado.Checked)
                {
                    // Quiere saber quien ha jugado mas partidas
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("5/0/");
                    server.Send(msg);
                }
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error con la peticion");
                return;
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

        private void ConectadosGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (numSelecionados == 0)
            {
                jugadores[numSelecionados] = nombre;
                numSelecionados++;
            }

            else
            {
                ConectadosGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                jugadores[numSelecionados] = ConectadosGrid.CurrentCell.Value.ToString();
                filasGrid[numSelecionados - 1] = e.RowIndex;
                numSelecionados++;
            }
        }

        private void Invitar_Click(object sender, EventArgs e)
        {
            // Quiere invitar a jugar 
            // Enviamos al servidor el nombre tecleado
            string mensaje = "6/0/";
            if (numSelecionados > 1)
            {
                for (int i = 0; i < numSelecionados; i++)
                {
                    mensaje = mensaje + jugadores[i] +",";
                }
                MessageBox.Show(mensaje);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }
    }
}
