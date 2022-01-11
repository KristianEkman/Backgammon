#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <time.h>
#include <Windows.h>
#include "Utils.h"
#include "Game.h"
#include "Tests.h"
#include "Ai.h"
#include "Trainer.h"

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
	printf("\n%s ", name);
	clock_t start = clock();
	test();
	printf("%.2fms", (float)(clock() - start) * 1000 / CLOCKS_PER_SEC);
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

	char* expected = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 0 0";
	AssertAreEqual(expected, s, "Unexpected GameString");
}

void TestReadGameString() {
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 4 3";
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
	AssertAreEqualInts(G.CurrentPlayer, Black, "Invalid current player");
	AssertAreEqualInts(G.Dice[0], 4, "Invalid dice 1");
	AssertAreEqualInts(G.Dice[1], 3, "Invalid dice 2");
}

void TestGameStringRountTrip() {

	char* gameString = "b2 b1 w3 0 0 0 w2 w1 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 w2 3 4 w 5 6";
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
	AssertAreEqualInts(G.CurrentPlayer, White, "Invalid current player");
	AssertAreEqualInts(G.Dice[0], 5, "Invalid dice 1");
	AssertAreEqualInts(G.Dice[1], 6, "Invalid dice 2");

	char written[100];
	WriteGameString(written, &G);
	AssertAreEqual(gameString, written, "Read and written string not same");
}

void TestTwoDigitGameString() {
	char* gameString = "b2 b10 w3 0 0 0 w2 w1 w3 0 0 0 b5 w5 0 0 0 0 0 b3 0 0 0 0 w2 w2 3 4 w 1 1";
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
	AssertAreEqualInts(G.Dice[0], 1, "Invalid dice 1");
	AssertAreEqualInts(G.Dice[1], 1, "Invalid dice 2");
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
	UndoMove(move, hit, &G, 0);
	AssertAreEqualInts(2 | Black, G.Position[1], "Should be 2 black checkers on 1");
	AssertAreEqualInts(0, G.Position[2], "Should be no checker on 2");
}

void TestDoUndoHomeBlack() {
	CheckerCountAssert = false;
	ReadGameString("0 0 0 0 0 0 w2 w1 w3 0 0 0 0 w5 0 0 0 0 0 b5 0 0 0 0 w2 w2 0 0 b 0 0", &G);

	AssertAreEqualInts(5 | Black, G.Position[19], "Should be 5 black checkers on 19");
	AssertAreEqualInts(0, G.BlackHome, "Should be no black checker on Black Home");

	Move move;
	move.from = 19;
	move.to = 25;
	move.color = Black;
	bool hit = DoMove(move, &G);

	AssertAreEqualInts(4 | Black, G.Position[19], "Should be 4 black checkers on 19");
	AssertAreEqualInts(1, G.BlackHome, "Should be one black checker on Black Home");
	UndoMove(move, hit, &G, 0);
	AssertAreEqualInts(5 | Black, G.Position[19], "Should be 5 black checkers on 19");
	AssertAreEqualInts(0, G.BlackHome, "Should be no black checker on Black Home");
}

void TestDoUndoHomeWhite() {
	ReadGameString("0 w1 0 0 w2 0 0 0 0 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 0 0", &G);

	AssertAreEqualInts(2 | White, G.Position[4], "Should be 2 white checkers on 4");
	AssertAreEqualInts(0, G.WhiteHome, "Should be no white checker on White Home");

	Move move;
	move.from = 4;
	move.to = 0;
	move.color = White;
	bool hit = DoMove(move, &G);

	AssertAreEqualInts(1 | White, G.Position[4], "Should be 1 white checkers on 4");
	AssertAreEqualInts(1, G.WhiteHome, "Should be one white checker on White Home");
	UndoMove(move, hit, &G, 0);
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
	char* gameString = "0 b2 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 b 0 0";
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
	AssertAreEqual("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 4 3", gs, "Game string should be start string.");
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
	AssertAreEqual("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 w 4 3", gs, "Game string should be start string.");
	AssertAreEqualInts(17, G.MoveSetsCount, "There should be 20 sets of moves.");
	for (int i = 0; i < G.MoveSetsCount; i++)
	{
		Assert(G.PossibleMoveSets[i].Length <= 4, "Invalid set length");
	}
}

void TestBlackCheckerOnBar() {
	CheckerCountAssert = false;
	char* gameString = "b1 b1 0 0 0 0 w5 0 w1 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 6 2";
	ReadGameString(gameString, &G);
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	CheckerCountAssert = true;
	//PrintMoves();
	AssertAreEqualInts(4, G.MoveSetsCount, "There should be 4 sets of moves.");
}

