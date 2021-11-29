using System.Collections;
using System.Collections.Generic;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.UI;
using UnityEngine;

using UnityEngine.Windows.Speech;
using UnityEngine.Networking;


// using AIAction;
namespace Unity.LEGO.Behaviours
{
    public class SpeakAction : RepeatableAction
    {
        protected DictationRecognizer dictationRecognizer;
        public const int MaxCharactersPerSpeechBubble = 60;
        

        private string feedBack;

        [SerializeField]
        List<SpeechBubblePrompt.BubbleInfo> m_SpeechBubbleInfos = new List<SpeechBubblePrompt.BubbleInfo>
            { new SpeechBubblePrompt.BubbleInfo { Text = "Hello!", Type = SpeechBubblePrompt.Type.Talk } };

        [SerializeField]
        GameObject m_SpeechBubblePromptPrefab = default;

        SpeechBubblePrompt m_SpeechBubblePrompt;
        bool m_PromptActive = true;
        int m_Id;

        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Speak Action.png";
        }

        protected override void Start()
        {
            base.Start();

            if (IsPlacedOnBrick())
            {
                foreach (var speechBubleInfo in m_SpeechBubbleInfos)
                {
                    if (speechBubleInfo.Text.Length > MaxCharactersPerSpeechBubble)
                    {
                        speechBubleInfo.Text = speechBubleInfo.Text.Substring(0, MaxCharactersPerSpeechBubble);
                    }
                }
            }


            StartDictationEngine();
            feedBack = "";

            // // get move board
            GameObject gos;
            gos = GameObject.Find("char_move");
            // currentBrick = gos[0];
            Debug.Log("Brick is" + gos);
            // Debug.Log("Brick name is" + currentBrick.name);
            Debug.Log("Brick status is" + gos.GetComponent<AIAction>().status);
        }

        protected void Update()
        {
            if (m_Active)
            {
                if (m_SpeechBubbleInfos.Count > 0 && !m_SpeechBubblePrompt)
                {
                    SetupPrompt();
                }

                UpdatePrompt(IsVisible());
            }
        }

        void SetupPrompt()
        {
            PromptPlacementHandler promptHandler = null;

            foreach (var brick in m_ScopedBricks)
            {
                if (brick.GetComponent<PromptPlacementHandler>())
                {
                    promptHandler = brick.GetComponent<PromptPlacementHandler>();
                }

                var speakActions = brick.GetComponents<SpeakAction>();

                foreach (var speakAction in speakActions)
                {
                    if (speakAction.m_SpeechBubblePrompt)
                    {
                        m_SpeechBubblePrompt = speakAction.m_SpeechBubblePrompt;
                        break;
                    }
                }
            }

            var activeFromStart = IsVisible();

            // Create a new speech bubble prompt if none was found.
            if (!m_SpeechBubblePrompt)
            {
                if (!promptHandler)
                {
                    promptHandler = gameObject.AddComponent<PromptPlacementHandler>();
                }

                var go = Instantiate(m_SpeechBubblePromptPrefab, promptHandler.transform);
                m_SpeechBubblePrompt = go.GetComponent<SpeechBubblePrompt>();

                // Get the current scoped bounds - might be different than the initial scoped bounds.
                var scopedBounds = GetScopedBounds(m_ScopedBricks, out _, out _);
                promptHandler.AddInstance(go, scopedBounds, PromptPlacementHandler.PromptType.SpeechBubble, activeFromStart);
            }

            // Add this Speak Action to the speech bubble prompt.
            m_Id = m_SpeechBubblePrompt.AddSpeech(m_SpeechBubbleInfos, m_Pause, m_Repeat, SpeechFinished, activeFromStart, promptHandler);
        }

        void UpdatePrompt(bool active)
        {
            if (m_PromptActive != active)
            {
                m_PromptActive = active;

                if (active)
                {
                    m_SpeechBubblePrompt.Activate(m_Id);
                }
                else
                {
                    m_SpeechBubblePrompt.Deactivate(m_Id);
                }
            }
        }

        void SpeechFinished(int id)
        {
            if (m_Id == id)
            {
                UpdatePrompt(false);
                m_Active = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_SpeechBubblePrompt)
            {
                UpdatePrompt(false);
            }
        }

        //

    void OnApplicationQuit()
    {
        CloseDictationEngine();
    }

    private void DictationRecognizer_OnDictationHypothesis(string text)
    {
        Debug.Log("Dictation hypothesis: " + text);
    }
    private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause)
    {
        switch (completionCause)
        {
            case DictationCompletionCause.TimeoutExceeded:
            case DictationCompletionCause.PauseLimitExceeded:
            case DictationCompletionCause.Canceled:
            case DictationCompletionCause.Complete:
                // Restart required
                CloseDictationEngine();
                StartDictationEngine();
                break;
            case DictationCompletionCause.UnknownError:
            case DictationCompletionCause.AudioQualityFailure:
            case DictationCompletionCause.MicrophoneUnavailable:
            case DictationCompletionCause.NetworkFailure:
                // Error
                CloseDictationEngine();
                break;
        }
    }
    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictation result: " + text);

        // check command
        string command = text.Split(' ')[0];
        if (command == "stay") {
            GameObject.Find("char_move").GetComponent<AIAction>().status = 0;
            feedBack = "Copy";
        } else if (command == "come") {
            GameObject.Find("char_move").GetComponent<AIAction>().status = 1;
            feedBack = "Yes, sir";
        } else {
            StartCoroutine(CallChatbotServer(text));
        }

        // connect chatbot server
        // reply would be save in feedBack
        

        text = feedBack;




        // display feedback
        if (m_Active == true) {
            m_SpeechBubbleInfos.Add( new SpeechBubblePrompt.BubbleInfo { Text = text, Type = SpeechBubblePrompt.Type.Talk });
        } else {
            Debug.Log("before: " + m_SpeechBubbleInfos);
            m_SpeechBubbleInfos.Clear();
            m_SpeechBubbleInfos = new List<SpeechBubblePrompt.BubbleInfo>{new SpeechBubblePrompt.BubbleInfo { Text = text, Type = SpeechBubblePrompt.Type.Talk }};
            SetupPrompt();
            m_Active = true;
            Debug.Log("after: " + m_SpeechBubbleInfos);
        }
        
        
    }
    private void DictationRecognizer_OnDictationError(string error, int hresult)
    {
        Debug.Log("Dictation error: " + error);
    }

    private void CloseDictationEngine()
    {
        if (dictationRecognizer != null)
        {
            // dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            // dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }
            dictationRecognizer.Dispose();
        }
    }

    private void StartDictationEngine()
    {
        dictationRecognizer = new DictationRecognizer();
        // // dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;

        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        // dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
        dictationRecognizer.Start();
    }

    IEnumerator CallChatbotServer(string input)
    {
        string chatbot_server_url = "http://127.0.0.1:8000/chat/c/api/";
        string target_url = chatbot_server_url + $"?input={input}";
        Debug.Log($"asking: {target_url}...");
        UnityWebRequest www = UnityWebRequest.Get(target_url);
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            feedBack = "Cannot connect to Chatser...";
        }
        else {
            // Show results as text
            feedBack = www.downloadHandler.text;
            Debug.Log(feedBack);
        }
    }


    }
}
