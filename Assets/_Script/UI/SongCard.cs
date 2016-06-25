namespace TentacleGuitarUnity {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SongCard : MonoBehaviour {


	public string ID {
		get {
			return id;
		}
		set {
			id = value;
		}
	}

	public string Title {
		get {
			return title;
		}
		set {
			title = value;
			titleText.text = value.ToString();
		}
	}

	public int Level {
		get {
			return level;
		}
		set {
			level = value;
			levelText.text = "Lv "+value.ToString("00");
			levelText.color = levelColor[Mathf.Clamp(value - 1, 0, levelColor.Length)];
		}
	}

	public bool IsDownloaded {
		set {
			downloadIcon.gameObject.SetActive(!value);
			Progress = 0f;
		}
		get{
			return !downloadIcon.gameObject.activeSelf;
		}
	}

	public bool IsDownloading {
		set {
			isDownloading = value;
		}
		get {
			return isDownloading;
		}
	}

	public float Progress {
		get {
			return progressBar.fillAmount;
		}
		set {
			progressBar.fillAmount = Mathf.Clamp01(value);
		}
	}

	public float Offset = 0f;


	private int level;
	private string title;
	private string id;
	private bool isDownloading;
	[SerializeField]
	private Text titleText;
	[SerializeField]
	private Text levelText;
	[SerializeField]
	private Transform downloadIcon;
	[SerializeField]
	private Image progressBar;
	[SerializeField]
	private Color[] levelColor;


	public void Init (string i, string t, int l, bool d, float o) {
		ID = i;
		Title = t;
		Level = l;
		IsDownloaded = d;
		Progress = 0f;
		Offset = o;
	}


	public void TryPlaySong () {
		Stage.TryStartSong(ID);
	}


	void OnDisable () {
		titleText.text = "";
		levelText.text = "";
		downloadIcon.gameObject.SetActive(false);
	}

}
}