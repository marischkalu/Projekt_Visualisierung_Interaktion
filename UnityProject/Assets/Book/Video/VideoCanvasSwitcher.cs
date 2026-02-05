using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoCanvasSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _videoCanvas;
    [SerializeField] private GameObject _nextCanvas;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private float _startDelay = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _videoCanvas.SetActive(true);
        _nextCanvas.SetActive(false);

        _videoPlayer.loopPointReached += OnVideoFinished;

        StartCoroutine(StartVideoWithDelay());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator StartVideoWithDelay()
    {
        _videoPlayer.Pause();
        yield return new WaitForSeconds(_startDelay);

        _videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        _videoCanvas.SetActive(false);
        _nextCanvas.SetActive(true);
    }
}
