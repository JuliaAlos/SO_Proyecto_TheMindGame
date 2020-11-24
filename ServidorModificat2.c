#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
int i;
int sockets[100];


//FUNCIONES Y ESTRUCTURAS PARA LA LISTA DE CONECTADOS

typedef struct{
	char nombre[20];
	int socket;
} Conectado;


typedef struct{
	Conectado conectados[100];
	int num;
} ListaConectados;


ListaConectados lista;

int Pon (ListaConectados *lista, char nombre[20], int socket){
	//Pon usuario a la lista de conectados
	if (lista->num == 100)//Lista llena
		return -1;
	for(int j=0;j<lista->num;j++){ 
		if(strcmp(lista->conectados[j].nombre,nombre)==0)//Usuario ya esta en la lista
			return -2;
	}
	strcpy (lista->conectados[lista->num].nombre, nombre);
	lista->conectados[lista->num].socket = socket;
	lista->num++;
	return 0;
	
}



int DamePosicion (ListaConectados *lista, char nombre[20]){
	//Si el usuario esta en la lista nos devulve su posicion en la lista
	for (int i =0; i<lista->num;i++){
		if (strcmp(lista->conectados[i].nombre,nombre) == 0)
			return i;
	}
	return -1;
}


int Eliminar (ListaConectados *lista, char nombre[20]){
	//Elimina el usuario y recoloca las usuarios
	int pos = DamePosicion(lista,nombre);
	if (pos == -1)
		return -1;
	for (int i=pos;i<lista->num-1;i++){
		lista->conectados[i]=lista->conectados[i+1];
	}
	lista->num--;
	return 0;
}


void DameConectados (ListaConectados *lista, char conectados[300]){
	//Retorna un string con todos los conectados
	sprintf(conectados,"%d",lista->num);
	for (int j =0; j<lista->num;j++){
		sprintf (conectados,"%s,%s",conectados,lista->conectados[j].nombre);
	}
}


void SocketsJugadores (ListaConectados *lista, char jugadores[300], char sockets[300]){
	//Devuleve el socket o -1 si no esta en la lista
	char *p = strtok(jugadores,",");
	while(p!=NULL){
		
		if(DamePosicion (lista,p) !=-1)
			sprintf(sockets,"%s%d,",sockets,lista->conectados[DamePosicion (lista,p)].socket);
		else
			sprintf(sockets,"%s-1,",sockets);
		p = strtok(NULL,",");
	}
	sockets[strlen(sockets)-1]=NULL;
}

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
		
		
		// Tenemos que aÃ±adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el peticion
		peticion[ret]='\0';
		
		//Escribimos el nombre en la consola
		printf ("Recibido\n");
		printf ("Se ha conectado: %s\n",peticion);
		//Obtenemos el codigo de la peticion
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		int Modificacion =0;
		
		
		if (codigo == 0){//Cerar conexion y eliminar usuario de la lista
			p=strtok(NULL,"/");
			strcpy (usuario,p);
			terminar=Desconectar (usuario);
			Modificacion=1;
		}
		else{
			
			if (codigo ==1){ //Registrar usuario
				p=strtok(NULL,"/");
				strcpy (usuario,p);
				p=strtok(NULL,"/");
				strcpy(password,p);
				terminar =Registrar(usuario,password,respuesta,conn);
				if (terminar ==0)
					Modificacion=1;
			}
			else if (codigo ==2){ //Login
				p=strtok(NULL,"/");
				strcpy (usuario,p);
				p=strtok(NULL,"/");
				strcpy(password,p);
				terminar =Login(usuario,password,respuesta,conn);
				if (terminar ==0)
					Modificacion=1;
			}
			else if (codigo ==3){
				MasRapido(respuesta,conn);
			}
			else if (codigo == 4){
				GanaronJoel(respuesta,conn);
			}
			else if (codigo ==5){
				Viciado(respuesta,conn);
			}
			write (sock_conn,respuesta, strlen(respuesta));
		}
		
		if (Modificacion==1)
		{
			Conectados ();
			Modificacion=0;
		}
			
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);
}


int Desconectar (char usuario[20]){
	//Borramos el cliente de la lista y cerramos conexion
	int res =DamePosicion (&lista, usuario);
	if (res != -1) //Si esta en la lista lo eliminamos
	{
		pthread_mutex_lock (&mutex);
		Eliminar(&lista,usuario);
		pthread_mutex_unlock (&mutex);
	}
	return 1;
}

