using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;

public class MainMenus : MonoBehaviour 
{
	public GameObject mouseController;
	private MouseInput mouseControllerRef;

	public GameObject mainEventSystem;

	//main menu
	public GameObject mainMenu;
	public GameObject charSelectMenu;
	public GameObject optionsMenu;
	public GameObject achievementsMenu;
	public GameObject creditsMenu;

	//part of control setups
	public GameObject numPlayersMenu;
	public GameObject usesMouseMenu;
	public GameObject eachPlayerMenu;
	private int playerIndex; //for traversing of players for each player menu
	private int maxPlayers = 1;

	//char select menu
	public GameObject vsMenu;
	//public GameObject controlsMenu;


	//vs menu
	public GameObject mapsMenu;
	public GameObject weaponsMenu;

	//controls menu
	//public GameObject player1Controls;
	//public GameObject player2Controls;
	//public GameObject player3Controls;
	//public GameObject player4Controls;


	void Start () 
	{

		/*mainMenu = GameObject.Find ("Main Menu");
		charSelectMenu = GameObject.Find ("Char Select Menu");
		optionsMenu = GameObject.Find ("Options Menu");
		achievementsMenu = GameObject.Find ("Achievements Menu");
		creditsMenu = GameObject.Find ("Credits Menu");
		vsMenu = GameObject.Find ("Vs Menu");
		controlsMenu = GameObject.Find ("Controls Menu");
		mapsMenu = GameObject.Find ("Maps Menu");
		weaponsMenu = GameObject.Find ("Weapons Menu");
		player1Controls = GameObject.Find ("Player1 Menu");
		player2Controls = GameObject.Find ("Player2 Menu");
		player3Controls = GameObject.Find ("Player3 Menu");
		player4Controls = GameObject.Find ("Player4 Menu");

		charSelectMenu.SetActive (false);
		optionsMenu.SetActive (false);
		achievementsMenu.SetActive (false);
		creditsMenu.SetActive (false);
		vsMenu.SetActive (false);
		controlsMenu.SetActive (false);
		mapsMenu.SetActive (false);
		weaponsMenu.SetActive (false);
		player1Controls.SetActive (false);
		player2Controls.SetActive (false);
		player3Controls.SetActive (false);
		player4Controls.SetActive (false);*/
	}
	public void switchScreens(GameObject from, GameObject to) {
		//if there are animations or such, we want it to the same function
		from.SetActive(false);
		to.SetActive(true);
		this.mainEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(to.transform.GetChild(0).GetComponentInChildren<Button>().gameObject);
	}

	//main menu
	//que char select menu
	public void mainMenuStart()
	{
		switchScreens(mainMenu, usesMouseMenu);
		//mainMenu.SetActive (false);
		//numPlayersMenu.SetActive (true);
		//Char select: select number of players 
		//Ask: Will more than one person be using a mouse? (If yes, screen will become minimized)
		//Create MouseInput, tell mouseinput to not do anything yet
		//Start probing for each player, if joystick or mouse
	}
	//que options menu
	public void mainMenuOptions()
	{
		switchScreens(mainMenu, optionsMenu);

		//mainMenu.SetActive (false);
		//optionsMenu.SetActive (true);	
	}
	//que achievements menu
	public void mainMenuAchievements()
	{
		switchScreens(mainMenu, achievementsMenu);

	//	mainMenu.SetActive (false);
	//	achievementsMenu.SetActive (true);
	}
	//que credits menu
	public void mainMenuCredits()
	{
		switchScreens(mainMenu, creditsMenu);

		//mainMenu.SetActive (false);
		//creditsMenu.SetActive (true);
	}
	//exit
	public void mainMenuExit()
	{
		Application.Quit ();
	}


	//character select menu
	//que vs menu
	public void charSelectMenuStart()
	{
		switchScreens(charSelectMenu, vsMenu);

		//charSelectMenu.SetActive (false);
		//vsMenu.SetActive (true);
	}
	//que controls menu
	/*public void charSelectMenuControls()
	{
		charSelectMenu.SetActive (false);
		controlsMenu.SetActive (true);
	}*/
	//que back to main menu
	public void charSelectMenuBack()
	{
		switchScreens(charSelectMenu, numPlayersMenu);

		//charSelectMenu.SetActive (false);
		//mainMenu.SetActive (true);
	}


	//options menu
	public void optionMenuBack() {
		optionsMenu.SetActive (false);
		mainMenu.SetActive (true);
	}




	//achievements menu
	//que back to main menu
	public void achievementsMenuBack()
	{
		switchScreens(achievementsMenu, mainMenu);

//		achievementsMenu.SetActive (false);
//		mainMenu.SetActive (true);
	}


