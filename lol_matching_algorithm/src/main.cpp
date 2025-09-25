#include "../include/MatchingEngine.h"
#include <iostream>
#include <vector>
#include <random>
#include <thread>
#include <chrono>

/**
 * @brief 生成随机玩家数据
 * @param count 生成的玩家数量
 * @param minScore 最低分数
 * @param maxScore 最高分数
 * @return 生成的玩家列表
 */
std::vector<std::shared_ptr<Player>> generateRandomPlayers(int count, int minScore = 1000, int maxScore = 3000) {
    std::vector<std::shared_ptr<Player>> players;
    std::random_device rd;
    std::mt19937 gen(rd());
    std::uniform_int_distribution<> scoreDist(minScore, maxScore);
    std::uniform_int_distribution<> posDist(0, 4);
    
    std::vector<Position> positions = {Position::TOP, Position::JUN, Position::MID, Position::ADC, Position::SUP};
    
    for (int i = 0; i < count; ++i) {
        std::string playerId = "Player_" + std::to_string(i + 1);
        Position pos = positions[posDist(gen)];
        int score = scoreDist(gen);
        
        auto player = std::make_shared<Player>(playerId, pos, score);
        players.push_back(player);
        
        // 模拟不同的请求时间
        if (i % 3 == 0) {
            std::this_thread::sleep_for(std::chrono::milliseconds(10));
        }
    }
    
    return players;
}

/**
 * @brief 测试基本匹配功能
 */
void testBasicMatching() {
    std::cout << "\n" << std::string(60, '=') << std::endl;
    std::cout << "           基本匹配功能测试" << std::endl;
    std::cout << std::string(60, '=') << std::endl;
    
    MatchingEngine engine(200, 1.0, 300.0);
    
    // 创建一组完整的玩家（每个位置至少一个）
    std::vector<std::shared_ptr<Player>> testPlayers = {
        std::make_shared<Player>("Alice", Position::TOP, 1500),
        std::make_shared<Player>("Bob", Position::JUN, 1520),
        std::make_shared<Player>("Charlie", Position::MID, 1480),
        std::make_shared<Player>("David", Position::ADC, 1510),
        std::make_shared<Player>("Eva", Position::SUP, 1490),
        std::make_shared<Player>("Frank", Position::TOP, 1600),
        std::make_shared<Player>("Grace", Position::JUN, 1580),
        std::make_shared<Player>("Henry", Position::MID, 1620),
        std::make_shared<Player>("Ivy", Position::ADC, 1590),
        std::make_shared<Player>("Jack", Position::SUP, 1570)
    };
    
    // 添加玩家到匹配队列
    for (auto& player : testPlayers) {
        engine.addPlayer(player);
        std::this_thread::sleep_for(std::chrono::milliseconds(100)); // 模拟不同请求时间
    }
    
    // 执行匹配
    engine.performMatching();
    
    // 打印统计信息
    engine.printMatchingStats();
    
    // 打印每个匹配的团队详细信息
    const auto& teams = engine.getMatchedTeams();
    for (const auto& team : teams) {
        team->printTeamInfo();
    }
}

/**
 * @brief 测试大规模匹配性能
 */
void testScalabilityPerformance() {
    std::cout << "\n" << std::string(60, '=') << std::endl;
    std::cout << "           大规模匹配性能测试" << std::endl;
    std::cout << std::string(60, '=') << std::endl;
    
    MatchingEngine engine(300, 1.5, 600.0);
    
    // 生成大量随机玩家
    auto players = generateRandomPlayers(100);
    
    auto startTime = std::chrono::high_resolution_clock::now();
    
    // 添加所有玩家
    for (auto& player : players) {
        engine.addPlayer(player);
    }
    
    // 执行匹配
    engine.performMatching();
    
    auto endTime = std::chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(endTime - startTime);
    
    std::cout << "处理 " << players.size() << " 个玩家，总耗时: " 
              << duration.count() << " 毫秒" << std::endl;
    
    engine.printMatchingStats();
}

/**
 * @brief 测试动态分数范围调整
 */
