#include "../include/MatchingEngine.h"
#include <iostream>
#include <chrono>
#include <algorithm>
#include <cmath>
#include <iomanip>
#include <climits>
#include <set>

MatchingEngine::MatchingEngine(int scoreRange, double timeWt, double maxWait)
    : baseScoreRange(scoreRange), timeWeight(timeWt), maxWaitTime(maxWait),
      totalPlayersAdded(0), totalTeamsMatched(0), totalMatchingTime(0.0) {
    
    // 初始化每个位置的等待队列
    PlayerComparator comparator = [](const std::shared_ptr<Player>& a, const std::shared_ptr<Player>& b) {
        return a->getWaitingTime() < b->getWaitingTime(); // 等待时间长的优先
    };
    
    waitingQueues[Position::TOP] = PlayerQueue(comparator);
    waitingQueues[Position::JUN] = PlayerQueue(comparator);
    waitingQueues[Position::MID] = PlayerQueue(comparator);
    waitingQueues[Position::ADC] = PlayerQueue(comparator);
    waitingQueues[Position::SUP] = PlayerQueue(comparator);
}

bool MatchingEngine::addPlayer(std::shared_ptr<Player> player) {
    if (!player || player->getIsMatched()) {
        return false;
    }
    
    waitingQueues[player->getPosition()].push(player);
    totalPlayersAdded++;
    
    std::cout << "玩家 " << player->getPlayerId() 
              << " (" << Player::getPositionName(player->getPosition()) 
              << ", 分数: " << player->getRankingScore() << ") 已加入匹配队列" << std::endl;
    
    return true;
}

int MatchingEngine::performMatching() {
    auto startTime = std::chrono::steady_clock::now();
    int teamsMatchedThisRound = 0;
    
    std::cout << "\n开始执行匹配算法..." << std::endl;
    
    // 持续尝试匹配，直到无法找到更多有效团队
    while (true) {
        std::vector<std::shared_ptr<Player>> optimalTeam = findOptimalTeam();
        
        if (optimalTeam.empty()) {
            break; // 无法找到更多有效团队
        }
        
        // 创建新团队
        auto team = std::make_unique<Team>(optimalTeam);
        if (team->isValidTeam()) {
            std::cout << "成功匹配团队 #" << team->getTeamId() 
                      << " (平均分数: " << std::fixed << std::setprecision(1) 
                      << team->getAverageScore() 
                      << ", 平均等待: " << team->getAverageWaitingTime() << "秒)" << std::endl;
            
            // 从等待队列中移除已匹配的玩家
            removeMatchedPlayers(optimalTeam);
            
            matchedTeams.push_back(std::move(team));
            teamsMatchedThisRound++;
            totalTeamsMatched++;
        }
    }
    
    auto endTime = std::chrono::steady_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(endTime - startTime);
    totalMatchingTime += duration.count();
    
    std::cout << "本轮匹配完成，成功匹配 " << teamsMatchedThisRound 
              << " 个团队，耗时 " << duration.count() << " 毫秒" << std::endl;
    
    return teamsMatchedThisRound;
}

int MatchingEngine::getDynamicScoreRange(double waitingTime) const {
    // 根据等待时间动态调整分数范围
    // 基础公式：scoreRange = baseScoreRange * (1 + timeWeight * (waitingTime / maxWaitTime))
    double multiplier = 1.0 + timeWeight * (waitingTime / maxWaitTime);
    return static_cast<int>(baseScoreRange * multiplier);
}

bool MatchingEngine::canFormTeam(const std::vector<std::shared_ptr<Player>>& players) const {
    if (players.size() != 5) {
        return false;
    }
    
    // 检查位置是否完整且不重复
    std::set<Position> positions;
    for (const auto& player : players) {
        if (player->getIsMatched()) {
            return false; // 已经被匹配的玩家不能再组队
        }
        positions.insert(player->getPosition());
    }
    
    return positions.size() == 5;
}

double MatchingEngine::calculateTeamMatchScore(const std::vector<std::shared_ptr<Player>>& players) const {
    if (players.size() != 5) {
        return 0.0;
    }
    
    // 计算分数统计
    double totalScore = 0.0;
    double totalWaitTime = 0.0;
    int minScore = INT_MAX, maxScore = INT_MIN;
    
    for (const auto& player : players) {
        int score = player->getRankingScore();
        double waitTime = player->getWaitingTime();
        
        totalScore += score;
        totalWaitTime += waitTime;
        minScore = std::min(minScore, score);
        maxScore = std::max(maxScore, score);
    }
    
    double avgWaitTime = totalWaitTime / 5.0;
    int scoreDifference = maxScore - minScore;
    
    // 计算匹配分数（越高越好）
    // 考虑因素：1. 减少分数差距 2. 减少等待时间 3. 给等待时间长的玩家更高权重
    double scorePenalty = scoreDifference / 100.0; // 分数差距惩罚
    double waitTimeReward = avgWaitTime * 0.1;     // 等待时间奖励
    
    return waitTimeReward - scorePenalty + 100.0;  // 基础分数100
}

