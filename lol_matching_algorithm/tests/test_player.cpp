#include "../include/Player.h"
#include <iostream>
#include <cassert>
#include <thread>
#include <chrono>

/**
 * @brief 测试Player类的基本功能
 */
void testPlayerBasicFunctionality() {
    std::cout << "测试Player类基本功能..." << std::endl;
    
    // 创建玩家
    Player player("TestPlayer", Position::MID, 1500);
    
    // 测试基本属性
    assert(player.getPlayerId() == "TestPlayer");
    assert(player.getPosition() == Position::MID);
    assert(player.getRankingScore() == 1500);
    assert(!player.getIsMatched());
    
    // 测试等待时间
    std::this_thread::sleep_for(std::chrono::milliseconds(100));
    double waitTime = player.getWaitingTime();
    assert(waitTime > 0);
    
    // 测试匹配状态设置
    player.setIsMatched(true);
    assert(player.getIsMatched());
    
    std::cout << "Player类基本功能测试通过" << std::endl;
}

/**
 * @brief 测试位置名称转换
 */
void testPositionNameConversion() {
    std::cout << "测试位置名称转换..." << std::endl;
    
    assert(Player::getPositionName(Position::TOP) == "上单");
    assert(Player::getPositionName(Position::JUN) == "打野");
    assert(Player::getPositionName(Position::MID) == "中单");
    assert(Player::getPositionName(Position::ADC) == "射手");
    assert(Player::getPositionName(Position::SUP) == "辅助");
    
    std::cout << "位置名称转换测试通过" << std::endl;
}

/**
 * @brief 测试玩家比较操作
 */
void testPlayerComparison() {
    std::cout << "测试玩家比较操作..." << std::endl;
    
    Player player1("Player1", Position::TOP, 1500);
    std::this_thread::sleep_for(std::chrono::milliseconds(50));
    Player player2("Player2", Position::TOP, 1600);
    
    // 测试等于操作
    Player player3("Player1", Position::MID, 2000); // 相同ID
    assert(player1 == player3);
    
    // 测试小于操作（基于等待时间）
    assert(player2 < player1); // player2等待时间更短
    
    std::cout << "玩家比较操作测试通过" << std::endl;
}

int main() {
    std::cout << "=== Player类单元测试 ===" << std::endl;
    
    try {
        testPlayerBasicFunctionality();
        testPositionNameConversion();
        testPlayerComparison();
        
        std::cout << "\n所有Player类测试通过！" << std::endl;
    } catch (const std::exception& e) {
        std::cerr << "测试失败: " << e.what() << std::endl;
        return 1;
    }
    
    return 0;
}