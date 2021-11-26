#pragma once

#define false 0
#define true 1
#define max(x, y) (((x) > (y)) ? (x) : (y))
#define min(x, y) (((x) < (y)) ? (x) : (y))

#define CheckerCount(x) ((x) & 15)
#define OtherColor(x) (~(x) & 48)

typedef char bool;
typedef unsigned long long U64;

U64 Llrand();

int RandomInt(int lower, int upper);
double RandomDouble(double lower, double upper);

bool Streq(char s1[], char s2[]);

bool StartsWith(char a[], char b[]);

void Stdout_wl(char* text);

bool Contains(char a[], char b[]);

int IndexOf(char * a, char * b);

void ConsoleWriteLine(char* text);

void SubString(char s[], char sub[], int start, int length);


typedef enum {
	black,
	blue,
	green,
	marine,
	red,
	purple,
	lightbrown,
	lightgray,
	gray,
	lightblue,
	lightgreen,
	seagreen,
	orange,
	lightpurple,
	yellow,
	white,
} ConsoleColor;

void PrintRed(char* msg);

void PrintGreen(char* msg);
void PrintInverted(char* msg);

void ColorPrint(char* text, ConsoleColor textColor, ConsoleColor background);
