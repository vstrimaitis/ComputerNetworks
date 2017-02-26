all: clean server client

clean:
	rm -f client.out server.out
server: server.c helpers.h
	gcc -Wall -o server.out server.c -lpthread -lm

client: client.c helpers.h
	gcc -Wall -o client.out client.c