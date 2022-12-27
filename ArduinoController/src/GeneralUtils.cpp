//
// Created by asdch on 27/12/2022.
//

#include "GeneralUtils.h"

void GeneralUtils::tryRfidFor(int seconds, byte* newUID) {
    seconds *= 10;
    while(!(*newCardDetected) && seconds > 0) {
        lcd->setCursor(15, 1);
        lcd->print(seconds/10);
        rfidUidUtils->readRFID(newUID, newCardDetected);
        delay(100);
        seconds -= 1;
    }
}