void TestWhiteCheckerOnBar() {
	CheckerCountAssert = false;
	char* gameString = "b1 b1 0 0 0 0 w5 0 w1 0 0 0 b5 w5 0 0 0 b1 0 b5 0 0 0 0 w1 w1 0 0 b 6 2";
	ReadGameString(gameString, &G);
	G.CurrentPlayer = White;
	CreateMoves(&G);
	CheckerCountAssert = true;

	//PrintMoves();
	AssertAreEqualInts(4, G.MoveSetsCount, "There should be 4 sets of moves.");
}

void TestBearingOffBlack() {
	char* gameString = "0 0 0 0 0 0 w5 0 w1 0 0 0 0 w5 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0 b 4 2";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	CheckerCountAssert = true;
	//PrintGame(&G);
	//PrintMoves();
	AssertAreEqualInts(11, G.MoveSetsCount, "There should be 11 sets of moves.");
}

void TestBearingOffWhite() {
	char* gameString = "0 w2 w2 0 w2 w2 w5 0 0 0 0 0 0 0 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0 w 4 2";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.CurrentPlayer = White;
	CreateMoves(&G);
	CheckerCountAssert = true;
	//PrintMoves();
	AssertAreEqualInts(11, G.MoveSetsCount, "There should be 11 sets of moves.");
}

void TestDoubleDiceBlack() {
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 2 2";
	ReadGameString(gameString, &G);
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	//PrintMoves();
	if (Settings.DiceQuads == 4) {
		AssertAreEqualInts(76, G.MoveSetsCount, "There should be 76 sets of moves.");
		for (int i = 0; i < G.MoveSetsCount; i++)
			Assert(G.PossibleMoveSets[i].Length <= 4, "To many moves in set");
	}
}

void TestDoubleDiceWhite() {
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 w 2 2";
	ReadGameString(gameString, &G);
	G.CurrentPlayer = White;
	CreateMoves(&G);
	//PrintMoves();
	if (Settings.DiceQuads == 4) {
		AssertAreEqualInts(76, G.MoveSetsCount, "There should be 76 sets of moves.");
	}
}

void PlayBothDiceIfPossible() {
	char* gameString = "0 0 b2 b2 0 0 w5 w3 0 b2 0 0 0 w5 0 0 0 0 0 b3 b2 0 b2 b2 w2 0 0 0 w 4 6";
	ReadGameString(gameString, &G);
	G.CurrentPlayer = White;
	CreateMoves(&G);
	AssertAreEqualInts(1, G.MoveSetsCount, "There should be 1 set of moves.");
	//PrintMoves();
}

void TestRemoveShorterSets() {

	char* gameString = "0 0 0 0 0 0 w5 0 w1 0 0 0 0 w5 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0 b 2 4";
	CheckerCountAssert = false;
	ReadGameString(gameString, &G);
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	AssertAreEqualInts(11, G.MoveSetsCount, "There should be 11 moves");

	//PrintMoves();
	G.PossibleMoveSets[2].Length = 1;
	G.PossibleMoveSets[5].Length = 1;
	RemoveShorterSets(2, &G);
	CheckerCountAssert = true;
	AssertAreEqualInts(9, G.MoveSetsCount, "There should be 9 moves left");
	/*ConsoleWriteLine("==================");
	PrintMoves();*/
}

void TestEvaluation() {
	char* gameString = "0 0 b2 b2 0 0 w5 w3 0 b2 0 0 0 w5 0 0 0 0 0 b3 b2 0 b2 b2 w2 0 0 0 b 0 0";
	ReadGameString(gameString, &G);
	InitAi(&AIs[0], true);
	InitAi(&AIs[1], true);
	double score = EvaluateCheckers(&G, Black);
}

void TestPointsLeft() {
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 0 0";
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

	UndoMove(bm, hit, &G, 0);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expected 167 Points left for Black");
	AssertAreEqualInts(167, (int)G.WhiteLeft, "Expected 167 Points left for White");

	Move wm;
	wm.from = 24;
	wm.to = 20;
	wm.color = White;
	hit = DoMove(wm, &G);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expcted 167 Points left for Black");
	AssertAreEqualInts(163, (int)G.WhiteLeft, "Expcted 163 Points left for White");

	UndoMove(wm, hit, &G, 0);
	AssertAreEqualInts(167, (int)G.BlackLeft, "Expcted 167 Points left for Black");
	AssertAreEqualInts(167, (int)G.WhiteLeft, "Expcted 167 Points left for White");
}

