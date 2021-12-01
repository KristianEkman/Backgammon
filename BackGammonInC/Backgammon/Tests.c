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

TEST(TestStartPos,
	StartPosition(&G);
Assert(G.Dice[0] == 0, "Dice 0 is not reset");
Assert(G.Dice[1] == 0, "Dice 1 is not reset");
AssertAreEqualInts(15, CountAllCheckers(Black, &G), "15 Black checkers expected");
AssertAreEqualInts(15, CountAllCheckers(White, &G), "15 White checkers expected");
)

TEST(TestRollDice,
	for (size_t i = 0; i < 10; i++)
	{
		RollDice(&G);
		Assert(G.Dice[0] >= 1 && G.Dice[0] <= 6, "Invalid Dice value");
		Assert(G.Dice[1] >= 1 && G.Dice[1] <= 6, "Invalid Dice value");
	}
)

TEST(TestWriteGameString,
	StartPosition(&G);
char s[100];
WriteGameString(s, &G);

char* expected = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
AssertAreEqual(expected, s, "Unexpected GameString");
)

TEST(TestReadGameString,
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
)

TEST(TestGameStringRountTrip,

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
)


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

TEST(TestOtherColor,
	AssertAreEqualInts(Black, OtherColor(White), "Other color should be Black");
AssertAreEqualInts(White, OtherColor(Black), "Other color should be White");
)


TEST(TestDoUndo,
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
)

TEST(TestDoUndoHomeBlack,
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
)

TEST(TestDoUndoHomeWhite,
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
)

TEST(TestIsBlocked,
	StartPosition(&G);

Assert(IsBlockedFor(1, White, &G), "Pos 1 should be blocked for white");
AssertNot(IsBlockedFor(1, Black, &G), "Pos 1 should not be blocked for black");

AssertNot(IsBlockedFor(2, White, &G), "Pos 1 should not be blocked for white");
AssertNot(IsBlockedFor(2, Black, &G), "Pos 1 should not be blocked for black");
)

void PrintMoves() {
	for (size_t i = 0; i < G.MoveSetsCount; i++)
	{
		for (size_t j = 0; j < G.SetLengths[i]; j++)
			printf("%d-%d, ", G.PossibleMoveSets[i].Moves[j].from, G.PossibleMoveSets[i].Moves[j].to);
		printf("(%d)\n", G.SetLengths[i]);
	}
}

TEST(TestSimpleBlack,
	char* gameString = "0 b2 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 1;
G.Dice[1] = 2;
G.CurrentPlayer = Black;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(2, G.MoveSetsCount, "There should be 2 sets of moves.");
)

TEST(TestCreateMovesBlackStart,
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
)

TEST(TestCreateMovesWhiteStart,
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
	Assert(G.SetLengths[i] <= 4, "Invalid set length");
}
)

TEST(TestBlackCheckerOnBar,
	char* gameString = "b1 b1 0 0 0 0 w5 0 w1 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 6;
G.CurrentPlayer = Black;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(4, G.MoveSetsCount, "There should be 4 sets of moves.");
)

TEST(TestWhiteCheckerOnBar,
	char* gameString = "b1 b1 0 0 0 0 w5 0 w1 0 0 0 b5 w5 0 0 0 b1 0 b5 0 0 0 0 w1 w1 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 6;
G.CurrentPlayer = White;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(4, G.MoveSetsCount, "There should be 4 sets of moves.");
)

TEST(TestBearingOffBlack,
	char* gameString = "0 0 0 0 0 0 w5 0 w1 0 0 0 0 w5 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 4;
G.CurrentPlayer = Black;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(12, G.MoveSetsCount, "There should be 12 sets of moves.");
)

TEST(TestBearingOffWhite,
	char* gameString = "0 w2 w2 0 w2 w2 w5 0 0 0 0 0 0 0 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 4;
G.CurrentPlayer = White;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(12, G.MoveSetsCount, "There should be 12 sets of moves.");
)

TEST(TestDoubleDiceBlack,
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 2;
G.CurrentPlayer = Black;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(538, G.MoveSetsCount, "There should be 538 sets of moves.");
for (int i = 0; i < G.MoveSetsCount; i++)
	Assert(G.SetLengths[i] <= 4, "To many moves in set");
)

TEST(TestDoubleDiceWhite,
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 2;
G.CurrentPlayer = White;
CreateMoves(&G);
//PrintMoves();
AssertAreEqualInts(538, G.MoveSetsCount, "There should be 538 sets of moves.");
)

TEST(PlayBothDiceIfPossible,
	char* gameString = "0 0 b2 b2 0 0 w5 w3 0 b2 0 0 0 w5 0 0 0 0 0 b3 b2 0 b2 b2 w2 0 0 0";
ReadGameString(gameString, &G);
G.CurrentPlayer = White;
G.Dice[0] = 4;
G.Dice[1] = 6;
CreateMoves(&G);
AssertAreEqualInts(1, G.MoveSetsCount, "There should be 1 set of moves.");
//PrintMoves();
)

