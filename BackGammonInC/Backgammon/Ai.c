
#include "Ai.h"
#include "Game.h"
#include "Utils.h"

//Negative score for leaving a blot on the board.
double BlotFactors[25];

//It is good to connect blocks since in the future the opponent might be blocked by them.
double ConnectedBlocksFactor[25];

void InitAi() {
	for (int i = 0; i < 25; i++)
	{
		BlotFactors[i] = RandomDouble(0, 1);
		ConnectedBlocksFactor[i] = RandomDouble(0, 1);
	}
}

bool PlayersPassedEachOther(Game *g) {
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

double EvaluateCheckers(Game* g, char color) {
	double score = 0;
	int blockCount = 0;
	bool inBlock = false;
	bool playersPassed = PlayersPassedEachOther(g);
	
	for (int i = 1; i < 25; i++)
	{
		int p = color == White ? 25 - i : i;
		short v = g->Position[p];
		int checkCount = CheckerCount(v);
		if (checkCount > 1 && (v & color)) {
			if (inBlock) {
				blockCount++;
			}
			inBlock = true;
		}
		else {
			if (inBlock) {
				if (!playersPassed) {
					score += ConnectedBlocksFactor[p] * inBlock;
				}				
			}
			blockCount = 0;
			inBlock = false;
		}

		if (checkCount == 1 && (v & color))
		{
			score -= (double)CheckerCount(v) * BlotFactors[p];
		}
	}
	return score;
}

int FindBestMoveSet(Game *g) {
	int bestIdx = 0;
	double bestScore = -10000;
	CreateMoves(g);
	for (int i = 0; i < g->MoveSetsCount; i++)
	{
		for (int m = 0; m < g->SetLengths[i]; m++)
		{
			DoMove(g->PossibleMoveSets[i][m]);
			double score = EvaluateCheckers(g, g->CurrentPlayer);
			if (g->CurrentPlayer == White)
				score += (g->BlackLeft - g->WhiteLeft);			
			else
				score += (g->WhiteLeft - g->BlackLeft);

			if (score > bestScore)
			{
				bestScore = score;
				bestIdx = i;
			}
		}
	}
	return bestIdx;
}

