#include <Arduino.h>
#include <SPI.h>

#include <LiquidCrystal.h>
#include <MFRC522.h>

#include "Enums.h"
#include "PrintingUtils.h"
#include "RfidUIDUtils.h"
#include "GeneralUtils.h"

// defines for LCD pins
#define LCD_RS_PIN 6
#define LCD_E_PIN 7
#define LCD_D0_PIN 5
#define LCD_D1_PIN 4
#define LCD_D2_PIN 3
#define LCD_D3_PIN 2

// Liquid Crystal monitor init
LiquidCrystal lcd(LCD_RS_PIN, LCD_E_PIN, LCD_D0_PIN, LCD_D1_PIN, LCD_D2_PIN, LCD_D3_PIN);

// defines for RFID pins
#define RFID_SS_PIN 10
#define RFID_RST_PIN 9

// RFID module init
MFRC522 rfid (RFID_SS_PIN, RFID_RST_PIN);

// Actual cards UIDs (cant be changed)
byte UID_CHZ1[] = {0xC7, 0x03, 0xDF, 0x79};
byte UID_CHZ2[] = {0x04, 0x8E, 0x56, 0xE2, 0x51, 0x23, 0x80};
byte UID_CHZ3[] = {0x04, 0x49, 0x22, 0xE2, 0x51, 0x23, 0x80};
byte UID_CHZ4[] = {0x04, 0x41, 0x16, 0xE2, 0x51, 0x23, 0x80};
byte UID_TAG[] = {0x79, 0x7A, 0xA5, 0x6D};
byte UID_TSP[] = {0x04, 0x52, 0x82, 0xE2, 0xC9, 0x5E, 0x80};
byte* UIDs[] = {UID_CHZ1, UID_CHZ2, UID_CHZ3, UID_CHZ4, UID_TAG, UID_TSP};

byte newUID[10];
bool newCardDetected = false;

// Printing utils custom functions init
PrintingUtils printingUtils(&lcd);

// Rfid uid utils custom functions init
RfidUIDUtils rfidUidUtils(&rfid);

// Rfid uid utils custom functions init
GeneralUtils generalUtils(&lcd, &rfidUidUtils, &newCardDetected);



CARD_NAME getCardName(byte* buffer){
    unsigned short int cardName = 1;
    for(byte* uid : UIDs) {
        if(RfidUIDUtils::UIDEquals(buffer, uid)) break;
        cardName += 1;
    }
    return cardName <= TSP ? (CARD_NAME) cardName : OTHER;
}

void(* softwareReboot) (void) = 0;

int chooseWhatToDo(int other, int chz1, int chz2, int chz3, int chz4, int tag, int tsp, int defaultReturn){

    // Should stop after 5s or if a card is detected
    generalUtils.tryRfidFor(5, newUID);

    // No card has been detected, go back to main menu
    if(!newCardDetected)
        return (int) NO_CARD_DETECTED;

    // Now that we have a card, let's know what to do
    CARD_NAME cardName = getCardName(newUID);
    switch (cardName) {
        case OTHER:
            return other;
        case CHZ1:
            return chz1;
        case CHZ2:
            return chz2;
        case CHZ3:
            return chz3;
        case CHZ4:
            return chz4;
        case TAG:
            return tag;
        case TSP:
            return tsp;
    }
    return defaultReturn;
}