std::vector<std::shared_ptr<Player>> MatchingEngine::findOptimalTeam() {
    // 检查是否所有位置都有玩家等待
    for (const auto& queue : waitingQueues) {
        if (queue.second.empty()) {
            return {}; // 有位置没有玩家，无法组成团队
        }
    }
    
    // 贪心策略：选择等待时间最长的玩家作为起点
    Position pivotPosition = Position::TOP;
    double maxWaitTime = 0.0;
    
    // 找到等待时间最长的位置
    for (const auto& [pos, queue] : waitingQueues) {
        if (!queue.empty() && queue.top()->getWaitingTime() > maxWaitTime) {
            maxWaitTime = queue.top()->getWaitingTime();
            pivotPosition = pos;
        }
    }
    
    auto pivotPlayer = waitingQueues[pivotPosition].top();
    int pivotScore = pivotPlayer->getRankingScore();
    int dynamicRange = getDynamicScoreRange(pivotPlayer->getWaitingTime());
    
    std::vector<std::shared_ptr<Player>> candidateTeam;
    candidateTeam.push_back(pivotPlayer);
    
    // 为其他位置选择最合适的玩家
    std::vector<Position> otherPositions = {Position::TOP, Position::JUN, Position::MID, Position::ADC, Position::SUP};
    otherPositions.erase(std::find(otherPositions.begin(), otherPositions.end(), pivotPosition));
    
    for (Position pos : otherPositions) {
        std::shared_ptr<Player> bestPlayer = nullptr;
        double bestScore = -1.0;
        
        // 创建队列副本以遍历
        auto queueCopy = waitingQueues[pos];
        std::vector<std::shared_ptr<Player>> candidates;
        
        while (!queueCopy.empty()) {
            auto player = queueCopy.top();
            queueCopy.pop();
            
            if (player->getIsMatched()) {
                continue;
            }
            
            // 检查分数是否在允许范围内
            int scoreDiff = std::abs(player->getRankingScore() - pivotScore);
            if (scoreDiff <= dynamicRange) {
                candidates.push_back(player);
            }
        }
        
        if (candidates.empty()) {
            return {}; // 无法为该位置找到合适的玩家
        }
        
        // 选择最佳候选者（综合考虑等待时间和分数差距）
        for (const auto& candidate : candidates) {
            double score = candidate->getWaitingTime() * 2.0 - 
                          std::abs(candidate->getRankingScore() - pivotScore) * 0.1;
            if (score > bestScore) {
                bestScore = score;
                bestPlayer = candidate;
            }
        }
        
        if (bestPlayer) {
            candidateTeam.push_back(bestPlayer);
        } else {
            return {}; // 无法找到合适的玩家
        }
    }
    
    // 验证团队有效性
    if (canFormTeam(candidateTeam)) {
        return candidateTeam;
    }
    
    return {};
}

void MatchingEngine::removeMatchedPlayers(const std::vector<std::shared_ptr<Player>>& players) {
    for (const auto& player : players) {
        player->setIsMatched(true);
    }
    
    // 注意：由于使用了priority_queue，已匹配的玩家会在下次访问时被跳过
    // 这是一种懒删除策略，避免了重建整个队列的开销
}

std::map<Position, int> MatchingEngine::getQueueStatus() const {
    std::map<Position, int> status;
    
    for (const auto& [pos, queue] : waitingQueues) {
        int activeCount = 0;
        auto queueCopy = queue;
        
        while (!queueCopy.empty()) {
            if (!queueCopy.top()->getIsMatched()) {
                activeCount++;
            }
            queueCopy.pop();
        }
        
        status[pos] = activeCount;
    }
    
    return status;
}

void MatchingEngine::printMatchingStats() const {
    std::cout << "\n=== 匹配引擎统计信息 ===" << std::endl;
    std::cout << "总加入玩家数: " << totalPlayersAdded << std::endl;
    std::cout << "总匹配成功团队数: " << totalTeamsMatched << std::endl;
    std::cout << "总匹配耗时: " << std::fixed << std::setprecision(2) 
              << totalMatchingTime << " 毫秒" << std::endl;
    std::cout << "匹配成功率: " << std::fixed << std::setprecision(1) 
              << (totalPlayersAdded > 0 ? (totalTeamsMatched * 5.0 / totalPlayersAdded * 100.0) : 0.0) 
              << "%" << std::endl;
    
    std::cout << "\n当前队列状态:" << std::endl;
    auto queueStatus = getQueueStatus();
    for (const auto& [pos, count] : queueStatus) {
        std::cout << Player::getPositionName(pos) << ": " << count << " 人等待" << std::endl;
    }
    
    if (!matchedTeams.empty()) {
        std::cout << "\n团队质量统计:" << std::endl;
        double totalAvgScore = 0.0, totalVariance = 0.0, totalWaitTime = 0.0;
        
        for (const auto& team : matchedTeams) {
            totalAvgScore += team->getAverageScore();
            totalVariance += team->getScoreVariance();
            totalWaitTime += team->getAverageWaitingTime();
        }
        
        int teamCount = matchedTeams.size();
        std::cout << "平均团队分数: " << std::fixed << std::setprecision(1) 
                  << totalAvgScore / teamCount << std::endl;
        std::cout << "平均分数方差: " << std::fixed << std::setprecision(1) 
                  << totalVariance / teamCount << std::endl;
        std::cout << "平均等待时间: " << std::fixed << std::setprecision(1) 
                  << totalWaitTime / teamCount << " 秒" << std::endl;
    }
}

void MatchingEngine::reset() {
    // 清空所有队列
    for (auto& [pos, queue] : waitingQueues) {
        while (!queue.empty()) {
            queue.pop();
        }
    }
    
    matchedTeams.clear();
    totalPlayersAdded = 0;
    totalTeamsMatched = 0;
    totalMatchingTime = 0.0;
    
    std::cout << "匹配引擎已重置" << std::endl;
}