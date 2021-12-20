#include <stdlib.h>;
#include <Windows.h>;
#include <stdio.h>;
#include <stdio.h>;

#include "Utils.h";
#include "Game.h";
#include "Trainer.h";
#include "Ai.h";

void TrainParaThreadStart(int threadNo) {
	PlayGame(&ThreadGames[threadNo], false);
}

// Comparing two AIs.
void PlayBatchMatch(AiConfig ai0, AiConfig ai1,int gameCount, int* score) {
	
	int batches = gameCount / ThreadCount; // batches * threads == no games.

	AIs[0] = ai0;
	AIs[1] = ai1;

	for (int i = 0; i < batches; i++)
	{
		HANDLE* handles = malloc(sizeof(HANDLE) * ThreadCount);
		for (int t = 0; t < ThreadCount; t++)
			handles[t] = CreateThread(NULL, 0, TrainParaThreadStart, t, 0, 0);

		WaitForMultipleObjects(ThreadCount, handles, TRUE, INFINITE);

		for (int t = 0; t < ThreadCount; t++)
			CloseHandle(handles[t]);

		free(handles);

		// Evaluate the games.
		for (int t = 0; t < ThreadCount; t++)
		{
			if (ThreadGames[t].BlackLeft == 0) {
				score[0] ++;
			}
			else if (ThreadGames[t].WhiteLeft == 0) {
				score[1] ++;
			}
			else {
				ASSERT_DBG(false); // There is no draw in backgammon
			}
		}
	}
}

void NewGeneration() {
	// 3. Select two best, and make 10 new children, randomize split pos of lists. Add a random mutation to two of them.
	double bf0[26];// = TrainedSet[0].BlotFactors;
	double bf1[26];// = TrainedSet[1].BlotFactors;
	double cf0[26];// = TrainedSet[0].ConnectedBlocksFactor;
	double cf1[26];// = TrainedSet[1].ConnectedBlocksFactor;
	memcpy(&bf0, &TrainedSet[0].BlotFactors, 26 * sizeof(double));
	memcpy(&bf1, &TrainedSet[1].BlotFactors, 26 * sizeof(double));
	memcpy(&cf0, &TrainedSet[0].ConnectedBlocksFactor, 26 * sizeof(double));
	memcpy(&cf1, &TrainedSet[1].ConnectedBlocksFactor, 26 * sizeof(double));

	for (int i = 0; i < TrainedSetCount; i++)
	{
		//combining values from 0 and 1.
		int split = RandomInt(0, 26); // todo: use mtwister
		if (split > 0)
			memcpy(&TrainedSet[i].BlotFactors, &bf0, split * sizeof(double));
		if (split < 26)
			memcpy(&TrainedSet[i].BlotFactors[split], &bf1[split], (26 - split) * sizeof(double));

		split = RandomInt(0, 26); // todo: use mtwister
		if (split > 0)
			memcpy(&TrainedSet[i].ConnectedBlocksFactor, &cf0, split * sizeof(double));
		if (split < 26)
			memcpy(&TrainedSet[i].ConnectedBlocksFactor[split], &cf1[split], (26 - split) * sizeof(double));
	}

	// Mutations
	int set = RandomInt(0, TrainedSetCount);
	int rndIdx = RandomInt(0, 26);
	double val = RandomDouble(0, 1);
	TrainedSet[set].BlotFactors[rndIdx] = val;

	set = RandomInt(0, TrainedSetCount);
	rndIdx = RandomInt(0, 26);
	val = RandomDouble(0, 1);
	TrainedSet[set].ConnectedBlocksFactor[rndIdx] = val;
}

void InitTrainer() {
	if (!LoadTrainedSet())
	{
		// 1. Make Random sets of Ais
		for (int i = 0; i < TrainedSetCount; i++)
			InitAi(&TrainedSet[i], false);
	}

	ThreadGames = malloc(sizeof(Game) * ThreadCount);
	for (int t = 0; t < ThreadCount; t++)
		InitSeed(&ThreadGames[t], RandomInt(1, 100));
}

void SaveTrainedSet() {
	FILE* file;
	fopen_s(&file, "TrainedSet.bin", "wb");
	if (file != NULL) {
		fwrite(&TrainedSet, sizeof(TrainedSet), 1, file);
		fclose(file);
	}
	else {
		printf("\nError. Failed to open file in SaveTrainedSet\n");
	}
}

bool LoadTrainedSet() {
	FILE* file;
	fopen_s(&file, "TrainedSet.bin", "rb");

	if (file != NULL) {
		fread(&TrainedSet, sizeof(TrainedSet), 1, file);
		fclose(file);
		return true;
	}
	else
	{
		printf("\nError. Failed to open file in LoadTrainedSet\n");
		return false;
	}
}

int CompareTrainedSet(const AiConfig* a, const AiConfig* b) {
	return b->Score - a->Score;
}

void Train() {
	InitTrainer();
	AiConfig untrained;
	InitAi(&untrained, true);
	int genCount = 100;
	for (int g = 0; g < genCount; g++)
	{
		printf("\nGeneration %d\n", g);
		printf("==============\n");
		for (int i = 0; i < TrainedSetCount; i++)
			TrainedSet[i].Score = 0;

		// Combine all configs
		for (int i = 0; i < TrainedSetCount; i++)
		{
			for (int j = i + 1; j < TrainedSetCount; j++)
			{
				int score[2] = { 0, 0 };
				// Let them compete, 200 games
				PlayBatchMatch(TrainedSet[i], TrainedSet[j], 200, score);
				TrainedSet[i].Score += score[0];
				TrainedSet[j].Score += score[1];
				printf("\nScore for %d vs %d: %d-%d", i, j, score[0], score[1]);
			}
		}

		//free(ThreadGames);
		//sorting out best 2
		qsort(TrainedSet, TrainedSetCount, sizeof(AiConfig), CompareTrainedSet);
		
		int tot = TrainedSetCount * (TrainedSetCount - 1);
		printf("\n\nTotals\n");
		for (int i = 0; i < TrainedSetCount; i++)
			printf("Wins for %d: %d\n", i, TrainedSet[i].Score);

		NewGeneration();

		SaveTrainedSet();

		if (g % 3 == 0) {			
			int score[2] = { 0, 0 };
			PlayBatchMatch(TrainedSet[0], untrained, 1000, score);
			printf("\nScore for trained vs untrained: %d-%d", score[0], score[1]);
		}
	}

	// 4. Spara historik av vinnare, kontrollera ibland var 10:e gång att utvecklingen blir bättre och bättre.

	// 5. Go to 2.	
}