void TestPointsLeftHit() {
	char* gs = "0 b2 0 0 0 0 w1 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b1 0 0 0 0 w2 0 0 0 b 0 0";
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
	UndoMove(bm, hit, &G, 0);
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
	UndoMove(wm, hit, &G, 0);
	CheckerCountAssert = true;

	AssertAreEqualInts(143, (int)G.BlackLeft, "Expcted 143 Points left for Black");
	AssertAreEqualInts(143, (int)G.WhiteLeft, "Expcted 143 Points left for White");
}

void TestGetScore() {
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 0 0";
	ReadGameString(gs, &G);
	InitAi(&AIs[0], true);
	InitAi(&AIs[1], true);
	double score = GetScore(&G);
	printf("Score: %f ", score);
}

void TestPrintGame() {
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 6 6";
	ReadGameString(gs, &G);
	PrintGame(&G);
}

void TestPlayGame() {
	PlayGame(&G);
}

void TestBestBearingOffBlack() {
	char* gs = "0 0 w6 w2 w2 w2 w3 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 b5 b5 b1 0 w0 0 0 b 3 2";
	ReadGameString(gs, &G);
	InitAi(&AIs[0], true);
	InitAi(&AIs[1], true);
	double score = 0;
	MoveSet set;
	FindBestMoveSet(&G, &set, 1);
	Move* m = set.Moves;
	for (int j = 0; j < set.Length; j++)
		AssertAreEqualInts(25, m[j].to, "Move should be to 25, Black Home");
}

void TestBestBearingOffWhite() {
	char* gs = "0 0 w6 w2 w2 w2 w3 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 b5 b5 b1 0 w0 0 0 w 3 2";
	ReadGameString(gs, &G);
	InitAi(&AIs[0], true);
	InitAi(&AIs[1], true);
	double score = 0;
	MoveSet set;
	FindBestMoveSet(&G, &set, 1);
	Move* m = set.Moves;
	for (int j = 0; j < set.Length; j++)
		AssertAreEqualInts(0, m[j].to, "Move should be to 0, White Home");
}

void TestManyCombos() {
	char* gs = "0 0 w3 w2 w3 0 w2 0 b2 0 0 0 0 0 0 0 w1 w2 b2 b2 0 w2 b2 b5 b2 0 0 0 b 1 1";
	ReadGameString(gs, &G);
	G.CurrentPlayer = Black;
	CreateMoves(&G);
	if (Settings.DiceQuads == 4) {
		AssertAreEqualInts(72, G.MoveSetsCount, "There should be 72 sets of moves.");
		for (int i = 0; i < G.MoveSetsCount; i++)
			Assert(G.PossibleMoveSets[i].Length <= 4, "To many moves in set");
	}
}

void TestNastyBug() {
	char* gs = "0 b2 w2 0 b1 0 w4 0 w2 w2 0 0 b3 w4 0 0 0 b2 0 b4 0 0 0 b3 w1 w1 0 0 w 0 0";
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
	G.Dice[0] = 1;
	G.Dice[1] = 6;
	MoveSet ms;
	FindBestMoveSet(&G, &ms, 1);
	AssertAreEqualInts(2, ms.Length, "There should be 2 moves");
	// 12-18, 17-18
	Assert(ms.Moves[0].from == 12 && ms.Moves[0].to == 18, "Move should be 12 - 18");
	Assert(ms.Moves[1].from == 17 && ms.Moves[1].to == 18, "Move should be 17 - 18");
}

void TestBestMoveWhite1() {
	StartPosition(&G);
	G.CurrentPlayer = White;
	double score = 0;
	G.Dice[0] = 1;
	G.Dice[1] = 6;
	MoveSet ms;
	FindBestMoveSet(&G, &ms, 1);
	AssertAreEqualInts(2, ms.Length, "There should be 2 moves");

	Assert(ms.Moves[0].from == 13 && ms.Moves[0].to == 7, "Move should be 13 - 7");
	Assert(ms.Moves[1].from == 8 && ms.Moves[1].to == 7, "Move should be 8 - 7");
}

