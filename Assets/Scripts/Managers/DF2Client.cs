using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Syrus.Plugins.DFV2Client;
using UnityEngine.UI;

public class DF2Client : MonoSingleton<DF2Client>
{
    internal DialogFlowV2Client Client;

    public InputField contentInputField;

    internal string SessionName;

    void Awake()
    {
        SessionName = Utils.GetMacAddress();
    }

    void Start()
    {
        Client = GetComponent<DialogFlowV2Client>();

        Client.ChatbotResponded += LogResponseText;
        Client.DetectIntentError += LogError;
        Client.ReactToContext("DefaultWelcomeIntent-followup", context => Debug.Log("Reacting to welcome followup"));
        Client.SessionCleared += sess => Debug.Log("Cleared session [" + SessionName + "]");
        Client.ClearSession(SessionName);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            SendText();
        }
    }

    private QAPanel _QAPanel;

    internal QAPanel QAPanel
    {
        get
        {
            if (_QAPanel == null)
            {
                _QAPanel = UIManager.Instance.GetBaseUIForm<QAPanel>();
            }

            return _QAPanel;
        }
    }

    private void LogResponseText(DF2Response response)
    {
        string text = response.queryResult.fulfillmentText;
        string talkingSpeciesName = "";
        if (text.Contains("{TalkingSpecies_"))
        {
            foreach (KeyValuePair<string, GeoGroupInfo> kv in NatureController.Instance.AllGeoGroupInfo)
            {
                if (text.Contains("{TalkingSpecies_" + kv.Key + "}"))
                {
                    talkingSpeciesName = kv.Key;
                    text = text.Replace("{TalkingSpecies_" + kv.Key + "}", "");
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(talkingSpeciesName))
        {
            GeoGroupInfo species = NatureController.Instance.AllGeoGroupInfo[talkingSpeciesName];

            text = text.Replace("@life", Mathf.RoundToInt(species.Life).ToString());
            text = text.Replace("@speed", Mathf.RoundToInt(species.Speed).ToString());
            text = text.Replace("@damage", Mathf.RoundToInt(species.Damage).ToString());
            text = text.Replace("@vision", Mathf.RoundToInt(species.Vision).ToString());
            text = text.Replace("@fertilityRate", Mathf.RoundToInt(species.FertilityRate) + "%");
            text = text.Replace("@matureSizePercent", Mathf.RoundToInt(species.MatureSizePercent) + "%");

            text = text.Replace("@diets", GetDescFromList(species.Diets.ToList()));
            text = text.Replace("@predators", GetDescFromList(species.Predators.ToList()));
        }

        QAPanel.GenerateText(text, TextBubble.Alignment.Left);
    }

    public static string GetDescFromList(List<string> list)
    {
        string res_str = "";
        if (list.Count > 1)
        {
            string last = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            res_str = string.Join(",", list);
            res_str += " and " + last;
        }
        else if (list.Count == 0)
        {
            res_str = "nothing";
        }
        else
        {
            res_str = list[0];
        }

        return res_str;
    }

    private void LogError(DF2ErrorResponse errorResponse)
    {
        Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), errorResponse.error.message));
    }

    public void SendText()
    {
        if (string.IsNullOrWhiteSpace(contentInputField.text)) return;
        Client.DetectIntentFromText(contentInputField.text, SessionName);
        QAPanel.GenerateText(contentInputField.text, TextBubble.Alignment.Right);
        contentInputField.text = "";
        contentInputField.ActivateInputField();
    }

    public void SendEvent()
    {
        Client.DetectIntentFromEvent(contentInputField.text,
            new Dictionary<string, object>(), SessionName);
    }

    public void Clear()
    {
        Client.ClearSession(name);
    }
}