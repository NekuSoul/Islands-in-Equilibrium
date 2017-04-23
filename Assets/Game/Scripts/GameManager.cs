using System;
using System.Collections;
using Assets.Game.Code;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Level[] Level;
        public Camera Camera;
        public TileManager TileManager;

        public AnimationCurve BarCurve;
        public AnimationCurve CameraCurve;

        public Image PercentageImage;
        public Image ForestPercentageImage;
        public Text ForestCountText;
        public Text CityCountText;

        public Button PlayButton;
        public Sprite PlaySprite;
        public Sprite PauseSprite;

        public Button WaterButton;
        public Button PlainButton;
        public Button MountainButton;
        public Button DesertButton;
        public Button ForestButton;
        public Button VillageButton;
        public Button CityButton;

        private Tile? _selectedTile;

        public float PlaySpeed = 1;

        private bool _isPlaying;
        private bool _inProgress;
        private float _nextAdvance;

        private int _solvedLevel;
        private Level _currentLevel;
        private GameObject _tileCursor;

        //private Vector3 _cameraFrom;
        //private Vector3 _cameraTo;
        //private float _cameraAnimationTime;

        // Use this for initialization
        public void Start()
        {
            _currentLevel = Level[0];
            StartCoroutine(ChangeCameraPosition(Level[_solvedLevel].transform.position, Level[_solvedLevel].transform.position));
            Reset();
        }

        // Update is called once per frame
        public void Update()
        {
            if (_selectedTile.HasValue)
            {
                Vector2 pos = Camera.ScreenToWorldPoint(Input.mousePosition);
                _tileCursor.transform.position = new Vector3(pos.x, pos.y, 0);

                if (Input.GetMouseButtonDown(0))
                {
                    Collider2D col = Physics2D.OverlapPoint(pos);

                    if (col != null)
                    {
                        TileInfo? info = _currentLevel.GetTileFromObject(col.gameObject);

                        if (info.HasValue)
                        {
                            if (info.Value.Type != _selectedTile.Value)
                            {
                                _currentLevel.ApplyTile(info.Value.X, info.Value.Y, _selectedTile.Value);
                                switch (_selectedTile)
                                {
                                    case Tile.Water:
                                        _currentLevel.CurrentWaterTiles--;
                                        break;
                                    case Tile.Plain:
                                        _currentLevel.CurrentPlainTiles--;
                                        break;
                                    case Tile.Mountain:
                                        _currentLevel.CurrentMountainTiles--;
                                        break;
                                    case Tile.Desert:
                                        _currentLevel.CurrentDesertTiles--;
                                        break;
                                    case Tile.Forest:
                                        _currentLevel.CurrentForestTiles--;
                                        break;
                                    case Tile.Village:
                                        _currentLevel.CurrentVillageTiles--;
                                        break;
                                    case Tile.City:
                                        _currentLevel.CurrentCityTiles--;
                                        break;
                                }
                                UpdateResourceBar();
                                Destroy(_tileCursor);
                                _selectedTile = null;
                                _tileCursor = null;
                            }
                        }
                    }
                }
            }

            if (_isPlaying)
            {
                if (_nextAdvance < 0)
                {
                    bool change = _currentLevel.AdvanceStep();
                    UpdatePercentage();
                    _nextAdvance = PlaySpeed;
                    if (!change)
                    {
                        _isPlaying = false;
                        if (_currentLevel.ForestCount == _currentLevel.CityCount && _solvedLevel < 9)
                        {
                            _solvedLevel++;
                            _currentLevel = Level[_solvedLevel];
                            StartCoroutine(ChangeCameraPosition(Level[_solvedLevel - 1].transform.position, Level[_solvedLevel].transform.position));
                            Reset();
                        }
                    }
                }

                _nextAdvance -= Time.smoothDeltaTime;
            }
        }

        IEnumerator ChangeBarPercentage(float from, float to)
        {
            float time = 0f;

            while (time < PlaySpeed)
            {
                time += Time.deltaTime;

                float percentage = BarCurve.Evaluate(time / PlaySpeed);
                float current = from * (1 - percentage) + to * percentage;
                ForestPercentageImage.rectTransform.sizeDelta = new Vector2(current, 64);
                yield return null;
            }

            ForestPercentageImage.rectTransform.sizeDelta = new Vector2(to, 64);
        }

        IEnumerator ChangeCameraPosition(Vector2 from, Vector2 to)
        {
            float time = 0f;
            float length = 2f;

            while (time < length)
            {
                time += Time.deltaTime;

                float percentage = CameraCurve.Evaluate(time / length);
                Vector3 current = Vector2.Lerp(from, to, percentage);
                Camera.transform.position = new Vector3(current.x, current.y + 1.5f, -10);
                yield return null;
            }

            Camera.transform.position = new Vector3(to.x, to.y + 1.5f, -10);
        }

        public void Play()
        {
            if (!_inProgress)
            {
                if (_currentLevel.CurrentWaterTiles == 0
                    && _currentLevel.CurrentPlainTiles == 0
                    && _currentLevel.CurrentMountainTiles == 0
                    && _currentLevel.CurrentDesertTiles == 0
                    && _currentLevel.CurrentForestTiles == 0
                    && _currentLevel.CurrentVillageTiles == 0
                    && _currentLevel.CurrentCityTiles == 0)
                {
                    _isPlaying = true;
                    _inProgress = true;
                    PlayButton.image.sprite = PauseSprite;
                }
            }
            else
            {
                _isPlaying = !_isPlaying;
                PlayButton.image.sprite = _isPlaying ? PauseSprite : PlaySprite;
                _nextAdvance = 0;
            }
        }

        private void UpdatePercentage()
        {
            float percentage =
                PercentageImage.rectTransform.rect.width /
                ((_currentLevel.ForestCount + _currentLevel.CityCount) / (float)_currentLevel.ForestCount);

            if (_currentLevel.ForestCount == 0 && _currentLevel.CityCount == 0)
                percentage = PercentageImage.rectTransform.rect.width / 2f;

            StartCoroutine(ChangeBarPercentage(ForestPercentageImage.rectTransform.sizeDelta.x, percentage));
            ForestCountText.text = _currentLevel.ForestCount.ToString();
            CityCountText.text = _currentLevel.CityCount.ToString();
        }

        public void Reset()
        {
            _isPlaying = false;
            _inProgress = false;
            _nextAdvance = 0;

            PlayButton.image.sprite = PlaySprite;
            _currentLevel.LoadLevel();
            UpdateResourceBar();
            UpdatePercentage();
        }

        public void UpdateResourceBar()
        {
            WaterButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentWaterTiles.ToString();
            PlainButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentPlainTiles.ToString();
            MountainButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentMountainTiles.ToString();
            DesertButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentDesertTiles.ToString();
            ForestButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentForestTiles.ToString();
            VillageButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentVillageTiles.ToString();
            CityButton.GetComponentInChildren<Text>().text = _currentLevel.CurrentCityTiles.ToString();
        }

        public void PlaceTile(int tile)
        {
            bool allowPlacement = false;

            switch ((Tile)tile)
            {
                case Tile.Water:
                    if (_currentLevel.AvailableWaterTiles > 0)
                        allowPlacement = true;
                    break;
                case Tile.Plain:
                    if (_currentLevel.AvailablePlainTiles > 0)
                        allowPlacement = true;
                    break;
                case Tile.Mountain:
                    if (_currentLevel.AvailableMountainTiles > 0)
                        allowPlacement = true;
                    break;
                case Tile.Desert:
                    if (_currentLevel.AvailableDesertTiles > 0)
                        allowPlacement = true;
                    break;
                case Tile.Forest:
                    if (_currentLevel.AvailableForestTiles > 0)
                        allowPlacement = true;
                    break;
                case Tile.Village:
                    if (_currentLevel.AvailableVillageTiles > 0)
                        allowPlacement = true;
                    break;
                case Tile.City:
                    if (_currentLevel.AvailableCityTiles > 0)
                        allowPlacement = true;
                    break;
                default:
                    throw new Exception("WTF");
            }

            if (allowPlacement)
            {
                _selectedTile = (Tile)tile;

                if (_tileCursor != null)
                    Destroy(_tileCursor);

                GameObject cursor = Instantiate(TileManager.GetByTile((Tile)tile));
                cursor.GetComponent<BoxCollider2D>().enabled = false;
                _tileCursor = cursor;
            }
        }
    }
}
