#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>
#include "Game.h"
#include "Utils.h"


void Reset() {
	for (int i = 0; i < 26; i++)
		Position[i] = 0;
	WhiteHome = 0;
	BlackHome = 0;

	Dice[0] = 0;
	Dice[1] = 0;
}

void StartPosition() {
	srand(time(NULL));   // Initialization, should only be called once.

	Reset();

	Position[1] = 2 | Black;
	Position[6] = 5 | White;
	Position[8] = 3 | White;
	Position[12] = 5 | Black;
	Position[13] = 5 | White;
	Position[17] = 3 | Black;
	Position[19] = 5 | Black;
	Position[24] = 2 | White;
}

void RollDice() {
	Dice[0] = rand() % 6 + 1;
	Dice[1] = rand() % 6 + 1;
}

void GeneratePossibleMoves() {
	if (Dice[0] == Dice[1]) {
		// 4 moves instead of two.
	}
}

void DoMoves(int from, int to, int color) {

}

void UndoMove() {

}

void FindBestMoves() {

}

void WriteGameString(char* s) {
	int idx = 0;	
	for (size_t i = 0; i < 26; i++)
	{
		if (Position[i] > 0) {
			ushort black = Position[i] >> 4; // TODO: Create function GetColor
			if (black) {
				s[idx++] = 'b';
			}
			else {
				s[idx++] = 'w';
			}
		}
		s[idx++] = '0' + (Position[i] & 0xF); // TODO: Create function CountCheckers		
		s[idx++] = ' ';		
	}
	s[idx++] = '0' + WhiteHome;
	s[idx++] = ' ';
	s[idx++] = '0' + BlackHome;
	s[idx] = '\0';
}

void ReadGameString(char* s) {
	Reset();
	// 28 tokens
	// BlackBar, pos 1 - 24, WhiteBar, BlackHome, WhiteHome
	// "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0"
	
	// todo: meaningfull error handling

	int len = strlen(s);
	char copy[100];
	memcpy(copy, s, len + 1);

	char* context;
	char* token = strtok_s(copy, " ", &context);

	for (size_t i = 0; i < 26; i++)
	{
		char n[100];

		if (StartsWith(token, "b")) {
			SubString(token, n, 2, 3);
			Position[i] = atoi(n) | Black;
		}
		else if (StartsWith(token, "w")) {
			SubString(token, n, 2, 3);
			Position[i] = atoi(n) | White;
		}
		else {
			Position[i] = atoi(token);
		}

		token = strtok_s(NULL, " ", &context);
	}
	WhiteHome = atoi(token);
	token = strtok_s(NULL, " ", &context);
	BlackHome = atoi(token);
}


