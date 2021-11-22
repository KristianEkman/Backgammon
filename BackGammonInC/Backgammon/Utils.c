#include <stdlib.h>
#include <stdio.h>
#include "utils.h"
#include <string.h>

#include <Windows.h>
#include <conio.h> 
#include <time.h>

int ParseChar(char c) {
	switch (c)
	{
	case '0': return 0;
	case '1': return 1;
	case '2': return 2;
	case '3': return 3;
	case '4': return 4;
	case '5': return 5;
	case '6': return 6;
	case '7': return 7;
	case '8': return 8;
	case '9': return 9;
	default:
		return 0;
	}
}

U64  rnd_seed = 1070372;

U64 _llrand() {

	rnd_seed ^= rnd_seed >> 12, rnd_seed ^= rnd_seed << 25, rnd_seed ^= rnd_seed >> 27;
	return rnd_seed * 2685821657736338717LL;
}

U64 Llrand() {
	U64 r = 0;

	for (int i = 0; i < 5; ++i) {
		r = (r << 15) | (rand() & 0x7FFF);
	}

	return r & 0xFFFFFFFFFFFFFFFFULL;
}
bool seeded;
int RandomInt(int lower, int upper)
{
	if (!seeded)
	{
		srand(time(0));
		seeded = true;
	}
	return (rand() % (upper - lower + 1)) + lower;
}

bool Streq(char s1[], char s2[])
{
	return strcmp(s1, s2) == 0;
}

bool StartsWith(char a[], char b[])
{
	if (strncmp(a, b, strlen(b)) == 0) return true;
	return false;
}

bool Contains(char a[], char b[]) {
	return strstr(a, b) != NULL;
}

int IndexOf(char* a, char* b) {
	char* p = strstr(a, b);
	if (p == NULL) return -1;
	return p - a;
}


void Stdout_wl(char* text) {
	printf("%s\n", text);
	fflush(stdout);
}

void ConsoleWriteLine(char* text) {
	printf("%s\n", text);
}

void printColor(char* msg, int color) {
	HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
	CONSOLE_SCREEN_BUFFER_INFO consoleInfo;
	WORD saved_attributes;

	/* Save current attributes */
	GetConsoleScreenBufferInfo(hConsole, &consoleInfo);
	saved_attributes = consoleInfo.wAttributes;

	SetConsoleTextAttribute(hConsole, color);
	printf(msg);

	/* Restore original attributes */
	SetConsoleTextAttribute(hConsole, saved_attributes);
}

void PrintRed(char* msg) {
	ColorPrint(msg, red, black);
}

void PrintGreen(char* msg) {
	ColorPrint(msg, green, black);
}

void PrintInverted(char* msg) {
	ColorPrint(msg, black, gray);
}

void ColorPrint(char* text, ConsoleColor textColor, ConsoleColor background) {
	int color = background * 16 + textColor;
	printColor(text, color);
}

void SubString(char s[], char sub[], int start, int length) {
	int i = 0;
	while (i < length && s[start + i -1]) {
		sub[i] = s[start + i - 1];
		i++;
	}
	sub[i] = '\0';
}

