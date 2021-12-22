#include <stdlib.h>
#include <Windows.h>
#include <stdio.h>
#include <stdio.h>

#include "Utils.h"
#include "Game.h"
#include "Trainer.h"
#include "Ai.h"

void TrainParaThreadStart(int threadNo) {
	PlayGame(&ThreadGames[threadNo], false);
}

// Comparing two AIs.
void PlayBatchMatch(AiConfig ai0, AiConfig ai1, int gameCount, int* score) {

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
				// Added a limit for number of moves
				ASSERT_DBG(false); // There is no draw in backgammon
			}
		}
	}
}

void NewGeneration() {

	Trainer.Generation++;
	// Select two best, and make 10 new children, randomize split pos of lists. Add a random mutation to two of them.
	double bf0[26];// = TrainedSet[0].BlotFactors;
	double bf1[26];// = TrainedSet[1].BlotFactors;
	double cf0[26];// = TrainedSet[0].ConnectedBlocksFactor;
	double cf1[26];// = TrainedSet[1].ConnectedBlocksFactor;
	memcpy(&bf0, &Trainer.Set[0].BlotFactors, 26 * sizeof(double));
	memcpy(&bf1, &Trainer.Set[1].BlotFactors, 26 * sizeof(double));
	memcpy(&cf0, &Trainer.Set[0].ConnectedBlocksFactor, 26 * sizeof(double));
	memcpy(&cf1, &Trainer.Set[1].ConnectedBlocksFactor, 26 * sizeof(double));

	for (int i = 0; i < TrainedSetCount; i++)
	{
		//combining values from 0 and 1.
		int split = RandomInt(0, 26); // todo: use mtwister
		if (split > 0)
			memcpy(&Trainer.Set[i].BlotFactors, &bf0, split * sizeof(double));
		if (split < 26)
			memcpy(&Trainer.Set[i].BlotFactors[split], &bf1[split], (26ll - split) * sizeof(double));

		split = RandomInt(0, 26); // todo: use mtwister
		if (split > 0)
			memcpy(&Trainer.Set[i].ConnectedBlocksFactor, &cf0, split * sizeof(double));
		if (split < 26)
			memcpy(&Trainer.Set[i].ConnectedBlocksFactor[split], &cf1[split], (26ll - split) * sizeof(double));
	}

	// Mutations
	int set = RandomInt(0, TrainedSetCount);
	int rndIdx = RandomInt(0, 26);
	double val = RandomDouble(0, 1);
	Trainer.Set[set].BlotFactors[rndIdx] = val;

	set = RandomInt(0, TrainedSetCount);
	rndIdx = RandomInt(0, 26);
	val = RandomDouble(0, 1);
	Trainer.Set[set].ConnectedBlocksFactor[rndIdx] = val;
}

void InitTrainer() {
	if (!LoadTrainedSet())
	{
		Trainer.Generation = 0;
		// 1. Make Random sets of Ais
		for (int i = 0; i < TrainedSetCount; i++)
		{
			InitAi(&Trainer.Set[i], false);
			Trainer.Set[i].Id = i + 1;
		}
	}

	ThreadGames = malloc(sizeof(Game) * ThreadCount);
	if (ThreadGames != NULL) {
		for (int t = 0; t < ThreadCount; t++)
			InitSeed(&ThreadGames[t], RandomInt(1, 100));
	}
}

void CheckDataIntegrity() {
	for (int i = 0; i < TrainedSetCount; i++)
	{
		if (Trainer.Set[i].Id < 1 || Trainer.Set[i].Id > TrainedSetCount) {
			printf("\nTrainedSet Id invalid");
			exit(100);
		}

		if (Trainer.Set[i].SearchDepth < 0 || Trainer.Set[i].SearchDepth > 2) {
			printf("\nTrainedSet SearchDepth invalid");
			exit(101);
		}

		if (Trainer.Set[i].Score < 0) {
			printf("\nTrainedSet Score invalid");
			exit(102);
		}

		for (int f = 0; f < 26; f++)
		{
			if (Trainer.Set[i].BlotFactors[f] < 0 || Trainer.Set[i].BlotFactors[f] > 1) {
				printf("\nTrainedSet Blotfactor invalid");
				exit(103);
			}
			if (Trainer.Set[i].ConnectedBlocksFactor[f] < 0 || Trainer.Set[i].ConnectedBlocksFactor[f] > 1) {
				printf("\nTrainedSet ConnectedBlocksFactor invalid");
				exit(104);
			}
		}
	}
}

void SaveTrainedSet(int generation) {
	CheckDataIntegrity();
	FILE* file1;
	fopen_s(&file1, "TrainedSet.bin", "wb");
	if (file1 != NULL) {
		fwrite(&Trainer, sizeof(Trainer), 1, file1);
		fclose(file1);
	}
	else {
		printf("\nError. Failed to open file TrainedSet.bin in SaveTrainedSet\n");
	}

	//Also saving each generation.
	char fName[100];
	sprintf_s(fName, sizeof(fName), "TrainedSet_Gen_%d.bin", generation);
	FILE* file2;
	fopen_s(&file2, fName, "wb");
	if (file2 != NULL) {
		fwrite(&Trainer, sizeof(Trainer), 1, file2);
		fclose(file2);
	}
	else {
		printf("\nError. Failed to open file %s in SaveTrainedSet\n", fName);
	}
}

bool LoadTrainedSet() {
	FILE* file;
	fopen_s(&file, "TrainedSet.bin", "rb");

	if (file != NULL) {
		fread(&Trainer, sizeof(Trainer), 1, file);
		fclose(file);
		return true;
	}
	else {
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
	for (int gen = 0; gen < genCount; gen++)
	{
		printf("\nGeneration %d\n", Trainer.Generation);
		printf("==============\n");
		for (int i = 0; i < TrainedSetCount; i++)
			Trainer.Set[i].Score = 0;

		// Combine all configs
		for (int i = 0; i < TrainedSetCount; i++)
		{
			for (int j = i + 1; j < TrainedSetCount; j++)
			{
				int score[2] = { 0, 0 };
				// Let them compete, 200 games
				PlayBatchMatch(Trainer.Set[i], Trainer.Set[j], 200, score);
				Trainer.Set[i].Score += score[0];
				Trainer.Set[j].Score += score[1];
				printf("\nScore for %d vs %d: %d-%d", i, j, score[0], score[1]);
			}
		}

		//free(ThreadGames);
		//sorting out best 2
		qsort(Trainer.Set, TrainedSetCount, sizeof(AiConfig), CompareTrainedSet);

		int tot = TrainedSetCount * (TrainedSetCount - 1);
		printf("\n\nTotals\n");
		for (int i = 0; i < TrainedSetCount; i++)
			printf("Wins for %d: %d\n", Trainer.Set[i].Id, Trainer.Set[i].Score);

		NewGeneration();

		SaveTrainedSet(gen);

		if (gen % 3 == 0) {
			int score[2] = { 0, 0 };
			PlayBatchMatch(Trainer.Set[0], untrained, 1000, score);
			printf("\nScore for trained vs untrained: %d-%d", score[0], score[1]);
		}
	}
}
