#ifndef RPLIDAR_SETTINGS_H
#define RPLIDAR_SETTINGS_H

#include <QDialog>
#include <QGridLayout>
#include <QComboBox>
#include <QPushButton>
#include <QLabel>
#include <QDoubleSpinBox>

#include "RPLIDAR_wrapper.h"

namespace lidar {
    class SettingsDialog : public QDialog {
        Q_OBJECT

        private:
            QComboBox *boxComports;
            QComboBox *boxBaudrate;
            QDoubleSpinBox *spinMin;
            QDoubleSpinBox *spinMax;
            QSpinBox       *spinSquares;
            QSpinBox       *spinThreshold;

            QPushButton *btnCalibrateMin;
            QPushButton *btnCalibrateMax;
            QPushButton *btnOk;

            QGridLayout *grid;

            lidar::Driver *driver = nullptr;

            bool initDevice() {
                if (driver == nullptr) {
                    this->driver = new Driver(this->getResult());
                    this->driver->Start();
                }

                Settings cfg;
                cfg.MINDIST = lidar::SEGMENT_DIST_START;
                cfg.MAXDIST = lidar::SEGMENT_DIST_END * 10.0;
                this->driver->setConfigDist(cfg);

                return this->driver->isAvailable();
            }

            double getDistance() {
                if (this->initDevice()) {
                    return this->driver->getClosestObjectDistance();
                }

                return 0.0;
            }

        public:
            SettingsDialog(Settings defaults = lidar::DEFAULT_SETTINGS) {
                this->grid = new QGridLayout;
                setLayout(this->grid);

                this->setWindowTitle("RPLIDAR driver settings");
                this->resize(350, 250);

                int row = 1;

                this->boxComports = new QComboBox;
                QStringList ports;
                for (int i = 0; i < 20; i++) {
                    ports.append(QString::fromStdString("COM" + std::to_string(i+1)));
                }

                this->boxComports->addItems(ports);
                this->boxComports->setCurrentIndex(defaults.COMPORT);
                this->grid->addWidget(new QLabel("Com port: "), row, 1);
                this->grid->addWidget(this->boxComports, row++, 2);

                this->boxBaudrate = new QComboBox;

                QStringList bauds{ QString::fromStdString(std::to_string(defaults.BAUD)) };
                this->boxBaudrate->addItems(bauds);
                this->grid->addWidget(new QLabel("Baud rate: "), row, 1);
                this->grid->addWidget(this->boxBaudrate, row++, 2);

                this->spinMin = new QDoubleSpinBox;
                this->spinMin->setRange(100, 400);
                this->spinMin->setValue(defaults.MINDIST);
                this->grid->addWidget(new QLabel("Min distance (mm): "), row, 1);
                this->grid->addWidget(this->spinMin, row, 2);

                this->btnCalibrateMin = new QPushButton("Calibrate");
                this->grid->addWidget(this->btnCalibrateMin, row++, 3);

                this->spinMax = new QDoubleSpinBox;
                this->spinMax->setRange(120, 500);
                this->spinMax->setValue(defaults.MAXDIST);
                this->grid->addWidget(new QLabel("Max distance (mm): "), row, 1);
                this->grid->addWidget(this->spinMax, row, 2);

                this->btnCalibrateMax = new QPushButton("Calibrate");
                this->grid->addWidget(this->btnCalibrateMax, row++, 3);

                this->spinSquares = new QSpinBox;
                this->spinSquares->setRange(4, 48);
                this->spinSquares->setValue(defaults.SQUARES);
                this->grid->addWidget(new QLabel("Game squares: "), row, 1);
                this->grid->addWidget(this->spinSquares, row++, 2);

                this->spinThreshold = new QSpinBox;
                this->spinThreshold->setRange(6, 20);
                this->spinThreshold->setValue(defaults.MATCH_THRESHOLD);
                this->grid->addWidget(new QLabel("Points match threshold: "), row, 1);
                this->grid->addWidget(this->spinThreshold, row++, 2);

                this->btnOk = new QPushButton("Continue");
                this->grid->addWidget(this->btnOk, row, 1, 1, 2);

                connect(this->btnCalibrateMin, &QPushButton::clicked, this, [this]() {
                    const double dist = this->getDistance();
                    if (dist > 0.0)
                        this->spinMin->setValue(dist);
                });
                connect(this->btnCalibrateMax, &QPushButton::clicked, this, [this]() {
                    const double dist = this->getDistance();
                    if (dist > 0.0)
                        this->spinMax->setValue(dist);
                });
                connect(this->btnOk, &QPushButton::clicked, this, [this]() {
                    accept();
                });
            }

            ~SettingsDialog() {
                delete this->boxComports;
                delete this->boxBaudrate;
                delete this->spinMin;
                delete this->spinMax;
                delete this->spinSquares;
                delete this->spinThreshold;
                delete this->grid;
                delete this->btnCalibrateMin;
                delete this->btnCalibrateMax;
                delete this->btnOk;

                delete this->driver;
            }

            lidar::Settings getResult() {
                double min = this->spinMin->value();
                double max = this->spinMax->value();
                if (max < min) std::swap(min, max);

                return lidar::Settings {
                    this->boxComports->currentIndex() + 1,
                    this->boxBaudrate->currentText().toInt(),
                    min,
                    max,
                    this->spinSquares->text().toInt(),
                    this->spinThreshold->text().toInt()
                };
            }
    };
}
#endif // RPLIDAR_SETTINGS_H
