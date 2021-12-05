#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <time.h>
#include <Windows.h>
#include "Utils.h"
#include "Game.h"
#include "Tests.h"
#include "Ai.h"

#pragma region TestsHelpers

int _failedAsserts = 0;

void Assert(int result, char* msg) {
	if (result == 0)
	{
		printf("\n");
		PrintRed(msg);
		printf("\n");
		_failedAsserts++;
	}
}

void AssertNot(int result, char* msg) {
	if (result != 0)
	{
		printf("\n");
		PrintRed(msg);
		printf("\n");
		_failedAsserts++;
	}
}

void AssertAreEqual(char* s1, char* s2, char* msg) {
	if (strcmp(s1, s2))
	{
		PrintRed(msg);
		printf("\n");
		printf("Expected: %s\n", s1);
		printf("Actual:   %s\n", s2);
		_failedAsserts++;
	}
}

void AssertAreEqualInts(int expected, int actual, char* msg) {
	if (expected != actual)
	{
		printf("\n");
		PrintRed(msg);
		printf("\n");
		char str[24];
		snprintf(str, 24, "Expected %d", expected);
		PrintRed(str);
		printf("\n");
		snprintf(str, 24, "Actual   %d", actual);
		PrintRed(str);
		printf("\n");
		_failedAsserts++;
	}
}

void AssertAreEqualLongs(U64 expected, U64 actual, char* msg) {
	if (expected != actual)
	{
		printf("\n");
		PrintRed(msg);
		printf("\n");
		char str[24];
		snprintf(str, 24, "Expected %llu", expected);
		PrintRed(str);
		printf("\n");
		snprintf(str, 24, "Actual   %llu", actual);
		PrintRed(str);
		_failedAsserts++;
	}
}

#pragma endregion


void Run(void (*test)(), char* name)
{
	printf("\nRunning %s took ", name);
	clock_t start = clock();
	test();
	printf("%fms", (float)(clock() - start) * 1000 / CLOCKS_PER_SEC);
}

void TestStartPos() {
	StartPosition(&G);
	Assert(G.Dice[0] == 0, "Dice 0 is not reset");
	Assert(G.Dice[1] == 0, "Dice 1 is not reset");
	AssertAreEqualInts(15, CountAllCheckers(Black, &G), "15 Black checkers expected");
	AssertAreEqualInts(15, CountAllCheckers(White, &G), "15 White checkers expected");
}

void TestRollDice() {
	for (size_t i = 0; i < 10; i++)
	{
		RollDice(&G);
		Assert(G.Dice[0] >= 1 && G.Dice[0] <= 6, "Invalid Dice value");
		Assert(G.Dice[1] >= 1 && G.Dice[1] <= 6, "Invalid Dice value");
	}
}

void TestWriteGameString() {
	StartPosition(&G);
	char s[100];
	WriteGameString(s, &G);

	char* expected = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	AssertAreEqual(expected, s, "Unexpected GameString");
}

void TestReadGameString() {
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gameString, &G);
	AssertAreEqualInts(G.Position[0], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[1], 2 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[2], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[3], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[4], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[5], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[6], 5 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[7], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[8], 3 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[9], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[10], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[11], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[12], 5 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[13], 5 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[14], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[15], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[16], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[17], 3 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[18], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[19], 5 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[20], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[21], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[22], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[23], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[24], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[25], 0, "Invalid checker count");
	AssertAreEqualInts(G.BlackHome, 0, "Invalid black home count");
	AssertAreEqualInts(G.WhiteHome, 0, "Invalid white home count");
}

