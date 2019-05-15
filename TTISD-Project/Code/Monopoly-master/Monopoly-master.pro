#-------------------------------------------------
#
# Project created by QtCreator 2019-05-09T15:35:13
#
#-------------------------------------------------

QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = Monopoly-master
TEMPLATE = app

# The following define makes your compiler emit warnings if you use
# any feature of Qt which has been marked as deprecated (the exact warnings
# depend on your compiler). Please consult the documentation of the
# deprecated API in order to know how to port your code away from it.
DEFINES += QT_DEPRECATED_WARNINGS

# You can also make your code fail to compile if you use deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

CONFIG += c++11

SOURCES += \
        RPLIDAR/drvlogic/lidarmgr.cpp \
        RPLIDAR/drvlogic/stdafx.cpp \
        RPLIDAR/sdk/src/arch/win32/net_serial.cpp \
        RPLIDAR/sdk/src/arch/win32/timer.cpp \
        RPLIDAR/sdk/src/hal/thread.cpp \
        RPLIDAR/sdk/src/rplidar_driver.cpp \
        boardgame.cpp \
        Game_Board.cpp \
        action.cpp \
        bank.cpp \
        bottomBar.cpp \
        card.cpp \
        card_money.cpp \
        card_move.cpp \
        centralwidget.cpp \
        dice.cpp \
        goToAction.cpp \
        guiplayers.cpp \
        mainwindow.cpp \
        money_action.cpp \
        move_action.cpp \
        player.cpp \
        space.cpp

HEADERS += \
        Game_Board.h \
        RPLIDAR/drvlogic/common.h \
        RPLIDAR/drvlogic/lidarmgr.h \
        RPLIDAR/drvlogic/stdafx.h \
        RPLIDAR/ref/wtl/atlapp.h \
        RPLIDAR/ref/wtl/atlbase.h \
        RPLIDAR/ref/wtl/atlcrack.h \
        RPLIDAR/ref/wtl/atlctrls.h \
        RPLIDAR/ref/wtl/atlctrlw.h \
        RPLIDAR/ref/wtl/atlctrlx.h \
        RPLIDAR/ref/wtl/atlddx.h \
        RPLIDAR/ref/wtl/atldef.h \
        RPLIDAR/ref/wtl/atldlgs.h \
        RPLIDAR/ref/wtl/atldwm.h \
        RPLIDAR/ref/wtl/atlfind.h \
        RPLIDAR/ref/wtl/atlframe.h \
        RPLIDAR/ref/wtl/atlgdi.h \
        RPLIDAR/ref/wtl/atlmisc.h \
        RPLIDAR/ref/wtl/atlprint.h \
        RPLIDAR/ref/wtl/atlres.h \
        RPLIDAR/ref/wtl/atlresce.h \
        RPLIDAR/ref/wtl/atlribbon.h \
        RPLIDAR/ref/wtl/atlscrl.h \
        RPLIDAR/ref/wtl/atlsplit.h \
        RPLIDAR/ref/wtl/atltheme.h \
        RPLIDAR/ref/wtl/atluser.h \
        RPLIDAR/ref/wtl/atlwince.h \
        RPLIDAR/ref/wtl/atlwinx.h \
        RPLIDAR/sdk/include/rplidar.h \
        RPLIDAR/sdk/include/rplidar_cmd.h \
        RPLIDAR/sdk/include/rplidar_driver.h \
        RPLIDAR/sdk/include/rplidar_protocol.h \
        RPLIDAR/sdk/include/rptypes.h \
        RPLIDAR/sdk/src/arch/win32/arch_win32.h \
        RPLIDAR/sdk/src/arch/win32/net_serial.h \
        RPLIDAR/sdk/src/arch/win32/timer.h \
        RPLIDAR/sdk/src/arch/win32/winthread.hpp \
        RPLIDAR/sdk/src/hal/abs_rxtx.h \
        RPLIDAR/sdk/src/hal/assert.h \
        RPLIDAR/sdk/src/hal/byteops.h \
        RPLIDAR/sdk/src/hal/event.h \
        RPLIDAR/sdk/src/hal/locker.h \
        RPLIDAR/sdk/src/hal/socket.h \
        RPLIDAR/sdk/src/hal/thread.h \
        RPLIDAR/sdk/src/hal/types.h \
        RPLIDAR/sdk/src/hal/util.h \
        RPLIDAR/sdk/src/rplidar_driver_impl.h \
        RPLIDAR/sdk/src/rplidar_driver_serial.h \
        RPLIDAR/sdk/src/sdkcommon.h \
        RPLIDAR_settings.h \
        RPLIDAR_wrapper.h \
        action.h \
        bank.h \
        bottomBar.h \
        card.h \
        card_money.h \
        card_move.h \
        centralwidget.h \
        dice.h \
        goToAction.h \
        guiplayers.h \
        mainwindow.h \
        money_action.h \
        move_action.h \
        player.h \
        space.h \
        textbox.h

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target
