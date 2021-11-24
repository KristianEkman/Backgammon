
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include "Game.h"
#include "utils.h"
#include "Tests.h"


int main() {
	printf("Welcome to backgammon\n: ");
	StartPosition();

	char buf[BUF_SIZE];
	fgets(buf, 5000, stdin);
	while (!Streq(buf, "quit\n") && !Streq(buf, "q\n"))
	{
		if (Streq(buf, "test\n") || Streq(buf, "t\n")) {
			RunAll();
		}
		else {
			ConsoleWriteLine("Unknown command");
		}
		printf(": ");
		fgets(buf, 5000, stdin);
	}

	ConsoleWriteLine("Bye");
}