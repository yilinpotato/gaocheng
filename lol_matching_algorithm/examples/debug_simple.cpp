#include "../include/MatchingEngine.h"
#include <iostream>

/**
 * @brief 调试简单使用示例
 */
int main() {
    std::cout << "=== LOL匹配算法调试示例 ===" << std::endl;
    
    // 1. 创建匹配引擎，使用更宽松的参数
    MatchingEngine engine(500, 1.0, 300.0);  // 增大基础分数范围到500
    
    // 2. 创建玩家
    auto player1 = std::make_shared<Player>("上单小王", Position::TOP, 1500);
    auto player2 = std::make_shared<Player>("打野小李", Position::JUN, 1520);
    auto player3 = std::make_shared<Player>("中单小张", Position::MID, 1480);
    auto player4 = std::make_shared<Player>("射手小赵", Position::ADC, 1510);
    auto player5 = std::make_shared<Player>("辅助小孙", Position::SUP, 1490);
    
    // 测试动态分数范围
    std::cout << "测试动态分数范围:" << std::endl;
    std::cout << "等待时间 0s -> 分数范围: ±" << engine.getDynamicScoreRange(0) << std::endl;
    std::cout << "等待时间 60s -> 分数范围: ±" << engine.getDynamicScoreRange(60) << std::endl;
    
    // 检查分数差距
    std::cout << "\n玩家分数分析:" << std::endl;
    std::vector<std::shared_ptr<Player>> players = {player1, player2, player3, player4, player5};
    for (const auto& p : players) {
        std::cout << Player::getPositionName(p->getPosition()) << ": " << p->getRankingScore() << std::endl;
    }
    
    int minScore = 1480, maxScore = 1520;
    int scoreDiff = maxScore - minScore;
    std::cout << "最大分数差距: " << scoreDiff << std::endl;
    std::cout << "需要的最小分数范围: " << scoreDiff << std::endl;
    std::cout << "当前动态分数范围: " << engine.getDynamicScoreRange(0) << std::endl;
    
    // 加入队列并匹配
    std::cout << "\n加入匹配队列..." << std::endl;
    for (const auto& player : players) {
        engine.addPlayer(player);
    }
    
    std::cout << "\n开始匹配..." << std::endl;
    int matched = engine.performMatching();
    std::cout << "匹配结果: " << matched << " 个团队" << std::endl;
    
    engine.printMatchingStats();
    
    return 0;
}