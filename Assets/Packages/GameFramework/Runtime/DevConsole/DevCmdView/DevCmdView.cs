using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevCmdView : MonoBehaviour
    {
        [SerializeField]
        private Text cmdOutput;
        [SerializeField]
        private DevCmdInput cmdInput;
        private bool initialized;

        private string cmdList;
        private LinkedListNode<string> currentNode;
        private CachedLinkedList<string> inputList = new CachedLinkedList<string>();
        private Dictionary<string, string> cmdInfos = new Dictionary<string, string>();
        private Dictionary<string, MethodInfo> cmdMethods = new Dictionary<string, MethodInfo>();
        

        private void Start()
        {
            Initialization();
            cmdInput.Ipt.onSubmit.AddListener(OnCmdInputSubmit);
            cmdInput.OnMove.AddListener(OnCmdInputMove);
        }

        private void Initialization()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            try
            {
                MultiDictionary<string, string> cmdDict = new MultiDictionary<string, string>();
                List<string> cmdList = new List<string>();
                foreach (string assembly in GameSetting.Instance.AssemblyNames)
                {
                    if (!AssemblyUtility.ExistAssembly(assembly))
                    {
                        continue;
                    }

                    foreach (Type type in AssemblyUtility.GetTypes(Assembly.Load(assembly)))
                    {
                        MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                        foreach (MethodInfo methodInfo in methodInfos)
                        {
                            if (!methodInfo.IsDefined(typeof(DevCmdAttribute)))
                            {
                                continue;
                            }

                            DevCmdAttribute attr = methodInfo.GetCustomAttribute<DevCmdAttribute>(true);
                            string cmd = attr.Cmd.ToLower().Trim();
                            if (!cmdMethods.TryAdd(cmd, methodInfo))
                            {
                                Debug.LogError($"This command {cmd} is repeated.");
                                continue;
                            }

                            string info = "";
                            ParameterInfo[] paramInfos = methodInfo.GetParameters();
                            if (paramInfos.Length > 0)
                            {
                                info = "params:";
                                foreach (var paramInfo in paramInfos)
                                {
                                    info += " " + paramInfo.ParameterType.Name.ToLower();
                                }
                            }

                            if (!string.IsNullOrEmpty(attr.Info))
                            {
                                if (!string.IsNullOrEmpty(info))
                                {
                                    info += " | info: ";
                                }

                                info += attr.Info;
                            }

                            cmdInfos.Add(cmd, info);

                            int index = cmd.IndexOf(" ", StringComparison.Ordinal);
                            if (index > 0)
                            {
                                string last = cmd.Substring(0, index++);
                                string next = cmd.Substring(index, cmd.Length - index);

                                if (!next.StartsWith("-"))
                                {
                                    next = "-" + next;
                                    Debug.LogError($"This command {cmd} is lack of -.");
                                }

                                cmdDict.Add(last, next.TrimStart('-'));
                            }
                            else
                            {
                                cmdDict.Add(cmd, "");
                            }
                        }
                    }

                    string step = "";
                    foreach (KeyValuePair<string, LinkedListRange<string>> kvPair in cmdDict)
                    {
                        string options = "";
                        foreach (var option in kvPair.Value)
                        {
                            if (string.IsNullOrEmpty(option))
                            {
                                continue;
                            }

                            options += step + option;
                            step = " | ";
                        }

                        if (!string.IsNullOrEmpty(options))
                        {
                            cmdList.Add(kvPair.Key + " -[option] " + options);
                        }
                        else
                        {
                            cmdList.Add(kvPair.Key);
                        }
                    }

                    cmdList.Sort();
                    step = "";
                    for (int i = 0; i < cmdList.Count; i++)
                    {
                        this.cmdList += step + (i + 1) + "." + cmdList[i];
                        step = "\n";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnCmdInputSubmit(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text == "ls")
            {
                WriteLine(cmdList);
            }
            else
            {
                WriteLine(text);
                string[] cmdsText = text.Split("|");
                foreach (var cmdText in cmdsText)
                {
                    if (cmdText.StartsWith("i "))
                    {
                        LookupCommandInfo(cmdText);
                    }
                    else
                    {
                        ExecuteCommand(cmdText);
                    }
                }
            }

            inputList.AddLast(cmdInput.Ipt.text);
            currentNode = null;
            cmdInput.Ipt.text = "";
#if UNITY_EDITOR || UNITY_STANDALONE
            cmdInput.Ipt.ActivateInputField();
#endif
        }

        private void OnCmdInputMove(DevCmdInput.MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case DevCmdInput.MoveDirection.Up:
                    if (currentNode == null)
                    {
                        currentNode = inputList.Last;
                    }
                    else
                    {
                        currentNode = currentNode?.Previous ?? inputList.First;
                    }

                    cmdInput.Ipt.text = currentNode?.Value;
                    break;
                case DevCmdInput.MoveDirection.Down:
                    currentNode = currentNode?.Next ?? inputList.Last;
                    cmdInput.Ipt.text = currentNode?.Value;
                    break;
            }
        }

        private void LookupCommandInfo(string text)
        {
            string cmdText = text.Trim();
            string cmd = cmdText.Substring(2, cmdText.Length - 2);
            if (!cmdInfos.TryGetValue(cmd, out var info))
            {
                WriteLine($"Command {cmd} not found.");
                return;
            }

            WriteLine("command: " + cmd + " | " + info);
        }

        private void ExecuteCommand(string text)
        {
            try
            {
                string cmdText = text.Trim();
                string[] cmdWithArgsText = cmdText.Split(" ");
                string cmd = cmdWithArgsText[0];
                int cmdLength = 1;
                if (cmdWithArgsText.Length > 1)
                {
                    if (cmdWithArgsText[1].StartsWith("-"))
                    {
                        cmd += " " + cmdWithArgsText[1];
                        cmdLength++;
                    }
                }

                if (!cmdMethods.TryGetValue(cmd, out var method))
                {
                    WriteLine($"Command {cmd} not found.");
                    return;
                }

                ParameterInfo[] paramInfos = method.GetParameters();
                if (paramInfos.Length > 0)
                {
                    object[] args = new object[paramInfos.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (cmdWithArgsText.Length <= i + cmdLength)
                        {
                            break;
                        }

                        args[i] = JsonUtility.ConvertToObject(cmdWithArgsText[i + cmdLength], paramInfos[i].ParameterType);
                    }

                    method.Invoke(null, args);
                }
                else
                {
                    method.Invoke(null, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void WriteLine(string text)
        {
            if (string.IsNullOrEmpty(cmdOutput.text))
            {
                cmdOutput.text = text;
            }
            else
            {
                cmdOutput.text += "\n" + text;
            }
        }
    }
}
