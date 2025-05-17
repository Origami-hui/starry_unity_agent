using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using static Unity.VisualScripting.Member;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class SoVITSAPI : MonoBehaviour
{
    [Header("音频播放")]
    public AudioSource audioSource;

    private Process pythonProcess; // 存储批处理进程实例

/*    void Start()
    {
        if (useTTSOnBeginning)
        {
            ExecuteBatFile();
        }
    }

    void OnApplicationQuit()
    {
        if (useTTSOnBeginning)
        {
            TerminateBatProcess();
        }
    }*/

    private void ExecuteBatFile()
    {
        
        try
        {
            string pythonExePath = GlobalConfig.instance.ttsMainFilePath + "\\runtime\\python.exe";
            string pythonScriptPath = GlobalConfig.instance.ttsMainFilePath + "\\api.py"; // 要运行的Python脚本绝对路径
            string scriptArguments = "-s \"SoVITS_weights_v2/kaituozhe_e10_s1190.pth\" -g \"GPT_weights_v2/kaituozhe-e10.ckpt\" -dr  \"examples/kaituozhe.wav\" -dt \"虽然嘴上说着不高兴，身体的反应还是很真实的。还是多去找找帕姆吧！\" -dl \"zh\" -a \"0.0.0.0\" -sm \"n\""; // 脚本参数（可选）
            // 检查文件是否存在
            if (!System.IO.File.Exists(pythonExePath) || !System.IO.File.Exists(pythonScriptPath))
            {
                Debug.LogError("Python解释器或脚本文件不存在, pythonExePath: " + pythonExePath);
                return;
            }

            // 配置进程参数
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonExePath, // 指定Python解释器路径
                Arguments = $"{pythonScriptPath} {" " + scriptArguments}", // 拼接脚本路径和参数
                UseShellExecute = false, // 不使用系统外壳（允许重定向输出）
                RedirectStandardOutput = true, // 重定向输出以便日志查看
                RedirectStandardError = true,
                CreateNoWindow = true, // 调试时显示命令行窗口（发布时可设为true）
                WorkingDirectory = Path.GetDirectoryName(pythonScriptPath) // 设置脚本所在目录为工作目录
            };

            // 启动进程并获取句柄
            pythonProcess = new Process { StartInfo = startInfo };
            pythonProcess.Start();

            // 监听输出（可选，用于调试）
            pythonProcess.OutputDataReceived += (sender, e) => Debug.Log($"Python输出: {e.Data}");
            pythonProcess.BeginOutputReadLine();

            Debug.Log("Python进程启动成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"启动失败: {e.Message}");
        }
    }

    private void TerminateBatProcess()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            try
            {
                // 强制终止进程及其所有子进程（关键）
                pythonProcess.Kill(); // true参数递归终止子进程
                pythonProcess.WaitForExit(2000); // 等待2秒确保终止
                Debug.Log("Python进程已终止");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"终止失败: {e.Message}");
            }
        }
    }

    public IEnumerator SendTTSRequest(string input)
    {
        if (!GlobalConfig.instance.textToSoundEnabled)
        {
            yield break;
        }
        string txt = WashTTSText(input);
        // 构造JSON请求体（根据服务端参数要求调整字段名）
        var requestBody = new
        {
            text = txt,          // 对应启动命令中的 -dt（文本内容）
            text_language = "zh"
        };

        // 将对象序列化为JSON字符串
        string json = JsonConvert.SerializeObject(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(GlobalConfig.instance.serviceUrl, "POST"))
        {
            // 设置请求头为JSON格式
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(request.url, AudioType.OGGVORBIS);  // 适配OGG格式
            request.timeout = 60;  // 延长超时时间（生成音频可能较慢）

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log("语音播放成功，ttsText: " + txt);
                }
                else
                {
                    Debug.LogError("无法解析OGG音频数据");
                }

                HistoryManager.AddDialogue(GlobalConfig.instance.roleName, txt, clip);
            }
            else
            {
                Debug.LogError($"无法连接音频服务");  // 关键排错信息
                HistoryManager.AddDialogue(GlobalConfig.instance.roleName, txt);
                yield return null;
            }
            StartCoroutine(TriggerCallbackWhenDone()); //重置idle状态
            request.Dispose();
        }
    }

    private IEnumerator TriggerCallbackWhenDone()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        EventCenter.EventTrigger("SetIdleState");
    }

    //给tts模块的文字清洗，洗去一些拟声词和描述等
    private string WashTTSText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // 正则匹配所有括号对（圆括号、方括号、花括号）及其内部内容
        // 模式说明：
        // \( [^)]* \)  匹配圆括号内的内容（非嵌套）
        // \[ [^\]]* \]  匹配方括号内的内容
        // \{ [^\}]* \}  匹配花括号内的内容
        string pattern = @"(（[^）]*）|\([^)]*\))";

        // 全局替换匹配到的括号及内部内容为空字符串
        return Regex.Replace(text, pattern, string.Empty);
    }

    // 可选：编辑器测试按钮
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Test Close Bat Window")]
    private static void TestClose()
    {
        SoVITSAPI manager = FindObjectOfType<SoVITSAPI>();
        Debug.Log(manager);
        manager?.TerminateBatProcess();
    }
#endif

}
