//
// Created by asdch on 27/12/2022.
//

#ifndef ARDUINOCONTROLLER_GENERALUTILS_H
#define ARDUINOCONTROLLER_GENERALUTILS_H

#include <Arduino.h>
#include <LiquidCrystal.h>
#include "RfidUIDUtils.h"

class GeneralUtils {
private:
    LiquidCrystal* lcd;
    RfidUIDUtils* rfidUidUtils;
    bool* newCardDetected;

public:
    explicit GeneralUtils(LiquidCrystal* lcd, RfidUIDUtils* rfid, bool* newCardDetected) : lcd(lcd), rfidUidUtils(rfid), newCardDetected(newCardDetected) {};

    void tryRfidFor(int seconds, byte* newUID);
};


#endif //ARDUINOCONTROLLER_GENERALUTILS_H
