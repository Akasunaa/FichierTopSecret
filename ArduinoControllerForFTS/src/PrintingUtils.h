//
// Created by asdch on 27/12/2022.
//

#ifndef TESTCARTE_PRINTINGUTILS_H
#define TESTCARTE_PRINTINGUTILS_H

#include <LiquidCrystal.h>
#include "Enums.h"

class PrintingUtils {
private:
    LiquidCrystal* lcd;
public:
    // Constructor
    explicit PrintingUtils(LiquidCrystal* lcd) : lcd(lcd){}

    // Methods
    void oneLineClearPrint(const String &line);
    void twoLinePrinting(const String& lineOne, const String& lineTwo);
    void printErr(ERR_TYPE errType);
    void printMenu(MENU_TYPE menuType);
};


#endif //TESTCARTE_PRINTINGUTILS_H
