#ifndef RPLIDAR_WRAPPER_H
#define RPLIDAR_WRAPPER_H

#include "RPLIDAR/drvlogic/lidarmgr.h"

#include <cstdint>
#include <vector>
#include <string>
#include <thread>

#include <QMessageBox>


namespace lidar {

    static constexpr size_t SEGMENT_AMOUNT      = 40;
    static constexpr size_t SEGMENT_DIST_START  = 115;
    static constexpr size_t SEGMENT_DIST_END    = 175;
    static constexpr size_t SEGMENT_POINTS_MATCH_THRESHOLD = 10;
    static constexpr size_t NODE_COUNT          = 8192;
    static constexpr size_t THREAD_SLEEP_MS     =  250;

    struct Settings {
        int COMPORT;
        int BAUD;
        double MINDIST;
        double MAXDIST;
        int SQUARES;
        int MATCH_THRESHOLD;
    };

    static constexpr Settings DEFAULT_SETTINGS {
        4 /*5*/, 115200,
        SEGMENT_DIST_START, SEGMENT_DIST_END,
        SEGMENT_AMOUNT, SEGMENT_POINTS_MATCH_THRESHOLD
    };

    struct scanDot {
        uint8_t quality;
        float   angle;
        double  dist;
    };

    struct scanObject {
        float angleStart;
        float angleEnd;
        float distMin;
        float distMax;
        int   pointsMatched;
        bool  detected;
        double avg_dist;
    };

    struct PlayerMovement {
        int moved_squares;
        int old_position;
        int new_position;
    };

    enum class Mode {
        WORKING_MODE_IDLE       = 0,
        WORKING_MODE_SCAN       = 3,
    };

    class Driver {
        private:
            Settings settings;

            bool    driver_init_ok;
            Mode    workingMode;
            bool    support_motor_ctrl;

            //lidar changeable parameters
            uint16_t usingScanMode_;   //record the currently using scan mode

            //firmware 1.24
            std::vector<RplidarScanMode> modeVec_;

            float SEGMENT_ANGLE_SWEEP = 360.0f;
            size_t measurements = 0;

            rplidar_response_measurement_node_hq_t nodes[NODE_COUNT];
            std::vector<scanObject> _scan_objects;

            std::vector<bool> result_squares;

            void readDataPoints() {
                if (workingMode == Mode::WORKING_MODE_SCAN) {
                    RPlidarDriver * lidar_drv = LidarMgr::GetInstance().lidar_drv;
                    measurements = NODE_COUNT;

                    if (IS_OK(lidar_drv->grabScanDataHq(this->nodes, measurements, 0))) {
                        // Reset
                        for (scanObject& o : this->_scan_objects) {
                            o.detected = false;
                            o.pointsMatched = 0;
                            o.avg_dist = 0.0;
                        }

                        std::fill(this->result_squares.begin(), this->result_squares.end(), false);

                        for (size_t pos = 0; pos < measurements; ++pos) {
                            if (!this->nodes[pos].dist_mm_q2) continue;

                            const scanDot dot{
                                this->nodes[pos].quality,
                                this->nodes[pos].angle_z_q14 * 90.f / 16384.f,
                                this->nodes[pos].dist_mm_q2 / 4.0
                            };

                            // Match point with object range
                            const size_t obj_idx_start = size_t(int(dot.angle / SEGMENT_ANGLE_SWEEP) % this->settings.SQUARES);
                            scanObject *current = &this->_scan_objects[obj_idx_start];

                            if (   dot.angle >= current->angleStart    && dot.angle < current->angleEnd
                                && dot.dist  >= this->settings.MINDIST && dot.dist  < this->settings.MAXDIST)
                            {
                                current->pointsMatched++;
                                current->avg_dist += double(dot.dist);

                                if (current->pointsMatched > this->settings.MATCH_THRESHOLD) {
                                    current->detected = true;
                                    this->result_squares[obj_idx_start] = true;
                                }
                            }
                        }
                    }
                }
            }

