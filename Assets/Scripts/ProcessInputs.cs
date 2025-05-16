using System.Linq;
using TMPro;
using UnityEngine;

public class ProcessInputs : MonoBehaviour, ITextPreprocessor {
    private TMP_Text m_text;

    private void Awake() {
        m_text = GetComponent<TMP_Text>();
    }

    private void Start() {
        m_text.textPreprocessor = this;
        InputSubscribe.onChanged.AddListener(OnChanged);
    }

    private void OnChanged() {
        m_text.SetText(m_text.text);
    }

    public string PreprocessText(string text) {
        var regex = new System.Text.RegularExpressions.Regex(@"<binding=""(.*?)"">");
        var matches = regex.Matches(text);

        foreach (System.Text.RegularExpressions.Match match in matches) {
            if (match.Groups.Count > 1) {
                string actionName = match.Groups[1].Value;
                var action = InputSubscribe.Raw.asset.FindAction(actionName);
                if (action != null) {
                    string bindingPath = action.bindings.First(w => w.groups.Contains(InputSubscribe.InputMethod.ToString())).path;
                    bindingPath = bindingPath.Replace("<", "").Replace(">", "");
                    text = text.Replace(match.Value, $"<sprite name=\"{bindingPath}\">");
                } else {
                    text = text.Replace(match.Value, $"[{actionName}]");
                }
            }
        }
        return text;
        //j
        //k
    }

}
