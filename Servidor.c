#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#include <time.h>

//FUNCIONES Y ESTRUCTURAS PARA LA LISTA DE CONECTADOS

//Estructura para la lista de conectados
typedef struct{
	char nombre[20];
	int socket;
} Conectado;

typedef struct{
	Conectado conectados[100];
	int num;
} ListaConectados;


/*Funcion que pone un usuario que se conecta a la lista de conectados. 
Devuelve -2 si el usuario ya esta en la lista, -1 si la lista esta llena 
y 0 si se ha introducido correctamente*/
int Pon (ListaConectados *lista, char nombre[20], int socket){
	if (lista->num == 100)
		return -1;
	for(int j=0;j<lista->num;j++){ 
		if(strcmp(lista->conectados[j].nombre,nombre)==0)
			return -2;
	}
	strcpy (lista->conectados[lista->num].nombre, nombre);
	lista->conectados[lista->num].socket = socket;
	printf("Socket %d de %s introducido en la posicion %d\n",socket,nombre,lista->num);
	lista->num++;
	return 0;
}

/*Funcion que devuelve la posicion que ocupa en la lista de conectados. 
Devuelve -1 si no esta en la lista*/
int DamePosicion (ListaConectados *lista, char nombre[20]){
	for (int i =0; i<lista->num;i++){
		if (strcmp(lista->conectados[i].nombre,nombre) == 0)
			return i;
	}
	return -1;
}

/*Funcion que devuelve el numero de socket de un usuario. 
Devuelve -1 si no esta en la lista*/
int DameSocket (ListaConectados *lista, char nombre[20]){
	for (int i =0; i<lista->num;i++){
		if (strcmp(lista->conectados[i].nombre,nombre) == 0)
			return lista->conectados[i].socket;
	}
	return -1;
}

/*Funcion que elimina un usuario y recoloca a los otros. 
Devuelve -1 si no esta en la lista y 0 si se ha eliminado correctamente.*/
int Eliminar (ListaConectados *lista, char nombre[20]){
	int pos = DamePosicion(lista,nombre);
	if (pos == -1)
		return -1;
	for (int i = pos;i<lista->num-1;i++){
		lista->conectados[i]=lista->conectados[i+1];
	}
	lista->num--;
	return 0;
}

//Funcion que retorna un string con todos los conectados de la lista conectados, separados por comas.
void DameConectados (ListaConectados *lista, char conectados[300]){
	sprintf(conectados,"%d",lista->num);
	for (int j =0; j<lista->num;j++){
		sprintf (conectados,"%s,%s",conectados,lista->conectados[j].nombre);
	}
}

/*Funcion que devuelve una lista con los sockets de los jugadores
entrados por la lista. Pon -1 si el jugador no esta en la lista*/
void SocketsJugadores (ListaConectados *lista, char jugadores[300], char sockets[300]){
	char *p = strtok(jugadores,",");
	while(p!=NULL){
		if(DameSocket (lista,p) !=-1)
			sprintf(sockets,"%s%d,",sockets,DameSocket (lista,p));
		else
			sprintf(sockets,"%s-1,",sockets);
		p = strtok(NULL,",");
	}
	sockets[strlen(sockets)-1]=NULL;
}

//Estructura para las partidas

typedef struct{
	Conectado PartidaJugadores[4];//Lista con los jugadores
	int Respuesta[4];//Respuesta de los jugadores (1->YES o 0->NO)
	int numRespuesta;
	int num;//Numero de jugadores en la partida
	int IDpartida;
	int ListaCartas[12];
	int CartasJugadas;
	char TiempoInicial[128];
	int Active;
	int SegundosIniciales;
	int Nivel;
} Partida;

typedef struct{
	Partida ListaPartidas[100];
	int numPartidas;//Total de partidas en la tabla
}TablaPartidas;


//Funcion que elimina de la partida a los jugadores que rechazaron la invitacion
void EliminarJugadores(TablaPartidas *tabla, int IDpartida){
	
	for (int j=1;j<tabla->ListaPartidas[IDpartida].num;j++)
	{
		if(tabla->ListaPartidas[IDpartida].Respuesta[j]==0)
		{
			for(int i=j;i<tabla->ListaPartidas[IDpartida].num-1;i++){
				tabla->ListaPartidas[IDpartida].PartidaJugadores[i]=tabla->ListaPartidas[IDpartida].PartidaJugadores[i+1];
			}
			tabla->ListaPartidas[IDpartida].num--;
		}
	}
}

