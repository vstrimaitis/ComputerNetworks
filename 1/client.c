#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define BUFFLEN 1024

void playGame(int socket){
    char buffer[BUFFLEN];
    memset(&buffer, 0, sizeof(buffer));
    recv(socket, buffer, BUFFLEN, 0);
    fprintf(stdout, "%s\n", buffer);
    fprintf(stdout, "Enter any character to begin: ");
    char c = getchar();
    send(socket, "start", 6, 0);
    while(1){
        memset(&buffer, 0, sizeof(buffer));
        recv(socket,buffer,BUFFLEN,0);
        if(strlen(buffer) == 0)
            break;
        fprintf(stdout, "%s", buffer);

        send(socket, "ping", 5, 0);
        int ping = recv(socket, buffer, BUFFLEN, 0);
        if(ping == 0)
            return;

        memset(&buffer, 0, sizeof(buffer));
        scanf("%s", buffer);
        send(socket,buffer,strlen(buffer),0);
    }
}

int main(int argc, char *argv[]){
    unsigned int port;
    int s_socket;
    struct sockaddr_in servaddr;

    char buffer[BUFFLEN];

    if (argc != 3){
        fprintf(stdout,"USAGE: %s <ip> <port>\n",argv[0]);
        exit(1);
    }

    port = atoi(argv[2]);

    if ((port < 1) || (port > 65535)){
        fprintf(stderr, "ERROR #1: invalid port specified.\n");
        exit(1);
    }

    if ((s_socket = socket(AF_INET, SOCK_STREAM,0))< 0){
        fprintf(stderr,"ERROR #2: cannot create socket.\n");
        exit(1);
    }

    memset(&servaddr,0,sizeof(servaddr));
    servaddr.sin_family = AF_INET;
    servaddr.sin_port = htons(port);

    if ( inet_aton(argv[1], &servaddr.sin_addr) <= 0 ) {
        fprintf(stderr,"ERROR #3: Invalid remote IP address.\n");
        exit(1);
    }       

    if (connect(s_socket,(struct sockaddr*)&servaddr,sizeof(servaddr))<0){
        fprintf(stderr,"ERROR #4: error in connect().\n");
        exit(1);
    }
  
    playGame(s_socket);
    close(s_socket);
    return 0;
}
