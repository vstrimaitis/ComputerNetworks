#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "helpers.h"

#define BUFFLEN 1024

void playGame(int socket){
    char buffer[BUFFLEN];
    memset(&buffer, 0, sizeof(buffer));
    if(recv(socket, buffer, BUFFLEN, 0) <= 0){
        fprintf(stdout, "Something went wrong while trying to receive the server's response.\n");
        return;
    }
    fprintf(stdout, "%s\n", buffer);
    fprintf(stdout, "Press `ENTER` to begin: ");
    char c;
    while((c = getchar()) != '\n');
    if(send(socket, "start", 6, 0) <= 0){
        fprintf(stdout, "Something went wrong while trying to send a message to the server.\n");
        return;
    }
    while(1){
        memset(&buffer, 0, sizeof(buffer));
        if(recv(socket,buffer,BUFFLEN,0) <= 0){
            fprintf(stdout, "Something went wrong while trying to receive the server's response.\n");
            return;
        }
        fprintf(stdout, "%s", buffer);

        if(send(socket, "ping", 5, 0) <= 0){
            fprintf(stdout, "Something went wrong while trying to send a message to the server.\n");
            return;
        }
        if(recv(socket, buffer, BUFFLEN, 0) <= 0) // the game is finished
            return;

        memset(&buffer, 0, sizeof(buffer));
        scanf("%s", buffer);
        if(send(socket,buffer,strlen(buffer),0) <= 0){
            fprintf(stdout, "Something went wrong while trying to send a message to the server.\n");
            return;
        }
    }
}

int establishConnection(Peer *p, char* ip, unsigned int port){
    if ((port < 1) || (port > 65535))
        return 1;

    // Step 1: socket()
    if ((p->socket = socket(AF_INET, SOCK_STREAM,0))< 0)
        return 2;

    memset(&(p->address),0,sizeof(p->address));
    // Set the protocol and port in the server address structure
    p->address.sin_family = AF_INET;
    p->address.sin_port = htons(port);
    
    // Convert char* ip into a numerical form and save it in the server address structure
    if ( inet_aton(ip, &p->address.sin_addr) <= 0 )
        return 3;
    
    // Step 2: connect()
    if (connect(p->socket,(struct sockaddr*)&(p->address),sizeof(p->address))<0)
        return 4;
    return 0;
}

int main(int argc, char *argv[]){
    unsigned int port;
    Peer server;

    if (argc != 3){
        fprintf(stdout,"USAGE: %s <ip> <port>\n",argv[0]);
        exit(1);
    }

    port = atoi(argv[2]);

    int connectionResult = establishConnection(&server, argv[1], port);
    if(connectionResult != 0){
        switch(connectionResult){
            case 1:
                fprintf(stderr, "Invalid port specified.\n");
                break;
            case 2:
                fprintf(stderr,"Cannot create socket.\n");
                break;
            case 3:
                fprintf(stderr,"Invalid remote IP address.\n");
                break;
            case 4:
                fprintf(stderr,"An error occurred while establishing the connection.\n");
                break;
        }
    }
  
    playGame(server.socket);
    close(server.socket);
    return 0;
}
