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
        private:
            QComboBox *boxComports;
            QComboBox *boxBaudrate;
            QDoubleSpinBox *spinMin;
            QDoubleSpinBox *spinMax;
            QSpinBox       *spinSquares;

            QGridLayout *grid;

        public:
            SettingsDialog(void) {
                this->grid = new QGridLayout;
                setLayout(this->grid);

                this->setWindowTitle("RPLIDAR driver settings");
                this->resize(350, 250);

                this->boxComports = new QComboBox;
                QStringList ports;
                for (int i = 0; i < 20; i++) {
                    ports.append(QString::fromStdString("COM" + std::to_string(i+1)));
                }

                this->boxComports->addItems(ports);
                this->boxComports->setCurrentIndex(4);
                this->grid->addWidget(new QLabel("Com port: "), 1, 1);
                this->grid->addWidget(this->boxComports, 1, 2);

                this->boxBaudrate = new QComboBox;

                QStringList bauds{ "115200" };
                this->boxBaudrate->addItems(bauds);
                this->grid->addWidget(new QLabel("Baud rate: "), 2, 1);
                this->grid->addWidget(this->boxBaudrate, 2, 2);

                this->spinMin = new QDoubleSpinBox;
                this->spinMin->setRange(250, 400);
                this->spinMin->setValue(300);
                this->grid->addWidget(new QLabel("Min distance (mm): "), 3, 1);
                this->grid->addWidget(this->spinMin, 3, 2);

                this->spinMax = new QDoubleSpinBox;
                this->spinMax->setRange(300, 500);
                this->spinMax->setValue(400);
                this->grid->addWidget(new QLabel("Max distance (mm): "), 4, 1);
                this->grid->addWidget(this->spinMax, 4, 2);

                this->spinSquares = new QSpinBox;
                this->spinSquares->setRange(4, 48);
                this->spinSquares->setValue(40);
                this->grid->addWidget(new QLabel("Game squares: "), 5, 1);
                this->grid->addWidget(this->spinSquares, 5, 2);


                QPushButton *ok = new QPushButton("Continue");
                this->grid->addWidget(ok, 6, 1, 1, 2);
                connect(ok, &QPushButton::clicked, this, [this]() {
                    accept();
                });
            }

            ~SettingsDialog() {
                delete this->boxComports;
                delete this->boxBaudrate;
                delete this->spinMin;
                delete this->spinMax;
                delete this->spinSquares;
                delete this->grid;
            }

            lidar::Settings getResult() {
                return lidar::Settings {
                    this->boxComports->currentIndex() + 1,
                    this->boxBaudrate->currentText().toInt(),
                    this->spinMin->value(),
                    this->spinMax->value(),
                    this->spinSquares->text().toInt()
                };
            }
    };
}
#endif // RPLIDAR_SETTINGS_H
