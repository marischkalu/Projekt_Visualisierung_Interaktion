using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.SceneManagement;
public class CanvasToVideoSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _startCanvas;
    [SerializeField] private GameObject _videoCanvas;
    [SerializeField] private VideoPlayer _videoPlayer;

    [SerializeField] private float _videoStartDelay = 0.5f;

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        _startCanvas.SetActive(true);
        _videoCanvas.SetActive(false);

        _videoPlayer.loopPointReached += OnVideoEnd;
    }

    // Button
    public void ShowVideoCanvas()
    {
        _startCanvas.SetActive(false);
        _videoCanvas.SetActive(true);

        StartCoroutine(PlayVideoWithDelay());
    }

    private IEnumerator PlayVideoWithDelay()
    {
        _videoPlayer.Pause();
        yield return new WaitForSeconds(_videoStartDelay);
        _videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer _videoPlayer)
    {
        SceneManager.LoadScene("Gallery");
    }
}