void TestGameStringRountTrip() {

	char* gameString = "b2 b1 w3 0 0 0 w2 w1 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 w2 3 4";
	ReadGameString(gameString, &G);
	AssertAreEqualInts(G.Position[0], 2 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[1], 1 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[2], 3 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[3], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[4], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[5], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[6], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[7], 1 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[8], 3 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[9], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[10], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[11], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[12], 5 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[13], 5 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[14], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[15], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[16], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[17], 3 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[18], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[19], 5 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[20], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[21], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[22], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[23], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[24], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[25], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.WhiteHome, 3, "Invalid black home count");
	AssertAreEqualInts(G.BlackHome, 4, "Invalid white home count");

	char written[100];
	WriteGameString(written, &G);
	AssertAreEqual(gameString, written, "Read and written string not same");
}


void TestTwoDigitGameString() {
	char* gameString = "b2 b10 w3 0 0 0 w2 w1 w3 0 0 0 b5 w5 0 0 0 0 0 b3 0 0 0 0 w2 w2 3 4";
	ReadGameString(gameString, &G);
	AssertAreEqualInts(G.Position[0], 2 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[1], 10 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[2], 3 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[3], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[4], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[5], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[6], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[7], 1 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[8], 3 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[9], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[10], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[11], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[12], 5 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[13], 5 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[14], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[15], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[16], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[17], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[18], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[19], 3 | Black, "Invalid checker count");
	AssertAreEqualInts(G.Position[20], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[21], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[22], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[23], 0, "Invalid checker count");
	AssertAreEqualInts(G.Position[24], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.Position[25], 2 | White, "Invalid checker count");
	AssertAreEqualInts(G.WhiteHome, 3, "Invalid black home count");
	AssertAreEqualInts(G.BlackHome, 4, "Invalid white home count");
	char written[100];
	WriteGameString(written, &G);
	AssertAreEqual(gameString, written, "Strings differ");
}

void TestOtherColor() {
	AssertAreEqualInts(Black, OtherColor(White), "Other color should be Black");
	AssertAreEqualInts(White, OtherColor(Black), "Other color should be White");
}


void TestDoUndo() {
	StartPosition(&G);
	Move move;
	move.from = 1;
	move.to = 2;
	move.color = Black;
	bool hit = DoMove(move, &G);
	AssertAreEqualInts(1 | Black, G.Position[1], "Should be one black checker on 1");
	AssertAreEqualInts(1 | Black, G.Position[2], "Should be one black checker on 2");
	UndoMove(move, hit, &G);
	AssertAreEqualInts(2 | Black, G.Position[1], "Should be 2 black checkers on 1");
	AssertAreEqualInts(0, G.Position[2], "Should be no checker on 2");
}

void TestDoUndoHomeBlack() {
	CheckerCountAssert = false;
	ReadGameString("0 0 0 0 0 0 w2 w1 w3 0 0 0 0 w5 0 0 0 0 0 b5 0 0 0 0 w2 w2 0 0", &G);

	AssertAreEqualInts(5 | Black, G.Position[19], "Should be 5 black checkers on 19");
	AssertAreEqualInts(0, G.BlackHome, "Should be no black checker on Black Home");

	Move move;
	move.from = 19;
	move.to = 25;
	move.color = Black;
	bool hit = DoMove(move, &G);

	AssertAreEqualInts(4 | Black, G.Position[19], "Should be 4 black checkers on 19");
	AssertAreEqualInts(1, G.BlackHome, "Should be one black checker on Black Home");
	UndoMove(move, hit, &G);
	AssertAreEqualInts(5 | Black, G.Position[19], "Should be 5 black checkers on 19");
	AssertAreEqualInts(0, G.BlackHome, "Should be no black checker on Black Home");
}

void TestDoUndoHomeWhite() {
	ReadGameString("0 w1 0 0 w2 0 0 0 0 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0", &G);

	AssertAreEqualInts(2 | White, G.Position[4], "Should be 2 white checkers on 4");
	AssertAreEqualInts(0, G.WhiteHome, "Should be no white checker on White Home");

	Move move;
	move.from = 4;
	move.to = 0;
	move.color = White;
	bool hit = DoMove(move, &G);

	AssertAreEqualInts(1 | White, G.Position[4], "Should be 1 white checkers on 4");
	AssertAreEqualInts(1, G.WhiteHome, "Should be one white checker on White Home");
	UndoMove(move, hit, &G);
	AssertAreEqualInts(2 | White, G.Position[4], "Should be 2 white checkers on 4");
	AssertAreEqualInts(0, G.WhiteHome, "Should be no white checker on White Home");
	CheckerCountAssert = true;
}

