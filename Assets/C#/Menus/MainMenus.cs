using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;

public class MainMenus : MonoBehaviour 
{

    public GameObject map;
	
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
	public GameObject eachPlayerMenu;
	private int playerIndex; //for traversing of players for each player menu
	private int maxPlayers = 1;

	//char select menu
	public GameObject vsMenu;
	//public GameObject controlsMenu;


	//vs menu
	public GameObject mapsMenu;
	public GameObject weaponsMenu;

    //Char select menu
    public GameObject player1Select, player2Select, player3Select, player4Select;
    public int player1Index, player2Index, player3Index, player4Index;
    public int player1Color, player2Color, player3Color, player4Color;
    public Color[] avaliableColors;
    public int player1Controls, player2Controls, player3Controls, player4Controls;
    public GameObject[] playerSelections;
    public Sprite[] sPlayerSelections;

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
		GameObject selected = to.transform.GetChild(0).GetComponentInChildren<Button>().gameObject;
		print(selected.name);
		this.mainEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(selected);
	}

	//main menu
	//que char select menu
	public void mainMenuStart()
	{
		switchScreens(mainMenu, numPlayersMenu);
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
    //select1
    public void NextChar1()
    {
        
        player1Index++;
        if (player1Index > sPlayerSelections.Length - 1)
        {
            player1Index = 0;
        }
        player1Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player1Index];
    }
    public void PrevChar1()
    {
        
        player1Index--;
        if (player1Index < 0)
        {
            player1Index = sPlayerSelections.Length - 1;
        }
        player1Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player1Index];
    }
    public void NextColor1()
    {
       

        player1Color++;
        if (player1Color > avaliableColors.Length - 1)
        {
            player1Color = 0;
        }
        player1Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player1Color];
    }
    public void PrevColor1()
    {
        
        player1Color--;
        if (player1Color < 0)
        {
            player1Color = avaliableColors.Length - 1;
        }
        player1Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player1Color];
    }
    public void Mouse1()
    {
        if (player1Controls == 1)
        {
            player1Controls = 0;
            player1Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = false;
            player1Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = true;
        } 
    }
    public void Joystick1()
    {
        if (player1Controls == 0)
        {
            player1Controls = 1;
            player1Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = false;
            player1Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = true;
        }
    }
    //select2
    public void NextChar2()
    {
        
        player2Index++;
        if (player2Index > sPlayerSelections.Length - 1)
        {
            player2Index = 0;
        }
        player2Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player2Index];
    }
    public void PrevChar2()
    {
        
        player2Index--;
        if (player2Index < 0)
        {
            player2Index = sPlayerSelections.Length - 1;
        }
        player2Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player2Index];
    }
    public void NextColor2()
    {
        
        player2Color++;
        if (player2Color > avaliableColors.Length - 1)
        {
            player2Color = 0;
        }
        player2Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player2Color];
    }
    public void PrevColor2()
    {
        
        player2Color--;
        if (player2Color < 0)
        {
            player2Color = avaliableColors.Length - 1;
        }
        player2Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player2Color];
    }
    public void Mouse2()
    {
        if (player2Controls == 1)
        {
            player2Controls = 0;
            player2Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = false;
            player2Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = true;
        }
    }
    public void Joystick2()
    {
        if (player2Controls == 0)
        {
            player2Controls = 1;
            player2Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = false;
            player2Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = true;
        }
    }
    //select3
    public void NextChar3()
    {
       
        player3Index++;
        if (player3Index > sPlayerSelections.Length - 1)
        {
            player3Index = 0;
        }
        player3Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player3Index];
    }
    public void PrevChar3()
    {
        
        player3Index--;
        if (player3Index < 0)
        {
            player3Index = sPlayerSelections.Length - 1;
        }
        player3Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player3Index];
    }
    public void NextColor3()
    {
       
        player3Color++;
        if (player3Color > avaliableColors.Length - 1)
        {
            player3Color = 0;
        }
        player3Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player3Color];
    }
    public void PrevColor3()
    {
        
        player3Color--;
        if (player3Color < 0)
        {
            player3Color = avaliableColors.Length - 1;
        }
        player3Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player3Color];
    }
    public void Mouse3()
    {
        if (player3Controls == 1)
        {
            player3Controls = 0;
            player3Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = false;
            player3Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = true;
        } 
    }
    public void Joystick3()
    {
        if (player3Controls == 0)
        {
            player3Controls = 1;
            player3Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = false;
            player3Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = true;
        }
    }
    //select4
    public void NextChar4()
    {
        
        player4Index++;
        if (player4Index > sPlayerSelections.Length - 1)
        {
            player4Index = 0;
        }
        player4Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player4Index];
    }
    public void PrevChar4()
    {
        
        player4Index--;
        if (player4Index < 0)
        {
            player4Index = sPlayerSelections.Length - 1;
        }
        player4Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[player4Index];
    }
    public void NextColor4()
    {
        
        player4Color++;
        if (player4Color > avaliableColors.Length - 1)
        {
            player4Color = 0;
        }
        player4Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player4Color];
    }
    public void PrevColor4()
    {
       
        player4Color--;
        if (player4Color < 0)
        {
            player4Color = avaliableColors.Length - 1;
        }
        player4Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().color = avaliableColors[player4Color];
    }
    public void Mouse4()
    {
        if (player4Controls == 1)
        {
            player4Controls = 0;
            player4Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = false;
            player4Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = true;
        }
    }
    public void Joystick4()
    {
        if (player4Controls == 0)
        {
            player4Controls = 1;
            player4Select.transform.FindChild("Controls").FindChild("JoystickBG").GetComponent<Button>().interactable = false;
            player4Select.transform.FindChild("Controls").FindChild("MouseBG").GetComponent<Button>().interactable = true;
        }
    }
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
        charSelectMenu.transform.FindChild("Player1Select");
        
        player1Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[0];
        player2Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[1];
        player3Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[2];
        player4Select.transform.FindChild("CharImage").FindChild("Image").GetComponent<Image>().sprite = sPlayerSelections[3];
        player1Index = 0;
        player2Index = 1;
        player3Index = 2;
        player4Index = 3;
        for (int i = 1; i <= 4; i++)
        {
            if (i >= maxPlayers + 1)
            {
                charSelectMenu.transform.FindChild("Main Menu Canvas (1)").FindChild("Player" + i + "Select").gameObject.SetActive(false);
            } else
            {
                charSelectMenu.transform.FindChild("Main Menu Canvas (1)").FindChild("Player" + i + "Select").gameObject.SetActive(true);
            }
        }

		//this.numPlayersMenu.SetActive(false);
		//this.usesMouseMenu.SetActive(true);
	}
	public void numPlayersBack() {
		switchScreens(numPlayersMenu, mainMenu);

		//this.numPlayersMenu.SetActive(false);
		//this.mainMenu.SetActive(true);
	}
	//usesmousemenu
	
	private void createMouse(ArrayList playerList) {
		if (mouseControllerRef != null) {
			GameObject.Destroy(mouseControllerRef.gameObject);

			print("Destroyed");
		}
		GameObject newObject = (GameObject)GameObject.Instantiate(mouseController, Vector3.zero, Quaternion.identity);
		mouseControllerRef = newObject.GetComponent<MouseInput>();
        mouseControllerRef.players = playerList;
        int mouseCount = 0;
       foreach (PlayerInfo o in playerList) {
            if (!o.usesJoystick)
            {
                mouseCount++;
            }
        }
        mouseControllerRef.singleMouse = mouseCount <= 1;
		mouseControllerRef.SetUpRound();
        mouseControllerRef.Init();
	}

    public void charSelectMenuDone()
    {
        //hide this
        charSelectMenu.SetActive(false);
        //generate players
        ArrayList playerList = GeneratePlayers();
        //generate mouse controller
        createMouse(playerList);
        //enable map
        map.SetActive(true);
    }
    private ArrayList GeneratePlayers()
    {
        ArrayList playerList = new ArrayList();
        int[] heroCount = new int[playerSelections.Length];
        //Color[] altColors = new Color[4];
        //altColors[0] = new Color(1, )
        //joystick id starts at 1
        //mouseID starts at 0
        int joystickID = 1;
        int mouseID = 0;
        int count = 1;
        /*
        rename Player(x)Parent, Grapple(x), Player(x), Reticle(x)
        Set PlayerID to x
        Set joystickID
        Set MouseID*/
        GameObject player1 = (GameObject)GameObject.Instantiate(playerSelections[player1Index], new Vector3(-7, 2, 0), Quaternion.identity);
        
        player1.transform.name = "Player" + count + "Parent";
        player1.transform.FindChild("Grapple").name = "Grapple" + count;
        player1.transform.FindChild("Player").FindChild("Reticle").name = "Reticle" + count;
        player1.transform.FindChild("Player").name = "Player" + count;
        player1.transform.FindChild("Player" + count).GetComponent<player>().playerid = count;
        player1.transform.FindChild("Player" + count).FindChild("AnimationController").GetComponent<AnimationHandler>().startColor = avaliableColors[player1Color];
        PlayerInfo p1 = new PlayerInfo();
        if (player1Controls == 0)
        {
            p1.id = mouseID;
            p1.usesJoystick = false;
            mouseID++;
        } else
        {
            p1.id = joystickID;
            p1.usesJoystick = true;
            joystickID++;
        }
        playerList.Add(p1);
        count++;
        if (maxPlayers == count - 1) return playerList;

        GameObject player2 = (GameObject)GameObject.Instantiate(playerSelections[player2Index], new Vector3(-3, 2, 0), Quaternion.identity);
        player2.transform.name = "Player" + count + "Parent";
        player2.transform.FindChild("Grapple").name = "Grapple" + count;
        player2.transform.FindChild("Player").FindChild("Reticle").name = "Reticle" + count;
        player2.transform.FindChild("Player").name = "Player" + count;
        player2.transform.FindChild("Player" + count).FindChild("AnimationController").GetComponent<AnimationHandler>().startColor = avaliableColors[player2Color];

        player2.transform.FindChild("Player" + count).GetComponent<player>().playerid = count;
        PlayerInfo p2 = new PlayerInfo();
        if (player2Controls == 0)
        {
            p2.id = mouseID;
            p2.usesJoystick = false;
            mouseID++;
        }
        else
        {
            p2.id = joystickID;
            p2.usesJoystick = true;
            joystickID++;
        }
        playerList.Add(p2);
        count++;
        if (maxPlayers == count - 1) return playerList;

        GameObject player3 = (GameObject)GameObject.Instantiate(playerSelections[player3Index], new Vector3(3, 2, 0), Quaternion.identity);
        player3.transform.name = "Player" + count + "Parent";
        player3.transform.FindChild("Grapple").name = "Grapple" + count;
        player3.transform.FindChild("Player").FindChild("Reticle").name = "Reticle" + count;
        player3.transform.FindChild("Player").name = "Player" + count;
        player3.transform.FindChild("Player" + count).FindChild("AnimationController").GetComponent<AnimationHandler>().startColor = avaliableColors[player3Color];

        player3.transform.FindChild("Player" + count).GetComponent<player>().playerid = count;
        PlayerInfo p3 = new PlayerInfo();
        if (player3Controls == 0)
        {
            p3.id = mouseID;
            p3.usesJoystick = false;
            mouseID++;
        }
        else
        {
            p3.id = joystickID;
            p3.usesJoystick = true;
            joystickID++;
        }
        playerList.Add(p3);
        count++;
        if (maxPlayers == count - 1) return playerList;

        GameObject player4 = (GameObject)GameObject.Instantiate(playerSelections[player4Index], new Vector3(3, 2, 0), Quaternion.identity);
        player4.transform.name = "Player" + count + "Parent";
        player4.transform.FindChild("Grapple").name = "Grapple" + count;
        player4.transform.FindChild("Player").FindChild("Reticle").name = "Reticle" + count;
        player4.transform.FindChild("Player").name = "Player" + count;
        player4.transform.FindChild("Player" + count).FindChild("AnimationController").GetComponent<AnimationHandler>().startColor = avaliableColors[player4Color];

        player3.transform.FindChild("Player" + count).GetComponent<player>().playerid = count;
        PlayerInfo p4 = new PlayerInfo();
        if (player4Controls == 0)
        {
            p4.id = mouseID;
            p4.usesJoystick = false;
            mouseID++;
        }
        else
        {
            p4.id = joystickID;
            p4.usesJoystick = true;
            joystickID++;
        }
        playerList.Add(p4);
        count++;
        if (maxPlayers == count - 1) return playerList;
        return playerList; 
    }


}