TEST(TestRemoveShorterSets,

	char* gameString = "0 0 0 0 0 0 w5 0 w1 0 0 0 0 w5 0 0 0 0 0 b5 b2 b2 0 b2 b2 0 0 0";
ReadGameString(gameString, &G);
G.Dice[0] = 2;
G.Dice[1] = 4;
G.CurrentPlayer = Black;
CreateMoves(&G);
AssertAreEqualInts(12, G.MoveSetsCount, "There should be 12 moves");

//PrintMoves();
G.SetLengths[2] = 1;
G.SetLengths[5] = 1;
RemoveShorterSets(2, &G);
AssertAreEqualInts(10, G.MoveSetsCount, "There should be 10 moves left");
/*ConsoleWriteLine("==================");
PrintMoves();*/
)

TEST(TestEvaluation,
	char* gameString = "0 0 b2 b2 0 0 w5 w3 0 b2 0 0 0 w5 0 0 0 0 0 b3 b2 0 b2 b2 w2 0 0 0";
ReadGameString(gameString, &G);
InitAi(true);
double score = EvaluateCheckers(&G, Black);

)

TEST(TestPointsLeft,
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

//TODO: Hits
)

TEST(TestPointsLeftHit,
	char* gs = "0 b2 0 0 0 0 w1 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b1 0 0 0 0 w2 0 0 0";
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

AssertAreEqualInts(143, (int)G.BlackLeft, "Expcted 143 Points left for Black");
AssertAreEqualInts(143, (int)G.WhiteLeft, "Expcted 143 Points left for White");
)

TEST(TestGetScore,
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
ReadGameString(gs, &G);
InitAi(true);
double score = GetScore(&G);
printf("Score: %f\n", score);
)

TEST(TestPrintGame,
	char* gs = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
ReadGameString(gs, &G);
PrintGame(&G);
)

TEST(TestPlayGame,
	PlayGame(&G);
)

void TestBestBearingOffBlack() {
	char* gs = "0 0 w6 w2 w2 w2 w3 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 b5 b5 b1 0 w0 0 0";
	ReadGameString(gs, &G);
	G.CurrentPlayer = Black;
	InitAi(true);
	G.Dice[0] = 3;
	G.Dice[1] = 2;
	int i = FindBestMoveSet(&G);
	Move *m = G.PossibleMoveSets[i].Moves;
	for (int j = 0; j < G.SetLengths[i]; j++)
		AssertAreEqualInts(25, m[j].to, "Move should be to 25, Black Home");
}

void TestBestBearingOffWhite() {
	char* gs = "0 0 w6 w2 w2 w2 w3 0 0 0 0 0 0 0 0 0 0 0 0 b2 b2 b5 b5 b1 0 w0 0 0";
	ReadGameString(gs, &G);
	G.CurrentPlayer = White;
	InitAi(true);
	G.Dice[0] = 3;
	G.Dice[1] = 2;
	int i = FindBestMoveSet(&G);
	Move* m = G.PossibleMoveSets[i].Moves;
	for (int j = 0; j < G.SetLengths[i]; j++)
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
		Assert(G.SetLengths[i] <= 4, "To many moves in set");
}

void TestNastyBug() {
	char* gs = "0 b2 w2 0 b1 0 w4 0 w2 w2 0 0 b3 w4 0 0 0 b2 0 b4 0 0 0 b3 w1 w0 0 0";
	ReadGameString(gs, &G);
	Move m;
	m.from = 25;
	m.to = 20;
	m.color = White;

	DoMove(m, &G);
}

void RunAll() {
	TestNastyBug();
	TestStartPos();
	TestRollDice();
	TestWriteGameString();
	TestReadGameString();
	TestTwoDigitGameString();
	TestGameStringRountTrip();
	TestDoUndo();
	TestIsBlocked();
	TestSimpleBlack();
	TestDoUndoHomeBlack();
	TestDoUndoHomeWhite();
	TestCreateMovesBlackStart();
	TestCreateMovesWhiteStart();
	TestOtherColor();
	TestBlackCheckerOnBar();
	TestWhiteCheckerOnBar();
	TestBearingOffBlack();
	TestBearingOffWhite();
	TestDoubleDiceBlack();
	TestDoubleDiceWhite();
	PlayBothDiceIfPossible();
	TestRemoveShorterSets();
	TestEvaluation();
	TestPointsLeft();
	TestPointsLeftHit();
	TestGetScore();
	//TestPrintGame();
	TestBestBearingOffBlack();
	TestBestBearingOffWhite();
	TestManyCombos();

	// TODO: Test Blocked -> zero moves.

	if (_failedAsserts == 0)
		PrintGreen("\nSuccess! Tests are good!\n");
	else
		PrintRed("\nThere are failed tests.\n");
}