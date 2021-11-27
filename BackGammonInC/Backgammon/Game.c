#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>

#include "Game.h"
#include "Utils.h"

void Reset() {
	for (int i = 0; i < 26; i++)
		G.Position[i] = 0;
	G.WhiteHome = 0;
	G.BlackHome = 0;
	G.CurrentPlayer = Black;

	G.Dice[0] = 0;
	G.Dice[1] = 0;
}

void StartPosition() {
	srand(time(NULL));   // Initialization, should only be called once.

	Reset();

	G.Position[1] = 2 | Black;
	G.Position[6] = 5 | White;
	G.Position[8] = 3 | White;
	G.Position[12] = 5 | Black;
	G.Position[13] = 5 | White;
	G.Position[17] = 3 | Black;
	G.Position[19] = 5 | Black;
	G.Position[24] = 2 | White;
}

void RollDice() {
	G.Dice[0] = rand() % 6 + 1;
	G.Dice[1] = rand() % 6 + 1;
}

bool ToHome(Move move) {
	return move.color == Black && move.to == 25 || move.color == White && move.to == 0;
}

bool DoMove(Move move) {
	ushort to = move.to;
	ushort from = move.from;
	bool toHome = ToHome(move);
	bool hit = G.Position[to] > 0 && !toHome;
	if (hit)
		G.Position[to] = 0;

	if (toHome) {
		move.color == Black ? G.BlackHome++ : G.WhiteHome++;
	}
	else {
		G.Position[to]++;
		G.Position[to] |= move.color;
	}

	G.Position[from]--;
	if (CheckerCount(G.Position[from]) == 0)
		G.Position[from] = 0;

	if (move.color == Black)
	{
		G.BlackLeft -= (move.to - move.from);
		if (hit)
			G.WhiteLeft += (25 - move.to);
	}
	if (move.color == White)
	{
		G.WhiteLeft -= (move.from - move.to);
		if (hit)
			G.BlackLeft += move.to; // += 25 - (25 - move.to);
	}

	return hit;
}

void UndoMove(Move move, bool hit) {
	ushort to = move.to;
	ushort from = move.from;
	bool fromHome = ToHome(move);

	G.Position[from]++;
	G.Position[from] |= move.color;

	if (fromHome) {
		move.color == Black ? G.BlackHome-- : G.WhiteHome--;
	}
	else {
		G.Position[to]--;
		if (CheckerCount(G.Position[to]) == 0)
			G.Position[to] = 0;

		if (hit)
			G.Position[to] = 1 | OtherColor(move.color);
	}

	if (move.color == Black)
	{
		G.BlackLeft += (move.to - move.from);
		if (hit)
			G.WhiteLeft -= (25 - move.to);
	}
	if (move.color == White)
	{
		G.WhiteLeft += (move.from - move.to);
		if (hit)
			G.BlackLeft -= move.to; // -= 25 - (25 - move.to);
	}
}

bool IsBlockedFor(ushort pos, ushort color) {
	if (pos > 25)
		return false;


	return G.Position[pos] & OtherColor(color) && CheckerCount(G.Position[pos]) >= 2;
}

bool IsBlackBearingOff(ushort* lastCheckerPos) {
	for (ushort i = 0; i <= 24; i++)
	{
		if ((G.Position[i] & Black) && CheckerCount(G.Position[i]) > 0)
		{
			*lastCheckerPos = i;
			return i >= 19;
		}
	}
	return true;
}

bool IsWhiteBearingOff(ushort* lastCheckerPos) {
	for (ushort i = 25; i >= 1; i--)
	{
		if ((G.Position[i] & White) && CheckerCount(G.Position[i]) > 0)
		{
			*lastCheckerPos = i;
			return i <= 6;
		}
	}
	return true;
}

