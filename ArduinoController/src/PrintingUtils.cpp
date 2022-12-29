//
// Created by asdch on 27/12/2022.
//

#include "PrintingUtils.h"

void PrintingUtils::oneLineClearPrint(const String &line) {
    baseTimerPrinted = !baseTimerPrinted;
    lcd->clear();
    lcd->setCursor(0, 0);
    lcd->print(line);
}

void PrintingUtils::twoLinePrinting(const String &lineOne, const String &lineTwo) {
    baseTimerPrinted = false;
    lcd->clear();
    lcd->setCursor(0, 0);
    lcd->print(lineOne);
    lcd->setCursor(0, 1);
    lcd->print(lineTwo);
}

void PrintingUtils::printErr(ERR_TYPE errType) {
    baseTimerPrinted = false;
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
    baseTimerPrinted = false;
    String lineOne = "";
    String lineTwo = "";

    switch (menuType) {

        case TIMER_MENU:
            lineOne = "1:Pause 2:Reset";
            lineTwo = "3:Switch4:Show";
            break;
        case MAIN_MENU:
            lineOne = "1:Timer 2:NotImp";
            lineTwo = "3:NotImp4:Settgs";
            break;
        case SETTINGS_MENU:
            lineOne = "1:Sync  2:Reboot";
            lineTwo = "3:Contr 4:Back";
            break;
    }
    twoLinePrinting(lineOne, lineTwo);
}

void PrintingUtils::printAt(const String &line, const uint8_t &column, const uint8_t &row) {
    baseTimerPrinted = false;
    lcd->setCursor(column, row);
    lcd->print(line);
}

void PrintingUtils::printTimer(const unsigned long &time) {
    if(!baseTimerPrinted) {
        lcd->clear();
        lcd->setCursor(0, 1);
        lcd->print("1:P 2:R  3:S 4:B");
        baseTimerPrinted = true;
    }
    const unsigned long minutes = time/60000;
    const unsigned long seconds = (time%60000) /1000  ;
    const unsigned long millis = (time%1000);

    lcd->setCursor(0, 0);
    lcd->print((minutes < 10 ? "0" :"") + String(minutes) + ":" + (seconds < 10 ? "0" :"") + String(seconds) + "." + (millis < 100 ? millis < 10 ? "00" : "0" :"") + String(millis));
}
