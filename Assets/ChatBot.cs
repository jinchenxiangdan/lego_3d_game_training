using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.Networking;

public class ChatBot : MonoBehaviour
{
    // public GameObject fruit;
    public GameObject talk_board;  // self object

    protected DictationRecognizer dictationRecognizer;

    [SerializeField]
    private Text m_Hypotheses;

    [SerializeField]
    private Text m_Recognitions;

    // Speech Keyword recognizer 
    // private KeywordRecognizer keywordRecognizer;

    // speech voice to text Dictation recognizer is currently functional only on Windows 10, and 
    // requires that dictation is permitted in the user's Speech privacy policy (Settings->Privacy->Speech, 
    // inking & typing). If dictation is not enabled, DictationRecognizer will fail on Start. Developers can 
    // handle this failure in an app-specific way by providing a DictationError delegate and testing 
    // for SPERR_SPEECH_PRIVACY_POLICY_NOT_ACCEPTED (0x80045509).
    private DictationRecognizer m_DictationRecognizer;

    public GameObject chat;
    void Start()
    {
        // m_DictationRecognizer = new DictationRecognizer();
        // m_DictationRecognizer.DictationResult += (text, confidence) =>
        // {
        //     Debug.LogFormat("Dictation result: {0}", text);
        //     // m_Recognitions.text += text + "\n";
        // };

        // m_DictationRecognizer.DictationHypothesis += (text) =>
        // {
        //     // Debug.LogFormat("Dictation hypothesis: {0}", text);
        //     // m_Hypotheses.text += text;
        // };

        // m_DictationRecognizer.DictationComplete += (completionCause) =>
        // {
        //     if (completionCause != DictationCompletionCause.Complete)
        //         Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        // };

        // m_DictationRecognizer.DictationError += (error, hresult) =>
        // {
        //     Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        // };
 
        // m_DictationRecognizer.Start();

        // StartCoroutine(CallChatbotServer("help e"));

        StartDictationEngine();
        
        // Debug.Log(chat.GetComponent<SpeakAction>().m_SpeechBubbleInfos);
    }

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
    }
    private void DictationRecognizer_OnDictationError(string error, int hresult)
    {
        Debug.Log("Dictation error: " + error);
    }

    private void CloseDictationEngine()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;
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
        dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
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
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
 
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    // private void PutFruit() 
    // {
    //     GameObject instance_object = Instantiate(fruit);
    //     // instance_object.name = "Fruit";
    //     Vector3 offset = new Vector3(0, 10, 2);
    //     // instance_object.name = "Fruit";
    //     instance_object.transform.position = player.transform.position + offset;
    // }

    void UpdateDiaglog()
    {

    }
}