/*Funcion que incluye las respuestas de la peticion de partida. 
Devuelve el numero de respuesta hasta este momento*/
int IncluirRespuesta(TablaPartidas *Tabla, int respuesta, int IDpartida, char nombre[10]){
	int i=0;
	int encontrado=0;
	while(encontrado==0){	//Primero buscamos la posicion del jugador dentro de la lista
		if (strcmp(Tabla->ListaPartidas[IDpartida].PartidaJugadores[i].nombre,nombre)==0)
			encontrado=1;
		else
			i++;
	}
	//Ponemos la respuesta a la lista
	Tabla->ListaPartidas[IDpartida].Respuesta[i]=respuesta;
	Tabla->ListaPartidas[IDpartida].numRespuesta++;
	return Tabla->ListaPartidas[IDpartida].numRespuesta;
}

/*Funcion que crea una partida con los jugadores invitados y el anfitrion. 
La funcion devuelve el numero de partida en la tabla de partidas.*/
int CrearPartida (ListaConectados *lista, char jugadores[300], TablaPartidas *Tabla){
	char *p =strtok( jugadores,",");
	int Posicion;
	Partida partida;
	partida.Active=0;
	partida.num=0;
	partida.Nivel=1;
	partida.CartasJugadas=0;
	while (p!=NULL){
		Posicion =DamePosicion (lista, p);
		if (Posicion !=-1)
		{
			partida.PartidaJugadores[partida.num] = lista->conectados[Posicion];
			partida.num++;
		}
		p =strtok( NULL,",");
	}
	if(partida.num>=2){//Ponemos la partida en la tabla
		partida.IDpartida= Tabla->numPartidas;
		Tabla->ListaPartidas[Tabla->numPartidas]= partida;
		Tabla->ListaPartidas[Tabla->numPartidas].numRespuesta=0;
		Tabla->numPartidas++;
		printf("Partida %d creada",partida.IDpartida);
		if(Tabla->numPartidas==100)
			Tabla->numPartidas=0;
	}
	return partida.num;
}

/*Funcion que nos da la fecha y hora en el momento en el que se incia una partida y 
lo inserta en el apartado de SegundosIniciales de la ListaPartidas */
void InsertarTiempoInicial(TablaPartidas *tabla,int id){
	time_t temps = time(0);
	struct tm *tlocal=localtime(&temps);
	char output[128];
	strftime(output,128,"%d-%m-%y %H:%M:%S",tlocal);
	strcpy(tabla->ListaPartidas[id].TiempoInicial,output);
	char Hora[128];
	char Min[128];
	char Seg[128];
	strftime(Hora,128,"%H",tlocal);
	strftime(Min,128,"%M",tlocal);
	strftime(Seg,128,"%S",tlocal);	
	tabla->ListaPartidas[id].SegundosIniciales= atoi(Hora)*3600+atoi(Min)*60+atoi(Seg);
	
}

/*Funcion que retorna en un vector la cantidad de cartas pedidas*/
void CartasAleatorias (int numeroCartas, int vector[12]){
	int num;
	srand(time(NULL));
	for (int y = 0; y <numeroCartas; y++)
	{
		num = rand() % 100 +1;//Numero aleatorio del 1 al 100
		int t = 0;
		while (t<=y)//Comprobamos que es diferente a los ya cogidos
		{
			if (vector[t] == num){
				num = rand() % 100 +1;
				t=0;
			}
			else
				t++;
		}
		vector[y]=num;
		printf("carta %d\n",vector[y]);
	}
}

/*Funcion que ordena las cartas de forma ascendente que se estan jugando en la partida*/
void OrdenarCartas(TablaPartidas *tabla,int Cartas[12], int idPartida){
	printf("ORDENAR CARTAS\n");
	int longitud = (tabla->ListaPartidas[idPartida].num*tabla->ListaPartidas[idPartida].Nivel);
	for(int i=0; i<longitud-1;i++)
	{
		for (int j=i+1; j<longitud;j++){
			if (Cartas[j]<Cartas[i]){
				int aux=Cartas[j];
				Cartas[j]=Cartas[i];
				Cartas[i]=aux;
			}
		}
		tabla->ListaPartidas[idPartida].ListaCartas[i]=Cartas[i];
		printf("-->CARTAS %d\n",tabla->ListaPartidas[idPartida].ListaCartas[i]);
	}
	tabla->ListaPartidas[idPartida].ListaCartas[longitud-1]=Cartas[longitud-1];
	printf("-->CARTAS %d\n",tabla->ListaPartidas[idPartida].ListaCartas[longitud-1]);
}