void CreateBlackMoveSets(int diceIdx, int diceCount, int* maxSetLength) {
	ushort start = 0;
	ushort lastCheckerPos;
	bool bearingOff = IsBlackBearingOff(&lastCheckerPos);
	if (bearingOff)
		start = 19;

	ushort toIndex = CheckerCount(G.Position[0]) > 0 ? 1 : 25;

	for (ushort i = start; i < toIndex; i++)
	{
		if (!(G.Position[i] & Black))
			continue;
		int diceVal = diceIdx > 1 ? G.Dice[0] : G.Dice[diceIdx];
		int toPos = i + diceVal;

		// När man bär av, får man använda tärningar med för hög summa,
		// men bara på den checker längst från home.
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
		if (G.MoveSetsCount == 0)
			G.MoveSetsCount = 1;
		ushort seqIdx = G.MoveSetsCount - 1;
		Move* move = &G.PossibleMoveSets[seqIdx][diceIdx];
		if (move->color != 0) {
			// A move is already generated for this dice in this sequence. Branch off a new sequence.
			int copyCount = diceIdx;// SetLengths[seqIdx] - 1;
			G.SetLengths[seqIdx + 1] = copyCount;
			if (copyCount > 0)
				memcpy(&G.PossibleMoveSets[seqIdx + 1][0], &G.PossibleMoveSets[seqIdx][0], copyCount * sizeof(Move));
			move = &G.PossibleMoveSets[seqIdx + 1][diceIdx];
			G.MoveSetsCount++;
			seqIdx++;
		}

		move->from = i;
		move->to = toPos;
		move->color = Black;

		G.SetLengths[seqIdx]++;
		*maxSetLength = max(*maxSetLength, G.SetLengths[seqIdx]);

		//TODO: Maybe omit identical sequences, hashing?
		if (diceIdx < diceCount - 1) {
			int hit = DoMove(*move);
			CreateBlackMoveSets(diceIdx + 1, diceCount, maxSetLength);
			UndoMove(*move, hit);
		}
	}
}

void CreateWhiteMoveSets(int diceIdx, int diceCount, int* maxSetLength) {
	int start = 25;
	ushort lastCheckerPos;
	bool bearingOff = IsWhiteBearingOff(&lastCheckerPos);
	if (bearingOff)
		start = 6;

	int toIndex = CheckerCount(G.Position[25]) > 0 ? 25 : 0;

	for (int i = start; i >= toIndex; i--)
	{
		if (!(G.Position[i] & White))
			continue;
		int diceVal = diceIdx > 1 ? G.Dice[0] : G.Dice[diceIdx];
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
		if (G.MoveSetsCount == 0)
			G.MoveSetsCount = 1;
		ushort seqIdx = G.MoveSetsCount - 1;
		Move* move = &G.PossibleMoveSets[seqIdx][diceIdx];
		if (move->color != 0) {
			// A move is already generated for this dice in this sequence. Branch off a new sequence.
			int copyCount = diceIdx;
			G.SetLengths[seqIdx + 1] = copyCount;
			if (copyCount > 0)
				memcpy(&G.PossibleMoveSets[seqIdx + 1][0], &G.PossibleMoveSets[seqIdx][0], copyCount * sizeof(Move));
			move = &G.PossibleMoveSets[seqIdx + 1][diceIdx];
			G.MoveSetsCount++;
			seqIdx++;
		}

		move->from = i;
		move->to = toPos;
		move->color = White;

		G.SetLengths[seqIdx]++;
		*maxSetLength = max(*maxSetLength, G.SetLengths[seqIdx]);

		//TODO: Maybe omit identical sequences, hashing?
		if (diceIdx < diceCount - 1) {
			int hit = DoMove(*move);
			CreateWhiteMoveSets(diceIdx + 1, diceCount, maxSetLength);
			UndoMove(*move, hit);
		}
	}
}

void ReverseDice() {
	short temp = G.Dice[0];
	G.Dice[0] = G.Dice[1];
	G.Dice[1] = temp;
}

void RemoveShorterSets(int maxSetLength) {
	bool modified = false;
	int realCount = G.MoveSetsCount;
	do
	{
		modified = false;
		for (int i = 0; i < realCount; i++)
		{
			if (G.SetLengths[i] < maxSetLength)
			{
				memcpy(&G.PossibleMoveSets[i], &G.PossibleMoveSets[i + 1], (MAX_SETS_LENGTH - i) * 4 * sizeof(Move));
				memcpy(&G.SetLengths[i], &G.SetLengths[i + 1], (MAX_SETS_LENGTH - i) * sizeof(ushort));
				modified = true;
				realCount--;
				break;
			}
		}
	} while (modified);
	G.MoveSetsCount = realCount;
}

void CreateMoves(Game* g) {
	for (int i = 0; i < MAX_SETS_LENGTH; i++)
	{
		g->SetLengths[i] = 0;
		for (int j = 0; j < 4; j++)
		{
			Move move;
			move.from = 0;
			move.to = 0;
			move.color = 0;
			g->PossibleMoveSets[i][j] = move;
		}
	}

	g->MoveSetsCount = 0;
	// Largest Dice first
	if (g->Dice[1] > g->Dice[0]) {
		ReverseDice();
	}

	int diceCount = g->Dice[0] == g->Dice[1] ? 4 : 2;
	int maxSetLength = 0;
	for (size_t i = 0; i < 2; i++)
	{
		maxSetLength = 0;
		if (g->CurrentPlayer & Black)
			CreateBlackMoveSets(0, diceCount, &maxSetLength);
		else
			CreateWhiteMoveSets(0, diceCount, &maxSetLength);

		//If no moves are found and dicecount == 2 reverse dice order and try again.
		if (g->MoveSetsCount == 0 && diceCount == 2) {
			ReverseDice();
		}
		else {
			break;
		}
	}

	RemoveShorterSets(maxSetLength);
}

