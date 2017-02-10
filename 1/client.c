#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define BUFFLEN 256

void playGame(int socket){
    char buffer[BUFFLEN];
    while(1){
        memset(&buffer, 0, sizeof(buffer));
        recv(socket,buffer,BUFFLEN,0);
        if(strlen(buffer) == 0)
            break;
        printf("%s", buffer);

        send(socket, "ping", 5, 0);
        int ping = recv(socket, buffer, BUFFLEN, 0);
        if(ping == 0)
            return;

        memset(&buffer, 0, sizeof(buffer));
        scanf("%s", buffer);
        //printf("Sending %s (%d)\n", buffer, (int)strlen(buffer));
        send(socket,buffer,strlen(buffer),0);
    }
}

int main(int argc, char *argv[]){   
    unsigned int port;
    int s_socket;
    struct sockaddr_in servaddr; // Serverio adreso struktûra

    char buffer[BUFFLEN];

    if (argc != 3){
        fprintf(stderr,"USAGE: %s <ip> <port>\n",argv[0]);
        exit(1);
    }

    port = atoi(argv[2]);

    if ((port < 1) || (port > 65535)){
        printf("ERROR #1: invalid port specified.\n");
        exit(1);
    }

    /*
     * Sukuriamas socket'as
     */
    if ((s_socket = socket(AF_INET, SOCK_STREAM,0))< 0){
        fprintf(stderr,"ERROR #2: cannot create socket.\n");
        exit(1);
    }
                                
   /*
    * Iðvaloma ir uþpildoma serverio struktûra
    */
    memset(&servaddr,0,sizeof(servaddr));
    servaddr.sin_family = AF_INET; // nurodomas protokolas (IP)
    servaddr.sin_port = htons(port); // nurodomas portas
    
    /*
     * Iðverèiamas simboliø eilutëje uþraðytas ip á skaitinæ formà ir
     * nustatomas serverio adreso struktûroje.
     */  
    if ( inet_aton(argv[1], &servaddr.sin_addr) <= 0 ) {
        fprintf(stderr,"ERROR #3: Invalid remote IP address.\n");
        exit(1);
    }       

    
    /* 
     * Prisijungiama prie serverio
     */
    if (connect(s_socket,(struct sockaddr*)&servaddr,sizeof(servaddr))<0){
        fprintf(stderr,"ERROR #4: error in connect().\n");
        exit(1);
    }
    /*
    printf("Enter the message: ");
    fgets(buffer, BUFFLEN, stdin);
    send(s_socket,buffer,strlen(buffer),0);

    memset(&buffer,0,BUFFLEN);
    recv(s_socket,buffer,BUFFLEN,0);
    printf("Server sent: %s\n", buffer);
*/
    playGame(s_socket);
    /*
     * Socket'as uþdaromas
     */
    close(s_socket);
    return 0;
}
