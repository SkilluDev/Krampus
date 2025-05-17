using System.Linq;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine;

[ExecuteAlways]
public class ProcessInputs : MonoBehaviour, ITextPreprocessor {
    private TMP_Text m_text;

    private void Awake() {
        m_text = GetComponent<TMP_Text>();
        m_text.textPreprocessor = this;
    }

    private void Start() {
        m_text.textPreprocessor = this;
        if (!Application.isPlaying) return;
        InputSubscribe.onChanged.AddListener(OnChanged);
    }

    private void OnChanged() {
        m_text.SetText(m_text.text);
    }

    private bool Check(string condition) {
        if (string.IsNullOrWhiteSpace(condition)) return false;
        string method = InputSubscribe.InputMethod.ToString();
        string[] conditions = condition.Split(' ');
        foreach (string c in conditions) {
            if (c.StartsWith('!')) {
                if (c.Contains(method)) return false;
            } else {
                if (!c.Contains(method)) return false;
            }
        }
        return true;
    }


    public string PreprocessText(string text) {
        var ifRegex = new Regex(@"<if (.*?)>(.*?)<\/if>");
        var ifMatches = ifRegex.Matches(text);

        foreach (Match ifMatch in ifMatches) {
            if (ifMatch.Groups.Count > 2) {
                string condition = ifMatch.Groups[1].Value;
                string content = ifMatch.Groups[2].Value;

                if (Check(condition)) {
                    text = text.Replace(ifMatch.Value, content);
                } else {
                    text = text.Replace(ifMatch.Value, string.Empty);
                }
            }
        }

        var bindingRegex = new Regex(@"<binding=""(.*?)"">");
        var bindingMatches = bindingRegex.Matches(text);

        foreach (Match match in bindingMatches) {
            if (match.Groups.Count > 1) {
                string actionName = match.Groups[1].Value;
                var action = InputSubscribe.Raw.asset.FindAction(actionName);
                if (action != null) {
                    var binding = action.bindings.FirstOrDefault(w => w.groups != null && w.groups.Contains(InputSubscribe.InputMethod.ToString()));
                    string replacement = "[Invalid]";
                    if (binding != null) {
                        replacement = $"<sprite name=\"{binding.path.Replace("<", "").Replace(">", "")}\">";
                    }
                    text = text.Replace(match.Value, replacement);
                } else {
                    text = text.Replace(match.Value, $"[{actionName}]");
                }
            }
        }

        var bindingsRegex = new Regex(@"<bindings=""(.*?)"">");
        var bindingsMatches = bindingsRegex.Matches(text);

        foreach (Match match in bindingsMatches) {
            if (match.Groups.Count > 1) {
                string actionName = match.Groups[1].Value;
                var action = InputSubscribe.Raw.asset.FindAction(actionName);
                if (action != null) {
                    var bindings = action.bindings.Where(w => w.groups != null && w.groups.Contains(InputSubscribe.InputMethod.ToString()));
                    string sprites = "";
                    foreach (var binding in bindings) {
                        string name = binding.path.Replace("<", "").Replace(">", "");
                        if (TMP_Settings.defaultSpriteAsset.GetSpriteIndexFromName(name) >= 0) {
                            sprites += $"<sprite name=\"{name}\">";
                        }
                    }
                    text = text.Replace(match.Value, sprites);
                } else {
                    text = text.Replace(match.Value, $"[{actionName}]");
                }
            }
        }
        return text;
    }

}
