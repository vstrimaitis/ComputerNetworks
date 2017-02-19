#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include "helpers.h"
#define BUFF_LEN 1024
#define MAX_CLIENTS 10
#define MIN_VAL 1
#define MAX_VAL 100

char* ip(Client c){
    return inet_ntoa(c.peer.address.sin_addr);
}

Client clients[MAX_CLIENTS];

Answer getInput(Client c){
    char buffer[BUFF_LEN];
    memset(&buffer, 0, sizeof(buffer));
    recv(c.peer.socket,buffer,sizeof(buffer),0);
    fprintf(stdout, "IP: %s\tReceived: '%s'\n",ip(c), buffer);
    int i = 0;
    for(; i < strlen(buffer); i++)
        buffer[i] = tolower(buffer[i]);
    if(strcmp(buffer, "yes") == 0)
        return Correct;
    if(strcmp(buffer, "less") == 0)
        return Less;
    if(strcmp(buffer, "more") == 0)
        return More;
    return Invalid;
}

void playGame(Client client){
    int lo = MIN_VAL, hi = MAX_VAL, guesses = 1;
    char buffer[BUFF_LEN];
    int maxGuesses = ceil(log(MAX_VAL - MIN_VAL + 2) / log(2));
    sprintf(buffer, "%s%s%d%s%d%s%s%d%s%s%s%s",
        "==============================================================\n",
        "Think of a number between ", lo, " and ", hi," (inclusive).\n",
        "I'll try to guess it in not more than ", maxGuesses, " tries.\n",
        "I'll keep guessing and you'll have to answer, whether\n",
        "your number is larger or smaller than the one I'm guessing.\n",
        "==============================================================\n");
    send(client.peer.socket, buffer, strlen(buffer), 0);
    getInput(client);
    while(1){
        if(hi < lo){
            sprintf(buffer, "You lied to me!\n");
            send(client.peer.socket, buffer, strlen(buffer), 0);
            break;
        }
        int mid = (hi+lo)/2;
        memset(&buffer, 0, sizeof(buffer));
        sprintf(buffer, "%d) Is your number %d? (Yes/Less/More) ", guesses, mid);
        fprintf(stdout, "IP: %s\tGuessing: %d\n", ip(client), mid);
        send(client.peer.socket,buffer,strlen(buffer),0);
        while(1){
            Answer ans = getInput(client);
            if(ans == Correct){
                fprintf(stdout, "IP: %s\tWon in %d guesses\n", ip(client), guesses);
                sprintf(buffer, "Gotcha in %d guesses. Good game ;)\n", guesses);
                send(client.peer.socket,buffer,strlen(buffer),0);
                return;
            } else if(ans == Less){
                hi = mid-1;
                break;
            } else if(ans == More){
                lo = mid+1;
                break;
            } else{
                sprintf(buffer, "Invalid choice!\n");
                send(client.peer.socket,buffer,strlen(buffer),0);
            }
        }
        guesses++;
    }
}

void* runNewGameInstance(void *client){
    Client c = *(Client*)client;
    playGame(c);
    fprintf(stdout, "Game finished for %s.\n", ip(c));
    close(c.peer.socket);
    c.isRunning = false;
    pthread_exit(0);
}

int getEmptySlotIndex(){
    int i;
    for(i = 0; i < MAX_CLIENTS; i++)
        if(!clients[i].isRunning)
            return i;
    return -1;
}

int establishConnection(Peer *peer, unsigned int port){
    if ((port < 1) || (port > 65535))
        return 1;

    // Step 1: socket()
    if ((peer->socket= socket(AF_INET, SOCK_STREAM,0))< 0)
        return 2;
    
    memset(&(peer->address),0, sizeof(peer->address));
    // Set the protocol, IP address of incoming connections and the port in the server's address structure
    peer->address.sin_family = AF_INET;
    peer->address.sin_addr.s_addr = htonl(INADDR_ANY); 
    peer->address.sin_port = htons(port);
    
    // Step 2: bind() address to socket
    if (bind (peer->socket, (struct sockaddr *)&(peer->address),sizeof(peer->address))<0)
        return 3;

    // Step 3: listen() for connections, max queue size - 5
    if (listen(peer->socket, 5) <0)
        return 4;
    return 0;
}

int acceptConnection(Peer *client, Peer server){
    memset(&(client->address),0, sizeof(client->address));
    socklen_t addressLength = sizeof(struct sockaddr);
    if ((client->socket = accept(server.socket,
        (struct sockaddr*)&(client->address), &addressLength))<0){
        return 1;
    }
    return 0;
}

int main(int argc, char *argv[]){
    unsigned int port;
    Peer listener, client;
    char buffer[BUFF_LEN];
    
    if (argc != 2){
        fprintf(stdout, "USAGE: %s <port>\n", argv[0]);
        return 1;
    }

    port = atoi(argv[1]);

    int connectionResult = establishConnection(&listener, port);
    if(connectionResult != 0){
        switch(connectionResult){
            case 1:
                fprintf(stderr, "Invalid port specified.\n");
                break;
            case 2:
                fprintf(stderr,"Could not create listening socket.\n");
                break;
            case 3:
                fprintf(stderr,"Failed to bind to listening socket.\n");
                break;
            case 4:
                fprintf(stderr,"An error occurred when trying to listen to socket.\n");
                break;
        }
        return 1;
    }

    while(1){
        if(acceptConnection(&client, listener) != 0){
            fprintf(stderr,"An error occured while accepting connection.\n");
            return 1;
        }else{
            int index = getEmptySlotIndex();
            if(index == -1){
                memset(&buffer,0,sizeof(buffer));
                sprintf(buffer, "No more available slots.");
                send(client.socket, buffer, strlen(buffer), 0);
            } else {
                clients[index].peer = client;
                clients[index].isRunning = true;
                pthread_create(&clients[index].thread, NULL, runNewGameInstance, (void*)&clients[index]);
            }
        }
        
    }

    return 0;
}