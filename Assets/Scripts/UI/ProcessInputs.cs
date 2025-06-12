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

		var currentLevelRegex = new Regex(@"<currentLevel>");
        var currentLevelMatches = currentLevelRegex.Matches(text);

        foreach (Match lMatch in currentLevelMatches) {
			text = text.Replace(lMatch.Value, ""+(Game.PogMan.CurrentLevel+1));
		}

		var levelsRegex = new Regex(@"<levels>");
        var levelsMatches = levelsRegex.Matches(text);

		foreach (Match lMatch in levelsMatches) {
			text = text.Replace(lMatch.Value, ""+Game.PogMan.LevelCount);
		}

		var timerRegex = new Regex(@"<totalRunTime>");
        var timerMatches = timerRegex.Matches(text);

		foreach (Match lMatch in timerMatches) {
			var totalTime = Game.PogMan.TotalRunTime;
			var minutes = (int) (totalTime / 60);
			var remainingSeconds = (int) (totalTime - minutes * 60);
			var formattedTime = minutes.ToString("00") + ':' + remainingSeconds.ToString("00");
			text = text.Replace(lMatch.Value, formattedTime);
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
