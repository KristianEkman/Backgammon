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
	if (pos > 25)
		return false;


	return Position[pos] & OtherColor(color) && CheckerCount(pos) >= 2;
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

void CreateBlackMoveSets(int diceIdx, int diceCount) {
	//TODO White moves backwards.
	ushort start = 0;
	ushort lastCheckerPos;
	bool bearingOff = IsBlackBearingOff(&lastCheckerPos);
	if (bearingOff)
		start = 19;

	ushort toIndex = CheckerCount(0) > 0 ? 0 : 25;

	for (ushort i = start; i < toIndex; i++)
	{
		if (!(Position[i] & Black))
			continue;
		int diceVal = diceIdx > 1 ? Dice[0] : Dice[diceIdx];
		int toPos = i + diceVal;

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

		// Atleast one move set is created.
		if (MoveSetsCount == 0)
			MoveSetsCount = 1;
		ushort seqIdx = MoveSetsCount - 1;
		Move* move = &PossibleMoveSets[seqIdx][diceIdx];
		if (move->color != 0) {
			// A move is already generated for this dice in this sequence. Branch off a new sequence.
			SetLengths[seqIdx + 1] = SetLengths[seqIdx] - 1;
			memcpy(&PossibleMoveSets[seqIdx + 1][0], &PossibleMoveSets[seqIdx][0], 4 * sizeof(Move));			
			move = &PossibleMoveSets[seqIdx + 1][diceIdx];
			MoveSetsCount++;
			seqIdx++;
		}

		move->from = i;
		move->to = toPos;
		move->color = Black;

		SetLengths[seqIdx]++;

		//TODO: Maybe omit identical sequences, hashing?
		if (diceIdx < diceCount - 1) {
			int hit = DoMove(move);			
			CreateBlackMoveSets(diceIdx + 1, diceCount);
			UndoMove(move, hit);
		}
	}
}

void CreateWhiteMoveSets(int diceIdx, int diceCount) {
	//TODO White moves backwards.
	int start = 25;
	ushort lastCheckerPos;
	bool bearingOff = IsWhiteBearingOff(&lastCheckerPos);
	if (bearingOff)
		start = 6;

	int toIndex = CheckerCount(25) > 0 ? 25 : 0;

	for (int i = start; i >= toIndex; i--)
	{
		if (!(Position[i] & White))
			continue;
		int diceVal = diceIdx > 1 ? Dice[0] : Dice[diceIdx];
		int toPos = i - diceVal;

		// När man bär av, får man använda tärningar med för hög summa
		// Men bara på den checker längst från home.
		if (IsBlockedFor(toPos, White))
			continue;

		if (bearingOff) {
			if (toPos < 0 && i != lastCheckerPos)
				continue;

			if (toPos < 0 && i == lastCheckerPos)
				toPos = 0;
		}
		else { //Not bearing off
			if (toPos < 1)
				continue;
		}

		// Atleast one move set is created.
		if (MoveSetsCount == 0)
			MoveSetsCount = 1;
		ushort seqIdx = MoveSetsCount - 1;
		Move* move = &PossibleMoveSets[seqIdx][diceIdx];
		if (move->color != 0) {
			// A move is already generated for this dice in this sequence. Branch off a new sequence.
			SetLengths[seqIdx + 1] = SetLengths[seqIdx] - 1;
			memcpy(&PossibleMoveSets[seqIdx + 1][0], &PossibleMoveSets[seqIdx][0], 4 * sizeof(Move));			
			move = &PossibleMoveSets[seqIdx + 1][diceIdx];
			MoveSetsCount++;
			seqIdx++;
		}

		move->from = i;
		move->to = toPos;
		move->color = White;

		SetLengths[seqIdx]++;

		//TODO: Maybe omit identical sequences, hashing?
		if (diceIdx < diceCount - 1) {
			int hit = DoMove(move);			
			CreateWhiteMoveSets(diceIdx + 1, diceCount);
			UndoMove(move, hit);
		}
	}
}

void CreateMoves() {	
	for (int i = 0; i < 500; i++)
	{
		SetLengths[i] = 0;
		for (int j = 0; j < 4; j++)
		{
			Move move;
			move.from = 0;
			move.to = 0;
			move.color = 0;
			PossibleMoveSets[i][j] = move;
		}
	}

	MoveSetsCount = 0;
	int diceCount = Dice[0] == Dice[1] ? 4 : 2;
	if (CurrentPlayer & Black)
		CreateBlackMoveSets(0, diceCount);
	else {
		CreateWhiteMoveSets(0, diceCount);
	}

	// TODO: Set length to 0 for sets that are shorter than max set length found.
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

	size_t len = strlen(s);
	char copy[100];
	memcpy(copy, s, len + 1);

	char* context = NULL;
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


