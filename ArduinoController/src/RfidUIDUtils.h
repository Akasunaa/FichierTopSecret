//
// Created by asdch on 27/12/2022.
//

#ifndef ARDUINOCONTROLLER_RFIDUIDUTILS_H
#define ARDUINOCONTROLLER_RFIDUIDUTILS_H

#include <Arduino.h>
#include <MFRC522.h>

class RfidUIDUtils {
private:
    MFRC522* rfid;

public:
    explicit RfidUIDUtils(MFRC522* rfid) : rfid(rfid) {};
    void readRFID(byte *newUID, bool *newCardDetected);
    static bool UIDEquals(const byte *buffer, const byte *other);
};

#endif //ARDUINOCONTROLLER_RFIDUIDUTILS_H