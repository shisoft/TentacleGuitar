using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HoldOn : MonoBehaviour {

	public static HoldOn Main;
	public static bool IsOn = false;

	[SerializeField]
	private Image ProgressBarIMG;
	[SerializeField]
	private Transform Pannel;



	void Awake () {
		Main = this;
		Hide();
	}



	public static void Show () {
		if (!Main) {
			return;
		}
		IsOn = true;
		Main.Pannel.gameObject.SetActive(true);
	}


	public static void Hide () {
		if (!Main) {
			return;
		}
		IsOn = false;
		SetProgress(0f);
		Main.Pannel.gameObject.SetActive(false);
	}


	public static void SetProgress (float p) {
		if (!Main || !IsOn) {
			return;
		}
		Main.ProgressBarIMG.fillAmount = Mathf.Clamp01(p);
	}



}