	//credits menu
	//que back to main menu
	public void creditsMenuBack()
	{
		switchScreens(creditsMenu, mainMenu);

		//creditsMenu.SetActive (false);
		//mainMenu.SetActive (true);
	}


	//vs menu
	//que maps menu
	//que weapons menu
	public void vsMenuWeapons()
	{
		switchScreens(vsMenu, weaponsMenu);

		//vsMenu.SetActive (false);
		//weaponsMenu.SetActive (true);
	}
	//que maps menu
	public void vsMenuMaps()
	{
		switchScreens(vsMenu, mapsMenu);

		//vsMenu.SetActive (false);
		//mapsMenu.SetActive (true);
	}
	//que back to char select
	public void vsMenuBack()
	{
		switchScreens(vsMenu, charSelectMenu);

		//vsMenu.SetActive (false);
		//charSelectMenu.SetActive (true);
	}

	//Num players menu
	public void increment() {
		//performance is not an issue here
		Text tx = GameObject.Find("Counter").GetComponent<Text>();
		int result = 0;
		string text = tx.text;
		for (int i = 0; i < text.Length; i++) {
			char letter = text[i];
			result = 10 * result + (int)char.GetNumericValue(letter);
		}
		int newResult = result+1;
		if (newResult <= 4) {
			maxPlayers = newResult;
			tx.text = newResult.ToString();
		}
	}
	public void decrement() {
		Text tx = GameObject.Find("Counter").GetComponent<Text>();
		int result = 0;
		string text = tx.text;
		for (int i = 0; i < text.Length; i++) {
			char letter = text[i];
			result = 10 * result + (int)char.GetNumericValue(letter);
		}
		int newResult = result-1;
		if (newResult > 0) {
			maxPlayers = newResult;
			tx.text = newResult.ToString();	
		}
	}
	public void toControlMenu() {
		switchScreens(numPlayersMenu, charSelectMenu);

		//this.numPlayersMenu.SetActive(false);
		//this.usesMouseMenu.SetActive(true);
	}
	public void numPlayersBack() {
		switchScreens(numPlayersMenu, usesMouseMenu);

		//this.numPlayersMenu.SetActive(false);
		//this.mainMenu.SetActive(true);
	}
	//usesmousemenu
	public void usesMouseBack() {
		switchScreens(usesMouseMenu, mainMenu);
	}
	public void mouseInputYes() {
		createMouse(true);
		switchScreens(usesMouseMenu, this.numPlayersMenu);
	}
	public void mouseInputNo() {
		createMouse(false);
		switchScreens(usesMouseMenu, this.numPlayersMenu);

	}
	private void createMouse(bool multiMouse) {
		if (mouseControllerRef != null) {
			GameObject.Destroy(mouseControllerRef.gameObject);

			print("Destroyed");
		}
		GameObject newObject = (GameObject)GameObject.Instantiate(mouseController, Vector3.zero, Quaternion.identity);
		mouseControllerRef = newObject.GetComponent<MouseInput>();
		mouseControllerRef.singleMouse = !multiMouse;
		mouseControllerRef.Init();
	}


	//maps menu
	//


	//weapons menu
	//



	//old stuff don't bother looking or messing with it but DON'T delete because

	//player1 controls menu
	//que back to controls menu
	/*public void player1MenuBack()
	{
		player1Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}*/
	//player2 controls menu
	//que back to controls menu
	/*public void player2MenuBack()
	{
		player2Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}*/
	//player3 controls menu
	//que back to controls menu
	/*public void player3MenuBack()
	{
		player3Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}*/
	//player4 controls menu
	//que back to controls menu
	/*public void player4MenuBack()
	{
		player4Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}*/

	//controls menu
		//que player 1 controls menu
	/*public void controlsMenuPlayer1Controls()
	{
		controlsMenu.SetActive (false);
		player1Controls.SetActive (true);
	}*/
	//que player 2 controls menu
	/*public void controlsMenuPlayer2Controls()
	{
		controlsMenu.SetActive (false);
		player2Controls.SetActive (true);
	}*/
	//que player 3 controls menu
	/*public void controlsMenuPlayer3Controls()
	{
		controlsMenu.SetActive (false);
		player3Controls.SetActive (true);
	}*/
	//que player 4 controls menu
	/*public void controlsMenuPlayer4Controls()
	{
		controlsMenu.SetActive (false);
		player4Controls.SetActive (true);
	}*/
	//que go back to char select menu
	/*public void controlsMenuBack()
	{
		controlsMenu.SetActive (false);
		charSelectMenu.SetActive (true);
	}*/





}