void handleWhatToDo(int whatToDo, MENU_TYPE menuType) {
    if(whatToDo < 0) {
        printingUtils.printErr((ERR_TYPE) whatToDo);
        delay(1500);
        return;
    }

    if(whatToDo == 0) {
        printingUtils.twoLinePrinting("No selection", "Back 2 main menu");
        delay(1500);
        return;
    }

    switch (menuType) {

        case TIMER_MENU:
            switch ((TIMER_ACTIONS) whatToDo) {
                case TIMER_A_PAUSE:
                    GeneralUtils::serialPrint(TIMER_PP);
                    printingUtils.oneLineClearPrint("Pause Chosen");
                    delay(1000);
                    break;
                case TIMER_A_RESET:
                    GeneralUtils::serialPrint(TIMER_RST);
                    printingUtils.oneLineClearPrint("Reset Chosen");
                    delay(1000);
                    break;
                case TIMER_A_SHOW:
                    GeneralUtils::serialPrint(TIMER_SWITCH);
                    printingUtils.oneLineClearPrint("Show Chosen");
                    delay(1000);
                default :
                    break;
            }
            break;
        case SETTINGS_MENU:
            switch ((SETTINGS_ACTIONS) whatToDo) {
                case SETTINGS_SYNC:
                    GeneralUtils::serialPrint(SYNC);
                    printingUtils.oneLineClearPrint("Doing some sync");
                    delay(1000);
                    break;
                case SETTINGS_REBOOT:
                    printingUtils.oneLineClearPrint("Rebooting in Xs"); // X will be replaced by i+1 in the next statement
                    for (int i = 1; i <= 3; i++) { // wait for 3s
                        printingUtils.printAt(String(4-i), 13, 0);
                        delay(1000);
                    }
                    GeneralUtils::serialPrint(REBOOT);
                    printingUtils.oneLineClearPrint("Rebooting...");
                    delay(1000);
                    softwareReboot(); // noreturn function, so no need to break the switch statement
                case SETTINGS_CONTRAST:
                    printingUtils.twoLinePrinting("Hardware change", "only, open me :)");
                    delay(3000);
                    break;
            }
            break;
        case MAIN_MENU: // whatToDo should never come here so we are good!
            break;
    }
}

void handleNewCard() {
    newCardDetected = false;

    CARD_NAME cardName = getCardName(newUID);

    int whatToDo;

    switch (cardName) {

        case OTHER:
            printingUtils.printErr(CARD_NOT_RECOGNISED);
            break;

        case CHZ1:
            printingUtils.printMenu(TIMER_MENU);
            whatToDo = chooseWhatToDo(CARD_NOT_RECOGNISED, TIMER_A_PAUSE, TIMER_A_RESET, TIMER_A_SHOW, 0,
                                      CARD_NOT_EXPECTED, CARD_NOT_EXPECTED, 0);
            handleWhatToDo(whatToDo, TIMER_MENU);
            break;

        case CHZ2:

        case CHZ3:
            break;

        case CHZ4:
            printingUtils.printMenu(SETTINGS_MENU);
            whatToDo = chooseWhatToDo(CARD_NOT_RECOGNISED, SETTINGS_SYNC, SETTINGS_REBOOT, SETTINGS_CONTRAST, 0,
                                      CARD_NOT_EXPECTED, CARD_NOT_EXPECTED, 0);
            handleWhatToDo(whatToDo, SETTINGS_MENU);
            break;

        case TAG:

        case TSP:
            break;
    }
}

void setup() {
    // lcd initialization
    lcd.begin(16, 2);

    // Serial connection initialization
    Serial.begin(115200);
    while(!Serial);

    // RFID module initialization
    SPIClass::begin();
    rfid.PCD_Init();

    // Unity program initialization

    // Acknowledge that the card is ok to go
    GeneralUtils::serialPrint(READY);
    printingUtils.oneLineClearPrint("Ready");
    delay(1000);
}


void loop() {
    printingUtils.printMenu(MAIN_MENU);
    rfidUidUtils.readRFID(newUID, &newCardDetected);

    if(newCardDetected) {
        handleNewCard();
        newCardDetected = false;
    }

    delay(200);
}




// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
    String message = Serial.readStringUntil('\n');
    if (message == "LED ON") {
        lcd.setCursor(0, 1);
        lcd.print("HIGH");
    } else if (message == "LED OFF") {
        lcd.setCursor(0, 1);
        lcd.print("LOW ");
    }
}





