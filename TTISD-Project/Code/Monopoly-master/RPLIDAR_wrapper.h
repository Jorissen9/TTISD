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

            static constexpr size_t NODE_COUNT = 8192;
            static constexpr    int SEGMENT_POINTS_MATCH_THRESHOLD = 8;
            float SEGMENT_ANGLE_SWEEP = 360.0f;
            size_t measurements = 0;

            rplidar_response_measurement_node_hq_t nodes[NODE_COUNT];
            std::vector<scanObject> _scan_objects;

            std::vector<bool> result_squares;

            const rplidar_response_device_info_t& getDeviceInfo() const {
                return LidarMgr::GetInstance().devinfo;
            }

            void readDataPoints() {
                RPlidarDriver * lidar_drv = LidarMgr::GetInstance().lidar_drv;
                measurements = NODE_COUNT;

                if (IS_OK(lidar_drv->grabScanDataHq(this->nodes, measurements, 0))) {
                    // Reset
                    for (scanObject& o : this->_scan_objects) {
                        o.detected = false;
                        o.pointsMatched = 0;
                    }

                    std::fill(this->result_squares.begin(), this->result_squares.end(), false);

                    for (size_t pos = 0; pos < measurements; ++pos) {
                        if (!this->nodes[pos].dist_mm_q2) continue;

                        const scanDot dot{
                            this->nodes[pos].quality,
                            this->nodes[pos].angle_z_q14 * 90.f / 16384.f,
                            this->nodes[pos].dist_mm_q2 / 4.0f
                        };

                        // Match point with object range
                        const size_t obj_idx_start = size_t(int(dot.angle / SEGMENT_ANGLE_SWEEP) % this->settings.SQUARES);
                        scanObject *current = &this->_scan_objects[obj_idx_start];

                        if (   dot.angle >= current->angleStart && dot.angle < current->angleEnd
                            && dot.dist  >= current->distMin    && dot.dist  < current->distMax)
                        {
                            current->pointsMatched++;

                            if (current->pointsMatched > SEGMENT_POINTS_MATCH_THRESHOLD) {
                                current->detected = true;
                                this->result_squares[obj_idx_start] = true;
                            }
                        }
                    }
                }
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

                    this->SEGMENT_ANGLE_SWEEP = 360.0f / float(this->settings.SQUARES);

                    this->result_squares.resize(settings.SQUARES);
                    this->_scan_objects.resize(settings.SQUARES);
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