void TestIsBlocked() {
	StartPosition(&G);

	Assert(IsBlockedFor(1, White, &G), "Pos 1 should be blocked for white");
	AssertNot(IsBlockedFor(1, Black, &G), "Pos 1 should not be blocked for black");

	AssertNot(IsBlockedFor(2, White, &G), "Pos 1 should not be blocked for white");
	AssertNot(IsBlockedFor(2, Black, &G), "Pos 1 should not be blocked for black");
}

void PrintMoves() {
	for (size_t i = 0; i < G.MoveSetsCount; i++)
	{
		MoveSet set = G.PossibleMoveSets[i];
		for (size_t j = 0; j < set.Length; j++)
			printf("%d-%d, ", set.Moves[j].from, set.Moves[j].to);
		printf("(%d)\n", set.Length);
	}
}

void TestSimpleBlack() {
	char* gameString = "0 b2 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.Dice[0] = 1;
	G.Dice[1] = 2;
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	CheckerCountAssert = true;
	//PrintMoves();
	AssertAreEqualInts(2, G.MoveSetsCount, "There should be 2 sets of moves.");
}

void TestCreateMovesBlackStart() {
	StartPosition(&G);
	G.CurrentPlayer = Black;
	G.Dice[0] = 3;
	G.Dice[1] = 4;
	CreateMoves(&G);
	char gs[100];
	WriteGameString(gs, &G);
	AssertAreEqual("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0", gs, "Game string should be start string.");
	AssertAreEqualInts(17, G.MoveSetsCount, "There should be 20 sets of moves.");
	//PrintMoves();
	// TODO: Assert moves
}

void TestCreateMovesWhiteStart() {
	StartPosition(&G);
	G.CurrentPlayer = White;
	G.Dice[0] = 3;
	G.Dice[1] = 4;
	CreateMoves(&G);
	char gs[100];
	WriteGameString(gs, &G);
	AssertAreEqual("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0", gs, "Game string should be start string.");
	AssertAreEqualInts(17, G.MoveSetsCount, "There should be 20 sets of moves.");
	for (int i = 0; i < G.MoveSetsCount; i++)
	{
		Assert(G.PossibleMoveSets[i].Length <= 4, "Invalid set length");
	}
}

void TestBlackCheckerOnBar() {
	CheckerCountAssert = false;
	char* gameString = "b1 b1 0 0 0 0 w5 0 w1 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 6;
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	CheckerCountAssert = true;
	//PrintMoves();
	AssertAreEqualInts(4, G.MoveSetsCount, "There should be 4 sets of moves.");
}

void TestWhiteCheckerOnBar() {
	CheckerCountAssert = false;
	char* gameString = "b1 b1 0 0 0 0 w5 0 w1 0 0 0 b5 w5 0 0 0 b1 0 b5 0 0 0 0 w1 w1 0 0";
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 6;
	G.CurrentPlayer = White;
	CreateMoves(&G);
	CheckerCountAssert = true;

	//PrintMoves();
	AssertAreEqualInts(4, G.MoveSetsCount, "There should be 4 sets of moves.");
}

void TestBearingOffBlack() {
	char* gameString = "0 0 0 0 0 0 w5 0 w1 0 0 0 0 w5 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 4;
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	CheckerCountAssert = true;

	//PrintMoves();
	AssertAreEqualInts(12, G.MoveSetsCount, "There should be 12 sets of moves.");
}

