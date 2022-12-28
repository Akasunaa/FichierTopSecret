//
// Created by asdch on 27/12/2022.
//

#include "GeneralUtils.h"

void GeneralUtils::tryRfidFor(int seconds, bool printTiming = true) {
    seconds *= 10;
    while(!(*newCardDetected) && seconds > 0) {
        if(printTiming) {
            lcd->setCursor(15, 1);
            lcd->print(seconds / 10 + 1);
        }
        rfidUidUtils->readRFID(newUID, newCardDetected);
        delay(100);
        seconds -= 1;
    }
}

size_t GeneralUtils::serialPrint(SERIAL_MESSAGE_CODE serialMessageCode) {
    switch (serialMessageCode) {

        case READY:
            return Serial.println("stg rd");
        case SYNC:
            return Serial.println("stg sy");
        case REBOOT:
            return Serial.println("stg rb");
        case TIMER_PP:
            return Serial.println("tmr pp");
        case TIMER_RST:
            return Serial.println("tmr rt");
        case TIMER_SWITCH:
            return Serial.println("tmr sc");
    }
    return 0;
}

void GeneralUtils::split(const String &string, const char &separator, String *stockIn) {
    String tmp;
    int j=0;

    for (char i : string) {
        if(i != separator)
            tmp += i;
        else {
            stockIn[j] = tmp;
            tmp = "";
            j++;
        }
    }
    stockIn[j] = tmp;
}

float GeneralUtils::floatOfStringCustom(const String &string) {
    return string.toFloat();
}

void GeneralUtils::newCardDetectionDo(void (*function)()) {
    rfidUidUtils->readRFID(newUID, newCardDetected);
    if(*newCardDetected) {
        function();
        *newCardDetected = false;
    }
}
