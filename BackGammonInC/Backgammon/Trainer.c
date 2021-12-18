#include <stdlib.h>;
#include <Windows.h>;
#include <stdio.h>;

#include "Utils.h";
#include "Game.h";
#include "Trainer.h";
#include "Ai.h";


void TrainParaThreadStart(int threadNo) {
	PlayGame(&ThreadGames[threadNo], false);
}

// Comparing two AIs.
void PlayBatchMatch(int ai0, int ai1, int* score) {
	int gameCount = 200;
	int batches = gameCount / ThreadCount; // batches * threads == no games.
		
	for (int i = 0; i < batches; i++)
	{
		HANDLE* handles = malloc(sizeof(HANDLE) * ThreadCount);
		for (int t = 0; t < ThreadCount; t++)
			handles[t] = CreateThread(NULL, 0, TrainParaThreadStart, t, 0, 0);

		WaitForMultipleObjects(ThreadCount, handles, TRUE, INFINITE);

		// Evaluate the games.
		for (int t = 0; t < ThreadCount; t++)
		{
			if (ThreadGames[t].BlackLeft == 0) {
				score[0] ++;
			} else if (ThreadGames[t].WhiteLeft == 0) {
				score[1] ++;
			} else {
				ASSERT_DBG(false); // There is no draw in backgammon
			}
		}
	}
}

void Train() {
	// 1. Make 10 random sets of Ais
	for (int i = 0; i < TrainedSetCount; i++)
		InitAi(&TrainedSet[i], true);

	ThreadGames = malloc(sizeof(Game) * ThreadCount);
	for (int t = 0; t < ThreadCount; t++)
		InitSeed(&ThreadGames[t], RandomInt(1, 100));

	int totalScores[TrainedSetCount] = { 0 };
	
	// Combine all configs
	for (int i = 0; i < TrainedSetCount; i++)
	{
		for (int j = i + 1; j < TrainedSetCount; j++)
		{
			int score[2] = {0, 0};
			// Let them compete, 100 games
			PlayBatchMatch(i, j, score);
			totalScores[i] += score[0];
			totalScores[j] += score[1];
			printf("\nScore for %d agains %d: %d-%d", i, j, score[0], score[1]);
		}
	}
	free(ThreadGames);
	printf("\nTotals\n");
	for (int i = 0; i < TrainedSetCount; i++)
		printf("Wins for %d: %d\n", i, totalScores[i]);

	// 3. Select two best, and make 10 new children, randomize split pos of lists. Add a random mutation to two of them.

	// 4. Spara historik av vinnare, kontrollera ibland var 10:e gång att utvecklingen blir bättre och bättre.
	
	// 5. Go to 2.	
}
