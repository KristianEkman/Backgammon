#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "Utils.h"
#include "Game.h"

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

void TestStartPos() {
	StartPosition();
	Assert(Dice[0] == 0, "Dice 0 is not reset");
	Assert(Dice[1] == 0, "Dice 1 is not reset");
}

void TestRollDice() {
	for (size_t i = 0; i < 10; i++)
	{
		RollDice();
		printf("%d %d ", Dice[0], Dice[1]);
		Assert(Dice[0] >= 1 && Dice[0] <= 6, "Invalid Dice value");
		Assert(Dice[1] >= 1 && Dice[1] <= 6, "Invalid Dice value");
	}
	printf("\n");

}

void TestWriteGameString() {
	StartPosition();
	char s[100];
	WriteGameString(s);
	
	char* expected = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	AssertAreEqual(expected, s, "Unexpected GameString");
}

void TestReadGameString() {
	
	char* gameString = "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0";
	ReadGameString(gameString);
	AssertAreEqualInts(Position[0], 0, "Invalid checker count");
	AssertAreEqualInts(Position[1], 18, "Invalid checker count");
	AssertAreEqualInts(Position[2], 0, "Invalid checker count");
	AssertAreEqualInts(Position[3], 0, "Invalid checker count");
	AssertAreEqualInts(Position[4], 0, "Invalid checker count");
	AssertAreEqualInts(Position[5], 0, "Invalid checker count");
	AssertAreEqualInts(Position[6], 5, "Invalid checker count");
	AssertAreEqualInts(Position[7], 0, "Invalid checker count");
	AssertAreEqualInts(Position[8], 3, "Invalid checker count");
	AssertAreEqualInts(Position[9], 0, "Invalid checker count");
	AssertAreEqualInts(Position[10], 0, "Invalid checker count");
	AssertAreEqualInts(Position[11], 0, "Invalid checker count");
	AssertAreEqualInts(Position[12], 21, "Invalid checker count");
	AssertAreEqualInts(Position[13], 5, "Invalid checker count");
	AssertAreEqualInts(Position[14], 0, "Invalid checker count");
	AssertAreEqualInts(Position[15], 0, "Invalid checker count");
	AssertAreEqualInts(Position[16], 0, "Invalid checker count");
	AssertAreEqualInts(Position[17], 19, "Invalid checker count");
	AssertAreEqualInts(Position[18], 0, "Invalid checker count");
	AssertAreEqualInts(Position[19], 21, "Invalid checker count");
	AssertAreEqualInts(Position[20], 0, "Invalid checker count");
	AssertAreEqualInts(Position[21], 0, "Invalid checker count");
	AssertAreEqualInts(Position[22], 0, "Invalid checker count");
	AssertAreEqualInts(Position[23], 0, "Invalid checker count");
	AssertAreEqualInts(Position[24], 2, "Invalid checker count");
	AssertAreEqualInts(Position[25], 0, "Invalid checker count");
	AssertAreEqualInts(BlackHome, 0, "Invalid black home count");
	AssertAreEqualInts(WhiteHome, 0, "Invalid white home count");
}

void TestGameStringRountTrip() {

	char* gameString = "b2 b1 w3 0 0 0 w2 w1 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 w2 3 4";
	ReadGameString(gameString);
	AssertAreEqualInts(Position[0], 18, "Invalid checker count");
	AssertAreEqualInts(Position[1], 17, "Invalid checker count");
	AssertAreEqualInts(Position[2], 3, "Invalid checker count");
	AssertAreEqualInts(Position[3], 0, "Invalid checker count");
	AssertAreEqualInts(Position[4], 0, "Invalid checker count");
	AssertAreEqualInts(Position[5], 0, "Invalid checker count");
	AssertAreEqualInts(Position[6], 2, "Invalid checker count");
	AssertAreEqualInts(Position[7], 1, "Invalid checker count");
	AssertAreEqualInts(Position[8], 3, "Invalid checker count");
	AssertAreEqualInts(Position[9], 0, "Invalid checker count");
	AssertAreEqualInts(Position[10], 0, "Invalid checker count");
	AssertAreEqualInts(Position[11], 0, "Invalid checker count");
	AssertAreEqualInts(Position[12], 21, "Invalid checker count");
	AssertAreEqualInts(Position[13], 5, "Invalid checker count");
	AssertAreEqualInts(Position[14], 0, "Invalid checker count");
	AssertAreEqualInts(Position[15], 0, "Invalid checker count");
	AssertAreEqualInts(Position[16], 0, "Invalid checker count");
	AssertAreEqualInts(Position[17], 19, "Invalid checker count");
	AssertAreEqualInts(Position[18], 0, "Invalid checker count");
	AssertAreEqualInts(Position[19], 21, "Invalid checker count");
	AssertAreEqualInts(Position[20], 0, "Invalid checker count");
	AssertAreEqualInts(Position[21], 0, "Invalid checker count");
	AssertAreEqualInts(Position[22], 0, "Invalid checker count");
	AssertAreEqualInts(Position[23], 0, "Invalid checker count");
	AssertAreEqualInts(Position[24], 2, "Invalid checker count");
	AssertAreEqualInts(Position[25], 2, "Invalid checker count");
	AssertAreEqualInts(WhiteHome, 3, "Invalid black home count");
	AssertAreEqualInts(BlackHome, 4, "Invalid white home count");

	char written[100];
	WriteGameString(written);
	AssertAreEqual(gameString, written, "Read and written string not same");
}


void RunAll() {
	TestStartPos();
	TestRollDice();
	TestWriteGameString();
	TestReadGameString();
	TestGameStringRountTrip();

	if (_failedAsserts == 0)
		PrintGreen("Success! Tests are good!\n");
	else
		PrintRed("There are failed tests.\n");
}