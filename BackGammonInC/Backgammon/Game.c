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
	CurrentPlayer = Black;

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

bool ToHome(Move move) {
	return move.color == Black && move.to == 25 || move.color == White && move.to == 0;
}

bool DoMove(Move* move) {
	ushort to = move->to;
	ushort from = move->from;
	bool toHome = ToHome(*move);
	bool hit = Position[to] > 0 && !toHome;
	if (hit)
		Position[to] = 0;

	if (toHome) {
		move->color == Black ? BlackHome++ : WhiteHome++;
	}
	else {
		Position[to]++;
		Position[to] |= move->color;
	}

	Position[from]--;
	if (CheckerCount(from) == 0)
		Position[from] = 0;

	return hit;
}

void UndoMove(Move* move, bool hit) {
	ushort to = move->to;
	ushort from = move->from;
	bool fromHome = ToHome(*move);

	Position[from]++;
	Position[from] |= move->color;

	if (fromHome) {
		move->color == Black ? BlackHome-- : WhiteHome--;
	}
	else {
		Position[to]--;
		if (CheckerCount(to) == 0)
			Position[to] = 0;

		if (hit)
			Position[to] = 1 | OtherColor(move->color);
	}
}

bool IsBlockedFor(ushort pos, ushort color) {
	if (CheckerCount(pos) < 2)
		return false;

	// Atleast two checkers
	OtherColor(color);
	return Position[pos] & OtherColor(color);
}

bool IsBlackBearingOff(ushort* lastCheckerPos) {
	for (ushort i = 0; i <= 24; i++)
	{
		if ((Position[i] & Black) && CheckerCount(i) > 0)
		{
			*lastCheckerPos = i;
			return false;
		}
	}
	return true;
}

bool IsWhiteBearingOff(ushort* lastCheckerPos) {
	for (ushort i = 25; i >= 1; i--)
	{
		if ((Position[i] & White) && CheckerCount(i) > 0)
		{
			*lastCheckerPos = i;
			return false;
		}
	}
	return true;
}

void CreateBlackMoveSequence(int seqIdx, int diceIdx, int diceCount) {
	//TODO White moves backwards.
	bool createdSet = false;
	ushort start = 0;
	ushort lastCheckerPos;
	bool bearingOff = IsBlackBearingOff(&lastCheckerPos);
	if (bearingOff)
		start = 19;

	for (ushort i = 19; i < 25; i++)
	{
		if (!(Position[i] & CurrentPlayer))
			continue;
		int diceVal = diceIdx > 1 ? Dice[0] : Dice[diceIdx];
		int toPos = i + diceVal;

		//TODO Check is bearing off

		// När man bär av, får man använda tärningar med för hög summa
		// Men bara på den checker längst från home.
		if (IsBlockedFor(toPos, Black))
			continue;

		if (bearingOff) {
			if (toPos > 25 && i != lastCheckerPos)
				continue;

			if (toPos > 25 && i == lastCheckerPos)
				toPos = 25;
		}
		else { //Not bearing off
			if (toPos > 24)
				continue;
		}

		Move* move = &PossibleMoveSets[seqIdx][diceIdx];
		move->from = i;
		move->to = toPos;
		move->color = Black;

		SetLengths[seqIdx]++;
		createdSet = true;

		//TODO: Maybe omit identical sequences, hashing?
		if (diceIdx < diceCount) {
			int hit = DoMove(move);
			CreateBlackMoveSequence(seqIdx + 1, diceIdx + 1, diceCount);
			UndoMove(move, hit);
		}
	}

	if (createdSet)
		MoveSetsCount++;
}

void CreateMoves() {
	//TODO: reset move sets
	for (int i = 0; i < 500; i++)
		SetLengths[i] = 0;
	MoveSetsCount = 0;

	int diceCount = Dice[0] == Dice[1] ? 4 : 2;
	if (CurrentPlayer & Black)
		CreateBlackMoveSequence(0, 0, diceCount);
	else {}
}


void FindBestMoves() {

}

void WriteGameString(char* s) {
	int idx = 0;
	for (size_t i = 0; i < 26; i++)
	{
		if (Position[i] > 0) {
			ushort black = Position[i] & Black; // TODO: Create function GetColor
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

		token = strtok_s(NULL, " ", &context);
	}
	WhiteHome = atoi(token);
	token = strtok_s(NULL, " ", &context);
	BlackHome = atoi(token);
}