void TestBearingOffWhite() {
	char* gameString = "0 w2 w2 0 w2 w2 w5 0 0 0 0 0 0 0 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 4;
	G.CurrentPlayer = White;
	CreateMoves(&G);
	CheckerCountAssert = true;
	//PrintMoves();
	AssertAreEqualInts(12, G.MoveSetsCount, "There should be 12 sets of moves.");
}

void TestDoubleDiceBlack() {
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 2;
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	//PrintMoves();
	AssertAreEqualInts(538, G.MoveSetsCount, "There should be 538 sets of moves.");
	for (int i = 0; i < G.MoveSetsCount; i++)
		Assert(G.PossibleMoveSets[i].Length <= 4, "To many moves in set");
}

void TestDoubleDiceWhite() {
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 2;
	G.CurrentPlayer = White;
	CreateMoves(&G);
	//PrintMoves();
	AssertAreEqualInts(538, G.MoveSetsCount, "There should be 538 sets of moves.");
}

void PlayBothDiceIfPossible() {
	char* gameString = "0 0 b2 b2 0 0 w5 w3 0 b2 0 0 0 w5 0 0 0 0 0 b3 b2 0 b2 b2 w2 0 0 0";
	ReadGameString(gameString, &G);
	G.CurrentPlayer = White;
	G.Dice[0] = 4;
	G.Dice[1] = 6;
	CreateMoves(&G);
	AssertAreEqualInts(1, G.MoveSetsCount, "There should be 1 set of moves.");
	//PrintMoves();
}

void TestRemoveShorterSets() {

	char* gameString = "0 0 0 0 0 0 w5 0 w1 0 0 0 0 w5 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.Dice[0] = 2;
	G.Dice[1] = 4;
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	AssertAreEqualInts(12, G.MoveSetsCount, "There should be 12 moves");

	//PrintMoves();
	G.PossibleMoveSets[2].Length = 1;
	G.PossibleMoveSets[5].Length = 1;
	RemoveShorterSets(2, &G);
	CheckerCountAssert = true;
	AssertAreEqualInts(10, G.MoveSetsCount, "There should be 10 moves left");
	/*ConsoleWriteLine("==================");
	PrintMoves();*/
}

void TestEvaluation() {
	char* gameString = "0 0 b2 b2 0 0 w5 w3 0 b2 0 0 0 w5 0 0 0 0 0 b3 b2 0 b2 b2 w2 0 0 0";
	ReadGameString(gameString, &G);
	InitAi(true);
	double score = EvaluateCheckers(&G, Black);
}

void TestPointsLeft() {
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gs, &G);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expected 167 Points left for Black");
	AssertAreEqualInts(167, (int)G.WhiteLeft, "Expected 167 Points left for White");

	Move bm;
	bm.from = 1;
	bm.to = 5;
	bm.color = Black;
	bool hit = DoMove(bm, &G);
	AssertAreEqualInts(163, (int)G.BlackLeft, "Expected 163 Points left for Black");
	AssertAreEqualInts(167, (int)G.WhiteLeft, "Expected 167 Points left for White");

	UndoMove(bm, hit, &G);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expected 167 Points left for Black");
	AssertAreEqualInts(167, (int)G.WhiteLeft, "Expected 167 Points left for White");

	Move wm;
	wm.from = 24;
	wm.to = 20;
	wm.color = White;
	hit = DoMove(wm, &G);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expcted 167 Points left for Black");
	AssertAreEqualInts(163, (int)G.WhiteLeft, "Expcted 163 Points left for White");

	UndoMove(wm, hit, &G);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expcted 167 Points left for Black");
	AssertAreEqualInts(167, (int)G.WhiteLeft, "Expcted 167 Points left for White");
}

