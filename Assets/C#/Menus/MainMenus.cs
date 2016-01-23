using UnityEngine;
using System.Collections;

public class MainMenus : MonoBehaviour 
{

	//main menu
	public GameObject mainMenu;
	public GameObject charSelectMenu;
	public GameObject optionsMenu;
	public GameObject achievementsMenu;
	public GameObject creditsMenu;

	//char select menu
	public GameObject vsMenu;
	public GameObject controlsMenu;


	//vs menu
	public GameObject mapsMenu;
	public GameObject weaponsMenu;

	//controls menu
	public GameObject player1Controls;
	public GameObject player2Controls;
	public GameObject player3Controls;
	public GameObject player4Controls;


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


	//main menu
	//que char select menu
	public void mainMenuStart()
	{
		mainMenu.SetActive (false);
		charSelectMenu.SetActive (true);
	}
	//que options menu
	public void mainMenuOptions()
	{
		mainMenu.SetActive (false);
		optionsMenu.SetActive (true);
	}
	//que achievements menu
	public void mainMenuAchievements()
	{
		mainMenu.SetActive (false);
		achievementsMenu.SetActive (true);
	}
	//que credits menu
	public void mainMenuCredits()
	{
		mainMenu.SetActive (false);
		creditsMenu.SetActive (true);
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
		charSelectMenu.SetActive (false);
		vsMenu.SetActive (true);
	}
	//que controls menu
	public void charSelectMenuControls()
	{
		charSelectMenu.SetActive (false);
		controlsMenu.SetActive (true);
	}
	//que back to main menu
	public void charSelectMenuBack()
	{
		charSelectMenu.SetActive (false);
		mainMenu.SetActive (true);
	}


	//options menu





	//achievements menu
	//que back to main menu
	public void achievementsMenuBack()
	{
		achievementsMenu.SetActive (false);
		mainMenu.SetActive (true);
	}


	//credits menu
	//que back to main menu
	public void creditsMenuBack()
	{
		creditsMenu.SetActive (false);
		mainMenu.SetActive (true);
	}


	//vs menu
	//que maps menu
	//que weapons menu
	public void vsMenuWeapons()
	{
		vsMenu.SetActive (false);
		weaponsMenu.SetActive (true);
	}
	//que maps menu
	public void vsMenuMaps()
	{
		vsMenu.SetActive (false);
		mapsMenu.SetActive (true);
	}
	//que back to char select
	public void vsMenuBack()
	{
		vsMenu.SetActive (false);
		charSelectMenu.SetActive (true);
	}


	//controls menu
	//que player 1 controls menu
	public void controlsMenuPlayer1Controls()
	{
		controlsMenu.SetActive (false);
		player1Controls.SetActive (true);
	}
	//que player 2 controls menu
	public void controlsMenuPlayer2Controls()
	{
		controlsMenu.SetActive (false);
		player2Controls.SetActive (true);
	}
	//que player 3 controls menu
	public void controlsMenuPlayer3Controls()
	{
		controlsMenu.SetActive (false);
		player3Controls.SetActive (true);
	}
	//que player 4 controls menu
	public void controlsMenuPlayer4Controls()
	{
		controlsMenu.SetActive (false);
		player4Controls.SetActive (true);
	}
	//que go back to char select menu
	public void controlsMenuBack()
	{
		controlsMenu.SetActive (false);
		charSelectMenu.SetActive (true);
	}


	//maps menu
	//


	//weapons menu
	//


	//player1 controls menu
	//que back to controls menu
	public void player1MenuBack()
	{
		player1Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}
	//player2 controls menu
	//que back to controls menu
	public void player2MenuBack()
	{
		player2Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}
	//player3 controls menu
	//que back to controls menu
	public void player3MenuBack()
	{
		player3Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}
	//player4 controls menu
	//que back to controls menu
	public void player4MenuBack()
	{
		player4Controls.SetActive (false);
		controlsMenu.SetActive (true);
	}





}