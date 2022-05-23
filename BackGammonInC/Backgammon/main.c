
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <Windows.h>
#include <tchar.h>

#include "Game.h"
#include "utils.h"
#include "Tests.h"
#include "Ai.h"
#include "main.h"
#include "Hash.h"
#include "Trainer.h"

bool Searching;

void PrintHelp() {
	ConsoleWriteLine("\nOptions\n-------\nt/test\np/play\npos w2 0 0 0 b5... (game string)\ns/selected tests\ng/game\nh/help\nq/quit");
}

void PrintCurrentDir() {
	TCHAR c[500];
	GetCurrentDirectory(500, c);
	_tprintf(TEXT("\nDir: %s\n"), c);
}


void PrintBest() {
	if (Searching) {
		printf("AI busy searcing\n");
		fflush(stdout);
		return;
	}
	MoveSet set;	
	set.Length = 0;
	Searching = true;
	InitAiManual(&AIs[0]);
	InitAiManual(&AIs[1]);
	int depth = G.Dice[0] == G.Dice[1] ? 1 : 1;
	int setIdx = FindBestMoveSet(&G, &set, depth);

	fflush(stdout);	
	PrintSet(set);	
	Searching = false;
}

int main() {
	system("cls");

	seedRand(1234, &g_Rand);

	//Default values
	Settings.DiceQuads = 4;
	Settings.MaxTurns = 2000;
	Settings.PausePlay = false;
	Settings.SearchDepth = 1;
	Searching = false;
	
	printf("Welcome to backgammon\n: ");

	/*for (int c = 170; c < 255; c++)
		printf("%d %c\n\n", c, c);*/
	CheckerCountAssert = true; // Dont change this here. Do it in the tests and switch back after test is done.	
	// Changing to a nice code page.
	system("chcp 437");
	SetDiceCombinations();
	InitSeed(&G, 100);
	InitAi(&AIs[0], true);
	InitAi(&AIs[1], true);
	InitHashes();
	StartPosition(&G);
	G.Dice[0] = 3;
	G.Dice[1] = 5;
	//PrintGame(&G);
	//PrintCurrentDir();
	printf("ready\n");
	printf("\n: ");
	fflush(stdout);

	char buf[BUF_SIZE];
	fgets(buf, BUF_SIZE, stdin);
	while (!Streq(buf, "quit\n") && !Streq(buf, "q\n"))
	{
		if (Streq(buf, "test\n") || Streq(buf, "t\n")) {
			RunAllTests();
		}
		else if (Streq(buf, "s\n")) {
			RunSelectedTests();
		}
		else if (Streq(buf, "play\n") || Streq(buf, "p\n")) {			
			PlayAndEvaluate();
		}
		else if (StartsWith(buf, "board ")) {					
			ReadGameString(&buf[6], &G);
			PrintBest();
		}
		else if (Streq(buf, "game\n") || Streq(buf, "g\n") ) {
			PrintGame(&G);
		}
		else if (Streq(buf, "search\n")) {
			printf("searching...\n");
			fflush(stdout);
			PrintBest();			
		}
		else if (Streq(buf, "w\n") || Streq(buf, "watch\n")) {
			WatchGame();
		}
		else if (Streq(buf, "help\n") || Streq(buf, "h\n")) {
			PrintHelp();
		}
		else {
			ConsoleWriteLine("Unknown command");
			PrintHelp();
		}
		printf(": ");
		fflush(stdout);
		fgets(buf, BUF_SIZE, stdin);
	}
	char heart = 3;
	printf("Bye %c", heart);
}