void TestPointsLeftHit() {
	char* gs = "0 b2 0 0 0 0 w1 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b1 0 0 0 0 w2 0 0 0";
	CheckerCountAssert = false;
	ReadGameString(gs, &G);
	AssertAreEqualInts(143, (int)G.BlackLeft, "Expcted 143 Points left for Black");
	AssertAreEqualInts(143, (int)G.WhiteLeft, "Expcted 143 Points left for White");
	Move bm;
	bm.from = 1;
	bm.to = 6;
	bm.color = Black;
	bool hit = DoMove(bm, &G);
	AssertAreEqualInts(138, (int)G.BlackLeft, "Expcted 138 Points left for Black");
	AssertAreEqualInts(143 - 6 + 25, (int)G.WhiteLeft, "Expcted 164 Points left for White");
	UndoMove(bm, hit, &G);
	AssertAreEqualInts(143, (int)G.BlackLeft, "Expcted 143 Points left for Black");
	AssertAreEqualInts(143, (int)G.WhiteLeft, "Expcted 143 Points left for White");
	//TODO, wm

	Move wm;
	wm.from = 24;
	wm.to = 19;
	wm.color = White;
	hit = DoMove(wm, &G);
	AssertAreEqualInts(143 - 6 + 25, (int)G.BlackLeft, "Expcted 164 Points left for Black");
	AssertAreEqualInts(138, (int)G.WhiteLeft, "Expcted 138 Points left for White");
	UndoMove(wm, hit, &G);
	CheckerCountAssert = true;

	AssertAreEqualInts(143, (int)G.BlackLeft, "Expcted 143 Points left for Black");
	AssertAreEqualInts(143, (int)G.WhiteLeft, "Expcted 143 Points left for White");
}

void TestGetScore() {
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gs, &G);
	InitAi(true);
	double score = GetScore(&G);
	printf("Score: %f\n", score);
}

void TestPrintGame() {
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gs, &G);
	PrintGame(&G);
}

void TestPlayGame() {
	PlayGame(&G);
}

void TestBestBearingOffBlack() {
	char* gs = "0 0 w6 w2 w2 w2 w3 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 b5 b5 b1 0 w0 0 0";
	ReadGameString(gs, &G);
	G.CurrentPlayer = Black;
	InitAi(true);
	G.Dice[0] = 3;
	G.Dice[1] = 2;
	double score = 0;
	MoveSet set = FindBestMoveSet(&G, &score, 0);
	Move* m = set.Moves;
	for (int j = 0; j < set.Length; j++)
		AssertAreEqualInts(25, m[j].to, "Move should be to 25, Black Home");
}

void TestBestBearingOffWhite() {
	char* gs = "0 0 w6 w2 w2 w2 w3 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 b5 b5 b1 0 w0 0 0";
	ReadGameString(gs, &G);
	G.CurrentPlayer = White;
	InitAi(true);
	G.Dice[0] = 3;
	G.Dice[1] = 2;
	double score = 0;
	MoveSet set = FindBestMoveSet(&G, &score, 0);
	Move* m = set.Moves;
	for (int j = 0; j < set.Length; j++)
		AssertAreEqualInts(0, m[j].to, "Move should be to 0, White Home");
}

void TestManyCombos() {
	char* gs = "0 0 w3 w2 w3 0 w2 0 b2 0 0 0 0 0 0 0 w1 w2 b2 b2 0 w2 b2 b5 b2 0 0 0";
	ReadGameString(gs, &G);
	G.Dice[0] = 1;
	G.Dice[1] = 1;
	G.CurrentPlayer = Black;
	CreateMoves(&G);

	AssertAreEqualInts(710, G.MoveSetsCount, "There should be 710 sets of moves.");
	for (int i = 0; i < G.MoveSetsCount; i++)
		Assert(G.PossibleMoveSets[i].Length <= 4, "To many moves in set");
}

void TestNastyBug() {
	char* gs = "0 b2 w2 0 b1 0 w4 0 w2 w2 0 0 b3 w4 0 0 0 b2 0 b4 0 0 0 b3 w1 w1 0 0";
	ReadGameString(gs, &G);
	Move m;
	m.from = 25;
	m.to = 20;
	m.color = White;
	CheckerCountAssert = false;
	DoMove(m, &G);
	CheckerCountAssert = true;
}

