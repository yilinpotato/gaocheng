#include "../include/Player.h"
#include <iostream>

Player::Player(const std::string& id, Position pos, int score)
    : playerId(id), position(pos), rankingScore(score), isMatched(false) {
    requestTime = std::chrono::steady_clock::now();
}

double Player::getWaitingTime() const {
    auto now = std::chrono::steady_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::seconds>(now - requestTime);
    return duration.count();
}

std::string Player::getPositionName(Position pos) {
    switch (pos) {
        case Position::TOP: return "上单";
        case Position::JUN: return "打野";
        case Position::MID: return "中单";
        case Position::ADC: return "射手";
        case Position::SUP: return "辅助";
        default: return "未知";
    }
}

bool Player::operator<(const Player& other) const {
    // 优先队列中等待时间越长的玩家优先级越高
    // 注意：priority_queue默认是最大堆，所以这里返回反向比较
    return this->getWaitingTime() < other.getWaitingTime();
}

bool Player::operator==(const Player& other) const {
    return this->playerId == other.playerId;
}