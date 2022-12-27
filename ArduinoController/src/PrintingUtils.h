#ifndef ARDUINOCONTROLLER_PRINTINGUTILS_H
#define ARDUINOCONTROLLER_PRINTINGUTILS_H

#include <Arduino.h>
#include <LiquidCrystal.h>
#include "Enums.h"

class PrintingUtils {
private:
    LiquidCrystal* lcd;

public:
    explicit PrintingUtils(LiquidCrystal* lcd) : lcd(lcd){};
    void oneLineClearPrint(const String &line);
    void twoLinePrinting(const String &lineOne, const String &lineTwo);
    void printErr(ERR_TYPE errType);
    void printMenu(MENU_TYPE menuType);
    void printAt(const String& line, const uint8_t& column, const uint8_t& row);
};

#endif //ARDUINOCONTROLLER_PRINTINGUTILS_H