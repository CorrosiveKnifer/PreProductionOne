using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct Dialog
{
    public Dialog(string _dialog, Dictionary<string, System.Action> _options, float _speed = 15f)
    {
        SpeakImage = null;
        SpeakTitle = "";
        dialog = _dialog;
        textSpeed = 5f;
        options = _options;
    }
    public Dialog(Sprite _SpeakImage, string _SpeakTitle, string _dialog, float _speed = 15f)
    {
        SpeakImage = _SpeakImage;
        SpeakTitle = _SpeakTitle;
        dialog = _dialog;
        textSpeed = _speed;
        options = null;
    }
    public Dialog(Sprite _SpeakImage, string _SpeakTitle, string _dialog, Dictionary<string, System.Action> _options, float _speed = 15f)
    {
        SpeakImage = _SpeakImage;
        SpeakTitle = _SpeakTitle;
        dialog = _dialog;
        textSpeed = _speed;
        options = _options;
    }

    public Sprite SpeakImage;
    public string SpeakTitle;
    public string dialog;
    public float textSpeed; //Character per second
    public Dictionary<string, System.Action> options;
}

public class UI_DialogSystem : UI_Element
{
    [Header("Dependencies")]
    public bool m_isShowingDependencies = true;
    [ShowIf("m_isShowingDependencies")]
    public Image m_speakerImage;
    [ShowIf("m_isShowingDependencies")]
    public Text m_speakerTitle;
    [ShowIf("m_isShowingDependencies")]
    public Text m_dialogText;
    [ShowIf("m_isShowingDependencies")]
    public Transform m_optionsLocation;
    [ShowIf("m_isShowingDependencies")]
    public GameObject m_optionsPrefab;

    private Dialog m_currentDialog;
    private List<UI_DialogOption> m_optionButtons = new List<UI_DialogOption>();
    private Coroutine m_currentRoutine = null;

    // Update is called once per frame
    void Update()
    {

        if(InputManager.instance.IsKeyDown(KeyType.ESC))
        {
            gameObject.SetActive(false);
        }

        if((InputManager.instance.IsAnyKeyDown() || InputManager.instance.IsAnyMouseButtonDown()) && m_currentRoutine != null)
        {
            StopCoroutine(m_currentRoutine);
            m_dialogText.text = m_currentDialog.dialog;
            SetOptionActive(true);
            m_currentRoutine = null;
        }
        else if ((InputManager.instance.IsAnyKeyDown() || InputManager.instance.IsAnyMouseButtonDown()) && m_dialogText.text == m_currentDialog.dialog && m_optionButtons.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void LoadDialog(Dialog dialog)
    {
        ClearDialog();
        m_currentDialog = dialog;

        //Image
        if (m_currentDialog.SpeakImage == null)
            m_speakerImage.gameObject.SetActive(false);
        else
            m_speakerImage.sprite = m_currentDialog.SpeakImage;

        //Title
        if (m_currentDialog.SpeakTitle == "")
            m_speakerTitle.gameObject.SetActive(false);
        else
            m_speakerTitle.text = m_currentDialog.SpeakTitle;

        if (m_currentDialog.options != null)
            foreach (var item in m_currentDialog.options)
            {
                UI_DialogOption option = Instantiate(m_optionsPrefab, m_optionsLocation).GetComponent<UI_DialogOption>();
                option.m_optionText = item.Key;
                m_optionButtons.Add(option);
            }

        m_currentRoutine = StartCoroutine(DisplayText(m_currentDialog.dialog, m_currentDialog.textSpeed));
        SetOptionActive(false);
    }

    private IEnumerator DisplayText(string text, float charPerSec)
    {
        m_dialogText.text = "";
        float time = 0.0f;
        
        while (m_dialogText.text != text)
        {
            if (Mathf.FloorToInt(charPerSec * time) < text.Length)
            {
                m_dialogText.text = text.Substring(0, Mathf.FloorToInt(charPerSec * time));
                GetComponent<MultiAudioAgent>().PlayOnce("DialogCharacter", false, Random.Range(0.85f, 1.15f));
            }
            else
            {
                m_dialogText.text = text;
            }
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        SetOptionActive(true);
        yield return null;
    }

    public void SetOptionActive(bool isActive)
    {
        foreach(var item in m_optionButtons)
        {
            item.gameObject.SetActive(isActive);
        }
    }

    public void OnOptionClickEvent(string optionKey)
    {
        System.Action result;
        if(m_currentDialog.options.TryGetValue(optionKey, out result))
        {
            result.Invoke();
        }
    }

    public void ClearDialog()
    {
        //Unload the old one
        m_speakerImage.gameObject.SetActive(true);
        m_speakerImage.sprite = null;
        m_speakerTitle.gameObject.SetActive(true);
        m_speakerTitle.text = "";

        foreach (var item in m_optionButtons)
        {
            Destroy(item.gameObject);
        }
        m_optionButtons.Clear();
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        //Do Nothing
    }

    public override void OnMouseUpEvent()
    {
        //Do Nothing
    }
    public void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().m_functionalityEnabled = false;
    }
    public void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().m_functionalityEnabled = true;
    }
}
