using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon;
using UnityEngine.SceneManagement;

class RegisterUser
{
    public string user_id;
    public string password;
    public string dateOfBirth;
}

class LoginUser
{
    public string user_id;
    public string password;
}

class ShopUser
{
    public string user_id;
    public string session_cookie;
    public string Money;
    public string item_id;
    public bool buyItem;
           
    public string info;
}

public class Network : MonoBehaviour
{
    // Scene Variables
    // Register
    public Canvas registerCanvas;
    public TMP_Text registerUsernameText;
    public TMP_Text registerPasswordText;
    public TMP_Text dateOfBirthText;

    // Login
    public Canvas loginCanvas;
    public TMP_Text loginUsernameText;
    public TMP_Text loginPasswordText;

    // Game
    public Canvas gameCanvas;
    public TMP_Text inventoryBodyText;
    public string item_id;
    public bool buy;

    bool update;

    RegisterUser newUser;
    LoginUser returningUser;
    ShopUser shopUser;

    // Start is called before the first frame update
    void Start()
    {
        EnableUIs(true, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Session.shopUser.sessionCookie);

        if (update)
        {

        }
    }

    // Registration
    const string registerURL = "https://3c9a8qkwri.execute-api.us-east-2.amazonaws.com/default/AddNewProfile";
    string registerJSON;

    public void Register()
    {
        newUser = new RegisterUser();
        newUser.user_id = registerUsernameText.text;
        newUser.password = registerPasswordText.text;
        newUser.dateOfBirth = dateOfBirthText.text;

        registerJSON = JsonUtility.ToJson(newUser);

        this.StartCoroutine(this.RegisterRequestRoutine(registerURL));
    }

    IEnumerator RegisterRequestRoutine(string _url)
    {
        var request = UnityWebRequest.Put(_url, registerJSON);

        yield return request.SendWebRequest();

        var data = request.downloadHandler.text;

        yield return new WaitForSeconds(1.0f);

        if (data.ToString() == "New user created")
        {
            EnableUIs(false, true, false);
        }
        Debug.Log(data);
    }

    // Login
    const string loginURL = "https://pfs49lks47.execute-api.us-east-2.amazonaws.com/default/LoginToProfile";
    string loginJSON;

    public void Login()
    {
        returningUser = new LoginUser();
        returningUser.user_id = loginUsernameText.text;
        returningUser.password = loginPasswordText.text;

        loginJSON = JsonUtility.ToJson(returningUser);

        //Session.shopUser.user_id = returningUser.user_id;

        this.StartCoroutine(this.LoginRequestRoutine(loginURL));
    }

    IEnumerator LoginRequestRoutine(string _url)
    {
        var request = UnityWebRequest.Put(_url, loginJSON);

        yield return request.SendWebRequest();

        var data = request.downloadHandler.text;

        shopUser = new ShopUser();
        shopUser.session_cookie = data.ToString();

        yield return new WaitForSeconds(2.0f);

        if (data.ToString() != "Incorrect username or password")
        {

            EnableUIs(false, false, true);

            GetUser();

            Debug.Log(shopUser.session_cookie);
        }
        else
        {
            Debug.Log(data.ToString());
        }
    }

    // Game
    const string gameURL = "https://waljebe2pi.execute-api.us-east-2.amazonaws.com/default/UpdateProfile";
    string gameJSON;

    public void BuyHealthPotion()
    {
        item_id = "health potion";
        buy = true;

        ItemInteract();
    }

    public void BuyManaPotion()
    {
        item_id = "mana potion";
        buy = true;

        ItemInteract();
    }

    public void BuyDiamondRing()
    {
        item_id = "diamond ring";
        buy = true;

        ItemInteract();
    }

    public void SellHealthPotion()
    {
        item_id = "health potion";
        buy = false;

        ItemInteract();
    }

    public void SellManaPotion()
    {
        item_id = "mana potion";
        buy = false;

        ItemInteract();
    }

    public void SellDiamondRing()
    {
        item_id = "diamond ring";
        buy = false;

        ItemInteract();
    }

    public void ItemInteract()
    {
        shopUser.item_id = item_id;
        shopUser.buyItem = buy;

        gameJSON = JsonUtility.ToJson(shopUser);

        this.StartCoroutine(this.BuyRequestRoutine(gameURL));
    }

    IEnumerator BuyRequestRoutine(string _url)
    {
        var request = UnityWebRequest.Put(_url, gameJSON);

        yield return request.SendWebRequest();

        var data = request.downloadHandler.text;

        Debug.Log(data.ToString());

        GetUser();
    }

    // Get User
    const string getUserURL = "https://gxz46dsnc9.execute-api.us-east-2.amazonaws.com/default/GetProfile";
    string getJSON;

    public void GetUser()
    {
        shopUser.user_id = returningUser.user_id;

        getJSON = JsonUtility.ToJson(shopUser);

        this.StartCoroutine(this.GetRequestRoutine(getUserURL));
    }

    IEnumerator GetRequestRoutine(string _url)
    {
        var request = UnityWebRequest.Put(_url, getJSON);

        yield return request.SendWebRequest();

        var data = request.downloadHandler.text;

        shopUser.info = data.ToString();

        inventoryBodyText.text = shopUser.info;

        Debug.Log("Shop: " + data.ToString());
    }

    public void EnableUIs(bool _register, bool _login, bool _game)
    {
        registerCanvas.enabled = _register;
        loginCanvas.enabled = _login;
        gameCanvas.enabled = _game;
    }

    public void Logout()
    {
        registerCanvas.enabled = false;
        loginCanvas.enabled = true;
        gameCanvas.enabled = false;

        shopUser.session_cookie = "";
    }

    public void RegisterScene()
    {
        registerCanvas.enabled = true;
        loginCanvas.enabled = false;
        gameCanvas.enabled = false;
    }
}
