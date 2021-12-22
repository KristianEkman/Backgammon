#include "Utils.h"
#include "Hash.h"
#include "mtwister.h"

void InitHashes() {
	for (int p = 0; p < 2; p++)
		for (int i = 0; i < PosCount; i++)
			for (int j = 0; j < 15; j++)
				PositionHash[p][i][j] = Random64(&g_Rand);

	GameStartHash = Random64(&g_Rand);
}