/*Funcion que comprueba si la carta jugada es la correcta.
Devuelve 1 si se han equivocado y por lo tanto la partida ha finalizado,
0 si la carta es la correcta o -1 si se ha finalizado la partida*/
int Jugada(TablaPartidas *tabla,int Carta, int idPartida){
	printf("Indice de partida: %d\n", idPartida);
	printf("Valor 0 del vector cartas: %d\n", tabla->ListaPartidas[idPartida].ListaCartas[0]);
	printf("%d\n",tabla->ListaPartidas[idPartida].CartasJugadas);
	printf("%d vs %d\n",Carta,tabla->ListaPartidas[idPartida].ListaCartas[tabla->ListaPartidas[idPartida].CartasJugadas]);
	
	if(Carta == tabla->ListaPartidas[idPartida].ListaCartas[tabla->ListaPartidas[idPartida].CartasJugadas])
	{
		tabla->ListaPartidas[idPartida].CartasJugadas++;
		if ((tabla->ListaPartidas[idPartida].num*tabla->ListaPartidas[idPartida].Nivel)==tabla->ListaPartidas[idPartida].CartasJugadas)//Fin de la partida
			return -1;//Han ganado
		else
			return 0;
	}
	else//Partida perdida
		return 1;
}

/* Funcion que devuelve en un vector, los sockets de los participantes de una partida */
void DameSocketParticipantes(TablaPartidas *tabla, int ID, char sockets[50]){
	for(int i=0;i>tabla->ListaPartidas[ID].num;i++){
		sprintf(sockets,"%s,%s",sockets,tabla->ListaPartidas[ID].PartidaJugadores[i].socket);
	}
}


//VARIABLES GLOBLES
//Estructura necesaria para acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;// Numero de sockets en el vector
int sockets[100];

ListaConectados lista;//Lista con todos los conectados
TablaPartidas Partidas;//Lista con las partidas