void TestBestMoveBlack1() {
	StartPosition(&G);
	G.CurrentPlayer = Black;
	double score = 0;
	G.Dice[0] = 1;
	G.Dice[1] = 6;
	MoveSet ms = FindBestMoveSet(&G, &score, 1);
	AssertAreEqualInts(2, ms.Length, "There should be 2 moves");
	// 12-18, 17-18
	Assert(ms.Moves[0].from == 12 && ms.Moves[0].to == 18, "Move should be 12 - 18");
	Assert(ms.Moves[1].from == 17 && ms.Moves[1].to == 18, "Move should be 12 - 18");
}

void TestBestMoveWhite1() {
	StartPosition(&G);
	G.CurrentPlayer = White;
	double score = 0;
	G.Dice[0] = 1;
	G.Dice[1] = 6;
	MoveSet ms = FindBestMoveSet(&G, &score, 1);
	AssertAreEqualInts(2, ms.Length, "There should be 2 moves");

	Assert(ms.Moves[0].from == 13 && ms.Moves[0].to == 7, "Move should be 13 - 7");
	Assert(ms.Moves[1].from == 8 && ms.Moves[1].to == 7, "Move should be 8 - 7");
}

void TestHitBlot() {

}

// 0 w3 w4 w6 0 0 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 w1 b3 b4 b4 w1 0 0 0
// Dice 5, 3
// 18 - 23, 18 - 21

void RunAll() {
	Run(TestNastyBug, "TestNastyBug");
	Run(TestStartPos, "TestStartPos");
	Run(TestRollDice, "TestRollDice");
	Run(TestWriteGameString, "TestWriteGameString");
	Run(TestReadGameString, "TestReadGameString");
	Run(TestTwoDigitGameString, "TestTwoDigitGameString");
	Run(TestGameStringRountTrip, "TestGameStringRountTrip");
	Run(TestDoUndo, "TestDoUndo");
	Run(TestIsBlocked, "TestIsBlocked");
	Run(TestSimpleBlack, "TestSimpleBlack");
	Run(TestDoUndoHomeBlack, "TestDoUndoHomeBlack");
	Run(TestDoUndoHomeWhite, "TestDoUndoHomeWhite");
	Run(TestCreateMovesBlackStart, "TestCreateMovesBlackStart");
	Run(TestCreateMovesWhiteStart, "TestCreateMovesWhiteStart");
	Run(TestOtherColor, "TestOtherColor");
	Run(TestBlackCheckerOnBar, "TestBlackCheckerOnBar");
	Run(TestWhiteCheckerOnBar, "TestWhiteCheckerOnBar");
	Run(TestBearingOffBlack, "TestBearingOffBlack");
	Run(TestBearingOffWhite, "TestBearingOffWhite");
	Run(TestDoubleDiceBlack, "TestDoubleDiceBlack");
	Run(TestDoubleDiceWhite, "TestDoubleDiceWhite");
	Run(PlayBothDiceIfPossible, "PlayBothDiceIfPossible");
	Run(TestRemoveShorterSets, "TestRemoveShorterSets");
	Run(TestEvaluation, "TestEvaluation");
	Run(TestPointsLeft, "TestPointsLeft");
	Run(TestPointsLeftHit, "TestPointsLeftHit");
	Run(TestGetScore, "TestGetScore");
	//Run(TestPrintGame(), "TestPrintGame");
	Run(TestBestBearingOffBlack, "TestBestBearingOffBlack");
	Run(TestBestBearingOffWhite, "TestBestBearingOffWhite");
	Run(TestManyCombos, "TestManyCombos");
	Run(TestBestMoveBlack1, "TestBestMoveBlack1");
	Run(TestBestMoveWhite1, "TestBestMoveWhite1");
	// TODO: Test Blocked -> zero moves.

	if (_failedAsserts == 0)
		PrintGreen("\nSuccess! Tests are good!\n");
	else
		PrintRed("\nThere are failed tests.\n");
}