            void initDevice() {
                this->driver_init_ok = false;
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

                    this->result_squares.clear();
                    this->_scan_objects.clear();

                    this->result_squares.resize(size_t(settings.SQUARES));
                    this->_scan_objects.resize(size_t(settings.SQUARES));
                    float angle_start = 0.0f;

                    for (scanObject &o : _scan_objects) {
                        o.angleStart = angle_start;
                        o.angleEnd = o.angleStart + SEGMENT_ANGLE_SWEEP;
                        o.pointsMatched = 0;
                        o.detected = false;
                        o.avg_dist = 0.0;

                        angle_start += SEGMENT_ANGLE_SWEEP;
                    }

                    _scan_objects.back().angleEnd = 360.0f;

                    this->setDeviceSettings();
                    std::this_thread::sleep_for(std::chrono::milliseconds(100));
                }
            }

            void setDeviceSettings() {
                RPlidarDriver * lidar_drv = LidarMgr::GetInstance().lidar_drv;

                lidar_drv->stop();
                lidar_drv->stopMotor();

                /*
                 *  Modes:  Normal, Express, Boost, Stability
                 */
                #define DEFAULT_MODE 2
                modeVec_.clear();
                lidar_drv->getAllSupportedScanModes(modeVec_);

                LidarMgr::GetInstance().lidar_drv->getTypicalScanMode(this->usingScanMode_);

                this->usingScanMode_ = this->modeVec_.size() > DEFAULT_MODE ? DEFAULT_MODE : this->usingScanMode_;

                lidar_drv->checkMotorCtrlSupport(this->support_motor_ctrl);
            }

        public:
            Driver(Settings lidar_settings)
                : settings(lidar_settings)
                , driver_init_ok(false)
                , workingMode(Mode::WORKING_MODE_IDLE)
                , support_motor_ctrl(false)
            {
                this->initDevice();
            }

            ~Driver() {
                this->Stop();
                LidarMgr::GetInstance().onDisconnect();
            }

            inline bool isAvailable() const {
                return this->driver_init_ok;
            }

            double getClosestObjectDistance() {
                this->readDataPoints();

                double min_dist = std::numeric_limits<double>::max();

                for (const scanObject& o : this->_scan_objects) {
                    if (o.detected) {
                        const double dist = o.avg_dist / double(o.pointsMatched);
                        if (dist < min_dist) min_dist = dist;
                    }
                }

                return min_dist;
            }

            const std::vector<bool> getSquares() {
                this->readDataPoints();
//                std::this_thread::sleep_for(std::chrono::milliseconds(lidar::THREAD_SLEEP_MS));
//                this->readDataPoints();
                return this->result_squares;
            }

            void Start() {
                if (this->isAvailable() && workingMode != Mode::WORKING_MODE_SCAN) {
                    LidarMgr::GetInstance().lidar_drv->startMotor();
                    LidarMgr::GetInstance().lidar_drv->startScanExpress(false, usingScanMode_);
                    workingMode = Mode::WORKING_MODE_SCAN;
                }
            }

            void Stop() {
                if (this->isAvailable() && workingMode != Mode::WORKING_MODE_IDLE) {
                    LidarMgr::GetInstance().lidar_drv->stop();
                    LidarMgr::GetInstance().lidar_drv->stopMotor();
                    workingMode = Mode::WORKING_MODE_IDLE;
                }
            }

            void setConfig(lidar::Settings cfg) {
                this->settings = cfg;

                this->Stop();
                LidarMgr::GetInstance().onDisconnect();
                this->initDevice();
                this->Start();
            }

            void setConfigDist(lidar::Settings cfg) {
                this->settings.MINDIST = cfg.MINDIST;
                this->settings.MAXDIST = cfg.MAXDIST;
            }

            lidar::Settings getConfig() const {
                return this->settings;
            }
    };
}

#endif // RPLIDAR_WRAPPER_H
