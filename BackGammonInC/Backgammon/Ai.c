
#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <time.h>

#include "Ai.h"
#include "Game.h"
#include "Utils.h"

void InitAi(bool constant) {
	for (int a = 0; a < 2; a++)
	{
		for (int i = 0; i < 25; i++)
		{
			if (constant) {
				BlotFactors[a][i] = 1;
				ConnectedBlocksFactor[a][i] = 1;
			}
			else {
				BlotFactors[a][i] = RandomDouble(0, 1);
				ConnectedBlocksFactor[a][i] = RandomDouble(0, 1);
			}
		}
	}

	int i = 0;
	for (int a = 1; a < 7; a++)
	{
		for (int b = a; b < 7; b++)
		{
			AllDices[i][0] = a;
			AllDices[i++][1] = b;
		}
	}

	AIs[0].Flags = EnableAlphaBetaPruning | EnableHashing;
	AIs[0].SearchDepth = 1;

	AIs[1].Flags = EnableAlphaBetaPruning | EnableHashing;
	AIs[1].SearchDepth = 1;
}

bool PlayersPassedEachOther(Game* g) {
	int minBlack = 25;
	int maxWhite = 0;
	for (int i = 0; i < 26; i++)
	{
		if (g->Position[i] & Black)
			minBlack = min(minBlack, i);

		if (g->Position[i] & White)
			maxWhite = max(maxWhite, i);
	}
	return minBlack > maxWhite;
}

double EvaluateCheckers(Game* g, PlayerSide color) {
	double score = 0;
	int blockCount = 0;
	bool playersPassed = PlayersPassedEachOther(g);

	char ai = color >> 5;
	// TODO: Try calculate both colors in same loop. Better performance?
	for (int i = 1; i < 25; i++)
	{
		int p = color == White ? 25 - i : i;
		short v = g->Position[p];
		int checkCount = CheckerCount(v);
		if (checkCount > 1 && (v & color)) {
			blockCount++;
		}
		else {
			if (blockCount && !playersPassed) {
				score += pow((double)blockCount, ConnectedBlocksFactor[ai][p]);
			}
			blockCount = 0;
		}

		if (checkCount == 1 && (v & color))
		{
			score -= (double)CheckerCount(v) * BlotFactors[ai][p];
		}
	}
	return score;
}

double GetScore(Game* g) {
	double bHome = (double)10000 * (double)g->BlackHome;
	double wHome = (double)10000 * (double)g->WhiteHome;
	// positive for white, neg for black.
	g->EvalCounts++;
	return wHome - bHome + EvaluateCheckers(g, White) - EvaluateCheckers(g, Black) - g->WhiteLeft + g->BlackLeft;
}

double GetProbablilityScore(Game* g, ubyte depth) {
	double totalScore = 0;
	g->CurrentPlayer = OtherColor(g->CurrentPlayer);
	short diceBuf[2] = { g->Dice[0], g->Dice[1] };
	for (int i = 0; i < DiceCombos; i++)
	{
		g->Dice[0] = AllDices[i][0];
		g->Dice[1] = AllDices[i][1];

		double score = 0;
		FindBestMoveSet(g, &score, depth);
		double m = g->Dice[0] == g->Dice[1] ? 2 : 1;
		totalScore += score * m;
	}
	// Since we are faking the dice down the stack, it is safer to but them back here.
	g->Dice[0] = diceBuf[0]; g->Dice[1] = diceBuf[1];
	g->CurrentPlayer = OtherColor(g->CurrentPlayer);
	return totalScore / 21;
}

MoveSet FindBestMoveSet(Game* g, double* bestScoreOut, ubyte depth) {
	int bestIdx = 0;
	double bestScore = g->CurrentPlayer == White ? -100000 : 100000;
	CreateMoves(g);
	int setsCount = g->MoveSetsCount;
	MoveSet* localSets = malloc(sizeof(MoveSet) * g->MoveSetsCount);
	memcpy(localSets, &g->PossibleMoveSets, sizeof(MoveSet) * setsCount);
	for (int i = 0; i < setsCount; i++)
	{
		MoveSet set = localSets[i];
		if (set.Duplicate)
			continue;

		Move moves[4];
		bool hits[4];
		for (int m = 0; m < set.Length; m++)
		{
			moves[m] = set.Moves[m];
			hits[m] = DoMove(moves[m], g);
		}

		double score;
		if (depth == 0)
			score = GetScore(g);
		else
			// The best score the opponent can get. Minimize it.
			score = GetProbablilityScore(g, depth - 1);

		if (g->CurrentPlayer == White) {
			// White is maximizing
			if (score > bestScore)
			{
				bestScore = score;
				bestIdx = i;
			}
		}
		else {
			// Black is minimizing
			if (score < bestScore) {
				bestScore = score;
				bestIdx = i;
			}
		}

		//Undoing in reverse
		for (int u = set.Length - 1; u >= 0; u--)
			UndoMove(moves[u], hits[u], g);
	}
	MoveSet bestSet = localSets[bestIdx];
	free(localSets);
	*bestScoreOut = bestScore;
	return bestSet;
}

void Pause(Game* g) {
	char buf[BUF_SIZE];
	SetCursorPosition(0, 0);
	PrintGame(g);
	fgets(buf, 5000, stdin);
}

void PlayGame(Game* g, bool pausePlay) {
	StartPosition(g);

	RollDice(g);
	while (g->Dice[0] == g->Dice[1])
		RollDice(g);

	g->CurrentPlayer = g->Dice[0] > g->Dice[1] ? Black : White;
	while (g->BlackLeft > 0 && g->WhiteLeft > 0)
	{
		if (pausePlay)
			Pause(g);
		double bestScore;
		g->EvalCounts = 0;
		ubyte depth = (ubyte)AI(g->CurrentPlayer).SearchDepth;
		MoveSet bestSet = FindBestMoveSet(g, &bestScore, depth );
		for (int i = 0; i < bestSet.Length; i++)
		{
			DoMove(bestSet.Moves[i], g);
			ASSERT_DBG(CountAllCheckers(Black, g) == 15 && CountAllCheckers(White, g) == 15);
			if (pausePlay)
				Pause(g);
		}
		g->CurrentPlayer = OtherColor(g->CurrentPlayer);
		RollDice(g);
	}
}


void AutoPlay()
{
	int whiteWins = 0;
	int blackWins = 0;
	clock_t start = clock();
	int batch = 3000;
	bool pausePlay = G_Config.Flags & EnablePlayPause;
	for (int i = 0; i < batch; i++)
	{
		PlayGame(&G, pausePlay);
		if (G.BlackLeft == 0)
			blackWins++;
		else if (G.WhiteLeft == 0)
			whiteWins++;
		//if (i % 50 == 0)
			printf("Of: %d   White: %d   Black: %d\n", i, whiteWins, blackWins);
	}
	printf("Of: %d   White: %d (%f)   Black: %d (%f)\n", batch, whiteWins, whiteWins / (double)batch, blackWins, blackWins / (double)batch);
	printf("%fms", (float)(clock() - start) * 1000 / CLOCKS_PER_SEC);
}