#include "Utils.h"
#include "Hash.h"

void InitHashes() {
	for (int p = 0; p < 2; p++)
		for (int i = 0; i < PosCount; i++)
			for (int j = 0; j < 15; j++)
				PositionHash[p][i][j] = Llrand();

	GameStartHash = Llrand();
}