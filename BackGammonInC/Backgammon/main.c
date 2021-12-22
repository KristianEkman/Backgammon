
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <Windows.h>

#include "Game.h"
#include "utils.h"
#include "Tests.h"
#include "Ai.h"
#include "main.h"
#include "Hash.h"

void PrintHelp() {
	ConsoleWriteLine("\nOptions\n-------\nt/test\np/play\nh/help\nq/quit");
}

int main() {
	system("cls");
	printf("Welcome to backgammon\n: ");
	/*for (int c = 170; c < 255; c++)
		printf("%d %c\n\n", c, c);*/
	CheckerCountAssert = true; // Dont change this here. Do it in the tests and switch back after test is done.	
	system("chcp 437");
	SetDiceCombinations();
	InitSeed(&G, 100);
	InitAi(&AIs[0], true);
	InitAi(&AIs[1], true);
	InitHashes();
	InitGameConfig();
	StartPosition(&G);
	PrintGame(&G);

	printf("\n: ");
	char buf[BUF_SIZE];
	fgets(buf, 5000, stdin);
	while (!Streq(buf, "quit\n") && !Streq(buf, "q\n"))
	{
		if (Streq(buf, "test\n") || Streq(buf, "t\n")) {
			RunAllTests();
		}
		else if (Streq(buf, "s\n")) {
			RunSelectedTests();
		}
		else if (Streq(buf, "play\n") || Streq(buf, "p\n")) {
			AutoPlay();
		}
		else if (Streq(buf, "help\n") || Streq(buf, "h\n")) {
			PrintHelp();
		}
		else {
			ConsoleWriteLine("Unknown command");
			PrintHelp();
		}
		printf(": ");
		fgets(buf, 5000, stdin);
	}
	ConsoleWriteLine("Bye");
}
