#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#define BUFF_LEN 1024
#define MAX_CLIENTS 10

const int MIN_VAL = 1;
const int MAX_VAL = 100;

typedef enum {
    Correct,
    Less,
    More,
    Invalid
} Answer;

typedef struct{
    struct sockaddr_in address;
    int socket;
    int threadId;
} ClientInfo;

pthread_t threads[MAX_CLIENTS];

Answer getInput(struct sockaddr_in clientaddr, int socket){
    char buffer[BUFF_LEN];
    memset(&buffer, 0, sizeof(buffer));
    int s_len = recv(socket,buffer,sizeof(buffer),0);
    fprintf(stdout, "IP: %s\tReceived: '%s'\n",inet_ntoa(clientaddr.sin_addr), buffer);
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

void playGame(struct sockaddr_in clientaddr, int socket){
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
    send(socket, buffer, strlen(buffer), 0);
    getInput(clientaddr, socket);
    while(1){
        if(hi < lo){
            sprintf(buffer, "You lied to me ;(\n");
            send(socket, buffer, strlen(buffer), 0);
            break;
        }
        int mid = (hi+lo)/2;
        memset(&buffer, 0, sizeof(buffer));
        sprintf(buffer, "%d) Is your number %d? (Yes/Less/More) ", guesses, mid);
        fprintf(stdout, "IP: %s\tGuessing: %d\n", inet_ntoa(clientaddr.sin_addr), mid);
        send(socket,buffer,strlen(buffer),0);
        while(1){
            Answer ans = getInput(clientaddr, socket);
            if(ans == Correct){
                fprintf(stdout, "IP: %s\tWon in %d guesses\n", inet_ntoa(clientaddr.sin_addr), guesses);
                sprintf(buffer, "Gotcha in %d guesses. Good game ;)\n", guesses);
                send(socket,buffer,strlen(buffer),0);
                return;
            } else if(ans == Less){
                hi = mid-1;
                break;
            } else if(ans == More){
                lo = mid+1;
                break;
            } else{
                sprintf(buffer, "Invalid choice!\n");
                send(socket,buffer,strlen(buffer),0);
            }
        }
        guesses++;
    }
}

void* runNewGameInstance(void *client){
    ClientInfo ci = *(ClientInfo*)client;
    playGame(ci.address, ci.socket);
    fprintf(stdout, "Game finished for %s.\n", inet_ntoa(ci.address.sin_addr));
    close(ci.socket);
    threads[ci.threadId] = -1;
    pthread_exit(0);
}

int getEmptySlotIndex(pthread_t* slots){
    int i;
    for(i = 0; i < MAX_CLIENTS; i++)
        if(slots[i] == -1)
            return i;
    return -1;
}

int main(int argc, char *argv[]){
    unsigned int port;
    int l_socket;
    int c_socket;

    struct sockaddr_in servaddr;
    struct sockaddr_in clientaddr;
    socklen_t clientaddrlen;

    int s_len;
    int r_len;
    char buffer[BUFF_LEN];

    int i;
    for(i = 0; i < MAX_CLIENTS; i++)
        threads[i] = -1;
    
    if (argc != 2){
        fprintf(stdout, "USAGE: %s <port>\n", argv[0]);
        exit(1);
    }

    port = atoi(argv[1]);

    if ((port < 1) || (port > 65535)){
        fprintf(stderr, "ERROR #1: invalid port specified.\n");
        exit(1);
    }

    if ((l_socket = socket(AF_INET, SOCK_STREAM,0))< 0){
        fprintf(stderr,"ERROR #2: cannot create listening socket.\n");
        exit(1);
    }
    
    memset(&servaddr,0, sizeof(servaddr));
    servaddr.sin_family = AF_INET;

    servaddr.sin_addr.s_addr = htonl(INADDR_ANY); 
    servaddr.sin_port = htons(port);
    
    if (bind (l_socket, (struct sockaddr *)&servaddr,sizeof(servaddr))<0){
        fprintf(stderr,"ERROR #3: bind listening socket.\n");
        exit(1);
    }

    if (listen(l_socket, 5) <0){
        fprintf(stderr,"ERROR #4: error in listen().\n");
        exit(1);
    }

    while(1){
        memset(&clientaddr,0, sizeof(clientaddr));
        memset(&buffer,0,sizeof(buffer));

        clientaddrlen = sizeof(struct sockaddr);
        if ((c_socket = accept(l_socket,
            (struct sockaddr*)&clientaddr,&clientaddrlen))<0){
            fprintf(stderr,"ERROR #5: error occured accepting connection.\n");
            exit(1);
        } else{
            int index = getEmptySlotIndex(threads);
            if(index == -1){
                sprintf(buffer, "No more available slots :/");
                send(c_socket, buffer, strlen(buffer), 0);
            } else {
                ClientInfo ci;
                ci.address = clientaddr;
                ci.socket = c_socket;
                ci.threadId = index;
                pthread_create(&threads[index], NULL, runNewGameInstance, (void*)&ci);
                threads[i] = 0;
            }
        }
        
    }

    return 0;
}