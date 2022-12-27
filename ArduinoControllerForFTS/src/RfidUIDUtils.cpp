//
// Created by asdch on 27/12/2022.
//

#include "RfidUIDUtils.h"

void RfidUIDUtils::readRFID(byte *newUID, bool *newCardDetected) {
    // Look if a card is present
    if(!rfid->PICC_IsNewCardPresent()) return;
    // Look if the NUID has been read
    if(!rfid->PICC_ReadCardSerial()) return;

    if(rfid->uid.size > 10) return;

    for(int i=0 ; i<rfid->uid.size; i++) {
        newUID[i] = rfid->uid.uidByte[i];
    }

    *newCardDetected = true;

    // Stop rfid connection
    rfid->PICC_HaltA();
    rfid->PCD_StopCrypto1();
}

bool RfidUIDUtils::UIDEquals(const byte *buffer, const byte *other) {
    for(int i=0 ; i < 2 ; i++)
        if(buffer[i] != other[i])
            return false;
    return true;
}
