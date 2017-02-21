typedef enum { false, true } bool;

typedef enum {
    None,
    Correct,
    Less,
    More,
    Invalid
} Answer;

typedef struct{
    int socket;
    struct sockaddr_in address;
} Peer;

typedef struct{
    Peer peer;
    pthread_t thread;
    bool isRunning;
} Client;