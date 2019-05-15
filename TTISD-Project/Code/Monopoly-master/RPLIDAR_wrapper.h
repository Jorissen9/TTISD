#ifndef RPLIDAR_WRAPPER_H
#define RPLIDAR_WRAPPER_H

#include "RPLIDAR/drvlogic/lidarmgr.h"

#include <cstdint>
#include <vector>

#define    SCANMODE_SUB 1

namespace lidar {
    enum class Mode {
        WORKING_MODE_IDLE       = 0,
        WORKING_MODE_SCAN       = 3,
    };

    class Driver {
        private:
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
        public:
            Driver() {}
            ~Driver() {}
    };
}

#endif // RPLIDAR_WRAPPER_H
