#include "../include/MatchingEngine.h"
#include <iostream>

/**
 * @brief 简单使用示例
 * 展示如何使用LOL匹配算法的基本功能
 */
int main() {
    std::cout << "=== LOL匹配算法简单使用示例 ===" << std::endl;
    
    // 1. 创建匹配引擎
    // 参数：基础分数范围(200), 时间权重(1.0), 最大等待时间(300秒)
    MatchingEngine engine(200, 1.0, 300.0);
    
    // 2. 创建玩家并加入匹配队列
    std::cout << "\n添加玩家到匹配队列..." << std::endl;
    
    // 创建第一组玩家（能组成完整团队）
    auto player1 = std::make_shared<Player>("上单小王", Position::TOP, 1500);
    auto player2 = std::make_shared<Player>("打野小李", Position::JUN, 1520);
    auto player3 = std::make_shared<Player>("中单小张", Position::MID, 1480);
    auto player4 = std::make_shared<Player>("射手小赵", Position::ADC, 1510);
    auto player5 = std::make_shared<Player>("辅助小孙", Position::SUP, 1490);
    
    // 加入匹配队列
    engine.addPlayer(player1);
    engine.addPlayer(player2);
    engine.addPlayer(player3);
    engine.addPlayer(player4);
    engine.addPlayer(player5);
    
    // 3. 执行匹配
    std::cout << "\n开始匹配..." << std::endl;
    int matchedTeams = engine.performMatching();
    
    if (matchedTeams > 0) {
        std::cout << "匹配成功！组成了 " << matchedTeams << " 个团队。" << std::endl;
        
        // 4. 查看匹配结果
        const auto& teams = engine.getMatchedTeams();
        for (const auto& team : teams) {
            team->printTeamInfo();
        }
    } else {
        std::cout << "暂时无法组成完整团队，请等待更多玩家加入。" << std::endl;
    }
    
    // 5. 查看统计信息
    std::cout << "\n=== 匹配统计信息 ===" << std::endl;
    engine.printMatchingStats();
    
    return 0;
}