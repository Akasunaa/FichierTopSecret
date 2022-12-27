//
// Created by asdch on 27/12/2022.
//

#include "PrintingUtils.h"

void PrintingUtils::oneLineClearPrint(const String &line) {
    lcd->clear();
    lcd->setCursor(0, 0);
    lcd->print(line);
}

void PrintingUtils::twoLinePrinting(const String &lineOne, const String &lineTwo) {
    lcd->clear();
    lcd->setCursor(0, 0);
    lcd->print(lineOne);
    lcd->setCursor(0, 1);
    lcd->print(lineTwo);
}

void PrintingUtils::printErr(ERR_TYPE errType) {
    String lineOne = "";
    String lineTwo = "Back 2 main menu";

    switch (errType) {
        case NO_CARD_DETECTED:
            lineOne = "No card detected";
            break;
        case CARD_NOT_RECOGNISED:
            lineOne = "Unknown card";
            break;
        case CARD_NOT_EXPECTED:
            lineOne = "Unexpected card";
            break;
    }
    twoLinePrinting(lineOne, lineTwo);
}

void PrintingUtils::printMenu(MENU_TYPE menuType) {
    String lineOne = "";
    String lineTwo = "";

    switch (menuType) {

        case TIMER_MENU:
            lineOne = "1:Pause 2:Reset";
            lineTwo = "3:Show  4:Back";
            break;
        case MAIN_MENU:
            lineOne = "1:Timer 2:NotImp";
            lineTwo = "3:NotImp4:NotImp";
            break;
    }
    twoLinePrinting(lineOne, lineTwo);
}
