//
// Created by asdch on 27/12/2022.
//

#ifndef TESTCARTE_ENUMS_H
#define TESTCARTE_ENUMS_H

enum ERR_TYPE {
    NO_CARD_DETECTED = -100,
    CARD_NOT_RECOGNISED = -200,
    CARD_NOT_EXPECTED = -300
};

enum MENU_TYPE {
    TIMER_MENU = 100
};

enum TIMER_ACTION {
    TIMER_PAUSE = 1,
    TIMER_RESET = 2,
    TIMER_SHOW = 3
};

enum CARD_NAME {
    OTHER = 999,
    CHZ1 = 1,
    CHZ2,
    CHZ3,
    CHZ4,
    TAG,
    TSP // =6
};

#endif //TESTCARTE_ENUMS_H
