#include "guiplayers.h"

GUIPlayers::GUIPlayers(Player *p, int playerNum) {
    this->setTitleBarWidget(new QWidget());

    player = p;
    ownedProperties = new QPushButton*[40];
    ownedCount = 0;

    //player's number
    QString qNum = QString::fromStdString("Player " + std::to_string(playerNum + 1));
    numLabel = new QLabel(this);
    QFont txtFont = numLabel->font();
    txtFont.setPointSize(18);
    numLabel->setFont(txtFont);
    numLabel->setText(qNum);

    //player's money
    QString qMoney = QString::fromStdString("$" + std::to_string(player->getMoneyAmount()));
    moneyLabel = new QLabel(this);
    moneyLabel->setText(qMoney);
    txtFont.setPointSize(18);
    txtFont.setBold(true);
    moneyLabel->setFont(txtFont);
    txtFont.setBold(false);

    //current history
    histTitle = new QLabel("History:");
    txtFont.setPointSize(14);
    histTitle->setFont(txtFont);

    historyLabel = new QTextEdit(this);
    historyLabel->setReadOnly(true);
    txtFont.setPointSize(14);
    historyLabel->setFont(txtFont);

    QPalette palette = historyLabel->palette();
    palette.setColor(QPalette::Base, palette.color(QPalette::Light));
    historyLabel->setPalette(palette);
    historyLabel->setFrameStyle(QFrame::Panel);

    resetHistory();

    //player's game piece
    string tempPiece = player->getGamePieceName();
    QString qTempPiece = QString::fromStdString(tempPiece);
    gamePieceImg = new QLabel(this);
    gamePieceImg->setPixmap(qTempPiece);
    gamePieceImg->setFixedWidth(55);
    gamePieceImg->setFixedHeight(50);
    gamePieceImg->setScaledContents(true);

    //Adding all parts to layout
    histTitle   ->setAlignment(Qt::AlignLeft | Qt::AlignBottom);
    numLabel    ->setAlignment(Qt::AlignRight | Qt::AlignVCenter);
    gamePieceImg->setAlignment(Qt::AlignCenter);
    moneyLabel  ->setAlignment(Qt::AlignRight | Qt::AlignVCenter);

    gamePieceImg ->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
    numLabel     ->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
    histTitle    ->setSizePolicy(QSizePolicy::Fixed    , QSizePolicy::Fixed);
    historyLabel ->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Expanding);
    moneyLabel   ->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);

    top_layout = new QHBoxLayout;
    top_layout->addStretch(0);
    top_layout->setSizeConstraint(QLayout::SetFixedSize);
    top_layout->setAlignment(Qt::AlignCenter);
    top_layout->addWidget(histTitle     , 0, histTitle->alignment());
    top_layout->addWidget(numLabel      , 1, numLabel->alignment());
    top_layout->addWidget(gamePieceImg  , 0, gamePieceImg->alignment());
    top_layout->addWidget(moneyLabel    , 1, moneyLabel->alignment());

    layout = new QVBoxLayout;
    layout->setAlignment(Qt::AlignCenter);
    layout->addLayout(top_layout);
    layout->addWidget(historyLabel);

    sideBar = new QWidget(this);
    sideBar->setLayout(layout);
    setWidget(sideBar);

    this->setMinimumSize(QSize(300, 650));
    //  this->setMaximumSize(QSize(300, 650));
    this->setMaximumWidth(1000);
}

GUIPlayers::GUIPlayers() {

}

void GUIPlayers::setMoneyText() {
    stringstream ss;
    ss << player->getMoneyAmount();
    string tempMoney;
    ss >> tempMoney;
    tempMoney = ("$" + tempMoney);
    ss.clear();

    QString qMoney = QString::fromStdString(tempMoney);
    moneyLabel->setText(qMoney);
}

