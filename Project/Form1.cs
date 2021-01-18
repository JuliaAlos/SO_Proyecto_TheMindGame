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

        string nombre;//Nombre del usuario
        string[] jugadores = new string [3];//Lista de invitados
        int numSelecionados=0;//Contador de invitados
        int ConectadosNum;
        
        List<Form2> formularios = new List<Form2>();
        int[] partidas= new int [100]; //Lista para relacionar el numero de form con la id de la partida

        delegate void DelegadoParaEscribir(string mensaje);

        public Form1() 
        {
            InitializeComponent();

        }

        public void AbrirForm2(int id,string menjaseCartas, string mensajeJugadores) 
        {//Abre un nuevo formulario y le envia los parametros necesarios para su correcta operacion

            partidas[id] = formularios.Count();
            Form2 f2 = new Form2(id, server, nombre, menjaseCartas, mensajeJugadores);
            formularios.Add(f2);
            f2.ShowDialog();
        }

        public void Listaconectados(string mensaje)
        {//Rellena el grid con los usuarios conectados
        
            string[] vector = mensaje.Split(',');
            ConectadosNum = Convert.ToInt32(vector[0]);
            ConectadosGrid.Rows.Clear();
            for (int i = 0;  (i<ConectadosNum); i++){
                
                    ConectadosGrid.Rows.Add(vector[i+1]);
                
            }
        }

        public void DesbloquearFunciones(string Nombre)
        {
            ActivarOperaciones();
            NombreLbl.Text = Nombre;
        }

        public void BloquearFunciones(string mensaje)
        {
            ActivarOperacionesIniciales();
            InfoLbl.Text = "Usuario eliminado correctamente";
            // Se terminó el servicio. 
            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
        public void EscribirRespuesta(string info) {
            RespuestasBox.Text = info;
        }
        public void EscrivirInformacion(string info)
        {
            InfoLbl.Text = info;
        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[400];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                //MessageBox.Show(trozos[0] + "/" + trozos[1] + "/" + trozos[2]);
                int codigo = Convert.ToInt32(trozos[0]);//Tipo de mensaje
                int idPartida = Convert.ToInt32(trozos[1]);//Obtenemos el id de la partida
                //Limpiamos el mensaje
                string mensaje = trozos[2].Split('\0')[0];

                switch (codigo)
                {
                    case 1:     //Registrar
                        if (mensaje == "0")
                        {
                            nombre = Usuario.Text;
                            DelegadoParaEscribir delegado = new DelegadoParaEscribir(DesbloquearFunciones);
                            this.Invoke(delegado, new object[] { nombre });
                        }
                        else
                        {
                            DelegadoParaEscribir delegado = new DelegadoParaEscribir(EscrivirInformacion);
                            if (mensaje == "1")                                
                                this.Invoke(delegado, new object[] { "Este usuario ya existe" });
                            else
                                this.Invoke(delegado, new object[] { "Registro correcto, pero se ha alcanzado el \n maximo de usuarios, intenta conectarte más tarde" });
                                

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
                            nombre = Usuario.Text;
                            DelegadoParaEscribir delegado = new DelegadoParaEscribir(DesbloquearFunciones);
                            this.Invoke(delegado, new object[] { nombre });
                        }
                        else
                        {
                            DelegadoParaEscribir delegado = new DelegadoParaEscribir(EscrivirInformacion);
                            if (mensaje == "1")
                                this.Invoke(delegado, new object[] {"Usuario y/o contraseña incorrecta"});

                            else if (mensaje == "2")
                                this.Invoke(delegado, new object[] {"Se ha alcanzado el maximo de usuarios conectados \n Intentalo más tarde"});

                            else
                                this.Invoke(delegado, new object[] {Usuario.Text + " ya tiene una sesion abierta"});
                            // Se terminó el servicio. 
                            // Nos desconectamos
                            atender.Abort();
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                        }

                        break;

                    case 3:     //El mas rapido
                        DelegadoParaEscribir delegado5 = new DelegadoParaEscribir(EscribirRespuesta);
                        RespuestasBox.Invoke(delegado5, new object[] { mensaje });
                        break;

                    case 4:     //Los que ganaron a Joel
                        string[] ganadores = mensaje.Split('*');
                        string respuesta = ganadores[0];
                        for (int i = 1; i < ganadores.Length; i++)
                        {
                            respuesta = respuesta + "\r\n\r\n" + ganadores[i];
                        }
                        DelegadoParaEscribir delegado3 = new DelegadoParaEscribir(EscribirRespuesta);
                        RespuestasBox.Invoke(delegado3, new object[] { respuesta });
                        break;

                    case 5:
                        string [] Viciado = mensaje.Split(',');
                        DelegadoParaEscribir delegado6 = new DelegadoParaEscribir(EscribirRespuesta);
                        RespuestasBox.Invoke(delegado6, new object[] { "El jugador que mas partidas ha jugado es " +  Viciado[0]+ " y ha jugado " + Viciado[1]+ " veces"});
                        break;

                    case 6:     //Actualiza la lista de conectados en el grid
                        DelegadoParaEscribir delegado2 = new DelegadoParaEscribir(Listaconectados);
                        ConectadosGrid.Invoke(delegado2,new object[] {mensaje});
                        break;

                    case 7:     //Crear partida o preguntar si quiere unirse a una
                        if (mensaje == "-1")
                            MessageBox.Show("Lo sentimos, no se ha podido crear la partida");
                        else
                        {
                            string[] partida = mensaje.Split(',');
                            string pregunta = "Quieres unirte a una partida con " + partida[1];

                            DialogResult result1 = MessageBox.Show(pregunta, "Nueva partida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result1 == DialogResult.Yes)
                                mensaje = "7/0/"+partida[0]+","+ nombre+",YES";
                            else
                                mensaje = "7/0/" + partida[0] + ","+nombre+ ",NO";
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);
                        }
                        break;

                    case 8:     //Iniciar el form de la partida si hay suficientes jugadores
                        trozos = mensaje.Split('-');
                        string [] aux = trozos[0].Split(',');
                        if (aux[0] == "1")
                        {
                            string mensajeJugadores = trozos[1];
                            string mensajeCartas = trozos[2];
                            idPartida = Convert.ToInt32(aux[1]);
                            ThreadStart ts = delegate { AbrirForm2(idPartida,mensajeCartas,mensajeJugadores); };
                            Thread T = new Thread(ts);
                            T.Start();                            
                        }
                        else
                            MessageBox.Show("No tienes suficientes amigos");
                        break;
                    case 9:     //Envia mensaje del chat al formulario correcto
                        formularios[partidas[idPartida]].chat(mensaje);
                        break;

                    case 10:
                        if (mensaje == "0")
                        {
                            DelegadoParaEscribir delegado4 = new DelegadoParaEscribir(BloquearFunciones);
                            this.Invoke(delegado4, new object[] { "" });
                        }
                        break;

                    case 11:    //Envia la jugada realizada por un jugador al formulario correspondente
                        formularios[partidas[idPartida]].Jugada(mensaje);
                        break;
                    case 12:    //Un jugador se ha desconectado
                        formularios[partidas[idPartida]].JugadorDesconectado(mensaje);
                        break;
                    case 13:    //Jugadores ganan la ronda y pasan al siguiente nivel
                        formularios[partidas[idPartida]].NextLevel(mensaje);
                        break;
                }
            }
        }

        
        private void Desconectar_Click(object sender, EventArgs e)
        {

            try
            {
                // Enviamos al servidor el nombre tecleado para realizar la desconexion
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/");
                server.Send(msg);

                // Se terminó el servicio. 
                // Nos desconectamos
                atender.Abort();
                server.Shutdown(SocketShutdown.Both);
                server.Close();
                for(int i=0; i<formularios.Count();i++)
                    formularios[i].Close();
                ActivarOperacionesIniciales();
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
                    IPAddress direc = IPAddress.Parse("147.83.117.22");
                    IPEndPoint ipep = new IPEndPoint(direc, 50063);
                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //Intentamos conectar el socket
                    server.Connect(ipep);
                    
                    //Pongo en marcha el thread que atendrá los mensajes del servidor
                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();

                    // Enviamos al servidor el nombre tecleado con la contraseña
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("2/0/" + Usuario.Text + "/" + Contraseña.Text);
                    server.Send(msg);
                    
                }

                else
                {
                    MessageBox.Show("Error al introducir los datos, asegurate que has rellenado los campos correctamente");
                }
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error con la conexion");
                return;
            }

        }
        
        private void Registro_Click(object sender, EventArgs e)
        {
            try
            {

                if ((Usuario.Text != "") && (Contraseña.Text != ""))
                {

                    //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                    //al que deseamos conectarnos
                    IPAddress direc = IPAddress.Parse("147.83.117.22");
                    IPEndPoint ipep = new IPEndPoint(direc, 50063);
                    //Creamos el socket 
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //Intentamos conectar el socket
                    server.Connect(ipep);

                    //Pongo en marcha el thread que atendrá los mensajes del servidor
                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();

                    // Enviamos al servidor el nombre tecleado con la nueva contraseña
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("1/0/" + Usuario.Text + "/" + Contraseña.Text);
                    server.Send(msg);
                }

                else
                {
                    MessageBox.Show("Error al introducir los datos, asegurate que has rellenado los campos correctamente");
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
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("3/0/");
                    server.Send(msg);
                }

                else if (Ganadores.Checked && NombreConsulta.Text !="")
                {
                    // Quiere saber quien ha ganado contra una persona
                    string mensaje = ("4/0/" + NombreConsulta.Text);
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }

                else if (Viciado.Checked)
                {
                    // Quiere saber quien ha jugado mas partidas
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
            //Preparamos el Grid donde se mostraran los usuarios conectados
            ConectadosGrid.Columns.Add("Usuarios conectados", "Usuarios conectados");
            ConectadosGrid.ColumnHeadersVisible = true;
            ConectadosGrid.RowHeadersVisible = false;
            ConectadosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            ConectadosGrid.Columns["Usuarios conectados"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ConectadosNum = 0;
            ActivarOperacionesIniciales();
        }

        public void ActivarOperaciones()
        {
            //Ocultamos las operaciones que ya no se pueden realizar
            UsuarioLbl.Visible = false;
            PswLbl.Visible = false;
            Usuario.Visible = false;
            Contraseña.Visible = false;
            Login.Visible = false;
            Registro.Visible = false;
            InfoLbl.Visible = false;
            PswVisible.Visible = false;
            
            //Activa las que ahora si se pueden
            Eliminar.Visible = true;
            ConectadosGrid.Visible = true;
            Invitar.Visible = true;
            Desconectar.Visible = true;
            Rapido.Visible = true;
            Ganadores.Visible = true;
            Viciado.Visible = true;
            Consultar.Visible = true;
            NombreLbl.Visible = true;
            RespuestasBox.Visible = true;
            NombreConsulta.Visible = true;

            this.BackColor = Color.PaleGreen;
        }

        public void ActivarOperacionesIniciales()
        {
            //Ocultamos las operaciones que no se pueden realizar
            Eliminar.Visible = false;
            ConectadosGrid.Visible = false;
            Invitar.Visible = false;
            Desconectar.Visible = false;
            Rapido.Visible = false;
            Ganadores.Visible = false;
            Viciado.Visible = false;
            Consultar.Visible = false;
            NombreLbl.Visible = false;
            RespuestasBox.Visible = false;
            NombreConsulta.Visible = false;
            //Activa las que si se pueden
            UsuarioLbl.Visible = true;
            PswLbl.Visible = true;
            Usuario.Visible = true;
            Contraseña.Visible = true;
            Login.Visible = true;
            Registro.Visible = true;
            PswVisible.Visible = true;
            InfoLbl.Visible = true;
            InfoLbl.Text = "";

            this.BackColor = Color.PowderBlue;
        }
        private void ConectadosGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {//Recoge los nombres de los jugadores que quiere invitar a la partida
            if (ConectadosGrid.CurrentCell.Value != null)
            {
                string selecionado = ConectadosGrid.CurrentCell.Value.ToString();//Recoge el nombre de la celda

                if (selecionado != nombre)//Solo permitimos selecionar las que no son el propio usuario
                {
                    if (numSelecionados == 3)
                        MaxLbl.Text = "Has alcanzado el maximo permitido de jugadores \n            Empezar partida clicando a Invitar";

                    else if ((selecionado != jugadores[0]) && (selecionado != jugadores[1]) && (selecionado != jugadores[2]))//Si aun no la hemos selecionado la añadimos
                    {
                        jugadores[numSelecionados] = selecionado;
                        numSelecionados++;
                        MaxLbl.Text = "";
                    }
                    else //Deselecionamos al jugador
                    {
                        bool encontrado = false;
                        for (int i = 0; i < numSelecionados; i++)
                        {
                            if (jugadores[i] == selecionado)
                                encontrado = true;
                            if (encontrado)
                                jugadores[i] = jugadores[i + 1];
                        }
                        numSelecionados--;
                        MaxLbl.Text = "";
                    }
                    selecionado = "";
                    for (int i = 0; i < numSelecionados; i++)
                        selecionado = selecionado + jugadores[i] + ",";
                    InvitarLbl.Text = selecionado;
                }
            }

            
        }

        private void Invitar_Click(object sender, EventArgs e)
        {// Quiere invitar a jugar
             
            // Enviamos al servidor el nombre del usuario más sus invitados
            string mensaje = "6/0/"+nombre+",";
            if (numSelecionados > 0)
            {
                for (int i = 0; i < numSelecionados; i++)
                {
                    mensaje = mensaje + jugadores[i] + ",";
                }
                //MessageBox.Show(mensaje);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
                MaxLbl.Text = "Primero debes seleccionar a un jugador";

            //Limpiamos el vector de jugadores
            numSelecionados = 0;
            for (int i = 0; i < 3; i++)
                jugadores[i] = "";
            InvitarLbl.Text ="";
            MaxLbl.Text = "";
        }

        private void Eliminar_Click(object sender, EventArgs e)
        {
            string mensaje = "9/0/"+nombre;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            ActivarOperacionesIniciales();
        }

        private void PswVisible_CheckedChanged(object sender, EventArgs e)
        {
            //evento decorativo que permita ocultar o visualizar la contraseña mientras se inserta en el textbox password
            {
                if (PswVisible.Checked == true)
                    Contraseña.PasswordChar = '\0';
                    
                else
                    Contraseña.PasswordChar = '✿';
            }
        }

        
    }
}
