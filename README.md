# 503 杀戮尖塔2模组

本项目基于教程https://tutorials.sts2modding.com/ 开发503杀戮尖塔2游戏模组

## 如何添加卡牌
1. 创建卡牌文件 参考 \503-sts2mod\Scripts\cards\TestCard.cs

    修改卡牌图片路径：
    ```
    PortraitPath: $"res://Demo/images/cards/zwk.png"
    ```
2. 添加卡牌图片资源：Demo\images\cards\zwk.png
3. 添加卡牌标题和描述文本 Demo\localization\zhs\cards.json
    key值为{modid}_{类别}_{原id}。例如这里的modid/C#第一命名空间是Demo, 类别是CARD。原始卡牌id为TEST_CARD，是TestCard C#类名的大写snake-case。
    ```
    "DEMO_CARD_TEST_CARD.title": "笑里藏刀",
    "DEMO_CARD_TEST_CARD.description": "造成{Damage:diff()}点伤害。"
    ```


# TODO
- [x] 根据点数抽牌 
- [x] 根据点数获得能量
- [x] 走运形态 每回合前一次摇骰子基础点数必为6
- [x] AOE
- [x] 多段 骰子数为打出次数
- [x] 催眠 虚弱易伤 点数6时翻倍
- [x] 压制 让敌人失去点数力量
- [x] 能力牌 奖池 获得一点奖池 之后骰子摇到6及以上时叠加一层奖池
- [ ] 爆运 攻击牌 0费 1-10 打击奖池数 11-20 打击2* 奖池数 21+ 打击3*奖池数 奖池数回到1
- [ ] 加自己力量，敏捷 6及以上判定