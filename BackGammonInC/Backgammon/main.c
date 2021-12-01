
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <Windows.h>

#include "Game.h"
#include "utils.h"
#include "Tests.h"
#include "Ai.h"
#include "main.h"


int main() {
	system("cls");
	printf("Welcome to backgammon\n: ");
	/*for (int c = 170; c < 255; c++)
		printf("%d %c\n\n", c, c);*/
	unsigned int cp = GetConsoleCP();
	SetConsoleCP(437);
	InitAi(true);	
	StartPosition(&G);
	PrintGame(&G);
	char buf[BUF_SIZE];
	fgets(buf, 5000, stdin);
	while (!Streq(buf, "quit\n") && !Streq(buf, "q\n"))
	{
		if (Streq(buf, "test\n") || Streq(buf, "t\n")) {
			RunAll();
		}
		else if (Streq(buf, "play\n") || Streq(buf, "p\n")) {
			AutoPlay();
		}
		else {
			ConsoleWriteLine("Unknown command");
		}
		printf(": ");
		fgets(buf, 5000, stdin);
	}

	SetConsoleCP(cp);
	ConsoleWriteLine("Bye");
}

