all: clean server client

clean:
	rm -f client.out server.out
server: server.c helpers.h
	gcc -o server.out server.c -lpthread -lm

client: client.c helpers.h
	gcc -o client.out client.c