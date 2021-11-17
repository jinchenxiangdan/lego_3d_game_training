using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.Networking;

public class ChatBot : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }
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

    void Start()
    {
        m_DictationRecognizer = new DictationRecognizer();
        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            // m_Recognitions.text += text + "\n";
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            // Debug.LogFormat("Dictation hypothesis: {0}", text);
            // m_Hypotheses.text += text;
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };
 
        m_DictationRecognizer.Start();

        StartCoroutine(CallChatbotServer("help e"));
    }

    void Update()
    {
        
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
}
