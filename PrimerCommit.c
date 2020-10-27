#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char peticion[512];
	char respuesta[512];
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9050);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	
	//Iniciar conecsion base de datos
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas 
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		
		printf ("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "bd",0, NULL, 0);
	if (conn==NULL) {
		
		printf ("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	for(;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		//sock_conn es el socket que usaremos para este cliente
		
		//Bucle de atencion al cliente
		int terminar =0;
		while (terminar==0)
		{
			// Ahora recibimos su nombre, que dejamos en peticion
			ret=read(sock_conn,peticion, sizeof(peticion));
			printf ("Recibido\n");
			
			// Tenemos que a?adirle la marca de fin de string 
			// para que no escriba lo que hay despues en el peticion
			peticion[ret]='\0';
			
			//Escribimos el nombre en la consola
			
			printf ("Se ha conectado: %s\n",peticion);
			
			
			char *p = strtok( peticion, "/");
			int codigo =  atoi (p);
			p = strtok( NULL, "/");
			char usuario[20];
			char password[20];
			if (codigo == 0)
			{
				terminar =1;
			}
			else
			{
				
				if (codigo ==1) //Registrar usuario
				{
					strcpy (usuario,p);
					p=strtok(NULL,"/");
					strcpy (password,p);
					sprintf (consulta,"SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Usuario = '%s' AND Jugador.Psw = '%s'",usuario,password);
					err=mysql_query (conn, consulta);
					if (err!=0) {
						printf ("Error 1 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
					row = mysql_fetch_row (resultado);
					if (row == NULL) 
					{
						printf ("Usuario no existe\n");
						printf("HOLA\n");
						err=mysql_query (conn, "SELECT Jugador.ID FROM Jugador WHERE Jugador.ID = (SELECT MAX(Jugador.ID) FROM Jugador)");
						printf("HOLA2\n");
						if(err!=0){
							printf ("Error 2 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
							exit (1);
						}
						resultado = mysql_store_result (conn);
						row = mysql_fetch_row (resultado);	
						printf("%s\n",row[0]);
						int ID=1;
						if (row != NULL)
						{
							ID = atoi(row[0])+1;
						}
						printf ("%d\n",ID);
						char IDs[10];
						sprintf(IDs,"%d",ID);
						strcpy (consulta, "INSERT INTO Jugador VALUES (");	
						strcat(consulta,IDs);
						strcat (consulta, ",'");
						strcat (consulta, usuario);
						strcat (consulta, "','");
						strcat (consulta, password);
						strcat (consulta, "');");
						printf("%s\n",consulta);
						err = mysql_query(conn, consulta);
						if (err!=0) 
						{
							printf ("Error 3 al introducir datos la base %u %s\n", 
									mysql_errno(conn), mysql_error(conn));
							exit (1);
						}
						strcpy(respuesta,"0");
						
					}
					else 
					{
						printf("El usuario %s ya existe\n", row[0]);
						strcpy(respuesta,"1");
					}
				}
				else if (codigo ==2) //Login
				{
					strcpy (usuario,p);
					p=strtok(NULL,"/");
					strcpy (password,p);
					sprintf (consulta,"SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Usuario = '%s' AND Jugador.Psw = '%s'",usuario,password);
					err=mysql_query (conn, consulta);
					if (err!=0) {
						printf ("Error 1 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
					row = mysql_fetch_row (resultado);
					if (row == NULL) 
					{
						printf ("Error en los datos del usuario");
						strcpy(respuesta,"1");
					}
					else
					{
						printf ("Login correct");
						strcpy(respuesta,"0");
					}
				}
				else if (codigo ==3)
				{
					//Consulta para que de la persona que menos ha tardado en ganar una partida
					err=mysql_query (conn, "SELECT Partida.Winner FROM Partida WHERE Partida.Time = (SELECT MIN(Partida.Time) FROM Partida)");
					if (err!=0) {
						printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
					char nombre[20];
					row = mysql_fetch_row (resultado);
					if (row == NULL) {
						printf ("No se han obtenido datos en la consulta\n");
					}
					else{
						while (row !=NULL) {
							//nombre = row[0];
							printf("El Jugador que mas rapido ha sido: %s\n", row[0]);
							strcpy(respuesta,row[0]);
							row = mysql_fetch_row (resultado);
						}
					}
				}
				else if (codigo == 4)
				{
					//CONSULTA PARA QUE DE LA LISTA DE PERSONAS QUE GANARON UNA PARTIDA DONDE JUGÓ JOEL
					err=mysql_query (conn, "SELECT DISTINCT Partida.Winner FROM (Partida, Participantes) WHERE Partida.ID IN (SELECT Participantes.ID_P FROM Participantes WHERE Participantes.ID_J=1)");
					if (err!=0) {
						printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
					row = mysql_fetch_row (resultado);
					if (row == NULL) {
						printf ("No se han obtenido datos en la consulta\n");
					}
					else{
						while (row !=NULL) {
							printf("%s\n", row[0]);
							sprintf(respuesta, "%s/",row[0]);
							row = mysql_fetch_row (resultado);
						}
					}
				
				}
				else if (codigo ==5)
				{
					//Consulta para que de la persona que mas veces a jugado
					err=mysql_query (conn, "SELECT Jugador.Usuario FROM (Jugador, Participantes) WHERE Jugador.ID = Participantes.ID_J ");
					if (err!=0) {
						printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
					char nombre[20];
					int cont=0;
					int max=0;
					char max_j[20];
					row = mysql_fetch_row (resultado);
					if (row == NULL) {
						printf ("No se han obtenido datos en la consulta\n");
					}
					else{
						while (row !=NULL) {
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
						strcpy(respuesta,max_j);
						
					}
				}
				
				write (sock_conn,respuesta, strlen(respuesta));
			}
		}
		
		// Se acabo el servicio para este cliente
		close(sock_conn); 
		
	}
}
