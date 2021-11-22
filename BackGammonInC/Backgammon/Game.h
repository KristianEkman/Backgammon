#pragma once

#define White 0
#define Black 16
#define ushort unsigned short
#define BUF_SIZE 5000

short Dice[2];
// 1 1111
short Position[26]; // 0 Black Bar, 1 - 24 common, 25 White Bar

short WhiteHome;
short BlackHome;

void StartPosition();
void RollDice();
void WriteGameString(char* s);
void ReadGameString(char* s);
