#ifndef RPLIDAR_WRAPPER_H
#define RPLIDAR_WRAPPER_H

#include "RPLIDAR/drvlogic/lidarmgr.h"

#include <cstdint>
#include <vector>
#include <string>

#include <QMessageBox>

#define    SCANMODE_SUB 1

namespace lidar {

    struct Settings {
        int COMPORT;
        int BAUD;
        double MINDIST;
        double MAXDIST;
        int SQUARES;
    };

    struct scanDot {
        uint8_t quality;
        float   angle;
        float   dist;
    };

    struct scanObject {
        float angleStart;
        float angleEnd;
        float distMin;
        float distMax;
        int   pointsMatched;
        bool  detected;
    };

    enum class Mode {
        WORKING_MODE_IDLE       = 0,
        WORKING_MODE_SCAN       = 3,
    };

    class Driver {
        private:
            Settings settings;

            bool    driver_init_ok;
            int     workingMode; // 0 - idle 1 - framegrabber
            bool    forcescan;
            bool    useExpressMode;
            bool    inExpressMode;
            bool    support_motor_ctrl;

            //lidar changeable parameters
            uint16_t usingScanMode_;   //record the currently using scan mode

            //firmware 1.24
            std::vector<RplidarScanMode> modeVec_;

            rplidar_response_device_info_t devInfo;

            std::vector<bool> result_squares;

            void getDeviceInfo() {
                devInfo;
            }

            void readDataPoints() {

            }

            void setDeviceSettings() {

            }
        public:
            Driver(Settings lidar_settings)
                : settings(lidar_settings)
                , driver_init_ok(false)
                , workingMode(0)
                , forcescan(false)
                , useExpressMode(true)
                , inExpressMode(false)
                , support_motor_ctrl(false)
            {
                std::string serialpath = "\\\\.\\com" + std::to_string(settings.COMPORT);

                if (!LidarMgr::GetInstance().onConnect(serialpath.c_str(), settings.BAUD)) {
                    QMessageBox::StandardButton result = QMessageBox::question(nullptr,
                                                                               QString::fromStdString("RPLIDAR driver error"),
                                                                               QString::fromStdString("Could not bind to COM port.\nAbort or continue with game?"),
                                                                               QMessageBox::Abort | QMessageBox::Ignore, QMessageBox::Abort);

                    if (result == QMessageBox::Abort) {
                        std::abort();
                    }
                } else {
                    QMessageBox m;
                    m.setText(QString::fromStdString("Conected to RPLIDAR at " + serialpath));
                    m.setWindowTitle(QString::fromStdString("RPLIDAR driver"));
                    m.exec();
                    driver_init_ok = true;

                    result_squares.resize(settings.SQUARES);
                }
            }

            ~Driver() {}

            inline bool isAvailable() const {
                return this->driver_init_ok;
            }

            const std::vector<bool>& getSquares() const {
                return this->result_squares;
            }
    };
}

#endif // RPLIDAR_WRAPPER_H
