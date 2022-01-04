using UnityEngine;
using UnityEngine.UI; // We need to edit text components
using UnityEngine.EventSystems; // And determine if our mouse is hovering
using UnityEngine.SceneManagement; // And have access to switching scenes.
using System.Collections;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	/// <summary>
	/// To contact me for any reason, please email me at jadewirestudios@gmail.com. 
	/// </summary>

	public bool modifyFontSize; // The bool value which tells our game wether or not we want to modify the size of the font when we highlight our button
	public int startingFontSize; // This is the font size the text for the button starts out at
	public int highlightedFontSize; // This is the font size for our text whenever we highlight the button

	public bool playHoverSound; // Tells our game wether or not we play a sound when we hover over our button
	public AudioClip hoverSound; // The audio clip that we play when we hover

	public bool playClickSound; // Tells our game wether or not we play a sound when we click on the button
	public AudioClip clickSound; // The audio clip that we play when we click

	public bool modifyFontColor; // The bool value which tells our game wether or not we want to modify the color of the text whenever we highlight our button
	public Color regularColor; // The starting color of the text attatched to the button
	public Color highlightedColor; // The highlighted color of the text attatched to the button

	MenuController menuControl; // A reference to the MenuController, which tells the game which canvas we want to load at any given point in time

	Text textForButton; // This is the variable that holds the component for the text that we want to modify

	List<Vector2> ScreenTypes = new List<Vector2> { new Vector2(640, 480), new Vector2(1280, 720), new Vector2(1920, 1080) };
	static int ScreenTypeIndex;
	static bool FullScreen;
	static bool soundOn;
	public bool isMusicButton = false;
	public bool isSoundButton = false;
	public bool isResButton = false;
	public bool isFullscreenButton = false;
	private static bool firstLoadGame = true;
	void Start() // Called once, on the first frame after the game starts playing
	{
		if (firstLoadGame)
        {
			FullScreen = true;
			soundOn = true;
			ScreenTypeIndex = 0;
			firstLoadGame = false;
        }
			
		textForButton = gameObject.GetComponentInChildren<Text> (); // Set our variable equal to a text component which is set as a child of our button

		if (modifyFontSize) { // If we are modifying our font size, we want to...
			textForButton.fontSize = startingFontSize; // We set the font size of that text equal to the font size we specified once we start the game
		}

		if (modifyFontColor) { // If we are modifying the color of our button... 
			textForButton.color = regularColor; // We set the color of the button at start equal to the color we specified in "regularColor"
		}

		menuControl = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MenuController> (); // We set our reference equal to a GameObject who has a tag of "GAmeManager", with a component of MenuController, our script.
		if (isResButton)
		{
			Screen.SetResolution((int)ScreenTypes[ScreenTypeIndex].x, (int)ScreenTypes[ScreenTypeIndex].y, FullScreen);
			textForButton.text = "Resolution:" + ((int)ScreenTypes[ScreenTypeIndex].x).ToString() + "x" + ((int)ScreenTypes[ScreenTypeIndex].y).ToString();
		}

		if (isFullscreenButton)
		{
			Screen.SetResolution((int)ScreenTypes[ScreenTypeIndex].x, (int)ScreenTypes[ScreenTypeIndex].y, FullScreen);
			textForButton.text = FullScreen ? "FullScreen:On" : "FullScreen:Off";
		}
		if (isSoundButton)
		{
			AudioListener.volume = soundOn ? 1.0f : 0f;
			textForButton.text = soundOn ? "Sound:On" : "Sound:Off";
		}
		if (isMusicButton)
		{
			var bgm = FindObjectsOfType<BGM>();
			if (bgm.Length != 0)
				textForButton.text = BGM.bgmOn ? "Music:On" : "Music:Off";
		}
	}

	public void OnPointerEnter(PointerEventData eventdata) // Whenever we mouse over the button:
	{
		if (playHoverSound) { // If we are playing a hover sound...
			AudioSource buttonAudioSource = GetComponent<AudioSource> (); // We create an audiosource variable, so that we can play sound.
			buttonAudioSource.clip = hoverSound; // We set the clip of this audio source equal to the one that we want to play when we hover
			buttonAudioSource.Play (); // We then play from that audiosource
		}

		if (modifyFontSize) { // If we are modifying the font size...
			textForButton.fontSize = highlightedFontSize; // We increase the font size to the size designated
		}

		if (modifyFontColor) { // If we are modifying the font color...
			textForButton.color = highlightedColor; // We set the font color of the button equal to that of the color of "highlightedColor"
		}
	}

	public void OnPointerExit(PointerEventData eventdata) // Whenever we mouse off of the button:
	{
		if (modifyFontSize) { // If we modified font size...
			textForButton.fontSize = startingFontSize; // We set the font size back to its starting point
		}

		if (modifyFontColor) { // If we modified color...
			textForButton.color = regularColor; // We set the color back to normal.
		}
	}

	public void OnPointerDown(PointerEventData eventdata) // Whenever we click on the button:
	{
		if (playClickSound) { // If we want to play a click sound....
			AudioSource buttonAudioSource = GetComponent<AudioSource> (); // We create an audioSource for that sound
			buttonAudioSource.clip = clickSound; // We set the clip of that audiosource equal to the one we want to play when we click
			buttonAudioSource.Play (); // We then play from that audio source
		}
	}

	public void LoadScene(string SceneToLoad) 
	{
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneToLoad); // We use the SceneManager to load a scene whose name matches that of the string that we passed in with the function
	}

	public void Quit()
	{
		Application.Quit (); // This line quits the application.
	}

	public void ChangeResolution()
    {
		ScreenTypeIndex = (ScreenTypeIndex + 1) % ScreenTypes.Count;
		Screen.SetResolution((int)ScreenTypes[ScreenTypeIndex].x, (int)ScreenTypes[ScreenTypeIndex].y, FullScreen);
		textForButton.text = "Resolution:" + ((int)ScreenTypes[ScreenTypeIndex].x).ToString() + "x" +((int)ScreenTypes[ScreenTypeIndex].y).ToString();
	}

	public void ChangeSound()
	{
		soundOn = !soundOn;
		AudioListener.volume = soundOn? 1.0f:0f;
		textForButton.text = soundOn ? "Sound:On" : "Sound:Off";
	}
	public void ChangeMusic()
	{
		var bgm = FindObjectsOfType<BGM>();
		if (bgm.Length != 0)
			textForButton.text = bgm[0].toggleBGM() ? "Music:On" : "Music:Off";
	}

	public void ChangeFullScreen()
	{
		FullScreen = !FullScreen;
		Screen.SetResolution((int)ScreenTypes[ScreenTypeIndex].x, (int)ScreenTypes[ScreenTypeIndex].y, FullScreen);
		textForButton.text =  FullScreen? "FullScreen:On":"FullScreen:Off";
	}

	public void LoadWebsite(string URLToOpen) // Maybe you want your users to be able to contact you?
	{
		Application.OpenURL (URLToOpen); // This opens up a website specified through the string passed in with the function. 
	}

	public void LoadCanvas(string canvasIndex) 
	{
		menuControl.canvasIndex = canvasIndex; // We change the canvasIndex to a specified string value that is passed in with the function
	}
}