void Performance() {
	char* gs = "0 b2 w2 w3 0 0 w6 0 0 0 w1 w1 b2 0 0 0 b1 b1 b2 b4 0 0 b3 0 w2 0 0 0 b 2 2";
	ReadGameString(gs, &G);
	MoveSet set;
	G.EvalCounts = 0;
	time_t start = clock();
	FindBestMoveSet(&G, &set, 2);
	float ellapsed = (float)(clock() - start) / CLOCKS_PER_SEC;
	printf("Eval count: %d - (%.1fk evs/sec) ", G.EvalCounts, G.EvalCounts / ellapsed / 1000);
}

void TestHashing() {
	StartPosition(&G);
	Move m1;
	m1.color = Black;
	m1.from = 1;
	m1.to = 4;
	U64 hash1 = G.Hash;
	DoMove(m1, &G);

	Assert(hash1 != G.Hash, "Hash should have changed after a move");

	Move m2;
	m2.color = Black;
	m2.from = 4;
	m2.to = 1;
	DoMove(m2, &G);
	Assert(hash1 == G.Hash, "Hash should have changed back");
}

void TestHashingHit() {
	ReadGameString("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 w1 w1 0 0 0 w 1 1", &G);
	Move move;
	move.from = 19;
	move.to = 23;
	move.color = Black;
	//ConsoleWriteLine("DoMove");
	DoMove(move, &G);
	U64 hash1 = G.Hash;
	char gs[100];
	WriteGameString(gs, &G);
	//ConsoleWriteLine("Read game string");
	ReadGameString(gs, &G);
	//printf("%s\n", gs);
	Assert(hash1 == G.Hash, "Hash after hit move differ");
}

void TestHashingWhiteTwoMoves() {
	ReadGameString("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 w 1 1", &G);
	Move move;
	move.from = 24;
	move.to = 23;
	move.color = White;
	//ConsoleWriteLine("DoMove");
	DoMove(move, &G);
	DoMove(move, &G);
	U64 hash1 = G.Hash;
	char gs[100];
	WriteGameString(gs, &G);
	//ConsoleWriteLine("Read game string");
	ReadGameString(gs, &G);
	//printf("%s\n", gs);
	Assert(hash1 == G.Hash, "Hash after hit move differ");
}

void TestNoMoves() {
	ReadGameString("0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b2 b2 b2 b2 b2 b2 w1 0 0 w 3 1", &G);
	CheckerCountAssert = false;	
	MoveSet ms;
	int r = FindBestMoveSet(&G, &ms, 1);
	AssertAreEqualInts(-1, r, "Expected no moves");
	AssertAreEqualInts(0, G.MoveSetsCount, "Expected no moves");
	CheckerCountAssert = true;
}

void TestTraining() {
	Train();
}

void TestNewGeneration() {
	InitTrainer();
	NewGeneration();
}

void TestLoadSave() {	
	InitTrainer();
	double d1 = Trainer.Set[0].BlotFactors[0];
	double d2 = Trainer.Set[4].BlotFactors[4];
	double d3 = Trainer.Set[7].BlotFactors[25];
	SaveTrainedSet(9999, "Test");

	for (size_t i = 0; i < TrainedSetCount; i++)
	{
		for (size_t j = 0; j < 26; j++)
		{
			Trainer.Set[i].ConnectedBlocksFactor[j] = 999;
			Trainer.Set[i].BlotFactors[j] = 999;
		}
	}

	LoadTrainedSet("Test");
	Assert(d1 == Trainer.Set[0].BlotFactors[0], "File serialization failed 1");
	Assert(d2 == Trainer.Set[4].BlotFactors[4], "File serialization failed 2");
	Assert(d3 == Trainer.Set[7].BlotFactors[25], "File serialization failed 3");
}

void RunSelectedTests() {
	_failedAsserts = 0;
	Run(TestTraining, "TrainParallel");

	if (_failedAsserts == 0)
		PrintGreen("\nSuccess! Tests are good!\n");
	else
		PrintRed("\nThere are failed tests.\n");
}

void RunAllTests() {
	_failedAsserts = 0;
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
	Run(TestHashing, "TestHashing");
	Run(TestHashingHit, "TestHashingHit");
	Run(TestHashingWhiteTwoMoves, "TestHashingWhiteTwoMoves");
	Run(TestNoMoves, "TestNoMoves");
	Run(TestNewGeneration, "TestNewGeneration");
	Run(TestLoadSave, "TestLoadSave");

	Run(Performance, "Performance");

	// TODO: Test Blocked -> zero moves.

	if (_failedAsserts == 0)
		PrintGreen("\nSuccess! Tests are good!\n");
	else
		PrintRed("\nThere are failed tests.\n");
}