void FindBestMoves() {

}

void WriteGameString(char* s) {
	int idx = 0;
	for (size_t i = 0; i < 26; i++)
	{
		if (G.Position[i] > 0) {
			ushort black = G.Position[i] & Black;
			if (black) {
				s[idx++] = 'b';
			}
			else {
				s[idx++] = 'w';
			}
		}
		s[idx++] = '0' + CheckerCount(G.Position[i]);
		s[idx++] = ' ';
	}
	s[idx++] = '0' + G.WhiteHome;
	s[idx++] = ' ';
	s[idx++] = '0' + G.BlackHome;
	s[idx] = '\0';
}

void SetPointsLeft(Game* g) {
	ushort blackLeft = 0;
	ushort whiteLeft = 0;
	for (int i = 0; i < 25; i++)
	{
		if (g->Position[i] & Black)
			blackLeft += (25 - i) * CheckerCount(g->Position[i]);
		if (g->Position[i] & White)
			whiteLeft += i * CheckerCount(g->Position[i]);
	}
	g->BlackLeft = blackLeft;
	g->WhiteLeft = whiteLeft;
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
			G.Position[i] = atoi(n) | Black;
		}
		else if (StartsWith(token, "w")) {
			SubString(token, n, 2, 3);
			G.Position[i] = atoi(n) | White;
		}

		token = strtok_s(NULL, " ", &context);
	}
	G.WhiteHome = atoi(token);
	token = strtok_s(NULL, " ", &context);
	G.BlackHome = atoi(token);
	SetPointsLeft(&G);
}

void PrintGame(Game* g) {
	// ╔═══╤═══╤═══╤═══╤═══╤═══╤═══╤═══╗
	char top[] = { 201,205,205,205,209,205,205,205,209,205,205,205,209,205,205,205,209,205,205,205,209,205,205,205,203,205,205,205,209,205,205,205,209,205,205,205,209,205,205,205,209,205,205,205,209,205,205,205,187,0 };
	// ╟───┼───┼───┼───┼───┼───┼───┼───╢
	char rowLine[] = { 199,196,196,196,197,196,196,196,197,196,196,196,197,196,196,196,197,196,196,196,197,196,196,196,215,196,196,196,197,196,196,196,197,196,196,196,197,196,196,196,197,196,196,196,197,196,196,196,182,0 };
	// ╚═══╧═══╧═══╧═══╧═══╧═══╧═══╧═══╝
	char lastLine[] = { 200,205,205,205, 207,205,205,205, 207,205,205,205, 207,205,205,205, 207,205,205,205, 207,205,205,205, 202,205,205,205, 207,205,205,205, 207,205,205,205, 207,205,205,205, 207,205,205,205, 207,205,205,205, 188, 0 };
	// ║
	char vBorder = 186;
	ConsoleWriteLine("");
	printf(" %s\n", top);
	for (int y = 0; y < 10; y++)
	{
		printf(" %c", vBorder);
		for (int x = 0; x < 12; x++)
		{
			PlayerSide color = g->Position[12 - x] & 48;
			int count = CheckerCount(g->Position[12 - x]);
			if (count > y)
				color == White ? PrintYellow(" O ") : PrintBlue(" O ");
			else
				printf("   ");
			if (x < 11)
				x == 5 ? printf("%c", 186) : printf("%c", 245);
		}
		printf("%c", vBorder);
		printf("\n");
	}

	printf(" %s\n", rowLine);


	for (int y = 0; y < 10; y++)
	{
		printf(" %c", vBorder);
		for (int x = 13; x <= 24; x++)
		{
			PlayerSide color = g->Position[x] & 48;
			int count = CheckerCount(g->Position[x]);
			if (count >= 10 - y)
				color == White ? PrintYellow(" O ") : PrintBlue(" O ");
			else
				printf("   ");
			if (x < 24)
				x == 18 ? printf("%c", 186) : printf("%c", 245);
		}
		printf("%c", vBorder);
		printf("\n");
	}
	printf(" %s\n", lastLine);

	printf("Black left: %d   Bar: %d   Home: %d\n", g->BlackLeft, CheckerCount(g->Position[0]), g->BlackHome);
	printf("White left: %d   Bar: %d   Home: %d\n", g->WhiteLeft, CheckerCount(g->Position[25]), g->WhiteHome);
}


