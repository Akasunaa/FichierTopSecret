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

size_t GeneralUtils::serialPrint(SERIAL_MESSAGE_CODE serialMessageCode) {
    switch (serialMessageCode) {

        case READY:
            return Serial.print("stg rd");
        case SYNC:
            return Serial.print("stg s");
        case REBOOT:
            return Serial.print("stg rb");
        case TIMER_PP:
            return Serial.print("timer p");
        case TIMER_RST:
            return Serial.print("timer r");
        case TIMER_SWITCH:
            return Serial.print("timer s");
    }
    return 0;
}
