#ifndef PLAYER_H
#define PLAYER_H

#include <string>
#include <chrono>

/**
 * @brief 玩家位置枚举
 * 定义《英雄联盟》中的五个核心位置
 */
enum class Position {
    TOP,    // 上单
    JUN,    // 打野
    MID,    // 中单
    ADC,    // 射手
    SUP     // 辅助
};

/**
 * @brief 玩家类
 * 包含玩家的基本信息：ID、位置、排位分数、请求匹配时间等属性
 */
class Player {
private:
    std::string playerId;           // 玩家唯一标识
    Position position;              // 玩家位置
    int rankingScore;              // 排位分数
    std::chrono::steady_clock::time_point requestTime;  // 请求匹配时间
    bool isMatched;                // 是否已匹配

public:
    /**
     * @brief 构造函数
     * @param id 玩家ID
     * @param pos 玩家位置
     * @param score 排位分数
     */
    Player(const std::string& id, Position pos, int score);

    // Getter方法
    const std::string& getPlayerId() const { return playerId; }
    Position getPosition() const { return position; }
    int getRankingScore() const { return rankingScore; }
    std::chrono::steady_clock::time_point getRequestTime() const { return requestTime; }
    bool getIsMatched() const { return isMatched; }

    // Setter方法
    void setIsMatched(bool matched) { isMatched = matched; }

    /**
     * @brief 计算等待时间（秒）
     * @return 从请求匹配到现在的等待时间
     */
    double getWaitingTime() const;

    /**
     * @brief 获取位置名称字符串
     * @param pos 位置枚举值
     * @return 对应的中文位置名称
     */
    static std::string getPositionName(Position pos);

    /**
     * @brief 重载小于运算符，用于优先队列排序
     * 优先级：等待时间越长，优先级越高
     */
    bool operator<(const Player& other) const;

    /**
     * @brief 重载等于运算符
     */
    bool operator==(const Player& other) const;
};

#endif // PLAYER_H