void testDynamicScoreRange() {
    std::cout << "\n" << std::string(60, '=') << std::endl;
    std::cout << "           动态分数范围调整测试" << std::endl;
    std::cout << std::string(60, '=') << std::endl;
    
    MatchingEngine engine(200, 2.0, 300.0);
    
    std::cout << "测试不同等待时间的分数范围调整:" << std::endl;
    for (int waitTime : {0, 60, 120, 300, 600}) {
        int range = engine.getDynamicScoreRange(waitTime);
        std::cout << "等待时间 " << waitTime << " 秒 -> 分数范围: ±" << range << std::endl;
    }
    
    // 创建分数差距较大的玩家
    std::vector<std::shared_ptr<Player>> diversePlayers = {
        std::make_shared<Player>("HighElo_TOP", Position::TOP, 2500),
        std::make_shared<Player>("LowElo_JUN", Position::JUN, 1200),
        std::make_shared<Player>("MidElo_MID", Position::MID, 1800),
        std::make_shared<Player>("HighElo_ADC", Position::ADC, 2300),
        std::make_shared<Player>("LowElo_SUP", Position::SUP, 1100)
    };
    
    for (auto& player : diversePlayers) {
        engine.addPlayer(player);
        std::this_thread::sleep_for(std::chrono::seconds(1)); // 增加等待时间
    }
    
    std::cout << "\n等待5秒以增加等待时间权重..." << std::endl;
    std::this_thread::sleep_for(std::chrono::seconds(5));
    
    engine.performMatching();
    engine.printMatchingStats();
}

/**
 * @brief 演示实时匹配场景
 */
void demonstrateRealTimeMatching() {
    std::cout << "\n" << std::string(60, '=') << std::endl;
    std::cout << "           实时匹配场景演示" << std::endl;
    std::cout << std::string(60, '=') << std::endl;
    
    MatchingEngine engine(250, 1.2, 180.0);
    
    std::cout << "模拟玩家陆续加入匹配队列的实时场景..." << std::endl;
    
    auto players = generateRandomPlayers(25);
    
    for (size_t i = 0; i < players.size(); ++i) {
        engine.addPlayer(players[i]);
        
        // 每添加5个玩家尝试一次匹配
        if ((i + 1) % 5 == 0) {
            std::cout << "\n--- 第 " << (i + 1) / 5 << " 轮匹配尝试 ---" << std::endl;
            int matched = engine.performMatching();
            if (matched > 0) {
                std::cout << "本轮成功匹配 " << matched << " 个团队" << std::endl;
            }
            
            // 显示当前队列状态
            auto queueStatus = engine.getQueueStatus();
            std::cout << "当前等待队列: ";
            for (const auto& [pos, count] : queueStatus) {
                std::cout << Player::getPositionName(pos) << "(" << count << ") ";
            }
            std::cout << std::endl;
        }
        
        std::this_thread::sleep_for(std::chrono::milliseconds(200));
    }
    
    // 最终匹配尝试
    std::cout << "\n--- 最终匹配尝试 ---" << std::endl;
    engine.performMatching();
    engine.printMatchingStats();
}

int main() {
    std::cout << std::string(80, '=') << std::endl;
    std::cout << "                 LOL游戏匹配算法演示程序" << std::endl;
    std::cout << "                      高级程序语言设计项目" << std::endl;
    std::cout << std::string(80, '=') << std::endl;
    
    try {
        // 测试1: 基本匹配功能
        testBasicMatching();
        
        // 等待用户确认
        std::cout << "\n按 Enter 键继续下一个测试..." << std::endl;
        std::cin.get();
        
        // 测试2: 大规模性能测试
        testScalabilityPerformance();
        
        std::cout << "\n按 Enter 键继续下一个测试..." << std::endl;
        std::cin.get();
        
        // 测试3: 动态分数范围调整
        testDynamicScoreRange();
        
        std::cout << "\n按 Enter 键继续下一个测试..." << std::endl;
        std::cin.get();
        
        // 测试4: 实时匹配演示
        demonstrateRealTimeMatching();
        
    } catch (const std::exception& e) {
        std::cerr << "程序执行出错: " << e.what() << std::endl;
        return 1;
    }
    
    std::cout << "\n" << std::string(80, '=') << std::endl;
    std::cout << "                      测试完成！" << std::endl;
    std::cout << std::string(80, '=') << std::endl;
    
    return 0;
}