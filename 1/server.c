#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define BUFF_LEN 256

const int MIN_VAL = 1;
const int MAX_VAL = 100;

 typedef enum {
	Correct,
	Less,
	More,
	Invalid
} Answer;

Answer getInput(struct sockaddr_in clientaddr, int socket){
    char buffer[BUFF_LEN];
    memset(&buffer, 0, sizeof(buffer));
    int s_len = recv(socket,buffer,sizeof(buffer),0);
    printf("IP: %s\tReceived: '%s'\n",inet_ntoa(clientaddr.sin_addr), buffer);
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

void playGame(struct sockaddr_in clientaddr, int socket, int min, int max){
	int lo = min, hi = max, guesses = 1;
	char buffer[BUFF_LEN];
	while(1){
		if(hi < lo){
			printf("You lied to me ;(\n");
			break;
		}
		int mid = (hi+lo)/2;
		memset(&buffer, 0, sizeof(buffer));
		sprintf(buffer, "%d) Is your number %d? (Yes/Less/More) ", guesses, mid);
		printf("IP: %s\tGuessing: %d\n", inet_ntoa(clientaddr.sin_addr), mid);
		send(socket,buffer,strlen(buffer),0);
		while(1){
			Answer ans = getInput(clientaddr, socket);
			if(ans == Correct){
				//printf("Gotcha in %d guesses ;)\n", guesses);
				printf("IP: %s\tWon in %d guesses\n", inet_ntoa(clientaddr.sin_addr), guesses);
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
				//printf("Invalid choice!\n");
				sprintf(buffer, "Invalid choice!\n");
				send(socket,buffer,strlen(buffer),0);
			}
		}
		guesses++;
	}
}

int main(int argc, char *argv[]){
	unsigned int port;
    int l_socket; // socket'as skirtas prisijungim� laukimui
    int c_socket; // prisijungusio kliento socket'as

    struct sockaddr_in servaddr; // Serverio adreso strukt�ra
    struct sockaddr_in clientaddr; // Prisijungusio kliento adreso strukt�ra
//    int clientaddrlen;
    socklen_t clientaddrlen;

    int s_len;
    int r_len;
    char buffer[BUFF_LEN];
    
    if (argc != 2){
        printf("USAGE: %s <port>\n", argv[0]);
        exit(1);
    }

    port = atoi(argv[1]);

    if ((port < 1) || (port > 65535)){
        printf("ERROR #1: invalid port specified.\n");
        exit(1);
    }

    /*
     * Sukuriamas serverio socket'as
     */
    if ((l_socket = socket(AF_INET, SOCK_STREAM,0))< 0){
        fprintf(stderr,"ERROR #2: cannot create listening socket.\n");
        exit(1);
    }
    
    /*
     * I�valoma ir u�pildoma serverio adreso strukt�ra
     */
    memset(&servaddr,0, sizeof(servaddr));
    servaddr.sin_family = AF_INET; // nurodomas protokolas (IP)

    /*
     * Nurodomas IP adresas, kuriuo bus laukiama klient�, �iuo atveju visi 
     * esami sistemos IP adresai (visi interfeis'ai)
     */
    servaddr.sin_addr.s_addr = htonl(INADDR_ANY); 
    servaddr.sin_port = htons(port); // nurodomas portas
    
    /*
     * Serverio adresas susiejamas su socket'u
     */
    if (bind (l_socket, (struct sockaddr *)&servaddr,sizeof(servaddr))<0){
        fprintf(stderr,"ERROR #3: bind listening socket.\n");
        exit(1);
    }

    /*
     * Nurodoma, kad socket'u l_socket bus laukiama klient� prisijungimo,
     * eil�je ne daugiau kaip 5 aptarnavimo laukiantys klientai
     */
    if (listen(l_socket, 5) <0){
        fprintf(stderr,"ERROR #4: error in listen().\n");
        exit(1);
    }

    for(;;){
        /*
         * I�valomas buferis ir kliento adreso strukt�ra
         */
        memset(&clientaddr,0, sizeof(clientaddr));
        memset(&buffer,0,sizeof(buffer));

        /*
         * Laukiama klient� prisijungim�
         */
        clientaddrlen = sizeof(struct sockaddr);
        if ((c_socket = accept(l_socket,
            (struct sockaddr*)&clientaddr,&clientaddrlen))<0){
            fprintf(stderr,"ERROR #5: error occured accepting connection.\n");
            exit(1);
        }

        playGame(clientaddr, c_socket, MIN_VAL, MAX_VAL);
        printf("Game finished.\n");
        close(c_socket);
        printf("Socket closed.\n");
    }

    return 0;
}