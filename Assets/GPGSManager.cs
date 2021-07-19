using UnityEngine;
using System.Collections;

using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GPGSManager : MonoBehaviour {

    void Awake() {
        PlayGamesClientConfiguration config =
            new PlayGamesClientConfiguration.Builder()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // Activate the Play Games platform. This will make it the default
        // implementation of Social.Active
        
        PlayGamesPlatform.Activate();

        Login();
    }

    // Update is called once per frame
    void Update() {

    }

    public void Login() {
        Social.localUser.Authenticate((bool success) => {
            if (success) {
                Debug.Log("Success");
                PostArchievement();
            } else {

            }

            //ShowLoginMenu();
        });
    }

    public void ShowLeaderboard() {
        Social.ShowLeaderboardUI();
    }

    public void PostArchievement() {
        if (Social.localUser.authenticated)
            Social.ReportProgress(GPGSSstatics.achievement_emergency_landing, 100.0f, (bool success) => {
                if (success) {
                    Debug.Log("Reported");
                } else {
                }
            });
    }
}