int Registrar (char usuario[20],char password[20], char respuesta[100],MYSQL *conn,int sock_conn){
	//Si el usuario no existe lo añadimos a la base de datos
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[100];
	sprintf (consulta,"SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Usuario = '%s' AND Jugador.Psw ='%s';",usuario,password);
	printf ("%s",consulta);
	int err=mysql_query (conn, consulta);
	if (err!=0){
		
		printf ("Error 1 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		
		printf ("Usuario no existe\n");
		err=mysql_query (conn, "SELECT Jugador.ID FROM Jugador WHERE Jugador.ID = (SELECT MAX(Jugador.ID) FROM Jugador)");
		if(err!=0){
			
			printf ("Error 2 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		int ID=1;
		if (row != NULL){
			
			ID = atoi(row[0])+1;
		}
		sprintf (consulta, "INSERT INTO Jugador VALUES (%d,'%s','%s')",ID,usuario,password);
		err = mysql_query(conn, consulta);
		if (err!=0){
			
			printf ("Error 3 al introducir datos la base %u %s\n",mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
		pthread_mutex_lock (&mutex);
		int res = Pon (&lista,usuario,sock_conn);
		pthread_mutex_unlock (&mutex);
		if (res==-1){
			printf("No se puede añadir mas usuarios\n");
			strcpy(respuesta,"1/2");
			return 1;
		}
		else{
			printf ("Login correct\n");	
			strcpy(respuesta,"1/0");
			return 0;
		}
		
	}
	else{
		
		printf("El usuario %s ya existe\n", row[0]);
		strcpy(respuesta,"1/1");
		return 1;
	}
}


int Login (char usuario[20],char password[20],char respuesta[100],MYSQL *conn,int sock_conn){
	//Comprobamos si el usuario y contraseÃ±a son correctos. Si lo es lo aÃ±adimos 
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
		strcpy(respuesta,"1/1");
	}
	else{		
		pthread_mutex_lock (&mutex);
		int res = Pon (&lista,usuario,sock_conn);
		pthread_mutex_unlock (&mutex);
		if (res==-1){
			printf("No se puede añadir mas usuarios\n");
			strcpy(respuesta,"2/2");
			return 1;
		}
		else if (res==-2){
			printf("Tiene una sesion \n");
			strcpy(respuesta,"2/3");
			return 1;
		}
		else{
			printf ("Login correct\n");	
			strcpy(respuesta,"2/0");
			return 0;
		}
	}
}



void MasRapido (char respuesta[100],MYSQL *conn){
	//Consulta para que de la persona que menos ha tardado en ganar una partida
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err=mysql_query (conn, "SELECT Partida.Winner FROM Partida WHERE Partida.Time = (SELECT MIN(Partida.Time) FROM Partida)");
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
		while (row !=NULL){
			
			printf("El Jugador que mas rapido ha sido: %s\n", row[0]);
			sprintf(respuesta,"3/%s",row[0]);
			row = mysql_fetch_row (resultado);
		}
	}
}




void GanaronJoel (char respuesta[100],MYSQL *conn){
	//CONSULTA PARA QUE DE LA LISTA DE PERSONAS QUE GANARON UNA PARTIDA DONDE JUGÃ“ JOEL
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	printf("Hola\n");
	int err=mysql_query (conn, "SELECT DISTINCT Partida.Winner FROM Partida, Participantes WHERE Partida.ID IN (SELECT Participantes.ID_P FROM Participantes WHERE Participantes.ID_J=1) AND Partida.Winner != 'Joel'");
	if (err!=0){
		
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		
		printf ("No se han obtenido datos en la consulta\n");
	}
	else{
		strcpy(respuesta, "4/");
		while (row !=NULL) {
			sprintf(respuesta, "%s%s,",respuesta,row[0]);
			row = mysql_fetch_row (resultado);
		}
	}
}





void Viciado (char respuesta [100],MYSQL *conn){
	//Consulta para que de la persona que mas veces a jugado
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err=mysql_query (conn, "SELECT Jugador.Usuario FROM (Jugador, Participantes) WHERE Jugador.ID = Participantes.ID_J ");
	if (err!=0){
		
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	char nombre[20];
	int cont=0;
	int max=0;
	char max_j[20];
	row = mysql_fetch_row (resultado);
	if (row == NULL){
		
		printf ("No se han obtenido datos en la consulta\n");
	}
	else{
		while(row !=NULL){
			
			strcpy(nombre,row[0]);
			if(strcmp(nombre,row[0])!=0){
				cont=0;
			}
			if(cont > max){
				max = cont;
				strcpy(max_j,nombre);
			}
			cont++;
			row = mysql_fetch_row (resultado);
		}
		printf("El Jugador que mas partidas ha jugado es: %s\n", max_j);
		sprintf(respuesta,"5/%s",max_j);
		
	}
}

void Conectados ()
{
	char notificacion[512];
	char Jugadores[300];
	DameConectados (&lista, Jugadores);
	//notificar a todos los conectados
	printf("%s\n",Jugadores);
	sprintf(notificacion,"6/%s",Jugadores);
	
	printf("%s\n",notificacion);
	for(int j=0;j<i;j++){
		write (sockets[j],notificacion, strlen(notificacion));
	}
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char peticion[512];
	char respuesta[512];
	
	
	lista.num=0;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0){
		
		printf("Error creant socket");
	}
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
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
		
		sockets[i] = sock_conn;
		//sock_conn es el socket que usaremos para este cliente
		//Crear thread y decir lo que tiene que hacer
		
		pthread_create (&thread[i],NULL,AtenderCliente,&sockets[i]);
		
		//Reiniciamos el contador de sockets
		if (i==99)
			i=0;
	}
}
