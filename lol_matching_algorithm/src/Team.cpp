#include "../include/Team.h"
#include <iostream>
#include <set>
#include <cmath>
#include <iomanip>

int Team::teamIdCounter = 1;

Team::Team(const std::vector<std::shared_ptr<Player>>& teamPlayers) 
    : players(teamPlayers), teamId(teamIdCounter++) {
    matchTime = std::chrono::steady_clock::now();
    
    // 将所有玩家标记为已匹配
    for (auto& player : players) {
        player->setIsMatched(true);
    }
}

bool Team::isValidTeam() const {
    if (players.size() != 5) {
        return false;
    }
    
    std::set<Position> positions;
    for (const auto& player : players) {
        positions.insert(player->getPosition());
    }
    
    // 检查是否包含所有5个位置
    return positions.size() == 5 && 
           positions.count(Position::TOP) == 1 &&
           positions.count(Position::JUN) == 1 &&
           positions.count(Position::MID) == 1 &&
           positions.count(Position::ADC) == 1 &&
           positions.count(Position::SUP) == 1;
}

double Team::getAverageScore() const {
    if (players.empty()) return 0.0;
    
    double totalScore = 0.0;
    for (const auto& player : players) {
        totalScore += player->getRankingScore();
    }
    return totalScore / players.size();
}

double Team::getScoreVariance() const {
    if (players.empty()) return 0.0;
    
    double avgScore = getAverageScore();
    double variance = 0.0;
    
    for (const auto& player : players) {
        double diff = player->getRankingScore() - avgScore;
        variance += diff * diff;
    }
    
    return variance / players.size();
}

double Team::getAverageWaitingTime() const {
    if (players.empty()) return 0.0;
    
    double totalWaitTime = 0.0;
    for (const auto& player : players) {
        totalWaitTime += player->getWaitingTime();
    }
    return totalWaitTime / players.size();
}

std::shared_ptr<Player> Team::getPlayerByPosition(Position pos) const {
    for (const auto& player : players) {
        if (player->getPosition() == pos) {
            return player;
        }
    }
    return nullptr;
}

void Team::printTeamInfo() const {
    std::cout << "\n=== 团队 #" << teamId << " 信息 ===" << std::endl;
    std::cout << "匹配时间: " << std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::steady_clock::now() - matchTime).count() << " 秒前" << std::endl;
    
    std::cout << "\n团队成员:" << std::endl;
    std::cout << std::left << std::setw(12) << "位置" 
              << std::setw(15) << "玩家ID" 
              << std::setw(10) << "分数" 
              << std::setw(15) << "等待时间(秒)" << std::endl;
    std::cout << std::string(52, '-') << std::endl;
    
    for (const auto& player : players) {
        std::cout << std::left << std::setw(12) << Player::getPositionName(player->getPosition())
                  << std::setw(15) << player->getPlayerId()
                  << std::setw(10) << player->getRankingScore()
                  << std::setw(15) << std::fixed << std::setprecision(1) 
                  << player->getWaitingTime() << std::endl;
    }
    
    std::cout << "\n团队统计:" << std::endl;
    std::cout << "平均分数: " << std::fixed << std::setprecision(1) << getAverageScore() << std::endl;
    std::cout << "分数方差: " << std::fixed << std::setprecision(1) << getScoreVariance() << std::endl;
    std::cout << "平均等待时间: " << std::fixed << std::setprecision(1) << getAverageWaitingTime() << " 秒" << std::endl;
    std::cout << "团队有效性: " << (isValidTeam() ? "有效" : "无效") << std::endl;
}