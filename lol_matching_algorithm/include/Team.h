#ifndef TEAM_H
#define TEAM_H

#include "Player.h"
#include <vector>
#include <memory>
#include <map>

/**
 * @brief 团队类
 * 存储匹配成功的5人团队，提供团队统计信息
 */
class Team {
private:
    std::vector<std::shared_ptr<Player>> players;  // 团队成员
    std::chrono::steady_clock::time_point matchTime;  // 匹配成功时间
    static int teamIdCounter;  // 团队ID计数器
    int teamId;  // 团队唯一标识

public:
    /**
     * @brief 构造函数
     * @param teamPlayers 团队成员列表（必须包含5个不同位置的玩家）
     */
    Team(const std::vector<std::shared_ptr<Player>>& teamPlayers);

    // Getter方法
    int getTeamId() const { return teamId; }
    const std::vector<std::shared_ptr<Player>>& getPlayers() const { return players; }
    std::chrono::steady_clock::time_point getMatchTime() const { return matchTime; }

    /**
     * @brief 验证团队是否有效
     * 检查是否包含所有5个位置且每个位置只有一个玩家
     * @return 团队是否有效
     */
    bool isValidTeam() const;

    /**
     * @brief 计算团队平均分数
     * @return 团队所有成员的平均排位分数
     */
    double getAverageScore() const;

    /**
     * @brief 计算团队分数方差
     * 用于评估团队内部成员间的分数差距
     * @return 团队分数方差
     */
    double getScoreVariance() const;

    /**
     * @brief 计算团队平均等待时间
     * @return 团队所有成员的平均等待时间（秒）
     */
    double getAverageWaitingTime() const;

    /**
     * @brief 获取指定位置的玩家
     * @param pos 位置
     * @return 该位置的玩家指针，如果不存在则返回nullptr
     */
    std::shared_ptr<Player> getPlayerByPosition(Position pos) const;

    /**
     * @brief 打印团队信息
     * 输出团队的详细统计信息
     */
    void printTeamInfo() const;
};

#endif // TEAM_H