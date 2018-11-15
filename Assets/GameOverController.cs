using Gamekit3D;
using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverController : MonoBehaviour
{
    public Camera cameraGameOver;
    public Camera cameraMain;

    public GameObject boss;
    public GameCommandReceiver hugeDoor;
    public GameObject Ellen;

    public GameObject spawnPointPerson01;
    public GameObject spawnPointPerson02;
    public GameObject spawnPointPerson03;

    public UnityEvent OnDialogShow;
    public UnityEvent OnDialogClose;

    public static bool finishGame = false;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartEndGame()
    {
        StartCoroutine(GameOver());
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        
        SendGameCommand sendGameCommand = new SendGameCommand();

        sendGameCommand.interactionType = GameCommandType.Open;
        sendGameCommand.interactiveObject = hugeDoor;
        sendGameCommand.coolDown = 1;
        sendGameCommand.oneShot = true;
        sendGameCommand.Send();

        yield return new WaitForSeconds(9f);
        Destroy(boss);
        cameraMain.enabled = false;
        cameraGameOver.enabled = true;
        Ellen.SetActive(false);
        OnDialogShow.Invoke();
        OnDialogClose.Invoke();

        for (int i = 0; i < 5; i++)
        {
            GameObject person01 = Resources.Load<GameObject>("Prefabs/TT_demo_female");
            Instantiate(person01, spawnPointPerson01.transform.position, spawnPointPerson01.transform.rotation, this.transform);
            yield return new WaitForSeconds(1f);

            GameObject person02 = Resources.Load<GameObject>("Prefabs/TT_demo_male_A");
            person02.transform.SetPositionAndRotation(spawnPointPerson02.transform.position, spawnPointPerson02.transform.rotation);
            Instantiate(person02, spawnPointPerson02.transform.position, spawnPointPerson02.transform.rotation, this.transform);
            yield return new WaitForSeconds(1f);

            GameObject person03 = Resources.Load<GameObject>("Prefabs/TT_demo_male_B");
            Instantiate(person03, spawnPointPerson03.transform.position, spawnPointPerson03.transform.rotation, this.transform);
            yield return new WaitForSeconds(1f);
        }

        ScreenFader.SetAlpha(1f);
        StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.GameOver));
        finishGame = true;
    }
}
