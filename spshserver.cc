/*
 * This is the server for our spreadsheet 
 * application.
 *
 * Implemented by,
 * Skyler Jones, Christy Bowen, Mike Miner & James Sullivan
 *
 * April 4, 2014
 */

//Referenced
// http://www.cplusplus.com/forum/unices/116977/

//g++ spshserver.cc -lpthread

#include <string.h>
#include <unistd.h>
#include <stdio.h>
#include <netdb.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <iostream>
#include <fstream>
#include <strings.h>
#include <stdlib.h>
#include <string>
#include <pthread.h>
using namespace std;

void *client_task(void *);

static int connFd;

/*
 * This is a simple multithreaded server that will listen
 * for cstrings and the server will just echo out the 
 * message received.
 */
int main (int argc, char* argv[])
{
  int listenFd, pID;
  socklen_t len; //size of the address
  struct sockaddr_in svrAdd, clntAdd;
  bool loop = false;

  pthread_t threadA[3];

  //create socket
  listenFd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

  //predefined port number?????????
  int portNo = 2000;

  if(listenFd < 0)
    {
      cerr << "Can't open socket"<< endl;
      return 0;
    }

    bzero((char*) &svrAdd, sizeof(svrAdd));
    
    svrAdd.sin_family = AF_INET;
    svrAdd.sin_addr.s_addr = INADDR_ANY;
    svrAdd.sin_port = htons(portNo);
    
    //bind socket
    if(bind(listenFd, (struct sockaddr *)&svrAdd, sizeof(svrAdd)) < 0)
    {
        cerr << "Cannot bind" << endl;
        return 0;
    }
    
    listen(listenFd, 5);
    
    int noThread = 0;

    // Listening for messages from the client.
    while (noThread < 3)
    {
      socklen_t len = sizeof(clntAdd);

        cout << "Listening" << endl;

        //this is where client connects. svr will hang in this mode until client conn
        connFd = accept(listenFd, (struct sockaddr *)&clntAdd, &len);

        if (connFd < 0)
        {
            cerr << "Cannot accept connection" << endl;
            return 0;
        }
        else
        {
            cout << "Connection successful" << endl;
        }
        
        pthread_create(&threadA[noThread], NULL, client_task, NULL); 
        
        noThread++;
    }
    
    for(int i = 0; i < 3; i++)
    {
        pthread_join(threadA[i], NULL);
    }   
    
}

/*
 * Processed the task in the next thread.
 */
void *client_task (void *dummyPt)
{
    cout << "Thread No: " << pthread_self() << endl;
    char test[300];
    bzero(test, 301);
    bool loop = false;
    while(!loop)
    {    
        bzero(test, 301);

        // MAY NOT WORK.

	//If you are using TCP, this cannot be relied on the be 300 each time. If you're always going to send 300, then loop read()-ing the outstanding bytes until you get to 300. You will need to append to the receiving buffer at the appropriate spot until done.

	//will read the number of bytes read
        read(connFd, test, 300);
        
        string tester (test);
        cout << tester << endl;
        
        
        if(tester == "exit")
            break;
    }
    cout << "\nClosing thread and conn" << endl;
    close(connFd);
}
