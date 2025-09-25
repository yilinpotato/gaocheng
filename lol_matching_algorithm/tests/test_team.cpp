#include "../include/Team.h"
#include <iostream>
#include <cassert>
#include <memory>
#include <cmath>

/**
 * @brief 测试Team类的基本功能
 */
void testTeamBasicFunctionality() {
    std::cout << "测试Team类基本功能..." << std::endl;
    
    // 创建完整的5人团队
    std::vector<std::shared_ptr<Player>> players = {
        std::make_shared<Player>("Player1", Position::TOP, 1500),
        std::make_shared<Player>("Player2", Position::JUN, 1520),
        std::make_shared<Player>("Player3", Position::MID, 1480),
        std::make_shared<Player>("Player4", Position::ADC, 1510),
        std::make_shared<Player>("Player5", Position::SUP, 1490)
    };
    
    Team team(players);
    
    // 测试基本属性
    assert(team.getPlayers().size() == 5);
    assert(team.getTeamId() > 0);
    assert(team.isValidTeam());
    
    // 测试平均分数计算
    double expectedAvg = (1500 + 1520 + 1480 + 1510 + 1490) / 5.0;
    assert(std::abs(team.getAverageScore() - expectedAvg) < 0.1);
    
    std::cout << "Team类基本功能测试通过" << std::endl;
}

/**
 * @brief 测试团队有效性验证
 */
void testTeamValidation() {
    std::cout << "测试团队有效性验证..." << std::endl;
    
    // 测试有效团队
    std::vector<std::shared_ptr<Player>> validTeam = {
        std::make_shared<Player>("P1", Position::TOP, 1500),
        std::make_shared<Player>("P2", Position::JUN, 1500),
        std::make_shared<Player>("P3", Position::MID, 1500),
        std::make_shared<Player>("P4", Position::ADC, 1500),
        std::make_shared<Player>("P5", Position::SUP, 1500)
    };
    Team team1(validTeam);
    assert(team1.isValidTeam());
    
    // 测试无效团队（缺少玩家）
    std::vector<std::shared_ptr<Player>> invalidTeam1 = {
        std::make_shared<Player>("P1", Position::TOP, 1500),
        std::make_shared<Player>("P2", Position::JUN, 1500),
        std::make_shared<Player>("P3", Position::MID, 1500)
    };
    Team team2(invalidTeam1);
    assert(!team2.isValidTeam());
    
    // 测试无效团队（重复位置）
    std::vector<std::shared_ptr<Player>> invalidTeam2 = {
        std::make_shared<Player>("P1", Position::TOP, 1500),
        std::make_shared<Player>("P2", Position::TOP, 1500), // 重复位置
        std::make_shared<Player>("P3", Position::MID, 1500),
        std::make_shared<Player>("P4", Position::ADC, 1500),
        std::make_shared<Player>("P5", Position::SUP, 1500)
    };
    Team team3(invalidTeam2);
    assert(!team3.isValidTeam());
    
    std::cout << "团队有效性验证测试通过" << std::endl;
}

/**
 * @brief 测试团队统计功能
 */
void testTeamStatistics() {
    std::cout << "测试团队统计功能..." << std::endl;
    
    std::vector<std::shared_ptr<Player>> players = {
        std::make_shared<Player>("P1", Position::TOP, 1000),
        std::make_shared<Player>("P2", Position::JUN, 1200),
        std::make_shared<Player>("P3", Position::MID, 1400),
        std::make_shared<Player>("P4", Position::ADC, 1600),
        std::make_shared<Player>("P5", Position::SUP, 1800)
    };
    
    Team team(players);
    
    // 测试平均分数
    double expectedAvg = (1000 + 1200 + 1400 + 1600 + 1800) / 5.0;
    assert(std::abs(team.getAverageScore() - expectedAvg) < 0.1);
    
    // 测试分数方差
    double variance = 0.0;
    for (const auto& player : players) {
        double diff = player->getRankingScore() - expectedAvg;
        variance += diff * diff;
    }
    variance /= 5.0;
    assert(std::abs(team.getScoreVariance() - variance) < 0.1);
    
    // 测试按位置获取玩家
    auto topPlayer = team.getPlayerByPosition(Position::TOP);
    assert(topPlayer != nullptr);
    assert(topPlayer->getPlayerId() == "P1");
    
    auto invalidPlayer = team.getPlayerByPosition(static_cast<Position>(-1));
    assert(invalidPlayer == nullptr);
    
    std::cout << "团队统计功能测试通过" << std::endl;
}

/**
 * @brief 测试团队信息打印
 */
void testTeamInfoPrinting() {
    std::cout << "测试团队信息打印..." << std::endl;
    
    std::vector<std::shared_ptr<Player>> players = {
        std::make_shared<Player>("Alice", Position::TOP, 1500),
        std::make_shared<Player>("Bob", Position::JUN, 1520),
        std::make_shared<Player>("Charlie", Position::MID, 1480),
        std::make_shared<Player>("David", Position::ADC, 1510),
        std::make_shared<Player>("Eva", Position::SUP, 1490)
    };
    
    Team team(players);
    
    // 这里只是确保打印功能不会崩溃
    std::cout << "--- 团队信息打印测试 ---" << std::endl;
    team.printTeamInfo();
    
    std::cout << "团队信息打印测试通过" << std::endl;
}

int main() {
    std::cout << "=== Team类单元测试 ===" << std::endl;
    
    try {
        testTeamBasicFunctionality();
        testTeamValidation();
        testTeamStatistics();
        testTeamInfoPrinting();
        
        std::cout << "\n所有Team类测试通过！" << std::endl;
    } catch (const std::exception& e) {
        std::cerr << "测试失败: " << e.what() << std::endl;
        return 1;
    }
    
    return 0;
}