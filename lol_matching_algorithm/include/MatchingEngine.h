#ifndef MATCHING_ENGINE_H
#define MATCHING_ENGINE_H

#include "Player.h"
#include "Team.h"
#include <queue>
#include <vector>
#include <memory>
#include <map>
#include <functional>

/**
 * @brief 匹配引擎类
 * 实现基于时间权重和分数范围的玩家匹配算法
 */
class MatchingEngine {
private:
    // 比较器类型定义
    using PlayerComparator = std::function<bool(const std::shared_ptr<Player>&, const std::shared_ptr<Player>&)>;
    using PlayerQueue = std::priority_queue<std::shared_ptr<Player>, std::vector<std::shared_ptr<Player>>, PlayerComparator>;
    
    // 每个位置的等待队列（优先队列，等待时间长的优先）
    std::map<Position, PlayerQueue> waitingQueues;
    
    std::vector<std::unique_ptr<Team>> matchedTeams;  // 已匹配的团队
    
    // 匹配算法参数
    int baseScoreRange;        // 基础分数匹配范围
    double timeWeight;         // 时间权重因子
    double maxWaitTime;        // 最大等待时间（秒），超过后放宽分数要求
    
    // 统计信息
    int totalPlayersAdded;     // 总加入玩家数
    int totalTeamsMatched;     // 总匹配成功团队数
    double totalMatchingTime;  // 总匹配耗时

public:
    /**
     * @brief 构造函数
     * @param scoreRange 基础分数匹配范围（默认200分）
     * @param timeWt 时间权重因子（默认1.0）
     * @param maxWait 最大等待时间（默认300秒）
     */
    MatchingEngine(int scoreRange = 200, double timeWt = 1.0, double maxWait = 300.0);

    /**
     * @brief 添加玩家到匹配队列
     * @param player 要添加的玩家
     * @return 是否成功添加
     */
    bool addPlayer(std::shared_ptr<Player> player);

    /**
     * @brief 执行匹配算法
     * 尝试为所有等待的玩家寻找匹配
     * @return 本次匹配成功的团队数量
     */
    int performMatching();

    /**
     * @brief 计算动态分数范围
     * 根据等待时间动态调整匹配的分数范围
     * @param waitingTime 等待时间（秒）
     * @return 调整后的分数范围
     */
    int getDynamicScoreRange(double waitingTime) const;

    /**
     * @brief 检查一组玩家是否可以组成有效团队
     * @param players 候选玩家列表
     * @return 是否可以组成有效团队
     */
    bool canFormTeam(const std::vector<std::shared_ptr<Player>>& players) const;

    /**
     * @brief 计算团队匹配分数
     * 综合考虑分数差距和等待时间的匹配质量评分
     * @param players 候选玩家列表
     * @return 匹配分数（越高越好）
     */
    double calculateTeamMatchScore(const std::vector<std::shared_ptr<Player>>& players) const;

    /**
     * @brief 贪心算法寻找最优组合
     * 从当前等待队列中寻找最优的5人组合
     * @return 找到的最优团队玩家列表，如果没找到返回空列表
     */
    std::vector<std::shared_ptr<Player>> findOptimalTeam();

    /**
     * @brief 移除已匹配的玩家
     * 从等待队列中移除指定的玩家
     * @param players 要移除的玩家列表
     */
    void removeMatchedPlayers(const std::vector<std::shared_ptr<Player>>& players);

    // Getter方法
    const std::vector<std::unique_ptr<Team>>& getMatchedTeams() const { return matchedTeams; }
    int getTotalPlayersAdded() const { return totalPlayersAdded; }
    int getTotalTeamsMatched() const { return totalTeamsMatched; }
    double getTotalMatchingTime() const { return totalMatchingTime; }

    /**
     * @brief 获取当前等待队列状态
     * @return 每个位置当前等待的玩家数量
     */
    std::map<Position, int> getQueueStatus() const;

    /**
     * @brief 打印匹配统计信息
     */
    void printMatchingStats() const;

    /**
     * @brief 清空所有队列和统计信息
     */
    void reset();
};

#endif // MATCHING_ENGINE_H