all: clean server client

clean:
	rm -f client.out server.out
server:
	gcc -o server.out server.c -lpthread -lm

client:
	gcc -o client.out client.c