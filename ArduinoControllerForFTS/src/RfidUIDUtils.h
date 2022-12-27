//
// Created by asdch on 27/12/2022.
//

#ifndef TESTCARTE_RFIDUIDUTILS_H
#define TESTCARTE_RFIDUIDUTILS_H

#include <MFRC522.h>
#include <SPI.h>
#include <Arduino.h>


class RfidUIDUtils {
private:
    MFRC522* rfid;
public:
    explicit RfidUIDUtils(MFRC522* rfid) : rfid(rfid) {}
    void readRFID(byte* newUID, bool* newCardDetected);
    static bool UIDEquals(const byte* buffer, const byte* other);
};


#endif //TESTCARTE_RFIDUIDUTILS_H
