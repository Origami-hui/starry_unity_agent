# Starry Agent: A Unity AI Dialogue System 🤖💬⭐

![Unity Version](https://img.shields.io/badge/Unity-2022.1.22f1c1%2B-blue)
![License](https://img.shields.io/badge/License-MIT-green)

一个基于Unity的智能对话系统，集成自然语言处理与语音合成技术，为游戏和交互应用提供沉浸式AI对话体验。

## 🌟 核心功能

### 1. 智能文本生成
- 接入大型语言模型实现上下文感知对话
- 支持自定义角色人格设定
- 指定文本生成模型

### 2. 情绪识别分析（可选功能）
- 实现文本情绪三分类（积极/消极/中性）
- 根据情绪实现不同表情动作

### 3. 语音合成输出（可选功能）
- 可连接本地GPT-SoVITS模型输出语音

### 4. 对话历史管理
- 自动记录完整对话时间线
- 历史对话**语音回放**
- 重新进入游戏**可保存历史对话信息**，也可清除对话信息

### 5. 其他功能
- 触摸系统
- 闲时动作

## 🛠️ 快速开始

### 环境要求
- Unity 2022.1+


### 安装步骤
1. 克隆仓库到Unity项目：
```bash
git clone https://github.com/Origami-hui/starry_unity_agent.git
```
2. 申请API密钥：
    - 对话大模型：[Open Router](https://openrouter.ai/)/[Silicon Flow](https://cloud.siliconflow.cn/)
    - 情绪识别（可选）：[Baidu Cloud](https://console.bce.baidu.com/ai-engine/nlp/resource?_=1747500787133&apiId=32)
3. 下载本地配置语音生成工程与模型（可选）：[GPT-SoVITS](https://)（下载好直接运行主目录的 `start_tts_api.bat` 文件即可）
4. 在Unity中打开项目
5. 在场景中的 `GlobalConifg` 物体脚本处配置API密钥及其他信息
6. 启动游戏！

## ⚠️ 注意事项

1. 本地tts可能生成速度较慢，如需更改API调用请到 `Assets/Script/Agent API/TTSAPI.cs` 处更改相关逻辑

## 最后

作为开发狗也想趁着学生时代的尾声蹭一蹭AI的热潮，为自己的OC赋予一个不太完美的灵魂，如果能帮助你或者能为你的项目做一些参考就再好不过了。

~~马上就要当社畜了，这种事情补药啊！~~
