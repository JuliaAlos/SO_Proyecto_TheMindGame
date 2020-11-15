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

void *AtenderCliente (void *socket){
	
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	int terminar =0;
	char peticion[512];
	char respuesta[512];
	int ret;
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas 
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
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
		// Ahora recibimos su nombre, que dejamos en peticion
		ret=read(sock_conn,peticion, sizeof(peticion));
		
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el peticion
		peticion[ret]='\0';
		
		//Escribimos el nombre en la consola
		if (strcmp(peticion,"/7")!=0){
			
			printf ("Recibido\n");
			printf ("Se ha conectado: %s\n",peticion);
		}
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		
		char *usuario[20];
		char *password[20];
		
		if (codigo == 0){
			pthread_mutex_lock (&mutex);
			terminar =1;
			sprintf(consulta, "UPDATE Jugador SET Conectado=0 WHERE Jugador.Usuario='%s';",usuario);
			err=mysql_query (conn,consulta);
			if (err!=0){
				
				printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			pthread_mutex_unlock (&mutex);
		}
		else{
			
			if (codigo ==1){ //Registrar usuario
				pthread_mutex_lock (&mutex);
				p = strtok( NULL, "/");
				strcpy (usuario,p);
				p=strtok(NULL,"/");
				strcpy (password,p);
				sprintf (consulta,"SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Usuario = '%s' AND Jugador.Psw ='%s';",usuario,password);
				err=mysql_query (conn, consulta);
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
					printf("%s3\n",peticion);
					
					printf("%s4\n",usuario);
					sprintf (consulta, "INSERT INTO Jugador VALUES (%d,'%s','%s',0)",ID,usuario,password);
					//strcat (consulta, usuario);
					//strcat (consulta, "','");
					//strcat (consulta, p);
					//strcat (consulta, "',0);");
					printf("%s\n",consulta);
					err = mysql_query(conn, consulta);
					if (err!=0){
						
						printf ("Error 3 al introducir datos la base %u %s\n",mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					strcpy(respuesta,"0");
					
				}
				else{
					
					printf("El usuario %s ya existe\n", row[0]);
					strcpy(respuesta,"1");
				}
				pthread_mutex_unlock (&mutex);
			}
			else if (codigo ==2){ //Login
				pthread_mutex_lock (&mutex);
				p = strtok( NULL, "/");
				strcpy (usuario,p);
				p=strtok(NULL,"/");
				strcpy(password,p);
				sprintf (consulta,"SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Usuario = '%s' AND Jugador.Psw ='%s';",usuario,password);
				printf("%s\n",consulta);
				err=mysql_query (conn, consulta);
				if (err!=0){
					
					printf ("Error 1 al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				if (row == NULL){
					
					printf ("Error en los datos del usuario");
					strcpy(respuesta,"1");
				}
				else{
					
					printf ("Login correct");
					strcpy(respuesta,"0");
					sprintf(consulta, "UPDATE Jugador SET Conectado=1 WHERE Jugador.Usuario='%s';",usuario);
					err=mysql_query (conn,consulta);
					if (err!=0){
						
						printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
				}
				pthread_mutex_unlock (&mutex);
			}
			else if (codigo ==3){
				pthread_mutex_lock (&mutex);
				//Consulta para que de la persona que menos ha tardado en ganar una partida
				err=mysql_query (conn, "SELECT Partida.Winner FROM Partida WHERE Partida.Time = (SELECT MIN(Partida.Time) FROM Partida)");
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
						strcpy(respuesta,row[0]);
						row = mysql_fetch_row (resultado);
					}
				}
				pthread_mutex_unlock (&mutex);
			}
			else if (codigo == 4){
				pthread_mutex_lock (&mutex);
				//CONSULTA PARA QUE DE LA LISTA DE PERSONAS QUE GANARON UNA PARTIDA DONDE JUGÓ JOEL
				err=mysql_query (conn, "SELECT DISTINCT Partida.Winner FROM (Partida, Participantes) WHERE Partida.ID IN (SELECT Participantes.ID_P FROM Participantes WHERE Participantes.ID_J=1)");
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
					while (row !=NULL) {
						printf("%s\n", row[0]);
						sprintf(respuesta, "%s/",row[0]);
						row = mysql_fetch_row (resultado);
					}
				}
				pthread_mutex_unlock (&mutex);
			}
			else if (codigo ==5){
				pthread_mutex_lock (&mutex);
				//Consulta para que de la persona que mas veces a jugado
				err=mysql_query (conn, "SELECT Jugador.Usuario FROM (Jugador, Participantes) WHERE Jugador.ID = Participantes.ID_J ");
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
					strcpy(respuesta,max_j);
					
				}
				pthread_mutex_unlock (&mutex);
			}
			else if (codigo ==6){
				pthread_mutex_lock (&mutex);
				sprintf(consulta, "UPDATE Jugador SET Conectado=0 WHERE Jugador.Usuario='%s'",usuario);
				
				err=mysql_query (conn,consulta);
				if (err!=0){
					printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit (1);
					
				}
				pthread_mutex_unlock (&mutex);
			}
			else if (codigo==7){
				pthread_mutex_lock (&mutex);
				sprintf(consulta, "SELECT Jugador.Usuario FROM Jugador WHERE Jugador.Conectado=1;");
				err=mysql_query (conn,consulta);
				if (err!=0){
					
					printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				resultado = mysql_store_result (conn);
				row=mysql_fetch_row(resultado);
				strcpy(respuesta,"");
				while(row!=NULL){
					
					sprintf(respuesta, "%s%s/", respuesta, row[0]);
					row=mysql_fetch_row(resultado);
				}
				pthread_mutex_unlock (&mutex);
			}
		}
		write (sock_conn,respuesta, strlen(respuesta));
	}
}
	
	// Se acabo el servicio para este cliente
	close(sock_conn); 

int main(int argc, char *argv[])
{
	
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char peticion[512];
	char respuesta[512];
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0){
		
		printf("Error creant socket");
	}
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9070);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		
		printf ("Error al bind");
	}
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 2) < 0){
		
		printf("Error en el Listen");
	}
	//Iniciar conexion base de datos
	
	int i;
	int sockets[100];
	pthread_t thread[100];
	for(i=0;i<100;i++){
		
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen,NULL,NULL);
		printf("He recibido conexion\n");
		
		sockets[i] = sock_conn;
		//sock_conn es el socket que usaremos para este cliente
		//Crear thread y decir lo que tiene que hacer
		
		pthread_create (&thread[i],NULL,AtenderCliente,&sockets[i]);
	}
}
