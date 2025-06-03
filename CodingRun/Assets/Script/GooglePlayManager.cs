using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayManager : MonoBehaviour
{
    public static GooglePlayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGPGS();
    }

    private void InitializeGPGS()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignIn(silent: true);
    }

    public void SignIn(bool silent)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Debug.Log("이미 로그인됨");
            return;
        }

        if (silent)
        {
            PlayGamesPlatform.Instance.Authenticate(OnSignIn);
        }
        else
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(OnSignIn);
        }
    }

    private void OnSignIn(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log($"GPGS 로그인 성공: {PlayGamesPlatform.Instance.GetUserDisplayName()}");
        }
        else
        {
            Debug.LogWarning($"GPGS 로그인 실패: {status}");
        }
    }

    public bool IsSignedIn()
    {
        return PlayGamesPlatform.Instance.IsAuthenticated();
    }

    public void RecordScore(long HighScore, bool UI = false)
    {
        Social.ReportScore(HighScore, GPGSIds.leaderboard_score, (bool success) => {
            if (success)
            {
                Debug.Log("Leader Good");
            }
        });
    }

    public void ShowLeaderboardUI()
    {
        if (IsSignedIn())
        {
            ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(GPGSIds.leaderboard_score);
        }
        else
        {
            Debug.LogWarning("로그인되어 있지 않음");
        }
    }
}
