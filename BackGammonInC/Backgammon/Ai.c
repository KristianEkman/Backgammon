#include "Ai.h"
#include "Game.h"

//Negative score for leaving a blot on the board.
double BlotFactors[25];

//It is good to connect blocks since in the future the opponent might be blocked by them.
double ConnectedBlocksFactor[25];

double EvaluateCheckers(Game* g, char color) {
	double score = 0;
	for (int i = 1; i < 25; i++)
	{

		int p = color == White ? 25 - i : i;
		short v = g->Position[p];

		if (CheckerCount(v) == 1 && (v & color))
		{
			score -= (double)CheckerCount(v) * BlotFactors[p];
		}
	}
	return score;
}