void GUIPlayers::addProperty(int propertyNum, string propertyName, Space **tempArray) {
    (void)tempArray;

    QString tempName = QString::fromStdString(propertyName);
    QPushButton *newButton = new QPushButton(tempName);
    ownedProperties[ownedCount] = newButton;
    ownedProperties[ownedCount]->setEnabled(false);
    currentPropertyNum = propertyNum;
    allProperties[ownedCount] = propertyNum;

    /*
        QSignalMapper* signalMapper = new QSignalMapper(this);

        for(int i = 0; i < ownedCount; i++){

            connect(ownedProperties[ownedCount], SIGNAL(clicked(i)), this, SLOT(upgradeSpace(i)) );

            //connect (ownedProperties[i], SIGNAL(clicked()), signalMapper, SLOT(map()));
            //signalMapper->setMapping(ownedProperties[i], i);


                    //connect(ownedProperties[i], SIGNAL(clicked()), this, SLOT(upgradeSpace()) );
        }
        //connect (signalMapper, SIGNAL(mapped(int)), this, SIGNAL(clicked(int)));
        //connect (signalMapper, SIGNAL(mapped(int)), this, SLOT(upgradeSpace(int)));

    */

    connect(ownedProperties[ownedCount], SIGNAL(clicked()), this, SLOT(upgradeSpace()));
    layout->addWidget(ownedProperties[ownedCount]);
    sideBar->setLayout(layout);
    setWidget(sideBar);
    ownedCount++;

}

void GUIPlayers::enableUpgrade() {
    for (int i = 0; i < ownedCount; i++) {
        if (((player->getMoneyAmount() - allSpaces[allProperties[i]]->getPropertyCost()) <= 0) || !(allSpaces[allProperties[i]]->getType() == "Property")) {
            ownedProperties[i]->setEnabled(false);
        } else {
            ownedProperties[i]->setEnabled(true);
        }
    }
}

void GUIPlayers::disableUpgrade() {
    for (int i = 0; i < ownedCount; i++) {
        ownedProperties[i]->setEnabled(false);
    }
}

void GUIPlayers::upgradeSpace() {
    // find out which button called the event
    auto button = qobject_cast<QPushButton *>(sender());
    int index = 0;
    QString propertyName = button->text();

    for (int i = 0; i <= 40; i++) {
        QString tempName = QString::fromStdString(allSpaces[i]->getName(0)).trimmed() + " "
                           + QString::fromStdString(allSpaces[i]->getName(1)).trimmed();

        if (tempName == propertyName.toUtf8().constData()) {
            index = i;
            break;
        }
    }

    // handel the event with the corrent index
    bool isUpgraded = allSpaces[index]->upgrade();

    if (isUpgraded == true) {
        if ((player->getMoneyAmount() - allSpaces[index]->getPropertyCost()) <= 0) {
            string text = "You do not have enough money to upgrade this property!";
            QString qText = QString::fromStdString(text);

            QMessageBox UpGradeLimit;
            UpGradeLimit.setText(qText);
            UpGradeLimit.exec();
        } else {
            moneyAction.giveBank(player, bankPointer, allSpaces[index]->getPropertyCost());
            addHistory("Player upgraded " + allSpaces[index]->getName(0) + allSpaces[index]->getName(1) + " for €" + to_string(allSpaces[index]->getPropertyCost()) +
                       " the property has now " + to_string(allSpaces[index]->getHouses()) + " houses.");
        }
    } else {
        string text = "Maximum (5) amount of upgrades reached for this property!";
        QString qText = QString::fromStdString(text);

        QMessageBox maxUpgrade;
        maxUpgrade.setText(qText);
        maxUpgrade.exec();
    }

    setMoneyText();
    disableUpgrade();

}

void GUIPlayers::setAllSpaces(Space **spaceArray) {
    allSpaces = new Space*[40];
    allSpaces = spaceArray;
}

void GUIPlayers::setBank(Bank *tempBank) {
    bankPointer = new Bank;
    bankPointer = tempBank;
}

void GUIPlayers::resetHistory() {
    historyLabel->setText("");
}

void GUIPlayers::addHistory(string action) {
    historyLabel->setHtml(historyLabel->toHtml().prepend(QString::fromStdString(action)));
    historyLabel->update();
}

