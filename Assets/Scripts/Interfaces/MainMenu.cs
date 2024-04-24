using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;

    [Header("Circuit Animation")]
    [SerializeField] private Texture2D _baseCircuitTexture;
    [SerializeField] private RawImage _circuitRawImg;
    [SerializeField] private float alphaThreshold = 0.1f;
    [SerializeField] private int _idleRenderRate = 100;
    [SerializeField] private int _idleTrailSize = 200;
    [SerializeField] private float _idleWaitTime = 3f;
    [SerializeField] private int _startRenderRate = 2000;
    private Texture2D _circuitTexture;

#nullable enable

    private void Start()
    {
        bool hasSave = !string.IsNullOrEmpty(SaveManager.Instance.ListSave().FirstOrDefault());
        _continueButton.interactable = hasSave;

        _circuitTexture = new Texture2D(_baseCircuitTexture.width, _baseCircuitTexture.height);
        ClearTexture(_circuitTexture);
        _circuitTexture.Apply();
        _circuitRawImg.texture = _circuitTexture;
        _circuitRawImg.enabled = true;
        StartCoroutine(IdleAnimation());
    }

    public void PlayGame()
    {
        SoundManager.Instance.PlayConfirm();
        StopAllCoroutines();
        DisableButtons();
        StartCoroutine(PlayGameAnimation());
    }
    public void LoadGame()
    {
        SoundManager.Instance.PlayConfirm();
        StopAllCoroutines();
        DisableButtons();
        StartCoroutine(LoadGameAnimation());
    }
    public void GoToOption()
    {
        SoundManager.Instance.PlayConfirm();
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        SoundManager.Instance.PlayConfirm();
        Application.Quit();
    }

    IEnumerator IdleAnimation()
    {
        Texture2D original = _baseCircuitTexture;
        Texture2D texture = _circuitTexture;

        while (true)
        {
            yield return new WaitForSecondsRealtime(_idleWaitTime);

            // sample random starting point
            var frontier = new Queue<Vector2Int>();
            for (int i = 0; i < 100; i++)
            {
                int x = Random.Range(0, texture.width);
                int y = Random.Range(0, texture.height);
                var px = original.GetPixel(x, y);
                if (px.a > alphaThreshold)
                {
                    frontier.Enqueue(new Vector2Int(x, y));
                    break;
                }
            }
            if (frontier.Count == 0)
            {
                continue;
            }

            // bfs the frontier, apply the color
            // also remove the painted color, keeping only a certain amount of painted pixels
            int it = 0;
            var painted = new Queue<Vector2Int>();
            while (frontier.TryDequeue(out Vector2Int cur))
            {
                if (it % _idleRenderRate == 0 && it != 0)
                {
                    texture.Apply();
                    yield return new WaitForEndOfFrame();
                }

                if (cur.y < 0 || cur.y >= texture.height || cur.x < 0 || cur.x >= texture.width)
                {
                    continue;
                }

                if (texture.GetPixel(cur.x, cur.y).a != 0)
                {
                    continue;
                }

                if (original.GetPixel(cur.x, cur.y).a < alphaThreshold)
                {
                    continue;
                }

                texture.SetPixel(cur.x, cur.y, original.GetPixel(cur.x, cur.y));
                painted.Enqueue(cur);
                it++;

                if (painted.Count > _idleTrailSize)
                {
                    var p = painted.Dequeue();
                    texture.SetPixel(p.x, p.y, Color.clear);
                }

                frontier.Enqueue(new Vector2Int(cur.x + 1, cur.y));
                frontier.Enqueue(new Vector2Int(cur.x - 1, cur.y));
                frontier.Enqueue(new Vector2Int(cur.x, cur.y + 1));
                frontier.Enqueue(new Vector2Int(cur.x, cur.y - 1));
            }

            while (painted.TryDequeue(out Vector2Int cur))
            {
                if (it % _idleRenderRate == 0 && it != 0)
                {
                    texture.Apply();
                    yield return new WaitForEndOfFrame();
                }
                texture.SetPixel(cur.x, cur.y, Color.clear);
                it++;
            }

            yield return new WaitForEndOfFrame();
            texture.Apply();
        }
    }

    IEnumerator PlayGameAnimation()
    {
        yield return CircuitAnimation(
            fromBorder: true,
            fromRect: _startButton.GetComponent<RectTransform>()
        );
        SaveManager.Instance.NewGame();
    }

    IEnumerator LoadGameAnimation()
    {
        yield return CircuitAnimation(
            fromBorder: true,
            fromRect: _continueButton.GetComponent<RectTransform>()
        );
        SaveManager.Instance.LoadSave();
    }

    IEnumerator CircuitAnimation(bool fromBorder = true, RectTransform? fromRect = null)
    {
        Texture2D original = _baseCircuitTexture;
        Texture2D texture = _circuitTexture;
        ClearTexture(texture);

        // find inital frontier
        var frontier = new Queue<Vector2Int>();

        if (fromBorder)
        {
            for (int i = 0; i < texture.width; i++)
            {
                frontier.Enqueue(new Vector2Int(i, 0));
                frontier.Enqueue(new Vector2Int(i, texture.height - 1));
            }
            for (int j = 0; j < texture.height; j++)
            {
                frontier.Enqueue(new Vector2Int(0, j));
                frontier.Enqueue(new Vector2Int(texture.width - 1, j));
            }
        }

        if (fromRect != null)
        {
            Vector3[] v = new Vector3[4];
            fromRect.GetWorldCorners(v);
            int x0 = texture.width * (int)v[0].x / Screen.width;
            int y0 = texture.height * (int)v[0].y / Screen.height;
            int x1 = texture.width * (int)v[2].x / Screen.width;
            int y1 = texture.height * (int)v[2].y / Screen.height;

            for (int i = x0; i <= x1; i++)
            {
                frontier.Enqueue(new Vector2Int(i, y0));
                frontier.Enqueue(new Vector2Int(i, y1));
            }
            for (int j = y0; j <= y1; j++)
            {
                frontier.Enqueue(new Vector2Int(x0, j));
                frontier.Enqueue(new Vector2Int(x1, j));
            }
        }

        // bfs the frontier, apply the color
        int it = 0;
        while (frontier.TryDequeue(out Vector2Int cur))
        {
            if (it % _startRenderRate == 0 && it != 0)
            {
                texture.Apply();
                yield return new WaitForEndOfFrame();
            }

            if (cur.y < 0 || cur.y >= texture.height || cur.x < 0 || cur.x >= texture.width)
            {
                continue;
            }

            if (texture.GetPixel(cur.x, cur.y).a != 0)
            {
                continue;
            }

            if (original.GetPixel(cur.x, cur.y).a < alphaThreshold)
            {
                continue;
            }

            texture.SetPixel(cur.x, cur.y, original.GetPixel(cur.x, cur.y));
            it++;

            frontier.Enqueue(new Vector2Int(cur.x + 1, cur.y));
            frontier.Enqueue(new Vector2Int(cur.x - 1, cur.y));
            frontier.Enqueue(new Vector2Int(cur.x, cur.y + 1));
            frontier.Enqueue(new Vector2Int(cur.x, cur.y - 1));
        }

        texture.Apply();
    }

    void DisableButtons()
    {
        _startButton.interactable = false;
        _continueButton.interactable = false;
        _startButton.transform.parent.GetComponentsInChildren<Button>().ToList().ForEach(b =>
            b.interactable = false
        );
    }

    void ClearTexture(Texture2D texture)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, Color.clear);
            }
        }
    }
}
