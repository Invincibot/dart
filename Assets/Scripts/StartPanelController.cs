using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class StartPanelController : MonoBehaviour
{
    private static readonly Color Transparent = new Color(0, 0, 0, 0);

    [SerializeField] private Image image;
    [SerializeField] private Text[] texts;
    private float _lerpDistance;
    private float _freezeTime;
    private float _fadeSpeed;

    public void SetEnemyText(int enemies)
    {
        texts[1].text = "Eliminate " + enemies + " Enemies";
    }

    private void Update()
    {
        if (_freezeTime > 0)
        {
            _freezeTime -= Time.deltaTime;
            return;
        }
        
        _lerpDistance += _fadeSpeed * Time.deltaTime;

        image.color = Color.Lerp(Color.black, Transparent, _lerpDistance);
        Color color = Color.Lerp(Color.white, Transparent, _lerpDistance);
        foreach (Text text in texts)
        {
            text.color = color;
        }
    }

    private void OnEnable()
    {
        image.color = Color.black;
        foreach (Text text in texts)
        {
            text.color = Color.white;
        }

        _lerpDistance = 0;
        _freezeTime = ((NetworkRoomManager) NetworkManager.singleton).freezeTime / 2;
        _fadeSpeed = 1 / _freezeTime;
    }
}