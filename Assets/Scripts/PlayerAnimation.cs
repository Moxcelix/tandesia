using System.Collections;
using UnityEngine;

public enum AnimationState : int
{
    NONE = 0,
    PLAY = 1,
}

public class PlayerAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct Frame 
    {
        public GameObject gameObject;
        public float time; 

        public WaitForSeconds Pause { get; private set; }

        public void Initialize(float speed)
        {
           Pause = new WaitForSeconds(time / speed);
        }
    }

    [System.Serializable]
    public struct Track
    {
        public Frame[] frames;
        public float speed;

        public void Initialize()
        {
            for (int i = 0; i < frames.Length; i++) frames[i].Initialize(speed);
        }
    }

    [SerializeField] private Track[] tracks;
    [SerializeField] private Transform frameGameObjectsParent;

    public AnimationState CurrentState { get; private set; } = AnimationState.PLAY;
    public Track CurrentTrack => tracks[(int)CurrentState];

    public GameObject[] frameGameObjects;

    private void Awake()
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].Initialize();
        }

        frameGameObjects = new GameObject[frameGameObjectsParent.childCount];

        for (int i = 0; i < frameGameObjectsParent.childCount; i++)
        {
            Transform child = frameGameObjectsParent.GetChild(i);
            frameGameObjects[i] = child.gameObject;
        }
    }

    private void Start()
    {
        StartCoroutine(TactUpdateCycle());
    }

    private void SetFrame(GameObject frameGameObject)
    {
        for (int i = 0; i < frameGameObjects.Length; i++)
            frameGameObjects[i].SetActive(false);

        frameGameObject.SetActive(true);
    }

    private IEnumerator TactUpdateCycle() 
    {
        while (true) 
        {
            for(var i = 0; i < CurrentTrack.frames.Length; i++)
            {
                SetFrame(CurrentTrack.frames[i].gameObject);

                yield return CurrentTrack.frames[i].Pause;
            }
        }
    }
}
