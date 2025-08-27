

using UnityEngine;
using System.Collections;

namespace ChainEmpires
{
    public class SessionManager : IManager
    {
        [Header("Session Settings")]
        public float sessionDuration = 300f; // 5 minutes in seconds
        public float raidCooldown = 60f; // Time between raid opportunities
        public float recoveryTime = 120f; // Time to recover after raid
        
        [Header("Session State")]
        public SessionState currentState = SessionState.Idle;
        public float sessionTimer = 0f;
        public float stateTimer = 0f;
        public int raidsCompleted = 0;
        public bool sessionActive = false;
        
        public enum SessionState
        {
            Idle,
            Building,
            RaidAvailable,
            RaidInProgress,
            Recovery,
            SessionComplete
        }
        
        // Events
        public System.Action<SessionState> OnSessionStateChanged;
        public System.Action<float> OnSessionTimerUpdated;
        public System.Action OnRaidAvailable;
        public System.Action OnRaidStarted;
        public System.Action OnRaidCompleted;
        public System.Action OnSessionComplete;
        
        public void Initialize()
        {
            Debug.Log("SessionManager initialized");
            ResetSession();
        }
        
        public void Update()
        {
            if (sessionActive)
            {
                sessionTimer += Time.deltaTime;
                stateTimer += Time.deltaTime;
                
                OnSessionTimerUpdated?.Invoke(sessionTimer);
                
                HandleSessionState();
                
                // Check for session completion
                if (sessionTimer >= sessionDuration)
                {
                    CompleteSession();
                }
            }
        }
        
        public void OnDestroy()
        {
            // Cleanup
        }
        
        public void StartSession()
        {
            sessionActive = true;
            sessionTimer = 0f;
            raidsCompleted = 0;
            ChangeState(SessionState.Building);
            Debug.Log("Session started - 5 minute timer running");
        }
        
        public void PauseSession()
        {
            sessionActive = false;
            Debug.Log("Session paused");
        }
        
        public void ResumeSession()
        {
            sessionActive = true;
            Debug.Log("Session resumed");
        }
        
        public void ResetSession()
        {
            sessionActive = false;
            sessionTimer = 0f;
            stateTimer = 0f;
            raidsCompleted = 0;
            ChangeState(SessionState.Idle);
            Debug.Log("Session reset");
        }
        
        public void CompleteSession()
        {
            sessionActive = false;
            ChangeState(SessionState.SessionComplete);
            OnSessionComplete?.Invoke();
            Debug.Log("Session completed! Total raids: " + raidsCompleted);
            
            // Save progress
            SaveSessionProgress();
        }
        
        private void HandleSessionState()
        {
            switch (currentState)
            {
                case SessionState.Building:
                    if (stateTimer >= raidCooldown)
                    {
                        ChangeState(SessionState.RaidAvailable);
                    }
                    break;
                    
                case SessionState.RaidAvailable:
                    // Wait for player to initiate raid
                    break;
                    
                case SessionState.RaidInProgress:
                    // Raid logic handled by combat system
                    // This state change should be triggered by combat completion
                    break;
                    
                case SessionState.Recovery:
                    if (stateTimer >= recoveryTime)
                    {
                        ChangeState(SessionState.Building);
                    }
                    break;
            }
        }
        
        public void StartRaid()
        {
            if (currentState == SessionState.RaidAvailable)
            {
                ChangeState(SessionState.RaidInProgress);
                OnRaidStarted?.Invoke();
                Debug.Log("Raid started!");
            }
        }
        
        public void CompleteRaid(bool success)
        {
            if (currentState == SessionState.RaidInProgress)
            {
                raidsCompleted++;
                ChangeState(SessionState.Recovery);
                OnRaidCompleted?.Invoke();
                Debug.Log($"Raid completed {(success ? "successfully" : "failed")}! Total raids: {raidsCompleted}");
            }
        }
        
        private void ChangeState(SessionState newState)
        {
            SessionState previousState = currentState;
            currentState = newState;
            stateTimer = 0f;
            
            OnSessionStateChanged?.Invoke(newState);
            Debug.Log($"Session state changed: {previousState} -> {newState}");
        }
        
        private void SaveSessionProgress()
        {
            // Save session data for persistence
            SessionData data = new SessionData
            {
                sessionTimer = sessionTimer,
                raidsCompleted = raidsCompleted,
                totalResources = GameManager.Instance?.ResourceManager?.GetAllResources() ?? new System.Collections.Generic.Dictionary<ResourceManager.ResourceType, float>(),
                buildingsConstructed = GameManager.Instance?.BuildingManager?.GetAllBuildings()?.Count ?? 0,
                unitsTrained = GameManager.Instance?.UnitManager?.GetAllUnits()?.Count ?? 0
            };
            
            // TODO: Implement actual save system (PlayerPrefs, file, or cloud)
            Debug.Log("Session progress saved: " + JsonUtility.ToJson(data));
        }
        
        public string GetSessionStatus()
        {
            string status = $"Session: {currentState}\n";
            status += $"Time: {sessionTimer:0}s / {sessionDuration:0}s\n";
            status += $"Raids: {raidsCompleted}\n";
            status += $"State Timer: {stateTimer:0}s";
            
            switch (currentState)
            {
                case SessionState.RaidAvailable:
                    status += "\n\nRAID AVAILABLE!\nTap to attack";
                    break;
                case SessionState.Recovery:
                    status += $"\n\nRecovering... {recoveryTime - stateTimer:0}s remaining";
                    break;
            }
            
            return status;
        }
        
        public float GetSessionProgress()
        {
            return sessionTimer / sessionDuration;
        }
        
        public float GetStateProgress()
        {
            switch (currentState)
            {
                case SessionState.Building:
                    return stateTimer / raidCooldown;
                case SessionState.Recovery:
                    return stateTimer / recoveryTime;
                default:
                    return 0f;
            }
        }
        
        [System.Serializable]
        public class SessionData
        {
            public float sessionTimer;
            public int raidsCompleted;
            public System.Collections.Generic.Dictionary<ResourceManager.ResourceType, float> totalResources;
            public int buildingsConstructed;
            public int unitsTrained;
        }
    }
}

