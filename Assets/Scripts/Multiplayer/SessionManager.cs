using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;

public static class SessionManager
{
    public static ISession Session {  get; private set; }
    public static string SessionCode = null;


    public const int MAXPLAYERS = 4;

    public static ConnectionState State { get; private set; } = ConnectionState.Disconnected;
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
    }

    public static async Task<TaskResult> HostSession()
    {
        //Signs in the player, returns out if sign in fails
        TaskResult playerSignInResult = await SignInPlayer();

        if (playerSignInResult.Equals(TaskResult.Faliure))
        {
            Debug.Log("Player sign in error");
            return TaskResult.Faliure;
        }

        //Generates a random session ID for simplicity
        string SessionName = UnityEngine.Random.Range(10, 99).ToString();

        // Set the session options.
        var options = new SessionOptions()
        {
            Name = SessionName,
            MaxPlayers = MAXPLAYERS
        }.WithDistributedAuthorityNetwork();

        Debug.Log($"Creating session {options.Name}...");

        //Attempts to host new session
        try
        {
            Session = await MultiplayerService.Instance.CreateSessionAsync(options);
            State = ConnectionState.Connected;
        }
        catch (Exception e)
        {
            State = ConnectionState.Disconnected;
            Debug.Log(e);
            return TaskResult.Faliure;
        }

        SessionCode = Session.Code;

        return TaskResult.Success;
    }

    public static async Task<TaskResult> JoinSession(string sessionCode)
    {
        //Signs in the player, returns out if sign in fails
        TaskResult result = await SignInPlayer();

        if (result.Equals(TaskResult.Faliure))
        {
            Debug.Log("Player sign in error");
            return TaskResult.Faliure;
        }

        Debug.Log($"Connecting to {sessionCode}...");

        try
        {
            Session = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);
            State = ConnectionState.Connected;

            SessionCode = sessionCode;
        }
        catch (Exception e)
        {
            State = ConnectionState.Disconnected;
            Debug.Log(e);
            return TaskResult.Faliure;
        }

        return TaskResult.Success;
    }

    public static async Task LeaveSession()
    {
        if (Session != null)
        {
            await Session.LeaveAsync();
        }

        State = ConnectionState.Disconnected;
    }

    public static async Task<TaskResult> SignInPlayer()
    {
        if (string.IsNullOrEmpty(PlayerProfile.PlayerName))
        {
            return TaskResult.Faliure;
        }

        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SwitchProfile(PlayerProfile.PlayerName);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return TaskResult.Faliure;
        }

        return TaskResult.Success;
    }

    public enum TaskResult
    {
        Success,
        Faliure
    }
}