/* Funcion para atender al cliente */
void *AtenderCliente (void *socket){
	
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	int terminar =0;
	char peticion[512];
	char respuesta[512];
	int ret;
	char *usuario[20];
	char *password[20];
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas 
	//Entramos en un bucle para atender todas las peticiones de estecliente
	//hasta que se desconecte
	conn = mysql_init(NULL);
	if (conn==NULL){
		
		printf ("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "bd",0, NULL, 0);
	if (conn==NULL){
		
		printf ("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	while (terminar ==0){
		// Ahora recibimos la peticion
		ret=read(sock_conn,peticion, sizeof(peticion));
		
		// Tenemos que poner la marca de fin de string para que no escriba lo que hay despues en el peticion
		peticion[ret]='\0';
		
		//Escribimos el nombre en la consola
		printf ("Recibido\n");
		printf ("Se ha conectado: %s\n",peticion);
		//Obtenemos el codigo de la peticion
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		int numform;
		int Modificacion =0;
		
		if (codigo == 0){//Cerar conexion y eliminar usuario de la lista
			terminar=Desconectar (usuario);
			Modificacion=1;
			PartidaActiva(usuario);
		}
		else{
			p=strtok(NULL, "/");
			numform = atoi(p);
			if (codigo ==1){ //Registrar usuario
				p=strtok(NULL,"/");
				strcpy (usuario,p);
				p=strtok(NULL,"/");
				strcpy(password,p);
				terminar =Registrar(usuario,password,respuesta,conn,sock_conn);
				if (terminar ==0)
					Modificacion=1;
			}
			else if (codigo ==2){ //Login
				p=strtok(NULL,"/");
				strcpy (usuario,p);
				p=strtok(NULL,"/");
				strcpy(password,p);
				terminar =Login(usuario,password,respuesta,conn,sock_conn);
				if (terminar ==0)
					Modificacion=1;
			}
			else if (codigo ==3){ //Consulta de quien ha sido el mas rapido en ganar
				MasRapido(respuesta,conn);
			}
			else if (codigo == 4){ //Consulta de quien ha ganado con una persona
				p=strtok(NULL, "/");
				char nombreConsulta[20];
				strcpy(nombreConsulta, p);
				GanaronCon(respuesta,conn,nombreConsulta);
			}
			else if (codigo ==5){ //Consulta de quien ha sido quien mï¿¡s ha jugado
				Viciado(respuesta,conn);
			}
			else if (codigo ==6){ //Invitar a jugar
				char *InvitadosJuego[300];
				p=strtok(NULL,"/");
				strcpy(InvitadosJuego,p);
				Invitar(InvitadosJuego,sock_conn);
			}
			else if (codigo ==7){ //Respuestas de invitaciï¿³n
				p=strtok(NULL,"/");
				RespuestaInvitacion(p);
			}
			else if (codigo ==8){ //Mensajes de chat
				p=strtok(NULL,"/");
				NuevoMensaje(p, numform);
				
			}
			else if (codigo ==9){//Eliminamos el jugador de la base de datos
				p=strtok(NULL,"/");
				strcpy (usuario,p);
				printf("%s\n",usuario);
				terminar=Desconectar(usuario);
				PartidaActiva(usuario);
				terminar =EliminarUsuario(usuario,conn,sock_conn);
				if (terminar==1)
					Modificacion=1;
			}
			else if (codigo ==10){//Se ha jugado una carta
				char jugador[20];
				p=strtok(NULL,"/");
				strcpy(jugador,p);
				p=strtok(NULL,"/");
				int carta;
				carta=atoi(p);
				printf("Jugador %s juega carta %d %d\n",jugador,carta,numform);
				JugarCarta(carta,jugador,numform,conn);
			}
			if (codigo ==1 || codigo ==2 || codigo ==3 || codigo ==4 || codigo ==5){
				write (sock_conn,respuesta, strlen(respuesta));
			}
		}
		if (Modificacion==1)
		{
			Conectados ();
			Modificacion=0;
		}
	}
	// Se acabo el servicio para este cliente y se desconecta
	close(sock_conn);
}

//Funcion para borrar al cliente de la lista y cerrar la conexion. Devuele 1 si todo OK.
int Desconectar (char usuario[20]){
	int res =DamePosicion (&lista, usuario);
	if (res != -1) //Si esta en la lista lo eliminamos
	{
		pthread_mutex_lock (&mutex);
		Eliminar(&lista,usuario);
		pthread_mutex_unlock (&mutex);
	}
	return 1;
}

//Funcion para registrar al cliente
int Registrar (char usuario[20],char password[20], char respuesta[100],MYSQL *conn,int sock_conn){
	//Si el usuario no existe lo ponemos en la base de datos
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[100];
	
	sprintf (consulta, "INSERT INTO Jugador VALUES (NULL,'%s','%s',0)",usuario,password);
	int err = mysql_query(conn, consulta);
	if (err!=0){
		printf ("USUARIO YA EXISTE Error al introducir datos la base %u %s\n",mysql_errno(conn), mysql_error(conn));
		printf("El usuario %s ya existe\n", usuario);
		strcpy(respuesta,"1/0/1");
		return 1;
		
	}
	else{
		pthread_mutex_lock (&mutex);
		int res = Pon (&lista,usuario,sock_conn);
		pthread_mutex_unlock (&mutex);
		if (res==-1){
			printf("No se pueden poner mas usuarios\n");
			strcpy(respuesta,"1/0/2");
			return 1;
		}
		else{
			printf ("Login correct\n");	
			strcpy(respuesta,"1/0/0");
			return 0;
		}
	}
}

//Funcion para poder acceder al programa una vez registrado en la base de datos
int Login (char usuario[20],char password[20],char respuesta[100],MYSQL *conn,int sock_conn){
	//Comprobamos si el usuario y password son correctos. Si lo son los ponemos en la lista
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[100];
	
	sprintf (consulta,"SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Usuario = '%s' AND Jugador.Psw ='%s';",usuario,password);
	printf("%s\n",consulta);
	int err=mysql_query (conn, consulta);
	if (err!=0){
		
		printf ("Error 1 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		//Si nos debuelve una tabla vacia significa que el usuario no esta registrado
		printf ("Usuario no existe\n");
		strcpy(respuesta,"2/0/1");
	}
	else{		
		pthread_mutex_lock (&mutex);
		printf("Voy a guardar socket %d\n",sock_conn);
		int res = Pon (&lista,usuario,sock_conn);
		pthread_mutex_unlock (&mutex);
		if (res==-1){
			printf("No se puede añadir mas usuarios\n");
			strcpy(respuesta,"2/0/2");
			return 1;
		}
		else if (res==-2){
			printf("Tiene una sesion \n");
			strcpy(respuesta,"2/0/3");
			return 1;
		}
		else{
			printf ("Login correct\n");	
			strcpy(respuesta,"2/0/0");
			return 0;
		}
	}
}

//Funcion que permite al cliente borrarse de la base de datos
int EliminarUsuario( char usuario[20],MYSQL *conn,int sock_conn){
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[100];
	char respuesta[512];
	sprintf (consulta,"DELETE FROM Jugador WHERE Jugador.Usuario = '%s';",usuario);
	
	int err=mysql_query (conn, consulta);
	if (err!=0){
		
		printf ("Error 1 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	
	printf ("Usurio eliminado de la base de dadas\n");
	pthread_mutex_lock (&mutex);
	Eliminar (&lista,usuario);
	pthread_mutex_unlock (&mutex);
	strcpy(respuesta,"10/0/0");
	write (sock_conn,respuesta, strlen(respuesta));
	return 1;
}

//Consulta para que de la persona que menos ha tardado en ganar una partida
void MasRapido (char respuesta[100],MYSQL *conn){
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err=mysql_query (conn, "SELECT Partida.ID,Jugador.Usuario FROM Jugador,Partida,Participantes WHERE Partida.Time = (SELECT MIN(Partida.Time) FROM Partida WHERE Partida.Winner=1) AND Participantes.ID_P = Partida.ID AND Jugador.ID = Participantes.ID_J ORDER BY Partida.ID");
	if (err!=0){
		
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	char nombre[20];
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		
		printf ("No se han obtenido datos en la consulta\n");
	}
	else{
		if (row !=NULL){
			printf("La partida mas rapida ha sido: %s\n", row[0]);
			sprintf(respuesta,"3/0/La partida mas rapida ganada ha sido la %s y jugaron %s",row[0],row[1]);
			int partidaID= atoi(row[0]);
			row = mysql_fetch_row (resultado);
			int fin=0;
			
			while ((fin==0) && (row!=NULL)){
				sprintf(respuesta,"%s, %s",respuesta,row[1]);
				if(atoi(row[0])==partidaID){
					fin=1;
				}
				row = mysql_fetch_row (resultado);
				
			}
			
		}
		printf("%s\n",respuesta);
	}
}

//CONSULTA PARA QUE DE LA LISTA DE PERSONAS QUE GANARON UNA PARTIDA DONDE JUGARON CON UNA PERSONA INTRODUCIDA POR EL CLIENTE
void GanaronCon (char respuesta[512],MYSQL *conn, char nombreConsulta[20]){
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[512];
	sprintf(consulta,"SELECT DISTINCT Partida.Data, Jugador.Usuario,Partida.ID FROM Jugador,Partida,Participantes WHERE Partida.ID IN (SELECT Partida.ID FROM Partida,Jugador,Participantes WHERE Jugador.Usuario='%s' AND Jugador.ID=Participantes.ID_J AND Participantes.ID_P=Partida.ID AND Partida.Winner=1) AND Partida.ID= Participantes.ID_P AND Participantes.ID_J= Jugador.ID AND Jugador.Usuario != '%s' ORDER BY Partida.ID ", nombreConsulta,nombreConsulta); 
	printf("%s\n", consulta);
	int err=mysql_query (conn, consulta);
	if (err!=0){
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		strcpy(respuesta, "4/0/No se han obtenido datos. Porfavor, asegurate de que el usuario existe");
	}
	else{
		strcpy(respuesta, "4/0/");
		printf("RESULTADO CONSULTA %s %s\n",row[0],row[1]);
		int cont=0;//Solo se enviaran 5 partidas
		while (row !=NULL && cont<5) {
			
			sprintf(respuesta, "%sEn la partida jugada el %s estuvieron %s",respuesta,row[0],row[1]);
			printf("RESPUESTA%s \n",respuesta);
			char Data[30];
			strcpy(Data,row[0]);
			row = mysql_fetch_row (resultado);
			int siguiente=0;
			while ((siguiente==0) && (row!=NULL)){
				if(strcmp(Data,row[0])==0){
					sprintf(respuesta, "%s, %s",respuesta,row[1]);
					printf("RESPUESTA%s \n",respuesta);
					printf("RESULTADO CONSULTA %s %s\n",row[0],row[1]);
				}
				else{
					siguiente=1;
				}
				row = mysql_fetch_row (resultado);
			}
			sprintf(respuesta, "%s*",respuesta);
			printf("RESPUESTA%s \n",respuesta);
			cont++;
		}
		printf("RESPUESTA%s \n",respuesta);
	}
}

//Consulta para que de la persona que mas veces a jugado
void Viciado (char respuesta [100],MYSQL *conn){
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err=mysql_query (conn, "SELECT Jugador.Usuario, COUNT(Participantes.ID_P) as Recuento FROM Participantes,Jugador WHERE Jugador.ID = Participantes.ID_J GROUP BY Participantes.ID_J ORDER BY Recuento DESC");
	if (err!=0){
		
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	char nombre[20];
	int partidas;
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		
		printf ("No se han obtenido datos en la consulta\n");
	}
	else{
		strcpy(nombre,row[0]);
		partidas=atoi(row[1]);
		
		printf("El Jugador que mas partidas ha jugado es: %s\n", nombre);
		sprintf(respuesta,"5/0/%s,%d",nombre,partidas);
	}
}

//Funcion que envia a todos los clientes conectados una lista con todos los jugadores conectados. 
void Conectados ()
{
	char notificacion[512];
	char Jugadores[300];
	DameConectados (&lista, Jugadores);
	printf("%s\n",Jugadores);
	sprintf(notificacion,"6/0/%s",Jugadores);
	printf("%s\n",notificacion);
	for(int j=0;j<lista.num;j++){
		write (lista.conectados[j].socket,notificacion, strlen(notificacion));
	}
}

//Funcion que permite al cliente anfitrion enviar solicitud de partida a otros usuarios
void Invitar (char jugadores[300],int sock_conn){
	char notificacion[512];
	char *players[300];
	strcpy(players,jugadores);
	int numJugadores =CrearPartida (&lista, jugadores,&Partidas);
	if (numJugadores<2){
		strcpy(notificacion,"7/0/-1");
		write (sock_conn,notificacion, strlen(notificacion));
	}
	else{
		sprintf(notificacion,"7/0/%d,%s",Partidas.numPartidas-1,players);
		for(int i=1;i<numJugadores;i++){
			write (Partidas.ListaPartidas[Partidas.numPartidas-1].PartidaJugadores[i].socket,notificacion, strlen(notificacion));
		}
	}
}

//Funcion que recoje las respuestas a las invitaciones a jugar. Si se cumplen los requisitos para poder jugar se envia 8/0/1 y sino 8/0/0
void RespuestaInvitacion(char respuesta[300]){
	char notificacion[512];
	char notificacion2[512];
	printf("Respuesta %s\n",respuesta);
	char *p= strtok(respuesta,",");
	int IDpartida = atoi(p);//Recogemos el ID
	printf("ID %d\n",IDpartida);
	p= strtok(NULL,",");
	char jugador[10];
	strcpy(jugador,p);//Recogemos el nombre
	p= strtok(NULL,",");
	if (strcmp(p,"YES")==0)//Recogemos respuesta
		respuesta=1;
	else
		respuesta=0;
	printf("ID partida %d jugador %s respuesta %d\n",IDpartida,jugador,respuesta);
	int res = IncluirRespuesta(&Partidas, respuesta, IDpartida, jugador);
	
	
	if(res == Partidas.ListaPartidas[IDpartida].num-1){
		Partidas.ListaPartidas[IDpartida].Active=1;
		EliminarJugadores(&Partidas, IDpartida);
		printf("RES %d\n",Partidas.ListaPartidas[IDpartida].num -1);
		if (Partidas.ListaPartidas[IDpartida].num>1){
			sprintf(notificacion,"8/0/1,%d-%d",IDpartida, Partidas.ListaPartidas[IDpartida].num);
			for(int i=0;i<Partidas.ListaPartidas[IDpartida].num;i++){
				sprintf(notificacion, "%s,%s", notificacion, Partidas.ListaPartidas[IDpartida].PartidaJugadores[i].nombre);
			}
			int vectorCartas[Partidas.ListaPartidas[IDpartida].num];//Nivel 1 (una carta por jugador)
			CartasAleatorias (Partidas.ListaPartidas[IDpartida].num, vectorCartas);
			
			for(int i=0; i<Partidas.ListaPartidas[IDpartida].num;i++)
			{
				char mensajeCartas[512];
				sprintf(mensajeCartas,"%s-%d",notificacion,vectorCartas[i]);
				
				printf("Este es el mensaje %s\n",mensajeCartas);
				write (Partidas.ListaPartidas[IDpartida].PartidaJugadores[i].socket,mensajeCartas, strlen(mensajeCartas));	
			}
			OrdenarCartas(&Partidas,vectorCartas,IDpartida);
			InsertarTiempoInicial(&Partidas,IDpartida);
			
		}
		else{
			sprintf(notificacion,"8/0/0,%d",IDpartida);
			write (Partidas.ListaPartidas[IDpartida].PartidaJugadores[0].socket,notificacion, strlen(notificacion));
		}
	}
}

//Funcion para enviar los mensajes que se muestran en el chat
void NuevoMensaje(char mensaje[100], int idPartida){
	printf("Respuesta %s\n",mensaje);
	char notificacion[512];
	sprintf(notificacion, "9/%d/%s", idPartida, mensaje);
	for(int i=0; i<Partidas.ListaPartidas[idPartida].num;i++){
		write (Partidas.ListaPartidas[idPartida].PartidaJugadores[i].socket,notificacion, strlen(notificacion));
	}
}

//Funcion para poder enviar una carta jugada al resto de participantes de la partida. Una tirada
void JugarCarta(int carta,char jugador[20],int idPartida,MYSQL *conn){
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[100];
	char consulta2[100];
	int err;
	
	pthread_mutex_lock (&mutex);
	int res=Jugada(&Partidas,carta,idPartida);
	pthread_mutex_unlock (&mutex);
	
	char notificacion[512];
	if (res==-1 && Partidas.ListaPartidas[idPartida].Nivel!=3)//Pasan al siguiente nivel
	{
		res=0;
		sprintf(notificacion, "13/%d/", idPartida);
		Partidas.ListaPartidas[idPartida].Nivel++;
		int vectorCartas[Partidas.ListaPartidas[idPartida].num*Partidas.ListaPartidas[idPartida].Nivel];
		CartasAleatorias (Partidas.ListaPartidas[idPartida].num*Partidas.ListaPartidas[idPartida].Nivel, vectorCartas);
		Partidas.ListaPartidas[idPartida].CartasJugadas=0;
		int cont=0;
		if(Partidas.ListaPartidas[idPartida].Nivel==2){//Se envian dos cartas por jugador
			for(int i=0; i<Partidas.ListaPartidas[idPartida].num;i++)
			{
				char mensajeCartas[512];
				sprintf(mensajeCartas,"%s%d,%d",notificacion,vectorCartas[cont],vectorCartas[cont+1]);
				
				printf("Este es el mensaje %s\n",mensajeCartas);
				write (Partidas.ListaPartidas[idPartida].PartidaJugadores[i].socket,mensajeCartas, strlen(mensajeCartas));	
				cont=cont+2;
			}
		}
		else{
			for(int i=0; i<Partidas.ListaPartidas[idPartida].num;i++)
			{
				char mensajeCartas[512];
				sprintf(mensajeCartas,"%s%d,%d,%d",notificacion,vectorCartas[cont],vectorCartas[cont+1],vectorCartas[cont+2]);
				
				printf("Este es el mensaje %s\n",mensajeCartas);
				write (Partidas.ListaPartidas[idPartida].PartidaJugadores[i].socket,mensajeCartas, strlen(mensajeCartas));	
				cont=cont+3;
			}
		}
		pthread_mutex_lock (&mutex);
		OrdenarCartas(&Partidas,vectorCartas,idPartida);
		pthread_mutex_unlock (&mutex);
	}
	else{
		sprintf(notificacion, "11/%d/%d,%s,%d", idPartida,res,jugador,carta);
		printf("%s\n",notificacion);
		
		for(int i=0; i<Partidas.ListaPartidas[idPartida].num;i++){
			write (Partidas.ListaPartidas[idPartida].PartidaJugadores[i].socket,notificacion, strlen(notificacion));
		}
	}
	if ((res==1 || (res==-1 && Partidas.ListaPartidas[idPartida].Nivel==3)) && Partidas.ListaPartidas[idPartida].Active!=0){//Fin de la partida
		
		pthread_mutex_lock (&mutex);
		Partidas.ListaPartidas[idPartida].Active=0;
		pthread_mutex_unlock (&mutex);
		int ganar;
		int IDJugadores[Partidas.ListaPartidas[idPartida].num];
		if(res==1){//Han perdido
			ganar=0;
		}
		else {//Jugadores han ganado
			ganar=1;
			sprintf(consulta,"UPDATE Jugador SET Jugador.Puntos=Jugador.Puntos+100 WHERE Jugador.Usuario = '%s'",Partidas.ListaPartidas[idPartida].PartidaJugadores[0].nombre);
			for (int i=1;i<Partidas.ListaPartidas[idPartida].num;i++){
				sprintf(consulta,"%s OR Jugador.Usuario ='%s'",consulta,Partidas.ListaPartidas[idPartida].PartidaJugadores[i].nombre);
			}
			err=mysql_query (conn, consulta);
			if(err!=0){
				printf ("Error 2 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
		}
		//Tiempo en que finaliza la partida
		time_t temps = time(0);
		struct tm *tlocal=localtime(&temps);
		char Hora[128];
		char Min[128];
		char Seg[128];
		strftime(Hora,128,"%H",tlocal);
		strftime(Min,128,"%M",tlocal);
		strftime(Seg,128,"%S",tlocal);	
		int Duracion;
		Duracion= atoi(Hora)*3600+atoi(Min)*60+atoi(Seg)-Partidas.ListaPartidas[idPartida].SegundosIniciales;
		
		//Insertamos datos en la base de dadas
		//Buscamos el ID de los jugadores
		sprintf(consulta,"SELECT Jugador.ID FROM Jugador WHERE Jugador.Usuario = '%s'",Partidas.ListaPartidas[idPartida].PartidaJugadores[0].nombre);
		for (int i=1;i<Partidas.ListaPartidas[idPartida].num;i++){
			sprintf(consulta,"%s OR Jugador.Usuario = '%s'",consulta,Partidas.ListaPartidas[idPartida].PartidaJugadores[i].nombre);
		}	
		err=mysql_query (conn, consulta);
		printf("Consulta de las ID %s\n",consulta);
		if(err!=0){
			printf ("Error 2 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		for (int i=0;row!=NULL;i++){
			IDJugadores[i]=atoi(row[0]);
			row = mysql_fetch_row (resultado);
			printf("ID %d\n",IDJugadores[i]);
		}
		
		char Data[128];
		strcpy(Data,Partidas.ListaPartidas[idPartida].TiempoInicial);
		pthread_mutex_lock (&mutex);	//Bloqueamos para poder sacar el ID de la partida sin que se anada otra	
		sprintf (consulta, "INSERT INTO Partida VALUES (NULL,'%s',%d,%d)",Data,Duracion,ganar);
		
		err = mysql_query(conn, consulta);
		if (err!=0){	
			printf ("Error al introducir la nueva partida datos la base %u %s\n",mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
		sprintf(consulta,"SELECT Partida.ID FROM Partida ORDER BY Partida.ID DESC LIMIT 1");
		err = mysql_query(conn, consulta);
		
		if (err!=0){	
			printf ("Error al introducir la nueva partida datos la base %u %s\n",mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		pthread_mutex_unlock (&mutex);
		int ID= atoi(row[0]);
		printf("ID de la partida %s\n",row[0]);
		for (int i=0; i<Partidas.ListaPartidas[idPartida].num;i++){
			strcpy(Data,Partidas.ListaPartidas[idPartida].TiempoInicial);
			sprintf (consulta, "INSERT INTO Participantes VALUES (%d,%d)",IDJugadores[i],ID);
			printf("Ortra consultilla %s\n",consulta);
			err = mysql_query(conn, consulta);
			if (err!=0){
				printf ("Error 3 al introducir datos la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
		}
		
	}
}

/*En caso de que un jugador se desconecte del servidor en medio de una partida,
esta funcion permite informar a los demas usuarios i poder finalizar la
partida como una derrota */
void PartidaActiva(char usuario [20]){
	
	for(int i=0;i<100;i++){
		if(Partidas.ListaPartidas[i].Active == 1){
			int j;
			for(j=0;j<Partidas.ListaPartidas[i].num;j++){
				if(strcmp(Partidas.ListaPartidas[i].PartidaJugadores[j].nombre,usuario)==0){
					Partidas.ListaPartidas[i].Active=0;
					char notificacion[512];
					sprintf(notificacion,"12/%d/%s",i,usuario);
					for(int z =0;z<Partidas.ListaPartidas[i].num;z++){
						write (Partidas.ListaPartidas[i].PartidaJugadores[z].socket,notificacion, strlen(notificacion));
					}
				}
			}
		}
	}
}


int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char peticion[512];
	char respuesta[512];
	
	lista.num=0;
	Partidas.numPartidas=0;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0){
		
		printf("Error creant socket");
	}
	// Fem el bind al port
	
	memset(&serv_adr, 0, sizeof(serv_adr));// Inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la maquina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9010);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		
		printf ("Error al bind");
	}
	
	//La cola de peticiones pendientes no podra ser superior a 4
	if (listen(sock_listen, 4) < 0){
		
		printf("Error en el Listen");
	}
	pthread_t thread[100];
	for(;;i++){
		
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen,NULL,NULL);
		printf("He recibido conexion\n");
		
		//sock_conn es el socket que usaremos para este cliente
		sockets[i] = sock_conn;
		
		//Crear thread y decir lo que tiene que hacer
		pthread_create (&thread[i],NULL,AtenderCliente,&sockets[i]);
		
		//Reiniciamos el contador de sockets
		if (i==99)
			i=0;
	}
}
