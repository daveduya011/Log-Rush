using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public int coinMultiplier = 5;
    public float speedIncrement = 0.2f;
    public float timerStarter = 3;
    public PlayerController player;
    public float nextLogDistance = 5f;
    public bool isGameOver;

    public float doNothingSpeed = 1f; 
    public Vector3 firstLogPos;
    public Score score;
    public RectTransform crocodilePanel;

    public bool isStarted;
    [HideInInspector]
    public bool isSpawnCrocodile;

    public Log logPrefab;
    public RectTransform splashPanel;
    public RectTransform drowningPanel;
    public CoinUpdater coinUpdater;
    public AdReviveCanvas adReviveCanvas;

    private Log currentLog;
    private InputManager inputManager;
    private CameraController cameraController;

    private Log[] logs;

    private float currentDoNothingTimer = 0;
    private float currentTimerStart = 0;
    private int tempTimerStart = 0;

    [Header("Shake Detection")]
    public int numOfShakes = 3;
    private int tempNumOfShakes = 0;
    public float shakeDetectionThreshold = 2.0f;
    private float sqrShakeDetectionThreshold;
    private float lastSpeed = 0;

    [Header("Audio")]
    public AudioClip timerSound;
    public AudioClip correctSound;
    public AudioClip drowningSound;
    public AudioClip jumpInWaterSound;
    public AudioClip alligatorSound;
    public AudioClip coinCollectSound;


    public AudioClip[] successfulJumps;
    public AudioClip[] failJumps;

    public bool isMultiplayer;
    public int finishLine;
    public GameObject finishLinePrefab;
    void Awake() {
        StartCoroutine(PlaySplashTransition(0.5f));
    }
    void Start() {
        cameraController = Camera.main.GetComponent<CameraController>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        inputManager.BeforeInputFinished.AddListener(OnInputFinished);
        sqrShakeDetectionThreshold = Mathf.Pow(shakeDetectionThreshold, 2);
        logs = new Log[5];

        CreateLog(0);
        GoToLog(0);

        currentTimerStart = timerStarter;
        doNothingSpeed = CalculateDoNothingSpeed();
        FXSoundSystem.Instance.Stop();
    }

    IEnumerator PlaySplashTransition(float outTime) {
        splashPanel.gameObject.SetActive(true);
        splashPanel.GetComponent<Animator>().SetBool("isSplashOut", true);
        yield return new WaitForSeconds(outTime);
        splashPanel.GetComponent<Animator>().SetBool("isSplashOut", false);
        yield return new WaitForSeconds(outTime + 1f);
        splashPanel.gameObject.SetActive(false);
    }

    private Log CreateLog(int index) {
        // generate finish line
        if (finishLine != -1 && isMultiplayer) {
            if (index == finishLine) {
                GameObject finishlineObj = Instantiate(finishLinePrefab, firstLogPos, Quaternion.identity);
                finishlineObj.transform.position = firstLogPos + (Vector3.up * index * nextLogDistance) - (Vector3.up * nextLogDistance * 1.5f);
                return null;
            } else if (index > finishLine){
                return null;
            }
        }

        logs[index % 5] = Instantiate(logPrefab, firstLogPos, Quaternion.identity);
        logs[index % 5].transform.position = firstLogPos + (Vector3.up * index * nextLogDistance);
        return logs[index % 5];
    }

    private void Update() {
        if (currentLog == null)
            return;
        if (player.isFinishLineReached)
            return;

        if (!isStarted) {
            if (isMultiplayer)
                return;
            if (currentTimerStart > 0) {
                currentTimerStart -= Time.deltaTime;
                if (tempTimerStart != (int)currentTimerStart) {
                    tempTimerStart = (int)currentTimerStart;
                    score.PlayText((tempTimerStart).ToString() );
                    if (tempTimerStart == 0) {
                        BGSoundSystem.Instance.PlayBGGamePlay();
                        score.PlayText("go!");
                    }
                    if (tempTimerStart <= 3) {
                        FXSoundSystem.Instance.PlaySound(timerSound);
                    }
                }
            }
            else {
                isStarted = true;
                cameraController.state = CameraController.State.Scrolling;
            }
            return;
        }

        if (isGameOver)
            return;

        if (isSpawnCrocodile) {
            if (IsShaking()) {
                if (tempNumOfShakes > 0) {
                    tempNumOfShakes--;
                } else {
                    RemoveCrocodile();
                }
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                RemoveCrocodile();
            }
            return;
        }


        if (currentLog.isDoNothing) {
            if (currentDoNothingTimer > 0) {
                currentDoNothingTimer -= Time.deltaTime;
            }
            else {
                doNothingSpeed = CalculateDoNothingSpeed();
                currentLog.OnCorrect();
                GoToNextLog();
            }
        }

    }

    private float CalculateDoNothingSpeed() {
        // speed = 1 - 15 = maxSpeed + 1
        // nothingspeed = 1 - 0.5 = nothingSpeed -= 0.5 / (maxSpeed + 1)
        if (cameraController.speed < cameraController.maxSpeed)
            return 1 - (0.5f / (cameraController.maxSpeed + 1)) * (0.2f * GameManager.Instance.currentLogIndex);
        return 0.5f;
    }

    private void RemoveCrocodile() {
        cameraController.speed = lastSpeed;
        isSpawnCrocodile = false;
        currentLog.animator.SetBool("isCrocodile", false);
        crocodilePanel.gameObject.SetActive(false);
        FXSoundSystem.Instance.Stop();
    }

    private bool IsShaking() {
        if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold) {
            return true;
        }
        return false;
    }

    private void OnInputFinished() {
        if (isSpawnCrocodile)
            return;
        if (!isStarted)
            return;
        if (isGameOver)
            return;
        if (cameraController.state == CameraController.State.Focusing)
            return;
        if (currentLog == null)
            return;

        if (currentLog.isDoNothing) {
            GameOver();
            return;
        }
        Command command = currentLog.currentCommand;
        Command.Gesture gesture = command.gesture;
        currentLog.isActive = false;


        if (currentLog.conjunction == Log.Conjunction.OR) {
            Command[] commands = currentLog.commands;
            bool isGestureCorrect = false;
            if (!command.isDone) {
                for (int i = 0; i < commands.Length; i++) {
                    isGestureCorrect = CheckGesture(commands[i].gesture);
                    if (isGestureCorrect) {
                        currentLog.currentCommand = currentLog.commands[i];
                        break;
                    }
                }
            }

            

        } 
        command.isDone = currentLog.isNegative ?
            !CheckGesture(currentLog.currentCommand.gesture) : CheckGesture(currentLog.currentCommand.gesture);

        if (command.isDone) {
            currentLog.inputCounter++;
            if (currentLog.inputCounter == currentLog.currentCommand.times) {
                currentLog.OnCorrect();
                if (currentLog.conjunction == Log.Conjunction.OR) {
                    GoToNextLog();
                } else {
                    if (currentLog.currentIndex < currentLog.commands.Length - 1) {
                        currentLog.GoToNextCommand();
                        FXSoundSystem.Instance.PlaySound(correctSound, 0.5f);
                    }
                    else {
                        GoToNextLog();
                    }
                }
            }
        }

        if (!command.isDone) {
            GameOver();
        }
    }

    private bool CheckGesture(Command.Gesture gesture) {
        switch (gesture) {
            case Command.Gesture.DOWN:
                return inputManager.isSwipedDown;
            case Command.Gesture.UP:
                return inputManager.isSwipedUp;
            case Command.Gesture.LEFT:
                return inputManager.isSwipedLeft;
            case Command.Gesture.RIGHT:
                return inputManager.isSwipedRight;
            case Command.Gesture.TAP:
                return inputManager.isTapped;
        }
        return false;
    }

    private void GoToNextLog() {
        int rand = RandomGenerator.GenerateRandom(successfulJumps.Length);
        FXSoundSystem.Instance.PlaySound(successfulJumps[rand]);

        currentDoNothingTimer = doNothingSpeed;
        player.JumpToPosition(currentLog.transform.position);
        cameraController.focusObject(currentLog.transform);

        currentLog.animator.SetBool("isLanded", true);
        StartCoroutine(SetLogLandedFalse(currentLog, 0.2f));

        int index = GameManager.Instance.NextLog();
        score.UpdateScore();

        if (finishLine != -1 && index == finishLine) {
            player.SetFinishLineReached();
            return;
        }

        GoToLog(index);
        if (cameraController.speed < cameraController.maxSpeed) {
            if (cameraController.speed > 12f) {
                BGSoundSystem.Instance.AddPitch(0.02f);
            }
            cameraController.speed += speedIncrement;
        }
        else {
            if (GameManager.Instance.currentLogIndex % 100 == 0) {
                BGSoundSystem.Instance.AddPitch(0.04f);
            }
            if (GameManager.Instance.currentLogIndex % 500 == 0) {
                cameraController.speed += speedIncrement;
            }
        }

        // every 10's logs, add 5 coins
        if (GameManager.Instance.currentLogIndex % 10 == 0) {
            coinUpdater.AddCoins(coinMultiplier);
            GameManager.Instance.coins += coinMultiplier;
            player.CollectCoins();
        }
    }

    public async void GameOver() {
        FXSoundSystem.Instance.Stop();
        Vector3 pos = currentLog.transform.position;
        pos.y -= 2.5f;
        player.Drown(pos);

        isGameOver = true;
        currentLog.OnIncorrect();
        player.state = PlayerController.State.Dead;
        BGSoundSystem.Instance.SetPitch(1f);

        int rand = RandomGenerator.GenerateRandom(failJumps.Length);
        FXSoundSystem.Instance.PlaySound(failJumps[rand]);

        cameraController.state = CameraController.State.Idle;

        // remove the crocodile if spawned
        if (isSpawnCrocodile)
            RemoveCrocodile();
        
        await Task.Delay(500);

        // show revive canvas
        adReviveCanvas.Show();
    }

    public async void Revive() {
        cameraController.focusObjectToCenter(player.transform);
        player.Revive();
        GoToNextLog();
        cameraController.state = CameraController.State.Idle;

        cameraController.speed -= 5f;
        if (cameraController.speed < 1)
            cameraController.speed = 1f;

        await Task.Delay(1000);

        isGameOver = false;
        cameraController.state = CameraController.State.Scrolling;
    }

    private void GoToLog(int index) {
        Log previousLog = logs[(index + 1) % 5];
        if (previousLog != null) {
            previousLog.gameObject.SetActive(false);
        }

        currentLog = logs[index % 5];
        currentLog.isActive = true;

        CreateLog((index + 1));

        isSpawnCrocodile = RandomGenerator.GenerateRandomCrocodile();
        if (isSpawnCrocodile) {
            lastSpeed = cameraController.speed;
            cameraController.speed = cameraController.onCrocsSpawnedSpeed < cameraController.speed ? cameraController.onCrocsSpawnedSpeed : cameraController.speed;
            tempNumOfShakes = numOfShakes;
            crocodilePanel.gameObject.SetActive(true);
            currentLog.animator.SetBool("isCrocodile", true);
            FXSoundSystem.Instance.PlayLooped(alligatorSound);
        }
    }

    IEnumerator SetLogLandedFalse(Log log, float time) {
        yield return new WaitForSeconds(time);
        log.animator.SetBool("isLanded", false);
    }
}