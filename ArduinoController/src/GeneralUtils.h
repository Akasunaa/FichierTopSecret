//
// Created by asdch on 27/12/2022.
//

#ifndef ARDUINOCONTROLLER_GENERALUTILS_H
#define ARDUINOCONTROLLER_GENERALUTILS_H

#include <Arduino.h>
#include <LiquidCrystal.h>

#include "Enums.h"

#include "RfidUIDUtils.h"

class GeneralUtils {
private:
    LiquidCrystal* lcd;
    RfidUIDUtils* rfidUidUtils;
    bool* newCardDetected;
    byte* newUID;

public:
    explicit GeneralUtils(LiquidCrystal* lcd, RfidUIDUtils* rfid, bool* newCardDetected, byte* newUID) : lcd(lcd), rfidUidUtils(rfid), newCardDetected(newCardDetected), newUID(newUID) {};

    void tryRfidFor(int seconds, bool printTiming);
    void newCardDetectionDo(void (*function)());
    static String syncParamToString(const bool *syncParams, size_t syncParamsSize);
    static size_t serialPrint(SERIAL_MESSAGE_CODE serialMessageCode);
    static size_t serialPrintSync(const String &syncElements);
    static size_t serialPrintTime(unsigned long time);
    static void split(const String &string, const char &separator, String *stockIn);
    static float floatOfStringCustom(const String& string);


};


#endif //ARDUINOCONTROLLER_GENERALUTILS_H
